namespace LightGallery.Models.Requests;

public class TagGridRequest
{
    public bool IncludePrivate { get; set; } = false;
    public bool IncludeChildren { get; set; } = false;
}