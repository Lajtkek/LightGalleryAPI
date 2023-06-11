using System.Security.Claims;
using LightGallery.Models.Enums;

namespace LightGallery.Helpers;

public class ContextHelper
{
    private readonly HttpContext _httpContext;
    private readonly ClaimsIdentity? _claimsIdentity;
    
    public ContextHelper(HttpContext httpContext)
    {
        _httpContext = httpContext;
        _claimsIdentity = _httpContext.User.Identity as ClaimsIdentity;
    }
    
    public Guid GetUserId()
    {
        var userIdString = _claimsIdentity?.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(userIdString)) throw new Exception("Token nemá userID");

        return Guid.Parse(userIdString);
    }
    
    public string[] GetRoles()
    {
        var issuerString = _claimsIdentity?.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(issuerString)) return Array.Empty<string>();

        return issuerString.Split(",");
    }

    public bool HasRole(Role role)
    {
        var roles = GetRoles();
        return roles.Contains(role.ToString()) || roles.Contains("Admin");
    }
}