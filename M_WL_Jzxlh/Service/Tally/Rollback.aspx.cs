//
//文件名：    Rollback.aspx.cs
//功能描述：  理箱
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

namespace M_YKT_Ysfw.Service.Tally
{
    public partial class Rollback : System.Web.UI.Page
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
            tallyE.StrNewShip_Id = Request.Params["NewShip_Id"];
            tallyE.StrContainer_No = Request.Params["Container_No"];
            tallyE.StrWork_No = Request.Params["Work_No"];

            if (tallyE.StrShip_Id == null || tallyE.StrNewShip_Id == null || tallyE.StrContainer_No == null || tallyE.StrWork_No == null)
            {
                Json = Result.Failure("参数错误");
                return;
            }

            Log log = new Log(Request, tallyE);
            log.strBehavior = "理箱回滚";
            log.strBehaviorURL = "/Tally/Rollback.aspx";
            log.strWorkType = "rollback";
            Json = new Pub(log).Rollback(tallyE);
        }

        protected string Json;
    }
}