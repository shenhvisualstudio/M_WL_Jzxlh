//
//文件名：    Login.aspx.cs
//功能描述：  登录
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

namespace M_YKT_Ysfw.Service.Entrance
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!InterfaceTool.IdentityVerify(Request))
            {
                Json = Result.Failure("身份认证错误");
                return;
            }

            //用户名
            string strAccount = Request.Params["Account"];
            //密码
            string strPassword = Request.Params["Password"];

            //strAccount = "wlgch";
            //strPassword = "~";

            if (strAccount == null || strPassword == null)
            {
                Json = Result.Failure("参数错误");
                return;
            }

            Log log = new Log(Request);
            log.strBehavior = "登录";
            log.strBehaviorURL = "/Entrance/Login.aspx";
            log.strUserName = strAccount;
            log.strWorkType = "login";         

            Json = new Con_Tally(log).Login(strAccount, strPassword);
        }

        protected string Json;
    }
}