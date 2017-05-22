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
    public class clsStockTakeCounting
    {
        clsLog oLog = new clsLog();

        public Int16 p_iDebugMode = DEBUG_ON;

        public const Int16 RTN_SUCCESS = 1;
        public const Int16 RTN_ERROR = 0;
        public const Int16 DEBUG_ON = 1;
        public const Int16 DEBUG_OFF = 0;


        DataAccess oDataAccess = new DataAccess();
        clsCommon oGetSingleValue = new clsCommon();
        public SqlTransaction oSQLTran;
        SqlConnection oSQLConnection = new SqlConnection();
        SqlCommand oSQLCommand = new SqlCommand();

        public DataSet Get_StockTakeCountingList(string sOutlet, string sUserRole, string sConnString, string sErrDesc)
        {
            string sFuncName = string.Empty;
            string sProcedureName = string.Empty;
            DataSet oDataSet = new DataSet();

            try
            {
                sFuncName = "Get_StockTakeCountingList()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);


                sProcedureName = "EXEC [AE_SP020_GetSalesTakeCountList] '" + sOutlet + "','" + sUserRole + "'";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_StoredProcedure()", sFuncName);

                oDataSet = oDataAccess.Run_StoredProcedure(sProcedureName, sConnString);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with SUCCESS", sFuncName);

                return oDataSet;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message;
                oDataSet.Dispose();
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);
                throw Ex;
            }

        }


        public DataSet Get_StockTakeCounting(string sOutlet, string sUserRole, string sStatus, string sDocEntry, string sDocDate
                                            , string sConnString, string sErrDesc)
        {
            string sFuncName = string.Empty;
            string sProcedureName = string.Empty;
            DataSet oDataSet = new DataSet();

            try
            {
                sFuncName = "Get_StockTakeCounting()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);

                sProcedureName = "EXEC [AE_SP018_StockTakeCounting] '" + sOutlet + "','" + sUserRole + "','" + sStatus + "','" + sDocEntry + "','" + sDocDate + "'";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_StoredProcedure()", sFuncName);

                oDataSet = oDataAccess.Run_StoredProcedure(sProcedureName, sConnString);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);

                return oDataSet;
            }
            catch (Exception Ex)
            {

                sErrDesc = Ex.Message;
                oDataSet.Dispose();
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);
                throw Ex;
            }

        }

        public string Create_DeliverOrderDraft(string sUserRole, string sOutlet, string sCompanyDB, string sDocOwner
                                        , string sConnString, string sINTConnString, string sRequester, string sDocDate, string sErrDesc)
        {

            string sFuncName = string.Empty;
            DateTime dtDocDate;
            DataSet oDSDetails = new DataSet();
            SqlConnection oSQLConnection = new SqlConnection();
            string sQuery = string.Empty;
            string sDocEntry = string.Empty;
            DataSet sPurReqDocEntry = new DataSet();
            string sWhsName = string.Empty;


            try
            {
                sFuncName = "Create_DeliverOrderDraft()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);


                sQuery = "select max(DocNumber) +1 [Count] from OSTK with (nolock)";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Executing Query for Getting Last Document Number : " + sQuery, sFuncName);
                sPurReqDocEntry = oGetSingleValue.Get_SingleValue(sQuery, sINTConnString, sErrDesc);

                sDocEntry = sPurReqDocEntry.Tables[0].Rows[0][0].ToString().Trim();
                dtDocDate = Convert.ToDateTime(sDocDate);


                oSQLConnection = new SqlConnection(sINTConnString);
                if (oSQLConnection.State == ConnectionState.Closed)
                    oSQLConnection.Open();

                oSQLTran = oSQLConnection.BeginTransaction();


                sQuery = "INSERT INTO OSTK( [DocEntry],[DocNumber],[CardCode],[CardName],[Requester],[WhsCode],[DocDate] ,[DocDueDate],[TaxDate] " +
                        " ,[DocStatus],[DocOwner],[CompanyDB],[StockTakeStatus] ,[UserRole],[ApprovedBy] ,[CreatedBy],[SAPSyncStatus]";

                sQuery += ") VALUES (";

                sQuery += "@DocEntry,@DocNumber,@CardCode,@CardName,@Requester,@WhsCode,@DocDate,@DocDueDate,@TaxDate,@DocStatus,@DocOwner, " +
                    "@CompanyDB,@StockTakeStatus,@UserRole,@ApprovedBy,@CreatedBy,@SAPSyncStatus )";

                oSQLCommand = new SqlCommand();
                oSQLCommand.Connection = oSQLConnection;
                oSQLCommand.CommandText = sQuery;
                oSQLCommand.Transaction = oSQLTran;
                oSQLCommand.CommandTimeout = 0;

                oSQLCommand.Parameters.Add("@DocEntry", SqlDbType.Int).Value = sDocEntry;
                oSQLCommand.Parameters.Add("@DocNumber", SqlDbType.Int).Value = Convert.ToInt32(sDocEntry);
                oSQLCommand.Parameters.Add("@CardCode", SqlDbType.NVarChar).Value = "STOCKTAKE";
                oSQLCommand.Parameters.Add("@CardName", SqlDbType.NVarChar).Value = "STOCKTAKE";
                oSQLCommand.Parameters.Add("@Requester", SqlDbType.NVarChar).Value = sRequester;
                oSQLCommand.Parameters.Add("@WhsCode", SqlDbType.NVarChar).Value = sOutlet;
                oSQLCommand.Parameters.Add("@DocDate", SqlDbType.Date).Value = dtDocDate;
                oSQLCommand.Parameters.Add("@DocDueDate", SqlDbType.Date).Value = dtDocDate;
                oSQLCommand.Parameters.Add("@TaxDate", SqlDbType.Date).Value = dtDocDate;
                oSQLCommand.Parameters.Add("@DocStatus", SqlDbType.Char).Value = "O";
                oSQLCommand.Parameters.Add("@DocOwner", SqlDbType.NVarChar).Value = sDocOwner;
                oSQLCommand.Parameters.Add("@CompanyDB", SqlDbType.NVarChar).Value = sCompanyDB;
                oSQLCommand.Parameters.Add("@StockTakeStatus", SqlDbType.NVarChar).Value = "Draft";
                oSQLCommand.Parameters.Add("@UserRole", SqlDbType.NVarChar).Value = sUserRole;
                oSQLCommand.Parameters.Add("@ApprovedBy", SqlDbType.NVarChar).Value = "";
                oSQLCommand.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = sRequester;
                oSQLCommand.Parameters.Add("@SAPSyncStatus", SqlDbType.Char).Value = "N";

                oSQLCommand.ExecuteNonQuery();


                oDSDetails.Clear();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Run_StoredProcedure() for Getting All the ItemCodes ", sFuncName);

                oDSDetails = oDataAccess.Run_StoredProcedure("Exec AE_SP019_GetItemsToDODraft '" + sUserRole + "','" + sOutlet + "' ", sConnString);

                if (oDSDetails != null && oDSDetails.Tables[0].Rows.Count != 0)
                {
                    sWhsName = oDSDetails.Tables[0].Rows[0][2].ToString().Trim();

                    for (int IRow = 0; IRow <= oDSDetails.Tables[0].Rows.Count - 1; IRow++)
                    {
                        sQuery = "INSERT INTO STK1( [DocEntry],[ItemCode],[ItemName],[LineId],[DocDate],[WhsCode],[WhsName],[CardCode],[CardName] " +
                            " ,[Quantity] ,[LineTotal],[Variance],[StocktakeQty1],[StocktakeQty2]";

                        sQuery += ") VALUES (";

                        sQuery += "@DocEntry,@ItemCode,@ItemName,@LineId,@DocDate,@WhsCode,@WhsName,@CardCode,@CardName,@Quantity,@LineTotal,@Variance, " +
                        " @StocktakeQty1,@StocktakeQty2 );";

                        //sQuery = "INSERT INTO STK1( [DocEntry],[ItemCode],[ItemName],[LineId],[DocDate],[WhsCode],[WhsName],[CardCode],[CardName] ";

                        //sQuery += ") VALUES (";

                        //sQuery += "@DocEntry,@ItemCode,@ItemName,@LineId,@DocDate,@WhsCode,@WhsName,@CardCode,@CardName );";


                        oSQLCommand = new SqlCommand();

                        if (oSQLConnection.State == ConnectionState.Closed)
                            oSQLConnection.Open();

                        oSQLCommand.Connection = oSQLConnection;
                        oSQLCommand.CommandText = sQuery;
                        oSQLCommand.Transaction = oSQLTran;
                        oSQLCommand.CommandTimeout = 0;

                        oSQLCommand.Parameters.Add("@DocEntry", SqlDbType.Int).Value = sDocEntry;
                        oSQLCommand.Parameters.Add("@ItemCode", SqlDbType.NVarChar).Value = oDSDetails.Tables[0].Rows[IRow][0].ToString().Trim();
                        oSQLCommand.Parameters.Add("@ItemName", SqlDbType.NVarChar).Value = oDSDetails.Tables[0].Rows[IRow][1].ToString().Trim();
                        oSQLCommand.Parameters.Add("@LineId", SqlDbType.Int).Value = IRow + 1;
                        oSQLCommand.Parameters.Add("@DocDate", SqlDbType.Date).Value = dtDocDate;
                        oSQLCommand.Parameters.Add("@WhsCode", SqlDbType.NVarChar).Value = sOutlet;
                        oSQLCommand.Parameters.Add("@WhsName", SqlDbType.NVarChar).Value = sWhsName;

                        oSQLCommand.Parameters.Add("@CardCode", SqlDbType.NVarChar).Value = oDSDetails.Tables[0].Rows[IRow][3].ToString().Trim();
                        oSQLCommand.Parameters.Add("@CardName", SqlDbType.NVarChar).Value = oDSDetails.Tables[0].Rows[IRow][4].ToString().Trim();

                        oSQLCommand.Parameters.Add("@Quantity", SqlDbType.Decimal).Value = 0;
                        oSQLCommand.Parameters.Add("@LineTotal", SqlDbType.Decimal).Value = 0;
                        oSQLCommand.Parameters.Add("@Variance", SqlDbType.Decimal).Value = 0;
                        oSQLCommand.Parameters.Add("@StocktakeQty1", SqlDbType.Decimal).Value = 0;
                        oSQLCommand.Parameters.Add("@StocktakeQty2", SqlDbType.Decimal).Value = 0;

                        oSQLCommand.ExecuteNonQuery();

                    }

                }

                oSQLTran.Commit();
                oSQLConnection.Dispose();
                oSQLCommand.Dispose();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);
                return "SUCCESS";
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message;
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                oSQLTran.Rollback();
                oSQLConnection.Dispose();
                oSQLCommand.Dispose();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);
                //throw Ex;
                return sErrDesc;
            }
        }

        public string Save_DeliveryOrderDraft(DataSet oDataSet, string sStatus, string sUserRole, string sOutlet, string sDocOwner, string sRequester
                                    , string sINTConnString, string sCompanyDB, bool bIsApprove, string sErrDesc)
        {

            string sFuncName = string.Empty;
            DateTime dtDocDate;
            DataSet oDSDetails = new DataSet();
            SqlConnection oSQLConnection = new SqlConnection();
            string sQuery = string.Empty;
            string sDocEntry = string.Empty;
            //DataSet sPurReqDocEntry = new DataSet();
            string sWhsName = string.Empty;
            DataTable oDTDistHeader = new DataTable();
            DataView oDVHeader = new DataView();
            DateTime dtCountDate;
            DateTime dtApproveDate;

            double dQuantity;
            double dInStock;
            try
            {
                sFuncName = "Save_DeliveryOrderDraft()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function", sFuncName);

                oDVHeader = oDataSet.Tables[0].DefaultView;
                oDSDetails = oDataSet;

                dtDocDate = Convert.ToDateTime(DateTime.Now);
                sDocEntry = oDVHeader[0]["DocEntry"].ToString().Trim();
                string[] sPostingDate = oDVHeader[0]["CountDate"].ToString().Split(' ');
                dtCountDate = Convert.ToDateTime(sPostingDate[0]);

                oSQLConnection = new SqlConnection(sINTConnString);
                if (oSQLConnection.State == ConnectionState.Closed)
                    oSQLConnection.Open();

                oSQLTran = oSQLConnection.BeginTransaction();

                sQuery = "DELETE FROM OSTK WHERE DocEntry='" + sDocEntry + "' ; DELETE FROM STK1 WHERE DocEntry='" + sDocEntry + "' ; ";

                oSQLCommand = new SqlCommand();
                oSQLCommand.Connection = oSQLConnection;
                oSQLCommand.CommandText = sQuery;
                oSQLCommand.Transaction = oSQLTran;
                oSQLCommand.CommandTimeout = 180;

                oSQLCommand.ExecuteNonQuery();

                if (oSQLConnection.State == ConnectionState.Closed)
                    oSQLConnection.Open();


                sQuery = "INSERT INTO OSTK( [DocEntry],[DocNumber],[CardCode],[CardName],[Requester],[WhsCode],[DocDate] ,[DocDueDate],[TaxDate] " +
                        " ,[DocStatus],[DocOwner],[CompanyDB],[StockTakeStatus] ,[UserRole],[ApprovedBy] ,[ApprovedDate],[CreatedBy],[SAPSyncStatus]";

                sQuery += ") VALUES (";

                sQuery += "@DocEntry,@DocNumber,@CardCode,@CardName,@Requester,@WhsCode,@DocDate,@DocDueDate,@TaxDate,@DocStatus,@DocOwner, " +
                    "@CompanyDB,@StockTakeStatus,@UserRole,@ApprovedBy,@ApprovedDate,@CreatedBy,@SAPSyncStatus )";

                oSQLCommand = new SqlCommand();
                oSQLCommand.Connection = oSQLConnection;
                oSQLCommand.CommandText = sQuery;
                oSQLCommand.Transaction = oSQLTran;
                oSQLCommand.CommandTimeout = 0;

                oSQLCommand.Parameters.Add("@DocEntry", SqlDbType.Int).Value = sDocEntry;
                oSQLCommand.Parameters.Add("@DocNumber", SqlDbType.Int).Value = Convert.ToInt32(sDocEntry);
                oSQLCommand.Parameters.Add("@CardCode", SqlDbType.NVarChar).Value = "STOCKTAKE";
                oSQLCommand.Parameters.Add("@CardName", SqlDbType.NVarChar).Value = "STOCKTAKE";
                oSQLCommand.Parameters.Add("@Requester", SqlDbType.NVarChar).Value = sRequester;
                oSQLCommand.Parameters.Add("@WhsCode", SqlDbType.NVarChar).Value = sOutlet;
                oSQLCommand.Parameters.Add("@DocDate", SqlDbType.Date).Value = dtCountDate;
                oSQLCommand.Parameters.Add("@DocDueDate", SqlDbType.Date).Value = dtCountDate;
                oSQLCommand.Parameters.Add("@TaxDate", SqlDbType.Date).Value = dtCountDate;
                oSQLCommand.Parameters.Add("@DocStatus", SqlDbType.Char).Value = "O";
                oSQLCommand.Parameters.Add("@DocOwner", SqlDbType.NVarChar).Value = sDocOwner;
                oSQLCommand.Parameters.Add("@CompanyDB", SqlDbType.NVarChar).Value = sCompanyDB;
                oSQLCommand.Parameters.Add("@StockTakeStatus", SqlDbType.NVarChar).Value = sStatus;
                oSQLCommand.Parameters.Add("@UserRole", SqlDbType.NVarChar).Value = sUserRole;
                oSQLCommand.Parameters.Add("@ApprovedBy", SqlDbType.NVarChar).Value = oDataSet.Tables[0].Rows[0]["ApprovedBy"].ToString();

                //if (oDataSet.Tables[0].Rows[0]["ApprovedBy"].ToString() != string.Empty)
                //{
                //    oSQLCommand.Parameters.Add("@ApprovedBy", SqlDbType.NVarChar).Value = oDataSet.Tables[0].Rows[0]["ApprovedBy"].ToString();
                //}

                if (oDataSet.Tables[0].Rows[0]["ApprovedDate"].ToString() != string.Empty)
                {
                    string[] sApproveDate = oDataSet.Tables[0].Rows[0]["ApprovedDate"].ToString().Split(' ');
                    dtApproveDate = Convert.ToDateTime(sApproveDate[0]);

                    oSQLCommand.Parameters.Add("@ApprovedDate", SqlDbType.Date).Value = dtApproveDate; //Convert.ToDateTime(oDataSet.Tables[0].Rows[0]["ApprovedDate"].ToString());
                }
                else
                {
                    oSQLCommand.Parameters.Add("@ApprovedDate", SqlDbType.Date).Value = DBNull.Value;
                }

                oSQLCommand.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = oDataSet.Tables[0].Rows[0]["CreatedBy"].ToString(); ;
                oSQLCommand.Parameters.Add("@SAPSyncStatus", SqlDbType.Char).Value = "N";

                oSQLCommand.ExecuteNonQuery();

                for (int iRowCount = 0; iRowCount <= oDVHeader.Count - 1; iRowCount++)
                {
                    sQuery = "INSERT INTO STK1( [DocEntry],[ItemCode],[ItemName],[LineId],[DocDate],[WhsCode],[WhsName],[Quantity] " +
                             ",[LineTotal],[Variance],[StocktakeQty1],[StocktakeQty2],[CardCode] ,[CardName] ";

                    sQuery += ") VALUES (";

                    sQuery += "@DocEntry,@ItemCode,@ItemName,@LineId,@DocDate,@WhsCode,@WhsName,@Quantity,@LineTotal,@Variance, " +
                    " @StocktakeQty1,@StocktakeQty2,@CardCode,@CardName );";

                    oSQLCommand = new SqlCommand();

                    if (oSQLConnection.State == ConnectionState.Closed)
                        oSQLConnection.Open();

                    oSQLCommand.Connection = oSQLConnection;
                    oSQLCommand.CommandText = sQuery;
                    oSQLCommand.Transaction = oSQLTran;
                    oSQLCommand.CommandTimeout = 0;

                    oSQLCommand.Parameters.Add("@DocEntry", SqlDbType.Int).Value = sDocEntry;
                    oSQLCommand.Parameters.Add("@ItemCode", SqlDbType.NVarChar).Value = oDVHeader[iRowCount]["ItemCode"].ToString().Trim();
                    oSQLCommand.Parameters.Add("@ItemName", SqlDbType.NVarChar).Value = oDVHeader[iRowCount]["Description"].ToString().Trim();
                    oSQLCommand.Parameters.Add("@LineId", SqlDbType.Int).Value = iRowCount + 1;
                    oSQLCommand.Parameters.Add("@DocDate", SqlDbType.Date).Value = dtDocDate;
                    oSQLCommand.Parameters.Add("@WhsCode", SqlDbType.NVarChar).Value = sOutlet;
                    oSQLCommand.Parameters.Add("@WhsName", SqlDbType.NVarChar).Value = sWhsName;

                    dQuantity = oDVHeader[iRowCount]["CountedInvUOM"].ToString() == string.Empty ? 0 : Convert.ToDouble(oDVHeader[iRowCount]["CountedInvUOM"].ToString());
                    dInStock = oDVHeader[iRowCount]["InStock"].ToString() == string.Empty ? 0 : Convert.ToDouble(oDVHeader[iRowCount]["InStock"].ToString());



                    oSQLCommand.Parameters.Add("@Quantity", SqlDbType.Decimal).Value = oDVHeader[iRowCount]["CountedInvUOM"].ToString();
                    oSQLCommand.Parameters.Add("@LineTotal", SqlDbType.Decimal).Value = 0;
                    oSQLCommand.Parameters.Add("@Variance", SqlDbType.Decimal).Value = dQuantity - dInStock;

                    //oSQLCommand.Parameters.Add("@StocktakeQty1", SqlDbType.Decimal).Value = oDVHeader[iRowCount]["CountedUOM1"].ToString() == string.Empty ? 0 : Convert.ToDouble(oDVHeader[iRowCount]["CountedUOM1"].ToString());
                    //oSQLCommand.Parameters.Add("@StocktakeQty2", SqlDbType.Decimal).Value = oDVHeader[iRowCount]["CountedUOM2"].ToString() == string.Empty ? 0 : Convert.ToDouble(oDVHeader[iRowCount]["CountedUOM2"].ToString());



                    if (oDVHeader[iRowCount]["CountedUOM1"].ToString().Trim() == string.Empty)
                    {
                        oSQLCommand.Parameters.AddWithValue("@StocktakeQty1", DBNull.Value);
                    }
                    else
                    {
                        oSQLCommand.Parameters.Add("@StocktakeQty1", SqlDbType.Decimal).Value = oDVHeader[iRowCount]["CountedUOM1"].ToString();
                    }

                    if (oDVHeader[iRowCount]["CountedUOM2"].ToString().Trim() == string.Empty)
                    {
                        oSQLCommand.Parameters.AddWithValue("@StocktakeQty2", DBNull.Value);
                    }
                    else
                    {
                        oSQLCommand.Parameters.Add("@StocktakeQty2", SqlDbType.Decimal).Value = oDVHeader[iRowCount]["CountedUOM2"].ToString();
                    }

                    oSQLCommand.Parameters.Add("@CardCode", SqlDbType.NVarChar).Value = oDVHeader[iRowCount]["CardCode"].ToString();
                    oSQLCommand.Parameters.Add("@CardName", SqlDbType.NVarChar).Value = oDVHeader[iRowCount]["CardName"].ToString();

                    oSQLCommand.ExecuteNonQuery();

                }

                if (bIsApprove == true)
                {
                    if (Approve_StockTakeCounting(sDocEntry, sOutlet, sCompanyDB, sINTConnString, sErrDesc) != "SUCCESS") throw new ArgumentException(sErrDesc);
                }

                oSQLTran.Commit();
                oSQLConnection.Dispose();
                oSQLCommand.Dispose();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);
                return "SUCCESS";
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message;
                oLog.WriteToLogFile(sErrDesc, sFuncName);
                oSQLTran.Rollback();
                oSQLConnection.Dispose();
                oSQLCommand.Dispose();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed with ERROR", sFuncName);
                //throw Ex;
                return sErrDesc;
            }
        }

        public string Approve_StockTakeCounting(string sDocEntry, string sOutlet, string sCompanyDB, string sConnString, string sErrDesc)
        {
            string sFuncName = string.Empty;
            DataView oDVHeader = new DataView();
            DataTable oDTDistinct = new DataTable();
            string sQueryString = string.Empty;

            try
            {
                sFuncName = "Approve_StockTakeCounting()";

                sQueryString = "INSERT INTO StockTakeApproval values(GETDATE (),'" + sDocEntry + "','" + sOutlet + "','" + sCompanyDB + "',NULL,NULL,NULL)";

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
                //throw Ex;
                return sErrDesc;
            }
        }

    }
}
