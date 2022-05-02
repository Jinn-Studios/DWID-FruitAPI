using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FruitUI.API.ServiceCore.Auth
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class JinnAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.Items["User"]?.ToString();
            if (string.IsNullOrWhiteSpace(user))
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}