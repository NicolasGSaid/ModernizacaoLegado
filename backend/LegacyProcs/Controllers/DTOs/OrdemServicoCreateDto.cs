using System.ComponentModel.DataAnnotations;

namespace LegacyProcs.Controllers.DTOs;

/// <summary>
/// DTO para criação de Ordem de Serviço
/// Usado para receber dados do frontend sem expor a entidade diretamente
/// </summary>
public class OrdemServicoCreateDto
{
    [Required(ErrorMessage = "Título é obrigatório")]
    [StringLength(200, ErrorMessage = "Título deve ter no máximo 200 caracteres")]
    public string Titulo { get; set; } = string.Empty;
    
    [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
    public string? Descricao { get; set; }
    
    [Required(ErrorMessage = "Técnico é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "TecnicoId deve ser maior que zero")]
    public int TecnicoId { get; set; }
}

/// <summary>
/// DTO para atualização completa da Ordem de Serviço
/// </summary>
public class OrdemServicoUpdateDto
{
    [Required(ErrorMessage = "Título é obrigatório")]
    [StringLength(200, ErrorMessage = "Título deve ter no máximo 200 caracteres")]
    public string Titulo { get; set; } = string.Empty;
    
    [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
    public string? Descricao { get; set; }
    
    [Required(ErrorMessage = "Técnico é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "TecnicoId deve ser maior que zero")]
    public int TecnicoId { get; set; }
    
    [Required(ErrorMessage = "Status é obrigatório")]
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// DTO para atualização de status da Ordem de Serviço
/// </summary>
public class OrdemServicoUpdateStatusDto
{
    [Required(ErrorMessage = "Status é obrigatório")]
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// DTO para resposta da API
/// </summary>
public class OrdemServicoResponseDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int TecnicoId { get; set; }
    public string TecnicoNome { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
}
