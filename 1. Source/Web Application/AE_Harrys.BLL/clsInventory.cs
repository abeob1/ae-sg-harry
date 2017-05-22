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
    public class clsInventory
    {
        clsLog oLog = new clsLog();

       public Int16 p_iDebugMode = DEBUG_ON;

        public const Int16 RTN_SUCCESS = 1;
        public const Int16 RTN_ERROR = 0;
        public const Int16 DEBUG_ON = 1;
        public const Int16 DEBUG_OFF = 0;
       

        public DataSet Get_InventoryRequest(string sOutlet, string sConnString, string sErrDesc)
        {

            DataSet oDataset;
            string sFuncName = string.Empty;

            try
            {
                sFuncName = "Get_InventoryRequest()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                DataAccess oDataAccess = new DataAccess();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_StoredProcedure() ", sFuncName);

                oDataset = oDataAccess.Run_StoredProcedure("EXEC AE_SP010_GetInventoryRequest '" + sOutlet + "'", sConnString);

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

        public DataSet Get_InventoryRequestDetails(string sDocEntry, string sConnString, string sErrDesc)
        {
            DataSet oDataset;
            string sFuncName = string.Empty;

            try
            {
                sFuncName = "Get_InventoryRequestDetails()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                DataAccess oDataAccess = new DataAccess();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_StoredProcedure() ", sFuncName);

                oDataset = oDataAccess.Run_StoredProcedure("EXEC AE_SP011_GetInventoryReqDetails '" + sDocEntry + "'", sConnString);

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
        
        public DataSet Get_ItemList(string sConnString, string sErrDesc)
        {
            DataSet oDataset;
            string sFuncName = string.Empty;

            try
            {
                sFuncName = "Get_ItemList()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                DataAccess oDataAccess = new DataAccess();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_StoredProcedure() ", sFuncName);

                oDataset = oDataAccess.Run_StoredProcedure("EXEC AE_SP016_GetItemList", sConnString);

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
                
        public DataSet Get_ReasonDetails(string sOutlet, string sConnString, string sErrDesc)
        {
            DataSet oDataset;
            string sFuncName = string.Empty;

            try
            {
                sFuncName = "Get_ReasonDetails()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                DataAccess oDataAccess = new DataAccess();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_StoredProcedure() ", sFuncName);

                oDataset = oDataAccess.Run_StoredProcedure("EXEC AE_SP017_GetReasonDetails  ", sConnString);

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

        public DataSet Get_OpenTRList(string sOutlet, string sConnString, string sErrDesc)
        {

            DataSet oDataset;
            string sFuncName = string.Empty;

            try
            {
                sFuncName = "Get_OpenTRList()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                DataAccess oDataAccess = new DataAccess();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_StoredProcedure() ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Executing Procedure :  " + " EXEC AE_SP022_OpenTRList '" + sOutlet + "'" + sConnString, sFuncName);

                oDataset = oDataAccess.Run_StoredProcedure("EXEC AE_SP022_OpenTRList '" + sOutlet + "'", sConnString);
                                
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

        public DataSet Get_TransferRequest_ItemList(string sDocEntry, string sConnString, string sErrDesc)
        {
            DataSet oDataset;
            string sFuncName = string.Empty;

            try
            {
                sFuncName = "Get_TransferRequest_ItemList()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                DataAccess oDataAccess = new DataAccess();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_StoredProcedure() ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Executing Procedure :  " + " EXEC AE_SP023_TransferRequest_ItemList '" + sDocEntry + "'" + sConnString, sFuncName);

                oDataset = oDataAccess.Run_StoredProcedure("EXEC AE_SP023_TransferRequest_ItemList '" + sDocEntry + "'", sConnString);

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

    }
}
