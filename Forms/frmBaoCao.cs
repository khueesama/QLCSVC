// ======= Forms/frmBaoCao.cs (FINAL FIXED VERSION) =======
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using QLCSVCWinApp.DataAccess;
using QLCSVCWinApp.Services;

namespace QLCSVCWinApp.Forms
{
    public partial class frmBaoCao : Form
    {
        // Hiển thị tên phòng trong CheckedListBox
        private sealed class RoomItem
        {
            public string Code { get; }
            public string Name { get; }
            public RoomItem(string code, string name) { Code = code; Name = name; }
            public override string ToString() => Name;
        }

        private readonly List<RoomItem> _rooms = new();
        private readonly LichSuThayDoiDAO _lsDao = new();

        public frmBaoCao()
        {
            InitializeComponent();
        }

        private void frmBaoCao_Load(object sender, EventArgs e)
        {
            cbLoaiBaoCao.Items.Clear();
            cbLoaiBaoCao.Items.AddRange(new object[] {
                "Thiết bị theo phòng",
                "Tổng hợp theo thiết bị",
                "Lịch sử thay đổi"
            });
            cbLoaiBaoCao.SelectedIndex = 0;

            cbDinhDang.Items.Clear();
            cbDinhDang.Items.AddRange(new object[] { "Excel (.xlsx)", "CSV (.csv)", "PDF (.pdf)" });
            cbDinhDang.SelectedIndex = 0;

            cbLocLoai.Items.Clear();
            cbLocLoai.Items.AddRange(new object[] { "(Tất cả)", "Quạt", "Máy lạnh", "Khác" });
            cbLocTinhTrang.Items.Clear();
            cbLocTinhTrang.Items.AddRange(new object[] { "(Tất cả)", "Đang sử dụng", "Đang sửa", "Hỏng", "Thanh lý" });
            cbLocLoai.SelectedIndex = 0;
            cbLocTinhTrang.SelectedIndex = 0;

            cbModule.Items.Clear();
            cbModule.Items.AddRange(new object[] { "(Tất cả)", "Thiết bị", "Phòng", "Sao lưu/Phục hồi" });
            cbHanhDong.Items.Clear();
            cbHanhDong.Items.AddRange(new object[] { "(Tất cả)", "Thêm", "Cập nhật", "Xoá", "Sao lưu", "Khôi phục" });
            cbModule.SelectedIndex = 0;
            cbHanhDong.SelectedIndex = 0;

            chkFrom.CheckedChanged += (_, __) => dtFrom.Enabled = chkFrom.Checked;
            chkTo.CheckedChanged += (_, __) => dtTo.Enabled = chkTo.Checked;

            LoadRoomsFromDb();
            BindRoomsToList();

            dgvPreview.ReadOnly = true;
            dgvPreview.AllowUserToAddRows = false;
            dgvPreview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPreview.RowHeadersVisible = false;
            dgvPreview.BackgroundColor = System.Drawing.Color.White;
            dgvPreview.BorderStyle = BorderStyle.None;

            CollapseHistoryRow(false);
        }

        private void CollapseHistoryRow(bool show)
        {
            if (layout.RowCount < 2) return;
            layout.RowStyles[1].Height = show ? 56f : 0f;
            pnlDoiTuongLS.Visible = show;
            pnlFilterTB.Visible = !show;
            layout.PerformLayout();
            layout.Refresh();
        }

