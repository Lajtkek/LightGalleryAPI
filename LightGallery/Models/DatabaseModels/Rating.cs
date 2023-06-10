using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LightGallery.Models;

public class Rating
{
    // TODO: PK from both
    public Guid IdFile { get; set; }
    
    [ForeignKey("IdFile")]
    public GalleryFile File { get; set; }
    
    [ForeignKey("IdUser")]
    public User User { get; set; }
    
    [MaxLength(36)]
    public string IdUser { get; set; }
    
    [Range(-1,1)]
    public int Value { get; set; } = 0;
}