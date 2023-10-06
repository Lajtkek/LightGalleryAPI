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
    private readonly IFTPService _ftpService;

    public FileController(IFileService fileService, IGalleryService galleryService, IConfiguration config, IFTPService ftpService)
    {
        _fileService = fileService;
        _galleryService = galleryService;
        _config = config;
        _ftpService = ftpService;
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

        var fileCreateResult = await _galleryService.UploadFile(new UploadFileRequest()
        {
            IdOwner = idUser,
            IdGallery = request.IdGallery,
            Description = request.Description,
            File = request.File,
            Tags = request.Tags
        });

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
    
    [AllowAnonymous]
    [HttpPost("TestUpload")]
    public async Task<IActionResult> UplaodFile(IFormFile galleryFile)
    {
        var stream = galleryFile.OpenReadStream();
        await _ftpService.UploadFile(new GalleryFile(), stream);
        return Ok();
    }
    
    [AllowAnonymous]
    [HttpGet("Test")]
    public async Task<IActionResult> FileIDKL()
    {
        using (var a = await _ftpService.GetFile(""))
        return File(a, "image/jpg");
    }
}