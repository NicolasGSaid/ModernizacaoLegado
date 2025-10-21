-- Script Rápido para Render - LegacyProcs
-- Copie e cole TUDO no PSQL Command do Render

-- Criar tabelas
CREATE TABLE IF NOT EXISTS "OrdemServico" (
    "Id" SERIAL PRIMARY KEY,
    "Titulo" VARCHAR(200) NOT NULL,
    "Descricao" VARCHAR(1000),
    "Tecnico" VARCHAR(100) NOT NULL,
    "Status" INTEGER NOT NULL DEFAULT 0,
    "DataCriacao" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "DataAtualizacao" TIMESTAMP
);

CREATE TABLE IF NOT EXISTS "Tecnicos" (
    "Id" SERIAL PRIMARY KEY,
    "Nome" VARCHAR(100) NOT NULL,
    "Email" VARCHAR(100) NOT NULL UNIQUE,
    "Telefone" VARCHAR(20) NOT NULL,
    "Especialidade" VARCHAR(100) NOT NULL,
    "Status" INTEGER NOT NULL DEFAULT 0,
    "DataCadastro" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

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
    "DataCadastro" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS "AuditLogs" (
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

-- Dados de teste
INSERT INTO "Tecnicos" ("Nome", "Email", "Telefone", "Especialidade", "Status")
VALUES 
    ('João Silva', 'joao.silva@legacyprocs.com', '11987654321', 'Elétrica', 0),
    ('Maria Santos', 'maria.santos@legacyprocs.com', '11912345678', 'Hidráulica', 0),
    ('Pedro Oliveira', 'pedro.oliveira@legacyprocs.com', '11923456789', 'Ar Condicionado', 0)
ON CONFLICT ("Email") DO NOTHING;

INSERT INTO "OrdemServico" ("Titulo", "Descricao", "Tecnico", "Status")
VALUES 
    ('Trocar lâmpada', 'Lâmpada queimada no corredor', 'João Silva', 0),
    ('Consertar impressora', 'Impressora não liga', 'Maria Santos', 1),
    ('Limpar filtro ar condicionado', 'Manutenção preventiva', 'Pedro Oliveira', 2),
    ('Verificar vazamento', 'Vazamento na copa', 'João Silva', 0),
    ('Substituir tomada', 'Tomada queimada na sala 301', 'Maria Santos', 0);

-- Verificar
SELECT 'OrdemServico' AS tabela, COUNT(*) AS total FROM "OrdemServico"
UNION ALL
SELECT 'Tecnicos', COUNT(*) FROM "Tecnicos";
