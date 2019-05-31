using System;
using System.Data.SqlClient;
using System.Data;

namespace ParserCSV_WinForm
{
    public static class StaticClass
    {
        static SqlConnection sqlcn;
        public static string[] work_lines;
        public static int total_lines = 0;
        public static int current_line = 0;
        public static int current_line_buf = 0;

        public static void InntitializeSqlConnection(string conection_string)
        {
            sqlcn = new SqlConnection(conection_string);
        }

        public static bool TestSql()
        {
            try
            {
                sqlcn.Open();
                return true;
            }
            catch(SqlException)
            {
                return false;
            }
            finally
            {
                sqlcn.Close();
            }
        }
    }
}
