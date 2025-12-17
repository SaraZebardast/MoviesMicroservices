using Users.APP.Domain;
using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Users.APP.Features.Roles
{
    public class RoleUpdateRequest : Request, IRequest<CommandResponse>
    {
        [Required, StringLength(50)]
        public string Name { get; set; }
    }

    public class RoleUpdateHandler : Service<Role>, IRequestHandler<RoleUpdateRequest, CommandResponse>
    {
        public RoleUpdateHandler(DbContext db) : base(db)
        {
        }

        public async Task<CommandResponse> Handle(RoleUpdateRequest request, CancellationToken cancellationToken)
        {
            if (await Query().AnyAsync(r => r.Id != request.Id && r.Name == request.Name.Trim(), cancellationToken))
                return Error("Role with the same name exists!");
            var entity = await Query(false).SingleOrDefaultAsync(r => r.Id == request.Id);
            if (entity is null)
                return Error("Role not found!");
            entity.Name = request.Name.Trim();
            await Update(entity, cancellationToken);
            return Success("Role updated successfully.", entity.Id);
        }
    }
}