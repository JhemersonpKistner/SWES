using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWES.Data.Migrations
{
    /// <inheritdoc />
    public partial class VinculaUsuariosInstituicoesEnsino : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InstituicaoEnsinoId",
                table: "SupervisoresEstagio",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InstituicaoEnsinoId",
                table: "ProfessoresOrientadores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InstituicaoEnsinoId",
                table: "Coordenadores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InstituicaoEnsinoId",
                table: "Alunos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProfessorInstituicoesEnsino",
                columns: table => new
                {
                    ProfessorOrientadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstituicaoEnsinoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessorInstituicoesEnsino", x => new { x.ProfessorOrientadorId, x.InstituicaoEnsinoId });
                    table.ForeignKey(
                        name: "FK_ProfessorInstituicoesEnsino_ProfessoresOrientadores_ProfessorOrientadorId",
                        column: x => x.ProfessorOrientadorId,
                        principalTable: "ProfessoresOrientadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfessorInstituicoesEnsino_UnidadesConcedentes_InstituicaoEnsinoId",
                        column: x => x.InstituicaoEnsinoId,
                        principalTable: "UnidadesConcedentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupervisoresEstagio_InstituicaoEnsinoId",
                table: "SupervisoresEstagio",
                column: "InstituicaoEnsinoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessoresOrientadores_InstituicaoEnsinoId",
                table: "ProfessoresOrientadores",
                column: "InstituicaoEnsinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Coordenadores_InstituicaoEnsinoId",
                table: "Coordenadores",
                column: "InstituicaoEnsinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Alunos_InstituicaoEnsinoId",
                table: "Alunos",
                column: "InstituicaoEnsinoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessorInstituicoesEnsino_InstituicaoEnsinoId",
                table: "ProfessorInstituicoesEnsino",
                column: "InstituicaoEnsinoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alunos_UnidadesConcedentes_InstituicaoEnsinoId",
                table: "Alunos",
                column: "InstituicaoEnsinoId",
                principalTable: "UnidadesConcedentes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Coordenadores_UnidadesConcedentes_InstituicaoEnsinoId",
                table: "Coordenadores",
                column: "InstituicaoEnsinoId",
                principalTable: "UnidadesConcedentes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessoresOrientadores_UnidadesConcedentes_InstituicaoEnsinoId",
                table: "ProfessoresOrientadores",
                column: "InstituicaoEnsinoId",
                principalTable: "UnidadesConcedentes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupervisoresEstagio_UnidadesConcedentes_InstituicaoEnsinoId",
                table: "SupervisoresEstagio",
                column: "InstituicaoEnsinoId",
                principalTable: "UnidadesConcedentes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alunos_UnidadesConcedentes_InstituicaoEnsinoId",
                table: "Alunos");

            migrationBuilder.DropForeignKey(
                name: "FK_Coordenadores_UnidadesConcedentes_InstituicaoEnsinoId",
                table: "Coordenadores");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfessoresOrientadores_UnidadesConcedentes_InstituicaoEnsinoId",
                table: "ProfessoresOrientadores");

            migrationBuilder.DropForeignKey(
                name: "FK_SupervisoresEstagio_UnidadesConcedentes_InstituicaoEnsinoId",
                table: "SupervisoresEstagio");

            migrationBuilder.DropTable(
                name: "ProfessorInstituicoesEnsino");

            migrationBuilder.DropIndex(
                name: "IX_SupervisoresEstagio_InstituicaoEnsinoId",
                table: "SupervisoresEstagio");

            migrationBuilder.DropIndex(
                name: "IX_ProfessoresOrientadores_InstituicaoEnsinoId",
                table: "ProfessoresOrientadores");

            migrationBuilder.DropIndex(
                name: "IX_Coordenadores_InstituicaoEnsinoId",
                table: "Coordenadores");

            migrationBuilder.DropIndex(
                name: "IX_Alunos_InstituicaoEnsinoId",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "InstituicaoEnsinoId",
                table: "SupervisoresEstagio");

            migrationBuilder.DropColumn(
                name: "InstituicaoEnsinoId",
                table: "ProfessoresOrientadores");

            migrationBuilder.DropColumn(
                name: "InstituicaoEnsinoId",
                table: "Coordenadores");

            migrationBuilder.DropColumn(
                name: "InstituicaoEnsinoId",
                table: "Alunos");
        }
    }
}
