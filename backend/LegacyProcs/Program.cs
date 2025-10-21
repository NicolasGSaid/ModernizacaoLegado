using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using LegacyProcs.Infrastructure.Data;
using LegacyProcs.Infrastructure.Repositories;
using LegacyProcs.Infrastructure.Swagger;
using LegacyProcs.Infrastructure.Configuration;
using LegacyProcs.Infrastructure.Middleware;
using LegacyProcs.Infrastructure.HealthChecks;
using LegacyProcs.Domain.Interfaces;
using LegacyProcs.Application.Behaviors;
using MediatR;
using FluentValidation;
using Serilog;

// Configure Serilog baseado no ambiente
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var loggerConfig = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "LegacyProcs")
    .Enrich.WithProperty("Environment", environment);

// Configuração por ambiente
if (environment == "Development")
{
    loggerConfig
        .MinimumLevel.Debug()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
        .WriteTo.File("logs/legacyprocs-dev-.txt", 
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
}
else if (environment == "Testing")
{
    loggerConfig
        .MinimumLevel.Warning()
        .WriteTo.Console()
        .WriteTo.File("logs/legacyprocs-test-.txt", rollingInterval: RollingInterval.Day);
}
else // Production
{
    loggerConfig
        .MinimumLevel.Information()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}")
        .WriteTo.File("logs/legacyprocs-.txt", 
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
        .WriteTo.File("logs/errors/legacyprocs-errors-.txt", 
            restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 90);
}

Log.Logger = loggerConfig.CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Host.UseSerilog();

// Configure settings
var securitySettings = builder.Configuration.GetSection(SecuritySettings.SectionName).Get<SecuritySettings>() ?? new SecuritySettings();
var apiSettings = builder.Configuration.GetSection(ApiSettings.SectionName).Get<ApiSettings>() ?? new ApiSettings();

builder.Services.Configure<SecuritySettings>(builder.Configuration.GetSection(SecuritySettings.SectionName));
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection(ApiSettings.SectionName));

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Não escapar caracteres Unicode (acentos, ç, etc)
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "LegacyProcs API",
        Version = "v1.0.0",
        Description = @"
## 🚀 API Modernizada LegacyProcs

### Arquitetura
- **.NET 8 LTS** com Clean Architecture
- **CQRS** com MediatR para separação de responsabilidades
- **Entity Framework Core 8** para acesso a dados seguro
- **FluentValidation** para validações robustas

### Segurança
- ✅ **SQL Injection eliminado** - Uso de EF Core parametrizado
- ✅ **Validações rigorosas** - FluentValidation em todos os endpoints
- ✅ **Logs estruturados** - Serilog para auditoria completa

### Funcionalidades
- **Ordens de Serviço**: CRUD completo com paginação
- **Clientes**: Gerenciamento com validação de CNPJ
- **Técnicos**: Controle de especialidades e status
        ",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Equipe de Desenvolvimento",
            Email = "dev@legacyprocs.com",
            Url = new Uri("https://github.com/alest-github/TesteTimeLegado")
        },
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Incluir comentários XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Configurar exemplos de resposta
    c.EnableAnnotations();
    
    // Configurar esquemas personalizados
    c.SchemaFilter<SwaggerSchemaFilter>();
});

// Add Entity Framework - Detectar provider baseado na connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Log para debug (temporário)
Console.WriteLine($"[DEBUG] Connection String: {(string.IsNullOrEmpty(connectionString) ? "VAZIA!" : connectionString.Substring(0, Math.Min(50, connectionString.Length)) + "...")}");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada! Verifique as variáveis de ambiente.");
}

// Determinar provider ANTES de registrar o DbContext
var isPostgreSQL = connectionString.Contains("postgres", StringComparison.OrdinalIgnoreCase);
Console.WriteLine($"[DEBUG] Provider detectado: {(isPostgreSQL ? "PostgreSQL" : "SQL Server")}");
Console.WriteLine($"[DEBUG] Connection String Length: {connectionString.Length}");
Console.WriteLine($"[DEBUG] Connection String (primeiros 100 chars): {connectionString.Substring(0, Math.Min(100, connectionString.Length))}");

// SOLUÇÃO RADICAL: Usar APENAS OnConfiguring, SEM DI!
Console.WriteLine($"[DEBUG] Configurando connection string APENAS via OnConfiguring (SEM DI)");
AppDbContext.SetConnectionString(connectionString);

