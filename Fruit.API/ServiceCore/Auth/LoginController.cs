using FruitUI.API.ServiceCore.Auth;
using Microsoft.AspNetCore.Mvc;

namespace FruitUI.API
{
    public class LoginController : ControllerBase
    {
        private readonly IAuthService _authSvc;

        public LoginController(IAuthService authSvc)
        {
            _authSvc = authSvc;
        }

        [HttpPost("api/login")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginModel.UserName))
                    return BadRequest("Username not specified");

                var token = await _authSvc.AuthorizeJWT(loginModel.UserName, loginModel.Password);
                if (string.IsNullOrWhiteSpace(token) == false)
                    return Ok(token);
            }
            catch
            {
                return BadRequest("An error occurred in generating the token");
            }

            return Unauthorized();
        }

        [HttpGet("api/login/{username}/{password}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetLogin(string userName, string password)
            => await Login(new LoginModel(userName, password));
    }
}