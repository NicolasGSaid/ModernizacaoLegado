using LegacyProcs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacyProcs.Infrastructure.Configurations
{
    /// <summary>
    /// Configuração do Entity Framework para AuditLog - LGPD Compliance
    /// Define mapeamento, índices e constraints para otimizar consultas de auditoria
    /// </summary>
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            // Configuração da tabela
            builder.ToTable("AuditLogs");
            
            // Chave primária
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            // Propriedades obrigatórias
            builder.Property(a => a.EntityName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.EntityId)
                .HasMaxLength(50);

            builder.Property(a => a.Operation)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.UserId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.UserName)
                .IsRequired()
                .HasMaxLength(200)
                .HasDefaultValue("Sistema");

            builder.Property(a => a.IpAddress)
                .IsRequired()
                .HasMaxLength(45)
                .HasDefaultValue("Unknown");

            builder.Property(a => a.UserAgent)
                .IsRequired()
                .HasMaxLength(500)
                .HasDefaultValue("Unknown");

            builder.Property(a => a.Changes)
                .HasColumnType("TEXT"); // Para suportar JSON grandes

            builder.Property(a => a.Timestamp)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Índices para otimizar consultas de auditoria
            builder.HasIndex(a => a.EntityName)
                .HasDatabaseName("IX_AuditLogs_EntityName");

            builder.HasIndex(a => new { a.EntityName, a.EntityId })
                .HasDatabaseName("IX_AuditLogs_Entity");

            builder.HasIndex(a => a.UserId)
                .HasDatabaseName("IX_AuditLogs_UserId");

            builder.HasIndex(a => a.Operation)
                .HasDatabaseName("IX_AuditLogs_Operation");

            builder.HasIndex(a => a.Timestamp)
                .HasDatabaseName("IX_AuditLogs_Timestamp");

            // Índice composto para consultas por período e operação
            builder.HasIndex(a => new { a.Timestamp, a.Operation })
                .HasDatabaseName("IX_AuditLogs_Timestamp_Operation");

            // Índice para operações sensíveis (DELETE, EXPORT, ANONYMIZE)
            builder.HasIndex(a => new { a.Operation, a.Timestamp })
                .HasDatabaseName("IX_AuditLogs_SensitiveOperations")
                .HasFilter("[Operation] IN ('DELETE', 'EXPORT', 'ANONYMIZE')");
        }
    }
}
