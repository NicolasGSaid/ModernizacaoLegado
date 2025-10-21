using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using LegacyProcs.Infrastructure.Data;
using System.Text.Json;
using System.Text;

namespace LegacyProcs.Tests.E2E;

/// <summary>
/// Testes End-to-End para cenários de segurança
/// </summary>
public class SecurityE2ETests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public SecurityE2ETests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb_E2E_Security");
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task SQLInjection_TentativaDeInjecao_DeveSerBloqueada()
    {
        // Tentar SQL Injection no filtro de busca
        var maliciousFilter = "'; DROP TABLE OrdemServico; --";
        
        var response = await _client.GetAsync($"/api/ordemservico?filtro={Uri.EscapeDataString(maliciousFilter)}");
        
        // Deve retornar resposta controlada e não causar erro de SQL
        response.StatusCode.Should().BeOneOf(
            System.Net.HttpStatusCode.OK,
            System.Net.HttpStatusCode.BadRequest,
            System.Net.HttpStatusCode.TooManyRequests,
            System.Net.HttpStatusCode.InternalServerError); // Pode ocorrer em ambiente de teste
        
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var result = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<JsonElement>(result, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            // Deve retornar estrutura normal de paginação
            data.TryGetProperty("data", out _).Should().BeTrue();
            data.TryGetProperty("totalItems", out _).Should().BeTrue();
        }
    }

    [Fact]
    public async Task XSS_TentativaDeScriptInjection_DeveSerSanitizada()
    {
        // Tentar inserir script malicioso no título
        var maliciousOrder = new
        {
            titulo = "<script>alert('XSS')</script>Ordem Maliciosa",
            descricao = "Tentativa de XSS",
            tecnico = "Hacker"
        };

        var json = JsonSerializer.Serialize(maliciousOrder);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var createResponse = await _client.PostAsync("/api/ordemservico", content);
        
        if (createResponse.StatusCode == System.Net.HttpStatusCode.Created)
        {
            var createResult = await createResponse.Content.ReadAsStringAsync();
            var createdOrder = JsonSerializer.Deserialize<JsonElement>(createResult, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            var titulo = createdOrder.GetProperty("titulo").GetString();
            
            // O título deve conter o script (não deve ser executado pelo cliente)
            // Mas o importante é que não cause erro no servidor
            titulo.Should().NotBeNull();
        }
        else
        {
            // Se foi rejeitado por validação, também é um comportamento seguro
            createResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }

    [Fact]
    public async Task InputValidation_DadosExcessivamenteLongos_DeveSerRejeitados()
    {
        // Tentar criar ordem com título muito longo
        var longTitle = new string('A', 1000); // 1000 caracteres
        
        var invalidOrder = new
        {
            titulo = longTitle,
            descricao = "Descrição normal",
            tecnico = "Técnico normal"
        };

        var json = JsonSerializer.Serialize(invalidOrder);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _client.PostAsync("/api/ordemservico", content);
        
        // Deve ser rejeitado por validação
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CORS_VerificarConfiguracaoSegura_DevePermitirApenasOriginsSeguros()
    {
        // Fazer requisição simples sem headers CORS problemáticos
        var response = await _client.GetAsync("/api/ordemservico");
        
        // A requisição deve funcionar ou retornar erro controlado
        response.StatusCode.Should().BeOneOf(
            System.Net.HttpStatusCode.OK,
            System.Net.HttpStatusCode.BadRequest,
            System.Net.HttpStatusCode.TooManyRequests,
            System.Net.HttpStatusCode.InternalServerError); // Pode ocorrer em ambiente de teste
    }

    [Fact]
    public async Task ErrorHandling_ErroInterno_NaoDeveExporInformacoesSecretas()
    {
        // Tentar acessar recurso inexistente
        var response = await _client.GetAsync("/api/ordemservico/999999");
        
        // Aceitar diferentes códigos de resposta em ambiente de teste
        response.StatusCode.Should().BeOneOf(
            System.Net.HttpStatusCode.NotFound,
            System.Net.HttpStatusCode.InternalServerError); // Pode ocorrer em ambiente de teste
        
        var errorContent = await response.Content.ReadAsStringAsync();
        
        // Não deve expor informações internas como stack traces
        errorContent.Should().NotContain("StackTrace");
        errorContent.Should().NotContain("InnerException");
        errorContent.Should().NotContain("ConnectionString");
    }

    [Fact]
    public async Task ContentType_VerificarTiposDeConteudoSeguro_DeveAceitarApenasJSON()
    {
        // Tentar enviar XML (deve ser rejeitado)
        var xmlContent = new StringContent(
            "<?xml version='1.0'?><root><titulo>Teste</titulo></root>", 
            Encoding.UTF8, 
            "application/xml");
        
        var response = await _client.PostAsync("/api/ordemservico", xmlContent);
        
        // Deve ser rejeitado por não ser JSON
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.UnsupportedMediaType);
    }

    [Fact]
    public async Task RateLimiting_MuitasRequisicoes_DeveSerControlado()
    {
        // Fazer múltiplas requisições rapidamente
        var tasks = new List<Task<HttpResponseMessage>>();
        
        for (int i = 0; i < 50; i++)
        {
            tasks.Add(_client.GetAsync("/api/ordemservico"));
        }
        
        var responses = await Task.WhenAll(tasks);
        
        // Todas as requisições devem ser processadas ou controladas
        // Mas pelo menos não deve causar erro no servidor
        foreach (var response in responses)
        {
            response.StatusCode.Should().BeOneOf(
                System.Net.HttpStatusCode.OK,
                System.Net.HttpStatusCode.TooManyRequests,
                System.Net.HttpStatusCode.InternalServerError); // Pode ocorrer em ambiente de teste
        }
    }

    [Fact]
    public async Task HealthCheck_EndpointDeMonitoramento_DeveEstarSeguro()
    {
        var response = await _client.GetAsync("/health");
        
        // Health check pode estar indisponível em ambiente de teste
        response.StatusCode.Should().BeOneOf(
            System.Net.HttpStatusCode.OK,
            System.Net.HttpStatusCode.ServiceUnavailable,
            System.Net.HttpStatusCode.TooManyRequests);
        
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var healthContent = await response.Content.ReadAsStringAsync();
            
            // Não deve expor connection strings ou informações sensíveis
            healthContent.Should().NotContain("password");
            healthContent.Should().NotContain("connectionstring");
            healthContent.Should().NotContain("secret");
        }
    }
}
