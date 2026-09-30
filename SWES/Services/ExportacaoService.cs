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

            planilha.Cell(1, 1).Value = "Matrícula/Registro";
            planilha.Cell(1, 2).Value = "Nome";
            planilha.Cell(1, 3).Value = "E-mail";
            planilha.Cell(1, 4).Value = "Perfil";
            planilha.Cell(1, 5).Value = "Situação";

            var cabecalho = planilha.Range(1, 1, 1, 5);
            cabecalho.Style.Font.Bold = true;
            cabecalho.Style.Fill.BackgroundColor = XLColor.LightGray;

            var linha = 2;

            foreach (var usuario in usuarios)
            {
                planilha.Cell(linha, 1).Value = usuario.Inscricao.HasValue
                    ? usuario.Inscricao.Value.ToString()
                    : "-";
                planilha.Cell(linha, 2).Value = usuario.Nome;
                planilha.Cell(linha, 3).Value = usuario.Email;
                planilha.Cell(linha, 4).Value = usuario.Tipo;
                planilha.Cell(linha, 5).Value = usuario.Ativo ? "Ativo" : "Inativo";

                linha++;
            }

            planilha.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }
    }
}