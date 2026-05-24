# 🍔 Good Hamburger

Sistema Full Stack para gerenciamento de pedidos, desenvolvido com **ASP.NET Core Web API** e **React**.

O projeto possui autenticação JWT, controle de acesso por perfil (RBAC), aplicação de regras de negócio complexas para composição de pedidos e é totalmente estruturado com base nos princípios de **Clean Architecture**, **Domain-Driven Design (DDD)** e **SOLID**.

---

## 🎯 Sobre o Projeto

O sistema simula um ambiente corporativo real e foi desenvolvido com foco em:

- APIs RESTful bem documentadas e desacopladas.
- Arquitetura em camadas com forte separação de responsabilidades.
- Regras de negócio centralizadas no domínio.
- Conteinerização de toda a infraestrutura para facilitar o provisionamento.

> **Evolução Técnica:** Inicialmente, o projeto utilizava Blazor WebAssembly no front-end. Posteriormente, foi refatorado para **React + TypeScript**, visando maior flexibilidade arquitetural, otimização do fluxo de desenvolvimento e melhor aderência aos ecossistemas front-end modernos.

---

## 🚀 Tecnologias Utilizadas

### Back-end

- C# / .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server 2022
- Autenticação JWT
- Automação de Migrations

### Front-end

- React
- TypeScript
- React Router
- Context API

### Infraestrutura

- Docker
- Docker Compose

---

## 🧱 Arquitetura e Estrutura

O código-fonte está dividido para garantir o isolamento do domínio e a inversão de dependência:

```text
/src
  /GoodHamburger.WebAPI        # Ponto de entrada, Controllers, Middlewares e Configurações
  /GoodHamburger.Application   # Casos de uso (AppServices) e DTOs de entrada/saída
  /GoodHamburger.Domain        # Entidades, Agregados, Interfaces de Repositório e Regras de Negócio
  /GoodHamburger.Infra         # Implementação de Repositórios, EF Core DbContext e Serviços Externos
  /GoodHamburger.Shared        # Validações (FluentValidation) e utilitários compartilhados
  /good-hamburger-web          # Aplicação Front-end (React)

/tests                         # Testes automatizados do projeto
```

---

## 🐳 Como Executar (Docker)

A aplicação está totalmente conteinerizada. Não é necessário ter o .NET, Node.js ou SQL Server instalados fisicamente na máquina, apenas o **Docker**.

### 1. Clone o repositório

```bash
git clone https://github.com/esdraslimaff/Order
```

### 2. Execute o projeto

Na raiz do projeto, execute:

```bash
docker compose up --build
```

### 📌 Notas de Inicialização

- A API executará automaticamente as *migrations* no banco de dados SQL Server durante a inicialização do contêiner.
- Aguarde a mensagem `Application started` aparecer nos logs do terminal.

### 🌐 Acessos

- **Front-end (React):** http://localhost:5000
- **Back-end (Swagger API):** http://localhost:8080/swagger

---

## ⚙️ Rodar Localmente (Sem Docker)

Caso prefira rodar via CLI convencional (requer .NET SDK 8 e Node.js 20+):

### 1. Back-end

```bash
dotnet restore
dotnet build
dotnet run --project src/GoodHamburger.WebAPI
```

### 2. Front-end

```bash
cd src/good-hamburger-web

npm install
npm run dev
```

---

## 📡 Funcionalidades e Regras de Negócio

### 🔐 Autenticação e Segurança

- Login com emissão de token JWT.
- Controle de acesso baseado em Roles (RBAC):
  - Administrador
  - Atendente

### 🧾 Gestão de Pedidos

- CRUD completo de pedidos com controle de permissões.
- Listagem pública do cardápio de produtos.

### 🧠 Lógica de Promoções Automáticas

- **20% de desconto** na compra conjunta de:
  - Sanduíche + Batata + Refrigerante

- **15% de desconto** na compra conjunta de:
  - Sanduíche + Refrigerante

- **10% de desconto** na compra conjunta de:
  - Sanduíche + Batata

### ⚠️ Restrições de Domínio

- Permitido apenas **1 item de cada tipo por pedido**.
- Tentativas de inserir itens duplicados no mesmo pedido retornam falha de validação da regra de negócio.

---

## 🧪 Executar Testes

Para rodar a suíte de testes automatizados da aplicação:

```bash
dotnet test
```

---

## 🔧 Possíveis Melhorias Futuras

- Implementação de mensageria (ex: RabbitMQ) para processamento assíncrono de pedidos.
- Adição de cache distribuído (Redis) para a listagem do cardápio.
- Configuração de pipeline CI/CD via GitHub Actions.
- Adição de telemetria e observabilidade (OpenTelemetry / Serilog).

---

## 👨‍💻 Autor

**Esdras Lima**

Desenvolvedor Full Stack com foco em Back-end (.NET / C#)

- GitHub: github.com/esdraslimaff
- LinkedIn: linkedin.com/in/esdrasdev