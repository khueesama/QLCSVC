using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using QLCSVCWinApp.Models;
using QLCSVCWinApp.Services;
using QLCSVCWinApp.Utils;

namespace QLCSVCWinApp.Forms
{
    public partial class frmQuanLyThietBi : Form
    {
        private readonly ThietBiService _svcTb = new ThietBiService();
        private readonly PhongHocService _svcP = new PhongHocService();

        private readonly List<string> _dsLoai = new() { "Máy lạnh", "CPU", "Màn hình", "Đèn", "Quạt", "Khác" };
        private readonly List<string> _dsTinhTrang = new() { "Đang sử dụng", "Hỏng", "Đang sửa", "Thanh lý" };

        private bool _isDirty = false;
        public bool IsDirty => _isDirty;

        private sealed class ComboItem
        {
            public string? Value { get; set; }  // MaPhong hoặc mã đặc biệt
            public string Text { get; set; } = "";
            public override string ToString() => Text;
        }

        public frmQuanLyThietBi()
        {
            InitializeComponent();

            cbLoai.SelectedIndexChanged += (_, __) => _isDirty = true;
            txtTenTB.TextChanged += (_, __) => _isDirty = true;
            dtNgayMua.ValueChanged += (_, __) => _isDirty = true;
            cbTinhTrang.SelectedIndexChanged += (_, __) => _isDirty = true;
            txtThongTin.TextChanged += (_, __) => _isDirty = true;
            txtGhiChu.TextChanged += (_, __) => _isDirty = true;
            cbTenPhong.SelectedIndexChanged += (_, __) => _isDirty = true;
        }

        private void frmQuanLyThietBi_Load(object sender, EventArgs e)
        {
            // combobox Loại/Tình trạng (nhập liệu)
            cbLoai.DataSource = new List<string>(_dsLoai);
            cbTinhTrang.DataSource = new List<string>(_dsTinhTrang);

            // combobox filter
            cbLocLoai.Items.Clear(); cbLocLoai.Items.Add("(Tất cả)");
            cbLocLoai.Items.AddRange(_dsLoai.ToArray());
            cbLocLoai.SelectedIndex = 0;

            cbLocTinhTrang.Items.Clear(); cbLocTinhTrang.Items.Add("(Tất cả)");
            cbLocTinhTrang.Items.AddRange(_dsTinhTrang.ToArray());
            cbLocTinhTrang.SelectedIndex = 0;

            LoadPhongToCombo();
            LoadData();
        }

        private void LoadPhongToCombo()
        {
            var rooms = _svcP.GetAll(); // List<PhongHoc>

            // ===== Combo NHẬP LIỆU: cbTenPhong =====
            var srcNhap = new List<ComboItem>
            {
                new ComboItem { Value = null, Text = "(Chưa có phòng)" }
            };
            foreach (var p in rooms)
                srcNhap.Add(new ComboItem { Value = p.MaPhong, Text = p.TenPhong });

            cbTenPhong.DisplayMember = "Text";
            cbTenPhong.ValueMember = "Value";
            cbTenPhong.DataSource = srcNhap;

            cbTenPhong.SelectedValueChanged -= CbTenPhong_SelectedValueChanged;
            cbTenPhong.SelectedValueChanged += CbTenPhong_SelectedValueChanged;

            // ===== Combo LỌC: cbLocPhong =====
            var srcLoc = new List<ComboItem>
            {
                new ComboItem { Value = null,       Text = "(Tất cả)" },
                new ComboItem { Value = "__NULL__", Text = "(Chưa có phòng)" }
            };
            foreach (var p in rooms)
                srcLoc.Add(new ComboItem { Value = p.MaPhong, Text = p.TenPhong });

            cbLocPhong.DisplayMember = "Text";
            cbLocPhong.ValueMember = "Value";
            cbLocPhong.DataSource = srcLoc;
            cbLocPhong.SelectedIndex = 0;
        }

        private void CbTenPhong_SelectedValueChanged(object? sender, EventArgs e)
        {
            var val = cbTenPhong.SelectedValue as string;
            txtMaPhong.Text = val ?? ""; // rỗng khi chưa có phòng
        }

        private string? GetRoomName(string? maPhong)
        {
            if (string.IsNullOrWhiteSpace(maPhong)) return null;
            var rooms = _svcP.GetAll();
            var r = rooms.Find(x => x.MaPhong == maPhong);
            return r?.TenPhong;
        }

        private void UpdateTbCount(int n)
        {
            var lbl = this.Controls.Find("lblTBCount", true);
            if (lbl.Length > 0 && lbl[0] is Label L)
                L.Text = $"Số thiết bị: {n}";
        }

