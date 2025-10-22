using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
// ref còn lại (OpenXml) không bắt buộc, có thể giữ hoặc bỏ
using QLCSVCWinApp.Models;
using QLCSVCWinApp.Services;

namespace QLCSVCWinApp.Forms
{
    public partial class frmLichSuThayDoi : Form
    {
        private readonly LichSuThayDoiService _svc = new LichSuThayDoiService();
        private readonly string[] _modules = { "(Tất cả)", "Thiết bị", "Phòng", "Sao lưu", "Phục hồi", "Khác" };
        private readonly string[] _actions = { "(Tất cả)", "Thêm", "Cập nhật", "Xoá", "Sao lưu", "Phục hồi", "Khác" };

        public frmLichSuThayDoi()
        {
            InitializeComponent();
        }

        private void frmLichSuThayDoi_Load(object sender, EventArgs e)
        {
            cbModule.Items.AddRange(_modules); cbModule.SelectedIndex = 0;
            cbHanhDong.Items.AddRange(_actions); cbHanhDong.SelectedIndex = 0;

            chkFrom.CheckedChanged += (_, __) => dtFrom.Enabled = chkFrom.Checked;
            chkTo.CheckedChanged += (_, __) => dtTo.Enabled = chkTo.Checked;

            // double-click để xem chi tiết
            dgvLS.CellDoubleClick += dgvLS_CellDoubleClick;

            LoadData();
        }

        private void LoadData()
        {
            var data = _svc.Search(null, null, null, null, null, null);
            dgvLS.DataSource = data;

            dgvLS.ClearSelection();
            if (dgvLS.Columns["MaLichSu"] != null) dgvLS.Columns["MaLichSu"].HeaderText = "Mã LS";
            if (dgvLS.Columns["Module"] != null) dgvLS.Columns["Module"].HeaderText = "Module";
            if (dgvLS.Columns["DoiTuong"] != null) dgvLS.Columns["DoiTuong"].HeaderText = "Đối tượng";
            if (dgvLS.Columns["HanhDong"] != null) dgvLS.Columns["HanhDong"].HeaderText = "Hành động";
            if (dgvLS.Columns["ThoiGian"] != null) dgvLS.Columns["ThoiGian"].HeaderText = "Thời gian";
            if (dgvLS.Columns["NguoiDung"] != null) dgvLS.Columns["NguoiDung"].HeaderText = "Người dùng";
            if (dgvLS.Columns["NoiDung"] != null) dgvLS.Columns["NoiDung"].HeaderText = "Nội dung";
            lblCount.Text = $"Bản ghi: {data.Count}";
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string? module = cbModule.SelectedIndex <= 0 ? null : cbModule.SelectedItem?.ToString();
            string? act = cbHanhDong.SelectedIndex <= 0 ? null : cbHanhDong.SelectedItem?.ToString();
            string? obj = string.IsNullOrWhiteSpace(txtDoiTuong.Text) ? null : txtDoiTuong.Text.Trim();
            string? user = string.IsNullOrWhiteSpace(txtNguoiDung.Text) ? null : txtNguoiDung.Text.Trim();
            DateTime? from = chkFrom.Checked ? dtFrom.Value : (DateTime?)null;
            DateTime? to = chkTo.Checked ? dtTo.Value : (DateTime?)null;

            var data = _svc.Search(module, obj, act, user, from, to);
            dgvLS.DataSource = data;
            dgvLS.ClearSelection();
            lblCount.Text = $"Bản ghi: {data.Count}";

            if (data.Count == 0)
                MessageBox.Show("Không có thay đổi nào được ghi nhận.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnLamMoi_Click(object sender, EventArgs e) => LoadData();

        // Double-click hiển thị chi tiết
        private void dgvLS_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvLS.Rows[e.RowIndex].DataBoundItem is not LichSuThayDoi x) return;

            MessageBox.Show(
                $"Mã LS: {x.MaLichSu}\nModule: {x.Module}\nĐối tượng: {x.DoiTuong}\n" +
                $"Hành động: {x.HanhDong}\nThời gian: {x.ThoiGian:yyyy-MM-dd HH:mm:ss}\n" +
                $"Người dùng: {x.NguoiDung}\n\n{x.NoiDung}",
                "Chi tiết", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            if (dgvLS.DataSource is not List<LichSuThayDoi> data || data.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất."); return;
            }
            using var sfd = new SaveFileDialog
            {
                Filter = "CSV file|*.csv",
                FileName = $"lich_su_thay_doi_{DateTime.Now:yyyyMMddHHmm}.csv"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            var sb = new StringBuilder();
            sb.AppendLine("MaLichSu,Module,DoiTuong,HanhDong,ThoiGian,NguoiDung,NoiDung");
            foreach (var x in data)
                sb.AppendLine($"{x.MaLichSu},\"{x.Module}\",\"{x.DoiTuong}\",\"{x.HanhDong}\"," +
                              $"{x.ThoiGian:yyyy-MM-dd HH:mm:ss},\"{x.NguoiDung}\",\"{x.NoiDung.Replace("\"", "\"\"")}\"");
            File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
            MessageBox.Show("Đã xuất CSV.");
        }
    }
}
