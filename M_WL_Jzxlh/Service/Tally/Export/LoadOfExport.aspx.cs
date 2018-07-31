//
//文件名：    LoadOfExport.aspx.cs
//功能描述：  出口装箱作业
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
    public partial class LoadOfExport : System.Web.UI.Page
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
            tallyE.StrTeam_No = Request.Params["Team_No"];
            tallyE.StrHoliady_Mark = Request.Params["Holiady_Mark"];
            tallyE.StrTime = Request.Params["Time"];
            tallyE.StrEndTime = Request.Params["EndTime"];
            tallyE.StrNewShip_Id = Request.Params["NewShip_Id"];
            tallyE.StrBerth_No = Request.Params["Berth_No"];
            tallyE.StrInOutType = "E";


            //strShip_Id = "6887";
            //strContainer_No = "185";

            if (tallyE.StrShip_Id == null || tallyE.StrContainer_No == null || tallyE.StrBay_No == null || tallyE.StrWork_No == null || tallyE.StrTeam_No == null || tallyE.StrTime == null)
            {
                Json = Result.Failure("参数错误");
                return;
            }

            Log log = new Log(Request, tallyE);
            log.strBehavior = "出口装箱作业";
            log.strBehaviorURL = "/Tally/Export/LoadOfExport.aspx";
            log.strWorkType = "tally";
            Json = new Con_Tally(log).Load_Unload(tallyE);
        }

        protected string Json;
    }
}