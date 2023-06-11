namespace LightGallery.Models.Requests;

public class CreateFileRequest
{
    public Guid IdGallery { get; set; }
    public Guid IdOwner { get; set; }
    public Guid[] Tags { get; set; } = Array.Empty<Guid>();
    public string Description { get; set; }
    public string FileName { get; set; }
    public string Extension { get; set; }
    public string MimeType { get; set; }
    public long FileSize { get; set; }
}