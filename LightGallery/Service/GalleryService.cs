using AutoMapper;
using LightGallery.Models;
using LightGallery.Models.Results;
using Microsoft.EntityFrameworkCore;

namespace LightGallery.Service;

public interface IGalleryService
{
    public Task<Gallery> CreateGallery(Guid idUser, string title);
    public Task<IEnumerable<GalleryGridDto>> GetGalleries();
    public Task<IEnumerable<GalleryGridDto>> GetUserGalleries(Guid idUser);
    public Task<IEnumerable<Gallery>> GalleryDetail(Guid idUser);
}

public class GalleryService : IGalleryService
{
    private readonly IMapper _mapper;
    private readonly DefaultDatabaseContext _context;

    public GalleryService(DefaultDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<Gallery> CreateGallery(Guid idUser, string title)
    {
        var user = await _context.Users.FirstAsync(x => x.Id == idUser);
        var gallery = new Gallery()
        {
            Title = title,
            Owner = user
        };

        _context.Galleries.Add(gallery);
        await _context.SaveChangesAsync();
        
        return gallery;
    }

    public Task<IEnumerable<GalleryGridDto>> GetGalleries()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<GalleryGridDto>> GetUserGalleries(Guid idUser)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Gallery>> GalleryDetail(Guid idUser)
    {
        throw new NotImplementedException();
    }
}