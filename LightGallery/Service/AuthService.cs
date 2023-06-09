using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LightGallery.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace LightGallery.Service;

public interface IAuthService
{
    public Task<string?> LoginByPassword(string username, string password);

    public Task<string> GenerateToken(User user);
}

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;
    private readonly string _key;
    private readonly UserManager<User> _userManager;

    public AuthService(IConfiguration config, UserManager<User> userManager)
    {
        _config = config;
        _key = config["JWT:Secret"];
        _userManager = userManager;
    }

    public async Task<string?> LoginByPassword(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return null;

        var result = await _userManager.CheckPasswordAsync(user, password);
        if (!result) return null;
        
        return await GenerateToken(user);
    }
    
    public async Task<string> GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenKey = Encoding.ASCII.GetBytes(_key);

        var tokenClaims = new List<Claim>()
        {
            new Claim("UserId", user.Id),
            new Claim("UserName", user.UserName),
        };
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(tokenClaims),
            Audience = _config["JWT:Issuer"],
            Issuer = _config["JWT:Issuer"],
            Expires = DateTime.UtcNow.Add(TimeSpan.Parse(_config["JWT:Lifetime"])),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256
            )
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}