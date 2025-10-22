using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using QLCSVCWinApp.DataAccess.QLCSVCWinApp.Models;
using QLCSVCWinApp.Models;

namespace QLCSVCWinApp.DataAccess
{
    public class PhongHocDAO
    {
        private readonly string connStr =
            ConfigurationManager.ConnectionStrings["connString"].ConnectionString;

        public List<PhongHoc> GetAll()
        {
            var list = new List<PhongHoc>();
            using (var conn = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("SELECT maPhong, tenPhong, moTa FROM phonghoc ORDER BY maPhong", conn))
            {
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new PhongHoc
                        {
                            MaPhong = r.GetString(0),
                            TenPhong = r.GetString(1),
                            MoTa = r.IsDBNull(2) ? "" : r.GetString(2)
                        });
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Thêm phòng và trả về mã phòng mới (PHxxxx) do DB sinh.
        /// </summary>
        public string AddAndGetId(PhongHoc p)
        {
            const string sql = @"
        INSERT INTO dbo.phonghoc(tenPhong, moTa)
        VALUES (@name, @desc);

        -- Lấy mã phòng mới (trigger đã sinh PHxxxxxx)
        SELECT TOP(1) maPhong
        FROM dbo.phonghoc
        WHERE tenPhong = @name
        ORDER BY maPhong DESC;";

            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", p.TenPhong);
            cmd.Parameters.AddWithValue("@desc", (object?)p.MoTa ?? string.Empty);

            conn.Open();
            var result = cmd.ExecuteScalar();
            return result?.ToString() ?? string.Empty;
        }


        /// <summary>
        /// Thêm phòng (trả bool). Nội bộ gọi AddAndGetId.
        /// </summary>
        public bool Add(PhongHoc p)
        {
            var id = AddAndGetId(p);
            return !string.IsNullOrEmpty(id);
        }

        public bool Update(PhongHoc p)
        {
            const string sql =
              "UPDATE phonghoc SET tenPhong = @name, moTa = @desc WHERE maPhong = @id";
            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", p.MaPhong);
            cmd.Parameters.AddWithValue("@name", p.TenPhong);
            cmd.Parameters.AddWithValue("@desc", (object?)p.MoTa ?? string.Empty);
            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(string maPhong)
        {
            const string sql = "DELETE FROM phonghoc WHERE maPhong = @id";
            using var conn = new SqlConnection(connStr);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", maPhong);
            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
