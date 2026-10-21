using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWES.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConverteIdsParaGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
            IF EXISTS (SELECT 1 FROM dbo.Alunos)
            OR EXISTS (SELECT 1 FROM dbo.Coordenadores)
            OR EXISTS (SELECT 1 FROM dbo.InstituicoesEnsino)
            OR EXISTS (SELECT 1 FROM dbo.ProfessoresOrientadores)
            OR EXISTS (SELECT 1 FROM dbo.SupervisoresEstagio)
            OR EXISTS (SELECT 1 FROM dbo.UnidadesConcedentes)
            OR EXISTS (SELECT 1 FROM dbo.Estagios)
            OR EXISTS (SELECT 1 FROM dbo.Avaliacoes)
            OR EXISTS (SELECT 1 FROM dbo.CursoProfessorOrientador)
            OR EXISTS (SELECT 1 FROM dbo.Atividades)
            OR EXISTS (SELECT 1 FROM dbo.Documentos)
            BEGIN
            THROW 50001,
            'Migracao cancelada: existem dados nas tabelas do SWES.',
            1;
            END
            """);

            migrationBuilder.DropTable(name: "Atividades");
            migrationBuilder.DropTable(name: "Avaliacoes");
            migrationBuilder.DropTable(name: "Documentos");
            migrationBuilder.DropTable(name: "CursoProfessorOrientador");
            migrationBuilder.DropTable(name: "Estagios");

            migrationBuilder.DropTable(name: "Alunos");
            migrationBuilder.DropTable(name: "Coordenadores");
            migrationBuilder.DropTable(name: "InstituicoesEnsino");
            migrationBuilder.DropTable(name: "ProfessoresOrientadores");
            migrationBuilder.DropTable(name: "SupervisoresEstagio");
            migrationBuilder.DropTable(name: "UnidadesConcedentes");

            migrationBuilder.CreateTable(
                name: "Alunos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Alteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    CPF = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Matricula = table.Column<int>(type: "int", nullable: false),
                    Curso = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Turno = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Semestre = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DataNasc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alunos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alunos_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alunos_UserId",
                table: "Alunos",
                column: "UserId");

                migrationBuilder.CreateTable(
                name: "Coordenadores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Alteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    CPF = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Setor = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coordenadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coordenadores_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coordenadores_UserId",
                table: "Coordenadores",
                column: "UserId");

                        migrationBuilder.CreateTable(
                name: "InstituicoesEnsino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Alteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    Nome = table.Column<string>(
                        type: "nvarchar(150)",
                        maxLength: 150,
                        nullable: false),
                    Cnpj = table.Column<string>(
                        type: "nvarchar(14)",
                        maxLength: 14,
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstituicoesEnsino", x => x.Id);
                });

                            migrationBuilder.CreateTable(
                name: "ProfessoresOrientadores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Alteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    CPF = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Registro = table.Column<int>(type: "int", nullable: false),
                    Curso = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessoresOrientadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfessoresOrientadores_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfessoresOrientadores_UserId",
                table: "ProfessoresOrientadores",
                column: "UserId");

                        migrationBuilder.CreateTable(
                name: "SupervisoresEstagio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Alteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    CPF = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    CNPJ = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Cargo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Empresa = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupervisoresEstagio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupervisoresEstagio_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupervisoresEstagio_UserId",
                table: "SupervisoresEstagio",
                column: "UserId");

                        migrationBuilder.CreateTable(
                name: "UnidadesConcedentes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Alteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    RazaoSocial = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Cnpj = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    InstituicaoEnsino = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadesConcedentes", x => x.Id);
                });

                            migrationBuilder.CreateTable(
                name: "Estagios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Horario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Situacao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlunoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfessorOrientadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupervisorEstagioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoordenadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnidadeConcedenteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstituicaoEnsinoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estagios", x => x.Id);

                    table.ForeignKey(
                        name: "FK_Estagios_Alunos_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Alunos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                        name: "FK_Estagios_Coordenadores_CoordenadorId",
                        column: x => x.CoordenadorId,
                        principalTable: "Coordenadores",
                        principalColumn: "Id");

                    table.ForeignKey(
                        name: "FK_Estagios_InstituicoesEnsino_InstituicaoEnsinoId",
                        column: x => x.InstituicaoEnsinoId,
                        principalTable: "InstituicoesEnsino",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                        name: "FK_Estagios_ProfessoresOrientadores_ProfessorOrientadorId",
                        column: x => x.ProfessorOrientadorId,
                        principalTable: "ProfessoresOrientadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                        name: "FK_Estagios_SupervisoresEstagio_SupervisorEstagioId",
                        column: x => x.SupervisorEstagioId,
                        principalTable: "SupervisoresEstagio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                        name: "FK_Estagios_UnidadesConcedentes_UnidadeConcedenteId",
                        column: x => x.UnidadeConcedenteId,
                        principalTable: "UnidadesConcedentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Estagios_AlunoId",
                table: "Estagios",
                column: "AlunoId");

            migrationBuilder.CreateIndex(
                name: "IX_Estagios_CoordenadorId",
                table: "Estagios",
                column: "CoordenadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Estagios_InstituicaoEnsinoId",
                table: "Estagios",
                column: "InstituicaoEnsinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Estagios_ProfessorOrientadorId",
                table: "Estagios",
                column: "ProfessorOrientadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Estagios_SupervisorEstagioId",
                table: "Estagios",
                column: "SupervisorEstagioId");

            migrationBuilder.CreateIndex(
                name: "IX_Estagios_UnidadeConcedenteId",
                table: "Estagios",
                column: "UnidadeConcedenteId");

                        migrationBuilder.CreateTable(
                name: "CursoProfessorOrientador",
                columns: table => new
                {
                    CursosId = table.Column<int>(type: "int", nullable: false),
                    ProfessoresId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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

                        migrationBuilder.CreateTable(
                name: "Avaliacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nota = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataAvaliacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstagioId = table.Column<int>(type: "int", nullable: false),
                    ProfessorOrientadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avaliacoes", x => x.Id);

                    table.ForeignKey(
                        name: "FK_Avaliacoes_Estagios_EstagioId",
                        column: x => x.EstagioId,
                        principalTable: "Estagios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                        name: "FK_Avaliacoes_ProfessoresOrientadores_ProfessorOrientadorId",
                        column: x => x.ProfessorOrientadorId,
                        principalTable: "ProfessoresOrientadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacoes_EstagioId",
                table: "Avaliacoes",
                column: "EstagioId");

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacoes_ProfessorOrientadorId",
                table: "Avaliacoes",
                column: "ProfessorOrientadorId");

                        migrationBuilder.CreateTable(
                name: "Atividades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Horas = table.Column<int>(type: "int", nullable: false),
                    EstagioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atividades", x => x.Id);

                    table.ForeignKey(
                        name: "FK_Atividades_Estagios_EstagioId",
                        column: x => x.EstagioId,
                        principalTable: "Estagios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Atividades_EstagioId",
                table: "Atividades",
                column: "EstagioId");

                        migrationBuilder.CreateTable(
                name: "Documentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataEnvio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstagioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documentos", x => x.Id);

                    table.ForeignKey(
                        name: "FK_Documentos_Estagios_EstagioId",
                        column: x => x.EstagioId,
                        principalTable: "Estagios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_EstagioId",
                table: "Documentos",
                column: "EstagioId");

        }

                /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotSupportedException(
                "Reversao ainda nao implementada para a conversao de IDs para Guid.");
        }
    }
}
