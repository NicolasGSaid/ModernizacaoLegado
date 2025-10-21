namespace LegacyProcs.Controllers.DTOs;

public class TecnicoResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Especialidade { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }
}
