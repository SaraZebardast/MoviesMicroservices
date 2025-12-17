namespace CORE.APP.Models.Authentication;

public class TokenResponse
{
    public string Token { get; set; } // The generated JWT
    
    public string RefreshToken { get; set; } // used to request a new JWT without requiring re-authentication
}