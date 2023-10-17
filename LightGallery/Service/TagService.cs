using LightGallery.Models.Results;

namespace LightGallery.Service;

public interface ITagService
{
    public Task<TagGrid> CreateTag(Guid idGallery);
}

public class TagService
{
    
}