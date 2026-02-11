using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Volkswagen.Dashboard.Repository;

namespace Volkswagen.Dashboard.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var user = await _userRepository.GetUserByEmail(request.Email);
            if (user == null)
            {
                throw new ArgumentException("Usuário ou senha inválidos");
            }

            request.Password = GetMD5Hash(request.Password);

            if (request.Password != user.Password)
            {
                throw new ArgumentException("Usuário ou senha inválidos");
            }

            return GenerateToken(user);
        }

        private LoginResponse GenerateToken(UserModel user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("d8cf9a98-bfb2-4e0a-85b3-7c94f8e908ad");
            var expireAt = DateTime.UtcNow.AddHours(2);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = expireAt,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new LoginResponse
            {
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expireAt,
                AccessToken = tokenHandler.WriteToken(token)
            };
        }

        public async Task<bool> Register(RegisterRequest request)
        {
            try
            {
                var isFaildedRequest = string.IsNullOrEmpty(request.Email) ||
                                       string.IsNullOrEmpty(request.Password) ||
                                       string.IsNullOrEmpty(request.Username);

                if (isFaildedRequest)
                {
                    throw new ArgumentException("Dados obrigatórios não informados");
                }

                if (!await _userRepository.IsEmailInWhitelist(request.Email))
                {
                    throw new ArgumentException("Email não autorizado para registro");
                }

                if (await _userRepository.ExistWithEmail(request.Email))
                {
                    throw new ArgumentException("Usuário já cadastrado na base");
                }

                request.Password = GetMD5Hash(request.Password);

                await _userRepository.InsertUser(request.Email, request.Username, request.Password);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string GetMD5Hash(string input)
        {
            using var md5Hash = MD5.Create();
            var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            var builder = new StringBuilder();

            foreach (var value in data)
            {
                builder.Append(value.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}
