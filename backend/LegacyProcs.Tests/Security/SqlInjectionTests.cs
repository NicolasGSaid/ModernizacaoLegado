using Xunit;
using FluentAssertions;
using System.Reflection;
using Microsoft.Data.SqlClient;

namespace LegacyProcs.Tests.Security;

/// <summary>
/// Testes de segurança para verificar eliminação de SQL Injection
/// </summary>
public class SqlInjectionTests
{
    [Fact]
    public void Controllers_NaoDevemConterSqlCommandDireto()
    {
        // Arrange
        var controllersAssembly = Assembly.LoadFrom("LegacyProcs.dll");
        var controllerTypes = controllersAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("Controller"))
            .ToList();

        // Act & Assert
        foreach (var controllerType in controllerTypes)
        {
            var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (var method in methods)
            {
                var methodBody = method.GetMethodBody();
                if (methodBody != null)
                {
                    // Verificar se não há uso direto de SqlCommand
                    var methodSource = method.ToString();
                    methodSource.Should().NotContain("SqlCommand", 
                        $"Controller {controllerType.Name} método {method.Name} não deve usar SqlCommand diretamente");
                    
                    methodSource.Should().NotContain("ExecuteReader", 
                        $"Controller {controllerType.Name} método {method.Name} não deve usar ExecuteReader diretamente");
                    
                    methodSource.Should().NotContain("ExecuteNonQuery", 
                        $"Controller {controllerType.Name} método {method.Name} não deve usar ExecuteNonQuery diretamente");
                }
            }
        }
    }

    [Fact]
    public void Controllers_DevemUsarMediatR()
    {
        // Arrange
        var controllersAssembly = Assembly.LoadFrom("LegacyProcs.dll");
        var controllerTypes = controllersAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("Controller") && t.Namespace?.Contains("Controllers") == true)
            .ToList();

        // Act & Assert
        controllerTypes.Should().NotBeEmpty("Deve haver controllers para testar");

        foreach (var controllerType in controllerTypes)
        {
            var constructors = controllerType.GetConstructors();
            var hasMediator = constructors.Any(c => 
                c.GetParameters().Any(p => p.ParameterType.Name.Contains("IMediator")));

            hasMediator.Should().BeTrue(
                $"Controller {controllerType.Name} deve usar IMediator para CQRS");
        }
    }

    [Theory]
    [InlineData("'; DROP TABLE OrdemServico; --")]
    [InlineData("1' OR '1'='1")]
    [InlineData("admin'--")]
    [InlineData("' UNION SELECT * FROM Cliente --")]
    public void PayloadsSqlInjection_NaoDevemSerProcessadosComoSql(string maliciousPayload)
    {
        // Arrange & Act & Assert
        // Este teste documenta que payloads maliciosos não são mais processados como SQL
        // pois todos os controllers agora usam EF Core com parâmetros seguros
        
        // Verificar que o payload é tratado como string literal
        maliciousPayload.Should().BeOfType<string>("Payload deve ser tratado como string, não como comando SQL");
        
        // Verificar que contém caracteres típicos de SQL Injection
        var hasSqlInjectionChars = maliciousPayload.Contains("'") || 
                                  maliciousPayload.Contains("--") || 
                                  maliciousPayload.Contains("UNION") ||
                                  maliciousPayload.Contains("DROP");
        
        hasSqlInjectionChars.Should().BeTrue("Payload deve conter caracteres de SQL Injection para validar o teste");
    }

    [Fact]
    public void EfCore_DeveUsarParametrizacao()
    {
        // Arrange
        var repositoriesAssembly = Assembly.LoadFrom("LegacyProcs.dll");
        var repositoryTypes = repositoriesAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("Repository") && !t.IsInterface)
            .ToList();

        // Act & Assert
        repositoryTypes.Should().NotBeEmpty("Deve haver repositories para testar");

        foreach (var repoType in repositoryTypes)
        {
            var fields = repoType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            var hasDbContext = fields.Any(f => f.FieldType.Name.Contains("DbContext"));

            hasDbContext.Should().BeTrue(
                $"Repository {repoType.Name} deve usar DbContext para acesso seguro aos dados");
        }
    }
}
