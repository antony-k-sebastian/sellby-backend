using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Sellby.Api.Common;

namespace Sellby.Api.Features.Conversations;

public static partial class ConversationEndpoints
{
    public static void MapGetConversationsEndpoint(this WebApplication app)
    {
        app.MapGet("/conversations", async (ClaimsPrincipal user, AppDbContext db) =>
        {
            var uid = user.GetUserId();

            var conversations = await db.Conversations
                .Where(c => c.ParticipantOneId == uid || c.ParticipantTwoId == uid)
                .Include(c => c.ParticipantOne)
                .Include(c => c.ParticipantTwo)
                .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1))
                .ToListAsync();

            var conversationIds = conversations.Select(c => c.Id).ToList();

            var unreadCounts = await db.Messages
                .Where(m => conversationIds.Contains(m.ConversationId) && m.SenderId != uid && !m.IsRead)
                .GroupBy(m => m.ConversationId)
                .Select(g => new { ConversationId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ConversationId, x => x.Count);

            var response = conversations
                .Select(c =>
                {
                    var other = c.ParticipantOneId == uid ? c.ParticipantTwo : c.ParticipantOne;
                    var lastMessage = c.Messages.FirstOrDefault();

                    return new ConversationSummaryResponse(
                        c.Id,
                        other.Id,
                        other.Name,
                        other.ProfilePictureUrl,
                        lastMessage?.Content,
                        lastMessage?.SentAt,
                        unreadCounts.GetValueOrDefault(c.Id, 0),
                        c.CreatedAt
                    );
                })
                .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
                .ToList();

            return Results.Ok(response);
        }).RequireAuthorization();
    }
}
