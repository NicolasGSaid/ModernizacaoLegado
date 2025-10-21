using Xunit;
using FluentAssertions;
using LegacyProcs.Domain.Entities;

namespace LegacyProcs.Tests.Domain;

public class ClienteTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveCriarCliente()
    {
        // Arrange
        var razaoSocial = "Empresa Teste LTDA";
        var nomeFantasia = "Empresa Teste";
        var cnpj = "12.345.678/0001-90";
        var email = "contato@empresa.com";
        var telefone = "(11) 99999-9999";
        var endereco = "Rua Teste, 123";
        var cidade = "São Paulo";
        var estado = "SP";
        var cep = "01234-567";

        // Act
        var cliente = Cliente.Criar(razaoSocial, nomeFantasia, cnpj, email, telefone, endereco, cidade, estado, cep);

        // Assert
        cliente.Should().NotBeNull();
        cliente.RazaoSocial.Should().Be(razaoSocial);
        cliente.NomeFantasia.Should().Be(nomeFantasia);
        cliente.CNPJ.Should().Be("12345678000190");
        cliente.Email.Should().Be("contato@empresa.com");
        cliente.Telefone.Should().Be("11999999999");
        cliente.Endereco.Should().Be(endereco);
        cliente.Cidade.Should().Be(cidade);
        cliente.Estado.Should().Be("SP");
        cliente.CEP.Should().Be("01234567");
        cliente.DataCadastro.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Criar_ComRazaoSocialInvalida_DeveLancarExcecao(string razaoSocialInvalida)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            Cliente.Criar(razaoSocialInvalida, "Nome", "12345678000190", "test@test.com", "11999999999", "Rua", "SP", "SP", "12345678"));
        
        exception.Message.Should().Contain("Razão Social é obrigatória");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("123456789012345")]
    [InlineData("abcd1234567890")]
    public void Criar_ComCNPJInvalido_DeveLancarExcecao(string cnpjInvalido)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            Cliente.Criar("Empresa", "Nome", cnpjInvalido, "test@test.com", "11999999999", "Rua", "SP", "SP", "12345678"));
        
        exception.Message.Should().Contain("CNPJ");
    }

    [Theory]
    [InlineData("email-invalido")]
    [InlineData("@test.com")]
    [InlineData("test@")]
    public void Criar_ComEmailInvalido_DeveLancarExcecao(string emailInvalido)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            Cliente.Criar("Empresa", "Nome", "12345678000190", emailInvalido, "11999999999", "Rua", "SP", "SP", "12345678"));
        
        exception.Message.Should().Contain("Email deve ter formato válido");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("123456789")]
    [InlineData("123456789012")]
    public void Criar_ComTelefoneInvalido_DeveLancarExcecao(string telefoneInvalido)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            Cliente.Criar("Empresa", "Nome", "12345678000190", "test@test.com", telefoneInvalido, "Rua", "SP", "SP", "12345678"));
        
        exception.Message.Should().Contain("Telefone deve ter 10 ou 11 dígitos");
    }

    [Theory]
    [InlineData("1234567")]
    [InlineData("123456789")]
    [InlineData("abcd1234")]
    public void Criar_ComCEPInvalido_DeveLancarExcecao(string cepInvalido)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            Cliente.Criar("Empresa", "Nome", "12345678000190", "test@test.com", "11999999999", "Rua", "SP", "SP", cepInvalido));
        
        exception.Message.Should().Contain("CEP deve ter 8 dígitos");
    }

    [Fact]
    public void GetCNPJFormatado_DeveRetornarFormatoCorreto()
    {
        // Arrange
        var cliente = Cliente.Criar("Empresa", "Nome", "12345678000190", "test@test.com", "11999999999", "Rua", "SP", "SP", "12345678");

        // Act
        var cnpjFormatado = cliente.GetCNPJFormatado();

        // Assert
        cnpjFormatado.Should().Be("12.345.678/0001-90");
    }

    [Fact]
    public void GetCEPFormatado_DeveRetornarFormatoCorreto()
    {
        // Arrange
        var cliente = Cliente.Criar("Empresa", "Nome", "12345678000190", "test@test.com", "11999999999", "Rua", "SP", "SP", "12345678");

        // Act
        var cepFormatado = cliente.GetCEPFormatado();

        // Assert
        cepFormatado.Should().Be("12345-678");
    }

    [Fact]
    public void AtualizarInformacoes_ComDadosValidos_DeveAtualizar()
    {
        // Arrange
        var cliente = Cliente.Criar("Empresa Original", "Nome Original", "12345678000190", "original@test.com", "11999999999", "Rua Original", "SP", "SP", "12345678");
        var novaRazaoSocial = "Empresa Atualizada";
        var novoEmail = "atualizado@test.com";

        // Act
        cliente.AtualizarInformacoes(novaRazaoSocial, "Nome Atualizado", novoEmail, "11888888888", "Rua Nova", "Rio de Janeiro", "RJ", "87654321");

        // Assert
        cliente.RazaoSocial.Should().Be(novaRazaoSocial);
        cliente.Email.Should().Be(novoEmail);
        cliente.Cidade.Should().Be("Rio de Janeiro");
        cliente.Estado.Should().Be("RJ");
    }
}
