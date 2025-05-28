# Metro do Porto - API

Metro Porto API (ASP.NET) serve para fornecer informações do https://www.metrodoporto.pt/
A API permite consultar horários, paradas, viagens e próximas partidas do metro.

## Docs

Você pode acessar os documentos da API por [aqui](https://metro-porto.gitbook.io/metro-porto).

## Arquitetura Técnica

- **Framework**: ASP.NET Core (.NET 8.0)
- **Base de Dados**: MongoDB (armazenamento principal dos dados GTFS)
- **Cache**: Redis (otimização de performance)
- **Formato de Dados**: GTFS convertido e otimizado

## Categorias da API

### 🏢 Informações da Agência
Fornece informações sobre as agências de transporte que operam o sistema de metro, incluindo detalhes da agência, informações de contacto e dados operacionais.

### 📅 Calendários e Horários
Gere informações do calendário de serviços que define quando os serviços de metro operam. Isto inclui horários semanais regulares, exceções de feriados e datas de serviços especiais que afetam as operações normais.

### 🚇 Rotas e Linhas
Contém informações sobre rotas e linhas do metro, incluindo nomes das rotas, cores, descrições e dados de viagens associadas. Permite acesso a informações completas da rota com todas as viagens relacionadas.

### 🚉 Paragens e Estações
Fornece dados abrangentes sobre paragens e estações do metro, incluindo coordenadas de localização, nomes, códigos e informações de partida em tempo real. Essencial para encontrar estações próximas e obter horários de partida ao vivo.

### 🚊 Viagens e Percursos
Gere dados de viagens individuais que representam percursos específicos do metro ao longo das rotas. Inclui horários de viagens, destinos e informações detalhadas de tempo paragem a paragem.

### ⏰ Horários de Paragens
Trata informações detalhadas de horários sobre quando os metros chegam e partem de cada paragem. Suporta paginação para grandes conjuntos de dados e fornece dados de tempo específicos da viagem.

### 🗺️ Formas Geográficas
Contém dados de coordenadas geográficas que definem os caminhos físicos das rotas do metro nos mapas. Essencial para exibir linhas de rota e rastrear movimentos do metro geograficamente.

### 🔄 Transferências e Ligações
Gere pontos de transferência entre diferentes linhas e rotas do metro, ajudando os utilizadores a planear viagens multi-linha e compreender possibilidades de ligação em toda a rede.

### 💰 Informações Tarifárias
Fornece informações de preços e regras tarifárias para viagens de metro, incluindo diferentes tipos de tarifas, métodos de pagamento, políticas de transferência e estruturas de preços baseadas em zonas.

### ℹ️ Informações do Sistema e Gestão
Oferece informações de estado do sistema, detalhes da versão da API e funções administrativas como recarregamento de dados. Essencial para monitorizar a saúde da API e gerir atualizações de dados.

## Funcionalidades Principais

- **Partidas em Tempo Real**: Obtenha informações de partida ao vivo para qualquer paragem do metro
- **Planeamento de Rotas**: Aceda a informações completas de rotas e viagens para planeamento de percursos
- **Dados Geográficos**: Recupere dados de coordenadas para mapeamento e serviços de localização
- **Informações de Horários**: Aceda a horários detalhados e calendários de serviços
- **Cálculo de Tarifas**: Obtenha informações de preços e regras tarifárias
- **Informações de Transferência**: Encontre pontos de ligação entre diferentes linhas

## Processamento de Dados

A API processa dados GTFS originais e converte-os para um formato otimizado armazenado no MongoDB. O Redis é utilizado para cache de consultas frequentes, garantindo tempos de resposta rápidos. O sistema inclui funcionalidades de recarregamento de dados para atualizações periódicas das informações de trânsito.

## Padrões de Dados

Esta API segue os padrões GTFS (General Transit Feed Specification), garantindo compatibilidade com aplicações de trânsito e fornecendo formatos de dados padronizados para informações de transporte público.

## Casos de Uso

- **Aplicações Móveis de Trânsito**: Construa aplicações que mostram partidas em tempo real e planeamento de rotas
- **Serviços de Mapeamento Web**: Exiba rotas e paragens do metro em mapas interativos
- **Planeadores de Viagem**: Crie ferramentas de planeamento de viagens com informações de transferência
- **Calculadoras de Tarifas**: Desenvolva ferramentas de estimativa de tarifas para viagens de metro
- **Análise de Dados**: Analise padrões de uso do metro e performance do serviço

## Informações Técnicas

- **URL Base**: `/v1/porto/metro`
- **Formatos Suportados**: JSON, Texto Simples
- **Autenticação**: Não requerida para endpoints públicos
- **Rate Limiting**: Recomenda-se uso responsável com implementação de cache local

## Licença
[MIT License](https://github.com/rodriaum/metro-porto-api?tab=MIT-1-ov-file#MIT-1-ov-file)