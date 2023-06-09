using LightGallery.Service;
using Microsoft.AspNetCore.Mvc;

namespace LightGallery.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost(Name = "Auth")]
    public async Task<IActionResult> Auth(string username, string password)
    {
        return Ok(await _authService.LoginByPassword(username, password));
    }
}