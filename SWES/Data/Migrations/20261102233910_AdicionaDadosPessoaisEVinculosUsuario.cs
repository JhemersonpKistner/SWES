using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWES.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaDadosPessoaisEVinculosUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Estagios_InstituicoesEnsino_InstituicaoEnsinoId",
                table: "Estagios");

            migrationBuilder.DropTable(
                name: "InstituicoesEnsino");

            migrationBuilder.DropColumn(
                name: "CNPJ",
                table: "SupervisoresEstagio");

            migrationBuilder.DropColumn(
                name: "Empresa",
                table: "SupervisoresEstagio");

            migrationBuilder.DropColumn(
                name: "Curso",
                table: "ProfessoresOrientadores");

            migrationBuilder.DropColumn(
                name: "Curso",
                table: "Alunos");

            migrationBuilder.AddColumn<string>(
                name: "Bairro",
                table: "SupervisoresEstagio",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CEP",
                table: "SupervisoresEstagio",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "SupervisoresEstagio",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endereco",
                table: "SupervisoresEstagio",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "SupervisoresEstagio",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstadoCivil",
                table: "SupervisoresEstagio",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nacionalidade",
                table: "SupervisoresEstagio",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrgaoExpedidor",
                table: "SupervisoresEstagio",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RG",
                table: "SupervisoresEstagio",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefoneSecundario",
                table: "SupervisoresEstagio",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UnidadeConcedenteId",
                table: "SupervisoresEstagio",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bairro",
                table: "ProfessoresOrientadores",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CEP",
                table: "ProfessoresOrientadores",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "ProfessoresOrientadores",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endereco",
                table: "ProfessoresOrientadores",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "ProfessoresOrientadores",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstadoCivil",
                table: "ProfessoresOrientadores",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nacionalidade",
                table: "ProfessoresOrientadores",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrgaoExpedidor",
                table: "ProfessoresOrientadores",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RG",
                table: "ProfessoresOrientadores",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefoneSecundario",
                table: "ProfessoresOrientadores",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bairro",
                table: "Coordenadores",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CEP",
                table: "Coordenadores",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "Coordenadores",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endereco",
                table: "Coordenadores",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Coordenadores",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstadoCivil",
                table: "Coordenadores",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nacionalidade",
                table: "Coordenadores",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrgaoExpedidor",
                table: "Coordenadores",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RG",
                table: "Coordenadores",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefoneSecundario",
                table: "Coordenadores",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bairro",
                table: "Alunos",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CEP",
                table: "Alunos",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "Alunos",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CursoId",
                table: "Alunos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endereco",
                table: "Alunos",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Alunos",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstadoCivil",
                table: "Alunos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nacionalidade",
                table: "Alunos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrgaoExpedidor",
                table: "Alunos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RG",
                table: "Alunos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefoneSecundario",
                table: "Alunos",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupervisoresEstagio_UnidadeConcedenteId",
                table: "SupervisoresEstagio",
                column: "UnidadeConcedenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Alunos_CursoId",
                table: "Alunos",
                column: "CursoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alunos_Cursos_CursoId",
                table: "Alunos",
                column: "CursoId",
                principalTable: "Cursos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Estagios_UnidadesConcedentes_InstituicaoEnsinoId",
                table: "Estagios",
                column: "InstituicaoEnsinoId",
                principalTable: "UnidadesConcedentes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupervisoresEstagio_UnidadesConcedentes_UnidadeConcedenteId",
                table: "SupervisoresEstagio",
                column: "UnidadeConcedenteId",
                principalTable: "UnidadesConcedentes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alunos_Cursos_CursoId",
                table: "Alunos");

            migrationBuilder.DropForeignKey(
                name: "FK_Estagios_UnidadesConcedentes_InstituicaoEnsinoId",
                table: "Estagios");

            migrationBuilder.DropForeignKey(
                name: "FK_SupervisoresEstagio_UnidadesConcedentes_UnidadeConcedenteId",
                table: "SupervisoresEstagio");

            migrationBuilder.DropIndex(
                name: "IX_SupervisoresEstagio_UnidadeConcedenteId",
                table: "SupervisoresEstagio");

            migrationBuilder.DropIndex(
                name: "IX_Alunos_CursoId",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "Bairro",
                table: "SupervisoresEstagio");

            migrationBuilder.DropColumn(
                name: "CEP",
                table: "SupervisoresEstagio");

            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "SupervisoresEstagio");

            migrationBuilder.DropColumn(
                name: "Endereco",
                table: "SupervisoresEstagio");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "SupervisoresEstagio");

            migrationBuilder.DropColumn(
                name: "EstadoCivil",
                table: "SupervisoresEstagio");

            migrationBuilder.DropColumn(
                name: "Nacionalidade",
                table: "SupervisoresEstagio");

            migrationBuilder.DropColumn(
                name: "OrgaoExpedidor",
                table: "SupervisoresEstagio");

            migrationBuilder.DropColumn(
                name: "RG",
                table: "SupervisoresEstagio");

            migrationBuilder.DropColumn(
                name: "TelefoneSecundario",
                table: "SupervisoresEstagio");

            migrationBuilder.DropColumn(
                name: "UnidadeConcedenteId",
                table: "SupervisoresEstagio");

            migrationBuilder.DropColumn(
                name: "Bairro",
                table: "ProfessoresOrientadores");

            migrationBuilder.DropColumn(
                name: "CEP",
                table: "ProfessoresOrientadores");

            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "ProfessoresOrientadores");

            migrationBuilder.DropColumn(
                name: "Endereco",
                table: "ProfessoresOrientadores");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "ProfessoresOrientadores");

            migrationBuilder.DropColumn(
                name: "EstadoCivil",
                table: "ProfessoresOrientadores");

            migrationBuilder.DropColumn(
                name: "Nacionalidade",
                table: "ProfessoresOrientadores");

            migrationBuilder.DropColumn(
                name: "OrgaoExpedidor",
                table: "ProfessoresOrientadores");

            migrationBuilder.DropColumn(
                name: "RG",
                table: "ProfessoresOrientadores");

            migrationBuilder.DropColumn(
                name: "TelefoneSecundario",
                table: "ProfessoresOrientadores");

            migrationBuilder.DropColumn(
                name: "Bairro",
                table: "Coordenadores");

            migrationBuilder.DropColumn(
                name: "CEP",
                table: "Coordenadores");

            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "Coordenadores");

            migrationBuilder.DropColumn(
                name: "Endereco",
                table: "Coordenadores");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Coordenadores");

            migrationBuilder.DropColumn(
                name: "EstadoCivil",
                table: "Coordenadores");

            migrationBuilder.DropColumn(
                name: "Nacionalidade",
                table: "Coordenadores");

            migrationBuilder.DropColumn(
                name: "OrgaoExpedidor",
                table: "Coordenadores");

            migrationBuilder.DropColumn(
                name: "RG",
                table: "Coordenadores");

            migrationBuilder.DropColumn(
                name: "TelefoneSecundario",
                table: "Coordenadores");

            migrationBuilder.DropColumn(
                name: "Bairro",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "CEP",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "CursoId",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "Endereco",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "EstadoCivil",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "Nacionalidade",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "OrgaoExpedidor",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "RG",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "TelefoneSecundario",
                table: "Alunos");

            migrationBuilder.AddColumn<string>(
                name: "CNPJ",
                table: "SupervisoresEstagio",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Empresa",
                table: "SupervisoresEstagio",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Curso",
                table: "ProfessoresOrientadores",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Curso",
                table: "Alunos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "InstituicoesEnsino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Alteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    Cadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cnpj = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstituicoesEnsino", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Estagios_InstituicoesEnsino_InstituicaoEnsinoId",
                table: "Estagios",
                column: "InstituicaoEnsinoId",
                principalTable: "InstituicoesEnsino",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
