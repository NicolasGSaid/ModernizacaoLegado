# ============================================
# LegacyProcs - Stop Script (Windows)
# ============================================

Write-Host "🛑 Stopping LegacyProcs..." -ForegroundColor Yellow

# Stop services
docker-compose stop

Write-Host "✅ Services stopped!" -ForegroundColor Green
Write-Host "`n📊 Service Status:" -ForegroundColor Yellow
docker-compose ps
