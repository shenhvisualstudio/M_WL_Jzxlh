//
//文件名：    Log.aspx.cs
//功能描述：  系统日志类（理货）
//创建时间：  2018/07/17
//作者：      
//修改时间：  暂无
//修改描述：  暂无
//
using M_YKT_Ysfw.Service.Tally;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceInterface.Common
{
    /// <summary>
    /// 系统日志服务
    /// </summary>
    public class Log
    {
        //航次ID
        public string strShip_Id { get; set; }
        //操作类别
        public string strWorkType { get; set; }
        //倒箱标志
        public string strMovedMark{ get; set; }
        //箱号
        public string strContainer_No { get; set; }
        //贝位号
        public string strBay_No { get; set; }
        //车号
        public string strTruck_No { get; set; }
        //进出口类型
        public string strInOutType { get; set; }
        //工号
        public string strWork_No { get; set; }
        //记录时间
        public string strInsTime { get; set; }
        //设备类型
        public string strDeviceType { get; set; }
        //IP
        public string strIP { get; set; }
        //操作行为
        public string strBehavior { get; set; }
        //操作行为URL（相对地址）
        public string strBehaviorURL { get; set; }
        //用户名
        public string strUserName { get; set; }


        #region 公共方法
        /// <summary>
        /// 初始化系统日志数据
        /// </summary>
        public Log(HttpRequest Request, TallyE tallyE)
        {
            strShip_Id = tallyE.StrShip_Id;
            strWorkType = null;
            strMovedMark = tallyE.StrMovedMark;
            strContainer_No = tallyE.StrContainer_No;
            strBay_No = tallyE.StrBay_No;
            strTruck_No = tallyE.StrTruck_No;
            strInOutType = tallyE.StrInOutType;
            strWork_No = tallyE.StrWork_No;
            strDeviceType = Request.ServerVariables["Http_User_Agent"].ToString();
            strIP = Request.ServerVariables.Get("Remote_Addr").ToString();
            strBehavior = null;
            strBehaviorURL = null;
            strUserName = tallyE.StrUserName;
        }

        public Log(HttpRequest Request)
        {
            strDeviceType = Request.ServerVariables["Http_User_Agent"].ToString();
            strIP = Request.ServerVariables.Get("Remote_Addr").ToString();
            strBehavior = null;
            strBehaviorURL = null;
        }

        #region 记录操作成功日志
        /// <summary>
        /// 记录操作成功日志
        /// </summary>
        /// <param name="strMessage">信息备注（不超过200个字符）</param>
        public void LogCatalogSuccess(string strMessage)
        {
            //错误提示
            string strErr = string.Empty;
            string strSql =
                    string.Format(@"insert into CON_INFO (ship_id,worktype,dx,cntr,bayno,truckno,ioport,workno,message,devicetype,ip,behavior,result,behaviorurl,username)
                                    values({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}')",
                                    Convert.ToInt16(strShip_Id), strWorkType, strMovedMark, strContainer_No, strBay_No, strTruck_No, strInOutType,
                                    strWork_No, strMessage, strDeviceType, strIP, strBehavior, "成功", strBehaviorURL, strUserName);
            Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteNonQuery(strSql, out strErr);
        }

        /// <summary>
        /// 记录操作成功日志
        /// </summary>
        public void LogCatalogSuccess()
        {
            //错误提示
            string strErr = string.Empty;
            string strSql =
                    string.Format(@"insert into CON_INFO (ship_id,worktype,dx,cntr,bayno,truckno,ioport,workno,message,devicetype,ip,behavior,result,behaviorurl,username)
                                    values({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}')",
                                    Convert.ToInt16(strShip_Id), strWorkType, strMovedMark, strContainer_No, strBay_No, strTruck_No, strInOutType,
                                    strWork_No, null, strDeviceType, strIP, strBehavior, "成功", strBehaviorURL, strUserName);
            Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteNonQuery(strSql, out strErr);
        }
        #endregion

        #region 记录操作失败日志
        /// <summary>
        /// 记录操作失败日志
        /// </summary>
        /// <param name="strReason">失败原因（不超过200个字符）</param>
        public void LogCatalogFailure(string strReason)
        {
            //错误提示
            string strErr = string.Empty;
            string strSql =
                    string.Format(@"insert into CON_INFO (ship_id,worktype,dx,cntr,bayno,truckno,ioport,workno,message,devicetype,ip,behavior,result,behaviorurl,username)
                                    values({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}')",
                                    Convert.ToInt16(strShip_Id), strWorkType, strMovedMark, strContainer_No, strBay_No, strTruck_No, strInOutType,
                                    strWork_No, strReason, strDeviceType, strIP, strBehavior, "失败", strBehaviorURL, strUserName);
            Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteNonQuery(strSql, out strErr);
        }
        #endregion
        #endregion








    }
}