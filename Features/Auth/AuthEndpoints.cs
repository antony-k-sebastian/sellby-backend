namespace Sellby.Api.Features.Auth;

public record RequestOtpDto(string Email);

public static class AuthEndpoints
{
    public static readonly Dictionary<string, (string otp, DateTime ExpiresAt)> otpStore = new ();

    private static readonly string[] AllowedDomains = ["mumail.ie"];

    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/auth/request-otp", (RequestOtpDto dto) =>
        {
            var domain = dto.Email.Split('@').LastOrDefault();

            if (domain is null || !AllowedDomains.Contains(domain))
            {
                return Results.BadRequest(new {message = "Please use your university email"});
            }

            var otp = Random.Shared.Next(100000, 999999).ToString();
            otpStore[dto.Email] = (otp, DateTime.UtcNow.AddMinutes(5));

            Console.WriteLine($"OTP for {dto.Email}: {otp}");

            return Results.Ok(new {message = "Otp is sent."});
        } );
    }
}