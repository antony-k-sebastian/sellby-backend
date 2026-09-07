namespace Sellby.Api.Features.Conversations;

public record ConversationSummaryResponse(
    Guid Id,
    Guid OtherUserId,
    string OtherUserName,
    string? OtherUserProfilePictureUrl,
    string? LastMessagePreview,
    DateTime? LastMessageAt,
    int UnreadCount,
    DateTime CreatedAt
);

public record MessageResponse(
    Guid Id,
    Guid ConversationId,
    string Content,
    DateTime SentAt,
    Guid SenderId,
    Guid? TaggedListingId,
    string? TaggedListingTitle,
    decimal? TaggedListingPrice
);

public record SendMessageDto(string Content);
