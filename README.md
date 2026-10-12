SWES


Tecnologias utilizadas

C#

ASP.NET Core 9

Entity Framework Core

ASP.NET Core Identity

SQL Server 2022

SQL Server Management Studio (SSMS)

Git e GitHub

Pré-requisitos

Para executar o projeto localmente, é necessário ter:

.NET SDK 9

SQL Server 2022 Express

Git

Visual Studio Code ou outra IDE compatível

O SQL Server Management Studio (SSMS) é recomendado para visualizar e administrar o banco de dados.

Configuração do projeto

1. Obter o código

Clone o repositório pelo Git ou baixe o projeto pelo GitHub.

2. Restaurar as dependências

Abra um terminal na pasta que contém o arquivo SWES.csproj e execute:

dotnet restore

3. Configurar a conexão com o banco

O projeto utiliza o banco de dados SWES_Dev.

Configure sua conexão local utilizando o Secret Manager do .NET:

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost\SQLEXPRESS2022;Database=SWES_Dev;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"

Atenção: substitua SQLEXPRESS2022 pelo nome da instância instalada no seu computador.

Essa configuração é individual e não precisa ser enviada ao GitHub.

4. Aplicar as migrations

Execute:

dotnet ef database update -- --environment Development

Esse comando cria ou atualiza a estrutura do banco de dados utilizando as migrations do projeto.

É necessário ter a ferramenta dotnet ef instalada.

5. Executar a aplicação

Execute:

dotnet run --environment Development

Abra no navegador o endereço informado pelo terminal.

Banco de dados

O projeto utiliza o SQL Server 2022 com Entity Framework Core.

O banco de desenvolvimento é chamado SWES_Dev.

A autenticação de usuários é gerenciada pelo ASP.NET Core Identity, e as alterações na estrutura do banco são controladas por migrations.

Desenvolvimento em equipe

O código-fonte é compartilhado pelo GitHub.

Cada integrante deve configurar sua própria conexão com o SQL Server 2022 e executar as migrations para preparar o ambiente local.

Não envie senhas, credenciais ou informações sensíveis para o repositório.

Hospedagem

Atualmente, o ambiente de desenvolvimento utiliza SQL Server 2022 local.

A possibilidade de hospedar um banco compartilhado para a equipe será avaliada futuramente.