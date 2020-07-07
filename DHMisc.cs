using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using BusinessObjects;
using System.Diagnostics;
namespace DataHelpers
{
    public class DHMisc : DataHelperBase
    {
        public DHMisc(string connStr)
            : base(connStr) { }
        public DHMisc() : base(string.Empty) { }

        const string _pipe = "|";
        public List<DevProjPath> GetDevProjects(string userName, string machineName)
        {
            using (var cmd = new SqlCommand("DevTrkr..GetDevProjects"))
            {
                cmd.Parameters.AddWithValue("@UserName", userName);
                cmd.Parameters.AddWithValue("@Machine", machineName);
                //DateTime? n = null;
                DataTable dt = GetDataTable(cmd);
                return
                    (from DataRow dr in dt.Rows
                     select new DevProjPath
                     {
                         ID = dr["ID"].ToString(),
                         DevProjectName = (string)dr["DevProjectName"],
                         DevProjectPath = dr["DevProjectPath"] == DBNull.Value ? string.Empty : (string)dr["DevProjectPath"],
                         Machine = dr["Machine"] == DBNull.Value ? string.Empty : (string)dr["Machine"],
                         UserName = dr["UserName"] == DBNull.Value ? string.Empty : (string)dr["UserName"],
                         IDEAppName = dr["IDEAppName"] == DBNull.Value ? string.Empty : (string)dr["IDEAppName"],
                         CreatedDate = dr["CreatedDate"] == DBNull.Value ? DateTime.MinValue : (DateTime)dr["CreatedDate"],
                         CompletedDate = dr["CompletedDate"] == DBNull.Value ? DateTime.Now : (DateTime)dr["CompletedDate"],
                         DatabaseProject = dr["DatabaseProject"] == DBNull.Value ? false : (bool)dr["DatabaseProject"],
                         SyncID = dr["SyncID"] == DBNull.Value ? string.Empty : dr["SyncID"].ToString(),
                         ProjFileExt = dr["ProjFileExt"] == DBNull.Value ? string.Empty : (string)dr["ProjFileExt"],
                         DevSLNPath = dr["DevSLNPath"] == DBNull.Value ? string.Empty : (string)dr["DevSLNPath"],
                         GitURL = dr["GitURL"] == DBNull.Value ? string.Empty : (string)dr["GitURL"]
                     }).ToList();
            }
        }

        /// <summary>
        /// NOTE: there could be multiple collaborative projects with the same name
        /// this code must handle that situation
        /// </summary>
        public ProjAndSyncReport GetProjectsForReporting()
        {
            using (var cmd = new SqlCommand("DevTrkr..GetProjectsForReporting"))
            {
                var ds = GetDataSet(cmd);
                ProjAndSyncReport rptData = new ProjAndSyncReport();
                List<ReportProjects> list = new List<ReportProjects>();
                // these variables work to handle multiple collaborative projs same name
                string projName = string.Empty;
                string syncID = string.Empty;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string devProjName = dr["DevProjectName"].ToString();
                    string sync = Convert.ToString(dr["SyncID"]);
                    if (projName == devProjName && sync == syncID) continue;
                    projName = devProjName;
                    syncID = sync;
                    list.Add(new ReportProjects
                            {
                                DevProjectName = (string)dr["DevProjectName"],
                                DevProjectCount = (int)dr["DevProjectCount"],
                                SyncID = Convert.ToString(dr["SyncID"]),
                                UserName = (string)dr["UserName"],
                                DevSLNPath = dr["DevSLNPath"] != DBNull.Value ? (string)dr["DevSLNPath"] : string.Empty,
                                GitURL = dr["GitURL"] != DBNull.Value ? (string)dr["GitURL"] : string.Empty,
                                DatabaseProject = dr["DatabaseProject"] == DBNull.Value ? false : (bool)dr["DatabaseProject"]
                    });
                }
                rptData.Projects = list;

                rptData.Names =
                    (from DataRow dr in ds.Tables[1].Rows
                     select new ReportUserNames 
                     { UserName = (string)dr["UserName"], 
                       DisplayName = (string)dr["UserDisplayName"] 
                     }).ToList();
                return rptData;
            }
        }

