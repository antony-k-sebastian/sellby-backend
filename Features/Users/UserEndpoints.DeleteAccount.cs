using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Sellby.Api.Common;

namespace Sellby.Api.Features.Users;

public static partial class UserEndpoints
{
    public static void MapDeleteAccountEndpoint(this WebApplication app)
    {
        app.MapDelete("/users/me", async (ClaimsPrincipal user, AppDbContext db) =>
        {
            var currentUser = await db.Users.FirstOrDefaultAsync(u => u.Id == user.GetUserId());

            if (currentUser is null)
            {
                return Results.NotFound(new { message = "User not found." });
            }

            db.Users.Remove(currentUser);
            await db.SaveChangesAsync();

            return Results.NoContent();
        }).RequireAuthorization();
    }
}
