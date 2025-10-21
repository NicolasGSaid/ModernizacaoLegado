using Microsoft.EntityFrameworkCore;
using LegacyProcs.Domain.Entities;
using LegacyProcs.Domain.Interfaces;
using LegacyProcs.Infrastructure.Data;

namespace LegacyProcs.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório para Cliente usando Entity Framework Core
/// </summary>
public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> GetByIdAsync(int id)
    {
        return await _context.Cliente.FindAsync(id);
    }

    public async Task<IEnumerable<Cliente>> GetAllAsync()
    {
        return await _context.Cliente
            .OrderBy(c => c.RazaoSocial)
            .ToListAsync();
    }

    public async Task<Cliente> AddAsync(Cliente cliente)
    {
        _context.Cliente.Add(cliente);
        await _context.SaveChangesAsync();
        return cliente;
    }

    public async Task UpdateAsync(Cliente cliente)
    {
        _context.Cliente.Update(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Cliente cliente)
    {
        _context.Cliente.Remove(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task<(IEnumerable<Cliente> Items, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        string? busca = null)
    {
        var query = _context.Cliente.AsQueryable();

        if (!string.IsNullOrWhiteSpace(busca))
        {
            query = query.Where(c => 
                c.RazaoSocial.Contains(busca) ||
                c.NomeFantasia != null && c.NomeFantasia.Contains(busca) ||
                c.CNPJ.Contains(busca) ||
                c.Email.Contains(busca));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(c => c.RazaoSocial)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Cliente?> GetByCNPJAsync(string cnpj)
    {
        return await _context.Cliente
            .FirstOrDefaultAsync(c => c.CNPJ == cnpj);
    }

    public async Task<Cliente?> GetByEmailAsync(string email)
    {
        return await _context.Cliente
            .FirstOrDefaultAsync(c => c.Email == email.ToLowerInvariant());
    }

    public async Task<IEnumerable<Cliente>> GetByCidadeAsync(string cidade)
    {
        return await _context.Cliente
            .Where(c => c.Cidade == cidade)
            .OrderBy(c => c.RazaoSocial)
            .ToListAsync();
    }

    public async Task<IEnumerable<Cliente>> GetByEstadoAsync(string estado)
    {
        return await _context.Cliente
            .Where(c => c.Estado == estado.ToUpperInvariant())
            .OrderBy(c => c.Cidade)
            .ThenBy(c => c.RazaoSocial)
            .ToListAsync();
    }

    public async Task<bool> ExistsByCNPJAsync(string cnpj)
    {
        return await _context.Cliente
            .AnyAsync(c => c.CNPJ == cnpj);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Cliente
            .AnyAsync(c => c.Id == id);
    }

    public async Task<int> CountAsync()
    {
        return await _context.Cliente.CountAsync();
    }
}
