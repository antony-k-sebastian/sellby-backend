using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Sellby.Api.Models;

namespace Sellby.Api.Features.Auth;

public record RequestOtpDto(string Email);

public static partial class AuthEndpoints
{
    private static readonly string[] AllowedDomains = ["mumail.ie"];
    private static readonly TimeSpan ResendCooldown = TimeSpan.FromSeconds(60);

    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/auth/request-otp", async (RequestOtpDto dto, AppDbContext db) =>
        {
            var domain = dto.Email.Split('@').LastOrDefault();

            if (domain is null || !AllowedDomains.Contains(domain))
            {
                return Results.BadRequest(new { message = "Please use your university email" });
            }

            // Cooldown check: find the most recent OTP request for this email
            var lastOtp = await db.OtpCodes
                .Where(o => o.Email == dto.Email)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (lastOtp is not null)
            {
                var timeSinceLastRequest = DateTime.UtcNow - lastOtp.CreatedAt;
                if (timeSinceLastRequest < ResendCooldown)
                {
                    var secondsRemaining = (int)(ResendCooldown - timeSinceLastRequest).TotalSeconds;
                    return Results.Json(
                        new { message = $"Please wait {secondsRemaining}s before requesting another OTP." },
                        statusCode: StatusCodes.Status429TooManyRequests
                    );
                }
            }

            var otp = Random.Shared.Next(100000, 999999).ToString();
            var otpHash = HashOtp(otp);

            var oldOtps = await db.OtpCodes
                .Where(o => o.Email == dto.Email && !o.IsUsed)
                .ToListAsync();

            foreach (var old in oldOtps)
            {
                old.IsUsed = true;
            }

            db.OtpCodes.Add(new OtpCode
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                CodeHash = otpHash,
                ExpiredAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            });

            await db.SaveChangesAsync();

            Console.WriteLine($"OTP for {dto.Email}: {otp}");

            return Results.Ok(new { message = "Otp is sent." });
        });
    }

    private static string HashOtp(string otp)
    {
        var bytes = Encoding.UTF8.GetBytes(otp);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
}