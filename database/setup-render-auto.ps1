# Script Automático para Criar Tabelas no Render
# Execute este script no PowerShell

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "  SETUP AUTOMÁTICO - RENDER DATABASE" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

# Verificar se psql está instalado
$psqlPath = Get-Command psql -ErrorAction SilentlyContinue

if (-not $psqlPath) {
    Write-Host "❌ PostgreSQL Client (psql) não encontrado!" -ForegroundColor Red
    Write-Host ""
    Write-Host "📥 Instalando PostgreSQL Client via Chocolatey..." -ForegroundColor Yellow
    Write-Host ""
    
    # Verificar se Chocolatey está instalado
    $chocoPath = Get-Command choco -ErrorAction SilentlyContinue
    
    if (-not $chocoPath) {
        Write-Host "❌ Chocolatey não encontrado!" -ForegroundColor Red
        Write-Host ""
        Write-Host "📋 OPÇÕES:" -ForegroundColor Yellow
        Write-Host "1. Instale o Chocolatey: https://chocolatey.org/install" -ForegroundColor White
        Write-Host "2. Ou baixe o PostgreSQL: https://www.postgresql.org/download/windows/" -ForegroundColor White
        Write-Host ""
        Write-Host "Depois execute este script novamente!" -ForegroundColor Yellow
        Read-Host "Pressione ENTER para sair"
        exit 1
    }
    
    Write-Host "📥 Instalando PostgreSQL..." -ForegroundColor Yellow
    choco install postgresql -y
    
    # Atualizar PATH
    $env:Path = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User")
    
    Write-Host "✅ PostgreSQL instalado!" -ForegroundColor Green
    Write-Host ""
}

Write-Host "✅ PostgreSQL Client encontrado!" -ForegroundColor Green
Write-Host ""

# Configurar variáveis
$env:PGPASSWORD = 'v5SDGvOKDPLdYATDqkjZ5wcjgVVWhMdN'
$host_db = 'dpg-d3qp1hvdiees73ahefng-a.oregon-postgres.render.com'
$user_db = 'legacyprocs_user'
$database = 'legacyprocs'

Write-Host "🔌 Conectando ao banco de dados Render..." -ForegroundColor Yellow
Write-Host "   Host: $host_db" -ForegroundColor Gray
Write-Host "   Database: $database" -ForegroundColor Gray
Write-Host ""

# SQL Script
$sqlScript = @"
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
SELECT 'Tecnicos', COUNT(*) FROM "Tecnicos"
UNION ALL
SELECT 'Clientes', COUNT(*) FROM "Clientes";
"@

# Salvar SQL em arquivo temporário
$tempSqlFile = [System.IO.Path]::GetTempFileName() + ".sql"
$sqlScript | Out-File -FilePath $tempSqlFile -Encoding UTF8

Write-Host "📝 Executando script SQL..." -ForegroundColor Yellow
Write-Host ""

try {
    # Executar psql
    $output = psql -h $host_db -U $user_db -d $database -f $tempSqlFile 2>&1
    
    Write-Host "✅ Script executado com sucesso!" -ForegroundColor Green
    Write-Host ""
    Write-Host "📊 RESULTADO:" -ForegroundColor Cyan
    Write-Host $output -ForegroundColor White
    Write-Host ""
    Write-Host "==================================================" -ForegroundColor Cyan
    Write-Host "  ✅ BANCO DE DADOS CONFIGURADO COM SUCESSO!" -ForegroundColor Green
    Write-Host "==================================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "🎉 Agora abra o frontend e teste!" -ForegroundColor Yellow
    Write-Host "   URL: https://legacyprocs-frontend.onrender.com" -ForegroundColor Cyan
    Write-Host ""
}
catch {
    Write-Host "❌ ERRO ao executar script!" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
}
finally {
    # Limpar arquivo temporário
    Remove-Item $tempSqlFile -ErrorAction SilentlyContinue
}

Read-Host "Pressione ENTER para sair"
