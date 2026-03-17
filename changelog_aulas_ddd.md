# Changelog DDD

## Contexto

Esta branch refatora a solução para um modelo mais próximo de Domain-Driven Design. A aplicação deixou de tratar documentos Mongo como se fossem o próprio domínio e passou a organizar responsabilidades em:

- `Volkswagen.Dashboard.Domain`: regras e linguagem ubíqua do negócio.
- `Volkswagen.Dashboard.Services`: camada de aplicação/casos de uso.
- `Volkswagen.Dashboard.Repository`: infraestrutura e persistência.
- `Volkswagen.Dashboard.WebApi`: camada de entrega, com REST, GraphQL e gRPC.

## Entidades de domínio

As entidades e value objects introduzidos para representar o domínio são:

- `Car`
  - Representa o agregado de catálogo de veículos.
  - Regras centrais: nome obrigatório e data de lançamento obrigatória.
- `User`
  - Representa o usuário apto a autenticar no sistema.
  - Regras centrais: username obrigatório, senha hash obrigatória e email tipado.
- `AuthorizedEmail`
  - Representa um email previamente autorizado para registro.
  - Faz explícita a regra de whitelist existente no negócio.
- `EmailAddress` (value object)
  - Normaliza e valida email antes de qualquer regra de negócio ou persistência.

## O que mudou e por que mudou

### 1. Nova camada `Domain`

**Mudança**

- Criação do projeto `Volkswagen.Dashboard.Domain`.
- Extração das entidades `Car`, `User`, `AuthorizedEmail`.
- Criação do value object `EmailAddress`.
- Criação das interfaces de repositório do domínio:
  - `ICarRepository`
  - `IUserRepository`
  - `IAuthorizedEmailRepository`

**Por que mudou**

- Antes, o sistema usava `CarModel` e `UserDocument` como se fossem o domínio.
- Isso misturava regra de negócio com detalhes de MongoDB.
- Em DDD, o domínio precisa ser independente da infraestrutura para preservar linguagem ubíqua, invariantes e clareza arquitetural.

### 2. `CarModel` deixou de ser domínio e virou persistência

**Mudança**

- `CarModel` foi substituído por `CarDocument` no repositório Mongo.
- `CarsRepository` passou a mapear `CarDocument <-> Car`.

**Por que mudou**

- Um documento de banco não deve vazar para controller, handler, GraphQL ou serviço de aplicação.
- O domínio de carros agora trafega como entidade `Car`, e a infraestrutura assume o papel de adaptação.

### 3. Regras de carro foram centralizadas na entidade `Car`

**Mudança**

- `Car` agora encapsula:
  - criação (`Create`)
  - restauração (`Restore`)
  - alteração de nome (`Rename`)
  - alteração de data (`ChangeReleaseDate`)

**Por que mudou**

- A lógica de validação não pode ficar espalhada por controller, handler ou repository.
- Em DDD, a entidade protege seu estado e impede dados inválidos de circularem entre as camadas.

### 4. Autenticação foi reorganizada em torno do domínio de usuários

**Mudança**

- `AuthService` deixou de trabalhar com modelos do repositório.
- Passou a usar:
  - `User`
  - `EmailAddress`
  - `IUserRepository`
  - `IAuthorizedEmailRepository`
  - `IPasswordHasher`
  - `ITokenService`

**Por que mudou**

- Login e registro são casos de uso da aplicação, não regras de persistência.
- A regra de whitelist agora ficou explícita no domínio por meio de `AuthorizedEmail`.
- Hash e JWT deixaram de ser detalhes misturados ao fluxo de negócio e passaram a ser serviços técnicos separados.

### 5. A camada `Services` virou camada de aplicação

**Mudança**

- `CarsService` passou a operar com:
  - `Car`
  - `CarDto`
  - `CreateCarInput`
  - `UpdateCarInput`
- Commands e queries do MediatR foram refatorados para trafegar dados de aplicação em vez de documentos Mongo.

**Por que mudou**

