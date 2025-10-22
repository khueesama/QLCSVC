namespace QLCSVCWinApp.Utils
{
    public static class Session
    {
        // để null khi chưa đăng nhập; sẽ được set = tên đăng nhập sau khi login
        public static string? CurrentUser { get; set; }
    }
}
