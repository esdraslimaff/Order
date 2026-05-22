# 🍔 Good Hamburger

Sistema Full Stack para gerenciamento de pedidos, desenvolvido com ASP.NET Core Web API e React.

O projeto possui autenticação JWT, controle de acesso por perfil (RBAC), regras de negócio para pedidos e arquitetura baseada em Clean Architecture e DDD.

---

# 🎯 Contexto do Projeto

O sistema foi desenvolvido com foco em:

* APIs REST
* Arquitetura em camadas
* Separação de responsabilidades
* Regras de negócio centralizadas
* Simulação de ambiente corporativo

Inicialmente o projeto utilizava Blazor WebAssembly no front-end. Posteriormente, foi evoluído para React + TypeScript visando maior flexibilidade, melhor experiência prática com ecossistemas front-end modernos e otimização do fluxo de desenvolvimento.

---

# 🧱 Arquitetura

```txt
/src
  /GoodHamburger.WebAPI        # API ASP.NET Core
  /GoodHamburger.Application   # Casos de uso
  /GoodHamburger.Domain        # Regras de negócio
  /GoodHamburger.Infra         # Acesso a dados
  /GoodHamburger.Shared        # DTOs
  /good-hamburger-web          # Frontend React
/tests
```

---

# 🚀 Stack Utilizada

## Back-end

* ASP.NET Core Web API
* Entity Framework Core
* PostgreSQL
* JWT Authentication
* DDD
* Clean Architecture
* Docker

## Front-end

* React
* TypeScript
* React Router
* Context API

---

# 🌐 Aplicação Online

🔗 Front-end:
https://order-2gz.pages.dev

---

# 🐳 Executar com Docker

## Pré-requisitos

* Docker

## Subir aplicação

```bash
docker compose up --build -d
```

---

# 📡 Funcionalidades

## 🔐 Autenticação

* Login com JWT
* Controle de acesso por perfil
* RBAC (Administrador e Atendente)

## 🧾 Pedidos

* Criar pedidos
* Atualizar pedidos
* Remover pedidos
* Controle de permissões
* Aplicação automática de promoções

## 📖 Cardápio

* Listagem pública de produtos

## 👥 Usuários

* Cadastro de usuários
* Controle de acesso

---

# 🧠 Regras de Negócio

Promoções automáticas:

* Sanduíche + Batata + Refrigerante → 20%
* Sanduíche + Refrigerante → 15%
* Sanduíche + Batata → 10%

Restrições:

* Máx. 1 item por tipo
* Itens duplicados retornam erro

---

# 🧪 Executar Testes

```bash
dotnet test
```

---

# ⚙️ Rodar sem Docker

```bash
dotnet restore
dotnet build
dotnet run --project src/GoodHamburger.WebAPI
```

---

# 🔧 Possíveis Melhorias

* Integração com mensageria
* Observabilidade
* Cache distribuído
* Testes de integração
* CI/CD automatizado

---

# 👨‍💻 Autor

Esdras Lima

🔗 LinkedIn:
https://linkedin.com/in/esdrasdev
