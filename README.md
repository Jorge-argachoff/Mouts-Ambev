# Developer Evaluation Project

## Como Começar

Siga estes passos para configurar os bancos de dados e executar a API localmente.

### Pré-requisitos

* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [Docker](https://www.docker.com/) e Docker Compose
* A ferramenta global `dotnet-ef`, usada para aplicar as migrations do banco de dados:

```bash
dotnet tool install --global dotnet-ef
```

### 1. Subir os containers de banco de dados

O arquivo `docker-compose.yml`, na raiz do repositório, provisiona os bancos de dados usados pela API:

* **PostgreSQL** (`ambev.developerevaluation.database`) — dados relacionais (Users, Sales, SaleItems)
* **MongoDB** (`ambev.developerevaluation.nosql`) — log de auditoria dos eventos de domínio das vendas (SaleEvents)

A partir da raiz do repositório, execute:

```bash
docker-compose up -d
```

Isso inicia o PostgreSQL da porta `5432` para `65224` e o MongoDB da porta `27017` para `65225`, usando as credenciais já configuradas em `src/Ambev.DeveloperEvaluation.WebApi/appsettings.json`.

>
### 2. Aplicar as migrations do banco de dados

Com o PostgreSQL em execução, aplique as migrations do EF Core para criar o schema:

```bash
cd src/Ambev.DeveloperEvaluation.WebApi
dotnet ef database update --project ../Ambev.DeveloperEvaluation.ORM --startup-project .
```

Isso cria as tabelas `Users`, `Sales` e `SaleItems`. O MongoDB não precisa de migration — a coleção `SaleEvents` é criada automaticamente na primeira inserção.

Se você alterar uma entidade e precisar criar uma nova migration, execute:

```bash
cd src/Ambev.DeveloperEvaluation.WebApi
dotnet ef migrations add <NomeDaMigration> --project ../Ambev.DeveloperEvaluation.ORM --startup-project .
```

### 3. Executar a API

```bash
cd src/Ambev.DeveloperEvaluation.WebApi
dotnet run
```

A API estará disponível com o Swagger UI habilitado no ambiente de Development.

### 4. Executar os testes

```bash
dotnet test
```

## Front-end

Esta API pode ser integrada ao front-end em Angular disponível no repositório:

[https://github.com/Jorge-argachoff/Mouts-Ambev-FrontEnd.git](https://github.com/Jorge-argachoff/Mouts-Ambev-FrontEnd.git)

Para utilizá-lo, clone o repositório, instale as dependências e execute o projeto:

```bash
git clone https://github.com/Jorge-argachoff/Mouts-Ambev-FrontEnd.git
cd Mouts-Ambev-FrontEnd
npm install
npm start
```

O front-end estará disponível em `http://localhost:4200` e já está configurado para consumir esta API em `https://localhost:7181/api`. Certifique-se de que a API esteja em execução (passo 3 acima) antes de utilizá-lo.
