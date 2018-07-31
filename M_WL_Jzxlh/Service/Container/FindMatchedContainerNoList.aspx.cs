//
//文件名：    FindMatchedContainerNoList.aspx.cs
//功能描述：  查找匹配箱号列表
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

namespace M_YKT_Ysfw.Service.Container
{
    public partial class FindMatchedContainerNoList : System.Web.UI.Page
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
            //查询箱号
            string strContainer_No = Request.Params["Container_No"];

            //strShip_Id = "6887";
            //strContainer_No = "185";

            if (strShip_Id == null || strContainer_No == null)
            {
                Json = Result.Failure("参数错误");
                return;
            }

            Log log = new Log(Request);
            log.strBehavior = "查找匹配箱号列表";
            log.strBehaviorURL = "/Container/FindMatchedContainerNoList.aspx";
            Json = new Con_Tally(log).FindMatchedContainerNoList(strShip_Id,strContainer_No);


        }

        protected string Json;
    }
}