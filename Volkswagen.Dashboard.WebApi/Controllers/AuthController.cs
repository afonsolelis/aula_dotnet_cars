using Microsoft.AspNetCore.Mvc;
using Volkswagen.Dashboard.Repository;
using Volkswagen.Dashboard.Services;

namespace Volkswagen.Dashboard.WebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost]
        public async Task<IActionResult> GetToken()
        {
            var user = new UserModel { Username = "Ronaldo", Role = "ADMIN"};
            return Ok(await _tokenService.GenerateToken(user)) ;
        }
    }
}
