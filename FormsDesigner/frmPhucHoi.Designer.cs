using System.Drawing;
using System.Windows.Forms;

namespace QLCSVCWinApp.Forms
{
    partial class frmPhucHoi
    {
        private System.ComponentModel.IContainer components = null;
        
        private Label lblTitle;
        private Label lblNote;
        private Label lblFile;
        private TextBox txtFile;
        private Button btnBrowse;
        private Button btnRestore;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            var tlTop = new TableLayoutPanel();
            var flButtons = new FlowLayoutPanel();
            var pnlFile = new TableLayoutPanel();

            lblTitle = new Label();
            lblNote = new Label();
            lblFile = new Label();
            txtFile = new TextBox();
            btnBrowse = new Button();
            btnRestore = new Button();

            SuspendLayout();

            // Top table (giống Sao lưu)
            tlTop.Dock = DockStyle.Top;
            tlTop.BackColor = Color.FromArgb(248, 251, 255);
            tlTop.Padding = new Padding(16, 12, 16, 12);
            tlTop.RowCount = 3;
            tlTop.ColumnCount = 2;
            tlTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlTop.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tlTop.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlTop.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlTop.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Title
            lblTitle.Text = "PHỤC HỒI DỮ LIỆU";
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(30, 60, 120);

            // Note
            lblNote.Text = "Chọn tệp sao lưu (.bak) để khôi phục cơ sở dữ liệu.";
            lblNote.AutoSize = true;
            lblNote.ForeColor = Color.FromArgb(90, 100, 120);

            // File row (label + textbox)
            pnlFile.ColumnCount = 2;
            pnlFile.Dock = DockStyle.Top;
            pnlFile.AutoSize = true;
            pnlFile.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlFile.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            lblFile.Text = "Chọn file .bak:";
            lblFile.AutoSize = true;

            txtFile.Dock = DockStyle.Fill;

            pnlFile.Controls.Add(lblFile, 0, 0);
            pnlFile.Controls.Add(txtFile, 1, 0);

            // Buttons (bên phải)
            flButtons.FlowDirection = FlowDirection.RightToLeft;
            flButtons.WrapContents = false;
            flButtons.AutoSize = true;
            flButtons.Dock = DockStyle.Fill;

            btnRestore.Text = "Phục hồi";
            btnRestore.AutoSize = true;
            btnRestore.Click += btnRestore_Click;

            btnBrowse.Text = "Chọn…";
            btnBrowse.AutoSize = true;
            btnBrowse.Click += btnBrowse_Click;

            flButtons.Controls.AddRange(new Control[] { btnRestore, btnBrowse });

            // Add vào top
            tlTop.Controls.Add(lblTitle, 0, 0);
            tlTop.SetColumnSpan(lblTitle, 2);
            tlTop.Controls.Add(lblNote, 0, 1);
            tlTop.SetColumnSpan(lblNote, 2);
            tlTop.Controls.Add(pnlFile, 0, 2);
            tlTop.Controls.Add(flButtons, 1, 2);

            // Form
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1188, 640);
            Controls.Add(tlTop);
            Text = "Phục hồi dữ liệu";

            ResumeLayout(false);
        }

    }
}
