using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace LegacyProcs.Infrastructure.Middleware
{
    /// <summary>
    /// Middleware para Input Sanitization - OWASP Protection
    /// Sanitiza automaticamente entradas para prevenir ataques de injeção
    /// </summary>
    public class InputSanitizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<InputSanitizationMiddleware> _logger;

        // Padrões maliciosos comuns
        private static readonly Regex[] PadroesPerigosos = new[]
        {
            new Regex(@"<script[^>]*>.*?</script>", RegexOptions.IgnoreCase | RegexOptions.Singleline),
            new Regex(@"javascript:", RegexOptions.IgnoreCase),
            new Regex(@"vbscript:", RegexOptions.IgnoreCase),
            new Regex(@"onload\s*=", RegexOptions.IgnoreCase),
            new Regex(@"onerror\s*=", RegexOptions.IgnoreCase),
            new Regex(@"onclick\s*=", RegexOptions.IgnoreCase),
            new Regex(@"onmouseover\s*=", RegexOptions.IgnoreCase),
            new Regex(@"<iframe[^>]*>.*?</iframe>", RegexOptions.IgnoreCase | RegexOptions.Singleline),
            new Regex(@"<object[^>]*>.*?</object>", RegexOptions.IgnoreCase | RegexOptions.Singleline),
            new Regex(@"<embed[^>]*>.*?</embed>", RegexOptions.IgnoreCase | RegexOptions.Singleline),
            new Regex(@"eval\s*\(", RegexOptions.IgnoreCase),
            new Regex(@"expression\s*\(", RegexOptions.IgnoreCase),
            new Regex(@"url\s*\(", RegexOptions.IgnoreCase),
            new Regex(@"@import", RegexOptions.IgnoreCase),
            new Regex(@"<!--.*?-->", RegexOptions.Singleline),
        };

        // Padrões SQL Injection
        private static readonly Regex[] PadroesSqlInjection = new[]
        {
            new Regex(@"(\b(ALTER|CREATE|DELETE|DROP|EXEC(UTE){0,1}|INSERT( +INTO){0,1}|MERGE|SELECT|UPDATE|UNION( +ALL){0,1})\b)", RegexOptions.IgnoreCase),
            new Regex(@"(\b(AND|OR)\b.{1,6}?(=|>|<|\!|<=|>=))", RegexOptions.IgnoreCase),
            new Regex(@"(\b(GRANT|REVOKE)\b)", RegexOptions.IgnoreCase),
            new Regex(@"(\b(GROUP_CONCAT|CONCAT|LOAD_FILE|INTO OUTFILE)\b)", RegexOptions.IgnoreCase),
            new Regex(@"(\b(INFORMATION_SCHEMA|SYSOBJECTS|SYSCOLUMNS)\b)", RegexOptions.IgnoreCase),
            new Regex(@"(\b(sp_executesql|xp_cmdshell|sp_makewebtask)\b)", RegexOptions.IgnoreCase),
            new Regex(@"(WAITFOR\s+(DELAY|TIME)\s+)", RegexOptions.IgnoreCase),
            new Regex(@"(\b(BENCHMARK|SLEEP)\s*\()", RegexOptions.IgnoreCase),
        };

        public InputSanitizationMiddleware(RequestDelegate next, ILogger<InputSanitizationMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Sanitizar apenas requisições com corpo (POST, PUT, PATCH)
            if (DeveProcessarRequisicao(context))
            {
                await ProcessarESanitizarRequisicao(context);
            }

            await _next(context);
        }

        private static bool DeveProcessarRequisicao(HttpContext context)
        {
            var method = context.Request.Method.ToUpperInvariant();
            var contentType = context.Request.ContentType?.ToLowerInvariant() ?? "";
            
            return new[] { "POST", "PUT", "PATCH" }.Contains(method) &&
                   (contentType.Contains("application/json") || contentType.Contains("application/x-www-form-urlencoded"));
        }

        private async Task ProcessarESanitizarRequisicao(HttpContext context)
        {
            try
            {
                context.Request.EnableBuffering();
                
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                if (string.IsNullOrEmpty(body))
                    return;

                // Detectar ataques antes da sanitização
                if (await DetectarAtaque(body, context))
                    return; // Requisição já foi rejeitada

                // Sanitizar o corpo da requisição
                var bodySanitizado = SanitizarString(body);

                // Se houve mudanças, substituir o corpo da requisição
                if (body != bodySanitizado)
                {
                    _logger.LogWarning("Input sanitizado para {Path} de {IP}. Conteúdo potencialmente malicioso removido.", 
                        context.Request.Path, ObterIpAddress(context));

                    var bytes = Encoding.UTF8.GetBytes(bodySanitizado);
                    context.Request.Body = new MemoryStream(bytes);
                    context.Request.ContentLength = bytes.Length;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao sanitizar input para {Path}", context.Request.Path);
                // Em caso de erro, continuar sem sanitização para não quebrar a aplicação
            }
        }

        private async Task<bool> DetectarAtaque(string input, HttpContext context)
        {
            // Verificar padrões XSS
            foreach (var padrao in PadroesPerigosos)
            {
                if (padrao.IsMatch(input))
                {
                    _logger.LogError("Tentativa de XSS detectada de {IP} para {Path}. Padrão: {Pattern}", 
                        ObterIpAddress(context), context.Request.Path, padrao.ToString());
                    
                    await RejeitarRequisicao(context, "Conteúdo malicioso detectado (XSS)");
                    return true;
                }
            }

            // Verificar padrões SQL Injection
            foreach (var padrao in PadroesSqlInjection)
            {
                if (padrao.IsMatch(input))
                {
                    _logger.LogError("Tentativa de SQL Injection detectada de {IP} para {Path}. Padrão: {Pattern}", 
                        ObterIpAddress(context), context.Request.Path, padrao.ToString());
                    
                    await RejeitarRequisicao(context, "Conteúdo malicioso detectado (SQL Injection)");
                    return true;
                }
            }

            return false;
        }

        private static async Task RejeitarRequisicao(HttpContext context, string motivo)
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";

            var response = new
            {
                error = "Bad Request",
                message = "Requisição rejeitada por motivos de segurança",
                details = motivo
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }

        private static string SanitizarString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var resultado = input;

            // Remover tags HTML perigosas
            foreach (var padrao in PadroesPerigosos)
            {
                resultado = padrao.Replace(resultado, "");
            }

            // Escapar caracteres especiais
            resultado = resultado
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&#x27;")
                .Replace("/", "&#x2F;");

            // Remover caracteres de controle
            resultado = Regex.Replace(resultado, @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", "");

            // Normalizar espaços em branco
            resultado = Regex.Replace(resultado, @"\s+", " ").Trim();

            return resultado;
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
    }
}
