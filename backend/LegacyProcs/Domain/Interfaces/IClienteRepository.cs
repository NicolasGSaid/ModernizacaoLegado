using LegacyProcs.Domain.Entities;

namespace LegacyProcs.Domain.Interfaces;

/// <summary>
/// Interface do repositório para Cliente
/// Define contratos para operações de persistência
/// </summary>
public interface IClienteRepository
{
    // Operações básicas CRUD
    Task<Cliente?> GetByIdAsync(int id);
    Task<IEnumerable<Cliente>> GetAllAsync();
    Task<Cliente> AddAsync(Cliente cliente);
    Task UpdateAsync(Cliente cliente);
    Task DeleteAsync(Cliente cliente);

    // Operações com paginação
    Task<(IEnumerable<Cliente> Items, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        string? busca = null);

    // Consultas específicas de negócio
    Task<Cliente?> GetByCNPJAsync(string cnpj);
    Task<Cliente?> GetByEmailAsync(string email);
    Task<IEnumerable<Cliente>> GetByCidadeAsync(string cidade);
    Task<IEnumerable<Cliente>> GetByEstadoAsync(string estado);
    Task<bool> ExistsByCNPJAsync(string cnpj);
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync();
}
