using Microsoft.EntityFrameworkCore;

namespace Movies.APP.Domain;

public class MoviesDb : DbContext
{
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Director> Directors { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<MovieGenre> MovieGenres { get; set; }
    
    public MoviesDb(DbContextOptions options) : base(options)
    {
    }

    // Extra:
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Genre>().HasIndex(genre => genre.Name).IsUnique();
        modelBuilder.Entity<Director>().HasIndex(directorEntity => directorEntity.LastName).IsUnique();
        modelBuilder.Entity<Movie>().HasIndex(movieEntity => movieEntity.Name).IsUnique();

        // Defining indices for optimizing query performance
        // if we frequently query "show all movies by this director"
        modelBuilder.Entity<Movie>().HasIndex(m => m.DirectorId);

        // Composite index on Name and ReleaseDate for optimizing searches involving both fields.
        modelBuilder.Entity<Movie>().HasIndex(movieEntity => new { movieEntity.Name, movieEntity.ReleaseDate });
        
        
        modelBuilder.Entity<MovieGenre>()
            .HasOne(mg => mg.Movie) // each MovieGenre entity has one related Movie entity
            .WithMany(mg => mg.MovieGenres) // each Movie entity has many related MovieGenre entities
            .HasForeignKey(mg => mg.MovieId) // the foreign key property in the Movie Genre entity that references the primary key of the related Movie entity
            .OnDelete(DeleteBehavior.NoAction); // prevents deletion of a Movie entity if there are related MovieRole entities

        modelBuilder.Entity<MovieGenre>()
            .HasOne(mg => mg.Genre) // each MovieGenre entity has one related Genre entity
            .WithMany(mg => mg.MovieGenres) // each Genre entity has many related MovieGenre entities
            .HasForeignKey(mg => mg.GenreId) // the foreign key property in the MovieGenre entity that references the primary key of the related Genre entity
            .OnDelete(DeleteBehavior.NoAction); // prevents deletion of a Genre entity if there are related MovieGenre entities

        modelBuilder.Entity<Movie>()
            .HasOne(m => m.Director) // each Movie entity has one related Director entity
            .WithMany(d => d.Movies) // each Director entity has many related Movie entities
            .HasForeignKey(m => m.DirectorId) // the foreign key property in the Movie entity that references the primary key of the related Director entity
            .OnDelete(DeleteBehavior.NoAction); // prevents deletion of a Director entity if there are related Movie entities
    }
}