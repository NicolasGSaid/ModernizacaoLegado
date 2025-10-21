using LegacyProcs.Application.Common;
using LegacyProcs.Application.DTOs;
using LegacyProcs.Controllers.DTOs;
using LegacyProcs.Domain.Interfaces;

namespace LegacyProcs.Application.Queries;

/// <summary>
/// Handler para listar técnicos
/// </summary>
public class ListarTecnicosQueryHandler : IQueryHandler<ListarTecnicosQuery, PaginatedResultDto<TecnicoResponseDto>>
{
    private readonly ITecnicoRepository _repository;
    private readonly ILogger<ListarTecnicosQueryHandler> _logger;

    public ListarTecnicosQueryHandler(
        ITecnicoRepository repository,
        ILogger<ListarTecnicosQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PaginatedResultDto<TecnicoResponseDto>> Handle(ListarTecnicosQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listando técnicos. Filtro: {Filtro}, Page: {Page}, PageSize: {PageSize}", 
            request.Filtro, request.Page, request.PageSize);

        var (tecnicos, totalItems) = await _repository.GetPagedAsync(request.Page, request.PageSize, request.Filtro);

        var tecnicosDto = tecnicos.Select(t => new TecnicoResponseDto
        {
            Id = t.Id,
            Nome = t.Nome,
            Email = t.Email,
            Telefone = t.Telefone,
            Especialidade = t.Especialidade,
            Status = t.Status.ToString(),
            DataCadastro = t.DataCadastro
        }).ToList();

        var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

        return new PaginatedResultDto<TecnicoResponseDto>
        {
            Data = tecnicosDto,
            TotalItems = totalItems,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = totalPages
        };
    }
}
