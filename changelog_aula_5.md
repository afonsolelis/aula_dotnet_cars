# Changelog Aula 5 - GoF, Padrões Modernos e Arquitetura Distribuída

Data: 19/02/2026  
Fonte da aula: https://afonsolelis.github.io/aulas/pages/module-9-sistemas-informacao/lesson-5.html

## 1) O que a aula 5 cobre

A aula aborda, em sequência:
- História da GoF e por que padrões existem.
- Categorias de padrões: Criacionais, Estruturais e Comportamentais.
- Padrões essenciais (Singleton, Factory Method, Observer, Strategy, Decorator, Adapter, Chain of Responsibility).
- Padrões modernos: Event Sourcing, CQRS e combinação CQRS + Event Sourcing.
- Padrões de microservices: Circuit Breaker, Saga, API Gateway, Strangler Fig, Service Mesh/Observability.
- Anti-patterns e quando não usar padrão.

## 2) Mapa rápido do projeto (onde cada camada está)

- API: `Volkswagen.Dashboard.WebApi`
- Serviços de negócio: `Volkswagen.Dashboard.Services`
- Repositório/dados (Mongo): `Volkswagen.Dashboard.Repository`
- Frontend Blazor: `Volkswagen.Dashboard.Web`
- Testes: `Volkswagen.Dashboard.Tests`

Fluxo atual principal:
- `Controller` -> `Service` -> `Repository` -> MongoDB

## 3) Onde os conceitos da aula aparecem hoje no código

### 3.1 Singleton
- Já aparece por DI no ASP.NET Core:
  - `Volkswagen.Dashboard.WebApi/Program.cs`
  - Registros `AddSingleton<IMongoClient>`, `AddSingleton<IMongoSchemaInitializer>`.
- Observação para alunos: em .NET moderno, muitas vezes usamos Singleton via container DI em vez de classe Singleton manual.

### 3.2 Factory (Factory Method)
- Não há Factory Method explícito no domínio atualmente.
- Há um ponto parecido com “fábrica de cliente HTTP” via DI:
  - `Volkswagen.Dashboard.Web/Program.cs` com `AddHttpClient<ICarService, CarService>` e `AddHttpClient<IAuthService, AuthService>`.
- Próximo passo didático: criar `NotificacaoFactory` ou `RelatorioFactory` na camada `Services`.

### 3.3 Observer
- Não há implementação explícita de Observer no backend hoje.
- Referência conceitual: o próprio modelo de eventos em UI reativa (Blazor) segue a ideia de pub/sub internamente.
- Próximo passo didático: criar um fluxo de evento de domínio (ex.: `CarroCriadoEvent`) e subscribers internos.

### 3.4 Strategy
- Não há Strategy explícita no projeto atual.
- Próximo passo didático: extrair regras variáveis para estratégias (ex.: validação de carro, política de ordenação/filtro, regra de score).

### 3.5 Decorator
- Implementação conceitual já existe no pipeline de middleware:
  - `Volkswagen.Dashboard.WebApi/Program.cs` (`UseHttpsRedirection`, `UseCors`, `UseAuthentication`, `UseAuthorization`)
  - `Volkswagen.Dashboard.Web/Program.cs` (`UseExceptionHandler`, `UseHsts`, `UseAntiforgery`)
- Observação para alunos: middleware do ASP.NET Core é um caso clássico de Decorator + Chain.

### 3.6 Adapter
- Não há adapter explícito nomeado no código.
- Ponto de adaptação atual:
  - `Volkswagen.Dashboard.Web/Services/CarService.cs`
  - `Volkswagen.Dashboard.Web/Services/AuthService.cs`
- Esses serviços já “adaptam” chamadas HTTP para objetos da aplicação; falta formalizar como Adapter Pattern com interface alvo + adaptee legada.

### 3.7 Chain of Responsibility
- Melhor exemplo atual: pipeline HTTP/middleware do ASP.NET Core (cada etapa decide continuar ou interromper).
- Não há cadeia de handlers de domínio implementada ainda.

### 3.8 Event Sourcing, CQRS e Microservices Patterns
- Ainda não implementados no projeto atual (estão como trilha de evolução arquitetural).
- O projeto atual é monolítico em camadas, bom para iniciar padrões GoF antes de avançar para distribuição.

## 4) Passo a passo para o aluno explorar o projeto em ordem

### Passo 1: Entender composição da aplicação
1. Abrir `Volkswagen.Dashboard.WebApi.sln`.
2. Ler `README.md` para subir ambiente e endpoints.
3. Confirmar estrutura de projetos (`WebApi`, `Services`, `Repository`, `Web`, `Tests`).

### Passo 2: Ver DI e ciclo de requisição
1. Abrir `Volkswagen.Dashboard.WebApi/Program.cs`.
2. Identificar registros `AddScoped` e `AddSingleton`.
3. Identificar pipeline (`UseCors`, `UseAuthentication`, etc.) e relacionar com Decorator/Chain.

### Passo 3: Seguir uma rota completa
1. Abrir `Volkswagen.Dashboard.WebApi/Controllers/CarController.cs`.
2. Seguir para `Volkswagen.Dashboard.Services/Cars/CarsService.cs`.
3. Seguir para `Volkswagen.Dashboard.Repository/CarsRepository.cs`.
4. Entender onde regra de negócio termina e onde persistência começa.

