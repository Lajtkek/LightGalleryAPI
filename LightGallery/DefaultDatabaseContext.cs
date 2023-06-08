using LightGallery.Models;
using Microsoft.EntityFrameworkCore;

namespace LightGallery;

public class DefaultDatabaseContext : DbContext
{
    public DefaultDatabaseContext(DbContextOptions<DefaultDatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
    
    public DbSet<User> Users { get; set; }
}