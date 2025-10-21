using Microsoft.Extensions.Diagnostics.HealthChecks;
using LegacyProcs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LegacyProcs.Infrastructure.HealthChecks;

/// <summary>
/// Health check customizado para banco de dados com timeout configurável
/// Seguindo Global Rules - Seção 14: SLO/SLI e Orçamento de Erros
/// </summary>
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly AppDbContext _context;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public DatabaseHealthCheck(AppDbContext context, ILogger<DatabaseHealthCheck> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Iniciando health check do banco de dados");

            // Usar timeout de 5 segundos
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken, timeoutCts.Token);

            // Testar conexão simples com mais detalhes
            try 
            {
                var canConnect = await _context.Database.CanConnectAsync(combinedCts.Token);
                
                if (!canConnect)
                {
                    _logger.LogWarning("CanConnectAsync retornou false para connection string: {ConnectionString}", 
                        _context.Database.GetConnectionString());
                    return HealthCheckResult.Unhealthy("CanConnectAsync retornou false");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao testar CanConnectAsync com connection string: {ConnectionString}", 
                    _context.Database.GetConnectionString());
                return HealthCheckResult.Unhealthy($"Erro CanConnect: {ex.Message}");
            }

            // Testar query simples
            var count = await _context.Cliente.CountAsync(combinedCts.Token);
            
            _logger.LogDebug("Health check do banco concluído com sucesso. Clientes: {Count}", count);
            
            return HealthCheckResult.Healthy($"Banco conectado. Clientes: {count}");
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "Health check do banco cancelado");
            return HealthCheckResult.Unhealthy("Health check cancelado");
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "Health check do banco expirou (timeout 5s)");
            return HealthCheckResult.Unhealthy("Timeout na conexão com banco (5s)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no health check do banco de dados");
            return HealthCheckResult.Unhealthy($"Erro na conexão: {ex.GetType().Name}");
        }
    }
}
