using LegacyProcs.Application.Common;
using LegacyProcs.Application.DTOs;
using LegacyProcs.Controllers.DTOs;
using LegacyProcs.Domain.Interfaces;

namespace LegacyProcs.Application.Queries;

/// <summary>
/// Handler para listar ordens de serviço
/// Contém lógica de consulta extraída do controller GET
/// </summary>
public class ListarOrdensServicoQueryHandler : IQueryHandler<ListarOrdensServicoQuery, PaginatedResultDto<OrdemServicoResponseDto>>
{
    private readonly IOrdemServicoRepository _repository;
    private readonly ILogger<ListarOrdensServicoQueryHandler> _logger;

    public ListarOrdensServicoQueryHandler(
        IOrdemServicoRepository repository,
        ILogger<ListarOrdensServicoQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PaginatedResultDto<OrdemServicoResponseDto>> Handle(ListarOrdensServicoQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listando ordens de serviço. Filtro: {Filtro}, Page: {Page}, PageSize: {PageSize}", 
            request.Filtro, request.Page, request.PageSize);

        // Usar repository para busca paginada
        var (ordens, totalItems) = await _repository.GetPagedAsync(request.Page, request.PageSize, request.Filtro);

        // Mapear entidades para DTOs
        var ordensDto = ordens.Select(os => new OrdemServicoResponseDto
        {
            Id = os.Id,
            Titulo = os.Titulo,
            Descricao = os.Descricao,
            TecnicoId = os.TecnicoId,
            TecnicoNome = os.Tecnico?.Nome ?? "N/A",
            Status = os.GetStatusDisplay(),
            DataCriacao = os.DataCriacao,
            DataAtualizacao = os.DataAtualizacao
        }).ToList();

        // Calcular total de páginas
        var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

        _logger.LogInformation("Listagem concluída. Total: {TotalItems} itens, {TotalPages} páginas", 
            totalItems, totalPages);

        return new PaginatedResultDto<OrdemServicoResponseDto>
        {
            Data = ordensDto,
            TotalItems = totalItems,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = totalPages
        };
    }
}
