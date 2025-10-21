-- =============================================
-- Script Simplificado PostgreSQL - LegacyProcs
-- =============================================

-- Limpar tabelas existentes
DROP TABLE IF EXISTS "AuditLogs" CASCADE;
DROP TABLE IF EXISTS "OrdemServico" CASCADE;
DROP TABLE IF EXISTS "Tecnicos" CASCADE;
DROP TABLE IF EXISTS "Clientes" CASCADE;

-- =============================================
-- TABELA: Clientes
-- =============================================
CREATE TABLE "Clientes" (
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
    "DataCadastro" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- =============================================
-- TABELA: Tecnicos
-- =============================================
CREATE TABLE "Tecnicos" (
    "Id" SERIAL PRIMARY KEY,
    "Nome" VARCHAR(100) NOT NULL,
    "Email" VARCHAR(100) NOT NULL UNIQUE,
    "Telefone" VARCHAR(20) NOT NULL,
    "Especialidade" VARCHAR(100) NOT NULL,
    "Status" INTEGER NOT NULL DEFAULT 0,
    "DataCadastro" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- =============================================
-- TABELA: OrdemServico
-- =============================================
CREATE TABLE "OrdemServico" (
    "Id" SERIAL PRIMARY KEY,
    "Titulo" VARCHAR(200) NOT NULL,
    "Descricao" VARCHAR(1000),
    "Tecnico" VARCHAR(100) NOT NULL,
    "Status" INTEGER NOT NULL DEFAULT 0,
    "DataCriacao" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "DataAtualizacao" TIMESTAMP
);

-- =============================================
-- TABELA: AuditLogs
-- =============================================
CREATE TABLE "AuditLogs" (
    "Id" SERIAL PRIMARY KEY,
    "EntityName" VARCHAR(100) NOT NULL,
    "EntityId" VARCHAR(50) NOT NULL,
    "Action" VARCHAR(50) NOT NULL,
    "UserId" VARCHAR(100),
    "UserName" VARCHAR(200),
    "Changes" TEXT,
    "IpAddress" VARCHAR(50),
    "UserAgent" VARCHAR(500),
    "Timestamp" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- =============================================
-- DADOS DE TESTE: Clientes
-- =============================================
INSERT INTO "Clientes" ("RazaoSocial", "NomeFantasia", "CNPJ", "Email", "Telefone", "Endereco", "Cidade", "Estado", "CEP")
VALUES 
    ('Tech Solutions Ltda', 'TechSol', '12345678000190', 'contato@techsol.com.br', '11987654321', 'Av. Paulista, 1000', 'São Paulo', 'SP', '01310100'),
    ('Inovação Digital EIRELI', 'InoDigi', '98765432000110', 'contato@inodigital.com.br', '11912345678', 'Rua Augusta, 500', 'São Paulo', 'SP', '01305000'),
    ('Serviços Gerais S/A', 'SerGer', '11223344000155', 'contato@sergeral.com.br', '21987654321', 'Av. Rio Branco, 200', 'Rio de Janeiro', 'RJ', '20040001');

-- =============================================
-- DADOS DE TESTE: Tecnicos
-- =============================================
INSERT INTO "Tecnicos" ("Nome", "Email", "Telefone", "Especialidade", "Status")
VALUES 
    ('João Silva', 'joao.silva@legacyprocs.com', '11987654321', 'Elétrica', 0),
    ('Maria Santos', 'maria.santos@legacyprocs.com', '11912345678', 'Hidráulica', 0),
    ('Pedro Oliveira', 'pedro.oliveira@legacyprocs.com', '11923456789', 'Ar Condicionado', 0),
    ('Ana Costa', 'ana.costa@legacyprocs.com', '11934567890', 'Pintura', 0),
    ('Carlos Souza', 'carlos.souza@legacyprocs.com', '11945678901', 'Marcenaria', 1);

-- =============================================
-- DADOS DE TESTE: Ordens de Serviço
-- =============================================
INSERT INTO "OrdemServico" ("Titulo", "Descricao", "Tecnico", "Status", "DataCriacao")
VALUES 
    ('Trocar lâmpada', 'Lâmpada queimada no corredor principal', 'João Silva', 0, CURRENT_TIMESTAMP),
    ('Consertar impressora', 'Impressora não liga', 'Maria Santos', 1, CURRENT_TIMESTAMP - INTERVAL '1 day'),
    ('Limpar filtro ar condicionado', 'Manutenção preventiva', 'Pedro Oliveira', 2, CURRENT_TIMESTAMP - INTERVAL '2 days'),
    ('Verificar vazamento', 'Vazamento na copa', 'João Silva', 0, CURRENT_TIMESTAMP - INTERVAL '3 days'),
    ('Substituir tomada', 'Tomada queimada na sala 301', 'Maria Santos', 0, CURRENT_TIMESTAMP - INTERVAL '5 days'),
    ('Reparar porta', 'Porta do banheiro não fecha', 'Pedro Oliveira', 1, CURRENT_TIMESTAMP - INTERVAL '7 days'),
    ('Trocar fechadura', 'Fechadura emperrada', 'João Silva', 2, CURRENT_TIMESTAMP - INTERVAL '10 days'),
    ('Pintura parede', 'Parede descascada no hall', 'Ana Costa', 0, CURRENT_TIMESTAMP - INTERVAL '12 days');

-- =============================================
-- VERIFICAÇÃO FINAL
-- =============================================
SELECT 'Clientes' AS tabela, COUNT(*) AS total FROM "Clientes"
UNION ALL
SELECT 'Tecnicos', COUNT(*) FROM "Tecnicos"
UNION ALL
SELECT 'OrdemServico', COUNT(*) FROM "OrdemServico"
UNION ALL
SELECT 'AuditLogs', COUNT(*) FROM "AuditLogs";
