using System;
using System.IO;
using System.Windows.Forms;
using QLCSVCWinApp.Services;
using QLCSVCWinApp.Utils;

namespace QLCSVCWinApp.Forms
{
    public partial class frmPhucHoi : Form
    {
        private readonly SaoLuuService _svc = new SaoLuuService();

        public frmPhucHoi()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "SQL Server backup (*.bak)|*.bak",
                Multiselect = false
            };
            if (ofd.ShowDialog() == DialogResult.OK)
                txtFile.Text = ofd.FileName;
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            var file = txtFile.Text.Trim();
            if (string.IsNullOrWhiteSpace(file) || !File.Exists(file))
            {
                MessageBox.Show("Hãy chọn file .bak hợp lệ.");
                return;
            }

            if (MessageBox.Show("Phục hồi sẽ ghi đè toàn bộ dữ liệu hiện tại. Tiếp tục?",
                                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                _svc.RestoreFrom(file);

                var userId = CurrentUser.MaNguoiDung;
                LichSuThayDoiLogger.Log(
                    module: "Sao lưu/Phục hồi",
                    doiTuong: Path.GetFileName(file),
                    hanhDong: "Khôi phục",
                    noiDung: $"Tệp: {file}; Người thực hiện: {userId}"
                );

                MessageBox.Show("Phục hồi thành công.", "Thành công",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // thông báo lỗi + log lại để tra cứu
                MessageBox.Show("Phục hồi thất bại: " + ex.Message, "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);

                try
                {
                    LichSuThayDoiLogger.Log(
                        module: "Sao lưu/Phục hồi",
                        doiTuong: Path.GetFileName(file),
                        hanhDong: "Khôi phục lỗi",
                        noiDung: $"Tệp: {file}; Lỗi: {ex.Message}"
                    );
                }
                catch { /* tránh lỗi lồng khi DB đang hỏng */ }
            }
        }

    }
}
