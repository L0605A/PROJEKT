using CodeFirst.Models;
using CodeFirst.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeFirst.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserDto loginRequest)
        {
            var token = _authService.Authenticate(loginRequest.Username, loginRequest.Password);

            if (token.Result == null)
            {
                return Unauthorized("Username or password is incorrect");
            }

            return Ok("Bearer " +token.Result);
        }
    }
}