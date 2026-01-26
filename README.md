# Volkswagen Dashboard WebAPI

API REST para gerenciamento de carros da Volkswagen, desenvolvida em **.NET 9.0** com arquitetura em camadas, autenticação JWT e PostgreSQL.

> Projeto de exemplo para acompanhamento de aulas.

---

## Estrutura do Projeto

```
Volkswagen.Dashboard.WebApi.sln
│
├── Volkswagen.Dashboard.WebApi/       # Camada de Apresentação (API)
│   ├── Controllers/                   # Endpoints REST
│   ├── Validators/                    # Validação de tokens
│   ├── Program.cs                     # Configuração da aplicação
│   └── appsettings.json              # Configurações
│
├── Volkswagen.Dashboard.Services/     # Camada de Negócio
│   ├── Auth/                          # Serviços de autenticação
│   └── Cars/                          # Serviços de carros
│
├── Volkswagen.Dashboard.Repository/   # Camada de Dados
│   ├── CarsRepository.cs              # Acesso a dados de carros
│   └── UserRepository.cs              # Acesso a dados de usuários
│
└── Volkswagen.Dashboard.Tests/        # Testes Unitários
    └── TestOne.cs                     # Testes com NUnit + Moq
```

---

## Arquitetura em Camadas

```
┌─────────────────────────────────────┐
│          Controllers (API)          │  ← Recebe requisições HTTP
├─────────────────────────────────────┤
│          Services (Negócio)         │  ← Regras de negócio
├─────────────────────────────────────┤
│        Repository (Dados)           │  ← Acesso ao banco de dados
├─────────────────────────────────────┤
│       PostgreSQL (Banco)            │  ← Persistência
└─────────────────────────────────────┘
```

### Princípios Aplicados
- **Separação de responsabilidades** - Cada camada tem uma função específica
- **Injeção de dependência** - Configurada no `Program.cs`
- **Interfaces** - Contratos entre camadas (ICarsService, ICarsRepository)

---

## Tecnologias Utilizadas

| Tecnologia | Versão | Finalidade |
|------------|--------|------------|
| .NET | 9.0 | Framework principal |
| ASP.NET Core | 9.0 | Web API REST |
| PostgreSQL | - | Banco de dados |
| Dapper | 2.1.28 | Micro-ORM |
| Npgsql | 9.0.3 | Driver PostgreSQL |
| JWT Bearer | 9.0.0 | Autenticação |
| Swagger | 6.5.0 | Documentação da API |
| NUnit | 3.13.3 | Framework de testes |
| Moq | 4.20.70 | Mocking para testes |

---

## Endpoints da API

### Autenticação (`/api/user`)

| Método | Rota | Descrição | Autenticação |
|--------|------|-----------|--------------|
| POST | `/api/user/register` | Registrar novo usuário | Não |
| POST | `/api/user/login` | Login (retorna JWT) | Não |

**Exemplo - Registro:**
```json
POST /api/user/register
{
  "username": "joao",
  "email": "joao@email.com",
  "password": "senha123"
}
```

**Exemplo - Login:**
```json
POST /api/user/login
{
  "email": "joao@email.com",
  "password": "senha123"
}

// Resposta:
{
  "createdAt": "2024-01-01T10:00:00",
  "expiresAt": "2024-01-01T12:00:00",
  "accessToken": "eyJhbGciOiJIUzI1NiIs..."
}
```

### Carros (`/api/car`)

| Método | Rota | Descrição | Autenticação |
|--------|------|-----------|--------------|
| GET | `/api/car` | Listar todos os carros | JWT |
| GET | `/api/car/{id}` | Buscar carro por ID | Não |
| POST | `/api/car` | Criar novo carro | Não |
| PUT | `/api/car/{id}` | Atualizar carro | Não |
| DELETE | `/api/car` | Deletar carro | Não |

**Exemplo - Criar Carro:**
```json
POST /api/car
{
  "name": "Polo",
  "dateRelease": "2024-01-15"
}
```

**Exemplo - Requisição Autenticada:**
```bash
GET /api/car
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

---

## Como Executar

### Pré-requisitos
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- (Opcional) [Docker](https://www.docker.com/)

### Execução Local

```bash
# 1. Clone o repositório
git clone <url-do-repositorio>
cd aula_dotnet_cars

