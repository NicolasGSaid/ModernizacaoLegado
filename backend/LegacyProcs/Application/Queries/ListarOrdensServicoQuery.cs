using LegacyProcs.Application.Common;
using LegacyProcs.Application.DTOs;
using LegacyProcs.Controllers.DTOs;

namespace LegacyProcs.Application.Queries;

/// <summary>
/// Query para listar ordens de serviço com paginação e filtro
/// Extrai lógica do controller GET para camada Application
/// </summary>
public class ListarOrdensServicoQuery : IQuery<PaginatedResultDto<OrdemServicoResponseDto>>
{
    public string? Filtro { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
