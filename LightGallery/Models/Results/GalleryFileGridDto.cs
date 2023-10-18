namespace LightGallery.Models.Results;

public class GalleryFileGridDto
{
        public Guid Id { get; set; }
        public UserDisplayDto Owner { get; set; }
        
        public string FileName { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;

        public int Size { get; set; } = 0;

        public string Description { get; set; } = "";
        public int Rating { get; set; } = 0;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    
        public bool IsTemporary { get; set; } = true;
}