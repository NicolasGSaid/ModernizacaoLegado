using Microsoft.EntityFrameworkCore;
using LegacyProcs.Domain.Entities;
using LegacyProcs.Domain.Interfaces;
using LegacyProcs.Infrastructure.Data;

namespace LegacyProcs.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório para OrdemServico usando Entity Framework Core
/// </summary>
public class OrdemServicoRepository : IOrdemServicoRepository
{
    private readonly AppDbContext _context;

    public OrdemServicoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OrdemServico?> GetByIdAsync(int id)
    {
        return await _context.OrdemServico
            .Include(os => os.Tecnico)
            .FirstOrDefaultAsync(os => os.Id == id);
    }

    public async Task<IEnumerable<OrdemServico>> GetAllAsync()
    {
        return await _context.OrdemServico
            .Include(os => os.Tecnico)
            .OrderByDescending(os => os.DataCriacao)
            .ToListAsync();
    }

    public async Task<OrdemServico> AddAsync(OrdemServico ordemServico)
    {
        _context.OrdemServico.Add(ordemServico);
        await _context.SaveChangesAsync();
        return ordemServico;
    }

    public async Task UpdateAsync(OrdemServico ordemServico)
    {
        _context.OrdemServico.Update(ordemServico);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(OrdemServico ordemServico)
    {
        _context.OrdemServico.Remove(ordemServico);
        await _context.SaveChangesAsync();
    }

    public async Task<(IEnumerable<OrdemServico> Items, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        string? filtro = null)
    {
        var query = _context.OrdemServico
            .Include(os => os.Tecnico)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            query = query.Where(os => os.Titulo.Contains(filtro));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(os => os.DataCriacao)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<OrdemServico>> GetByTecnicoAsync(int tecnicoId)
    {
        return await _context.OrdemServico
            .Include(os => os.Tecnico)
            .Where(os => os.TecnicoId == tecnicoId)
            .OrderByDescending(os => os.DataCriacao)
            .ToListAsync();
    }

    public async Task<IEnumerable<OrdemServico>> GetByStatusAsync(string status)
    {
        return await _context.OrdemServico
            .Include(os => os.Tecnico)
            .Where(os => os.GetStatusDisplay() == status)
            .OrderByDescending(os => os.DataCriacao)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.OrdemServico
            .AnyAsync(os => os.Id == id);
    }

    public async Task<int> CountAsync()
    {
        return await _context.OrdemServico.CountAsync();
    }
}
