using AutoMapper;
using LightGallery.Models;
using LightGallery.Models.Results;

namespace LightGallery.Maps;

public class UserMaps : Profile
{
    public UserMaps()
    {
        CreateMap<User, UserDisplayDto>();
    }
}