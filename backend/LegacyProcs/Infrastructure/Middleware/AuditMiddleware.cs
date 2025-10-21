using LegacyProcs.Domain.Entities;
using LegacyProcs.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;

namespace LegacyProcs.Infrastructure.Middleware
{
    /// <summary>
    /// Middleware para auditoria automática de operações - LGPD Compliance
    /// Intercepta todas as requisições e registra operações em dados pessoais
    /// </summary>
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditMiddleware> _logger;

        public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context, IAuditLogRepository auditRepository)
        {
            // Verificar se é uma operação que precisa de auditoria
            if (!DeveAuditar(context))
            {
                await _next(context);
                return;
            }

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var originalBodyStream = context.Response.Body;

            try
            {
                // Capturar dados da requisição
                var requestInfo = await CapturarInformacoesRequisicao(context);

                // Capturar resposta
                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                await _next(context);

                stopwatch.Stop();

                // Registrar auditoria apenas se a operação foi bem-sucedida
                if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
                {
                    await RegistrarAuditoria(context, requestInfo, auditRepository);
                }

                // Restaurar o corpo da resposta
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no middleware de auditoria para {Path}", context.Request.Path);
                throw;
            }
            finally
            {
                context.Response.Body = originalBodyStream;
                stopwatch.Stop();
            }
        }

        private static bool DeveAuditar(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
            var method = context.Request.Method.ToUpperInvariant();

            // Auditar apenas operações em entidades de dados pessoais
            var entidadesAuditadas = new[] { "/api/cliente", "/api/tecnico", "/api/ordemservico" };
            
            // Não auditar health checks, swagger, etc.
            var pathsIgnorados = new[] { "/health", "/swagger", "/api/docs" };

            return entidadesAuditadas.Any(e => path.StartsWith(e)) && 
                   !pathsIgnorados.Any(p => path.StartsWith(p)) &&
                   new[] { "GET", "POST", "PUT", "PATCH", "DELETE" }.Contains(method);
        }

        private static async Task<RequestInfo> CapturarInformacoesRequisicao(HttpContext context)
        {
            var request = context.Request;
            
            // Capturar corpo da requisição se for POST/PUT/PATCH
            string? requestBody = null;
            if (new[] { "POST", "PUT", "PATCH" }.Contains(request.Method) && 
                request.ContentLength > 0 && 
                request.ContentType?.Contains("application/json") == true)
            {
                request.EnableBuffering();
                using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                request.Body.Position = 0;
            }

            return new RequestInfo
            {
                Path = request.Path.Value ?? "",
                Method = request.Method,
                QueryString = request.QueryString.Value,
                IpAddress = ObterIpAddress(context),
                UserAgent = request.Headers.UserAgent.ToString(),
                UserId = ObterUserId(context),
                UserName = ObterUserName(context),
                RequestBody = requestBody
            };
        }

        private static async Task RegistrarAuditoria(HttpContext context, RequestInfo requestInfo, IAuditLogRepository auditRepository)
        {
            try
            {
                var (entityName, entityId, operation) = ExtrairInformacoesEntidade(requestInfo);
                
                if (string.IsNullOrEmpty(entityName))
                    return;

                AuditLog auditLog = operation switch
                {
                    "CREATE" => AuditLog.CriarRegistroCreate(entityName, entityId ?? "N/A", requestInfo.UserId, 
                                                           requestInfo.UserName, requestInfo.IpAddress, requestInfo.UserAgent, 
                                                           requestInfo.RequestBody),
                    "READ" => AuditLog.CriarRegistroRead(entityName, entityId ?? "N/A", requestInfo.UserId, 
                                                       requestInfo.UserName, requestInfo.IpAddress, requestInfo.UserAgent),
                    "UPDATE" => AuditLog.CriarRegistroUpdate(entityName, entityId ?? "N/A", requestInfo.UserId, 
                                                           requestInfo.UserName, requestInfo.IpAddress, requestInfo.UserAgent, 
                                                           requestInfo.RequestBody),
                    "DELETE" => AuditLog.CriarRegistroDelete(entityName, entityId ?? "N/A", requestInfo.UserId, 
                                                           requestInfo.UserName, requestInfo.IpAddress, requestInfo.UserAgent),
                    _ => throw new InvalidOperationException($"Operação não suportada: {operation}")
                };

                await auditRepository.AdicionarAsync(auditLog);
            }
            catch (Exception ex)
            {
                // Log do erro, mas não falhar a requisição
                var logger = context.RequestServices.GetService<ILogger<AuditMiddleware>>();
                logger?.LogError(ex, "Erro ao registrar auditoria para {Path}", requestInfo.Path);
            }
        }

        private static (string entityName, string? entityId, string operation) ExtrairInformacoesEntidade(RequestInfo requestInfo)
        {
            var pathSegments = requestInfo.Path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            
            if (pathSegments.Length < 2)
                return ("", null, "");

            var entityName = pathSegments[1].ToLowerInvariant() switch
            {
                "cliente" => "Cliente",
                "tecnico" => "Tecnico",
                "ordemservico" => "OrdemServico",
                _ => ""
            };

            var entityId = pathSegments.Length > 2 ? pathSegments[2] : null;
            
            var operation = requestInfo.Method switch
            {
                "GET" => "READ",
                "POST" => "CREATE",
                "PUT" or "PATCH" => "UPDATE",
                "DELETE" => "DELETE",
                _ => ""
            };

            return (entityName, entityId, operation);
        }

        private static string ObterIpAddress(HttpContext context)
        {
            // Tentar obter IP real considerando proxies
            var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            
            if (string.IsNullOrEmpty(ipAddress))
                ipAddress = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            
            if (string.IsNullOrEmpty(ipAddress))
                ipAddress = context.Connection.RemoteIpAddress?.ToString();

            return ipAddress ?? "Unknown";
        }

        private static string ObterUserId(HttpContext context)
        {
            // Por enquanto, usar um ID padrão. Em implementação real, extrair do JWT/Claims
            return context.User?.Identity?.Name ?? "sistema";
        }

        private static string ObterUserName(HttpContext context)
        {
            // Por enquanto, usar um nome padrão. Em implementação real, extrair do JWT/Claims
            return context.User?.Identity?.Name ?? "Sistema";
        }

        private class RequestInfo
        {
            public string Path { get; set; } = "";
            public string Method { get; set; } = "";
            public string? QueryString { get; set; }
            public string IpAddress { get; set; } = "";
            public string UserAgent { get; set; } = "";
            public string UserId { get; set; } = "";
            public string UserName { get; set; } = "";
            public string? RequestBody { get; set; }
        }
    }
}
