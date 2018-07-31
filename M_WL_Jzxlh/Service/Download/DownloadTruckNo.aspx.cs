//
//文件名：    DownloadTruckNo.aspx.cs
//功能描述：  下载车号
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

namespace M_YKT_Ysfw.Service.Download
{
    public partial class DownloadTruckNo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!InterfaceTool.IdentityVerify(Request))
            //{
            //    Json = Result.Failure("身份认证错误");
            //    return;
            //}

            Log log = new Log(Request);
            log.strBehavior = "下载车号";
            log.strBehaviorURL = "/Download/DownloadTruckNo.aspx";
            Json = new Con_Tally(log).DownloadTruckNo();

        }

        protected string Json;
    }
}