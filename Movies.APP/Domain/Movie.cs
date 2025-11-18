using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CORE.APP.Domain;

namespace Movies.APP.Domain;

public class Movie :Entity
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; }
    
    public DateTime? ReleaseDate { get; set; }
    public decimal TotalRevenue { get; set; }
    
    // for director-movies one to many relationship
    public int DirectorId { get; set; } 
    public Director Director { get; set; } // navigation property
    
    
    // for movies-genre many to many relationship
    public List<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
    
    
    //It's a data transfer helper that makes working with many-to-many relationships more developer-friendly
    //without cluttering your database schema.
    [NotMapped] // Not stored in database - it's just a helper
    public List<int> GenreIds // helps to easily manage the MovieGenre relational entities by Genre Id values
    {
        get => MovieGenres.Select(movieGenreEntity => movieGenreEntity.GenreId).ToList();
        set => MovieGenres = value.Select(genreId => new MovieGenre() { GenreId = genreId }).ToList();
    }
}