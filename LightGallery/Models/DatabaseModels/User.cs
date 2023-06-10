using Microsoft.AspNetCore.Identity;

namespace LightGallery.Models;

public class User : IdentityUser
{
    public ICollection<Gallery> Galleries { get; set; }
}