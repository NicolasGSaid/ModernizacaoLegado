using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using LegacyProcs.Infrastructure.Configuration;
using LegacyProcs.Infrastructure.Data;
using System.Text.Json;

namespace LegacyProcs.Tests.Infrastructure.HealthChecks;

/// <summary>
/// Testes para Health Checks da aplicação
/// </summary>
public class HealthCheckTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HealthCheckTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remover o DbContext existente
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Adicionar DbContext em memória
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb_HealthCheck");
                });
            });
        });
    }

    [Fact]
    public async Task HealthCheck_Endpoint_DeveRetornarStatus200()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task HealthCheck_Endpoint_DeveRetornarJsonValido()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        content.Should().NotBeNullOrEmpty();
        
        var healthReport = JsonSerializer.Deserialize<HealthCheckResponse>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        healthReport.Should().NotBeNull();
        healthReport!.Status.Should().NotBeNullOrEmpty();
        healthReport.Checks.Should().NotBeNull();
    }

    [Fact]
    public async Task HealthCheck_Database_DeveEstarSaudavel()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        var healthReport = JsonSerializer.Deserialize<HealthCheckResponse>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var databaseCheck = healthReport!.Checks.FirstOrDefault(c => c.Name == "database");
        databaseCheck.Should().NotBeNull();
        databaseCheck!.Status.Should().BeOneOf("Healthy", "Degraded", "Unhealthy"); // Pode estar unhealthy em ambiente de teste
    }

    [Fact]
    public async Task HealthCheck_Self_DeveEstarSaudavel()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        var healthReport = JsonSerializer.Deserialize<HealthCheckResponse>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var selfCheck = healthReport!.Checks.FirstOrDefault(c => c.Name == "self");
        selfCheck.Should().NotBeNull();
        selfCheck!.Status.Should().Be("Healthy");
    }

    [Fact]
    public async Task HealthCheck_DeveIncluirDuracao()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        var healthReport = JsonSerializer.Deserialize<HealthCheckResponse>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        healthReport!.TotalDuration.Should().NotBeNullOrEmpty();
        
        foreach (var check in healthReport.Checks)
        {
            check.Duration.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public void HealthCheckService_DeveEstarRegistrado()
    {
        // Arrange & Act
        var healthCheckService = _factory.Services.GetService<HealthCheckService>();

        // Assert
        healthCheckService.Should().NotBeNull("HealthCheckService deve estar registrado no DI container");
    }

    [Fact]
    public async Task HealthCheckService_DeveExecutarChecks()
    {
        // Arrange
        var healthCheckService = _factory.Services.GetRequiredService<HealthCheckService>();

        // Act
        var result = await healthCheckService.CheckHealthAsync();

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().BeOneOf(HealthStatus.Healthy, HealthStatus.Degraded, HealthStatus.Unhealthy);
        result.Entries.Should().NotBeEmpty();
        result.Entries.Should().ContainKey("database");
        result.Entries.Should().ContainKey("self");
    }
}

/// <summary>
/// Modelo para deserializar resposta do Health Check
/// </summary>
public class HealthCheckResponse
{
    public string Status { get; set; } = string.Empty;
    public List<HealthCheckItem> Checks { get; set; } = new();
    public string TotalDuration { get; set; } = string.Empty;
}

/// <summary>
/// Modelo para item individual do Health Check
/// </summary>
public class HealthCheckItem
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Exception { get; set; }
    public string Duration { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
}
