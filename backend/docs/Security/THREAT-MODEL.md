# 🛡️ Threat Model - LegacyProcs Backend

## 📋 **Resumo Executivo**
Este documento apresenta o modelo de ameaças para o sistema LegacyProcs modernizado, identificando assets, ameaças, vulnerabilidades e controles implementados conforme OWASP Top 10 2021 e NIST SSDF.

---

## 🎯 **Assets (Ativos)**

### 1. **Dados Pessoais (LGPD)**
- **Criticidade**: 🔴 **CRÍTICA**
- **Tipos**: 
  - Dados de clientes (CNPJ, razão social, contatos)
  - Dados de técnicos (nome, email, telefone)
  - Logs de auditoria com informações pessoais
- **Localização**: Banco SQL Server, logs estruturados
- **Regulamentação**: LGPD (Lei 13.709/2018)

### 2. **Dados de Negócio**
- **Criticidade**: 🟡 **ALTA**
- **Tipos**:
  - Ordens de serviço
  - Relacionamentos cliente-técnico
  - Histórico de operações
- **Localização**: Banco SQL Server

### 3. **Infraestrutura**
- **Criticidade**: 🟡 **ALTA**
- **Componentes**:
  - API .NET 8 (LegacyProcs.dll)
  - Banco SQL Server
  - Logs de aplicação
  - Configurações (appsettings.json)

---

## 🎭 **Threat Actors (Agentes de Ameaça)**

### 1. **Atacantes Externos**
- **Motivação**: Roubo de dados, ransomware, defacement
- **Capacidade**: Baixa a média
- **Métodos**: SQL Injection, XSS, DDoS, força bruta

### 2. **Insiders Maliciosos**
- **Motivação**: Vingança, ganho financeiro
- **Capacidade**: Alta (acesso interno)
- **Métodos**: Abuso de privilégios, exfiltração de dados

### 3. **Atacantes Automatizados**
- **Motivação**: Exploração em massa
- **Capacidade**: Média
- **Métodos**: Bots, scanners de vulnerabilidades

---

## ⚠️ **Ameaças Identificadas (STRIDE)**

### 🔓 **Spoofing (Falsificação)**
| ID | Ameaça | Impacto | Probabilidade | Risco |
|----|--------|---------|---------------|-------|
| S01 | Falsificação de identidade de usuário | Alto | Baixa | 🟡 Médio |
| S02 | Spoofing de IP para bypass de rate limiting | Médio | Média | 🟡 Médio |

### 🔧 **Tampering (Adulteração)**
| ID | Ameaça | Impacto | Probabilidade | Risco |
|----|--------|---------|---------------|-------|
| T01 | Modificação de dados via SQL Injection | Alto | Baixa | 🟡 Médio |
| T02 | Adulteração de logs de auditoria | Alto | Baixa | 🟡 Médio |
| T03 | Modificação de configurações | Crítico | Baixa | 🟡 Médio |

### 🚫 **Repudiation (Repúdio)**
| ID | Ameaça | Impacto | Probabilidade | Risco |
|----|--------|---------|---------------|-------|
| R01 | Negação de operações realizadas | Médio | Baixa | 🟢 Baixo |

### 📖 **Information Disclosure (Vazamento)**
| ID | Ameaça | Impacto | Probabilidade | Risco |
|----|--------|---------|---------------|-------|
| I01 | Exposição de dados pessoais via API | Crítico | Baixa | 🟡 Médio |
| I02 | Vazamento via logs de erro | Alto | Média | 🟡 Médio |
| I03 | Exposição de connection strings | Crítico | Baixa | 🟡 Médio |

### 🚫 **Denial of Service (Negação de Serviço)**
| ID | Ameaça | Impacto | Probabilidade | Risco |
|----|--------|---------|---------------|-------|
| D01 | DDoS na API | Alto | Média | 🟡 Médio |
| D02 | Resource exhaustion via payloads grandes | Médio | Média | 🟡 Médio |
| D03 | Database connection pool exhaustion | Alto | Baixa | 🟡 Médio |

### ⬆️ **Elevation of Privilege (Elevação de Privilégio)**
| ID | Ameaça | Impacto | Probabilidade | Risco |
|----|--------|---------|---------------|-------|
| E01 | Bypass de validações de entrada | Alto | Baixa | 🟡 Médio |
| E02 | Exploração de vulnerabilidades em dependências | Crítico | Baixa | 🟡 Médio |

---

## 🛡️ **Controles Implementados**

### **A01 - Broken Access Control**
- ✅ **Implementado**: Validações de entrada rigorosas
- ✅ **Implementado**: Rate limiting por IP/usuário
- ✅ **Implementado**: Logs de auditoria para todas as operações

### **A02 - Cryptographic Failures**
- ✅ **Implementado**: HTTPS obrigatório (HSTS)
- ✅ **Implementado**: Connection strings em variáveis de ambiente
- ✅ **Implementado**: Headers de segurança (CSP, HSTS)

### **A03 - Injection**
- ✅ **Implementado**: Entity Framework Core (queries parametrizadas)
- ✅ **Implementado**: FluentValidation em todos os inputs
- ✅ **Implementado**: Input sanitization middleware
- ✅ **Implementado**: Detecção automática de payloads maliciosos