// Registrar DbContext SEM configuração - OnConfiguring vai fazer tudo!
builder.Services.AddDbContext<AppDbContext>();

Console.WriteLine($"[DEBUG] DbContext registrado SEM configuração DI - OnConfiguring vai configurar!");

// Add MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
});

// Add FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Add Repositories
builder.Services.AddScoped<IOrdemServicoRepository, OrdemServicoRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ITecnicoRepository, TecnicoRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();

// Add Security Services
builder.Services.AddMemoryCache();
builder.Services.AddSingleton(new LegacyProcs.Infrastructure.Middleware.RateLimitOptions
{
    DefaultMaxRequests = 100,  // Valores mais altos para produção
    GetMaxRequests = 200,      // Valores mais altos para produção
    PostMaxRequests = 50,      // Valores mais altos para produção
    PutMaxRequests = 30,       // Valores mais altos para produção
    DeleteMaxRequests = 10,    // Valores mais altos para produção
    DefaultWindowMinutes = 15  // Janela padrão
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database")
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("API está funcionando"));

// Configure CORS baseado no ambiente
if (securitySettings.EnableCors)
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            if (builder.Environment.IsDevelopment())
            {
                // Desenvolvimento: mais permissivo
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            }
            else
            {
                // Produção: ler origens do appsettings
                var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
                    ?? new[] { "https://legacyprocs-frontend.onrender.com" };
                
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            }
        });
    });
}

var app = builder.Build();

// ✅ EXECUTAR MIGRATIONS AUTOMATICAMENTE NO STARTUP (Production-First)
// Isso garante que o schema do banco esteja sempre atualizado
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("🔄 Verificando e aplicando migrations pendentes...");
        
        // Aplicar migrations pendentes
        var pendingMigrations = context.Database.GetPendingMigrations().ToList();
        if (pendingMigrations.Any())
        {
            logger.LogWarning("⚠️ Encontradas {Count} migrations pendentes: {Migrations}", 
                pendingMigrations.Count, string.Join(", ", pendingMigrations));
            
            context.Database.Migrate();
            
            logger.LogInformation("✅ Migrations aplicadas com sucesso!");
        }
        else
        {
            logger.LogInformation("✅ Banco de dados já está atualizado. Nenhuma migration pendente.");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "❌ ERRO ao aplicar migrations no startup. A aplicação continuará, mas pode haver problemas de schema.");
        // Não lançar exceção para não impedir o startup da aplicação
    }
}

// Configure the HTTP request pipeline.
// Ordem correta: Swagger/Static -> Security -> CORS -> Auth -> Endpoints

// Swagger habilitado sempre que EnableSwagger for true (Global Rules - Production First)
if (apiSettings.EnableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{apiSettings.Title} {apiSettings.Version}");
        c.RoutePrefix = "swagger"; // Swagger em /swagger
        c.DocumentTitle = "LegacyProcs API - Documentação";
    });
}

// Segurança
if (securitySettings.RequireHttps)
{
    app.UseHttpsRedirection();
}

// CORS apenas se habilitado (ANTES de Authorization e Endpoints)
if (securitySettings.EnableCors)
{
    app.UseCors();
}

app.UseAuthorization();

// Middlewares de segurança
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();

// Rota raiz sempre disponível
app.MapGet("/", () => new { 
    message = "LegacyProcs API está funcionando", 
    version = "1.0.0",
    timestamp = DateTime.UtcNow,
    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
    endpoints = new[] { "/health", "/swagger", "/api/ordemservico", "/api/cliente", "/api/tecnico" }
});

// Map Health Checks
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(x => new
            {
                name = x.Key,
                status = x.Value.Status.ToString(),
                // Não expor exception ou detalhes sensíveis em produção
                duration = x.Value.Duration.ToString()
            }),
            totalDuration = report.TotalDuration.ToString()
        };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            WriteIndented = true
        }));
    }
});

// Map Health Checks detalhados (apenas desenvolvimento)
if (app.Environment.IsDevelopment())
{
    app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready")
    });
    
    app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = _ => false
    });
}

app.MapControllers();

app.Run();

// Tornar Program público para testes de integração
public partial class Program { }
