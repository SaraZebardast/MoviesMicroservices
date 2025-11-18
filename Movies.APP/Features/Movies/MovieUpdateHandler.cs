using System.ComponentModel.DataAnnotations;
using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Movies.APP.Domain;

namespace Movies.APP.Features.Movies;

public class MovieUpdateRequest : Request, IRequest<CommandResponse>
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; }
    
    public DateTime? ReleaseDate { get; set; }
    public decimal TotalRevenue { get; set; }
    public int DirectorId { get; set; } 
    public List<int> GenreIds { get; set; }= new List<int>();
}

public class MovieUpdateHandler : Service<Movie>, IRequestHandler<MovieUpdateRequest, CommandResponse>
{
    public MovieUpdateHandler(DbContext db) : base(db)
    {
    }
    
    // we override the query from base to add include because EF Core is lazy. It doesn't load related data unless you explicitly ask for it
    protected override IQueryable<Movie> Query(bool isNoTracking = true)
    {
        return base.Query(isNoTracking).Include(m => m.MovieGenres);
    }

    public async Task<CommandResponse> Handle(MovieUpdateRequest request, CancellationToken cancellationToken)
    {
        if (await Query().AnyAsync(m => m.Id != request.Id && m.Name.Trim() == request.Name.Trim(), cancellationToken))
            return Error("Movie with the same name exists!");
        
        var entity = await Query(false).SingleOrDefaultAsync(m => m.Id == request.Id, cancellationToken);
        if (entity is null)
            return Error("Movie not found!");
        
        Delete(entity.MovieGenres);

        entity.Name = request.Name.Trim();
        entity.ReleaseDate = request.ReleaseDate?.Date;
        entity.TotalRevenue = request.TotalRevenue;
        entity.DirectorId = request.DirectorId;
        entity.GenreIds = request.GenreIds;
        
        await Update(entity, cancellationToken);
        return Success("Movie updated successfully.", entity.Id);
    }
}