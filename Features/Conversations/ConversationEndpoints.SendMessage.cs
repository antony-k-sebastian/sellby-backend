using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Sellby.Api.Common;
using Sellby.Api.Models;

namespace Sellby.Api.Features.Conversations;

public static partial class ConversationEndpoints
{
    public static void MapSendConversationMessageEndpoint(this WebApplication app)
    {
        app.MapPost("/conversations/{id:guid}/messages", async (
            Guid id,
            SendMessageDto dto,
            ClaimsPrincipal user,
            AppDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                return Results.BadRequest(new { message = "Content is required." });
            }

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

            var message = new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = conversation.Id,
                SenderId = uid,
                Content = dto.Content,
                SentAt = DateTime.UtcNow
            };

            db.Messages.Add(message);
            await db.SaveChangesAsync();

            return Results.Created($"/conversations/{conversation.Id}/messages", message.ToResponse());
        }).RequireAuthorization();
    }
}
