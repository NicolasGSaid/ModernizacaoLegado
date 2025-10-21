using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using LegacyProcs.Controllers.DTOs;

namespace LegacyProcs.Infrastructure.Swagger;

/// <summary>
/// Filtro para personalizar esquemas Swagger com exemplos
/// </summary>
public class SwaggerSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(OrdemServicoResponseDto))
        {
            schema.Example = new Microsoft.OpenApi.Any.OpenApiObject
            {
                ["id"] = new Microsoft.OpenApi.Any.OpenApiInteger(1),
                ["titulo"] = new Microsoft.OpenApi.Any.OpenApiString("Manutenção preventiva do servidor"),
                ["descricao"] = new Microsoft.OpenApi.Any.OpenApiString("Verificar funcionamento dos componentes e realizar limpeza"),
                ["tecnico"] = new Microsoft.OpenApi.Any.OpenApiString("João Silva"),
                ["status"] = new Microsoft.OpenApi.Any.OpenApiString("Em Andamento"),
                ["dataCriacao"] = new Microsoft.OpenApi.Any.OpenApiString("2025-10-15T10:30:00Z"),
                ["dataAtualizacao"] = new Microsoft.OpenApi.Any.OpenApiString("2025-10-15T14:20:00Z")
            };
        }
        else if (context.Type == typeof(ClienteResponseDto))
        {
            schema.Example = new Microsoft.OpenApi.Any.OpenApiObject
            {
                ["id"] = new Microsoft.OpenApi.Any.OpenApiInteger(1),
                ["razaoSocial"] = new Microsoft.OpenApi.Any.OpenApiString("Tech Solutions Ltda"),
                ["nomeFantasia"] = new Microsoft.OpenApi.Any.OpenApiString("TechSol"),
                ["cnpj"] = new Microsoft.OpenApi.Any.OpenApiString("12345678000195"),
                ["cnpjFormatado"] = new Microsoft.OpenApi.Any.OpenApiString("12.345.678/0001-95"),
                ["email"] = new Microsoft.OpenApi.Any.OpenApiString("contato@techsol.com.br"),
                ["telefone"] = new Microsoft.OpenApi.Any.OpenApiString("(11) 99999-9999"),
                ["endereco"] = new Microsoft.OpenApi.Any.OpenApiString("Rua das Flores, 123"),
                ["cidade"] = new Microsoft.OpenApi.Any.OpenApiString("São Paulo"),
                ["estado"] = new Microsoft.OpenApi.Any.OpenApiString("SP"),
                ["cep"] = new Microsoft.OpenApi.Any.OpenApiString("01234567"),
                ["cepFormatado"] = new Microsoft.OpenApi.Any.OpenApiString("01234-567"),
                ["dataCadastro"] = new Microsoft.OpenApi.Any.OpenApiString("2025-10-15T09:00:00Z")
            };
        }
        else if (context.Type == typeof(TecnicoResponseDto))
        {
            schema.Example = new Microsoft.OpenApi.Any.OpenApiObject
            {
                ["id"] = new Microsoft.OpenApi.Any.OpenApiInteger(1),
                ["nome"] = new Microsoft.OpenApi.Any.OpenApiString("João Silva"),
                ["email"] = new Microsoft.OpenApi.Any.OpenApiString("joao.silva@empresa.com"),
                ["telefone"] = new Microsoft.OpenApi.Any.OpenApiString("(11) 98765-4321"),
                ["especialidade"] = new Microsoft.OpenApi.Any.OpenApiString("Redes e Infraestrutura"),
                ["status"] = new Microsoft.OpenApi.Any.OpenApiString("Ativo"),
                ["dataCadastro"] = new Microsoft.OpenApi.Any.OpenApiString("2025-10-15T08:00:00Z")
            };
        }
        else if (context.Type == typeof(ClienteCreateDto))
        {
            schema.Example = new Microsoft.OpenApi.Any.OpenApiObject
            {
                ["razaoSocial"] = new Microsoft.OpenApi.Any.OpenApiString("Nova Empresa Ltda"),
                ["nomeFantasia"] = new Microsoft.OpenApi.Any.OpenApiString("NovaEmp"),
                ["cnpj"] = new Microsoft.OpenApi.Any.OpenApiString("98765432000187"),
                ["email"] = new Microsoft.OpenApi.Any.OpenApiString("contato@novaemp.com.br"),
                ["telefone"] = new Microsoft.OpenApi.Any.OpenApiString("(11) 88888-8888"),
                ["endereco"] = new Microsoft.OpenApi.Any.OpenApiString("Av. Paulista, 1000"),
                ["cidade"] = new Microsoft.OpenApi.Any.OpenApiString("São Paulo"),
                ["estado"] = new Microsoft.OpenApi.Any.OpenApiString("SP"),
                ["cep"] = new Microsoft.OpenApi.Any.OpenApiString("01310100")
            };
        }
        else if (context.Type == typeof(TecnicoCreateDto))
        {
            schema.Example = new Microsoft.OpenApi.Any.OpenApiObject
            {
                ["nome"] = new Microsoft.OpenApi.Any.OpenApiString("Maria Santos"),
                ["email"] = new Microsoft.OpenApi.Any.OpenApiString("maria.santos@empresa.com"),
                ["telefone"] = new Microsoft.OpenApi.Any.OpenApiString("(11) 77777-7777"),
                ["especialidade"] = new Microsoft.OpenApi.Any.OpenApiString("Desenvolvimento de Software")
            };
        }
    }
}
