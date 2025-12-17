using Users.APP.Domain;
using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Users.APP.Features.Users
{
    public class UserCreateRequest : Request, IRequest<CommandResponse>
    {
        [Required, StringLength(10)]
        [DisplayName("User Name")]
        public string UserName { get; set; }

        [Required, StringLength(15)]
        public string Password { get; set; }

        [DisplayName("Active")]
        public bool IsActive { get; set; }

        [Required]
        [DisplayName("Roles")]
        public List<int> RoleIds { get; set; }
    }

    public class UserCreateHandler : Service<User>, IRequestHandler<UserCreateRequest, CommandResponse>
    {
        public UserCreateHandler(DbContext db) : base(db)
        {
        }

        public async Task<CommandResponse> Handle(UserCreateRequest request, CancellationToken cancellationToken)
        {
            if (await Query().AnyAsync(u => u.UserName == request.UserName, cancellationToken))
                return Error("User with the same user name exists!");
            var entity = new User
            {
                UserName = request.UserName,
                Password = request.Password,
                IsActive = request.IsActive,
                RoleIds = request.RoleIds
            };
            await Create(entity, cancellationToken);
            return Success("User created successfully.", entity.Id);
        }
    }
}