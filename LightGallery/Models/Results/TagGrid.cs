namespace LightGallery.Models.Results;

public class TagGrid
{
    public Guid Id { get; set; }
    
    public ICollection<Guid> Children { get; set; }
    
    public string Name { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public string TextColor { get; set; } = "FFFFFF";
    public string BackgroundColor { get; set; } = "000000";
}