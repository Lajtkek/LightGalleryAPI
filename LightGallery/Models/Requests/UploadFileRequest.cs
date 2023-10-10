namespace LightGallery.Models.Requests;

public class UploadFileRequest
{
    public Guid IdGallery { get; set; }
    public Guid IdOwner { get; set; }
    public Guid[] Tags { get; set; } = Array.Empty<Guid>();
    public IFormFile File { get; set; }
    public string Description { get; set; } = "";
}