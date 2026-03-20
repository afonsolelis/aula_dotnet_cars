using Moq;
using Volkswagen.Dashboard.Domain.Repositories;
using Volkswagen.Dashboard.Domain.Users;
using Volkswagen.Dashboard.Services.Auth;
using Volkswagen.Dashboard.Services.Security;

namespace Volkswagen.Dashboard.Tests.Auth;

[TestFixture]
public class AuthServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private Mock<IAuthorizedEmailRepository> _authorizedEmailRepositoryMock = null!;
    private Mock<IPasswordHasher> _passwordHasherMock = null!;
    private Mock<ITokenService> _tokenServiceMock = null!;
    private Volkswagen.Dashboard.Services.Auth.AuthService _service = null!;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _authorizedEmailRepositoryMock = new Mock<IAuthorizedEmailRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _tokenServiceMock = new Mock<ITokenService>();

        _service = new Volkswagen.Dashboard.Services.Auth.AuthService(
            _userRepositoryMock.Object,
            _authorizedEmailRepositoryMock.Object,
            _passwordHasherMock.Object,
            _tokenServiceMock.Object);
    }

    [Test]
    public async Task Login_ShouldUsePasswordHasherVerify()
    {
        var request = new LoginRequest
        {
            Email = "driver@vw.com",
            Password = "plain-password"
        };
        var email = new EmailAddress(request.Email);
        var user = User.Register("driver", email, "stored-hash");
        var expectedResponse = new LoginResponse();

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<EmailAddress>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.Verify(request.Password, user.PasswordHash))
            .Returns(true);
        _tokenServiceMock.Setup(x => x.GenerateToken(user))
            .Returns(expectedResponse);

        var result = await _service.Login(request);

        Assert.That(result, Is.SameAs(expectedResponse));
        _passwordHasherMock.Verify(x => x.Verify(request.Password, user.PasswordHash), Times.Once);
        _passwordHasherMock.Verify(x => x.Hash(It.IsAny<string>()), Times.Never);
    }
}
