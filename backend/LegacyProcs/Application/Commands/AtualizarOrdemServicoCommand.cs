using LegacyProcs.Application.Common;
using LegacyProcs.Controllers.DTOs;

namespace LegacyProcs.Application.Commands;

/// <summary>
/// Command para atualizar todos os dados de uma ordem de serviço
/// </summary>
public class AtualizarOrdemServicoCommand : ICommand<OrdemServicoResponseDto>
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int TecnicoId { get; set; }
    public string Status { get; set; } = string.Empty;
}
