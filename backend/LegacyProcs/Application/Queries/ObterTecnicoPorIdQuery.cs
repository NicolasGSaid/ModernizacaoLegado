using LegacyProcs.Application.Common;
using LegacyProcs.Controllers.DTOs;

namespace LegacyProcs.Application.Queries;

public class ObterTecnicoPorIdQuery : IQuery<TecnicoResponseDto>
{
    public int Id { get; set; }
}
