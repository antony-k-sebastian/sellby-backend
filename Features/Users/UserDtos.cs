using Sellby.Api.Models;

namespace Sellby.Api.Features.Users;

public record UserProfileResponse(
    Guid Id,
    string Email,
    string Name,
    string? ProfilePictureUrl,
    DateTime CreatedAt
);

public static class UserMappingExtensions
{
    public static UserProfileResponse ToProfileResponse(this User user) => new(
        user.Id,
        user.Email,
        user.Name,
        user.ProfilePictureUrl,
        user.CreatedAt
    );
}
