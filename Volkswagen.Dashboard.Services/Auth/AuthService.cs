using Volkswagen.Dashboard.Domain.Common;
using Volkswagen.Dashboard.Domain.Repositories;
using Volkswagen.Dashboard.Domain.Users;
using Volkswagen.Dashboard.Services.Security;

namespace Volkswagen.Dashboard.Services.Auth;

public class AuthService : IAuthService, ILoginService, IRegistrationService
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthorizedEmailRepository _authorizedEmailRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUserRepository userRepository,
        IAuthorizedEmailRepository authorizedEmailRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _authorizedEmailRepository = authorizedEmailRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        try
        {
            var email = new EmailAddress(request.Email);
            var user = await _userRepository.GetByEmailAsync(email);

            if (user is null)
            {
                throw new DomainException("Usuário ou senha inválidos");
            }

            if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                throw new DomainException("Usuário ou senha inválidos");
            }

            return _tokenService.GenerateToken(user);
        }
        catch (DomainException ex)
        {
            throw new ArgumentException(ex.Message);
        }
    }

    public async Task<bool> Register(RegisterRequest request)
    {
        try
        {
            var email = new EmailAddress(request.Email);
            var authorizedEmail = await _authorizedEmailRepository.GetAuthorizedEmailAsync(email);
            if (authorizedEmail is null)
            {
                throw new DomainException("Email não autorizado para registro");
            }

            if (await _userRepository.ExistsByEmailAsync(email))
            {
                throw new DomainException("Usuário já cadastrado na base");
            }

            var passwordHash = _passwordHasher.Hash(request.Password);
            var user = User.Register(
                request.Username,
                email,
                passwordHash,
                whitelistEntryId: authorizedEmail.Id);

            await _userRepository.AddAsync(user);
            return true;
        }
        catch (DomainException)
        {
            return false;
        }
    }
}
