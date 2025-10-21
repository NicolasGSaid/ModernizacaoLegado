using LegacyProcs.Domain.Entities;
using LegacyProcs.Domain.Enums;

namespace LegacyProcs.Domain.Interfaces;

/// <summary>
/// Interface do repositório para Tecnico
/// Define contratos para operações de persistência
/// </summary>
public interface ITecnicoRepository
{
    // Operações básicas CRUD
    Task<Tecnico?> GetByIdAsync(int id);
    Task<IEnumerable<Tecnico>> GetAllAsync();
    Task<Tecnico> AddAsync(Tecnico tecnico);
    Task UpdateAsync(Tecnico tecnico);
    Task DeleteAsync(Tecnico tecnico);

    // Operações com paginação
    Task<(IEnumerable<Tecnico> Items, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        string? filtro = null);

    // Consultas específicas de negócio
    Task<Tecnico?> GetByEmailAsync(string email);
    Task<IEnumerable<Tecnico>> GetByEspecialidadeAsync(string especialidade);
    Task<IEnumerable<Tecnico>> GetByStatusAsync(StatusTecnico status);
    Task<IEnumerable<Tecnico>> GetTecnicosDisponiveisAsync();
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync();
    Task<int> CountByStatusAsync(StatusTecnico status);
}
