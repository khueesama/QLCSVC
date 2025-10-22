using System;
using System.Data;
using System.IO;
using System.Text;

// ClosedXML (Excel)
using ClosedXML.Excel;

// MigraDoc + PdfSharp (PDF)
using PdfSharp.Pdf;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;

namespace QLCSVCWinApp.Services
{
    public static class ReportExportService
    {
        // ===== CSV (UTF-8 BOM) =====
        public static void ExportCsv(DataTable table, string path)
        {
            using var sw = new StreamWriter(path, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

            // Header
            for (int c = 0; c < table.Columns.Count; c++)
            {
                if (c > 0) sw.Write(",");
                sw.Write(EscapeCsv(table.Columns[c].ColumnName));
            }
            sw.WriteLine();

            // Rows
            foreach (DataRow row in table.Rows)
            {
                for (int c = 0; c < table.Columns.Count; c++)
                {
                    if (c > 0) sw.Write(",");
                    sw.Write(EscapeCsv(row[c]?.ToString() ?? ""));
                }
                sw.WriteLine();
            }
        }

        private static string EscapeCsv(string input)
        {
            if (input.Contains(",") || input.Contains("\"") || input.Contains("\r") || input.Contains("\n"))
                return "\"" + input.Replace("\"", "\"\"") + "\"";
            return input;
        }

        // ===== Excel (.xlsx) =====
        public static void ExportExcel(DataTable table, string path,
            string mainTitle = "BÁO CÁO",
            string subTitle = "Trung tâm Ngoại ngữ – Tin học, Trường ĐH Kỹ thuật – Công nghệ Cần Thơ")
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Báo cáo");

            int cols = table.Columns.Count;

            // --- Title
            ws.Cell(1, 1).Value = mainTitle.ToUpper();
            ws.Range(1, 1, 1, cols).Merge();
            var titleCell = ws.Cell(1, 1);
            titleCell.Style.Font.Bold = true;
            titleCell.Style.Font.FontSize = 16;
            titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            titleCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // --- Subtitle
            if (!string.IsNullOrWhiteSpace(subTitle))
            {
                ws.Cell(2, 1).Value = subTitle;
                ws.Range(2, 1, 2, cols).Merge();
                var sub = ws.Cell(2, 1);
                sub.Style.Font.Italic = true;
                sub.Style.Font.FontSize = 11;
                sub.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // Header
            for (int c = 0; c < cols; c++)
            {
                ws.Cell(4, c + 1).Value = table.Columns[c].ColumnName;
                var hc = ws.Cell(4, c + 1);
                hc.Style.Font.Bold = true;
                hc.Style.Fill.BackgroundColor = XLColor.FromHtml("#F2F2F2");
                hc.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                hc.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                hc.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }

            // Body
            for (int r = 0; r < table.Rows.Count; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    var cell = ws.Cell(r + 5, c + 1);
                    var v = table.Rows[r][c];

                    if (v == DBNull.Value || v is null)
                        cell.Value = string.Empty;
                    else if (v is DateTime dt)
                    {
                        cell.Value = dt;
                        cell.Style.DateFormat.Format = "yyyy-MM-dd";
                    }
                    else if (v is bool b)
                        cell.Value = b;
                    else if (v is byte || v is sbyte || v is short || v is ushort ||
                             v is int || v is uint || v is long || v is ulong ||
                             v is float || v is double || v is decimal)
                        cell.Value = Convert.ToDouble(v);
                    else
                        cell.Value = v.ToString();

                    cell.Style.Alignment.WrapText = true;
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }
            }

            ws.Columns().AdjustToContents(5.0, 60.0);
            ws.Rows().AdjustToContents();
            ws.SheetView.FreezeRows(4);
            wb.SaveAs(path);
        }

        // ===== PDF (MigraDoc) =====
        public static void ExportPdf(DataTable table, string path,
            string mainTitle = "BÁO CÁO",
            string subTitle = "Trung tâm Ngoại ngữ – Tin học, Trường ĐH Kỹ thuật – Công nghệ Cần Thơ")
        {
            var doc = new Document();
            var section = doc.AddSection();
            section.PageSetup.LeftMargin = Unit.FromCentimeter(2);
            section.PageSetup.RightMargin = Unit.FromCentimeter(2);
            section.PageSetup.TopMargin = Unit.FromCentimeter(2);
            section.PageSetup.BottomMargin = Unit.FromCentimeter(2);

            // --- Title
            var h = section.AddParagraph(mainTitle.ToUpper());
            h.Format.Font.Name = "Arial";
            h.Format.Font.Size = 16;
            h.Format.Font.Bold = true;
            h.Format.Alignment = ParagraphAlignment.Center;
            h.Format.SpaceAfter = Unit.FromPoint(4);

            // --- Subtitle
            if (!string.IsNullOrWhiteSpace(subTitle))
            {
                var sub = section.AddParagraph(subTitle);
                sub.Format.Font.Name = "Arial";
                sub.Format.Font.Size = 11;
                sub.Format.Font.Italic = true;
                sub.Format.Alignment = ParagraphAlignment.Center;
                sub.Format.SpaceAfter = Unit.FromPoint(12);
            }

            // --- Table
            var tbl = section.AddTable();
            tbl.Format.Font.Name = "Arial";
            tbl.Format.Font.Size = 10;
            tbl.Borders.Width = 0.5;

            int colCount = table.Columns.Count;
            double[] widths = CalcColumnWidths(colCount);

            for (int i = 0; i < colCount; i++)
            {
                var col = tbl.AddColumn(Unit.FromCentimeter(widths[i]));
                col.Format.Alignment = ParagraphAlignment.Left;
            }

            // Header row
            var header = tbl.AddRow();
            header.HeadingFormat = true;
            header.Format.Font.Bold = true;
            header.Shading.Color = Colors.WhiteSmoke;
            header.VerticalAlignment = VerticalAlignment.Center;

            for (int c = 0; c < colCount; c++)
                header.Cells[c].AddParagraph(table.Columns[c].ColumnName);

            // Body
            foreach (DataRow dr in table.Rows)
            {
                var row = tbl.AddRow();
                row.TopPadding = Unit.FromPoint(2);
                row.BottomPadding = Unit.FromPoint(2);
                row.VerticalAlignment = VerticalAlignment.Top;

                for (int c = 0; c < colCount; c++)
                    row.Cells[c].AddParagraph(dr[c]?.ToString() ?? "");
            }

            // Render
            var renderer = new PdfDocumentRenderer(true);
            renderer.Document = doc;
            renderer.RenderDocument();
            renderer.Save(path);
        }

        private static double[] CalcColumnWidths(int colCount)
        {
            switch (colCount)
            {
                case 5: return new[] { 3.0, 5.0, 3.0, 2.5, 3.5 };
                case 6: return new[] { 3.0, 5.0, 3.0, 2.5, 3.0, 2.5 };
                case 7: return new[] { 3.0, 5.0, 3.0, 2.0, 2.5, 2.5, 2.0 };
                case 8: return new[] { 2.5, 4.5, 2.5, 2.0, 2.0, 2.0, 1.8, 1.7 };
                default:
                    var arr = new double[colCount];
                    double each = 17.0 / colCount;
                    for (int i = 0; i < colCount; i++) arr[i] = each;
                    return arr;
            }
        }
    }
}
