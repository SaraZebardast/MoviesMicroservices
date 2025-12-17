using Users.APP.Domain;
using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Users.APP.Features.Users
{
    public class UserDeleteRequest : Request, IRequest<CommandResponse>
    {
    }

    public class UserDeleteHandler : Service<User>, IRequestHandler<UserDeleteRequest, CommandResponse>
    {
        public UserDeleteHandler(DbContext db) : base(db)
        {
        }

        // we have navigational data so we include it in the query
        protected override IQueryable<User> Query(bool isNoTracking = true)
        {
            return base.Query(isNoTracking).Include(u => u.UserRoles);
        }

        public async Task<CommandResponse> Handle(UserDeleteRequest request, CancellationToken cancellationToken)
        {
            var entity = await Query(false).SingleOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
            if (entity is null)
                return Error("User not found!");
            Delete(entity.UserRoles);
            await Delete(entity, cancellationToken);
            return Success("User deleted successfully.", entity.Id);
        }
    }
}