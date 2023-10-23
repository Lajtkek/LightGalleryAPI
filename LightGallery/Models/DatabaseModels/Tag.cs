using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LightGallery.Models;

public class Tag
{
    [Key]
    public Guid Id { get; set; }
    public Guid GalleryId { get; set; }
    [ForeignKey("GalleryId")]
    public Gallery Gallery { get; set; }
    public ICollection<GalleryFile> Files { get; set; }
    

    public ICollection<Tag> Parents { get; set; }
    
    public ICollection<Tag> Children { get; set; }
    
    [Required]
    [MinLength(3)]
    [MaxLength(128)]
    public string Name { get; set; }
    
    // TODO: allow only one url for tag
    [Required]
    [MinLength(1)]
    [MaxLength(32)]
    public string Code { get; set; }
    
    [Required]
    [MaxLength(2048)]
    public string Description { get; set; }

    [Required] 
    [MaxLength(6)] 
    public string TextColor { get; set; } = "FFFFFF";
    
    [Required] 
    [MaxLength(6)] 
    public string BackgroundColor { get; set; } = "000000";
    
    // isPublic
}