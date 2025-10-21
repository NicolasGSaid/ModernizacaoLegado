using LegacyProcs.Application.Common;
using LegacyProcs.Controllers.DTOs;

namespace LegacyProcs.Application.Queries;

/// <summary>
/// Query para obter cliente por ID
/// </summary>
public class ObterClientePorIdQuery : IQuery<ClienteResponseDto>
{
    public int Id { get; set; }
}
