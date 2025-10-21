namespace LegacyProcs.Controllers.DTOs;

/// <summary>
/// DTO para atualização de cliente
/// </summary>
public class ClienteUpdateDto
{
    public string RazaoSocial { get; set; } = string.Empty;
    public string? NomeFantasia { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string Endereco { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string CEP { get; set; } = string.Empty;
}
