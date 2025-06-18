using System;
using System.Collections.Generic;

using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;


namespace AccessDevelopment.Services.Data
{
    public sealed class SqlHelper
    {
        //Since this class provides only static methods, make the default constructor private to prevent 
        //instances from being created with "new SqlHelper()".
        private SqlHelper()
        {

        }

        private static string _connectionString;
        public static string GetConnectionString()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                //var config = new Configuration()
                //      .AddJsonFile("config.json")
                //      .AddEnvironmentVariables();
                _connectionString = "Password=@cc3ss124;Persist Security Info=True;User ID=RSGAccess;Initial Catalog=ReservationServicesIntl;Data Source=10.10.0.85;";// config.Get("Data:RSINewConnectionString:ConnectionString");
            }

            return _connectionString;
        }

        #region Async

        public static async Task<int> ExecuteNonQueryAsync(SqlConnection conn, string cmdText, SqlParameter[] cmdParms)
        {
            SqlCommand cmd = conn.CreateCommand();
            using (conn)
            {
                await PrepareCommandAsync(cmd, conn, null, CommandType.Text, cmdText, cmdParms);
                int val = await cmd.ExecuteNonQueryAsync();
                cmd.Parameters.Clear();
                return val;
            }
        }
        public static async Task<int> ExecuteNonQueryAsync(SqlConnection conn, CommandType cmdType, string cmdText, SqlParameter[] cmdParms, int timeoutInSeconds = 30)
        {
            SqlCommand cmd = conn.CreateCommand();
            using (conn)
            {
                await PrepareCommandAsync(cmd, conn, null, cmdType, cmdText, cmdParms);
                cmd.CommandTimeout = timeoutInSeconds;
                int val = await cmd.ExecuteNonQueryAsync();
                cmd.Parameters.Clear();
                return val;
            }
        }
        public static async Task<SqlDataReader> ExecuteReaderAsync(SqlConnection conn, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            SqlCommand cmd = conn.CreateCommand();
            await PrepareCommandAsync(cmd, conn, null, cmdType, cmdText, cmdParms);
            var rdr = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            return rdr;
        }
        public static async Task<object> ExecuteScalarAsync(SqlConnection conn, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            SqlCommand cmd = conn.CreateCommand();
            await PrepareCommandAsync(cmd, conn, null, cmdType, cmdText, cmdParms);
            object val = await cmd.ExecuteScalarAsync();
            cmd.Parameters.Clear();
            return val;
        }
        private static async Task PrepareCommandAsync(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] commandParameters)
        {
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync();
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            cmd.CommandType = cmdType;
            //attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(cmd, commandParameters);
            }
        }

        #endregion

        public static int ExecuteNonQuery(SqlConnection conn, string cmdText, SqlParameter[] cmdParms)
        {
            SqlCommand cmd = conn.CreateCommand();
            using (conn)
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, cmdText, cmdParms);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public static int ExecuteNonQuery(SqlConnection conn, CommandType cmdType, string cmdText, SqlParameter[] cmdParms, int timeoutInSeconds = 30)
        {
            SqlCommand cmd = conn.CreateCommand();
            using (conn)
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                cmd.CommandTimeout = timeoutInSeconds;
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }
        

        public static SqlDataReader ExecuteReader(SqlConnection conn, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            SqlCommand cmd = conn.CreateCommand();
            PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
            var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return rdr;
        }


        public static object ExecuteScalar(SqlConnection conn, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            SqlCommand cmd = conn.CreateCommand();
            PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] commandParameters)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            cmd.CommandType = cmdType;
            //attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(cmd, commandParameters);
            }
        }
        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            foreach (SqlParameter p in commandParameters)
            {
                //check for derived output value with no value assigned
                if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                {
                    p.Value = DBNull.Value;
                }

                command.Parameters.Add(p);
            }
        }
    }
}
