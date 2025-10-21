-- ============================================
-- Script de População Massiva de Dados
-- LegacyProcs - Banco de Dados
-- ============================================

USE LegacyProcs;
GO

-- Limpar dados existentes (se necessário)
-- DELETE FROM OrdemServico;
-- DELETE FROM Tecnico;
-- DELETE FROM Cliente;
-- DBCC CHECKIDENT ('OrdemServico', RESEED, 0);
-- DBCC CHECKIDENT ('Tecnico', RESEED, 0);
-- DBCC CHECKIDENT ('Cliente', RESEED, 0);

-- ============================================
-- CLIENTES (100 registros)
-- ============================================

PRINT 'Inserindo Clientes...';

INSERT INTO Cliente (Nome, Email, Telefone, Endereco, DataCadastro)
VALUES
-- Empresas
('Tech Solutions Ltda', 'contato@techsolutions.com.br', '11987654321', 'Av. Paulista, 1000 - São Paulo, SP', GETDATE()),
('Inovação Digital SA', 'comercial@inovacaodigital.com.br', '11987654322', 'Rua Augusta, 2500 - São Paulo, SP', GETDATE()),
('Sistemas Integrados ME', 'atendimento@sistemasintegrados.com.br', '11987654323', 'Av. Faria Lima, 3000 - São Paulo, SP', GETDATE()),
('DataCenter Brasil', 'suporte@datacenterbrasil.com.br', '11987654324', 'Rua Vergueiro, 4000 - São Paulo, SP', GETDATE()),
('Cloud Computing Corp', 'info@cloudcomputing.com.br', '11987654325', 'Av. Berrini, 5000 - São Paulo, SP', GETDATE()),
('Automação Industrial Ltda', 'vendas@automacaoindustrial.com.br', '11987654326', 'Rua Industrial, 100 - São Bernardo, SP', GETDATE()),
('Logística Express', 'contato@logisticaexpress.com.br', '11987654327', 'Av. dos Bandeirantes, 2000 - Guarulhos, SP', GETDATE()),
('Varejo Digital SA', 'comercial@varejodigital.com.br', '11987654328', 'Shopping Center, Loja 200 - São Paulo, SP', GETDATE()),
('Financeira Prime', 'atendimento@financeiraprime.com.br', '11987654329', 'Av. Brigadeiro, 1500 - São Paulo, SP', GETDATE()),
('Saúde Conectada', 'suporte@saudeconectada.com.br', '11987654330', 'Rua dos Médicos, 300 - São Paulo, SP', GETDATE()),

-- Pessoas Físicas
('João Silva Santos', 'joao.silva@email.com', '11998765001', 'Rua das Flores, 123 - São Paulo, SP', GETDATE()),
('Maria Oliveira Costa', 'maria.oliveira@email.com', '11998765002', 'Av. Brasil, 456 - São Paulo, SP', GETDATE()),
('Pedro Henrique Souza', 'pedro.souza@email.com', '11998765003', 'Rua São João, 789 - São Paulo, SP', GETDATE()),
('Ana Paula Ferreira', 'ana.ferreira@email.com', '11998765004', 'Av. Ipiranga, 321 - São Paulo, SP', GETDATE()),
('Carlos Eduardo Lima', 'carlos.lima@email.com', '11998765005', 'Rua da Consolação, 654 - São Paulo, SP', GETDATE()),
('Juliana Martins Rocha', 'juliana.rocha@email.com', '11998765006', 'Av. Rebouças, 987 - São Paulo, SP', GETDATE()),
('Ricardo Alves Pereira', 'ricardo.pereira@email.com', '11998765007', 'Rua Oscar Freire, 147 - São Paulo, SP', GETDATE()),
('Fernanda Costa Ribeiro', 'fernanda.ribeiro@email.com', '11998765008', 'Av. Europa, 258 - São Paulo, SP', GETDATE()),
('Lucas Henrique Dias', 'lucas.dias@email.com', '11998765009', 'Rua Haddock Lobo, 369 - São Paulo, SP', GETDATE()),
('Patricia Santos Almeida', 'patricia.almeida@email.com', '11998765010', 'Av. Angélica, 741 - São Paulo, SP', GETDATE()),
('Roberto Carlos Nunes', 'roberto.nunes@email.com', '11998765011', 'Rua Estados Unidos, 852 - São Paulo, SP', GETDATE()),
('Camila Rodrigues Silva', 'camila.silva@email.com', '11998765012', 'Av. Cidade Jardim, 963 - São Paulo, SP', GETDATE()),
('Bruno Henrique Costa', 'bruno.costa@email.com', '11998765013', 'Rua Bela Cintra, 159 - São Paulo, SP', GETDATE()),
('Amanda Ferreira Lima', 'amanda.lima@email.com', '11998765014', 'Av. Nove de Julho, 357 - São Paulo, SP', GETDATE()),
('Felipe Santos Oliveira', 'felipe.oliveira@email.com', '11998765015', 'Rua Augusta, 753 - São Paulo, SP', GETDATE()),
('Gabriela Costa Martins', 'gabriela.martins@email.com', '11998765016', 'Av. Ibirapuera, 951 - São Paulo, SP', GETDATE()),
('Thiago Alves Souza', 'thiago.souza@email.com', '11998765017', 'Rua Pamplona, 357 - São Paulo, SP', GETDATE()),
('Mariana Oliveira Dias', 'mariana.dias@email.com', '11998765018', 'Av. Santo Amaro, 159 - São Paulo, SP', GETDATE()),
('Rafael Lima Pereira', 'rafael.pereira@email.com', '11998765019', 'Rua Joaquim Floriano, 753 - São Paulo, SP', GETDATE()),
('Larissa Santos Costa', 'larissa.costa@email.com', '11998765020', 'Av. Juscelino Kubitschek, 951 - São Paulo, SP', GETDATE()),

