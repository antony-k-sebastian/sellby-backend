using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Sellby.Api.Common;
using Sellby.Api.Models;

namespace Sellby.Api.Features.Listings;

public static partial class ListingEndpoints
{
    public static void MapCreateListingEndpoint(this WebApplication app)
    {
        app.MapPost("/listings", async (CreateListingDto dto, ClaimsPrincipal user, AppDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                return Results.BadRequest(new { message = "Title is required." });
            }

            if (dto.Price < 0)
            {
                return Results.BadRequest(new { message = "Price cannot be negative." });
            }

            var categoryExists = await db.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
            {
                return Results.BadRequest(new { message = "Category not found." });
            }

            var listing = new Listing
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                SellerId = user.GetUserId(),
                CreatedAt = DateTime.UtcNow
            };

            db.Listings.Add(listing);
            await db.SaveChangesAsync();

            await db.Entry(listing).Reference(l => l.Seller).LoadAsync();
            await db.Entry(listing).Reference(l => l.Category).LoadAsync();

            return Results.Created($"/listings/{listing.Id}", listing.ToResponse());
        }).RequireAuthorization();
    }
}