### Passo 4: Entender autenticação
1. Abrir `Volkswagen.Dashboard.WebApi/Controllers/UserController.cs`.
2. Seguir para `Volkswagen.Dashboard.Services/Auth/AuthService.cs`.
3. Seguir para `Volkswagen.Dashboard.Repository/UserRepository.cs`.

### Passo 5: Entender o frontend como cliente da API
1. Abrir `Volkswagen.Dashboard.Web/Program.cs`.
2. Ler `Volkswagen.Dashboard.Web/Services/CarService.cs`.
3. Ler `Volkswagen.Dashboard.Web/Services/AuthService.cs`.
4. Ver uso em páginas Blazor (`Volkswagen.Dashboard.Web/Components/Pages/*.razor`).

### Passo 6: Entender base de testes
1. Abrir `Volkswagen.Dashboard.Tests/Volkswagen.Dashboard.Tests.csproj`.
2. Ler `Volkswagen.Dashboard.Tests/TestOne.cs`.
3. Identificar uso de `Moq` para isolamento da camada `Services`.

## 5) Como implementar os padrões da aula 5 neste repositório (roteiro prático)

### 5.1 Strategy (primeiro padrão recomendado)
Objetivo: remover condicionais grandes de regra de negócio.

1. Criar pasta `Volkswagen.Dashboard.Services/Cars/Strategies`.
2. Criar interface `ICarValidationStrategy`.
3. Criar estratégias concretas (ex.: validação para criação, validação para atualização).
4. Injetar estratégia no `CarsService`.
5. Criar testes unitários por estratégia em `Volkswagen.Dashboard.Tests`.

### 5.2 Factory Method
Objetivo: centralizar criação de objetos com variação de tipo.

1. Criar pasta `Volkswagen.Dashboard.Services/Notifications`.
2. Criar `INotification`, implementações concretas e `NotificationFactory`.
3. Chamar factory no fluxo de negócio (ex.: após cadastro/login).
4. Testar seleção da factory e objeto retornado.

### 5.3 Observer (evento interno)
Objetivo: desacoplar ações secundárias da ação principal.

1. Criar evento de domínio (ex.: `CarCreatedEvent`).
2. Criar handlers (ex.: log, analytics, auditoria).
3. Publicar evento no fim de `InsertCar`.
4. Validar por teste que handlers são acionados.

### 5.4 Adapter (integração externa)
Objetivo: desacoplar API de terceiros do seu domínio.

1. Criar interface alvo na `Services` (ex.: `IAnalyticsClient`).
2. Criar `AnalyticsAdapter` que converte DTO interno para contrato externo.
3. Isolar chamadas HTTP do provedor externo no adapter.
4. Mockar adapter nos testes de serviço.

### 5.5 Chain of Responsibility (pipeline de validação)
Objetivo: validar comentários/cadastros em cadeia.

1. Criar `IHandler<T>` com `SetNext` e `Handle`.
2. Criar handlers pequenos (ex.: spam, tamanho, caracteres proibidos).
3. Montar corrente no startup/DI.
4. Testar cenário que interrompe e cenário que passa por todos.

## 6) Trilha avançada (Aula 5 moderna)

### 6.1 Event Sourcing (introdução no projeto)
1. Criar agregado (ex.: `CommentAggregate`).
2. Criar eventos (`CommentCreated`, `CommentEdited`).
3. Persistir eventos em uma coleção de eventos.
4. Reconstruir estado aplicando eventos.

### 6.2 CQRS
1. Separar comandos e queries em pastas/projetos lógicos.
2. Write side: comando altera estado.
3. Read side: query otimizada para leitura.
4. Criar testes separados para command handlers e query handlers.

### 6.3 Microservices patterns (quando migrar)
- Circuit Breaker: proteger chamadas entre serviços (Polly).
- Saga: compensação em fluxos distribuídos.
- API Gateway: entrada única para múltiplos serviços.
- Strangler Fig: migrar monólito gradualmente.
- Service Mesh/Observability: tráfego, métricas, logs e tracing.

## 7) Anti-patterns da aula e alerta para este projeto

Evitar:
- God Object (serviços gigantes com responsabilidades demais).
- Spaghetti code (regras sem organização por camada/padrão).
- Copy-paste de validações entre `AuthService` e `CarsService`.
- Golden Hammer (forçar padrão quando um fluxo simples resolve).
- Premature optimization (otimizar antes de medir).

## 8) Checklist de aula para os alunos

1. Conseguir apontar no código um exemplo de Singleton via DI.
2. Explicar por que middleware do ASP.NET se relaciona com Decorator/Chain.
3. Implementar 1 padrão novo (Strategy recomendado) com testes.
4. Registrar no PR: problema original, padrão escolhido e ganho obtido.
5. Não subir artefatos de build/teste (`bin/`, `obj/`, `TestResults/`).

## 9) Comandos úteis para laboratório

```bash
# Restaurar
dotnet restore Volkswagen.Dashboard.WebApi.sln

# Rodar testes
dotnet test Volkswagen.Dashboard.WebApi.sln

# Subir ambiente local
docker compose up -d

# API
cd Volkswagen.Dashboard.WebApi && dotnet run

# Frontend
cd Volkswagen.Dashboard.Web && dotnet run --urls "http://localhost:5100"
```
