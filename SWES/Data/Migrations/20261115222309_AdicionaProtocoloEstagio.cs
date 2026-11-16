using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWES.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaProtocoloEstagio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Protocolo",
                table: "Estagios",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
                UPDATE Estagios
                SET Protocolo = 'SWES-LEG-' +
                    LEFT(REPLACE(CONVERT(varchar(36), NEWID()), '-', ''), 21)
                WHERE Protocolo = '';
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Estagios_Protocolo",
                table: "Estagios",
                column: "Protocolo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Estagios_Protocolo",
                table: "Estagios");

            migrationBuilder.DropColumn(
                name: "Protocolo",
                table: "Estagios");
        }
    }
}
