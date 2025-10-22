using System;
using System.Windows.Forms;
using QLCSVCWinApp.Forms;

namespace QLCSVCWinApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            while (true)
            {
                // 1) Show login dạng modal
                using (var login = new frmDangNhap())
                {
                    if (login.ShowDialog() != DialogResult.OK)
                        return;    // Nếu user Cancel/close → thoát app
                }

                // 2) Nếu login OK → Show main cũng modal
                using (var main = new frmMain())
                {
                    main.ShowDialog();
                    // Khi main.Close() (tức user bấm Logout) → quay về login
                }
            }
        }
    }
}
