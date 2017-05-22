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
    public class clsInventoryTransferRequest
    {
        clsLog oLog = new clsLog();

        public Int16 p_iDebugMode = DEBUG_ON;

        public const Int16 RTN_SUCCESS = 1;
        public const Int16 RTN_ERROR = 0;
        public const Int16 DEBUG_ON = 1;
        public const Int16 DEBUG_OFF = 0;


        public string Create_InventoryTransferRequest(SAPbobsCOM.Company oDICompany, DataSet oDSInvRequest
                                                      , string sConnString, string sErrDesc)
        {
            string sFuncName = string.Empty;
            double lRetCode;
            DataTable oDTInvRequest = new DataTable();
            string sFromWarehouse = string.Empty;
            string sToWarehouse = string.Empty;
            string sSubmittedBy = string.Empty;
            string sRemarks = string.Empty;
            DateTime dtRequestDate;
            clsCommon oGetSingleValue = new clsCommon();
            string sQuery = string.Empty;
            string sSeries = string.Empty;

            string sItemCode = string.Empty;
            string sBatchItem = string.Empty;

            SAPbobsCOM.Recordset oDIRs1;
            double iQuantity = 0;
            SAPbobsCOM.StockTransfer oInvTransRequest;

            try
            {
                sFuncName = "Create_InventoryTransferRequest()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);


                oDTInvRequest = oDSInvRequest.Tables[0];

                oInvTransRequest = (SAPbobsCOM.StockTransfer)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest));

                sFromWarehouse = oDTInvRequest.Rows[0]["FromOutlet"].ToString().Trim();
                sToWarehouse = oDTInvRequest.Rows[0]["ToOutlet"].ToString().Trim();
                sSubmittedBy = oDTInvRequest.Rows[0]["SubmittedBy"].ToString().Trim();
                sRemarks = oDTInvRequest.Rows[0]["Remarks"].ToString().Trim();

                //string[] sPostingDate = oDTInvRequest.Rows[0]["RequestDate"].ToString().Split(' ');
                dtRequestDate = Convert.ToDateTime(oDTInvRequest.Rows[0]["RequestDate"].ToString().Trim());


                sQuery = "select Series from NNM1 WITH (NOLOCK) where ObjectCode ='1250000001' and SeriesName NOT IN ('SYS1','SYS2')";

                sSeries = oGetSingleValue.GetSingleValue(sQuery, sConnString, sErrDesc);


                oInvTransRequest.DocDate = dtRequestDate;
                oInvTransRequest.DueDate = dtRequestDate;
                oInvTransRequest.TaxDate = dtRequestDate;
                oInvTransRequest.FromWarehouse = sFromWarehouse;
                oInvTransRequest.ToWarehouse = sToWarehouse;
                oInvTransRequest.Series = Convert.ToInt32(sSeries);

                oInvTransRequest.UserFields.Fields.Item("U_CreatedBy").Value = sSubmittedBy;
                oInvTransRequest.JournalMemo = sRemarks;

                //copy the lines
                int count = oDTInvRequest.Rows.Count;
                SAPbobsCOM.StockTransfer_Lines oTargetLines = oInvTransRequest.Lines;

                for (int iRow = 0; iRow < count; iRow++)
                {
                    if (string.IsNullOrEmpty(oDTInvRequest.Rows[iRow]["OrderQuantity"].ToString()) == true) continue;
                    if (Convert.ToDecimal(oDTInvRequest.Rows[iRow]["OrderQuantity"].ToString()) == 0) continue;

                    sItemCode = oDTInvRequest.Rows[iRow]["ItemCode"].ToString().Trim();
                    iQuantity = Convert.ToDouble(oDTInvRequest.Rows[iRow]["OrderQuantity"].ToString().Trim());

                    oTargetLines.ItemCode = sItemCode;
                    oTargetLines.Quantity = iQuantity;
                    oTargetLines.FromWarehouseCode = sFromWarehouse;
                    oTargetLines.WarehouseCode = sToWarehouse;

                    sQuery = String.Format("SELECT ManBtchNum FROM OITM WITH(NOLOCK) WHERE ItemCode = '{0}'", sItemCode);

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Execute SQL : " + sQuery, sFuncName);
                    sBatchItem = oGetSingleValue.GetSingleValue(sQuery, sConnString, sErrDesc);

                    if (sBatchItem.ToString().ToUpper().Trim() == "Y")
                    {

                        sQuery = String.Format("SELECT BatchNum, BaseLinNum, Quantity FROM OIBT WITH(NOLOCK) WHERE ItemCode = '{0}' And WhsCode = '{1}' ORDER BY InDate", sItemCode, sFromWarehouse);

                        oDIRs1 = (SAPbobsCOM.Recordset)oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Execute SQL : " + sQuery, sFuncName);

                        oDIRs1.DoQuery(sQuery);

                        while (!oDIRs1.EoF)
                        {
                            oTargetLines.BatchNumbers.BatchNumber = Convert.ToString(oDIRs1.Fields.Item("BatchNum").Value);
                            if (iQuantity > Convert.ToDouble(oDIRs1.Fields.Item("Quantity").Value))
                            {
                                //If Balance Qty>Batch Qty, then get full Batch Qty
                                oTargetLines.BatchNumbers.Quantity = Convert.ToDouble(oDIRs1.Fields.Item("Quantity").Value);
                                //minus current qty with Batch Qty
                                iQuantity = iQuantity - (Convert.ToDouble(oDIRs1.Fields.Item("Quantity").Value));
                            }
                            else
                            {
                                oTargetLines.BatchNumbers.Quantity = iQuantity;
                                iQuantity = iQuantity - iQuantity;
                            }

                            if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Add BatchNumbers ", sFuncName);
                            oTargetLines.BatchNumbers.Add();
                            if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Success - Add BatchNumbers ", sFuncName);

                            if (iQuantity <= 0)
                                break; // TODO: might not be correct. Was : Exit Do
                            oDIRs1.MoveNext();
                        }
                    }

                    oTargetLines.Add();

                }

                lRetCode = oInvTransRequest.Add();

                if (lRetCode != 0)
                {
                    sErrDesc = oDICompany.GetLastErrorDescription(); throw new ArgumentException(sErrDesc);
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

        public string Approve_InventoryTransferRequest(SAPbobsCOM.Company oDICompany, DataSet oDSInvTransfer
                                                     , string sConnString, string sErrDesc)
        {
            string sFuncName = string.Empty;

            DataTable oDTInvTransfer = new DataTable();
            clsCommon oGetSingleValue = new clsCommon();
            string sQuery = string.Empty;
            string sSeries = string.Empty;
            Int32 sDocEntry;


            try
            {
                sFuncName = "Approve_InventoryTransferRequest()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                oDTInvTransfer = oDSInvTransfer.Tables[0];

                sQuery = "select Series from NNM1 WITH (NOLOCK) where ObjectCode ='67' and SeriesName NOT IN ('SYS1','SYS2')";

                sSeries = oGetSingleValue.GetSingleValue(sQuery, sConnString, sErrDesc);

                sDocEntry = Convert.ToInt32(oDTInvTransfer.Rows[0]["DocEntry"].ToString().Trim());


                if (ConvertRequestToTransfer(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest, sDocEntry,
                                        SAPbobsCOM.BoObjectTypes.oStockTransfer, oDICompany, sSeries, false, sConnString, sErrDesc) != "SUCCESS") throw new ArgumentException(sErrDesc);



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

        public string ConvertRequestToTransfer(SAPbobsCOM.BoObjectTypes oBaseType, int oBaseEntry, SAPbobsCOM.BoObjectTypes oTarType,
                        SAPbobsCOM.Company oDICompany, string sSeries, bool IsDraft, string sConnString, string sErrDesc)
        {

            double lRetCode = 0;
            string sFuncName = string.Empty;
            string sItemCode = string.Empty;
            string sWhsCode = string.Empty;
            string sQuery = string.Empty;
            SAPbobsCOM.Recordset oDIRs1;
            double iQuantity = 0;

            clsCommon oGetSingleValue = new clsCommon();

            string sBatchItem = string.Empty;

            try
            {
                sFuncName = "ConvertRequestToTransfer()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);


                SAPbobsCOM.StockTransfer oBaseDoc = ((SAPbobsCOM.StockTransfer)(oDICompany.GetBusinessObject(oBaseType)));
                if (oBaseDoc.GetByKey(oBaseEntry))
                {
                    //base document found, copy to target doc
                    SAPbobsCOM.StockTransfer oTarDoc = ((SAPbobsCOM.StockTransfer)(oDICompany.GetBusinessObject(oTarType)));

                    if (IsDraft == false)
                    {
                        oTarDoc = ((SAPbobsCOM.StockTransfer)(oDICompany.GetBusinessObject(oTarType)));
                    }
                    else
                    {
                        oTarDoc = ((SAPbobsCOM.StockTransfer)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts)));
                        oTarDoc.DocObjectCode = oTarType;
                    }


                    //todo: copy the cardcode, docduedate, lines
                    oTarDoc.CardCode = oBaseDoc.CardCode;
                    oTarDoc.Series = Convert.ToInt32(sSeries);
                    oTarDoc.JournalMemo = oBaseDoc.JournalMemo;

                    sWhsCode = oBaseDoc.FromWarehouse;

                    //copy the lines
                    int count = oBaseDoc.Lines.Count - 1;
                    SAPbobsCOM.StockTransfer_Lines oTargetLines = oTarDoc.Lines;
                    for (int i = 0; i <= count; i++)
                    {
                        oBaseDoc.Lines.SetCurrentLine(i);
                        sItemCode = oBaseDoc.Lines.ItemCode;
                        iQuantity = oBaseDoc.Lines.Quantity;

                        oTargetLines.BaseType = SAPbobsCOM.InvBaseDocTypeEnum.InventoryTransferRequest;
                        oTargetLines.BaseEntry = oBaseEntry;
                        oTargetLines.BaseLine = i;


                        sQuery = String.Format("SELECT ManBtchNum FROM OITM WHERE ItemCode = '{0}'", sItemCode);

                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Execute SQL : " + sQuery, sFuncName);
                        sBatchItem = oGetSingleValue.GetSingleValue(sQuery, sConnString, sErrDesc);

                        if (sBatchItem.ToString().ToUpper().Trim() == "Y")
                        {

                            sQuery = String.Format("SELECT BatchNum, BaseLinNum, Quantity FROM OIBT WHERE ItemCode = '{0}' And WhsCode = '{1}' ORDER BY InDate", sItemCode, sWhsCode);

                            oDIRs1 = (SAPbobsCOM.Recordset)oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                            if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Execute SQL : " + sQuery, sFuncName);

                            oDIRs1.DoQuery(sQuery);

                            while (!oDIRs1.EoF)
                            {
                                oTargetLines.BatchNumbers.BatchNumber = Convert.ToString(oDIRs1.Fields.Item("BatchNum").Value);
                                if (iQuantity > Convert.ToDouble(oDIRs1.Fields.Item("Quantity").Value))
                                {
                                    //If Balance Qty>Batch Qty, then get full Batch Qty
                                    oTargetLines.BatchNumbers.Quantity = Convert.ToDouble(oDIRs1.Fields.Item("Quantity").Value);
                                    //minus current qty with Batch Qty
                                    iQuantity = iQuantity - (Convert.ToDouble(oDIRs1.Fields.Item("Quantity").Value));
                                }
                                else
                                {
                                    oTargetLines.BatchNumbers.Quantity = iQuantity;
                                    iQuantity = iQuantity - iQuantity;
                                }

                                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Add BatchNumbers ", sFuncName);
                                oTargetLines.BatchNumbers.Add();
                                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Success - Add BatchNumbers ", sFuncName);

                                if (iQuantity <= 0)
                                    break; // TODO: might not be correct. Was : Exit Do
                                oDIRs1.MoveNext();
                            }
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

        public string Reject_InventoryTransferRequest(SAPbobsCOM.Company oDICompany, DataSet oDSInvTransfer
                                                  , string sConnString, string sErrDesc)
        {
            string sFuncName = string.Empty;
            double lRetCode;
            DataTable oDTInvTransfer = new DataTable();
            Int32 sDocEntry;
            SAPbobsCOM.StockTransfer oInvTransRequest;

            try
            {
                sFuncName = "Reject_InventoryTransferRequest()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                oInvTransRequest = (SAPbobsCOM.StockTransfer)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest));

                oDTInvTransfer = oDSInvTransfer.Tables[0];

                sDocEntry = Convert.ToInt32(oDTInvTransfer.Rows[0]["DocEntry"].ToString().Trim());

                oInvTransRequest.GetByKey(sDocEntry);

                oInvTransRequest.Comments = "Rejected";

                lRetCode = oInvTransRequest.Update();

                if (lRetCode != 0)
                {
                    sErrDesc = oDICompany.GetLastErrorDescription(); throw new ArgumentException(sErrDesc);
                }

                lRetCode = oInvTransRequest.Close();

                if (lRetCode != 0)
                {
                    sErrDesc = oDICompany.GetLastErrorDescription(); throw new ArgumentException(sErrDesc);
                }

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName);
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


        public DataSet Get_OpenTransferRequest(string sFromOutlet, string sToOutlet, string sStatus, string sFromDate
                                            , string sToDate, string sConnString, string sErrDesc)
        {
            DataSet oDataset;
            string sFuncName = string.Empty;
            string sProcedureName = string.Empty;

            try
            {
                sFuncName = "Get_OpenTransferRequest()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                DataAccess oDataAccess = new DataAccess();

                sProcedureName = "EXEC [AE_SP026_OpenTransferRequest] '" + sFromOutlet + "','" + sToOutlet + "','" + sStatus + "','" + sFromDate + "','" + sToDate + "'";

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

        public DataSet Get_OpenTransferRequestDetails(string sDocEntry, string sConnString, string sErrDesc)
        {
            DataSet oDataset;
            string sFuncName = string.Empty;
            string sProcedureName = string.Empty;

            try
            {
                sFuncName = "Get_OpenTransferRequestDetails()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                DataAccess oDataAccess = new DataAccess();

                sProcedureName = "EXEC [AE_SP027_OpenTransferRequestDetails] '" + sDocEntry + "'";

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

        public DataSet Get_InventoryTransferRequest_ItemSearch(string sFromWhsCode, string sToWhsCode, string sGroupType
                                                    , string sConnString, string sErrDesc)
        {
            DataSet oDataset;
            string sFuncName = string.Empty;
            string sProcedureName = string.Empty;

            try
            {
                sFuncName = "Get_InventoryTransferRequest_ItemSearch()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                DataAccess oDataAccess = new DataAccess();

                sProcedureName = "EXEC [AE_SP025_InventoryTransferRequest_ItemSearch] '" + sFromWhsCode + "','" + sToWhsCode + "','" + sGroupType + "'";

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
    }
}
