using CORE.APP.Domain;
using System.ComponentModel.DataAnnotations;

namespace Users.APP.Domain
{
    public class Role : Entity
    {
        [Required, StringLength(50)]
        public string Name { get; set; }

        // navigation property for the relationship between Role and User
        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}