using System.Drawing;
using System.Windows.Forms;

namespace QLCSVCWinApp.Forms
{
    partial class frmDangKy
    {
        private Panel card;
        private Label lblTitle, lblSub;
        private TextBox txtTenDangNhap, txtEmail, txtMatKhau, txtXacNhanMK, txtOTP;
        private Button btnGuiOTP, btnXacNhanOTP;

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(246, 248, 252);
            this.ClientSize = new Size(520, 560);
            this.Font = new Font("Segoe UI", 10F);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Tạo tài khoản";

            card = new Panel
            {
                BackColor = Color.White,
                Size = new Size(420, 460),
                Location = new Point((this.ClientSize.Width - 420) / 2, (this.ClientSize.Height - 460) / 2),
                Anchor = AnchorStyles.None,
                Padding = new Padding(24)
            };
            card.SuspendLayout();

            lblTitle = new Label
            {
                Text = "Tạo tài khoản",
                Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(24, 20)
            };
            lblSub = new Label
            {
                Text = "Xác thực email bằng OTP để hoàn tất",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.DimGray,
                AutoSize = true,
                Location = new Point(24, 55)
            };

            txtTenDangNhap = new TextBox
            {
                Name = "txtTenDangNhap",
                PlaceholderText = "Tên đăng nhập (chữ/số)",
                Location = new Point(24, 100),
                Width = 360,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtEmail = new TextBox
            {
                Name = "txtEmail",
                PlaceholderText = "Email",
                Location = new Point(24, 145),
                Width = 280,
                BorderStyle = BorderStyle.FixedSingle
            };
            btnGuiOTP = new Button
            {
                Name = "btnGuiOTP",
                Text = "Gửi OTP",
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(314, 143),
                Size = new Size(70, 28)
            };
            btnGuiOTP.FlatAppearance.BorderSize = 0;
            btnGuiOTP.Click += new System.EventHandler(this.btnGuiOTP_Click);

            txtMatKhau = new TextBox
            {
                Name = "txtMatKhau",
                PlaceholderText = "Mật khẩu",
                UseSystemPasswordChar = true,
                Location = new Point(24, 190),
                Width = 360,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtXacNhanMK = new TextBox
            {
                Name = "txtXacNhanMK",
                PlaceholderText = "Xác nhận mật khẩu",
                UseSystemPasswordChar = true,
                Location = new Point(24, 235),
                Width = 360,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtOTP = new TextBox
            {
                Name = "txtOTP",
                PlaceholderText = "Nhập OTP",
                Location = new Point(24, 280),
                Width = 360,
                BorderStyle = BorderStyle.FixedSingle
            };

            btnXacNhanOTP = new Button
            {
                Name = "btnXacNhanOTP",
                Text = "Xác nhận & Đăng ký",
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(24, 330),
                Size = new Size(360, 36)
            };
            btnXacNhanOTP.FlatAppearance.BorderSize = 0;
            btnXacNhanOTP.Click += new System.EventHandler(this.btnXacNhanOTP_Click);

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblSub);
            card.Controls.Add(txtTenDangNhap);
            card.Controls.Add(txtEmail);
            card.Controls.Add(btnGuiOTP);
            card.Controls.Add(txtMatKhau);
            card.Controls.Add(txtXacNhanMK);
            card.Controls.Add(txtOTP);
            card.Controls.Add(btnXacNhanOTP);
            this.Controls.Add(card);

            this.AcceptButton = btnXacNhanOTP;

            card.ResumeLayout(false);
            card.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
