using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace LegacyProcs.Infrastructure.Middleware
{
    /// <summary>
    /// Middleware para Rate Limiting - OWASP Protection
    /// Implementa proteção contra DDoS e abuso de API
    /// </summary>
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private readonly RateLimitOptions _options;

        public RateLimitingMiddleware(
            RequestDelegate next, 
            IMemoryCache cache, 
            ILogger<RateLimitingMiddleware> logger,
            RateLimitOptions options)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientId = ObterClientId(context);
            var endpoint = ObterEndpoint(context);
            
            // Verificar rate limit
            if (await ExcedeuRateLimit(clientId, endpoint, context))
            {
                await RetornarRateLimitExcedido(context);
                return;
            }

            // Adicionar headers informativos
            AdicionarRateLimitHeaders(context, clientId, endpoint);

            await _next(context);
        }

        private async Task<bool> ExcedeuRateLimit(string clientId, string endpoint, HttpContext context)
        {
            var config = ObterConfiguracaoEndpoint(endpoint);
            var chaveCache = $"rate_limit:{clientId}:{endpoint}";
            
            var requestInfo = _cache.Get<RequestInfo>(chaveCache);
            
            if (requestInfo == null)
            {
                // Primeira requisição
                requestInfo = new RequestInfo
                {
                    Count = 1,
                    WindowStart = DateTime.UtcNow
                };
                
                _cache.Set(chaveCache, requestInfo, TimeSpan.FromMinutes(config.WindowMinutes));
                return false;
            }

            // Verificar se a janela de tempo expirou
            if (DateTime.UtcNow - requestInfo.WindowStart > TimeSpan.FromMinutes(config.WindowMinutes))
            {
                // Reset da janela
                requestInfo.Count = 1;
                requestInfo.WindowStart = DateTime.UtcNow;
                _cache.Set(chaveCache, requestInfo, TimeSpan.FromMinutes(config.WindowMinutes));
                return false;
            }

            // Incrementar contador
            requestInfo.Count++;
            _cache.Set(chaveCache, requestInfo, TimeSpan.FromMinutes(config.WindowMinutes));

            // Verificar se excedeu o limite
            if (requestInfo.Count > config.MaxRequests)
            {
                _logger.LogWarning("Rate limit excedido para cliente {ClientId} no endpoint {Endpoint}. " +
                                 "Requests: {Count}/{Max} em {Window} minutos", 
                                 clientId, endpoint, requestInfo.Count, config.MaxRequests, config.WindowMinutes);
                
                // Registrar tentativa de abuso
                await RegistrarTentativaAbuso(context, clientId, endpoint, requestInfo.Count);
                return true;
            }

            return false;
        }

        private async Task RetornarRateLimitExcedido(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            context.Response.ContentType = "application/json";

            var response = new
            {
                error = "Rate limit exceeded",
                message = "Muitas requisições. Tente novamente mais tarde.",
                retryAfter = _options.DefaultWindowMinutes * 60 // segundos
            };

            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
        }

        private void AdicionarRateLimitHeaders(HttpContext context, string clientId, string endpoint)
        {
            var config = ObterConfiguracaoEndpoint(endpoint);
            var chaveCache = $"rate_limit:{clientId}:{endpoint}";
            var requestInfo = _cache.Get<RequestInfo>(chaveCache);

            if (requestInfo != null)
            {
                var remaining = Math.Max(0, config.MaxRequests - requestInfo.Count);
                var resetTime = requestInfo.WindowStart.AddMinutes(config.WindowMinutes);
                var resetSeconds = (int)(resetTime - DateTime.UtcNow).TotalSeconds;

                context.Response.Headers["X-RateLimit-Limit"] = config.MaxRequests.ToString();
                context.Response.Headers["X-RateLimit-Remaining"] = remaining.ToString();
                context.Response.Headers["X-RateLimit-Reset"] = resetSeconds.ToString();
            }
        }

        private string ObterClientId(HttpContext context)
        {
            // Prioridade: UserId > IP Address
            var userId = context.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(userId))
                return $"user:{userId}";

            var ipAddress = ObterIpAddress(context);
            return $"ip:{ipAddress}";
        }

        private static string ObterEndpoint(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
            var method = context.Request.Method.ToUpperInvariant();
            
            // Agrupar endpoints similares
            if (path.StartsWith("/api/cliente"))
                return $"{method}:/api/cliente";
            if (path.StartsWith("/api/tecnico"))
                return $"{method}:/api/tecnico";
            if (path.StartsWith("/api/ordemservico"))
                return $"{method}:/api/ordemservico";
            
            return $"{method}:{path}";
        }

        private RateLimitConfig ObterConfiguracaoEndpoint(string endpoint)
        {
            // Configurações específicas por endpoint
            return endpoint.Split(':')[0] switch
            {
                "GET" => new RateLimitConfig { MaxRequests = _options.GetMaxRequests, WindowMinutes = _options.DefaultWindowMinutes },
                "POST" => new RateLimitConfig { MaxRequests = _options.PostMaxRequests, WindowMinutes = _options.DefaultWindowMinutes },
                "PUT" => new RateLimitConfig { MaxRequests = _options.PutMaxRequests, WindowMinutes = _options.DefaultWindowMinutes },
                "DELETE" => new RateLimitConfig { MaxRequests = _options.DeleteMaxRequests, WindowMinutes = _options.DefaultWindowMinutes },
                _ => new RateLimitConfig { MaxRequests = _options.DefaultMaxRequests, WindowMinutes = _options.DefaultWindowMinutes }
            };
        }

        private async Task RegistrarTentativaAbuso(HttpContext context, string clientId, string endpoint, int requestCount)
        {
            // Em implementação real, registrar em sistema de auditoria
            _logger.LogError("Possível tentativa de abuso detectada. Cliente: {ClientId}, Endpoint: {Endpoint}, " +
                           "Requests: {Count}, IP: {IP}, UserAgent: {UserAgent}", 
                           clientId, endpoint, requestCount, 
                           ObterIpAddress(context), 
                           context.Request.Headers.UserAgent.ToString());
            
            await Task.CompletedTask;
        }

        private static string ObterIpAddress(HttpContext context)
        {
            var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            
            if (string.IsNullOrEmpty(ipAddress))
                ipAddress = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            
            if (string.IsNullOrEmpty(ipAddress))
                ipAddress = context.Connection.RemoteIpAddress?.ToString();

            return ipAddress ?? "Unknown";
        }

        private class RequestInfo
        {
            public int Count { get; set; }
            public DateTime WindowStart { get; set; }
        }

        private class RateLimitConfig
        {
            public int MaxRequests { get; set; }
            public int WindowMinutes { get; set; }
        }
    }

    public class RateLimitOptions
    {
        public int DefaultMaxRequests { get; set; } = 100;
        public int GetMaxRequests { get; set; } = 200;
        public int PostMaxRequests { get; set; } = 50;
        public int PutMaxRequests { get; set; } = 30;
        public int DeleteMaxRequests { get; set; } = 10;
        public int DefaultWindowMinutes { get; set; } = 15;
    }
}