        public List<ProjectSync> GetProjectSyncs()
        {
            using (var cmd = new SqlCommand("DevTrkr..GetProjectSyncs"))
            {
                var dt = GetDataTable(cmd);
                return (from DataRow dr in dt.Rows
                         select new ProjectSync
                         {
                             ID = dr["ID"].ToString(),
                             DevProjectName = (string)dr["DevProjectName"],
                             CreatedDate = (DateTime)dr["CreatedDate"],
                             DevProjectCount = (int)dr["DevProjectCount"],
                             GitURL = dr["GitURL"] == DBNull.Value ? string.Empty : (string)dr["GitURL"]
                         }).ToList();
            }
        }

        public List<DevProjPath> GetDevProjects()
        {
            using (var cmd = new SqlCommand("DevTrkr..GetDevProjects"))
            {
                DateTime? n = null;
                DataTable dt = GetDataTable(cmd);
                return
                    (from DataRow dr in dt.Rows
                     select new DevProjPath
                     {
                         ID = dr["ID"].ToString(),
                         DevProjectName = (string)dr["DevProjectName"],
                         DevProjectPath = dr["DevProjectPath"] == DBNull.Value ? string.Empty : (string)dr["DevProjectPath"],
                         Machine = dr["Machine"] == DBNull.Value ? string.Empty : (string)dr["Machine"],
                         UserName = dr["UserName"] == DBNull.Value ? string.Empty : (string)dr["UserName"],
                         IDEAppName = dr["IDEAppName"] == DBNull.Value ? string.Empty : (string)dr["IDEAppName"],
                         CreatedDate = dr["CreatedDate"] == DBNull.Value ? DateTime.Now : (DateTime)dr["CreatedDate"],
                         CompletedDate = dr["CompletedDate"] == DBNull.Value ? DateTime.MinValue : (DateTime)dr["CompletedDate"],
                         DatabaseProject = dr["DatabaseProject"] == DBNull.Value ? false : (bool)dr["DatabaseProject"],
                         SyncID = dr["SyncID"] == DBNull.Value ? string.Empty : dr["SyncID"].ToString(),
                         ProjFileExt = dr["ProjFileExt"] == DBNull.Value ? string.Empty : (string)dr["ProjFileExt"],
                         DevSLNPath = dr["DevSLNPath"] == DBNull.Value ? string.Empty : (string)dr["DevSLNPath"],
                         GitURL = dr["GitURL"] == DBNull.Value ? string.Empty : (string)dr["GitURL"]
                     }).ToList();
            }
        }

