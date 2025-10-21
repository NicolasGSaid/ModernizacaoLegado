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

public class ExcluirOrdemServicoCommandHandlerTests
{
    private readonly Mock<IOrdemServicoRepository> _repositoryMock;
    private readonly Mock<ILogger<ExcluirOrdemServicoCommandHandler>> _loggerMock;
    private readonly ExcluirOrdemServicoCommandHandler _handler;

    public ExcluirOrdemServicoCommandHandlerTests()
    {
        _repositoryMock = new Mock<IOrdemServicoRepository>();
        _loggerMock = new Mock<ILogger<ExcluirOrdemServicoCommandHandler>>();
        _handler = new ExcluirOrdemServicoCommandHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ComOrdemServicoExistenteEPendente_DeveExcluir()
    {
        // Arrange
        var ordemServico = OrdemServico.Criar("Teste", "Descrição", "João");
        var command = new ExcluirOrdemServicoCommand { Id = 1 };

        _repositoryMock.Setup(r => r.GetByIdAsync(command.Id))
                      .ReturnsAsync(ordemServico);
        _repositoryMock.Setup(r => r.DeleteAsync(It.IsAny<OrdemServico>()))
                      .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetByIdAsync(command.Id), Times.Once);
        _repositoryMock.Verify(r => r.DeleteAsync(ordemServico), Times.Once);
    }

    [Fact]
    public async Task Handle_ComOrdemServicoInexistente_DeveLancarExcecao()
    {
        // Arrange
        var command = new ExcluirOrdemServicoCommand { Id = 999 };

        _repositoryMock.Setup(r => r.GetByIdAsync(command.Id))
                      .ReturnsAsync((OrdemServico?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OrdemServicoNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));

        exception.EntityId.Should().Be(command.Id);
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<OrdemServico>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ComOrdemServicoEmAndamento_DeveLancarExcecao()
    {
        // Arrange
        var ordemServico = OrdemServico.Criar("Teste", "Descrição", "João");
        ordemServico.AlterarStatus(StatusOS.EmAndamento);
        var command = new ExcluirOrdemServicoCommand { Id = 1 };

        _repositoryMock.Setup(r => r.GetByIdAsync(command.Id))
                      .ReturnsAsync(ordemServico);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(
            () => _handler.Handle(command, CancellationToken.None));

        exception.RuleName.Should().Be("ExclusaoOrdemServico");
        exception.Message.Should().Contain("Em Andamento");
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<OrdemServico>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ComOrdemServicoConcluida_DeveLancarExcecao()
    {
        // Arrange
        var ordemServico = OrdemServico.Criar("Teste", "Descrição", "João");
        ordemServico.AlterarStatus(StatusOS.EmAndamento);
        ordemServico.AlterarStatus(StatusOS.Concluida);
        var command = new ExcluirOrdemServicoCommand { Id = 1 };

        _repositoryMock.Setup(r => r.GetByIdAsync(command.Id))
                      .ReturnsAsync(ordemServico);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(
            () => _handler.Handle(command, CancellationToken.None));

        exception.RuleName.Should().Be("ExclusaoOrdemServico");
        exception.Message.Should().Contain("Concluída");
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<OrdemServico>()), Times.Never);
    }
}
