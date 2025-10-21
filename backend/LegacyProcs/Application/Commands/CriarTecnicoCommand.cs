using LegacyProcs.Application.Common;
using LegacyProcs.Controllers.DTOs;

namespace LegacyProcs.Application.Commands;

public class CriarTecnicoCommand : ICommand<TecnicoResponseDto>
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Especialidade { get; set; } = string.Empty;
}
