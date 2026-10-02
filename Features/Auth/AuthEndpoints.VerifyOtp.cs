// Updated: Features/Auth/AuthEndpoints.VerifyOtp.cs
using Microsoft.EntityFrameworkCore;
using Sellby.Api.Models;

namespace Sellby.Api.Features.Auth;

public record VerifyOtpDto(string Email, string Otp, string? Name);

public static partial class AuthEndpoints
{
    public static void MapVerifyOtpEndpoint(this WebApplication app)
    {
        app.MapPost("/auth/verify-otp", async (VerifyOtpDto dto, AppDbContext db, IConfiguration config) =>
        {
            var otpHash = AuthLogic.HashOtp(dto.Otp);

            var otpRecord = await db.OtpCodes
                .Where(o => o.Email == dto.Email && !o.IsUsed && o.ExpiredAt > DateTime.UtcNow)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (!AuthLogic.IsOtpValid(otpRecord?.CodeHash, otpHash))
            {
                return Results.BadRequest(new { message = "Invalid or expired OTP." });
            }

            otpRecord!.IsUsed = true;

            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user is null)
            {
                if (AuthLogic.RequiresNameForNewUser(dto.Name))
                {
                    return Results.BadRequest(new { message = "Name is required for new users.", isNewUser = true });
                }

                user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = dto.Email,
                    Name = dto.Name!,
                    CreatedAt = DateTime.UtcNow
                };
                db.Users.Add(user);
            }

            await db.SaveChangesAsync();

            var jwtSection = config.GetSection("Jwt");
            var token = AuthLogic.GenerateJwt(
                user,
                jwtSection["Key"]!,
                jwtSection["Issuer"]!,
                jwtSection["Audience"]!,
                double.Parse(jwtSection["ExpiryMinutes"]!));

            return Results.Ok(new { token, user = new { user.Id, user.Email, user.Name } });
        });
    }
}