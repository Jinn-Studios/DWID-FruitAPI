using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FruitUI.API
{
    public class AuthService : IAuthService
    {
        public const string SECRET = "Secret, Should be Configurable";

        public JwtSecurityToken BuildJWT(string token)
        {
            new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
            {
                IssuerSigningKey = GetSecurityKey(),
                ValidateIssuer = false,
                ValidateLifetime = false,
                ValidateAudience = false
            }, out SecurityToken validatedToken);

            return (JwtSecurityToken)validatedToken;
        }

        public Task<string> AuthorizeJWT(string userName, string password)
        {
            var user = GetUser(userName, password);
            if (user == null) return Task.FromResult(string.Empty);

            var jwtSecurityToken = new JwtSecurityToken(
                claims: new List<Claim> { new Claim(ClaimTypes.Name, user.UserName) },
                signingCredentials: new SigningCredentials(GetSecurityKey(), SecurityAlgorithms.HmacSha256)
            );

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken));
        }

        private static SymmetricSecurityKey GetSecurityKey()
            => new(Encoding.ASCII.GetBytes(SECRET));

        private record FoundUser(string UserName, string Password);
        private static FoundUser? GetUser(string userName, string password)
        {
            // Emulates finding a user in a DB.
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                return null;

            return new FoundUser(userName, password);
        }
    }
}