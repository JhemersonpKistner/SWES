using ClosedXML.Excel;
using SWES.ViewModels;

namespace SWES.Services
{
    public class ExportacaoService
    {
        public byte[] ExportarUsuariosParaExcel(List<UsuarioViewModel> usuarios)
        {
            using var workbook = new XLWorkbook();
            var planilha = workbook.Worksheets.Add("Usuarios");

            // Cabeçalhos
            string[] cabecalhos =
            {
                "ID",
                "Nome",
                "E-mail",
                "Telefone",
                "CPF",
                "Perfil",
                "Situação",
                "Matrícula",
                "Registro",
                "Curso",
                "Turno",
                "Semestre",
                "Data de Nascimento",
                "Cursos do Professor",
                "Cargo",
                "Empresa",
                "CNPJ",
                "Setor"
            };

            for (int i = 0; i < cabecalhos.Length; i++)
            {
                planilha.Cell(1, i + 1).Value = cabecalhos[i];
            }

            var cabecalho = planilha.Range(1, 1, 1, cabecalhos.Length);
            cabecalho.Style.Font.Bold = true;
            cabecalho.Style.Fill.BackgroundColor = XLColor.LightGray;

            int linha = 2;

            foreach (var usuario in usuarios)
            {
                planilha.Cell(linha, 1).Value = usuario.Id;
                planilha.Cell(linha, 2).Value = usuario.Nome;
                planilha.Cell(linha, 3).Value = usuario.Email;
                planilha.Cell(linha, 4).Value = usuario.Telefone;
                planilha.Cell(linha, 5).Value = usuario.CPF;
                planilha.Cell(linha, 6).Value = usuario.Tipo;
                planilha.Cell(linha, 7).Value = usuario.Ativo ? "Ativo" : "Inativo";
                planilha.Cell(linha, 8).Value = usuario.Matricula;
                planilha.Cell(linha, 9).Value = usuario.Registro;
                planilha.Cell(linha, 10).Value = usuario.Curso;
                planilha.Cell(linha, 11).Value = usuario.Turno;
                planilha.Cell(linha, 12).Value = usuario.Semestre;

                if (usuario.DataNasc.HasValue)
                {
                    planilha.Cell(linha, 13).Value =
                        usuario.DataNasc.Value.ToString("dd/MM/yyyy");
                }

                if (usuario.CursosProfessor != null)
                {
                    planilha.Cell(linha, 14).Value =
                        string.Join(", ", usuario.CursosProfessor);
                }

                planilha.Cell(linha, 15).Value = usuario.Cargo;
                planilha.Cell(linha, 16).Value = usuario.Empresa;
                planilha.Cell(linha, 17).Value = usuario.CNPJ;
                planilha.Cell(linha, 18).Value = usuario.Setor;

                linha++;
            }

            planilha.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }
    }
}