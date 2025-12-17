using CORE.APP.Models.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CORE.APP.Services.Authentication;

    
public class TokenAuthService : ITokenAuthService
{
    public TokenResponse GetTokenResponse(int userId, string userName, string[] userRoleNames, DateTime expiration, 
        string securityKey, string issuer, string audience, string refreshToken)
    {
        // Build a list of claims (user information to store in the token)
        var claims = new List<Claim>()
        {
            new Claim("Id", userId.ToString()), 
            new Claim(ClaimTypes.Name, userName)
        };
        
        // Add each role the user has (like "Admin", "User", etc.)
        foreach (var userRoleName in userRoleNames)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRoleName));
        }

        // Create a signing key from the secret key
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        // Build the actual JWT token
        var jwtSecurityToken = new JwtSecurityToken(issuer, audience, claims, DateTime.Now, expiration, signingCredentials);
        
        // Convert the token object to a string
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        var jwt = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);

        // Return both tokens in a response object
        return new TokenResponse()
        {
            Token = $"{JwtBearerDefaults.AuthenticationScheme} {jwt}", // JwtBearerDefaults.AuthenticationScheme: "Bearer"
            RefreshToken = refreshToken
        };
    }
    
    // Generates a new random refresh token.
    public string GetRefreshToken()
    {
        var bytes = new byte[32]; // Create 32 random bytes
        using (var randomNumberGenerator = RandomNumberGenerator.Create())
        {
            randomNumberGenerator.GetBytes(bytes);
        }
        return Convert.ToBase64String(bytes);
    }
    
    // Reads and validates a JWT token, then extracts the user information (claims) from it.
    public IEnumerable<Claim> GetClaims(string token, string securityKey)
    {

        // Remove the "Bearer " prefix if it exists
        token = token.StartsWith(JwtBearerDefaults.AuthenticationScheme) ? 
            token.Remove(0, JwtBearerDefaults.AuthenticationScheme.Length + 1) : token;

        // Prepare the signing key for validation
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        
        // Set up validation rules
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey
        };

        // Validate the token and extract the claims
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        
        // This validates the signature and decrypts the token
        var principal = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        
        // Return the claims (user ID, name, roles, etc.) or null if validation failed
        return securityToken is null ? null : principal.Claims;
    }
}