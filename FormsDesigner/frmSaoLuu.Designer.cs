using System.Drawing;
using System.Windows.Forms;

namespace QLCSVCWinApp.Forms
{
    partial class frmSaoLuu
    {
        private System.ComponentModel.IContainer components = null;
        
        private Label lblTitle;
        private Label lblHint;
        private Label lblPath;
        private TextBox txtPath;
        private Button btnBrowse;
        private Button btnBackup;
        private Button btnRefresh;
        private DataGridView dgv;
        private Label lblSection;
        private Label lblCount;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            // Panels / layouts
            var tlTop = new TableLayoutPanel();
            var flButtons = new FlowLayoutPanel();

            // Controls
            lblTitle = new Label();
            lblHint = new Label();
            lblPath = new Label();
            txtPath = new TextBox();
            btnBrowse = new Button();
            btnBackup = new Button();
            btnRefresh = new Button();
            lblSection = new Label();
            lblCount = new Label();
            dgv = new DataGridView();

            SuspendLayout();

            // ===== Top TableLayout (2 cột: trái là label/textbox, phải là cụm nút) =====
            tlTop.Dock = DockStyle.Top;
            tlTop.BackColor = Color.FromArgb(248, 251, 255);
            tlTop.Padding = new Padding(16, 12, 16, 12);
            tlTop.RowCount = 3;
            tlTop.ColumnCount = 2;
            tlTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlTop.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tlTop.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // title
            tlTop.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // hint
            tlTop.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // path + buttons

            // Title
            lblTitle.Text = "SAO LƯU DỮ LIỆU";
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(30, 60, 120);

            // Hint
            lblHint.Text = "Đặt tên file .bak (lưu trên thư mục BACKUP mặc định của SQL Server)";
            lblHint.AutoSize = true;
            lblHint.ForeColor = Color.FromArgb(90, 100, 120);

            // Path row (label + textbox)
            var pnlPath = new TableLayoutPanel { ColumnCount = 2, Dock = DockStyle.Top, AutoSize = true };
            pnlPath.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlPath.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            lblPath.Text = "Đường dẫn file .bak:";
            lblPath.AutoSize = true;
            txtPath.Dock = DockStyle.Fill;

            pnlPath.Controls.Add(lblPath, 0, 0);
            pnlPath.Controls.Add(txtPath, 1, 0);

            // Buttons (flow right-to-left để luôn dính cạnh phải)
            flButtons.FlowDirection = FlowDirection.RightToLeft;
            flButtons.WrapContents = false;
            flButtons.AutoSize = true;
            flButtons.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flButtons.Dock = DockStyle.Fill;
            flButtons.Padding = new Padding(0);

            btnRefresh.Text = "Làm mới";
            btnRefresh.AutoSize = true;
            btnRefresh.Click += btnRefresh_Click;

            btnBackup.Text = "Sao lưu";
            btnBackup.AutoSize = true;
            btnBackup.Click += btnBackup_Click;

            btnBrowse.Text = "Chọn…";
            btnBrowse.AutoSize = true;
            btnBrowse.Click += btnBrowse_Click;

            // Add theo thứ tự ngược (Rtl) để nút “Làm mới” nằm ngoài cùng bên phải
            flButtons.Controls.AddRange(new Control[] { btnRefresh, btnBackup, btnBrowse });

            // Đặt các control vào top layout
            tlTop.Controls.Add(lblTitle, 0, 0);
            tlTop.SetColumnSpan(lblTitle, 2);

            tlTop.Controls.Add(lblHint, 0, 1);
            tlTop.SetColumnSpan(lblHint, 2);

            tlTop.Controls.Add(pnlPath, 0, 2);
            tlTop.Controls.Add(flButtons, 1, 2);

            // Section + count hàng lịch sử
            var tlHead = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Padding = new Padding(16, 6, 16, 6),
                ColumnCount = 2,
                AutoSize = true
            };
            tlHead.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlHead.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            lblSection.Text = "Lịch sử sao lưu";
            lblSection.AutoSize = true;
            lblSection.Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold);

            lblCount.Text = "0 bản ghi";
            lblCount.AutoSize = true;

            tlHead.Controls.Add(lblSection, 0, 0);
            tlHead.Controls.Add(lblCount, 1, 0);

            // Grid
            dgv.Dock = DockStyle.Fill;
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.AllowUserToAddRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Form
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1188, 640);
            Controls.Add(dgv);
            Controls.Add(tlHead);
            Controls.Add(tlTop);
            Text = "Sao lưu dữ liệu";
            Load += frmSaoLuu_Load;

            ResumeLayout(false);
        }

    }
}