-- Mais clientes (continuação até 100)
('Gustavo Henrique Silva', 'gustavo.silva@email.com', '11998765021', 'Rua Cardeal Arcoverde, 123 - São Paulo, SP', GETDATE()),
('Beatriz Costa Lima', 'beatriz.lima@email.com', '11998765022', 'Av. Sumaré, 456 - São Paulo, SP', GETDATE()),
('Diego Santos Rocha', 'diego.rocha@email.com', '11998765023', 'Rua Teodoro Sampaio, 789 - São Paulo, SP', GETDATE()),
('Isabela Ferreira Dias', 'isabela.dias@email.com', '11998765024', 'Av. Henrique Schaumann, 321 - São Paulo, SP', GETDATE()),
('Vinicius Alves Costa', 'vinicius.costa@email.com', '11998765025', 'Rua Fradique Coutinho, 654 - São Paulo, SP', GETDATE()),
('Carolina Martins Silva', 'carolina.silva@email.com', '11998765026', 'Av. Eusébio Matoso, 987 - São Paulo, SP', GETDATE()),
('Leonardo Lima Santos', 'leonardo.santos@email.com', '11998765027', 'Rua Mourato Coelho, 147 - São Paulo, SP', GETDATE()),
('Natalia Costa Oliveira', 'natalia.oliveira@email.com', '11998765028', 'Av. Pedroso de Morais, 258 - São Paulo, SP', GETDATE()),
('Rodrigo Santos Pereira', 'rodrigo.pereira@email.com', '11998765029', 'Rua Aspicuelta, 369 - São Paulo, SP', GETDATE()),
('Vanessa Alves Lima', 'vanessa.lima@email.com', '11998765030', 'Av. Rebouças, 741 - São Paulo, SP', GETDATE()),

