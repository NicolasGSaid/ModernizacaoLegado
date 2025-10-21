namespace LegacyProcs.Domain.Enums;

/// <summary>
/// Enum para status do Técnico
/// Modernizado de string para enum type-safe
/// </summary>
public enum StatusTecnico
{
    Ativo = 1,
    Inativo = 2,
    Ferias = 3
}

/// <summary>
/// Extensões para o enum StatusTecnico
/// </summary>
public static class StatusTecnicoExtensions
{
    public static string ToDisplayString(this StatusTecnico status)
    {
        return status switch
        {
            StatusTecnico.Ativo => "Ativo",
            StatusTecnico.Inativo => "Inativo",
            StatusTecnico.Ferias => "Férias",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static StatusTecnico FromString(string statusString)
    {
        return statusString?.ToLowerInvariant().Trim() switch
        {
            "ativo" => StatusTecnico.Ativo,
            "inativo" => StatusTecnico.Inativo,
            "ferias" => StatusTecnico.Ferias,
            "férias" => StatusTecnico.Ferias,
            _ => throw new ArgumentException($"Status inválido: {statusString}", nameof(statusString))
        };
    }

    public static bool PodeTrabalhar(this StatusTecnico status)
    {
        return status == StatusTecnico.Ativo;
    }
}