# 2. Restaure as dependências
dotnet restore

# 3. Execute a aplicação
cd Volkswagen.Dashboard.WebApi
dotnet run

# A API estará disponível em:
# - HTTP:  http://localhost:5266
# - HTTPS: https://localhost:7037
```

### Execução com Docker

```bash
# Build da imagem
docker build -t volkswagen-api -f Volkswagen.Dashboard.WebApi/Dockerfile .

# Executar container
docker run -p 8080:8080 -p 8081:8081 volkswagen-api
```

### Acessar Documentação Swagger

Após iniciar a aplicação, acesse:
```
http://localhost:5266/swagger
```

---

## Executar Testes

```bash
# Executar todos os testes
dotnet test

# Executar com detalhes
dotnet test --verbosity normal

# Executar com cobertura de código
dotnet test --collect:"XPlat Code Coverage"
```

### Testes Implementados
- `Should_GetCarsWithSuccess` - Testa listagem de carros
- `Should_GetCarByIdWithSuccess` - Testa busca por ID

---

## Modelos de Dados

### CarModel
```csharp
public class CarModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime DateRelease { get; set; }
}
```

### UserModel
```csharp
public class UserModel
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
```

---

## Banco de Dados

### Tabelas

**volksdatatable** (Carros)
| Coluna | Tipo | Descrição |
|--------|------|-----------|
| id | SERIAL | Chave primária |
| carname | VARCHAR | Nome do carro |
| car_date_release | DATE | Data de lançamento |

**users** (Usuários)
| Coluna | Tipo | Descrição |
|--------|------|-----------|
| id | SERIAL | Chave primária |
| username | VARCHAR | Nome do usuário |
| email | VARCHAR | Email (único) |
| password | VARCHAR | Senha (hash MD5) |
| created_at | TIMESTAMP | Data de criação |

---

## Estrutura de Pastas Detalhada

```
.
├── Volkswagen.Dashboard.WebApi/
│   ├── Controllers/
│   │   ├── CarController.cs           # CRUD de carros
│   │   └── UserController.cs          # Autenticação
│   ├── Validators/
│   │   └── TokenValidator.cs          # Validação JWT
│   ├── Program.cs                     # Startup + DI + Auth
│   ├── appsettings.json              # Connection string
│   └── Dockerfile                     # Container config
│
├── Volkswagen.Dashboard.Services/
│   ├── Auth/
│   │   ├── IAuthService.cs            # Interface
│   │   └── AuthService.cs             # Implementação
│   ├── Cars/
│   │   ├── ICarsService.cs            # Interface
│   │   └── CarsService.cs             # Implementação
│   └── Models/
│       ├── CarModel.cs
│       ├── UserModel.cs
│       ├── LoginRequest.cs
│       ├── LoginResponse.cs
│       └── RegisterRequest.cs
│
├── Volkswagen.Dashboard.Repository/
│   ├── ICarsRepository.cs             # Interface
│   ├── CarsRepository.cs              # Implementação
│   ├── IUserRepository.cs             # Interface
│   └── UserRepository.cs              # Implementação
│
└── Volkswagen.Dashboard.Tests/
    └── TestOne.cs                     # Testes unitários
```

---

## Conceitos Abordados nas Aulas

- [x] Arquitetura em camadas (Layered Architecture)
- [x] Injeção de Dependência (DI)
- [x] Padrão Repository
- [x] RESTful API
- [x] Autenticação JWT
- [x] Integração com PostgreSQL
- [x] Micro-ORM (Dapper)
- [x] Testes unitários com Mocking
- [x] Containerização com Docker
- [ ] CI/CD (GitLab CI)
- [x] Documentação com Swagger/OpenAPI

---

## Comandos Úteis

```bash
# Criar nova migration (se usar EF Core)
dotnet ef migrations add NomeMigration

# Criar novo projeto na solution
dotnet new classlib -n NomeProjeto
dotnet sln add NomeProjeto/NomeProjeto.csproj

# Adicionar referência entre projetos
dotnet add reference ../OutroProjeto/OutroProjeto.csproj

# Adicionar pacote NuGet
dotnet add package NomePacote

# Limpar e rebuildar
dotnet clean && dotnet build
```

---

## Licença

Projeto desenvolvido para fins educacionais.
