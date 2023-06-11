using AutoMapper;
using EntityFrameworkPaginateCore;
using LightGallery.Models;
using LightGallery.Models.Results;

namespace LightGallery.Maps;

public class GalleryMaps : Profile
{
    public GalleryMaps()
    {
        CreateMap<Gallery, GalleryGridDto>();

        CreateMap<Page<GalleryFile>, Page<GalleryFileGridDto>>();
        CreateMap<GalleryFile, GalleryFileGridDto>().AfterMap((src, dest) =>
        {
            dest.FileName = src.FileName + src.Extension;
        });
    }
}