# ============================================
# Script para Criar Repositório Privado
# LegacyProcs
# ============================================

Write-Host "🔒 Criando repositório privado..." -ForegroundColor Green
Write-Host ""

# Verificar se está na branch correta
$currentBranch = git branch --show-current
Write-Host "📍 Branch atual: $currentBranch" -ForegroundColor Cyan

if ($currentBranch -ne "NicolasDias/Modernizacao") {
    Write-Host "⚠️  Você não está na branch NicolasDias/Modernizacao" -ForegroundColor Yellow
    $continue = Read-Host "Continuar mesmo assim? (s/N)"
    if ($continue -ne "s") {
        exit 0
    }
}

Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "INSTRUÇÕES:" -ForegroundColor Yellow
Write-Host "1. Acesse: https://github.com/new" -ForegroundColor White
Write-Host "2. Repository name: LegacyProcs-Portfolio" -ForegroundColor White
Write-Host "3. Visibility: PRIVATE (marque esta opção!)" -ForegroundColor White
Write-Host "4. NÃO marque README, .gitignore, license" -ForegroundColor White
Write-Host "5. Clique em 'Create repository'" -ForegroundColor White
Write-Host "6. Copie a URL SSH que aparece" -ForegroundColor White
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host ""

# Solicitar URL do novo repo
$repoUrl = Read-Host "Cole aqui a URL SSH do novo repositório"

if ([string]::IsNullOrWhiteSpace($repoUrl)) {
    Write-Host "❌ URL não fornecida. Abortando." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "🔧 Configurando remote..." -ForegroundColor Yellow

# Adicionar remote
try {
    git remote add private $repoUrl
    Write-Host "✅ Remote 'private' adicionado!" -ForegroundColor Green
}
catch {
    Write-Host "⚠️  Remote 'private' já existe. Atualizando..." -ForegroundColor Yellow
    git remote set-url private $repoUrl
}

Write-Host ""
Write-Host "📤 Fazendo push da branch..." -ForegroundColor Yellow

# Push da branch como main
git push private NicolasDias/Modernizacao:main

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Push realizado com sucesso!" -ForegroundColor Green
}
else {
    Write-Host "❌ Erro no push. Verifique as permissões." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "🏷️  Fazendo push das tags..." -ForegroundColor Yellow
git push private --tags

Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "✅ REPOSITÓRIO PRIVADO CRIADO!" -ForegroundColor Green
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host ""
Write-Host "📊 Remotes configurados:" -ForegroundColor Cyan
git remote -v
Write-Host ""

# Perguntar se quer usar apenas o novo repo
Write-Host "❓ Deseja usar APENAS o novo repositório privado?" -ForegroundColor Yellow
Write-Host "   (Isso vai remover o remote 'origin' antigo)" -ForegroundColor Gray
$useOnlyPrivate = Read-Host "   (s/N)"

if ($useOnlyPrivate -eq "s") {
    Write-Host ""
    Write-Host "🔄 Reconfigurando remotes..." -ForegroundColor Yellow
    
    git remote remove origin
    git remote rename private origin
    git branch -M main
    
    Write-Host "✅ Agora você está usando apenas o repo privado!" -ForegroundColor Green
    Write-Host ""
    Write-Host "📊 Novo remote:" -ForegroundColor Cyan
    git remote -v
}

Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "🎯 PRÓXIMOS PASSOS:" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Verifique o repo: https://github.com/NicolasDiasAlest/LegacyProcs-Portfolio" -ForegroundColor White
Write-Host "2. Confirme que tem o ícone de cadeado 🔒" -ForegroundColor White
Write-Host "3. Atualize o Render para usar o novo repo" -ForegroundColor White
Write-Host "4. Faça o deploy!" -ForegroundColor White
Write-Host ""
Write-Host "📖 Mais detalhes: CRIAR_REPO_PRIVADO.md" -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host ""
Write-Host "✅ Concluído!" -ForegroundColor Green
