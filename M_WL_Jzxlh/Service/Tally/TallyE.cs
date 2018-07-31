//
//文件名：    TallyE.cs
//功能描述：  理货数据模型
//创建时间：  2018/7/18
//作者：      sh
//修改时间：  
//修改描述：  暂无
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace M_YKT_Ysfw.Service.Tally
{
    public class TallyE
    {
        /// <summary>
        /// 航次ID
        /// </summary>
        private string strShip_Id = null;
        /// <summary>
        /// 箱号
        /// </summary>
        private string strContainer_No = null;
        /// <summary>
        /// 贝位号
        /// </summary>
        private string strBay_No = null;
        /// <summary>
        /// 工号
        /// </summary>
        private string strWork_No = null;
        /// <summary>
        /// 工班号
        /// </summary>
        private string strTeam_No = null;
        /// <summary>
        /// 节假日标志
        /// </summary>
        private string strHoliady_Mark = "0";
        /// <summary>
        /// 夜班标志
        /// </summary>
        private string strNight_Mark = "01";
        /// <summary>
        /// 时间
        /// </summary>
        private string strTime = null;
        /// <summary>
        /// 时间
        /// </summary>
        private string strEndTime = null;
        /// <summary>
        /// 新航次ID
        /// </summary>
        private string strNewShip_Id = null;
        /// <summary>
        /// 泊位号
        /// </summary>
        private string strBerth_No = null;
        /// <summary>
        /// 进出口类型
        /// </summary>
        private string strInOutType = "I";
        /// <summary>
        /// 倒箱标志
        /// </summary>
        private string strMovedMark = "0";
        /// <summary>
        /// 车号
        /// </summary>
        private string strTruck_No = null;
        /// <summary>
        /// 用户名
        /// </summary>
        private string strUserName = null;
        /// <summary>
        /// 理货单编号
        /// </summary>
        private string strBhno = null;

        public string StrShip_Id
        {
            get
            {
                return strShip_Id;
            }

            set
            {
                strShip_Id = value;
            }
        }

        public string StrContainer_No
        {
            get
            {
                return strContainer_No;
            }

            set
            {
                strContainer_No = value;
            }
        }

        public string StrBay_No
        {
            get
            {
                return strBay_No;
            }

            set
            {
                strBay_No = value;
            }
        }

        public string StrWork_No
        {
            get
            {
                return strWork_No;
            }

            set
            {
                strWork_No = value;
            }
        }

        public string StrTeam_No
        {
            get
            {
                return strTeam_No;
            }

            set
            {
                strTeam_No = value;
            }
        }

        public string StrHoliady_Mark
        {
            get
            {
                return strHoliady_Mark;
            }

            set
            {
                strHoliady_Mark = value;
            }
        }

        public string StrNight_Mark
        {
            get
            {
                return strNight_Mark;
            }

            set
            {
                strNight_Mark = value;
            }
        }

        public string StrTime
        {
            get
            {
                return strTime;
            }

            set
            {
                strTime = value;
            }
        }

        public string StrEndTime
        {
            get
            {
                return strEndTime;
            }

            set
            {
                strEndTime = value;
            }
        }

        public string StrNewShip_Id
        {
            get
            {
                return strNewShip_Id;
            }

            set
            {
                strNewShip_Id = value;
            }
        }

        public string StrBerth_No
        {
            get
            {
                return strBerth_No;
            }

            set
            {
                strBerth_No = value;
            }
        }

        public string StrInOutType
        {
            get
            {
                return strInOutType;
            }

            set
            {
                strInOutType = value;
            }
        }

        public string StrMovedMark
        {
            get
            {
                return strMovedMark;
            }

            set
            {
                strMovedMark = value;
            }
        }

        public string StrTruck_No
        {
            get
            {
                return strTruck_No;
            }

            set
            {
                strTruck_No = value;
            }
        }

        public string StrUserName
        {
            get
            {
                return strUserName;
            }

            set
            {
                strUserName = value;
            }
        }

        public string StrBhno
        {
            get
            {
                return strBhno;
            }

            set
            {
                strBhno = value;
            }
        }
    }
}