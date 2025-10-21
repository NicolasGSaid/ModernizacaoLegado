using LegacyProcs.Application.Common;

namespace LegacyProcs.Application.Commands;

/// <summary>
/// Command para excluir uma ordem de serviço
/// Extrai lógica do controller DELETE para camada Application
/// </summary>
public class ExcluirOrdemServicoCommand : ICommand
{
    public int Id { get; set; }
}
