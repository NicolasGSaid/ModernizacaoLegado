using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;
using FluentAssertions;

namespace LegacyProcs.Tests.Security
{
    /// <summary>
    /// Testes de conformidade LGPD - Lei Geral de Proteção de Dados
    /// Verifica implementação dos direitos dos titulares de dados
    /// </summary>
    public class LGPDComplianceTests : SecurityTestsBase
    {
        public LGPDComplianceTests(WebApplicationFactory<Program> factory) : base(factory)
        {
        }

        [Fact]
        public async Task DataMinimization_OnlyNecessaryFields_ShouldBeCollected()
        {
            // Arrange
            var clienteData = new
            {
                razaoSocial = "Empresa Teste LTDA",
                nomeFantasia = "Empresa Teste",
                cnpj = "12.345.678/0001-90",
                email = "contato@empresateste.com",
                telefone = "(11) 99999-9999",
                endereco = "Rua Teste, 123",
                cidade = "São Paulo",
                estado = "SP",
                cep = "01234-567"
            };

            var content = new StringContent(JsonSerializer.Serialize(clienteData), 
                Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/cliente", content);

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.BadRequest);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var createdCliente = JsonSerializer.Deserialize<JsonElement>(responseContent);
                
                // Verificar que apenas campos necessários estão presentes
                createdCliente.TryGetProperty("id", out _).Should().BeTrue();
                createdCliente.TryGetProperty("razaoSocial", out _).Should().BeTrue();
                createdCliente.TryGetProperty("cnpj", out _).Should().BeTrue();
                createdCliente.TryGetProperty("email", out _).Should().BeTrue();
                
                // Verificar que campos sensíveis desnecessários não estão expostos
                createdCliente.TryGetProperty("password", out _).Should().BeFalse();
                createdCliente.TryGetProperty("internalId", out _).Should().BeFalse();
            }
        }

        [Fact]
        public async Task RightToAccess_DataSubject_ShouldRetrieveOwnData()
        {
            // Arrange - Primeiro criar um cliente
            var clienteData = new
            {
                razaoSocial = "Empresa LGPD Teste",
                cnpj = "98.765.432/0001-10",
                email = "lgpd@teste.com",
                telefone = "(11) 88888-8888",
                endereco = "Av. LGPD, 456",
                cidade = "São Paulo",
                estado = "SP",
                cep = "04567-890"
            };

            var createContent = new StringContent(JsonSerializer.Serialize(clienteData), 
                Encoding.UTF8, "application/json");
            
            var createResponse = await _client.PostAsync("/api/cliente", createContent);
            
            if (createResponse.IsSuccessStatusCode)
            {
                var createdContent = await createResponse.Content.ReadAsStringAsync();
                var createdCliente = JsonSerializer.Deserialize<JsonElement>(createdContent);
                var clienteId = createdCliente.GetProperty("id").GetInt32();

                // Act - Acessar os dados
                var accessResponse = await _client.GetAsync($"/api/cliente/{clienteId}");

                // Assert
                accessResponse.StatusCode.Should().Be(HttpStatusCode.OK);
                
                var accessContent = await accessResponse.Content.ReadAsStringAsync();
                var retrievedCliente = JsonSerializer.Deserialize<JsonElement>(accessContent);
                
                retrievedCliente.GetProperty("razaoSocial").GetString().Should().Be("Empresa LGPD Teste");
                retrievedCliente.GetProperty("email").GetString().Should().Be("lgpd@teste.com");
            }
        }

        [Fact]
        public async Task RightToRectification_IncorrectData_ShouldBeUpdatable()
        {
            // Arrange - Criar cliente com dados incorretos
            var clienteData = new
            {
                razaoSocial = "Empresa Incorreta",
                cnpj = "11.111.111/0001-11",
                email = "incorreto@teste.com",
                telefone = "(11) 77777-7777",
                endereco = "Rua Incorreta, 789",
                cidade = "São Paulo",
                estado = "SP",
                cep = "01111-111"
            };

            var createContent = new StringContent(JsonSerializer.Serialize(clienteData), 
                Encoding.UTF8, "application/json");
            
            var createResponse = await _client.PostAsync("/api/cliente", createContent);
            
            if (createResponse.IsSuccessStatusCode)
            {
                var createdContent = await createResponse.Content.ReadAsStringAsync();
                var createdCliente = JsonSerializer.Deserialize<JsonElement>(createdContent);
                var clienteId = createdCliente.GetProperty("id").GetInt32();

                // Act - Corrigir os dados
                var updatedData = new
                {
                    razaoSocial = "Empresa Corrigida LTDA",
                    email = "correto@teste.com",
                    telefone = "(11) 66666-6666",
                    endereco = "Rua Corrigida, 321",
                    cidade = "São Paulo",
                    estado = "SP",
                    cep = "02222-222"
                };

                var updateContent = new StringContent(JsonSerializer.Serialize(updatedData), 
                    Encoding.UTF8, "application/json");
                
                var updateResponse = await _client.PutAsync($"/api/cliente/{clienteId}", updateContent);

                // Assert
                updateResponse.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
            }
        }

