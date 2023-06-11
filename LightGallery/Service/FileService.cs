using AutoMapper;
using LightGallery.Models;
using LightGallery.Models.Results;
using Microsoft.EntityFrameworkCore;

namespace LightGallery.Service;

public interface IFileService
{
    public Task<TemporaryFileResult> UploadFileToTemporaryStorage(Guid idUser, IFormFile file);
    public Task<bool> MakeFilePermanent(string oldPath, GalleryFile galleryFile);
    public Task<string> GetFileFolder(GalleryFile galleryFile);
    public Task<string> GetFilePath(GalleryFile galleryFile);
}

public class FileService : IFileService
{
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly string _rootPath;

    public FileService(IWebHostEnvironment context, IMapper mapper)
    {
        _hostingEnvironment = context;
        _mapper = mapper;
        _rootPath = _hostingEnvironment.ContentRootPath;
    }
    
    public async Task<TemporaryFileResult> UploadFileToTemporaryStorage(Guid idUser, IFormFile file)
    {
        var directoryPath = Path.Combine(_rootPath, "Files", idUser.ToString(), "TempUpload");
        
        var extension = Path.GetExtension(file.FileName);
        var filePath = Path.Combine(directoryPath, DateTime.UtcNow.Ticks + extension);

        Directory.CreateDirectory(directoryPath);

        await using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        var fileInfo = new FileInfo(filePath);
        
        return new TemporaryFileResult()
        {
            Success = true,
            Extension = extension,
            Path = filePath,
            FileSize = fileInfo.Length / 1024,
            MimeType = file.ContentType
        };
    }

    public async Task<bool> MakeFilePermanent(string oldPath, GalleryFile galleryFile)
    {
        var newPath = await GetFilePath(galleryFile);
        Directory.CreateDirectory(await GetFileFolder(galleryFile));
        File.Move(oldPath, newPath);
        return true;
    }

    public Task<string> GetFileFolder(GalleryFile galleryFile)
    {
        var folderIndex = $"{galleryFile.FolderIndex:0000}";
        return Task.FromResult(Path.Combine(_rootPath, "Files", galleryFile.IdOwner.ToString(), galleryFile.IdGallery.ToString(), $"Chunk_{folderIndex}"));
    }
    
    public async Task<string> GetFilePath(GalleryFile galleryFile)
    {
        return Path.Combine(await GetFileFolder(galleryFile), galleryFile.Id + galleryFile.Extension);
    }
}