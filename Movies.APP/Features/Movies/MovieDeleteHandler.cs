using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Movies.APP.Domain;

namespace Movies.APP.Features.Movies;

public class MovieDeleteRequest : Request, IRequest<CommandResponse>
{
}

public class MovieDeleteHandler : Service<Movie>, IRequestHandler<MovieDeleteRequest, CommandResponse>
{
    public MovieDeleteHandler(DbContext db) : base(db)
    {
    }
    
    // we override teh query from base because EF Core is lazy. It doesn't load related data unless you explicitly ask for it 
    protected override IQueryable<Movie> Query(bool isNoTracking = true)
    {
        return base.Query(isNoTracking).Include(m => m.MovieGenres);
    }

    public async Task<CommandResponse> Handle(MovieDeleteRequest request, CancellationToken cancellationToken)
    {
        var entity = await Query(false).SingleOrDefaultAsync(m => m.Id == request.Id, cancellationToken);
        if (entity is null)
            return Error("Movie not found!");
        
        Delete(entity.MovieGenres); // will remove the relational entities data from the MovieGenres DbSet
        await Delete(entity,cancellationToken);
        return Success("Movie deleted successfully.", entity.Id);
    }
}