-- Empresas adicionais
('Tecnologia Avançada Ltda', 'contato@tecavancada.com.br', '11987654331', 'Rua Tech, 1000 - São Paulo, SP', GETDATE()),
('Soluções Empresariais SA', 'vendas@solempresariais.com.br', '11987654332', 'Av. Empresarial, 2000 - São Paulo, SP', GETDATE()),
('Consultoria Estratégica', 'info@consultoriaestrategica.com.br', '11987654333', 'Rua Consultores, 3000 - São Paulo, SP', GETDATE()),
('Desenvolvimento Web Pro', 'contato@devwebpro.com.br', '11987654334', 'Av. Digital, 4000 - São Paulo, SP', GETDATE()),
('Marketing Digital 360', 'comercial@marketing360.com.br', '11987654335', 'Rua Marketing, 5000 - São Paulo, SP', GETDATE()),
('E-commerce Solutions', 'suporte@ecommercesol.com.br', '11987654336', 'Av. Comércio, 6000 - São Paulo, SP', GETDATE()),
('Segurança Cibernética', 'atendimento@segcibernetica.com.br', '11987654337', 'Rua Segurança, 7000 - São Paulo, SP', GETDATE()),
('Infraestrutura TI', 'contato@infrati.com.br', '11987654338', 'Av. Infraestrutura, 8000 - São Paulo, SP', GETDATE()),
('Banco de Dados Corp', 'vendas@bancodados.com.br', '11987654339', 'Rua Database, 9000 - São Paulo, SP', GETDATE()),
('Redes e Telecom', 'info@redestelecom.com.br', '11987654340', 'Av. Redes, 10000 - São Paulo, SP', GETDATE()),

-- Mais 50 clientes variados
('André Luiz Ferreira', 'andre.ferreira@email.com', '11998765031', 'Rua Exemplo, 100 - São Paulo, SP', GETDATE()),
('Bianca Rodrigues', 'bianca.rodrigues@email.com', '11998765032', 'Av. Teste, 200 - São Paulo, SP', GETDATE()),
('César Augusto', 'cesar.augusto@email.com', '11998765033', 'Rua Demo, 300 - São Paulo, SP', GETDATE()),
('Daniela Cristina', 'daniela.cristina@email.com', '11998765034', 'Av. Sample, 400 - São Paulo, SP', GETDATE()),
('Eduardo Henrique', 'eduardo.henrique@email.com', '11998765035', 'Rua Mock, 500 - São Paulo, SP', GETDATE()),
('Fabiana Santos', 'fabiana.santos@email.com', '11998765036', 'Av. Data, 600 - São Paulo, SP', GETDATE()),
('Gabriel Costa', 'gabriel.costa@email.com', '11998765037', 'Rua Info, 700 - São Paulo, SP', GETDATE()),
('Helena Silva', 'helena.silva@email.com', '11998765038', 'Av. Tech, 800 - São Paulo, SP', GETDATE()),
('Igor Alves', 'igor.alves@email.com', '11998765039', 'Rua Dev, 900 - São Paulo, SP', GETDATE()),
('Jéssica Lima', 'jessica.lima@email.com', '11998765040', 'Av. Code, 1000 - São Paulo, SP', GETDATE()),
('Kevin Martins', 'kevin.martins@email.com', '11998765041', 'Rua Prog, 1100 - São Paulo, SP', GETDATE()),
('Letícia Rocha', 'leticia.rocha@email.com', '11998765042', 'Av. Soft, 1200 - São Paulo, SP', GETDATE()),
('Marcelo Dias', 'marcelo.dias@email.com', '11998765043', 'Rua Hard, 1300 - São Paulo, SP', GETDATE()),
('Natasha Pereira', 'natasha.pereira@email.com', '11998765044', 'Av. Net, 1400 - São Paulo, SP', GETDATE()),
('Otávio Nunes', 'otavio.nunes@email.com', '11998765045', 'Rua Web, 1500 - São Paulo, SP', GETDATE()),
('Priscila Costa', 'priscila.costa@email.com', '11998765046', 'Av. App, 1600 - São Paulo, SP', GETDATE()),
('Quintino Silva', 'quintino.silva@email.com', '11998765047', 'Rua API, 1700 - São Paulo, SP', GETDATE()),
('Renata Oliveira', 'renata.oliveira@email.com', '11998765048', 'Av. REST, 1800 - São Paulo, SP', GETDATE()),
('Samuel Santos', 'samuel.santos@email.com', '11998765049', 'Rua HTTP, 1900 - São Paulo, SP', GETDATE()),
('Tatiana Ferreira', 'tatiana.ferreira@email.com', '11998765050', 'Av. JSON, 2000 - São Paulo, SP', GETDATE()),
('Ulisses Lima', 'ulisses.lima@email.com', '11998765051', 'Rua XML, 2100 - São Paulo, SP', GETDATE()),
('Viviane Alves', 'viviane.alves@email.com', '11998765052', 'Av. SQL, 2200 - São Paulo, SP', GETDATE()),
('Wagner Costa', 'wagner.costa@email.com', '11998765053', 'Rua NoSQL, 2300 - São Paulo, SP', GETDATE()),
('Xuxa Martins', 'xuxa.martins@email.com', '11998765054', 'Av. Cloud, 2400 - São Paulo, SP', GETDATE()),
('Yuri Rocha', 'yuri.rocha@email.com', '11998765055', 'Rua Docker, 2500 - São Paulo, SP', GETDATE()),
('Zilda Dias', 'zilda.dias@email.com', '11998765056', 'Av. K8s, 2600 - São Paulo, SP', GETDATE()),
('Alberto Pereira', 'alberto.pereira@email.com', '11998765057', 'Rua CI, 2700 - São Paulo, SP', GETDATE()),
('Bruna Nunes', 'bruna.nunes@email.com', '11998765058', 'Av. CD, 2800 - São Paulo, SP', GETDATE()),
('Claudio Costa', 'claudio.costa@email.com', '11998765059', 'Rua DevOps, 2900 - São Paulo, SP', GETDATE()),
('Denise Silva', 'denise.silva@email.com', '11998765060', 'Av. Agile, 3000 - São Paulo, SP', GETDATE()),
('Emerson Oliveira', 'emerson.oliveira@email.com', '11998765061', 'Rua Scrum, 3100 - São Paulo, SP', GETDATE()),
('Flávia Santos', 'flavia.santos@email.com', '11998765062', 'Av. Kanban, 3200 - São Paulo, SP', GETDATE()),
('Gilberto Ferreira', 'gilberto.ferreira@email.com', '11998765063', 'Rua Sprint, 3300 - São Paulo, SP', GETDATE()),
('Heloisa Lima', 'heloisa.lima@email.com', '11998765064', 'Av. Backlog, 3400 - São Paulo, SP', GETDATE()),
('Ivo Alves', 'ivo.alves@email.com', '11998765065', 'Rua Story, 3500 - São Paulo, SP', GETDATE()),
('Joana Costa', 'joana.costa@email.com', '11998765066', 'Av. Epic, 3600 - São Paulo, SP', GETDATE()),
('Klaus Martins', 'klaus.martins@email.com', '11998765067', 'Rua Task, 3700 - São Paulo, SP', GETDATE()),
('Luana Rocha', 'luana.rocha@email.com', '11998765068', 'Av. Bug, 3800 - São Paulo, SP', GETDATE()),
('Márcio Dias', 'marcio.dias@email.com', '11998765069', 'Rua Fix, 3900 - São Paulo, SP', GETDATE()),
('Núbia Pereira', 'nubia.pereira@email.com', '11998765070', 'Av. Feature, 4000 - São Paulo, SP', GETDATE());

