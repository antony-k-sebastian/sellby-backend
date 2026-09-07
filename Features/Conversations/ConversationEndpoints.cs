using Microsoft.EntityFrameworkCore;
using Sellby.Api.Models;

namespace Sellby.Api.Features.Conversations;

public static partial class ConversationEndpoints
{
    private static async Task<Conversation> GetOrCreateConversationAsync(AppDbContext db, Guid userAId, Guid userBId)
    {
        var (participantOneId, participantTwoId) = userAId.CompareTo(userBId) <= 0
            ? (userAId, userBId)
            : (userBId, userAId);

        var conversation = await db.Conversations.FirstOrDefaultAsync(c =>
            c.ParticipantOneId == participantOneId && c.ParticipantTwoId == participantTwoId);

        if (conversation is not null)
        {
            return conversation;
        }

        conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            ParticipantOneId = participantOneId,
            ParticipantTwoId = participantTwoId,
            CreatedAt = DateTime.UtcNow
        };

        db.Conversations.Add(conversation);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            db.Entry(conversation).State = EntityState.Detached;
            conversation = await db.Conversations.FirstAsync(c =>
                c.ParticipantOneId == participantOneId && c.ParticipantTwoId == participantTwoId);
        }

        return conversation;
    }

    private static MessageResponse ToResponse(this Message message) => new(
        message.Id,
        message.ConversationId,
        message.Content,
        message.SentAt,
        message.SenderId,
        message.TaggedListingId,
        message.TaggedListingTitle,
        message.TaggedListingPrice
    );
}
