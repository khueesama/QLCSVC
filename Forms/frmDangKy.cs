using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using QLCSVCWinApp.Services;
using QLCSVCWinApp.Utils;

namespace QLCSVCWinApp.Forms
{
    public partial class frmDangKy : Form
    {
        private readonly AuthService _auth = new AuthService();
        private string _currentOtp = "";
        private DateTime _otpSentAt;
        private const int OTP_VALID_MINUTES = 5;

        public frmDangKy()
        {
            InitializeComponent();
            // Ban đầu chỉ cho nhập thông tin cơ bản
            txtOTP.Enabled = false;
            btnXacNhanOTP.Enabled = false;
        }

        private static bool IsStrongPassword(string s)
        {
            // Ví dụ đơn giản: >= 6 ký tự. (Bạn có thể nâng cấp: có chữ hoa/thường/số/ký tự đặc biệt)
            return !string.IsNullOrWhiteSpace(s) && s.Length >= 6;
        }

        private void btnGuiOTP_Click(object sender, EventArgs e)
        {
            // 1) Kiểm tra nhập liệu cơ bản
            if (string.IsNullOrEmpty(txtTenDangNhap.Text) ||
                string.IsNullOrEmpty(txtEmail.Text) ||
                string.IsNullOrEmpty(txtMatKhau.Text) ||
                string.IsNullOrEmpty(txtXacNhanMK.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tên đăng nhập, Email và Mật khẩu.",
                                "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2) Tên đăng nhập chỉ letters&digits
            if (!Regex.IsMatch(txtTenDangNhap.Text, @"^[a-zA-Z0-9]+$"))
            {
                MessageBox.Show("Tên đăng nhập chỉ được chứa chữ cái và số.",
                                "Tên đăng nhập không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3) Định dạng email
            if (!Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Email không hợp lệ. Ví dụ: user@example.com",
                                "Email không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4) Mật khẩu xác nhận và độ mạnh
            if (txtMatKhau.Text != txtXacNhanMK.Text)
            {
                MessageBox.Show("Mật khẩu và xác nhận không khớp.",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!IsStrongPassword(txtMatKhau.Text))
            {
                MessageBox.Show("Mật khẩu quá yếu (ít nhất 6 ký tự).",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 5) Username / Email đã tồn tại?
            if (_auth.KiemTraTenDangNhap(txtTenDangNhap.Text.Trim()))
            {
                MessageBox.Show("Tên đăng nhập đã tồn tại.",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (_auth.KiemTraEmail(txtEmail.Text.Trim()))
            {
                MessageBox.Show("Email này đã được đăng ký.",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 6) Gửi OTP
            try
            {
                _currentOtp = EmailHelper.GuiOtp(txtEmail.Text.Trim());
                _otpSentAt = DateTime.Now;
                MessageBox.Show("OTP đã được gửi đến email của bạn.",
                                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Cho phép nhập OTP và bật nút Xác nhận
                txtOTP.Enabled = true;
                btnXacNhanOTP.Enabled = true;

                // Khoá nút Gửi OTP 60s
                btnGuiOTP.Enabled = false;
                var t = new System.Windows.Forms.Timer { Interval = 60000 };
                t.Tick += (_, __) => { btnGuiOTP.Enabled = true; t.Stop(); t.Dispose(); };
                t.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gửi OTP thất bại: " + ex.Message,
                                "Lỗi gửi email", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXacNhanOTP_Click(object sender, EventArgs e)
        {
            // 1) Kiểm tra OTP nhập
            if (string.IsNullOrEmpty(txtOTP.Text))
            {
                MessageBox.Show("Vui lòng nhập OTP.",
                                "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (DateTime.Now > _otpSentAt.AddMinutes(OTP_VALID_MINUTES))
            {
                MessageBox.Show("OTP đã hết hạn. Vui lòng gửi lại.",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txtOTP.Text.Trim() != _currentOtp)
            {
                MessageBox.Show("OTP không đúng.",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2) Thực hiện đăng ký (DAO sẽ Hash mật khẩu)
            bool ok = _auth.DangKy(
                txtTenDangNhap.Text.Trim(),
                txtEmail.Text.Trim(),
                txtMatKhau.Text
            );

            if (ok)
            {
                MessageBox.Show("Đăng ký thành công! Vui lòng đăng nhập.",
                                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            else
            {
                MessageBox.Show("Đăng ký thất bại. Vui lòng thử lại.",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
