# LocadoraVeiculosAPI

`CURSO: Análise e Desenvolvimento de Sistemas`
`DISCIPLINA: Tecnologias para Análise e Desenvolvimento de Sistemas`
`4º semestre / 2026`

Aluna: Geovanna Duarte
Professor: Ramon Lacerda Marques

## Sobre o projeto

O projeto LocadoraVeiculosAPI tem como objetivo desenvolver o backend de um sistema de aluguel de veículos utilizando C#, ASP.NET Core, Entity Framework Core, SQL Server Express e Swagger.

A proposta contempla desde a modelagem do banco de dados até a implementação das APIs RESTful, permitindo o gerenciamento de fabricantes, categorias de veículos, veículos, clientes e aluguéis. O projeto inclui validações de dados, relacionamentos entre entidades, regras de negócio (como o controle de disponibilidade dos veículos e o cálculo automático do valor da locação), persistência de dados e documentação completa das rotas via Swagger.

## Tecnologias utilizadas

* C#
* ASP.NET Core
* Entity Framework Core
* SQL Server Express
* Swagger
* Git
* GitHub

## Entidades principais

* Fabricante
* Categoria
* Veiculo
* Cliente
* Aluguel

## Estrutura do trabalho

**Etapa 1 - Modelagem do Banco de Dados**
Modelo conceitual do banco de dados, tradução para o esquema relacional via Entity Framework Core e definição de chaves primárias, estrangeiras e demais restrições de integridade. Documentos disponíveis em `documentacao/`.

**Etapa 2 - Implementação do Backend**
API RESTful desenvolvida em ASP.NET Core com Entity Framework Core, contendo endpoints de CRUD completo para todas as entidades, validação de dados de entrada, tratamento de erros e 5 filtros de consulta utilizando dois tipos de junção diferentes (LEFT JOIN via `Include()` e INNER JOIN via `join` explícito do LINQ).

**Etapa 3 - Testes e Documentação**
Integração do Swagger ao projeto, documentação de todos os endpoints (métodos HTTP, parâmetros e códigos de resposta) e relatório de testes manuais com evidências. Relatório completo disponível em `documentacao/Relatorio_Testes_Etapa3.pdf`.

**Etapa 4 - Apresentação em formato Pitch**
Vídeo demonstrativo do funcionamento do sistema, com cadastro de veículos e clientes, abertura e devolução de locações, e uso prático das APIs.

## Banco de dados

O projeto utiliza SQL Server Express com Entity Framework Core para persistência dos dados. As migrations estão disponíveis na pasta `Migrations/`.

## Como executar o projeto

1. Clone o repositório
2. Configure a connection string em `appsettings.json` apontando para sua instância do SQL Server Express
3. Execute `dotnet ef database update` para aplicar as migrations
4. Execute `dotnet run`
5. Acesse `http://localhost:5273/swagger` para testar as APIs


## Front-end (TP2)

1. `cd frontend`
2. `npm install`
3. `npm run dev` e acesse `http://localhost:5173` (a API precisa estar rodando em `http://localhost:5273`)

## Wireframes

https://www.figma.com/design/Pz6ScsLEmc0BYXTxuMihyh
## Wireframes

[Ver wireframes no Figma](https://www.figma.com/design/Pz6ScsLEmc0BYXTxuMihyh)