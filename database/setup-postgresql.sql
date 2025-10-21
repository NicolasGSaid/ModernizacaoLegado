-- =============================================
-- Script de Setup PostgreSQL - LegacyProcs
-- Render.com Database
-- =============================================

-- Criar extensões necessárias
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- =============================================
-- TABELA: Clientes
-- =============================================
CREATE TABLE IF NOT EXISTS "Clientes" (
    "Id" SERIAL PRIMARY KEY,
    "RazaoSocial" VARCHAR(200) NOT NULL,
    "NomeFantasia" VARCHAR(200),
    "CNPJ" VARCHAR(18) NOT NULL UNIQUE,
    "Email" VARCHAR(100) NOT NULL,
    "Telefone" VARCHAR(20) NOT NULL,
    "Endereco" VARCHAR(300) NOT NULL,
    "Cidade" VARCHAR(100) NOT NULL,
    "Estado" VARCHAR(2) NOT NULL,
    "CEP" VARCHAR(10) NOT NULL,
    "DataCadastro" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT "CK_Cliente_Estado" CHECK (LENGTH("Estado") = 2),
    CONSTRAINT "CK_Cliente_Email" CHECK ("Email" ~* '^[^@\s]+@[^@\s]+\.[^@\s]+$')
);

-- Índices para performance
CREATE INDEX IF NOT EXISTS "IX_Clientes_CNPJ" ON "Clientes"("CNPJ");
CREATE INDEX IF NOT EXISTS "IX_Clientes_Email" ON "Clientes"("Email");
CREATE INDEX IF NOT EXISTS "IX_Clientes_RazaoSocial" ON "Clientes"("RazaoSocial");

-- =============================================
-- TABELA: Tecnicos
-- =============================================
CREATE TABLE IF NOT EXISTS "Tecnicos" (
    "Id" SERIAL PRIMARY KEY,
    "Nome" VARCHAR(100) NOT NULL,
    "Email" VARCHAR(100) NOT NULL UNIQUE,
    "Telefone" VARCHAR(20) NOT NULL,
    "Especialidade" VARCHAR(100) NOT NULL,
    "Status" INTEGER NOT NULL DEFAULT 0, -- 0=Ativo, 1=Inativo, 2=Ferias
    "DataCadastro" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT "CK_Tecnico_Email" CHECK ("Email" ~* '^[^@\s]+@[^@\s]+\.[^@\s]+$'),
    CONSTRAINT "CK_Tecnico_Status" CHECK ("Status" IN (0, 1, 2))
);

-- Índices para performance
CREATE INDEX IF NOT EXISTS "IX_Tecnicos_Email" ON "Tecnicos"("Email");
CREATE INDEX IF NOT EXISTS "IX_Tecnicos_Status" ON "Tecnicos"("Status");
CREATE INDEX IF NOT EXISTS "IX_Tecnicos_Especialidade" ON "Tecnicos"("Especialidade");

-- =============================================
-- TABELA: OrdemServico
-- =============================================
CREATE TABLE IF NOT EXISTS "OrdemServico" (
    "Id" SERIAL PRIMARY KEY,
    "Titulo" VARCHAR(200) NOT NULL,
    "Descricao" VARCHAR(1000),
    "Tecnico" VARCHAR(100) NOT NULL,
    "Status" INTEGER NOT NULL DEFAULT 0, -- 0=Pendente, 1=EmAndamento, 2=Concluida, 3=Cancelada
    "DataCriacao" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "DataAtualizacao" TIMESTAMP,
    
    CONSTRAINT "CK_OrdemServico_Status" CHECK ("Status" IN (0, 1, 2, 3))
);

-- Índices para performance
CREATE INDEX IF NOT EXISTS "IX_OrdemServico_Status" ON "OrdemServico"("Status");
CREATE INDEX IF NOT EXISTS "IX_OrdemServico_Tecnico" ON "OrdemServico"("Tecnico");
CREATE INDEX IF NOT EXISTS "IX_OrdemServico_DataCriacao" ON "OrdemServico"("DataCriacao");

