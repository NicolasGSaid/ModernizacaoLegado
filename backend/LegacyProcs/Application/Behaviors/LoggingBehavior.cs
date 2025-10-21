using MediatR;
using System.Diagnostics;

namespace LegacyProcs.Application.Behaviors;

/// <summary>
/// Pipeline behavior para logging automático de requests/responses
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("Iniciando {RequestName}: {@Request}", requestName, request);

        try
        {
            var response = await next();
            
            stopwatch.Stop();
            _logger.LogInformation("Concluído {RequestName} em {ElapsedMilliseconds}ms", 
                requestName, stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Erro em {RequestName} após {ElapsedMilliseconds}ms: {ErrorMessage}", 
                requestName, stopwatch.ElapsedMilliseconds, ex.Message);
            throw;
        }
    }
}
