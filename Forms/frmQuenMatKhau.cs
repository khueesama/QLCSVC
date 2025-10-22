using System;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using QLCSVCWinApp.Services;
using QLCSVCWinApp.Utils;
using System.Text.RegularExpressions;



namespace QLCSVCWinApp.Forms
{
    public partial class frmQuenMatKhau : Form
    {
        private readonly AuthService _auth = new AuthService();
        private string? _currentOtp;
        private DateTime _otpSentAt;
        private const int OTP_VALID_MINUTES = 5;

        public frmQuenMatKhau()
        {
            InitializeComponent();
            // Ban đầu chỉ cho nhập email và gửi OTP
            txtOTP.Enabled = false;
            txtMatKhauMoi.Enabled = false;
            btnXacNhan.Enabled = false;
        }

        private void btnGuiOTP_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vui lòng nhập email.", "Thiếu thông tin",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra email có tồn tại
            if (!_auth.KiemTraEmail(email))
            {
                MessageBox.Show("Email không tồn tại trong hệ thống.",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Gửi OTP và lưu lại
                _currentOtp = EmailHelper.GuiOtp(email);
                _otpSentAt = DateTime.Now;

                MessageBox.Show("OTP đã được gửi đến email của bạn. Vui lòng kiểm tra hộp thư.",
                                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Cho phép nhập OTP và mật khẩu mới
                txtOTP.Enabled = true;
                txtMatKhauMoi.Enabled = true;
                btnXacNhan.Enabled = true;

                // Khóa lại nút gửi lại trong 60s nếu cần
                btnGuiOTP.Enabled = false;
                var t = new System.Windows.Forms.Timer { Interval = 60000 };
                t.Tick += (_, __) => { btnGuiOTP.Enabled = true; t.Stop(); t.Dispose(); };
                t.Start();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Gửi OTP thất bại: " + ex.Message,
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            string otp = txtOTP.Text.Trim();
            string pass = txtMatKhauMoi.Text;

            // 1) Kiểm tra OTP và thời gian
            if (string.IsNullOrEmpty(otp))
            {
                MessageBox.Show("Vui lòng nhập OTP.", "Thiếu thông tin",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (DateTime.Now > _otpSentAt.AddMinutes(OTP_VALID_MINUTES))
            {
                MessageBox.Show("OTP đã hết hạn. Vui lòng gửi lại.", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (otp != _currentOtp)
            {
                MessageBox.Show("OTP không đúng.", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2) Kiểm tra mật khẩu mới
            if (string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu mới.", "Thiếu thông tin",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (pass.Length < 6)
            {
                MessageBox.Show("Mật khẩu quá yếu (ít nhất 6 ký tự).", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3) Cập nhật mật khẩu (DAO sẽ Hash)
            try
            {
                _auth.CapNhatMatKhau(txtEmail.Text.Trim(), pass);
                MessageBox.Show("Đổi mật khẩu thành công! Vui lòng đăng nhập lại.",
                                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đổi mật khẩu thất bại: " + ex.Message,
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
