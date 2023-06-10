namespace LightGallery.Models.Results;

public enum LoginResult
{
    Ok,
    BadPassword,
    NotFound
}

public class LoginResultDto
{
    public LoginResult Result { get; set; }
    public User User { get; set; }
}