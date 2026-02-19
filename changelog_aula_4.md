# Changelog Aula 4 - Padrões, AAA e TDD

Data: 19/02/2026  
Fonte da aula: https://afonsolelis.github.io/aulas/pages/module-9-sistemas-informacao/lesson-4.html

## 1) O que a aula 4 cobre

A aula enfatiza:
- OO com foco em comportamento (não modelo anêmico).
- Encapsulamento real, Tell Don't Ask e Lei de Demeter.
- Herança vs Composição (preferir composição).
- Polimorfismo e DIP (programar para interfaces).
- TDD como filosofia de design.
- Ciclo Red-Green-Refactor.
- Anti-patterns de testes.
- Estrutura de repositório com separação de responsabilidades.
- Exemplo AAA (Arrange, Act, Assert) no workshop.

## 2) Padrões e práticas encontrados no projeto atual

### 2.1 Arquitetura em camadas (Controller -> Service -> Repository)
- Evidência:
  - `Volkswagen.Dashboard.WebApi/Controllers/CarController.cs`
  - `Volkswagen.Dashboard.Services/Cars/CarsService.cs`
  - `Volkswagen.Dashboard.Repository/CarsRepository.cs`
- Resultado: separa API, regra de negócio e acesso a dados.

### 2.2 Repository Pattern
- Evidência:
  - `Volkswagen.Dashboard.Repository/ICarsRepository.cs`
  - `Volkswagen.Dashboard.Repository/CarsRepository.cs`
  - `Volkswagen.Dashboard.Repository/IUserRepository.cs`
  - `Volkswagen.Dashboard.Repository/UserRepository.cs`
- Resultado: abstrai persistência (Mongo) do domínio/serviços.

### 2.3 Service Layer
- Evidência:
  - `Volkswagen.Dashboard.Services/Cars/ICarsService.cs`
  - `Volkswagen.Dashboard.Services/Cars/CarsService.cs`
  - `Volkswagen.Dashboard.Services/Auth/IAuthService.cs`
  - `Volkswagen.Dashboard.Services/Auth/AuthService.cs`
- Resultado: concentra regras de negócio e orquestração.

### 2.4 Dependency Injection + DIP
- Evidência:
  - `Volkswagen.Dashboard.WebApi/Program.cs` (registrações `AddScoped`, `AddSingleton`)
  - Uso de interfaces em controllers/serviços/repositórios.
- Resultado: baixo acoplamento, melhor testabilidade, troca de implementações.

### 2.5 API pattern (Controllers + IActionResult)
- Evidência:
  - `Volkswagen.Dashboard.WebApi/Controllers/CarController.cs`
  - `Volkswagen.Dashboard.WebApi/Controllers/UserController.cs`
- Resultado: endpoints REST com respostas HTTP sem vazar detalhes de dados.

### 2.6 Testes unitários com mocking
- Evidência:
  - `Volkswagen.Dashboard.Tests/Volkswagen.Dashboard.Tests.csproj` (`NUnit`, `Moq`, `coverlet`)
  - `Volkswagen.Dashboard.Tests/TestOne.cs`
- Resultado: testes de serviço sem bater em banco (mock de repositório).

## 3) O que da aula ainda não está forte no projeto (e como criar)

### 3.1 AAA explícito em todos os testes
- Estado atual: os testes seguem a ideia de AAA, mas sem padrão visual consistente em todos.
- Criar: padronizar todos os testes com blocos claros `// Arrange`, `// Act`, `// Assert`.

### 3.2 TDD estrito (Red -> Green -> Refactor)
- Estado atual: existe projeto de testes, mas o fluxo de desenvolvimento por teste não está formalizado.
- Criar: adotar rotina obrigatória por issue/feature:
  1. Escrever teste que falha.
  2. Implementar mínimo para passar.
  3. Refatorar com testes verdes.

### 3.3 Encapsulamento mais forte em entidades de domínio
- Estado atual: modelos são majoritariamente estruturas de dados.
- Criar: mover invariantes/regras para métodos da entidade quando fizer sentido.

