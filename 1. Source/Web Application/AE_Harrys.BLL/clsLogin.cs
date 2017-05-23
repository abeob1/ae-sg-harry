using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

using AE_Harrys.DAL;
using AE_Harrys.Common;

namespace AE_Harrys.BLL
{

    public class clsLogin
    {
        clsLog oLog = new clsLog();

        public string sServer;
        public string sDataBaseName;
        public string sLicServerName;
        public string sDBUserName;
        public string sDBPassword;
        public Int32 sSQLType;
        public string sDebug;
        public string sLogPath;
        public string sErrDesc;
        public SAPbobsCOM.Company oCompany;
        public Int16 p_iDebugMode = DEBUG_ON;

        public const Int16 RTN_SUCCESS = 1;
        public const Int16 RTN_ERROR = 0;
        public const Int16 DEBUG_ON = 1;
        public const Int16 DEBUG_OFF = 0;

        public string SAPLogin(string sUserName, string sPassword, string sDBName, string sServer
                        , string sLicServerName, string sDBUserName, string sDBPassword, string sErrDesc)
        {
            long lRetCode;
            string sFuncName = string.Empty;

            try
            {
                sFuncName = "SAPLogin()";
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function   ", sFuncName);

                oCompany = new SAPbobsCOM.Company();
                oCompany.Server = sServer;
                oCompany.LicenseServer = sLicServerName;
                oCompany.DbUserName = sDBUserName;
                oCompany.DbPassword = sDBPassword;
                oCompany.language = SAPbobsCOM.BoSuppLangs.ln_English;
                oCompany.UseTrusted = false;
                oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2014;


                oCompany.CompanyDB = sDBName;// sDataBaseName;
                oCompany.UserName = sUserName;
                oCompany.Password = sPassword;

                lRetCode = oCompany.Connect();

                if (lRetCode != 0)
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR : " + oCompany.GetLastErrorDescription(), "SAPLogin");

                    return oCompany.GetLastErrorDescription();
                }
                else
                {
                    oLog.WriteToLogFile_Debug("Completed With SUCCESS", sFuncName);

                    return "SUCCESS";
                }
            }

            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oLog.WriteToLogFile(sErrDesc, sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR", sFuncName);

                return Ex.Message;
            }
        }

        public string UserValidation(string sUserName, string sPassword, string sConnString)
        {
            string sFuncName = string.Empty;
            DataAccess oDataAccess = new DataAccess();
            DataSet oDSData = new DataSet();
            try
            {
                sFuncName = "UserValidation()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_QueryString()", sFuncName);

                oDSData = oDataAccess.Run_QueryString("select USER_CODE ,U_WebPassword  from OUSR with (nolock) where USER_CODE ='" + sUserName + "'", sConnString);

                if (oDSData == null || oDSData.Tables[0].Rows.Count == 0)
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Invalid User Name", sFuncName);
                    return "Invalid User Name";
                }

                else
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("UserName :" + oDSData.Tables[0].Rows[0][0].ToString() +
                        "Password :" + oDSData.Tables[0].Rows[0][1].ToString(), sFuncName);
                if (oDSData.Tables[0].Rows[0][0].ToString().ToUpper() != sUserName.ToUpper() || oDSData.Tables[0].Rows[0][1].ToString() != sPassword)
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("UserName and Password is Incorrect", sFuncName);
                    return "UserName and Password is Incorrect";
                }
                else
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS", sFuncName);
                oDSData.Dispose();
                return "SUCCESS";

            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oDSData.Dispose();
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR", sFuncName);
                return sErrDesc;
            }
        }

        public DataSet Get_UserInformation(string sUserCode, string sConnString, string sErrDesc)
        {
            string sFuncName = string.Empty;
            DataAccess oDataAccess = new DataAccess();
            DataSet oDSData;

            try
            {
                sFuncName = "Get_UserInformation()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_QueryString()", sFuncName);

                oDSData = oDataAccess.Run_QueryString("EXEC AE_SP001_UserInformation '" + sUserCode + "'", sConnString);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS", sFuncName);

                return oDSData;
            }


            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR", sFuncName);
                throw Ex;

            }
        }

        public string MUser_Acknowlwdge(string sUserName, string sPassword, string sOutlet, string sConnString,string sErrDesc)
        {
            string sResult = string.Empty;
            string sFuncName = string.Empty;
            DataAccess oDataAccess = new DataAccess();
            DataSet oDSData = new DataSet();
            string sQueryString = string.Empty;
            try
            {
                sFuncName = "MUser_Acknowlwdge()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_QueryString()", sFuncName);

                sQueryString = "select 1 [Result] from OUSR with (nolock) where USER_CODE='" + sUserName + "' and U_WebPassword='" + sPassword + "' and ( " +
                   "(CASE when isnull(U_Outlet ,'ALL') ='ALL' then 1 end)=1 or (CASE when isnull(U_Outlet ,'ALL') <>'ALL' then U_Outlet  end)='" + sOutlet + "')  ";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Exucting Query : " + sQueryString , sFuncName);

                oDSData = oDataAccess.Run_QueryString(sQueryString, sConnString);

                if (oDSData == null || oDSData.Tables[0].Rows.Count == 0)
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Invalid User Name", sFuncName);
                    return "Invalid User Name";
                }

                oDSData.Dispose();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS", sFuncName);

                sResult = "SUCCESS";

            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oDSData.Dispose();
                sResult = sErrDesc;
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR", sFuncName);
            }
            return sResult;
        }
    }
}
