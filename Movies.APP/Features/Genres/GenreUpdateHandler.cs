using System.ComponentModel.DataAnnotations;
using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Movies.APP.Domain;

namespace Movies.APP.Features.Genres;

public class GenreUpdateRequest : Request, IRequest<CommandResponse>
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; }
}

public class GenreUpdateHandler : Service<Genre>, IRequestHandler<GenreUpdateRequest, CommandResponse>
{
    public GenreUpdateHandler(DbContext db) : base(db)
    {
    }

    public async Task<CommandResponse> Handle(GenreUpdateRequest request, CancellationToken cancellationToken)
    {
        if (await Query().AnyAsync(g => g.Id != request.Id && g.Name == request.Name.Trim(), cancellationToken))
            return Error("Genre with the same name exists");
        var entity = await Query(false).SingleOrDefaultAsync(g => g.Id == request.Id);
        if (entity is null)
            return Error("Genre not found!");
        
        entity.Name = request.Name?.Trim();
        await Update(entity, cancellationToken);
        return Success("Genre updated!", entity.Id);
    }
}