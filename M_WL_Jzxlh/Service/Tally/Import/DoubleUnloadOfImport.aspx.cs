﻿//
//文件名：    DoubleUnloadOfImport.aspx.cs
//功能描述：  进口双吊卸箱作业
//创建时间：  2018/5/28
//作者：      sh
//修改时间：  
//修改描述：  暂无
//
using Newtonsoft.Json;
using ServiceInterface.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace M_YKT_Ysfw.Service.Tally.Import
{
    public partial class DoubleUnloadOfImport : System.Web.UI.Page
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
            tallyE.StrTruck_No = Request.Params["Truck_No"];
            tallyE.StrWork_No = Request.Params["Work_No"];
            tallyE.StrTeam_No = Request.Params["Team_No"];
            tallyE.StrHoliady_Mark = Request.Params["Holiady_Mark"];
            tallyE.StrTime = Request.Params["Time"];
            tallyE.StrNewShip_Id = Request.Params["NewShip_Id"];
            tallyE.StrBerth_No = Request.Params["Berth_No"];
            string strContainer_No2 = Request.Params["Container_No2"];


            //strShip_Id = "6887";
            //strContainer_No = "185";

            if (tallyE.StrShip_Id == null || tallyE.StrContainer_No == null || strContainer_No2 == null || tallyE.StrTruck_No == null || tallyE.StrWork_No == null || tallyE.StrTeam_No == null || tallyE.StrHoliady_Mark == null || tallyE.StrNight_Mark == null || tallyE.StrTime == null)
            {
                Json = Result.Failure("参数错误");
                return;
            }

            Log log = new Log(Request, tallyE);
            log.strBehavior = "进口双吊卸箱作业";
            log.strBehaviorURL = "/Tally/Import/DoubleUnloadOfImport.aspx";
            log.strWorkType = "tally";

            Json = new Con_Tally(log).Load_Unload(tallyE);
            Result reslut = JsonConvert.DeserializeObject<Result>(Json);
            if (reslut.IsSuccess == true && reslut.Message == null) {
                tallyE.StrContainer_No = strContainer_No2;
                Json = new Con_Tally(log).Load_Unload(tallyE);
            }
        }

        protected string Json;
    }
}