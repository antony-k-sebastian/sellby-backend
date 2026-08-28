using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Sellby.Api.Common;

namespace Sellby.Api.Features.Users;

public static partial class UserEndpoints
{
    public static void MapGetProfileEndpoint(this WebApplication app)
    {
        app.MapGet("/users/me", async (ClaimsPrincipal user, AppDbContext db) =>
        {
            var currentUser = await db.Users.FirstOrDefaultAsync(u => u.Id == user.GetUserId());

            return currentUser is null
                ? Results.NotFound(new { message = "User not found." })
                : Results.Ok(currentUser.ToProfileResponse());
        }).RequireAuthorization();
    }
}
