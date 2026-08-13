using System.Runtime.InteropServices.JavaScript;

namespace Sellby.Api.Models;

public class OtpCode
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string CodeHash { get; set; } = string.Empty;
    public DateTime ExpiredAt { get; set; }
    public bool IsUsed { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}