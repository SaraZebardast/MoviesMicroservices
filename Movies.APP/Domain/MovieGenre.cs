using CORE.APP.Domain;

namespace Movies.APP.Domain;

public class MovieGenre : Entity
{
    public int MovieId { get; set; } // foreign key that references to the Movies table's Id primary key

    public Movie Movie { get; set; } // navigation property for retrieving related Movie entity data of the MovieGenre entity data in queries

    public int GenreId { get; set; } // foreign key that references to the Genre table's Id primary key

    public Genre Genre { get; set; } // navigation property for retrieving related Genre entity data of the MovieGenre entity data in queries
}