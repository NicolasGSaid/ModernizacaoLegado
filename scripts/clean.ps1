# ============================================
# LegacyProcs - Clean Script (Windows)
# ============================================

Write-Host "🧹 Cleaning LegacyProcs..." -ForegroundColor Yellow

# Confirm action
$confirmation = Read-Host "This will remove all containers, volumes, and images. Continue? (y/N)"
if ($confirmation -ne 'y') {
    Write-Host "❌ Cancelled" -ForegroundColor Red
    exit 0
}

# Stop and remove containers
Write-Host "🛑 Stopping containers..." -ForegroundColor Yellow
docker-compose down -v

# Remove images
Write-Host "🗑️  Removing images..." -ForegroundColor Yellow
docker rmi legacyprocs-backend legacyprocs-frontend 2>$null

# Prune system
Write-Host "🧹 Pruning Docker system..." -ForegroundColor Yellow
docker system prune -f

Write-Host "✅ Cleanup complete!" -ForegroundColor Green
