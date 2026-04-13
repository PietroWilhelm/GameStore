# 🎮 GameStore  
## 👥 Integrantes do Grupo

Pietro Paranhos Wilhelm Rm561378

João Vitor Biribilli Ravelli Rm565594

Pedro de Matos Previtali Rm564184

## 🗂️ Entidades Modeladas
Content: Classe base que contém atributos comuns como Nome, Descrição e Data de Lançamento.

Game: Entidade especializada que herda de Content.
Studio: Representa a empresa desenvolvedora do jogo.

Category: Define os gêneros dos jogos (Ex: RPG, Ação, Terror).

Customer: Entidade principal do usuário, contendo dados cadastrais, CPF, e-mail e validações de idade.

CustomerConfiguration: Armazena preferências do usuário, como tema da interface.

## 🎯 Domínio Escolhido
O domínio selecionado para este projeto é uma Loja de Jogos (GameStore). O sistema foca no gerenciamento do catálogo de produtos (jogos), seus respectivos desenvolvedores (studios), categorização e a gestão de perfis de clientes com suas configurações personalizadas.


## 🔗 Resumo dos Relacionamentos

Studio 1 : N Game: Um Studio pode desenvolver múltiplos jogos, mas cada jogo é associado a um único Studio principal.

Game N : N Category: Um jogo pode pertencer a várias categorias, e uma categoria pode estar vinculada a diversos jogos (resolvido via tabela associativa).

Customer 1 : N Order: Um cliente pode realizar vários pedidos ao longo do tempo.

Game N : N : Um pedido pode conter vários jogos e um jogo pode estar em vários pedidos diferentes.

## 📋 Funcionamento do projeto e documentação
Os prints mostrando o funcionamento do projeto está em docs, contém imagens do migrations e a criação dentro do banco 
 
## 🔖 Banco utilizado
O banco que foi utilizado para o projeto foi OracleSql 

## 📍 Sequência de comandos para Migration
Rodar a partir da pasta onde vc baixou o projeto exemplo:

E:\2TDSPG\Developement with .net\GameStore\GameStore
 
1) Restaurar os pacotes

dotnet restore GameStore.sln

2) Compilar a solução

dotnet build GameStore.sln

3) Criar uma nova migration

Exemplo com o nome InitialCreate:

dotnet ef migrations add InitialCreate --project GameStore.Infrastructure --startup-project GameStore.Api

Se for outra migration, trocar o nome:

dotnet ef migrations add AddStudioRelation --project GameStore.Infrastructure --startup-project GameStore.Api
 
4) Listar as migrations existentes

dotnet ef migrations list --project GameStore.Infrastructure --startup-project GameStore.Api

5) Aplicar a migration no banco

dotnet ef database update --project GameStore.Infrastructure --startup-project GameStore.Api

# ❗ Observação importante no projeto

No estado atual do meu projeto, o Oracle já mostrou que ao gerar migration automaticamente ele pode criar tipos como:
NVARCHAR2
BOOLEAN
E no meu banco isso causou erro.
Então, depois de gerar a migration, a você precisa revisar o arquivo em GameStore.Infrastructure\Migrations\... e garantir que fique assim:

✅ NVARCHAR2 → VARCHAR2

✅ BOOLEAN → CHAR(1)

🔴 Senão o database update pode falhar.
