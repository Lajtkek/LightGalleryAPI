namespace LightGallery.Models.Results;

public class TemporaryFileResult
{
    public bool Success { get; set; } = false;
    
    public string Path { get; set; }
    public string Extension { get; set; }
    public string MimeType { get; set; }

     public long FileSize { get; set; }
 }