using System;
using System.Configuration;
using System.Data.SqlClient;

namespace QLCSVCWinApp.DataAccess
{
    public static class DbHelper
    {
        private static readonly string connStr = ConfigurationManager.ConnectionStrings["connString"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connStr);
        }

        public static bool TestConnection()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    return conn.State == System.Data.ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
