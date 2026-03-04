# Changelog - Aula 8

Data: 2026-03-04

## Resumo da aula
Nesta aula, o foco foi em qualidade de software com testes de integracao reais e automacao de pipeline:
- Testcontainers para subir dependencias em container durante teste.
- VCR para gravar e reproduzir respostas de APIs externas.
- CI no GitHub (com o mesmo conceito aplicavel ao GitLab).

## O que e Testcontainers
Testcontainers e uma biblioteca que permite criar containers Docker diretamente pelos testes.

Na pratica:
- O teste sobe um container (ex.: MongoDB).
- Executa o cenario usando esse banco real.
- No fim, derruba o container automaticamente.

Vantagens:
- Ambiente de teste mais proximo de producao.
- Menos mock para infraestrutura critica.
- Menos "funciona na minha maquina" porque o setup fica no proprio teste.

Ponto de atencao:
- Precisa de Docker disponivel no ambiente onde os testes rodam.

## O que e VCR
VCR e um padrao para testes com API externa.

Como funciona:
- Primeira execucao (modo `record`): chama a API real e grava a resposta em um arquivo (cassette).
- Proximas execucoes (modo `replay`): nao chama internet; le a resposta gravada.

Vantagens:
- Testes mais rapidos.
- Testes deterministicos (menos instabilidade de rede/API externa).
- Menos risco de falha por indisponibilidade de terceiros.

Exemplo da aula:
- Consulta de CEP com servico dos Correios (via endpoint externo).
- Resposta persistida em `Integration/Cassettes/*.json`.

## Como funciona o CI (GitHub e GitLab)
O conceito e igual nas duas plataformas: executar um pipeline automatico a cada push/merge request.

Fluxo padrao:
1. `restore`: baixar dependencias.
2. `build`: compilar a solucao.
3. `test`: executar testes (unitarios e integracao conforme estrategia).
4. `chaos` (opcional): simular falhas controladas para validar resiliencia.

### GitHub Actions
No GitHub, isso fica em `.github/workflows/*.yml`.

### GitLab CI
No GitLab, isso fica em `.gitlab-ci.yml`.

O YAML muda de sintaxe, mas as ideias sao as mesmas:
- Stages/jobs
- Variaveis de ambiente
- Artefatos de teste
- Regras de execucao

## Engenharia do caos no pipeline
A etapa de caos testa comportamento em falha real:
- Derruba/pausa uma dependencia (ex.: MongoDB).
- Observa se a aplicacao se recupera quando a dependencia volta.

Para aula, essa etapa pode ser `allow_failure`:
- nao bloqueia merge
- gera aprendizado sobre resiliencia sem travar entrega

## Comandos uteis da aula
- Rodar testes:
  - `dotnet test Volkswagen.Dashboard.Tests/Volkswagen.Dashboard.Tests.csproj`
- Rodar apenas o teste de Testcontainers (MongoDB):
  - `dotnet test Volkswagen.Dashboard.Tests/Volkswagen.Dashboard.Tests.csproj --filter "FullyQualifiedName~CarsRepositoryTestcontainersTests.InsertAndGetById_ShouldPersistUsingRealMongoContainer"`
- Regravar cassete VCR:
  - `VCR_MODE=record dotnet test Volkswagen.Dashboard.Tests/Volkswagen.Dashboard.Tests.csproj --filter "FullyQualifiedName~CorreiosApiVcrTests"`
