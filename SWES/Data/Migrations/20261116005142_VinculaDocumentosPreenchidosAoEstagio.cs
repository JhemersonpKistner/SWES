using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWES.Data.Migrations
{
    /// <inheritdoc />
    public partial class VinculaDocumentosPreenchidosAoEstagio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstagioId",
                table: "DocumentosPreenchidos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosPreenchidos_EstagioId",
                table: "DocumentosPreenchidos",
                column: "EstagioId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentosPreenchidos_Estagios_EstagioId",
                table: "DocumentosPreenchidos",
                column: "EstagioId",
                principalTable: "Estagios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentosPreenchidos_Estagios_EstagioId",
                table: "DocumentosPreenchidos");

            migrationBuilder.DropIndex(
                name: "IX_DocumentosPreenchidos_EstagioId",
                table: "DocumentosPreenchidos");

            migrationBuilder.DropColumn(
                name: "EstagioId",
                table: "DocumentosPreenchidos");
        }
    }
}
