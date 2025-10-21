namespace LegacyProcs.Infrastructure.Configuration;

/// <summary>
/// Configurações da API
/// </summary>
public class ApiSettings
{
    public const string SectionName = "Api";

    /// <summary>
    /// Versão da API
    /// </summary>
    public string Version { get; set; } = "v1.0.0";

    /// <summary>
    /// Título da API
    /// </summary>
    public string Title { get; set; } = "LegacyProcs API";

    /// <summary>
    /// Tamanho máximo de página para paginação
    /// </summary>
    public int MaxPageSize { get; set; } = 100;

    /// <summary>
    /// Tamanho padrão de página
    /// </summary>
    public int DefaultPageSize { get; set; } = 10;

    /// <summary>
    /// Habilita Swagger (apenas desenvolvimento)
    /// </summary>
    public bool EnableSwagger { get; set; }

    /// <summary>
    /// Exibe erros detalhados (apenas desenvolvimento/teste)
    /// </summary>
    public bool DetailedErrors { get; set; }
}
