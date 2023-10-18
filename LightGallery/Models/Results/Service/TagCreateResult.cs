namespace LightGallery.Models.Results.Service;

public class TagCreateResult
{
    public bool IsSuccess { get; set; }
    public TagGrid? Tag { get; set; }
}