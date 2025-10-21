using LegacyProcs.Application.Common;
using LegacyProcs.Controllers.DTOs;

namespace LegacyProcs.Application.Commands;

/// <summary>
/// Command para criar cliente
/// </summary>
public class CriarClienteCommand : ICommand<ClienteResponseDto>
{
    public string RazaoSocial { get; set; } = string.Empty;
    public string? NomeFantasia { get; set; }
    public string CNPJ { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string Endereco { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string CEP { get; set; } = string.Empty;
}
