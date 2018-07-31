//
//文件名：    MoveBay.aspx.cs
//功能描述：  出口调贝
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

namespace M_YKT_Ysfw.Service.Tally.Export
{
    public partial class MoveBay : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!InterfaceTool.IdentityVerify(Request))
            {
                Json = Result.Failure("身份认证错误");
                return;
            }

            TallyE tallyE = new TallyE();
            tallyE.StrShip_Id = Request.Params["Ship_Id"];
            tallyE.StrContainer_No = Request.Params["Container_No"];
            tallyE.StrBay_No = Request.Params["Bay_No"];
            tallyE.StrWork_No = Request.Params["Work_No"];
            tallyE.StrTime = Request.Params["Time"];

            //strShip_Id = "6887";
            //strContainer_No = "185";

            if (tallyE.StrShip_Id == null || tallyE.StrContainer_No == null || tallyE.StrBay_No == null || tallyE.StrWork_No == null || tallyE.StrTime == null)
            {
                Json = Result.Failure("参数错误");
                return;
            }

            Log log = new Log(Request, tallyE);
            log.strBehavior = "出口调贝";
            log.strBehaviorURL = "/Tally/Export/MoveBay.aspx";
            log.strWorkType = "chgbay";
            Json = new Con_Tally(log).MoveBay(tallyE);
        }

        protected string Json;
    }
}