using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace LightGallery.Models;

public class RefreshToken
{
    [Key]
    public Guid Id { get; set; }

    public string Token { get; set; } = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow;
    
    public User Owner { get; set; } 
}