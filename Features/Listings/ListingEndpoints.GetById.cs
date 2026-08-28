using Microsoft.EntityFrameworkCore;

namespace Sellby.Api.Features.Listings;

public static partial class ListingEndpoints
{
    public static void MapGetListingByIdEndpoint(this WebApplication app)
    {
        app.MapGet("/listings/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var listing = await db.Listings
                .Include(l => l.Seller)
                .Include(l => l.Category)
                .FirstOrDefaultAsync(l => l.Id == id);

            return listing is null
                ? Results.NotFound(new { message = "Listing not found." })
                : Results.Ok(listing.ToResponse());
        });
    }
}
