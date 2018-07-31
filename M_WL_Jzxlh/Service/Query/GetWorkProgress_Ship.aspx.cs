//
//文件名：    GetWorkProgress_Ship.aspx.cs
//功能描述：  获取作业进度
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
    public partial class GetWorkProgress_Ship : System.Web.UI.Page
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
            //进出口类型
            string strInOutType = Request.Params["InOutType"];


            if (strShip_Id == null || strInOutType == null)
            {
                Json = Result.Failure("参数错误");
                return;
            }

            Log log = new Log(Request);
            log.strBehavior = "获取作业进度";
            log.strBehaviorURL = "/Query/GetWorkProgress_Ship.aspx";
            Json = new Con_Tally(log).GetWorkProgress_Ship(strShip_Id, strInOutType);
            }

        protected string Json;
    }
}