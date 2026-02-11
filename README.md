# Volkswagen Dashboard

Sistema completo de gerenciamento de carros da Volkswagen, desenvolvido em **.NET 9.0** com arquitetura em camadas, autenticação JWT, PostgreSQL e frontend Blazor SSR.

> Projeto de exemplo para acompanhamento de aulas.

---

## Passo a Passo (Terminal)

### 1. Entrar na pasta do projeto

```bash
cd aula_dotnet_cars
```

### 2. Restaurar dependencias

```bash
dotnet restore Volkswagen.Dashboard.WebApi.sln
```

### 3. Subir o banco (MongoDB via Docker)

```bash
docker compose up -d
docker compose ps
```

### 4. Subir a API (Terminal 1)

```bash
cd Volkswagen.Dashboard.WebApi
dotnet run
```

API: `http://localhost:5266`  
Swagger: `http://localhost:5266/swagger`

### 5. Subir o Frontend (Terminal 2)

```bash
cd Volkswagen.Dashboard.Web
dotnet run --urls "http://localhost:5100"
```

Frontend: `http://localhost:5100`

### 6. (Opcional) Testar login via curl (Terminal 3)

```bash
curl -X POST http://localhost:5266/api/user/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@vw.com","password":"admin123"}'
```

### 7. Parar ambiente

```bash
docker compose down
```

**Credenciais de teste:**
- Email: `admin@vw.com`
- Senha: `admin123`

---

## Estrutura do Projeto

```
Volkswagen.Dashboard.WebApi.sln
│
├── Volkswagen.Dashboard.WebApi/       # API REST
├── Volkswagen.Dashboard.Web/          # Frontend Blazor SSR
├── Volkswagen.Dashboard.Services/     # Camada de Negocio
├── Volkswagen.Dashboard.Repository/   # Camada de Dados
├── Volkswagen.Dashboard.Tests/        # Testes Unitarios
│
├── scripts/
│   └── init.sql                       # Script de inicializacao do banco
│
└── docker-compose.yml                 # PostgreSQL local
```

---

## Arquitetura

```
┌─────────────────────────────────────┐
│      Blazor Web App (Frontend)      │  ← Interface do usuario
├─────────────────────────────────────┤
│          Controllers (API)          │  ← Endpoints REST
├─────────────────────────────────────┤
│          Services (Negocio)         │  ← Regras de negocio
├─────────────────────────────────────┤
│        Repository (Dados)           │  ← Acesso ao banco
├─────────────────────────────────────┤
│       PostgreSQL (Docker)           │  ← Persistencia
└─────────────────────────────────────┘
```

---

## Tecnologias

| Tecnologia | Versao | Finalidade |
|------------|--------|------------|
| .NET | 9.0 | Framework |
| ASP.NET Core | 9.0 | Web API REST |
| Blazor Web App | 9.0 | Frontend SSR |
| PostgreSQL | 16 | Banco de dados |
| Docker Compose | - | Orquestracao |
| Dapper | 2.1.28 | Micro-ORM |
| Npgsql | 9.0.3 | Driver PostgreSQL |
| JWT Bearer | 9.0.0 | Autenticacao |
| Swagger | 6.5.0 | Documentacao API |

---

## Banco de Dados

### Subir com Docker Compose

```bash
# Subir o banco
docker compose up -d

# Verificar se esta rodando
docker compose ps

# Ver logs
docker compose logs -f postgres

# Parar o banco
docker compose down

# Parar e remover dados
docker compose down -v
```

### Credenciais do Banco

| Parametro | Valor |
|-----------|-------|
| Host | localhost |
| Porta | 5432 |
| Database | volksdb |
| Usuario | volksdata |
| Senha | volksdata123 |

### Tabelas

**volksdatatable** (Carros)
| Coluna | Tipo | Descricao |
|--------|------|-----------|
| id | SERIAL | Chave primaria |
| carname | VARCHAR | Nome do carro |
| car_date_release | DATE | Data de lancamento |

**users** (Usuarios)
| Coluna | Tipo | Descricao |
|--------|------|-----------|
| id | SERIAL | Chave primaria |
| username | VARCHAR | Nome do usuario |
| email | VARCHAR | Email (unico) |
| password | VARCHAR | Senha (hash MD5) |
| created_at | TIMESTAMP | Data de criacao |

**email_whitelist** (Emails autorizados)
| Coluna | Tipo | Descricao |
|--------|------|-----------|
| id | SERIAL | Chave primaria |
| email | VARCHAR | Email autorizado |
| created_at | TIMESTAMP | Data de criacao |

### Dados de Teste

O script `scripts/init.sql` cria automaticamente:

**Carros:**
- Gol, Polo, Golf, Jetta, Tiguan, T-Cross, Nivus, Amarok, Virtus, Saveiro