-- =============================================
-- TABELA: AuditLogs (LGPD Compliance)
-- =============================================
CREATE TABLE IF NOT EXISTS "AuditLogs" (
    "Id" SERIAL PRIMARY KEY,
    "EntityName" VARCHAR(100) NOT NULL,
    "EntityId" VARCHAR(50) NOT NULL,
    "Action" VARCHAR(50) NOT NULL, -- Create, Update, Delete, Read
    "UserId" VARCHAR(100),
    "UserName" VARCHAR(200),
    "Changes" TEXT,
    "IpAddress" VARCHAR(50),
    "UserAgent" VARCHAR(500),
    "Timestamp" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Índices para auditoria
CREATE INDEX IF NOT EXISTS "IX_AuditLogs_EntityName" ON "AuditLogs"("EntityName");
CREATE INDEX IF NOT EXISTS "IX_AuditLogs_EntityId" ON "AuditLogs"("EntityId");
CREATE INDEX IF NOT EXISTS "IX_AuditLogs_Action" ON "AuditLogs"("Action");
CREATE INDEX IF NOT EXISTS "IX_AuditLogs_Timestamp" ON "AuditLogs"("Timestamp");
CREATE INDEX IF NOT EXISTS "IX_AuditLogs_UserId" ON "AuditLogs"("UserId");

-- =============================================
-- DADOS DE TESTE: Clientes
-- =============================================
INSERT INTO "Clientes" ("RazaoSocial", "NomeFantasia", "CNPJ", "Email", "Telefone", "Endereco", "Cidade", "Estado", "CEP")
VALUES 
    ('Tech Solutions Ltda', 'TechSol', '12345678000190', 'contato@techsol.com.br', '11987654321', 'Av. Paulista, 1000', 'São Paulo', 'SP', '01310100'),
    ('Inovação Digital EIRELI', 'InoDigi', '98765432000110', 'contato@inodigital.com.br', '11912345678', 'Rua Augusta, 500', 'São Paulo', 'SP', '01305000'),
    ('Serviços Gerais S/A', 'SerGer', '11223344000155', 'contato@sergeral.com.br', '21987654321', 'Av. Rio Branco, 200', 'Rio de Janeiro', 'RJ', '20040001')
ON CONFLICT ("CNPJ") DO NOTHING;

-- =============================================
-- DADOS DE TESTE: Tecnicos
-- =============================================
INSERT INTO "Tecnicos" ("Nome", "Email", "Telefone", "Especialidade", "Status")
VALUES 
    ('João Silva', 'joao.silva@legacyprocs.com', '11987654321', 'Elétrica', 0),
    ('Maria Santos', 'maria.santos@legacyprocs.com', '11912345678', 'Hidráulica', 0),
    ('Pedro Oliveira', 'pedro.oliveira@legacyprocs.com', '11923456789', 'Ar Condicionado', 0),
    ('Ana Costa', 'ana.costa@legacyprocs.com', '11934567890', 'Pintura', 0),
    ('Carlos Souza', 'carlos.souza@legacyprocs.com', '11945678901', 'Marcenaria', 1)
ON CONFLICT ("Email") DO NOTHING;

-- =============================================
-- DADOS DE TESTE: Ordens de Serviço
-- =============================================
INSERT INTO "OrdemServico" ("Titulo", "Descricao", "Tecnico", "Status", "DataCriacao", "DataAtualizacao")
VALUES 
    ('Trocar lâmpada', 'Lâmpada queimada no corredor principal', 'João Silva', 0, CURRENT_TIMESTAMP, NULL),
    ('Consertar impressora', 'Impressora não liga', 'Maria Santos', 1, CURRENT_TIMESTAMP - INTERVAL '1 day', CURRENT_TIMESTAMP - INTERVAL '12 hours'),
    ('Limpar filtro ar condicionado', 'Manutenção preventiva', 'Pedro Oliveira', 2, CURRENT_TIMESTAMP - INTERVAL '2 days', CURRENT_TIMESTAMP - INTERVAL '1 day'),
    ('Verificar vazamento', 'Vazamento na copa', 'João Silva', 0, CURRENT_TIMESTAMP - INTERVAL '3 days', NULL),
    ('Substituir tomada', 'Tomada queimada na sala 301', 'Maria Santos', 0, CURRENT_TIMESTAMP - INTERVAL '5 days', NULL),
    ('Reparar porta', 'Porta do banheiro não fecha', 'Pedro Oliveira', 1, CURRENT_TIMESTAMP - INTERVAL '7 days', CURRENT_TIMESTAMP - INTERVAL '6 days'),
    ('Trocar fechadura', 'Fechadura emperrada', 'João Silva', 2, CURRENT_TIMESTAMP - INTERVAL '10 days', CURRENT_TIMESTAMP - INTERVAL '9 days'),
    ('Pintura parede', 'Parede descascada no hall', 'Ana Costa', 0, CURRENT_TIMESTAMP - INTERVAL '12 days', NULL);

-- =============================================
-- VIEWS ÚTEIS
-- =============================================

-- View: Ordens de Serviço com Status Legível
CREATE OR REPLACE VIEW "vw_OrdemServico_Detalhada" AS
SELECT 
    "Id",
    "Titulo",
    "Descricao",
    "Tecnico",
    CASE "Status"
        WHEN 0 THEN 'Pendente'
        WHEN 1 THEN 'Em Andamento'
        WHEN 2 THEN 'Concluída'
        WHEN 3 THEN 'Cancelada'
    END AS "StatusDescricao",
    "Status",
    "DataCriacao",
    "DataAtualizacao",
    EXTRACT(DAY FROM (CURRENT_TIMESTAMP - "DataCriacao")) AS "DiasDesdeC riacao"
FROM "OrdemServico";

-- View: Técnicos Ativos
CREATE OR REPLACE VIEW "vw_Tecnicos_Ativos" AS
SELECT 
    "Id",
    "Nome",
    "Email",
    "Telefone",
    "Especialidade",
    CASE "Status"
        WHEN 0 THEN 'Ativo'
        WHEN 1 THEN 'Inativo'
        WHEN 2 THEN 'Férias'
    END AS "StatusDescricao"
FROM "Tecnicos"
WHERE "Status" = 0;

-- View: Estatísticas de Ordens de Serviço
CREATE OR REPLACE VIEW "vw_Estatisticas_OS" AS
SELECT 
    COUNT(*) AS "TotalOS",
    COUNT(*) FILTER (WHERE "Status" = 0) AS "Pendentes",
    COUNT(*) FILTER (WHERE "Status" = 1) AS "EmAndamento",
    COUNT(*) FILTER (WHERE "Status" = 2) AS "Concluidas",
    COUNT(*) FILTER (WHERE "Status" = 3) AS "Canceladas",
    ROUND(AVG(EXTRACT(EPOCH FROM ("DataAtualizacao" - "DataCriacao"))/3600), 2) AS "TempoMedioHoras"
FROM "OrdemServico"
WHERE "DataAtualizacao" IS NOT NULL;

-- =============================================
-- FUNÇÕES ÚTEIS
-- =============================================

-- Função: Validar CNPJ (simplificada)
CREATE OR REPLACE FUNCTION validar_cnpj(cnpj VARCHAR)
RETURNS BOOLEAN AS $$
BEGIN
    -- Remove caracteres não numéricos
    cnpj := REGEXP_REPLACE(cnpj, '[^0-9]', '', 'g');
    
    -- Verifica se tem 14 dígitos
    IF LENGTH(cnpj) != 14 THEN
        RETURN FALSE;
    END IF;
    
    -- Verifica se todos os dígitos são iguais
    IF cnpj ~ '^(.)\1*$' THEN
        RETURN FALSE;
    END IF;
    
    RETURN TRUE;
END;
$$ LANGUAGE plpgsql;

-- Função: Formatar CNPJ
CREATE OR REPLACE FUNCTION formatar_cnpj(cnpj VARCHAR)
RETURNS VARCHAR AS $$
BEGIN
    cnpj := REGEXP_REPLACE(cnpj, '[^0-9]', '', 'g');
    
    IF LENGTH(cnpj) != 14 THEN
        RETURN cnpj;
    END IF;
    
    RETURN SUBSTRING(cnpj, 1, 2) || '.' || 
           SUBSTRING(cnpj, 3, 3) || '.' || 
           SUBSTRING(cnpj, 6, 3) || '/' || 
           SUBSTRING(cnpj, 9, 4) || '-' || 
           SUBSTRING(cnpj, 13, 2);
END;
$$ LANGUAGE plpgsql;

-- Função: Formatar CEP
CREATE OR REPLACE FUNCTION formatar_cep(cep VARCHAR)
RETURNS VARCHAR AS $$
BEGIN
    cep := REGEXP_REPLACE(cep, '[^0-9]', '', 'g');
    
    IF LENGTH(cep) != 8 THEN
        RETURN cep;
    END IF;
    
    RETURN SUBSTRING(cep, 1, 5) || '-' || SUBSTRING(cep, 6, 3);
END;
$$ LANGUAGE plpgsql;

-- =============================================
-- TRIGGERS PARA AUDITORIA AUTOMÁTICA
-- =============================================

-- Função de trigger para auditoria
CREATE OR REPLACE FUNCTION audit_trigger_func()
RETURNS TRIGGER AS $$
BEGIN
    IF (TG_OP = 'INSERT') THEN
        INSERT INTO "AuditLogs" ("EntityName", "EntityId", "Action", "Changes", "Timestamp")
        VALUES (TG_TABLE_NAME, NEW."Id"::VARCHAR, 'Create', row_to_json(NEW)::TEXT, CURRENT_TIMESTAMP);
        RETURN NEW;
    ELSIF (TG_OP = 'UPDATE') THEN
        INSERT INTO "AuditLogs" ("EntityName", "EntityId", "Action", "Changes", "Timestamp")
        VALUES (TG_TABLE_NAME, NEW."Id"::VARCHAR, 'Update', 
                json_build_object('old', row_to_json(OLD), 'new', row_to_json(NEW))::TEXT, 
                CURRENT_TIMESTAMP);
        RETURN NEW;
    ELSIF (TG_OP = 'DELETE') THEN
        INSERT INTO "AuditLogs" ("EntityName", "EntityId", "Action", "Changes", "Timestamp")
        VALUES (TG_TABLE_NAME, OLD."Id"::VARCHAR, 'Delete', row_to_json(OLD)::TEXT, CURRENT_TIMESTAMP);
        RETURN OLD;
    END IF;
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

-- Triggers de auditoria
DROP TRIGGER IF EXISTS audit_clientes ON "Clientes";
CREATE TRIGGER audit_clientes
    AFTER INSERT OR UPDATE OR DELETE ON "Clientes"
    FOR EACH ROW EXECUTE FUNCTION audit_trigger_func();

DROP TRIGGER IF EXISTS audit_tecnicos ON "Tecnicos";
CREATE TRIGGER audit_tecnicos
    AFTER INSERT OR UPDATE OR DELETE ON "Tecnicos"
    FOR EACH ROW EXECUTE FUNCTION audit_trigger_func();

DROP TRIGGER IF EXISTS audit_ordemservico ON "OrdemServico";
CREATE TRIGGER audit_ordemservico
    AFTER INSERT OR UPDATE OR DELETE ON "OrdemServico"
    FOR EACH ROW EXECUTE FUNCTION audit_trigger_func();

-- =============================================
-- VERIFICAÇÃO FINAL
-- =============================================

-- Contar registros
SELECT 'Clientes' AS tabela, COUNT(*) AS total FROM "Clientes"
UNION ALL
SELECT 'Tecnicos', COUNT(*) FROM "Tecnicos"
UNION ALL
SELECT 'OrdemServico', COUNT(*) FROM "OrdemServico"
UNION ALL
SELECT 'AuditLogs', COUNT(*) FROM "AuditLogs";

-- Mostrar estatísticas
SELECT * FROM "vw_Estatisticas_OS";

-- =============================================
-- SCRIPT CONCLUÍDO
-- =============================================
-- Para executar este script no Render:
-- 1. Acesse o Dashboard do Render
-- 2. Vá em "legacyprocs-db"
-- 3. Clique em "Connect" → "External Connection"
-- 4. Copie o comando PSQL
-- 5. Execute no terminal: psql [URL] < setup-postgresql.sql
-- 
-- Ou use o Render Shell:
-- 1. Clique em "Shell" no dashboard do banco
-- 2. Cole este script SQL
-- =============================================
