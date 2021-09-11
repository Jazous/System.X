using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

namespace System.X.Data.SqlClient
{
    class SQLHelper
    {
        public static int ExecuteSqlNonQuery(string connectionString, string sql, params SqlParameter[] oParams)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                if (oParams != null && oParams.Length > 0)
                    foreach (SqlParameter prm in oParams)
                        cmd.Parameters.Add(prm);
                return cmd.ExecuteNonQuery();
            }
        }

        void BackupDbMSSQL(string connectionStr, string dbName, string filePath)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = connectionStr;
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandTimeout = 0; //设置不超时
                    command.CommandText = string.Format("backup database {0} to disk='{1}'", dbName, filePath);
                    command.ExecuteNonQuery();
                }
            }
        }
        void RestoreDbMSSQL(string connectionStr, string dbName, string filePath)
        {

            //1、尝试终止需要备份数据库的会话进行
            string killspid = string.Format("declare @dbname nvarchar(50) set @dbname = '{0}' declare @spid nvarchar(20) declare #tb cursor for select spid=cast(spid as varchar(20)) from master.sys.sysprocesses where dbid=db_id(@dbname) open #tb fetch next from #tb into @spid while @@fetch_status=0 begin exec('kill '+@spid) fetch next from #tb into @spid end close #tb deallocate #tb", dbName);
            SqlConnection connection = new SqlConnection(connectionStr);
            connection.Open();
            using (SqlCommand command = new SqlCommand(killspid, connection))
            {
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                }
            }
            connection.Close();

            //2、开启新链接，备份数据库
            string rescmd = string.Format("restore database {0} from disk = '{1}' with replace", dbName, filePath);
            connection.Open();
            using (SqlCommand command = new SqlCommand(rescmd, connection))
            {
                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        void BackupMySQL(string host, string port, string user, string pwd, string db, string backupfile)
        {
            if (string.IsNullOrEmpty(port))
                port = "3306";

            string command = $"mysqldump -h{host} -P{port} -u{user} -p{pwd} --databases {db} > {backupfile}";
            Fn.RunCmd(command);
        }
        void ExecMySQL(string host, string port, string user, string pwd, string db, string restorefile)
        {
            if (Fn.IsLocalhost(host))
            {
                string command = $"mysql -P{port} -u{user} -p{pwd} {db} < {restorefile}";
                Fn.RunCmd(command);
            }
            else
            {
                string command = $"mysql -h {host} -P{port} -u{user} -p{pwd} {db} < {restorefile}";
                Fn.RunCmd(command);
            }
        }
    }
}
