using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using QLCSVCWinApp.Models;

namespace QLCSVCWinApp.DataAccess
{
    public class LichSuThayDoiDAO
    {
        private readonly string connStr =
            ConfigurationManager.ConnectionStrings["connString"].ConnectionString;

        public void Ghi(string module, string? doiTuong, string hanhDong, string noiDung, string nguoiDung)
        {
            const string sql = @"
                INSERT INTO lichsuthaydoi([module], doiTuong, hanhDong, thoiGian, nguoiDung, noiDung)
                VALUES(@m, @obj, @act, GETDATE(), @user, @nd)";
            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@m", module);
            cmd.Parameters.AddWithValue("@obj", (object?)doiTuong ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@act", hanhDong);
            cmd.Parameters.AddWithValue("@user", nguoiDung);
            cmd.Parameters.AddWithValue("@nd", noiDung);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public List<LichSuThayDoi> Search(string? module, string? doiTuong, string? hanhDong,
                                          string? nguoiDung, DateTime? from, DateTime? to)
        {
            var list = new List<LichSuThayDoi>();
            var sql = @"SELECT maLichSu, [module], doiTuong, hanhDong, thoiGian, nguoiDung, noiDung
                        FROM lichsuthaydoi WHERE 1=1";
            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand { Connection = conn };

            if (!string.IsNullOrWhiteSpace(module))
            {
                sql += " AND [module] = @m"; cmd.Parameters.AddWithValue("@m", module);
            }
            if (!string.IsNullOrWhiteSpace(doiTuong))
            {
                sql += " AND doiTuong LIKE @obj"; cmd.Parameters.AddWithValue("@obj", "%" + doiTuong.Trim() + "%");
            }
            if (!string.IsNullOrWhiteSpace(hanhDong))
            {
                sql += " AND hanhDong = @act"; cmd.Parameters.AddWithValue("@act", hanhDong);
            }
            if (!string.IsNullOrWhiteSpace(nguoiDung))
            {
                sql += " AND nguoiDung LIKE @u"; cmd.Parameters.AddWithValue("@u", "%" + nguoiDung.Trim() + "%");
            }
            if (from.HasValue)
            {
                sql += " AND thoiGian >= @from"; cmd.Parameters.AddWithValue("@from", from.Value);
            }
            if (to.HasValue)
            {
                sql += " AND thoiGian <= @to"; cmd.Parameters.AddWithValue("@to", to.Value);
            }
            sql += " ORDER BY thoiGian DESC";
            cmd.CommandText = sql;

            conn.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new LichSuThayDoi
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
            return list;
        }
    }
}
