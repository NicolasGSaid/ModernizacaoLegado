namespace LegacyProcs.Domain.Exceptions;

/// <summary>
/// Exceção base para erros de domínio
/// Representa violações de regras de negócio
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }

    protected DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Exceção para quando uma entidade não é encontrada
/// </summary>
public class EntityNotFoundException : DomainException
{
    public string EntityName { get; }
    public object EntityId { get; }

    public EntityNotFoundException(string entityName, object entityId)
        : base($"{entityName} com ID '{entityId}' não foi encontrado.")
    {
        EntityName = entityName;
        EntityId = entityId;
    }
}

/// <summary>
/// Exceção específica para OrdemServico não encontrada
/// </summary>
public class OrdemServicoNotFoundException : EntityNotFoundException
{
    public OrdemServicoNotFoundException(int id) 
        : base("Ordem de Serviço", id)
    {
    }
}

/// <summary>
/// Exceção para violações de regras de negócio
/// </summary>
public class BusinessRuleViolationException : DomainException
{
    public string RuleName { get; }

    public BusinessRuleViolationException(string ruleName, string message) 
        : base($"Violação da regra de negócio '{ruleName}': {message}")
    {
        RuleName = ruleName;
    }
}

/// <summary>
/// Exceção para dados duplicados
/// </summary>
public class DuplicateDataException : DomainException
{
    public string FieldName { get; }
    public object FieldValue { get; }

    public DuplicateDataException(string fieldName, object fieldValue)
        : base($"Já existe um registro com {fieldName} = '{fieldValue}'.")
    {
        FieldName = fieldName;
        FieldValue = fieldValue;
    }
}
