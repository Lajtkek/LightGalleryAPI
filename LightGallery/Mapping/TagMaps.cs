using AutoMapper;
using LightGallery.Models;
using LightGallery.Models.Requests;
using LightGallery.Models.Results;
using NpgsqlTypes;

namespace LightGallery.Maps;

public class TagMaps : Profile
{
    public TagMaps()
    {
        CreateMap<TagCreateRequest, Tag>().ForMember(x => x.Children, opt => opt.Ignore()).AfterMap((src, dest) =>
        {
            dest.Description = src.Description ?? "";
        });

        CreateMap<Tag, Guid>().ConstructUsing(x => x.Id);
        CreateMap<Tag, TagGrid>();
    }
}