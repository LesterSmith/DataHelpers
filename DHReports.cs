using System;
using System.Data;
using System.Data.SqlClient;
namespace DataHelpers
{
    public class DHReports : DataHelperBase
    {
        #region ..ctor
        public DHReports(string connStr)
            : base(connStr) { }
        #endregion

        #region public methods
        public DataSet GetDataSetFromSQL(string sql)
        {
            using (var cmd = new SqlCommand(sql))
            {
                return GetDataSet(cmd, false);
            }
        }
        #endregion

    }
}
