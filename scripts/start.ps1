# ============================================
# LegacyProcs - Start Script (Windows)
# ============================================

Write-Host "🚀 Starting LegacyProcs..." -ForegroundColor Green

# Check if Docker is running
Write-Host "🔍 Checking Docker..." -ForegroundColor Yellow
docker info > $null 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Docker is not running. Please start Docker Desktop." -ForegroundColor Red
    exit 1
}

# Check if .env exists
if (-not (Test-Path ".env")) {
    Write-Host "⚠️  .env file not found. Creating from .env.example..." -ForegroundColor Yellow
    Copy-Item ".env.example" ".env"
}

# Pull latest images (optional)
Write-Host "📥 Pulling latest images..." -ForegroundColor Yellow
docker-compose pull

# Build and start services
Write-Host "🏗️  Building and starting services..." -ForegroundColor Yellow
docker-compose up -d --build

# Wait for services to be healthy
Write-Host "⏳ Waiting for services to be healthy..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Check status
Write-Host "`n📊 Service Status:" -ForegroundColor Green
docker-compose ps

# Show URLs
Write-Host "`n✅ LegacyProcs is running!" -ForegroundColor Green
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "🌐 Frontend:  http://localhost:4200" -ForegroundColor Cyan
Write-Host "🔌 Backend:   http://localhost:5000" -ForegroundColor Cyan
Write-Host "📚 Swagger:   http://localhost:5000/swagger" -ForegroundColor Cyan
Write-Host "🗄️  Database:  localhost:1433" -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan

# Show logs
Write-Host "`n📋 Showing logs (Ctrl+C to exit)..." -ForegroundColor Yellow
docker-compose logs -f
