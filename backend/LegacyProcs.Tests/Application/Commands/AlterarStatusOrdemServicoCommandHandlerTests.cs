using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using LegacyProcs.Application.Commands;
using LegacyProcs.Domain.Entities;
using LegacyProcs.Domain.Enums;
using LegacyProcs.Domain.Interfaces;
using LegacyProcs.Domain.Exceptions;

namespace LegacyProcs.Tests.Application.Commands;

public class AlterarStatusOrdemServicoCommandHandlerTests
{
    private readonly Mock<IOrdemServicoRepository> _repositoryMock;
    private readonly Mock<ILogger<AlterarStatusOrdemServicoCommandHandler>> _loggerMock;
    private readonly AlterarStatusOrdemServicoCommandHandler _handler;

    public AlterarStatusOrdemServicoCommandHandlerTests()
    {
        _repositoryMock = new Mock<IOrdemServicoRepository>();
        _loggerMock = new Mock<ILogger<AlterarStatusOrdemServicoCommandHandler>>();
        _handler = new AlterarStatusOrdemServicoCommandHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ComOrdemServicoExistente_DeveAlterarStatus()
    {
        // Arrange
        var ordemServico = OrdemServico.Criar("Teste", "Descrição", "João");
        var command = new AlterarStatusOrdemServicoCommand
        {
            Id = 1,
            Status = "EmAndamento"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(command.Id))
                      .ReturnsAsync(ordemServico);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<OrdemServico>()))
                      .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be("Em Andamento");
        result.DataAtualizacao.Should().NotBeNull();

        _repositoryMock.Verify(r => r.GetByIdAsync(command.Id), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<OrdemServico>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ComOrdemServicoInexistente_DeveLancarExcecao()
    {
        // Arrange
        var command = new AlterarStatusOrdemServicoCommand
        {
            Id = 999,
            Status = "EmAndamento"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(command.Id))
                      .ReturnsAsync((OrdemServico?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OrdemServicoNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));

        exception.EntityId.Should().Be(command.Id);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<OrdemServico>()), Times.Never);
    }

    [Theory]
    [InlineData("StatusInvalido")]
    [InlineData("")]
    [InlineData("Qualquer")]
    public async Task Handle_ComStatusInvalido_DeveLancarExcecao(string statusInvalido)
    {
        // Arrange
        var ordemServico = OrdemServico.Criar("Teste", "Descrição", "João");
        var command = new AlterarStatusOrdemServicoCommand
        {
            Id = 1,
            Status = statusInvalido
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(command.Id))
                      .ReturnsAsync(ordemServico);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.Handle(command, CancellationToken.None));

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<OrdemServico>()), Times.Never);
    }
}
