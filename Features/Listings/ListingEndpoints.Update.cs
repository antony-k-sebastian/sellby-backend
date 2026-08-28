using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Sellby.Api.Common;

namespace Sellby.Api.Features.Listings;

public static partial class ListingEndpoints
{
    public static void MapUpdateListingEndpoint(this WebApplication app)
    {
        app.MapPut("/listings/{id:guid}", async (Guid id, UpdateListingDto dto, ClaimsPrincipal user, AppDbContext db) =>
        {
            var listing = await db.Listings
                .Include(l => l.Seller)
                .Include(l => l.Category)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (listing is null)
            {
                return Results.NotFound(new { message = "Listing not found." });
            }

            if (listing.SellerId != user.GetUserId())
            {
                return Results.Forbid();
            }

            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                return Results.BadRequest(new { message = "Title is required." });
            }

            if (dto.Price < 0)
            {
                return Results.BadRequest(new { message = "Price cannot be negative." });
            }

            if (dto.CategoryId != listing.CategoryId)
            {
                var categoryExists = await db.Categories.AnyAsync(c => c.Id == dto.CategoryId);
                if (!categoryExists)
                {
                    return Results.BadRequest(new { message = "Category not found." });
                }
            }

            listing.Title = dto.Title;
            listing.Description = dto.Description;
            listing.Price = dto.Price;
            listing.CategoryId = dto.CategoryId;

            await db.SaveChangesAsync();
            await db.Entry(listing).Reference(l => l.Category).LoadAsync();

            return Results.Ok(listing.ToResponse());
        }).RequireAuthorization();
    }
}
