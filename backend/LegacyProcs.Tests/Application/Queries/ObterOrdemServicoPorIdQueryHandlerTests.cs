using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using LegacyProcs.Application.Queries;
using LegacyProcs.Domain.Entities;
using LegacyProcs.Domain.Interfaces;
using LegacyProcs.Domain.Exceptions;

namespace LegacyProcs.Tests.Application.Queries;

public class ObterOrdemServicoPorIdQueryHandlerTests
{
    private readonly Mock<IOrdemServicoRepository> _repositoryMock;
    private readonly Mock<ILogger<ObterOrdemServicoPorIdQueryHandler>> _loggerMock;
    private readonly ObterOrdemServicoPorIdQueryHandler _handler;

    public ObterOrdemServicoPorIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IOrdemServicoRepository>();
        _loggerMock = new Mock<ILogger<ObterOrdemServicoPorIdQueryHandler>>();
        _handler = new ObterOrdemServicoPorIdQueryHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ComOrdemServicoExistente_DeveRetornarDto()
    {
        // Arrange
        var ordemServico = OrdemServico.Criar("Manutenção", "Descrição detalhada", "João Silva");
        var query = new ObterOrdemServicoPorIdQuery { Id = 1 };

        _repositoryMock.Setup(r => r.GetByIdAsync(query.Id))
                      .ReturnsAsync(ordemServico);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Titulo.Should().Be("Manutenção");
        result.Descricao.Should().Be("Descrição detalhada");
        result.Tecnico.Should().Be("João Silva");
        result.Status.Should().Be("Pendente");
        result.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _repositoryMock.Verify(r => r.GetByIdAsync(query.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_ComOrdemServicoInexistente_DeveLancarExcecao()
    {
        // Arrange
        var query = new ObterOrdemServicoPorIdQuery { Id = 999 };

        _repositoryMock.Setup(r => r.GetByIdAsync(query.Id))
                      .ReturnsAsync((OrdemServico?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OrdemServicoNotFoundException>(
            () => _handler.Handle(query, CancellationToken.None));

        exception.EntityId.Should().Be(query.Id);
        _repositoryMock.Verify(r => r.GetByIdAsync(query.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveLogarInformacoes()
    {
        // Arrange
        var ordemServico = OrdemServico.Criar("Teste", "Descrição", "Técnico");
        var query = new ObterOrdemServicoPorIdQuery { Id = 1 };

        _repositoryMock.Setup(r => r.GetByIdAsync(query.Id))
                      .ReturnsAsync(ordemServico);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Buscando ordem de serviço por ID")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("encontrada")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }
}
