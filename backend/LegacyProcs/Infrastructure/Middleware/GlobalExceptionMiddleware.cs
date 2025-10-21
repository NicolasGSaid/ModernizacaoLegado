using System.Net;
using System.Text.Json;
using LegacyProcs.Domain.Exceptions;
using FluentValidation;

namespace LegacyProcs.Infrastructure.Middleware;

/// <summary>
/// Middleware global para tratamento de exceções
/// Implementa logging seguro sem exposição de dados sensíveis
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next, 
        ILogger<GlobalExceptionMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log detalhado SEMPRE (temporário para debug)
            _logger.LogError(ex, "Erro não tratado na requisição {Method} {Path}\nTipo: {ExceptionType}\nMensagem: {Message}\nStackTrace: {StackTrace}", 
                context.Request.Method, context.Request.Path, ex.GetType().FullName, ex.Message, ex.StackTrace);
            
            // Log da inner exception se existir
            if (ex.InnerException != null)
            {
                _logger.LogError("Inner Exception: {InnerType} - {InnerMessage}", 
                    ex.InnerException.GetType().FullName, ex.InnerException.Message);
            }
            
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Verificar se a resposta já foi iniciada
        if (context.Response.HasStarted)
        {
            _logger.LogWarning("Não é possível modificar a resposta após ela ter sido iniciada");
            return;
        }

        context.Response.ContentType = "application/json";

        var response = new ErrorResponse();

        switch (exception)
        {
            case ValidationException validationEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = "Dados de entrada inválidos";
                response.Details = validationEx.Errors.Select(e => new ValidationError
                {
                    Field = e.PropertyName,
                    Message = e.ErrorMessage
                }).ToList();
                break;

            case EntityNotFoundException notFoundEx:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = notFoundEx.Message;
                response.Details = new { EntityName = notFoundEx.EntityName, EntityId = notFoundEx.EntityId };
                break;

            case BusinessRuleViolationException businessEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = businessEx.Message;
                response.Details = new { RuleName = businessEx.RuleName };
                break;

            case DomainException domainEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = domainEx.Message;
                break;

            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Message = "Acesso não autorizado";
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "Erro interno do servidor. Contate o administrador.";
                
                // Em desenvolvimento, incluir detalhes do erro
                if (_environment.IsDevelopment())
                {
                    response.Details = new 
                    { 
                        Type = exception.GetType().Name,
                        Message = exception.Message,
                        StackTrace = exception.StackTrace
                    };
                }
                break;
        }

        // Gerar ID único para rastreamento
        response.TraceId = context.TraceIdentifier;
        response.Timestamp = DateTime.UtcNow;

        context.Response.StatusCode = response.StatusCode;

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

/// <summary>
/// Modelo de resposta de erro padronizado
/// </summary>
public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Details { get; set; }
    public string TraceId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Modelo para erros de validação
/// </summary>
public class ValidationError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
