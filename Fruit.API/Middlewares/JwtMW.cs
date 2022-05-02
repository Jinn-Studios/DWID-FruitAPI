using System.Security.Claims;

namespace FruitUI.API.ServiceCore
{
    public class JwtMW
    {
        private readonly RequestDelegate _next;
        private readonly IAuthService _authSvc;

        public JwtMW(RequestDelegate next, IAuthService authSvc)
        {
            _next = next;
            _authSvc = authSvc;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrWhiteSpace(token) == false)
            {
                try { context.Items["User"] = _authSvc.BuildJWT(token).Claims.First(x => x.Type == ClaimTypes.Name).Value; }
                catch { }
            }

            await _next(context);
        }
    }
}