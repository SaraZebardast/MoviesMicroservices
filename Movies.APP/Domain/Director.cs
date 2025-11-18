using System.ComponentModel.DataAnnotations;
using CORE.APP.Domain;

namespace Movies.APP.Domain;

public class Director : Entity
{
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; }
    
    [Required]
    [StringLength(50)]
    public string LastName { get; set; }
    
    public bool IsRetired { get; set; }
    
    // for director-movie one to many relationship
    // a director can have many movies, a movie can have only one director
    public List<Movie> Movies { get; set; } = new List<Movie>();
    
}