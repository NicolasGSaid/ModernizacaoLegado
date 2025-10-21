@echo off
echo Populando banco de dados...
echo.

docker-compose exec -T sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "LegacyProcs@2025" -d LegacyProcs -i /docker-entrypoint-initdb.d/seed-data.sql

echo.
echo Banco de dados populado!
echo.
pause
