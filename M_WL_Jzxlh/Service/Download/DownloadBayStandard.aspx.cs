//
//文件名：    DownloadBayStandard.aspx.cs
//功能描述：  下载贝位规范
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
    public partial class DownloadBayStandard : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!InterfaceTool.IdentityVerify(Request))
            {
                Json = Result.Failure("身份认证错误");
                return;
            }

            //航次ID
            string strV_Id = Request.Params["V_Id"];
            //strV_Id = "2664";

            if (strV_Id == null)
            {
                Json = Result.Failure("参数错误");
                return;
            }

            Log log = new Log(Request);
            log.strBehavior = "下载贝位规范";
            log.strBehaviorURL = "/Download/DownloadBayStandard.aspx";
            Json = new Con_Tally(log).DownloadBayStandard(strV_Id);


        }

        protected string Json;
    }
}