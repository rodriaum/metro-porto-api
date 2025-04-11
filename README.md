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

## Estrutura do Projeto

O projeto segue uma arquitetura em camadas:
- **API**: Controladores e endpoints da aplicação
- **Services**: Lógica de negócios
- **Models/DTOs**: Objetos de transferência de dados
- **Repositories**: Acesso a dados

## Endpoints Principais (Incompleto)

### Estações
```bash
GET /v1/porto/metro/stops/
```
```bash
GET /v1/porto/metro/stops/{stopId}
```
*Retorna todas as estações de metro incluindo o id, nome, etc.*

### Chegada e Saída de Metros
```bash
/v1/porto/metro/stop-times/
```
```bash
/v1/porto/metro/stop-times/stop/{stopId}
```
```bash
/v1/porto/metro/stop-times/trip/{tripId}
```

*Retorna as próximas partidas a partir de uma parada específica.*



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