using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegacyProcs.Migrations
{
    /// <inheritdoc />
    public partial class AlterarOrdemServicoTecnicoParaFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Remover índice antigo da coluna Tecnico (string)
            migrationBuilder.DropIndex(
                name: "IX_OrdemServico_Tecnico",
                table: "OrdemServico");

            // 2. Adicionar nova coluna TecnicoId (int) temporária
            migrationBuilder.AddColumn<int>(
                name: "TecnicoId",
                table: "OrdemServico",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                comment: "ID do técnico responsável (FK)");

            // 3. Migrar dados: tentar encontrar técnico pelo nome, senão usar ID 1
            migrationBuilder.Sql(@"
                UPDATE ""OrdemServico"" os
                SET ""TecnicoId"" = COALESCE(
                    (SELECT t.""Id"" FROM ""Tecnico"" t WHERE t.""Nome"" = os.""Tecnico"" LIMIT 1),
                    1
                )
                WHERE os.""Tecnico"" IS NOT NULL;
            ");

            // 4. Remover coluna antiga Tecnico (string)
            migrationBuilder.DropColumn(
                name: "Tecnico",
                table: "OrdemServico");

            // 5. Criar índice na nova coluna TecnicoId
            migrationBuilder.CreateIndex(
                name: "IX_OrdemServico_TecnicoId",
                table: "OrdemServico",
                column: "TecnicoId");

            // 6. Criar Foreign Key
            migrationBuilder.AddForeignKey(
                name: "FK_OrdemServico_Tecnico",
                table: "OrdemServico",
                column: "TecnicoId",
                principalTable: "Tecnico",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. Remover Foreign Key
            migrationBuilder.DropForeignKey(
                name: "FK_OrdemServico_Tecnico",
                table: "OrdemServico");

            // 2. Remover índice TecnicoId
            migrationBuilder.DropIndex(
                name: "IX_OrdemServico_TecnicoId",
                table: "OrdemServico");

            // 3. Adicionar coluna Tecnico (string) de volta
            migrationBuilder.AddColumn<string>(
                name: "Tecnico",
                table: "OrdemServico",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                comment: "Nome do técnico responsável");

            // 4. Migrar dados de volta: copiar nome do técnico
            migrationBuilder.Sql(@"
                UPDATE ""OrdemServico"" os
                SET ""Tecnico"" = COALESCE(
                    (SELECT t.""Nome"" FROM ""Tecnico"" t WHERE t.""Id"" = os.""TecnicoId""),
                    'Técnico Desconhecido'
                );
            ");

            // 5. Remover coluna TecnicoId
            migrationBuilder.DropColumn(
                name: "TecnicoId",
                table: "OrdemServico");

            // 6. Recriar índice na coluna Tecnico
            migrationBuilder.CreateIndex(
                name: "IX_OrdemServico_Tecnico",
                table: "OrdemServico",
                column: "Tecnico");
        }
    }
}
