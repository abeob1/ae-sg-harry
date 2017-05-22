using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.IO;

using System.Text.RegularExpressions;

using AE_Harrys.DAL;
using AE_Harrys.Common;

namespace AE_Harrys.BLL
{
    public class clsCopyMarketingDocument
    {
        clsLog oLog = new clsLog();

        public Int16 p_iDebugMode = DEBUG_ON;

        public const Int16 RTN_SUCCESS = 1;
        public const Int16 RTN_ERROR = 0;
        public const Int16 DEBUG_ON = 1;
        public const Int16 DEBUG_OFF = 0;

        clsCommon oCommon = new clsCommon();
        public string sErrDesc = string.Empty;
        string sInvTransDocEntry = string.Empty;

        public string CopyPurReqToPurchaseQuot(SAPbobsCOM.BoObjectTypes oBaseType, int oBaseEntry, SAPbobsCOM.BoObjectTypes oTarType,
                                 SAPbobsCOM.Company oDICompany, string sErrDesc, bool IsDraft)
        {

            double lRetCode = 0;
            string sFuncName = string.Empty;

            try
            {
                sFuncName = "CopyPurReqToPurchaseQuot()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);

                SAPbobsCOM.Documents oBaseDoc = ((SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(oBaseType)));
                if (oBaseDoc.GetByKey(oBaseEntry))
                {
                    //base document found, copy to target doc
                    SAPbobsCOM.Documents oTarDoc = ((SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(oTarType)));

                    if (IsDraft == false)
                    {
                        oTarDoc = ((SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(oTarType)));
                    }
                    else
                    {
                        oTarDoc = ((SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts)));
                        oTarDoc.DocObjectCode = oTarType;
                    }


                    //todo: copy the cardcode, docduedate, lines
                    oTarDoc.CardCode = "VA-HARHO";// oBaseDoc.CardCode;
                    oTarDoc.DocDueDate = oBaseDoc.DocDueDate;
                    oTarDoc.DocDate = oBaseDoc.DocDate;
                    oTarDoc.TaxDate = oBaseDoc.TaxDate;
                    oTarDoc.RequriedDate = oBaseDoc.RequriedDate;



                    //copy the lines
                    int count = oBaseDoc.Lines.Count - 1;
                    SAPbobsCOM.Document_Lines oTargetLines = oTarDoc.Lines;
                    for (int i = 0; i <= count; i++)
                    {
                        if (i != 0)
                        {
                            oTargetLines.Add();
                        }
                        oTargetLines.BaseType = Convert.ToInt32(oBaseType);
                        oTargetLines.BaseEntry = oBaseEntry;
                        oTargetLines.BaseLine = i;
                        //oTargetLines.ItemCode = oBaseDoc.Lines.ItemCode;
                        //oTargetLines.Quantity = oBaseDoc.Lines.Quantity;
                        //  oTarDoc.CardCode = oBaseDoc.Lines.LineVendor;
                    }

                    lRetCode = oTarDoc.Add();


                    if (lRetCode != 0)
                    {
                        sErrDesc = oDICompany.GetLastErrorDescription();

                        throw new ArgumentException(sErrDesc);

                    }
                    else
                    {
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with SUCCESS ", sFuncName);
                        return "SUCCESS";
                    }

                }
                else
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Base Document Not Found ", sFuncName);
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with SUCCESS ", sFuncName);
                    return "SUCCESS";
                }
            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message;
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);
                return ex.Message.ToString();
            }
        }

        public string PurReqToPurchaseQuot(DataSet oDataSet, SAPbobsCOM.BoObjectTypes oBaseType, int oBaseEntry
                                        , SAPbobsCOM.BoObjectTypes oTarType, SAPbobsCOM.Company oDICompany, string sConnString, string sErrDesc)
        {

            double lRetCode = 0;
            string sFuncName = string.Empty;
            DataTable oDTHeader = new DataTable();
            DataTable oDTLine = new DataTable();
            DateTime dtDocDate;
            string sWhsCode = string.Empty;
            clsCommon oGetSingleValue = new clsCommon();
            string sLineNum = string.Empty;
            string sItemCode = string.Empty;
            string sQueryString = string.Empty;
            string sRequester = string.Empty;

            try
            {
                sFuncName = "PurReqToPurchaseQuot()";

                SAPbobsCOM.Recordset oRecordSet = null;
                oRecordSet = ((SAPbobsCOM.Recordset)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)));

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);

                SAPbobsCOM.Documents oBaseDoc = ((SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(oBaseType)));
                if (oBaseDoc.GetByKey(oBaseEntry))
                {
                    //base document found, copy to target doc
                    SAPbobsCOM.Documents oTarDoc = ((SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(oTarType)));


                    oTarDoc = ((SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts)));
                    oTarDoc.DocObjectCode = oTarType;

                    oDTHeader = oDataSet.Tables[0];
                    oDTLine = oDataSet.Tables[1];

                    string[] sPostingDate = oDTHeader.Rows[0]["PostingDate"].ToString().Split(' ');
                    dtDocDate = Convert.ToDateTime(sPostingDate[0]);
                    //dtDocDate = Convert.ToDateTime(oDTHeader.Rows[0]["PostingDate"].ToString());
                    sWhsCode = oDTHeader.Rows[0]["Outlet"].ToString();
                    sRequester = oDTHeader.Rows[0]["UserCode"].ToString().Trim();

                    //todo: copy the cardcode, docduedate, lines
                    oTarDoc.CardCode = "VA-HARHO";// oBaseDoc.CardCode;
                    string[] sDeliveryDate = oDTHeader.Rows[0]["DeliveryDate"].ToString().Split(' ');
                    oTarDoc.RequriedDate = Convert.ToDateTime(sDeliveryDate[0]);
                    oTarDoc.DocumentsOwner = Convert.ToInt16(oDTHeader.Rows[0]["UserId"]);
                    oTarDoc.UserFields.Fields.Item("U_UserID").Value = sRequester;

                    //copy the lines
                    int count = oDTLine.Rows.Count;
                    SAPbobsCOM.Document_Lines oTargetLines = oTarDoc.Lines;

                    for (int iRow = 0; iRow < count; iRow++)
                    {

                        if (string.IsNullOrEmpty(oDTLine.Rows[iRow]["OrderQuantity"].ToString()) == true) continue;
                        if (Convert.ToDecimal(oDTLine.Rows[iRow]["OrderQuantity"].ToString()) == 0) continue;
                        sItemCode = oDTLine.Rows[iRow]["ItemCode"].ToString();

                        oTargetLines.ItemCode = sItemCode;
                        oTargetLines.Quantity = Convert.ToDouble(oDTLine.Rows[iRow]["OrderQuantity"].ToString());
                        oTargetLines.UnitPrice = Convert.ToDouble(oDTLine.Rows[iRow]["Price"].ToString());
                        oTargetLines.LineTotal = Convert.ToDouble(oDTLine.Rows[iRow]["Total"].ToString());
                        oTargetLines.VatGroup = "TX7";
                        oTargetLines.WarehouseCode = sWhsCode;

                        string sCostCenter = oGetSingleValue.GetSingleValue("select U_CostCenter [CostCenter] from OWHS where WhsCode ='" + sWhsCode + "'", sConnString, sErrDesc);

                        oTargetLines.COGSCostingCode = sCostCenter;
                        oTargetLines.CostingCode = sCostCenter;
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("After getting & assigning the costing code from warehouse master : " + sWhsCode, sFuncName);

                        string[] sDelDate = oDTLine.Rows[iRow]["DeliveryDate"].ToString().Split(' ');
                        oTargetLines.RequiredDate = Convert.ToDateTime(sDelDate[0]);
                        oTargetLines.UserFields.Fields.Item("U_MR_OIC_Rmks").Value = oDTLine.Rows[iRow]["Remarks"].ToString();
                        oTargetLines.UserFields.Fields.Item("U_MR_App_Rmks").Value = oDTLine.Rows[iRow]["ApproverRemarks"].ToString();

                        sQueryString = "select LineNum from PRQ1 WITH (NOLOCK) where ItemCode='" + sItemCode + "' and DocEntry ='" + oBaseEntry + "'";

                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling GetSingleValue() for Getting PR Line Number", sFuncName);

                        sLineNum = oGetSingleValue.GetSingleValue(sQueryString, sConnString, sErrDesc);

                        if (sLineNum != "-1")
                        {
                            oTargetLines.BaseType = Convert.ToInt32(oBaseType);
                            oTargetLines.BaseEntry = oBaseEntry;
                            oTargetLines.BaseLine = Convert.ToInt32(sLineNum);
                        }

                        oTargetLines.Add();
                    }

                    lRetCode = oTarDoc.Add();


                    if (lRetCode != 0)
                    {
                        sErrDesc = oDICompany.GetLastErrorDescription();

                        throw new ArgumentException(sErrDesc);

                    }
                    else
                    {
                        String DocEntry = oDICompany.GetNewObjectKey();
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("PQ DocEntry " + DocEntry, sFuncName);
                        String sSQL = "update DRF1 set [U_ApprovalLevel] =  null, [U_L1ApprovalStatus] = null, [U_L1Approver] = null,[U_L2ApprovalStatus] = null, [U_L2Approver] = null where DocEntry = '" + DocEntry + "'";
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Set NULL  " + sSQL, sFuncName);
                        oRecordSet.DoQuery(sSQL);

                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with SUCCESS ", sFuncName);
                        return "SUCCESS";
                    }

                }
                else
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Base Document Not Found ", sFuncName);
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with SUCCESS ", sFuncName);
                    return "SUCCESS";
                }
            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message;
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);
                return ex.Message.ToString();
            }
        }

        public DataTable MergeAutoNumberedToDataTable(DataTable SourceTable, string sErrDesc)
        {

            try
            {

                DataTable ResultTable = new DataTable();
                DataColumn AutoNumberColumn = new DataColumn();
                AutoNumberColumn.ColumnName = "SNo";
                AutoNumberColumn.DataType = typeof(int);
                AutoNumberColumn.AutoIncrement = true;
                AutoNumberColumn.AutoIncrementSeed = 1;
                AutoNumberColumn.AutoIncrementStep = 1;
                ResultTable.Columns.Add(AutoNumberColumn);
                ResultTable.Merge(SourceTable);
                ResultTable.Columns[0].SetOrdinal((ResultTable.Columns.Count - 1));

                return ResultTable;
            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message;
                return null;
            }

        }

        public string ReceiveInOutlet(DataSet oDataSet, SAPbobsCOM.Company oDICompany, string sConnString, string sErrDesc)
        {
            string sFuncName = string.Empty;
            DataTable oDTHeader = new DataTable();
            DataView oDataViewHeader = new DataView();
            DataView oDVDetail = new DataView();
            DataTable oDTDistinctHeader = new DataTable();
            DataTable oDTDistinctDocEntry = new DataTable();
            DataTable oDTDistBySupplier = new DataTable();
            string sSupplierCode = string.Empty;
            string sDocEntry = string.Empty;
            DataTable oDTDocentry = null;
            SAPbobsCOM.Recordset oRest = null;
            SAPbobsCOM.StockTransfer oInvTransReq = null;
            clsCopyMarketingDocument oCopyDocument = new clsCopyMarketingDocument();
            String sQuery = String.Empty;
            int lRetCode;

            try
            {
                sFuncName = "ReceiveInOutlet()";
                oDTDocentry = new DataTable();
                oDTDocentry.Columns.Add("DocEntry", typeof(String));
                oRest = (SAPbobsCOM.Recordset)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset));
                oInvTransReq = (SAPbobsCOM.StockTransfer)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest));
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);

                DataTable oDTL = new DataTable();
                oDTL = oDataSet.Tables[0];

                oDTL = MergeAutoNumberedToDataTable(oDTL, sErrDesc);
                oDataViewHeader = oDTL.DefaultView;
                //  oDataViewHeader.RowFilter = "SNo>=1 and SNo<=100";
                oDVDetail = oDataViewHeader;
                ////oDataViewHeader = oDataSet.Tables[0].DefaultView;
                ////oDVDetail = oDataSet.Tables[0].DefaultView;
                oDTDistinctHeader = oDataViewHeader.Table.DefaultView.ToTable(true, "SupplierCode");

                // Remove the Lines if supplier code is null
                oDTDistinctHeader.DefaultView.RowFilter = "SupplierCode IS NOT NULL";
                oDTDistinctHeader = oDTDistinctHeader.DefaultView.ToTable();

                //  if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting the SAP Transactions ", sFuncName);

                // if (!oDICompany.InTransaction) oDICompany.StartTransaction();

                for (int IntHeader = 0; IntHeader < oDTDistinctHeader.Rows.Count; IntHeader++)
                {
                    sSupplierCode = oDataViewHeader[IntHeader]["SupplierCode"].ToString();

                    if (sSupplierCode.ToUpper() == "VA-HARCT")
                    {
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling ConvertITReqToIT() Line Number : " + sDocEntry, sFuncName);

                        string sResult = ConvertITReqToIT(oDVDetail, oDICompany, sDocEntry, sSupplierCode, sConnString, ref  oDTDocentry, sErrDesc);
                        if (sResult != "SUCCESS")
                        { 
                            throw new ArgumentException(sResult); 
                        }
                    }
                    else
                    {
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling ConvertPOToGRPO() Line Number : " + sDocEntry, sFuncName);

                        string sResult = ConvertPOToGRPO(oDVDetail, oDICompany, sDocEntry, sSupplierCode, sConnString, sErrDesc);
                        if (sResult != "SUCCESS")
                        {
                            throw new ArgumentException(sResult); 
                        }
                    }
                }

                //   if (oDICompany.InTransaction) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                oDataViewHeader.RowFilter = null;
                //if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Committed the SAP Transations", sFuncName);
                oDTDocentry = oDTDocentry.DefaultView.ToTable(true, "DocEntry");
                foreach (DataRow odr in oDTDocentry.Rows)
                {
                    sQuery = "SELECT DocEntry FROM OWTQ T0 WITH (NOLOCK) WHERE DocEntry = '" + odr[0].ToString().Trim() + "' and  T0.[DocStatus] = 'O'";
                    oRest.DoQuery(sQuery);
                    if (oRest.RecordCount > 0)
                    {
                        if (oInvTransReq.GetByKey(Convert.ToInt32(odr[0])))
                        {
                            lRetCode = oInvTransReq.Close();
                            if (lRetCode != 0)
                            {
                                sErrDesc = oDICompany.GetLastErrorDescription();
                                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("DocEntry : " + odr[0] + " Exception " + sErrDesc, sFuncName);
                            }
                        }
                    }
                }

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS", sFuncName);
                return "SUCCESS";
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message;
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);
                return sErrDesc;
                //throw Ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oInvTransReq);
                oInvTransReq = null;
                oRest = null;
            }
        }

        public string CloseLineStatus(SAPbobsCOM.Company oDICompany, string sTypeofDocument
                                      , string sDocEntry, string sWhscode, string sErrDesc)
        {
            string sFuncName = string.Empty;
            Int32 iLineCount;
            double lRetCode;
            string sLineNum = string.Empty;
            clsCommon oUpdteStatus = new clsCommon();
            string sQueryString = string.Empty;
            //SAPbobsCOM.Recordset oRS;


            try
            {
                sFuncName = "CloseLineStatus()";
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);

                //if (sTypeofDocument.ToUpper() == "PO")
                //{
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("PO ", sFuncName);
                SAPbobsCOM.Documents oPO = (SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders));

                oPO.GetByKey(Convert.ToInt32(sDocEntry));

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("DocEntry : " + sDocEntry, sFuncName);

                iLineCount = oPO.Lines.Count;

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("PO Line Count  : " + iLineCount, sFuncName);

                for (int iCount = 0; iCount < iLineCount; iCount++)
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("PO Line : " + iCount, sFuncName);

                    oPO.Lines.SetCurrentLine(iCount);

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Quantity Checking : Remaining OpenQty : " + oPO.Lines.RemainingOpenQuantity + "  Reaining Inv Qty : " + oPO.Lines.RemainingOpenInventoryQuantity, sFuncName);

                    //if (oPO.Lines.RemainingOpenQuantity != oPO.Lines.Quantity)
                    if (oPO.Lines.LineStatus != SAPbobsCOM.BoStatus.bost_Open && oPO.Lines.WarehouseCode == sWhscode)
                        oPO.Lines.LineStatus = SAPbobsCOM.BoStatus.bost_Close;

                }

                lRetCode = oPO.Update();

                if (lRetCode != 0)
                {
                    sErrDesc = oDICompany.GetLastErrorDescription();
                    throw new ArgumentException(sErrDesc);
                }
                else
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with SUCCESS ", sFuncName);
                    return "SUCCESS";
                }

            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message;
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);
                //throw Ex;
                return sErrDesc;
            }
        }

        public string ReceiveIntoVAN(DataSet oDSData, SAPbobsCOM.Company oDICompany, string sConnString, string sErrDesc)
        {
            string sFuncName = string.Empty;
            DataTable oDTHeader = new DataTable();
            DataView oDataViewHeader = new DataView();
            DataView oDVDetail = new DataView();
            DataTable oDTDistinctHeader = new DataTable();
            DataTable oDTDocEntry = null;
            string sRequestNo = string.Empty;
            SAPbobsCOM.Recordset oRest = null;
            SAPbobsCOM.StockTransfer oInvTransReq = null;
            int lRetCode;
            String sQuery = String.Empty;
            clsPurchaseRequest oPurchaseRequest = new clsPurchaseRequest();


            try
            {
                sFuncName = "ReceiveIntoVAN()";
                oRest = (SAPbobsCOM.Recordset)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset));
                oInvTransReq = (SAPbobsCOM.StockTransfer)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest));
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);

                oDataViewHeader = oDSData.Tables[0].DefaultView;
                oDVDetail = oDSData.Tables[0].DefaultView;
                oDTDistinctHeader = oDataViewHeader.Table.DefaultView.ToTable(true, "SupplierCode");

                // Remove the Lines if supplier code is null
                oDTDistinctHeader.DefaultView.RowFilter = "SupplierCode IS NOT NULL";
                oDTDistinctHeader = oDTDistinctHeader.DefaultView.ToTable();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting the SAP Transactions ", sFuncName);
                oDTDocEntry = new DataTable();
                oDTDocEntry.Columns.Add("DocEntry", typeof(String));

                if (!oDICompany.InTransaction) oDICompany.StartTransaction();

                for (int IntHeader = 0; IntHeader < oDTDistinctHeader.Rows.Count; IntHeader++)
                {
                    sRequestNo = oDataViewHeader[IntHeader]["DocEntry"].ToString();

                    oDVDetail.RowFilter = "DocEntry = '" + sRequestNo + "'";

                    if (Convert_InventoryTransfer(oDVDetail, oDICompany, sConnString, ref oDTDocEntry, sErrDesc) != "SUCCESS") throw new ArgumentException(sErrDesc);

                    if (Update_SYS2BatchNumber(oDICompany, oDVDetail, sInvTransDocEntry, oDVDetail[0]["RequestNo"].ToString(), sConnString, sErrDesc) != "SUCCESS") throw new ArgumentException(sErrDesc);

                    // oPurchaseRequest.Create_InventoryTransferRequest_SYS2(oDICompany, sInvTransDocEntry, "", "", sConnString, sErrDesc);

                }

                if (oDICompany.InTransaction) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);

                foreach (DataRow odr in oDTDocEntry.Rows)
                {
                    //sQuery = "SELECT DocEntry FROM OWTQ T0 WHERE DocNum = '100002289' and  T0.[DocStatus] = 'O'";
                    sQuery = "SELECT DocEntry FROM OWTQ T0 WHERE DocEntry = '" + odr[0].ToString().Trim() + "' and  T0.[DocStatus] = 'O'";
                    oRest.DoQuery(sQuery);
                    if (oRest.RecordCount > 0)
                    {

                        if (oInvTransReq.GetByKey(Convert.ToInt32(oRest.Fields.Item("DocEntry").Value)))
                        {
                            lRetCode = oInvTransReq.Close();
                            if (lRetCode != 0)
                            {
                                sErrDesc = oDICompany.GetLastErrorDescription();
                                throw new ArgumentException(sErrDesc);
                            }
                        }
                    }
                }


                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS", sFuncName);
                //oDICompany.Disconnect();
                //oDICompany = null;

                return "SUCCESS";
            }
            catch (Exception Ex)
            {
                if (oDICompany.InTransaction) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                sErrDesc = Ex.Message;
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);

                //oDICompany.Disconnect();
                //oDICompany = null;
                throw Ex;
                //return Ex.Message.ToString();
            }
            finally
            {

                System.Runtime.InteropServices.Marshal.ReleaseComObject(oInvTransReq);
                oInvTransReq = null;
                oRest = null;
            }
        }

        public string Convert_InventoryTransfer(DataView oDataView, SAPbobsCOM.Company oDICompany, string sConnString, ref DataTable oDTDocentry, string sErrDesc)
        {

            long lRetCode;
            string sFuncName = string.Empty;
            string sDocEntry = string.Empty;
            clsCommon oDataAccess = new clsCommon();
            string sToBINLocCode = string.Empty;
            string sWhsCode = string.Empty;
            DateTime dtReceiptDate;
            double dReceiptQty;
            double dBalReceiptQty;
            DataSet sBatch = new DataSet();
            string sSeries = string.Empty;
            string sBatchItem = string.Empty;

            string sLineNum = string.Empty;
            double dBatchQty = 0;
            double dBalBatchQty = 0;
            string sQuery = string.Empty;
            double dNumInSale;


            SAPbobsCOM.StockTransfer oInventoryTransfer;
            SAPbobsCOM.StockTransfer oInvTransReq;
            SAPbobsCOM.Recordset oRest = null;

            oInventoryTransfer = (SAPbobsCOM.StockTransfer)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oStockTransfer));
            oInvTransReq = (SAPbobsCOM.StockTransfer)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest));
            oRest = (SAPbobsCOM.Recordset)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset));

            try
            {
                sFuncName = "Convert_InventoryTransfer()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);


                dtReceiptDate = DateTime.Parse(oDataView[0]["RequestDate"].ToString());
                sWhsCode = oDataView[0]["Outlet"].ToString();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling GetSingleValue() for Taking the Inventory DocEntry", sFuncName);

                sDocEntry = oDataAccess.GetSingleValue("SELECT DocEntry FROM OWTQ T0 with (nolock) INNER JOIN NNM1 T1 with (nolock) ON T0.[Series] = T1.[Series] " +
                    " and T1.[ObjectCode] ='1250000001'  WHERE T0.[DocNum] ='" + oDataView[0]["RequestNo"].ToString() + "' and T1.SeriesName ='SYS1'", sConnString, sErrDesc);

                oDTDocentry.Rows.Add(sDocEntry);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling GetSingleValue() for Taking the Batch Code", sFuncName);
                sToBINLocCode = oDataAccess.GetSingleValue("SELECT T0.[AbsEntry] FROM OBIN T0 with (nolock) WHERE T0.[BinCode] ='01CKT-" + sWhsCode + "'", sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling GetSingleValue() for Taking the Series Code", sFuncName);
                sSeries = oDataAccess.GetSingleValue("select Series from NNM1 with (nolock) where ObjectCode ='67' and SeriesName ='SYS1'", sConnString, sErrDesc);


                // sDocEntry = "5";
                oInvTransReq.GetByKey(Convert.ToInt32(sDocEntry));

                oInventoryTransfer.CardCode = oInvTransReq.CardCode;
                oInventoryTransfer.FromWarehouse = oInvTransReq.FromWarehouse;
                oInventoryTransfer.ToWarehouse = oInvTransReq.ToWarehouse;
                oInventoryTransfer.DocDate = dtReceiptDate;
                oInventoryTransfer.DocDate = dtReceiptDate;
                oInventoryTransfer.TaxDate = dtReceiptDate;
                oInventoryTransfer.Series = Convert.ToInt32(sSeries);

                for (int iRow = 0; iRow <= oDataView.Count - 1; iRow++)
                {
                    if (string.IsNullOrEmpty(oDataView[iRow]["Receipt Qty"].ToString())) continue;
                    if (Convert.ToDouble(oDataView[iRow]["Receipt Qty"].ToString()) == 0) continue;

                    sQuery = "SELECT ISNULL(NumInSale,1) FROM OITM WITH(NOLOCK) WHERE ITEMCODE='" + oDataView[iRow]["ItemCode"].ToString().Trim() + "'";

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling GetSingleValue() for Taking the NumInSale Value", sFuncName);
                    dNumInSale = Convert.ToDouble(oDataAccess.GetSingleValue(sQuery, sConnString, sErrDesc));

                    dReceiptQty = (Convert.ToDouble(oDataView[iRow]["Receipt Qty"].ToString().Trim()) * dNumInSale);

                    sLineNum = oDataAccess.GetSingleValue("select LineNum from WTQ1 with (nolock) where DocEntry ='" + sDocEntry + "' and ItemCode ='" + oDataView[iRow]["ItemCode"].ToString() + "'", sConnString, sErrDesc);

                    // dReceiptQty = Convert.ToDouble(oDataView[iRow]["Receipt Qty"].ToString());

                    ////if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Line Num " + iRow, sFuncName);
                    ////if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Item Code " + oDataView[iRow]["ItemCode"].ToString().Trim(), sFuncName);
                    ////if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Line Num " + sLineNum, sFuncName);
                    ////if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("DocEntry " + sDocEntry, sFuncName);

                    oInventoryTransfer.Lines.BaseType = SAPbobsCOM.InvBaseDocTypeEnum.InventoryTransferRequest;
                    oInventoryTransfer.Lines.BaseEntry = Convert.ToInt32(sDocEntry);
                    oInventoryTransfer.Lines.BaseLine = Convert.ToInt32(sLineNum);//Convert.ToInt32(oDataView[iRow]["LineNum"].ToString());
                    oInventoryTransfer.Lines.Quantity = dReceiptQty;
                    dBalReceiptQty = dReceiptQty;
                    dBalBatchQty = 0;

                    sBatchItem = oDataAccess.GetSingleValue("select ManBtchNum  from OITM with (nolock) where ItemCode ='" + oDataView[iRow]["ItemCode"].ToString() + "'", sConnString, sErrDesc);

                    if (sBatchItem == "Y")
                    {

                        sBatch = oDataAccess.Get_SingleValue("SELECT BatchNum,Quantity FROM IBT1 T0 with (nolock) where Quantity>0 and BaseType ='1250000001' and BaseEntry ='" + sDocEntry + "' and BaseLinNum ='" + sLineNum + "' and CardCode ='VA-HARCT' order by Quantity desc", sConnString, sErrDesc);

                        for (int iBatchRowCount = 0; iBatchRowCount < sBatch.Tables[0].Rows.Count; iBatchRowCount++)
                        {
                            if (dBalBatchQty >= dReceiptQty && iBatchRowCount > 0) break;

                            oInventoryTransfer.Lines.BatchNumbers.BatchNumber = sBatch.Tables[0].Rows[iBatchRowCount]["BatchNum"].ToString();
                            oInventoryTransfer.Lines.BatchNumbers.AddmisionDate = Convert.ToDateTime(DateTime.Today).Date;

                            dBatchQty = Convert.ToDouble(sBatch.Tables[0].Rows[iBatchRowCount]["Quantity"].ToString());

                            if (dBalReceiptQty > dBatchQty)
                            {
                                oInventoryTransfer.Lines.BatchNumbers.Quantity = dBatchQty;//Convert.ToDouble(sBatch.Tables[0].Rows[iBatchRowCount]["Quantity"].ToString());
                            }
                            else
                            {
                                oInventoryTransfer.Lines.BatchNumbers.Quantity = dBalReceiptQty;//Convert.ToDouble(sBatch.Tables[0].Rows[iBatchRowCount]["Quantity"].ToString());
                            }

                            oInventoryTransfer.Lines.BatchNumbers.Add();// Add the Batch Items

                            //Allocate the BIN Locations for the ItemCodes

                            oInventoryTransfer.Lines.BinAllocations.BinActionType = SAPbobsCOM.BinActionTypeEnum.batFromWarehouse;
                            oInventoryTransfer.Lines.BinAllocations.BinAbsEntry = 1;

                            if (dBalReceiptQty > dBatchQty)
                            {
                                oInventoryTransfer.Lines.BinAllocations.Quantity = dBatchQty;
                            }
                            else
                            {
                                oInventoryTransfer.Lines.BinAllocations.Quantity = dBalReceiptQty;
                            }

                            oInventoryTransfer.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = iBatchRowCount;
                            oInventoryTransfer.Lines.BinAllocations.AllowNegativeQuantity = SAPbobsCOM.BoYesNoEnum.tYES;
                            oInventoryTransfer.Lines.BinAllocations.Add();

                            //To BIN Location
                            oInventoryTransfer.Lines.BinAllocations.BinActionType = SAPbobsCOM.BinActionTypeEnum.batToWarehouse;
                            oInventoryTransfer.Lines.BinAllocations.BinAbsEntry = Convert.ToInt32(sToBINLocCode);

                            if (dBalReceiptQty > dBatchQty)
                            {
                                oInventoryTransfer.Lines.BinAllocations.Quantity = dBatchQty;
                            }
                            else
                            {
                                oInventoryTransfer.Lines.BinAllocations.Quantity = dBalReceiptQty;
                            }

                            oInventoryTransfer.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = iBatchRowCount;
                            oInventoryTransfer.Lines.BinAllocations.AllowNegativeQuantity = SAPbobsCOM.BoYesNoEnum.tYES;
                            oInventoryTransfer.Lines.BinAllocations.Add();

                            dBalBatchQty += dBatchQty;
                            dBalReceiptQty = dReceiptQty - dBatchQty;
                        }



                        //for (int iBatchRowCount = 0; iBatchRowCount < sBatch.Tables[0].Rows.Count; iBatchRowCount++)
                        //{
                        //    if (dBalBatchQty >= dReceiptQty && iBatchRowCount > 0) break;

                        //    oInventoryTransfer.Lines.BatchNumbers.BatchNumber = sBatch.Tables[0].Rows[iBatchRowCount]["BatchNum"].ToString();
                        //    oInventoryTransfer.Lines.BatchNumbers.AddmisionDate = Convert.ToDateTime(DateTime.Today).Date;

                        //    dBatchQty = Convert.ToDouble(sBatch.Tables[0].Rows[iBatchRowCount]["Quantity"].ToString());

                        //    if (dBalReceiptQty > dBatchQty)
                        //    {
                        //        oInventoryTransfer.Lines.BatchNumbers.Quantity = dBatchQty;//Convert.ToDouble(sBatch.Tables[0].Rows[iBatchRowCount]["Quantity"].ToString());
                        //    }
                        //    else
                        //    {

                        //        oInventoryTransfer.Lines.BatchNumbers.Quantity = dBalReceiptQty;//Convert.ToDouble(sBatch.Tables[0].Rows[iBatchRowCount]["Quantity"].ToString());
                        //    }

                        //    oInventoryTransfer.Lines.BatchNumbers.Add();// Add the Batch Items



                        //    //Allocate the BIN Locations for the ItemCodes

                        //    //From BIN Location
                        //    //if (iBatchRowCount == 0)
                        //    //{
                        //    oInventoryTransfer.Lines.BinAllocations.BinActionType = SAPbobsCOM.BinActionTypeEnum.batFromWarehouse;
                        //    oInventoryTransfer.Lines.BinAllocations.BinAbsEntry = 1;

                        //    if (dBalReceiptQty > dBatchQty)
                        //    {
                        //        oInventoryTransfer.Lines.BinAllocations.Quantity = dBatchQty;
                        //    }
                        //    else
                        //    {
                        //        oInventoryTransfer.Lines.BinAllocations.Quantity = dBalReceiptQty;
                        //    }

                        //    oInventoryTransfer.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = iBatchRowCount;

                        //    oInventoryTransfer.Lines.BinAllocations.Add();
                        //    //  }
                        //    //To BIN Location 
                        //    oInventoryTransfer.Lines.BinAllocations.BinActionType = SAPbobsCOM.BinActionTypeEnum.batToWarehouse;
                        //    oInventoryTransfer.Lines.BinAllocations.BinAbsEntry = Convert.ToInt32(sToBINLocCode);

                        //    if (dBalReceiptQty > dBatchQty)
                        //    {
                        //        oInventoryTransfer.Lines.BinAllocations.Quantity = dBatchQty;
                        //    }
                        //    else
                        //    {
                        //        oInventoryTransfer.Lines.BinAllocations.Quantity = dBalReceiptQty;
                        //    }

                        //    oInventoryTransfer.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = iBatchRowCount;

                        //    oInventoryTransfer.Lines.BinAllocations.Add();

                        //    dBalBatchQty += dBatchQty;

                        //    dBalReceiptQty = dReceiptQty - dBatchQty;
                        //}

                    }
                    else
                    {
                        //oInventoryTransfer.Lines.SetCurrentLine(0);
                        oInventoryTransfer.Lines.BinAllocations.BinActionType = SAPbobsCOM.BinActionTypeEnum.batFromWarehouse;
                        oInventoryTransfer.Lines.BinAllocations.BinAbsEntry = 1;
                        oInventoryTransfer.Lines.BinAllocations.Quantity = dReceiptQty;
                        oInventoryTransfer.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = 0;
                        oInventoryTransfer.Lines.BinAllocations.AllowNegativeQuantity = SAPbobsCOM.BoYesNoEnum.tYES;

                        oInventoryTransfer.Lines.BinAllocations.Add();

                        //oInventoryTransfer.Lines.SetCurrentLine(1);
                        oInventoryTransfer.Lines.BinAllocations.BinActionType = SAPbobsCOM.BinActionTypeEnum.batToWarehouse;
                        oInventoryTransfer.Lines.BinAllocations.BinAbsEntry = Convert.ToInt32(sToBINLocCode);
                        oInventoryTransfer.Lines.BinAllocations.Quantity = dReceiptQty;
                        oInventoryTransfer.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = 0;
                        oInventoryTransfer.Lines.BinAllocations.AllowNegativeQuantity = SAPbobsCOM.BoYesNoEnum.tYES;

                        oInventoryTransfer.Lines.BinAllocations.Add();
                    }


                    oInventoryTransfer.Lines.Add(); //Add the Line Items
                }

                // oInventoryTransfer.SaveXML("F:\\SVN WorkArea\\Harrys\\Sources\\Harrys Web Application\\InventoryTransfer1.xml");


                lRetCode = oInventoryTransfer.Add(); //Add the Header 


                if (lRetCode != 0)
                {

                    sErrDesc = oDICompany.GetLastErrorDescription();
                    throw new ArgumentException(sErrDesc);

                }
                else
                {
                    oDICompany.GetNewObjectCode(out sInvTransDocEntry);
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with SUCCESS ", sFuncName);
                    //string SQL = "Select top(1) from OWTQ where \"DocEntry\" = '" + sDocEntry + "' and \"DocStatus\" = 'O'";
                    //oRest.DoQuery(SQL);


                    return "SUCCESS";
                }

                // oDICompany.GetNewObjectCode(out sInvTransDocEntry);

                // add script here: select 1 from owtq where docnum = inventory transfer request number and docu status = 'O'
                // if can find then lretcode = oinvtransreq.close();
                // before let me show you where I did 




            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message;
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);
                throw Ex;
            }

            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oInvTransReq);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oInventoryTransfer);
            }
        }

        public string ConvertITReqToIT(DataView oDataView, SAPbobsCOM.Company oDICompany, string sDcoEntry, string sSupplierCode
                                       , string sConnString, ref DataTable oDTDocentry, string sErrDesc)
        {

            long lRetCode;
            string sFuncName = string.Empty;
            clsCommon oDataAccess = new clsCommon();
            string sManBatchNumber = string.Empty;
            DataSet sBatch = new DataSet();

            string sDocEntry = string.Empty;
            Int32 iLineNum;
            Int32 iBinSerial;
            bool bContentExist = false;
            string sWhsCode = string.Empty;
            string sToBINLocCode = string.Empty;
            double dBalBinQty = 0;

            double dBatchQty = 0;
            double dBalBatchQty = 0;
            double dReceiptQty;
            double dBalReceiptQty;
            string sOutletReceivedBy;
            DateTime dtReceiptDate;
            double dNumInSale;
            string sQuery = string.Empty;
            string sSeries = string.Empty;
            String sItemCode = String.Empty;
            string sVisorder = string.Empty;
            String sInvenReqDocentry = String.Empty;


            try
            {
                sFuncName = "ConvertITReqToIT()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);


                SAPbobsCOM.StockTransfer oInventoryTransfer;
                SAPbobsCOM.StockTransfer oInvTransReq;
                oInventoryTransfer = (SAPbobsCOM.StockTransfer)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oStockTransfer));
                //  oInventoryTransfer = (SAPbobsCOM.StockTransfer)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oStockTransferDraft));
                oInvTransReq = (SAPbobsCOM.StockTransfer)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest));


                // oInventoryTransfer.DocObjectCode = SAPbobsCOM.BoObjectTypes.oStockTransfer;

                //oInventoryTransfer.GetByKey(2444);
                //oInventoryTransfer.SaveXML("C:\\inetpub\\wwwroot\\UAT\\HarrysServices\\bin\\InvTrans_2444.xml");


                sDocEntry = oDataView[0]["DocEntry"].ToString().Trim();
                sWhsCode = oDataView[0]["Outlet"].ToString().Trim();

                sOutletReceivedBy = Convert.ToString(oDataView[0]["OutletReceivedBy"].ToString().Trim().ToUpper());

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling GetSingleValue() for Taking the Batch Code", sFuncName);
                sToBINLocCode = oDataAccess.GetSingleValue("SELECT T0.[AbsEntry] FROM OBIN T0 with (nolock) WHERE T0.[BinCode] ='01CKT-" + sWhsCode + "'", sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling GetSingleValue() for Taking the Series Code", sFuncName);
                sSeries = oDataAccess.GetSingleValue("select Series from NNM1 with (nolock) where ObjectCode ='67' and SeriesName ='SYS2'", sConnString, sErrDesc);

                dtReceiptDate = Convert.ToDateTime(oDataView[0]["RequestDate"].ToString().Trim());

                oInventoryTransfer.CardCode = sSupplierCode;
                oInventoryTransfer.DocDate = dtReceiptDate;
                oInventoryTransfer.Series = Convert.ToInt32(sSeries);

                if (oInvTransReq.GetByKey(Convert.ToInt32(sDocEntry)) == true)
                {
                    oInventoryTransfer.FromWarehouse = oInvTransReq.FromWarehouse;
                    oInventoryTransfer.ToWarehouse = oInvTransReq.ToWarehouse;
                }

                iBinSerial = 0;
                for (int iRow = 0; iRow <= oDataView.Count - 1; iRow++)
                {
                    if (string.IsNullOrEmpty(oDataView[iRow]["Receipt Qty"].ToString())) continue;
                    if (Convert.ToDouble(oDataView[iRow]["Receipt Qty"].ToString()) <= 0) continue;

                    bContentExist = true;

                    sQuery = "SELECT ISNULL(NumInSale,1) FROM OITM WITH (NOLOCK) WHERE ITEMCODE='" + oDataView[iRow]["ItemCode"].ToString().Trim() + "'";

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling GetSingleValue() for Taking the NumInSale Value", sFuncName);
                    dNumInSale = Convert.ToDouble(oDataAccess.GetSingleValue(sQuery, sConnString, sErrDesc));

                    dReceiptQty = (Convert.ToDouble(oDataView[iRow]["Receipt Qty"].ToString().Trim()) * dNumInSale);

                    sDocEntry = oDataView[iRow]["DocEntry"].ToString().Trim();
                    iLineNum = Convert.ToInt32(oDataView[iRow]["LineNum"].ToString().Trim());
                    sItemCode = oDataView[iRow]["ItemCode"].ToString().Trim();

                    sVisorder = oDataAccess.GetSingleValue("SELECT T0.[VisOrder] FROM WTQ1 T0 WHERE T0.[DocEntry]  = '" + sDocEntry + "' and  T0.[LineNum]  = " + iLineNum + "", sConnString, sErrDesc);

                    if (oInvTransReq.GetByKey(Convert.ToInt32(sDocEntry)) == true)
                    {

                        oDTDocentry.Rows.Add(sDocEntry);

                        oInvTransReq.Lines.SetCurrentLine(Convert.ToInt32(sVisorder));

                        dBalReceiptQty = dReceiptQty;
                        dBalBinQty = dReceiptQty;

                        oInventoryTransfer.Lines.BaseType = SAPbobsCOM.InvBaseDocTypeEnum.InventoryTransferRequest;
                        oInventoryTransfer.Lines.BaseEntry = Convert.ToInt32(sDocEntry);
                        oInventoryTransfer.Lines.BaseLine = iLineNum;
                        oInventoryTransfer.Lines.InventoryQuantity = dReceiptQty;
                        oInventoryTransfer.Lines.ItemCode = sItemCode; // oInvTransReq.Lines.ItemCode;

                        //if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Sno " + oDataView[iRow]["SNo"].ToString().Trim(), sFuncName);
                        //if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Item Code " + oDataView[iRow]["ItemCode"].ToString().Trim(), sFuncName);
                        //if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Line No. " + iLineNum, sFuncName);

                        // String sit = oInventoryTransfer.Lines.ItemCode;

                        oInventoryTransfer.Lines.UserFields.Fields.Item("U_ReasonCode").Value = string.Format(oDataView[iRow]["Reason Code"].ToString());
                        oInventoryTransfer.Lines.UserFields.Fields.Item("U_OutletReceivedBy").Value = sOutletReceivedBy;

                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling GetSingleValue() for Checking Batch Item", sFuncName);

                        sManBatchNumber = oDataAccess.GetSingleValue("select ManBtchNum  from OITM with(nolock) where ItemCode='" + oDataView[iRow]["ItemCode"].ToString().Trim() + "'", sConnString, sErrDesc);

                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Batch : " + sManBatchNumber + "ItemCode :" + oDataView[iRow]["ItemCode"].ToString().Trim(), sFuncName);

                        if (sManBatchNumber.ToString().ToUpper() == "Y")
                        {

                            if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling GetSingleValue() for Taking the BatchCode and Quantity ", sFuncName);

                            sBatch = oDataAccess.Get_SingleValue("SELECT BatchNum,Quantity FROM IBT1 T0 with (nolock) where Quantity >0 and BaseType ='1250000001' and BaseEntry ='" + sDocEntry + "' and BaseLinNum ='" + iLineNum + "' and CardCode ='VA-HARCT'", sConnString, sErrDesc);

                            if (sBatch.Tables[0].Rows.Count == 0)
                            {
                                sErrDesc = "No batches found for this item " + oDataView[iRow]["ItemCode"].ToString().Trim() + " in SAP.";
                                throw new ArgumentException(sErrDesc);

                            }

                            for (int iBatchRowCount = 0; iBatchRowCount < sBatch.Tables[0].Rows.Count; iBatchRowCount++)
                            {
                                //Add the Batch Details:

                                if (dBalBatchQty >= dReceiptQty && iBatchRowCount > 0) break;

                                oInventoryTransfer.Lines.BatchNumbers.BatchNumber = sBatch.Tables[0].Rows[iBatchRowCount]["BatchNum"].ToString();
                                oInventoryTransfer.Lines.BatchNumbers.AddmisionDate = Convert.ToDateTime(DateTime.Today).Date;

                                dBatchQty = Convert.ToDouble(sBatch.Tables[0].Rows[iBatchRowCount]["Quantity"].ToString());

                                if (dBalReceiptQty > dBatchQty)
                                {
                                    oInventoryTransfer.Lines.BatchNumbers.Quantity = dBatchQty;//Convert.ToDouble(sBatch.Tables[0].Rows[iBatchRowCount]["Quantity"].ToString());
                                }
                                else
                                {
                                    oInventoryTransfer.Lines.BatchNumbers.Quantity = dBalReceiptQty;//Convert.ToDouble(sBatch.Tables[0].Rows[iBatchRowCount]["Quantity"].ToString());
                                }

                                oInventoryTransfer.Lines.BatchNumbers.Add();

                                //Add the BIN Locations:

                                oInventoryTransfer.Lines.BinAllocations.BinActionType = SAPbobsCOM.BinActionTypeEnum.batFromWarehouse;
                                oInventoryTransfer.Lines.BinAllocations.BinAbsEntry = Convert.ToInt32(sToBINLocCode); //Convert.ToInt32(1); 

                                if (dBalReceiptQty > dBatchQty)
                                {
                                    oInventoryTransfer.Lines.BinAllocations.Quantity = dBatchQty;
                                }
                                else
                                {
                                    oInventoryTransfer.Lines.BinAllocations.Quantity = dBalReceiptQty;
                                }

                                // oInventoryTransfer.Lines.BinAllocations.Quantity = dBalBinQty;
                                oInventoryTransfer.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = iBatchRowCount;
                                oInventoryTransfer.Lines.BinAllocations.AllowNegativeQuantity = SAPbobsCOM.BoYesNoEnum.tYES;
                                oInventoryTransfer.Lines.BinAllocations.Add();
                                dBalBatchQty += dBatchQty;
                                dBalReceiptQty = dReceiptQty - dBatchQty;


                            }
                        }
                        else
                        {
                            oInventoryTransfer.Lines.BinAllocations.BinActionType = SAPbobsCOM.BinActionTypeEnum.batFromWarehouse;
                            oInventoryTransfer.Lines.BinAllocations.BinAbsEntry = Convert.ToInt32(sToBINLocCode); //Convert.ToInt32(1);
                            oInventoryTransfer.Lines.BinAllocations.Quantity = dReceiptQty;
                            oInventoryTransfer.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = 0;
                            oInventoryTransfer.Lines.BinAllocations.AllowNegativeQuantity = SAPbobsCOM.BoYesNoEnum.tYES;
                            oInventoryTransfer.Lines.BinAllocations.Add();
                        }

                        oInventoryTransfer.Lines.Add();
                        iBinSerial++;
                    }
                }
                //   sInvenReqDocentry = Left(sInvenReqDocentry, sInvenReqDocentry.Length - 1); //sInvenReqDocentry.Substring( 0 , sInvenReqDocentry.Length -1);

                if (bContentExist == true)
                {
                    // oInventoryTransfer.SaveXML("E:\\xml\\InvTrans.xml");
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Converting inventory transfer ", sFuncName);
                    lRetCode = oInventoryTransfer.Add();


                    if (lRetCode != 0)
                    {

                        sErrDesc = oDICompany.GetLastErrorDescription();
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Converting error :  " + sErrDesc, sFuncName);
                        throw new ArgumentException(sErrDesc);

                    }
                    else
                    {
                        // sInvenReqDocentry = sInvenReqDocentry.Substring( 0 , sInvenReqDocentry.Length -1);
                        //  sInvenReqDocentry = Left(sInvenReqDocentry, sInvenReqDocentry.Length - 1);



                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with SUCCESS ", sFuncName);
                        return "SUCCESS";
                    }
                }

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with SUCCESS ", sFuncName);
                sBatch.Dispose();
                return "SUCCESS";

            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message;
                sBatch.Dispose();
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);
                //return Ex.Message.ToString();
                //throw Ex;
                return sErrDesc;
            }
        }

        public string Left(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            maxLength = Math.Abs(maxLength);

            return (value.Length <= maxLength
                   ? value
                   : value.Substring(0, maxLength)
                   );
        }

        public string ConvertPOToGRPO(DataView oDataView, SAPbobsCOM.Company oDICompany, string sDcoEntry, string sSupplierCode, string sConnString
                                        , string sErrDesc)
        {

            long lRetCode;
            string sFuncName = string.Empty;
            clsCommon oDataAccess = new clsCommon();
            string sManBatchNumber = string.Empty;
            string sWhsCode = string.Empty;
            string sItemCode = string.Empty;
            bool bContentExist = false;
            string sDocEntry = string.Empty;
            DataTable oDTDistinctDocEntry = new DataTable();

            double dNumInBuy = 0;
            double dBatchQty = 0;
            double dReceiptQty = 0;
            string sOutletReceivedBy = string.Empty;
            DateTime dtReceiptDate;

            try
            {
                sFuncName = "ConvertPOToGRPO()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);

                SAPbobsCOM.Documents oGRPO;
                //SAPbobsCOM.Documents oPurchaseOrder;

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("After object initialisation of SAP Documents", sFuncName);
                //oPurchaseOrder = (SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders));
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Checking Before the Content type/html issue ", sFuncName);

                oGRPO = (SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseDeliveryNotes));

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Checking After the Content type/html issue ", sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("After initialisation of Purchase delivery Note object ", sFuncName);
                //sErrDesc = "This is for testing the TYPE/HTML issue";
                //throw new ArgumentException(sErrDesc);
                //if (oPurchaseOrder.GetByKey(Convert.ToInt32(oDataView[0]["DocEntry"].ToString())) == true)

                sOutletReceivedBy = Convert.ToString(oDataView[0]["OutletReceivedBy"].ToString().Trim().ToUpper());
                dtReceiptDate = Convert.ToDateTime(oDataView[0]["ReceiptDate"].ToString().Trim());


                oGRPO.CardCode = sSupplierCode;
                oGRPO.DocDate = dtReceiptDate;

                for (int iRow = 0; iRow <= oDataView.Count - 1; iRow++)
                {

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug(oDataView[iRow]["Receipt Qty"].ToString(), sFuncName);

                    if (string.IsNullOrEmpty(oDataView[iRow]["Receipt Qty"].ToString())) continue;
                    if (Convert.ToDouble(oDataView[iRow]["Receipt Qty"].ToString()) == 0) continue;

         

                    bContentExist = true;

                    sWhsCode = oDataView[iRow]["Outlet"].ToString().Trim();
                    sItemCode = oDataView[iRow]["ItemCode"].ToString().Trim();
                    sDocEntry = oDataView[iRow]["DocEntry"].ToString().Trim();
                    dReceiptQty = Convert.ToDouble(oDataView[iRow]["Receipt Qty"].ToString().Trim());

                    dNumInBuy = Convert.ToDouble(oDataAccess.GetSingleValue("select isnull(NumInBuy,1)  from OITM WITH (NOLOCK) where ItemCode='" + sItemCode + "'", sConnString, sErrDesc));
                    
                    dBatchQty = (dReceiptQty * dNumInBuy);

                    oGRPO.Lines.ItemCode = sItemCode;
                    oGRPO.Lines.WarehouseCode = sWhsCode;

                    if (sDocEntry.ToString() != "0")
                    {
                        oGRPO.Lines.BaseType = 22;
                        oGRPO.Lines.BaseEntry = Convert.ToInt32(sDocEntry);
                        oGRPO.Lines.BaseLine = Convert.ToInt32(oDataView[iRow]["LineNum"].ToString().Trim());
                    }

                    oGRPO.Lines.InventoryQuantity = dBatchQty; //dReceiptQty;
                    oGRPO.Lines.UserFields.Fields.Item("U_ReasonCode").Value = string.Format(oDataView[iRow]["Reason Code"].ToString().Trim());
                    oGRPO.Lines.UserFields.Fields.Item("U_OutletReceivedBy").Value = sOutletReceivedBy;

                    string sCostCenter = oDataAccess.GetSingleValue("select U_CostCenter [CostCenter] from OWHS with (NOLOCK) where WhsCode ='" + sWhsCode + "'", sConnString, sErrDesc);

                    oGRPO.Lines.COGSCostingCode = sCostCenter;
                    oGRPO.Lines.CostingCode = sCostCenter;
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("After getting & assigning the costing code from warehouse master : " + sWhsCode, sFuncName);

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling GetSingleValue() for Checking Batch Item", sFuncName);

                    sManBatchNumber = oDataAccess.GetSingleValue("select ManBtchNum  from OITM with(nolock) where ItemCode='" + sItemCode + "'", sConnString, sErrDesc);

                    if (sManBatchNumber.ToString().ToUpper() == "Y")
                    {

                        oGRPO.Lines.BatchNumbers.BatchNumber = sWhsCode;
                        oGRPO.Lines.BatchNumbers.Quantity = dBatchQty;//Convert.ToDouble(oDataView[iRow]["Receipt Qty"].ToString());
                        oGRPO.Lines.BatchNumbers.AddmisionDate = Convert.ToDateTime(DateTime.Today);
                        oGRPO.Lines.BatchNumbers.Add();
                    }

                    oGRPO.Lines.Add();
                }

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Before Adding GRPO ", sFuncName);

                if (bContentExist == true)
                {

                    //  if (oDICompany.InTransaction == false) oDICompany.StartTransaction();

                    lRetCode = oGRPO.Add();

                    if (lRetCode != 0)
                    {
                        sErrDesc = oDICompany.GetLastErrorDescription();
                        throw new ArgumentException(sErrDesc);

                    }
                    else
                    {
                        //if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with SUCCESS ", sFuncName);

                        oDTDistinctDocEntry = oDataView.Table.DefaultView.ToTable(true, "DocEntry");

                        if (oDataView[0]["CloseStatus"].ToString().ToUpper() == "Y")
                        {
                            for (int iDocCount = 0; iDocCount < oDTDistinctDocEntry.Rows.Count; iDocCount++)
                            {
                                sDocEntry = oDTDistinctDocEntry.Rows[iDocCount][0].ToString();

                                if (sDocEntry.ToString() == "0") continue;

                                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling CloseLineStatus() for closing the PO. Ref Number : " + sDocEntry, sFuncName);

                                if (CloseLineStatus(oDICompany, "PO", sDocEntry, sWhsCode, sErrDesc) != "SUCCESS") throw new ArgumentException(sErrDesc);

                            }
                        }

                        //return "SUCCESS";
                    }

                }

                // if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with SUCCESS ", sFuncName);
                oDTDistinctDocEntry.Dispose();
                return "SUCCESS";

            }

            catch (Exception Ex)
            {
                sErrDesc = Ex.Message;
                oDTDistinctDocEntry.Dispose();
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);
                // if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                return sErrDesc;
                //throw Ex;
            }
        }

        public string Update_PRQuantity(DataView oDataView, SAPbobsCOM.Company oDICompany
                            , string sConnstring, string sErrDesc)
        {
            string sFuncName = string.Empty;
            SAPbobsCOM.Documents oPurchaseOrder; //= new SAPbobsCOM.Documents();
            long lRetCode;


            try
            {
                sFuncName = "Update_PRQuantity()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);

                oPurchaseOrder = (SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders));



                if (oPurchaseOrder.GetByKey(Convert.ToInt32(oDataView[0]["DocEntry"].ToString())) == true)

                    for (int iRow = 0; iRow <= oDataView.Count - 1; iRow++)
                    {
                        if (string.IsNullOrEmpty(oDataView[iRow]["Receipt Qty"].ToString())) continue;

                        oPurchaseOrder.Lines.SetCurrentLine(iRow);

                        if (oPurchaseOrder.Lines.LineStatus == SAPbobsCOM.BoStatus.bost_Close) continue;

                        if (oPurchaseOrder.Lines.Quantity < Convert.ToDouble(oDataView[iRow]["Receipt Qty"].ToString()))
                            oPurchaseOrder.Lines.Quantity = Convert.ToDouble(oDataView[iRow]["Receipt Qty"]);

                        oPurchaseOrder.Lines.UserFields.Fields.Item("U_ReasonCode").Value = oDataView[iRow]["Reason Code"].ToString();
                    }

                lRetCode = oPurchaseOrder.Update();


                if (lRetCode != 0)
                {

                    sErrDesc = oDICompany.GetLastErrorDescription();
                    throw new ArgumentException(sErrDesc);
                }
                else
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with SUCCESS", sFuncName);

                    return "SUCCESS";
                }

                //if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS", sFuncName);

                //return "SUCCESS";

            }

            catch (Exception Ex)
            {
                if (oDICompany.InTransaction) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                sErrDesc = Ex.Message;
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);

                //return Ex.Message.ToString();
                throw Ex;
            }
        }

        public string Update_ITRQuantity(DataView oDataView, SAPbobsCOM.Company oDICompany
                                   , string sConnstring, string sErrDesc)
        {
            string sFuncName = string.Empty;
            SAPbobsCOM.StockTransfer oTransferReq; //= new SAPbobsCOM.StockTransfer();
            long lRetCode;


            try
            {
                sFuncName = "Update_ITRQuantity()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);

                oTransferReq = (SAPbobsCOM.StockTransfer)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest));

                if (oTransferReq.GetByKey(Convert.ToInt32(oDataView[0]["DocEntry"].ToString())) == true)

                    for (int iRow = 0; iRow <= oDataView.Count - 1; iRow++)
                    {
                        if (string.IsNullOrEmpty(oDataView[iRow]["Receipt Qty"].ToString())) continue;
                        oTransferReq.Lines.SetCurrentLine(Convert.ToInt32(oDataView[iRow]["LineNum"].ToString()));

                        if (oTransferReq.Lines.Quantity < Convert.ToDouble(oDataView[iRow]["Receipt Qty"].ToString()))
                            oTransferReq.Lines.Quantity = Convert.ToDouble(oDataView[iRow]["Receipt Qty"].ToString());

                        oTransferReq.Lines.UserFields.Fields.Item("U_ReasonCode").Value = oDataView[iRow]["Reason Code"].ToString();
                    }

                lRetCode = oTransferReq.Update();


                if (lRetCode != 0)
                {

                    sErrDesc = oDICompany.GetLastErrorDescription();
                    throw new ArgumentException(sErrDesc);
                }
                else
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with SUCCESS", sFuncName);
                    return "SUCCESS";
                }


                //if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS", sFuncName);

                //return "SUCCESS";

            }

            catch (Exception Ex)
            {
                if (oDICompany.InTransaction) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                sErrDesc = Ex.Message;
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);

                //return Ex.Message.ToString();
                throw Ex;
            }
        }

        public string CopyMarketingDocument(SAPbobsCOM.BoObjectTypes oBaseType, int oBaseEntry, SAPbobsCOM.BoObjectTypes oTarType,
                                 SAPbobsCOM.Company oDICompany, bool IsDraft, string sErrDesc)
        {

            double lRetCode = 0;
            string sFuncName = string.Empty;

            try
            {
                sFuncName = "CopyMarketingDocument()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);


                SAPbobsCOM.Documents oBaseDoc = ((SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(oBaseType)));
                if (oBaseDoc.GetByKey(oBaseEntry))
                {
                    //base document found, copy to target doc
                    SAPbobsCOM.Documents oTarDoc = ((SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(oTarType)));

                    if (IsDraft == false)
                    {
                        oTarDoc = ((SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(oTarType)));
                    }
                    else
                    {
                        oTarDoc = ((SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts)));
                        oTarDoc.DocObjectCode = oTarType;
                    }


                    //todo: copy the cardcode, docduedate, lines
                    oTarDoc.CardCode = oBaseDoc.CardCode;
                    oTarDoc.DocDueDate = oBaseDoc.DocDueDate;
                    oTarDoc.DocDate = oBaseDoc.DocDate;
                    oTarDoc.TaxDate = oBaseDoc.TaxDate;


                    //copy the lines
                    int count = oBaseDoc.Lines.Count - 1;
                    SAPbobsCOM.Document_Lines oTargetLines = oTarDoc.Lines;
                    for (int i = 0; i <= count; i++)
                    {
                        if (i != 0)
                        {
                            oTargetLines.Add();
                        }
                        oTargetLines.BaseType = Convert.ToInt32(oBaseType);
                        oTargetLines.BaseEntry = oBaseEntry;
                        oTargetLines.BaseLine = i;

                    }

                    lRetCode = oTarDoc.Add();


                    if (lRetCode != 0)
                    {
                        sErrDesc = oDICompany.GetLastErrorDescription();
                        throw new ArgumentException(sErrDesc);

                    }
                    else
                    {
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with SUCCESS", sFuncName);
                        return "SUCCESS";
                    }

                }
                else
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with SUCCESS", sFuncName);
                    return "SUCCESS";
                }
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message;
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);
                //return Ex.Message.ToString();
                throw Ex;
            }
        }

        public string Create_GRPOWithoutPO(DataView oDataView, SAPbobsCOM.Company oDICompany, string sConnString
                                   , string sErrDesc)
        {

            long lRetCode;
            string sFuncName = string.Empty;
            clsCommon oDataAccess = new clsCommon();
            string sItemCode = string.Empty;
            string sWhsCode = string.Empty;
            bool bContentExist = false;
            string sManBatchNumber = string.Empty;

            SAPbobsCOM.Documents oGRPO;
            oGRPO = (SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseDeliveryNotes));

            try
            {
                sFuncName = "Create_GRPOWithoutPO()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);


                sWhsCode = oDataView[0]["Outlet"].ToString();

                oGRPO.CardCode = oDataView[0]["SupplierCode"].ToString();

                for (int iRow = 0; iRow <= oDataView.Count - 1; iRow++)
                {
                    if (string.IsNullOrEmpty(oDataView[iRow]["Receipt Qty"].ToString())) continue;
                    if (Convert.ToDouble(oDataView[iRow]["Receipt Qty"].ToString()) == 0) continue;

                    bContentExist = true;

                    sItemCode = oDataView[iRow]["ItemCode"].ToString();

                    oGRPO.Lines.ItemCode = sItemCode;
                    oGRPO.Lines.WarehouseCode = sWhsCode;
                    oGRPO.Lines.Quantity = Convert.ToDouble(oDataView[iRow]["Receipt Qty"].ToString());
                    oGRPO.Lines.UserFields.Fields.Item("U_ReasonCode").Value = string.Format(oDataView[iRow]["Reason Code"].ToString());

                    string sCostCenter = oDataAccess.GetSingleValue("select U_CostCenter [CostCenter] from OWHS where WhsCode ='" + oDataView[iRow]["Outlet"].ToString() + "'", sConnString, sErrDesc);

                    oGRPO.Lines.COGSCostingCode = sCostCenter;
                    oGRPO.Lines.CostingCode = sCostCenter;
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("After getting & assigning the costing code from warehouse master : " + oDataView[iRow]["Outlet"].ToString(), sFuncName);

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling GetSingleValue() for Checking Batch Item", sFuncName);

                    sManBatchNumber = oDataAccess.GetSingleValue("select ManBtchNum  from OITM with(nolock) where ItemCode='" + sItemCode + "'", sConnString, sErrDesc);

                    if (sManBatchNumber.ToString().ToUpper() == "Y")
                    {
                        oGRPO.Lines.BatchNumbers.BatchNumber = sWhsCode;
                        oGRPO.Lines.BatchNumbers.Quantity = Convert.ToDouble(oDataView[iRow]["Receipt Qty"].ToString());
                        oGRPO.Lines.BatchNumbers.AddmisionDate = Convert.ToDateTime(DateTime.Today);
                        oGRPO.Lines.BatchNumbers.Add();
                    }


                    oGRPO.Lines.Add();
                }

                if (bContentExist == true)
                {
                    lRetCode = oGRPO.Add();

                    if (lRetCode != 0)
                    {
                        sErrDesc = oDICompany.GetLastErrorDescription();
                        throw new ArgumentException(sErrDesc);
                    }
                    else
                    {
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with SUCCESS ", sFuncName);
                        return "SUCCESS";
                    }
                }

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with SUCCESS ", sFuncName);
                return "SUCCESS";
            }

            catch (Exception Ex)
            {
                sErrDesc = Ex.Message;
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);
                throw Ex;
            }

            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oGRPO);
            }
        }

        public string Update_SYS2BatchNumber(SAPbobsCOM.Company oDICompany, DataView oDataView, string sInvTransNum,
                                                string sSYS1DocNum, string sConnString, string sErrDesc)
        {
            long lRetCode;
            string sFuncName = string.Empty;
            clsCommon oDataAccess = new clsCommon();
            string sItemCode = string.Empty;
            string sWhsCode = string.Empty;
            string sSYS2DocEntry = string.Empty;
            string sSYS1DocEntry = string.Empty;
            DataSet oDSPRNumeber;
            string sBatchItem = string.Empty;
            DataSet oDSBatchNum = new DataSet();
            //Int32 iInvLineCount = 0;
            string sLineNum = string.Empty;
            string sQuery = string.Empty;
            double dNumInSale;
            double dReceiptQty;

            SAPbobsCOM.StockTransfer oSTransReqSYS2 = (SAPbobsCOM.StockTransfer)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest));

            // SAPbobsCOM.StockTransfer oStockTrans = (SAPbobsCOM.StockTransfer)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oStockTransfer));


            try
            {
                sFuncName = "Update_SYS2BatchNumber()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);
                //SAPbobsCOM.StockTransfer oSTransReqSYS1 = (SAPbobsCOM.StockTransfer)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest));


                //oDSPRNumeber = oDataAccess.Get_SingleValue("select DocEntry ,U_PR_No,(select Count(DocEntry) from OWTR WITH(NOLOCK) where DocEntry ='" + sInvTransNum + "')  from OWTQ with (nolock) where DocNum ='" + sSYS1DocNum + "'", sConnString, sErrDesc);
                oDSPRNumeber = oDataAccess.Get_SingleValue("select DocEntry ,U_PR_No from OWTQ with (nolock) where DocNum ='" + sSYS1DocNum + "'", sConnString, sErrDesc);

                sSYS1DocEntry = oDSPRNumeber.Tables[0].Rows[0][0].ToString();

                //iInvLineCount = Convert.ToInt32(oDSPRNumeber.Tables[0].Rows[0][2].ToString());

                sSYS2DocEntry = oDataAccess.GetSingleValue("select DocEntry from OWTQ with (nolock) where DocNum  =(select U_ITR2 from OPRQ with (nolock) where DocEntry='" + oDSPRNumeber.Tables[0].Rows[0][1].ToString() + "')", sConnString, sErrDesc);

                // oStockTrans.GetByKey(Convert.ToInt32(sInvTransNum));

                oSTransReqSYS2.GetByKey(Convert.ToInt32(sSYS2DocEntry));

                // oSTransReqSYS1.GetByKey(Convert.ToInt32(sSYS1DocEntry));

                //Boolean bDelete = false;


                // for (int iLineCount = 0; iLineCount < oSTransReqSYS2.Lines.Count; iLineCount++)

                for (int iLineCount = 0; iLineCount <= oDataView.Count - 1; iLineCount++)
                {

                    sItemCode = oDataView[iLineCount]["ItemCode"].ToString().Trim();


                    sLineNum = oDataAccess.GetSingleValue("select VisOrder from WTQ1 with (nolock) where DocEntry ='" + sSYS2DocEntry + "' and ItemCode ='" + sItemCode + "'", sConnString, sErrDesc);

                    //if (bDelete == true)
                    //{
                    //    bDelete = false;
                    //    oSTransReqSYS2.Lines.SetCurrentLine(Convert.ToInt32(sLineNum) - 1);
                    //}
                    //else
                    oSTransReqSYS2.Lines.SetCurrentLine(Convert.ToInt32(sLineNum));

                    sQuery = "SELECT ISNULL(NumInSale,1) FROM OITM WITH(NOLOCK) WHERE ITEMCODE='" + oSTransReqSYS2.Lines.ItemCode + "'";

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling GetSingleValue() for Taking the NumInSale Value", sFuncName);
                    dNumInSale = Convert.ToDouble(oDataAccess.GetSingleValue(sQuery, sConnString, sErrDesc));

                    dReceiptQty = (Convert.ToDouble(oDataView[iLineCount]["Receipt Qty"].ToString().Trim()) * dNumInSale);


                    if (string.IsNullOrEmpty(oDataView[iLineCount]["Receipt Qty"].ToString().Trim()) || (Convert.ToDouble(oDataView[iLineCount]["Receipt Qty"].ToString().Trim()) == 0))
                    {

                        // oSTransReqSYS2.Lines.Quantity = Convert.ToDouble(oDataView[iLineCount]["Receipt Qty"].ToString().Trim());
                        oSTransReqSYS2.Lines.Quantity = Convert.ToDouble(dReceiptQty);

                        //oSTransReqSYS2.Lines.Delete();

                        continue;
                    }

                    //oSTransReqSYS2.Lines.Quantity = Convert.ToDouble(oDataView[iLineCount]["Receipt Qty"].ToString().Trim());
                    oSTransReqSYS2.Lines.Quantity = dReceiptQty;

                    sBatchItem = oDataAccess.GetSingleValue("select ManBtchNum  from OITM with (NOLOCK) where ItemCode ='" + sItemCode + "'", sConnString, sErrDesc);

                    if (sBatchItem == "Y")
                    {

                        //oDSBatchNum = oDataAccess.Get_SingleValue("select BatchNum,Quantity from IBT1 with (NOLOCK) where BaseEntry ='" + sSYS1DocEntry + "' and BaseType ='1250000001' and ItemCode ='" + sItemCode + "'", sConnString, sErrDesc);

                        oDSBatchNum = oDataAccess.Get_SingleValue("select BatchNum,ABS( Quantity) [Quantity] from IBT1 with (NOLOCK) where BaseEntry ='" + sInvTransNum + "' and BaseType ='67' and BsDocType ='1250000001' and ItemCode ='" + sItemCode + "'", sConnString, sErrDesc);

                        for (int iBatchRowCount = 0; iBatchRowCount < oDSBatchNum.Tables[0].Rows.Count; iBatchRowCount++)
                        {
                            oSTransReqSYS2.Lines.BatchNumbers.BatchNumber = oDSBatchNum.Tables[0].Rows[iBatchRowCount]["BatchNum"].ToString();
                            oSTransReqSYS2.Lines.BatchNumbers.AddmisionDate = Convert.ToDateTime(DateTime.Today).Date;
                            oSTransReqSYS2.Lines.BatchNumbers.Quantity = Convert.ToDouble(oDSBatchNum.Tables[0].Rows[iBatchRowCount]["Quantity"].ToString());
                            oSTransReqSYS2.Lines.BatchNumbers.Add();
                        }
                    }
                }


                //Delete the Zero Quantity from the Inventory Transfer Request:

                for (int iRowCount = oSTransReqSYS2.Lines.Count; iRowCount > 0; iRowCount--)
                {

                    oSTransReqSYS2.Lines.SetCurrentLine(iRowCount - 1);

                    if (oSTransReqSYS2.Lines.Quantity == 0)
                        oSTransReqSYS2.Lines.Delete();
                }

                lRetCode = oSTransReqSYS2.Update();

                if (lRetCode != 0)
                {
                    sErrDesc = oDICompany.GetLastErrorDescription();
                    throw new ArgumentException(sErrDesc);

                }
                else
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with SUCCESS ", sFuncName);
                    return "SUCCESS";
                }
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message;
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);
                //  return Ex.Message.ToString();
                throw Ex;
            }

            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oSTransReqSYS2);

            }
        }

        public string DeleteZeroQuantity(SAPbobsCOM.Documents oDocument, string sDraftKey, SAPbobsCOM.Company oDICompany, string sErrDesc)
        {
            double lRetCode = 0;
            string sFuncName = string.Empty;
            // SAPbobsCOM.Documents oDraft;
            Int32 iCount;
            try
            {
                sFuncName = "DeleteZeroQuantity()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);

                // oDraft = (SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts));

                oDocument.GetByKey(Convert.ToInt32(sDraftKey));

                iCount = oDocument.Lines.Count;

                for (int iRowCount = iCount; iRowCount > 0; iRowCount--)
                {
                    oDocument.Lines.SetCurrentLine(iRowCount - 1);

                    if (oDocument.Lines.Quantity == 0)
                    {

                        oDocument.Lines.Delete();

                    }
                }

                lRetCode = oDocument.Update();

                if (lRetCode != 0)
                {
                    sErrDesc = oDICompany.GetLastErrorDescription();
                    throw new ArgumentException(sErrDesc);

                }
                else
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with SUCCESS", sFuncName);
                    return "SUCCESS";
                }
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message;
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);
                return Ex.Message.ToString();
            }
        }

        public DataTable ConvertJSONToDataTable(string jsonString)
        {
            DataTable dt = new DataTable();
            //strip out bad characters
            string[] jsonParts = Regex.Split(jsonString.Replace("[", "").Replace("]", ""), "},{");

            //hold column names
            List<string> dtColumns = new List<string>();

            //get columns
            foreach (string jp in jsonParts)
            {
                //only loop thru once to get column names
                string[] propData = Regex.Split(jp.Replace("{", "").Replace("}", ""), ",");
                foreach (string rowData in propData)
                {
                    try
                    {
                        int idx = rowData.IndexOf(":");
                        string n = rowData.Substring(0, idx - 1);
                        string v = rowData.Substring(idx + 1);
                        if (!dtColumns.Contains(n))
                        {
                            dtColumns.Add(n.Replace("\"", ""));
                        }
                    }
                    catch (Exception)
                    {
                        throw new Exception(string.Format("Error Parsing Column Name : {0}", rowData));
                    }

                }
                break; // TODO: might not be correct. Was : Exit For
            }

            //build dt
            foreach (string c in dtColumns)
            {
                dt.Columns.Add(c);
            }
            //get table data
            foreach (string jp in jsonParts)
            {
                string[] propData = Regex.Split(jp.Replace("{", "").Replace("}", ""), ",");
                DataRow nr = dt.NewRow();
                foreach (string rowData in propData)
                {
                    try
                    {
                        int idx = rowData.IndexOf(":");
                        string n = rowData.Substring(0, idx - 1).Replace("\"", "");
                        string v = rowData.Substring(idx + 1).Replace("\"", "");
                        nr[n] = v;
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                }
                dt.Rows.Add(nr);
            }
            return dt;
        }
    }
}
