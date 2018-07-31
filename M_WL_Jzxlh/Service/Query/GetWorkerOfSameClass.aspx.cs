//
//文件名：    GetWorkerOfSameClass.aspx.cs
//功能描述：  获取同班人员
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

namespace M_YKT_Ysfw.Service.Query
{
    public partial class GetWorkerOfSameClass : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!InterfaceTool.IdentityVerify(Request))
            {
                Json = Result.Failure("身份认证错误");
                return;
            }

            //航次ID
            string strShip_Id = Request.Params["Ship_Id"];
            //夜班标志
            string strNight_Mark = Request.Params["Night_Mark"];

            if (strShip_Id == null || strNight_Mark == null)
            {
                Json = Result.Failure("参数错误");
                return;
            }

            Log log = new Log(Request);
            log.strBehavior = "获取同班人员";
            log.strBehaviorURL = "/Query/GetWorkerOfSameClass.aspx";
            Json = new Con_Tally(log).GetWorkerOfSameClass(strShip_Id, strNight_Mark);
        }

        protected string Json;
    }
}