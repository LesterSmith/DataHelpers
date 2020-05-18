using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using BusinessObjects; 
namespace DataHelpers
{
    public class DHWindowEvents : DataHelperBase
    {
        public DHWindowEvents(string connStr)
            : base(connStr) { }
        public DHWindowEvents() : base(string.Empty) { }

        public int InsertWindowEvent(WindowEvent item)
        {
            using (var cmd = new SqlCommand("DevTrkr..InsertWindowEvent"))
            {
                cmd.Parameters.AddWithValue("@ID", Guid.NewGuid());
                cmd.Parameters.AddWithValue("@StartTime", item.StartTime);
                cmd.Parameters.AddWithValue("@WindowTitle", item.WindowTitle);
                cmd.Parameters.AddWithValue("@AppName", item.AppName);
                cmd.Parameters.AddWithValue("@ModuleName", item.ModuleName);
                cmd.Parameters.AddWithValue("@EndTime", item.EndTime);
                cmd.Parameters.AddWithValue("@DevProjectName", item.DevProjectName);
                cmd.Parameters.AddWithValue("@ITProjectID", item.ITProjectID);
                cmd.Parameters.AddWithValue("@UserName", item.UserName);
                cmd.Parameters.AddWithValue("@MachineName", item.MachineName);
                cmd.Parameters.AddWithValue("@UserDisplayName", item.UserDisplayName);
                cmd.Parameters.AddWithValue("@SyncID", item.SyncID);
                return UpdateDatabase(cmd);
            }
        }

        public WindowEvent GetLastWindowEventWritten(string winEvntID)
        {
            using (var cmd = new SqlCommand("DevTrkr..GetLastWindowEventWritten"))
            {
                cmd.Parameters.AddWithValue("@ID", winEvntID);
                DataTable dt = GetDataTable(cmd);

                if (dt.Rows.Count < 1) return null;
                DataRow dr = dt.Rows[0];
                return new WindowEvent
                {
                    ID = dr["ID"].ToString(),
                    StartTime = (DateTime)dr["StartTime"],
                    WindowTitle = (string)dr["WindowTitle"],
                    AppName = (string)dr["AppName"],
                    ModuleName = (string)dr["ModuleName"],
                    DevProjectName = (string)dr["DevProjectName"],
                    ITProjectID = dr["ITProjectID"] == DBNull.Value ? string.Empty : (string)dr["ITProjectID"],
                    EndTime = dr["EndTime"] == DBNull.Value ? DateTime.Now : (DateTime)dr["EndTime"],
                    UserName = dr["UserName"] == DBNull.Value ? string.Empty : (string)dr["UserName"],
                    MachineName = dr["MachineName"] == DBNull.Value ? string.Empty : (string)dr["MachineName"]
                };
            }
        }

        public DataTable GetWindowEvents(string userName, string projName, DateTime stTime, DateTime endTime)
        {
            using (var cmd = new SqlCommand("DevTrkr..GetWindowEvents"))
            {
                cmd.Parameters.AddWithValue("@UserName", userName);
                cmd.Parameters.AddWithValue("@DevProjectName", projName);
                cmd.Parameters.AddWithValue("@StartTime", stTime);
                cmd.Parameters.AddWithValue("@EndTime", endTime);
                return GetDataTable(cmd);
            }
        }

        public string GetUserDisplayName(string userName, string machineName)
        {
            using (var cmd = new SqlCommand("GetUserDisplayName"))
            {
                cmd.Parameters.AddWithValue("@UserName", userName);
                cmd.Parameters.AddWithValue("@MachineName", machineName);
                return GetOneDataValue(cmd).GetNotDBNull();
            }
        }

        public int UpadateProjAndPathInWindowEvent(string id, string devProjectName)
        {
            using (var cmd = new SqlCommand("DevTrkr..UpadateDevProjNameInWindowEvent"))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@DevProjectName", devProjectName);
                return UpdateDatabase(cmd);
            }
        }

        public object UpdateWindowEvent(WindowEvent item)
        {
            using (var cmd = new SqlCommand("DevTrkr..UpdateWindowEvent"))
            {
                cmd.Parameters.AddWithValue("@ID", item.ID);
                cmd.Parameters.AddWithValue("@StartTime", item.StartTime);
                cmd.Parameters.AddWithValue("@WindowTitle", item.WindowTitle);
                cmd.Parameters.AddWithValue("@AppName", item.AppName);
                cmd.Parameters.AddWithValue("@ModuleName", item.ModuleName);
                cmd.Parameters.AddWithValue("@DevProjectName", item.DevProjectName);
                cmd.Parameters.AddWithValue("@ITProjectID", item.ITProjectID);
                cmd.Parameters.AddWithValue("@EndTime", item.EndTime);
                cmd.Parameters.AddWithValue("@UserName", item.UserName);
                cmd.Parameters.AddWithValue("@MachineName", item.MachineName);
                cmd.Parameters.AddWithValue("@UserDisplayName", item.UserDisplayName);
                return UpdateDatabase(cmd);
            }
        }

        public int UpdateUnknownProjectNameForIDEMatch(string devProjName, string appName, string unknownKey, string machineName, string userName)
        {
            using (var cmd = new SqlCommand("UpdateUnknownProjectNameForIDEMatch"))
            {
                cmd.Parameters.AddWithValue("@DevProjectName", devProjName);
                cmd.Parameters.AddWithValue("@AppName", appName);
                cmd.Parameters.AddWithValue("@UnknownKey", unknownKey);
                cmd.Parameters.AddWithValue("@Machine", machineName);
                cmd.Parameters.AddWithValue("@UserName", userName);
                return UpdateDatabase(cmd);
            }
        }
    }
}
