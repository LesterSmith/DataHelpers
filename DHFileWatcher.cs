using System;
using System.Data;
using System.Data.SqlClient;
using BusinessObjects;
namespace DataHelpers
{
    public class DHFileWatcher : DataHelperBase
    {
        public DHFileWatcher(string connStr)
            : base(connStr) { }
        public DHFileWatcher() : base(string.Empty) { }

        const string _pipe = "|";

        public int InsertUpdateFileActivity(FileActivity item)
        {
            using (var cmd = new SqlCommand("DevTrkr..InsertUpdateFileActivity"))
            {
                cmd.Parameters.AddWithValue("@Machine", item.Machine);
                cmd.Parameters.AddWithValue("@DevProjName", item.DevProjName);
                cmd.Parameters.AddWithValue("@Filename", item.Filename);
                cmd.Parameters.AddWithValue("@Username", item.Username);
                cmd.Parameters.AddWithValue("@FileLength", item.FileLength);
                cmd.Parameters.AddWithValue("@LastAction", item.LastAction);
                cmd.Parameters.AddWithValue("@CreatedBy", item.CreatedBy);
                cmd.Parameters.AddWithValue("@UpdatedBy", item.UpdatedBy);
                cmd.Parameters.AddWithValue("@UpdateCount", item.UpdateCount);
                cmd.Parameters.AddWithValue("@DevProjectPath", item.DevProjectPath);
                cmd.Parameters.AddWithValue("@CodeLines", item.CodeLines);
                cmd.Parameters.AddWithValue("@BlankLines", item.BlankLines);
                cmd.Parameters.AddWithValue("@CommentLines", item.CommentLines);
                cmd.Parameters.AddWithValue("@DesignerLines", item.DesignerLines);
                return UpdateDatabase(cmd);
            }
        }

        /// <summary>
        /// have to rewrite in code since it can' be done in the orig sproc
        /// </summary>
        /// <returns>DevProjPath object</returns>
        public DevProjPath IsFileInDevPrjPath(string fileName)
        {
            using (var cmd = new SqlCommand("DevTrkr..GetDevProjects"))
            {
                DataTable dt = GetDataTable(cmd);

                // now find an object where the devprojectpath is
                // contained in the fileName
                foreach (DataRow dr in dt.Rows)
                {
                    if (fileName.ToLower().Contains(dr["DevProjectPath"].ToString().ToLower()))
                    {
                        return new DevProjPath
                        {
                            ID = dr["ID"].ToString(),
                            DevProjectName = (string)dr["DevProjectName"],
                            DevProjectPath = dr["DevProjectPath"] == DBNull.Value ? string.Empty : (string)dr["DevProjectPath"],
                            Machine = dr["Machine"] == DBNull.Value ? string.Empty : (string)dr["Machine"],
                            UserName = dr["UserName"] == DBNull.Value ? string.Empty : (string)dr["UserName"],
                            IDEAppName = dr["IDEAppName"] == DBNull.Value ? string.Empty : (string)dr["IDEAppName"],
                            CreatedDate = dr["CreatedDate"] == DBNull.Value ? DateTime.Now : (DateTime)dr["CreatedDate"],
                            CompletedDate = dr["CompletedDate"] == DBNull.Value ? DateTime.Now : (DateTime)dr["CompletedDate"],
                            DatabaseProject = dr["DatabaseProject"] == DBNull.Value ? false : (bool)dr["DatabaseProject"],
                            SyncID = dr["SyncID"] == DBNull.Value ? string.Empty : dr["SyncID"].ToString(),
                            ProjFileExt = dr["ProjFileExt"] == DBNull.Value ? string.Empty : (string)dr["ProjFileExt"],
                            DevSLNPath = dr["DevSLNPath"] == DBNull.Value ? string.Empty : (string)dr["DevSLNPath"],
                            GitURL = dr["GitURL"] == DBNull.Value ? string.Empty : (string)dr["GitURL"]
                        };
                    }
                }
            }
            return null;
        }

        public int CheckForInsertingNewProjectPath(DevProjPath item) 
        {
            using (var cmd = new SqlCommand("DevTrkr..CheckForInsertingNewProjectPath"))
            {
                cmd.Parameters.AddWithValue("@DevProjectName", item.DevProjectName);
                cmd.Parameters.AddWithValue("@DevProjectPath", item.DevProjectPath);
                cmd.Parameters.AddWithValue("@Machine", item.Machine);
                cmd.Parameters.AddWithValue("@UserName", item.UserName);
                cmd.Parameters.AddWithValue("@IDEAppName", item.IDEAppName);
                cmd.Parameters.AddWithValue("@DatabaseProject", item.DatabaseProject);
                return UpdateDatabase(cmd);
            }
        }
    }
}
