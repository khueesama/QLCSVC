using System.Drawing;
using System.Windows.Forms;

namespace QLCSVCWinApp.Forms
{
    partial class frmQuanLyThietBi
    {
        private System.ComponentModel.IContainer components = null;

        // grid
        private DataGridView dgvTB;

        // nhập liệu
        private ComboBox cbLoai, cbTinhTrang, cbPhong; // cbPhong sẽ bị ẩn
        private TextBox txtTenTB, txtThongTin, txtGhiChu;
        private DateTimePicker dtNgayMua;
        private ComboBox cbTenPhong;    // chọn tên phòng
        private TextBox txtMaPhong;     // hiển mã phòng (readonly)

        // filter
        private TextBox txtTim;
        private ComboBox cbLocLoai, cbLocTinhTrang, cbLocPhong;
        private DateTimePicker dtFrom, dtTo;
        private CheckBox chkFrom, chkTo;
        private Button btnTim, btnLamMoi;

        // action
        private Button btnThem, btnCapNhat, btnXoa, btnClear;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            // ====== TOP BAR ======
            var pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                BackColor = Color.White,
                Padding = new Padding(12, 8, 12, 6)
            };
            var lblTitle = new Label
            {
                AutoSize = true,
                Text = "QUẢN LÝ THIẾT BỊ",
                Font = new Font("Segoe UI Semibold", 16f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 60, 120),
                Margin = new Padding(0),
                Location = new Point(16, 8)
            };
            pnlTop.Controls.Add(lblTitle);

