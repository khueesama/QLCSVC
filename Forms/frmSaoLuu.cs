using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Wordprocessing;
using QLCSVCWinApp.Services;
using QLCSVCWinApp.Utils;

namespace QLCSVCWinApp.Forms
{
    public partial class frmSaoLuu : Form
    {
        private readonly SaoLuuService _svc = new SaoLuuService();

        public frmSaoLuu()
        {
            InitializeComponent();
            // chống nháy khi vẽ grid
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.SetProperty,
                null, dgv, new object[] { true });
        }

        private void frmSaoLuu_Load(object sender, EventArgs e)
        {
            // gợi ý tên file .bak
            txtPath.Text = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                $"backup_{DateTime.Now:yyyyMMdd_HHmm}.bak");

            RefreshGrid();
        }

        private void RefreshGrid()
        {
            dgv.DataSource = _svc.GetHistory();
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.RowHeadersVisible = false;

            // ==== ĐỔI TÊN CỘT & ĐẾM BẢN GHI  ====
            if (dgv.Columns["MaSaoLuu"] != null) dgv.Columns["MaSaoLuu"].HeaderText = "Mã SL";
            if (dgv.Columns["ThoiGianSaoLuu"] != null) dgv.Columns["ThoiGianSaoLuu"].HeaderText = "Thời gian";
            if (dgv.Columns["DuongDanFile"] != null) dgv.Columns["DuongDanFile"].HeaderText = "Đường dẫn";
            if (dgv.Columns["MaNguoiDung"] != null) dgv.Columns["MaNguoiDung"].HeaderText = "Người dùng";

            lblCount.Text = $"{dgv.Rows.Count} bản ghi";
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog
            {
                Filter = "SQL Server backup|*.bak",
                FileName = $"backup_{DateTime.Now:yyyyMMdd_HHmm}.bak"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
                txtPath.Text = sfd.FileName;
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            var fileNameOnly = Path.GetFileName(txtPath.Text.Trim());
            if (string.IsNullOrWhiteSpace(fileNameOnly))
            {
                MessageBox.Show("Vui lòng nhập tên file .bak.");
                return;
            }

            try
            {
                // 1) Sao lưu – trả về đường dẫn thật trên máy SQL
                var serverPath = _svc.BackupTo(fileNameOnly);

                // 2) Ghi bảng saoluu (nếu có user)
                var userId = CurrentUser.MaNguoiDung;
                if (!string.IsNullOrWhiteSpace(userId))
                    _svc.LogBackup(serverPath, userId);

                // 3) Log lịch sử tổng
                LichSuThayDoiLogger.Log(
                    module: "Sao lưu/Phục hồi",
                    doiTuong: Path.GetFileName(serverPath),
                    hanhDong: "Sao lưu",
                    noiDung: $"Tệp: {serverPath}; Người thực hiện: {userId}"
                );

                MessageBox.Show($"Sao lưu thành công!\n(File trên máy SQL):\n{serverPath}",
                                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sao lưu thất bại: " + ex.Message,
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e) => RefreshGrid();
    }
}
