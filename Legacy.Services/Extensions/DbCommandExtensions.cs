using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Legacy.Services.Data
{
    public static class DbCommandExtensions
    {
        public static DbCommand LoadStoredProc(this DbContext context, string storedProcName)
        {
            //SqlConnection conn = 
            var cmd = context.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = storedProcName;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            return cmd;
        }

        public static DbCommand WithSqlParam(this DbCommand cmd, string paramName, object paramValue, ParameterDirection paramDir = ParameterDirection.Input)
        {
            if (string.IsNullOrEmpty(cmd.CommandText))
                throw new InvalidOperationException("Call LoadStoredProc before using this method");

            var param = cmd.CreateParameter();
            param.ParameterName = paramName;
            param.Value = paramValue;
            param.Direction = paramDir;
            cmd.Parameters.Add(param);
            return cmd;
        }

        private static List<T> MapToList<T>(this DbDataReader dr)
        {
            var objList = new List<T>();
            var props = typeof(T).GetRuntimeProperties();

            var colMapping = dr.GetColumnSchema()
                .Where(x => props.Any(y => y.Name.ToLower() == x.ColumnName.ToLower()))
                .ToDictionary(key => key.ColumnName.ToLower());

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    T obj = Activator.CreateInstance<T>();
                    foreach (var prop in props)
                    {
                        var val = dr.GetValue(colMapping[prop.Name.ToLower()].ColumnOrdinal.Value);
                        prop.SetValue(obj, val == DBNull.Value ? null : val);
                    }
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public class DbOutput<T> : List<T>
        {
            public DbParameterCollection DbParameters;
        }
        public async static Task<DbOutput<T>> ExecuteStoredProcAsync<T>(this DbCommand command)
        {
            using (command)
            {
                if (command.Connection.State == System.Data.ConnectionState.Closed)
                    command.Connection.Open();
                try
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        DbOutput<T> output = new DbOutput<T>();
                        output.AddRange(reader.MapToList<T>());
                        output.DbParameters = command.Parameters;
                        return output;
                    }
                }
                finally
                {
                    command.Connection.Close();
                }
            }
        }

        public static T FirstOrDefault<T>(this IList<T> list)
        {
            if (list.Count == 0) return Activator.CreateInstance<T>();
            return (list[0]);
        }
    }
}
