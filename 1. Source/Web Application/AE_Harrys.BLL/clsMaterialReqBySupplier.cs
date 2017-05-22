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
    public class clsMaterialReqBySupplier
    {
        clsLog oLog = new clsLog();

        public Int16 p_iDebugMode = DEBUG_ON;

        public const Int16 RTN_SUCCESS = 1;
        public const Int16 RTN_ERROR = 0;
        public const Int16 DEBUG_ON = 1;
        public const Int16 DEBUG_OFF = 0;

        public string sErrDesc = string.Empty;

        public DataSet Get_MaterialReqBySupplier(string sSupplier, string sOutlet, string sUserRole
                                                  , string sConnString, string sErrDesc)
        {
            DataSet oDataset;
            string sFuncName = string.Empty;

            try
            {
                sFuncName = "Get_MaterialReqBySupplier()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                DataAccess oDataAccess = new DataAccess();


                if (sSupplier != string.Empty)
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_StoredProcedure() ", sFuncName);
                    oDataset = oDataAccess.Run_QueryString("EXEC AE_SP004_MaterialReqBySupplier '" + sSupplier + "','" + sOutlet + "','" + sUserRole + "'", sConnString);
                }
                else
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_StoredProcedure() ", sFuncName);
                    oDataset = oDataAccess.Run_QueryString("EXEC AE_SP010_Popup_ItemSearch '" + sOutlet + "','" + sUserRole + "'", sConnString);
                }

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

        public string Create_PurchaseRequest(DataSet oDSRequestData, Boolean IsDraft, SAPbobsCOM.Company oDICompany
                                            , Boolean IsAdd, Boolean bIsSubmit, Boolean bIsDelCharge, string sConnString, string sErrDesc)
        {
            string sFuncName = string.Empty;
            DataTable oDTHeader;
            DataTable oDTLine;
            DateTime dtDocDate;
            string sWhsCode;
            clsCommon oGetSingleValue = new clsCommon();
            DataSet sPurReqDocEntry = new DataSet();
            clsCopyMarketingDocument oCopyDocument = new clsCopyMarketingDocument();
            string sQueryString = string.Empty;

            try
            {
                sFuncName = "Create_PurchaseRequest()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                //if (oDICompany.InTransaction == false) oDICompany.StartTransaction();

                oDTHeader = oDSRequestData.Tables[0];
                oDTLine = oDSRequestData.Tables[1];

                string[] sPostingDate = oDTHeader.Rows[0]["PostingDate"].ToString().Split(' ');
                dtDocDate = Convert.ToDateTime(sPostingDate[0]);
                //dtDocDate = Convert.ToDateTime(oDTHeader.Rows[0]["PostingDate"].ToString());

                sWhsCode = oDTHeader.Rows[0]["Outlet"].ToString();


                if (oDTLine.Rows[0]["SupplierCode"].ToString().ToUpper() == "VA-HARHO")
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Supplier Code : VA-HARHO", sFuncName);


                    if (IsDraft == true)
                    {
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_SingleValue()", sFuncName);

                        sQueryString = " select Count(T1.DocEntry) [Count] ,(select COUNT(DocEntry) from DRF1 with (nolock) where DocEntry =T0.DocEntry )[TotCount],T0.DocEntry from ODRF T0 with (nolock)" +
                                            " INNER JOIN DRF1 T1 with (nolock) ON T0.DocEntry =T1.DocEntry where T0.ObjType ='540000006' and ISNULL (T0.CANCELED ,'N')<>'Y' and T0.DocStatus ='O' " +
                                            " AND T1.U_ApprovalLevel IS NULL AND T1.WhsCode ='" + oDTHeader.Rows[0]["Outlet"].ToString() + "' group by T0.DocEntry";

                        sPurReqDocEntry = oGetSingleValue.Get_SingleValue(sQueryString, sConnString, sErrDesc);


                        if (sPurReqDocEntry != null && sPurReqDocEntry.Tables[0].Rows.Count > 0)
                        {
                            if (sPurReqDocEntry.Tables[0].Rows[0][0].ToString() == sPurReqDocEntry.Tables[0].Rows[0][1].ToString())
                            {
                                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Update_PRDraft()", sFuncName);

                                if (Update_PRDraft(oDSRequestData, oDICompany, sPurReqDocEntry.Tables[0].Rows[0][2].ToString(), sWhsCode
                                    , sConnString, IsAdd, bIsSubmit, bIsDelCharge, sErrDesc) != "SUCCESS") throw new ArgumentException(sErrDesc);

                                // if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                                return "SUCCESS";
                            }
                        }


                        else
                        {

                            if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_SingleValue()", sFuncName);

                            sQueryString = "select  Top 1  T0.DocEntry   from PRQ1 T0 with (nolock) inner join OPRQ T1 with (nolock) " +
                                           " on T0.DocEntry = T1.DocEntry where LineVendor ='VA-HARHO' and T1.CANCELED <> 'Y' " +
                                           " and T1.DocStatus = 'O' group by T0.DocEntry  order by DocEntry desc ";

                            sPurReqDocEntry = oGetSingleValue.Get_SingleValue(sQueryString, sConnString, sErrDesc);

                            if (sPurReqDocEntry.Tables[0].Rows.Count != 0 && sPurReqDocEntry != null)
                            {

                                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Document Already Exists!!!", sFuncName);

                                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling CopyPurReqToPurchaseQuot()", sFuncName);

                                if (oCopyDocument.PurReqToPurchaseQuot(oDSRequestData, SAPbobsCOM.BoObjectTypes.oPurchaseRequest
                                                        , Convert.ToInt16(sPurReqDocEntry.Tables[0].Rows[0][0].ToString()), SAPbobsCOM.BoObjectTypes.oPurchaseQuotations
                                                        , oDICompany, sConnString, sErrDesc) != "SUCCESS")

                                    throw new ArgumentException(sErrDesc);

                                else
                                    //if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);

                                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS", sFuncName);
                                //oDICompany.Disconnect();
                                //oDICompany = null;
                                return "SUCCESS";

                            }
                            else
                            {
                                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Create_PurchaseQuotation()", sFuncName);

                                if (Create_PurchaseQuotation(oDSRequestData, oDICompany, IsDraft, sConnString, sErrDesc) != "SUCCESS") throw new ArgumentException(sErrDesc);
                                //if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                                //oDICompany.Disconnect();
                                //oDICompany = null;
                                return "SUCCESS";
                            }
                        }
                    }

                }

                else if (oDTLine.Rows[0]["SupplierCode"].ToString().ToUpper() == "VA-HARCT")//VA-HARCT
                {

                    if (PurchaseRequest(oDSRequestData, IsDraft, oDICompany, IsAdd, bIsSubmit, bIsDelCharge, sConnString, sErrDesc) != "SUCCESS")
                        throw new ArgumentException(sErrDesc);
                }


                else
                    if (PurchaseRequest(oDSRequestData, IsDraft, oDICompany, IsAdd, bIsSubmit, bIsDelCharge, sConnString, sErrDesc) != "SUCCESS")
                        throw new ArgumentException(sErrDesc);


                //if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);
                //oDICompany.Disconnect();
                //oDICompany = null;
                return "SUCCESS";

            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                //if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                //oDICompany.Disconnect();
                //oDICompany = null;
                throw Ex;

            }
        }

        public string PurchaseRequest(DataSet oDataSet, Boolean IsDraft, SAPbobsCOM.Company oDICompany
                                            , Boolean IsAdd, Boolean bIsSubmit, Boolean bIsDelCharge, string sConnString, string sErrDesc)
        {
            string sFuncName = string.Empty;
            long lRetCode;
            DataTable oDTHeader;
            DataTable oDTLine;
            DateTime dtDocDate;
            string sWhsCode;
            int iLineCount;


            clsCommon oGetSingleValue = new clsCommon();
            DataSet sPurReqDocEntry = new DataSet();
            clsCopyMarketingDocument oCopyDocument = new clsCopyMarketingDocument();


            try
            {
                sFuncName = "PurchaseRequest()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                oDTHeader = oDataSet.Tables[0];
                oDTLine = oDataSet.Tables[1];

                SAPbobsCOM.Documents oPurRequest;

                string[] sPostingDate = oDTHeader.Rows[0]["PostingDate"].ToString().Split(' ');
                dtDocDate = Convert.ToDateTime(sPostingDate[0]);
                string[] sDelDate = oDTHeader.Rows[0]["DeliveryDate"].ToString().Split(' ');
                sWhsCode = oDTHeader.Rows[0]["Outlet"].ToString();


                if (IsDraft == true)
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_SingleValue()", sFuncName);


                    sPurReqDocEntry = oGetSingleValue.Get_SingleValue("select Count(T1.DocEntry) [Count] ,(select COUNT(DocEntry) from DRF1 with (nolock) where DocEntry =T0.DocEntry )[TotCount],T0.DocEntry from ODRF T0 with (nolock) " +
                                        " INNER JOIN DRF1 T1 with (nolock) ON T0.DocEntry =T1.DocEntry where T0.ObjType ='1470000113' and ISNULL (T0.CANCELED ,'N')<>'Y' and T0.DocStatus ='O' " +
                                        " AND T1.U_ApprovalLevel IS NULL AND T1.WhsCode ='" + oDTHeader.Rows[0]["Outlet"].ToString() + "' group by T0.DocEntry", sConnString, sErrDesc);

                    if (sPurReqDocEntry != null && sPurReqDocEntry.Tables[0].Rows.Count > 0)
                    {
                        if (sPurReqDocEntry.Tables[0].Rows[0][0].ToString() == sPurReqDocEntry.Tables[0].Rows[0][1].ToString())
                        {
                            if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Update_PRDraft()", sFuncName);

                            if (Update_PRDraft(oDataSet, oDICompany, sPurReqDocEntry.Tables[0].Rows[0][2].ToString(), sWhsCode
                                , sConnString, IsAdd, bIsSubmit, bIsDelCharge, sErrDesc) != "SUCCESS") throw new ArgumentException(sErrDesc);

                            //if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                            return "SUCCESS";
                        }
                    }

                    oPurRequest = ((SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts)));
                    oPurRequest.DocObjectCode = SAPbobsCOM.BoObjectTypes.oPurchaseRequest;
                }

                else
                {
                    oPurRequest = ((SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseRequest)));
                }

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Creating New Document", sFuncName);

                oPurRequest.ReqType = 12;
                oPurRequest.Requester = oDTHeader.Rows[0]["UserCode"].ToString();//.Trim();
                oPurRequest.RequriedDate = dtDocDate;


                oPurRequest.DocDate = dtDocDate;
                oPurRequest.DocDueDate = Convert.ToDateTime(sDelDate[0]);
                oPurRequest.TaxDate = dtDocDate;

                oPurRequest.DocumentsOwner = Convert.ToInt16(oDTHeader.Rows[0]["UserId"]);
                if (oDTHeader.Rows[0]["Urgent"].ToString().ToUpper() == "Y")
                    oPurRequest.UserFields.Fields.Item("U_Urgent").Value = "Y";

                else if (oDTHeader.Rows[0]["Urgent"].ToString().ToUpper() == "N")
                    oPurRequest.UserFields.Fields.Item("U_Urgent").Value = "N";

                oPurRequest.UserFields.Fields.Item("U_OrderTime").Value = oDTHeader.Rows[0]["OrderTime"].ToString();

                iLineCount = oDTLine.Rows.Count;

                for (int iRow = 0; iRow < iLineCount; iRow++)
                {
                    //if (string.IsNullOrEmpty(oDTLine.Rows[iRow]["OrderQuantity"].ToString()) && Convert.ToDecimal(oDTLine.Rows[iRow]["OrderQuantity"].ToString()) == 0) continue;

                    if (string.IsNullOrEmpty(oDTLine.Rows[iRow]["OrderQuantity"].ToString()) == true) continue;
                    if (Convert.ToDecimal(oDTLine.Rows[iRow]["OrderQuantity"].ToString()) == 0) continue;

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Before Supplier Code : " + oDTLine.Rows[iRow]["SupplierCode"].ToString() + " Row No : " + iRow, sFuncName);

                    oPurRequest.Lines.ItemCode = oDTLine.Rows[iRow]["ItemCode"].ToString();
                    oPurRequest.Lines.LineVendor = oDTLine.Rows[iRow]["SupplierCode"].ToString();

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("After Supplier Code : " + oDTLine.Rows[iRow]["SupplierCode"].ToString() + " Row No : " + iRow, sFuncName);
                    string[] sLineDelDate = oDTLine.Rows[iRow]["DeliveryDate"].ToString().Split(' ');
                    oPurRequest.Lines.RequiredDate = Convert.ToDateTime(sLineDelDate[0]);
                    oPurRequest.Lines.Quantity = Convert.ToDouble(oDTLine.Rows[iRow]["OrderQuantity"].ToString());
                    oPurRequest.Lines.UnitPrice = Convert.ToDouble(oDTLine.Rows[iRow]["Price"].ToString());
                    oPurRequest.Lines.LineTotal = Convert.ToDouble(oDTLine.Rows[iRow]["Total"].ToString());
                    oPurRequest.Lines.WarehouseCode = sWhsCode;

                    string sCostCenter = oGetSingleValue.GetSingleValue("select U_CostCenter [CostCenter] from OWHS where WhsCode ='" + sWhsCode + "'", sConnString, sErrDesc);

                    oPurRequest.Lines.COGSCostingCode = sCostCenter;
                    oPurRequest.Lines.CostingCode = sCostCenter;
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("After getting & assigning the costing code from warehouse master : " + sWhsCode, sFuncName);

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed Line Supplier Code : " + oDTLine.Rows[iRow]["SupplierCode"].ToString() + " Row No : " + iRow, sFuncName);

                    oPurRequest.Lines.Add();

                }

                lRetCode = oPurRequest.Add();
                if (lRetCode != 0)
                {
                    sErrDesc = oDICompany.GetLastErrorDescription(); throw new ArgumentException(sErrDesc);

                }
                else
                {
                    // if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS", sFuncName);
                    return "SUCCESS";
                }

            }

            catch (Exception Ex)
            {

                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                // if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;

            }
        }

        public string Update_PRDraft(DataSet oDataset, SAPbobsCOM.Company oDICompany, string sDraftKey
                                        , string sWhsCode, string sConnString, Boolean IsAdd, Boolean bIsSubmit, Boolean bIsDelCharge, string sErrDesc)
        {

            string sFuncName = string.Empty;
            long lRetCode;
            DataTable oDTLine = new DataTable();
            DataView oDVLine = new DataView();
            int iLineCount;
            string sItemCode = string.Empty;
            string iLineNum = string.Empty;
            clsCommon oGetSingleValue = new clsCommon();
            double dTotFinalQty;
            DataTable oDTDistinct = new DataTable();
            double dOrderQuantity;
            //Int32 iApprovalLevel;
            clsCopyMarketingDocument oDeleteRow = new clsCopyMarketingDocument();
            //string sDraftDocEntry;

            try
            {
                sFuncName = "Update_PRDraft()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                SAPbobsCOM.Documents oPRDraft;
                oPRDraft = ((SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts)));

                oDTLine = oDataset.Tables[1];
                oDVLine = oDataset.Tables[1].DefaultView;

                oPRDraft.GetByKey(Convert.ToInt16(sDraftKey));

                //if (bIsSubmit == true)
                //{
                if (oDVLine.Count == 1)
                {
                    if (oDVLine[0]["OrderQuantity"].ToString() == "0" || string.IsNullOrEmpty(oDTLine.Rows[0]["OrderQuantity"].ToString()) == true)
                    {
                        lRetCode = oPRDraft.Remove();

                        if (lRetCode != 0)
                        {
                            sErrDesc = oDICompany.GetLastErrorDescription();
                            return sErrDesc;

                        }
                        else
                        {
                            return "SUCCESS";

                        }
                    }

                }
                //}
                oDVLine.RowFilter = null;


                iLineCount = oDTLine.Rows.Count;


                for (int iRow = 0; iRow < iLineCount; iRow++)
                {

                    if (string.IsNullOrEmpty(oDTLine.Rows[iRow]["OrderQuantity"].ToString()) == true) continue;


                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting the Line Number:  " + iRow, sFuncName);

                    sItemCode = oDTLine.Rows[iRow]["ItemCode"].ToString();

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling GetSingleValue() for Getting LineCount " + iRow, sFuncName);

                    iLineNum = oGetSingleValue.GetSingleValue("select VisOrder from DRF1 with (nolock) where DocEntry='" + sDraftKey + "' and ItemCode='" + sItemCode + "'", sConnString, sErrDesc);
                    dTotFinalQty = 0;

                    if (iLineNum != "-1")
                    {
                        oPRDraft.Lines.SetCurrentLine(Convert.ToInt32(iLineNum));
                        dOrderQuantity = oPRDraft.Lines.Quantity;

                        oDVLine.RowFilter = "ItemCode='" + sItemCode + "' and SupplierCode='" + oDTLine.Rows[iRow]["SupplierCode"].ToString() + "'";

                        for (int iRCount = 0; iRCount < oDVLine.Count; iRCount++)
                        {
                            dTotFinalQty += Convert.ToDouble(oDVLine[iRCount]["OrderQuantity"].ToString());
                        }

                        if (IsAdd == true)
                            dTotFinalQty += dOrderQuantity;

                        oDVLine.RowFilter = null;
                        //dTotQuantity = oPRDraft.Lines.Quantity;
                        //dTotOrderQty = Convert.ToDouble(oDTLine.Rows[iRow]["OrderQuantity"].ToString());

                    }
                    else
                    {
                        dTotFinalQty = Convert.ToDouble(oDTLine.Rows[iRow]["OrderQuantity"].ToString());
                        oPRDraft.Lines.Add();
                        //dTotOrderQty = Convert.ToDouble(oDTLine.Rows[iRow]["OrderQuantity"].ToString());
                        //dTotQuantity = 0; [RecommendedQuantity]
                    }

                    oPRDraft.Lines.ItemCode = sItemCode;

                    oPRDraft.Lines.Quantity = dTotFinalQty;// (dTotQuantity - dTotOrderQty);
                    oPRDraft.Lines.UnitPrice = Convert.ToDouble(oDTLine.Rows[iRow]["Price"].ToString());
                    oPRDraft.Lines.LineTotal = Convert.ToDouble(oDTLine.Rows[iRow]["Total"].ToString());
                    string[] sDelDate = oDTLine.Rows[iRow]["DeliveryDate"].ToString().Split(' ');
                    oPRDraft.Lines.RequiredDate = Convert.ToDateTime(sDelDate[0]);
                    //oPRDraft.Lines.VatGroup = "TX7";
                    oPRDraft.Lines.WarehouseCode = sWhsCode;

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Before getting & assigning the costing code from warehouse master : " + sWhsCode, sFuncName);
                    string sCostCenter = oGetSingleValue.GetSingleValue("select U_CostCenter [CostCenter] from OWHS where WhsCode ='" + sWhsCode + "'", sConnString, sErrDesc);

                    oPRDraft.Lines.COGSCostingCode = sCostCenter;
                    oPRDraft.Lines.CostingCode = sCostCenter;
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("After getting & assigning the costing code from warehouse master : " + sWhsCode, sFuncName);

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Ended Line Number : " + iRow, sFuncName);
                    //}
                }


                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Before Update the Header ", sFuncName);

                lRetCode = oPRDraft.Update();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("After Update the Header ", sFuncName);

                if (lRetCode != 0)
                {
                    sErrDesc = oDICompany.GetLastErrorDescription(); throw new ArgumentException(sErrDesc);
                }
                else
                {

                    if (oDeleteRow.DeleteZeroQuantity(oPRDraft, sDraftKey, oDICompany, sErrDesc) != "SUCCESS")
                        throw new ArgumentException(sErrDesc);


                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName);
                    return "SUCCESS";
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

        public string Create_PurchaseQuotation(DataSet oDataSet, SAPbobsCOM.Company oDICompany, Boolean IsDraft,string sConnString, string sErrDesc)
        {
            clsCommon oGetSingleValue = new clsCommon();
            string sFuncName = string.Empty;
            long lRetCode;
            DataTable oDTHeader;
            DataTable oDTLine;
            DateTime dtDocDate;
            string sWhsCode;
            int iLineCount;

            try
            {

                sFuncName = "Create_PurchaseQuotation()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);
                SAPbobsCOM.Documents oPurQuotation;


                oDTHeader = oDataSet.Tables[0];
                oDTLine = oDataSet.Tables[1];


                if (IsDraft == true)
                {
                    oPurQuotation = ((SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts)));
                    oPurQuotation.DocObjectCode = SAPbobsCOM.BoObjectTypes.oPurchaseQuotations;
                }
                else
                {
                    oPurQuotation = ((SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseQuotations)));
                }

                string[] sPostingDate = oDTHeader.Rows[0]["PostingDate"].ToString().Split(' ');
                dtDocDate = Convert.ToDateTime(sPostingDate[0]);
                string[] sDelDate = oDTHeader.Rows[0]["DeliveryDate"].ToString().Split(' ');
                sWhsCode = oDTHeader.Rows[0]["Outlet"].ToString();

                oPurQuotation.CardCode = oDTLine.Rows[0]["SupplierCode"].ToString();
                oPurQuotation.DocDate = dtDocDate;
                oPurQuotation.TaxDate = dtDocDate;
                oPurQuotation.RequriedDate = Convert.ToDateTime(sDelDate[0]);
                oPurQuotation.DocDueDate = Convert.ToDateTime(sDelDate[0]);
                oPurQuotation.DocumentsOwner = Convert.ToInt16(oDTHeader.Rows[0]["UserId"]);
                oPurQuotation.UserFields.Fields.Item("U_Urgent").Value = oDTHeader.Rows[0]["Urgent"].ToString();
                oPurQuotation.UserFields.Fields.Item("U_OrderTime").Value = oDTHeader.Rows[0]["OrderTime"].ToString();
                oPurQuotation.Comments = "No Supporting Purchase Request";

                iLineCount = oDTLine.Rows.Count;
                for (int iRow = 0; iRow < iLineCount; iRow++)
                {
                    if (string.IsNullOrEmpty(oDTLine.Rows[iRow]["OrderQuantity"].ToString()) == true) continue;
                    if (Convert.ToDecimal(oDTLine.Rows[iRow]["OrderQuantity"].ToString()) == 0) continue;
                    //if (string.IsNullOrEmpty(oDTLine.Rows[iRow]["OrderQuantity"].ToString()) && Convert.ToDecimal(oDTLine.Rows[iRow]["OrderQuantity"].ToString()) == 0) continue;

                    //oPurQuotation.Lines.LineVendor = oDTLine.Rows[iRow]["SupplierCode"].ToString();
                    oPurQuotation.Lines.ItemCode = oDTLine.Rows[iRow]["ItemCode"].ToString();
                    oPurQuotation.Lines.Quantity = Convert.ToDouble(oDTLine.Rows[iRow]["OrderQuantity"].ToString());
                    oPurQuotation.Lines.UnitPrice = Convert.ToDouble(oDTLine.Rows[iRow]["Price"].ToString());
                    oPurQuotation.Lines.LineTotal = Convert.ToDouble(oDTLine.Rows[iRow]["Total"].ToString());
                    oPurQuotation.Lines.VatGroup = "TX7";
                    oPurQuotation.Lines.WarehouseCode = sWhsCode;

                    string sCostCenter = oGetSingleValue.GetSingleValue("select U_CostCenter [CostCenter] from OWHS where WhsCode ='" + sWhsCode + "'", sConnString, sErrDesc);

                    oPurQuotation.Lines.COGSCostingCode = sCostCenter;
                    oPurQuotation.Lines.CostingCode = sCostCenter;
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("After getting & assigning the costing code from warehouse master : " + sWhsCode, sFuncName);


                    //oPurQuotation.Lines.CostingCode = sWhsCode;
                    //oPurQuotation.Lines.COGSCostingCode = sWhsCode;
                    string[] sLineDelDate = oDTLine.Rows[iRow]["DeliveryDate"].ToString().Split(' ');
                    oPurQuotation.Lines.RequiredDate = Convert.ToDateTime(sLineDelDate[0]);
                    oPurQuotation.Lines.UserFields.Fields.Item("U_MR_OIC_Rmks").Value = oDTLine.Rows[iRow]["Remarks"].ToString();
                    oPurQuotation.Lines.UserFields.Fields.Item("U_MR_App_Rmks").Value = oDTLine.Rows[iRow]["ApproverRemarks"].ToString();
                    oPurQuotation.Lines.Add();

                }

                lRetCode = oPurQuotation.Add();

                if (lRetCode != 0)
                {
                    sErrDesc = oDICompany.GetLastErrorDescription(); throw new ArgumentException(sErrDesc);
                }

                else
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName);
                    return "SUCCESS";
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

        public string PR_DraftSubmit(DataSet oDataset, SAPbobsCOM.Company oCompany, string sConnString, string ErrDesc)
        {
            string sFuncName = string.Empty;
            string sDocEntry = string.Empty;
            clsCommon oGetSingleValue = new clsCommon();
            DataTable oDTHeader = new DataTable();
            DataTable oDTLine = new DataTable();
            SAPbobsCOM.Documents oPRDraft;
            Int32 iApprovalLevel;
            string sItemCode = string.Empty;
            string iLineNum = string.Empty;
            long lRetCode;
            string sReturnMsg = string.Empty;
            clsCopyMarketingDocument oDelEmptyRow = new clsCopyMarketingDocument();
            clsPurchaseOrder oPurchaseOrder = new clsPurchaseOrder();
            SAPbobsCOM.Company oDICompany = oCompany;

            try
            {
                sFuncName = "PR_DraftSubmit";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                oDTHeader = oDataset.Tables[0];
                oDTLine = oDataset.Tables[1];

                oPRDraft = (SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts));

                sDocEntry = oDTHeader.Rows[0]["DocEntry"].ToString();

                // if (oDICompany.InTransaction == false) oDICompany.StartTransaction();

                oPRDraft.GetByKey(Convert.ToInt32(sDocEntry));

                for (int iRow = 0; iRow < oDTLine.Rows.Count; iRow++)
                {

                    sItemCode = oDTLine.Rows[iRow]["ItemCode"].ToString();


                    iLineNum = oGetSingleValue.GetSingleValue("select VisOrder from DRF1 with (nolock) where DocEntry='" + sDocEntry + "' and ItemCode='" + sItemCode + "'", sConnString, sErrDesc);

                    if (iLineNum != "-1")
                    {
                        oPRDraft.Lines.SetCurrentLine(Convert.ToInt32(iLineNum));

                        oPRDraft.Lines.UserFields.Fields.Item("U_DeliveryCharge").Value = oDTLine.Rows[iRow]["DelChargeUDF"].ToString();

                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling GetSingleValue() for Fetching Tolerance Level" + iRow + " " + sDocEntry, sFuncName);

                        iApprovalLevel = Convert.ToInt32(oGetSingleValue.GetSingleValue("select dbo.Web_getToleranceLevel('" + sItemCode + "'," + Convert.ToDouble(oDTLine.Rows[iRow]["RecommendedQuantity"].ToString()) + "," + oDTLine.Rows[iRow]["OrderQuantity"].ToString() + ")", sConnString, sErrDesc));

                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("iApprovalLevel " + iApprovalLevel + " " + sDocEntry, sFuncName);

                        oPRDraft.Lines.UserFields.Fields.Item("U_DeliveryCharge").Value = oDTLine.Rows[iRow]["DelChargeUDF"].ToString();
                        oPRDraft.Lines.UserFields.Fields.Item("U_MR_OIC_Rmks").Value = oDTLine.Rows[iRow]["Remarks"].ToString();
                        oPRDraft.Lines.UserFields.Fields.Item("U_MR_App_Rmks").Value = oDTLine.Rows[iRow]["ApproverRemarks"].ToString();

                        if (iApprovalLevel == 0)
                            oPRDraft.Lines.UserFields.Fields.Item("U_ApprovalLevel").Value = 0;

                        else if (iApprovalLevel == 1)
                        {
                            oPRDraft.Lines.UserFields.Fields.Item("U_ApprovalLevel").Value = 1;
                            oPRDraft.Lines.UserFields.Fields.Item("U_L1ApprovalStatus").Value = "Pending";
                        }
                        else if (iApprovalLevel == 2)
                        {
                            oPRDraft.Lines.UserFields.Fields.Item("U_ApprovalLevel").Value = 2;
                            oPRDraft.Lines.UserFields.Fields.Item("U_L1ApprovalStatus").Value = "Pending";
                            oPRDraft.Lines.UserFields.Fields.Item("U_L2ApprovalStatus").Value = "Pending";
                        }

                        else
                        {
                            oPRDraft.Lines.UserFields.Fields.Item("U_ApprovalLevel").Value = 0;
                        }
                    }

                }

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting the SAP Transaction " + sDocEntry, sFuncName);

                if (oDICompany.InTransaction == false) oDICompany.StartTransaction();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Before PRDraft Update " + sDocEntry, sFuncName);
                lRetCode = oPRDraft.Update();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("After PRDraft Update " + sDocEntry, sFuncName);

                if (lRetCode != 0)
                {
                    sErrDesc = oDICompany.GetLastErrorDescription();
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Here is the Issue ", sFuncName);
                    return sErrDesc;
                    //throw new ArgumentException(sErrDesc);
                }
                else
                {


                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling DeleteZeroQuantity() to Delete the Zero Quantity Line Items ", sFuncName);
                    if (oDelEmptyRow.DeleteZeroQuantity(oPRDraft, sDocEntry, oDICompany, sErrDesc) != "SUCCESS") throw new ArgumentException(sErrDesc);

                    Int32 iPendingResult;
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling GetSingleValue() for Getting Approval Status ", sFuncName);
                    iPendingResult = Convert.ToInt32(oGetSingleValue.GetSingleValue("Exec AE_SP021_CheckingPendingApproval '" + sDocEntry + "'", sConnString, sErrDesc));

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
                        string sResult = oPurchaseOrder.ConvertDraftToDocument(oPRDraft, sDocEntry, oDICompany, sConnString, sErrDesc);
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Result " + sResult, sFuncName);
                        if (sResult != "SUCCESS")
                        {
                            if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("After Calling ConvertDraftToDocument(), The Error message is " + sResult, sFuncName);
                            return sResult;
                        }
                    }

                }

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Committed the SAP Transation " + sDocEntry, sFuncName);
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

                if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Rollback the Transaction " + sDocEntry, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                //oDICompany.Disconnect();
                //oDICompany = null;
                return sErrDesc;

            }
        }

    }
}
