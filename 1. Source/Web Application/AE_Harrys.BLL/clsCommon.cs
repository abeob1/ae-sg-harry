using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

using AE_Harrys.Common;

using AE_Harrys.DAL;

namespace AE_Harrys.BLL
{
    public class clsCommon
    {
        clsLog oLog = new clsLog();

        public Int16 p_iDebugMode = DEBUG_ON;

        public const Int16 RTN_SUCCESS = 1;
        public const Int16 RTN_ERROR = 0;
        public const Int16 DEBUG_ON = 1;
        public const Int16 DEBUG_OFF = 0;
        public string sErrDesc = string.Empty;
        
        
        public DataSet Get_SingleValue(string sQuery, string sConnString, string sErrDesc)
        {
            DataSet oDataset;
            string sFuncName = string.Empty;

            try
            {
                sFuncName = "Get_SingleValue()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                DataAccess oDataAccess = new DataAccess();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_QueryString() ", sFuncName);

                oDataset = oDataAccess.Run_QueryString(sQuery, sConnString);

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

        public string GetSingleValue(string sQuery, string sConnString, string sErrDesc)
        {
            DataSet oDataset;
            string sFuncName = string.Empty;

            try
            {
                sFuncName = "Get_SingleValue()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                DataAccess oDataAccess = new DataAccess();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_QueryString() ", sFuncName);

                oDataset = oDataAccess.Run_QueryString(sQuery, sConnString);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                if (oDataset.Tables[0].Rows.Count > 0)
                    return oDataset.Tables[0].Rows[0][0].ToString();
                else
                    return "-1";
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                return "100";

            }

        }

        public string DeleteDrafts(DataSet oDataSet, SAPbobsCOM.Company oDICompany, string sErrDesc)
        {

            string sFuncName = string.Empty;
            DataTable oDTHeader = new DataTable();
            long lRetCode;

            string sDraftKey = string.Empty;

            try
            {
                sFuncName = "DeleteDrafts()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                oDTHeader = oDataSet.Tables[0];
                SAPbobsCOM.Documents oDraft;

                //clsLogin oLogin = new clsLogin();

                //if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling SAPLogin() ", sFuncName);

                //if (oLogin.SAPLogin(sUserName, sPassword, sDBName, sServer, sLicServerName, sDBUserName, sDBPassword, sErrDesc).ToString().ToUpper() != "SUCCESS")

                //    return "Fail";


                //oDICompany = oLogin.oCompany;

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Start the SAP Transaction", sFuncName);

                if (!oDICompany.InTransaction) oDICompany.StartTransaction();


                for (int iDraftCount = 0; iDraftCount < oDTHeader.Rows.Count; iDraftCount++)
                {
                    oDraft = ((SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts)));

                    sDraftKey = Convert.ToString(oDTHeader.Rows[iDraftCount][0].ToString());

                    oDraft.GetByKey(Convert.ToInt32(sDraftKey));

                    lRetCode = oDraft.Remove();

                    if (lRetCode != 0)
                    {
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Deleted Error!!! RefNumber : " + sDraftKey, sFuncName);

                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Rollback the SAP Transaction ", sFuncName);

                        if (oDICompany.InTransaction) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);

                        sErrDesc = oDICompany.GetLastErrorDescription();

                        //oDICompany.Disconnect();
                        //oDICompany = null;

                    }
                    else

                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Deleted Successfully!!! RefNumber : " + sDraftKey, sFuncName);


                }
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Committed the Transaction ", sFuncName);

                if (oDICompany.InTransaction) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS", sFuncName);

                //oDICompany.Disconnect();
                //oDICompany = null;

