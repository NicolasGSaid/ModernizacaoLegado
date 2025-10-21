namespace LegacyProcs.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando um técnico não é encontrado
/// </summary>
public class TecnicoNotFoundException : EntityNotFoundException
{
    public TecnicoNotFoundException(int tecnicoId) 
        : base("Tecnico", tecnicoId)
    {
    }
}
