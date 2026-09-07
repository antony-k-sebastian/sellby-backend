using Microsoft.EntityFrameworkCore;
using Sellby.Api.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Listing> Listings => Set<Listing>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<OtpCode> OtpCodes => Set<OtpCode>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OtpCode>().HasIndex(o => o.Email);

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasOne(c => c.ParticipantOne)
                .WithMany()
                .HasForeignKey(c => c.ParticipantOneId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.ParticipantTwo)
                .WithMany()
                .HasForeignKey(c => c.ParticipantTwoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(c => new { c.ParticipantOneId, c.ParticipantTwoId }).IsUnique();
        });

        modelBuilder.Entity<Message>()
            .HasOne(m => m.TaggedListing)
            .WithMany()
            .HasForeignKey(m => m.TaggedListingId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}