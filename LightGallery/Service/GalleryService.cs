using AutoMapper;
using EntityFrameworkPaginateCore;
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
    public Task<CreateFileResult> UploadFile(UploadFileRequest request);
    public Task<Page<GalleryFileGridDto>> GetFilesPage(Guid idGallery, int pageNumber = 1, int pageSize = 5);
    public Task<GalleryFile?> GetFile(Guid idFile);
}

public class GalleryService : IGalleryService
{
    private readonly IMapper _mapper;
    private readonly DefaultDatabaseContext _context;
    private readonly IFTPService _ftpService;
    
    private readonly int _maxFilesInFolder;
    public GalleryService(DefaultDatabaseContext context, IMapper mapper, IConfiguration config, IFTPService ftpService)
    {
        _context = context;
        _mapper = mapper;
        _ftpService = ftpService;

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

    public async Task<CreateFileResult> UploadFile(UploadFileRequest request)
    {
        var gallery = await _context.Galleries.Include(x => x.Owner).FirstOrDefaultAsync(x => x.Id == request.IdGallery);
        if (gallery == null) return new CreateFileResult{ Success = false};
        
        var owner = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.IdOwner);
        if (owner == null) return new CreateFileResult{ Success = false};
        
        var tags = await _context.Tags.Where(x => request.Tags.Contains(x.Id)).ToListAsync();

        var file = request.File;

        var extension = Path.GetExtension(file.FileName);
        var fileName = file.FileName;
        var mimeType = file.ContentType;
        long fileSize = file.Length;

        gallery.LastFileIndex++;
        if (gallery.LastFileIndex > _maxFilesInFolder)
        {
            gallery.LastFileIndex = 0;
            gallery.LastFolderIndex++;
        }
        
        var galleryFile = new GalleryFile()
        {
            FileName = fileName,
            Owner = owner,
            Gallery = gallery,
            Description = request.Description,
            Size = fileSize,
            FolderIndex = gallery.LastFolderIndex,
            MimeType = mimeType,
            Extension = extension,
            Tags = tags
        };

        gallery.TotalGallerySize += fileSize / 1024; // to Kb

        _context.Files.Add(galleryFile);
        
        await _context.SaveChangesAsync();

        using (var stream = file.OpenReadStream())
            await _ftpService.UploadFile(galleryFile, stream);

        galleryFile.IsTemporary = false;

        await _context.SaveChangesAsync();

        return new CreateFileResult{ Success = true, GalleryFile = galleryFile };
    }

    public async Task<Page<GalleryFileGridDto>> GetFilesPage(Guid idGallery, int pageNumber = 1, int pageSize = 5)
    {
        var sorts = new Sorts<GalleryFile>();

        var page = await _context.Files.Include(x => x.Owner).PaginateAsync(pageNumber, pageSize, sorts);

        return _mapper.Map<Page<GalleryFileGridDto>>(page);
    }

    public async Task<GalleryFile?> GetFile(Guid idFile)
    {
        return await _context.Files.FirstOrDefaultAsync(x => x.Id == idFile);
    }
}