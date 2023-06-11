namespace LightGallery.Models.Results;

public class GalleryGridDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public UserDisplayDto Owner { get; set; }
}