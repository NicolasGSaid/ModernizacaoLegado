using LegacyProcs.Application.Common;
using LegacyProcs.Controllers.DTOs;

namespace LegacyProcs.Application.Commands;

/// <summary>
/// Command para criar uma nova ordem de serviço
/// Extrai lógica do controller POST para camada Application
/// </summary>
public class CriarOrdemServicoCommand : ICommand<OrdemServicoResponseDto>
{
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int TecnicoId { get; set; }
}
