# Guia da Aula 7 — Protocolos de Comunicação Web no ASP.NET Core

Este documento descreve o que foi implementado nesta branch (`aulas/protcom`) para cobrir o conteúdo da Aula 7: REST, gRPC, GraphQL, CQRS com MediatR e Observabilidade.

---

## Arquitetura implementada

```
REST   → CarController   \
gRPC   → CarGrpcService   >──→ IMediator → Handler → ICarsService → ICarsRepository → MongoDB
GraphQL → CarQuery/Mutation /
                                    ↑
                           (regra de negócio aqui)
                                    ↓
                         GET /metrics → Prometheus
```

**Regra central:** o transporte é adaptador. Toda lógica de negócio permanece nos handlers/serviços — os controllers e resolvers só despacham comandos e queries.

---

## 1. CQRS com MediatR

### Por que?
Desacopla o transporte (REST, gRPC, GraphQL) do core de negócio. Handlers ficam testáveis de forma independente do protocolo.

### O que foi implementado
| Arquivo | Tipo | Responsabilidade |
|---|---|---|
| `Services/CQRS/Queries/GetCarsQuery.cs` | Query | Busca todos os carros |
| `Services/CQRS/Queries/GetCarByIdQuery.cs` | Query | Busca carro por ID |
| `Services/CQRS/Commands/InsertCarCommand.cs` | Command | Insere novo carro |
| `Services/CQRS/Commands/UpdateCarCommand.cs` | Command | Atualiza carro existente |
| `Services/CQRS/Commands/DeleteCarCommand.cs` | Command | Remove carro |
| `Services/CQRS/Handlers/GetCarsQueryHandler.cs` | Handler | Executa GetCarsQuery via ICarsService |
| `Services/CQRS/Handlers/GetCarByIdQueryHandler.cs` | Handler | Executa GetCarByIdQuery via ICarsService |
| `Services/CQRS/Handlers/InsertCarCommandHandler.cs` | Handler | Executa InsertCarCommand via ICarsService |
| `Services/CQRS/Handlers/UpdateCarCommandHandler.cs` | Handler | Executa UpdateCarCommand via ICarsService |
| `Services/CQRS/Handlers/DeleteCarCommandHandler.cs` | Handler | Executa DeleteCarCommand via ICarsService |

### Como funciona
```csharp
// Antes (acoplado ao transporte):
var carros = await _carsService.GetCars();

// Depois (desacoplado via MediatR):
var carros = await _mediator.Send(new GetCarsQuery());
// → IMediator roteia para GetCarsQueryHandler.Handle()
// → Handler chama ICarsService.GetCars()
```

### Registro em Program.cs
```csharp
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GetCarsQueryHandler).Assembly));
```
O scan automático registra todos os `IRequestHandler<,>` do assembly Services.

---

## 2. REST (refatorado)

### O que mudou
`WebApi/Controllers/CarController.cs` — trocou `ICarsService` por `IMediator`.

```csharp
// Antes
public CarController(ICarsService carsService) { ... }
return Ok(await _carsService.GetCars());

// Depois
public CarController(IMediator mediator) { ... }
return Ok(await _mediator.Send(new GetCarsQuery()));
```

O contrato HTTP é idêntico — nenhum cliente REST precisou mudar.

### Endpoints disponíveis
| Método | Rota | Comportamento |
|---|---|---|
| `GET` | `/api/car` | Lista todos (requer JWT) |
| `GET` | `/api/car/{id}` | Busca por ID — 404 se não encontrar |
| `POST` | `/api/car` | Cria novo carro — 201 Created |
| `PUT` | `/api/car/{id}` | Atualiza — 404 se não encontrar |
| `DELETE` | `/api/car/{id}` | Remove — 204 No Content |

---

## 3. gRPC

### Por que?
Comunicação serviço-a-serviço de alta frequência: payload menor (Protobuf vs JSON), tipagem forte via contrato `.proto`, HTTP/2 nativo.

