namespace LegacyProcs.Domain.Enums;

/// <summary>
/// Enum para status da Ordem de Serviço
/// Modernizado de string para enum type-safe
/// </summary>
public enum StatusOS
{
    Pendente = 1,
    EmAndamento = 2,
    Concluida = 3
}

/// <summary>
/// Extensões para o enum StatusOS
/// </summary>
public static class StatusOSExtensions
{
    public static string ToDisplayString(this StatusOS status)
    {
        return status switch
        {
            StatusOS.Pendente => "Pendente",
            StatusOS.EmAndamento => "Em Andamento", 
            StatusOS.Concluida => "Concluída",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static StatusOS FromString(string statusString)
    {
        return statusString?.ToLowerInvariant().Trim() switch
        {
            "pendente" => StatusOS.Pendente,
            "em andamento" => StatusOS.EmAndamento,
            "emandamento" => StatusOS.EmAndamento,
            "concluida" => StatusOS.Concluida,
            "concluída" => StatusOS.Concluida,
            _ => throw new ArgumentException($"Status inválido: {statusString}", nameof(statusString))
        };
    }

    public static bool IsValidTransition(StatusOS from, StatusOS to)
    {
        return (from, to) switch
        {
            (StatusOS.Pendente, StatusOS.EmAndamento) => true,
            (StatusOS.Pendente, StatusOS.Concluida) => true,
            (StatusOS.EmAndamento, StatusOS.Concluida) => true,
            (StatusOS.EmAndamento, StatusOS.Pendente) => true, // Permitir voltar para pendente
            _ when from == to => false, // Não permitir transição para o mesmo status
            _ => false
        };
    }
}
