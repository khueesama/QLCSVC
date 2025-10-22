using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace QLCSVCWinApp.Forms
{
    partial class frmMain
    {
        private IContainer components = null;

        // Banner
        private Panel panelHeader;
        private PictureBox pbHeader;
        private Label lblChao;

        // Thân dưới banner
        private Panel pnlBody;
        private Panel panelMenu;
        private Panel panelMain;

        // Menu buttons
        private Button btnQLThietBi;
        private Button btnQLPhong;
        private Button btnBaoCao;
        private Button btnLichSu;
        private Button btnSaoLuu;
        private Button btnPhucHoi;
        private Button btnDangXuat;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new Container();

            // ===== Banner =====
            this.panelHeader = new Panel();
            this.pbHeader = new PictureBox();
            this.lblChao = new Label();

            // ===== Thân =====
            this.pnlBody = new Panel();
            this.panelMenu = new Panel();
            this.panelMain = new Panel();

            this.btnQLThietBi = new Button();
            this.btnQLPhong = new Button();
            this.btnBaoCao = new Button();
            this.btnLichSu = new Button();
            this.btnSaoLuu = new Button();
            this.btnPhucHoi = new Button();
            this.btnDangXuat = new Button();

            // ===== frmMain =====
            this.SuspendLayout();
            this.ClientSize = new Size(1100, 680);
            this.Text = "Hệ thống quản lý cơ sở vật chất";
            this.Load += new System.EventHandler(this.frmMain_Load);

            // ===== panelHeader =====
            this.panelHeader.Dock = DockStyle.Top;
            this.panelHeader.Height = 110;
            this.panelHeader.BackColor = Color.FromArgb(245, 248, 252);
            this.panelHeader.Padding = new Padding(10);

            this.pbHeader.Dock = DockStyle.Fill;
            this.pbHeader.BackColor = Color.Transparent;
            this.pbHeader.SizeMode = PictureBoxSizeMode.Zoom;

            this.lblChao.AutoSize = true;
            this.lblChao.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            this.lblChao.Location = new Point(16, 16);
            this.lblChao.Text = "Chào mừng bạn đến với hệ thống quản lý cơ sở vật chất";

            this.panelHeader.Controls.Add(this.pbHeader);

            // ===== pnlBody =====
            this.pnlBody.Dock = DockStyle.Fill;
            this.pnlBody.BackColor = Color.White;

            // ===== panelMenu =====
            this.panelMenu.Dock = DockStyle.Left;
            this.panelMenu.Width = 200;
            this.panelMenu.BackColor = Color.FromArgb(245, 248, 252);
            this.panelMenu.Padding = new Padding(12, 14, 12, 12);

            var panelMenuBorder = new Panel
            {
                Dock = DockStyle.Right,
                Width = 1,
                BackColor = Color.FromArgb(220, 230, 240)
            };
            this.panelMenu.Controls.Add(panelMenuBorder);

            // ===== panelMain =====
            this.panelMain.Dock = DockStyle.Fill;
            this.panelMain.BackColor = Color.Transparent;

            // ===== Buttons =====
            Size btnSize = new Size(this.panelMenu.Width - 24, 40);
            int top = 0, step = 44;

            Color normalBg = Color.FromArgb(245, 248, 252);
            Color hoverBg = Color.FromArgb(232, 241, 250);
            Color pressedBg = Color.FromArgb(220, 235, 248);
            Color textColor = Color.FromArgb(30, 40, 55);

            void Style(Button b, string text)
            {
                b.Text = "   " + text;
                b.TextAlign = ContentAlignment.MiddleLeft;
                b.ImageAlign = ContentAlignment.MiddleLeft;
                b.TextImageRelation = TextImageRelation.ImageBeforeText;

                b.Size = btnSize;
                b.Location = new Point(0, top);
                top += step;

                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;
                b.FlatAppearance.MouseOverBackColor = hoverBg;
                b.FlatAppearance.MouseDownBackColor = pressedBg;
                b.UseVisualStyleBackColor = false;
                b.BackColor = normalBg;

                b.ForeColor = textColor;
                b.Font = new Font("Segoe UI", 10.5f, FontStyle.Regular);
                b.Padding = new Padding(10, 0, 8, 0);
                b.Cursor = Cursors.Hand;
            }

            Style(this.btnQLThietBi, "Quản lý thiết bị");
            Style(this.btnQLPhong, "Quản lý phòng");
            Style(this.btnBaoCao, "Báo cáo");
            Style(this.btnLichSu, "Lịch sử thay đổi");
            Style(this.btnSaoLuu, "Sao lưu");
            Style(this.btnPhucHoi, "Phục hồi");
            Style(this.btnDangXuat, "Đăng xuất");

            this.btnQLThietBi.Click += new System.EventHandler(this.btnQLThietBi_Click);
            this.btnQLPhong.Click += new System.EventHandler(this.btnQLPhong_Click);
            this.btnBaoCao.Click += new System.EventHandler(this.btnBaoCao_Click);
            this.btnLichSu.Click += new System.EventHandler(this.btnLichSu_Click);
            this.btnSaoLuu.Click += new System.EventHandler(this.btnSaoLuu_Click);
            this.btnPhucHoi.Click += new System.EventHandler(this.btnPhucHoi_Click);
            this.btnDangXuat.Click += new System.EventHandler(this.btnDangXuat_Click);

            this.panelMenu.Controls.AddRange(new Control[] {
                this.btnQLThietBi, this.btnQLPhong, this.btnBaoCao, this.btnLichSu,
                this.btnSaoLuu, this.btnPhucHoi, this.btnDangXuat
            });

            // Lời chào trong panelMain
            this.panelMain.Controls.Add(this.lblChao);

            // Lắp ráp
            this.pnlBody.Controls.Add(this.panelMain);
            this.pnlBody.Controls.Add(this.panelMenu);

            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.panelHeader);

            this.ResumeLayout(false);
        }
    }
}