        private void LoadData()
        {
            var data = _svcTb.GetAll();
            dgvTB.DataSource = data;
            if (dgvTB.Columns["MaThietBi"] != null) dgvTB.Columns["MaThietBi"].HeaderText = "Mã thiết bị";
            if (dgvTB.Columns["LoaiThietBi"] != null) dgvTB.Columns["LoaiThietBi"].HeaderText = "Loại thiết bị";
            if (dgvTB.Columns["TenThietBi"] != null) dgvTB.Columns["TenThietBi"].HeaderText = "Tên thiết bị";
            if (dgvTB.Columns["NgayMua"] != null) dgvTB.Columns["NgayMua"].HeaderText = "Ngày mua";
            if (dgvTB.Columns["TinhTrang"] != null) dgvTB.Columns["TinhTrang"].HeaderText = "Tình trạng";
            if (dgvTB.Columns["ThongTin"] != null) dgvTB.Columns["ThongTin"].HeaderText = "Thông tin";
            if (dgvTB.Columns["GhiChu"] != null) dgvTB.Columns["GhiChu"].HeaderText = "Ghi chú";
            if (dgvTB.Columns["MaPhong"] != null) dgvTB.Columns["MaPhong"].HeaderText = "Mã phòng";
            if (dgvTB.Columns["TenPhong"] != null) dgvTB.Columns["TenPhong"].HeaderText = "Tên phòng";

            dgvTB.ClearSelection();

            UpdateTbCount(data.Count);
            ClearInputs();
        }

        private void ClearInputs()
        {
            cbLoai.SelectedIndex = 0;
            txtTenTB.Clear();
            dtNgayMua.Value = DateTime.Today;
            cbTinhTrang.SelectedIndex = 0;
            txtThongTin.Clear();
            txtGhiChu.Clear();

            cbTenPhong.SelectedIndex = 0; // "(Chưa có phòng)" => txtMaPhong=""
            txtMaPhong.Clear();

            _isDirty = false;
        }

        private ThietBi ReadForm()
        {
            var maPhongVal = cbTenPhong.SelectedValue as string; // null nếu "(Chưa có phòng)"
            return new ThietBi
            {
                LoaiThietBi = cbLoai.SelectedItem?.ToString() ?? "",
                TenThietBi = txtTenTB.Text.Trim(),
                NgayMua = dtNgayMua.Value.Date,
                TinhTrang = cbTinhTrang.SelectedItem?.ToString() ?? "",
                ThongTin = txtThongTin.Text.Trim(),
                GhiChu = string.IsNullOrWhiteSpace(txtGhiChu.Text) ? null : txtGhiChu.Text.Trim(),
                MaPhong = maPhongVal
            };
        }

        private bool ValidateRequiredInputs()
        {
            if (string.IsNullOrWhiteSpace(txtTenTB.Text))
            {
                MessageBox.Show("Tên thiết bị không được để trống.", "Thiếu thông tin",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenTB.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtThongTin.Text))
            {
                MessageBox.Show("Thông tin thiết bị không được để trống.", "Thiếu thông tin",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtThongTin.Focus();
                return false;
            }
            return true;
        }

        private void dgvTB_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTB.CurrentRow == null) return;
            if (dgvTB.CurrentRow.DataBoundItem is not ThietBi tb) return;

            cbLoai.SelectedItem = tb.LoaiThietBi;
            txtTenTB.Text = tb.TenThietBi;
            dtNgayMua.Value = tb.NgayMua;
            cbTinhTrang.SelectedItem = tb.TinhTrang;
            txtThongTin.Text = tb.ThongTin;
            txtGhiChu.Text = tb.GhiChu ?? "";

            if (tb.MaPhong == null)
            {
                cbTenPhong.SelectedIndex = 0; // "(Chưa có phòng)"
                txtMaPhong.Clear();
            }
            else
            {
                cbTenPhong.SelectedValue = tb.MaPhong;
                txtMaPhong.Text = tb.MaPhong;
            }

            _isDirty = false;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!ValidateRequiredInputs()) return;

            var tb = ReadForm();

            var tenPhong = GetRoomName(tb.MaPhong);

