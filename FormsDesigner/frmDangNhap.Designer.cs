using System.Drawing;
using System.Windows.Forms;

namespace QLCSVCWinApp.Forms
{
    partial class frmDangNhap
    {
        private System.ComponentModel.IContainer components = null;
        private Panel card;
        private Label lblTitle, lblSub;
        private TextBox txtTaiKhoan, txtMatKhau;
        private Button btnDangNhap, btnDangKy;
        private LinkLabel lnkQuenMK;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            // form
            this.SuspendLayout();
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(246, 248, 252);  // nền sáng
            this.ClientSize = new Size(420, 520);
            this.Font = new Font("Segoe UI", 10F);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Đăng nhập";
            this.Load += new System.EventHandler(this.frmDangNhap_Load);

            // card
            card = new Panel
            {
                BackColor = Color.White,
                Size = new Size(340, 380),
                Location = new Point((this.ClientSize.Width - 340) / 2, (this.ClientSize.Height - 380) / 2),
                Anchor = AnchorStyles.None,
                Padding = new Padding(24),
            };
            card.SuspendLayout();

            // title
            lblTitle = new Label
            {
                Text = "Đăng nhập",
                Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(24, 20)
            };

            lblSub = new Label
            {
                Text = "Vui lòng nhập thông tin để tiếp tục",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.DimGray,
                AutoSize = true,
                Location = new Point(24, 55)
            };

            // txtTaiKhoan
            txtTaiKhoan = new TextBox
            {
                Name = "txtTaiKhoan",
                PlaceholderText = "Tên đăng nhập",
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(24, 100),
                Width = 292
            };

            // txtMatKhau
            txtMatKhau = new TextBox
            {
                Name = "txtMatKhau",
                PlaceholderText = "Mật khẩu",
                UseSystemPasswordChar = true,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(24, 145),
                Width = 292
            };

            // btnDangNhap (primary)
            btnDangNhap = new Button
            {
                Name = "btnDangNhap",
                Text = "Đăng nhập",
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(24, 195),
                Width = 292,
                Height = 36
            };
            btnDangNhap.FlatAppearance.BorderSize = 0;
            btnDangNhap.Click += new System.EventHandler(this.btnDangNhap_Click);

            // lnkQuenMK
            lnkQuenMK = new LinkLabel
            {
                Name = "lnkQuenMK",
                Text = "Quên mật khẩu?",
                AutoSize = true,
                Location = new Point(24, 242),
                LinkBehavior = LinkBehavior.HoverUnderline
            };
            lnkQuenMK.Click += new System.EventHandler(this.btnQuenMK_Click);

            // btnDangKy (ghost)
            btnDangKy = new Button
            {
                Name = "btnDangKy",
                Text = "Tạo tài khoản mới",
                BackColor = Color.White,
                ForeColor = Color.FromArgb(0, 123, 255),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(24, 285),
                Width = 292,
                Height = 34
            };
            btnDangKy.FlatAppearance.BorderColor = Color.FromArgb(204, 221, 255);
            btnDangKy.Click += new System.EventHandler(this.btnDangKy_Click);

            // assemble
            card.Controls.Add(lblTitle);
            card.Controls.Add(lblSub);
            card.Controls.Add(txtTaiKhoan);
            card.Controls.Add(txtMatKhau);
            card.Controls.Add(btnDangNhap);
            card.Controls.Add(lnkQuenMK);
            card.Controls.Add(btnDangKy);
            this.Controls.Add(card);

            // default button
            this.AcceptButton = btnDangNhap;

            card.ResumeLayout(false);
            card.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
