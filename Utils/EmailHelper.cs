using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace QLCSVCWinApp.Utils
{
    public static class EmailHelper
    {
        // .NET cũ đôi khi cần ép TLS 1.2 để Gmail chấp nhận
        static EmailHelper()
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            }
            catch { /* ignore */ }
        }

        /// <summary>
        /// Gửi OTP đến email và trả về chuỗi OTP đó.
        /// <para>purpose: "dang-ky" | "quen-mk" (để tùy biến tiêu đề/nội dung)</para>
        /// </summary>
        public static string GuiOtp(string toEmail, string purpose = "quen-mk")
        {
            // 1) Sinh OTP 6 chữ số bằng RNG (an toàn hơn Random)
            var otp = GenerateNumericOtp(6);

            // 2) Đọc cấu hình từ App.config
            string fromEmail = ConfigurationManager.AppSettings["EmailUser"]
                ?? throw new InvalidOperationException("Thiếu AppSettings: EmailUser.");
            string appPassword = ConfigurationManager.AppSettings["EmailAppPassword"]
                ?? throw new InvalidOperationException("Thiếu AppSettings: EmailAppPassword.");

            // Optional: cho phép override host/port/ssl qua AppSettings (có cũng được, không có vẫn dùng mặc định Gmail)
            string smtpHost = ConfigurationManager.AppSettings["SmtpHost"] ?? "smtp.gmail.com";
            int smtpPort = TryParseInt(ConfigurationManager.AppSettings["SmtpPort"], 587);
            bool smtpSsl = TryParseBool(ConfigurationManager.AppSettings["SmtpUseSsl"], true);
            string fromDisplay = ConfigurationManager.AppSettings["EmailFromDisplay"] ?? "Hệ thống QLCSVC";

            // 3) Chuẩn bị nội dung theo mục đích
            string subject, body;
            BuildSubjectBody(purpose, otp, out subject, out body);

            // 4) Gửi email (synchronous). Nếu muốn, bạn có thể viết thêm bản async với SendMailAsync.
            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress(fromEmail, fromDisplay, Encoding.UTF8);
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.SubjectEncoding = Encoding.UTF8;
                mail.Body = body;
                mail.BodyEncoding = Encoding.UTF8;
                mail.IsBodyHtml = false;

                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    client.EnableSsl = smtpSsl;
                    client.Credentials = new NetworkCredential(fromEmail, appPassword);
                    client.Timeout = 15000; // 15s

                    try
                    {
                        client.Send(mail);
                    }
                    catch (SmtpException ex)
                    {
                        // Ép thông báo dễ hiểu hơn
                        throw new InvalidOperationException(
                            "Không gửi được email OTP. Vui lòng kiểm tra cấu hình SMTP/App Password hoặc kết nối Internet.\nChi tiết: " + ex.Message, ex);
                    }
                }
            }

            return otp;
        }

        // ===== Helpers =====

        private static string GenerateNumericOtp(int digits)
        {
            if (digits <= 0) digits = 6;
            // sinh số trong [0, 10^digits - 1]
            var max = (int)Math.Pow(10, digits);
            // dùng RNG để random 4 bytes rồi mod
            int value;
            var buf = new byte[4];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(buf);
                value = Math.Abs(BitConverter.ToInt32(buf, 0)) % max;
            }
            return value.ToString(new string('0', digits)); // zero-padding
        }

        private static void BuildSubjectBody(string purpose, string otp, out string subject, out string body)
        {
            string commonTail = "— Thời gian có hiệu lực: 5 phút.";
            switch ((purpose ?? "").Trim().ToLowerInvariant())
            {
                case "dang-ky":
                    subject = "OTP xác minh đăng ký tài khoản";
                    body = $"Mã OTP của bạn là: {otp}\n{commonTail}";
                    break;

                case "quen-mk":
                default:
                    subject = "OTP xác nhận đổi mật khẩu";
                    body = $"Mã OTP của bạn là: {otp}\n{commonTail}";
                    break;
            }
        }

        private static int TryParseInt(string? s, int defaultValue)
            => int.TryParse(s, out var v) ? v : defaultValue;

        private static bool TryParseBool(string? s, bool defaultValue)
            => bool.TryParse(s, out var v) ? v : defaultValue;
    }
}