            string id = _svcTb.AddAndGetId(tb);
            if (!string.IsNullOrEmpty(id))
            {
                tb.MaThietBi = id;
                LichSuThayDoiLogger.LogThietBi(id, "Thêm", ChangeFormatter.BuildTBAdd(tb, tenPhong));
                LoadData();
            }
            else
            {
                MessageBox.Show("Thêm thiết bị thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCapNhat_Click(object sender, EventArgs e)
        {
            if (dgvTB.CurrentRow == null)
            {
                MessageBox.Show("Chọn một thiết bị để cập nhật.");
                return;
            }
            if (!ValidateRequiredInputs()) return;
            if (dgvTB.CurrentRow.DataBoundItem is not ThietBi current) return;

            var oldT = new ThietBi
            {
                MaThietBi = current.MaThietBi,
                LoaiThietBi = current.LoaiThietBi,
                TenThietBi = current.TenThietBi,
                NgayMua = current.NgayMua,
                TinhTrang = current.TinhTrang,
                ThongTin = current.ThongTin,
                GhiChu = current.GhiChu,
                MaPhong = current.MaPhong
            };
            var oldRoomName = GetRoomName(oldT.MaPhong);

            var tb = ReadForm();
            tb.MaThietBi = current.MaThietBi;
            var newRoomName = GetRoomName(tb.MaPhong);

            if (_svcTb.Update(tb))
            {
                var detail = ChangeFormatter.BuildTBDiff(oldT, tb, oldRoomName, newRoomName);
                LichSuThayDoiLogger.LogThietBi(tb.MaThietBi, "Cập nhật", detail);
                LoadData();
            }
            else
            {
                MessageBox.Show("Cập nhật thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvTB.CurrentRow?.DataBoundItem is not ThietBi current) return;

            if (MessageBox.Show($"Xác nhận xoá thiết bị {current.MaThietBi} - {current.TenThietBi}?",
                                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes)
            {
                var roomName = GetRoomName(current.MaPhong);
                if (_svcTb.Delete(current.MaThietBi))
                {
                    LichSuThayDoiLogger.LogThietBi(current.MaThietBi, "Xoá",
                        ChangeFormatter.BuildTBDelete(current, roomName));
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Xoá thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public void SaveChanges()
        {
            // Nếu đang chọn 1 dòng => cập nhật, nếu không => thêm mới
            if (dgvTB.CurrentRow != null)
                btnCapNhat_Click(this, EventArgs.Empty);
            else
                btnThem_Click(this, EventArgs.Empty);
        }
        public void DiscardChanges()
        {
            // Huỷ thay đổi và nạp lại dữ liệu
            LoadData();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            // 1) Reset bộ lọc
            txtTim.Clear();
            if (cbLocLoai.Items.Count > 0) cbLocLoai.SelectedIndex = 0;   // (Tất cả)
            if (cbLocTinhTrang.Items.Count > 0) cbLocTinhTrang.SelectedIndex = 0;
            if (cbLocPhong.Items.Count > 0) cbLocPhong.SelectedIndex = 0;
            chkFrom.Checked = false;
            chkTo.Checked = false;

            // 2) Nạp lại danh sách thiết bị
            var data = _svcTb.GetAll();
            dgvTB.DataSource = data;

            if (dgvTB.Columns["TenPhong"] != null)
                dgvTB.Columns["TenPhong"].HeaderText = "Tên phòng";

            dgvTB.ClearSelection();

            // 3) Cập nhật bộ đếm (nếu có)
            var lbl = this.Controls.Find("lblTBCount", true);
            if (lbl.Length > 0 && lbl[0] is Label L)
                L.Text = $"Số thiết bị: {data.Count}";

            // 4) Xoá input khu vực nhập liệu
            cbLoai.SelectedIndex = 0;
            txtTenTB.Clear();
            dtNgayMua.Value = DateTime.Today;
            cbTinhTrang.SelectedIndex = 0;
            txtThongTin.Clear();
            txtGhiChu.Clear();
            if (cbTenPhong.Items.Count > 0) cbTenPhong.SelectedIndex = 0; // (Chưa có phòng)
            txtMaPhong.Clear();

            _isDirty = false;
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string? kw = string.IsNullOrWhiteSpace(txtTim.Text) ? null : txtTim.Text.Trim();
            string? loai = cbLocLoai.SelectedIndex <= 0 ? null : cbLocLoai.SelectedItem?.ToString();
            string? tt = cbLocTinhTrang.SelectedIndex <= 0 ? null : cbLocTinhTrang.SelectedItem?.ToString();

            string? phongFilter;
            var val = cbLocPhong.SelectedValue as string;
            if (cbLocPhong.SelectedIndex == 1)           // "(Chưa có phòng)"
                phongFilter = "";                        // dùng rỗng -> hiểu là NULL
            else if (cbLocPhong.SelectedIndex == 0)      // "(Tất cả)"
                phongFilter = null;
            else
                phongFilter = val;

            DateTime? from = chkFrom.Checked ? dtFrom.Value.Date : (DateTime?)null;
            DateTime? to = chkTo.Checked ? dtTo.Value.Date : (DateTime?)null;

            var data = _svcTb.Search(kw, loai, tt,
                         phongFilter == "" ? null : phongFilter, from, to);

            dgvTB.DataSource = data;

            if (dgvTB.Columns["TenPhong"] != null)
                dgvTB.Columns["TenPhong"].HeaderText = "Tên phòng";

            dgvTB.ClearSelection();

            if (this.Controls.ContainsKey("lblTBCount"))
            {
                var lbl = this.Controls["lblTBCount"] as Label;
                if (lbl != null) lbl.Text = $"Số thiết bị: {data.Count}";
            }

            if (data.Count == 0)
            {
                MessageBox.Show("Không tìm thấy thiết bị phù hợp.",
                                "Không có kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
