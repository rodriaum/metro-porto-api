# Metro Porto API

## Descrição
Metro Porto API (ASP.NET) serve para fornecer informações do https://www.metrodoporto.pt/
A API permite consultar horários, paradas, viagens e próximas partidas do metro.

## Funcionalidades

- Consulta de paradas de metro
- Obtenção de detalhes de viagens com suas paradas
- Consulta de próximas partidas a partir de uma parada específica
- Referência temporal para consultas de horários

## Tecnologias Utilizadas

- .NET 8.0
- ASP.NET Core Web API
- Entity Framework Core
- Injeção de Dependência
- Logging
- MongoDB

## Como Executar

1. Clone o repositório
2. Restaure os pacotes NuGet:
```bash
dotnet restore
```
3. Execute a aplicação:
```bash
dotnet run
```
*A API estará disponível em* https://localhost:5001 *ou* http://localhost:5000

### Licença
[MIT License](https://github.com/rodriaum/metro-porto-api?tab=MIT-1-ov-file#MIT-1-ov-file)
