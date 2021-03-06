﻿
//文件名：    InterfaceTool.aspx.cs
//功能描述：  接口工具类
//创建时间：  2018/05/31
//作者：      
//修改时间：  暂无
//修改描述：  暂无
//
using ServiceInterface.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace ServiceInterface.Common
{
    public class InterfaceTool
    {
        #region 移动接口身份校验
        /// <summary>
        /// 移动接口身份校验
        /// </summary>
        /// <param name="Request">Http请求对象</param>
        /// <returns></returns>
        public static bool IdentityVerify(HttpRequest Request)
        {
            //应用名称
            string strAppName = Request.Params["AppName"]; 
            //签名
            string strSign = Request.Params["Sign"];

            if (string.IsNullOrWhiteSpace(strAppName) || string.IsNullOrWhiteSpace(strSign))
            {
                return false;
            }

            //错误信息
            string strErr = string.Empty;
            //拼接令牌
            string strSql =
                string.Format(@"select token 
                                from VW_APP_TOKEN 
                                where appname='{0}'",
                                strAppName);
            string strToken = Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Ma).ExecuteQuery_String(strSql, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr) || string.IsNullOrWhiteSpace(strToken)) {
                return false;
            }

            //键值数组
            string[] strKeyList = null;
            if (Request.QueryString.Count != 0)
            {
                strKeyList = new string[Request.QueryString.Count];
                Request.QueryString.AllKeys.CopyTo(strKeyList, 0);
            }
            else if (Request.Form.Count != 0)
            {
                strKeyList = new string[Request.Form.Count];
                Request.Form.AllKeys.CopyTo(strKeyList, 0);
            }
            
            //键值排序
            Array.Sort(strKeyList);
            string strSet = string.Empty;
            //拼接（参数名+参数）
            for (int iKey = 0; iKey < strKeyList.Length; iKey++)
            {
                string strKey = strKeyList[iKey];
                if (strKey == "Sign")
                {
                    continue;
                }
                strSet += strKeyList[iKey] + Request.Params[strKey];
            }
            strSet += strToken;
            //身份校验(拼接字符串MD5加密与签名比较)
            if (EncryptionTool.MD5_Encrypt(strSet) != strSign.ToUpper())
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}