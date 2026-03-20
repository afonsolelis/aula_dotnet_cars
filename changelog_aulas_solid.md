# Changelog SOLID

## Objetivo

Esta refatoração reorganiza partes do projeto para aplicar os princípios `S`, `O`, `L`, `I` e `D` de forma separada e identificável. O foco aqui não é DDD; por isso o conteúdo anterior foi removido e substituído por um changelog específico de SOLID.

## S: Single Responsibility Principle

**Refatoração**

- O acesso a usuários e o acesso à whitelist de e-mails deixaram de ficar concentrados na mesma responsabilidade.

**Onde foi refatorado**

- [UserRepository](./Volkswagen.Dashboard.Repository/UserRepository.cs)
- [AuthorizedEmailRepository](./Volkswagen.Dashboard.Repository/AuthorizedEmailRepository.cs)
- [Program.cs](./Volkswagen.Dashboard.WebApi/Program.cs)

**Antes**

- A mesma camada de persistência acabava responsável tanto por usuário quanto por whitelist.

```csharp
public class UserRepository
{
    public Task<User?> GetByEmailAsync(EmailAddress email) { ... }
    public Task AddAsync(User user) { ... }
    public Task<AuthorizedEmail?> GetAuthorizedEmailAsync(EmailAddress email) { ... }
}
```

**Depois**

- Cada repositório passou a expor apenas o comportamento do seu agregado.

```csharp
public sealed class UserRepository : IUserRepository
{
    public Task<User?> GetByEmailAsync(EmailAddress email) { ... }
    public Task AddAsync(User user) { ... }
}
```

```csharp
public sealed class AuthorizedEmailRepository : IAuthorizedEmailRepository
{
    public Task<AuthorizedEmail?> GetAuthorizedEmailAsync(EmailAddress email) { ... }
}
```

```csharp
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthorizedEmailRepository, AuthorizedEmailRepository>();
```

**Como ficou**

- `UserRepository` agora cuida apenas de persistência de `User`.
- `AuthorizedEmailRepository` agora cuida apenas da consulta da whitelist.

**Por que isso melhora**

- Cada classe passou a ter um único motivo para mudar.
- A manutenção da whitelist não impacta mais a classe responsável por usuários.

## O: Open/Closed Principle

**Refatoração**

- A conversão de `Car` para `CarDto` deixou de ficar presa a um método fixo dentro do fluxo do serviço e passou a ser feita por uma abstração de mapeamento.

**Onde foi refatorado**

- [ICarDtoMapper](./Volkswagen.Dashboard.Services/Cars/ICarDtoMapper.cs)
- [CarDtoMapper](./Volkswagen.Dashboard.Services/Cars/CarDtoMapper.cs)
- [CarsService](./Volkswagen.Dashboard.Services/Cars/CarsService.cs)
- [CarDto](./Volkswagen.Dashboard.Services/Cars/CarDto.cs)

**Antes**

- O serviço ficava acoplado diretamente à forma de montar o DTO.

```csharp
public async Task<IReadOnlyCollection<CarDto>> GetCarsAsync()
{
    var cars = await _carRepository.GetAllAsync();
    return cars.Select(car => new CarDto(car.Id, car.Name, car.DateRelease)).ToArray();
}
```

**Depois**

- A projeção foi extraída para um contrato que pode receber novas implementações.

```csharp
public interface ICarDtoMapper
{
    CarDto Map(Car car);
}
```

```csharp
public sealed class CarDtoMapper : ICarDtoMapper
{
    public CarDto Map(Car car)
        => new(car.Id, car.Name, car.DateRelease);
}
```

```csharp
public class CarsService : ICarsService
{
    private readonly ICarDtoMapper _carDtoMapper;

    public async Task<IReadOnlyCollection<CarDto>> GetCarsAsync()
    {
        var cars = await _carRepository.GetAllAsync();
        return cars.Select(_carDtoMapper.Map).ToArray();
    }
}
```

**Como ficou**

- `CarsService` depende de `ICarDtoMapper`.
- Para criar outra projeção ou outra estratégia de mapeamento, basta adicionar nova implementação sem alterar a regra principal do serviço.

**Por que isso melhora**

- O serviço ficou aberto para extensão e fechado para modificação.
- Novas formas de mapear carro não exigem mexer no fluxo principal de consulta.

## L: Liskov Substitution Principle

**Refatoração**

- O contrato de hashing foi fortalecido para permitir substituição real de algoritmos de senha.

**Onde foi refatorado**

- [IPasswordHasher](./Volkswagen.Dashboard.Services/Security/IPasswordHasher.cs)
- [Md5PasswordHasher](./Volkswagen.Dashboard.Services/Security/Md5PasswordHasher.cs)
- [AuthService](./Volkswagen.Dashboard.Services/Auth/AuthService.cs)

**Antes**

- O consumidor podia assumir que validar senha era recalcular o hash manualmente e comparar texto.

```csharp
public interface IPasswordHasher
{
    string Hash(string input);
}
```

```csharp
var passwordHash = _passwordHasher.Hash(request.Password);
if (passwordHash != user.PasswordHash)
{
    throw new DomainException("Usuário ou senha inválidos");
}
```

**Depois**

- O contrato passou a incluir a verificação, permitindo trocar a implementação sem quebrar o consumidor.

```csharp
public interface IPasswordHasher
{
    string Hash(string input);
    bool Verify(string input, string hash);
}
```

```csharp
public sealed class Md5PasswordHasher : IPasswordHasher
{
    public bool Verify(string input, string hash)
        => string.Equals(Hash(input), hash, StringComparison.Ordinal);
}
```

```csharp
if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
{
    throw new DomainException("Usuário ou senha inválidos");
}
```

