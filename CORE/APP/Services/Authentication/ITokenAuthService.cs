using System.Security.Claims;
using CORE.APP.Models.Authentication;

namespace CORE.APP.Services.Authentication;


public interface ITokenAuthService
{
    // Creates a complete token package when someone logs in successfully
    public TokenResponse GetTokenResponse(int userId, string userName, string[] userRoleNames, DateTime expiration, string securityKey, string issuer, string audience, string refreshToken);

    public string GetRefreshToken(); // Generates a random refresh token

    public IEnumerable<Claim> GetClaims(string token, string securityKey); //Extracts information from an existing JWT token
}