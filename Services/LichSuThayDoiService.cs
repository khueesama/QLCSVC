// File: Services/LichSuThayDoiService.cs
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using QLCSVCWinApp.Models;

namespace QLCSVCWinApp.Services
{
    public class LichSuThayDoiService
    {
        private readonly string connStr =
            ConfigurationManager.ConnectionStrings["connString"].ConnectionString;

        /// <summary>
        /// Ghi một bản ghi lịch sử thay đổi.
        /// </summary>
        public void Ghi(string module, string? doiTuong, string hanhDong, string noiDung, string nguoiDung)
        {
            // Đổi "dbo.lichsuthaydoi" nếu bảng của bạn đặt tên khác (ví dụ: dbo.LichSuThayDoi)
            const string sql = @"
                INSERT INTO dbo.lichsuthaydoi (module, doiTuong, hanhDong, thoiGian, nguoiDung, noiDung)
                VALUES (@m, @d, @h, GETDATE(), @u, @n)";
            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@m", module);
            cmd.Parameters.AddWithValue("@d", (object?)doiTuong ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@h", hanhDong);
            cmd.Parameters.AddWithValue("@u", nguoiDung);   // tên đăng nhập hiện tại (vd: "khue")
            cmd.Parameters.AddWithValue("@n", noiDung);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Tìm kiếm lịch sử thay đổi theo nhiều tiêu chí (tham số có thể null).
        /// </summary>
        public List<LichSuThayDoi> Search(
            string? module, string? doiTuong, string? hanhDong,
            string? nguoiDung, DateTime? from, DateTime? to)
        {
            var result = new List<LichSuThayDoi>();

            var sql = @"
SELECT maLichSu, module, doiTuong, hanhDong, thoiGian, nguoiDung, noiDung
FROM dbo.lichsuthaydoi
WHERE 1=1";
            var prms = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(module))
            {
                sql += " AND module = @m";
                prms.Add(new SqlParameter("@m", module));
            }
            if (!string.IsNullOrWhiteSpace(doiTuong))
            {
                sql += " AND doiTuong LIKE @d";
                prms.Add(new SqlParameter("@d", "%" + doiTuong.Trim() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(hanhDong))
            {
                sql += " AND hanhDong = @h";
                prms.Add(new SqlParameter("@h", hanhDong));
            }
            if (!string.IsNullOrWhiteSpace(nguoiDung))
            {
                sql += " AND nguoiDung LIKE @u";
                prms.Add(new SqlParameter("@u", "%" + nguoiDung.Trim() + "%"));
            }
            if (from.HasValue)
            {
                sql += " AND thoiGian >= @from";
                prms.Add(new SqlParameter("@from", from.Value));
            }
            if (to.HasValue)
            {
                sql += " AND thoiGian <= @to";
                prms.Add(new SqlParameter("@to", to.Value));
            }

            sql += " ORDER BY thoiGian DESC";

            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(sql, conn);
            if (prms.Count > 0) cmd.Parameters.AddRange(prms.ToArray());
            conn.Open();

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                result.Add(new LichSuThayDoi
                {
                    MaLichSu = r.GetString(0),
                    Module = r.GetString(1),
                    DoiTuong = r.IsDBNull(2) ? null : r.GetString(2),
                    HanhDong = r.GetString(3),
                    ThoiGian = r.GetDateTime(4),
                    NguoiDung = r.GetString(5),
                    NoiDung = r.GetString(6)
                });
            }

            return result;
        }

        /// <summary>
        /// Lấy tất cả lịch sử (tiện khi cần nạp mặc định).
        /// </summary>
        public List<LichSuThayDoi> GetAll() => Search(null, null, null, null, null, null);
    }
}
