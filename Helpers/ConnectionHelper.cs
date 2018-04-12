using EscapeDBUsage.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.Helpers
{
    public static class ConnectionHelper
    {
        public static string ToConnectionString(DbConnection connection) {
            
            var s = string.Empty;

            var ser = connection.ServerName;
            var user = connection.Login;
            var pass = connection.Password;

            var builder = new SqlConnectionStringBuilder() { DataSource = ser, UserID = user, Password = pass, IntegratedSecurity = false };
            return builder.ToString();
        } 
    }
}