## 4) Passo a passo: criar (ou recriar) projeto de testes

Observação: neste repo, `Volkswagen.Dashboard.Tests` já existe. O fluxo abaixo serve para criar do zero em outro módulo/turma.

### 4.1 Criar projeto de testes
```bash
dotnet new nunit -n Volkswagen.Dashboard.Tests
```

### 4.2 Adicionar o projeto na solução
```bash
dotnet sln Volkswagen.Dashboard.WebApi.sln add Volkswagen.Dashboard.Tests/Volkswagen.Dashboard.Tests.csproj
```

### 4.3 Referenciar projetos a serem testados
```bash
dotnet add Volkswagen.Dashboard.Tests/Volkswagen.Dashboard.Tests.csproj reference \
  Volkswagen.Dashboard.Services/Volkswagen.Dashboard.Services.csproj \
  Volkswagen.Dashboard.Repository/Volkswagen.Dashboard.Repository.csproj
```

### 4.4 Instalar bibliotecas de teste
```bash
dotnet add Volkswagen.Dashboard.Tests/Volkswagen.Dashboard.Tests.csproj package Moq
dotnet add Volkswagen.Dashboard.Tests/Volkswagen.Dashboard.Tests.csproj package coverlet.collector
```

### 4.5 Restaurar e executar
```bash
dotnet restore Volkswagen.Dashboard.WebApi.sln
dotnet test Volkswagen.Dashboard.WebApi.sln
```

## 5) Como rodar `dotnet test` com cobertura de código

### 5.1 Cobertura básica (XPlat Code Coverage)
```bash
dotnet test Volkswagen.Dashboard.Tests/Volkswagen.Dashboard.Tests.csproj --collect:"XPlat Code Coverage"
```

### 5.2 Cobertura focada na camada de Services
```bash
dotnet test Volkswagen.Dashboard.Tests/Volkswagen.Dashboard.Tests.csproj \
  --collect:"XPlat Code Coverage" \
  --results-directory ./TestResults \
  -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Include="[Volkswagen.Dashboard.Services]*"
```

### 5.3 Meta prática para “cobrir o service todo”
- Cobrir **todos os métodos públicos** de `CarsService` e `AuthService`.
- Cobrir **todos os ramos** de decisão (`if/else`, validações e exceções).
- Cobrir cenários de sucesso e falha com mocks (`Moq`) dos repositórios.

Exemplos de cenários que precisam existir:
- `CarsService.InsertCar` com `Id` vazio (insere).
- `CarsService.InsertCar` com `Id` preenchido (atualiza).
- `CarsService.GetCarById` retornando item e retornando `null`.
- `AuthService.Login` com usuário inexistente.
- `AuthService.Login` com senha inválida.
- `AuthService.Login` com sucesso (token gerado).
- `AuthService.Register` com dados inválidos.
- `AuthService.Register` com email fora da whitelist.
- `AuthService.Register` com usuário já existente.
- `AuthService.Register` com sucesso.

### 5.4 Gerar relatório HTML com ReportGenerator

Instalar ferramenta global (uma vez):
```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```

Se o comando `reportgenerator` não for encontrado ou reclamar de runtime, configure `PATH` e `DOTNET_ROOT` no shell:

Linux (bash):
```bash
echo 'export DOTNET_ROOT="/home/afonsolelis/dotnet"' >> ~/.bashrc
echo 'export DOTNET_ROOT_X64="/home/afonsolelis/dotnet"' >> ~/.bashrc
echo 'export PATH="$PATH:/home/afonsolelis/.dotnet/tools"' >> ~/.bashrc
source ~/.bashrc
```

Linux (zsh):
```bash
echo 'export DOTNET_ROOT="/home/afonsolelis/dotnet"' >> ~/.zshrc
echo 'export DOTNET_ROOT_X64="/home/afonsolelis/dotnet"' >> ~/.zshrc
echo 'export PATH="$PATH:/home/afonsolelis/.dotnet/tools"' >> ~/.zshrc
source ~/.zshrc
```

