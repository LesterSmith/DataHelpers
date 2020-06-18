using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
namespace DataHelpers
{
    public class DataHelperBase
    {
        #region public members
        public string ConnectionString { get; set; }
        #endregion

        #region ..ctor
        /// <summary>
        /// If the connection string is empty, retrieve it
        /// from the main application appConfig file.
        /// 
        /// </summary>
        /// <param name="connString"></param>
        public DataHelperBase(string connString="")
        {
            ConnectionString = !string.IsNullOrWhiteSpace(connString) ? connString : ConfigurationManager.ConnectionStrings[AppWrapper.AppWrapper.DBName].ConnectionString;
        }
        #endregion

        #region public methods
        protected internal DataSet GetDataSet(SqlCommand cmd, bool sProc = true)
        {
            var timeoutCnt = 0;
        TryAgain:
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    cmd.CommandTimeout = 300;
                    cmd.CommandType = sProc ? CommandType.StoredProcedure : CommandType.Text;
                    cmd.Connection = conn;
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    var ds = new DataSet();
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                        conn.Close();
                    }
                    return ds;
                }
            }
            catch (Exception ex)
            {
                if ((ex.Message.ToLower().IndexOf("transport") > -1 || ex.Message.ToLower().IndexOf("connection") > -1 || ex.Message.ToLower().Contains("timeout") || ex.Message.ToLower().Contains("network")) && ++timeoutCnt < 3)
                    goto TryAgain;
                throw;
            }
        }

        protected internal DataTable GetDataTable(SqlCommand cmd, bool sProc = true)
        {
            var timeoutCnt = 0;
        TryAgain:
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    cmd.CommandTimeout = 600;
                    cmd.CommandType = sProc ? CommandType.StoredProcedure : CommandType.Text;
                    cmd.Connection = conn;
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    var dt = new DataTable();
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                        conn.Close();
                    }
                    return dt;
                }
            }
            catch (Exception ex)
            {
                if ((ex.Message.ToLower().IndexOf("transport") > -1 || ex.Message.ToLower().IndexOf("connection") > -1 || ex.Message.ToLower().Contains("timeout") || ex.Message.ToLower().Contains("network")) && ++timeoutCnt < 3)
                    goto TryAgain;
                throw;
            }
        }


        protected internal object GetOneDataValue(SqlCommand cmd, bool sProc = true)
        {
            var timeoutCnt = 0;
        TryAgain:
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    cmd.CommandTimeout = 300;
                    cmd.CommandType = sProc ? CommandType.StoredProcedure : CommandType.Text;
                    cmd.Connection = conn;

                    if (conn.State != ConnectionState.Open)
                        conn.Open();
                    var obj = cmd.ExecuteScalar();
                    conn.Close();
                    return obj;
                }
            }
            catch (Exception ex)
            {
                if ((ex.Message.ToLower().IndexOf("transport") > -1 || ex.Message.ToLower().IndexOf("connection") > -1 || ex.Message.ToLower().Contains("timeout") || ex.Message.ToLower().Contains("network")) && ++timeoutCnt < 3)
                    goto TryAgain;
                throw;
            }
        }

        /// <summary>
        /// ExecuteNonQuery with return parameter(s)
        /// since cmd is an object, updating in this method updates it in caller
        /// the return parameter(s) will be updated in the cmd upon return from ExecuteNonQuery
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>cmd</returns>
        protected internal int UpdateWithReturnParameter(SqlCommand cmd)
        {
            var timeoutCnt = 0;
        TryAgain:
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = conn;
                    if (conn.State != ConnectionState.Open)
                        conn.Open();
                    var rows = cmd.ExecuteNonQuery();
                    conn.Close();
                    return rows;
                }
            }
            catch (Exception ex)
            {
                if ((ex.Message.ToLower().IndexOf("transport") > -1 || ex.Message.ToLower().IndexOf("connection") > -1 || ex.Message.ToLower().Contains("timeout") || ex.Message.ToLower().Contains("network")) && ++timeoutCnt < 3)
                    goto TryAgain;
                throw;
            }
        }

        protected internal int UpdateDatabase(SqlCommand cmd, bool sProc = true)
        {
            var timeoutCnt = 0;
        TryAgain:
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    cmd.CommandType = sProc ? CommandType.StoredProcedure : CommandType.Text;
                    cmd.Connection = conn;
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    var rows = cmd.ExecuteNonQuery();
                    conn.Close();
                    conn.Dispose();
                    return rows;
                }
            }
            catch (Exception ex)
            {
                if ((ex.Message.ToLower().IndexOf("transport") > -1 || ex.Message.ToLower().IndexOf("connection") > -1 || ex.Message.ToLower().Contains("timeout") || ex.Message.ToLower().Contains("network")) && ++timeoutCnt < 3)
                    goto TryAgain;
                throw;
            }
        }
        #endregion
    }
}
