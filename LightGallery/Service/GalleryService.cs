using AutoMapper;
using LightGallery.Models;
using LightGallery.Models.Requests;
using LightGallery.Models.Results;
using Microsoft.EntityFrameworkCore;

namespace LightGallery.Service;

public interface IGalleryService
{
    public Task<Gallery> CreateGallery(Guid idUser, string title);
    public Task<IEnumerable<GalleryGridDto>> GetGalleries();
    public Task<IEnumerable<GalleryGridDto>> GetUserGalleries(Guid idUser);
    public Task<IEnumerable<Gallery>> GalleryDetail(Guid idUser);
    public Task<bool?> CanUpload(Guid idUser, Guid idGallery);
    public Task<CreateFileResult> CreateFile(CreateFileRequest request);
}

public class GalleryService : IGalleryService
{
    private readonly IMapper _mapper;
    private readonly DefaultDatabaseContext _context;

    private readonly int _maxFilesInFolder;
    public GalleryService(DefaultDatabaseContext context, IMapper mapper, IConfiguration config)
    {
        _context = context;
        _mapper = mapper;

        if (!int.TryParse(config["Gallery:FilesPerFolder"], out _maxFilesInFolder))
        {
            _maxFilesInFolder = 100;
        }
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

    public async Task<IEnumerable<GalleryGridDto>> GetGalleries()
    {
        var galleries = await _context.Galleries.Include(x => x.Owner).ToListAsync();

        return _mapper.Map<List<GalleryGridDto>>(galleries);
    }

    public Task<IEnumerable<GalleryGridDto>> GetUserGalleries(Guid idUser)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Gallery>> GalleryDetail(Guid idUser)
    {
        throw new NotImplementedException();
    }

    public async Task<bool?> CanUpload(Guid idUser, Guid idGallery)
    {
        var gallery = await _context.Galleries.Include(x =>x.Owner).FirstOrDefaultAsync(x => x.Id == idGallery);

        if (gallery == null) return null;

        return gallery.Owner.Id == idUser;
    }

    public async Task<CreateFileResult> CreateFile(CreateFileRequest request)
    {
        var gallery = await _context.Galleries.Include(x => x.Owner).FirstOrDefaultAsync(x => x.Id == request.IdGallery);
        if (gallery == null) return new CreateFileResult{ Success = false};
        
        var owner = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.IdOwner);
        if (owner == null) return new CreateFileResult{ Success = false};
        
        // TODO: move to tagService
        // TODO: throw exception if tags are not valid or something
        var tags = _context.Tags.Where(x => request.Tags.Contains(x.Id));

        gallery.LastFileIndex++;
        if (gallery.LastFileIndex > _maxFilesInFolder)
        {
            gallery.LastFileIndex = 0;
            gallery.LastFolderIndex++;
        }
        
        var galleryFile = new GalleryFile()
        {
            FileName = request.FileName,
            Owner = owner,
            Gallery = gallery,
            Description = request.Description,
            Size = (int) request.FileSize,
            FolderIndex = gallery.LastFolderIndex,
            MimeType = request.MimeType,
            Extension = request.Extension
        };

        gallery.TotalGallerySize += request.FileSize;

        _context.Files.Add(galleryFile);
        
        await _context.SaveChangesAsync();

        return new CreateFileResult()
        {
            Success = true,
            GalleryFile = galleryFile
        };
    }
}