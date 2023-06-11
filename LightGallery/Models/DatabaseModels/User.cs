using Microsoft.AspNetCore.Identity;

namespace LightGallery.Models;

public class User : IdentityUser<Guid>
{
    public ICollection<Gallery> Galleries { get; set; }
}