**Como ficou**

- `IPasswordHasher` agora expõe `Hash` e `Verify`.
- `AuthService` não assume mais que validar senha significa recalcular hash e comparar texto.

**Por que isso melhora**

- Agora uma implementação com salt, bcrypt ou argon2 pode substituir a atual sem quebrar o contrato esperado pelo serviço.
- O consumidor depende do comportamento correto de verificação, e não de um detalhe interno do algoritmo.

## I: Interface Segregation Principle

**Refatoração**

- O contrato de autenticação foi segregado em interfaces menores para login e registro.

**Onde foi refatorado**

- [ILoginService](./Volkswagen.Dashboard.Services/Auth/ILoginService.cs)
- [IRegistrationService](./Volkswagen.Dashboard.Services/Auth/IRegistrationService.cs)
- [AuthService](./Volkswagen.Dashboard.Services/Auth/AuthService.cs)
- [UserController](./Volkswagen.Dashboard.WebApi/Controllers/UserController.cs)

**Antes**

- O controller dependia de uma interface única com mais operações do que precisava por endpoint.

```csharp
public interface IAuthService
{
    Task<LoginResponse> Login(LoginRequest request);
    Task<bool> Register(RegisterRequest request);
}
```

```csharp
public class UserController : ControllerBase
{
    private readonly IAuthService _authService;
}
```

**Depois**

- O login e o registro passaram a depender de contratos menores e específicos.

```csharp
public interface ILoginService
{
    Task<LoginResponse> Login(LoginRequest request);
}
```

```csharp
public interface IRegistrationService
{
    Task<bool> Register(RegisterRequest request);
}
```

```csharp
public class UserController : ControllerBase
{
    private readonly IRegistrationService _registrationService;
    private readonly ILoginService _loginService;
}
```

```csharp
builder.Services.AddScoped<IAuthService>(sp => sp.GetRequiredService<AuthService>());
builder.Services.AddScoped<ILoginService>(sp => sp.GetRequiredService<AuthService>());
builder.Services.AddScoped<IRegistrationService>(sp => sp.GetRequiredService<AuthService>());
```

**Como ficou**

- `UserController` usa interfaces menores e mais específicas.
- O endpoint de login não depende mais de um contrato que também obriga conhecer registro, e vice-versa.

**Por que isso melhora**

- Os consumidores passam a depender apenas do que realmente usam.
- O acoplamento entre casos de uso distintos foi reduzido.

## D: Dependency Inversion Principle

**Refatoração**

- O controller de carros deixou de depender de uma implementação concreta de inspeção do token e passou a depender de uma abstração.

**Onde foi refatorado**

- [ITokenInspector](./Volkswagen.Dashboard.WebApi/Validators/ITokenInspector.cs)
- [TokenValidator](./Volkswagen.Dashboard.WebApi/Validators/TokenValidator.cs)
- [CarController](./Volkswagen.Dashboard.WebApi/Controllers/CarController.cs)
- [Program.cs](./Volkswagen.Dashboard.WebApi/Program.cs)

**Antes**

- O controller instanciava ou chamava diretamente a implementação concreta.

```csharp
public async Task<IActionResult> GetCars()
{
    var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
    new TokenValidator().Inspect(token);
    return Ok(await _mediator.Send(new GetCarsQuery()));
}
```

**Depois**

- A dependência concreta foi invertida para a composição da aplicação.

```csharp
public interface ITokenInspector
{
    void Inspect(string token);
}
```

```csharp
public class CarController : ControllerBase
{
    private readonly ITokenInspector _tokenInspector;

    public async Task<IActionResult> GetCars()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        _tokenInspector.Inspect(token);
        return Ok(await _mediator.Send(new GetCarsQuery()));
    }
}
```

```csharp
builder.Services.AddSingleton<ITokenInspector, TokenValidator>();
```

**Como ficou**

- `CarController` conhece apenas `ITokenInspector`.
- A implementação concreta fica invertida para a composição no `Program.cs`.

**Por que isso melhora**

- O módulo de alto nível não depende mais diretamente da implementação concreta.
- Fica mais simples trocar implementação, testar controller e evoluir a inspeção do token.

## Testes adicionados

- [AuthServiceTests](./Volkswagen.Dashboard.Tests/Auth/AuthServiceTests.cs)
- [CarsServiceTests](./Volkswagen.Dashboard.Tests/Cars/CarsServiceTests.cs)

**AuthServiceTests**

- Garante que o login usa `Verify`, e não uma comparação manual derivada de `Hash`.

```csharp
_passwordHasherMock.Setup(x => x.Verify(request.Password, user.PasswordHash))
    .Returns(true);

await _service.Login(request);

_passwordHasherMock.Verify(x => x.Verify(request.Password, user.PasswordHash), Times.Once);
_passwordHasherMock.Verify(x => x.Hash(It.IsAny<string>()), Times.Never);
```

**CarsServiceTests**

- Garante que a projeção ficou delegada ao mapper.

```csharp
_repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new[] { gol, polo });
_mapperMock.Setup(x => x.Map(gol)).Returns(new CarDto(gol.Id, gol.Name, gol.DateRelease));
_mapperMock.Setup(x => x.Map(polo)).Returns(new CarDto(polo.Id, polo.Name, polo.DateRelease));

var result = await _service.GetCarsAsync();

_mapperMock.Verify(x => x.Map(gol), Times.Once);
_mapperMock.Verify(x => x.Map(polo), Times.Once);
```

## Validação planejada

- `dotnet build Volkswagen.Dashboard.WebApi.sln`
- `dotnet test Volkswagen.Dashboard.WebApi.sln`