### Onde está o código
| Arquivo | Descrição |
|---|---|
| `WebApi/Protos/car.proto` | Contrato Protobuf — define serviço e mensagens |
| `WebApi/Grpc/CarGrpcService.cs` | Implementação do servidor gRPC |

### O contrato (car.proto)
```protobuf
service CarService {
  rpc GetCars    (GetCarsRequest)    returns (GetCarsResponse);
  rpc GetCarById (GetCarByIdRequest) returns (CarResponse);
  rpc InsertCar  (InsertCarRequest)  returns (InsertCarResponse);
  rpc UpdateCar  (UpdateCarRequest)  returns (UpdateCarResponse);
  rpc DeleteCar  (DeleteCarRequest)  returns (DeleteCarResponse);
}
```

### Como funciona (unary call)
```
gRPC client  →  CarGrpcService.GetCars()
                  → _mediator.Send(new GetCarsQuery())
                  → GetCarsQueryHandler
                  → ICarsService.GetCars()
                  → MongoDB
                  ← IEnumerable<CarModel>
                  ← GetCarsResponse { cars: [...] }
gRPC client  ←  GetCarsResponse
```

### Erros mapeados
```csharp
// NotFound em REST vira StatusCode.NotFound em gRPC:
if (car is null)
    throw new RpcException(new Status(StatusCode.NotFound, "Carro não encontrado"));
```

### Testando via grpcurl
```bash
# Listar métodos disponíveis
grpcurl -plaintext localhost:5266 list car.CarService

# Buscar todos os carros
grpcurl -plaintext localhost:5266 car.CarService/GetCars

# Buscar por ID
grpcurl -plaintext -d '{"id":"SEU_ID_AQUI"}' localhost:5266 car.CarService/GetCarById
```

### Configuração em Program.cs
```csharp
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
builder.Services.AddGrpc();
app.MapGrpcService<CarGrpcService>();
```

---

## 4. GraphQL (HotChocolate)

### Por que?
Front-end com múltiplas telas que precisam de formas diferentes de dados: o cliente declara exatamente os campos que quer — elimina overfetch e underfetch.

### Onde está o código
| Arquivo | Descrição |
|---|---|
| `WebApi/GraphQL/CarType.cs` | Tipo GraphQL para CarModel (evita conflito com atributos Bson) |
| `WebApi/GraphQL/CarQuery.cs` | Queries: `cars`, `carById` |
| `WebApi/GraphQL/CarMutation.cs` | Mutations: `insertCar`, `updateCar`, `deleteCar` |

### Exemplos de uso (POST /graphql)

**Query — buscar todos os campos:**
```json
{ "query": "{ cars { id name dateRelease } }" }
```

**Query — cliente pede só nome (evita overfetch):**
```json
{ "query": "{ cars { name } }" }
```

**Query — buscar por ID:**
```json
{
  "query": "query($id: String!) { carById(id: $id) { id name } }",
  "variables": { "id": "SEU_ID_AQUI" }
}
```

**Mutation — criar carro:**
```json
{
  "query": "mutation($name: String!, $dateRelease: DateTime!) { insertCar(name: $name, dateRelease: $dateRelease) }",
  "variables": { "name": "Polo GTI", "dateRelease": "2024-01-01T00:00:00Z" }
}
```

### IDE interativa
Acesse `GET /graphql` no browser — o Banana Cake Pop (IDE do HotChocolate) abre automaticamente com schema explorer e executor de queries.

### Configuração em Program.cs
```csharp
builder.Services
    .AddGraphQLServer()
    .AddQueryType<CarQuery>()
    .AddMutationType<CarMutation>()
    .AddType<CarType>();

app.MapGraphQL(); // /graphql
```

---

## 5. Observabilidade — OpenTelemetry + Prometheus

