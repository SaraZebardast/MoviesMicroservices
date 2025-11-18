using System.ComponentModel.DataAnnotations;
using CORE.APP.Domain;

namespace Movies.APP.Domain;

public class Genre : Entity
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; }
    
    // for movies-genres many to many relationship
    public List<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
}