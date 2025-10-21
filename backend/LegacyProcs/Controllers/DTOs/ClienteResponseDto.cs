namespace LegacyProcs.Controllers.DTOs;

/// <summary>
/// DTO de resposta para cliente
/// </summary>
public class ClienteResponseDto
{
    public int Id { get; set; }
    public string RazaoSocial { get; set; } = string.Empty;
    public string? NomeFantasia { get; set; }
    public string CNPJ { get; set; } = string.Empty;
    public string CNPJFormatado { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string Endereco { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string CEP { get; set; } = string.Empty;
    public string CEPFormatado { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }
}
