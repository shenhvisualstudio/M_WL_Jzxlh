//文件名：    SqlServer.aspx.cs
//功能描述：  SqlServer数据库访问类
//创建时间：  2018/05/31
//作者：      sh
//修改时间：  暂无
//修改描述：  暂无
//
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ServiceInterface.Common
{
    public class SqlServer : DBHelper
    {
        /// <summary>
        /// 监听对象
        /// </summary>
        private string strTns;

        /// <summary>
        ///  构造函数
        /// </summary>
        /// <param name="strTns">监听对象</param>
        private SqlServer(string strTns)
        {
            this.strTns = strTns;
            Init();
        }

        /// <summary>
        ///  创建SqlServer对象
        /// </summary>
        /// <param name="strTns">监听对象</param>
        public static SqlServer DataAccess(string strTns)
        {
            return new SqlServer(strTns);
        }

        protected override DbConnection GetConnection()
        {
            return new SqlConnection(strTns);
        }

        protected override DbCommandBuilder GetDbCommandBuilder()
        {
            return new SqlCommandBuilder();
        }

        protected override DbDataAdapter GetDbDataAdapter(string strSql)
        {
            return new SqlDataAdapter(strSql, GetConnection() as SqlConnection);
        }
    }
}