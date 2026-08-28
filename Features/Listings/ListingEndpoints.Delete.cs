using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Sellby.Api.Common;

namespace Sellby.Api.Features.Listings;

public static partial class ListingEndpoints
{
    public static void MapDeleteListingEndpoint(this WebApplication app)
    {
        app.MapDelete("/listings/{id:guid}", async (Guid id, ClaimsPrincipal user, AppDbContext db) =>
        {
            var listing = await db.Listings.FirstOrDefaultAsync(l => l.Id == id);

            if (listing is null)
            {
                return Results.NotFound(new { message = "Listing not found." });
            }

            if (listing.SellerId != user.GetUserId())
            {
                return Results.Forbid();
            }

            db.Listings.Remove(listing);
            await db.SaveChangesAsync();

            return Results.NoContent();
        }).RequireAuthorization();
    }
}