        private void LoadRoomsFromDb()
        {
            _rooms.Clear();
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                var candidates = new[] { "[dbo].[PhongHoc]", "[dbo].[tblPhong]", "[dbo].[Phong]", "[Phong]", "[tblPhong]" };
                string? foundTable = null;

                foreach (var tbl in candidates)
                {
                    using var cmdExists = new SqlCommand(
                        $"SELECT CASE WHEN OBJECT_ID('{tbl}','U') IS NULL THEN 0 ELSE 1 END", conn);
                    var exists = Convert.ToInt32(cmdExists.ExecuteScalar());
                    if (exists == 1) { foundTable = tbl; break; }
                }

                if (foundTable == null)
                {
                    using var cmdFind = new SqlCommand(@"
                        SELECT TOP 1 QUOTENAME(s.name)+'.'+QUOTENAME(t.name)
                        FROM sys.tables t
                        JOIN sys.schemas s ON s.schema_id = t.schema_id
                        WHERE t.name LIKE '%Phong%' ORDER BY t.name", conn);
                    foundTable = cmdFind.ExecuteScalar() as string;
                }

                if (string.IsNullOrEmpty(foundTable))
                    throw new InvalidOperationException("Không tìm được bảng chứa phòng trong CSDL.");

                var schemaName = "dbo";
                var full = foundTable.Replace("[", "").Replace("]", "");
                var parts = full.Split('.');
                var tableName = (parts.Length == 2) ? parts[1] : full;
                if (parts.Length == 2) schemaName = parts[0];

                var cols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                using (var cmdCols = new SqlCommand(@"
                    SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_SCHEMA=@s AND TABLE_NAME=@t", conn))
                {
                    cmdCols.Parameters.AddWithValue("@s", schemaName);
                    cmdCols.Parameters.AddWithValue("@t", tableName);
                    using var rd = cmdCols.ExecuteReader();
                    while (rd.Read()) cols.Add(rd.GetString(0));
                }

                string? colMa = new[] { "MaPhong", "Ma_Phong", "PhongID", "IDPhong", "Id" }
                                 .FirstOrDefault(cols.Contains);
                string? colTen = new[] { "TenPhong", "Ten_Phong", "Ten", "Name", "PhongName" }
                                 .FirstOrDefault(cols.Contains);

                if (colMa == null || colTen == null)
                    throw new InvalidOperationException($"Không tìm được cột mã/tên phòng trong {foundTable}.");

                using (var cmd = new SqlCommand($@"
                    SELECT {colMa} AS MaPhong, {colTen} AS TenPhong
                    FROM {foundTable} ORDER BY {colTen}", conn))
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var code = rd["MaPhong"]?.ToString() ?? "";
                        var name = rd["TenPhong"]?.ToString() ?? code;
                        if (!string.IsNullOrWhiteSpace(code))
                            _rooms.Add(new RoomItem(code, name));
                    }
                }
            }
        }

        private void BindRoomsToList()
        {
            clbPhong.Items.Clear();
            foreach (var r in _rooms) clbPhong.Items.Add(r, false);
        }

        private void cbLoaiBaoCao_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isLS = cbLoaiBaoCao.SelectedIndex == 2;
            CollapseHistoryRow(isLS);
        }

        private void btnXem_Click(object sender, EventArgs e)
        {
            if (cbLoaiBaoCao.SelectedIndex == 2)
            {
                LoadPreview_LichSu();
                return;
            }
            Preview_ThietBi();
        }

        // ----------- Lịch sử thay đổi -----------
        private void LoadPreview_LichSu()
        {
            string? module = cbModule?.SelectedItem?.ToString();
            string? hanhDong = cbHanhDong?.SelectedItem?.ToString();
            string? doiTuong = string.IsNullOrWhiteSpace(txtDoiTuong?.Text) ? null : txtDoiTuong.Text.Trim();
            string? nguoiDung = string.IsNullOrWhiteSpace(txtNguoiDung?.Text) ? null : txtNguoiDung.Text.Trim();

            if (string.Equals(module, "(Tất cả)", StringComparison.OrdinalIgnoreCase)) module = null;
            if (string.Equals(hanhDong, "(Tất cả)", StringComparison.OrdinalIgnoreCase)) hanhDong = null;

            DateTime? from = chkFrom.Checked ? dtFrom.Value.Date : (DateTime?)null;
            DateTime? to = chkTo.Checked ? dtTo.Value.Date : (DateTime?)null;

            var list = _lsDao.Search(module, doiTuong, hanhDong, nguoiDung, from, to);
            var tbl = new DataTable();

            tbl.Columns.Add("MaLichSu");
            tbl.Columns.Add("Module");
            tbl.Columns.Add("DoiTuong");
            tbl.Columns.Add("HanhDong");
            tbl.Columns.Add("ThoiGian", typeof(DateTime));
            tbl.Columns.Add("NguoiDung");
            tbl.Columns.Add("NoiDung");

            foreach (var x in list)
            {
                tbl.Rows.Add(x.MaLichSu, x.Module, x.DoiTuong ?? "",
                             x.HanhDong, x.ThoiGian, x.NguoiDung, x.NoiDung);
            }

            // 🌐 Đổi tên cột sang tiếng Việt
            tbl.Columns["MaLichSu"].ColumnName = "Mã lịch sử";
            tbl.Columns["Module"].ColumnName = "Module";
            tbl.Columns["DoiTuong"].ColumnName = "Đối tượng";
            tbl.Columns["HanhDong"].ColumnName = "Hành động";
            tbl.Columns["ThoiGian"].ColumnName = "Thời gian";
            tbl.Columns["NguoiDung"].ColumnName = "Người dùng";
            tbl.Columns["NoiDung"].ColumnName = "Nội dung";

            dgvPreview.DataSource = tbl;
            lblCount.Text = $"Số dòng: {tbl.Rows.Count}";
        }

        // ----------- Thiết bị theo phòng -----------
        private void Preview_ThietBi()
        {
            var selectedRoomCodes = clbPhong.CheckedItems.Cast<RoomItem>().Select(r => r.Code).ToList();
            string kw = (txtKw.Text ?? "").Trim();
            string loai = cbLocLoai.SelectedItem?.ToString() ?? "(Tất cả)";
            string tinhTrang = cbLocTinhTrang.SelectedItem?.ToString() ?? "(Tất cả)";
            bool includeNoRoom = chkChuaCoPhong.Checked;
            DateTime? from = chkFrom.Checked ? dtFrom.Value.Date : (DateTime?)null;
            DateTime? to = chkTo.Checked ? dtTo.Value.Date : (DateTime?)null;

            var sql = new StringBuilder(@"
SELECT tb.maThietBi AS MaThietBi,
       tb.tenThietBi AS TenThietBi,
       tb.tinhTrang AS TinhTrang,
       tb.maPhong AS MaPhong,
       COALESCE(p.tenPhong, N'(chưa có phòng)') AS TenPhong,
       tb.ngayMua AS NgayMua
FROM ThietBi tb
LEFT JOIN PhongHoc p ON p.maPhong = tb.maPhong
WHERE 1=1
");
            var prms = new List<SqlParameter>();

            if (selectedRoomCodes.Count > 0)
            {
                var inNames = new List<string>();
                for (int i = 0; i < selectedRoomCodes.Count; i++)
                {
                    string pName = "@room" + i;
                    inNames.Add(pName);
                    prms.Add(new SqlParameter(pName, selectedRoomCodes[i]));
                }
                sql.Append($" AND (tb.maPhong IN ({string.Join(",", inNames)})");
                if (includeNoRoom) sql.Append(" OR tb.maPhong IS NULL");
                sql.Append(")");
            }
            else
            {
                if (!includeNoRoom) sql.Append(" AND tb.maPhong IS NOT NULL");
            }

            if (!string.IsNullOrEmpty(kw))
            {
                sql.Append(" AND (tb.maThietBi LIKE @kw OR tb.tenThietBi LIKE @kw)");
                prms.Add(new SqlParameter("@kw", "%" + kw + "%"));
            }

            if (!string.Equals(loai, "(Tất cả)", StringComparison.OrdinalIgnoreCase))
            {
                sql.Append(" AND tb.loaiThietBi = @loai");
                prms.Add(new SqlParameter("@loai", loai));
            }

            if (!string.Equals(tinhTrang, "(Tất cả)", StringComparison.OrdinalIgnoreCase))
            {
                sql.Append(" AND tb.tinhTrang = @tt");
                prms.Add(new SqlParameter("@tt", tinhTrang));
            }

            if (from.HasValue)
            {
                sql.Append(" AND CONVERT(date, tb.ngayMua) >= @from");
                prms.Add(new SqlParameter("@from", from.Value));
            }
            if (to.HasValue)
            {
                sql.Append(" AND CONVERT(date, tb.ngayMua) <= @to");
                prms.Add(new SqlParameter("@to", to.Value));
            }

            sql.Append(" ORDER BY tb.maPhong, tb.maThietBi");

            var tbl = new DataTable();
            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand(sql.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                if (prms.Count > 0) cmd.Parameters.AddRange(prms.ToArray());
                conn.Open();
                da.Fill(tbl);
            }

            // 🌐 Đổi tên cột sang tiếng Việt
            tbl.Columns["MaThietBi"].ColumnName = "Mã thiết bị";
            tbl.Columns["TenThietBi"].ColumnName = "Tên thiết bị";
            tbl.Columns["TinhTrang"].ColumnName = "Tình trạng";
            tbl.Columns["MaPhong"].ColumnName = "Mã phòng";
            tbl.Columns["TenPhong"].ColumnName = "Tên phòng";
            tbl.Columns["NgayMua"].ColumnName = "Ngày mua";

            dgvPreview.DataSource = tbl;
            lblCount.Text = $"Số dòng: {tbl.Rows.Count}";
        }

        // ----------- Xuất báo cáo -----------
        private void btnXuat_Click(object sender, EventArgs e)
        {
            if (dgvPreview.DataSource is not DataTable tbl || tbl.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất.", "Xuất báo cáo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string mainTitle = cbLoaiBaoCao.SelectedIndex switch
            {
                0 => "BÁO CÁO THIẾT BỊ THEO PHÒNG",
                1 => "BÁO CÁO TỔNG HỢP THEO THIẾT BỊ",
                2 => "BÁO CÁO LỊCH SỬ THAY ĐỔI",
                _ => "BÁO CÁO"
            };

            string subTitle = string.IsNullOrWhiteSpace(txtTieuDePhu.Text)
                ? "Trung tâm Ngoại ngữ – Tin học, Trường ĐH Kỹ thuật – Công nghệ Cần Thơ"
                : txtTieuDePhu.Text.Trim();

            string fmt = cbDinhDang.SelectedItem?.ToString() ?? "";

            using var sfd = new SaveFileDialog
            {
                Filter = fmt.Contains("CSV") ? "CSV (*.csv)|*.csv"
                       : fmt.Contains("PDF") ? "PDF (*.pdf)|*.pdf"
                       : "Excel (*.xlsx)|*.xlsx",
                FileName = "baocao"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                if (fmt.Contains("CSV"))
                    ReportExportService.ExportCsv(tbl, sfd.FileName);
                else if (fmt.Contains("PDF"))
                    ReportExportService.ExportPdf(tbl, sfd.FileName, mainTitle, subTitle);
                else
                    ReportExportService.ExportExcel(tbl, sfd.FileName, mainTitle, subTitle);

                MessageBox.Show("Xuất báo cáo thành công!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Xuất thất bại: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
