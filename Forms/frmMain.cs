using QLCSVCWinApp.Services;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace QLCSVCWinApp.Forms
{
    public partial class frmMain : Form
    {
        private static readonly string BannerPath = Path.Combine(Application.StartupPath, "image", "banner.png");
        private static readonly string HomeBackgroundPath = Path.Combine(Application.StartupPath, "image", "bd.jpg");
        private static readonly string IconDevice = Path.Combine(Application.StartupPath, "image", "device.png");
        private static readonly string IconRoom = Path.Combine(Application.StartupPath, "image", "occupancy.png");
        private static readonly string IconReport = Path.Combine(Application.StartupPath, "image", "report.png");
        private static readonly string IconHistory = Path.Combine(Application.StartupPath, "image", "history.png");
        private static readonly string IconBackup = Path.Combine(Application.StartupPath, "image", "backup.png");
        private static readonly string IconRestore = Path.Combine(Application.StartupPath, "image", "restore.png");
        private static readonly string IconLogout = Path.Combine(Application.StartupPath, "image", "logout.png");

        private bool _isLogout = false;
        public bool IsExitApp { get; private set; } = false;

        private Panel? _pnlHeaderText;
        private Label? _lblBannerTitle;

        private enum Module { None = 0, ThietBi, Phong, BaoCao, LichSu, SaoLuu, PhucHoi }
        private Module _current = Module.None;

        // Theme menu
        private readonly Color _menuNormal = Color.FromArgb(245, 248, 252);
        private readonly Color _menuHover = Color.FromArgb(232, 241, 250);
        private readonly Color _menuActive = Color.FromArgb(220, 235, 248);
        private Button? _activeBtn;

        public frmMain()
        {
            InitializeComponent();

            this.FormClosing += frmMain_FormClosing;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            lblChao.Text = "";
            ApplyTheme();
            EnsureHeaderTextBar();
            ShowHome();

            try { BackupScheduler.StartIfEnabled(); } catch { }
        }

        private void ApplyTheme()
        {
            if (File.Exists(BannerPath))
            {
                try { pbHeader.Image = Image.FromFile(BannerPath); } catch { }
                pbHeader.SizeMode = PictureBoxSizeMode.Zoom;
            }

            if (File.Exists(HomeBackgroundPath))
            {
                try
                {
                    pnlBody.BackgroundImage = Image.FromFile(HomeBackgroundPath);
                    pnlBody.BackgroundImageLayout = ImageLayout.Stretch;
                }
                catch { }
            }

            TrySetButtonIcon(btnQLThietBi, IconDevice);
            TrySetButtonIcon(btnQLPhong, IconRoom);
            TrySetButtonIcon(btnBaoCao, IconReport);
            TrySetButtonIcon(btnLichSu, IconHistory);
            TrySetButtonIcon(btnSaoLuu, IconBackup);
            TrySetButtonIcon(btnPhucHoi, IconRestore);
            TrySetButtonIcon(btnDangXuat, IconLogout);

            foreach (Control c in panelMenu.Controls)
            {
                if (c is Button b && b.Image != null)
                {
                    b.ImageAlign = ContentAlignment.MiddleLeft;
                    b.TextImageRelation = TextImageRelation.ImageBeforeText;
                }
            }

            WireMenuHover();
        }

        private void TrySetButtonIcon(Button btn, string path, int size = 18)
        {
            try
            {
                if (!File.Exists(path)) return;
                using var img = Image.FromFile(path);
                btn.Image = new Bitmap(img, new Size(size, size));
            }
            catch { }
        }

        private void EnsureHeaderTextBar()
        {
            if (_pnlHeaderText != null) return;

            Control host = panelHeader;
            _pnlHeaderText = new Panel
            {
                Dock = DockStyle.Top,
                Height = 38,
                BackColor = pbHeader.BackColor
            };
            host.Controls.Add(_pnlHeaderText);
            pbHeader.BringToFront();

            _lblBannerTitle = new Label
            {
                Text = "TRUNG TÂM NGOẠI NGỮ - TIN HỌC",
                AutoSize = true,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(30, 60, 120),
                Font = new Font("Segoe UI", 16f, FontStyle.Bold)
            };

            _pnlHeaderText.Controls.Add(_lblBannerTitle);
            _pnlHeaderText.Resize += (_, __) => CenterHeaderTitle();
            CenterHeaderTitle();
        }

        private void CenterHeaderTitle()
        {
            if (_pnlHeaderText == null || _lblBannerTitle == null) return;
            int x = Math.Max(0, (_pnlHeaderText.Width - _lblBannerTitle.Width) / 2);
            int y = Math.Max(0, (_pnlHeaderText.Height - _lblBannerTitle.Height) / 2);
            _lblBannerTitle.Location = new Point(x, y);
        }

        // ===== Menu Highlight =====
        private void Highlight(Button? target)
        {
            foreach (Control c in panelMenu.Controls)
                if (c is Button b) b.BackColor = _menuNormal;

            if (target != null)
            {
                target.BackColor = _menuActive;
                _activeBtn = target;
            }
            else
            {
                _activeBtn = null;
            }
        }

        private void WireMenuHover()
        {
            foreach (Control c in panelMenu.Controls)
            {
                if (c is Button b)
                {
                    b.MouseEnter += (_, __) => { if (_activeBtn != b) b.BackColor = _menuHover; };
                    b.MouseLeave += (_, __) => { if (_activeBtn != b) b.BackColor = _menuNormal; };
                }
            }
        }

        // ===== Logic =====
        private bool PromptSaveIfNeeded()
        {
            if (panelMain.Controls.Count == 0) return true;
            var ctrl = panelMain.Controls[0];

            if (ctrl is frmQuanLyPhong ph && ph.IsDirty)
            {
                var dr = MessageBox.Show("Bạn chưa lưu thông tin phòng! Lưu trước khi chuyển?",
                                         "Chưa lưu", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes) ph.SaveChanges();
                else if (dr == DialogResult.No) ph.DiscardChanges();
                else return false;
            }
            if (ctrl is frmQuanLyThietBi tb && tb.IsDirty)
            {
                var dr = MessageBox.Show("Bạn chưa lưu thông tin thiết bị! Lưu trước khi chuyển?",
                                         "Chưa lưu", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes) tb.SaveChanges();
                else if (dr == DialogResult.No) tb.DiscardChanges();
                else return false;
            }
            return true;
        }

        private void OpenChildForm(Form childForm, Module target)
        {
            if (!PromptSaveIfNeeded()) return;

            panelMain.Controls.Clear();
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panelMain.Controls.Add(childForm);
            panelMain.Tag = childForm;
            childForm.Show();

            _current = target;
        }

        private void ShowHome()
        {
            panelMain.Controls.Clear();
            _current = Module.None;
        }

        // ===== Events =====
        private void btnQLThietBi_Click(object sender, EventArgs e)
        {
            if (_current == Module.ThietBi) { if (!PromptSaveIfNeeded()) return; ShowHome(); Highlight(null); }
            else { OpenChildForm(new frmQuanLyThietBi(), Module.ThietBi); Highlight(btnQLThietBi); }
        }

        private void btnQLPhong_Click(object sender, EventArgs e)
        {
            if (_current == Module.Phong) { if (!PromptSaveIfNeeded()) return; ShowHome(); Highlight(null); }
            else { OpenChildForm(new frmQuanLyPhong(), Module.Phong); Highlight(btnQLPhong); }
        }

        private void btnBaoCao_Click(object sender, EventArgs e)
        {
            if (_current == Module.BaoCao) { if (!PromptSaveIfNeeded()) return; ShowHome(); Highlight(null); }
            else { OpenChildForm(new frmBaoCao(), Module.BaoCao); Highlight(btnBaoCao); }
        }

        private void btnLichSu_Click(object sender, EventArgs e)
        {
            if (_current == Module.LichSu) { if (!PromptSaveIfNeeded()) return; ShowHome(); Highlight(null); }
            else { OpenChildForm(new frmLichSuThayDoi(), Module.LichSu); Highlight(btnLichSu); }
        }

        private void btnSaoLuu_Click(object sender, EventArgs e)
        {
            if (_current == Module.SaoLuu) { if (!PromptSaveIfNeeded()) return; ShowHome(); Highlight(null); }
            else { OpenChildForm(new frmSaoLuu(), Module.SaoLuu); Highlight(btnSaoLuu); }
        }

        private void btnPhucHoi_Click(object sender, EventArgs e)
        {
            if (_current == Module.PhucHoi) { if (!PromptSaveIfNeeded()) return; ShowHome(); Highlight(null); }
            else { OpenChildForm(new frmPhucHoi(), Module.PhucHoi); Highlight(btnPhucHoi); }
        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            if (!PromptSaveIfNeeded()) return;
            if (MessageBox.Show("Bạn muốn đăng xuất?", "Xác nhận",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _isLogout = true;
                this.Close();
            }
        }

        private void frmMain_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (_isLogout) return;
                var dr = MessageBox.Show("Bạn có chắc chắn muốn thoát ứng dụng?",
                                         "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.No) { e.Cancel = true; return; }
                IsExitApp = true;
            }

            try { BackupScheduler.Stop().GetAwaiter().GetResult(); } catch { }
        }
    }
}
