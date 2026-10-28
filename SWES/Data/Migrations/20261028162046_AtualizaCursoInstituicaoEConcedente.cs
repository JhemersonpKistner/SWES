using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWES.Data.Migrations
{
    /// <inheritdoc />
    public partial class AtualizaCursoInstituicaoEConcedente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
{
    // Impede a exclusão de cursos ou vínculos existentes.
    migrationBuilder.Sql("""
        IF EXISTS (SELECT 1 FROM dbo.Cursos)
           OR EXISTS (SELECT 1 FROM dbo.CursoProfessorOrientador)
        BEGIN
            THROW 50002,
                'Migracao cancelada: existem cursos ou vinculos cadastrados. Remova os dados de teste antes de continuar.',
                1;
        END
        """);

    // Remove temporariamente as restrições dos estágios.
    migrationBuilder.DropForeignKey(
        name: "FK_Estagios_InstituicoesEnsino_InstituicaoEnsinoId",
        table: "Estagios");

    migrationBuilder.DropForeignKey(
        name: "FK_Estagios_UnidadesConcedentes_UnidadeConcedenteId",
        table: "Estagios");

    // Remove a tabela intermediária antes da tabela Cursos.
    migrationBuilder.DropTable(
        name: "CursoProfessorOrientador");

    migrationBuilder.DropTable(
        name: "Cursos");

    // Recria Cursos com chave primária Guid.
    migrationBuilder.CreateTable(
        name: "Cursos",
        columns: table => new
        {
            Id = table.Column<Guid>(
                type: "uniqueidentifier",
                nullable: false),

            Cadastro = table.Column<DateTime>(
                type: "datetime2",
                nullable: false),

            Alteracao = table.Column<DateTime>(
                type: "datetime2",
                nullable: true),

            Ativo = table.Column<bool>(
                type: "bit",
                nullable: false),

            Nome = table.Column<string>(
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false),

            QuantidadeSemestres = table.Column<int>(
                type: "int",
                nullable: false),

            Periodo = table.Column<string>(
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false),

            InstituicaoId = table.Column<Guid>(
                type: "uniqueidentifier",
                nullable: false)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_Cursos", x => x.Id);

            table.ForeignKey(
                name: "FK_Cursos_UnidadesConcedentes_InstituicaoId",
                column: x => x.InstituicaoId,
                principalTable: "UnidadesConcedentes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        });

    migrationBuilder.CreateIndex(
        name: "IX_Cursos_InstituicaoId",
        table: "Cursos",
        column: "InstituicaoId");

    // Recria a relação entre cursos e professores.
    migrationBuilder.CreateTable(
        name: "CursoProfessorOrientador",
        columns: table => new
        {
            CursosId = table.Column<Guid>(
                type: "uniqueidentifier",
                nullable: false),

            ProfessoresId = table.Column<Guid>(
                type: "uniqueidentifier",
                nullable: false)
        },
        constraints: table =>
        {
            table.PrimaryKey(
                "PK_CursoProfessorOrientador",
                x => new { x.CursosId, x.ProfessoresId });

            table.ForeignKey(
                name: "FK_CursoProfessorOrientador_Cursos_CursosId",
                column: x => x.CursosId,
                principalTable: "Cursos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            table.ForeignKey(
                name: "FK_CursoProfessorOrientador_ProfessoresOrientadores_ProfessoresId",
                column: x => x.ProfessoresId,
                principalTable: "ProfessoresOrientadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        });

    migrationBuilder.CreateIndex(
        name: "IX_CursoProfessorOrientador_ProfessoresId",
        table: "CursoProfessorOrientador",
        column: "ProfessoresId");

    // Recria as restrições dos estágios com Restrict.
    migrationBuilder.AddForeignKey(
        name: "FK_Estagios_InstituicoesEnsino_InstituicaoEnsinoId",
        table: "Estagios",
        column: "InstituicaoEnsinoId",
        principalTable: "InstituicoesEnsino",
        principalColumn: "Id",
        onDelete: ReferentialAction.Restrict);

    migrationBuilder.AddForeignKey(
        name: "FK_Estagios_UnidadesConcedentes_UnidadeConcedenteId",
        table: "Estagios",
        column: "UnidadeConcedenteId",
        principalTable: "UnidadesConcedentes",
        principalColumn: "Id",
        onDelete: ReferentialAction.Restrict);
}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
{
    throw new NotSupportedException(
        "Reversao automatica nao suportada: esta migration recria as tabelas Cursos e CursoProfessorOrientador. Restaure um backup para retornar a estrutura anterior.");
}
    }
}
