using LegacyProcs.Domain.Entities;

namespace LegacyProcs.Domain.Interfaces
{
    /// <summary>
    /// Interface para repositório de Audit Log - LGPD Compliance
    /// Responsável por persistir logs de auditoria das operações
    /// </summary>
    public interface IAuditLogRepository
    {
        /// <summary>
        /// Adiciona um novo registro de auditoria
        /// </summary>
        /// <param name="auditLog">Registro de auditoria</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        Task AdicionarAsync(AuditLog auditLog, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém logs de auditoria por entidade
        /// </summary>
        /// <param name="entityName">Nome da entidade</param>
        /// <param name="entityId">ID da entidade (opcional)</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        Task<IEnumerable<AuditLog>> ObterPorEntidadeAsync(string entityName, string? entityId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém logs de auditoria por usuário
        /// </summary>
        /// <param name="userId">ID do usuário</param>
        /// <param name="dataInicio">Data de início (opcional)</param>
        /// <param name="dataFim">Data de fim (opcional)</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        Task<IEnumerable<AuditLog>> ObterPorUsuarioAsync(string userId, DateTime? dataInicio = null, DateTime? dataFim = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém logs de auditoria por operação
        /// </summary>
        /// <param name="operation">Tipo de operação</param>
        /// <param name="dataInicio">Data de início (opcional)</param>
        /// <param name="dataFim">Data de fim (opcional)</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        Task<IEnumerable<AuditLog>> ObterPorOperacaoAsync(string operation, DateTime? dataInicio = null, DateTime? dataFim = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém logs de operações sensíveis (DELETE, EXPORT, ANONYMIZE)
        /// </summary>
        /// <param name="dataInicio">Data de início (opcional)</param>
        /// <param name="dataFim">Data de fim (opcional)</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        Task<IEnumerable<AuditLog>> ObterOperacoesSensiveisAsync(DateTime? dataInicio = null, DateTime? dataFim = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Conta total de registros de auditoria
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento</param>
        Task<int> ContarTotalAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém estatísticas de auditoria
        /// </summary>
        /// <param name="dataInicio">Data de início</param>
        /// <param name="dataFim">Data de fim</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        Task<Dictionary<string, int>> ObterEstatisticasAsync(DateTime dataInicio, DateTime dataFim, CancellationToken cancellationToken = default);
    }
}
