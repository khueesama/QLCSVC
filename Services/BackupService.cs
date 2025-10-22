using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.IO.Compression;

namespace QLCSVCWinApp.Services
{
    public static class BackupService
    {
        private static readonly string ConnStr =
            ConfigurationManager.ConnectionStrings["connString"].ConnectionString;

        // Lấy tên DB từ chuỗi kết nối (Initial Catalog)
        private static string GetDatabaseName()
        {
            var builder = new SqlConnectionStringBuilder(ConnStr);
            return builder.InitialCatalog;
        }

        public static string BackupOnce(string targetFolder, bool zipAfter = false)
        {
            if (string.IsNullOrWhiteSpace(targetFolder))
                throw new ArgumentException("Backup folder is empty.", nameof(targetFolder));

            Directory.CreateDirectory(targetFolder);

            string dbName = GetDatabaseName();
            string ts = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            string bakPath = Path.Combine(targetFolder, $"{dbName}_{ts}.bak");

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand($@"
                BACKUP DATABASE [{dbName}]
                TO DISK = @path
                WITH INIT, COMPRESSION;", conn))
            {
                cmd.Parameters.AddWithValue("@path", bakPath);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            if (!zipAfter) return bakPath;

            string zipPath = Path.ChangeExtension(bakPath, ".zip");
            using (var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(bakPath, Path.GetFileName(bakPath), CompressionLevel.Optimal);
            }
            File.Delete(bakPath);
            return zipPath;
        }
    }
}
