using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Sellby.Api.Common;
using Sellby.Api.Models;

namespace Sellby.Api.Features.Conversations;

public static partial class ConversationEndpoints
{
    public static void MapStartListingConversationEndpoint(this WebApplication app)
    {
        app.MapPost("/listings/{id:guid}/messages", async (
            Guid id,
            SendMessageDto dto,
            ClaimsPrincipal user,
            AppDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                return Results.BadRequest(new { message = "Content is required." });
            }

            var listing = await db.Listings.FirstOrDefaultAsync(l => l.Id == id);

            if (listing is null)
            {
                return Results.NotFound(new { message = "Listing not found." });
            }

            var uid = user.GetUserId();

            if (listing.SellerId == uid)
            {
                return Results.BadRequest(new { message = "You cannot message yourself about your own listing." });
            }

            var conversation = await GetOrCreateConversationAsync(db, uid, listing.SellerId);

            var message = new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = conversation.Id,
                SenderId = uid,
                Content = dto.Content,
                SentAt = DateTime.UtcNow,
                TaggedListingId = listing.Id,
                TaggedListingTitle = listing.Title,
                TaggedListingPrice = listing.Price
            };

            db.Messages.Add(message);
            await db.SaveChangesAsync();

            return Results.Created($"/conversations/{conversation.Id}/messages", message.ToResponse());
        }).RequireAuthorization();
    }
}
