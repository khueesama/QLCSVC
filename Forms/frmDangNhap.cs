using System;
using System.Windows.Forms;
using QLCSVCWinApp.Services;
using QLCSVCWinApp.Utils;   // <-- THÊM DÒNG NÀY

namespace QLCSVCWinApp.Forms
{
    public partial class frmDangNhap : Form
    {
        private readonly AuthService _auth = new AuthService();

        public frmDangNhap()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void ResetInputs()
        {
            txtTaiKhoan.Clear();
            txtMatKhau.Clear();
            txtTaiKhoan.Focus();
        }

        private void frmDangNhap_Load(object sender, EventArgs e)
        {
            try
            {
                if (!DataAccess.DbHelper.TestConnection())
                {
                    MessageBox.Show("Không thể kết nối tới CSDL.", "Kết nối",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message, "Kết nối",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string user = txtTaiKhoan.Text.Trim();
            string pass = txtMatKhau.Text.Trim();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thiếu thông tin",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_auth.CheckLogin(user, pass))
            {
                MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu.", "Đăng nhập thất bại",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // >>> SET NGƯỜI DÙNG HIỆN TẠI <<<
            Session.CurrentUser = user;     // ví dụ: "khue"

            Hide();
            using (var main = new frmMain())
            {
                main.ShowDialog();
                if (main.IsExitApp)
                {
                    // (tuỳ chọn) reset phiên khi thoát hẳn app
                    Session.CurrentUser = null;
                    Close();
                    return;
                }
            }

            // Quay lại màn hình đăng nhập (đăng xuất)
            if (!IsDisposed)
            {
                Show();
                ResetInputs();
                // (tuỳ chọn) reset phiên khi đăng xuất
                Session.CurrentUser = null;
            }
        }

        private void btnDangKy_Click(object sender, EventArgs e)
        {
            Hide();
            using (var frm = new frmDangKy())
            {
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog(this);
            }
            if (!IsDisposed) Show();
        }

        private void btnQuenMK_Click(object sender, EventArgs e)
        {
            Hide();
            using (var frm = new frmQuenMatKhau())
                frm.ShowDialog();
            if (!IsDisposed)
            {
                Show();
                ResetInputs();
            }
        }
    }
}
