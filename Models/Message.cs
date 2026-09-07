public class Message
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;

    public Guid ConversationId { get; set; }
    public Conversation Conversation { get; set; } = null!;

    public Guid SenderId { get; set; }
    public User Sender { get; set; } = null!;

    public Guid? TaggedListingId { get; set; }
    public Listing? TaggedListing { get; set; }
    public string? TaggedListingTitle { get; set; }
    public decimal? TaggedListingPrice { get; set; }
}
