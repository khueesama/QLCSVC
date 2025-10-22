using BCrypt.Net;

namespace QLCSVCWinApp.Utils
{
    public static class PasswordHasher
    {
        public static string Hash(string plainPassword)
            => BCrypt.Net.BCrypt.HashPassword(plainPassword); // work factor mặc định 10

        public static bool Verify(string plainPassword, string hash)
            => !string.IsNullOrWhiteSpace(hash) && BCrypt.Net.BCrypt.Verify(plainPassword, hash);

        // Hỗ trợ nhận diện chuỗi có phải bcrypt không (phục vụ nâng cấp)
        public static bool LooksLikeBcrypt(string? v)
            => !string.IsNullOrEmpty(v) && (v.StartsWith("$2a$") || v.StartsWith("$2b$") || v.StartsWith("$2y$"));
    }
}
