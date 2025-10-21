using Xunit;
using FluentAssertions;
using LegacyProcs.Domain.Entities;
using LegacyProcs.Domain.Enums;

namespace LegacyProcs.Tests.Domain;

public class OrdemServicoTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveCriarOrdemServico()
    {
        // Arrange
        var titulo = "Manutenção preventiva";
        var descricao = "Verificar equipamentos";
        var tecnico = "João Silva";

        // Act
        var ordemServico = OrdemServico.Criar(titulo, descricao, tecnico);

        // Assert
        ordemServico.Should().NotBeNull();
        ordemServico.Titulo.Should().Be(titulo);
        ordemServico.Descricao.Should().Be(descricao);
        ordemServico.Tecnico.Should().Be(tecnico);
        ordemServico.Status.Should().Be(StatusOS.Pendente);
        ordemServico.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        ordemServico.DataAtualizacao.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Criar_ComTituloInvalido_DeveLancarExcecao(string tituloInvalido)
    {
        // Arrange
        var descricao = "Descrição válida";
        var tecnico = "João Silva";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            OrdemServico.Criar(tituloInvalido, descricao, tecnico));
        
        exception.Message.Should().Contain("Título é obrigatório");
    }

    [Fact]
    public void Criar_ComTituloMuitoLongo_DeveLancarExcecao()
    {
        // Arrange
        var tituloLongo = new string('A', 201);
        var descricao = "Descrição válida";
        var tecnico = "João Silva";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            OrdemServico.Criar(tituloLongo, descricao, tecnico));
        
        exception.Message.Should().Contain("200 caracteres");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Criar_ComTecnicoInvalido_DeveLancarExcecao(string tecnicoInvalido)
    {
        // Arrange
        var titulo = "Título válido";
        var descricao = "Descrição válida";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            OrdemServico.Criar(titulo, descricao, tecnicoInvalido));
        
        exception.Message.Should().Contain("Técnico é obrigatório");
    }

    [Fact]
    public void AlterarStatus_TransicaoValida_DeveAlterarStatus()
    {
        // Arrange
        var ordemServico = OrdemServico.Criar("Teste", "Desc", "João");
        var dataAntes = ordemServico.DataAtualizacao;

        // Act
        ordemServico.AlterarStatus(StatusOS.EmAndamento);

        // Assert
        ordemServico.Status.Should().Be(StatusOS.EmAndamento);
        ordemServico.DataAtualizacao.Should().NotBeNull();
        ordemServico.DataAtualizacao.Should().BeAfter(dataAntes ?? DateTime.MinValue);
    }

    [Fact]
    public void AlterarStatus_TransicaoInvalida_DeveLancarExcecao()
    {
        // Arrange
        var ordemServico = OrdemServico.Criar("Teste", "Desc", "João");
        ordemServico.AlterarStatus(StatusOS.Concluida);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            ordemServico.AlterarStatus(StatusOS.EmAndamento));
        
        exception.Message.Should().Contain("Transição inválida");
    }

    [Fact]
    public void PodeSerExcluida_StatusPendente_DeveRetornarTrue()
    {
        // Arrange
        var ordemServico = OrdemServico.Criar("Teste", "Desc", "João");

        // Act
        var podeExcluir = ordemServico.PodeSerExcluida();

        // Assert
        podeExcluir.Should().BeTrue();
    }

    [Fact]
    public void PodeSerExcluida_StatusEmAndamento_DeveRetornarFalse()
    {
        // Arrange
        var ordemServico = OrdemServico.Criar("Teste", "Desc", "João");
        ordemServico.AlterarStatus(StatusOS.EmAndamento);

        // Act
        var podeExcluir = ordemServico.PodeSerExcluida();

        // Assert
        podeExcluir.Should().BeFalse();
    }

    [Fact]
    public void AtualizarInformacoes_ComDadosValidos_DeveAtualizar()
    {
        // Arrange
        var ordemServico = OrdemServico.Criar("Título Original", "Desc Original", "João Original");
        var novoTitulo = "Título Atualizado";
        var novaDescricao = "Descrição Atualizada";
        var novoTecnico = "Maria Silva";

        // Act
        ordemServico.AtualizarInformacoes(novoTitulo, novaDescricao, novoTecnico);

        // Assert
        ordemServico.Titulo.Should().Be(novoTitulo);
        ordemServico.Descricao.Should().Be(novaDescricao);
        ordemServico.Tecnico.Should().Be(novoTecnico);
        ordemServico.DataAtualizacao.Should().NotBeNull();
    }

    [Fact]
    public void GetStatusDisplay_DeveRetornarStringCorreta()
    {
        // Arrange
        var ordemServico = OrdemServico.Criar("Teste", "Desc", "João");

        // Act
        var statusDisplay = ordemServico.GetStatusDisplay();

        // Assert
        statusDisplay.Should().Be("Pendente");
    }
}
