using CORE.APP.Models.Authentication;
using CORE.APP.Services;
using CORE.APP.Services.Authentication;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Users.APP.Domain;

namespace Users.APP.Features.Tokens
{

    // Username and password comes from base class
    public class TokenRequest : TokenRequestBase, IRequest<TokenResponse> // return a token response
    {
    }

    // we need to use service of user to do user query
    public class TokenHandler : Service<User>, IRequestHandler<TokenRequest, TokenResponse>
    {
        
        private readonly ITokenAuthService _tokenAuthService;
            
        public TokenHandler(DbContext db, ITokenAuthService tokenAuthService) : base(db)
        {
            _tokenAuthService = tokenAuthService;
        }
        
        protected override IQueryable<User> Query(bool isNoTracking = true)
        {
            return base.Query(isNoTracking).Include(u => u.UserRoles).ThenInclude(ur => ur.Role);
        }

        // get token request 
        public async Task<TokenResponse> Handle(TokenRequest request, CancellationToken cancellationToken)
        {
            
            var user = await Query(false).SingleOrDefaultAsync(u => u.UserName == request.UserName && u.Password == request.Password 
                && u.IsActive, cancellationToken);

            if (user is null) // no user no token
                return null;

            user.RefreshToken = _tokenAuthService.GetRefreshToken(); // generate a refresh token a.k.a a random string instead of asking for username and password over and over again , Used to re-issue access tokens later
            user.RefreshTokenExpiration = DateTime.Now.AddDays(7); // give some expiration, usually kept long
            await Update(user, cancellationToken); // save the refresh token

            var expiration = DateTime.Now.AddMinutes(5);
            
            // create the JWT response and sends it back to the client
            return _tokenAuthService.GetTokenResponse(user.Id, user.UserName, user.UserRoles.Select(ur => ur.Role.Name).ToArray(), // user.UserRoles.Select(ur => ur.Role.Name) is an ienumerable so we need to turn it into an array
                expiration, request.SecurityKey, request.Issuer, request.Audience, user.RefreshToken);
        }
    }
}