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
    public class clsPurchaseRequest
    {
        clsLog oLog = new clsLog();

        public Int16 p_iDebugMode = DEBUG_ON;

        public const Int16 RTN_SUCCESS = 1;
        public const Int16 RTN_ERROR = 0;
        public const Int16 DEBUG_ON = 1;
        public const Int16 DEBUG_OFF = 0;

        public string sErrDesc = string.Empty;

        string sPRDocNum = string.Empty;
        string sReturnMsg = string.Empty;

        public SqlTransaction oSQLTran;
        SqlConnection oSQLConnection = new SqlConnection();
        SqlCommand oSQLCommand = new SqlCommand();
        SAPbobsCOM.Company oDICompany = new SAPbobsCOM.Company();


        public string Insert_PurchaseRequest(DataSet oDSRequestData, Boolean IsDraft, SAPbobsCOM.Company oDICompany
                                          , Boolean IsAdd, Boolean bIsSubmit, Boolean bIsDelCharge, string sConnString,
                                            string sINTConnString, string sCompanyDB, string sErrDesc)
        {
            string sFuncName = String.Empty;
            SqlConnection oSQLConnection = new SqlConnection();
            DataTable oDTHeader = new DataTable();
            DataTable oDTDetails = new DataTable();
            string P_sConString = string.Empty;
            string sQuery = string.Empty;
            string sDocEntry = string.Empty;

            DateTime dtDocDate;
            DateTime dtDueDate;
            string sWhsCode;
            string sRequester;

            clsCommon oGetSingleValue = new clsCommon();
            DataSet sPurReqDocEntry = new DataSet();
            bool bDocExist = false;
            decimal dDocTotal = 0;


            try
            {
                sFuncName = "Insert_PurchaseRequest()";
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                oDTHeader = oDSRequestData.Tables[0];
                oDTDetails = oDSRequestData.Tables[1];

                // Declare an object variable.
                object sumTotal;
                sumTotal = oDTDetails.Compute("Sum(Total)", "");

                dDocTotal = Convert.ToDecimal(sumTotal.ToString());


                // dDocTotal = (decimal)oDTDetails.Compute("Sum(Total)", "");

                string[] sPostingDate = oDTHeader.Rows[0]["PostingDate"].ToString().Split(' ');
                string[] sDeliveryDate = oDTHeader.Rows[0]["DeliveryDate"].ToString().Split(' ');
                dtDocDate = Convert.ToDateTime(sPostingDate[0]);
                dtDueDate = Convert.ToDateTime(sDeliveryDate[0]);
                sWhsCode = oDTHeader.Rows[0]["Outlet"].ToString();
                sRequester = oDTHeader.Rows[0]["UserCode"].ToString().Trim();

                sQuery = "select Count(T1.DocEntry) [Count] ,(select COUNT(DocEntry) from DRF1 with (nolock) where DocEntry =T0.DocEntry )[TotCount] " +
                    " ,T0.DocEntry from ODRF T0 with (nolock) INNER JOIN DRF1 T1 with (nolock) ON T0.DocEntry =T1.DocEntry where  T0.DocStatus ='O' " +
                    " AND T1.U_ApprovalLevel IS NULL AND T1.WhsCode ='" + sWhsCode + "' and T0.[DocType] ='PR' group by T0.DocEntry  ";

                sPurReqDocEntry = oGetSingleValue.Get_SingleValue(sQuery, sINTConnString, sErrDesc);

                if (sPurReqDocEntry != null && sPurReqDocEntry.Tables[0].Rows.Count > 0)
                {
                    if (sPurReqDocEntry.Tables[0].Rows[0][0].ToString() == sPurReqDocEntry.Tables[0].Rows[0][1].ToString())
                    {
                        bDocExist = true;
                        sDocEntry = sPurReqDocEntry.Tables[0].Rows[0][2].ToString();

                    }
                    else
                    {
                        sQuery = "select cast(COUNT(DocEntry) as Int) +1 [Count] from ODRF ";

                        sPurReqDocEntry = oGetSingleValue.Get_SingleValue(sQuery, sINTConnString, sErrDesc);

                        sDocEntry = sPurReqDocEntry.Tables[0].Rows[0][0].ToString().Trim();
                    }
                }
                else  //Getting the Last Document Number
                {
                    sQuery = "select cast(COUNT(DocEntry) as Int) +1 [Count] from ODRF ";

                    sPurReqDocEntry = oGetSingleValue.Get_SingleValue(sQuery, sINTConnString, sErrDesc);

                    sDocEntry = sPurReqDocEntry.Tables[0].Rows[0][0].ToString().Trim();
                }


                oSQLConnection = new SqlConnection(sINTConnString);
                if (oSQLConnection.State == ConnectionState.Closed)
                    oSQLConnection.Open();

                oSQLTran = oSQLConnection.BeginTransaction();

                if (bDocExist == false)
                {

                    sQuery = "INSERT INTO ODRF( [DocEntry],[ReqType],[DocType],[Requester],[WhsCode],[ReqDate],[DocDate],[DocDueDate] " +
                        " ,[TaxDate],[DocStatus] ,[DocOwner] ,[CompanyDB] ,[U_Urgent] ,[DocTotal] ,[CANCELED] ";

                    sQuery += ") VALUES (";

                    sQuery += "@DocEntry,@ReqType,@DocType,@Requester,@WhsCode,@ReqDate,@DocDate,@DocDueDate,@TaxDate,@DocStatus,@DocOwner, " +
                        "@CompanyDB,@U_Urgent,@DocTotal,@CANCELED )";

                    oSQLCommand = new SqlCommand();
                    oSQLCommand.Connection = oSQLConnection;
                    oSQLCommand.CommandText = sQuery;
                    oSQLCommand.Transaction = oSQLTran;
                    oSQLCommand.CommandTimeout = 0;

                    oSQLCommand.Parameters.Add("@DocEntry", SqlDbType.Int).Value = sDocEntry;
                    oSQLCommand.Parameters.Add("@ReqType", SqlDbType.NVarChar).Value = 12;
                    oSQLCommand.Parameters.Add("@DocType", SqlDbType.NVarChar).Value = "PR";
                    oSQLCommand.Parameters.Add("@Requester", SqlDbType.NVarChar).Value = sRequester;
                    oSQLCommand.Parameters.Add("@WhsCode", SqlDbType.NVarChar).Value = sWhsCode;
                    oSQLCommand.Parameters.Add("@ReqDate", SqlDbType.Date).Value = dtDueDate;
                    oSQLCommand.Parameters.Add("@DocDate", SqlDbType.Date).Value = dtDocDate;
                    oSQLCommand.Parameters.Add("@DocDueDate", SqlDbType.Date).Value = dtDueDate;
                    oSQLCommand.Parameters.Add("@TaxDate", SqlDbType.Date).Value = dtDocDate;
                    oSQLCommand.Parameters.Add("@DocStatus", SqlDbType.Char).Value = "O";
                    oSQLCommand.Parameters.Add("@DocOwner", SqlDbType.NVarChar).Value = Convert.ToInt16(oDTHeader.Rows[0]["UserId"]);
                    oSQLCommand.Parameters.Add("@CompanyDB", SqlDbType.NVarChar).Value = sCompanyDB;
                    oSQLCommand.Parameters.Add("@U_Urgent", SqlDbType.Char).Value = oDTHeader.Rows[0]["Urgent"].ToString();
                    oSQLCommand.Parameters.Add("@DocTotal", SqlDbType.Decimal).Value = dDocTotal;
                    oSQLCommand.Parameters.Add("@CANCELED", SqlDbType.Char).Value = "N";
                    // TimeSpan ts = TimeSpan.Parse(oDTHeader.Rows[0]["OrderTime"].ToString());

                    // oSQLCommand.Parameters.Add("@U_OrderTime", SqlDbType.Time).Value = DateTime.Today.TimeOfDay;

                    oSQLCommand.ExecuteNonQuery();
                }

                for (int iDetailCount = 0; iDetailCount < oDTDetails.Rows.Count; iDetailCount++)
                {

                    if (string.IsNullOrEmpty(oDTDetails.Rows[iDetailCount]["OrderQuantity"].ToString()) == true) continue;
                    if (Convert.ToDecimal(oDTDetails.Rows[iDetailCount]["OrderQuantity"].ToString()) == 0) continue;

                    sQuery = "INSERT INTO DRF1( [DocEntry],[LineId],[ItemCode] ,[ItemName] ,[LineVendor]  ,[ReqDate] ,[WhsCode]  ,[Quantity] ,[UnitPrice] ,[LineTotal] " +
                    " ,[RcmdQty],[OnHand] ,[MinSpend],[InStock],[EventOrder] ,[Last7Days],[AlreadyOrdered] ,[MinLevel] ,[UOM] , [U_MR_OIC_Rmks], [U_MR_App_Rmks],[ItemPerUnit]";


                    sQuery += ") VALUES (";

                    sQuery += "@DocEntry,@LineId,@ItemCode,@ItemName,@LineVendor,@ReqDate,@WhsCode,@Quantity,@UnitPrice,@LineTotal,@RcmdQty, " +
                    " @OnHand,@MinSpend,@InStock,@EventOrder,@Last7Days,@AlreadyOrdered,@MinLevel,@UOM, @U_MR_OIC_Rmks,@U_MR_App_Rmks,@ItemPerUnit);";

                    oSQLCommand = new SqlCommand();

                    if (oSQLConnection.State == ConnectionState.Closed)
                        oSQLConnection.Open();

                    oSQLCommand.Connection = oSQLConnection;
                    oSQLCommand.CommandText = sQuery;
                    oSQLCommand.Transaction = oSQLTran;
                    oSQLCommand.CommandTimeout = 0;


                    oSQLCommand.Parameters.Add("@DocEntry", SqlDbType.Int).Value = sDocEntry;
                    oSQLCommand.Parameters.Add("@LineId", SqlDbType.Int).Value = iDetailCount + 1;
                    oSQLCommand.Parameters.Add("@ItemCode", SqlDbType.NVarChar).Value = oDTDetails.Rows[iDetailCount]["ItemCode"].ToString();
                    oSQLCommand.Parameters.Add("@ItemName", SqlDbType.NVarChar).Value = oDTDetails.Rows[iDetailCount]["Description"].ToString();
                    oSQLCommand.Parameters.Add("@LineVendor", SqlDbType.NVarChar).Value = oDTDetails.Rows[iDetailCount]["SupplierCode"].ToString();
                    oSQLCommand.Parameters.Add("@ReqDate", SqlDbType.Date).Value = dtDueDate;
                    oSQLCommand.Parameters.Add("@WhsCode", SqlDbType.NVarChar).Value = sWhsCode;
                    oSQLCommand.Parameters.Add("@Quantity", SqlDbType.Decimal).Value = oDTDetails.Rows[iDetailCount]["OrderQuantity"].ToString();
                    oSQLCommand.Parameters.Add("@UnitPrice", SqlDbType.Decimal).Value = oDTDetails.Rows[iDetailCount]["Price"].ToString();
                    oSQLCommand.Parameters.Add("@LineTotal", SqlDbType.Decimal).Value = oDTDetails.Rows[iDetailCount]["Total"].ToString();
                    oSQLCommand.Parameters.Add("@RcmdQty", SqlDbType.Decimal).Value = oDTDetails.Rows[iDetailCount]["RecommendedQuantity"].ToString();
                    oSQLCommand.Parameters.Add("@OnHand", SqlDbType.Decimal).Value = oDTDetails.Rows[iDetailCount]["InStock"].ToString();
                    oSQLCommand.Parameters.Add("@MinSpend", SqlDbType.Decimal).Value = oDTDetails.Rows[iDetailCount]["MinSpend"].ToString();
                    oSQLCommand.Parameters.Add("@InStock", SqlDbType.Decimal).Value = oDTDetails.Rows[iDetailCount]["InStock"].ToString();
                    oSQLCommand.Parameters.Add("@EventOrder", SqlDbType.Decimal).Value = oDTDetails.Rows[iDetailCount]["EventOrder"].ToString();
                    oSQLCommand.Parameters.Add("@Last7Days", SqlDbType.NVarChar).Value = oDTDetails.Rows[iDetailCount]["Last7DaysAvg"].ToString();
                    oSQLCommand.Parameters.Add("@AlreadyOrdered", SqlDbType.Decimal).Value = oDTDetails.Rows[iDetailCount]["AlreadyOrdered"].ToString();

                    if (oDTDetails.Columns["MinStock"] != null)
                        oSQLCommand.Parameters.Add("@MinLevel", SqlDbType.Decimal).Value = oDTDetails.Rows[iDetailCount]["MinStock"].ToString();
                    else
                        oSQLCommand.Parameters.Add("@MinLevel", SqlDbType.Decimal).Value = oDTDetails.Rows[iDetailCount]["MinLevel"].ToString();

                    oSQLCommand.Parameters.Add("@UOM", SqlDbType.NVarChar).Value = oDTDetails.Rows[iDetailCount]["UOM"].ToString();
                    oSQLCommand.Parameters.Add("@U_MR_OIC_Rmks", SqlDbType.NVarChar).Value = oDTDetails.Rows[iDetailCount]["Remarks"].ToString();
                    oSQLCommand.Parameters.Add("@U_MR_App_Rmks", SqlDbType.NVarChar).Value = oDTDetails.Rows[iDetailCount]["ApproverRemarks"].ToString();
                    oSQLCommand.Parameters.Add("@ItemPerUnit", SqlDbType.Decimal).Value = Convert.ToDecimal(oDTDetails.Rows[iDetailCount]["ItemPerUnit"].ToString().Trim());


                    oSQLCommand.ExecuteNonQuery();

                }

                oSQLTran.Commit();

                oSQLConnection.Dispose();
                oSQLCommand.Dispose();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);
                return "SUCCESS";

            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);

                oSQLTran.Rollback();
                oSQLConnection.Dispose();
                oSQLCommand.Dispose();

                throw Ex;
            }

        }

        public string Update_PurchaseRequest(DataSet oDSRequestData, Boolean IsDraft, SAPbobsCOM.Company oDICompany
                                  , Boolean IsAdd, Boolean bIsSubmit, Boolean bIsDelCharge, string sConnString
                                    , string sINTConnString, string sCompanyDB, string sErrDesc)
        {
            string sFuncName = String.Empty;
            SqlConnection oSQLConnection = new SqlConnection();
            DataTable oDTHeader = new DataTable();
            DataTable oDTDetails = new DataTable();
            string P_sConString = string.Empty;
            string sQuery = string.Empty;
            string sDocEntry = string.Empty;

            DateTime dtDocDate;
            DateTime dtDueDate;
            string sWhsCode;
            string sRequester;

            clsCommon oGetSingleValue = new clsCommon();
            DataSet sPurReqDocEntry = new DataSet();
            DataView oDVDetails = new DataView();
            double dTotQuantity = 0;
            string sItemCode = string.Empty;
            DataView oDVLine = new DataView();


            decimal dDocTotal = 0;

            try
            {
                sFuncName = "Update_PurchaseRequest()";
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                oDTHeader = oDSRequestData.Tables[0];
                oDTDetails = oDSRequestData.Tables[1];

                //DVDetails = oDTDetails.DefaultView;

                object sumTotal;
                sumTotal = oDTDetails.Compute("Sum(Total)", "");

                dDocTotal = Convert.ToDecimal(sumTotal.ToString());

                string[] sDocDate = oDTHeader.Rows[0]["PostingDate"].ToString().Split(' ');
                string[] sDocDueDate = oDTHeader.Rows[0]["DeliveryDate"].ToString().Split(' ');
                dtDocDate = Convert.ToDateTime(sDocDate[0]);
                dtDueDate = Convert.ToDateTime(sDocDueDate[0]);

                sWhsCode = oDTHeader.Rows[0]["Outlet"].ToString();

                sDocEntry = oDTHeader.Rows[0]["DocEntry"].ToString().Trim();
                sRequester = oDTHeader.Rows[0]["UserCode"].ToString().Trim();

                oSQLConnection = new SqlConnection(sINTConnString);
                if (oSQLConnection.State == ConnectionState.Closed)
                    oSQLConnection.Open();

                oSQLTran = oSQLConnection.BeginTransaction();


                //Delete the Existing Records:
                sQuery = "DELETE FROM ODRF WHERE DocEntry='" + sDocEntry + "' ; DELETE FROM DRF1 WHERE DocEntry='" + sDocEntry + "' ; ";

                oSQLCommand = new SqlCommand();
                oSQLCommand.Connection = oSQLConnection;
                oSQLCommand.CommandText = sQuery;
                oSQLCommand.Transaction = oSQLTran;
                oSQLCommand.CommandTimeout = 180;

                oSQLCommand.ExecuteNonQuery();

                oDVLine = oDSRequestData.Tables[1].DefaultView;

                if (oDVLine.Count == 1)
                {
                    if (oDVLine[0]["OrderQuantity"].ToString() == "0" || string.IsNullOrEmpty(oDVLine[0]["OrderQuantity"].ToString()) == true)
                    {

                        oSQLConnection.Dispose();
                        oSQLCommand.Dispose();
                        return "SUCCESS";

                    }

                }

                //Insert the Header Records:


                sQuery = "INSERT INTO ODRF( [DocEntry],[ReqType],[DocType],[Requester],[WhsCode],[ReqDate],[DocDate],[DocDueDate] " +
                " ,[TaxDate],[DocStatus] ,[DocOwner] ,[CompanyDB] ,[U_Urgent],[DocTotal],[CANCELED]  ";

                sQuery += ") VALUES (";

                sQuery += "@DocEntry,@ReqType,@DocType,@Requester,@WhsCode,@ReqDate,@DocDate,@DocDueDate,@TaxDate,@DocStatus,@DocOwner, " +
                    "@CompanyDB,@U_Urgent,@DocTotal, @CANCELED )";


                // oSQLCommand = new SqlCommand();
                oSQLCommand.Connection = oSQLConnection;
                oSQLCommand.CommandText = sQuery;
                oSQLCommand.Transaction = oSQLTran;
                oSQLCommand.CommandTimeout = 180;

                oSQLCommand.Parameters.Add("@DocEntry", SqlDbType.Int).Value = sDocEntry;
                oSQLCommand.Parameters.Add("@ReqType", SqlDbType.NVarChar).Value = 12;
                oSQLCommand.Parameters.Add("@DocType", SqlDbType.NVarChar).Value = "PR";
                oSQLCommand.Parameters.Add("@Requester", SqlDbType.NVarChar).Value = sRequester;
                oSQLCommand.Parameters.Add("@WhsCode", SqlDbType.NVarChar).Value = sWhsCode;
                oSQLCommand.Parameters.Add("@ReqDate", SqlDbType.Date).Value = dtDueDate;
                oSQLCommand.Parameters.Add("@DocDate", SqlDbType.Date).Value = dtDocDate;
                oSQLCommand.Parameters.Add("@DocDueDate", SqlDbType.Date).Value = dtDueDate;
                oSQLCommand.Parameters.Add("@TaxDate", SqlDbType.Date).Value = dtDocDate;
                oSQLCommand.Parameters.Add("@DocStatus", SqlDbType.Char).Value = "O";
                oSQLCommand.Parameters.Add("@DocOwner", SqlDbType.NVarChar).Value = Convert.ToInt16(oDTHeader.Rows[0]["UserId"]);
                oSQLCommand.Parameters.Add("@CompanyDB", SqlDbType.NVarChar).Value = sCompanyDB;
                oSQLCommand.Parameters.Add("@U_Urgent", SqlDbType.Char).Value = oDTHeader.Rows[0]["Urgent"].ToString();
                oSQLCommand.Parameters.Add("@DocTotal", SqlDbType.Decimal).Value = dDocTotal;
                oSQLCommand.Parameters.Add("@CANCELED", SqlDbType.Char).Value = "N";
                //TimeSpan ts;
                //oSQLCommand.Parameters.Add("@U_OrderTime", SqlDbType.Time).Value = TimeSpan.TryParse(oDTHeader.Rows[0]["OrderTime"].ToString(), out ts);

                // TimeSpan ts = TimeSpan.Parse(oDTHeader.Rows[0]["OrderTime"].ToString());
                // oSQLCommand.Parameters.Add("@U_OrderTime", SqlDbType.Time).Value = DateTime.Today.TimeOfDay;


                oSQLCommand.ExecuteNonQuery();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Header Inserted ", sFuncName);

                //Insert the Line Records:
                for (int iDetailCount = 0; iDetailCount < oDTDetails.Rows.Count; iDetailCount++)
                {

                    if (string.IsNullOrEmpty(oDTDetails.Rows[iDetailCount]["OrderQuantity"].ToString()) == true) continue;
                    if (Convert.ToDecimal(oDTDetails.Rows[iDetailCount]["OrderQuantity"].ToString()) == 0) continue;

                    oDVDetails.RowFilter = "ItemCode='" + sItemCode + "' and SupplierCode='" + oDTDetails.Rows[iDetailCount]["SupplierCode"].ToString() + "'";

                    for (int iRCount = 0; iRCount < oDVDetails.Count; iRCount++)
                    {
                        dTotQuantity += Convert.ToDouble(oDVDetails[iRCount]["OrderQuantity"].ToString());
                    }

                    sQuery = "INSERT INTO DRF1( [DocEntry],[LineId] ,[ItemCode] ,[ItemName] ,[LineVendor]  ,[ReqDate] ,[WhsCode]  ,[Quantity] ,[UnitPrice] ,[LineTotal] " +
                    " ,[RcmdQty],[OnHand] ,[MinSpend],[InStock],[EventOrder] ,[Last7Days],[AlreadyOrdered] ,[MinLevel],[UOM], [U_MR_OIC_Rmks] ,[U_MR_App_Rmks],[ItemPerUnit] ";


                    sQuery += ") VALUES (";

                    sQuery += "@DocEntry,@LineId,@ItemCode,@ItemName,@LineVendor,@ReqDate,@WhsCode,@Quantity,@UnitPrice,@LineTotal,@RcmdQty, " +
                    " @OnHand,@MinSpend,@InStock,@EventOrder,@Last7Days,@AlreadyOrdered,@MinLevel,@UOM, @U_MR_OIC_Rmks,@U_MR_App_Rmks,@ItemPerUnit );";


                    oSQLCommand = new SqlCommand();

                    if (oSQLConnection.State == ConnectionState.Closed)
                        oSQLConnection.Open();

                    oSQLCommand.Connection = oSQLConnection;
                    oSQLCommand.CommandText = sQuery;
                    oSQLCommand.Transaction = oSQLTran;
                    oSQLCommand.CommandTimeout = 180;


                    oSQLCommand.Parameters.Add("@DocEntry", SqlDbType.Int).Value = sDocEntry;
                    oSQLCommand.Parameters.Add("@LineId", SqlDbType.Int).Value = iDetailCount + 1;
                    oSQLCommand.Parameters.Add("@ItemCode", SqlDbType.NVarChar).Value = oDTDetails.Rows[iDetailCount]["ItemCode"].ToString();
                    oSQLCommand.Parameters.Add("@ItemName", SqlDbType.NVarChar).Value = oDTDetails.Rows[iDetailCount]["Description"].ToString();
                    oSQLCommand.Parameters.Add("@LineVendor", SqlDbType.NVarChar).Value = oDTDetails.Rows[iDetailCount]["SupplierCode"].ToString();
                    oSQLCommand.Parameters.Add("@ReqDate", SqlDbType.Date).Value = dtDueDate;
                    oSQLCommand.Parameters.Add("@WhsCode", SqlDbType.NVarChar).Value = sWhsCode;
                    oSQLCommand.Parameters.Add("@Quantity", SqlDbType.Decimal).Value = oDTDetails.Rows[iDetailCount]["OrderQuantity"].ToString();
                    oSQLCommand.Parameters.Add("@UnitPrice", SqlDbType.Decimal).Value = oDTDetails.Rows[iDetailCount]["Price"].ToString();
                    oSQLCommand.Parameters.Add("@LineTotal", SqlDbType.Decimal).Value = oDTDetails.Rows[iDetailCount]["Total"].ToString();
                    oSQLCommand.Parameters.Add("@RcmdQty", SqlDbType.Decimal).Value = oDTDetails.Rows[iDetailCount]["RecommendedQuantity"].ToString();
                    oSQLCommand.Parameters.Add("@OnHand", SqlDbType.Decimal).Value = oDTDetails.Rows[iDetailCount]["InStock"].ToString();
                    // oSQLCommand.Parameters.Add("@IsCommitted", SqlDbType.NVarChar).Value = oDTDetails.Rows[iDetailCount]["IsCommited"].ToString();
                    oSQLCommand.Parameters.Add("@MinSpend", SqlDbType.Decimal).Value = oDTDetails.Rows[iDetailCount]["MinSpend"].ToString();
                    oSQLCommand.Parameters.Add("@InStock", SqlDbType.Decimal).Value = oDTDetails.Rows[iDetailCount]["Instock"].ToString();
                    oSQLCommand.Parameters.Add("@EventOrder", SqlDbType.Decimal).Value = oDTDetails.Rows[iDetailCount]["EventOrder"].ToString();
                    oSQLCommand.Parameters.Add("@Last7Days", SqlDbType.NVarChar).Value = oDTDetails.Rows[iDetailCount]["Last7DaysAvg"].ToString();
                    oSQLCommand.Parameters.Add("@AlreadyOrdered", SqlDbType.Decimal).Value = oDTDetails.Rows[iDetailCount]["AlreadyOrdered"].ToString();

                    if (oDTDetails.Columns["MinStock"] != null)
                        oSQLCommand.Parameters.Add("@MinLevel", SqlDbType.Decimal).Value = oDTDetails.Rows[iDetailCount]["MinStock"].ToString();
                    else
                        oSQLCommand.Parameters.Add("@MinLevel", SqlDbType.Decimal).Value = oDTDetails.Rows[iDetailCount]["MinLevel"].ToString();

                    oSQLCommand.Parameters.Add("@UOM", SqlDbType.NVarChar).Value = oDTDetails.Rows[iDetailCount]["UOM"].ToString();
                    oSQLCommand.Parameters.Add("@U_MR_OIC_Rmks", SqlDbType.NVarChar).Value = oDTDetails.Rows[iDetailCount]["Remarks"].ToString();
                    oSQLCommand.Parameters.Add("@U_MR_App_Rmks", SqlDbType.NVarChar).Value = oDTDetails.Rows[iDetailCount]["ApproverRemarks"].ToString();
                    oSQLCommand.Parameters.Add("@ItemPerUnit", SqlDbType.Decimal).Value = Convert.ToDecimal(oDTDetails.Rows[iDetailCount]["ItemPerUnit"].ToString().Trim());
                    oSQLCommand.ExecuteNonQuery();
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Line " + iDetailCount + " Inserted ", sFuncName);
                }
                //Commit the Transaction
                oSQLTran.Commit();



                //Close the SQL Connections
                oSQLConnection.Dispose();
                oSQLCommand.Dispose();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);
                return "SUCCESS";

            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);

                oSQLTran.Rollback();
                oSQLConnection.Dispose();
                oSQLCommand.Dispose();

                throw Ex;
            }

        }

        public string Submit_PurchaseRequest(DataSet oDataset, string sCompanyDB, string sConnString,
                                            string sINTConString, string ErrDesc)
        {

            string sFuncName = string.Empty;

            string sReturnMsg = string.Empty;
            string sDocEntry = string.Empty;
            DataTable oDTHeader = new DataTable();
            DataTable oDTLine = new DataTable();
            string sItemCode = string.Empty;
            string iLineNum = string.Empty;
            Int32 iApprovalLevel;
            string sLineVendor = string.Empty;
            Int32 iLineID;
            clsCommon oGetSingleValue = new clsCommon();
            clsPurchaseOrder oPurchaseOrder = new clsPurchaseOrder();
            string sQuery = string.Empty;

            string sCardCode = string.Empty;
            string sSYS1DocEntry = string.Empty;
            string sSYS2DocEntry = string.Empty;
            string sPRDocEntry = string.Empty;
            string sPRDocNum = string.Empty;
            string sPRDraftKey = string.Empty;

            string sSYS1Series = string.Empty;
            string sSYS2Series = string.Empty;
            DataSet oDSSeries = new DataSet();


            try
            {
                sFuncName = "Submit_PurchaseRequest";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);
                oDTHeader = oDataset.Tables[0];
                oDTLine = oDataset.Tables[1];


                sDocEntry = oDTHeader.Rows[0]["DocEntry"].ToString();

                for (int iRow = 0; iRow < oDTLine.Rows.Count; iRow++)
                {
                    sItemCode = oDTLine.Rows[iRow]["ItemCode"].ToString().Trim();
                    sLineVendor = oDTLine.Rows[iRow]["SupplierCode"].ToString().Trim();
                    iLineID = Convert.ToInt32(oDTLine.Rows[iRow]["LineId"].ToString().Trim());

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling GetSingleValue() for Fetching Tolerance Level" + iRow + " " + sDocEntry, sFuncName);
                    iApprovalLevel = Convert.ToInt32(oGetSingleValue.GetSingleValue("select dbo.Web_getToleranceLevel('" + sItemCode + "'," + Convert.ToDouble(oDTLine.Rows[iRow]["RecommendedQuantity"].ToString()) + "," + oDTLine.Rows[iRow]["OrderQuantity"].ToString() + ")", sConnString, sErrDesc));
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("iApprovalLevel " + iApprovalLevel + " " + sDocEntry, sFuncName);

                    if (iApprovalLevel == 0)
                        sQuery += "update DRF1 set U_ApprovalLevel ='0',U_L1ApprovalStatus =NULL,U_L2ApprovalStatus =NULL, U_MR_App_Rmks ='" + oDTLine.Rows[iRow]["ApproverRemarks"].ToString() + "', " +
                            " U_DeliveryCharge ='" + oDTLine.Rows[iRow]["DelChargeUDF"].ToString() + "',U_MR_OIC_Rmks ='" + oDTLine.Rows[iRow]["Remarks"].ToString() + "' where ItemCode ='" + sItemCode + "' and " +
                            " LineVendor ='" + sLineVendor + "' and DocEntry ='" + sDocEntry + "' and LineId='" + iLineID + "'";
                    else if (iApprovalLevel == 1)
                    {
                        sQuery += "update DRF1 set U_ApprovalLevel ='1',U_L1ApprovalStatus ='Pending',U_L2ApprovalStatus =NULL, U_MR_App_Rmks ='" + oDTLine.Rows[iRow][""].ToString() + "',  " +
                           " U_DeliveryCharge ='" + oDTLine.Rows[iRow]["DelChargeUDF"].ToString() + "',U_MR_OIC_Rmks ='" + oDTLine.Rows[iRow]["Remarks"].ToString() + "' where ItemCode ='" + sItemCode + "' and " +
                           " LineVendor ='" + sLineVendor + "' and DocEntry ='" + sDocEntry + "' and LineId='" + iLineID + "'";
                    }
                    else if (iApprovalLevel == 2)
                    {
                        sQuery += "update DRF1 set U_ApprovalLevel ='2',U_L1ApprovalStatus ='Pending',U_L2ApprovalStatus ='Pending', U_MR_App_Rmks ='" + oDTLine.Rows[iRow]["ApproverRemarks"].ToString() + "'," +
                        " U_DeliveryCharge ='" + oDTLine.Rows[iRow]["DelChargeUDF"].ToString() + "',U_MR_OIC_Rmks ='" + oDTLine.Rows[iRow]["Remarks"].ToString() + "' where ItemCode ='" + sItemCode + "' and " +
                        " LineVendor ='" + sLineVendor + "' and DocEntry ='" + sDocEntry + "' and LineId='" + iLineID + "'";
                    }
                    else
                    {
                        sQuery += "update DRF1 set U_ApprovalLevel ='0',U_L1ApprovalStatus =NULL,U_L2ApprovalStatus =NULL, U_MR_App_Rmks ='" + oDTLine.Rows[iRow]["ApproverRemarks"].ToString() + "', " +
                           " U_DeliveryCharge ='" + oDTLine.Rows[iRow]["DelChargeUDF"].ToString() + "' ,U_MR_App_Rmks ='" + oDTLine.Rows[iRow]["Remarks"].ToString() + "' where ItemCode ='" + sItemCode + "' and " +
                           " LineVendor ='" + sLineVendor + "' and DocEntry ='" + sDocEntry + "' and LineId='" + iLineID + "'";
                    }

                }

                oSQLConnection = new SqlConnection(sINTConString);
                if (oSQLConnection.State == ConnectionState.Closed)
                    oSQLConnection.Open();

                oSQLTran = oSQLConnection.BeginTransaction();

                oSQLCommand.Connection = oSQLConnection;
                oSQLCommand.CommandText = sQuery;
                oSQLCommand.Transaction = oSQLTran;
                oSQLCommand.CommandTimeout = 0;

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("PR Submit Query :  " + sQuery, sFuncName);

                oSQLCommand.ExecuteNonQuery();


                //if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Delete_ZeroQuantity() for Deleting the Zero Quantity Lines", sFuncName);
                //if (Delete_ZeroQuantity(sDocEntry, sINTConString, sErrDesc) != "SUCCESS") throw new ArgumentException(sErrDesc);

                Int32 iPendingResult;
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling GetSingleValue() for Getting Approval Status ", sFuncName);
                iPendingResult = Convert.ToInt32(oGetSingleValue.GetSingleValue("Exec AE_SP021_CheckingPendingApproval '" + sDocEntry + "'", sINTConString, sErrDesc));

                if (iPendingResult != 0)
                {
                    sErrDesc = " Approval is Pending. Ref Number : " + sDocEntry;
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug(sErrDesc, sFuncName);
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName);

                    sReturnMsg += sErrDesc;

                }
                else
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling ConnectToTargetCompany() ", sFuncName);
                    if (ConnectToTargetCompany(sCompanyDB, sConnString, sErrDesc) != "SUCCESS") throw new ArgumentException(sErrDesc);

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Create_PurchaseRequest() ", sFuncName);

                    if (Create_PurchaseRequest(oDICompany, sDocEntry, sConnString, sINTConString, sErrDesc) != "SUCCESS") throw new ArgumentException(sErrDesc);

                    oDICompany.GetNewObjectCode(out sPRDraftKey);


                    sQuery = "select DocNum from OPRQ with (nolock) where DocEntry='" + sPRDraftKey + "'";

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("calling GetSingleValue() for Getting PR Document Number", sFuncName);
                    sPRDocNum = oGetSingleValue.GetSingleValue(sQuery, sConnString, sErrDesc);


                    sQuery = "select Top 1 LineVendor from PRQ1 T0 with (nolock) " +
                         " WHERE T0.DocEntry='" + sPRDraftKey + "' AND LineVendor ='VA-HARCT'";

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("calling GetSingleValue() for Getting PR Customer Code ", sFuncName);
                    sCardCode = oGetSingleValue.GetSingleValue(sQuery, sConnString, sErrDesc);


                    if (sCardCode.ToString().ToUpper() == "VA-HARCT")
                    {
                        oDICompany.GetNewObjectCode(out sPRDocEntry);

                        //if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("SQL statement " + " " + "select DocNum from OPRQ with (nolock) where DocEntry ='" + sPRDocEntry + "'", sFuncName);
                        //sPRDocNum = oGetSingleValue.GetSingleValue("select DocNum from OPRQ with (nolock) where DocEntry ='" + sPRDocEntry + "'", sConnString, sErrDesc);

                        sQuery = "select Series from NNM1 WITH (NOLOCK) where ObjectCode ='1250000001' and SeriesName in('SYS1','SYS2')";

                        oDSSeries = oGetSingleValue.Get_SingleValue(sQuery, sConnString, sErrDesc);

                        sSYS1Series = oDSSeries.Tables[0].Rows[0][0].ToString().Trim();
                        sSYS2Series = oDSSeries.Tables[0].Rows[1][0].ToString().Trim();

                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Create_InventoryTransferRequest_SYS1() ", sFuncName);
                        if (Create_InventoryTransferRequest_SYS1(oDICompany, sDocEntry, sPRDocEntry, sSYS1Series, sPRDocNum, sConnString, sErrDesc) != "SUCCESS") throw new ArgumentException(sErrDesc);
                        oDICompany.GetNewObjectCode(out sSYS1DocEntry);

                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Create_InventoryTransferRequest_SYS2() ", sFuncName);
                        if (Create_InventoryTransferRequest_SYS2(oDICompany, sDocEntry, sPRDocEntry, sSYS2Series, sPRDocNum, sConnString, sErrDesc) != "SUCCESS") throw new ArgumentException(sErrDesc);
                        oDICompany.GetNewObjectCode(out sSYS2DocEntry);

                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Update_InvTransRequestNumber() ", sFuncName);
                        if (Update_InvTransRequestNumber(oDICompany, sDocEntry, sPRDocEntry, sSYS1DocEntry, sSYS2DocEntry, sConnString, sErrDesc) != "SUCCESS")
                            throw new ArgumentException(sErrDesc);

                    }

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Update_Status() ", sFuncName);
                    if (Update_Status(sDocEntry, sPRDraftKey, sPRDocNum, sINTConString, sErrDesc) != "SUCCESS") throw new ArgumentException(sErrDesc);
                }

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                if (sReturnMsg.ToString() == string.Empty)
                    sReturnMsg = "SUCCESS";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Committed the SQL Transaction", sFuncName);
                oSQLTran.Commit();
                oSQLConnection.Dispose();
                oSQLCommand.Dispose();

                return sReturnMsg;

            }

            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                oSQLTran.Rollback();
                oSQLConnection.Dispose();
                oSQLCommand.Dispose();
                return sErrDesc;
            }
        }


        public string Approve_PurchaseRequest(DataSet oDataset, string sCompanyDB, string sConnString,
                                                string sINTConString, string sErrDesc)
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

            clsCommon oCommon = new clsCommon();
            clsCopyMarketingDocument oDelEmptyRow = new clsCopyMarketingDocument();
            string sUserName = string.Empty;
            string sUserRole = string.Empty;


            string sQuery = string.Empty;

            string sCardCode = string.Empty;
            string sSYS1DocEntry = string.Empty;
            string sSYS2DocEntry = string.Empty;
            string sPRDocEntry = string.Empty;
            string sPRDocNum = string.Empty;
            clsCommon oGetSingleValue = new clsCommon();

            string sSYS1Series = string.Empty;
            string sSYS2Series = string.Empty;
            DataSet oDSSeries = new DataSet();
            //Boolean bIsTransaction = false;
            Int32 iLineID;

            try
            {

                sFuncName = "Approve_PurchaseRequest()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);


                oDVHeader = oDataset.Tables[0].DefaultView;
                oDVDetail = oDataset.Tables[1].DefaultView;
                sReturnMsg = string.Empty;

                //oSQLCommand = new SqlCommand();
                //oSQLConnection = new SqlConnection(sINTConString);
                //if (oSQLConnection.State == ConnectionState.Closed)
                //    oSQLConnection.Open();

                oDTDistinct = oDVHeader.Table.DefaultView.ToTable(true, "DocEntry");


                for (int IntHeader = 0; IntHeader <= oDTDistinct.Rows.Count - 1; IntHeader++)
                {

                    sDocEntry = oDVHeader[IntHeader]["DocEntry"].ToString();
                    // sRejected = oDVHeader[IntHeader]["Rejected"].ToString();
                    sApproved = oDVHeader[IntHeader]["Approved"].ToString();
                    sDeliveryCharge = oDVHeader[IntHeader]["DeliveryCharge"].ToString();

                    sUserName = oDVHeader[IntHeader]["UserName"].ToString();
                    sUserRole = oDVHeader[IntHeader]["UserRole"].ToString();


                    oDVDetail.RowFilter = "DocEntry = '" + sDocEntry + "'";

                    sQuery = string.Empty;

                    for (int IntLine = 0; IntLine <= oDVDetail.Count - 1; IntLine++)
                    {
                        iLineID = Convert.ToInt32(oDVDetail[IntLine]["LineNum"].ToString().Trim());


                        if (Convert.ToInt16(sUserRole) == 1)
                        {
                            if (sApproved.ToUpper() == "Y")
                            {

                                sQuery += "update DRF1 set Quantity =" + Convert.ToDouble(oDVDetail[IntLine]["OrderQuantity"].ToString().Trim()) + ", U_DeliveryCharge ='" + oDVDetail[IntLine]["DelChargeUDF"].ToString().Trim() + "', U_MR_App_Rmks ='" + oDVDetail[IntLine]["ApproverRemarks"].ToString().Trim() + "', U_MR_OIC_Rmks ='" + oDVDetail[IntLine]["Remarks"].ToString().Trim() + "', " +
                                    " U_L1ApprovalStatus ='Approved',LineTotal =" + Convert.ToDouble(oDVDetail[IntLine]["Total"].ToString().Trim()) + ",U_L1Approver ='" + sUserName + "' where LineId='" + iLineID + "' AND DocEntry ='" + sDocEntry + "' and LineVendor ='" + oDVDetail[IntLine]["SupplierCode"].ToString().Trim() + "' and ItemCode ='" + oDVDetail[IntLine]["ItemCode"].ToString().Trim() + "'";

                            }

                            else
                            {
                                sQuery += "update DRF1 set Quantity =" + Convert.ToDouble(oDVDetail[IntLine]["OrderQuantity"].ToString().Trim()) + ", U_DeliveryCharge ='" + oDVDetail[IntLine]["DelChargeUDF"].ToString().Trim() + "', U_MR_App_Rmks ='" + oDVDetail[IntLine]["ApproverRemarks"].ToString().Trim() + "', U_MR_OIC_Rmks ='" + oDVDetail[IntLine]["Remarks"].ToString().Trim() + "', " +
                                   " U_L1ApprovalStatus ='Rejected',LineTotal =" + Convert.ToDouble(oDVDetail[IntLine]["Total"].ToString().Trim()) + ",U_L1Approver ='" + sUserName + "' where LineId='" + iLineID + "' AND DocEntry ='" + sDocEntry + "' and LineVendor ='" + oDVDetail[IntLine]["SupplierCode"].ToString().Trim() + "' and ItemCode ='" + oDVDetail[IntLine]["ItemCode"].ToString().Trim() + "'";

                            }
                        }

                        else if (Convert.ToInt16(sUserRole) == 2)
                        {
                            if (sApproved.ToUpper() == "Y")
                            {
                                sQuery += "update DRF1 set Quantity =" + Convert.ToDouble(oDVDetail[IntLine]["OrderQuantity"].ToString().Trim()) + ", U_DeliveryCharge ='" + oDVDetail[IntLine]["DelChargeUDF"].ToString().Trim() + "', U_MR_App_Rmks ='" + oDVDetail[IntLine]["ApproverRemarks"].ToString().Trim() + "', U_MR_OIC_Rmks ='" + oDVDetail[IntLine]["Remarks"].ToString().Trim() + "', " +
                                 " U_L2ApprovalStatus ='Approved',LineTotal =" + Convert.ToDouble(oDVDetail[IntLine]["Total"].ToString().Trim()) + ",U_L2Approver ='" + sUserName + "' where LineId='" + iLineID + "' AND DocEntry ='" + sDocEntry + "' and LineVendor ='" + oDVDetail[IntLine]["SupplierCode"].ToString().Trim() + "' and ItemCode ='" + oDVDetail[IntLine]["ItemCode"].ToString().Trim() + "'";

                            }
                            else
                            {
                                sQuery += "update DRF1 set Quantity =" + Convert.ToDouble(oDVDetail[IntLine]["OrderQuantity"].ToString().Trim()) + ", U_DeliveryCharge ='" + oDVDetail[IntLine]["DelChargeUDF"].ToString().Trim() + "', U_MR_App_Rmks ='" + oDVDetail[IntLine]["ApproverRemarks"].ToString().Trim() + "', U_MR_OIC_Rmks ='" + oDVDetail[IntLine]["Remarks"].ToString().Trim() + "', " +
                          " U_L2ApprovalStatus ='Rejected',LineTotal =" + Convert.ToDouble(oDVDetail[IntLine]["Total"].ToString().Trim()) + ",U_L2Approver ='" + sUserName + "' where LineId='" + iLineID + "' AND DocEntry ='" + sDocEntry + "' and LineVendor ='" + oDVDetail[IntLine]["SupplierCode"].ToString().Trim() + "' and ItemCode ='" + oDVDetail[IntLine]["ItemCode"].ToString().Trim() + "'";

                            }

                        }

                    }

                    //if (bIsTransaction == false)
                    //{
                    //    oSQLTran = oSQLConnection.BeginTransaction();
                    //    bIsTransaction = true;
                    //}

                    oSQLCommand = new SqlCommand();
                    oSQLConnection = new SqlConnection(sINTConString);
                    if (oSQLConnection.State == ConnectionState.Closed)
                        oSQLConnection.Open();

                    oSQLTran = oSQLConnection.BeginTransaction();

                    oSQLCommand.Connection = oSQLConnection;
                    oSQLCommand.CommandText = sQuery;
                    oSQLCommand.Transaction = oSQLTran;
                    oSQLCommand.CommandTimeout = 0;

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Approval Query :  " + sQuery, sFuncName);

                    oSQLCommand.ExecuteNonQuery();

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Delete_ZeroQuantity() for Deleting the Zero Quantity Lines", sFuncName);
                    if (Delete_ZeroQuantity(sDocEntry, sINTConString, sErrDesc) != "SUCCESS") throw new ArgumentException(sErrDesc);

                    Int32 iPendingResult;
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling GetSingleValue() for Getting Approval Status ", sFuncName);
                    iPendingResult = Convert.ToInt32(oCommon.GetSingleValue("Exec AE_SP021_CheckingPendingApproval '" + sDocEntry + "'", sINTConString, sErrDesc));

                    if (iPendingResult != 0)
                    {
                        sErrDesc = " Approval is Pending. Ref Number : " + sDocEntry;
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug(sErrDesc, sFuncName);
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName);

                        sReturnMsg += sErrDesc;

                    }
                    else
                    {
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling ConnectToTargetCompany() ", sFuncName);

                        if (oDICompany.Connected == false)
                            if (ConnectToTargetCompany(sCompanyDB, sConnString, sErrDesc) != "SUCCESS") throw new ArgumentException(sErrDesc);

                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Create_PurchaseRequest() ", sFuncName);

                        if (oDICompany.InTransaction == false) oDICompany.StartTransaction();

                        sErrDesc = Create_PurchaseRequest(oDICompany, sDocEntry, sConnString, sINTConString, sErrDesc); //throw new ArgumentException(sErrDesc);
                        if (sErrDesc.ToString().Trim().ToUpper() != "SUCCESS")
                        {
                            sReturnMsg += sErrDesc;
                            if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Rollback the SQL Transactions ", sFuncName);
                            oSQLTran.Rollback();
                            oSQLConnection.Dispose();
                            oSQLCommand.Dispose();
                            if (oDICompany.Connected == true)
                            {
                                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Rollback the Transactions ", sFuncName);
                                if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                            }
                            continue;
                        }

                        oDICompany.GetNewObjectCode(out sPRDocEntry);


                        sQuery = "select DocNum from OPRQ with (nolock) where DocEntry='" + sPRDocEntry + "'";

                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("calling GetSingleValue() for Getting PR Documene Number", sFuncName);
                        sPRDocNum = oGetSingleValue.GetSingleValue(sQuery, sConnString, sErrDesc);



                        sQuery = "select Top 1 LineVendor from PRQ1 T0 with (nolock) " +
                             " WHERE T0.DocEntry='" + sPRDocEntry + "' AND LineVendor ='VA-HARCT'";

                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("calling GetSingleValue() for Getting the Customer Code", sFuncName);
                        sCardCode = oGetSingleValue.GetSingleValue(sQuery, sConnString, sErrDesc);


                        if (sCardCode.ToString().ToUpper() == "VA-HARCT")
                        {

                            //oDICompany.GetNewObjectCode(out sPRDocEntry);

                            //if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("SQL statement " + " " + "select DocNum from OPRQ with (nolock) where DocEntry ='" + sPRDocEntry + "'", sFuncName);
                            //sPRDocNum = oGetSingleValue.GetSingleValue("select DocNum from OPRQ with (nolock) where DocEntry ='" + sPRDocEntry + "'", sConnString, sErrDesc);

                            sQuery = "select Series from NNM1 WITH (NOLOCK) where ObjectCode ='1250000001' and SeriesName in('SYS1','SYS2')";
                            if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("SQL statement " + " " + sQuery, sFuncName);

                            oDSSeries = oGetSingleValue.Get_SingleValue(sQuery, sConnString, sErrDesc);

                            sSYS1Series = oDSSeries.Tables[0].Rows[0][0].ToString().Trim();
                            sSYS2Series = oDSSeries.Tables[0].Rows[1][0].ToString().Trim();

                            if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Create_InventoryTransferRequest_SYS1() ", sFuncName);
                            sErrDesc = Create_InventoryTransferRequest_SYS1(oDICompany, sDocEntry, sPRDocEntry, sSYS1Series, sPRDocNum, sConnString, sErrDesc);     //!= "SUCCESS") throw new ArgumentException(sErrDesc);

                            if (sErrDesc.ToString().Trim().ToUpper() != "SUCCESS")
                            {
                                sReturnMsg += sErrDesc;
                                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Rollback the SQL Transactions ", sFuncName);
                                oSQLTran.Rollback();
                                oSQLConnection.Dispose();
                                oSQLCommand.Dispose();
                                if (oDICompany.Connected == true)
                                {
                                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Rollback the Transactions ", sFuncName);
                                    if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                                }
                                continue;
                            }

                            oDICompany.GetNewObjectCode(out sSYS1DocEntry);

                            if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Create_InventoryTransferRequest_SYS2() ", sFuncName);
                            sErrDesc = Create_InventoryTransferRequest_SYS2(oDICompany, sDocEntry, sPRDocEntry, sSYS2Series, sPRDocNum, sConnString, sErrDesc); //!= "SUCCESS") throw new ArgumentException(sErrDesc);

                            if (sErrDesc.ToString().Trim().ToUpper() != "SUCCESS")
                            {
                                sReturnMsg += sErrDesc;
                                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Rollback the SQL Transactions ", sFuncName);
                                oSQLTran.Rollback();
                                oSQLConnection.Dispose();
                                oSQLCommand.Dispose();

                                if (oDICompany.Connected == true)
                                {
                                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Rollback the Transactions ", sFuncName);
                                    if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                                }
                                continue;
                            }
                            oDICompany.GetNewObjectCode(out sSYS2DocEntry);

                            if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Update_InvTransRequestNumber() ", sFuncName);
                            sErrDesc = Update_InvTransRequestNumber(oDICompany, sDocEntry, sPRDocEntry, sSYS1DocEntry, sSYS2DocEntry, sConnString, sErrDesc);  //!= "SUCCESS") throw new ArgumentException(sErrDesc);

                            if (sErrDesc.ToString().Trim().ToUpper() != "SUCCESS")
                            {
                                sReturnMsg += sErrDesc;
                                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Rollback the SQL Transactions ", sFuncName);
                                oSQLTran.Rollback();
                                oSQLConnection.Dispose();
                                oSQLCommand.Dispose();

                                if (oDICompany.Connected == true)
                                {
                                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Rollback the Transactions ", sFuncName);
                                    if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                                }

                                continue;
                            }
                        }

                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Update_Status() ", sFuncName);
                        sErrDesc = Update_Status(sDocEntry, sPRDocEntry, sPRDocNum, sINTConString, sErrDesc);   //!= "SUCCESS") throw new ArgumentException(sErrDesc);

                        if (sErrDesc.ToString().Trim().ToUpper() != "SUCCESS")
                        {
                            sReturnMsg += sErrDesc;
                            if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Rollback the SQL Transactions ", sFuncName);
                            oSQLTran.Rollback();
                            oSQLConnection.Dispose();
                            oSQLCommand.Dispose();
                            if (oDICompany.Connected == true)
                            {
                                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Rollback the Transactions ", sFuncName);
                                if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                            }

                            continue;
                        }
                    }

                    //Commit the Every Transactions in Staging DB and SAP DB.
                    if (oDICompany.Connected == true)
                    {
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Committed the SAP Transaction ", sFuncName);
                        if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                    }

                    // if (bIsTransaction == true)
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Committed the SQL Transaction", sFuncName);
                    oSQLTran.Commit();

                    oSQLConnection.Dispose();
                    oSQLCommand.Dispose();
                }


                if (sReturnMsg.ToString() == string.Empty)
                    sReturnMsg = "SUCCESS";

                //if (oDICompany.Connected == true)
                //{
                //    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Committed the SAP Transaction ", sFuncName);
                //    if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                //}

                //if (bIsTransaction == true)
                //    oSQLTran.Commit();

                //oSQLConnection.Dispose();
                //oSQLCommand.Dispose();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return sReturnMsg;

            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);

                if (oDICompany.Connected == true)
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Rollback the Transactions ", sFuncName);
                    if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                }

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Rollback the SQL Transactions ", sFuncName);
                //if (bIsTransaction == true)
                oSQLTran.Rollback();

                oSQLConnection.Dispose();
                oSQLCommand.Dispose();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);

                //throw Ex;
                return sErrDesc;
            }
        }


        public string Create_PurchaseRequest(SAPbobsCOM.Company oDICompany, string sDocEntry, string sConnString
                                                , string sINTConnString, string sErrMsg)
        {
            string sFuncName = string.Empty;
            long lRetCode;
            DataTable oDTHeader;
            DataTable oDTLine;
            DateTime dtDocDate;
            DateTime dtDocDueDate;

            string sWhsCode;
            int iLineCount;
            string sRequester;

            clsCommon oGetSingleValue = new clsCommon();
            string sQuery = string.Empty;
            DataSet oDSResult = new DataSet();
            Boolean bContentExist = false;
            DataView oDVLine = new DataView();
            DataTable oDistLineTable = new DataTable();
            double dQuantity = 0.0;
            double dLineTotal = 0.0;

            try
            {
                sFuncName = "Create_PurchaseRequest()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                sQuery = "SELECT * FROM ODRF WITH(NOLOCK) WHERE DOCENTRY ='" + sDocEntry + "';SELECT * FROM DRF1 WITH(NOLOCK) WHERE QUANTITY>0 AND DOCENTRY ='" + sDocEntry + "'";

                oDSResult = oGetSingleValue.Get_SingleValue(sQuery, sINTConnString, sErrDesc);


                oDTHeader = oDSResult.Tables[0];
                oDTLine = oDSResult.Tables[1];

                // Convert data table to dataview.
                oDVLine = new DataView(oDTLine);

                //sQuery = "SELECT * FROM DRF1 WHERE DOCENTRY ='" + sDocEntry + "'";

                //oDSResult = oGetSingleValue.Get_SingleValue(sQuery, sINTConnString, sErrDesc);

                //oDTLine = oDSResult.Tables[0];

                SAPbobsCOM.Documents oPurRequest;

                dtDocDate = Convert.ToDateTime(oDTHeader.Rows[0]["DocDate"].ToString());
                dtDocDueDate = Convert.ToDateTime(oDTHeader.Rows[0]["DocDueDate"].ToString());
                sWhsCode = oDTHeader.Rows[0]["WhsCode"].ToString();
                sRequester = oDTHeader.Rows[0]["Requester"].ToString().Trim();

                oPurRequest = ((SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseRequest)));
                oPurRequest.ReqType = 12;
                oPurRequest.Requester = sRequester;
                oPurRequest.RequriedDate = dtDocDate;


                oPurRequest.DocDate = dtDocDate;
                oPurRequest.DocDueDate = dtDocDueDate;//Convert.ToDateTime(oDTHeader.Rows[0]["DocDueDate"].ToString());
                oPurRequest.TaxDate = dtDocDate;

                oPurRequest.DocumentsOwner = Convert.ToInt16(oDTHeader.Rows[0]["DocOwner"]);
                if (oDTHeader.Rows[0]["U_Urgent"].ToString().ToUpper() == "Y")
                    oPurRequest.UserFields.Fields.Item("U_Urgent").Value = "Y";

                else if (oDTHeader.Rows[0]["U_Urgent"].ToString().ToUpper() == "N")
                    oPurRequest.UserFields.Fields.Item("U_Urgent").Value = "N";

                oPurRequest.UserFields.Fields.Item("U_WEBPRDocNum").Value = sDocEntry;

                iLineCount = oDTLine.Rows.Count;

                oDistLineTable = oDVLine.Table.DefaultView.ToTable(true, "ItemCode");

                for (int iRow = 0; iRow < oDistLineTable.Rows.Count; iRow++)
                // for (int iRow = 0; iRow < iLineCount; iRow++)
                {
                    //if (string.IsNullOrEmpty(oDTLine.Rows[iRow]["OrderQuantity"].ToString()) && Convert.ToDecimal(oDTLine.Rows[iRow]["OrderQuantity"].ToString()) == 0) continue;

                    //if (string.IsNullOrEmpty(oDTLine.Rows[iRow]["Quantity"].ToString()) == true) continue;
                    //if (Convert.ToDecimal(oDTLine.Rows[iRow]["Quantity"].ToString()) == 0) continue;

                    bContentExist = true;
                    dQuantity = 0.0;
                    dLineTotal = 0.0;

                    oDVLine.RowFilter = "ItemCode = '" + oDistLineTable.Rows[iRow][0].ToString() + "'";
                    for (int iItemCount = 0; iItemCount < oDVLine.Count; iItemCount++)
                    {
                        dQuantity += Convert.ToDouble(oDVLine[iItemCount]["Quantity"].ToString().Trim());
                        dLineTotal += Convert.ToDouble(oDVLine[iItemCount]["LineTotal"].ToString().Trim());
                    }

                    oPurRequest.Lines.ItemCode = oDVLine[0]["ItemCode"].ToString().Trim();
                    oPurRequest.Lines.LineVendor = oDVLine[0]["LineVendor"].ToString().Trim();
                    oPurRequest.Lines.RequiredDate = dtDocDueDate;// Convert.ToDateTime(oDTHeader.Rows[0]["DocDueDate"].ToString());
                    oPurRequest.Lines.InventoryQuantity = Convert.ToDouble(dQuantity) * Convert.ToDouble(oDVLine[0]["ItemPerUnit"].ToString().Trim());
                    oPurRequest.Lines.UnitPrice = Convert.ToDouble(oDVLine[0]["UnitPrice"].ToString().Trim());
                    oPurRequest.Lines.UserFields.Fields.Item("U_ApprovalLevel").Value = oDVLine[0]["U_ApprovalLevel"].ToString().Trim();
                    oPurRequest.Lines.UserFields.Fields.Item("U_L1ApprovalStatus").Value = oDVLine[0]["U_L1ApprovalStatus"].ToString().Trim();
                    oPurRequest.Lines.UserFields.Fields.Item("U_L1Approver").Value = oDVLine[0]["U_L1Approver"].ToString().Trim();
                    oPurRequest.Lines.UserFields.Fields.Item("U_L2ApprovalStatus").Value = oDVLine[0]["U_L2ApprovalStatus"].ToString().Trim();
                    oPurRequest.Lines.UserFields.Fields.Item("U_L2Approver").Value = oDVLine[0]["U_L2Approver"].ToString().Trim();
                    oPurRequest.Lines.UserFields.Fields.Item("U_DeliveryCharge").Value = oDVLine[0]["U_DeliveryCharge"].ToString().Trim();
                    oPurRequest.Lines.UserFields.Fields.Item("U_MR_App_Rmks").Value = oDVLine[0]["U_MR_App_Rmks"].ToString().Trim();
                    oPurRequest.Lines.UserFields.Fields.Item("U_MR_OIC_Rmks").Value = oDVLine[0]["U_MR_OIC_Rmks"].ToString().Trim();
                    oPurRequest.Lines.LineTotal = Convert.ToDouble(dLineTotal);

                    oPurRequest.Lines.WarehouseCode = sWhsCode;

                    string sCostCenter = oGetSingleValue.GetSingleValue("select U_CostCenter [CostCenter] from OWHS where WhsCode ='" + sWhsCode + "'", sConnString, sErrDesc);

                    oPurRequest.Lines.COGSCostingCode = sCostCenter;
                    oPurRequest.Lines.CostingCode = sCostCenter;
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("After getting & assigning the costing code from warehouse master : " + sWhsCode, sFuncName);

                    oPurRequest.Lines.Add();
                }

                if (bContentExist == true)
                {
                    lRetCode = oPurRequest.Add();
                    if (lRetCode != 0)
                    {
                        sErrDesc = " PR Number : " + sDocEntry + " Error : " + oDICompany.GetLastErrorDescription(); //throw new ArgumentException(sErrDesc);
                        return sErrDesc;

                    }
                    else
                    {

                        // if (oDICompany.InTransaction == true) oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                        if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS", sFuncName);
                        return "SUCCESS";
                    }
                }
                else
                {
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

        string Create_InventoryTransferRequest_SYS1(SAPbobsCOM.Company oDICompany, string sDraftNumber, string sDocEntry, string sSeries,
                                                    string sPRDocNum, string sConnString, string sErrDesc)
        {
            string sFuncName = string.Empty;
            double lRetCode;
            SAPbobsCOM.Documents oPurchaseRequest;
            SAPbobsCOM.StockTransfer oInvTransRequest;
            string sWhsCode = string.Empty;

            clsCommon oCommon = new clsCommon();

            try
            {
                sFuncName = "Create_InventoryTransferRequest_SYS1()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                oPurchaseRequest = (SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseRequest));
                oInvTransRequest = (SAPbobsCOM.StockTransfer)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest));

                sWhsCode = oCommon.GetSingleValue("select Top 1 WhsCode from PRQ1 with (nolock) where DocEntry ='" + sDocEntry + "'", sConnString, sErrDesc);

                oPurchaseRequest.GetByKey(Convert.ToInt32(sDocEntry));

                oInvTransRequest.CardCode = "VA-HARCT";//oPurchaseOrder.CardCode;
                oInvTransRequest.FromWarehouse = "01CKT";
                oInvTransRequest.ToWarehouse = "01CKT";
                oInvTransRequest.UserFields.Fields.Item("U_PR_No").Value = sPRDocNum;
                oInvTransRequest.UserFields.Fields.Item("U_Outlet").Value = sWhsCode;
                oInvTransRequest.Series = Convert.ToInt32(sSeries);
                oInvTransRequest.DocDate = oPurchaseRequest.DocDate;
                oInvTransRequest.TaxDate = oPurchaseRequest.TaxDate;
                oInvTransRequest.DueDate = oPurchaseRequest.DocDueDate;

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
                    oTargetLines.DistributionRule = oBaseLines.COGSCostingCode;
                    oTargetLines.Add();

                }

                lRetCode = oInvTransRequest.Add();

                if (lRetCode != 0)
                {
                    sErrDesc = " PR Number : " + sDraftNumber + " Error : " + oDICompany.GetLastErrorDescription(); throw new ArgumentException(sErrDesc);
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

        public string Create_InventoryTransferRequest_SYS2(SAPbobsCOM.Company oDICompany, string sDraftNumber, string sDocEntry, string sSeries, string sPRDocNum,
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
                oInvTransRequest.Series = Convert.ToInt32(sSeries);
                oInvTransRequest.DocDate = oPurchaseRequest.DocDate;
                oInvTransRequest.TaxDate = oPurchaseRequest.TaxDate;
                oInvTransRequest.DueDate = oPurchaseRequest.DocDueDate;

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
                    oTargetLines.DistributionRule = oBaseLines.COGSCostingCode;
                    oTargetLines.Add();
                }

                lRetCode = oInvTransRequest.Add();

                if (lRetCode != 0)
                {
                    sErrDesc = " PR Number : " + sDraftNumber + " Error : " + oDICompany.GetLastErrorDescription(); throw new ArgumentException(sErrDesc);
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

        string Update_InvTransRequestNumber(SAPbobsCOM.Company oDICompany, string sDocEntry, string sPRDocEntry, string sSYS1DocEntry
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
                    sErrDesc = " PR Number : " + sDocEntry + " Error : " + oDICompany.GetLastErrorDescription(); throw new ArgumentException(sErrDesc);
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

        public string Create_PurchaseQuotation(DataSet oDataSet, SAPbobsCOM.Company oDICompany, Boolean IsDraft, string sConnString, string sErrDesc)
        {
            clsCommon oCommon = new clsCommon();
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
                //dtDocDate = Convert.ToDateTime(oDTHeader.Rows[0]["PostingDate"].ToString());
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

                    string sCostCenter = oCommon.GetSingleValue("select U_CostCenter [CostCenter] from OWHS where WhsCode ='" + sWhsCode + "'", sConnString, sErrDesc);

                    oPurQuotation.Lines.COGSCostingCode = sCostCenter;
                    oPurQuotation.Lines.CostingCode = sCostCenter;
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("After getting & assigning the costing code from warehouse master : " + sWhsCode, sFuncName);

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

        public string Delete_PurchaseRequest(DataSet oDataSet, string sINTConnString, string sErrDesc)
        {

            string sFuncName = string.Empty;
            DataTable oDTDetails = new DataTable();
            string sDocEntry = string.Empty;
            string sQuery = string.Empty;


            try
            {
                sFuncName = "Delete_PurchaseRequest()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                oDTDetails = oDataSet.Tables[0];

                for (int iRow = 0; iRow < oDTDetails.Rows.Count; iRow++)
                {

                    sDocEntry += "'" + oDTDetails.Rows[iRow][0].ToString().Trim() + "',";

                }

                sDocEntry = sDocEntry.Substring(0, sDocEntry.Length - 1);


                sQuery = "Update ODRF set DocStatus='C',CANCELED='Y' where DocEntry in(" + sDocEntry + ")";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Executing the Query : " + sQuery, sFuncName);

                oSQLConnection = new SqlConnection(sINTConnString);
                if (oSQLConnection.State == ConnectionState.Closed)
                    oSQLConnection.Open();


                oSQLCommand = new SqlCommand();
                oSQLCommand.Connection = oSQLConnection;
                oSQLCommand.CommandText = sQuery;
                oSQLCommand.CommandTimeout = 180;

                oSQLCommand.ExecuteNonQuery();

                oSQLConnection.Dispose();
                oSQLCommand.Dispose();


                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName);

                return "SUCCESS";
            }
            catch (Exception Ex)
            {

                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                oSQLConnection.Dispose();
                oSQLCommand.Dispose();
                //throw Ex;
                return sErrDesc;
            }
        }

        public string Update_Status(string sDocEntry, string sPRDocEntry, string sPRDocNum, string sINTConnString, string sErrDesc)
        {

            string sFuncName = string.Empty;
            string sQuery = string.Empty;

            try
            {
                sFuncName = "Update_Status()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                sQuery = "Update ODRF set DocStatus='C',PRDocEntry='" + sPRDocEntry + "',PRDocNum='" + sPRDocNum + "' where DocEntry ='" + sDocEntry + "'";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Executing the Query : " + sQuery, sFuncName);

                //oSQLConnection = new SqlConnection(sINTConnString);
                //if (oSQLConnection.State == ConnectionState.Closed)
                //    oSQLConnection.Open();


                oSQLCommand.Connection = oSQLConnection;
                oSQLCommand.CommandText = sQuery;
                oSQLCommand.Transaction = oSQLTran;
                oSQLCommand.CommandTimeout = 0;

                oSQLCommand.ExecuteNonQuery();

                //oSQLConnection.Dispose();
                //oSQLCommand.Dispose();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName);

                return "SUCCESS";

            }
            catch (Exception Ex)
            {
                sErrDesc = " PR Number : " + sDocEntry + " Error : " + Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                oSQLConnection.Dispose();
                oSQLCommand.Dispose();
                throw Ex;
            }
        }

        public string Delete_ZeroQuantity(string sDocEntry, string sINTConnString, string sErrDesc)
        {

            string sFuncName = string.Empty;
            string sQuery = string.Empty;

            try
            {
                sFuncName = "Delete_ZeroQuantity()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                sQuery = "DELETE from DRF1 where Quantity=0 and DocEntry ='" + sDocEntry + "'";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Executing the Query : " + sQuery, sFuncName);

                //oSQLConnection = new SqlConnection(sINTConnString);
                //if (oSQLConnection.State == ConnectionState.Closed)
                //    oSQLConnection.Open();


                oSQLCommand.Connection = oSQLConnection;
                oSQLCommand.CommandText = sQuery;
                oSQLCommand.Transaction = oSQLTran;
                oSQLCommand.CommandTimeout = 0;

                oSQLCommand.ExecuteNonQuery();

                //oSQLConnection.Dispose();
                //oSQLCommand.Dispose();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName);

                return "SUCCESS";

            }
            catch (Exception Ex)
            {

                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                oSQLConnection.Dispose();
                oSQLCommand.Dispose();
                throw Ex;
            }
        }

        public DataSet Get_ApprovalStatus_Summary(string sFromDate, string sToDate, string sOutlet, string sConnString, string sErrDesc)
        {
            DataSet oDataset;
            string sFuncName = string.Empty;

            try
            {
                sFuncName = "Get_ApprovalStatus_Summary()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                DataAccess oDataAccess = new DataAccess();

                //DateTime dtfrom = DateTime.Parse("fromDate");

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_StoredProcedure() ", sFuncName);

                oDataset = oDataAccess.Run_QueryString("EXEC getApprovalStatus_Summary '" + sFromDate + "','" + sToDate + "' , '" + sOutlet + "'", sConnString);

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

        public string ConnectToTargetCompany(string sCompanyDB, string sConnString, string sErrDesc)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDTCompanyList = new DataSet();
            DataSet oDSResult = new DataSet();
            //// string sConnString = string.Empty;
            DataView oDTView = new DataView();
            clsCompanyList oCompanyList = new clsCompanyList();

            clsCommon oCommon = new clsCommon();

            try
            {
                sFuncName = "ConnectToTargetCompany()";


                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);
                oDTCompanyList = oCompanyList.Get_CompanyList(sConnString);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Filter Based on Company DB() ", sFuncName);
                oDTView = oDTCompanyList.Tables[0].DefaultView;
                oDTView.RowFilter = "U_DBName= '" + sCompanyDB + "'";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling ConnectToTargetCompany() ", sFuncName);

                oDICompany = oCommon.ConnectToTargetCompany(oDICompany, oDTView[0]["U_SAPUserName"].ToString(), oDTView[0]["U_SAPPassword"].ToString()
                                   , oDTView[0]["U_DBName"].ToString(), oDTView[0]["U_Server"].ToString(), oDTView[0]["U_LicenseServer"].ToString()
                                   , oDTView[0]["U_DBUserName"].ToString(), oDTView[0]["U_DBPassword"].ToString(), sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

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