PRINT 'Clientes inseridos com sucesso!';

-- ============================================
-- TÉCNICOS (30 registros)
-- ============================================

PRINT 'Inserindo Técnicos...';

INSERT INTO Tecnico (Nome, Email, Telefone, Especialidade, Status, DataCadastro)
VALUES
-- Técnicos Ativos
('João Carlos Técnico', 'joao.tecnico@legacyprocs.com', '11991111001', 'Elétrica', 'Ativo', GETDATE()),
('Maria Técnica Silva', 'maria.tecnica@legacyprocs.com', '11991111002', 'Hidráulica', 'Ativo', GETDATE()),
('Pedro Especialista', 'pedro.especialista@legacyprocs.com', '11991111003', 'Ar Condicionado', 'Ativo', GETDATE()),
('Ana Paula Tech', 'ana.tech@legacyprocs.com', '11991111004', 'Refrigeração', 'Ativo', GETDATE()),
('Carlos Manutenção', 'carlos.manutencao@legacyprocs.com', '11991111005', 'Elétrica', 'Ativo', GETDATE()),
('Juliana Instaladora', 'juliana.instaladora@legacyprocs.com', '11991111006', 'Redes', 'Ativo', GETDATE()),
('Ricardo Técnico', 'ricardo.tecnico@legacyprocs.com', '11991111007', 'TI', 'Ativo', GETDATE()),
('Fernanda Suporte', 'fernanda.suporte@legacyprocs.com', '11991111008', 'Hardware', 'Ativo', GETDATE()),
('Lucas Especialista', 'lucas.especialista@legacyprocs.com', '11991111009', 'Software', 'Ativo', GETDATE()),
('Patricia Tech', 'patricia.tech@legacyprocs.com', '11991111010', 'Elétrica', 'Ativo', GETDATE()),
('Roberto Manutenção', 'roberto.manutencao@legacyprocs.com', '11991111011', 'Hidráulica', 'Ativo', GETDATE()),
('Camila Técnica', 'camila.tecnica@legacyprocs.com', '11991111012', 'Ar Condicionado', 'Ativo', GETDATE()),
('Bruno Instalador', 'bruno.instalador@legacyprocs.com', '11991111013', 'Refrigeração', 'Ativo', GETDATE()),
('Amanda Suporte', 'amanda.suporte@legacyprocs.com', '11991111014', 'Elétrica', 'Ativo', GETDATE()),
('Felipe Especialista', 'felipe.especialista@legacyprocs.com', '11991111015', 'Redes', 'Ativo', GETDATE()),
('Gabriela Tech', 'gabriela.tech@legacyprocs.com', '11991111016', 'TI', 'Ativo', GETDATE()),
('Thiago Técnico', 'thiago.tecnico@legacyprocs.com', '11991111017', 'Hardware', 'Ativo', GETDATE()),
('Mariana Manutenção', 'mariana.manutencao@legacyprocs.com', '11991111018', 'Software', 'Ativo', GETDATE()),
('Rafael Instalador', 'rafael.instalador@legacyprocs.com', '11991111019', 'Elétrica', 'Ativo', GETDATE()),
('Larissa Suporte', 'larissa.suporte@legacyprocs.com', '11991111020', 'Hidráulica', 'Ativo', GETDATE()),

