using LegacyProcs.Domain.Entities;

namespace LegacyProcs.Domain.Interfaces;

/// <summary>
/// Interface do repositório para OrdemServico
/// Define contratos para operações de persistência
/// </summary>
public interface IOrdemServicoRepository
{
    // Operações básicas CRUD
    Task<OrdemServico?> GetByIdAsync(int id);
    Task<IEnumerable<OrdemServico>> GetAllAsync();
    Task<OrdemServico> AddAsync(OrdemServico ordemServico);
    Task UpdateAsync(OrdemServico ordemServico);
    Task DeleteAsync(OrdemServico ordemServico);

    // Operações com paginação
    Task<(IEnumerable<OrdemServico> Items, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        string? filtro = null);

    // Consultas específicas de negócio
    Task<IEnumerable<OrdemServico>> GetByTecnicoAsync(int tecnicoId);
    Task<IEnumerable<OrdemServico>> GetByStatusAsync(string status);
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync();
}
