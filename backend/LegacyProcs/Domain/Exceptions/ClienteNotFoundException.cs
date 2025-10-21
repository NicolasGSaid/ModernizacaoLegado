namespace LegacyProcs.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando um cliente não é encontrado
/// </summary>
public class ClienteNotFoundException : EntityNotFoundException
{
    public ClienteNotFoundException(int clienteId) 
        : base("Cliente", clienteId)
    {
    }
}
