@echo off
chcp 65001 >nul
echo ==================================================
echo   SETUP AUTOMATICO - RENDER DATABASE
echo ==================================================
echo.

set PGPASSWORD=v5SDGvOKDPLdYATDqkjZ5wcjgVVWhMdN

echo Conectando ao banco de dados Render...
echo.

psql -h dpg-d3qp1hvdiees73ahefng-a.oregon-postgres.render.com -U legacyprocs_user -d legacyprocs -f setup-render-quick.sql

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ==================================================
    echo   SUCESSO! Banco de dados configurado!
    echo ==================================================
    echo.
    echo Agora abra o frontend e teste!
    echo URL: https://legacyprocs-frontend.onrender.com
    echo.
) else (
    echo.
    echo ERRO! Verifique se o PostgreSQL esta instalado.
    echo Baixe em: https://www.postgresql.org/download/windows/
    echo.
)

pause
