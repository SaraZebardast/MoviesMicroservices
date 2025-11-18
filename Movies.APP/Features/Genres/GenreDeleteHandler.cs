using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Movies.APP.Domain;

namespace Movies.APP.Features.Genres;

public class GenreDeleteRequest : Request, IRequest<CommandResponse>
{
}

public class GenreDeleteHandler : Service<Genre>, IRequestHandler<GenreDeleteRequest, CommandResponse>
{
    public GenreDeleteHandler(DbContext db) : base(db)
    {
    }
    
    protected override IQueryable<Genre> Query(bool isNoTracking = true)
    {
        return base.Query().Include(g => g.MovieGenres);
    }


    public async Task<CommandResponse> Handle(GenreDeleteRequest request, CancellationToken cancellationToken)
    {
        var entity = await Query(false).SingleOrDefaultAsync(g => g.Id == request.Id, cancellationToken);
        if (entity == null)
            return Error("Genre Not found!");
        
        Delete(entity.MovieGenres);
        await Delete(entity, cancellationToken);
        return Success("Genre deleted!", entity.Id);
    }
}