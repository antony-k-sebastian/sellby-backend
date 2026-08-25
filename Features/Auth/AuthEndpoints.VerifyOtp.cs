using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Sellby.Api.Features.Auth;

public record VerifyOtpDto(string Email, string Otp, string? Name);

public static partial class AuthEndpoints
{
    public static void MapVerifyOtpEndpoint(this WebApplication app)
    {
        app.MapPost("/auth/verify-otp", async (VerifyOtpDto dto, AppDbContext db, IConfiguration config) =>
        {
            var otpHash = HashOtp(dto.Otp);

            var otpRecord = await db.OtpCodes
                .Where(o => o.Email == dto.Email && !o.IsUsed && o.ExpiredAt > DateTime.UtcNow)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otpRecord is null || otpRecord.CodeHash != otpHash)
            {
                return Results.BadRequest(new { message = "Invalid or expired OTP." });
            }

            otpRecord.IsUsed = true;

            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user is null)
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    return Results.BadRequest(new { message = "Name is required for new users.", isNewUser = true });
                }

                user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = dto.Email,
                    Name = dto.Name,
                    CreatedAt = DateTime.UtcNow
                };
                db.Users.Add(user);
            }

            await db.SaveChangesAsync();

            var token = GenerateJwt(user, config);

            return Results.Ok(new { token, user = new { user.Id, user.Email, user.Name } });
        });
    }

    private static string GenerateJwt(User user, IConfiguration config)
    {
        var jwtSection = config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("name", user.Name)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSection["ExpiryMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}