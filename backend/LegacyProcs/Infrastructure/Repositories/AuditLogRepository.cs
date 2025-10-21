using LegacyProcs.Domain.Entities;
using LegacyProcs.Domain.Interfaces;
using LegacyProcs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LegacyProcs.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação do repositório de Audit Log - LGPD Compliance
    /// Responsável por persistir e consultar logs de auditoria
    /// </summary>
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext _context;

        public AuditLogRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AdicionarAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
        {
            if (auditLog == null)
                throw new ArgumentNullException(nameof(auditLog));

            await _context.AuditLogs.AddAsync(auditLog, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<AuditLog>> ObterPorEntidadeAsync(string entityName, string? entityId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(entityName))
                throw new ArgumentException("Nome da entidade é obrigatório", nameof(entityName));

            var query = _context.AuditLogs
                .Where(a => a.EntityName == entityName)
                .AsQueryable();

            if (!string.IsNullOrEmpty(entityId))
            {
                query = query.Where(a => a.EntityId == entityId);
            }

            return await query
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<AuditLog>> ObterPorUsuarioAsync(string userId, DateTime? dataInicio = null, DateTime? dataFim = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("ID do usuário é obrigatório", nameof(userId));

            var query = _context.AuditLogs
                .Where(a => a.UserId == userId)
                .AsQueryable();

            if (dataInicio.HasValue)
            {
                query = query.Where(a => a.Timestamp >= dataInicio.Value);
            }

            if (dataFim.HasValue)
            {
                query = query.Where(a => a.Timestamp <= dataFim.Value);
            }

            return await query
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<AuditLog>> ObterPorOperacaoAsync(string operation, DateTime? dataInicio = null, DateTime? dataFim = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(operation))
                throw new ArgumentException("Operação é obrigatória", nameof(operation));

            var query = _context.AuditLogs
                .Where(a => a.Operation == operation.ToUpperInvariant())
                .AsQueryable();

            if (dataInicio.HasValue)
            {
                query = query.Where(a => a.Timestamp >= dataInicio.Value);
            }

            if (dataFim.HasValue)
            {
                query = query.Where(a => a.Timestamp <= dataFim.Value);
            }

            return await query
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<AuditLog>> ObterOperacoesSensiveisAsync(DateTime? dataInicio = null, DateTime? dataFim = null, CancellationToken cancellationToken = default)
        {
            var operacoesSensiveis = new[] { "DELETE", "EXPORT", "ANONYMIZE" };

            var query = _context.AuditLogs
                .Where(a => operacoesSensiveis.Contains(a.Operation))
                .AsQueryable();

            if (dataInicio.HasValue)
            {
                query = query.Where(a => a.Timestamp >= dataInicio.Value);
            }

            if (dataFim.HasValue)
            {
                query = query.Where(a => a.Timestamp <= dataFim.Value);
            }

            return await query
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> ContarTotalAsync(CancellationToken cancellationToken = default)
        {
            return await _context.AuditLogs.CountAsync(cancellationToken);
        }

        public async Task<Dictionary<string, int>> ObterEstatisticasAsync(DateTime dataInicio, DateTime dataFim, CancellationToken cancellationToken = default)
        {
            var estatisticas = await _context.AuditLogs
                .Where(a => a.Timestamp >= dataInicio && a.Timestamp <= dataFim)
                .GroupBy(a => a.Operation)
                .Select(g => new { Operacao = g.Key, Quantidade = g.Count() })
                .ToListAsync(cancellationToken);

            return estatisticas.ToDictionary(e => e.Operacao, e => e.Quantidade);
        }
    }
}