            // Divider 1px
            var divider = new Panel
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = Color.FromArgb(225, 230, 240),
                Margin = new Padding(0)
            };

            // ====== FILTER BAR ======
            var pnlFilter = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                Padding = new Padding(8, 6, 8, 0),
                BackColor = Color.White
            };

            var lblTBCount = new Label
            {
                Name = "lblTBCount",
                AutoSize = true,
                Padding = new Padding(6, 8, 0, 0),
                Text = "Số thiết bị: 0"
            };

            txtTim = new TextBox { Width = 170, PlaceholderText = "Tìm tên/thông tin..." };
            cbLocLoai = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
            cbLocTinhTrang = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
            cbLocPhong = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 160 };
            chkFrom = new CheckBox { Text = "Từ ngày", AutoSize = true };
            dtFrom = new DateTimePicker { Width = 130, Enabled = false };
            chkTo = new CheckBox { Text = "Đến ngày", AutoSize = true };
            dtTo = new DateTimePicker { Width = 130, Enabled = false };
            btnTim = new Button { Text = "Tìm", Width = 70 };
            btnLamMoi = new Button { Text = "Làm mới", Width = 80 };

            chkFrom.CheckedChanged += (_, __) => dtFrom.Enabled = chkFrom.Checked;
            chkTo.CheckedChanged += (_, __) => dtTo.Enabled = chkTo.Checked;
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

            var lblLocLoai = new Label { Text = "Loại:", AutoSize = true, Padding = new Padding(8, 8, 4, 0) };
            var lblLocTinhTrang = new Label { Text = "Tình trạng:", AutoSize = true, Padding = new Padding(8, 8, 4, 0) };
            var lblLocPhong = new Label { Text = "Phòng:", AutoSize = true, Padding = new Padding(8, 8, 4, 0) };

            Control[] filters = {
                txtTim, lblLocLoai, cbLocLoai, lblLocTinhTrang, cbLocTinhTrang, lblLocPhong, cbLocPhong,
                chkFrom, dtFrom, chkTo, dtTo, btnTim, btnLamMoi, lblTBCount
            };
            foreach (var c in filters) c.Margin = new Padding(0, 0, 8, 0);

            flow.Controls.AddRange(filters);
            pnlFilter.Controls.Add(flow);

            // ====== GRID ======
            dgvTB = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 240,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                Margin = new Padding(0)
            };
            dgvTB.SelectionChanged += dgvTB_SelectionChanged;

            // ====== INPUTS ======
            var pnlInputs = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12, 8, 12, 12),
                BackColor = Color.White
            };

            // Hàng 1
            var lblLoai = new Label { Text = "Loại TB:", AutoSize = true, Location = new Point(8, 8) };
            cbLoai = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(70, 4), Width = 140 };

            var lblTen = new Label { Text = "Tên TB:", AutoSize = true, Location = new Point(220, 8) };
            txtTenTB = new TextBox { Location = new Point(270, 4), Width = 220 };

            var lblNgay = new Label { Text = "Ngày mua:", AutoSize = true, Location = new Point(500, 8) };
            dtNgayMua = new DateTimePicker { Location = new Point(572, 4), Width = 130 };

            // Hàng 2
            var lblTinhTrang = new Label { Text = "Tình trạng:", AutoSize = true, Location = new Point(8, 40) };
            cbTinhTrang = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(80, 36), Width = 140 };

            // Ẩn "Phòng (mã)" cũ
            var lblPhong = new Label { Visible = false };
            cbPhong = new ComboBox { Visible = false };

            // Đưa "Tên phòng" vào vị trí trung tâm
            var lblPhongTen = new Label { Text = "Tên phòng:", AutoSize = true, Location = new Point(228, 40) };
            cbTenPhong = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(306, 36), Width = 200 };

            // Mã phòng (readonly) – cùng hàng
            var lblMaPhong = new Label { Text = "Mã phòng:", AutoSize = true, Location = new Point(520, 40) };
            txtMaPhong = new TextBox
            {
                Location = new Point(592, 36),
                Width = 110,
                ReadOnly = true,
                TabStop = false,
                BackColor = SystemColors.Control
            };

            // Thông tin / Ghi chú
            var lblInfo = new Label { Text = "Thông tin:", AutoSize = true, Location = new Point(8, 102) };
            txtThongTin = new TextBox
            {
                Location = new Point(80, 98),
                Width = 622,
                Height = 56,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            var lblNote = new Label { Text = "Ghi chú:", AutoSize = true, Location = new Point(8, 160) };
            txtGhiChu = new TextBox
            {
                Location = new Point(80, 156),
                Width = 622,
                Height = 56,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            // Buttons
            btnThem = new Button { Text = "Thêm", Location = new Point(80, 220), Size = new Size(90, 32) };
            btnCapNhat = new Button { Text = "Cập nhật", Location = new Point(180, 220), Size = new Size(90, 32) };
            btnXoa = new Button { Text = "Xoá", Location = new Point(280, 220), Size = new Size(90, 32) };
            btnClear = new Button { Text = "Làm mới", Location = new Point(380, 220), Size = new Size(90, 32) };
            btnThem.Click += btnThem_Click;
            btnCapNhat.Click += btnCapNhat_Click;
            btnXoa.Click += btnXoa_Click;
            btnClear.Click += btnClear_Click;

            pnlInputs.Controls.AddRange(new Control[] {
                // hàng 1
                lblLoai, cbLoai, lblTen, txtTenTB, lblNgay, dtNgayMua,
                // hàng 2 (đã chỉnh)
                lblTinhTrang, cbTinhTrang, lblPhongTen, cbTenPhong, lblMaPhong, txtMaPhong,
                // mô tả
                lblInfo, txtThongTin, lblNote, txtGhiChu,
                // buttons
                btnThem, btnCapNhat, btnXoa, btnClear
            });

            // ====== FORM ======
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(780, 640);
            Text = "Quản lý thiết bị";
            BackColor = Color.White;

            Controls.Add(pnlInputs);
            Controls.Add(dgvTB);
            Controls.Add(pnlFilter);
            Controls.Add(divider);
            Controls.Add(pnlTop);

            Load += frmQuanLyThietBi_Load;
        }
    }
}
