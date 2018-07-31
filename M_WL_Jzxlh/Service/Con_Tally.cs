using M_YKT_Ysfw.Service.Tally;
using ServiceInterface.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace M_YKT_Ysfw.Service
{
    public class Con_Tally
    {
        #region 日志对象
        /// <summary>
        /// 日志对象
        /// </summary>
        private Log log;

        public Con_Tally(Log log)
        {
            this.log = log;
        }
        #endregion

        #region 登录
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="strAccount">账户</param>
        /// <param name="strPassword">密码</param>
        /// <returns>结果</returns>
        public string Login(string strAccount, string strPassword)
        {

            //部门编码
            string strDeptCode = "26.11.12";
            //错误提示
            string strErr = string.Empty;

            string strSql = string.Format(@"select work_no 
                                            from WS_LOGIN
                                            where dept_code='{0}' and serial_nam='{1}' and password='{2}' ",
                                            strDeptCode, strAccount, strPassword);
            string strWrok_no = SqlServer.DataAccess(RegistryKey.KeyPath_SqlServer_Tally).ExecuteQuery_String(strSql, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("异常：" + strErr);
                return Result.Failure("异常：" + strErr);
            }
            if (strWrok_no == null)
            {
                log.LogCatalogFailure("pwd_err");
                return Result.Failure("用户名或密码错误");
            }

            log.strWork_No = strWrok_no;
            log.LogCatalogSuccess();
            return Result.Success(strWrok_no, null);
        }
        #endregion

        #region 获取派工航次
        /// <summary>
        /// 获取派工航次
        /// </summary>
        /// <param name="strWork_No">工号</param>
        /// <param name="strWork_Date">作业日期</param>
        /// <param name="strNight_Mark">夜班标志</param>
        /// <returns>结果</returns>
        public string GetArrangeVoyages(string strWork_No, string strWork_Date, string strNight_Mark)
        {

            //部门编码
            string strDeptCode = "26.11.12";
            //错误提示
            string strErr = string.Empty;

            string strSql =
                string.Format(@"select distinct ship_newid,ship_id,v_id,berthno,voyage,chi_vessel,inoutport,vessel_newid as v_newid 
                                from VIEW_DOWNLOAD_APP 
                                where (work_no='{0}' or work_tally='{0}') and dept_code='{1}' and daynightmark='{2}' and datediff(dd, work_day, '{3}')=0 and ship_newid is not null",
                                strWork_No, strDeptCode, strNight_Mark, strWork_Date);
            var dt = SqlServer.DataAccess(RegistryKey.KeyPath_SqlServer_Tally).ExecuteQuery_DataTable(strSql, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("异常：" + strErr);
                return Result.Failure("异常：" + strErr);
            }

            if (dt.Rows.Count <= 0)
            {
                log.LogCatalogSuccess("无派工数据");
                return Result.Success(null, null);
            }

            string[,] strArray = new string[dt.Rows.Count, 8];
            for (int iRow = 0; iRow < dt.Rows.Count; iRow++)
            {
                strArray[iRow, 0] = Convert.ToString(dt.Rows[iRow]["ship_newid"]);
                strArray[iRow, 1] = Convert.ToString(dt.Rows[iRow]["ship_id"]);
                strArray[iRow, 2] = Convert.ToString(dt.Rows[iRow]["v_id"]);
                strArray[iRow, 3] = Convert.ToString(dt.Rows[iRow]["berthno"]);
                strArray[iRow, 4] = Convert.ToString(dt.Rows[iRow]["voyage"]);
                strArray[iRow, 5] = Convert.ToString(dt.Rows[iRow]["chi_vessel"]);
                strArray[iRow, 6] = Convert.ToString(dt.Rows[iRow]["inoutport"]);
                strArray[iRow, 7] = Convert.ToString(dt.Rows[iRow]["v_newid"]);
            }

            log.LogCatalogSuccess();
            return Result.Success(strArray, null);
        }
        #endregion

        #region 下载贝位规范
        /// <summary>
        /// 下载贝位规范
        /// </summary>
        /// <param name="strV_Id">航次ID</param>
        /// <returns>结果</returns>
        public string DownloadBayStandard(string strV_Id)
        {
            //错误提示
            string strErr = string.Empty;

            string strSql =
                string.Format(@"select v_id,eng_vessel,chi_vessel,location,screen_row,screen_col,bay_num,bay_row,bay_col,occupy,user_char
                                from CODE_CON_MAP 
                                where v_id={0}",
                                strV_Id);
            var dt = Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteQuery_DataTable(strSql, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("异常：" + strErr);
                return Result.Failure("异常：" + strErr);
            }

            if (dt.Rows.Count <= 0)
            {
                log.LogCatalogSuccess("无贝位规范数据");
                return Result.Success(null, null);
            }

            string[,] strArray = new string[dt.Rows.Count, 11];
            for (int iRow = 0; iRow < dt.Rows.Count; iRow++)
            {
                strArray[iRow, 0] = Convert.ToString(dt.Rows[iRow]["v_id"]);
                strArray[iRow, 1] = Convert.ToString(dt.Rows[iRow]["eng_vessel"]);
                strArray[iRow, 2] = Convert.ToString(dt.Rows[iRow]["chi_vessel"]);
                strArray[iRow, 3] = Convert.ToString(dt.Rows[iRow]["location"]);
                strArray[iRow, 4] = Convert.ToString(dt.Rows[iRow]["screen_row"]);
                strArray[iRow, 5] = Convert.ToString(dt.Rows[iRow]["screen_col"]);
                strArray[iRow, 6] = Convert.ToString(dt.Rows[iRow]["bay_num"]);
                strArray[iRow, 7] = Convert.ToString(dt.Rows[iRow]["bay_row"]);
                strArray[iRow, 8] = Convert.ToString(dt.Rows[iRow]["bay_col"]);
                strArray[iRow, 9] = Convert.ToString(dt.Rows[iRow]["occupy"]);
                strArray[iRow, 10] = Convert.ToString(dt.Rows[iRow]["user_char"]);
            }

            log.LogCatalogSuccess();
            return Result.Success(strArray, null);
        }
        #endregion

        #region 下载船图
        /// <summary>
        /// 下载船图
        /// </summary>
        /// <param name="strShip_Id">航次ID</param>
        /// <param name="strBay_No">贝号</param>
        /// <returns>结果</returns>
        public string DownloadImageOfBay(string strShip_Id, string strBay_No)
        {
            //错误提示
            string strErr = string.Empty;

            string strSql =
                string.Format(@"select SHIP_ID, CONTAINER_NO,CONTAINER_TYPE,SIZE_CON, FULLOREMPTY,SIZE_CON || CONTAINER_TYPE || '/' || FULLOREMPTY CONTYPE,BAYNO,BAYNO POS,
                                SubStr(BAYNO,1,2) BAY,SubStr(BAYNO,3,2) BAYCOL,SubStr(BAYNO,5,2) BAYROW,SEALNO,BLNO,OLDBAYNO,AMOUNT,GROSSWEIGHT,GROSSWEIGHT/1000 WEIGHT,
                                VOLUME,CODE_LOAD_PORT,CODE_UNLOAD_PORT,TPMARK,EDITBAYMARK,MOVED,PASS_MARK,SHORT_UNLOAD,UNLOAD_MARK,WORK_NO,WORK_DATE,CODE_CON_COMPANY,USER_NAME,OPER_TIME
                                from CON_IMAGE 
                                where SHIP_ID='{0}' and SubStr(BAYNO,1,2)='{1}'",
                                strShip_Id, strBay_No);
            var dt = Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteQuery_DataTable(strSql, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("异常：" + strErr);
                return Result.Failure("异常：" + strErr);
            }

            if (dt.Rows.Count <= 0)
            {
                log.LogCatalogSuccess("无船图数据");
                return Result.Success(null, null);
            }

            string[,] strArray = new string[dt.Rows.Count, 31];
            for (int iRow = 0; iRow < dt.Rows.Count; iRow++)
            {
                strArray[iRow, 0] = Convert.ToString(dt.Rows[iRow]["SHIP_ID"]);
                strArray[iRow, 1] = Convert.ToString(dt.Rows[iRow]["CONTAINER_NO"]);
                strArray[iRow, 2] = Convert.ToString(dt.Rows[iRow]["CONTAINER_TYPE"]);
                strArray[iRow, 3] = Convert.ToString(dt.Rows[iRow]["SIZE_CON"]);
                strArray[iRow, 4] = Convert.ToString(dt.Rows[iRow]["FULLOREMPTY"]);
                strArray[iRow, 5] = Convert.ToString(dt.Rows[iRow]["CONTYPE"]);
                strArray[iRow, 6] = Convert.ToString(dt.Rows[iRow]["BAYNO"]);
                strArray[iRow, 7] = Convert.ToString(dt.Rows[iRow]["POS"]);
                strArray[iRow, 8] = Convert.ToString(dt.Rows[iRow]["BAY"]);
                strArray[iRow, 9] = Convert.ToString(dt.Rows[iRow]["BAYCOL"]);
                strArray[iRow, 10] = Convert.ToString(dt.Rows[iRow]["BAYROW"]);
                strArray[iRow, 11] = Convert.ToString(dt.Rows[iRow]["SEALNO"]);
                strArray[iRow, 12] = Convert.ToString(dt.Rows[iRow]["BLNO"]);
                strArray[iRow, 13] = Convert.ToString(dt.Rows[iRow]["OLDBAYNO"]);
                strArray[iRow, 14] = Convert.ToString(dt.Rows[iRow]["AMOUNT"]);
                strArray[iRow, 15] = Convert.ToString(dt.Rows[iRow]["GROSSWEIGHT"]);
                strArray[iRow, 16] = Convert.ToString(dt.Rows[iRow]["WEIGHT"]);
                strArray[iRow, 17] = Convert.ToString(dt.Rows[iRow]["VOLUME"]);
                strArray[iRow, 18] = Convert.ToString(dt.Rows[iRow]["CODE_LOAD_PORT"]);
                strArray[iRow, 19] = Convert.ToString(dt.Rows[iRow]["CODE_UNLOAD_PORT"]);
                strArray[iRow, 20] = Convert.ToString(dt.Rows[iRow]["TPMARK"]);
                strArray[iRow, 21] = Convert.ToString(dt.Rows[iRow]["EDITBAYMARK"]);
                strArray[iRow, 22] = Convert.ToString(dt.Rows[iRow]["MOVED"]);
                strArray[iRow, 23] = Convert.ToString(dt.Rows[iRow]["PASS_MARK"]);
                strArray[iRow, 24] = Convert.ToString(dt.Rows[iRow]["SHORT_UNLOAD"]);
                strArray[iRow, 25] = Convert.ToString(dt.Rows[iRow]["UNLOAD_MARK"]);
                strArray[iRow, 26] = Convert.ToString(dt.Rows[iRow]["WORK_NO"]);
                strArray[iRow, 27] = Convert.ToString(dt.Rows[iRow]["WORK_DATE"]);
                strArray[iRow, 28] = Convert.ToString(dt.Rows[iRow]["CODE_CON_COMPANY"]);
                strArray[iRow, 29] = Convert.ToString(dt.Rows[iRow]["USER_NAME"]);
                strArray[iRow, 30] = Convert.ToString(dt.Rows[iRow]["OPER_TIME"]);

            }

            log.LogCatalogSuccess();
            return Result.Success(strArray, null);
        }
        #endregion

        #region 下载车号
        /// <summary>
        /// 下载车号
        /// </summary>
        /// <returns>结果</returns>
        public string DownloadTruckNo()
        {
            //错误提示
            string strErr = string.Empty;

            //注：要完善数据库表，添加车队编码和车队名称，只获取新东方和PSA的车队即可
            string strSql =
                string.Format(@"select code_jzx,code_hik,code_pda,truck,c_team,n_team 
                                from CODE_TRUCK_JZX");
            var dt = Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteQuery_DataTable(strSql, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("异常：" + strErr);
                return Result.Failure("异常：" + strErr);
            }

            if (dt.Rows.Count <= 0)
            {
                log.LogCatalogSuccess("无车号数据");
                return Result.Success(null, null);
            }

            string[,] strArray = new string[dt.Rows.Count, 6];
            for (int iRow = 0; iRow < dt.Rows.Count; iRow++)
            {
                strArray[iRow, 0] = Convert.ToString(dt.Rows[iRow]["code_jzx"]);
                strArray[iRow, 1] = Convert.ToString(dt.Rows[iRow]["code_hik"]);
                strArray[iRow, 2] = Convert.ToString(dt.Rows[iRow]["code_pda"]);
                strArray[iRow, 3] = Convert.ToString(dt.Rows[iRow]["truck"]);
                strArray[iRow, 4] = Convert.ToString(dt.Rows[iRow]["c_team"]);
                strArray[iRow, 5] = Convert.ToString(dt.Rows[iRow]["n_team"]);
            }

            log.LogCatalogSuccess();
            return Result.Success(strArray, null);
        }
        #endregion

        #region 查找匹配箱号列表
        /// <summary>
        /// 查找匹配箱号列表
        /// </summary>
        /// <param name="strShip_Id">航次ID</param>
        /// <param name="strContainer_No">查询箱号</param>
        /// <returns>结果</returns>
        public string FindMatchedContainerNoList(string strShip_Id, string strContainer_No)
        {
            //错误提示
            string strErr = string.Empty;

            string strSql =
                string.Format(@"select distinct container_no 
                                from CON_IMAGE 
                                where ship_id='{0}' and container_no like '%{1}'",
                                strShip_Id, strContainer_No);
            var dt = Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteQuery_DataTable(strSql, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("异常：" + strErr);
                return Result.Failure("异常：" + strErr);
            }

            if (dt.Rows.Count <= 0)
            {
                log.LogCatalogSuccess("无匹配箱号数据");
                return Result.Success(null, null);
            }

            string[] strArray = new string[dt.Rows.Count];
            for (int iRow = 0; iRow < dt.Rows.Count; iRow++)
            {
                strArray[iRow] = Convert.ToString(dt.Rows[iRow]["container_no"]);
            }

            log.LogCatalogSuccess();
            return Result.Success(strArray, null);
        }
        #endregion

        #region 获取理箱信息
        /// <summary>
        /// 获取理箱信息
        /// </summary>
        /// <param name="strShip_Id">航次ID</param>
        /// <param name="strContainer_No">箱号</param>
        /// <param name="strWorkType">作业类型——0普通,1出舱,2重装,3调贝,4甩箱</param>
        /// <param name="strBLInstruction">指令标志</param>
        /// <returns>结果</returns>
        public string GetContainerInfo(string strShip_Id, string strContainer_No, string strWorkType, string strBLInstruction)
        {
            //错误提示
            string strErr = string.Empty;
            //卸箱标志
            string strUnloadMark = "1";
            if (strWorkType.Equals("0") || strWorkType.Equals("1") || strWorkType.Equals("2"))
            {
                strUnloadMark = "0";
            }

            //查询船图箱信息
            string strSql_image =
                string.Format(@"select bayno,sealno,moved,code_unload_port,code_load_port,size_con,container_type,fullorempty
                                from CON_IMAGE  where ship_id='{0}' and container_no ='{1}' and unload_mark='{2}'",
                                strShip_Id, strContainer_No, strUnloadMark);
            var dt_image = Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteQuery_DataTable(strSql_image, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("异常：" + strErr);
                return Result.Failure("异常：" + strErr);
            }

            if (dt_image.Rows.Count <= 0)
            {
                log.LogCatalogFailure("无理箱信息");
                return Result.Failure(null);
            }

            string[] strArray = new string[8];
            strArray[0] = Convert.ToString(dt_image.Rows[0]["bayno"]);
            strArray[1] = Convert.ToString(dt_image.Rows[0]["sealno"]);
            strArray[2] = Convert.ToString(dt_image.Rows[0]["moved"]);
            strArray[3] = Convert.ToString(dt_image.Rows[0]["code_unload_port"]);
            strArray[4] = Convert.ToString(dt_image.Rows[0]["code_load_port"]);
            strArray[5] = Convert.ToString(dt_image.Rows[0]["size_con"]);
            strArray[6] = Convert.ToString(dt_image.Rows[0]["container_type"]);
            strArray[7] = Convert.ToString(dt_image.Rows[0]["fullorempty"]);

            //指令校验
            if (strBLInstruction.Equals("1") && strUnloadMark.Equals("0"))
            {

                string strSql_Instruction =
                    string.Format(@"select ctn_no
                                    from TB_CON_INSTRUCTION  where ship_id='{0}' and ctn_no ='{1}'",
                                    strShip_Id, strContainer_No);
                //需要在Oracle里新建指令表——查询指令升级需求
                //string strCtn_no = Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteQuery_String(strSql_Instruction, out strErr);
                string strCtn_no = SqlServer.DataAccess(RegistryKey.KeyPath_SqlServer_Tally).ExecuteQuery_String(strSql_Instruction, out strErr);
                if (!string.IsNullOrWhiteSpace(strErr))
                {
                    log.LogCatalogFailure("异常：" + strErr);
                    return Result.Failure("异常：" + strErr);
                }

                if (string.IsNullOrWhiteSpace(strCtn_no))
                {
                    log.LogCatalogSuccess("此箱无指令");
                    return Result.Success(strArray, "此箱无指令");
                }
            }

            log.LogCatalogSuccess();
            return Result.Success(strArray, null);
        }
        #endregion

        #region 进出口装卸箱作业
        /// <summary>
        /// 进出口装卸箱作业
        /// </summary>
        /// <param name="tallyE">TallyE数据对象</param>
        /// <returns>结果</returns>
        public string Load_Unload(TallyE tallyE)
        {
            //错误提示
            string strErr = string.Empty;
            //返回数据对象
            string strJson = null;

            log.LogCatalogSuccess("gb." + tallyE.StrTeam_No + ".jjr." + tallyE.StrHoliady_Mark + ".yb." + tallyE.StrNight_Mark);

            //作业校验
            strJson = new Pub(log).CheckWork(tallyE);
            if (!string.IsNullOrWhiteSpace(strJson))
            {
                return strJson;
            }

            try
            {
                //同步舱单
                strJson = new Pub(log).UpdateHatch(tallyE);
                if (!string.IsNullOrWhiteSpace(strJson))
                {
                    return strJson;
                }
                //同步船图
                strJson = new Pub(log).UpdateImage(tallyE);
                if (!string.IsNullOrWhiteSpace(strJson))
                {
                    return strJson;
                }
                //更新配载指令
                strJson = new Pub(log).UpdateInstruction(tallyE);
                if (!string.IsNullOrWhiteSpace(strJson))
                {
                    return strJson;
                }
                //生成理货单编号
                strJson = new Pub(log).GenerateTallyNum(tallyE);
                if (!string.IsNullOrWhiteSpace(strJson))
                {
                    return strJson;
                }
                //同步理箱单主子表
                strJson = new Pub(log).UpdateTallyList(tallyE);
                if (!string.IsNullOrWhiteSpace(strJson))
                {
                    return strJson;
                }

                log.LogCatalogSuccess("tally_ok");

                //发送指令
                strJson = new Pub(log).SendInstruction(tallyE);
                if (!string.IsNullOrWhiteSpace(strJson))
                {
                    return strJson;
                }

                return Result.Success(null, null);
            }
            catch (Exception ex)
            {
                log.strWorkType = "load_unload_ec";
                log.LogCatalogFailure("异常：" + ex.Message);
                return new Pub(log).Rollback(tallyE);
                //return Result.Failure("异常：" + ex.Message);
            }
        }
        #endregion

        #region 捣箱进出口装卸箱作业
        /// <summary>
        /// 捣箱进出口装卸箱作业
        /// </summary>
        /// <param name="tallyE">TallyE数据对象</param>
        /// <returns>结果</returns>
        public string Moved_Load_Unload(TallyE tallyE)
        {
            //错误提示
            string strErr = string.Empty;
            //返回数据对象
            string strJson = null;

            log.LogCatalogSuccess("gb." + tallyE.StrTeam_No + ".jjr." + tallyE.StrHoliady_Mark + ".yb." + tallyE.StrNight_Mark);

            //作业校验
            strJson = new Pub(log).Moved_CheckWork(tallyE);
            if (!string.IsNullOrWhiteSpace(strJson))
            {
                return strJson;
            }

            try
            {
                //同步船图
                strJson = new Pub(log).Moved_UpdateImage(tallyE);
                if (!string.IsNullOrWhiteSpace(strJson))
                {
                    return strJson;
                }
                //更新配载指令
                strJson = new Pub(log).Moved_UpdateInstruction(tallyE);
                if (!string.IsNullOrWhiteSpace(strJson))
                {
                    return strJson;
                }
                //生成理货单编号
                strJson = new Pub(log).Moved_GenerateTallyNum(tallyE);
                if (!string.IsNullOrWhiteSpace(strJson))
                {
                    return strJson;
                }
                //同步理箱单主子表
                strJson = new Pub(log).UpdateTallyList(tallyE);
                if (!string.IsNullOrWhiteSpace(strJson))
                {
                    return strJson;
                }

                log.LogCatalogSuccess("tally_dx_ok");

                //调指令发送码头
                strJson = new Pub(log).SendInstruction(tallyE);
                if (!string.IsNullOrWhiteSpace(strJson))
                {
                    return strJson;
                }

                return Result.Success(null, null);
            }
            catch (Exception ex)
            {
                log.strWorkType = "dxload_unload_ec";
                log.LogCatalogFailure("异常：" + ex.Message);
                return new Pub(log).Rollback(tallyE);
                //return Result.Failure("异常：" + ex.Message);
            }
        }
        #endregion

        #region 出口调贝
        /// <summary>
        /// 出口调贝
        /// </summary>
        /// <param name="tallyE">TallyE数据对象</param>
        /// <returns>结果</returns>
        public string MoveBay(TallyE tallyE)
        {
            //错误提示
            string strErr = string.Empty;
            //返回数据对象
            string strJson = null;

            log.LogCatalogSuccess();

            try
            {
                //作业校验
                strJson = new Pub(log).Moved_CheckWork(tallyE);
                if (!string.IsNullOrWhiteSpace(strJson))
                {
                    return strJson;
                }

                //如果新贝位目前存在箱号，则与当前箱所在贝位调换
                string strSql = "update CON_IMAGE set BayNo=nvl((select Max(BayNo) BayNo from con_image where SHIP_ID=" + tallyE.StrNewShip_Id + " and CONTAINER_NO ='" + tallyE.StrContainer_No + "'),'" + tallyE.StrBay_No + "')"
                    + " where SHIP_ID=" + tallyE.StrNewShip_Id + " and  BayNO='" + tallyE.StrBay_No + "'  and CONTAINER_NO !='" + tallyE.StrContainer_No + "'";
                Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteNonQuery(strSql, out strErr);
                if (!string.IsNullOrWhiteSpace(strErr))
                {
                    log.LogCatalogFailure("出口调贝异常：" + strErr);
                    return Result.Failure("出口调贝异常：" + strErr);
                }

                strSql = "update CON_IMAGE set WORK_NO='" + tallyE.StrWork_No + "',USER_NAME='" + tallyE.StrWork_No + "',BAYNO='" + tallyE.StrBay_No + "' where SHIP_ID='" + tallyE.StrNewShip_Id + "' and CONTAINER_NO='" + tallyE.StrContainer_No + "'";
                Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteNonQuery(strSql, out strErr);
                if (!string.IsNullOrWhiteSpace(strErr))
                {
                    log.LogCatalogFailure("出口调贝异常：" + strErr);
                    return Result.Failure("出口调贝异常：" + strErr);
                }

                log.LogCatalogSuccess("chgbay_ok");
                return Result.Success(null, null);
            }
            catch (Exception ex)
            {
                log.strWorkType = "chgbay_ec";
                log.LogCatalogFailure("异常：" + ex.Message);
                return Result.Failure("异常：" + ex.Message);
            }
        }
        #endregion

        #region 获取作业进度
        /// <summary>
        /// 获取作业进度
        /// </summary>
        /// <param name="strShip_Id">航次ID</param>
        /// <param name="strInOutType">进出口类型</param>
        /// <returns>结果</returns>
        public string GetWorkProgress_Ship(string strShip_Id, string strInOutType)
        {
            //错误提示
            string strErr = string.Empty;
            string strSql = string.Empty;

            strSql =
                string.Format(@"select SIZE_CON,FULLOREMPTY,Count(*) CON_TOLTAL,Sum(UNLOAD_MARK) CON_TALLY,Sum(YB) CON_YB,Sum(JJR) CON_JJR 
                                from vcon_image_pda_stat
                                where Old_ID=‘{0}’and PORT like '%LYG' group by SIZE_CON,FULLOREMPTY order by SIZE_CON,FULLOREMPTY",
                                strShip_Id);
            var dt = Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteQuery_DataTable(strSql, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("异常：" + strErr);
                return Result.Failure("异常：" + strErr);
            }

            if (dt.Rows.Count <= 0)
            {
                log.LogCatalogFailure("无作业数据");
                return Result.Failure(null);
            }

            string[,] strArray = new string[dt.Rows.Count, 6];
            for (int iRow = 0; iRow < dt.Rows.Count; iRow++)
            {
                strArray[iRow, 0] = Convert.ToString(dt.Rows[iRow]["SIZE_CON"]);
                strArray[iRow, 1] = Convert.ToString(dt.Rows[iRow]["FULLOREMPTY"]);
                strArray[iRow, 2] = Convert.ToString(dt.Rows[iRow]["CON_TOLTAL"]);
                strArray[iRow, 3] = Convert.ToString(dt.Rows[iRow]["CON_TALLY"]);
                strArray[iRow, 4] = Convert.ToString(dt.Rows[iRow]["CON_YB"]);
                strArray[iRow, 5] = Convert.ToString(dt.Rows[iRow]["FULLOREMPTY"]);
            }

            log.LogCatalogSuccess();
            return Result.Success(strArray, null);
        }
        #endregion

        #region 获取个人作业进度
        /// <summary>
        /// 获取个人作业进度
        /// </summary>
        /// <param name="strShip_Id">航次ID</param>
        /// <param name="strInOutType">进出口类型</param>
        /// <param name="strWork_No">工号</param>
        /// <returns>结果</returns>
        public string GetWorkProgress_Perssonal(string strShip_Id, string strInOutType, string strWork_No)
        {
            //错误提示
            string strErr = string.Empty;
            string strSql = string.Empty;

            strSql =
                 string.Format(@"select Max(NAME) NAME,SIZE_CON,FULLOREMPTY,Count(*) CON_TOLTAL,Sum(UNLOAD_MARK) CON_TALLY,Sum(YB) CON_YB,Sum(JJR) CON_JJR 
                                 from vcon_image_pda_stat
                                 where Old_ID='{0}' and PORT like '%LYG' and WORK_NO='{1}' 
                                 group by SIZE_CON,FULLOREMPTY order by SIZE_CON,FULLOREMPTY",
                                 strShip_Id, strWork_No);
            var dt = Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteQuery_DataTable(strSql, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("异常：" + strErr);
                return Result.Failure("异常：" + strErr);
            }

            if (dt.Rows.Count <= 0)
            {
                log.LogCatalogFailure("无作业数据");
                return Result.Failure(null);
            }

            string[,] strArray = new string[dt.Rows.Count, 7];
            for (int iRow = 0; iRow < dt.Rows.Count; iRow++)
            {
                strArray[iRow, 0] = Convert.ToString(dt.Rows[iRow]["NAME"]);
                strArray[iRow, 1] = Convert.ToString(dt.Rows[iRow]["SIZE_CON"]);
                strArray[iRow, 2] = Convert.ToString(dt.Rows[iRow]["FULLOREMPTY"]);
                strArray[iRow, 3] = Convert.ToString(dt.Rows[iRow]["CON_TOLTAL"]);
                strArray[iRow, 4] = Convert.ToString(dt.Rows[iRow]["CON_TALLY"]);
                strArray[iRow, 5] = Convert.ToString(dt.Rows[iRow]["CON_YB"]);
                strArray[iRow, 6] = Convert.ToString(dt.Rows[iRow]["FULLOREMPTY"]);
            }

            log.LogCatalogSuccess();
            return Result.Success(strArray, null);
        }
        #endregion

        #region 获取同班人员
        /// <summary>
        /// 获取同班人员
        /// </summary>
        /// <param name="strShip_Id">航次ID</param>
        /// <param name="strNight_Mark">夜班标志</param>
        /// <returns>结果</returns>
        public string GetWorkerOfSameClass(string strShip_Id, string strNight_Mark)
        {
            //错误提示
            string strErr = string.Empty;

            string strSql =
                string.Format(@"",
                                strShip_Id, strNight_Mark);
            var dt = Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteQuery_DataTable(strSql, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("异常：" + strErr);
                return Result.Failure("异常：" + strErr);
            }

            if (dt.Rows.Count <= 0)
            {
                log.LogCatalogFailure("无同班人员数据");
                return Result.Failure(null);
            }

            string[,] strArray = new string[dt.Rows.Count, 6];
            for (int iRow = 0; iRow < dt.Rows.Count; iRow++)
            {
                strArray[iRow, 0] = Convert.ToString(dt.Rows[iRow]["SIZE_CON"]);
                strArray[iRow, 1] = Convert.ToString(dt.Rows[iRow]["FULLOREMPTY"]);
                strArray[iRow, 2] = Convert.ToString(dt.Rows[iRow]["CON_TOLTAL"]);
                strArray[iRow, 3] = Convert.ToString(dt.Rows[iRow]["CON_TALLY"]);
                strArray[iRow, 4] = Convert.ToString(dt.Rows[iRow]["CON_YB"]);
                strArray[iRow, 5] = Convert.ToString(dt.Rows[iRow]["FULLOREMPTY"]);
            }

            log.LogCatalogSuccess();
            return Result.Success(strArray, null);
        }
        #endregion
    }
}