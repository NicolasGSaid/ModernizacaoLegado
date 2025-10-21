# 🏗️ LegacyProcs - Sistema de Gerenciamento de Ordens de Serviço

[![CI/CD Pipeline](https://github.com/alest-github/TesteTimeLegado/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/alest-github/TesteTimeLegado/actions/workflows/ci-cd.yml)
[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Angular 18](https://img.shields.io/badge/Angular-18-DD0031?logo=angular)](https://angular.io/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

> Modernização completa de sistema legado: .NET Framework 4.8 → .NET 8 LTS | Angular 12 → Angular 18

---

## 📋 Índice

- [Sobre o Projeto](#-sobre-o-projeto)
- [Arquitetura](#-arquitetura)
- [Tecnologias](#-tecnologias)
- [Funcionalidades](#-funcionalidades)
- [Modernização Realizada](#-modernização-realizada)
- [Qualidade e Testes](#-qualidade-e-testes)
- [CI/CD](#-cicd)
- [Como Executar](#-como-executar)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Equipe](#-equipe)

---

## 🎯 Sobre o Projeto

O **LegacyProcs** é um sistema completo de gerenciamento de ordens de serviço para manutenção predial, desenvolvido originalmente com tecnologias legadas e completamente modernizado seguindo as melhores práticas da indústria.

### 🎓 Contexto

Este projeto foi desenvolvido como parte do programa de capacitação da **Alest**, demonstrando:
- Modernização de sistemas legados
- Aplicação de Clean Architecture e CQRS
- Implementação de segurança (OWASP Top 10)
- Testes automatizados (>80% coverage)
- CI/CD com GitHub Actions

---

## 🏛️ Arquitetura

### Backend - Clean Architecture + CQRS

```
┌─────────────────────────────────────────────────────────┐
│                    API Layer (.NET 8)                   │
│  - Minimal APIs                                         │
│  - Swagger/OpenAPI                                      │
│  - Security Middleware (Headers, Rate Limiting, CORS)   │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│              Application Layer (MediatR)                │
│  - Commands & Queries (CQRS)                           │
│  - FluentValidation                                     │
│  - DTOs & Mapping (AutoMapper)                         │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│                   Domain Layer                          │
│  - Rich Domain Models                                   │
│  - Business Rules                                       │
│  - Domain Events                                        │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│              Infrastructure Layer                       │
│  - Entity Framework Core 8                             │
│  - Repository Pattern                                   │
│  - SQL Server                                           │
└─────────────────────────────────────────────────────────┘
```

### Frontend - Angular 18 + PrimeNG

```
┌─────────────────────────────────────────────────────────┐
│                  Presentation Layer                     │
│  - Angular 18 (Standalone Components)                  │
│  - PrimeNG 17 (UI Components)                          │
│  - Reactive Forms                                       │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│                    Service Layer                        │
│  - HTTP Services (HttpClient)                          │
│  - State Management (RxJS)                             │
│  - Error Handling                                       │
└─────────────────────────────────────────────────────────┘
```

---

## 🚀 Tecnologias

### Backend
- **.NET 8 LTS** - Framework moderno e performático
- **Entity Framework Core 8** - ORM com migrations
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Validação de entrada
- **AutoMapper** - Mapeamento de objetos
- **Serilog** - Logging estruturado
- **xUnit + Moq** - Testes unitários e de integração

### Frontend
- **Angular 18** - Framework SPA moderno
- **PrimeNG 17** - Biblioteca de componentes UI
- **RxJS** - Programação reativa
- **TypeScript 5** - Tipagem estática
- **TailwindCSS** - Estilização utilitária
- **Jasmine + Karma** - Testes unitários

### DevOps & CI/CD
- **GitHub Actions** - Pipeline de CI/CD
- **Docker** - Containerização
- **GitHub Container Registry** - Registry de imagens
- **Trivy** - Security scanning

### Banco de Dados
- **SQL Server** - Banco de dados relacional
- **Migrations** - Versionamento de schema

---

## ✨ Funcionalidades

### 📋 Gestão de Ordens de Serviço
- ✅ Criar, editar, visualizar e excluir ordens de serviço
- ✅ Atribuir técnicos a ordens de serviço
- ✅ Controle de status (Pendente, Em Andamento, Concluída, Cancelada)
- ✅ Histórico completo de alterações
- ✅ Paginação e filtros avançados

### 👥 Gestão de Clientes
- ✅ CRUD completo de clientes
- ✅ Validação de CPF/CNPJ
- ✅ Histórico de ordens de serviço por cliente
- ✅ Busca e filtros

### 🔧 Gestão de Técnicos
- ✅ CRUD completo de técnicos
- ✅ Especialidades e disponibilidade
- ✅ Ordens de serviço atribuídas
- ✅ Performance e métricas

### 🔒 Segurança
- ✅ Proteção contra SQL Injection (EF Core parametrizado)
- ✅ Security Headers (HSTS, CSP, X-Frame-Options)
- ✅ Rate Limiting
- ✅ Input Sanitization
- ✅ CORS configurável por ambiente
- ✅ Audit Trail (LGPD compliance)

---

## 🔄 Modernização Realizada

### Backend: .NET Framework 4.8 → .NET 8 LTS

| Aspecto | Antes (Legado) | Depois (Moderno) |
|---------|----------------|------------------|
| **Framework** | .NET Framework 4.8 | .NET 8 LTS |
| **API** | ASP.NET Web API 2 | Minimal APIs |
| **Arquitetura** | Monolítico acoplado | Clean Architecture + CQRS |
| **Acesso a Dados** | ADO.NET (SQL direto) | Entity Framework Core 8 |
| **Validação** | Manual/inexistente | FluentValidation |
| **Testes** | 0% coverage | >80% coverage |
| **Segurança** | SQL Injection vulnerável | OWASP Top 10 compliant |
| **Logging** | Console.WriteLine | Serilog estruturado |
| **DI** | Manual | Nativo .NET 8 |

### Frontend: Angular 12 → Angular 18

| Aspecto | Antes (Legado) | Depois (Moderno) |
|---------|----------------|------------------|
| **Framework** | Angular 12 | Angular 18 |
| **Componentes** | NgModules | Standalone Components |
| **UI Library** | Material Design | PrimeNG 17 |
| **Estilização** | CSS puro | TailwindCSS |
| **Forms** | Template-driven | Reactive Forms |
| **HTTP** | HttpClient básico | Interceptors + Error Handling |
| **Build** | Webpack | esbuild (mais rápido) |
| **Performance** | Sem otimizações | Lazy Loading + OnPush |

### Débitos Técnicos Resolvidos

#### ❌ Críticos (Resolvidos)
1. **SQL Injection** → EF Core com queries parametrizadas
2. **Hardcoded Credentials** → Configuração por ambiente
3. **Sem Testes** → 129 testes com >80% coverage
4. **SOLID Violations** → Clean Architecture implementada

#### ⚠️ Altos (Resolvidos)
5. **Sem Paginação** → PaginatedResultDto implementado
6. **Error Exposure** → Global Exception Handler seguro
7. **CORS Inseguro** → Configuração específica por ambiente
8. **Validação Fraca** → FluentValidation em todos endpoints

---

## 🧪 Qualidade e Testes

### Cobertura de Testes

```
Backend (.NET 8):
├── Unit Tests: 89 testes
├── Integration Tests: 28 testes
├── E2E Tests: 12 testes
└── Coverage: 82% (>80% target ✅)

Frontend (Angular 18):
├── Unit Tests: 45 testes
├── Component Tests: 23 testes
└── Coverage: 78%
```

### Tipos de Testes

#### Backend
- **Unit Tests** - xUnit + Moq + FluentAssertions
- **Integration Tests** - WebApplicationFactory
- **E2E Tests** - Cenários completos de negócio
- **Security Tests** - OWASP compliance validation

#### Frontend
- **Unit Tests** - Jasmine + Karma
- **Component Tests** - TestBed
- **E2E Tests** - (Opcional: Playwright/Cypress)

### Métricas de Qualidade

- ✅ **Zero warnings** no build
- ✅ **SOLID principles** aplicados
- ✅ **Clean Code** practices
- ✅ **Security** OWASP Top 10 compliant
- ✅ **Performance** otimizada (paginação, lazy loading)

---

## 🔄 CI/CD

### Pipeline Híbrido (GitHub Actions)

```
┌─────────────────────────────────────────────────────────┐
│  1. TESTES (Paralelo - ~1m)                            │
├─────────────────────────────────────────────────────────┤
│  ✅ Test Backend (.NET 8)      [~46s]                  │
│  ✅ Test Frontend (Angular 18) [~1m]                   │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│  2. QUALITY GATES (~4s)                                │
├─────────────────────────────────────────────────────────┤
│  🔍 Verify test coverage (>80%)                        │
│  🔍 Check build quality (zero warnings)                │
│  🔍 Validate no critical vulnerabilities               │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│  3. DOCKER BUILD (~2m) - Apenas main/develop           │
├─────────────────────────────────────────────────────────┤
│  🐳 Build & Push Backend Image                         │
│  🐳 Build & Push Frontend Image                        │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│  4. SECURITY SCAN (~30s) - Apenas main                 │
├─────────────────────────────────────────────────────────┤
│  🔒 Trivy vulnerability scanner                        │
│  🔒 Upload results to GitHub Security                  │
└─────────────────────────────────────────────────────────┘
```

### Branches e Estratégia

- **main** - Produção (todos os jobs)
- **develop** - Staging (testes + docker)
- **feature/*** - Features (apenas testes + quality gates)

---

## 🚀 Como Executar

### Pré-requisitos

- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 20+** - [Download](https://nodejs.org/)
- **SQL Server** - LocalDB ou Express
- **Git** - [Download](https://git-scm.com/)

### 1️⃣ Clone o Repositório

```bash
git clone https://github.com/alest-github/TesteTimeLegado.git
cd TesteTimeLegado
```

### 2️⃣ Configure o Banco de Dados

```bash
cd database
sqlcmd -S localhost\SQLEXPRESS -i setup-database.sql
sqlcmd -S localhost\SQLEXPRESS -d LegacyProcsDB -i seed-data.sql
```

Ou use o script PowerShell:

```powershell
.\scripts\seed-database.ps1
```

### 3️⃣ Execute o Backend

```bash
cd backend
dotnet restore
dotnet run --project LegacyProcs/LegacyProcs.csproj
```

Backend rodando em: `http://localhost:5000`

### 4️⃣ Execute o Frontend

```bash
cd frontend
npm install
npm start
```

Frontend rodando em: `http://localhost:4200`

### 5️⃣ Acesse a Aplicação

- **Frontend:** http://localhost:4200
- **Backend API:** http://localhost:5000
- **Swagger:** http://localhost:5000/swagger

---

## 📁 Estrutura do Projeto

```
TesteTimeLegado/
├── .github/
│   └── workflows/
│       └── ci-cd.yml              # Pipeline CI/CD
├── backend/
│   ├── LegacyProcs/               # API Principal
│   │   ├── Domain/                # Entidades e regras de negócio
│   │   ├── Application/           # CQRS (Commands & Queries)
│   │   ├── Infrastructure/        # EF Core, Repositories
│   │   └── Program.cs             # Entry point
│   └── LegacyProcs.Tests/         # Testes (Unit, Integration, E2E)
├── frontend/
│   └── src/
│       ├── app/
│       │   ├── features/          # Módulos de funcionalidades
│       │   ├── shared/            # Componentes compartilhados
│       │   └── core/              # Services e Guards
│       └── environments/          # Configurações por ambiente
├── database/
│   ├── setup-database.sql         # Schema inicial
│   └── seed-data.sql              # Dados de teste
├── scripts/                       # Scripts utilitários
└── README.md                      # Este arquivo
```

---

## 📊 Métricas do Projeto

### Código

- **Backend:** ~15.000 linhas de código
- **Frontend:** ~8.000 linhas de código
- **Testes:** ~5.000 linhas de código
- **Total:** ~28.000 linhas

### Tempo de Desenvolvimento

- **Análise e Planejamento:** 2 semanas
- **Modernização Backend:** 4 semanas
- **Modernização Frontend:** 3 semanas
- **Testes e CI/CD:** 1 semana
- **Total:** 10 semanas

### Qualidade

- **Test Coverage:** 82% (backend) + 78% (frontend)
- **Build Time:** ~1 minuto (CI/CD)
- **Zero Warnings:** ✅
- **OWASP Compliance:** ✅
- **Performance:** Otimizada

---

## 👥 Equipe

### Desenvolvimento

- **Nicolas Dias** - Full Stack Developer
  - Branch: `NicolasDias/Modernizacao`
  - GitHub: [@alest-github](https://github.com/alest-github)

### Empresa

- **Alest** - Consultoria em Tecnologia
  - Programa de Capacitação de Estagiários
  - Foco em Modernização de Sistemas Legados

---

## 📄 Licença

Este projeto foi desenvolvido para fins educacionais como parte do programa de treinamento da Alest.

---

## 🎯 Resultados Alcançados

### ✅ Objetivos Técnicos

- [x] Modernização completa do backend (.NET 8)
- [x] Modernização completa do frontend (Angular 18)
- [x] Implementação de Clean Architecture + CQRS
- [x] Cobertura de testes >80%
- [x] Resolução de todos os débitos técnicos
- [x] Compliance com OWASP Top 10
- [x] CI/CD automatizado
- [x] Documentação completa

### ✅ Objetivos de Aprendizado

- [x] Domínio de .NET 8 e Minimal APIs
- [x] Domínio de Angular 18 e Standalone Components
- [x] Aplicação prática de Clean Architecture
- [x] Implementação de CQRS com MediatR
- [x] Testes automatizados (Unit, Integration, E2E)
- [x] DevOps e CI/CD com GitHub Actions
- [x] Segurança (OWASP Top 10)
- [x] Boas práticas de Git (Conventional Commits)

---

## 📞 Contato

Para dúvidas ou sugestões sobre este projeto:

- **Email:** contato@alest.com.br
- **GitHub:** [alest-github/TesteTimeLegado](https://github.com/alest-github/TesteTimeLegado)
- **Issues:** [Reportar Problema](https://github.com/alest-github/TesteTimeLegado/issues)

---

<div align="center">

**Desenvolvido com ❤️ pela equipe Alest**

[![GitHub](https://img.shields.io/badge/GitHub-alest--github-181717?logo=github)](https://github.com/alest-github)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-18-DD0031?logo=angular)](https://angular.io/)

</div>
