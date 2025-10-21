using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LegacyProcs.Domain.Entities;
using LegacyProcs.Domain.Enums;

namespace LegacyProcs.Infrastructure.Configurations;

/// <summary>
/// Configuração específica da entidade Tecnico
/// Centraliza mapeamento e configurações de banco de dados
/// </summary>
public class TecnicoConfiguration : IEntityTypeConfiguration<Tecnico>
{
    public void Configure(EntityTypeBuilder<Tecnico> builder)
    {
        // Configuração da tabela
        builder.ToTable("Tecnico");
        
        // Chave primária
        builder.HasKey(e => e.Id);
        
        // Configuração do ID
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasComment("Identificador único do técnico");

        // Configuração do Nome
        builder.Property(e => e.Nome)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Nome completo do técnico");

        // Configuração do Email
        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Email do técnico");

        // Configuração do Telefone
        builder.Property(e => e.Telefone)
            .HasMaxLength(20)
            .HasComment("Telefone do técnico (apenas números)");

        // Configuração da Especialidade
        builder.Property(e => e.Especialidade)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Especialidade técnica do profissional");

        // Configuração do Status com conversão de enum
        builder.Property(e => e.Status)
            .HasConversion(
                v => v.ToDisplayString(),
                v => StatusTecnicoExtensions.FromString(v))
            .HasMaxLength(50)
            .HasComment("Status atual do técnico");

        // Configuração da Data de Cadastro
        builder.Property(e => e.DataCadastro)
            .IsRequired()
            .HasComment("Data e hora de cadastro do técnico");

        // Índice único
        builder.HasIndex(e => e.Email)
            .IsUnique()
            .HasDatabaseName("IX_Tecnico_Email_Unique");

        // Índices para performance
        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_Tecnico_Status");
            
        builder.HasIndex(e => e.Especialidade)
            .HasDatabaseName("IX_Tecnico_Especialidade");
            
        builder.HasIndex(e => e.DataCadastro)
            .HasDatabaseName("IX_Tecnico_DataCadastro");

        // Índice composto para consultas frequentes
        builder.HasIndex(e => new { e.Status, e.Especialidade })
            .HasDatabaseName("IX_Tecnico_Status_Especialidade");
    }
}
