using System.Net;
using System.Text;

namespace FruitUI.API
{
    public class SwaggerMW
    {
        private readonly RequestDelegate _next;
        private readonly IAuthService _authSvc;

        public SwaggerMW(RequestDelegate next, IAuthService authSvc)
        {
            _next = next;
            _authSvc = authSvc;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                if (authHeader != null && authHeader.StartsWith("Basic "))
                {
                    var encodedUsernamePassword = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();
                    if (string.IsNullOrWhiteSpace(encodedUsernamePassword) == false)
                    {
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':', 2)[0];
                        var password = decodedUsernamePassword.Split(':', 2)[1];

                        if (!string.IsNullOrWhiteSpace(await _authSvc.AuthorizeJWT(username, password)))
                        {
                            await _next.Invoke(context);
                            return;
                        }
                    }
                }

                // causes browser to show login dialog
                context.Response.Headers["WWW-Authenticate"] = "Basic";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
}