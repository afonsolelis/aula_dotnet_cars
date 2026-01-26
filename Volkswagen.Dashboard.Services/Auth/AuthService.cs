using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Volkswagen.Dashboard.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
                throw new ArgumentException("Usuário ou senha inválidos");

            request.Password = GetMD5Hash(request.Password);

            if (request.Password != user.Password)
                throw new ArgumentException("Usuário ou senha inválidos");
            return GenerateToken(user);
        }

        private LoginResponse GenerateToken(UserModel user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("d8cf9a98-bfb2-4e0a-85b3-7c94f8e908ad");
            var expireAt = DateTime.UtcNow.AddHours(2);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = expireAt,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new LoginResponse()
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
                    throw new ArgumentException("Dados obrigatórios não informados");

                // Verifica se o email esta na whitelist
                if (!await _userRepository.IsEmailInWhitelist(request.Email))
                    throw new ArgumentException("Email não autorizado para registro");

                if (await _userRepository.ExistWithEmail(request.Email))
                    throw new ArgumentException("Usuário já cadastrado na base");

                request.Password = GetMD5Hash(request.Password);

                await _userRepository.InsertUser(request.Email, request.Username, request.Password);

                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        private string GetMD5Hash(string input)
        {
            MD5 md5Hash = MD5.Create();
            // Converter a String para array de bytes, que é como a biblioteca trabalha.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Cria-se um StringBuilder para recompôr a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop para formatar cada byte como uma String em hexadecimal
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

    }
}
