namespace LegacyProcs.Infrastructure.Configuration;

/// <summary>
/// Configurações de segurança da aplicação
/// </summary>
public class SecuritySettings
{
    public const string SectionName = "Security";

    /// <summary>
    /// Requer HTTPS para todas as requisições
    /// </summary>
    public bool RequireHttps { get; set; } = true;

    /// <summary>
    /// Habilita CORS (apenas para desenvolvimento)
    /// </summary>
    public bool EnableCors { get; set; }
}
