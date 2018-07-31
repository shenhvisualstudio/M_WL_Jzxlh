using M_YKT_Ysfw.Service.Tally;
using ServiceInterface.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Web;

namespace M_YKT_Ysfw.Service
{
    public class Pub
    {
        #region 日志
        /// <summary>
        /// 日志对象
        /// </summary>
        private Log log;

        public Pub(Log log)
        {
            this.log = log;
        }

        #endregion

        #region 作业校验
        /// <summary>
        /// 作业校验
        /// </summary>
        /// <param name="tallyE">TallyE数据对象</param>
        /// <returns>结果</returns>
        public string CheckWork(TallyE tallyE)
        {

            //错误提示
            string strErr = string.Empty;
            string strSql = string.Empty;
            if (tallyE.StrInOutType.Equals("E"))
            {   //出口装箱，判断前后贝
                string pos1 = Convert.ToString((Convert.ToInt16(tallyE.StrBay_No.Substring(0, 2)) - 1)).PadLeft(2, '0') + tallyE.StrBay_No.Substring(2).PadLeft(4, '0');
                string pos2 = Convert.ToString((Convert.ToInt16(tallyE.StrBay_No.Substring(0, 2)) + 1)).PadLeft(2, '0') + tallyE.StrBay_No.Substring(2).PadLeft(4, '0');
                strSql = string.Format(@"select container_no, bayno 
                                         from VCON_IMAGE_PDA 
                                         where old_id='{0}' and bayno in ('{1}','{2}','{3}') and unload_mark='1' ",
                                         tallyE.StrShip_Id, tallyE.StrBay_No, pos1, pos2);
            }
            else {
                //进口
                strSql = string.Format(@"select container_no 
                                         from VCON_IMAGE_PDA 
                                         where old_id='{0}' and container_no='{1}' and unload_mark='1' ",
                                         tallyE.StrShip_Id, tallyE.StrContainer_No);
            }

            var dt = Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteQuery_DataTable(strSql, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("作业校验异常：" + strErr);
                return Result.Failure("作业校验异常：" + strErr);
            }
            if (dt.Rows.Count > 0)
            {
                string strMsg = string.Empty;
                if (tallyE.StrInOutType.Equals("E"))
                {   //出口已装船
                    strMsg = Convert.ToString(dt.Rows[0]["container_no"]) + "已装船！" + Convert.ToString(dt.Rows[0]["bayno"]);

                }
                else {
                    //进口已卸船
                    strMsg = Convert.ToString(dt.Rows[0]["container_no"]) + "已卸船！" + "，请注意调整！";
                }
                log.LogCatalogSuccess(strMsg);
                return Result.Success(null, strMsg);
            }

            return null;
        }
        #endregion

        #region 同步舱单
        /// <summary>
        /// 同步舱单
        /// </summary>
        /// <param name="tallyE">TallyE数据对象</param>
        /// <returns>结果</returns>
        public string UpdateHatch(TallyE tallyE)
        {
            //错误提示
            string strErr = string.Empty;
            string strSql = string.Empty;
            if (tallyE.StrInOutType.Equals("E"))
            {
                strSql = string.Format(@"update CON_HATCH_RECORD
                                         set unload_mark = '1',bayno='{0}',user_code='{1}'
                                         where ship_id='{2}' and container_no='{3}'",
                                         tallyE.StrBay_No, tallyE.StrWork_No, tallyE.StrNewShip_Id, tallyE.StrContainer_No);
            }
            else {
                strSql = string.Format(@"update CON_HATCH_RECORD
                                         set unload_mark = '1',user_code='{0}'
                                         where ship_id='{1}' and container_no='{2}'",
                                         tallyE.StrWork_No, tallyE.StrNewShip_Id, tallyE.StrContainer_No);
            }
            Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteNonQuery(strSql, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("同步舱单异常：" + strErr);
                return Rollback(tallyE);
                //return Result.Failure("同步舱单异常：" + strErr);
            }

            return null;
        }
        #endregion

