public class Conversation
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid ListingId { get; set; }
    public Listing Listing { get; set; } = null!;

    public Guid BuyerId { get; set; }
    public User Buyer { get; set; } = null!;

    public ICollection<Message> Messages { get; set; } = new List<Message>();
}