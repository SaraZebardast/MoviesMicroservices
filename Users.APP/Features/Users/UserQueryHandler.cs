using Users.APP.Domain;
using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Users.APP.Features.Users
{
    public class UserQueryRequest : Request, IRequest<IQueryable<UserQueryResponse>>
    {
    }

    public class UserQueryResponse : Response
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public bool IsActive { get; set; }

        public string IsActiveF { get; set; }

        public List<int> RoleIds { get; set; }

        public string Roles { get; set; }
    }

    public class UserQueryHandler : Service<User>, IRequestHandler<UserQueryRequest, IQueryable<UserQueryResponse>>
    {
        public UserQueryHandler(DbContext db) : base(db)
        {
        }

        protected override IQueryable<User> Query(bool isNoTracking = true)
        {
            return base.Query(isNoTracking).Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .OrderByDescending(u => u.IsActive).ThenBy(u => u.UserName);
        }

        public Task<IQueryable<UserQueryResponse>> Handle(UserQueryRequest request, CancellationToken cancellationToken)
        {
            var query = Query().Select(u => new UserQueryResponse
            {
                Id = u.Id,
                Guid = u.Guid,
                UserName = u.UserName,
                Password = u.Password,
                IsActive = u.IsActive,
                IsActiveF = u.IsActive ? "Active" : "Inactive",
                RoleIds = u.RoleIds,
                Roles = string.Join("<br>", u.UserRoles.Select(ur => ur.Role.Name).ToList())
            });
            return Task.FromResult(query);
        }
    }
}