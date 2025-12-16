using ClosedXML.Excel;
using MiniMarket.Models;

namespace MiniMarket.Services
{
    public static class ExportExcelService
    {
        public static void Exportar(string ruta, ReporteGuardado r)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Reporte");

            ws.Cell(1, 1).Value = "Reporte";
            ws.Cell(1, 2).Value = r.Nombre;

            ws.Cell(2, 1).Value = "Fecha";
            ws.Cell(2, 2).Value = r.FechaGuardado.ToString("yyyy-MM-dd HH:mm");

            ws.Cell(4, 1).Value = "Contenido";
            ws.Cell(5, 1).Value = r.Contenido;

            ws.Columns().AdjustToContents();
            wb.SaveAs(ruta);
        }
    }
}
