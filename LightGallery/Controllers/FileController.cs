using LightGallery.Helpers;
using LightGallery.Models;
using LightGallery.Models.Errors;
using LightGallery.Models.Requests;
using LightGallery.Models.Results;
using LightGallery.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LightGallery.Controllers;

[ApiController]
[Route("[controller]")]
public class FileController : ControllerBase
{
    private readonly IGalleryService _galleryService;
    private readonly IConfiguration _config;
    private readonly IFileService _fileService;

    public FileController(IFileService fileService, IGalleryService galleryService, IConfiguration config)
    {
        _fileService = fileService;
        _galleryService = galleryService;
        _config = config;
        
    }
    
    [Authorize]
    [HttpPost("Upload")]
    public async Task<IActionResult> UploadFile([FromForm] FileUploadRequest request)
    {
        var contextHelper = new ContextHelper(HttpContext);
        var idUser = contextHelper.GetUserId();

        var canUpload = await _galleryService.CanUpload(idUser, request.IdGallery);

        if (canUpload == null)
            return NotFound(new ErrorDto()
            {
                Code = "GALLERY_NOT_FOUND",
                Message = "This gallery doesn't exist."
            });

        if (canUpload == false) return Unauthorized();

        var tempFileResult = await _fileService.UploadFileToTemporaryStorage(idUser, request.File);

        if (tempFileResult.Success == false) return StatusCode(500);

        var fileCreateResult = await _galleryService.CreateFile(new CreateFileRequest()
        {
            IdOwner = idUser,
            IdGallery = request.IdGallery,
            FileName = request.FileName,
            Description = request.Description,
            Extension = tempFileResult.Extension,
            MimeType = tempFileResult.MimeType,
            FileSize = tempFileResult.FileSize,
        });
    
        // Remove file if error in database when uploading
        await _fileService.MakeFilePermanent(tempFileResult.Path, fileCreateResult.GalleryFile);

        return Ok(fileCreateResult.GalleryFile.Id);
    }
    
    [AllowAnonymous]
    [HttpGet("{idFile}")]
    public async Task<IActionResult> GetFile(Guid idFile)
    {
        var galleryFile = await _galleryService.GetFile(idFile);
        if (galleryFile == null) return NotFound();

        var filePath = await _fileService.GetFilePath(galleryFile);
        if (System.IO.File.Exists(filePath))
        {
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(fileStream, galleryFile.MimeType, galleryFile.FileName + galleryFile.Extension);
        }

        return NotFound();
    }
}