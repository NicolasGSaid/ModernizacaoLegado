using Microsoft.EntityFrameworkCore;
using LegacyProcs.Domain.Entities;
using LegacyProcs.Domain.Enums;
using LegacyProcs.Infrastructure.Configurations;

namespace LegacyProcs.Infrastructure.Data;

/// <summary>
/// Contexto do Entity Framework Core
/// Substitui o ADO.NET legado por ORM moderno e seguro
/// </summary>
public class AppDbContext : DbContext
{
    private static string? _connectionString;
    private static bool _isPostgreSQL;
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    // Método estático para configurar a connection string globalmente
    public static void SetConnectionString(string connectionString)
    {
        // Se for URI do PostgreSQL (postgresql://...), converter para formato Npgsql
        if (connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) ||
            connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"[AppDbContext] Detectado formato URI PostgreSQL - convertendo...");
            _isPostgreSQL = true;
            _connectionString = ConvertPostgresUriToConnectionString(connectionString);
            Console.WriteLine($"[AppDbContext] Connection String convertida: '{_connectionString}'");
        }
        else if (connectionString.Contains("postgres", StringComparison.OrdinalIgnoreCase))
        {
            _isPostgreSQL = true;
            _connectionString = connectionString;
            Console.WriteLine($"[AppDbContext] Connection String PostgreSQL configurada: Length={connectionString.Length}");
        }
        else
        {
            _isPostgreSQL = false;
            _connectionString = connectionString;
            Console.WriteLine($"[AppDbContext] Connection String SQL Server configurada: Length={connectionString.Length}");
        }
    }
    
    // Converter URI do PostgreSQL para formato Npgsql
    private static string ConvertPostgresUriToConnectionString(string uri)
    {
        try
        {
            var parsedUri = new Uri(uri);
            var userInfo = parsedUri.UserInfo.Split(':');
            var username = userInfo[0];
            var password = userInfo.Length > 1 ? userInfo[1] : "";
            var host = parsedUri.Host;
            var port = parsedUri.Port > 0 ? parsedUri.Port : 5432;
            var database = parsedUri.AbsolutePath.TrimStart('/');
            
            return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AppDbContext] ERRO ao converter URI: {ex.Message}");
            return uri; // Retorna original se falhar
        }
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // FORÇAR reconfiguração SEMPRE para garantir que a connection string seja usada
        if (!string.IsNullOrEmpty(_connectionString))
        {
            Console.WriteLine($"[AppDbContext.OnConfiguring] FORÇANDO configuração com connection string estática: Length={_connectionString.Length}");
            Console.WriteLine($"[AppDbContext.OnConfiguring] Connection String COMPLETA: '{_connectionString}'");
            Console.WriteLine($"[AppDbContext.OnConfiguring] IsPostgreSQL: {_isPostgreSQL}");
            
            if (_isPostgreSQL)
            {
                // Usar UseNpgsql SEMPRE, mesmo se já foi configurado
                optionsBuilder.UseNpgsql(_connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(3);
                    npgsqlOptions.CommandTimeout(30);
                });
                Console.WriteLine($"[AppDbContext.OnConfiguring] PostgreSQL configurado com sucesso!");
            }
            else
            {
                optionsBuilder.UseSqlServer(_connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(3);
                    sqlOptions.CommandTimeout(30);
                });
                Console.WriteLine($"[AppDbContext.OnConfiguring] SQL Server configurado com sucesso!");
            }
        }
        else
        {
            Console.WriteLine($"[AppDbContext.OnConfiguring] ERRO: Connection string não configurada!");
        }
        
        base.OnConfiguring(optionsBuilder);
    }

    public DbSet<OrdemServico> OrdemServico { get; set; }
    public DbSet<Cliente> Cliente { get; set; }
    public DbSet<Tecnico> Tecnico { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar configurações centralizadas
        modelBuilder.ApplyConfiguration(new OrdemServicoConfiguration());
        modelBuilder.ApplyConfiguration(new ClienteConfiguration());
        modelBuilder.ApplyConfiguration(new TecnicoConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
    }
}
