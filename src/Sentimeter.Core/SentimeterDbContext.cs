using Microsoft.EntityFrameworkCore;
using Sentimeter.Core.Models;

namespace Sentimeter.Core;

public class SentimeterDbContext : DbContext
{
    public SentimeterDbContext(DbContextOptions<SentimeterDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Video> Videos { get; set; } = default!;

    public DbSet<Comment> Comments { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Video>(builder =>
        {
            builder.Property(v => v.Identifier).HasMaxLength(50).IsRequired();
            builder.Property(v => v.Title).HasMaxLength(255).IsRequired();
            builder.Property(v => v.Description).HasMaxLength(1024);
            builder.Property(v => v.ThumbnailUrl).HasMaxLength(255);

            builder.HasMany(v => v.Comments)
                .WithOne(c => c.Video)
                .HasForeignKey(c => c.VideoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Comment>(builder =>
        {
            builder.Property(c => c.Author).HasMaxLength(255).IsRequired();
            builder.Property(c => c.Content).HasMaxLength(1024).IsRequired();
            builder.HasMany(c => c.Replies)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
