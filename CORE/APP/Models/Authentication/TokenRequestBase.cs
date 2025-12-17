using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CORE.APP.Models.Authentication;

public class TokenRequestBase
{
    [Required]
    public string UserName { get; set; } // The user's login name

    [Required]
    public string Password { get; set; } // The user's password

    // never sent in JSON; ensures that the security key and the rest is not exposed in JSON payloads sent
    [JsonIgnore]
    public string SecurityKey { get; set; } // code to encrypt/decrypt the token

    [JsonIgnore]
    public string Issuer { get; set; } // Who created this token

    [JsonIgnore]
    public string Audience { get; set; } //Who can use this token
}