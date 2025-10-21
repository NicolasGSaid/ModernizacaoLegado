using Xunit;
using FluentAssertions;
using LegacyProcs.Domain.Entities;
using LegacyProcs.Domain.Enums;

namespace LegacyProcs.Tests.Domain;

public class TecnicoTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveCriarTecnico()
    {
        // Arrange
        var nome = "João Silva";
        var email = "joao@empresa.com";
        var telefone = "(11) 99999-9999";
        var especialidade = "Elétrica";

        // Act
        var tecnico = Tecnico.Criar(nome, email, telefone, especialidade);

        // Assert
        tecnico.Should().NotBeNull();
        tecnico.Nome.Should().Be(nome);
        tecnico.Email.Should().Be("joao@empresa.com");
        tecnico.Telefone.Should().Be("11999999999");
        tecnico.Especialidade.Should().Be(especialidade);
        tecnico.Status.Should().Be(StatusTecnico.Ativo);
        tecnico.DataCadastro.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Criar_ComNomeInvalido_DeveLancarExcecao(string nomeInvalido)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            Tecnico.Criar(nomeInvalido, "test@test.com", "11999999999", "Elétrica"));
        
        exception.Message.Should().Contain("Nome é obrigatório");
    }

    [Theory]
    [InlineData("email-invalido")]
    [InlineData("@test.com")]
    [InlineData("test@")]
    public void Criar_ComEmailInvalido_DeveLancarExcecao(string emailInvalido)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            Tecnico.Criar("João Silva", emailInvalido, "11999999999", "Elétrica"));
        
        exception.Message.Should().Contain("Email deve ter formato válido");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Criar_ComEspecialidadeInvalida_DeveLancarExcecao(string especialidadeInvalida)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            Tecnico.Criar("João Silva", "test@test.com", "11999999999", especialidadeInvalida));
        
        exception.Message.Should().Contain("Especialidade é obrigatória");
    }

    [Fact]
    public void AlterarStatus_StatusDiferente_DeveAlterarStatus()
    {
        // Arrange
        var tecnico = Tecnico.Criar("João", "joao@test.com", "11999999999", "Elétrica");

        // Act
        tecnico.AlterarStatus(StatusTecnico.Ferias);

        // Assert
        tecnico.Status.Should().Be(StatusTecnico.Ferias);
    }

    [Fact]
    public void AlterarStatus_MesmoStatus_DeveLancarExcecao()
    {
        // Arrange
        var tecnico = Tecnico.Criar("João", "joao@test.com", "11999999999", "Elétrica");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            tecnico.AlterarStatus(StatusTecnico.Ativo));
        
        exception.Message.Should().Contain("já está com status");
    }

    [Fact]
    public void PodeTrabalhar_StatusAtivo_DeveRetornarTrue()
    {
        // Arrange
        var tecnico = Tecnico.Criar("João", "joao@test.com", "11999999999", "Elétrica");

        // Act
        var podeTrabalhar = tecnico.PodeTrabalhar();

        // Assert
        podeTrabalhar.Should().BeTrue();
    }

    [Fact]
    public void PodeTrabalhar_StatusFerias_DeveRetornarFalse()
    {
        // Arrange
        var tecnico = Tecnico.Criar("João", "joao@test.com", "11999999999", "Elétrica");
        tecnico.AlterarStatus(StatusTecnico.Ferias);

        // Act
        var podeTrabalhar = tecnico.PodeTrabalhar();

        // Assert
        podeTrabalhar.Should().BeFalse();
    }

    [Fact]
    public void GetStatusDisplay_DeveRetornarStringCorreta()
    {
        // Arrange
        var tecnico = Tecnico.Criar("João", "joao@test.com", "11999999999", "Elétrica");

        // Act
        var statusDisplay = tecnico.GetStatusDisplay();

        // Assert
        statusDisplay.Should().Be("Ativo");
    }

    [Fact]
    public void AtualizarInformacoes_ComDadosValidos_DeveAtualizar()
    {
        // Arrange
        var tecnico = Tecnico.Criar("João Original", "original@test.com", "11999999999", "Elétrica");
        var novoNome = "João Atualizado";
        var novoEmail = "atualizado@test.com";
        var novaEspecialidade = "Hidráulica";

        // Act
        tecnico.AtualizarInformacoes(novoNome, novoEmail, "11888888888", novaEspecialidade);

        // Assert
        tecnico.Nome.Should().Be(novoNome);
        tecnico.Email.Should().Be(novoEmail);
        tecnico.Especialidade.Should().Be(novaEspecialidade);
        tecnico.Telefone.Should().Be("11888888888");
    }
}
