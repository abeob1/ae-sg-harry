using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

using AE_Harrys.DAL;
using AE_Harrys.Common;

namespace AE_Harrys.BLL
{
    public class clsOutlet
    {
        clsLog oLog = new clsLog();

       public Int16 p_iDebugMode = DEBUG_ON;

        public const Int16 RTN_SUCCESS = 1;
        public const Int16 RTN_ERROR = 0;
        public const Int16 DEBUG_ON = 1;
        public const Int16 DEBUG_OFF = 0;
        public string sErrDesc = string.Empty;
      

        public DataSet Get_Outlets(string sSuperUser, string sOutletCode, string sConnString, int iApprovalLevel, string sErrDesc)
        {
            string sQuery;
            DataAccess oDataAccess = new DataAccess();
            DataSet oDataset;
            string sFuncName = string.Empty;

            try
            {
                 


                sFuncName = "Get_Outlets()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (iApprovalLevel > 0)
                {
                    sQuery = "exec AE_SP001_GetOutlets";

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_StoredProcedure ", sFuncName);

                    oDataset = oDataAccess.Run_StoredProcedure(sQuery, sConnString);

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName);

                    return oDataset;
                }
                else
                {
                    sQuery = "select Distinct T1.WhsCode ,T1.WhsName  from OUSR T0 with (nolock) JOIN OWHS T1 with (nolock) on T0.U_Outlet =T1.WhsCode where T0.U_Outlet='" + sOutletCode + "'";

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_QueryString ", sFuncName);

                    oDataset = oDataAccess.Run_QueryString(sQuery, sConnString);

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName);

                    return oDataset;
                }
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;

            }

        }
    }
}
