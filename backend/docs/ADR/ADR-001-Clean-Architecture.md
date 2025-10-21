# ADR-001: Adoção da Clean Architecture

## Status
✅ **ACEITO** - Implementado em 15/10/2025

## Contexto
O projeto LegacyProcs estava utilizando uma arquitetura monolítica em .NET Framework 4.8 com acoplamento forte entre camadas, dificultando manutenção, testes e evolução do sistema.

## Decisão
Adotar **Clean Architecture** (Uncle Bob) com 4 camadas bem definidas:

### 1. **Domain Layer** (Núcleo)
- **Responsabilidade**: Regras de negócio e entidades
- **Dependências**: Nenhuma (independente)
- **Componentes**:
  - Entidades com Rich Domain Model
  - Value Objects
  - Domain Services
  - Interfaces de repositório
  - Exceções de domínio

### 2. **Application Layer** (Casos de Uso)
- **Responsabilidade**: Orquestração e casos de uso
- **Dependências**: Apenas Domain Layer
- **Componentes**:
  - Commands e Queries (CQRS)
  - Handlers (MediatR)
  - DTOs
  - Validators (FluentValidation)
  - Behaviors (Cross-cutting concerns)

### 3. **Infrastructure Layer** (Detalhes)
- **Responsabilidade**: Implementações técnicas
- **Dependências**: Domain e Application
- **Componentes**:
  - Repositórios (Entity Framework)
  - Configurações de banco
  - Serviços externos
  - Middleware
  - Migrations

### 4. **API Layer** (Interface)
- **Responsabilidade**: Exposição HTTP
- **Dependências**: Application e Infrastructure
- **Componentes**:
  - Controllers
  - DTOs de entrada/saída
  - Configuração de DI
  - Swagger/OpenAPI

## Consequências

### ✅ **Positivas**
- **Testabilidade**: Cada camada pode ser testada isoladamente
- **Manutenibilidade**: Separação clara de responsabilidades
- **Flexibilidade**: Fácil troca de implementações
- **Independência**: Domain não depende de frameworks
- **Escalabilidade**: Estrutura preparada para crescimento

### ⚠️ **Negativas**
- **Complexidade inicial**: Mais arquivos e estrutura
- **Curva de aprendizado**: Equipe precisa entender os conceitos
- **Over-engineering**: Pode ser excessivo para projetos muito simples

### 🔧 **Mitigações**
- Documentação clara da arquitetura
- Exemplos práticos de implementação
- Code reviews para garantir aderência
- Testes automatizados para validar estrutura

## Implementação

### Estrutura de Pastas
```
LegacyProcs/
├── Domain/
│   ├── Entities/
│   ├── Enums/
│   ├── Exceptions/
│   └── Interfaces/
├── Application/
│   ├── Commands/
│   ├── Queries/
│   ├── Handlers/
│   ├── DTOs/
│   └── Behaviors/
├── Infrastructure/
│   ├── Data/
│   ├── Repositories/
│   ├── Configurations/
│   └── Middleware/
└── Controllers/
```

### Regras de Dependência
1. **Domain** → Não depende de nada
2. **Application** → Depende apenas de Domain
3. **Infrastructure** → Depende de Domain e Application
4. **API** → Depende de Application e Infrastructure

### Padrões Utilizados
- **Repository Pattern**: Abstração de acesso a dados
- **CQRS**: Separação de comandos e consultas
- **Mediator Pattern**: Desacoplamento via MediatR
- **Dependency Injection**: Inversão de controle

## Métricas de Sucesso
- ✅ **Cobertura de testes**: >80% (129 testes passando)
- ✅ **Acoplamento**: Baixo entre camadas
- ✅ **Coesão**: Alta dentro de cada camada
- ✅ **Manutenibilidade**: Facilidade para adicionar features

## Referências
- [Clean Architecture (Uncle Bob)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [.NET Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture)
- [Domain-Driven Design](https://martinfowler.com/tags/domain%20driven%20design.html)

---
**Autor**: Nicolas Dias  
**Data**: 15/10/2025  
**Revisores**: Equipe de Desenvolvimento  
**Próxima Revisão**: 15/01/2026
