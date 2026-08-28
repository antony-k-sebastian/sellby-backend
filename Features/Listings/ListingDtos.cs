using Sellby.Api.Models;

namespace Sellby.Api.Features.Listings;

public record ListingResponse(
    Guid Id,
    string Title,
    string Description,
    decimal Price,
    DateTime CreatedAt,
    Guid SellerId,
    string SellerName,
    Guid CategoryId,
    string CategoryName
);

public record CreateListingDto(string Title, string Description, decimal Price, Guid CategoryId);

public record UpdateListingDto(string Title, string Description, decimal Price, Guid CategoryId);

public static class ListingMappingExtensions
{
    public static ListingResponse ToResponse(this Listing listing) => new(
        listing.Id,
        listing.Title,
        listing.Description,
        listing.Price,
        listing.CreatedAt,
        listing.SellerId,
        listing.Seller.Name,
        listing.CategoryId,
        listing.Category.Name
    );
}
