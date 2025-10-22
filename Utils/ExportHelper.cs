using System;
using System.Data;
using System.IO;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfDoc = iTextSharp.text.Document;


namespace QLCSVCWinApp.Utils
{
    public static class ExportHelper
    {
        public static void ToExcel(DataTable dt, string path, string title,
                                   string? subtitle = null, string? footer = null)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Report");

            int r = 1;
            ws.Cell(r, 1).Value = title;
            ws.Cell(r, 1).Style.Font.Bold = true;
            ws.Cell(r, 1).Style.Font.FontSize = 16;
            ws.Range(r, 1, r, dt.Columns.Count).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            r++;

            if (!string.IsNullOrWhiteSpace(subtitle))
            {
                ws.Cell(r, 1).Value = subtitle;
                ws.Range(r, 1, r, dt.Columns.Count).Merge()
                    .Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                r++;
            }

            ws.Cell(r, 1).InsertTable(dt, true);
            ws.Columns().AdjustToContents();

            if (!string.IsNullOrWhiteSpace(footer))
            {
                var lastRow = ws.LastRowUsed().RowNumber() + 2;
                ws.Cell(lastRow, 1).Value = footer;
                ws.Range(lastRow, 1, lastRow, dt.Columns.Count).Merge();
            }

            wb.SaveAs(path);
        }

        public static void ToPdf(DataTable dt, string path, string title,
                         string? subtitle = null, string? footer = null)
        {
            using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            var doc = new PdfDoc(PageSize.A4.Rotate(), 24, 24, 24, 24); // <-- dùng alias PdfDoc
            var writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();

            var fTitle = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
            var fNorm = FontFactory.GetFont(FontFactory.HELVETICA, 10);

            var pTitle = new Paragraph(title, fTitle) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 6 };
            doc.Add(pTitle);
            if (!string.IsNullOrWhiteSpace(subtitle))
                doc.Add(new Paragraph(subtitle, fNorm) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 10 });

            var table = new PdfPTable(dt.Columns.Count) { WidthPercentage = 100 };
            foreach (DataColumn c in dt.Columns)
            {
                var cell = new PdfPCell(new Phrase(c.ColumnName, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)))
                { BackgroundColor = new BaseColor(230, 230, 230) };
                table.AddCell(cell);
            }
            foreach (DataRow row in dt.Rows)
                foreach (var obj in row.ItemArray)
                    table.AddCell(new Phrase(obj?.ToString() ?? "", fNorm));

            doc.Add(table);

            if (!string.IsNullOrWhiteSpace(footer))
                doc.Add(new Paragraph("\n" + footer, fNorm));

            doc.Close();
            writer.Close();
        }

    }
}
