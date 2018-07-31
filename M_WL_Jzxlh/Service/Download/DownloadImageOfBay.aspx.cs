//
//文件名：    DownloadImageOfBay.aspx.cs
//功能描述：  下载船图（单贝）
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
    public partial class DownloadImageOfBay : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!InterfaceTool.IdentityVerify(Request))
            //{
            //    Json = Result.Failure("身份认证错误");
            //    return;
            //}


            //航次ID
            string strShip_Id = Request.Params["Ship_Id"];
            //贝号
            string strBay_No = Request.Params["Bay_No"];
            strShip_Id = "6887";
            strBay_No = "30";

            if (strShip_Id == null || strBay_No == null)
            {
                Json = Result.Failure("参数错误");
                return;
            }

            Log log = new Log(Request);
            log.strBehavior = "下载船图";
            log.strBehaviorURL = "/Download/DownloadImageOfBay.aspx";
            Json = new Con_Tally(log).DownloadImageOfBay(strShip_Id, strBay_No);


        }

        protected string Json;
    }
}