### Por que?
Sem telemetria, incidentes viram suposição. Com OTel registramos traces e métricas correlacionados. Com Prometheus coletamos séries temporais para SLO e alertas.

### Configuração em Program.cs
```csharp
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("volkswagen-dashboard-api"))
    .WithTracing(t => t
        .AddAspNetCoreInstrumentation()   // spans automáticos para HTTP
        .AddHttpClientInstrumentation())  // spans para HttpClient outbound
    .WithMetrics(m => m
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddPrometheusExporter());        // expõe /metrics

app.MapPrometheusScrapingEndpoint();      // GET /metrics
```

### Métricas expostas (exemplos)
```
# Duração das requisições HTTP (histograma)
http_server_request_duration_seconds_bucket{...}

# Requisições HTTP em andamento
http_server_active_requests{...}

# Taxa de erros por rota
http_server_request_duration_seconds_count{http_response_status_code="500",...}
```

### Acessando as métricas
```bash
curl http://localhost:5266/metrics
```

---

## 6. Testes

### Testes unitários (black-box do handler)
```
Tests/CQRS/GetCarsQueryHandlerTests.cs       — 2 casos
Tests/CQRS/GetCarByIdQueryHandlerTests.cs    — 2 casos
Tests/CQRS/InsertCarCommandHandlerTests.cs   — 1 caso
Tests/CQRS/DeleteCarCommandHandlerTests.cs   — 1 caso
```

**Padrão AAA:**
```csharp
// Arrange — setup do mock
_serviceMock.Setup(s => s.GetCars()).ReturnsAsync(expectedCars);

// Act — chama o handler diretamente
var result = await _handler.Handle(new GetCarsQuery(), CancellationToken.None);

// Assert — valida saída sem depender de detalhe interno
Assert.That(result.Count(), Is.EqualTo(2));
```

### Testes de integração (black-box do endpoint)
```
Tests/Integration/CarGrpcServiceTests.cs    — gRPC via WebApplicationFactory in-process
Tests/Integration/CarGraphQLTests.cs        — GraphQL via HttpClient in-process
```

`WebApplicationFactory<Program>` sobe toda a API na memória — sem porta de rede, sem Docker necessário para o teste.

### Rodando os testes
```bash
# Só testes unitários CQRS
dotnet test --filter "FullyQualifiedName~CQRS"

# Todos exceto integração (requer MongoDB)
dotnet test --filter "FullyQualifiedName!~Integration"

# Todos (requer MongoDB ativo)
dotnet test
```

---

## 7. Resumo de decisões arquiteturais

| Decisão | Justificativa |
|---|---|
| MediatR como hub central | Permite trocar/adicionar transporte sem reescrever regra de negócio |
| gRPC para serviço-a-serviço | Payload Protobuf < JSON, contrato forte via .proto, HTTP/2 |
| GraphQL para composição de telas | Cliente pede campos exatos — elimina overfetch/underfetch |
| REST mantido | Interoperabilidade com clientes externos e compatibilidade com Swagger |
| OTel + Prometheus | Observabilidade padronizada, endpoint /metrics para scrape |

---

## Pacotes adicionados nesta aula

| Projeto | Pacote | Versão |
|---|---|---|
| Services | MediatR | 12.4.1 |
| WebApi | MediatR | 12.4.1 |
| WebApi | Grpc.AspNetCore | 2.67.0 |
| WebApi | HotChocolate.AspNetCore | 13.9.14 |
| WebApi | OpenTelemetry.Extensions.Hosting | 1.10.0 |
| WebApi | OpenTelemetry.Instrumentation.AspNetCore | 1.10.1 |
| WebApi | OpenTelemetry.Instrumentation.Http | 1.10.0 |
| WebApi | OpenTelemetry.Exporter.Prometheus.AspNetCore | 1.15.0-beta.1 |
| Tests | Grpc.Net.Client | 2.67.0 |
| Tests | Microsoft.AspNetCore.Mvc.Testing | 9.0.0 |