**Usuario padrao:**
- Email: `admin@vw.com`
- Senha: `admin123`

**Whitelist de emails (autorizados para registro):**
- admin@vw.com
- teste@vw.com
- aluno@inteli.edu.br
- professor@inteli.edu.br
- dev@volkswagen.com.br

> Apenas emails na whitelist podem se registrar no sistema.

---

## API REST

### URLs

| Servico | URL |
|---------|-----|
| API | http://localhost:5266 |
| Swagger | http://localhost:5266/swagger |
| Frontend | http://localhost:5100 |

### Endpoints

**Autenticacao (`/api/user`)**
| Metodo | Rota | Descricao |
|--------|------|-----------|
| POST | `/api/user/register` | Registrar (requer email na whitelist) |
| POST | `/api/user/login` | Login (retorna JWT) |

**Carros (`/api/car`)**
| Metodo | Rota | Descricao | Auth |
|--------|------|-----------|------|
| GET | `/api/car` | Listar carros | JWT |
| GET | `/api/car/{id}` | Buscar por ID | - |
| POST | `/api/car` | Criar carro | - |
| PUT | `/api/car/{id}` | Atualizar | - |
| DELETE | `/api/car` | Deletar | - |

### Exemplos

**Login:**
```bash
curl -X POST http://localhost:5266/api/user/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@vw.com","password":"admin123"}'
```

**Listar carros (autenticado):**
```bash
curl http://localhost:5266/api/car \
  -H "Authorization: Bearer <seu-token>"
```

---

## Frontend Blazor

O frontend usa Blazor Web App com Server-Side Rendering (SSR).

### Paginas

| Rota | Descricao |
|------|-----------|
| `/` | Home |
| `/login` | Pagina de login |
| `/cars` | Lista de carros (requer login) |

### Executar

```bash
cd Volkswagen.Dashboard.Web
dotnet run --urls "http://localhost:5100"
```

---

## Desenvolvimento

### Pre-requisitos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/)

### Instalar .NET 9.0 (Linux)

```bash
# Baixar script de instalacao
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh

# Instalar SDK 9.0
./dotnet-install.sh --channel 9.0

# Adicionar ao PATH (~/.bashrc ou ~/.zshrc)
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools
```

### Comandos Uteis

```bash
# Restaurar dependencias
dotnet restore

# Compilar
dotnet build

# Executar testes
dotnet test

# Limpar e rebuildar
dotnet clean && dotnet build

# Adicionar pacote NuGet
dotnet add package NomePacote
```

---

## Testes

```bash
# Executar todos os testes
dotnet test

# Com detalhes
dotnet test --verbosity normal

# Com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

---

## Estrutura de Pastas Detalhada

```
.
├── docker-compose.yml                 # PostgreSQL
├── scripts/
│   └── init.sql                       # DDL + dados de teste
│
├── Volkswagen.Dashboard.WebApi/
│   ├── Controllers/
│   │   ├── CarController.cs
│   │   └── UserController.cs
│   ├── Validators/
│   │   └── TokenValidator.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── Dockerfile
│
├── Volkswagen.Dashboard.Web/          # Blazor Frontend
│   ├── Components/
│   │   ├── Layout/
│   │   │   └── NavMenu.razor
│   │   └── Pages/
│   │       ├── Home.razor
│   │       ├── Login.razor
│   │       └── Cars.razor
│   ├── Models/
│   ├── Services/
│   │   ├── ICarService.cs
│   │   ├── CarService.cs
│   │   ├── IAuthService.cs
│   │   └── AuthService.cs
│   └── Program.cs
│
├── Volkswagen.Dashboard.Services/
│   ├── Auth/
│   │   ├── IAuthService.cs
│   │   └── AuthService.cs
│   └── Cars/
│       ├── ICarsService.cs
│       └── CarsService.cs
│
├── Volkswagen.Dashboard.Repository/
│   ├── ICarsRepository.cs
│   ├── CarsRepository.cs
│   ├── IUserRepository.cs
│   └── UserRepository.cs
│
└── Volkswagen.Dashboard.Tests/
    └── TestOne.cs
```

---

## Conceitos Abordados

- [x] Arquitetura em camadas
- [x] Injecao de Dependencia (DI)
- [x] Padrao Repository
- [x] RESTful API
- [x] Autenticacao JWT
- [x] PostgreSQL com Docker
- [x] Micro-ORM (Dapper)
- [x] Blazor Web App (SSR)
- [x] Testes unitarios com Mocking
- [x] Docker Compose
- [ ] CI/CD (GitLab CI)
- [x] Swagger/OpenAPI

---

## Licenca

Projeto desenvolvido para fins educacionais.