### **A04 - Insecure Design**
- ✅ **Implementado**: Clean Architecture
- ✅ **Implementado**: CQRS para separação de responsabilidades
- ✅ **Implementado**: Threat modeling (este documento)

### **A05 - Security Misconfiguration**
- ✅ **Implementado**: Configurações por ambiente (Dev/Prod)
- ✅ **Implementado**: Desabilitação de features de debug em produção
- ✅ **Implementado**: Headers de segurança obrigatórios
- ✅ **Implementado**: Remoção de informações do servidor

### **A06 - Vulnerable and Outdated Components**
- ✅ **Implementado**: .NET 8 LTS (versão mais recente)
- ✅ **Implementado**: Dependências atualizadas
- ✅ **Implementado**: Verificação automática de vulnerabilidades
- ✅ **Implementado**: Microsoft.Data.SqlClient versão segura

### **A07 - Identification and Authentication Failures**
- ✅ **Implementado**: Middleware de autenticação
- ✅ **Implementado**: Rate limiting para prevenir força bruta
- ✅ **Implementado**: Logs de tentativas de acesso

### **A08 - Software and Data Integrity Failures**
- ✅ **Implementado**: Code signing via commits assinados
- ✅ **Implementado**: Validação de integridade de dados
- ✅ **Implementado**: Audit trail completo

### **A09 - Security Logging and Monitoring Failures**
- ✅ **Implementado**: Serilog com logs estruturados
- ✅ **Implementado**: Logs de segurança específicos
- ✅ **Implementado**: Auditoria de todas as operações CRUD
- ✅ **Implementado**: Monitoramento via Health Checks

### **A10 - Server-Side Request Forgery (SSRF)**
- ✅ **Implementado**: Validação de URLs externas
- ✅ **Implementado**: Whitelist de domínios permitidos

---

## 🔒 **Controles LGPD Implementados**

### **Privacy by Design**
- ✅ **Data Minimization**: Apenas campos necessários coletados
- ✅ **Audit Trail**: Rastreamento completo de operações
- ✅ **Right to Erasure**: Hard delete com confirmação
- ✅ **Data Portability**: Exportação em múltiplos formatos
- ✅ **Consent Management**: Controle de consentimento

### **Direitos dos Titulares**
- ✅ **Acesso**: API para consulta de dados
- ✅ **Retificação**: Endpoints de atualização
- ✅ **Exclusão**: Exclusão completa com protocolo
- ✅ **Portabilidade**: Exportação JSON/XML/CSV
- ✅ **Oposição**: Capacidade de recusar processamento

---

## 📊 **Matriz de Risco**

| Risco | Controles | Status | Responsável |
|-------|-----------|--------|-------------|
| 🔴 **Alto** | SQL Injection | ✅ Mitigado | EF Core + Validação |
| 🔴 **Alto** | XSS | ✅ Mitigado | Input Sanitization |
| 🟡 **Médio** | DDoS | ✅ Mitigado | Rate Limiting |
| 🟡 **Médio** | Data Breach | ✅ Mitigado | LGPD Controls |
| 🟢 **Baixo** | Info Disclosure | ✅ Mitigado | Security Headers |

---

## 🔄 **Monitoramento Contínuo**

### **Métricas de Segurança**
- **Rate Limiting**: Requisições bloqueadas por minuto
- **Input Sanitization**: Payloads maliciosos detectados
- **Audit Trail**: Operações sensíveis registradas
- **Vulnerabilidades**: Scan automático de dependências

### **Alertas Configurados**
- 🚨 **Crítico**: Tentativas de SQL Injection
- 🚨 **Crítico**: Múltiplas tentativas de acesso negado
- ⚠️ **Aviso**: Rate limiting ativado
- ℹ️ **Info**: Operações de exclusão de dados (LGPD)

---

## 📋 **Plano de Resposta a Incidentes**

### **Classificação de Incidentes**
1. **P0 - Crítico**: Vazamento de dados pessoais
2. **P1 - Alto**: Comprometimento da aplicação
3. **P2 - Médio**: Tentativas de ataque bloqueadas
4. **P3 - Baixo**: Anomalias de comportamento

### **Procedimentos**
1. **Detecção**: Logs automáticos + monitoramento
2. **Contenção**: Isolamento imediato do sistema
3. **Investigação**: Análise de logs de auditoria
4. **Recuperação**: Restore de backups seguros
5. **Lições Aprendidas**: Atualização do threat model

---

## 📅 **Cronograma de Revisão**

| Atividade | Frequência | Responsável | Próxima Data |
|-----------|------------|-------------|--------------|
| Threat Model Review | Trimestral | Security Team | 15/01/2026 |
| Vulnerability Scan | Mensal | DevOps | 15/11/2025 |
| Penetration Test | Semestral | External | 15/04/2026 |
| LGPD Compliance Audit | Anual | Legal + Tech | 15/10/2026 |

---

**Documento**: Threat Model LegacyProcs  
**Versão**: 1.0  
**Data**: 15/10/2025  
**Autor**: Nicolas Dias  
**Aprovação**: Equipe de Segurança  
**Próxima Revisão**: 15/01/2026
