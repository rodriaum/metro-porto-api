# Metro Porto API

## Descrição
Metro Porto API (ASP.NET) serve para fornecer informações do https://www.metrodoporto.pt/
A API permite consultar horários, paradas, viagens e próximas partidas do metro.

## Docs

Você pode acessar os documentos da API por [aqui](https://metro-porto.gitbook.io/metro-porto).

## Funcionalidades

### Agências
Consulta de informações das agências do Metro do Porto.

### Calendários de Operação
Lista de datas e períodos em que os serviços estão ativos (ex: dias úteis, fins de semana, feriados).

### Datas de Calendário
Exceções ao calendário padrão (inclusões ou exclusões de datas específicas).

### Tarifas
- Atributos de tarifa (tipo, zona, valor)
- Regras de aplicação tarifária por rota e horário

### Rotas
Informações detalhadas sobre linhas do metro, incluindo nomes, cores e trajetos.

### Viagens
Lista de viagens por rota e calendário, com horários e veículos associados.

### Formas de Trajeto
Tipos de locomoção envolvidos (ex: metro, caminhada, transferência).

### Paragens (Stops)
Localização e identificação das estações disponíveis.

### Horários de Paragens
Lista de horários previstos e reais de chegada e partida em cada paragem.

### Transferências
Pontos e regras de transbordo entre linhas e rotas.

### Próximas Partidas
Estimativas em tempo das próximas partidas de uma determinada estação.
  
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
