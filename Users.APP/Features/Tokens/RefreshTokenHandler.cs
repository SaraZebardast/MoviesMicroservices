using CORE.APP.Models.Authentication;
using CORE.APP.Services;
using CORE.APP.Services.Authentication;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Users.APP.Domain;

namespace Users.APP.Features.Tokens
{
    public class RefreshTokenRequest : RefreshTokenRequestBase, IRequest<TokenResponse>
    {
    }

    public class RefreshTokenHandler : Service<User>, IRequestHandler<RefreshTokenRequest, TokenResponse>
    {
        private readonly ITokenAuthService _tokenAuthService; 
        
        public RefreshTokenHandler(DbContext db, ITokenAuthService tokenAuthService) : base(db)
        {
            _tokenAuthService = tokenAuthService;
        }
        
        protected override IQueryable<User> Query(bool isNoTracking = true)
        {
            return base.Query(isNoTracking).Include(u => u.UserRoles).ThenInclude(ur => ur.Role);
        }
        
        public async Task<TokenResponse> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            
            // Extract user info from old access token
            var claims = _tokenAuthService.GetClaims(request.Token, request.SecurityKey);

            // reads user ID from old token
            var userId = Convert.ToInt32(claims.SingleOrDefault(claim => claim.Type == "Id").Value);

            
            // Validate refresh token
            var user = await Query(false).SingleOrDefaultAsync(user => user.Id == userId && user.RefreshToken == request.RefreshToken 
                && user.RefreshTokenExpiration >= DateTime.Now, cancellationToken);

            if (user is null)
                return null;
            
            // if ok creates new access token and creates new refresh token
            user.RefreshToken = _tokenAuthService.GetRefreshToken();
            await Update(user, cancellationToken);

            var expiration = DateTime.Now.AddMinutes(5);
            
            // Sends new tokens back
            return _tokenAuthService.GetTokenResponse(user.Id, user.UserName, user.UserRoles.Select(ur => ur.Role.Name).ToArray(), 
                expiration, request.SecurityKey, request.Issuer, request.Audience, user.RefreshToken);
        }
    }
}