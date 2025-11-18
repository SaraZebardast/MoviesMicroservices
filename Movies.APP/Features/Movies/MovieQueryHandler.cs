using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Movies.APP.Domain;

namespace Movies.APP.Features.Movies;

public class MovieQueryRequest : Request, IRequest<IQueryable<MovieQueryResponse>>
{
    public string Name { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public decimal? TotalRevenue { get; set; }
    public int? DirectorId { get; set; } 
    public List<int> GenreIds { get; set; } = new List<int>(); 
}

public class MovieQueryResponse : Response 
{
    // copy all the non navigation properties
    public string Name { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public decimal TotalRevenue { get; set; } 
    public int DirectorId { get; set; } 
    public List<int> GenreIds { get; set; }

    //Formatted 
    public string ReleaseDateF { get; set; }
    public string TotalRevenueF { get; set; }
    public string Director { get; set; }
    public List<string> Genres { get; set; }
}

public class MovieQueryHandler : Service<Movie>, IRequestHandler<MovieQueryRequest, IQueryable<MovieQueryResponse>>
{
    public MovieQueryHandler(DbContext db) : base(db)
    {
    }
    
    // we overrise the Query from the base class here
    // returns a query that fetches movies with their directors and genres, sorted newest-first, then by revenue, then by name.
    protected override IQueryable<Movie> Query(bool isNoTracking = true)
    {
        return base.Query(isNoTracking) // will return Movies DbSet
            .Include(m => m.Director) 
            .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre) 
            .OrderByDescending(m => m.ReleaseDate)
            .ThenBy(m => m.Name); 
    }

    public Task<IQueryable<MovieQueryResponse>> Handle(MovieQueryRequest request, CancellationToken cancellationToken)
    {
        var entityQuery = Query(); // query of all movies
       
        // Some Filtering options but hocam said Filtering is not necessary
        
        // If the request has a Name, only show movies with that name
        if (!string.IsNullOrWhiteSpace(request.Name))
            entityQuery = entityQuery.Where(m => m.Name == request.Name);
        
        // If the request has a DirectorId, only show movies by that director
        if (request.DirectorId.HasValue)
            entityQuery = entityQuery.Where(m => m.DirectorId == request.DirectorId.Value);
        
        // Filter by ReleaseDate
        if (request.ReleaseDate.HasValue)
            entityQuery = entityQuery.Where(m => m.ReleaseDate == request.ReleaseDate.Value.Date);

        // Filter by TotalRevenue
        if (request.TotalRevenue.HasValue)
            entityQuery = entityQuery.Where(m => m.TotalRevenue >= request.TotalRevenue.Value);

        // Filter by GenreIds (movies that have ANY of the requested genres)
        if (request.GenreIds != null && request.GenreIds.Any())
            entityQuery = entityQuery.Where(m => m.MovieGenres.Any(mg => request.GenreIds.Contains(mg.GenreId)));
        
        var query = entityQuery.Select(m => new MovieQueryResponse
        {
            Id = m.Id,
            Guid = m.Guid,
            Name = m.Name,
            ReleaseDate = m.ReleaseDate,
            TotalRevenue = m.TotalRevenue,
            DirectorId = m.DirectorId,
            GenreIds =  m.GenreIds,
        
            // Formatted stuff
            ReleaseDateF = m.ReleaseDate.HasValue ? m.ReleaseDate.Value.ToString("MM/dd/yyyy") : string.Empty,
            TotalRevenueF = m.TotalRevenue.ToString("C"), // C is currency format
            Director = m.Director != null ? m.Director.FirstName + " " + m.Director.LastName : null, // full director name
            Genres = m.MovieGenres.Select(mg => mg.Genre.Name).ToList() // this is extracting just the genre names from the many-to-many relationship and putting them in a simple list of strings, instead of returning complex MovieGenre objects
        });
        
        return Task.FromResult(query);
    }
}