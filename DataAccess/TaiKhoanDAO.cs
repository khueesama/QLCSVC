using System;
using System.Configuration;
using System.Data.SqlClient;
using BCrypt.Net; // cần cài BCrypt.Net-Next

namespace QLCSVCWinApp.DataAccess
{
    public class TaiKhoanDAO
    {
        private readonly string connStr =
            ConfigurationManager.ConnectionStrings["connString"].ConnectionString;

        private static bool ToBoolAffected(int n) => n > 0;

        // ====== Core APIs ======

        /// Kiểm tra đăng nhập (hash password), chỉ cho phép trangThai = 'Hoạt động'
        public bool CheckLogin(string tenDangNhap, string matKhauNhap)
        {
            const string sql = @"
                SELECT TOP(1) matKhau
                FROM taikhoan
                WHERE tenDangNhap = @u AND trangThai = N'Hoạt động'";

            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@u", tenDangNhap);
            conn.Open();

            var hash = cmd.ExecuteScalar() as string;
            if (string.IsNullOrEmpty(hash)) return false;

            // So sánh mật khẩu nhập với hash trong DB
            return BCrypt.Net.BCrypt.Verify(matKhauNhap, hash);
        }

        public bool KiemTraEmailTonTai(string email)
        {
            const string sql = "SELECT COUNT(*) FROM taikhoan WHERE email = @e";
            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@e", email);
            conn.Open();
            return (int)cmd.ExecuteScalar() > 0;
        }

        public bool KiemTraTenDangNhapTonTai(string tenDangNhap)
        {
            const string sql = "SELECT COUNT(*) FROM taikhoan WHERE tenDangNhap = @u";
            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@u", tenDangNhap);
            conn.Open();
            return (int)cmd.ExecuteScalar() > 0;
        }

        public (string? MaNguoiDung, string? TenDangNhap, string? Email, string? TrangThai)? GetByTenDangNhap(string tenDangNhap)
        {
            const string sql = @"
                SELECT TOP(1) maNguoiDung, tenDangNhap, email, trangThai
                FROM taikhoan WHERE tenDangNhap = @u";

            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@u", tenDangNhap);
            conn.Open();

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return (r.GetString(0), r.GetString(1), r.GetString(2), r.GetString(3));
        }

        public bool ExistsById(string maNguoiDung)
        {
            const string sql = "SELECT COUNT(*) FROM taikhoan WHERE maNguoiDung = @id";
            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", maNguoiDung);
            conn.Open();
            return (int)cmd.ExecuteScalar() > 0;
        }

        public bool DangKyTaiKhoan(string tenDangNhap, string email, string matKhau, string trangThai = "Hoạt động")
        {
            // Hash mật khẩu trước khi lưu
            string hash = BCrypt.Net.BCrypt.HashPassword(matKhau);

            const string sql = @"
                INSERT INTO taikhoan (tenDangNhap, matKhau, email, trangThai)
                VALUES (@u, @p, @e, @s)";

            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@u", tenDangNhap);
            cmd.Parameters.AddWithValue("@p", hash);
            cmd.Parameters.AddWithValue("@e", email);
            cmd.Parameters.AddWithValue("@s", trangThai);
            conn.Open();
            return ToBoolAffected(cmd.ExecuteNonQuery());
        }

        public string DangKyTaiKhoan_GetId(string tenDangNhap, string email, string matKhau, string trangThai = "Hoạt động")
        {
            string hash = BCrypt.Net.BCrypt.HashPassword(matKhau);

            const string sql = @"
                INSERT INTO taikhoan (tenDangNhap, matKhau, email, trangThai)
                OUTPUT inserted.maNguoiDung
                VALUES (@u, @p, @e, @s)";

            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@u", tenDangNhap);
            cmd.Parameters.AddWithValue("@p", hash);
            cmd.Parameters.AddWithValue("@e", email);
            cmd.Parameters.AddWithValue("@s", trangThai);
            conn.Open();
            return (string)cmd.ExecuteScalar();
        }

        public bool CapNhatMatKhau_ByEmail(string email, string matKhauMoi)
        {
            string hash = BCrypt.Net.BCrypt.HashPassword(matKhauMoi);

            const string sql = "UPDATE taikhoan SET matKhau = @p WHERE email = @e";
            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@p", hash);
            cmd.Parameters.AddWithValue("@e", email);
            conn.Open();
            return ToBoolAffected(cmd.ExecuteNonQuery());
        }

        public bool CapNhatMatKhau_ByUser(string tenDangNhap, string matKhauMoi)
        {
            string hash = BCrypt.Net.BCrypt.HashPassword(matKhauMoi);

            const string sql = "UPDATE taikhoan SET matKhau = @p WHERE tenDangNhap = @u";
            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@p", hash);
            cmd.Parameters.AddWithValue("@u", tenDangNhap);
            conn.Open();
            return ToBoolAffected(cmd.ExecuteNonQuery());
        }

        public bool CapNhatTrangThai(string maNguoiDung, string trangThai)
        {
            const string sql = "UPDATE taikhoan SET trangThai = @s WHERE maNguoiDung = @id";
            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@s", trangThai);
            cmd.Parameters.AddWithValue("@id", maNguoiDung);
            conn.Open();
            return ToBoolAffected(cmd.ExecuteNonQuery());
        }

        // ====== Compat wrappers (để AuthService không lỗi) ======

        public bool KiemTraDangNhap(string tenDangNhap, string matKhau)
            => CheckLogin(tenDangNhap, matKhau);

        public bool CapNhatMatKhau(string email, string matKhauMoi)
            => CapNhatMatKhau_ByEmail(email, matKhauMoi);

        public bool KiemTraEmail(string email)
            => KiemTraEmailTonTai(email);

        public bool DangKy(string tenDangNhap, string email, string matKhau)
            => DangKyTaiKhoan(tenDangNhap, email, matKhau);
    }
}
