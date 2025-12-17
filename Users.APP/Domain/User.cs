using CORE.APP.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Users.APP.Domain
{
    public class User : Entity
    {
        [Required, StringLength(10)]
        public string UserName { get; set; }

        [Required, StringLength(15)]
        public string Password { get; set; }

        public bool IsActive { get; set; }

        // Navigation property
        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();

        // Tells Entity Framework NOT to create a database column for this property
        [NotMapped]
        public List<int> RoleIds
        {
            get => UserRoles.Select(ur => ur.RoleId).ToList();
            set => UserRoles = value.Select(id => new UserRole { RoleId = id }).ToList();
        }
    
        public string RefreshToken { get; set; } // When the access token expires, the client sends the refresh token to get a new access token
        public DateTime? RefreshTokenExpiration { get; set; }
    }
}

