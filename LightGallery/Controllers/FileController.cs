using LightGallery.Helpers;
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
}