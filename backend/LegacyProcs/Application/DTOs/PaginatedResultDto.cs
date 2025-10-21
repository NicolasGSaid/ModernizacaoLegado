namespace LegacyProcs.Application.DTOs;

/// <summary>
/// DTO para resultados paginados
/// Encapsula dados + metadados de paginação
/// </summary>
public class PaginatedResultDto<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
