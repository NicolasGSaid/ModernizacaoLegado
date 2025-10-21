using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using LegacyProcs.Application.Commands;
using LegacyProcs.Domain.Entities;
using LegacyProcs.Domain.Interfaces;

namespace LegacyProcs.Tests.Application.Commands;

public class CriarOrdemServicoCommandHandlerTests
{
    private readonly Mock<IOrdemServicoRepository> _repositoryMock;
    private readonly Mock<ILogger<CriarOrdemServicoCommandHandler>> _loggerMock;
    private readonly CriarOrdemServicoCommandHandler _handler;

    public CriarOrdemServicoCommandHandlerTests()
    {
        _repositoryMock = new Mock<IOrdemServicoRepository>();
        _loggerMock = new Mock<ILogger<CriarOrdemServicoCommandHandler>>();
        _handler = new CriarOrdemServicoCommandHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ComDadosValidos_DeveCriarOrdemServico()
    {
        // Arrange
        var command = new CriarOrdemServicoCommand
        {
            Titulo = "Manutenção preventiva",
            Descricao = "Verificar equipamentos",
            Tecnico = "João Silva"
        };

        var ordemServicoEsperada = OrdemServico.Criar(command.Titulo, command.Descricao, command.Tecnico);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<OrdemServico>()))
                      .ReturnsAsync((OrdemServico os) => os);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Titulo.Should().Be(command.Titulo);
        result.Descricao.Should().Be(command.Descricao);
        result.Tecnico.Should().Be(command.Tecnico);
        result.Status.Should().Be("Pendente");
        result.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<OrdemServico>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveLogarInformacoes()
    {
        // Arrange
        var command = new CriarOrdemServicoCommand
        {
            Titulo = "Teste",
            Descricao = "Descrição teste",
            Tecnico = "Técnico teste"
        };

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<OrdemServico>()))
                      .ReturnsAsync((OrdemServico os) => os);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Criando nova ordem de serviço")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }
}
