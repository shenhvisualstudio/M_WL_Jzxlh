//
//文件名：    GetWorkProgress_Perssonal.aspx.cs
//功能描述：  获取个人作业进度
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
    public partial class GetWorkProgress_Perssonal : System.Web.UI.Page
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
            //工号
            string strWork_No = Request.Params["Work_No"];


            if (strShip_Id == null || strInOutType == null || strWork_No == null)
            {
                Json = Result.Failure("参数错误");
                return;
            }

            Log log = new Log(Request);
            log.strBehavior = "获取个人作业进度";
            log.strBehaviorURL = "/Query/GetWorkProgress_Perssonal.aspx";
            Json = new Con_Tally(log).GetWorkProgress_Perssonal(strShip_Id, strInOutType, strWork_No);
            }

        protected string Json;
    }
}