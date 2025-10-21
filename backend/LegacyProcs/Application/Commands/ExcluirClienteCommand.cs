using LegacyProcs.Application.Common;

namespace LegacyProcs.Application.Commands;

/// <summary>
/// Command para excluir cliente
/// </summary>
public class ExcluirClienteCommand : ICommand
{
    public int Id { get; set; }
}
