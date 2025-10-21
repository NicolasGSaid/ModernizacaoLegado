using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LegacyProcs.Domain.Entities;
using LegacyProcs.Domain.Enums;

namespace LegacyProcs.Infrastructure.Configurations;

/// <summary>
/// Configuração específica da entidade OrdemServico
/// Centraliza mapeamento e configurações de banco de dados
/// </summary>
public class OrdemServicoConfiguration : IEntityTypeConfiguration<OrdemServico>
{
    public void Configure(EntityTypeBuilder<OrdemServico> builder)
    {
        // Configuração da tabela
        builder.ToTable("OrdemServico");
        
        // Chave primária
        builder.HasKey(e => e.Id);
        
        // Configuração do ID
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasComment("Identificador único da ordem de serviço");

        // Configuração do Título
        builder.Property(e => e.Titulo)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Título da ordem de serviço");

        // Configuração da Descrição
        builder.Property(e => e.Descricao)
            .HasMaxLength(1000)
            .HasComment("Descrição detalhada da ordem de serviço");

        // Configuração do TecnicoId (FK)
        builder.Property(e => e.TecnicoId)
            .IsRequired()
            .HasComment("ID do técnico responsável (FK)");

        // Configuração do relacionamento com Tecnico
        builder.HasOne(e => e.Tecnico)
            .WithMany()
            .HasForeignKey(e => e.TecnicoId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_OrdemServico_Tecnico");

        // Configuração do Status com conversão de enum
        builder.Property(e => e.Status)
            .HasConversion(
                v => v.ToDisplayString(),
                v => StatusOSExtensions.FromString(v))
            .HasMaxLength(50)
            .HasComment("Status atual da ordem de serviço");

        // Configuração das datas
        builder.Property(e => e.DataCriacao)
            .IsRequired()
            .HasComment("Data e hora de criação da ordem de serviço");

        builder.Property(e => e.DataAtualizacao)
            .HasComment("Data e hora da última atualização");

        // Índices para performance
        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_OrdemServico_Status");
            
        builder.HasIndex(e => e.DataCriacao)
            .HasDatabaseName("IX_OrdemServico_DataCriacao");
            
        builder.HasIndex(e => e.TecnicoId)
            .HasDatabaseName("IX_OrdemServico_TecnicoId");

        // Índice composto para consultas frequentes
        builder.HasIndex(e => new { e.Status, e.DataCriacao })
            .HasDatabaseName("IX_OrdemServico_Status_DataCriacao");
    }
}
