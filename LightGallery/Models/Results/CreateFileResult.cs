namespace LightGallery.Models.Results;

public class CreateFileResult
{
    public bool Success { get; set; }
    // TODO: enum with error codes 
    
    public GalleryFile GalleryFile { get; set; }
}
