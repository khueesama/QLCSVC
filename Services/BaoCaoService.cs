using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace QLCSVCWinApp.Services
{
    public class BaoCaoService
    {
        private readonly string _connStr =
            ConfigurationManager.ConnectionStrings["connString"].ConnectionString;

        // 2.3.8.1 Thiết bị theo phòng (có thể chọn 1 hoặc nhiều phòng; null => tất cả)
        public DataTable ThietBiTheoPhong(string[]? dsMaPhong)
        {
            var sql = @"
                SELECT t.maThietBi   AS [Mã TB],
                       t.tenThietBi  AS [Tên TB],
                       t.loaiThietBi AS [Loại],
                       t.ngayMua     AS [Ngày mua],
                       t.tinhTrang   AS [Tình trạng],
                       t.ghiChu      AS [Ghi chú],
                       t.maPhong     AS [Mã phòng],
                       p.tenPhong    AS [Tên phòng]
                FROM thietbi t
                LEFT JOIN phonghoc p ON p.maPhong=t.maPhong
                WHERE 1=1";

            var dt = new DataTable();
            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand();
            cmd.Connection = conn;

            if (dsMaPhong != null && dsMaPhong.Length > 0)
            {
                // tạo IN (@p0,@p1,…)
                var inNames = new string[dsMaPhong.Length];
                for (int i = 0; i < dsMaPhong.Length; i++)
                {
                    var p = "@p" + i;
                    inNames[i] = p;
                    cmd.Parameters.AddWithValue(p, dsMaPhong[i]);
                }
                sql += $" AND t.maPhong IN ({string.Join(",", inNames)})";
            }

            sql += " ORDER BY p.tenPhong, t.maThietBi";
            cmd.CommandText = sql;

            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        // 2.3.8.2 Tổng hợp theo loại + thống kê tình trạng
        public DataTable TongHopTheoLoai()
        {
            var sql = @"
                SELECT loaiThietBi AS [Loại],
                       COUNT(*)    AS [Tổng số],
                       SUM(CASE WHEN tinhTrang=N'Đang sử dụng' THEN 1 ELSE 0 END) AS [Đang sử dụng],
                       SUM(CASE WHEN tinhTrang=N'Đang sửa'     THEN 1 ELSE 0 END) AS [Đang sửa],
                       SUM(CASE WHEN tinhTrang=N'Hỏng'         THEN 1 ELSE 0 END) AS [Hỏng],
                       SUM(CASE WHEN tinhTrang=N'Thanh lý'     THEN 1 ELSE 0 END) AS [Thanh lý]
                FROM thietbi
                GROUP BY loaiThietBi
                ORDER BY loaiThietBi";
            var dt = new DataTable();
            using var da = new SqlDataAdapter(sql, _connStr);
            da.Fill(dt);
            return dt;
        }

        // 2.3.8.3 TB hỏng/đã thanh lý – có lọc thời gian
        public DataTable ThietBiCanBaoTri(DateTime? from, DateTime? to)
        {
            var sql = @"
                SELECT maThietBi   AS [Mã TB],
                       tenThietBi  AS [Tên TB],
                       loaiThietBi AS [Loại],
                       ngayMua     AS [Ngày mua],
                       tinhTrang   AS [Tình trạng],
                       ghiChu      AS [Ghi chú],
                       maPhong     AS [Mã phòng]
                FROM thietbi
                WHERE tinhTrang IN (N'Hỏng', N'Thanh lý')";

            var dt = new DataTable();
            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(sql, conn);

            if (from.HasValue)
            {
                cmd.CommandText += " AND ngayMua >= @from";
                cmd.Parameters.AddWithValue("@from", from.Value.Date);
            }
            if (to.HasValue)
            {
                cmd.CommandText += " AND ngayMua <= @to";
                cmd.Parameters.AddWithValue("@to", to.Value.Date);
            }
            cmd.CommandText += " ORDER BY ngayMua DESC";

            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        // 2.3.8.4 Lịch sử thay đổi – có bộ lọc giống màn Lịch sử
        public DataTable LichSu(string? module, string? doiTuong, string? hanhDong,
                                string? nguoiDung, DateTime? from, DateTime? to)
        {
            var dt = new DataTable();
            var sql = @"
                SELECT MaLichSu  AS [Mã LS],
                       Module    AS [Module],
                       DoiTuong  AS [Đối tượng],
                       HanhDong  AS [Hành động],
                       ThoiGian  AS [Thời gian],
                       NguoiDung AS [Người dùng],
                       NoiDung   AS [Nội dung]
                FROM LichSuThayDoi
                WHERE 1=1";

            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand();
            cmd.Connection = conn;

            void P(string n, object v) => cmd.Parameters.AddWithValue(n, v);

            if (!string.IsNullOrWhiteSpace(module)) { sql += " AND Module=@m"; P("@m", module); }
            if (!string.IsNullOrWhiteSpace(doiTuong)) { sql += " AND DoiTuong LIKE @dt"; P("@dt", "%" + doiTuong.Trim() + "%"); }
            if (!string.IsNullOrWhiteSpace(hanhDong)) { sql += " AND HanhDong=@h"; P("@h", hanhDong); }
            if (!string.IsNullOrWhiteSpace(nguoiDung)) { sql += " AND NguoiDung LIKE @u"; P("@u", "%" + nguoiDung.Trim() + "%"); }
            if (from.HasValue) { sql += " AND ThoiGian >= @f"; P("@f", from.Value); }
            if (to.HasValue) { sql += " AND ThoiGian <= @t"; P("@t", to.Value); }

            sql += " ORDER BY ThoiGian DESC";
            cmd.CommandText = sql;

            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }
    }
}
