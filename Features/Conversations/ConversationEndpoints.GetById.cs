using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Sellby.Api.Common;

namespace Sellby.Api.Features.Conversations;

public static partial class ConversationEndpoints
{
    public static void MapGetConversationByIdEndpoint(this WebApplication app)
    {
        app.MapGet("/conversations/{id:guid}", async (Guid id, ClaimsPrincipal user, AppDbContext db) =>
        {
            var uid = user.GetUserId();

            var conversation = await db.Conversations
                .Include(c => c.ParticipantOne)
                .Include(c => c.ParticipantTwo)
                .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1))
                .FirstOrDefaultAsync(c => c.Id == id);

            if (conversation is null)
            {
                return Results.NotFound(new { message = "Conversation not found." });
            }

            if (conversation.ParticipantOneId != uid && conversation.ParticipantTwoId != uid)
            {
                return Results.Forbid();
            }

            var other = conversation.ParticipantOneId == uid ? conversation.ParticipantTwo : conversation.ParticipantOne;
            var lastMessage = conversation.Messages.FirstOrDefault();

            var unreadCount = await db.Messages
                .CountAsync(m => m.ConversationId == id && m.SenderId != uid && !m.IsRead);

            var response = new ConversationSummaryResponse(
                conversation.Id,
                other.Id,
                other.Name,
                other.ProfilePictureUrl,
                lastMessage?.Content,
                lastMessage?.SentAt,
                unreadCount,
                conversation.CreatedAt
            );

            return Results.Ok(response);
        }).RequireAuthorization();
    }
}
