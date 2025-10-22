// ======= FormsDesigner/frmBaoCao.Designer.cs (FINAL) =======
using System.Drawing;
using System.Windows.Forms;

namespace QLCSVCWinApp.Forms
{
    partial class frmBaoCao
    {
        private System.ComponentModel.IContainer components = null;

        // Header
        private Panel pTop;
        private Label lblTitle;
        private Label lblLoai;
        private ComboBox cbLoaiBaoCao;
        private Label lblFmt;
        private ComboBox cbDinhDang;
        private Label lblTD;
        private TextBox txtTieuDePhu;

        // Content
        private TableLayoutPanel layout;
        public GroupBox pnlPhong, pnlDate, pnlFilterTB, pnlDoiTuongLS;

        private CheckedListBox clbPhong;

        private CheckBox chkFrom, chkTo, chkChuaCoPhong;
        private DateTimePicker dtFrom, dtTo;

        private TextBox txtKw;
        private ComboBox cbLocLoai, cbLocTinhTrang;

        private ComboBox cbModule, cbHanhDong;
        private TextBox txtDoiTuong, txtNguoiDung;

        // Footer / Preview
        private Panel pButtons;
        private Button btnXem, btnXuat;
        private Label lblCount;
        private DataGridView dgvPreview;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            // ===== Form =====
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.White;
            Text = "Xuất báo cáo";
            ClientSize = new Size(1188, 720);

            // ===== Header =====
            pTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 88,
                BackColor = Color.FromArgb(248, 251, 255),
                Padding = new Padding(16)
            };

