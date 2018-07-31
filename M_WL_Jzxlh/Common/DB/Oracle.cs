//文件名：    Oracle.aspx.cs
//功能描述：  Oracle数据库访问类
//创建时间：  2018/05/31
//作者：      sh
//修改时间：  暂无
//修改描述：  暂无
//
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.OracleClient;
using System.Linq;
using System.Web;

namespace ServiceInterface.Common
{
    public class Oracle : DBHelper
    {
        /// <summary>
        /// 监听对象
        /// </summary>
        private string strTns;
         
        /// <summary>
        ///  构造函数
        /// </summary>
        /// <param name="strTns">监听对象</param>
        private Oracle(string strTns) {
            this.strTns = strTns;
            Init();
        }

        /// <summary>
        ///  创建Oracle对象
        /// </summary>
        /// <param name="strTn">监听对象</param>
        public static Oracle DataAccess(string strTns) {
            return new Oracle(strTns);
        }

        protected override DbConnection GetConnection()
        {
            return new OracleConnection(strTns);
        }

        protected override DbCommandBuilder GetDbCommandBuilder()
        {
            return new OracleCommandBuilder();
        }

        protected override DbDataAdapter GetDbDataAdapter(string strSql)
        {
            return new OracleDataAdapter(strSql, GetConnection() as OracleConnection);
        }
    }
}