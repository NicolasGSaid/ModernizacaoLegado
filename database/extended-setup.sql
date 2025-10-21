-- =============================================
-- Script Estendido - Tabelas Adicionais
-- =============================================

USE LegacyProcs;
GO

-- Criar tabela Tecnico
IF OBJECT_ID('dbo.Tecnico', 'U') IS NOT NULL
    DROP TABLE dbo.Tecnico;
GO

CREATE TABLE Tecnico (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nome NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    Telefone NVARCHAR(20),
    Especialidade NVARCHAR(50) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Ativo',
    DataCadastro DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Criar tabela Cliente
IF OBJECT_ID('dbo.Cliente', 'U') IS NOT NULL
    DROP TABLE dbo.Cliente;
GO

CREATE TABLE Cliente (
    Id INT PRIMARY KEY IDENTITY(1,1),
    RazaoSocial NVARCHAR(200) NOT NULL,
    NomeFantasia NVARCHAR(200),
    CNPJ NVARCHAR(18) NOT NULL,
    Email NVARCHAR(100),
    Telefone NVARCHAR(20),
    Endereco NVARCHAR(300),
    Cidade NVARCHAR(100),
    Estado NVARCHAR(2),
    CEP NVARCHAR(10),
    DataCadastro DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Inserir técnicos de teste
INSERT INTO Tecnico (Nome, Email, Telefone, Especialidade, Status, DataCadastro)
VALUES 
    ('João Silva', 'joao.silva@exemplo.com', '(11) 98765-4321', 'Elétrica', 'Ativo', GETDATE()),
    ('Maria Santos', 'maria.santos@exemplo.com', '(11) 97654-3210', 'Hidráulica', 'Ativo', DATEADD(DAY, -5, GETDATE())),
    ('Pedro Oliveira', 'pedro.oliveira@exemplo.com', '(11) 96543-2109', 'Ar Condicionado', 'Ativo', DATEADD(DAY, -10, GETDATE())),
    ('Ana Costa', 'ana.costa@exemplo.com', '(11) 95432-1098', 'Geral', 'Férias', DATEADD(DAY, -15, GETDATE())),
    ('Carlos Souza', 'carlos.souza@exemplo.com', '(11) 94321-0987', 'Pintura', 'Ativo', DATEADD(DAY, -20, GETDATE()));
GO

-- Inserir clientes de teste
INSERT INTO Cliente (RazaoSocial, NomeFantasia, CNPJ, Email, Telefone, Endereco, Cidade, Estado, CEP, DataCadastro)
VALUES 
    ('Tech Solutions Ltda', 'Tech Solutions', '12.345.678/0001-90', 'contato@techsolutions.com', '(11) 3456-7890', 
     'Av. Paulista, 1000', 'São Paulo', 'SP', '01310-100', GETDATE()),
    ('Comercial ABC S.A.', 'ABC Comércio', '98.765.432/0001-10', 'vendas@abc.com.br', '(11) 3210-9876', 
     'Rua Augusta, 500', 'São Paulo', 'SP', '01305-000', DATEADD(DAY, -30, GETDATE())),
    ('Indústrias XYZ Ltda', 'XYZ Indústria', '11.222.333/0001-44', 'contato@xyz.ind.br', '(11) 4567-8901', 
     'Rua dos Industriais, 2000', 'Guarulhos', 'SP', '07040-000', DATEADD(DAY, -60, GETDATE())),
    ('Serviços Mega Ltda', 'Mega Serviços', '55.666.777/0001-88', 'mega@servicos.com', '(11) 5678-9012', 
     'Av. Brigadeiro, 1500', 'São Paulo', 'SP', '01402-000', DATEADD(DAY, -90, GETDATE()));
GO

-- Adicionar FK na tabela OrdemServico (se ainda não existe)
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OrdemServico_Tecnico')
BEGIN
    -- Primeiro, adicionar coluna TecnicoId se não existir
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('OrdemServico') AND name = 'TecnicoId')
    BEGIN
        ALTER TABLE OrdemServico ADD TecnicoId INT NULL;
    END
    
    -- Atualizar registros existentes com base no nome do técnico
    UPDATE os
    SET os.TecnicoId = t.Id
    FROM OrdemServico os
    INNER JOIN Tecnico t ON os.Tecnico = t.Nome;
    
    -- Criar FK
    ALTER TABLE OrdemServico
    ADD CONSTRAINT FK_OrdemServico_Tecnico
    FOREIGN KEY (TecnicoId) REFERENCES Tecnico(Id);
END
GO

-- Adicionar coluna ClienteId se não existir
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('OrdemServico') AND name = 'ClienteId')
BEGIN
    ALTER TABLE OrdemServico ADD ClienteId INT NULL;
    
    -- Criar FK
    ALTER TABLE OrdemServico
    ADD CONSTRAINT FK_OrdemServico_Cliente
    FOREIGN KEY (ClienteId) REFERENCES Cliente(Id);
END
GO

-- Atualizar algumas OS com clientes
UPDATE OrdemServico SET ClienteId = 1 WHERE Id IN (1, 2);
UPDATE OrdemServico SET ClienteId = 2 WHERE Id IN (3, 4);
UPDATE OrdemServico SET ClienteId = 3 WHERE Id IN (5, 6);
UPDATE OrdemServico SET ClienteId = 4 WHERE Id IN (7, 8);
GO

PRINT 'Setup estendido concluído com sucesso!';
PRINT 'Técnicos cadastrados: ' + CAST((SELECT COUNT(*) FROM Tecnico) AS VARCHAR);
PRINT 'Clientes cadastrados: ' + CAST((SELECT COUNT(*) FROM Cliente) AS VARCHAR);
GO