                return "SUCCESS";


            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                //throw Ex;
                return sErrDesc;
            }

        }


        public string ConvertDraftToDocument(DataSet oDataSet, SAPbobsCOM.BoObjectTypes oTargetDoc
                                            , SAPbobsCOM.Company oDICompany, string sConnString, string sErrDesc)
        {
            long lRetCode;
            string sFuncName = string.Empty;
            string sDraftKey = string.Empty;
            Int32 iPendingResult;
            string sCardCode = string.Empty;
            string sSYS1DocEntry = string.Empty;
            string sSYS2DocEntry = string.Empty;
            string sPRDocEntry = string.Empty;
            string sPRDocNum = string.Empty;
            string sQueryString = string.Empty;

            clsCopyMarketingDocument oInvRequest = new clsCopyMarketingDocument();

            try
            {
                sFuncName = "ConvertDraftToDocument()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Start the SAP Transaction ", sFuncName);

                if (oDICompany.InTransaction != true) oDICompany.StartTransaction();


                for (int iRowCount = 0; iRowCount < oDataSet.Tables[0].Rows.Count; iRowCount++)
                {

                    SAPbobsCOM.Documents oDocuments = (SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(oTargetDoc));

                    sDraftKey = oDataSet.Tables[0].Rows[iRowCount]["DocEntry"].ToString();

                    iPendingResult = Convert.ToInt32(GetSingleValue("Exec AE_SP021_CheckingPendingApproval '" + sDraftKey + "'", sConnString, sErrDesc));

                    if (iPendingResult != 0)
                    {
                        sErrDesc = "Approval is Pending. Ref Number : " + sDraftKey;
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug(sErrDesc, sFuncName);
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName);
                        return sErrDesc;

                    }

                    oDocuments.GetByKey(Convert.ToInt32(sDraftKey));

                    //sCardCode = oDocuments.Lines.LineVendor;

                    // sQueryString = "select Top 1 LineVendor from DRF1 T0 with (nolock) " +
                    //   "INNER JOIN ODRF T1 WITH(NOLOCK) ON T0.DocEntry =T1.DocEntry WHERE T0.DocEntry='" + sDraftKey + "' AND T1.CardCode ='VA-HARCT'";

                    sQueryString = "select Top 1 LineVendor from DRF1 T0 with (nolock) " +
                         " WHERE T0.DocEntry='" + sDraftKey + "' AND LineVendor ='VA-HARCT'";

                    sCardCode = GetSingleValue(sQueryString, sConnString, sErrDesc);

                    //if (sCardCode.ToString().ToUpper() == "VA-HARHO")
                    //{
                    //    if (ConvertFromDraftoDocument(SAPbobsCOM.BoObjectTypes.oPurchaseQuotations, oDICompany, sDraftKey, sErrDesc) != "SUCCESS") throw new ArgumentException(sErrDesc);
                    //}
                    //else
                    //{
                    //    if (ConvertFromDraftoDocument(SAPbobsCOM.BoObjectTypes.oPurchaseRequest, oDICompany, sDraftKey, sErrDesc) != "SUCCESS") throw new ArgumentException(sErrDesc);
                    //}

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Before Converting the Draft to Document ", sFuncName);
                    lRetCode = oDocuments.SaveDraftToDocument();


                    if (lRetCode != 0)
                    {
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(oDICompany.GetLastErrorDescription() + " - " + oDICompany.GetLastErrorCode(), sFuncName);
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Rollback the SAP Transaction ", sFuncName);

                        if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);

                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR ", sFuncName);

                        return oDICompany.GetLastErrorDescription();

                    }
                    else
                    {

                        if (sCardCode.ToString().ToUpper() == "VA-HARCT")
                        {
                            oDICompany.GetNewObjectCode(out sPRDocEntry);

                            if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("SQL statement " + " " + "select DocNum from OPRQ with (nolock) where DocEntry ='" + sPRDocEntry + "'", sFuncName);
                            sPRDocNum = GetSingleValue("select DocNum from OPRQ with (nolock) where DocEntry ='" + sPRDocEntry + "'", sConnString, sErrDesc);

                            if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Create_InventoryTransferRequest_SYS1() ", sFuncName);
                            if (Create_InventoryTransferRequest_SYS1(oDICompany, sPRDocEntry, sPRDocNum, sErrDesc) != "SUCCESS") throw new ArgumentException(sErrDesc);
                            oDICompany.GetNewObjectCode(out sSYS1DocEntry);

                            if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Create_InventoryTransferRequest_SYS2() ", sFuncName);
                            if (Create_InventoryTransferRequest_SYS2(oDICompany, sPRDocEntry, sPRDocNum, sConnString, sErrDesc) != "SUCCESS") throw new ArgumentException(sErrDesc);
                            oDICompany.GetNewObjectCode(out sSYS2DocEntry);

                            if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Update_InvTransRequestNumber() ", sFuncName);
                            if (Update_InvTransRequestNumber(oDICompany, sPRDocEntry, sSYS1DocEntry, sSYS2DocEntry, sConnString, sErrDesc) != "SUCCESS")
                                throw new ArgumentException(sErrDesc);

                        }
                    }

                }
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Committed the SAP Transaction ", sFuncName);

                if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName);

                return "SUCCESS";

            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Rollback the SAP Transaction ", sFuncName);
                if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                //GC.Collect();
                //oDICompany.Disconnect();
                //oDICompany = null;

                throw Ex;

            }
        }

        string Update_InvTransRequestNumber(SAPbobsCOM.Company oDICompany, string sPRDocEntry, string sSYS1DocEntry
                                                , string sSYS2DocEntry, string sConnString, string sErrDesc)
        {
            string sFuncName = string.Empty;
            double lRetCode;
            SAPbobsCOM.Documents oPurchaseOrder;
            DataSet oDSReqDocNum;
            Int32 iCount;

            try
            {
                sFuncName = "Update_InvTransRequestNumber()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                oPurchaseOrder = (SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseRequest));

                oPurchaseOrder.GetByKey(Convert.ToInt32(sPRDocEntry));

                oDSReqDocNum = Get_SingleValue("select DocNum from OWTQ with (nolock) where DocEntry in('" + sSYS1DocEntry + "','" + sSYS2DocEntry + "')", sConnString, sErrDesc);

                //oPurchaseOrder.UserFields.Fields.Item("U_ITR1").Value = GetSingleValue("", "", sErrDesc);// sSYS1DocEntry;
                //oPurchaseOrder.UserFields.Fields.Item("U_ITR2").Value = sSYS2DocEntry;

                oPurchaseOrder.UserFields.Fields.Item("U_ITR1").Value = oDSReqDocNum.Tables[0].Rows[0][0].ToString();
                oPurchaseOrder.UserFields.Fields.Item("U_ITR2").Value = oDSReqDocNum.Tables[0].Rows[1][0].ToString();

                iCount = oPurchaseOrder.Lines.Count;

                for (int iLineCount = 0; iLineCount < iCount; iLineCount++)
                {
                    oPurchaseOrder.Lines.SetCurrentLine(iLineCount);

                    if (oPurchaseOrder.Lines.LineVendor.ToString().ToUpper() == "VA-HARCT" &&
                        oPurchaseOrder.Lines.LineStatus == SAPbobsCOM.BoStatus.bost_Open)
                    {
                        oPurchaseOrder.Lines.LineStatus = SAPbobsCOM.BoStatus.bost_Close;
                    }

                }

                lRetCode = oPurchaseOrder.Update();

                if (lRetCode != 0)
                {
                    sErrDesc = oDICompany.GetLastErrorDescription();
                    throw new ArgumentException(sErrDesc);
                }

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName);
                return "SUCCESS";

            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        string Create_InventoryTransferRequest_SYS1(SAPbobsCOM.Company oDICompany, string sDocEntry, string sPRDocNum, string sErrDesc)
        {
            string sFuncName = string.Empty;
            double lRetCode;
            SAPbobsCOM.Documents oPurchaseRequest;
            SAPbobsCOM.StockTransfer oInvTransRequest;

            try
            {
                sFuncName = "Create_InventoryTransferRequest_SYS1()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                oPurchaseRequest = (SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseRequest));
                oInvTransRequest = (SAPbobsCOM.StockTransfer)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest));


                oPurchaseRequest.GetByKey(Convert.ToInt32(sDocEntry));

                oInvTransRequest.CardCode = "VA-HARCT";//oPurchaseOrder.CardCode;
                oInvTransRequest.FromWarehouse = "01CKT";
                oInvTransRequest.ToWarehouse = "01CKT";
                oInvTransRequest.UserFields.Fields.Item("U_PR_No").Value = sPRDocNum;
                oInvTransRequest.Series = 60;

                //copy the lines
                int count = oPurchaseRequest.Lines.Count - 1;
                SAPbobsCOM.StockTransfer_Lines oTargetLines = oInvTransRequest.Lines;
                SAPbobsCOM.Document_Lines oBaseLines = oPurchaseRequest.Lines;
                for (int i = 0; i <= count; i++)
                {
                    oBaseLines.SetCurrentLine(i);

                    // if (oBaseLines.ItemCode.ToString().ToUpper() != "VA-HARCT") continue;
                    if (oBaseLines.LineVendor.ToString().ToUpper() != "VA-HARCT") continue;

                    oTargetLines.ItemCode = oBaseLines.ItemCode;
                    oTargetLines.Quantity = oBaseLines.InventoryQuantity;
                    oTargetLines.FromWarehouseCode = "01CKT";
                    oTargetLines.WarehouseCode = "01CKT";

                    oTargetLines.Add();


                }

                lRetCode = oInvTransRequest.Add();

                if (lRetCode != 0)
                {
                    sErrDesc = oDICompany.GetLastErrorDescription();
                    throw new ArgumentException(sErrDesc);

                }


                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName);
                return "SUCCESS";
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        string Create_InventoryTransferRequest_SYS2(SAPbobsCOM.Company oDICompany, string sDocEntry, string sPRDocNum,
                                                    string sConnString, string sErrDesc)
        {
            string sFuncName = string.Empty;
            double lRetCode;
            SAPbobsCOM.Documents oPurchaseRequest;
            SAPbobsCOM.StockTransfer oInvTransRequest;
            string sWhsCode = string.Empty;


            try
            {
                sFuncName = "Create_InventoryTransferRequest_SYS2()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                oPurchaseRequest = (SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseRequest));
                oInvTransRequest = (SAPbobsCOM.StockTransfer)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest));

                sWhsCode = GetSingleValue("select Top 1 WhsCode from PRQ1 with (nolock) where DocEntry ='" + sDocEntry + "'", sConnString, sErrDesc);


                oPurchaseRequest.GetByKey(Convert.ToInt32(sDocEntry));

                oInvTransRequest.CardCode = "VA-HARCT";//oPurchaseRequest.CardCode;
                oInvTransRequest.FromWarehouse = "01CKT";
                oInvTransRequest.ToWarehouse = sWhsCode;
                //oInvTransRequest.ToWarehouse = "11APK";
                oInvTransRequest.UserFields.Fields.Item("U_PR_No").Value = sPRDocNum;
                oInvTransRequest.Series = 61;

                //copy the lines
                int count = oPurchaseRequest.Lines.Count - 1;
                SAPbobsCOM.StockTransfer_Lines oTargetLines = oInvTransRequest.Lines;
                SAPbobsCOM.Document_Lines oBaseLines = oPurchaseRequest.Lines;
                for (int i = 0; i <= count; i++)
                {
                    oBaseLines.SetCurrentLine(i);

                    // if (oBaseLines.ItemCode.ToString().ToUpper() != "VA-HARCT") continue;
                    if (oBaseLines.LineVendor.ToString().ToUpper() != "VA-HARCT") continue;

                    oTargetLines.ItemCode = oBaseLines.ItemCode;
                    oTargetLines.Quantity = oBaseLines.InventoryQuantity;
                    oTargetLines.FromWarehouseCode = "01CKT";
                    oTargetLines.WarehouseCode = sWhsCode;

                    oTargetLines.Add();
                }

                lRetCode = oInvTransRequest.Add();

                if (lRetCode != 0)
                {
                    sErrDesc = oDICompany.GetLastErrorDescription();
                    throw new ArgumentException(sErrDesc);
                }


                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName);
                return "SUCCESS";
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        public SAPbobsCOM.Company ConnectToTargetCompany(SAPbobsCOM.Company oCompany, string sUserName, string sPassword, string sDBName,
                                                         string sServer, string sLicServerName, string sDBUserName
                                                        , string sDBPassword, string sErrDesc)
        {
            string sFuncName = string.Empty;
            long lRetCode;

            try
            {
                sFuncName = "ConnectToTargetCompany()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                ////if (oCompany != null)
                ////{
                ////    if (oCompany.CompanyDB == sDBName)
                ////    {
                ////        return oCompany;
                ////    }
                ////    else
                ////    {
                ////        oCompany.Disconnect();
                ////        oCompany = null;
                ////    }
                ////}

                oCompany = new SAPbobsCOM.Company();
                oCompany.Server = sServer;
                oCompany.LicenseServer = sLicServerName;
                oCompany.DbUserName = sDBUserName;
                oCompany.DbPassword = sDBPassword;
                oCompany.language = SAPbobsCOM.BoSuppLangs.ln_English;
                oCompany.UseTrusted = false;
                oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2008;


                oCompany.CompanyDB = sDBName;// sDataBaseName;
                oCompany.UserName = sUserName;
                oCompany.Password = sPassword;

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Connecting the Database...", sFuncName);

                lRetCode = oCompany.Connect();

                if (lRetCode != 0)
                {

                    throw new ArgumentException(oCompany.GetLastErrorDescription());
                }
                else
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Company Connection Established", sFuncName);
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS", sFuncName);
                    return oCompany;
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

        public string MAcknowledgment(DataSet oDTCompanyList, string sUserName, string sPassword, String sCompanyDB, string sErrDesc)
        {
            string sFuncName = string.Empty;
            DataView oDTView = new DataView();
            DataSet oDTUserRole = new DataSet();

            try
            {
                sFuncName = "MAcknowledgment()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (oDTCompanyList != null)
                {

                    oDTView = oDTCompanyList.Tables[0].DefaultView;

                    oDTView.RowFilter = "U_DBName= '" + sCompanyDB + "'";
                    clsLogin AccessSAP = new clsLogin();

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_SingleValue()", sFuncName);

                    oDTUserRole = Get_SingleValue("Select U_UserRole from OUSR with (nolock) where  U_UserRole ='DRIVER'and USER_CODE='" + sUserName + "'",
                                    oDTView[0]["U_ConnString"].ToString(), sErrDesc);

                    if (oDTUserRole.Tables[0].Rows.Count == 0)
                    {
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Your Role is not a Driver", sFuncName);

                        return "Your Role is not a Driver";
                    }
                    else
                    {
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling SAPLogin()", sFuncName);

                        return AccessSAP.UserValidation(sUserName, sPassword, oDTView[0]["U_ConnString"].ToString());

                        //return (AccessSAP.SAPLogin(sUserName, sPassword, sCompanyDB, oDTView[0]["U_Server"].ToString(), oDTView[0]["U_LicenseServer"].ToString()
                        // , oDTView[0]["U_DBUserName"].ToString(), oDTView[0]["U_DBPassword"].ToString(), sErrDesc));
                    }
                }

                else
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("There is No Company List in the Holding Company ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName);
                return "There is No Company List in the Holding Company";

            }

            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        public DataSet MGetOutlet(DataSet oDTCompanyList, string sUserName, string sPassword, string sCompanyDB, string sErrDesc)
        {
            string sFuncName = string.Empty;
            DataView oDTView = new DataView();
            DataSet oDTUserInformation = new DataSet();
            DataSet oDTOutlet = new DataSet();

            clsLogin oLogin = new clsLogin();
            clsOutlet oOutlet = new clsOutlet();
            clsLog oLog = new clsLog();
            try
            {
                sFuncName = "MGetOutlet()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (oDTCompanyList != null)
                {

                    oDTView = oDTCompanyList.Tables[0].DefaultView;
                    oDTView.RowFilter = "U_DBName= '" + sCompanyDB + "'";
                    clsLogin AccessSAP = new clsLogin();

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_UserInformation()", sFuncName);

                    oDTUserInformation = oLogin.Get_UserInformation(sUserName, oDTView[0]["U_ConnString"].ToString(), sErrDesc);

                    if ((oDTUserInformation.Tables.Count > 0) && (oDTView.Count > 0))
                    {
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Outlets()", sFuncName);

                        oDTOutlet = oOutlet.Get_Outlets(Convert.ToString(oDTUserInformation.Tables[0].Rows[0]["U_AE_Access"])
                            , Convert.ToString(oDTUserInformation.Tables[0].Rows[0]["WhsCode"]), Convert.ToString(oDTView[0]["U_ConnString"]),
                            Convert.ToInt16(oDTUserInformation.Tables[0].Rows[0]["U_ApprovalLevel"]), sErrDesc);


                    }

                    if (oDTOutlet.Tables[0].Rows.Count == 0)
                    {
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Outlet Dataset is Empty", sFuncName);
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS", sFuncName);
                    }
                    else
                    {
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS", sFuncName);
                        return (oDTOutlet);
                    }
                }

                else
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Dataset is NULL", sFuncName);
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS", sFuncName);

                }
                return null;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
            finally
            {
                oDTUserInformation.Dispose();
                oDTOutlet.Dispose();
            }
        }

        public string Get_ConnectionString(DataSet oDTCompanyList, string sCompanyDB, string sErrDesc)
        {

            DataView oDTView = new DataView();
            string sFuncName = string.Empty;
            string sConnString = string.Empty;

            try
            {
                sFuncName = "Get_ConnectionString()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (oDTCompanyList != null && oDTCompanyList.Tables[0].Rows.Count > 0)
                {

                    oDTView = oDTCompanyList.Tables[0].DefaultView;
                    oDTView.RowFilter = "U_DBName= '" + sCompanyDB + "'";
                    sConnString = oDTView[0]["U_ConnString"].ToString();

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                    return sConnString;
                }

                else
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Please Update the Connecting String in DBInfo Table :  " + sCompanyDB, sFuncName);
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);
                    return null;
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

        string ConvertFromDraftoDocument(SAPbobsCOM.BoObjectTypes oTargeType, SAPbobsCOM.Company oDICompany
                                    , string sDocEntry, string sErrDesc)
        {
            string sFuncName = string.Empty;
            double lRetCode;
            SAPbobsCOM.Documents oDraft;
            //DataSet oDSReqDocNum;

            try
            {
                sFuncName = "ConvertFromDraftoDocument()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                oDraft = ((SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts)));
                SAPbobsCOM.Documents oTarDoc = ((SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(oTargeType)));


                oDraft.GetByKey(Convert.ToInt32(sDocEntry));

                oTarDoc.ReqType = oDraft.ReqType;
                oTarDoc.Requester = oDraft.Requester;//.Trim();
                oTarDoc.RequriedDate = oDraft.RequriedDate;
                oTarDoc.DocDate = oDraft.DocDate;
                oTarDoc.DocDueDate = oDraft.DocDueDate;
                oTarDoc.TaxDate = oDraft.TaxDate;

                //oTarDoc.documen = oDraft.TaxDate;

                oTarDoc.DocumentsOwner = oDraft.DocumentsOwner;

                oTarDoc.UserFields.Fields.Item("U_Urgent").Value = oDraft.UserFields.Fields.Item("U_Urgent").Value;
                oTarDoc.UserFields.Fields.Item("U_OrderTime").Value = oDraft.UserFields.Fields.Item("U_OrderTime").Value;

                for (int iCount = 0; iCount < oDraft.Lines.Count; iCount++)
                {
                    oDraft.Lines.SetCurrentLine(iCount);

                    //oTarDoc.Lines.BaseType = oDraft.Lines.BaseType;
                    //oTarDoc.Lines.BaseEntry = oDraft.Lines.BaseEntry;
                    //oTarDoc.Lines.BaseLine = oDraft.Lines.BaseLine;

                    oTarDoc.Lines.ItemCode = oDraft.Lines.ItemCode;
                    oTarDoc.Lines.Quantity = oDraft.Lines.Quantity;
                    oTarDoc.Lines.UnitPrice = oDraft.Lines.UnitPrice;
                    oTarDoc.Lines.LineTotal = oDraft.Lines.LineTotal;
                    oTarDoc.Lines.RequiredDate = oDraft.Lines.RequiredDate;

                    oTarDoc.Lines.WarehouseCode = oDraft.Lines.WarehouseCode;

                    //oTarDoc.UserFields.Fields.Item("U_ApprovalLevel").Value = oDraft.UserFields.Fields.Item("U_ApprovalLevel").Value;
                    //oTarDoc.UserFields.Fields.Item("U_L1ApprovalStatus").Value = oDraft.UserFields.Fields.Item("U_L1ApprovalStatus").Value;
                    //oTarDoc.UserFields.Fields.Item("U_L2ApprovalStatus").Value = oDraft.UserFields.Fields.Item("U_L2ApprovalStatus").Value;
                    //oTarDoc.UserFields.Fields.Item("U_L1Approver").Value = oDraft.UserFields.Fields.Item("U_L1Approver").Value;
                    //oTarDoc.UserFields.Fields.Item("U_L2Approver").Value = oDraft.UserFields.Fields.Item("U_L2Approver").Value;
                    //oTarDoc.UserFields.Fields.Item("U_DeliveryCharge").Value = oDraft.UserFields.Fields.Item("U_DeliveryCharge").Value;



                    oTarDoc.Lines.Add();

                }

                lRetCode = oTarDoc.Add();
                if (lRetCode != 0)
                {
                    sErrDesc = oDICompany.GetLastErrorDescription();
                    throw new ArgumentException(sErrDesc);

                }



                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName);

                return "SUCCESS";
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                //GC.Collect();
                throw Ex;

            }
        }
    }
}
