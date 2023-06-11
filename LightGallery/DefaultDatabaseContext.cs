using LightGallery.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LightGallery;

public class DefaultDatabaseContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DefaultDatabaseContext(DbContextOptions<DefaultDatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Gallery>()
            .HasOne(x => x.Owner)
            .WithMany(x => x.Galleries);
        
        builder.Entity<GalleryFile>()
            .HasOne(x => x.Gallery)
            .WithMany(x => x.Files);

        builder.Entity<GalleryFile>()
            .HasOne(x => x.Owner);
        
        builder.Entity<Tag>()
            .HasOne(x => x.Gallery)
            .WithMany(x => x.Tags);
        
        builder.Entity<Tag>()
            .HasMany(x => x.Files)
            .WithMany(x => x.Tags);
        
        builder.Entity<Tag>()
            .HasOne(x => x.Parent)
            .WithMany(x => x.Children);

        builder.Entity<Rating>()
            .HasKey(x => new { x.IdFile, x.IdUser });
        
    }
    
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<GalleryFile> Files { get; set; }
    public DbSet<Gallery> Galleries { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Rating> Ratings { get; set; }
}