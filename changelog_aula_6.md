# Changelog Aula 6 - Classes, Instâncias e Herança (MVC)

Data: 19/02/2026  
Fonte da aula: `/home/afonsolelis/aulas/pages/module-9-sistemas-informacao/lesson-6.html`

## 1) Tema da aula 6

A aula 6 cobre:
- Programação Orientada a Objetos avançada.
- Classes e instâncias (atributos, métodos, construtores).
- Herança e polimorfismo.
- Interfaces e classes abstratas.
- UML (Unified Modeling Language, ou Linguagem de Modelagem Unificada): padrão visual para representar classes, relações e estrutura do software.
- Modelo MVC (Model-View-Controller): arquitetura que separa dados/regra (Model), interface (View) e fluxo de entrada/saída (Controller).
- Boas práticas para projetos corporativos.

## 2) Onde isso aparece no projeto atual

### 2.1 Classes e instâncias
Exemplos de classes concretas:
- `Volkswagen.Dashboard.Repository/CarModel.cs`
- `Volkswagen.Dashboard.Repository/UserModel.cs`
- `Volkswagen.Dashboard.Services/Cars/CarsService.cs`
- `Volkswagen.Dashboard.Services/Auth/AuthService.cs`

Como observar instâncias na prática:
- O ASP.NET (framework web do .NET) cria instâncias por requisição via DI (Dependency Injection, ou Injeção de Dependência), que é a técnica de fornecer dependências prontas para uma classe em vez de ela criar tudo sozinha.
- Exemplo: `CarsService` recebe uma instância de `ICarsRepository` no construtor.

### 2.2 Herança e polimorfismo
No projeto atual, o uso mais claro de herança/polimorfismo está na API (Application Programming Interface), que é a camada de contrato de comunicação entre sistemas/clientes e o backend:
- `Volkswagen.Dashboard.WebApi/Controllers/CarController.cs`
- `Volkswagen.Dashboard.WebApi/Controllers/UserController.cs`

Detalhe:
- Controllers herdam de `ControllerBase`.
- Polimorfismo aparece no uso de interfaces (`ICarsService`, `IAuthService`, `ICarsRepository`, `IUserRepository`).

### 2.3 Interfaces e abstração
Interfaces já implementadas no projeto:
- `Volkswagen.Dashboard.Services/Cars/ICarsService.cs`
- `Volkswagen.Dashboard.Services/Auth/IAuthService.cs`
- `Volkswagen.Dashboard.Repository/ICarsRepository.cs`
- `Volkswagen.Dashboard.Repository/IUserRepository.cs`

Implementações concretas:
- `CarsService : ICarsService`
- `AuthService : IAuthService`
- `CarsRepository : ICarsRepository`
- `UserRepository : IUserRepository`

### 2.4 MVC (adaptação ao projeto)
O projeto não está em ASP.NET MVC tradicional com Views Razor no backend; ele está dividido em:
- API REST (Representational State Transfer), estilo arquitetural de APIs HTTP baseado em recursos e verbos (GET, POST, PUT, DELETE), em `WebApi` para controller + lógica de aplicação.
- Frontend Blazor (`Web`) como camada de interface.

Mapeamento didático para entender MVC no contexto atual:
- Model: `CarModel`, `UserModel`, DTOs (Data Transfer Objects, ou Objetos de Transferência de Dados), que são objetos simples usados para transportar dados entre camadas sem carregar regra de negócio.
- Controller: `CarController`, `UserController`.
- View: páginas Blazor em `Volkswagen.Dashboard.Web/Components/Pages`.

## 3) Passo a passo para os alunos (roteiro de estudo)

### Passo 1: começar pela entrada da aplicação
1. Abrir `Volkswagen.Dashboard.WebApi/Program.cs`.
2. Identificar registro de dependências (`AddScoped`, `AddSingleton`).
3. Entender como as instâncias são criadas automaticamente.

### Passo 2: seguir o fluxo completo de uma funcionalidade
1. Abrir `Volkswagen.Dashboard.WebApi/Controllers/CarController.cs`.
2. Seguir para `Volkswagen.Dashboard.Services/Cars/CarsService.cs`.
3. Seguir para `Volkswagen.Dashboard.Repository/CarsRepository.cs`.
4. Fechar no modelo `Volkswagen.Dashboard.Repository/CarModel.cs`.

### Passo 3: estudar polimorfismo por interface
1. Abrir `Volkswagen.Dashboard.Services/Cars/ICarsService.cs`.
2. Comparar com `Volkswagen.Dashboard.Services/Cars/CarsService.cs`.
3. Repetir para Auth:
   - `Volkswagen.Dashboard.Services/Auth/IAuthService.cs`
   - `Volkswagen.Dashboard.Services/Auth/AuthService.cs`

### Passo 4: ligar backend com frontend (visão MVC prática)
1. Abrir `Volkswagen.Dashboard.Web/Program.cs`.
2. Ver os `AddHttpClient` que conectam o frontend à API.
3. Abrir páginas em `Volkswagen.Dashboard.Web/Components/Pages` para ver a “View”.

### Passo 5: validar comportamento por testes
1. Abrir `Volkswagen.Dashboard.Tests/TestOne.cs`.
2. Observar mock de repositório e teste da camada de serviço.
3. Relacionar com “desacoplamento via interface”.

## 4) Aula 6 aplicada: exercícios recomendados

### Exercício A - Classes e construtor
1. Criar uma nova entidade de domínio (ex.: `MaintenanceRecord`).
2. Adicionar construtor com validações básicas.
3. Escrever teste para criação válida/inválida.

### Exercício B - Herança e polimorfismo
1. Criar uma classe base abstrata `VehicleBase`.
2. Criar classes derivadas (ex.: `CarVehicle`, `TruckVehicle`).
3. Consumir via referência da classe base para demonstrar polimorfismo.

### Exercício C - Interface para desacoplamento
1. Criar interface nova (ex.: `ICarValidationService`).
2. Implementar classe concreta.
3. Injetar no `CarsService` e cobrir com testes usando `Moq`.

### Exercício D - UML para o código atual
1. Montar diagrama de classes com:
   - `CarController`
   - `ICarsService`/`CarsService`
   - `ICarsRepository`/`CarsRepository`
   - `CarModel`
2. Marcar no diagrama:
   - Dependências por interface.
   - Relação entre controller e service.
   - Relação entre service e repository.

## 5) Boas práticas reforçadas na aula 6

- Preferir composição e interfaces para reduzir acoplamento.
- Manter responsabilidade clara por classe/camada.
- Evitar lógica de negócio em controller.
- Tratar modelos como representação de domínio/dados, não “depósitos de tudo”.
- Usar testes para garantir comportamento ao refatorar.

## 6) Checklist rápido do aluno

1. Consegue explicar diferença entre classe e instância no projeto.
2. Consegue apontar exemplo de herança (controllers) e de polimorfismo (interfaces).
3. Consegue mapear Model-Controller-View no contexto API + Blazor.
4. Consegue desenhar um diagrama UML básico da feature de carros.
5. Consegue rodar testes após qualquer alteração.

## 7) Comandos úteis para a aula

```bash
# Restaurar dependências
dotnet restore Volkswagen.Dashboard.WebApi.sln

# Rodar testes
dotnet test Volkswagen.Dashboard.WebApi.sln

# Subir banco
docker compose up -d

# Rodar API
cd Volkswagen.Dashboard.WebApi
dotnet run

# Rodar Frontend (outro terminal)
cd Volkswagen.Dashboard.Web
dotnet run --urls "http://localhost:5100"
```
