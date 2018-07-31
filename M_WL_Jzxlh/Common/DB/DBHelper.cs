//文件名：    DBHelper.aspx.cs
//功能描述：  通用数据库访问基类
//创建时间：  2018/05/31
//作者：      sh
//修改时间：  暂无
//修改描述：  暂无
//
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace ServiceInterface.Common
{
    public abstract class DBHelper : IDB
    {
        /// <summary>
        /// 连接对象
        /// </summary>
        private DbConnection connection = null;

        /// <summary>
        /// 命令对象
        /// </summary>
        private DbCommand command = null;

        /// <summary>
        /// 初始化连接
        /// </summary>
        protected void Init() {
            if (connection == null) {
                connection = GetConnection();
                connection.Open();
                command = connection.CreateCommand();
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        private void Close() {
            if (connection != null || command != null) {
                command.Cancel();
                connection.Close();
            }
        }

        /// <summary>
        /// 获取Connectio对象
        /// </summary>
        /// <returns>Connectio对象</returns>
        protected abstract DbConnection GetConnection();

        /// <summary>
        /// 获取DbDataAdapter对象
        /// </summary>
        /// <param name="strSql">Sql语句</param>
        /// <returns>DbDataAdapter对象</returns>
        protected abstract DbDataAdapter GetDbDataAdapter(string strSql);

        /// <summary>
        /// 获取DbCommandBuilder对象
        /// </summary>
        /// <param name="strSql">Sql语句</param>
        /// <returns>DbCommandBuilder</returns>
        protected abstract DbCommandBuilder GetDbCommandBuilder();

        public int ExecuteNonQuery(string strSql, out string strErr)
        {
            strErr = string.Empty;
            try
            {
                command.CommandText = strSql;
                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                strErr = ex.Message;
                return 0;
            }
            finally {
                Close();
            }    
        }

        public DataSet ExecuteQuery_DataSet(string strSql, out string strErr)
        {
            strErr = string.Empty;
            DataSet ds = new DataSet();
            DbDataAdapter dataAdapter = GetDbDataAdapter(strSql);
            try
            {
                command.CommandText = strSql;
                dataAdapter.SelectCommand = command;
                dataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                strErr = ex.Message;
                return null;
            }
            finally
            {
                Close();
            }
        }

        public DataTable ExecuteQuery_DataTable(string strSql, out string strErr)
        {
            strErr = string.Empty;
            DataTable dt = new DataTable();
            DbDataAdapter dataAdapter = GetDbDataAdapter(strSql);
            try
            {
                command.CommandText = strSql;
                dataAdapter.SelectCommand = command;
                dataAdapter.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                strErr = ex.Message;
                return null;
            }
            finally
            {
                Close();
            }
        }

        public string ExecuteQuery_String(string strSql, out string strErr)
        {
            strErr = string.Empty;
            DataTable dt = new DataTable();
            DbDataAdapter dataAdapter = GetDbDataAdapter(strSql);
            try
            {
                command.CommandText = strSql;
                dataAdapter.SelectCommand = command;
                dataAdapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return Convert.ToString(dt.Rows[0][0]);
                }

                return null;
            }
            catch (Exception ex)
            {
                strErr = ex.Message;
                return null;
            }
            finally
            {
                Close();
            }
        }

        public int UpdateTable(DataTable dt, string strSql, out string strErr)
        {
            strErr = string.Empty;
            DbDataAdapter dataAdapter = GetDbDataAdapter(strSql);
            DbCommandBuilder builder = GetDbCommandBuilder();
            try
            {
                command.CommandText = strSql;
                dataAdapter.InsertCommand = builder.GetUpdateCommand();    
                return dataAdapter.Update(dt);
            }
            catch (Exception ex)
            {
                strErr = ex.Message;
                return -1;
            }
            finally
            {
                   Close();
            }
        }
    }
}