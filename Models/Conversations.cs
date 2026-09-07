public class Conversation
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid ParticipantOneId { get; set; }
    public User ParticipantOne { get; set; } = null!;

    public Guid ParticipantTwoId { get; set; }
    public User ParticipantTwo { get; set; } = null!;

    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
