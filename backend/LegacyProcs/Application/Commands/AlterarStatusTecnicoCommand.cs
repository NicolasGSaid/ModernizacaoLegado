using LegacyProcs.Application.Common;
using LegacyProcs.Controllers.DTOs;

namespace LegacyProcs.Application.Commands;

public class AlterarStatusTecnicoCommand : ICommand<TecnicoResponseDto>
{
    public int Id { get; set; }
    public string NovoStatus { get; set; } = string.Empty;
}
