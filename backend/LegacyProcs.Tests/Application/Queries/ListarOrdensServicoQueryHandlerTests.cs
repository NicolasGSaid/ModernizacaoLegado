using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using LegacyProcs.Application.Queries;
using LegacyProcs.Domain.Entities;
using LegacyProcs.Domain.Interfaces;

namespace LegacyProcs.Tests.Application.Queries;

public class ListarOrdensServicoQueryHandlerTests
{
    private readonly Mock<IOrdemServicoRepository> _repositoryMock;
    private readonly Mock<ILogger<ListarOrdensServicoQueryHandler>> _loggerMock;
    private readonly ListarOrdensServicoQueryHandler _handler;

    public ListarOrdensServicoQueryHandlerTests()
    {
        _repositoryMock = new Mock<IOrdemServicoRepository>();
        _loggerMock = new Mock<ILogger<ListarOrdensServicoQueryHandler>>();
        _handler = new ListarOrdensServicoQueryHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ComParametrosValidos_DeveRetornarResultadoPaginado()
    {
        // Arrange
        var query = new ListarOrdensServicoQuery
        {
            Filtro = "teste",
            Page = 1,
            PageSize = 10
        };

        var ordens = new List<OrdemServico>
        {
            OrdemServico.Criar("Ordem 1", "Descrição 1", "João"),
            OrdemServico.Criar("Ordem 2", "Descrição 2", "Maria")
        };

        _repositoryMock.Setup(r => r.GetPagedAsync(query.Page, query.PageSize, query.Filtro))
                      .ReturnsAsync((ordens, 25)); // 25 total items

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.TotalItems.Should().Be(25);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalPages.Should().Be(3); // Math.Ceiling(25/10) = 3

        var firstItem = result.Data.First();
        firstItem.Titulo.Should().Be("Ordem 1");
        firstItem.Status.Should().Be("Pendente");

        _repositoryMock.Verify(r => r.GetPagedAsync(query.Page, query.PageSize, query.Filtro), Times.Once);
    }

    [Fact]
    public async Task Handle_SemResultados_DeveRetornarListaVazia()
    {
        // Arrange
        var query = new ListarOrdensServicoQuery
        {
            Page = 1,
            PageSize = 10
        };

        _repositoryMock.Setup(r => r.GetPagedAsync(query.Page, query.PageSize, query.Filtro))
                      .ReturnsAsync((new List<OrdemServico>(), 0));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty();
        result.TotalItems.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task Handle_DeveLogarInformacoes()
    {
        // Arrange
        var query = new ListarOrdensServicoQuery { Page = 1, PageSize = 10 };
        
        _repositoryMock.Setup(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                      .ReturnsAsync((new List<OrdemServico>(), 0));

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Listando ordens de serviço")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }
}
