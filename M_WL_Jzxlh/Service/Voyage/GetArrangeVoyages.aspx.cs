//
//文件名：    GetArrangeVoyages.aspx.cs
//功能描述：  获取派工航次
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

namespace M_YKT_Ysfw.Service.Voyage
{
    public partial class GetArrangeVoyages : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!InterfaceTool.IdentityVerify(Request))
            {
                Json = Result.Failure("身份认证错误");
                return;
            }

            //工号
            string strWork_No = Request.Params["Work_No"];
            //作业日期
            string strWork_Date = Request.Params["Work_Date"];
            //夜班标志
            string strNight_Mark = Request.Params["Night_Mark"];

            //strWork_No = "020974";
            //strWork_Date = "2018-06-04 09:33:02";
            //strNight_Mark = "01";

            if (strWork_No == null || strWork_Date == null || strNight_Mark == null)
            {
                Json = Result.Failure("参数错误");
                return;
            }

            Log log = new Log(Request);
            log.strBehavior = "获取派工航次";
            log.strBehaviorURL = "/Voyage/GetArrangeVoyages.aspx";
            Json = new Con_Tally(log).GetArrangeVoyages(strWork_No, strWork_Date, strNight_Mark);
        }

        protected string Json;
    }
}