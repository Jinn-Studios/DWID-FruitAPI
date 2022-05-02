using System.IdentityModel.Tokens.Jwt;

namespace FruitUI.API
{
    public interface IAuthService
    {
        Task<string> AuthorizeJWT(string userName, string password);
        JwtSecurityToken BuildJWT(string token);
    }
}