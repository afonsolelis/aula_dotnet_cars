using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Volkswagen.Dashboard.WebApi.Validators
{
    public static class TokenValidator
    {
        public static void GetPermissionFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            var tokenS = handler.ReadJwtToken(token);

            var claims = tokenS.Claims;

            foreach (Claim claim in claims)
            {
                Console.WriteLine($"Tipo de Reivindicação: {claim.Type}, Valor: {claim.Value}");
            }
        }
    }
}
