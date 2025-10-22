using System.Drawing;
using System.Windows.Forms;

namespace QLCSVCWinApp.Forms
{
    partial class frmQuanLyPhong
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlTop;
        private Label lblTitle;
        private Label lblPhongCount;   // NEW: đếm số phòng
        private Panel pnlBody;
        private SplitContainer split;
        private DataGridView dgvPhong;

        private Panel pnlInputs;
        private Label lblTenPhong, lblMoTa, lblTitleSoTB, lblSoTB;
        private TextBox txtTenPhong, txtMoTa;
        private Button btnAdd, btnUpdate, btnDelete, btnClear;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            // ===== Top =====
            pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                BackColor = Color.White,
                Padding = new Padding(12, 8, 12, 6)
            };

            lblTitle = new Label
            {
                AutoSize = true,
                Text = "QUẢN LÝ PHÒNG",
                Font = new Font("Segoe UI Semibold", 16f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 60, 120),
                Margin = new Padding(0),
                Location = new Point(16, 8)
            };

            lblPhongCount = new Label     // NEW
            {
                Name = "lblPhongCount",
                AutoSize = true,
                Text = "Số phòng: 0",
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(830, 12) // tuỳ khung form
            };

            pnlTop.Controls.Add(lblTitle);
            pnlTop.Controls.Add(lblPhongCount);

            // Divider
            var divider = new Panel
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = Color.FromArgb(225, 230, 240),
                Margin = new Padding(0)
            };

            // ===== Body =====
            pnlBody = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(12, 8, 12, 12)
            };

            split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 260,
                BackColor = Color.White
            };

            dgvPhong = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                Margin = new Padding(0)
            };
            dgvPhong.SelectionChanged += dgvPhong_SelectionChanged;
            split.Panel1.Controls.Add(dgvPhong);

            pnlInputs = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(0)
            };

            lblTenPhong = new Label { Text = "Tên phòng:", AutoSize = true, Location = new Point(6, 10) };
            txtTenPhong = new TextBox { Location = new Point(90, 8), Width = 360 };

            lblMoTa = new Label { Text = "Mô tả:", AutoSize = true, Location = new Point(6, 44) };
            txtMoTa = new TextBox
            {
                Location = new Point(90, 42),
                Size = new Size(360, 72),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            btnAdd = new Button { Text = "Thêm", Location = new Point(90, 122), Size = new Size(86, 30) };
            btnUpdate = new Button { Text = "Cập nhật", Location = new Point(182, 122), Size = new Size(86, 30) };
            btnDelete = new Button { Text = "Xoá", Location = new Point(274, 122), Size = new Size(86, 30) };
            btnClear = new Button { Text = "Làm mới", Location = new Point(366, 122), Size = new Size(86, 30) };

            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnClear.Click += btnClear_Click;

            lblTitleSoTB = new Label { AutoSize = true, Text = "Số thiết bị trong phòng:", Location = new Point(6, 162) };
            lblSoTB = new Label { AutoSize = true, Text = "0", Location = new Point(150, 162) };

            pnlInputs.Controls.AddRange(new Control[] {
                lblTenPhong, txtTenPhong,
                lblMoTa, txtMoTa,
                btnAdd, btnUpdate, btnDelete, btnClear,
                lblTitleSoTB, lblSoTB
            });

            split.Panel2.Controls.Add(pnlInputs);
            pnlBody.Controls.Add(split);

            // ===== Form =====
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(980, 640);
            Text = "Quản lý phòng";

            Controls.Add(pnlBody);
            Controls.Add(divider);
            Controls.Add(pnlTop);

            Load += frmQuanLyPhong_Load;
        }
    }
}