-- Técnicos em Férias
('Gustavo Férias', 'gustavo.ferias@legacyprocs.com', '11991111021', 'Ar Condicionado', 'Férias', GETDATE()),
('Beatriz Ausente', 'beatriz.ausente@legacyprocs.com', '11991111022', 'Refrigeração', 'Férias', GETDATE()),
('Diego Descanso', 'diego.descanso@legacyprocs.com', '11991111023', 'Elétrica', 'Férias', GETDATE()),

-- Técnicos Inativos
('Isabela Inativa', 'isabela.inativa@legacyprocs.com', '11991111024', 'Redes', 'Inativo', GETDATE()),
('Vinicius Afastado', 'vinicius.afastado@legacyprocs.com', '11991111025', 'TI', 'Inativo', GETDATE()),

-- Mais técnicos ativos
('Carolina Técnica', 'carolina.tecnica@legacyprocs.com', '11991111026', 'Hardware', 'Ativo', GETDATE()),
('Leonardo Especialista', 'leonardo.especialista@legacyprocs.com', '11991111027', 'Software', 'Ativo', GETDATE()),
('Natalia Suporte', 'natalia.suporte@legacyprocs.com', '11991111028', 'Elétrica', 'Ativo', GETDATE()),
('Rodrigo Manutenção', 'rodrigo.manutencao@legacyprocs.com', '11991111029', 'Hidráulica', 'Ativo', GETDATE()),
('Vanessa Instaladora', 'vanessa.instaladora@legacyprocs.com', '11991111030', 'Ar Condicionado', 'Ativo', GETDATE());

PRINT 'Técnicos inseridos com sucesso!';

-- ============================================
-- ORDENS DE SERVIÇO (500 registros)
-- ============================================

PRINT 'Inserindo Ordens de Serviço (isso pode levar alguns segundos)...';

DECLARE @i INT = 1;
DECLARE @ClienteId INT;
DECLARE @TecnicoId INT;
DECLARE @Status NVARCHAR(20);
DECLARE @DataCriacao DATETIME;
DECLARE @Titulo NVARCHAR(200);
DECLARE @Descricao NVARCHAR(MAX);

