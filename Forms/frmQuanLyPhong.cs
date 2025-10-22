using QLCSVCWinApp.DataAccess.QLCSVCWinApp.Models;
using QLCSVCWinApp.Models;
using QLCSVCWinApp.Services;
using QLCSVCWinApp.Utils;
using System;
using System.Windows.Forms;

namespace QLCSVCWinApp.Forms
{
    public partial class frmQuanLyPhong : Form
    {
        private readonly PhongHocService _svc = new PhongHocService();
        private bool _isDirty = false;

        public frmQuanLyPhong()
        {
            InitializeComponent();

            // đánh dấu dirty khi user sửa
            txtTenPhong.TextChanged += (_, __) => _isDirty = true;
            txtMoTa.TextChanged += (_, __) => _isDirty = true;
        }

        private void frmQuanLyPhong_Load(object sender, EventArgs e)
        {
            LoadPhong();
        }

        private void LoadPhong()
        {
            var data = _svc.GetAll();
            dgvPhong.DataSource = data;

            if (dgvPhong.Columns["MaPhong"] != null) dgvPhong.Columns["MaPhong"].HeaderText = "Mã phòng";
            if (dgvPhong.Columns["TenPhong"] != null) dgvPhong.Columns["TenPhong"].HeaderText = "Tên phòng";
            if (dgvPhong.Columns["MoTa"] != null) dgvPhong.Columns["MoTa"].HeaderText = "Mô tả";

            ClearInputs();
            _isDirty = false;

            UpdatePhongCount(data.Count);
        }

        private void UpdatePhongCount(int n)
        {
            var lbl = this.Controls.Find("lblPhongCount", true);
            if (lbl.Length > 0 && lbl[0] is Label L)
                L.Text = $"Số phòng: {n}";
        }

        private void ClearInputs()
        {
            txtTenPhong.Clear();
            txtMoTa.Clear();
            dgvPhong.ClearSelection();
            lblSoTB.Text = "0";
            _isDirty = false;
        }

        private void dgvPhong_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPhong.CurrentRow == null) return;
            if (dgvPhong.CurrentRow.DataBoundItem is not PhongHoc p) return;

            txtTenPhong.Text = p.TenPhong;
            txtMoTa.Text = p.MoTa;

            // Đếm thiết bị trong phòng
            try
            {
                int cnt = _svc.CountDevicesInRoom(p.MaPhong);
                lblSoTB.Text = cnt.ToString();
            }
            catch
            {
                lblSoTB.Text = "0";
            }

            _isDirty = false;   // vừa load, chưa sửa gì
        }

        private bool ValidateRequired()
        {
            if (string.IsNullOrWhiteSpace(txtTenPhong.Text))
            {
                MessageBox.Show("Tên phòng không được để trống.",
                                "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenPhong.Focus();
                return false;
            }
            return true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateRequired()) return;

            var p = new PhongHoc
            {
                TenPhong = txtTenPhong.Text.Trim(),
                MoTa = txtMoTa.Text.Trim()
            };

            // Lấy mã phòng mới để log
            string newId = _svc.AddAndGetId(p);
            if (!string.IsNullOrEmpty(newId))
            {
                LichSuThayDoiLogger.LogPhong(newId, "Thêm", ChangeFormatter.BuildPhongAdd(p));
                LoadPhong();
            }
            else
            {
                MessageBox.Show("Thêm phòng thất bại.", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvPhong.CurrentRow == null)
            {
                MessageBox.Show("Hãy chọn một phòng để cập nhật.",
                                "Thiếu lựa chọn", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!ValidateRequired()) return;

            var p = (PhongHoc)dgvPhong.CurrentRow.DataBoundItem;

            // snapshot bản cũ
            var oldP = new PhongHoc { MaPhong = p.MaPhong, TenPhong = p.TenPhong, MoTa = p.MoTa };

            // cập nhật theo input (đã đảm bảo tên phòng không rỗng ở ValidateRequired)
            p.TenPhong = txtTenPhong.Text.Trim();
            p.MoTa = txtMoTa.Text.Trim();

            if (_svc.Update(p))
            {
                var detail = ChangeFormatter.BuildPhongDiff(oldP, p);
                LichSuThayDoiLogger.LogPhong(p.MaPhong, "Cập nhật", detail);
                LoadPhong();
            }
            else
            {
                MessageBox.Show("Cập nhật phòng thất bại.", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvPhong.CurrentRow == null) return;
            var p = (PhongHoc)dgvPhong.CurrentRow.DataBoundItem;

            int cnt = _svc.CountDevicesInRoom(p.MaPhong);
            if (cnt > 0)
            {
                MessageBox.Show(
                    $"Phòng đang có {cnt} thiết bị. Bạn cần chuyển thiết bị trước khi xoá.",
                    "Không thể xoá", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Xác nhận xoá phòng {p.TenPhong}?",
                                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes)
            {
                if (_svc.Delete(p.MaPhong))
                {
                    LichSuThayDoiLogger.LogPhong(p.MaPhong, "Xoá", ChangeFormatter.BuildPhongDelete(p));
                    LoadPhong();
                }
                else
                {
                    MessageBox.Show("Xoá phòng thất bại.", "Lỗi",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        // Expose để frmMain kiểm tra / điều khiển
        public bool IsDirty => _isDirty;

        public void SaveChanges()
        {
            if (dgvPhong.CurrentRow != null)
                btnUpdate_Click(this, EventArgs.Empty);
            else
                btnAdd_Click(this, EventArgs.Empty);
        }

        public void DiscardChanges()
        {
            LoadPhong();
        }
    }
}
