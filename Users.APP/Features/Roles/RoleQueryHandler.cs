using Users.APP.Domain;
using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Users.APP.Features.Users;

namespace Users.APP.Features.Roles
{
    public class RoleQueryRequest : Request, IRequest<IQueryable<RoleQueryResponse>>
    {
    }

    public class RoleQueryResponse : Response
    {
        public string Name { get; set; }

        public int UserCount { get; set; }

        public List<UserQueryResponse> Users { get; set; }
    }

    public class RoleQueryHandler : Service<Role>, IRequestHandler<RoleQueryRequest, IQueryable<RoleQueryResponse>>
    {
        public RoleQueryHandler(DbContext db) : base(db)
        {
        }

        protected override IQueryable<Role> Query(bool isNoTracking = true)
        {
            return base.Query(isNoTracking).Include(r => r.UserRoles).ThenInclude(ur => ur.User)
                .OrderBy(r => r.Name);
        }

        public Task<IQueryable<RoleQueryResponse>> Handle(RoleQueryRequest request, CancellationToken cancellationToken)
        {
            var query = Query().Select(entity => new RoleQueryResponse
            {
                Id = entity.Id,
                Guid = entity.Guid,
                Name = entity.Name,
                UserCount = entity.UserRoles.Count,
                Users = entity.UserRoles.Select(ur => new UserQueryResponse
                {
                    Id = ur.User.Id,
                    Guid = ur.User.Guid,
                    UserName = ur.User.UserName,
                    IsActive = ur.User.IsActive,
                    IsActiveF = ur.User.IsActive ? "Active" : "Inactive"
                }).ToList()
            });
            return Task.FromResult(query);
        }
    }
}