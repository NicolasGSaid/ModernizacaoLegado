using System;
using System.ComponentModel.DataAnnotations;

namespace LegacyProcs.Domain.Entities
{
    /// <summary>
    /// Entidade para Audit Trail - LGPD Compliance
    /// Registra todas as operações realizadas nos dados pessoais
    /// Conforme Art. 37 da LGPD - Registro das operações de tratamento
    /// </summary>
    public class AuditLog
    {
        // Construtor privado para EF Core
        private AuditLog() 
        {
            EntityName = string.Empty;
            Operation = string.Empty;
            UserId = string.Empty;
            UserName = string.Empty;
            IpAddress = string.Empty;
            UserAgent = string.Empty;
            Changes = string.Empty;
        }

        // Construtor para criação via factory method
        private AuditLog(string entityName, string entityId, string operation, string userId, 
                        string userName, string ipAddress, string userAgent, string? changes = null)
        {
            ValidarCamposObrigatorios(entityName, operation, userId);

            EntityName = entityName.Trim();
            EntityId = entityId?.Trim();
            Operation = operation.Trim();
            UserId = userId.Trim();
            UserName = userName?.Trim() ?? "Sistema";
            IpAddress = ipAddress?.Trim() ?? "Unknown";
            UserAgent = userAgent?.Trim() ?? "Unknown";
            Changes = changes?.Trim();
            Timestamp = DateTime.UtcNow;
        }

        // Propriedades com encapsulamento
        public int Id { get; private set; }
        
        [Required(ErrorMessage = "Nome da entidade é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome da entidade deve ter no máximo 100 caracteres")]
        public string EntityName { get; private set; }
        
        [StringLength(50, ErrorMessage = "ID da entidade deve ter no máximo 50 caracteres")]
        public string? EntityId { get; private set; }
        
        [Required(ErrorMessage = "Operação é obrigatória")]
        [StringLength(50, ErrorMessage = "Operação deve ter no máximo 50 caracteres")]
        public string Operation { get; private set; }
        
        [Required(ErrorMessage = "ID do usuário é obrigatório")]
        [StringLength(100, ErrorMessage = "ID do usuário deve ter no máximo 100 caracteres")]
        public string UserId { get; private set; }
        
        [StringLength(200, ErrorMessage = "Nome do usuário deve ter no máximo 200 caracteres")]
        public string UserName { get; private set; }
        
        [StringLength(45, ErrorMessage = "Endereço IP deve ter no máximo 45 caracteres")]
        public string IpAddress { get; private set; }
        
        [StringLength(500, ErrorMessage = "User Agent deve ter no máximo 500 caracteres")]
        public string UserAgent { get; private set; }
        
        public string? Changes { get; private set; }
        
        public DateTime Timestamp { get; private set; }

        // Factory methods para diferentes tipos de operação
        public static AuditLog CriarRegistroCreate(string entityName, string entityId, string userId, 
                                                  string userName, string ipAddress, string userAgent, 
                                                  string? dadosInseridos = null)
        {
            return new AuditLog(entityName, entityId, "CREATE", userId, userName, ipAddress, userAgent, dadosInseridos);
        }

        public static AuditLog CriarRegistroRead(string entityName, string entityId, string userId, 
                                               string userName, string ipAddress, string userAgent)
        {
            return new AuditLog(entityName, entityId, "READ", userId, userName, ipAddress, userAgent);
        }

        public static AuditLog CriarRegistroUpdate(string entityName, string entityId, string userId, 
                                                 string userName, string ipAddress, string userAgent, 
                                                 string? alteracoes = null)
        {
            return new AuditLog(entityName, entityId, "UPDATE", userId, userName, ipAddress, userAgent, alteracoes);
        }

        public static AuditLog CriarRegistroDelete(string entityName, string entityId, string userId, 
                                                 string userName, string ipAddress, string userAgent, 
                                                 string? dadosExcluidos = null)
        {
            return new AuditLog(entityName, entityId, "DELETE", userId, userName, ipAddress, userAgent, dadosExcluidos);
        }

        public static AuditLog CriarRegistroExport(string entityName, string userId, 
                                                 string userName, string ipAddress, string userAgent, 
                                                 string? detalhesExportacao = null)
        {
            return new AuditLog(entityName, string.Empty, "EXPORT", userId, userName, ipAddress, userAgent, detalhesExportacao);
        }

        public static AuditLog CriarRegistroAnonymize(string entityName, string entityId, string userId, 
                                                    string userName, string ipAddress, string userAgent)
        {
            return new AuditLog(entityName, entityId, "ANONYMIZE", userId, userName, ipAddress, userAgent, "Dados anonimizados conforme LGPD");
        }

        // Métodos de consulta
        public bool IsOperacaoSensivel()
        {
            return Operation is "DELETE" or "EXPORT" or "ANONYMIZE";
        }

        public bool IsOperacaoLeitura()
        {
            return Operation == "READ";
        }

        public bool IsOperacaoEscrita()
        {
            return Operation is "CREATE" or "UPDATE" or "DELETE";
        }

        public string GetOperacaoDescricao()
        {
            return Operation switch
            {
                "CREATE" => "Criação de registro",
                "READ" => "Consulta de dados",
                "UPDATE" => "Atualização de dados",
                "DELETE" => "Exclusão de dados",
                "EXPORT" => "Exportação de dados",
                "ANONYMIZE" => "Anonimização de dados",
                _ => "Operação desconhecida"
            };
        }

        // Validações de domínio privadas
        private static void ValidarCamposObrigatorios(string entityName, string operation, string userId)
        {
            if (string.IsNullOrWhiteSpace(entityName))
                throw new ArgumentException("Nome da entidade é obrigatório", nameof(entityName));
            
            if (string.IsNullOrWhiteSpace(operation))
                throw new ArgumentException("Operação é obrigatória", nameof(operation));
            
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("ID do usuário é obrigatório", nameof(userId));

            // Validar operações permitidas
            var operacoesValidas = new[] { "CREATE", "READ", "UPDATE", "DELETE", "EXPORT", "ANONYMIZE" };
            if (!operacoesValidas.Contains(operation.ToUpperInvariant()))
                throw new ArgumentException($"Operação '{operation}' não é válida. Operações válidas: {string.Join(", ", operacoesValidas)}", nameof(operation));
        }
    }
}
