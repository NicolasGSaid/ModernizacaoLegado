-- =============================================
-- Script de Setup - Banco de Dados LegacyProcs
-- =============================================

-- Criar banco de dados
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'LegacyProcs')
BEGIN
    CREATE DATABASE LegacyProcs;
END
GO

USE LegacyProcs;
GO

-- Criar tabela OrdemServico
IF OBJECT_ID('dbo.OrdemServico', 'U') IS NOT NULL
    DROP TABLE dbo.OrdemServico;
GO

CREATE TABLE OrdemServico (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Titulo NVARCHAR(200) NOT NULL,
    Descricao NVARCHAR(MAX),
    Tecnico NVARCHAR(100) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pendente',
    DataCriacao DATETIME NOT NULL DEFAULT GETDATE(),
    DataAtualizacao DATETIME NULL
);
GO

-- Inserir dados de teste
INSERT INTO OrdemServico (Titulo, Descricao, Tecnico, Status, DataCriacao)
VALUES 
    ('Trocar lâmpada', 'Lâmpada queimada no corredor principal', 'João Silva', 'Pendente', GETDATE()),
    ('Consertar impressora', 'Impressora não liga', 'Maria Santos', 'Em Andamento', DATEADD(DAY, -1, GETDATE())),
    ('Limpar filtro ar condicionado', 'Manutenção preventiva', 'Pedro Oliveira', 'Concluída', DATEADD(DAY, -2, GETDATE())),
    ('Verificar vazamento', 'Vazamento na copa', 'João Silva', 'Pendente', DATEADD(DAY, -3, GETDATE())),
    ('Substituir tomada', 'Tomada queimada na sala 301', 'Maria Santos', 'Pendente', DATEADD(DAY, -5, GETDATE())),
    ('Reparar porta', 'Porta do banheiro não fecha', 'Pedro Oliveira', 'Em Andamento', DATEADD(DAY, -7, GETDATE())),
    ('Trocar fechadura', 'Fechadura emperrada', 'João Silva', 'Concluída', DATEADD(DAY, -10, GETDATE())),
    ('Pintura parede', 'Parede descascada no hall', 'Maria Santos', 'Pendente', DATEADD(DAY, -12, GETDATE()));
GO

-- Verificar dados inseridos
SELECT * FROM OrdemServico;
GO

PRINT 'Setup concluído com sucesso!';
PRINT 'Total de registros: ' + CAST((SELECT COUNT(*) FROM OrdemServico) AS VARCHAR);
GO
