using QLCSVCWinApp.DataAccess;
using QLCSVCWinApp.Utils;

namespace QLCSVCWinApp.Services
{
    public class AuthService
    {
        private readonly TaiKhoanDAO _dao = new TaiKhoanDAO();

        /// Đăng nhập: so sánh user + mật khẩu (DAO đã dùng BCrypt.Verify)
        public bool CheckLogin(string username, string password)
        {
            var ok = _dao.KiemTraDangNhap(username, password);
            if (!ok) return false;

            // Lấy thông tin tài khoản và set vào CurrentUser
            var info = _dao.GetByTenDangNhap(username);
            if (info.HasValue)
            {
                CurrentUser.MaNguoiDung = info.Value.MaNguoiDung ?? "";
                CurrentUser.TenDangNhap = info.Value.TenDangNhap ?? "";
                CurrentUser.Email = info.Value.Email ?? "";
            }
            else
            {
                CurrentUser.MaNguoiDung = "";
                CurrentUser.TenDangNhap = "";
                CurrentUser.Email = "";
            }
            return true;
        }

        /// Đăng ký (DAO sẽ Hash mật khẩu trước khi lưu)
        public bool DangKy(string username, string email, string password)
            => _dao.DangKyTaiKhoan(username, email, password);

        /// Email đã tồn tại?
        public bool KiemTraEmail(string email)
            => _dao.KiemTraEmailTonTai(email);

        /// Tên đăng nhập đã tồn tại?
        public bool KiemTraTenDangNhap(string username)
            => _dao.KiemTraTenDangNhapTonTai(username);

        /// Cập nhật mật khẩu (DAO sẽ Hash)
        public void CapNhatMatKhau(string email, string newPassword)
            => _dao.CapNhatMatKhau_ByEmail(email, newPassword);
    }
}
