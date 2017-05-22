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
    public class clsDeliveryOrder
    {
        clsLog oLog = new clsLog();

        public Int16 p_iDebugMode = DEBUG_ON;

        public const Int16 RTN_SUCCESS = 1;
        public const Int16 RTN_ERROR = 0;
        public const Int16 DEBUG_ON = 1;
        public const Int16 DEBUG_OFF = 0;


        DataAccess oDataAccess = new DataAccess();
        clsCommon oGetSingleValue = new clsCommon();

        public DataSet Get_StockTakeCounting(string sOutlet, string sUserRole, string sStatus, string sDocEntry, string sConnString, string sErrDesc)
        {
            string sFuncName = string.Empty;
            string sProcedureName = string.Empty;
            DataSet oDataSet = new DataSet();

            try
            {
                sFuncName = "Get_StockTakeCounting()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);

                sProcedureName = "EXEC [AE_SP018_StockTakeCounting] '" + sOutlet + "','" + sUserRole + "','" + sStatus + "','" + sDocEntry + "'";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_StoredProcedure()", sFuncName);

                oDataSet = oDataAccess.Run_StoredProcedure(sProcedureName, sConnString);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);

                return oDataSet;
            }
            catch (Exception Ex)
            {

                sErrDesc = Ex.Message;
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);
                throw Ex;
            }

        }

        public DataSet Get_StockTakeCountingList(string sOutlet, string sConnString, string sErrDesc)
        {
            string sFuncName = string.Empty;
            string sProcedureName = string.Empty;
            DataSet oDataSet = new DataSet();

            try
            {
                sFuncName = "Get_StockTakeCountingList()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);


                sProcedureName = "EXEC [AE_SP020_GetSalesTakeCountList] '" + sOutlet + "'";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_StoredProcedure()", sFuncName);

                oDataSet = oDataAccess.Run_StoredProcedure(sProcedureName, sConnString);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with SUCCESS", sFuncName);

                return oDataSet;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message;
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);
                throw Ex;
            }

        }

        public string StockTakeApprove(DataSet oDataSet, string sOutlet, string sCompanyDB, string sConnString, string sErrDesc)
        {
            string sFuncName = string.Empty;
            DataView oDVHeader = new DataView();
            DataTable oDTDistinct = new DataTable();
            string sQueryString = string.Empty;
            string sDocEntry = string.Empty;
            try
            {
                sFuncName = "StockTakeApprove()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);

                oDVHeader = oDataSet.Tables[0].DefaultView;
                oDTDistinct = oDVHeader.Table.DefaultView.ToTable(true, "DocEntry");

                for (int iRowCount = 0; iRowCount <= oDTDistinct.Rows.Count - 1; iRowCount++)
                {
                    sDocEntry = oDTDistinct.Rows[0]["DocEntry"].ToString();

                    sQueryString += "INSERT INTO StockTakeApproval values(GETDATE (),'" + sDocEntry + "','" + sOutlet + "','" + sCompanyDB + "',NULL,NULL,NULL)";
                }

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_QueryString()", sFuncName);
                oDataAccess.Run_QueryString(sQueryString, sConnString);


                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with SUCCESS", sFuncName);

                return "SUCCESS";
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message;
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);
                throw Ex;
            }
        }

        public string CreateDeliverOrderDraft(SAPbobsCOM.Company oDICompany, string sUserRole, string sOutlet
                                                , string sConnString, string sErrDesc)
        {
            double lRetCode = 0;
            string sFuncName = string.Empty;
            DateTime dtDocDate;
            DataSet oDSDetails = new DataSet();

            try
            {
                sFuncName = "CreateDeliverOrderDraft()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);

                SAPbobsCOM.Documents oDeliveryDraft;
                oDeliveryDraft = (SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts));

                dtDocDate = Convert.ToDateTime(DateTime.Now);

                oDeliveryDraft.CardCode = "STOCKTAKE";
                oDeliveryDraft.DocDate = dtDocDate;
                oDeliveryDraft.DocDueDate = dtDocDate;
                oDeliveryDraft.TaxDate = dtDocDate;
                oDeliveryDraft.UserFields.Fields.Item("U_StockTakeStatus").Value = "Draft";
                oDeliveryDraft.UserFields.Fields.Item("U_UserRole").Value = sUserRole;

                oDeliveryDraft.DocObjectCode = SAPbobsCOM.BoObjectTypes.oDeliveryNotes;



                oDSDetails.Clear();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_StoredProcedure() for Getting All the ItemCodes ", sFuncName);

                oDSDetails = oDataAccess.Run_StoredProcedure("Exec AE_SP019_GetItemsToDODraft '" + sUserRole + "','" + sOutlet + "' ", sConnString);

                if (oDSDetails != null && oDSDetails.Tables[0].Rows.Count != 0)
                {
                    for (int IRow = 0; IRow <= oDSDetails.Tables[0].Rows.Count - 1; IRow++)
                    {
                        oDeliveryDraft.Lines.ItemCode = oDSDetails.Tables[0].Rows[IRow][0].ToString();
                        oDeliveryDraft.Lines.WarehouseCode = sOutlet;
                        oDeliveryDraft.Lines.Quantity = 0;
                        oDeliveryDraft.Lines.UnitPrice = 0;
                        oDeliveryDraft.Lines.VatGroup = "SR";

                        string sCostCenter = oDataAccess.GetSingleValue("select U_CostCenter [CostCenter] from OWHS where WhsCode ='" + sOutlet + "'", sConnString);

                        oDeliveryDraft.Lines.COGSCostingCode = sCostCenter;
                        oDeliveryDraft.Lines.CostingCode = sCostCenter;
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("After getting & assigning the costing code from warehouse master : " + sOutlet, sFuncName);

                        oDeliveryDraft.Lines.Add();
                    }

                }

                lRetCode = oDeliveryDraft.Add();

                if (lRetCode != 0)
                {
                    sErrDesc = oDICompany.GetLastErrorDescription();
                    throw new ArgumentException(sErrDesc);

                }
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS", sFuncName);
                //oDICompany.Disconnect();
                //oDICompany = null;

                return "SUCCESS";
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message;
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);
                //oDICompany.Disconnect();
                //oDICompany = null;
                throw Ex;
            }
        }

        public string UpdateDOStatus(DataSet oDataSet, SAPbobsCOM.Company oDICompany, string sStatus, string sUserRole
                                    , Boolean bIsApprove, string sErrDesc)
        {
            string sFuncName = string.Empty;
            DataTable oDTDistHeader = new DataTable();
            DataView oDVHeader = new DataView();
            DateTime dtCountDate;

            long lRetCode;
            string sDocEntry = string.Empty;
            clsCopyMarketingDocument oDeleteEmptyRow = new clsCopyMarketingDocument();
            try
            {
                sFuncName = "UpdateDOStatus()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);

                SAPbobsCOM.Documents oDraft;

                oDVHeader = oDataSet.Tables[0].DefaultView;

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting the SAP Transaction ", sFuncName);

                if (!oDICompany.InTransaction) oDICompany.StartTransaction();

                oDTDistHeader = oDVHeader.Table.DefaultView.ToTable(true, "DocEntry");
                dtCountDate = Convert.ToDateTime(oDVHeader[0]["CountDate"].ToString());

                for (int iDraftCount = 0; iDraftCount < oDTDistHeader.Rows.Count; iDraftCount++)
                {
                    sDocEntry = oDTDistHeader.Rows[iDraftCount][0].ToString();
                    oDVHeader.RowFilter = "DocEntry = '" + sDocEntry + "'";


                    oDraft = ((SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts)));
                    var docentry = Convert.ToInt32(oDVHeader[iDraftCount]["DocEntry"].ToString());
                    oDraft.GetByKey(Convert.ToInt32(docentry));

                    oDraft.DocDate = dtCountDate;

                    oDraft.UserFields.Fields.Item("U_StockTakeStatus").Value = sStatus;
                    oDraft.UserFields.Fields.Item("U_UserRole").Value = sUserRole;

                    if (bIsApprove == true)
                    {
                        oDraft.UserFields.Fields.Item("U_PendingForApproval").Value = "Y";
                    }

                    if (oDataSet.Tables[0].Rows[0]["ApprovedBy"].ToString() != string.Empty)
                    {
                        oDraft.UserFields.Fields.Item("U_ApprovedBy").Value = oDataSet.Tables[0].Rows[0]["ApprovedBy"].ToString();
                    }
                    if (oDataSet.Tables[0].Rows[0]["ApprovedDate"].ToString() != string.Empty)
                    {
                        oDraft.UserFields.Fields.Item("U_ApprovedDate").Value = Convert.ToDateTime(oDataSet.Tables[0].Rows[0]["ApprovedDate"].ToString());
                    }
                    oDraft.UserFields.Fields.Item("U_CreatedBy").Value = oDataSet.Tables[0].Rows[0]["CreatedBy"].ToString();

                    for (int iRowCount = 0; iRowCount <= oDVHeader.Count - 1; iRowCount++)
                    {
                        int lineNum = Convert.ToInt32(oDVHeader[iRowCount]["LineNum"].ToString());
                        oDraft.Lines.SetCurrentLine(lineNum);
                        oDraft.Lines.Quantity = oDVHeader[iRowCount]["CountedInvUOM"].ToString() == string.Empty ? 0 : Convert.ToDouble(oDVHeader[iRowCount]["CountedInvUOM"].ToString());
                        oDraft.Lines.UserFields.Fields.Item("U_Variance").Value = Convert.ToDouble(oDVHeader[iRowCount]["Variance"].ToString());
                        oDraft.Lines.UserFields.Fields.Item("U_StocktakeQty1").Value = Convert.ToDouble(oDVHeader[iRowCount]["CountedUOM1"].ToString());
                        oDraft.Lines.UserFields.Fields.Item("U_StocktakeQty2").Value = Convert.ToDouble(oDVHeader[iRowCount]["CountedUOM2"].ToString());

                    }

                    lRetCode = oDraft.Update();

                    if (lRetCode != 0)
                    {
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("RollBack the SAP Transaction", sFuncName);

                        if (oDICompany.InTransaction) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                        sErrDesc = oDICompany.GetLastErrorDescription(); throw new ArgumentException(sErrDesc);

                    }
                    else
                    {
                        if (bIsApprove == true)
                        {
                            if (oDeleteEmptyRow.DeleteZeroQuantity(oDraft, sDocEntry, oDICompany, sErrDesc) != "SUCCESS") throw new ArgumentException(sErrDesc);
                        }
                    }
                }
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Comitted the SAP Transaction", sFuncName);
                if (oDICompany.InTransaction) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS", sFuncName);
                //oDICompany.Disconnect();
                //oDICompany = null;
                return "SUCCESS";

            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message;
                oLog.WriteToLogFile(sErrDesc, sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("RollBack the SAP Transaction", sFuncName);
                if (oDICompany.InTransaction) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);

                //oDICompany.Disconnect();
                //oDICompany = null;
                throw Ex;
            }

        }

        public string Create_StockTakeCounting(DataSet oDataset, SAPbobsCOM.Company oDICompany, DateTime dtCountDate
                                                , string sUserID, string sErrDesc)
        {
            //double lRetCode;
            DataTable oDTHeader = new DataTable();
            string sFuncName = string.Empty;
            //SAPbobsCOM.StockTaking  oStockTakeCount;

            try
            {
                sFuncName = "Create_StockTakeCounting()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);

                oDTHeader = oDataset.Tables[0];


                SAPbobsCOM.CompanyService oCS = oDICompany.GetCompanyService();
                SAPbobsCOM.InventoryCountingsService oICS = (SAPbobsCOM.InventoryCountingsService)(oCS.GetBusinessService(SAPbobsCOM.ServiceTypes.InventoryCountingsService));
                SAPbobsCOM.InventoryCounting oIC = oICS.GetDataInterface(SAPbobsCOM.InventoryCountingsServiceDataInterfaces.icsInventoryCounting) as SAPbobsCOM.InventoryCounting;
                oIC.CountDate = DateTime.Now;


                SAPbobsCOM.InventoryCountingLines oICLS = oIC.InventoryCountingLines;
                SAPbobsCOM.InventoryCountingLine oICL = oICLS.Add();
                oICL.ItemCode = "00111520102Y";
                oICL.CountedQuantity = 4;

                oICL.WarehouseCode = "1";

                SAPbobsCOM.InventoryCountingParams oICP = oICS.Add(oIC);



                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);

                return "SUCCESS";

            }
            catch (Exception Ex)
            {

                sErrDesc = Ex.Message;
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);
                throw Ex;
            }
        }

        public string Create_GoodsReceipt(DataSet oDataSet, SAPbobsCOM.Company oDICompany, string sConnString, string sErrDesc)
        {
            DateTime dtDocDate;
            DataTable oDTHeader;
            double lRetCode;
            string sItemCode = string.Empty;
            string sWhscode = string.Empty;
            string sFuncName = string.Empty;

            try
            {
                sFuncName = "Create_GoodsReceipt()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);

                SAPbobsCOM.Documents oGoodsReceipt;
                oGoodsReceipt = (SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenEntry));

                dtDocDate = Convert.ToDateTime(DateTime.Today);
                oDTHeader = oDataSet.Tables[0];


                oGoodsReceipt.DocDate = dtDocDate;
                oGoodsReceipt.DocDueDate = dtDocDate;
                oGoodsReceipt.TaxDate = dtDocDate;


                for (int iRow = 0; iRow <= oDTHeader.Rows.Count - 1; iRow++)
                {
                    if (Convert.ToDouble(oDTHeader.Rows[iRow]["Adjust in Inventory UOM"].ToString()) < 0) continue;

                    sItemCode = oDTHeader.Rows[iRow]["ItemCode"].ToString();
                    sWhscode = oDTHeader.Rows[iRow]["Outlet"].ToString();

                    oGoodsReceipt.Lines.ItemCode = sItemCode;
                    oGoodsReceipt.Lines.WarehouseCode = sWhscode;
                    oGoodsReceipt.Lines.Quantity = Convert.ToDouble(oDTHeader.Rows[iRow]["Variance"].ToString());

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling GetSingleValue() for Checking the Batch Item", sFuncName);

                    if (oGetSingleValue.Get_SingleValue("select ManBtchNum  from OITM with (nolock) where ItemCode='" + sItemCode + "'",
                           sConnString, sErrDesc).ToString().ToUpper() == "Y")
                    {
                        oGoodsReceipt.Lines.BatchNumbers.BatchNumber = sWhscode;
                        oGoodsReceipt.Lines.BatchNumbers.AddmisionDate = Convert.ToDateTime(DateTime.Today);
                        oGoodsReceipt.Lines.BatchNumbers.Add();

                    }


                    oGoodsReceipt.Lines.Add();

                }

                lRetCode = oGoodsReceipt.Add();

                if (lRetCode != 0)
                {
                    sErrDesc = oDICompany.GetLastErrorDescription(); throw new ArgumentException(sErrDesc);
                }
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS", sFuncName);
                return "SUCCESS";
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message;
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);
                throw Ex;
            }
        }

    }
}
