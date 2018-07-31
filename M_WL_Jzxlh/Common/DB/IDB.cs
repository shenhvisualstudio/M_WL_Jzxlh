//文件名：    IDB.aspx.cs
//功能描述：  基本数据库访问接口
//创建时间：  2018/05/31
//作者：      
//修改时间：  暂无
//修改描述：  暂无
//
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ServiceInterface.Common
{
    interface IDB
    {
        /// <summary>
        /// 执行Select查询语句，并返回DataTable对象
        /// </summary>
        /// <param name="strSql">Sql语句</param>
        /// <param name="strErr">错误提示</param>
        /// <returns>DataTable对象</returns>
        DataTable ExecuteQuery_DataTable(string strSql, out string strErr);

        /// <summary>
        /// 执行Select查询语句，并返回DataSet对象
        /// </summary>
        /// <param name="strSql">Sql语句</param>
        /// <param name="strErr">错误提示</param>
        /// <returns>DataSet对象</returns>
        DataSet ExecuteQuery_DataSet(string strSql, out string strErr);

        /// <summary>
        /// 执行Select查询语句，并返回string对象
        /// </summary>
        /// <param name="strSql">Sql语句</param>
        /// <param name="strErr">错误提示</param>
        /// <returns>string对象</returns>
        string ExecuteQuery_String(string strSql, out string strErr);

        /// <summary>
        /// 执行非Select查询语句，包括UPDATE DELETE INSERT
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <param name="strErr">错误提示</param>
        /// <returns>DataTable对象</returns>
        int ExecuteNonQuery(string strSql, out string strErr);

        /// <summary>
        /// 更新数据表
        /// </summary>
        /// <param name="dt">DataTable对象</param>
        /// <param name="strSql">sql语句</param>
        /// <param name="strErr">错误提示</param>
        /// <returns></returns>
        int UpdateTable(DataTable dt, string strSql, out string strErr);

     



    }
}