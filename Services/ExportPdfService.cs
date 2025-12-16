using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

using MiniMarket.Models;

namespace MiniMarket.Services
{
    public class ExportPdfService
    {
        
        public void ExportarVentas(string rutaArchivo, List<ReporteVenta> ventas)
        {
            if (ventas == null) ventas = new List<ReporteVenta>();

            PdfFont fontNormal = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            PdfFont fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            using var writer = new PdfWriter(rutaArchivo);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            
            document.Add(
                new Paragraph("Reporte de Ventas")
                    .SetFont(fontBold)
                    .SetFontSize(16)
                    .SetTextAlignment(TextAlignment.CENTER)
            );

            document.Add(new Paragraph($"Generado: {DateTime.Now:yyyy-MM-dd HH:mm}")
                .SetFont(fontNormal)
                .SetFontSize(10)
                .SetTextAlignment(TextAlignment.CENTER));

            document.Add(new Paragraph(" "));

            
            var table = new Table(3).UseAllAvailableWidth();

            table.AddHeaderCell(new Cell().Add(new Paragraph("Fecha").SetFont(fontBold)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Método Pago").SetFont(fontBold)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Total").SetFont(fontBold)));

            foreach (var v in ventas)
            {
                table.AddCell(new Cell().Add(new Paragraph(v.Fecha ?? "").SetFont(fontNormal)));
                table.AddCell(new Cell().Add(new Paragraph(v.MetodoPago ?? "").SetFont(fontNormal)));
                table.AddCell(new Cell().Add(new Paragraph($"$ {v.Total:N0}").SetFont(fontNormal)));
            }

            document.Add(table);

            double total = ventas.Sum(x => x.Total);

            document.Add(new Paragraph(" "));
            document.Add(new Paragraph($"TOTAL: $ {total:N0}")
                .SetFont(fontBold)
                .SetFontSize(12));

            document.Close();
        }

        
        public void ExportarReporteGuardado(string rutaArchivo, ReporteGuardado reporte)
        {
            PdfFont fontNormal = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            PdfFont fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            using var writer = new PdfWriter(rutaArchivo);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            document.Add(
                new Paragraph(reporte?.Nombre ?? "Reporte Guardado")
                    .SetFont(fontBold)
                    .SetFontSize(16)
                    .SetTextAlignment(TextAlignment.CENTER)
            );

            document.Add(new Paragraph($"Guardado: {reporte?.FechaGuardado:yyyy-MM-dd HH:mm}")
                .SetFont(fontNormal)
                .SetFontSize(10)
                .SetTextAlignment(TextAlignment.CENTER));

            document.Add(new Paragraph(" "));

            string contenido = reporte?.Contenido ?? "";
            var lineas = contenido
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            
            bool pareceTabla = lineas.Any(l => l.Contains("|"));

            if (pareceTabla)
            {
                var table = new Table(3).UseAllAvailableWidth();
                table.AddHeaderCell(new Cell().Add(new Paragraph("Fecha").SetFont(fontBold)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Método").SetFont(fontBold)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Total").SetFont(fontBold)));

                foreach (var l in lineas)
                {
                    var parts = l.Split('|').Select(x => x.Trim()).ToArray();
                    string f = parts.Length > 0 ? parts[0] : "";
                    string m = parts.Length > 1 ? parts[1] : "";
                    string t = parts.Length > 2 ? parts[2] : "";

                    table.AddCell(new Cell().Add(new Paragraph(f).SetFont(fontNormal)));
                    table.AddCell(new Cell().Add(new Paragraph(m).SetFont(fontNormal)));
                    table.AddCell(new Cell().Add(new Paragraph(t).SetFont(fontNormal)));
                }

                document.Add(table);
            }
            else
            {
                document.Add(new Paragraph(contenido).SetFont(fontNormal));
            }

            document.Close();
        }
    }
}
