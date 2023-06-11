using AutoMapper;
using LightGallery.Models;
using LightGallery.Models.Results;

namespace LightGallery.Maps;

public class GalleryMaps : Profile
{
    public GalleryMaps()
    {
        CreateMap<Gallery, GalleryGridDto>();
    }
}