using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLCSVCWinApp.Models
{
    public class ThietBi
    {
        public string MaThietBi { get; set; } = string.Empty;
        public string LoaiThietBi { get; set; } = string.Empty; // "Máy lạnh", "CPU", ...
        public string TenThietBi { get; set; } = string.Empty;
        public DateTime NgayMua { get; set; }
        public string TinhTrang { get; set; } = string.Empty; // "Đang sử dụng", "Hỏng"...
        public string ThongTin { get; set; } = string.Empty; // mô tả chi tiết
        public string? GhiChu { get; set; }
        public string? MaPhong { get; set; }                 // null nếu chưa có phòng

        // NEW: chỉ để hiển thị (JOIN từ phonghoc)
        public string? TenPhong { get; set; }
    }
}