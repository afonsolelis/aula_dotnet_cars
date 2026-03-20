using Microsoft.AspNetCore.Mvc;
using Volkswagen.Dashboard.Services.Auth;

namespace Volkswagen.Dashboard.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IRegistrationService _registrationService;
    private readonly ILoginService _loginService;

    public UserController(IRegistrationService registrationService, ILoginService loginService)
    {
        _registrationService = registrationService;
        _loginService = loginService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _registrationService.Register(request);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _loginService.Login(request);
        return Ok(result);
    }
}
