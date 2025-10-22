using System;
using System.Text;
using QLCSVCWinApp.DataAccess.QLCSVCWinApp.Models;
using QLCSVCWinApp.Models;

namespace QLCSVCWinApp.Utils
{
    public static class ChangeFormatter
    {
        private static string Val(object? v) => v == null ? "(null)" : v.ToString()!;

        public static string BuildPhongAdd(PhongHoc p)
            => $"Thêm phòng: Tên='{p.TenPhong}', Mô tả='{Val(p.MoTa)}'.";

        public static string BuildPhongDelete(PhongHoc p)
            => $"Xoá phòng: {p.MaPhong} - '{p.TenPhong}'.";

        public static string BuildPhongDiff(PhongHoc oldP, PhongHoc newP)
        {
            var sb = new StringBuilder($"Cập nhật phòng {oldP.MaPhong}: ");
            int n = 0;

            if (!string.Equals(oldP.TenPhong, newP.TenPhong, StringComparison.Ordinal))
            {
                sb.Append($"Tên: '{oldP.TenPhong}' → '{newP.TenPhong}'"); n++;
            }
            if (!string.Equals(oldP.MoTa ?? "", newP.MoTa ?? "", StringComparison.Ordinal))
            {
                if (n > 0) sb.Append("; ");
                sb.Append($"Mô tả: '{Val(oldP.MoTa)}' → '{Val(newP.MoTa)}'"); n++;
            }

            return n == 0 ? "Cập nhật phòng: (không có thay đổi dữ liệu)." : sb.ToString();
        }

        public static string BuildTBAdd(ThietBi t, string? tenPhong)
            => $"Thêm thiết bị: Mã (auto), Tên='{t.TenThietBi}', Loại='{t.LoaiThietBi}', " +
               $"Ngày mua={t.NgayMua:yyyy-MM-dd}, Tình trạng='{t.TinhTrang}', " +
               $"Phòng={(t.MaPhong == null ? "(chưa gán)" : $"{t.MaPhong} - '{tenPhong}'")}, " +
               $"Thông tin='{t.ThongTin}', Ghi chú='{Val(t.GhiChu)}'.";

        public static string BuildTBDelete(ThietBi t, string? tenPhong)
            => $"Xoá thiết bị: {t.MaThietBi} - '{t.TenThietBi}', Loại='{t.LoaiThietBi}', " +
               $"Phòng={(t.MaPhong == null ? "(chưa gán)" : $"{t.MaPhong} - '{tenPhong}'")}.";

        public static string BuildTBDiff(ThietBi oldT, ThietBi newT, string? oldRoomName, string? newRoomName)
        {
            var sb = new StringBuilder($"Cập nhật thiết bị {oldT.MaThietBi}: ");
            int n = 0;

            void add(string field, string ov, string nv)
            {
                if (n > 0) sb.Append("; ");
                sb.Append($"{field}: '{ov}' → '{nv}'");
                n++;
            }

            if (!string.Equals(oldT.TenThietBi, newT.TenThietBi, StringComparison.Ordinal))
                add("Tên", oldT.TenThietBi, newT.TenThietBi);

            if (!string.Equals(oldT.LoaiThietBi, newT.LoaiThietBi, StringComparison.Ordinal))
                add("Loại", oldT.LoaiThietBi, newT.LoaiThietBi);

            if (oldT.NgayMua.Date != newT.NgayMua.Date)
                add("Ngày mua", oldT.NgayMua.ToString("yyyy-MM-dd"), newT.NgayMua.ToString("yyyy-MM-dd"));

            if (!string.Equals(oldT.TinhTrang, newT.TinhTrang, StringComparison.Ordinal))
                add("Tình trạng", oldT.TinhTrang, newT.TinhTrang);

            if (!string.Equals(oldT.ThongTin, newT.ThongTin, StringComparison.Ordinal))
                add("Thông tin", oldT.ThongTin, newT.ThongTin);

            if (!string.Equals(oldT.GhiChu ?? "", newT.GhiChu ?? "", StringComparison.Ordinal))
                add("Ghi chú", Val(oldT.GhiChu), Val(newT.GhiChu));

            // chuyển phòng
            string ovRoom = oldT.MaPhong == null ? "(chưa gán)" : $"{oldT.MaPhong} - '{oldRoomName}'";
            string nvRoom = newT.MaPhong == null ? "(chưa gán)" : $"{newT.MaPhong} - '{newRoomName}'";
            if (!string.Equals(oldT.MaPhong ?? "", newT.MaPhong ?? "", StringComparison.Ordinal))
                add("Phòng", ovRoom, nvRoom);

            return n == 0 ? "Cập nhật thiết bị: (không có thay đổi dữ liệu)." : sb.ToString();
        }
    }
}
