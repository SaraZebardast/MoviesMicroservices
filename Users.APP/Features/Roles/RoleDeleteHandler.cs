using Users.APP.Domain;
using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Users.APP.Features.Roles
{
    public class RoleDeleteRequest : Request, IRequest<CommandResponse>
    {
    }

    public class RoleDeleteHandler : Service<Role>, IRequestHandler<RoleDeleteRequest, CommandResponse>
    {
        public RoleDeleteHandler(DbContext db) : base(db)
        {
        }

        protected override IQueryable<Role> Query(bool isNoTracking = true)
        {
            return base.Query(isNoTracking).Include(r => r.UserRoles);
        }

        public async Task<CommandResponse> Handle(RoleDeleteRequest request, CancellationToken cancellationToken)
        {
            var entity = await Query(false).SingleOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
            if (entity is null)
                return Error("Role not found!");
            if (entity.UserRoles.Any())
                return Error("Role is assigned to users and cannot be deleted!");
            await Delete(entity, cancellationToken);
            return Success("Role deleted successfully.", entity.Id);
        }
    }
}