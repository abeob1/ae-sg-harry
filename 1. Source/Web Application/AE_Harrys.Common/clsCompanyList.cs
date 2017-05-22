using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Sql;

using AE_Harrys.DAL;


namespace AE_Harrys.Common
{
    public class clsCompanyList
    {
        clsLog oLog = new clsLog();

        public Int16 p_iDebugMode = DEBUG_ON;

        public const Int16 RTN_SUCCESS = 1;
        public const Int16 RTN_ERROR = 0;
        public const Int16 DEBUG_ON = 1;
        public const Int16 DEBUG_OFF = 0;
        public string sErrDesc = string.Empty;

        public DataSet Get_CompanyList(string sConnString)
        {
            string sFuncName = string.Empty;
            DataSet oDSCompanyList = new DataSet();
            DataAccess oCompanyList = new DataAccess();
            try
            {
                sFuncName = "Get_CompanyList()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_QueryString()", sFuncName);

                oDSCompanyList = oCompanyList.Run_QueryString("SELECT T0.[U_DBName], T0.[U_CompName], T0.[U_SAPUserName], T0.[U_SAPPassword], " +
                        " T0.[U_DBUserName], T0.[U_DBPassword], T0.[U_ConnString] ,T0.[U_Server],T0.[U_LicenseServer] FROM [dbo].[@WEB_CMPDET]  T0 with (nolock) ", sConnString);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName);

                return oDSCompanyList;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oDSCompanyList.Dispose();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

    }
}