PowerShell (Windows):
```powershell
[Environment]::SetEnvironmentVariable("DOTNET_ROOT", "$env:USERPROFILE\\dotnet", "User")
[Environment]::SetEnvironmentVariable("DOTNET_ROOT_X64", "$env:USERPROFILE\\dotnet", "User")
[Environment]::SetEnvironmentVariable("PATH", $env:PATH + ";$env:USERPROFILE\\.dotnet\\tools", "User")
```

PowerShell (sessão atual):
```powershell
$env:DOTNET_ROOT = "$HOME\\dotnet"
$env:DOTNET_ROOT_X64 = "$HOME\\dotnet"
$env:PATH += ";$HOME\\.dotnet\\tools"
```

Gerar relatório HTML a partir do XML do `dotnet test`:
```bash
reportgenerator \
  -reports:"TestResults/**/coverage.cobertura.xml" \
  -targetdir:"TestResults/CoverageReport" \
  -reporttypes:"Html;TextSummary"
```

Leitura prática:
- Resumo no terminal: `cat TestResults/CoverageReport/Summary.txt`
- Relatório detalhado: abrir `TestResults/CoverageReport/index.html`

Fluxo completo (teste + HTML):
```bash
dotnet test Volkswagen.Dashboard.Tests/Volkswagen.Dashboard.Tests.csproj --collect:"XPlat Code Coverage"
reportgenerator \
  -reports:"TestResults/**/coverage.cobertura.xml" \
  -targetdir:"TestResults/CoverageReport" \
  -reporttypes:"Html;TextSummary"
```

### 5.5 Troubleshooting real (dificuldades comuns da turma)

Problema 1: `zsh: command not found: reportgenerator`  
Causa: tool instalada, mas pasta `~/.dotnet/tools` fora do `PATH`.

Solução (Linux zsh):
```bash
echo 'export PATH="$PATH:/home/afonsolelis/.dotnet/tools"' >> ~/.zshrc
source ~/.zshrc
```

Solução (Linux bash):
```bash
echo 'export PATH="$PATH:/home/afonsolelis/.dotnet/tools"' >> ~/.bashrc
source ~/.bashrc
```

Problema 2: `You must install .NET to run this application` / `libhostfxr.so not found`  
Causa: `dotnet` está em diretório customizado e a tool global não encontra o runtime.

Solução (Linux zsh):
```bash
echo 'export DOTNET_ROOT="/home/afonsolelis/dotnet"' >> ~/.zshrc
echo 'export DOTNET_ROOT_X64="/home/afonsolelis/dotnet"' >> ~/.zshrc
source ~/.zshrc
```

Solução (Linux bash):
```bash
echo 'export DOTNET_ROOT="/home/afonsolelis/dotnet"' >> ~/.bashrc
echo 'export DOTNET_ROOT_X64="/home/afonsolelis/dotnet"' >> ~/.bashrc
source ~/.bashrc
```

Solução (PowerShell - sessão atual):
```powershell
$env:DOTNET_ROOT = "$HOME\\dotnet"
$env:DOTNET_ROOT_X64 = "$HOME\\dotnet"
$env:PATH += ";$HOME\\.dotnet\\tools"
```

Problema 3: `The report file pattern ... found no matching files`  
Causa: o XML foi gerado em outro caminho (ex.: dentro da pasta do projeto de testes).

Como localizar o XML:
```bash
find . -type f -name "coverage.cobertura.xml"
```

No nosso caso, o XML ficou em:
`Volkswagen.Dashboard.Tests/TestResults/<guid>/coverage.cobertura.xml`

Comando correto na raiz do repositório:
```bash
reportgenerator \
  -reports:"Volkswagen.Dashboard.Tests/TestResults/**/coverage.cobertura.xml" \
  -targetdir:"Volkswagen.Dashboard.Tests/TestResults/CoverageReport" \
  -reporttypes:"Html;TextSummary"
```

