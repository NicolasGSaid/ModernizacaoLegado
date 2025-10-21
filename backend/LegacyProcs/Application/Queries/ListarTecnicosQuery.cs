using LegacyProcs.Application.Common;
using LegacyProcs.Application.DTOs;
using LegacyProcs.Controllers.DTOs;

namespace LegacyProcs.Application.Queries;

public class ListarTecnicosQuery : IQuery<PaginatedResultDto<TecnicoResponseDto>>
{
    public string? Filtro { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
