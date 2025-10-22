using System.Drawing;
using System.Windows.Forms;

namespace QLCSVCWinApp.Forms
{
    partial class frmLichSuThayDoi
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlTop;
        private Label lblTitle;
        private Panel pnlFilter;
        private DataGridView dgvLS;

        private ComboBox cbModule, cbHanhDong;
        private TextBox txtDoiTuong, txtNguoiDung;
        private DateTimePicker dtFrom, dtTo;
        private CheckBox chkFrom, chkTo;
        private Button btnTim, btnLamMoi;   // <-- ĐÃ BỎ nút Xem chi tiết
        private Label lblCount;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            // Top
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
                Text = "LỊCH SỬ THAY ĐỔI",
                Font = new Font("Segoe UI Semibold", 16f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 60, 120),
                Margin = new Padding(0),
                Location = new Point(16, 8)
            };
            pnlTop.Controls.Add(lblTitle);

            // Divider
            var divider = new Panel
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = Color.FromArgb(225, 230, 240),
                Margin = new Padding(0)
            };

            // Filter
            pnlFilter = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = Color.White,
                Padding = new Padding(8, 6, 8, 0)
            };

            cbModule = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 130 };
            cbHanhDong = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 130 };
            txtDoiTuong = new TextBox { Width = 160, PlaceholderText = "Đối tượng (mã/chuỗi)" };
            txtNguoiDung = new TextBox { Width = 130, PlaceholderText = "Người dùng" };
            chkFrom = new CheckBox { Text = "Từ", AutoSize = true };
            dtFrom = new DateTimePicker { Width = 140, Enabled = true };
            chkTo = new CheckBox { Text = "Đến", AutoSize = true };
            dtTo = new DateTimePicker { Width = 140, Enabled = true };
            btnTim = new Button { Text = "Tìm", Width = 70 };
            btnLamMoi = new Button { Text = "Làm mới", Width = 80 };
            lblCount = new Label { AutoSize = true, Padding = new Padding(8, 8, 0, 0), Text = "Bản ghi: 0" };

            btnTim.Click += btnTim_Click;
            btnLamMoi.Click += btnLamMoi_Click;

            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                WrapContents = false,
                AutoScroll = true,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            var lblModule = new Label { Text = "Module:", AutoSize = true, Padding = new Padding(0, 8, 4, 0) };
            var lblHanhDong = new Label { Text = "Hành động:", AutoSize = true, Padding = new Padding(12, 8, 4, 0) };

            Control[] filters = {
                lblModule, cbModule, lblHanhDong, cbHanhDong,
                txtDoiTuong, txtNguoiDung,
                chkFrom, dtFrom, chkTo, dtTo,
                btnTim, btnLamMoi, lblCount
            };
            foreach (var c in filters) c.Margin = new Padding(0, 0, 8, 0);

            flow.Controls.AddRange(filters);
            pnlFilter.Controls.Add(flow);

            // Grid
            dgvLS = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                Margin = new Padding(0)
            };
            // Double-click handler gán trong Load ở .cs

            // Form
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1188, 640);
            Text = "Lịch sử thay đổi";

            Controls.Add(dgvLS);
            Controls.Add(pnlFilter);
            Controls.Add(divider);
            Controls.Add(pnlTop);

            Load += frmLichSuThayDoi_Load;
        }
    }
}
