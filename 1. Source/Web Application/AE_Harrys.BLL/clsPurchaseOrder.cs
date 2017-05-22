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
    public class clsPurchaseOrder
    {
        clsLog oLog = new clsLog();

        public Int16 p_iDebugMode = DEBUG_ON;

        public const Int16 RTN_SUCCESS = 1;
        public const Int16 RTN_ERROR = 0;
        public const Int16 DEBUG_ON = 1;
        public const Int16 DEBUG_OFF = 0;
        public string sErrDesc = string.Empty;



        public DataSet Get_OpenPOList(string sSupplier, string sSupplierType, string sAccessType,
                                      string sOutletCode, string sConnString, string sErrDesc)
        {
            DataSet oDataset;
            string sFuncName = string.Empty;
            string sProcedureName = string.Empty;

            try
            {
                sFuncName = "Get_OpenPOList()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                DataAccess oDataAccess = new DataAccess();

                sProcedureName = "EXEC [AE_SP013_GetPOList] '" + sSupplier + "','" + sSupplierType + "','" + sAccessType + "','" + sOutletCode + "'";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_StoredProcedure() ", sFuncName);
                oDataset = oDataAccess.Run_StoredProcedure(sProcedureName, sConnString);

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

        public DataSet Get_OpenPOListDetails(string sSupplier, string sOutlet, string sAccessType, string sConnString, string sErrDesc)
        {
            DataSet oDataset;
            string sFuncName = string.Empty;
            string sProcedureName = string.Empty;

            try
            {
                sFuncName = "Get_OpenPOListDetails()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                DataAccess oDataAccess = new DataAccess();

                sProcedureName = "EXEC [AE_SP014_Get_POListDetails] '" + sOutlet + "','" + sAccessType + "','" + sSupplier + "'";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_StoredProcedure() ", sFuncName);

                oDataset = oDataAccess.Run_StoredProcedure(sProcedureName, sConnString);

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

        public string Update_ApprovalStatus(DataSet oDataset, SAPbobsCOM.Company oDICompany, string sConnString, string sErrDesc)
        {

            DataView oDVHeader = new DataView();
            DataView oDVDetail = new DataView();
            DataTable oDTDistinct = new DataTable();

            string sFuncName = string.Empty;
            string sDocEntry = string.Empty;
            string sRejected = string.Empty;
            string sApproved = string.Empty;
            string sDeliveryCharge = string.Empty;
            string sSupplierCode = string.Empty;
            //double dDeliveryCharge = 0.0;
            clsCommon oCommon = new clsCommon();
            clsCopyMarketingDocument oDelEmptyRow = new clsCopyMarketingDocument();
            string sUserName = string.Empty;
            string sUserRole = string.Empty;
            string sReturnMsg = string.Empty;

            long lRetCode;

            try
            {

                sFuncName = "Update_ApprovalStatus()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                SAPbobsCOM.Documents oPRDrafts;

                oDVHeader = oDataset.Tables[0].DefaultView;
                oDVDetail = oDataset.Tables[1].DefaultView;



                oDTDistinct = oDVHeader.Table.DefaultView.ToTable(true, "DocEntry");


                for (int IntHeader = 0; IntHeader <= oDTDistinct.Rows.Count - 1; IntHeader++)
                {

                    oPRDrafts = (SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts));

                    sDocEntry = oDVHeader[IntHeader]["DocEntry"].ToString();
                    // sRejected = oDVHeader[IntHeader]["Rejected"].ToString();
                    sApproved = oDVHeader[IntHeader]["Approved"].ToString();
                    sDeliveryCharge = oDVHeader[IntHeader]["DeliveryCharge"].ToString();

                    sUserName = oDVHeader[IntHeader]["UserName"].ToString();
                    sUserRole = oDVHeader[IntHeader]["UserRole"].ToString();

                    if (oPRDrafts.GetByKey(Convert.ToInt16(sDocEntry)) == false) continue;

                    oDVDetail.RowFilter = "DocEntry = '" + sDocEntry + "'";

                    for (int IntLine = 0; IntLine <= oDVDetail.Count - 1; IntLine++)
                    {
                        oPRDrafts.Lines.SetCurrentLine(Convert.ToInt16(oDVDetail[IntLine]["LineNum"].ToString()));

                        oPRDrafts.Lines.Quantity = Convert.ToDouble(oDVDetail[IntLine]["OrderQuantity"].ToString());
                        oPRDrafts.Lines.UserFields.Fields.Item("U_DeliveryCharge").Value = oDVDetail[IntLine]["DelChargeUDF"].ToString();

                        if (Convert.ToInt16(sUserRole) == 1)
                        {
                            if (sApproved.ToUpper() == "Y")
                            {
                                oPRDrafts.Lines.UserFields.Fields.Item("U_L1ApprovalStatus").Value = "Approved";
                                oPRDrafts.Lines.UserFields.Fields.Item("U_L1Approver").Value = sUserName;
                                oPRDrafts.Lines.UserFields.Fields.Item("U_MR_OIC_Rmks").Value = oDVDetail[IntLine]["Remarks"].ToString();
                                oPRDrafts.Lines.UserFields.Fields.Item("U_MR_App_Rmks").Value = oDVDetail[IntLine]["ApproverRemarks"].ToString();
                            }

                            else
                            {
                                oPRDrafts.Lines.UserFields.Fields.Item("U_L1ApprovalStatus").Value = "Rejected";
                                oPRDrafts.Lines.UserFields.Fields.Item("U_L1Approver").Value = sUserName;
                                oPRDrafts.Lines.UserFields.Fields.Item("U_MR_OIC_Rmks").Value = oDVDetail[IntLine]["Remarks"].ToString();
                                oPRDrafts.Lines.UserFields.Fields.Item("U_MR_App_Rmks").Value = oDVDetail[IntLine]["ApproverRemarks"].ToString();
                            }
                        }

                        else if (Convert.ToInt16(sUserRole) == 2)
                        {
                            if (sApproved.ToUpper() == "Y")
                            {

                                oPRDrafts.Lines.UserFields.Fields.Item("U_L2ApprovalStatus").Value = "Approved";
                                oPRDrafts.Lines.UserFields.Fields.Item("U_L2Approver").Value = sUserName;
                                oPRDrafts.Lines.UserFields.Fields.Item("U_MR_OIC_Rmks").Value = oDVDetail[IntLine]["Remarks"].ToString();
                                oPRDrafts.Lines.UserFields.Fields.Item("U_MR_App_Rmks").Value = oDVDetail[IntLine]["ApproverRemarks"].ToString();
                            }
                            else
                            {
                                oPRDrafts.Lines.UserFields.Fields.Item("U_L2Approver").Value = sUserName;
                                oPRDrafts.Lines.UserFields.Fields.Item("U_L2ApprovalStatus").Value = "Rejected";
                                oPRDrafts.Lines.UserFields.Fields.Item("U_MR_OIC_Rmks").Value = oDVDetail[IntLine]["Remarks"].ToString();
                                oPRDrafts.Lines.UserFields.Fields.Item("U_MR_App_Rmks").Value = oDVDetail[IntLine]["ApproverRemarks"].ToString();
                            }

                        }

                    }

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting the SAP Transaction ", sFuncName);

                    if (oDICompany.InTransaction == false) oDICompany.StartTransaction();

                    lRetCode = oPRDrafts.Update();

                    if (lRetCode != 0)
                    {
                        sErrDesc = "Document Number : " + sDocEntry + " - " + oDICompany.GetLastErrorDescription(); throw new ArgumentException(sErrDesc);
                    }


                    else
                    {
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling DeleteZeroQuantity() to Delete the Zero Quantity Line Items ", sFuncName);
                        if (oDelEmptyRow.DeleteZeroQuantity(oPRDrafts, sDocEntry, oDICompany, sErrDesc) != "SUCCESS") throw new ArgumentException(sErrDesc);

                        Int32 iPendingResult;
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling GetSingleValue() for Getting Approval Status ", sFuncName);
                        iPendingResult = Convert.ToInt32(oCommon.GetSingleValue("Exec AE_SP021_CheckingPendingApproval '" + sDocEntry + "'", sConnString, sErrDesc));

                        if (iPendingResult != 0)
                        {
                            sErrDesc = " Approval is Pending. Ref Number : " + sDocEntry;
                            if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug(sErrDesc, sFuncName);
                            if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName);

                            sReturnMsg += sErrDesc;

                        }
                        else
                        {
                            if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling ConvertDraftToDocument() ", sFuncName);
                            string sResult = ConvertDraftToDocument(oPRDrafts, sDocEntry, oDICompany, sConnString, sErrDesc);
                            if (sResult != "SUCCESS")
                            {
                                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("After Calling ConvertDraftToDocument() " + sResult, sFuncName);
                                return sResult;
                            }
                        }
                    }


                }
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Committed the SAP Transation ", sFuncName);
                if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                //oDICompany.Disconnect();
                //oDICompany = null;
                if (sReturnMsg.ToString() == string.Empty)
                    sReturnMsg = "SUCCESS";


                return sReturnMsg;

            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Rollback the SAP Transation ", sFuncName);
                if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);

                //oDICompany.Disconnect();
                //oDICompany = null;
                //throw Ex;
                return sErrDesc;
            }
        }


        public string ConvertDraftToDocument(SAPbobsCOM.Documents oDocuments, string sDocEntry, SAPbobsCOM.Company oDICompany
                                            , string sConnString, string sErrDesc)
        {
            long lRetCode;
            string sFuncName = string.Empty;
            //Int32 iPendingResult;
            string sCardCode = string.Empty;
            string sSYS1DocEntry = string.Empty;
            string sSYS2DocEntry = string.Empty;
            string sPRDocEntry = string.Empty;
            string sPRDocNum = string.Empty;
            string sQueryString = string.Empty;

            clsCopyMarketingDocument oInvRequest = new clsCopyMarketingDocument();
            clsCommon oCommon = new clsCommon();


            try
            {
                sFuncName = "ConvertDraftToDocument()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);


                //if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling GetSingleValue() for Getting Approval Status ", sFuncName);
                //iPendingResult = Convert.ToInt32(oCommon.GetSingleValue("Exec AE_SP021_CheckingPendingApproval '" + sDocEntry + "'", sConnString, sErrDesc));

                //if (iPendingResult != 0)
                //{
                //    sErrDesc = "Approval is Pending. Ref Number : " + sDocEntry;
                //    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug(sErrDesc, sFuncName);
                //    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName);
                //    return "SUCCESS";

                //}

                sQueryString = "select Top 1 LineVendor from DRF1 T0 with (nolock) " +
                     " WHERE T0.DocEntry='" + sDocEntry + "' AND LineVendor ='VA-HARCT'";

                sCardCode = oCommon.GetSingleValue(sQueryString, sConnString, sErrDesc);

                oDocuments.GetByKey(Convert.ToInt32(sDocEntry));

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Before Converting the Draft to Document ", sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Testing ", sFuncName);
                lRetCode = oDocuments.SaveDraftToDocument();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("lRetCode " + lRetCode, sFuncName);
                if (lRetCode != 0)
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(oDICompany.GetLastErrorDescription() + " - " + oDICompany.GetLastErrorCode(), sFuncName);
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Rollback the SAP Transaction ", sFuncName);

                    if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR ", sFuncName);

                    sErrDesc = oDICompany.GetLastErrorDescription();

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("ERROR Description : " + sErrDesc, sFuncName);

                    return sErrDesc;

                    
                }
                else
                {

                    if (sCardCode.ToString().ToUpper() == "VA-HARCT")
                    {
                        oDICompany.GetNewObjectCode(out sPRDocEntry);

                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("SQL statement " + " " + "select DocNum from OPRQ with (nolock) where DocEntry ='" + sPRDocEntry + "'", sFuncName);
                        sPRDocNum = oCommon.GetSingleValue("select DocNum from OPRQ with (nolock) where DocEntry ='" + sPRDocEntry + "'", sConnString, sErrDesc);

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




                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName);

                return "SUCCESS";

            }
            catch (Exception Ex)
            {
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("ERROR Description :  " + Ex.Message.ToString(), sFuncName);
                sErrDesc = Ex.Message.ToString();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                //GC.Collect();
                //throw new System.Exception(sErrDesc);
                return sErrDesc;

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
            clsCommon oCommon = new clsCommon();

            try
            {
                sFuncName = "Create_InventoryTransferRequest_SYS2()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                oPurchaseRequest = (SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseRequest));
                oInvTransRequest = (SAPbobsCOM.StockTransfer)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest));

                sWhsCode = oCommon.GetSingleValue("select Top 1 WhsCode from PRQ1 with (nolock) where DocEntry ='" + sDocEntry + "'", sConnString, sErrDesc);


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

        string Update_InvTransRequestNumber(SAPbobsCOM.Company oDICompany, string sPRDocEntry, string sSYS1DocEntry
                                            , string sSYS2DocEntry, string sConnString, string sErrDesc)
        {
            string sFuncName = string.Empty;
            double lRetCode;
            SAPbobsCOM.Documents oPurchaseOrder;
            DataSet oDSReqDocNum;
            Int32 iCount;
            clsCommon oCommon = new clsCommon();


            try
            {
                sFuncName = "Update_InvTransRequestNumber()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                oPurchaseOrder = (SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseRequest));

                oPurchaseOrder.GetByKey(Convert.ToInt32(sPRDocEntry));

                oDSReqDocNum = oCommon.Get_SingleValue("select DocNum from OWTQ with (nolock) where DocEntry in('" + sSYS1DocEntry + "','" + sSYS2DocEntry + "')", sConnString, sErrDesc);

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
    }
}
