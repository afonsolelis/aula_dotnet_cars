# Changelog - Aula 8

Data: 2026-03-04

## Objetivo da aula
- Mostrar testes de integracao modernos com infraestrutura real (Testcontainers).
- Demonstrar o conceito de VCR para testes de APIs externas (Correios/CEP) com replay deterministico.
- Introduzir pipeline GitLab CI com uma etapa de engenharia do caos.

## Entregas realizadas
- Remocao do changelog anterior da aula 7.
- Criacao deste novo changelog da aula 8.
- Novo teste com Testcontainers para subir MongoDB em container durante o teste.
- Novo teste estilo VCR para consulta de CEP (dados dos Correios), com gravacao/replay via cassette.
- Inclusao de `.gitlab-ci.yml` com stages de build, teste e caos.

## Destaques tecnicos
- Testcontainers:
  - Sobe um MongoDB efemero para validar `CarsRepository` com banco real.
  - O teste ignora automaticamente quando Docker nao estiver disponivel.
- VCR:
  - Usa um arquivo de cassette versionado para evitar dependencia de rede em toda execucao.
  - Permite regravar resposta real com `VCR_MODE=record`.
- CI com caos:
  - Pipeline inclui job de caos que pausa o MongoDB durante runtime da API.
  - Verifica recuperacao do endpoint GraphQL apos restaurar o banco.

## Como executar localmente
- Rodar todos os testes:
  - `dotnet test Volkswagen.Dashboard.Tests/Volkswagen.Dashboard.Tests.csproj`
- Regravar cassette do VCR:
  - `VCR_MODE=record dotnet test Volkswagen.Dashboard.Tests/Volkswagen.Dashboard.Tests.csproj --filter "FullyQualifiedName~CorreiosApiVcrTests"`

## Observacoes para aula
- O teste VCR em modo `replay` e ideal para CI: rapido e estavel.
- O teste com Testcontainers ensina "teste de integracao de verdade", mas requer Docker ativo.
- A etapa de caos foi desenhada para aprendizado: ela pode ser marcada como `allow_failure` sem bloquear merge.
