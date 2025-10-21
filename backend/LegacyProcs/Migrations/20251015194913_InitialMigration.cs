using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegacyProcs.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Operation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, defaultValue: "Sistema"),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false, defaultValue: "Unknown"),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: "Unknown"),
                    Changes = table.Column<string>(type: "TEXT", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Identificador único do cliente")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RazaoSocial = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "Razão social da empresa"),
                    NomeFantasia = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true, comment: "Nome fantasia da empresa"),
                    CNPJ = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false, comment: "CNPJ da empresa (apenas números)"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Email de contato do cliente"),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "Telefone de contato (apenas números)"),
                    Endereco = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false, comment: "Endereço completo do cliente"),
                    Cidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Cidade do cliente"),
                    Estado = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false, comment: "Estado do cliente (sigla UF)"),
                    CEP = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, comment: "CEP do cliente (apenas números)"),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Data e hora de cadastro do cliente")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrdemServico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Identificador único da ordem de serviço")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "Título da ordem de serviço"),
                    Descricao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true, comment: "Descrição detalhada da ordem de serviço"),
                    Tecnico = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Nome do técnico responsável"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Status atual da ordem de serviço"),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Data e hora de criação da ordem de serviço"),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Data e hora da última atualização")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdemServico", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tecnico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Identificador único do técnico")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Nome completo do técnico"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Email do técnico"),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "Telefone do técnico (apenas números)"),
                    Especialidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Especialidade técnica do profissional"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Status atual do técnico"),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Data e hora de cadastro do técnico")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tecnico", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Entity",
                table: "AuditLogs",
                columns: new[] { "EntityName", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityName",
                table: "AuditLogs",
                column: "EntityName");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Operation",
                table: "AuditLogs",
                column: "Operation");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_SensitiveOperations",
                table: "AuditLogs",
                columns: new[] { "Operation", "Timestamp" },
                filter: "[Operation] IN ('DELETE', 'EXPORT', 'ANONYMIZE')");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Timestamp",
                table: "AuditLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Timestamp_Operation",
                table: "AuditLogs",
                columns: new[] { "Timestamp", "Operation" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_Cidade",
                table: "Cliente",
                column: "Cidade");

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_CNPJ_Unique",
                table: "Cliente",
                column: "CNPJ",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_DataCadastro",
                table: "Cliente",
                column: "DataCadastro");

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_Email_Unique",
                table: "Cliente",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_Estado",
                table: "Cliente",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_Estado_Cidade",
                table: "Cliente",
                columns: new[] { "Estado", "Cidade" });

            migrationBuilder.CreateIndex(
                name: "IX_OrdemServico_DataCriacao",
                table: "OrdemServico",
                column: "DataCriacao");

            migrationBuilder.CreateIndex(
                name: "IX_OrdemServico_Status",
                table: "OrdemServico",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OrdemServico_Status_DataCriacao",
                table: "OrdemServico",
                columns: new[] { "Status", "DataCriacao" });

            migrationBuilder.CreateIndex(
                name: "IX_OrdemServico_Tecnico",
                table: "OrdemServico",
                column: "Tecnico");

            migrationBuilder.CreateIndex(
                name: "IX_Tecnico_DataCadastro",
                table: "Tecnico",
                column: "DataCadastro");

            migrationBuilder.CreateIndex(
                name: "IX_Tecnico_Email_Unique",
                table: "Tecnico",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tecnico_Especialidade",
                table: "Tecnico",
                column: "Especialidade");

            migrationBuilder.CreateIndex(
                name: "IX_Tecnico_Status",
                table: "Tecnico",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Tecnico_Status_Especialidade",
                table: "Tecnico",
                columns: new[] { "Status", "Especialidade" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "OrdemServico");

            migrationBuilder.DropTable(
                name: "Tecnico");
        }
    }
}
