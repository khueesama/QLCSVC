using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using QLCSVCWinApp.Models;

namespace QLCSVCWinApp.DataAccess
{
    public class ThietBiDAO
    {
        
        private readonly string connStr =
            ConfigurationManager.ConnectionStrings["connString"].ConnectionString;

        private static string RequireNonEmpty(string? value, string fieldLabel)
        {
            var v = value?.Trim();
            if (string.IsNullOrEmpty(v))
                throw new ArgumentException($"{fieldLabel} không được để trống.");
            return v;
        }
        public int CountByRoom(string maPhong)
        {
            const string sql = "SELECT COUNT(*) FROM thietbi WHERE maPhong = @ph";
            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ph", maPhong);
            conn.Open();
            return (int)cmd.ExecuteScalar();
        }
        public List<ThietBi> GetAll()
        {
            var list = new List<ThietBi>();
            const string sql = @"
                SELECT t.maThietBi, t.loaiThietBi, t.tenThietBi, t.ngayMua, t.tinhTrang, 
                       t.thongTin, t.ghiChu, t.maPhong, p.tenPhong
                FROM thietbi t
                LEFT JOIN phonghoc p ON p.maPhong = t.maPhong
                ORDER BY t.maThietBi DESC";
            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(sql, conn);
            conn.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new ThietBi
                {
                    MaThietBi = r.GetString(0),
                    LoaiThietBi = r.GetString(1),
                    TenThietBi = r.GetString(2),
                    NgayMua = r.GetDateTime(3),
                    TinhTrang = r.GetString(4),
                    ThongTin = r.GetString(5),
                    GhiChu = r.IsDBNull(6) ? null : r.GetString(6),
                    MaPhong = r.IsDBNull(7) ? null : r.GetString(7),
                    TenPhong = r.IsDBNull(8) ? null : r.GetString(8) // NEW
                });
            }
            return list;
        }

        // Thêm không cần truyền mã – trigger sẽ sinh.
        public bool Add(ThietBi tb)
        {
            // Bắt buộc
            var ten = RequireNonEmpty(tb.TenThietBi, "Tên thiết bị");
            var info = RequireNonEmpty(tb.ThongTin, "Thông tin thiết bị");

            const string sql = @"
        INSERT INTO thietbi
            (loaiThietBi, tenThietBi, ngayMua, tinhTrang, thongTin, ghiChu, maPhong)
        VALUES
            (@loai, @ten, @ngay, @tt, @info, @ghiChu, @phong)";

            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@loai", tb.LoaiThietBi);                 // có thể để mặc định từ UI
            cmd.Parameters.AddWithValue("@ten", ten);
            cmd.Parameters.AddWithValue("@ngay", (object?)tb.NgayMua ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@tt", tb.TinhTrang ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@info", info);
            cmd.Parameters.AddWithValue("@ghiChu", (object?)tb.GhiChu?.Trim() ?? DBNull.Value);
            cmd.Parameters.Add("@phong", SqlDbType.Char, 6).Value =
                tb.MaPhong is null ? (object)DBNull.Value : tb.MaPhong;

            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }


        // Nếu cần lấy ID mới tạo
        public string AddAndGetId(ThietBi tb)
        {
            var ten = RequireNonEmpty(tb.TenThietBi, "Tên thiết bị");
            var info = RequireNonEmpty(tb.ThongTin, "Thông tin thiết bị");

            const string sql = @"
        INSERT INTO thietbi
            (loaiThietBi, tenThietBi, ngayMua, tinhTrang, thongTin, ghiChu, maPhong)
        VALUES
            (@loai, @ten, @ngay, @tt, @info, @ghiChu, @phong);

        SELECT TOP(1) maThietBi
        FROM thietbi
        WHERE tenThietBi=@ten AND loaiThietBi=@loai
        ORDER BY ngayMua DESC, maThietBi DESC;";

            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@loai", tb.LoaiThietBi);
            cmd.Parameters.AddWithValue("@ten", ten);
            cmd.Parameters.AddWithValue("@ngay", (object?)tb.NgayMua ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@tt", tb.TinhTrang ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@info", info);
            cmd.Parameters.AddWithValue("@ghiChu", (object?)tb.GhiChu?.Trim() ?? DBNull.Value);
            cmd.Parameters.Add("@phong", SqlDbType.Char, 6).Value =
                tb.MaPhong is null ? (object)DBNull.Value : tb.MaPhong;

            conn.Open();
            var result = cmd.ExecuteScalar();
            return result?.ToString() ?? string.Empty;
        }




        public bool Update(ThietBi tb)
        {
            var ten = RequireNonEmpty(tb.TenThietBi, "Tên thiết bị");
            var info = RequireNonEmpty(tb.ThongTin, "Thông tin thiết bị");

            const string sql = @"
        UPDATE thietbi
           SET loaiThietBi=@loai, tenThietBi=@ten, ngayMua=@ngay,
               tinhTrang=@tt, thongTin=@info, ghiChu=@ghiChu, maPhong=@phong
         WHERE maThietBi=@id";

            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", tb.MaThietBi);
            cmd.Parameters.AddWithValue("@loai", tb.LoaiThietBi);
            cmd.Parameters.AddWithValue("@ten", ten);
            cmd.Parameters.AddWithValue("@ngay", (object?)tb.NgayMua ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@tt", tb.TinhTrang ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@info", info);
            cmd.Parameters.AddWithValue("@ghiChu", (object?)tb.GhiChu?.Trim() ?? DBNull.Value);
            cmd.Parameters.Add("@phong", SqlDbType.Char, 6).Value =
                tb.MaPhong is null ? (object)DBNull.Value : tb.MaPhong;

            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }


        public bool Delete(string maThietBi)
        {
            const string sql = "DELETE FROM thietbi WHERE maThietBi=@id";
            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", maThietBi);
            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        // Tìm kiếm theo nhiều tiêu chí (mọi tham số đều optional)
        public List<ThietBi> Search(string? keyword, string? loai, string? tinhTrang,
                            string? maPhong, DateTime? from, DateTime? to)
        {
            var list = new List<ThietBi>();
            var sql = @"
        SELECT t.maThietBi, t.loaiThietBi, t.tenThietBi, t.ngayMua, t.tinhTrang,
               t.thongTin, t.ghiChu, t.maPhong, p.tenPhong
        FROM thietbi t
        LEFT JOIN phonghoc p ON p.maPhong = t.maPhong
        WHERE 1=1";

            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand();
            cmd.Connection = conn;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sql += " AND (t.tenThietBi LIKE @kw OR t.thongTin LIKE @kw OR t.ghiChu LIKE @kw)";
                cmd.Parameters.AddWithValue("@kw", "%" + keyword.Trim() + "%");
            }
            if (!string.IsNullOrWhiteSpace(loai))
            {
                sql += " AND t.loaiThietBi = @loai";
                cmd.Parameters.AddWithValue("@loai", loai);
            }
            if (!string.IsNullOrWhiteSpace(tinhTrang))
            {
                sql += " AND t.tinhTrang = @tt";
                cmd.Parameters.AddWithValue("@tt", tinhTrang);
            }

            // --- lọc phòng: null = (Tất cả), "__NULL__" = (Chưa có phòng), còn lại = mã phòng
            if (maPhong == "__NULL__")
            {
                sql += " AND t.maPhong IS NULL";
            }
            else if (!string.IsNullOrWhiteSpace(maPhong))
            {
                sql += " AND t.maPhong = @ph";
                cmd.Parameters.AddWithValue("@ph", maPhong);
            }

            if (from.HasValue)
            {
                sql += " AND t.ngayMua >= @from";
                cmd.Parameters.AddWithValue("@from", from.Value.Date);
            }
            if (to.HasValue)
            {
                sql += " AND t.ngayMua <= @to";
                cmd.Parameters.AddWithValue("@to", to.Value.Date);
            }

            sql += " ORDER BY t.maThietBi DESC";
            cmd.CommandText = sql;

            conn.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new ThietBi
                {
                    MaThietBi = r.GetString(0),
                    LoaiThietBi = r.GetString(1),
                    TenThietBi = r.GetString(2),
                    NgayMua = r.GetDateTime(3),
                    TinhTrang = r.GetString(4),
                    ThongTin = r.GetString(5),
                    GhiChu = r.IsDBNull(6) ? null : r.GetString(6),
                    MaPhong = r.IsDBNull(7) ? null : r.GetString(7),
                    TenPhong = r.IsDBNull(8) ? null : r.GetString(8)   // <--- QUAN TRỌNG
                });
            }
            return list;
        }
    }
}
