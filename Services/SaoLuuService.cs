using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace QLCSVCWinApp.Services
{
    public class SaoLuuService
    {
        private readonly string _connStr =
            ConfigurationManager.ConnectionStrings["connString"].ConnectionString;

        // Lấy tên database hiện đang kết nối
        private string GetDatabaseName()
        {
            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand("SELECT DB_NAME()", conn);
            conn.Open();
            return (string)cmd.ExecuteScalar();
        }

        // Lấy thư mục backup mặc định của SQL Server (trên máy SQL)
        private string GetDefaultBackupDir()
        {
            var csb = new SqlConnectionStringBuilder(_connStr) { InitialCatalog = "master" };
            using var conn = new SqlConnection(csb.ToString());
            using var cmd = new SqlCommand(@"
                DECLARE @dir NVARCHAR(4000);
                EXEC xp_instance_regread 
                     N'HKEY_LOCAL_MACHINE',
                     N'SOFTWARE\Microsoft\MSSQLServer\MSSQLServer',
                     N'BackupDirectory', @dir OUTPUT;
                SELECT @dir;", conn);
            conn.Open();
            var dir = cmd.ExecuteScalar() as string;
            if (string.IsNullOrWhiteSpace(dir))
                throw new Exception("Không xác định được thư mục backup mặc định của SQL Server.");
            return dir!;
        }

        // (Tuỳ chọn) Phát hiện edition có hỗ trợ COMPRESSION hay không
        private bool IsExpress(SqlConnection? existing = null)
        {
            bool close = false;
            var conn = existing;
            if (conn == null)
            {
                var csb = new SqlConnectionStringBuilder(_connStr) { InitialCatalog = "master" };
                conn = new SqlConnection(csb.ToString());
                conn.Open();
                close = true;
            }
            using var cmd = new SqlCommand(
                "SELECT CAST(CASE WHEN SERVERPROPERTY('Edition') LIKE '%Express%' THEN 1 ELSE 0 END AS bit);",
                conn);
            bool isExpress = (bool)cmd.ExecuteScalar();
            if (close) conn!.Close();
            return isExpress;
        }

        /// <summary>
        /// BACKUP vào thư mục backup mặc định của SQL Server. Chỉ cần truyền TÊN FILE (.bak).
        /// Trả về đường dẫn đầy đủ TRÊN MÁY SQL.
        /// </summary>
        public string BackupTo(string fileNameOnly)
        {
            if (string.IsNullOrWhiteSpace(fileNameOnly))
                throw new ArgumentException("Tên file .bak không hợp lệ.");

            var db = GetDatabaseName();
            var csb = new SqlConnectionStringBuilder(_connStr) { InitialCatalog = "master" };
            var backupDir = GetDefaultBackupDir();
            var serverPath = Path.Combine(backupDir, Path.GetFileName(fileNameOnly));

            // Express KHÔNG hỗ trợ COMPRESSION → giữ WITH INIT, STATS=5 cho mọi edition
            string sql = $@"
                BACKUP DATABASE [{db}]
                TO DISK = @path
                WITH INIT, STATS = 5;";

            using var conn = new SqlConnection(csb.ToString());
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@path", serverPath);
            conn.Open();
            cmd.CommandTimeout = 0;
            cmd.ExecuteNonQuery();

            return serverPath;
        }

        /// <summary> RESTORE từ file .bak. Lưu ý: quyền của dịch vụ SQL và đường dẫn trên MÁY SQL. </summary>
        public void RestoreFrom(string filePathOnServer)
        {
            var db = GetDatabaseName();
            var csb = new SqlConnectionStringBuilder(_connStr) { InitialCatalog = "master" };

            string sql = $@"
                ALTER DATABASE [{db}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                RESTORE DATABASE [{db}]
                FROM DISK = @path
                WITH REPLACE, STATS = 5;
                ALTER DATABASE [{db}] SET MULTI_USER;";

            using var conn = new SqlConnection(csb.ToString());
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@path", filePathOnServer);
            conn.Open();
            cmd.CommandTimeout = 0;
            cmd.ExecuteNonQuery();
        }

        /// <summary> Lấy lịch sử sao lưu từ bảng saoluu (tuỳ bạn có log sẵn hay chưa) </summary>
        public DataTable GetHistory()
        {
            var dt = new DataTable();
            const string sql = @"
                SELECT maSaoLuu       AS [Mã SL],
                       thoiGianSaoLuu  AS [Thời gian],
                       duongDanFile    AS [Đường dẫn],
                       maNguoiDung     AS [Người dùng]
                FROM saoluu
                ORDER BY thoiGianSaoLuu DESC";
            using var da = new SqlDataAdapter(sql, _connStr);
            da.Fill(dt);
            return dt;
        }

        /// <summary> Ghi 1 dòng log vào bảng saoluu </summary>
        public void LogBackup(string pathOnServer, string maNguoiDung)
        {
            const string sql = @"
                INSERT INTO saoluu(thoiGianSaoLuu, duongDanFile, maNguoiDung)
                VALUES (GETDATE(), @p, @u)";
            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@p", pathOnServer);
            cmd.Parameters.AddWithValue("@u", maNguoiDung);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
