using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Xunit;

namespace LegacyProcs.Tests.Security
{
    /// <summary>
    /// Classe base para testes de segurança - OWASP Top 10 & NIST SSDF
    /// Implementa cenários de teste para vulnerabilidades comuns
    /// </summary>
    public abstract class SecurityTestsBase : IClassFixture<WebApplicationFactory<Program>>
    {
        protected readonly WebApplicationFactory<Program> _factory;
        protected readonly HttpClient _client;

        protected SecurityTestsBase(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        /// <summary>
        /// Payloads maliciosos para testes de injection
        /// </summary>
        protected static readonly string[] SqlInjectionPayloads = new[]
        {
            "'; DROP TABLE Users; --",
            "' OR '1'='1",
            "' UNION SELECT * FROM Users --",
            "'; INSERT INTO Users VALUES ('hacker', 'password'); --",
            "' OR 1=1 --",
            "admin'--",
            "admin'/*",
            "' OR 'x'='x",
            "'; EXEC xp_cmdshell('dir'); --"
        };

        /// <summary>
        /// Payloads XSS para testes de cross-site scripting
        /// </summary>
        protected static readonly string[] XssPayloads = new[]
        {
            "<script>alert('XSS')</script>",
            "<img src=x onerror=alert('XSS')>",
            "javascript:alert('XSS')",
            "<svg onload=alert('XSS')>",
            "<iframe src=javascript:alert('XSS')></iframe>",
            "<body onload=alert('XSS')>",
            "<input onfocus=alert('XSS') autofocus>",
            "<select onfocus=alert('XSS') autofocus>",
            "<textarea onfocus=alert('XSS') autofocus>",
            "<keygen onfocus=alert('XSS') autofocus>"
        };

        /// <summary>
        /// Payloads para testes de path traversal
        /// </summary>
        protected static readonly string[] PathTraversalPayloads = new[]
        {
            "../../../etc/passwd",
            "..\\..\\..\\windows\\system32\\drivers\\etc\\hosts",
            "....//....//....//etc/passwd",
            "..%2F..%2F..%2Fetc%2Fpasswd",
            "..%252F..%252F..%252Fetc%252Fpasswd",
            "%2e%2e%2f%2e%2e%2f%2e%2e%2fetc%2fpasswd"
        };

        /// <summary>
        /// Headers maliciosos para testes
        /// </summary>
        protected static readonly Dictionary<string, string> MaliciousHeaders = new()
        {
            { "X-Forwarded-For", "127.0.0.1, <script>alert('XSS')</script>" },
            { "User-Agent", "<script>alert('XSS')</script>" },
            { "Referer", "javascript:alert('XSS')" },
            { "X-Real-IP", "'; DROP TABLE Users; --" },
            { "X-Forwarded-Host", "evil.com" },
            { "Host", "evil.com" }
        };

        /// <summary>
        /// Verifica se a resposta contém headers de segurança obrigatórios
        /// </summary>
        protected static void AssertSecurityHeaders(HttpResponseMessage response)
        {
            // Headers obrigatórios de segurança
            Assert.True(response.Headers.Contains("X-Content-Type-Options"), "Header X-Content-Type-Options ausente");
            Assert.True(response.Headers.Contains("X-Frame-Options"), "Header X-Frame-Options ausente");
            Assert.True(response.Headers.Contains("Referrer-Policy"), "Header Referrer-Policy ausente");
            Assert.True(response.Headers.Contains("Content-Security-Policy"), "Header Content-Security-Policy ausente");

            // Verificar valores dos headers
            var xContentTypeOptions = response.Headers.GetValues("X-Content-Type-Options").FirstOrDefault();
            Assert.Equal("nosniff", xContentTypeOptions);

            var xFrameOptions = response.Headers.GetValues("X-Frame-Options").FirstOrDefault();
            Assert.Equal("DENY", xFrameOptions);

            // Verificar se headers perigosos não estão presentes
            Assert.False(response.Headers.Contains("Server") && 
                        response.Headers.GetValues("Server").Any(v => v.Contains("Microsoft") || v.Contains("IIS")), 
                        "Header Server expõe informações sensíveis");
        }

        /// <summary>
        /// Verifica se a resposta não contém informações sensíveis
        /// </summary>
        protected static async Task AssertNoSensitiveInformation(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            
            // Verificar se não há vazamento de informações sensíveis
            Assert.DoesNotContain("password", content.ToLowerInvariant());
            Assert.DoesNotContain("connectionstring", content.ToLowerInvariant());
            Assert.DoesNotContain("secret", content.ToLowerInvariant());
            Assert.DoesNotContain("token", content.ToLowerInvariant());
            Assert.DoesNotContain("stacktrace", content.ToLowerInvariant());
            Assert.DoesNotContain("exception", content.ToLowerInvariant());
            Assert.DoesNotContain("sql", content.ToLowerInvariant());
        }

        /// <summary>
        /// Verifica rate limiting
        /// </summary>
        protected async Task<bool> TestRateLimiting(string endpoint, int maxRequests = 250)
        {
            var tasks = new List<Task<HttpResponseMessage>>();
            
            // Fazer múltiplas requisições simultâneas (mais que o limite de produção)
            for (int i = 0; i < maxRequests; i++)
            {
                tasks.Add(_client.GetAsync(endpoint));
            }

            var responses = await Task.WhenAll(tasks);
            
            // Verificar se alguma requisição foi bloqueada (429 Too Many Requests)
            return responses.Any(r => r.StatusCode == System.Net.HttpStatusCode.TooManyRequests);
        }

        /// <summary>
        /// Testa input sanitization
        /// </summary>
        protected async Task<bool> TestInputSanitization(string endpoint, string payload)
        {
            var content = new StringContent($"{{\"test\": \"{payload}\"}}", 
                System.Text.Encoding.UTF8, "application/json");
            
            var response = await _client.PostAsync(endpoint, content);
            
            // Se retornar 400 Bad Request, a sanitização está funcionando
            return response.StatusCode == System.Net.HttpStatusCode.BadRequest;
        }

        /// <summary>
        /// Testa input sanitization e retorna a resposta completa
        /// </summary>
        protected async Task<HttpResponseMessage> TestInputSanitizationResponse(string endpoint, string payload)
        {
            var content = new StringContent($"{{\"test\": \"{payload}\"}}", 
                System.Text.Encoding.UTF8, "application/json");
            
            return await _client.PostAsync(endpoint, content);
        }
    }
}
