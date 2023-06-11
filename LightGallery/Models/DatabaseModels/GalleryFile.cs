using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace LightGallery.Models;

public class GalleryFile
{
    [Key]
    public Guid Id { get; set; }
    public Gallery Gallery { get; set; }
    public User Owner { get; set; }
    public ICollection<Tag> Tags { get; set; }

    [Required]
    [MinLength(3)]
    [MaxLength(256)]
    public string FileName { get; set; } = string.Empty;
    
    [Required]
    [MinLength(1)]
    [MaxLength(12)]
    public string Extension { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(32)]
    public string MimeType { get; set; } = string.Empty;

    [Required]
    [MinLength(0)]
    public int Size { get; set; } = 0;

    [Required] 
    [MaxLength(8192)] 
    public string Description { get; set; } = "";

    public int Rating { get; set; } = 0;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public bool IsTemporary { get; set; } = true;
    
    public int FolderIndex { get; set; } = 0;
}