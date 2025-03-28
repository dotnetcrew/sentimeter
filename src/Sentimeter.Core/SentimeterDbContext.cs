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
            builder.Property(c => c.Content).IsRequired();
            builder.HasMany(c => c.Replies)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<VideoSentimentResult>(builder =>
        {
            builder.Property(v => v.Result).IsRequired();
            builder.HasOne(v => v.Video)
                .WithOne()
                .HasForeignKey<VideoSentimentResult>(v => v.VideoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CommentSentimentResult>(builder =>
        {
            builder.Property(c => c.Result).IsRequired();
            builder.HasOne(c => c.Comment)
                .WithOne()
                .HasForeignKey<CommentSentimentResult>(c => c.CommentId)
                .OnDelete(DeleteBehavior.Cascade);
        });


    }
}