WHILE @i <= 500
BEGIN
    -- Cliente aleatório (1-70)
    SET @ClienteId = (ABS(CHECKSUM(NEWID())) % 70) + 1;
    
    -- Técnico aleatório (1-30)
    SET @TecnicoId = (ABS(CHECKSUM(NEWID())) % 30) + 1;
    
    -- Status aleatório
    SET @Status = CASE (ABS(CHECKSUM(NEWID())) % 3)
        WHEN 0 THEN 'Pendente'
        WHEN 1 THEN 'Em Andamento'
        ELSE 'Concluída'
    END;
    
    -- Data de criação nos últimos 90 dias
    SET @DataCriacao = DATEADD(DAY, -(ABS(CHECKSUM(NEWID())) % 90), GETDATE());
    
    -- Título e descrição variados
    SET @Titulo = CASE (ABS(CHECKSUM(NEWID())) % 10)
        WHEN 0 THEN 'Instalação de Sistema Elétrico - OS #' + CAST(@i AS NVARCHAR)
        WHEN 1 THEN 'Manutenção Preventiva de Ar Condicionado - OS #' + CAST(@i AS NVARCHAR)
        WHEN 2 THEN 'Reparo em Rede de Computadores - OS #' + CAST(@i AS NVARCHAR)
        WHEN 3 THEN 'Instalação de Câmeras de Segurança - OS #' + CAST(@i AS NVARCHAR)
        WHEN 4 THEN 'Manutenção Corretiva de Hardware - OS #' + CAST(@i AS NVARCHAR)
        WHEN 5 THEN 'Configuração de Servidor - OS #' + CAST(@i AS NVARCHAR)
        WHEN 6 THEN 'Instalação de Sistema de Refrigeração - OS #' + CAST(@i AS NVARCHAR)
        WHEN 7 THEN 'Reparo Hidráulico Emergencial - OS #' + CAST(@i AS NVARCHAR)
        WHEN 8 THEN 'Upgrade de Infraestrutura TI - OS #' + CAST(@i AS NVARCHAR)
        ELSE 'Suporte Técnico Geral - OS #' + CAST(@i AS NVARCHAR)
    END;
    
    SET @Descricao = 'Ordem de serviço criada automaticamente para testes. ' +
                     'Cliente solicitou atendimento técnico especializado. ' +
                     'Prioridade: ' + CASE (ABS(CHECKSUM(NEWID())) % 3)
                         WHEN 0 THEN 'Baixa'
                         WHEN 1 THEN 'Média'
                         ELSE 'Alta'
                     END + '. ' +
                     'Observações: Verificar disponibilidade de materiais e agendar com antecedência.';
    
    INSERT INTO OrdemServico (Titulo, Descricao, ClienteId, TecnicoId, Status, DataCriacao, DataAtualizacao)
    VALUES (@Titulo, @Descricao, @ClienteId, @TecnicoId, @Status, @DataCriacao, @DataCriacao);
    
    SET @i = @i + 1;
    
    -- Feedback a cada 100 registros
    IF @i % 100 = 0
        PRINT 'Inseridas ' + CAST(@i AS NVARCHAR) + ' ordens de serviço...';
END;

PRINT 'Ordens de Serviço inseridas com sucesso!';

-- ============================================
-- ESTATÍSTICAS
-- ============================================

PRINT '';
PRINT '============================================';
PRINT 'POPULAÇÃO DE DADOS CONCLUÍDA!';
PRINT '============================================';
PRINT '';
PRINT 'Estatísticas:';
PRINT '- Clientes: ' + CAST((SELECT COUNT(*) FROM Cliente) AS NVARCHAR);
PRINT '- Técnicos: ' + CAST((SELECT COUNT(*) FROM Tecnico) AS NVARCHAR);
PRINT '- Ordens de Serviço: ' + CAST((SELECT COUNT(*) FROM OrdemServico) AS NVARCHAR);
PRINT '';
PRINT 'Distribuição de Status das Ordens:';
PRINT '- Pendente: ' + CAST((SELECT COUNT(*) FROM OrdemServico WHERE Status = 'Pendente') AS NVARCHAR);
PRINT '- Em Andamento: ' + CAST((SELECT COUNT(*) FROM OrdemServico WHERE Status = 'Em Andamento') AS NVARCHAR);
PRINT '- Concluída: ' + CAST((SELECT COUNT(*) FROM OrdemServico WHERE Status = 'Concluída') AS NVARCHAR);
PRINT '';
PRINT 'Banco de dados populado com sucesso!';
PRINT '============================================';

GO
