using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWES.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaArquivosDocumentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documentos_Estagios_EstagioId",
                table: "Documentos");

            migrationBuilder.AddColumn<string>(
                name: "CaminhoArquivo",
                table: "Documentos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Documentos",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomeArquivo",
                table: "Documentos",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Documentos_Estagios_EstagioId",
                table: "Documentos",
                column: "EstagioId",
                principalTable: "Estagios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documentos_Estagios_EstagioId",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "CaminhoArquivo",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "NomeArquivo",
                table: "Documentos");

            migrationBuilder.AddForeignKey(
                name: "FK_Documentos_Estagios_EstagioId",
                table: "Documentos",
                column: "EstagioId",
                principalTable: "Estagios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
