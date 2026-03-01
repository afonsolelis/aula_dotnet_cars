# Changelog: Testcontainers + VCR + Chaos

Data: 2026-03-01

## Objetivo
Este pacote de testes cobre 3 frentes:

1. Integracao com banco real em container (`MongoDB` via `Testcontainers`).
2. Integracao com API externa gratuita (`ViaCEP`) com VCR semanal.
3. Engenharia do caos no banco (forcar indisponibilidade e validar recuperacao).
4. Erro de infraestrutura padronizado com circuit breaker no repositorio.

## Dependencias adicionadas
- Projeto: `Volkswagen.Dashboard.Tests`
- Pacote: `Testcontainers.MongoDb` (v3.9.0)

## Classe de API externa (CEP)
Arquivos:
- `Volkswagen.Dashboard.Services/Shipping/ICepLookupService.cs`
- `Volkswagen.Dashboard.Services/Shipping/CepAddress.cs`
- `Volkswagen.Dashboard.Services/Shipping/ViaCepLookupService.cs`

Como funciona:
- Normaliza CEP para 8 digitos.
- Chama `GET https://viacep.com.br/ws/{cep}/json/`.
- Mapeia para `CepAddress`.
- Retorna `null` quando o CEP nao existe (`erro=true`).

DI na API:
- `Volkswagen.Dashboard.WebApi/Program.cs`
- Registro: `AddHttpClient<ICepLookupService, ViaCepLookupService>` com `BaseAddress` ViaCEP.

## VCR semanal
Arquivos:
- `Volkswagen.Dashboard.Tests/Support/Vcr/WeeklyVcrHandler.cs`
- `Volkswagen.Dashboard.Tests/Cassettes/viacep-01001000.json`

Regra do VCR:
- Se o cassette existe e foi gravado na mesma semana ISO do `UtcNow`, responde do arquivo (sem chamada real).
- Se o cassette nao existe ou esta de semana anterior, chama API real e sobrescreve o arquivo.

Formato do cassette:
- `method`, `url`, `recordedAtUtc`, `statusCode`, `reasonPhrase`, `mediaType`, `content`.

## Testes criados/atualizados

### 1) Unitarios existentes (servico de carros)
Arquivo: `Volkswagen.Dashboard.Tests/TestOne.cs`
- `Should_GetCarsWithSuccess`
- `Should_GetCarByIdWithSuccess`

O que validam:
- Com `Moq`, garantem o comportamento do `CarsService` sem banco real.

### 2) Integracao com banco real (Testcontainers)
Arquivo: `Volkswagen.Dashboard.Tests/Integration/CarsRepositoryMongoContainerTests.cs`

- `Should_Insert_And_Get_Car_Using_Mongo_Testcontainer`
  - Sobe Mongo em container.
  - Insere carro via `CarsRepository`.
  - Busca por ID e valida persistencia.

- `Should_Update_Car_Using_Mongo_Testcontainer`
  - Insere registro inicial.
  - Atualiza nome.
  - Rebusca e confirma update.

### 3) Engenharia do caos (Testcontainers)
Arquivo: `Volkswagen.Dashboard.Tests/Integration/CarsRepositoryMongoContainerTests.cs`

- `Should_Open_CircuitBreaker_When_Mongo_Is_Unavailable_Chaos`
  - Cria repositorio com endpoint Mongo invalido para simular indisponibilidade.
  - Primeira chamada: valida erro de infraestrutura.
  - Segunda chamada imediata: valida erro de circuit breaker aberto (fail-fast).

- `Should_Recover_On_Healthy_Mongo_After_Chaos`
  - Usa Mongo real em Testcontainer.
  - Executa escrita e leitura apos o cenario de caos.
  - Valida recuperacao em ambiente saudavel.

Observacoes de execucao:
- Fixture marcada como `NonParallelizable` para evitar corrida entre testes que param/subem container.
- Se Docker nao estiver acessivel no host de teste, os testes desse fixture sao `Skipped` com motivo.

### 4) Integracao API externa com VCR semanal
Arquivo: `Volkswagen.Dashboard.Tests/Integration/ViaCepVcrTests.cs`

- `Should_Call_Real_ViaCep_And_Refresh_Cassette_When_Week_Is_Stale`
  - Prepara cassette antigo (semana anterior).
  - Faz 1 chamada real.
  - Atualiza `recordedAtUtc` no arquivo.
  - Garante refresh do cassette.

- `Should_Use_Weekly_Cassette_Without_Calling_Real_Api`
  - Prepara cassette da semana atual.
  - Injeta handler que falha se houver chamada externa.
  - Valida que a resposta vem do cassette.

## Resultado esperado na pratica
- Primeira execucao da semana: API externa e cassette atualizado.
- Execucoes seguintes na mesma semana: somente replay do cassette.
- Banco com Testcontainers: valida caminho feliz + resiliencia em falha/recuperacao.

## Erro correto e circuit breaker (repositorio)
Arquivos:
- `Volkswagen.Dashboard.Repository/RepositoryUnavailableException.cs`
- `Volkswagen.Dashboard.Repository/CarsRepository.cs`

Comportamento:
- Falhas transitórias de Mongo (`MongoConnectionException`, timeout etc.) viram `RepositoryUnavailableException`.
- Circuit breaker abre por 10 segundos apos falha transitória.
- Enquanto aberto, o repositorio retorna erro de circuito aberto sem tentar nova chamada no banco.

## Comando de execucao
```bash
dotnet test Volkswagen.Dashboard.WebApi.sln
```

## Comandos para rodar os testes

Todos os testes:
```bash
dotnet test Volkswagen.Dashboard.WebApi.sln
```

Somente testes de VCR (ViaCEP):
```bash
dotnet test Volkswagen.Dashboard.Tests/Volkswagen.Dashboard.Tests.csproj --filter ViaCepVcrTests
```

Somente testes com Mongo Testcontainers (inclui caos):
```bash
dotnet test Volkswagen.Dashboard.Tests/Volkswagen.Dashboard.Tests.csproj --filter CarsRepositoryMongoContainerTests
```

Somente testes de caos:
```bash
dotnet test Volkswagen.Dashboard.Tests/Volkswagen.Dashboard.Tests.csproj --filter "Should_Fail_When_Mongo_Is_Stopped_Chaos|Should_Recover_After_Mongo_Restart_Chaos"
```

Se houver erro de permissao no Docker socket, rode com `sudo`:
```bash
sudo -n dotnet test Volkswagen.Dashboard.Tests/Volkswagen.Dashboard.Tests.csproj --filter CarsRepositoryMongoContainerTests
```

## Nota de ambiente
Para rodar testes com Testcontainers sem `skip`, o usuario precisa permissao no Docker socket (`/var/run/docker.sock`) ou executar os testes com um contexto que tenha acesso ao daemon Docker.