        #region 同步船图
        /// <summary>
        /// 同步船图
        /// </summary>
        /// <param name="tallyE">TallyE数据对象</param>
        /// <returns>结果</returns>
        public string UpdateImage(TallyE tallyE)
        {
            //错误提示
            string strErr = string.Empty;
            string strSql_ImageAll =
                string.Format("select * from CON_IMAGE where 1=0");
            string strSql =
                string.Format(@"select * from CON_IMAGE 
                                where ship_id='{0}' and container_no='{1}'",
                                tallyE.StrNewShip_Id, tallyE.StrContainer_No);
            var dt = Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteQuery_DataTable(strSql, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("同步船图异常：" + strErr);
                return Rollback(tallyE);
                //return Result.Failure("同步船图异常：" + strErr);
            }
            if (dt.Rows.Count > 0)
            {
                dt.Rows[0]["UNLOAD_MARK"] = "1";
                dt.Rows[0]["MOVED"] = "0";
                //智能识别自动理货处理
                if (tallyE.StrWork_No.Substring(0, 1) == "P" || tallyE.StrWork_No.Substring(0, 1) == "X")
                {
                    dt.Rows[0]["WORK_NO"] = "9999";
                    dt.Rows[0]["TALLY_MARK"] = "A";
                    dt.Rows[0]["CODE_CRANE"] = tallyE.StrWork_No;
                }
                else {
                    dt.Rows[0]["WORK_NO"] = tallyE.StrWork_No;
                }
                dt.Rows[0]["USER_CODE"] = tallyE.StrWork_No;
                if (tallyE.StrInOutType.Equals("I"))
                {
                    if (Convert.IsDBNull(dt.Rows[0]["WORK_DATE"]))
                        dt.Rows[0]["WORK_DATE"] = tallyE.StrTime;
                    dt.Rows[0]["USER_NAME"] = tallyE.StrWork_No;
                }
                else {
                    dt.Rows[0]["WORK_DATE"] = tallyE.StrTime; ;
                    dt.Rows[0]["BAYNO"] = tallyE.StrBay_No;
                    //如果新贝位目前存在箱号，则与当前箱所在贝位调换
                    string strSqlUpdate = "update CON_IMAGE set BayNo=nvl((select Max(BayNo) BayNo from con_image where SHIP_ID=" + tallyE.StrNewShip_Id + " and CONTAINER_NO ='" + tallyE.StrContainer_No + "'),'" + tallyE.StrBay_No + "')"
                        + " where SHIP_ID=" + tallyE.StrNewShip_Id + " and  BayNO='" + tallyE.StrBay_No + "'  and CONTAINER_NO !='" + tallyE.StrContainer_No + "'";
                    Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteNonQuery(strSqlUpdate, out strErr);
                    if (!string.IsNullOrWhiteSpace(strErr))
                    {
                        log.LogCatalogFailure("同步舱单异常：" + strErr);
                        return Rollback(tallyE);
                        //return Result.Failure("同步舱单异常：" + strErr);
                    }

                    Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).UpdateTable(dt, strSql_ImageAll, out strErr);
                    if (!string.IsNullOrWhiteSpace(strErr))
                    {
                        log.LogCatalogFailure("同步舱单异常：" + strErr);
                        return Rollback(tallyE);
                        //return Result.Failure("同步舱单异常：" + strErr);
                    }
                }
            }

            return null;
        }
        #endregion

        #region 更新配载指令
        /// <summary>
        /// 更新配载指令
        /// </summary>
        /// <param name="tallyE">TallyE数据对象</param>
        /// <returns>结果</returns>
        public string UpdateInstruction(TallyE tallyE)
        {
            //错误提示
            string strErr = string.Empty;
            string strLoaddir = tallyE.StrInOutType.Equals("I") == true ? "0" : "1";
            string strSql = string.Format(@"update TB_CON_INSTRUCTION 
                                            set complete_mark='1',bayno='{3}',tally_mark='{4}',updatetime='{5}' 
                                            where ship_id='{0}' and ctn_no='{1}' and loadunload_mark='{2}'",
                                            tallyE.StrShip_Id, tallyE.StrContainer_No, strLoaddir, tallyE.StrBay_No, tallyE.StrWork_No, tallyE.StrTime);
            Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteNonQuery(strSql, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("更新配载指令异常：" + strErr);
                return Rollback(tallyE);
                //return Result.Failure("更新配载指令异常：" + strErr);
            }

            if (tallyE.StrInOutType.Equals("I"))
            {
                strSql = string.Format(@"update TB_CON_INSTRUCTION 
                                         set truck_no='{2}' where ship_id='{0}' and ctn_no='{1}' and loadunload_mark='{3}' ",
                                         tallyE.StrShip_Id, tallyE.StrContainer_No, tallyE.StrTruck_No, strLoaddir);
                Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteNonQuery(strSql, out strErr);
                if (!string.IsNullOrWhiteSpace(strErr))
                {
                    log.LogCatalogFailure("更新配载指令异常：" + strErr);
                    return Rollback(tallyE);
                    //return Result.Failure("更新配载指令异常：" + strErr);
                }
            }

            return null;
        }
        #endregion

