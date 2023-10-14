using LightGallery.Models.Errors;
using LightGallery.Models.Requests;
using LightGallery.Models.Results;
using LightGallery.Service;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LightGallery.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _config;

    private readonly TimeSpan _refreshTokenLifetimeWindow;
    public AuthController(IAuthService authService, IConfiguration config)
    {
        _authService = authService;
        _config = config;
        
        _refreshTokenLifetimeWindow = TimeSpan.Parse(_config["RefreshToken:LifetimeWindow"]);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Auth(LoginDto loginData)
    {
        var (username, password) = loginData;
        var loginResult = await _authService.LoginByPassword(username, password);

        if (loginResult.Result == LoginResult.NotFound) return NotFound();
        if (loginResult.Result == LoginResult.BadPassword) return Unauthorized();

        var user = loginResult.User;
        var token = await _authService.GenerateToken(user);
        var refreshToken = await _authService.GenerateRefreshToken(user);
        
        Response.Cookies.Delete("RefreshToken");
        Response.Cookies.Append("RefreshToken", refreshToken, _authService.GetHttpsCookieOptions());

        return Ok(token);
    }

    #if !DEBUG
    [RequireHttps]
    #endif
    [HttpPost("RefreshToken")]
    public async Task<IActionResult> RefreshToken()
    {
       var refreshToken = Request.Cookies["RefreshToken"];
       
        if (string.IsNullOrEmpty(refreshToken)) return BadRequest(new ErrorDto()
        {
            Code = "NO_TOKEN_SUPPLIED",
            Message = "Refresh token is missing.",
        });

        var token = await _authService.GetRefreshToken(refreshToken);

        if (token == null) return BadRequest(new ErrorDto()
        {
            Code = "TOKEN_NOT_FOUND",
            Message = "Refresh token was not found."
        });

        if (token.ExpiresAt <= DateTime.UtcNow) return BadRequest(new ErrorDto()
        {
            Code = "TOKEN_EXPIRED",
            Message = "Refresh token is expired"
        });

        if (token.ExpiresAt.Subtract(_refreshTokenLifetimeWindow) <= DateTime.UtcNow)
        {
            token = await _authService.RefreshRefreshToken(token);
            
            Response.Cookies.Delete("RefreshToken");
            Response.Cookies.Append("RefreshToken", token.Token, _authService.GetHttpsCookieOptions());
        }

        return Ok(await _authService.GenerateToken(token.Owner));
    }
    
    [RequireHttps]
    [HttpPost("Logout")]
    public async Task<IActionResult> LogOut()
    {
        var refreshToken = Request.Cookies["RefreshToken"];

        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _authService.InvalidateRefreshToken(refreshToken);
            // TODO: Find if cookie cannot be deleted in better way
            Response.Cookies.Append("RefreshToken", "", _authService.GetHttpsCookieOptions());
        }
        
        return Ok();
    }
}