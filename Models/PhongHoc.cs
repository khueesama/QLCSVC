using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using QLCSVCWinApp.Models;

namespace QLCSVCWinApp.DataAccess
{
    namespace QLCSVCWinApp.Models
    {
        
        public class PhongHoc
        {
            public string MaPhong { get; set; } = string.Empty;
            public string TenPhong { get; set; } = string.Empty;
            public string MoTa { get; set; } = string.Empty;  // hoặc string? nếu cho phép null
        }

    }

}
