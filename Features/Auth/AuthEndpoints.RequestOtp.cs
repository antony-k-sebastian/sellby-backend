// Features/Auth/AuthEndpoints.RequestOtp.cs
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
            if (!AuthLogic.IsAllowedDomain(dto.Email, AllowedDomains))
            {
                return Results.BadRequest(new { message = "Please use your university email" });
            }

            var lastOtp = await db.OtpCodes
                .Where(o => o.Email == dto.Email)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            var secondsRemaining = AuthLogic.GetCooldownSecondsRemaining(
                lastOtp?.CreatedAt, DateTime.UtcNow, ResendCooldown);

            if (secondsRemaining is not null)
            {
                return Results.Json(
                    new { message = $"Please wait {secondsRemaining}s before requesting another OTP." },
                    statusCode: StatusCodes.Status429TooManyRequests
                );
            }

            var (otp, otpHash) = AuthLogic.GenerateOtp();

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
}