public class User
{
    public Guid Id {get; set;}
    public string Email {get; set;} = string.Empty;
    public string Name {get; set;} = string.Empty;
    public string? ProfilePictureUrl {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    public ICollection<Listing> Listing {get; set;} = new List<Listing>(); 
}