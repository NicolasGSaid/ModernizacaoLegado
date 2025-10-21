using Microsoft.AspNetCore.Http;

namespace LegacyProcs.Infrastructure.Middleware
{
    /// <summary>
    /// Middleware para Security Headers - NIST SSDF Compliance
    /// Implementa headers de segurança conforme OWASP Security Headers
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityHeadersMiddleware> _logger;

        public SecurityHeadersMiddleware(RequestDelegate next, ILogger<SecurityHeadersMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Adicionar headers de segurança antes de processar a requisição
            AdicionarSecurityHeaders(context);

            await _next(context);

            // Log para auditoria de segurança
            if (context.Response.StatusCode >= 400)
            {
                _logger.LogWarning("Resposta com erro {StatusCode} para {Path} de {IP}", 
                    context.Response.StatusCode, 
                    context.Request.Path, 
                    ObterIpAddress(context));
            }
        }

        private static void AdicionarSecurityHeaders(HttpContext context)
        {
            var headers = context.Response.Headers;

            // X-Content-Type-Options: Previne MIME type sniffing
            headers["X-Content-Type-Options"] = "nosniff";

            // X-Frame-Options: Previne clickjacking
            headers["X-Frame-Options"] = "DENY";

            // X-XSS-Protection: Ativa proteção XSS do browser (legacy)
            headers["X-XSS-Protection"] = "1; mode=block";

            // Referrer-Policy: Controla informações de referrer
            headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

            // Content-Security-Policy: Previne XSS e injection attacks
            var csp = "default-src 'self'; " +
                     "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                     "style-src 'self' 'unsafe-inline'; " +
                     "img-src 'self' data: https:; " +
                     "font-src 'self'; " +
                     "connect-src 'self'; " +
                     "frame-ancestors 'none'; " +
                     "base-uri 'self'; " +
                     "form-action 'self'";
            headers["Content-Security-Policy"] = csp;

            // Strict-Transport-Security: Força HTTPS (apenas em produção)
            if (!context.Request.Host.Host.Contains("localhost"))
            {
                headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload";
            }

            // Permissions-Policy: Controla APIs do browser
            var permissionsPolicy = "accelerometer=(), " +
                                   "camera=(), " +
                                   "geolocation=(), " +
                                   "gyroscope=(), " +
                                   "magnetometer=(), " +
                                   "microphone=(), " +
                                   "payment=(), " +
                                   "usb=()";
            headers["Permissions-Policy"] = permissionsPolicy;

            // Cache-Control: Previne cache de dados sensíveis
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                headers["Pragma"] = "no-cache";
                headers["Expires"] = "0";
            }

            // Server: Remove informações do servidor
            headers.Remove("Server");
            headers["Server"] = "LegacyProcs-API";

            // X-Powered-By: Remove informações da tecnologia
            headers.Remove("X-Powered-By");

            // Cross-Origin-Embedder-Policy: Isola origem
            headers["Cross-Origin-Embedder-Policy"] = "require-corp";

            // Cross-Origin-Opener-Policy: Isola contexto de navegação
            headers["Cross-Origin-Opener-Policy"] = "same-origin";

            // Cross-Origin-Resource-Policy: Controla recursos cross-origin
            headers["Cross-Origin-Resource-Policy"] = "same-origin";
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
    }
}
