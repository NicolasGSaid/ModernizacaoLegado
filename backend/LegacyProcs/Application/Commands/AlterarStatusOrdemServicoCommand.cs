using LegacyProcs.Application.Common;
using LegacyProcs.Controllers.DTOs;

namespace LegacyProcs.Application.Commands;

/// <summary>
/// Command para alterar status de uma ordem de serviço
/// Extrai lógica do controller PUT para camada Application
/// </summary>
public class AlterarStatusOrdemServicoCommand : ICommand<OrdemServicoResponseDto>
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
}