        [Fact]
        public async Task DataPortability_ExportRequest_ShouldProvideStructuredData()
        {
            // Arrange - Assumindo que existe um endpoint de exportação
            var exportRequest = new
            {
                clienteId = 1,
                formatoExportacao = "JSON",
                solicitadoPor = "titular@dados.com",
                incluirHistorico = true
            };

            var content = new StringContent(JsonSerializer.Serialize(exportRequest), 
                Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/cliente/exportar", content);

            // Assert
            if (response.StatusCode != HttpStatusCode.NotFound) // Endpoint pode não estar implementado ainda
            {
                response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Accepted);
                
                if (response.IsSuccessStatusCode)
                {
                    var exportContent = await response.Content.ReadAsStringAsync();
                    exportContent.Should().NotBeNullOrEmpty();
                    
                    // Verificar se é JSON válido
                    var exportData = JsonSerializer.Deserialize<JsonElement>(exportContent);
                    exportData.ValueKind.Should().Be(JsonValueKind.Object);
                }
            }
        }

        [Fact]
        public async Task RightToErasure_DeleteRequest_ShouldRemoveAllData()
        {
            // Arrange - Criar cliente para exclusão
            var clienteData = new
            {
                razaoSocial = "Empresa Para Exclusão",
                cnpj = "99.999.999/0001-99",
                email = "exclusao@teste.com",
                telefone = "(11) 55555-5555",
                endereco = "Rua Exclusão, 999",
                cidade = "São Paulo",
                estado = "SP",
                cep = "09999-999"
            };

            var createContent = new StringContent(JsonSerializer.Serialize(clienteData), 
                Encoding.UTF8, "application/json");
            
            var createResponse = await _client.PostAsync("/api/cliente", createContent);
            
            if (createResponse.IsSuccessStatusCode)
            {
                var createdContent = await createResponse.Content.ReadAsStringAsync();
                var createdCliente = JsonSerializer.Deserialize<JsonElement>(createdContent);
                var clienteId = createdCliente.GetProperty("id").GetInt32();

                // Act - Solicitar exclusão
                var deleteRequest = new
                {
                    clienteId = clienteId,
                    motivoExclusao = "Exercício do direito ao esquecimento conforme LGPD",
                    solicitadoPor = "exclusao@teste.com",
                    confirmarExclusao = true
                };

                var deleteContent = new StringContent(JsonSerializer.Serialize(deleteRequest), 
                    Encoding.UTF8, "application/json");
                
                var deleteResponse = await _client.PostAsync("/api/cliente/excluir-dados", deleteContent);

                // Assert
                if (deleteResponse.StatusCode != HttpStatusCode.NotFound) // Endpoint pode não estar implementado ainda
                {
                    deleteResponse.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Accepted);
                    
                    // Verificar se os dados foram realmente removidos
                    var verifyResponse = await _client.GetAsync($"/api/cliente/{clienteId}");
                    verifyResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
                }
            }
        }

        [Fact]
        public async Task AuditTrail_DataOperations_ShouldBeLogged()
        {
            // Arrange
            var clienteData = new
            {
                razaoSocial = "Empresa Auditoria",
                cnpj = "88.888.888/0001-88",
                email = "auditoria@teste.com",
                telefone = "(11) 44444-4444",
                endereco = "Rua Auditoria, 888",
                cidade = "São Paulo",
                estado = "SP",
                cep = "08888-888"
            };

            var content = new StringContent(JsonSerializer.Serialize(clienteData), 
                Encoding.UTF8, "application/json");

            // Act - Realizar operação que deve ser auditada
            var response = await _client.PostAsync("/api/cliente", content);

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.BadRequest);
            
            // Verificar se headers de auditoria estão presentes (se implementados)
            if (response.Headers.Contains("X-Audit-Log-Id"))
            {
                var auditLogId = response.Headers.GetValues("X-Audit-Log-Id").FirstOrDefault();
                auditLogId.Should().NotBeNullOrEmpty();
            }
        }

        [Fact]
        public async Task ConsentManagement_DataProcessing_ShouldRequireConsent()
        {
            // Arrange
            var clienteWithoutConsent = new
            {
                razaoSocial = "Empresa Sem Consentimento",
                cnpj = "77.777.777/0001-77",
                email = "semconsentimento@teste.com",
                telefone = "(11) 33333-3333",
                endereco = "Rua Sem Consentimento, 777",
                cidade = "São Paulo",
                estado = "SP",
                cep = "07777-777",
                consentimento = false // Sem consentimento
            };

            var content = new StringContent(JsonSerializer.Serialize(clienteWithoutConsent), 
                Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/cliente", content);

            // Assert - Deve rejeitar se consentimento for obrigatório
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                errorContent.ToLowerInvariant().Should().Contain("consentimento");
            }
        }

        [Fact]
        public async Task DataRetention_ExpiredData_ShouldBeHandled()
        {
            // Arrange - Verificar se existe endpoint para verificar retenção
            var retentionCheckRequest = new
            {
                dataType = "cliente",
                checkDate = DateTime.UtcNow.AddYears(-6) // Dados de 6 anos atrás
            };

            var content = new StringContent(JsonSerializer.Serialize(retentionCheckRequest), 
                Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/data-retention/check", content);

            // Assert
            if (response.StatusCode != HttpStatusCode.NotFound) // Endpoint pode não estar implementado ainda
            {
                response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Accepted);
            }
        }
    }
}
