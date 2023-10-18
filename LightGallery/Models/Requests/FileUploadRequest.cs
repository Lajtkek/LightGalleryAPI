using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace LightGallery.Models.Requests;

public class FileUploadRequest
{    
    [Required]
    public Guid IdGallery { get; set; }
    
    [Required]
    [MinLength(3)]
    [MaxLength(256)]
    public string FileName { get; set; }
    
    [Required]
    public Guid[] Tags { get; set; } = Array.Empty<Guid>();
        
    [Required]
    public IFormFile File { get; set; }

    [MaxLength(8192)]
    public string? Description { get; set; } = "";
    
    public bool IsTemporary { get; set; } = true;
}