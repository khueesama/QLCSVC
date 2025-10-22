using System.Drawing;
using System.Windows.Forms;

namespace QLCSVCWinApp.Forms
{
    partial class frmQuenMatKhau
    {
        private Panel card;
        private Label lblTitle, lblSub;
        private TextBox txtEmail, txtOTP, txtMatKhauMoi;
        private Button btnGuiOTP, btnXacNhan;

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(246, 248, 252);
            this.ClientSize = new Size(520, 480);
            this.Font = new Font("Segoe UI", 10F);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Quên mật khẩu";

            card = new Panel
            {
                BackColor = Color.White,
                Size = new Size(420, 380),
                Location = new Point((this.ClientSize.Width - 420) / 2, (this.ClientSize.Height - 380) / 2),
                Anchor = AnchorStyles.None,
                Padding = new Padding(24)
            };
            card.SuspendLayout();

            lblTitle = new Label
            {
                Text = "Khôi phục mật khẩu",
                Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(24, 20)
            };
            lblSub = new Label
            {
                Text = "Nhập email để nhận mã OTP và đặt lại mật khẩu",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.DimGray,
                AutoSize = true,
                Location = new Point(24, 55)
            };

            txtEmail = new TextBox
            {
                Name = "txtEmail",
                PlaceholderText = "Email đăng ký",
                Location = new Point(24, 100),
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
                Location = new Point(314, 98),
                Size = new Size(70, 28)
            };
            btnGuiOTP.FlatAppearance.BorderSize = 0;
            btnGuiOTP.Click += new System.EventHandler(this.btnGuiOTP_Click);

            txtOTP = new TextBox
            {
                Name = "txtOTP",
                PlaceholderText = "Nhập OTP",
                Location = new Point(24, 145),
                Width = 360,
                BorderStyle = BorderStyle.FixedSingle
            };

            txtMatKhauMoi = new TextBox
            {
                Name = "txtMatKhauMoi",
                PlaceholderText = "Mật khẩu mới",
                UseSystemPasswordChar = true,
                Location = new Point(24, 190),
                Width = 360,
                BorderStyle = BorderStyle.FixedSingle
            };

            btnXacNhan = new Button
            {
                Name = "btnXacNhan",
                Text = "Xác nhận",
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(24, 240),
                Size = new Size(360, 36)
            };
            btnXacNhan.FlatAppearance.BorderSize = 0;
            btnXacNhan.Click += new System.EventHandler(this.btnXacNhan_Click);

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblSub);
            card.Controls.Add(txtEmail);
            card.Controls.Add(btnGuiOTP);
            card.Controls.Add(txtOTP);
            card.Controls.Add(txtMatKhauMoi);
            card.Controls.Add(btnXacNhan);
            this.Controls.Add(card);

            this.AcceptButton = btnXacNhan;

            card.ResumeLayout(false);
            card.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
