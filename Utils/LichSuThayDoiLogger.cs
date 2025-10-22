using QLCSVCWinApp.Services;

namespace QLCSVCWinApp.Utils
{
    public static class LichSuThayDoiLogger
    {
        private static readonly LichSuThayDoiService _svc = new LichSuThayDoiService();

        public static void Log(string module, string? doiTuong, string hanhDong, string noiDung, string? user = null)
        {
            var nguoi = string.IsNullOrWhiteSpace(user)
                ? (Session.CurrentUser ?? "Không xác định")
                : user!;
            _svc.Ghi(module, doiTuong, hanhDong, noiDung, nguoi);
        }

        public static void LogThietBi(string maTB, string hanhDong, string noiDung)
            => Log("Thiết bị", maTB, hanhDong, noiDung);

        public static void LogPhong(string maPhong, string hanhDong, string noiDung)
            => Log("Phòng", maPhong, hanhDong, noiDung);

        public static void LogSaoLuu(string filePath)
            => Log("Sao lưu", filePath, "Sao lưu", $"Tạo bản sao lưu: {filePath}");

        public static void LogPhucHoi(string filePath)
            => Log("Phục hồi", filePath, "Phục hồi", $"Phục hồi từ: {filePath}");
    }

}