            lblTitle = new Label
            {
                AutoSize = true,
                Text = "XUẤT BÁO CÁO",
                Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 60, 120),
                Location = new Point(16, 8)
            };

            lblLoai = new Label { AutoSize = true, Text = "Loại báo cáo:", Location = new Point(16, 52) };
            cbLoaiBaoCao = new ComboBox
            {
                Location = new Point(100, 48),
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            lblFmt = new Label { AutoSize = true, Text = "Định dạng:", Location = new Point(340, 52) };
            cbDinhDang = new ComboBox
            {
                Location = new Point(410, 48),
                Width = 140,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            lblTD = new Label { AutoSize = true, Text = "Tiêu đề phụ:", Location = new Point(570, 52) };
            txtTieuDePhu = new TextBox
            {
                Location = new Point(640, 48),
                Width = 360,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                PlaceholderText = "VD: Báo cáo thiết bị theo phòng"
            };

            pTop.Controls.AddRange(new Control[] { lblTitle, lblLoai, cbLoaiBaoCao, lblFmt, cbDinhDang, lblTD, txtTieuDePhu });

            // ===== Content layout (AutoSize để không đè lưới) =====
            layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 3,
                RowCount = 2,
                BackColor = Color.White,
                Padding = new Padding(0)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40f));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // hàng bộ lọc TB (3 group)
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // hàng Bộ lọc Lịch sử

            // ---- CHỌN PHÒNG ----
            pnlPhong = new GroupBox
            {
                Text = "Chọn phòng (bỏ trống = tất cả)",
                Dock = DockStyle.Fill,
                Margin = new Padding(8, 6, 8, 0)
            };
            clbPhong = new CheckedListBox
            {
                Dock = DockStyle.Fill,
                CheckOnClick = true,
                IntegralHeight = false
            };
            pnlPhong.Controls.Add(clbPhong);

            // ---- KHOẢNG THỜI GIAN ----
            pnlDate = new GroupBox
            {
                Text = "Khoảng thời gian",
                Dock = DockStyle.Fill,
                Margin = new Padding(8, 6, 8, 0)
            };
            chkFrom = new CheckBox { Text = "Từ", Location = new Point(12, 30), AutoSize = true };
            dtFrom = new DateTimePicker { Location = new Point(60, 26), Width = 220, Enabled = false };
            chkTo = new CheckBox { Text = "Đến", Location = new Point(12, 70), AutoSize = true };
            dtTo = new DateTimePicker { Location = new Point(60, 66), Width = 220, Enabled = false };
            pnlDate.Controls.AddRange(new Control[] { chkFrom, dtFrom, chkTo, dtTo });

            // ---- BỘ LỌC THIẾT BỊ ----
            pnlFilterTB = new GroupBox
            {
                Text = "Bộ lọc thiết bị",
                Dock = DockStyle.Fill,
                Margin = new Padding(8, 6, 8, 0)
            };
            var lblKw = new Label { Text = "Từ khóa:", AutoSize = true, Location = new Point(12, 30) };
            txtKw = new TextBox { Location = new Point(70, 26), Width = 150 };
            var lblLoaiTB = new Label { Text = "Loại:", AutoSize = true, Location = new Point(240, 30) };
            cbLocLoai = new ComboBox { Location = new Point(280, 26), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            var lblTT = new Label { Text = "Tình trạng:", AutoSize = true, Location = new Point(12, 70) };
            cbLocTinhTrang = new ComboBox { Location = new Point(90, 66), Width = 130, DropDownStyle = ComboBoxStyle.DropDownList };
            chkChuaCoPhong = new CheckBox { Text = "Kể cả chưa có phòng", AutoSize = true, Location = new Point(240, 68) };
            pnlFilterTB.Controls.AddRange(new Control[] { lblKw, txtKw, lblLoaiTB, cbLocLoai, lblTT, cbLocTinhTrang, chkChuaCoPhong });

            // ---- BỘ LỌC LỊCH SỬ ----
            pnlDoiTuongLS = new GroupBox
            {
                Text = "Bộ lọc Lịch sử",
                Dock = DockStyle.Fill,
                Visible = false,                 // mặc định ẩn
                Margin = new Padding(8, 6, 8, 6)
            };
            var lblM = new Label { Text = "Module:", AutoSize = true, Location = new Point(12, 34) };
            cbModule = new ComboBox { Location = new Point(70, 30), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            var lblH = new Label { Text = "Hành động:", AutoSize = true, Location = new Point(240, 34) };
            cbHanhDong = new ComboBox { Location = new Point(315, 30), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            var lblDT = new Label { Text = "Đối tượng:", AutoSize = true, Location = new Point(480, 34) };
            txtDoiTuong = new TextBox { Location = new Point(540, 30), Width = 170 };
            var lblUD = new Label { Text = "Người dùng:", AutoSize = true, Location = new Point(730, 34) };
            txtNguoiDung = new TextBox { Location = new Point(810, 30), Width = 160 };
            pnlDoiTuongLS.Controls.AddRange(new Control[] { lblM, cbModule, lblH, cbHanhDong, lblDT, txtDoiTuong, lblUD, txtNguoiDung });

            // Add 3 nhóm trên hàng 0
            layout.Controls.Add(pnlPhong, 0, 0);
            layout.Controls.Add(pnlDate, 1, 0);
            layout.Controls.Add(pnlFilterTB, 2, 0);

            // Hàng 1: nhóm lịch sử chiếm 3 cột
            layout.Controls.Add(pnlDoiTuongLS, 0, 1);
            layout.SetColumnSpan(pnlDoiTuongLS, 3);

            // ===== Buttons + counter =====
            pButtons = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                BackColor = Color.White,
                Padding = new Padding(10, 6, 10, 6)
            };
            btnXem = new Button { Text = "Xem trước", Width = 100, Height = 30, Location = new Point(10, 7) };
            btnXuat = new Button { Text = "Xuất", Width = 100, Height = 30, Location = new Point(120, 7) };
            lblCount = new Label { AutoSize = true, Text = "Số dòng: 0", Location = new Point(230, 12) };
            pButtons.Controls.AddRange(new Control[] { btnXem, btnXuat, lblCount });

            // ===== Preview grid (Fill phần còn lại) =====
            dgvPreview = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            // ===== Add to form theo thứ tự Dock =====
            Controls.Add(dgvPreview);   // Fill
            Controls.Add(pButtons);     // Top
            Controls.Add(layout);       // Top (AutoSize → đẩy pButtons xuống)
            Controls.Add(pTop);         // Top

            // ===== Events =====
            Load += new System.EventHandler(this.frmBaoCao_Load);
            cbLoaiBaoCao.SelectedIndexChanged += new System.EventHandler(this.cbLoaiBaoCao_SelectedIndexChanged);
            btnXem.Click += new System.EventHandler(this.btnXem_Click);
            btnXuat.Click += new System.EventHandler(this.btnXuat_Click);
        }
    }
}
