using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWES.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaRelacionamentosIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "SupervisoresEstagio",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ProfessoresOrientadores",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Coordenadores",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Alunos",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_SupervisoresEstagio_UserId",
                table: "SupervisoresEstagio",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessoresOrientadores_UserId",
                table: "ProfessoresOrientadores",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Coordenadores_UserId",
                table: "Coordenadores",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Alunos_UserId",
                table: "Alunos",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alunos_AspNetUsers_UserId",
                table: "Alunos",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Coordenadores_AspNetUsers_UserId",
                table: "Coordenadores",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessoresOrientadores_AspNetUsers_UserId",
                table: "ProfessoresOrientadores",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupervisoresEstagio_AspNetUsers_UserId",
                table: "SupervisoresEstagio",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alunos_AspNetUsers_UserId",
                table: "Alunos");

            migrationBuilder.DropForeignKey(
                name: "FK_Coordenadores_AspNetUsers_UserId",
                table: "Coordenadores");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfessoresOrientadores_AspNetUsers_UserId",
                table: "ProfessoresOrientadores");

            migrationBuilder.DropForeignKey(
                name: "FK_SupervisoresEstagio_AspNetUsers_UserId",
                table: "SupervisoresEstagio");

            migrationBuilder.DropIndex(
                name: "IX_SupervisoresEstagio_UserId",
                table: "SupervisoresEstagio");

            migrationBuilder.DropIndex(
                name: "IX_ProfessoresOrientadores_UserId",
                table: "ProfessoresOrientadores");

            migrationBuilder.DropIndex(
                name: "IX_Coordenadores_UserId",
                table: "Coordenadores");

            migrationBuilder.DropIndex(
                name: "IX_Alunos_UserId",
                table: "Alunos");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "SupervisoresEstagio",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ProfessoresOrientadores",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Coordenadores",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Alunos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
