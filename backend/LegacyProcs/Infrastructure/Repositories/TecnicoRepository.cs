using Microsoft.EntityFrameworkCore;
using LegacyProcs.Domain.Entities;
using LegacyProcs.Domain.Enums;
using LegacyProcs.Domain.Interfaces;
using LegacyProcs.Infrastructure.Data;

namespace LegacyProcs.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório para Tecnico usando Entity Framework Core
/// </summary>
public class TecnicoRepository : ITecnicoRepository
{
    private readonly AppDbContext _context;

    public TecnicoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Tecnico?> GetByIdAsync(int id)
    {
        return await _context.Tecnico.FindAsync(id);
    }

    public async Task<IEnumerable<Tecnico>> GetAllAsync()
    {
        return await _context.Tecnico
            .OrderBy(t => t.Nome)
            .ToListAsync();
    }

    public async Task<Tecnico> AddAsync(Tecnico tecnico)
    {
        _context.Tecnico.Add(tecnico);
        await _context.SaveChangesAsync();
        return tecnico;
    }

    public async Task UpdateAsync(Tecnico tecnico)
    {
        _context.Tecnico.Update(tecnico);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Tecnico tecnico)
    {
        _context.Tecnico.Remove(tecnico);
        await _context.SaveChangesAsync();
    }

    public async Task<(IEnumerable<Tecnico> Items, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        string? filtro = null)
    {
        var query = _context.Tecnico.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            query = query.Where(t => 
                t.Nome.Contains(filtro) ||
                t.Email.Contains(filtro) ||
                t.Especialidade.Contains(filtro));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(t => t.Nome)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Tecnico?> GetByEmailAsync(string email)
    {
        return await _context.Tecnico
            .FirstOrDefaultAsync(t => t.Email == email.ToLowerInvariant());
    }

    public async Task<IEnumerable<Tecnico>> GetByEspecialidadeAsync(string especialidade)
    {
        return await _context.Tecnico
            .Where(t => t.Especialidade == especialidade)
            .OrderBy(t => t.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tecnico>> GetByStatusAsync(StatusTecnico status)
    {
        return await _context.Tecnico
            .Where(t => t.Status == status)
            .OrderBy(t => t.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tecnico>> GetTecnicosDisponiveisAsync()
    {
        return await _context.Tecnico
            .Where(t => t.Status == StatusTecnico.Ativo)
            .OrderBy(t => t.Nome)
            .ToListAsync();
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Tecnico
            .AnyAsync(t => t.Email == email.ToLowerInvariant());
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Tecnico
            .AnyAsync(t => t.Id == id);
    }

    public async Task<int> CountAsync()
    {
        return await _context.Tecnico.CountAsync();
    }

    public async Task<int> CountByStatusAsync(StatusTecnico status)
    {
        return await _context.Tecnico
            .CountAsync(t => t.Status == status);
    }
}
