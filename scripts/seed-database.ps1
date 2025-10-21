# ============================================
# Script de População do Banco de Dados
# LegacyProcs
# ============================================

Write-Host "🌱 Populando banco de dados..." -ForegroundColor Green

# Configurações
$ServerInstance = "localhost,1433"
$Database = "LegacyProcs"
$Username = "sa"
$Password = "LegacyProcs@2025"
$ScriptPath = "../database/seed-data.sql"

# Verificar se o SQL Server está acessível
Write-Host "🔍 Verificando conexão com SQL Server..." -ForegroundColor Yellow

try {
    # Tentar conexão
    $connectionString = "Server=$ServerInstance;Database=master;User Id=$Username;Password=$Password;TrustServerCertificate=True;"
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    $connection.Close()
    Write-Host "✅ Conexão estabelecida com sucesso!" -ForegroundColor Green
}
catch {
    Write-Host "❌ Erro ao conectar ao SQL Server: $_" -ForegroundColor Red
    Write-Host "💡 Certifique-se de que o SQL Server está rodando (docker-compose up -d)" -ForegroundColor Yellow
    exit 1
}

# Executar script de seed
Write-Host "📊 Executando script de população..." -ForegroundColor Yellow

try {
    # Usando sqlcmd (se disponível)
    if (Get-Command sqlcmd -ErrorAction SilentlyContinue) {
        sqlcmd -S $ServerInstance -U $Username -P $Password -d $Database -i $ScriptPath
        Write-Host "✅ Dados inseridos com sucesso!" -ForegroundColor Green
    }
    else {
        Write-Host "⚠️  sqlcmd não encontrado. Tentando método alternativo..." -ForegroundColor Yellow
        
        # Ler e executar script
        $scriptContent = Get-Content $ScriptPath -Raw
        $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
        $connection.Open()
        
        $command = $connection.CreateCommand()
        $command.CommandText = $scriptContent
        $command.CommandTimeout = 300  # 5 minutos
        $command.ExecuteNonQuery()
        
        $connection.Close()
        Write-Host "✅ Dados inseridos com sucesso!" -ForegroundColor Green
    }
}
catch {
    Write-Host "❌ Erro ao executar script: $_" -ForegroundColor Red
    exit 1
}

# Mostrar estatísticas
Write-Host "`n📈 Estatísticas do banco:" -ForegroundColor Cyan

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    # Contar registros
    $queries = @{
        "Clientes" = "SELECT COUNT(*) FROM Cliente"
        "Técnicos" = "SELECT COUNT(*) FROM Tecnico"
        "Ordens de Serviço" = "SELECT COUNT(*) FROM OrdemServico"
    }
    
    foreach ($table in $queries.Keys) {
        $command = $connection.CreateCommand()
        $command.CommandText = $queries[$table]
        $count = $command.ExecuteScalar()
        Write-Host "  - $table: $count" -ForegroundColor White
    }
    
    $connection.Close()
}
catch {
    Write-Host "⚠️  Não foi possível obter estatísticas" -ForegroundColor Yellow
}

Write-Host "`n✅ Banco de dados populado com sucesso!" -ForegroundColor Green
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "🌐 Acesse: http://localhost:4200" -ForegroundColor Cyan
Write-Host "🔌 API: http://localhost:5000" -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