        #region 生成理货单编号
        /// <summary>
        /// 生成理货单编号
        /// </summary>
        /// <param name="tallyE">TallyE数据对象</param>
        /// <returns>结果</returns>
        public string GenerateTallyNum(TallyE tallyE)
        {
            //错误提示
            string strErr = string.Empty;
            string strSql = string.Format(@"select max(no) from CON_TALLY_LIST 
                                            where ship_id='{0}' and team_no='{1}' and tally_clerk1='{2}' and inout_mark='0' and reload='0' ",
                                            tallyE.StrNewShip_Id, tallyE.StrTeam_No, tallyE.StrWork_No);
            string strTemp = Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteQuery_String(strSql, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("生成理货单编号异常：" + strErr);
                return Rollback(tallyE);
                //return Result.Failure("生成理货单编号异常：" + strErr);
            }

            if (strTemp.Length == 0)
                tallyE.StrBhno = tallyE.StrWork_No + tallyE.StrTeam_No + "0001";
            else
                tallyE.StrBhno = Convert.ToString(Convert.ToInt64(strTemp) + 1).PadLeft(12, '0');

            return null;
        }
        #endregion

        #region 同步理箱单主子表
        /// <summary>
        /// 同步理箱单主子表
        /// </summary>
        /// <param name="tallyE">TallyE数据对象</param>
        /// <returns>结果</returns>
        public string UpdateTallyList(TallyE tallyE)
        {
            //错误提示
            string strErr = string.Empty;
            string strSqlGetConList = "select * from CON_TALLY_LIST where 1=0";
            string sqlGetConDetail = "select * from CON_TALLY_DETAIL where 1=0";
            string sqlGetConListNewID = "Select SQ_CON_TALLY_LIST_ID.Nextval NEWID from dual";
            var dt_conList = Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteQuery_DataTable(strSqlGetConList, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("同步理箱单主子表异常：" + strErr);
                return Rollback(tallyE);
                //return Result.Failure("同步理箱单主子表异常：" + strErr);
            }
            var dt_conDetail = Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteQuery_DataTable(sqlGetConDetail, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("同步理箱单主子表异常：" + strErr);
                return Rollback(tallyE);
                //return Result.Failure("同步理箱单主子表异常：" + strErr);
            }
            string strConListNewID = Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteQuery_String(sqlGetConListNewID, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("同步理箱单主子表异常：" + strErr);
                return Rollback(tallyE);
                //return Result.Failure("同步理箱单主子表异常：" + strErr);
            }

            //理箱单主表
            DataRow drRow;
            drRow = dt_conList.NewRow();
            drRow["ID"] = strConListNewID;
            drRow["SHIP_ID"] = tallyE.StrNewShip_Id;
            drRow["TEAM_NO"] = tallyE.StrTeam_No;
            drRow["NO"] = tallyE.StrBhno;
            drRow["WORKDATE"] = tallyE.StrTime;
            drRow["TIMEFROM"] = tallyE.StrEndTime;
            drRow["TIMETO"] = tallyE.StrEndTime;
            drRow["BERTHNO"] = tallyE.StrBerth_No;
            drRow["NIGHT_MARK"] = tallyE.StrNight_Mark;
            drRow["HOLIDAY"] = tallyE.StrHoliady_Mark;
            drRow["INOUT_MARK"] = "0";
            drRow["RELOAD"] = "0";
            if (tallyE.StrMovedMark.Equals("1"))
            {
                drRow["INOUT_MARK"] = "1";
            }
            else if (tallyE.StrMovedMark.Equals("2"))
            {
                drRow["RELOAD"] = "1";
            }
            drRow["TALLY_CLERK1"] = tallyE.StrWork_No;
            drRow["USER_NAME"] = tallyE.StrWork_No;
            dt_conList.Rows.Add(drRow);

            //子表
            drRow = dt_conList.NewRow();
            drRow["LIST_ID"] = strConListNewID;
            drRow["SHIP_ID"] = tallyE.StrNewShip_Id;
            drRow["CONTAINER_NO"] = tallyE.StrContainer_No;
            drRow["BAYNO"] = tallyE.StrBay_No;
            drRow["UNLOAD_MARK"] = "1";

            string strSql =
                string.Format(@"select * from CON_IMAGE 
                                where ship_id='{0}' and container_no='{1}'",
                                tallyE.StrNewShip_Id, tallyE.StrContainer_No);
            var dt = Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteQuery_DataTable(strSql, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("同步理箱单主子表异常：" + strErr);
                return Rollback(tallyE);
                //return Result.Failure("同步理箱单主子表异常：" + strErr);
            }
            if (dt.Rows.Count > 0)
            {
                drRow["SIZE_CON"] = dt.Rows[0]["SIZE_CON"];
                drRow["CODE_EMPTY"] = dt.Rows[0]["FULLOREMPTY"];
                string strWeight = string.Empty;
                if (tallyE.StrMovedMark.Equals("1") || tallyE.StrMovedMark.Equals("2"))
                {
                    strWeight = dt.Rows[0]["WEIGHT"].ToString();
                }
                else {
                    strWeight = dt.Rows[0]["GROSSWEIGHT"].ToString(); ;
                }
                if (strWeight.Length > 0)
                    drRow["WEIGHT"] = decimal.Parse(strWeight) / 1000;
                else
                    drRow["WEIGHT"] = DBNull.Value;
            }
            dt_conDetail.Rows.Add(drRow);
            Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).UpdateTable(dt_conList, strSqlGetConList, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("同步理箱单主子表异常：" + strErr);
                return Rollback(tallyE);
                //return Result.Failure("同步理箱单主子表异常：" + strErr);
            }
            Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).UpdateTable(dt_conDetail, sqlGetConDetail, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("同步理箱单主子表异常：" + strErr);
                return Rollback(tallyE);
                //return Result.Failure("同步理箱单主子表异常：" + strErr);
            }

            return null;
        }
        #endregion

        #region 发送指令
        /// <summary>
        /// 发送指令
        /// </summary>
        /// <param name="tallyE">TallyE数据对象</param>
        /// <returns>结果</returns>
        public string SendInstruction(TallyE tallyE)
        {
            //错误提示
            string strErr = string.Empty;
            string strSql = string.Empty;

            strSql =
                string.Format(@"select active_id 
                                from TB_CON_INSTRUCTION 
                                where ship_id='{0}' and ctn_no='{1}' and loadunload_mark='{2}'",
                                tallyE.StrShip_Id, tallyE.StrContainer_No);
            string strActive_id = Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteQuery_String(strSql, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("异常：" + strErr);
                return Result.Failure("异常：" + strErr);
            }

            if (strActive_id.Equals("2"))
            {
                log.LogCatalogSuccess("xdf_over:中控已确认指令");
                return Result.Success(null, "xdf_over:中控已确认指令");
            }
            else {
                string strInfo = Con_Instruction(tallyE);

                if (strInfo.Equals("1"))
                {
                    log.LogCatalogSuccess("xdf_ok");
                    return Result.Success(null, "xdf_ok");
                }
                else {
                    log.LogCatalogSuccess("xdf_err: " + strInfo);
                    return Result.Success(null, "xdf_err: " + strInfo);
                }
            }
        }
        #endregion

        #region 理箱回滚
        /// <summary>
        /// 理箱回滚
        /// </summary>
        /// <param name="tallyE">TallyE数据对象</param>
        /// <returns>结果</returns>
        public string Rollback(TallyE tallyE)
        {
            //错误提示
            string strErr = string.Empty;
            string strSql = string.Empty;

            try
            {
                strSql =
                    string.Format(@"update CON_HATCH_RECORD 
                                    set unload_mark='0' 
                                    where ship_id='{0}' and container_no='{1}' and unload_mark='1'",
                                    tallyE.StrNewShip_Id, tallyE.StrContainer_No);
                Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteNonQuery(strSql, out strErr);
                if (!string.IsNullOrWhiteSpace(strErr))
                {
                    log.LogCatalogFailure("理箱回滚（舱单）异常：" + strErr);
                    return Result.Failure("理箱回滚（舱单）异常：" + strErr);
                }

                strSql =
                     string.Format(@"update CON_IMAGE 
                                     set unload_mark='0',work_no=null,editbaymark='0' 
                                     where ship_id='{0}' and container_no='{1}' and unload_mark='1'",
                                     tallyE.StrNewShip_Id, tallyE.StrContainer_No);
                Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteNonQuery(strSql, out strErr);
                if (!string.IsNullOrWhiteSpace(strErr))
                {
                    log.LogCatalogFailure("理箱回滚（船图）异常：" + strErr);
                    return Result.Failure("理箱回滚（船图）异常：" + strErr);
                }

                strSql =
                      string.Format(@"delete from CON_TALLY_DETAIL 
                                      where ship_id='{0}' and container_no='{1}'",
                                      tallyE.StrNewShip_Id, tallyE.StrContainer_No);
                Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteNonQuery(strSql, out strErr);
                if (!string.IsNullOrWhiteSpace(strErr))
                {
                    log.LogCatalogFailure("理箱回滚（理箱单子表）异常：" + strErr);
                    return Result.Failure("理箱回滚（理箱单子表）异常：" + strErr);
                }

                log.LogCatalogSuccess("rollback_ok");
                return Result.Success(null, null);

            }
            catch (Exception ex)
            {
                log.strWorkType = "rollback_ec";
                log.LogCatalogFailure("异常：" + ex.Message);
                return Result.Failure("异常：" + ex.Message);
            }
        }
        #endregion

        #region 倒箱作业校验
        /// <summary>
        /// 倒箱作业校验
        /// </summary>
        /// <param name="tallyE">TallyE数据对象</param>
        /// <returns>结果</returns>
        public string Moved_CheckWork(TallyE tallyE)
        {

            //错误提示
            string strErr = string.Empty;
            string strSql = string.Empty;
            //出口装箱，判断前后贝
            string pos1 = Convert.ToString((Convert.ToInt16(tallyE.StrBay_No.Substring(0, 2)) - 1)).PadLeft(2, '0') + tallyE.StrBay_No.Substring(2).PadLeft(4, '0');
            string pos2 = Convert.ToString((Convert.ToInt16(tallyE.StrBay_No.Substring(0, 2)) + 1)).PadLeft(2, '0') + tallyE.StrBay_No.Substring(2).PadLeft(4, '0');
            strSql = string.Format(@"select container_no, bayno 
                                     from VCON_IMAGE_PDA 
                                     where old_id='{0}' and bayno in ('{1}','{2}','{3}') and unload_mark='1' ",
                                     tallyE.StrShip_Id, tallyE.StrBay_No, pos1, pos2);

            var dt = Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteQuery_DataTable(strSql, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("作业校验异常：" + strErr);
                return Result.Failure("作业校验异常：" + strErr);
            }
            if (dt.Rows.Count > 0)
            {
                string strMsg = Convert.ToString(dt.Rows[0]["container_no"]) + "已装船！" + Convert.ToString(dt.Rows[0]["bayno"]);
                strMsg += "，请注意调整";
                log.LogCatalogSuccess(strMsg);
                return Result.Success(null, strMsg);
            }

            return null;
        }
        #endregion

        #region 倒箱同步船图
        /// <summary>
        /// 倒箱同步船图
        /// </summary>
        /// <param name="tallyE">TallyE数据对象</param>
        /// <returns>结果</returns>
        public string Moved_UpdateImage(TallyE tallyE)
        {
            //错误提示
            string strErr = string.Empty;
            string strSplit = string.Empty;
            if (tallyE.StrMovedMark.Equals("1"))
            {
                strSplit = "moved='1',";
            }
            else if (tallyE.StrMovedMark.Equals("2"))
            {
                strSplit = "bayno='" + tallyE.StrBay_No + "',";
                string strSql =
                    string.Format(@"update CON_IMAGE 
                                    set oldbayno=bayno,moved='1'
                                    where ship_id='{0}' and bayno='{1}'",
                                    tallyE.StrNewShip_Id, tallyE.StrBay_No);
                Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteNonQuery(strSql, out strErr);
                if (!string.IsNullOrWhiteSpace(strErr))
                {
                    log.LogCatalogFailure("倒箱同步船图异常：" + strErr);
                    return Rollback(tallyE);
                    //return Result.Failure("倒箱同步船图异常：" + strErr);
                }
            }

            string sqlUpdate = string.Empty;
            //智能识别自动理货处理
            if (tallyE.StrWork_No.Substring(0, 1) == "P" || tallyE.StrWork_No.Substring(0, 1) == "X")
                sqlUpdate = "update CON_IMAGE set UNLOAD_MARK='1'," + strSplit + "WORK_NO='A',WORK_NO='9999',CODE_CRANE='" + tallyE.StrWork_No + "',USER_CODE='" + tallyE.StrWork_No + "',USER_NAME='" + tallyE.StrWork_No + "',WORK_DATE=TO_DATE('" + tallyE.StrTime + "','YYYY-MM-DD HH24:MI:SS') where SHIP_ID='" + tallyE.StrNewShip_Id + "' and CONTAINER_NO='" + tallyE.StrContainer_No + "'";
            else
                sqlUpdate = "update CON_IMAGE set UNLOAD_MARK='1'," + strSplit + "WORK_NO='" + tallyE.StrWork_No + "',USER_CODE='" + tallyE.StrWork_No + "',USER_NAME='" + tallyE.StrWork_No + "',WORK_DATE=TO_DATE('" + tallyE.StrTime + "','YYYY-MM-DD HH24:MI:SS') where SHIP_ID='" + tallyE.StrNewShip_Id + "' and CONTAINER_NO='" + tallyE.StrContainer_No + "'";
            Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteNonQuery(sqlUpdate, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("倒箱同步船图异常：" + strErr);
                return Rollback(tallyE);
                //return Result.Failure("倒箱同步船图异常：" + strErr);
            }

            return null;
        }
        #endregion

        #region 倒箱更新配载指令
        /// <summary>
        /// 倒箱更新配载指令
        /// </summary>
        /// <param name="tallyE">TallyE数据对象</param>
        /// <returns>结果</returns>
        public string Moved_UpdateInstruction(TallyE tallyE)
        {
            //错误提示
            string strErr = string.Empty;
            string strSql = string.Empty;
            if (tallyE.StrInOutType.Equals("I"))
            {
                strSql =
                    string.Format(@"update TB_CON_INSTRUCTION 
                                         set complete_mark='1', bayno='{5}',truck_no='{2}',tally_mark='{3}',updatetime='{6}' 
                                         where ship_id='{0}' and ctn_no='{1}' and loadUnload_Mark='{4}' and reLoad_mark='1' ",
                                         tallyE.StrShip_Id, tallyE.StrContainer_No, tallyE.StrTruck_No, tallyE.StrWork_No, "0", tallyE.StrBay_No, tallyE.StrTime);
            }
            else {
                strSql =
                    string.Format(@"update TB_CON_INSTRUCTION 
                                    set complete_mark = '1', bayno = '{2}', tally_mark = '{3}', updatetime = '{5}' 
                                    where ship_id = '{0}' and ctn_no = '{1}' and loadUnload_mark = '{4}' and reLoad_mark = '1' ",
                                    tallyE.StrShip_Id, tallyE.StrContainer_No, tallyE.StrBay_No, tallyE.StrWork_No, "1", tallyE.StrTime);
            }

            Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteNonQuery(strSql, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("倒箱更新配载指令：" + strErr);
                return Rollback(tallyE);
                //return Result.Failure("倒箱更新配载指令：" + strErr);
            }

            return null;
        }
        #endregion

        #region 倒箱生成理货单编号
        /// <summary>
        /// 倒箱生成理货单编号
        /// </summary>
        /// <param name="tallyE">TallyE数据对象</param>
        /// <returns>结果</returns>
        public string Moved_GenerateTallyNum(TallyE tallyE)
        {
            //错误提示
            string strErr = string.Empty;
            string strSql = string.Format(@"select max(no) from CON_TALLY_LIST 
                                            where ship_id='{0}' and team_no='{1}' and tally_clerk1='{2}' and inout_mark='0' and reload='0' ",
                                            tallyE.StrNewShip_Id, tallyE.StrTeam_No, tallyE.StrWork_No);
            string strTemp = Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteQuery_String(strSql, out strErr);
            if (!string.IsNullOrWhiteSpace(strErr))
            {
                log.LogCatalogFailure("倒箱生成理货单编号：" + strErr);
                return Rollback(tallyE);
                //return Result.Failure("倒箱生成理货单编号：" + strErr);
            }

            if (strTemp.Length == 0)
                tallyE.StrBhno = "dx" + tallyE.StrWork_No + tallyE.StrTeam_No + "0001";
            else
                tallyE.StrBhno = strTemp.Substring(0, 10) + Convert.ToString(Convert.ToInt64(strTemp.Substring(10)) + 1).PadLeft(4, '0');

            return null;
        }
        #endregion

        #region 调用指令发送至码头
        /// <summary>
        /// 调用指令发送至码头
        /// </summary>
        /// <param name="tallyE"></param>
        /// <returns>结果</returns>
        private string Con_Instruction(TallyE tallyE)
        {

            //错误提示
            string strErr = string.Empty;
            string strSql = string.Empty;
            string strInfo = string.Empty;
            var dt = new DataTable();
            string strLoaddir = tallyE.StrInOutType.Equals("I") == true ? "0" : "1";

            try
            {
                strSql =
                    string.Format(@"select * from TB_CON_INSTRUCTION 
                                    where ship_id='{0}' and ctn_no='{1}' and loadunload_mark='{2}'",
                                    tallyE.StrShip_Id, tallyE.StrContainer_No, strLoaddir);
                dt = Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteQuery_DataTable(strSql, out strErr);
                if (dt.Rows.Count > 0)
                {
                    if (tallyE.StrInOutType.Equals("I"))
                    {
                        return Con_Instruction_InPort(tallyE, dt);
                    }
                    else {
                        return Con_Instruction_OutPort(tallyE, dt);
                    }

                }
            }
            catch (Exception ex)
            {
                //回滚
                log.strWorkType = "con_instruction_ec";
                log.strBay_No = Convert.ToString(dt.Rows[0]["bayno"]);
                log.strTruck_No = Convert.ToString(dt.Rows[0]["truck_no"]);
                log.LogCatalogFailure("异常：" + ex.Message + "; " + strInfo);

                if (tallyE.StrInOutType.Equals("I")) {
                    oracle_find_cntr_plac_try_rollback(Convert.ToString(dt.Rows[0]["ctn_no]));
                }
                   
                rollback_xdf(tallyE);
            }

            return null;
        }
        #endregion

        #region 调用指令发送至码头（进口）
        /// <summary>
        /// 调用指令发送至码头（进口）
        /// </summary>
        /// <param name="tallyE">TallyE对象</param>
        /// <param name="dt">DataTable对象</param>
        /// <returns></returns>
        private string Con_Instruction_InPort(TallyE tallyE, DataTable dt)
        {
            //错误提示
            string strErr = string.Empty;
            string strSql = string.Empty;
            string strInfo = string.Empty;
            string strMsg_detail = "lygwl,tally08,";

            if (Convert.ToString(dt.Rows[0]["direct_id"]).Equals("0") && Convert.ToString(dt.Rows[0]["pass_barg_id"]).Equals("0"))
            {

            }
            else {
                if ((oracle_find_cntr_plac_try(Convert.ToString(dt.Rows[0]["ctn_no"]), "", "", "", "2").Equals("99")))
                {

                }
                else {
                }
            }





            return strInfo;
        }

        #endregion

        #region 调用指令发送至码头（出口）
        /// <summary>
        /// 调用指令发送至码头（出口）
        /// </summary>
        /// <param name="tallyE">TallyE对象</param>
        /// <param name="dt">DataTable对象</param>
        /// <returns></returns>
        private string Con_Instruction_OutPort(TallyE tallyE, DataTable dt)
        {
            //错误提示
            string strErr = string.Empty;
            string strSql = string.Empty;
            string strInfo = string.Empty;
            string strMsg_detail = "lygwl,tally08,";



            return strInfo;
        }
        #endregion

        #region 卸箱找位
        ///// <summary>
        ///// 卸箱找位
        ///// </summary>
        //public string oracle_find_cntr_plac_try(string cntr_no, string cntr_no1, string che_group_no, string return_plac, string find_id, DataTable dt, DataTable dt_return)
        //{
        //    //错误提示
        //    string strErr = string.Empty;
        //    string strSql = string.Empty;
        //    string strInfo = string.Empty;
        //    string strMsg_cod = string.Empty;
        //    string strMsg_text = string.Empty;

        //    try
        //    {
        //        //此处可能要区分新东方或者PSA
        //        OracleConnection orcn = new OracleConnection(Global_gl.connstr);
        //        string str_Sql = @"call pkg_costaco.p_find_cntr_plac_try(:p1,:p2,:p3,:p4,:p5,:p6,:p7,:p8,:p9)";
        //        OracleCommand cmd = new OracleCommand(str_Sql, orcn);
        //        strInfo = "333";
        //        OracleParameter pram1 = new OracleParameter("p1", OracleType.VarChar, 11);
        //        pram1.Value = cntr_no;
        //        cmd.Parameters.Add(pram1);
        //        strInfo = "201";
        //        OracleParameter pram2 = new OracleParameter("p2", OracleType.VarChar, 11);
        //        pram2.Direction = ParameterDirection.Output;
        //        cmd.Parameters.Add(pram2);
        //        strInfo = "202";
        //        OracleParameter pram3 = new OracleParameter("p3", OracleType.VarChar, 11);
        //        pram3.Value = cntr_no1;
        //        cmd.Parameters.Add(pram3);
        //        strInfo = "203";
        //        OracleParameter pram4 = new OracleParameter("p4", OracleType.VarChar, 11);
        //        pram4.Direction = ParameterDirection.Output;
        //        cmd.Parameters.Add(pram4);
        //        strInfo = "204";
        //        OracleParameter pram5 = new OracleParameter("p5", OracleType.VarChar, 10);
        //        pram5.Direction = ParameterDirection.Output;
        //        cmd.Parameters.Add(pram5);
        //        strInfo = "205";
        //        OracleParameter pram6 = new OracleParameter("p6", OracleType.VarChar, 50);
        //        pram6.Direction = ParameterDirection.Output;
        //        cmd.Parameters.Add(pram6);
        //        strInfo = "206";
        //        OracleParameter pram7 = new OracleParameter("p7", OracleType.VarChar, 11);
        //        pram7.Value = che_group_no;
        //        cmd.Parameters.Add(pram7);
        //        strInfo = "207";
        //        OracleParameter pram8 = new OracleParameter("p8", OracleType.VarChar, 11);
        //        pram8.Value = return_plac;
        //        cmd.Parameters.Add(pram8);
        //        strInfo = "208";
        //        OracleParameter pram9 = new OracleParameter("p9", OracleType.VarChar, 1);
        //        pram9.Value = find_id;
        //        cmd.Parameters.Add(pram9);
        //        strInfo = "209";

        //        if (orcn.State == ConnectionState.Closed)
        //            orcn.Open();
        //        cmd.ExecuteNonQuery();

        //        orcn.Close();
        //        orcn.Dispose();

        //        string strCntr = cmd.Parameters["p2"].Value.ToString();
        //        strMsg_cod = cmd.Parameters["p5"].Value.ToString();
        //        strMsg_text = cmd.Parameters["p6"].Value.ToString();
        //        strInfo = strMsg_cod;

        //        if (strMsg_cod == "99")
        //        {
        //            strSql = string.Format(@"update TB_CON_INSTRUCTION 
        //                                     set field_no='{1}' where id='{0}' ", 
        //                                     dt.Rows[0]["ID"].ToString(), strCntr);
        //            //此处可能要区分新东方或者PSA
        //            Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteNonQuery(strSql, out strErr);
        //            strSql = string.Format(@"select * from TB_CON_INSTRUCTION 
        //                                     where id='{0}' ", 
        //                                     Convert.ToString(dt.Rows[0]["ID"]));
        //            dt_return = Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteQuery_DataTable(strSql, out strErr);
        //        }
        //        else
        //        {
        //            log.strWorkType = "find_cntr_plac_err";
        //            log.LogCatalogSuccess(strMsg_cod + "." + strMsg_text);
        //        }

        //        return strInfo;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.strWorkType = "find_cntr_plac_ec";
        //        log.LogCatalogFailure("异常：" + ex.Message);
        //        return strMsg_cod;
        //    }
        //}
        #endregion

        #region 卸箱找车
        ///// <summary>
        ///// 卸箱找车
        ///// </summary>
        //public string oracle_find_truck(string strTruck_no, string strSize) {

        //    //错误提示
        //    string strErr = string.Empty;

        //    try
        //    {
        //        string strSql =
        //            string.Format("select pkg_costaco.f_if_truck_ok_wl('" + strTruck_no + "','" + strSize + "') from dual");
        //        //此处可能要区分新东方或者PSA
        //        var dt = Oracle.DataAccess(RegistryKey.KeyPath_Oracle_Tally).ExecuteQuery_DataTable(strSql, out strErr);
        //        if (!Convert.ToString(dt.Rows[0][0]).Equals("1")) {
        //            log.strWorkType = "find_truck_err";
        //            log.LogCatalogSuccess(Convert.ToString(dt.Rows[0][0]));
        //        }

        //        return Convert.ToString(dt.Rows[0][0]);
        //    }
        //    catch (Exception ex) {
        //        log.strWorkType = "find_truck_ec";
        //        log.LogCatalogFailure("异常：" + ex.Message);
        //        return "-1";
        //    }
        //}
        #endregion
    }
}