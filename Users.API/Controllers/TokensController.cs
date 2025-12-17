using MediatR;
using Microsoft.AspNetCore.Mvc;
using Users.APP.Features.Tokens;

namespace Users.API.Controllers
{

    [Route("api/[controller]")] // Sets the base route for this controller to "api/tokens" (controller name is replaced at runtime).
    [ApiController]
    public class TokensController : ControllerBase
    {
        private readonly IMediator _mediator; 
        private readonly IConfiguration _configuration;

        public TokensController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }

        [HttpPost] 
        [Route("~/api/[action]")] // Overrides the controller's base route. The route becomes "api/Token" (action name is replaced at runtime).
        public async Task<IActionResult> Token(TokenRequest request)
        {
            request.SecurityKey = _configuration["SecurityKey"]; // program.cs
            request.Audience = _configuration["Audience"]; // appsettings.json
            request.Issuer = _configuration["Issuer"]; // appsettings.json
            if (ModelState.IsValid)
            {
                var response = await _mediator.Send(request);
                if (response is not null)
                    return Ok(response);
                return NotFound(_configuration["TokenMessage:NotFound"]);
            }
            return BadRequest(_configuration["TokenMessage:BadRequest"]); 
        }
        
        [HttpPost] 
        [Route("~/api/[action]")] 
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            request.SecurityKey = _configuration["SecurityKey"]; // get the SecurityKey section value from previously added section in Program.cs
            request.Audience = _configuration["Audience"]; // get the Audience section value from appsettings.json
            request.Issuer = _configuration["Issuer"]; // get the Issuer section value from appsettings.json
            if (ModelState.IsValid)
            {
                var response = await _mediator.Send(request);
                if (response is not null)
                    return Ok(response);
                return NotFound(_configuration["TokenMessage:NotFound"]); // return the NotFound section value of the TokenMessage section
                                                                          // from appsettings.json as a HTTP 404 NotFound response
            }
            return BadRequest(_configuration["TokenMessage:BadRequest"]); // return the BadRequest section value of the TokenMessage section
                                                                          // from appsettings.json as a HTTP 400 BadRequest response
        }
    }
}