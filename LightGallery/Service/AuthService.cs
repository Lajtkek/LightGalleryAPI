using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LightGallery.Models;
using LightGallery.Models.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LightGallery.Service;

public interface IAuthService
{
    public Task<LoginResultDto> LoginByPassword(string username, string password);

    public Task<string> GenerateToken(User user);
    public Task<string> GenerateRefreshToken(User user);
    public Task<RefreshToken?> GetRefreshToken(string token);
    public Task<RefreshToken?> RefreshRefreshToken(RefreshToken token);
    public Task<bool> InvalidateRefreshToken(string token);
    public CookieOptions GetHttpsCookieOptions();
}

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;
    private readonly UserManager<User> _userManager;
    private readonly DefaultDatabaseContext _context;
    
    private readonly string _key;
    private readonly TimeSpan _refreshTokenExpire;
    
    public AuthService(IConfiguration config, UserManager<User> userManager, DefaultDatabaseContext context)
    {
        _config = config;
        _key = config["JWT:Secret"];
        _userManager = userManager;
        _context = context;
        
        _refreshTokenExpire = TimeSpan.Parse(_config["RefreshToken:Lifetime"]);
    }

    public async Task<LoginResultDto> LoginByPassword(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return new LoginResultDto()
        {
            Result = LoginResult.NotFound
        };

        var result = await _userManager.CheckPasswordAsync(user, password);
        if (!result) return new LoginResultDto()
        {
            Result = LoginResult.BadPassword
        };
        
        return new LoginResultDto()
        {
            Result = LoginResult.Ok,
            User= user  
        };
    }
    
    public async Task<string> GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenKey = Encoding.ASCII.GetBytes(_key);

        var tokenClaims = new List<Claim>()
        {
            new Claim("UserId", user.Id.ToString()),
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

    public async Task<string> GenerateRefreshToken(User user)
    {
        var token = new RefreshToken()
        {
            ExpiresAt = DateTime.UtcNow.Add(_refreshTokenExpire),
            Owner = user,
        };

        _context.RefreshTokens.Add(token);
        await _context.SaveChangesAsync();

        return token.Token;
    }

    public async Task<RefreshToken?> GetRefreshToken(string token)
    {
        var tokenInDb = await _context.RefreshTokens
            .Include(x => x.Owner)
            .FirstOrDefaultAsync(x => x.Token == token);
        return tokenInDb;
    }
    
    public async Task<RefreshToken?> RefreshRefreshToken(RefreshToken token)
    {
        if (token.ExpiresAt <= DateTime.UtcNow)
        {
            return null;
        }
        
        token.ExpiresAt = DateTime.UtcNow;
        
        var refreshToken = new RefreshToken()
        {
            ExpiresAt = DateTime.UtcNow.Add(_refreshTokenExpire),
            Owner = token.Owner
        };
        
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        return refreshToken;
    }

    public async Task<bool> InvalidateRefreshToken(string tokenString)
    {
        var token = await GetRefreshToken(tokenString);
        if (token == null) return false;
        
        token.ExpiresAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
    
    public CookieOptions GetHttpsCookieOptions()
    {
        #if DEBUG
        return new CookieOptions
        {
            Secure = false,
            HttpOnly = false,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.Add(_refreshTokenExpire)
        };
        #else 
        return new CookieOptions
        {
            Secure = true,
            HttpOnly = true,
            SameSite = SameSiteMode.None, 
            Expires = DateTime.UtcNow.Add(_refreshTokenExpire),
        };
        #endif
    }
}