using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LegacyProcs.Domain.Entities;

namespace LegacyProcs.Infrastructure.Configurations;

/// <summary>
/// Configuração específica da entidade Cliente
/// Centraliza mapeamento e configurações de banco de dados
/// </summary>
public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        // Configuração da tabela
        builder.ToTable("Cliente");
        
        // Chave primária
        builder.HasKey(e => e.Id);
        
        // Configuração do ID
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasComment("Identificador único do cliente");

        // Configuração da Razão Social
        builder.Property(e => e.RazaoSocial)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Razão social da empresa");

        // Configuração do Nome Fantasia
        builder.Property(e => e.NomeFantasia)
            .HasMaxLength(200)
            .HasComment("Nome fantasia da empresa");

        // Configuração do CNPJ
        builder.Property(e => e.CNPJ)
            .IsRequired()
            .HasMaxLength(18)
            .HasComment("CNPJ da empresa (apenas números)");

        // Configuração do Email
        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Email de contato do cliente");

        // Configuração do Telefone
        builder.Property(e => e.Telefone)
            .HasMaxLength(20)
            .HasComment("Telefone de contato (apenas números)");

        // Configuração do Endereço
        builder.Property(e => e.Endereco)
            .IsRequired()
            .HasMaxLength(300)
            .HasComment("Endereço completo do cliente");

        // Configuração da Cidade
        builder.Property(e => e.Cidade)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Cidade do cliente");

        // Configuração do Estado
        builder.Property(e => e.Estado)
            .IsRequired()
            .HasMaxLength(2)
            .HasComment("Estado do cliente (sigla UF)");

        // Configuração do CEP
        builder.Property(e => e.CEP)
            .IsRequired()
            .HasMaxLength(10)
            .HasComment("CEP do cliente (apenas números)");

        // Configuração da Data de Cadastro
        builder.Property(e => e.DataCadastro)
            .IsRequired()
            .HasComment("Data e hora de cadastro do cliente");

        // Índices únicos
        builder.HasIndex(e => e.CNPJ)
            .IsUnique()
            .HasDatabaseName("IX_Cliente_CNPJ_Unique");
            
        builder.HasIndex(e => e.Email)
            .IsUnique()
            .HasDatabaseName("IX_Cliente_Email_Unique");

        // Índices para performance
        builder.HasIndex(e => e.Cidade)
            .HasDatabaseName("IX_Cliente_Cidade");
            
        builder.HasIndex(e => e.Estado)
            .HasDatabaseName("IX_Cliente_Estado");
            
        builder.HasIndex(e => e.DataCadastro)
            .HasDatabaseName("IX_Cliente_DataCadastro");

        // Índice composto para consultas por localização
        builder.HasIndex(e => new { e.Estado, e.Cidade })
            .HasDatabaseName("IX_Cliente_Estado_Cidade");
    }
}
