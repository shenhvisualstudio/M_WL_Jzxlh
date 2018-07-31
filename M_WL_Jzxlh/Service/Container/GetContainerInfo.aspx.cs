//
//文件名：    GetContainerInfo.aspx.cs
//功能描述：  获取理箱信息
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
    public partial class GetContainerInfo : System.Web.UI.Page
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
            //作业类型——0普通,1出舱,2重装,3调贝,4甩箱
            string strWorkType = Request.Params["WorkType"];
            //指令标志
            string strBLInstruction = Request.Params["BLInstruction"];

            //strShip_Id = "6887";
            //strContainer_No = "TCNU5462434";
            //strWorkType = "0";
            //strBLInstruction = "1";

            if (strShip_Id == null || strContainer_No == null || strWorkType == null || strBLInstruction == null)
            {
                Json = Result.Failure("参数错误");
                return;
            }

            Log log = new Log(Request);
            log.strBehavior = "获取理箱信息";
            log.strBehaviorURL = "/Container/GetContainerInfo.aspx";
            Json = new Con_Tally(log).GetContainerInfo(strShip_Id, strContainer_No, strWorkType, strBLInstruction);
        }

        protected string Json;
    }
}