using AutoMapper;
using LightGallery.Models;
using LightGallery.Models.Requests;
using LightGallery.Models.Results;

namespace LightGallery.Maps;

public class TagMaps : Profile
{
    public TagMaps()
    {
        CreateMap<TagCreateRequest, Tag>();
        
        CreateMap<Tag, TagGrid>();
    }
}