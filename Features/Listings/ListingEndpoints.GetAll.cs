using Microsoft.EntityFrameworkCore;

namespace Sellby.Api.Features.Listings;

public static partial class ListingEndpoints
{
    public static void MapGetListingsEndpoint(this WebApplication app)
    {
        app.MapGet("/listings", async (
            AppDbContext db,
            Guid? categoryId,
            string? search,
            int page = 1,
            int pageSize = 20) =>
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 50);

            var query = db.Listings
                .Include(l => l.Seller)
                .Include(l => l.Category)
                .AsQueryable();

            if (categoryId is not null)
            {
                query = query.Where(l => l.CategoryId == categoryId);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(l => EF.Functions.ILike(l.Title, $"%{search}%"));
            }

            var listings = await query
                .OrderByDescending(l => l.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Results.Ok(listings.Select(l => l.ToResponse()));
        });
    }
}
