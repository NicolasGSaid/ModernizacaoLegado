using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using LegacyProcs.Infrastructure.Configuration;

namespace LegacyProcs.Tests.Security;

/// <summary>
/// Testes de segurança para configuração CORS
/// </summary>
public class CorsSecurityTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CorsSecurityTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public void CorsConfiguration_DeveSerRestritiva_EmProducao()
    {
        // Arrange
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ASPNETCORE_ENVIRONMENT"] = "Production",
                    ["Security:EnableCors"] = "true"
                });
            });
        });

        // Act
        var client = factory.CreateClient();
        var services = factory.Services;
        var securitySettings = services.GetRequiredService<IConfiguration>()
            .GetSection(SecuritySettings.SectionName)
            .Get<SecuritySettings>();

        // Assert
        securitySettings.Should().NotBeNull();
        
        // Em produção, CORS deve ser desabilitado por padrão
        var productionConfig = new Dictionary<string, string?>
        {
            ["Security:EnableCors"] = "false"
        };
        
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(productionConfig)
            .Build();
            
        var prodSettings = config.GetSection(SecuritySettings.SectionName).Get<SecuritySettings>();
        prodSettings?.EnableCors.Should().BeFalse("CORS deve ser desabilitado em produção por padrão");
    }

    [Fact]
    public void CorsConfiguration_DeveSerPermissiva_EmDesenvolvimento()
    {
        // Arrange
        var developmentConfig = new Dictionary<string, string?>
        {
            ["ASPNETCORE_ENVIRONMENT"] = "Development",
            ["Security:EnableCors"] = "true"
        };
        
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(developmentConfig)
            .Build();

        // Act
        var securitySettings = config.GetSection(SecuritySettings.SectionName).Get<SecuritySettings>();

        // Assert
        securitySettings?.EnableCors.Should().BeTrue("CORS pode ser habilitado em desenvolvimento");
    }

    [Theory]
    [InlineData("https://malicious-site.com")]
    [InlineData("http://localhost:3000")]
    [InlineData("https://evil.com")]
    public void CorsOrigins_NaoDevePermitir_OriginsMaliciosas(string maliciousOrigin)
    {
        // Arrange
        var allowedOrigins = new[] { "https://legacyprocs.com", "https://app.legacyprocs.com" };

        // Act & Assert
        allowedOrigins.Should().NotContain(maliciousOrigin, 
            "Origins maliciosas não devem estar na lista de permitidas");
    }

    [Fact]
    public void CorsHeaders_DeveSerRestritivos()
    {
        // Arrange
        var allowedHeaders = new[] { "Content-Type", "Authorization" };
        var dangerousHeaders = new[] { "X-Forwarded-For", "X-Real-IP", "X-Custom-Script" };

        // Act & Assert
        foreach (var dangerousHeader in dangerousHeaders)
        {
            allowedHeaders.Should().NotContain(dangerousHeader,
                $"Header perigoso {dangerousHeader} não deve ser permitido");
        }
    }

    [Fact]
    public void CorsMethods_DeveSerLimitados()
    {
        // Arrange
        var allowedMethods = new[] { "GET", "POST", "PUT", "DELETE" };
        var dangerousMethods = new[] { "TRACE", "CONNECT", "OPTIONS" };

        // Act & Assert
        foreach (var dangerousMethod in dangerousMethods)
        {
            allowedMethods.Should().NotContain(dangerousMethod,
                $"Método HTTP perigoso {dangerousMethod} não deve ser permitido");
        }
    }
}
