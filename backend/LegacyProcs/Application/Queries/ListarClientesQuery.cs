using LegacyProcs.Application.Common;
using LegacyProcs.Application.DTOs;
using LegacyProcs.Controllers.DTOs;

namespace LegacyProcs.Application.Queries;

/// <summary>
/// Query para listar clientes com paginação e busca
/// </summary>
public class ListarClientesQuery : IQuery<PaginatedResultDto<ClienteResponseDto>>
{
    public string? Busca { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
