using LightGallery.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LightGallery;

public class DefaultDatabaseContext : IdentityDbContext<User>
{
    public DefaultDatabaseContext(DbContextOptions<DefaultDatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
    
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}