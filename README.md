# QLCSVCWinApp – Quản lý Cơ sở Vật chất

Ứng dụng **WinForms C#** được phát triển theo mô hình **3 lớp (Presentation – Business – Data Access)** nhằm hỗ trợ quản lý cơ sở vật chất cho các trung tâm, trường học hoặc tổ chức.  
Hệ thống giúp quản lý phòng học, trang thiết bị, ---

## ✨ Tính năng chính

- **Quản lý thiết bị:** Thêm mới, chỉnh sửa, chuyển phòng, cập nhật tình trạng và ghi chú thiết bị.
- **Quản lý phòng học:** Thêm, sửa, xóa phòng; theo dõi danh sách thiết bị theo từng phòng và tình trạng.
- **Tìm kiếm & thống kê:** Tìm theo nhiều tiêu chí (mã, tên, loại, tình trạng, phòng...) và thống kê theo loại/tình trạng.
- **Xuất báo cáo:** Xuất báo cáo theo phòng, loại thiết bị, tình trạng, hoặc lịch sử thay đổi (quyền quản trị).
- **Lịch sử thay đổi:** Tự động ghi nhận thay đổi (tình trạng, chuyển phòng, sửa thông tin) và cho phép tra cứu.
- **Sao lưu & phục hồi:** Sao lưu thủ công/tự động toàn hệ thống và khôi phục khi cần.

---

## 🏗️ Kiến trúc phần mềm

Ứng dụng được xây dựng theo **mô hình 3 lớp:**

| Lớp | Mô tả |
|-----|-------|
| **Presentation Layer** | Giao diện | **Business Layer** | Xử lý nghiệp vụ, gọi các phương thức từ lớp DAL |
| **Data Access Layer (DAL)** | Thao tác cơ sở dữ liệu bằng `System.Data.SqlClient`, thông qua chuỗi kết nối trong App.config |

---

## 📂 Cấu trúc thư mục

```
QLCSVCWinApp/
├── QLCSVCWinApp.sln                 # Solution file
├── QLCSVCWinApp/
│   ├── QLCSVCWinApp.csproj         # Project definition
│   ├── App.config                   # Email + Database + Backup configuration
│   ├── Program.cs                   # Entry point (login → main)
│   ├── Forms/                       # Giao diện (WinForms)
│   │   ├── frmDangNhap.cs
│   │   ├── frmMain.cs
│   │   ├── frmPhong.cs
│   │   ├── frmThietBi.cs
│   │   ├── frmMuonTra.cs
│   │   └── ...
│   ├── Services/                    # Xử lý backup, email, kết nối DB
│   ├── DAL/                         # Data Access Layer
│   ├── BLL/                         # Business Logic Layer
│   ├── Models/                      # Các lớp mô hình dữ liệu
│   └── bin/ & obj/                  # Thư mục build
└── README.md
```

---

## ⚙️ Cấu hình hệ thống

Cấu hình nằm trong file **`App.config`**:

```xml
<appSettings>
  <!-- Email SMTP -->
  <add key="EmailUser" value="...." />
  <add key="EmailAppPassword" value="..." />
  <add key="SmtpHost" value="smtp.gmail.com"/>
  <add key="SmtpPort" value="587"/>
  <add key="SmtpUseSsl" value="true"/>
  <add key="EmailFromDisplay" value="Hệ thống QLCSVC"/>

  <!-- Backup Settings -->
  <add key="Backup:Enabled" value="true" />
  <add key="Backup:Folder" value="C:\QLCSVC\Backups" />
  <add key="Backup:Cron" value="0 0 2 ? * * *" /> <!-- 02:00 AM mỗi ngày -->
</appSettings>

<connectionStrings>
  <add name="connString"
       connectionString="Data Source=.;Initial Catalog=qlcsvc;Integrated Security=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

> ⚠️ **Lưu ý:**  
> - Cập nhật `Initial Catalog` hoặc `Data Source` nếu tên CSDL hoặc máy chủ khác.  
> - Tạo database `qlcsvc` và import dữ liệu mẫu nếu có.  
> - Cấp quyền ghi cho thư mục `C:\QLCSVC\Backups` nếu sử dụng tính năng sao lưu.

---

## 🧩 Cách chạy ứng dụng

### 1️⃣ Môi trường yêu cầu
- .NET SDK 8.0 trở lên
- SQL Server 2019 trở lên
- Visual Studio 2022 hoặc Rider

### 2️⃣ Mở dự án
- Mở `QLCSVCWinApp.sln` trong Visual Studio  
- Khởi chạy bằng **Ctrl + F5** hoặc nút **Run (▶)**

### 3️⃣ Cấu hình database
- Mở SQL Server Management Studio (SSMS)  
- Tạo cơ sở dữ liệu `qlcsvc`  
- Cập nhật chuỗi kết nối trong `App.config` nếu cần.

### 4️⃣ Đăng nhập & sử dụng
- Đăng nhập bằng tài khoản mẫu (nếu có trong CSDL).  
- Sử dụng giao diện chính để quản lý phòng, thiết bị, phiếu mượn trả, sao lưu, v.v.

---

## 📦 Chức năng chính trong giao diện

| Module | Mô tả |
|---------|-------|
| **Quản lý phòng** | Thêm, sửa, xóa, tìm kiếm phòng học |
| **Quản lý thiết bị** | Quản lý danh sách thiết bị, trạng thái, phân loại |
| **Quản lý báo cáo** | Báo cáo tình trạng thiết bị, phòng, lịch sử sửa đổi |
| **Lịch sử thay đổi** | Hiển thị toàn bộ các hành động thay đổi thông tin trên hệ thống |
| **Sao lưu & phục hồi** | Backup định kỳ hoặc thủ công |

---

## 👥 Tác giả & Liên hệ
- **Nhóm phát triển:** QLCSVC Team  
- **Ngôn ngữ:** C#, WinForms (.NET 8.0), SQL Server  
- **Liên hệ:** meochua18@gmail.com

---

> 📘 *Ứng dụng này được xây dựng phục vụ mục đích học tập và nghiên cứu cho học phần Phân tích & Thiết kế Hệ thống Thông tin.*
