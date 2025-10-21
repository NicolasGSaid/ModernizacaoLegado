using LegacyProcs.Application.Common;
using LegacyProcs.Controllers.DTOs;

namespace LegacyProcs.Application.Queries;

/// <summary>
/// Query para obter uma ordem de serviço por ID
/// Extrai lógica do controller GET por ID para camada Application
/// </summary>
public class ObterOrdemServicoPorIdQuery : IQuery<OrdemServicoResponseDto>
{
    public int Id { get; set; }
}
