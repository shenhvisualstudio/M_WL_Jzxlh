//
//文件名：    GetDoubleContainerInfo.aspx.cs
//功能描述：  获取双吊理箱信息
//创建时间：  2018/5/28
//作者：      sh
//修改时间：  
//修改描述：  暂无
//
using ServiceInterface.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace M_YKT_Ysfw.Service.Container
{
    public partial class GetDoubleContainerInfo : System.Web.UI.Page
    {
        /// <summary>
        /// 当前操作
        /// </summary>
        private const string OPERATION = "获取双吊理箱信息";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!InterfaceTool.IdentityVerify(Request))
            {
                Json = Result.Failure("身份认证错误");
                return;
            }

            try
            {



            }
            catch (Exception ex)
            {
                Json = Result.Failure(string.Format("{0}：{1}发生异常。{2}", OPERATION, ex.Source, ex.Message));
            }
        }

        private string Json;
    }
}