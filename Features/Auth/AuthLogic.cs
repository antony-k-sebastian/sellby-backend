// Features/Auth/AuthLogic.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Sellby.Api.Models;

namespace Sellby.Api.Features.Auth;

public static class AuthLogic
{
    public static bool IsAllowedDomain(string email, string[] allowedDomains)
    {
        var domain = email.Split('@').LastOrDefault();
        return domain is not null && allowedDomains.Contains(domain);
    }

    public static int? GetCooldownSecondsRemaining(DateTime? lastOtpCreatedAt, DateTime now, TimeSpan cooldown)
    {
        if (lastOtpCreatedAt is null) return null;

        var elapsed = now - lastOtpCreatedAt.Value;
        if (elapsed >= cooldown) return null;

        return (int)(cooldown - elapsed).TotalSeconds;
    }

    public static (string Otp, string Hash) GenerateOtp()
    {
        var otp = Random.Shared.Next(100000, 999999).ToString();
        return (otp, HashOtp(otp));
    }

    public static string HashOtp(string otp)
    {
        var bytes = Encoding.UTF8.GetBytes(otp);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }

    public static bool IsOtpValid(string? storedHash, string providedHash)
    {
        return storedHash is not null && storedHash == providedHash;
    }

    public static bool RequiresNameForNewUser(string? name)
    {
        return string.IsNullOrWhiteSpace(name);
    }

    public static Claim[] BuildClaims(User user)
    {
        return new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("name", user.Name)
        };
    }

    public static string GenerateJwt(
        User user, string signingKey, string issuer, string audience, double expiryMinutes)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: BuildClaims(user),
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}