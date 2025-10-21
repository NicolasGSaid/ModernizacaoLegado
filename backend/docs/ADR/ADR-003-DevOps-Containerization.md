# ADR-003: DevOps & Containerização

**Status:** ✅ Aprovado  
**Data:** 19/10/2025  
**Autor:** Nicolas Dias  
**Decisores:** Equipe Técnica

---

## Contexto

O projeto LegacyProcs necessita de uma estratégia DevOps completa para garantir:
- Deploy consistente em múltiplos ambientes
- Facilidade de desenvolvimento local
- CI/CD automatizado
- Escalabilidade e portabilidade

---

## Decisão

Implementar containerização completa com Docker e orquestração via Docker Compose, além de CI/CD com GitHub Actions.

### Stack Escolhida

1. **Containerização:** Docker 24+
2. **Orquestração Local:** Docker Compose
3. **CI/CD:** GitHub Actions
4. **Registry:** GitHub Container Registry (GHCR)
5. **Monitoramento:** Application Insights (futuro)

---

## Justificativa

### Docker

**Prós:**
- ✅ Padrão de mercado
- ✅ Portabilidade total
- ✅ Isolamento de dependências
- ✅ Reprodutibilidade
- ✅ Facilita onboarding

**Contras:**
- ❌ Curva de aprendizado inicial
- ❌ Overhead de recursos (mínimo)

### Docker Compose

**Prós:**
- ✅ Simples para desenvolvimento local
- ✅ Orquestração multi-container
- ✅ Configuração declarativa
- ✅ Suporte a múltiplos ambientes

**Contras:**
- ❌ Não recomendado para produção em larga escala
- ❌ Limitado comparado a Kubernetes

**Decisão:** Usar Docker Compose para dev/staging, preparar para Kubernetes em produção futura.

### GitHub Actions

**Prós:**
- ✅ Integração nativa com GitHub
- ✅ Gratuito para repositórios públicos
- ✅ Marketplace rico
- ✅ Fácil configuração

**Contras:**
- ❌ Limitações de minutos (plano free)
- ❌ Menos flexível que Jenkins

**Decisão:** Suficiente para o projeto atual, migração futura se necessário.

---

## Arquitetura Implementada

### Multi-Stage Builds

**Backend (.NET 8):**
```
Stage 1: Build (SDK 8.0) → Compila aplicação
Stage 2: Publish → Otimiza artefatos
Stage 3: Runtime (ASP.NET 8.0) → Executa aplicação
```

**Benefícios:**
- Reduz imagem final de ~1GB para ~450MB
- Separa dependências de build/runtime
- Melhora segurança (menos ferramentas em produção)

**Frontend (Angular 18):**
```
Stage 1: Build (Node 20) → Compila aplicação
Stage 2: Runtime (Nginx Alpine) → Serve estático
```

**Benefícios:**
- Reduz imagem final de ~1.5GB para ~45MB
- Nginx otimizado para servir arquivos estáticos
- Compressão gzip automática

### Segurança

1. **Non-root users:** Todos os containers rodam com usuários não-privilegiados
2. **Health checks:** Monitoramento automático de saúde
3. **Secrets:** Variáveis de ambiente, nunca hardcoded
4. **Scan de vulnerabilidades:** Trivy no CI/CD
5. **Security headers:** Nginx configurado com headers OWASP

### Networks & Volumes

- **Network:** Bridge isolada (`legacyprocs-network`)
- **Volume:** Persistência do SQL Server (`sqlserver-data`)
- **Comunicação:** Containers se comunicam por nome de serviço

---

## Alternativas Consideradas

### 1. Kubernetes desde o início

**Rejeitado porque:**
- Complexidade desnecessária para MVP
- Overhead operacional
- Custo de infraestrutura
- Time pequeno

**Preparação futura:**
- Docker Compose pode ser convertido para Kubernetes
- Helm charts podem ser criados posteriormente

### 2. Azure DevOps

**Rejeitado porque:**
- GitHub Actions mais simples
- Integração nativa com repositório
- Custo (Actions é gratuito)

### 3. Heroku/Vercel

**Rejeitado porque:**
- Menos controle sobre infraestrutura
- Vendor lock-in
- Limitações de customização
- Objetivo é aprender DevOps completo

---

## Consequências

### Positivas

- ✅ Ambiente de desenvolvimento idêntico à produção
- ✅ Onboarding simplificado (1 comando)
- ✅ CI/CD automatizado
- ✅ Deploy consistente
- ✅ Facilita testes de integração
- ✅ Portabilidade total

### Negativas

- ⚠️ Requer Docker instalado localmente
- ⚠️ Consumo de recursos (RAM/CPU)
- ⚠️ Curva de aprendizado para time
- ⚠️ Necessita monitoramento de custos em cloud

### Mitigações

- Documentação completa (DOCKER_README.md)
- Scripts auxiliares (start.ps1, stop.ps1)
- Configuração de recursos limitados
- Treinamento da equipe

---

## Métricas de Sucesso

| Métrica | Objetivo | Atual | Status |
|---------|----------|-------|--------|
| Build Time | <10min | ~7min | ✅ |
| Image Size (Backend) | <500MB | ~450MB | ✅ |
| Image Size (Frontend) | <50MB | ~45MB | ✅ |
| Deploy Time | <5min | ~3min | ✅ |
| Test Coverage | >80% | 76% | ⚠️ |

---

## Implementação

### Fase 1: Containerização ✅
- [x] Dockerfile Backend
- [x] Dockerfile Frontend
- [x] .dockerignore
- [x] Nginx configuration

### Fase 2: Orquestração ✅
- [x] docker-compose.yml
- [x] docker-compose.override.yml
- [x] .env.example

### Fase 3: CI/CD ✅
- [x] GitHub Actions workflow
- [x] Build & Test
- [x] Docker Build & Push
- [x] Security Scan

### Fase 4: Documentação ✅
- [x] DOCKER_README.md
- [x] Scripts auxiliares
- [x] ADR DevOps

---

## Próximos Passos

### Curto Prazo (1-2 meses)
- [ ] Deploy em Azure App Service / AWS ECS
- [ ] Configurar Application Insights
- [ ] Implementar backup automatizado
- [ ] Adicionar smoke tests pós-deploy

### Médio Prazo (3-6 meses)
- [ ] Migrar para Kubernetes (AKS/EKS)
- [ ] Implementar Helm charts
- [ ] Service mesh (Istio/Linkerd)
- [ ] Observabilidade completa (Prometheus + Grafana)

### Longo Prazo (6-12 meses)
- [ ] Multi-region deployment
- [ ] Auto-scaling avançado
- [ ] Disaster recovery
- [ ] Chaos engineering

---

## Referências

- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [.NET Docker](https://learn.microsoft.com/en-us/dotnet/core/docker/)
- [GitHub Actions](https://docs.github.com/en/actions)
- [OWASP Docker Security](https://cheatsheetseries.owasp.org/cheatsheets/Docker_Security_Cheat_Sheet.html)
- [Global Rules - Seção 7 (CI/CD)](../../../GLOBAL_RULES.md#7-cicd-e-deploy)

---

**Status:** ✅ Implementado e Aprovado  
**Revisão:** Trimestral  
**Próxima Revisão:** 19/01/2026
