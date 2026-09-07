using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Sellby.Api.Common;

namespace Sellby.Api.Features.Conversations;

public static partial class ConversationEndpoints
{
    public static void MapGetConversationMessagesEndpoint(this WebApplication app)
    {
        app.MapGet("/conversations/{id:guid}/messages", async (
            Guid id,
            ClaimsPrincipal user,
            AppDbContext db,
            int page = 1,
            int pageSize = 50) =>
        {
            var uid = user.GetUserId();

            var conversation = await db.Conversations.FirstOrDefaultAsync(c => c.Id == id);

            if (conversation is null)
            {
                return Results.NotFound(new { message = "Conversation not found." });
            }

            if (conversation.ParticipantOneId != uid && conversation.ParticipantTwoId != uid)
            {
                return Results.Forbid();
            }

            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var messages = await db.Messages
                .Where(m => m.ConversationId == id)
                .OrderBy(m => m.SentAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Results.Ok(messages.Select(m => m.ToResponse()));
        }).RequireAuthorization();
    }
}
