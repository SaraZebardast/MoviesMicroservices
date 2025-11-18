using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Movies.APP.Domain;

namespace Movies.APP.Features.Genres;

public class GenreQueryRequest : Request, IRequest<IQueryable<GenreQueryResponse>>
{
}

public class GenreQueryResponse : Response
{
    public string Name { get; set; }
    public int MovieCount { get; set; }

    public string Movies { get; set; }
}

public class GenreQueryHandler : Service<Genre>, IRequestHandler<GenreQueryRequest, IQueryable<GenreQueryResponse>> 
{
    public GenreQueryHandler(DbContext db) : base(db)
    {
    }
    
    protected override IQueryable<Genre> Query(bool isNoTracking = true)
    {
        return base.Query(isNoTracking) // will return Roles DbSet
            .Include(g => g.MovieGenres).ThenInclude(mg => mg.Movie) // will first include the relational MovieGenre then Movie data
            .OrderBy(g => g.Name);
    }


    public Task<IQueryable<GenreQueryResponse>> Handle(GenreQueryRequest request, CancellationToken cancellationToken)
    {
        var query = Query().Select(g => new GenreQueryResponse
        {
            Guid = g.Guid,
            Id = g.Id,
            Name = g.Name,
            MovieCount = g.MovieGenres.Count, // returns the Movies count of each Genre
            Movies = string.Join(", ", g.MovieGenres.Select(mg => mg.Movie.Name)) 
        });
        
        return Task.FromResult(query);
    }
}