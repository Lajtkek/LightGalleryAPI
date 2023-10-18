namespace LightGallery.Models.Requests;

public class TagCreateRequest
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public IEnumerable<Guid> Children { get; set; }
}