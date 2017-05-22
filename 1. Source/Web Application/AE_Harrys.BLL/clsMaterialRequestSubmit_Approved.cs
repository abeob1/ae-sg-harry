using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using AE_Harrys.DAL;
using AE_Harrys.Common;

namespace AE_Harrys.BLL
{
    public class clsMaterialRequestSubmit_Approved
    {

        clsLog oLog = new clsLog();

       public Int16 p_iDebugMode = DEBUG_ON;

        public const Int16 RTN_SUCCESS = 1;
        public const Int16 RTN_ERROR = 0;
        public const Int16 DEBUG_ON = 1;
        public const Int16 DEBUG_OFF = 0;
        public string sErrDesc = string.Empty;
     


        public DataSet Get_MaterialReqSubmit_Approved(string sOutlet,int iStatus,string fromDate 
                                                      ,string todate,string sUserRole, string sConnString,string sErrDesc)
        {
            DataSet oDataset;
            string sFuncName = string.Empty;

            try
            {
                sFuncName = "Get_MaterialReqSubmit_Approved()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                DataAccess oDataAccess = new DataAccess();
                
                //DateTime dtfrom = DateTime.Parse("fromDate");

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_StoredProcedure() ", sFuncName);

                oDataset = oDataAccess.Run_QueryString("EXEC AE_SP008_PurchaseRequest '" + sOutlet + "','" + sUserRole + "'," + iStatus + ",'" + fromDate + "','" + todate + "'", sConnString);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return oDataset;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;

            }

             
        }

        public DataSet Get_Submit_ApprovedDetails(string sDocEntry, string sDocType, string sConnString,string sErrDesc)
        {
            DataSet oDataset = new DataSet();
            string sFuncName = string.Empty;

            try
            {
                sFuncName = "Get_Submit_ApprovedDetails()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                DataAccess oDataAccess = new DataAccess();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_StoredProcedure() ", sFuncName);
                if (sDocType.ToUpper() == "PR")
                {
                    oDataset = oDataAccess.Run_QueryString("EXEC AE_SP009_GetPurchaseRequestDetails '" + sDocEntry + "'", sConnString);
                }
                else if (sDocType.ToUpper() == "PQ")
                {
                    oDataset = oDataAccess.Run_QueryString("EXEC AE_SP009_GetPurchaseQuotationDetails '" + sDocEntry + "'", sConnString);
                }
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return oDataset;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oDataset.Dispose();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;

            }


            
        }
    }
}