- O papel da camada de aplicação é orquestrar casos de uso, não refletir a forma como os dados são persistidos.
- Isso reduz acoplamento e deixa os handlers expressando intenção de negócio.

### 6. REST, GraphQL e gRPC deixaram de receber tipos de persistência

**Mudança**

- REST agora recebe `SaveCarRequest`.
- GraphQL e gRPC passaram a enviar comandos baseados em propriedades do caso de uso.
- A saída pública continua com `id`, `name` e `dateRelease`, mas agora derivada de `CarDto`.

**Por que mudou**

- A borda da aplicação deve trabalhar com contratos de entrada e saída, não com entidades de banco.
- Isso preserva o contrato público e evita acoplamento entre API e MongoDB.

### 7. Repositórios foram reclassificados como infraestrutura

**Mudança**

- `CarsRepository` implementa `ICarRepository`.
- `UserRepository` implementa `IUserRepository` e `IAuthorizedEmailRepository`.
- O repositório agora faz apenas acesso ao banco e mapeamento.

**Por que mudou**

- Em DDD, infraestrutura adapta a tecnologia ao domínio.
- Ela não deve ser a fonte da modelagem do negócio.

### 8. Segurança técnica foi isolada da regra de negócio

**Mudança**

- Criação de:
  - `IPasswordHasher`
  - `Md5PasswordHasher`
  - `ITokenService`
  - `JwtTokenService`

**Por que mudou**

- O domínio não deve conhecer MD5 nem JWT.
- A aplicação precisa só declarar que depende de hashing e geração de token.
- O algoritmo MD5 foi mantido para não quebrar compatibilidade com a base atual, mas agora está isolado e facilmente substituível.

### 9. Testes unitários foram ajustados para a nova linguagem do domínio

**Mudança**

- Os testes de CQRS deixaram de depender de `CarModel` e passaram a validar `CarDto` e os novos casos de uso.
- O teste antigo `TestOne.cs`, que exercia a API anterior do serviço, foi removido.

**Por que mudou**

- O alvo do teste deve refletir a arquitetura atual.
- Manter testes baseados em contratos antigos aumentaria ruído e falso acoplamento.

## Impacto arquitetural

### Antes

- Controllers, handlers e serviços manipulavam tipos de persistência.
- Regras de negócio estavam misturadas com Mongo, hash e JWT.
- O modelo de domínio era implícito e frágil.

### Depois

- O domínio é explícito e independente.
- A aplicação orquestra casos de uso.
- A infraestrutura adapta Mongo para o domínio.
- As bordas da API consomem DTOs e requests próprios.

## Arquivos principais alterados

- `Volkswagen.Dashboard.Domain/*`
- `Volkswagen.Dashboard.Services/Cars/*`
- `Volkswagen.Dashboard.Services/Auth/*`
- `Volkswagen.Dashboard.Services/Security/*`
- `Volkswagen.Dashboard.Services/CQRS/*`
- `Volkswagen.Dashboard.Repository/CarsRepository.cs`
- `Volkswagen.Dashboard.Repository/UserRepository.cs`
- `Volkswagen.Dashboard.Repository/MongoSchemaInitializer.cs`
- `Volkswagen.Dashboard.WebApi/Controllers/CarController.cs`
- `Volkswagen.Dashboard.WebApi/GraphQL/*`
- `Volkswagen.Dashboard.WebApi/Grpc/CarGrpcService.cs`
- `Volkswagen.Dashboard.WebApi/Program.cs`
- `Volkswagen.Dashboard.WebApi.sln`

## Validação executada

- `dotnet build Volkswagen.Dashboard.WebApi.sln` ✅
- `dotnet test Volkswagen.Dashboard.WebApi.sln --no-build`
  - 5 testes passaram.
  - 3 testes de integração falharam por timeout de conexão com o Mongo configurado no ambiente (`crossover.proxy.rlwy.net:24258`).
  - A falha observada é de infraestrutura externa, não de compilação da refatoração.