Problema 4: rodei `reportgenerator` antes de gerar cobertura  
Causa: faltou executar o `dotnet test` com coleta de cobertura.

Solução:
```bash
dotnet test Volkswagen.Dashboard.Tests/Volkswagen.Dashboard.Tests.csproj --collect:"XPlat Code Coverage"
```

## 6) Como escrever testes no padrão AAA

Modelo:
```csharp
[Test]
public async Task InsertCar_WithId_DeveAtualizar()
{
    // Arrange
    var repo = new Mock<ICarsRepository>();
    var service = new CarsService(repo.Object);
    var car = new CarModel { Id = "65f0d5934f4f35f8d2cd1001", Name = "Fox" };
    repo.Setup(r => r.UpdateCar(car)).ReturnsAsync(car.Id);

    // Act
    var result = await service.InsertCar(car);

    // Assert
    Assert.That(result, Is.EqualTo(car.Id));
    repo.Verify(r => r.UpdateCar(car), Times.Once);
    repo.Verify(r => r.InsertCar(It.IsAny<CarModel>()), Times.Never);
}
```

Checklist AAA:
- Arrange: prepara dados, mocks, SUT.
- Act: executa uma única ação principal.
- Assert: valida resultado e colaborações relevantes.

## 7) Como aplicar TDD no padrão (com AAA)

### 7.1 RED
1. Escolha um comportamento pequeno (ex.: “não atualizar se Id inválido”).
2. Escreva um teste AAA para esse comportamento.
3. Rode `dotnet test` e confirme falha.

### 7.2 GREEN
1. Implemente o mínimo para passar.
2. Sem otimizar cedo; foco em verde rápido.
3. Rode `dotnet test` e confirme sucesso.

### 7.3 REFACTOR
1. Limpe duplicações, nomes e extrações.
2. Mantenha sem alterar comportamento.
3. Rode `dotnet test` novamente.

## 8) Anti-patterns para evitar (aula + projeto)

- Teste mentiroso: `Assert.True(true)` sem valor real.
- Teste acoplado a detalhes internos (`private/internal`).
- Setup gigante: sinal de design muito acoplado.
- Teste lento que depende de rede/banco em teste unitário.
- Falta de isolamento: não usar mock quando deveria.
- Teste com múltiplos comportamentos no mesmo método.

## 9) Conceitos-chave resumidos

- TDD não é “testar depois”; é dirigir o design antes da implementação.
- AAA é a estrutura mínima para teste legível e previsível.
- DIP + interfaces facilitam mocking e reduzem acoplamento.
- Composição tende a gerar código mais flexível e testável que herança.

## 10) Próxima evolução recomendada no repo

1. Padronizar nomes de testes: `Metodo_Cenario_ResultadoEsperado`.
2. Tornar testes assíncronos com `async/await` (evitar `GetAwaiter().GetResult()`).
3. Adicionar testes para fluxos de erro no `CarsService` e `AuthService`.
4. Criar pipeline CI para rodar `dotnet test` automaticamente.

## 11) Importância de um `.gitignore` completo (e por que não subir `bin/`)

Um `.gitignore` incompleto causa poluição no repositório e dificulta muito o trabalho em equipe.

Por que **não** versionar `bin/`, `obj/` e `TestResults/`:
- São artefatos gerados automaticamente pelo build/teste.
- Mudam entre máquinas, sistema operacional e versão do SDK.
- Geram diffs enormes e ruído em pull requests.
- Podem criar conflitos de merge desnecessários.
- Aumentam o tamanho do repositório sem agregar código-fonte.
- Podem mascarar problemas reais (parece “mudança de código”, mas é só build).

Regra prática para turma:
- Commitar apenas código-fonte, configuração e documentação.
- Nunca commitar saída de compilação (`bin/`, `obj/`) nem cobertura (`TestResults/`, `CoverageReport/`).

Regras recomendadas no `.gitignore`:
```gitignore
**/bin/
**/obj/
**/TestResults/
**/CoverageReport/
coverage*.xml
*.coverage
*.coveragexml
```