        public ProjectsAndSyncs GetProjectsAndSyncByName(string devProjectName)
        {
            ProjectsAndSyncs ps = new ProjectsAndSyncs();
            using (var cmd = new SqlCommand("DevTrkr..GetDevProjectsAndSyncByName"))
            {
                cmd.Parameters.AddWithValue("@DevProjectName", devProjectName);
                var ds = GetDataSet(cmd);

                var list =
                (from DataRow dr in ds.Tables[0].Rows
                 select new DevProjPath
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
                 }).ToList();
                ps.ProjectList = list;

                ps.ProjectSyncList =
                    (from DataRow dr in ds.Tables[1].Rows
                     select 
                    new ProjectSync
                    {
                        ID = dr["ID"].ToString(),
                        DevProjectName = (string)dr["DevProjectName"],
                        CreatedDate = (DateTime)dr["CreatedDate"],
                        DevProjectCount = (int)dr["DevProjectCount"],
                        GitURL = dr["GitURL"] == DBNull.Value ? string.Empty : (string)dr["GitURL"]
                    }).ToList();
                return ps;
            }
        }

        public int InsertUpdateProjectFiles(ProjectFiles item)
        {
            using (var cmd = new SqlCommand("DevTrkr..InsertUpdateProjectFiles"))
            {
                cmd.Parameters.AddWithValue("@DevProjectName", item.DevProjectName);
                cmd.Parameters.AddWithValue("@RelativeFileName", item.RelativeFileName);
                cmd.Parameters.AddWithValue("@SyncID", item.SyncID);
                cmd.Parameters.AddWithValue("@GitURL", item.GitURL);
                cmd.Parameters.AddWithValue("@CodeLines", item.CodeLines);
                cmd.Parameters.AddWithValue("@CommentLines", item.CommentLines);
                cmd.Parameters.AddWithValue("@BlankLines", item.BlankLines);
                cmd.Parameters.AddWithValue("@DesignerLines", item.DesignerLines);
                cmd.Parameters.AddWithValue("@CreatedBy", item.CreatedBy);
                cmd.Parameters.AddWithValue("@LastUpdatedBy", item.LastUpdatedBy);
                return UpdateDatabase(cmd);
            }
        }

        public DevProjPath GetDevDBProjectByName(string devProjectName)
        {
            using (var cmd = new SqlCommand("DevTrkr..GetDevDBProjectByName"))
            {
                cmd.Parameters.AddWithValue("@DevProjectName", devProjectName);
                var dt = GetDataTable(cmd);
                if (dt.Rows.Count.Equals(0))
                    return null;
                DataRow dr = dt.Rows[0];
                DevProjPath item = new DevProjPath
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
                return item;
            }
        }

        public int InsertProjectSyncObject(ProjectSync item)
        {
            using (var cmd = new SqlCommand("DevTrkr..InsertUpdateProjectSync"))
            {
                cmd.Parameters.AddWithValue("@ID", item.ID);
                cmd.Parameters.AddWithValue("@DevProjectName", item.DevProjectName);
                cmd.Parameters.AddWithValue("@CreatedDate", item.CreatedDate);
                cmd.Parameters.AddWithValue("@DevProjectCount", item.DevProjectCount);
                cmd.Parameters.AddWithValue("@GitURL", item.GitURL);
                return UpdateDatabase(cmd);
            }
        }

        public int DeleteDuplicateProjectFile(string iD)
        {
            using (var cmd = new SqlCommand("DevTrkr..DeleteDuplicateProjectFile"))
            {
                cmd.Parameters.AddWithValue("@ID", iD);
                return UpdateDatabase(cmd);
            }
        }

        public int UpdateSLNPathInDevProject(DevProjPath item)
        {
            using (var cmd = new SqlCommand("DevTrkr..UpdateSLNPathInDevProject"))
            {
                cmd.Parameters.AddWithValue("@ID", item.ID);
                cmd.Parameters.AddWithValue("@DevSLNPath", item.DevSLNPath);
                return UpdateDatabase(cmd);
            }
        }
        public List<ConfigOption> GetConfigOptions(string optionName="")
        {
            using (var cmd = new SqlCommand("DevTrkr..GetConfigOptions"))
            {
                if (!string.IsNullOrWhiteSpace(optionName))
                    cmd.Parameters.AddWithValue("@SpecificOption", optionName);
                var dt = GetDataTable(cmd);
                return
                    (from DataRow dr in dt.Rows
                     select new ConfigOption
                     {
                         ID = dr["ID"].ToString(),
                         Name = dr["Name"].ToString(),
                         Value = dr["Value"].ToString(),
                         Description = dr["Description"].ToString(),
                         Dirty = false
                     }).ToList();
            }
        }

        public List<DeveloperNames> GetDeveloperNames()
        {
            using (var cmd = new SqlCommand("DevTrkr..GetDeveloperNames"))
            {
                var dt = GetDataTable(cmd);
                return (from DataRow dr in dt.Rows
                        select new DeveloperNames
                        {
                            UserDisplayName = dr["UserDisplayName"].GetNotDBNull(),
                            UserName = dr["UserName"].GetNotDBNull()
                        }).ToList();
            }
        }

        public DataTable GetProjectData(string sql)
        {
            using (var cmd = new SqlCommand(sql))
            {
                return GetDataTable(cmd, false);
            }
        }
        public List<IDEMatch> GetProjectNameMatches()
        {
            using (var cmd = new SqlCommand("DevTrkr..GetProjectNameMatches"))
            {
                var dt = GetDataTable(cmd);
                List<IDEMatch> list = new List<IDEMatch>();
                list = (from DataRow dr in dt.Rows
                        select new IDEMatch
                        {
                            ID = dr["ID"].ToString(),
                            Regex = dr["RegEx"].ToString(),
                            RegexGroupName = dr["RegexGroupName"].ToString(),
                            AppName = dr["AppName"].ToString().ToLower(),
                            UnknownValue = dr["UnknownValue"].ToString(),
                            ProjNameReplaces = dr["ProjNameReplaces"].GetNotDBNull(),
                            ProjNameConcat = dr["ProjNameConcat"].GetNotDBNull(),
                            ConcatChar = dr["ConcatChar"].GetNotDBNull(),
                            Description = dr["Description"].GetNotDBNull(),
                            IsIde = dr["IsIDE"].GetNotDBNullBool(),
                            AlternateProjName = dr["AlternateProjName"] !=DBNull.Value ? dr["AlternateProjName"].GetNotDBNull() : null,
                            Sequence = dr["Sequence"].GetNotDBNullInt(),
                            IsDBEngine = dr["IsDBEngine"].GetNotDBNullBool()
                        }).ToList();
                return list;
            }
        }

        public int UpdateProjectSyncWithGitURL(ProjectSync ps)
        {
            using (var cmd = new SqlCommand("DevTrkr..UpdateProjectSyncWithGitURL"))
            {
                cmd.Parameters.AddWithValue("@ID", ps.ID);
                cmd.Parameters.AddWithValue("@GitURL", ps.GitURL);
                return UpdateDatabase(cmd);
            }
        }

        public int InsertUpdateProjNameMatches(IDEMatch item)
        {
            using (SqlCommand cmd = new SqlCommand("DevTrkr..InsertUpdateProjNameMatches"))
            {
                cmd.Parameters.AddWithValue("@ID", item.ID);
                cmd.Parameters.AddWithValue("@Regex", item.Regex);
                cmd.Parameters.AddWithValue("@RegexGroupName", item.RegexGroupName);
                cmd.Parameters.AddWithValue("@UnknownValue", item.UnknownValue);
                cmd.Parameters.AddWithValue("@AppName", item.AppName);
                cmd.Parameters.AddWithValue("@ProjNameReplaces", item.ProjNameReplaces);
                cmd.Parameters.AddWithValue("@ProjNameConcat", item.ProjNameConcat);
                cmd.Parameters.AddWithValue("@ConcatChar", item.ConcatChar);
                cmd.Parameters.AddWithValue("@IsIde", item.IsIde);
                cmd.Parameters.AddWithValue("@Description", item.Description);
                cmd.Parameters.AddWithValue("@AlternateProjName", item.AlternateProjName);
                cmd.Parameters.AddWithValue("@Sequence", item.Sequence);
                cmd.Parameters.AddWithValue("@IsDBEngine", item.IsDBEngine);
                return UpdateDatabase(cmd);
            }
        }

        public void InsertUpdateMeetings(CalendarMeeting item)
        {
            using (var cmd = new SqlCommand("DevTrkr..InsertUpdateMeetings"))
            {
                cmd.Parameters.AddWithValue("@ID", item.ID);
                cmd.Parameters.AddWithValue("@Subject", item.Subject);
                cmd.Parameters.AddWithValue("@Organizer", item.Organizer);
                cmd.Parameters.AddWithValue("@StartTime", item.StartTime);
                cmd.Parameters.AddWithValue("@EndTime", item.EndTime);
                cmd.Parameters.AddWithValue("@Recurring", item.Recurring);
                cmd.Parameters.AddWithValue("@UserName", item.UserName);
                cmd.Parameters.AddWithValue("@UserDisplayName", item.UserDisplayName);
                UpdateDatabase(cmd);
            }
        }

        public int DeleteProjNameMatches(string iD)
        {
            using (var cmd = new SqlCommand("DevTrkr..DeleteProjNameMatches"))
            {
                cmd.Parameters.AddWithValue("@ID", iD);
                return UpdateDatabase(cmd);
            }
        }

        public int DeleteConfigOption(string iD)
        {
            using (var cmd = new SqlCommand("DevTrkr..DeleteConfigOption"))
            {
                cmd.Parameters.AddWithValue("@ID", iD);
                return UpdateDatabase(cmd);
            }
        }

        public int DeleteDevProjects(string iD)
        {
            using (var cmd = new SqlCommand("DevTrkr..DeleteDevProjects"))
            {
                cmd.Parameters.AddWithValue("@ID", iD);
                return UpdateDatabase(cmd);
            }
        }

        public int DeleteNotableFiles(string iD)
        {
            using (var cmd = new SqlCommand("DevTrkr..DeleteNotableFileTypes"))
            {
                cmd.Parameters.AddWithValue("@ID", iD);
                return UpdateDatabase(cmd);
            }
        }

        public int DeleteApplications(string iD)
        {
            using (var cmd = new SqlCommand("DevTrkr..DeleteNotableApplications"))
            {
                cmd.Parameters.AddWithValue("@ID", iD);
                return UpdateDatabase(cmd);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateable">false cuts down space in cached options</param>
        /// <returns></returns>
        public List<NotableFileExtension> GetNotableFileExtensions(bool updateable=false)
        {
            using (var cmd = new SqlCommand("DevTrkr..GetNotableFileExtensions"))
            {
                var sb = new StringBuilder();
                var dt = GetDataTable(cmd);
                
                return (from DataRow dr in dt.Rows
                        select new NotableFileExtension
                        {
                            ID = updateable ? dr["ID"].ToString() : string.Empty,
                            Extension = dr["Extension"].ToString(),
                            CreatedDate = updateable ? (DateTime?)dr["CreatedDate"] : null,
                            IDEProjectExtension = dr["IDEProjectExtension"].GetNotDBNull(),
                            CountLines = dr["CountLines"].GetNotDBNullBool()
                        }).ToList();
            }
        }

        public List<NotableFileExtension> GetNotableFileExtensionsList()
        {
            using (var cmd = new SqlCommand("DevTrkr..GetNotableFileExtensions"))
            {
                var sb = new StringBuilder();
                var dt = GetDataTable(cmd);
                return (from DataRow dr in dt.Rows
                        select new NotableFileExtension
                        {
                            ID = dr["ID"].ToString(),
                            Extension = dr["Extension"].ToString(),
                            CreatedDate = (DateTime)dr["CreatedDate"],
                            IDEProjectExtension = dr["IDEProjectExtension"].GetNotDBNull(),
                            CountLines = dr["CountLines"].GetNotDBNullBool()
                        }).ToList();
            }
        }

        public List<NotableApplication> GetNotableApplications()
        {
            using (SqlCommand cmd = new SqlCommand("DevTrkr..GetNotableApplications"))
            {
                DataTable dt = GetDataTable(cmd);
                List<NotableApplication> list =
                (from DataRow dr in dt.Rows
                 select new NotableApplication
                 {
                     ID = dr["ID"].ToString(),
                     AppName = dr["AppName"].ToString(),
                     AppFriendlyName = dr["AppFriendlyName"].ToString(),
                     InterestingTitle = dr["InterestingTitle"].GetNotDBNull()
                 }).ToList();
                return list;
            }
        }

        public int InsertUpdateConfigOptions(ConfigOption item)
        {
            using (SqlCommand cmd = new SqlCommand("DevTrkr..InsertUpdateConfigOptions"))
            {
                cmd.Parameters.AddWithValue("@ID", item.ID);
                cmd.Parameters.AddWithValue("@Name", item.Name);
                cmd.Parameters.AddWithValue("@Value", item.Value);
                cmd.Parameters.AddWithValue("@Description", item.Description);
                return UpdateDatabase(cmd);
            }
        }

        public int InsertUpdateNotableFileTypes(NotableFileExtension item)
        {
            using (var cmd = new SqlCommand("InsertUpdateNotableFileTypes"))
            {
                cmd.Parameters.AddWithValue("@ID", item.ID);
                cmd.Parameters.AddWithValue("@Extension", item.Extension);
                cmd.Parameters.AddWithValue("@CreatedDate", item.CreatedDate);
                cmd.Parameters.AddWithValue("@IDEProjectExtension", item.IDEProjectExtension);
                return UpdateDatabase(cmd);
            }
        }

        public int InsertUpdateNotableApplications(NotableApplication item)
        {
            using(var cmd = new SqlCommand("InsertUpdateNotableApplications"))
            {
                cmd.Parameters.AddWithValue("@ID", item.ID);
                cmd.Parameters.AddWithValue("@AppName", item.AppName);
                cmd.Parameters.AddWithValue("@AppFriendlyName", item.AppFriendlyName);
                cmd.Parameters.AddWithValue("@InterestingTitle", item.InterestingTitle);
                return UpdateDatabase(cmd);
            }
        }

        public int InsertUpdateDevProject(DevProjPath item)
        {
            if (string.IsNullOrWhiteSpace(item.DevProjectName))
            {
                Debug.WriteLine("DevProjectName is empty");
                return -1;
            }
            using (var cmd = new SqlCommand("DevTrkr..InsertUpdateDevProject"))
            {
                cmd.Parameters.AddWithValue("@ID", item.ID);
                cmd.Parameters.AddWithValue("@DevProjectName", item.DevProjectName);
                cmd.Parameters.AddWithValue("@DevProjectPath", item.DevProjectPath);
                cmd.Parameters.AddWithValue("@Machine", item.Machine);
                cmd.Parameters.AddWithValue("@UserName", item.UserName);
                cmd.Parameters.AddWithValue("@IDEAppName", item.IDEAppName);
                cmd.Parameters.AddWithValue("@CreatedDate", item.CreatedDate);
                if (item.CompletedDate != null && item.CompletedDate > item.CreatedDate)
                    cmd.Parameters.AddWithValue("@CompletedDate", item.CompletedDate);
                cmd.Parameters.AddWithValue("@DatabaseProject", item.DatabaseProject);
                cmd.Parameters.AddWithValue("@SyncID", item.SyncID);
                cmd.Parameters.AddWithValue("@ProjFileExt", item.ProjFileExt);
                cmd.Parameters.AddWithValue("@DevSLNPath", item.DevSLNPath);
                cmd.Parameters.AddWithValue("@GitURL", item.GitURL);
                return UpdateDatabase(cmd);
            }
        }

        public int UpdateFileActivityWithProjectData(FileActivity item)
        {
            using (var cmd = new SqlCommand("DevTrkr..UpdateFileActivityWithProjectData"))
            {
                cmd.Parameters.AddWithValue("@DevProjName", item.DevProjName);
                cmd.Parameters.AddWithValue("@DevProjectPath", item.DevProjectPath);
                cmd.Parameters.AddWithValue("@Machine", item.Machine);
                cmd.Parameters.AddWithValue("@UserName", item.Username);
                return UpdateDatabase(cmd);
            }
        }

        public List<DevProjPath> GetDevProjectByName(string devProjectName)
        {
            using (var cmd = new SqlCommand("DevTrkr..GetDevProjectByName"))
            {
                cmd.Parameters.AddWithValue("@DevProjectName", devProjectName);
                var dt = GetDataTable(cmd);
                return (from DataRow dr in dt.Rows
                        select new DevProjPath
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
                        }).ToList();
            }
        }

        public int InsertUpdateProjectSync(ProjectSync item)
        {
            using (var cmd = new SqlCommand("DevTrkr..InsertUpdateProjectSync"))
            {
                cmd.Parameters.AddWithValue("@ID", item.ID);
                cmd.Parameters.AddWithValue("@DevProjectName", item.DevProjectName);
                cmd.Parameters.AddWithValue("@CreatedDate", item.CreatedDate);
                cmd.Parameters.AddWithValue("@DevProjectCount", item.DevProjectCount);
                cmd.Parameters.AddWithValue("@GitURL", item.GitURL);
                return UpdateDatabase(cmd);
            }
        }

        public int UpdateDevProjectsWithSyncIDAndGitURL(DevProjPath item)
        {
            using (var cmd = new SqlCommand("DevTrkr..UpdateDevProjectsWithSyncIDAndGitURL"))
            {
                cmd.Parameters.AddWithValue("@ID", item.ID);
                cmd.Parameters.AddWithValue("@SyncID", item.SyncID);
                cmd.Parameters.AddWithValue("@GitURL", item.GitURL);
                return UpdateDatabase(cmd);
            }
        }

        public int UpdateProjectFilesWithGitURL(string gitUrl, string devProjectName, string syncID)
        {
            using (var cmd = new SqlCommand("DevTrkr..UpdateProjectFilesWithGitURL"))
            {
                cmd.Parameters.AddWithValue("@GitURL", gitUrl);
                cmd.Parameters.AddWithValue("@DevProjectName", devProjectName);
                cmd.Parameters.AddWithValue("@SyncID", syncID);
                return UpdateDatabase(cmd);
            }
        }

        public List<ProjectFiles> GetDuplicateProjectFiles(string devProjectName, string syncID)
        {
            using (var cmd = new SqlCommand("DevTrkr..GetDuplicateProjectFiles"))
            {
                cmd.Parameters.AddWithValue("@DevProjectName", devProjectName);
                cmd.Parameters.AddWithValue("@SyncID", syncID);
                var dt = GetDataTable(cmd);
                return (from DataRow dr in dt.Rows
                        select new BusinessObjects.ProjectFiles
                        {
                            ID = dr["ID"].ToString(),
                            DevProjectName = (string)dr["DevProjectName"],
                            RelativeFileName = (string)dr["RelativeFileName"],
                            SyncID = dr["SyncID"] == DBNull.Value ? string.Empty : dr["SyncID"].ToString(),
                            GitURL = dr["GitURL"] == DBNull.Value ? string.Empty : (string)dr["GitURL"],
                            CodeLines = (int)dr["CodeLines"],
                            CommentLines = (int)dr["CommentLines"],
                            BlankLines = (int)dr["BlankLines"],
                            DesignerLines = (int)dr["DesignerLines"],
                            UpdateCount = (int)dr["UpdateCount"],
                            CreatedDate = (DateTime)dr["CreatedDate"],
                            CreatedBy = (string)dr["CreatedBy"],
                            LastUpdate = dr["LastUpdate"] == DBNull.Value ? DateTime.Now : (DateTime)dr["LastUpdate"],
                            LastUpdatedBy = dr["LastUpdatedBy"] == DBNull.Value ? string.Empty : (string)dr["LastUpdatedBy"]
                        }).ToList();
                
            }
        }

        public int WriteErrorLog(ErrorLog item)
        {
            using (var cmd = new SqlCommand("DevTrkr..InsertErrorLog"))
            {
                cmd.Parameters.AddWithValue("@Module", item.Module);
                cmd.Parameters.AddWithValue("@Method", item.Method);
                cmd.Parameters.AddWithValue("@Message", item.Message);
                cmd.Parameters.AddWithValue("@DateCreated", item.DateCreated);
                cmd.Parameters.AddWithValue("@Machine", item.Machine);
                cmd.Parameters.AddWithValue("@Username", item.Username);
                return UpdateDatabase(cmd);
            }
        }
    }
}
