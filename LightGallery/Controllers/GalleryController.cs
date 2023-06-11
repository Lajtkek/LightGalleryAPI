using LightGallery.Models.Errors;
using LightGallery.Models.Results;
using LightGallery.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LightGallery.Controllers;

[ApiController]
[Route("[controller]")]
public class GalleryController : ControllerBase
{
    private readonly IGalleryService _galleryService;
    private readonly IConfiguration _config;

    public GalleryController(IGalleryService galleryService, IConfiguration config)
    {
        _galleryService = galleryService;
        _config = config;
        
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IEnumerable<GalleryGridDto>> GetGalleries()
    {
        return await _galleryService.GetGalleries();
    }
}