using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

using AE_Harrys.Common;


namespace AE_Harrys.DAL
{
    public class DataAccess
    {
        #region Fields

        public string sQueryString = string.Empty;
        public SqlDataAdapter oSQLAdapter = new SqlDataAdapter();
        public SqlCommand oSQLCommand = new SqlCommand();
        private SqlConnection oConnection;
        public DataSet oDataset = new DataSet();
        public SqlTransaction Master;
        public bool isMaster;

        clsLog oLog = new clsLog();

        public Int16 p_iDebugMode = DEBUG_ON;

        public const Int16 RTN_SUCCESS = 1;
        public const Int16 RTN_ERROR = 0;
        public const Int16 DEBUG_ON = 1;
        public const Int16 DEBUG_OFF = 0;
        public string sErrDesc = string.Empty;


        #endregion Fields

        #region Public Methods

        public DataSet Run_StoredProcedure(string sProcedureName, string sConnString)
        {
            oConnection = new SqlConnection(sConnString);
            oDataset = new DataSet();
            sQueryString = sProcedureName;
            string sFuncName = string.Empty;
            try
            {
                sFuncName = "Run_StoredProcedure()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Query : " + sProcedureName, sFuncName);

                if (oConnection.State == ConnectionState.Closed)
                    oConnection.Open();

                oSQLCommand = new SqlCommand(sQueryString, oConnection);
                oSQLAdapter.SelectCommand = oSQLCommand;
                oSQLCommand.CommandTimeout = 120;
                oSQLAdapter.Fill(oDataset);
                oSQLAdapter.Dispose();
                oSQLCommand.Dispose();
                oConnection.Close();
                
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName);
                return oDataset;

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
                oConnection.Dispose();
                oConnection = null;
            }
        }

        public DataSet Run_QueryString(string sQuery, string sConnString)
        {

            oConnection = new SqlConnection(sConnString);
            sQueryString = sQuery;
            string sFuncName = string.Empty;
            try
            {
                sFuncName = "Run_QueryString()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Query : " + sQuery, sFuncName);

                if (oConnection.State == ConnectionState.Closed)
                    oConnection.Open();

                oSQLCommand = new SqlCommand(sQueryString, oConnection);
                oSQLAdapter.SelectCommand = oSQLCommand;
                oSQLCommand.CommandTimeout = 120;
                oSQLAdapter.Fill(oDataset);
                oSQLAdapter.Dispose();
                oSQLCommand.Dispose();
                oConnection.Close();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName);

                return oDataset;

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
                oConnection.Dispose();
                oConnection = null;
            }
        }

        public string GetSingleValue(string sQuery, string sConnString)
        {

            oConnection = new SqlConnection(sConnString);
            sQueryString = sQuery;
            string sFuncName = string.Empty;
            try
            {
                sFuncName = "GetSingleValue()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Query : " + sQuery, sFuncName);
                if (oConnection.State == ConnectionState.Closed)
                    oConnection.Open();
                oSQLCommand = new SqlCommand(sQueryString, oConnection);
                oSQLAdapter.SelectCommand = oSQLCommand;
                oSQLAdapter.Fill(oDataset);
                oSQLAdapter.Dispose();
                oSQLCommand.Dispose();
                oConnection.Close();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);
                return oDataset.Tables[0].Rows[0][0].ToString();

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
                oConnection = null;
            }
        }


        //public DataSet SalesOrderData(string sConnectionString)
        //{

        //    DataSet oDataSet = null;
        //    SqlDataAdapter oDataAdapter = default(SqlDataAdapter);
        //    DataTable oDataTable = null;
        //    string sQuery = string.Empty;
        //    string sFuncName = string.Empty;
        //    //Reading the Header Values Based on Condition
        //    try
        //    {
        //        //sFuncName = "SalesOrderData()";
        //        //if (p_iDebugMode == DEBUG_ON)
        //        //    WriteToLogFile_Debug("Starting Function", sFuncName);

        //        oDataSet = new DataSet();

        //        sQuery = "SELECT T0.[DocEntry],T0.[DocNum], T0.[CardCode], T0.[CardName], T0.[Address], T1.[Tel1], T1.[E_MailL], T0.[DocNum], T2.[SlpName], T0.[U_DistDate]" + " , T0.[DocDueDate], T0.[U_EventTimeStart], T0.[U_EventTimeEnd], T0.[U_FoodTimeStart], T0.[U_FoodTimeEnd], T0.[U_BvgTimeStart]" + " , T0.[U_BvgTimeEnd], T0.[U_Venue], T0.[U_Function], T0.[U_Setup], T0.[U_GTD_Attd], T0.[U_EXP_Attd], T0.[U_AudioVisual], T0.[U_BanquetOp]" + " , T0.[U_Linens], T0.[U_Signage], T0.[U_Finance],T0.[U_ReqDownPaymt], T0.[Project], T0.[DocTotal], T0.[U_PaidToDate], T0.[DocStatus] " + " FROM ORDR T0  LEFT OUTER JOIN OCPR T1 ON T0.[CntctCode] = T1.[CntctCode]  LEFT OUTER JOIN OSLP T2 ON T0.[SlpCode] = T2.[SlpCode] where T0.[DocStatus]='O' ";


        //        oDataAdapter = new SqlDataAdapter(sQuery, sConnectionString);
        //        oDataTable = new DataTable();
        //        oDataAdapter.Fill(oDataTable);
        //        oDataSet.Tables.Add(oDataTable);

        //        if (oDataSet.Tables[0].Rows.Count > 0)
        //        {
        //            //Get a DocEntry for Read the Lines Level Values Based on Respective DocEntry

        //            sQuery = "";
        //            sQuery = "declare  @diststring as varchar(max)";
        //            sQuery += Constants.vbCrLf + "select @diststring=coalesce(@diststring+ ''', ''','')+convert(varchar,docentry)from ORDR Where DocStatus='O' ";
        //            sQuery += Constants.vbCrLf + "select  ''''+@diststring+ '''' as DocEntry ";
        //            oDataTable = new DataTable();
        //            oDataAdapter = new SqlDataAdapter(sQuery, sConnectionString);
        //            oDataAdapter.Fill(oDataTable);


        //            //To Get Get a Line Level Data's 

        //            sQuery = "";
        //            sQuery += Constants.vbCrLf + "SELECT T0.[DocEntry], T0.[LineNum], T0.[WhsCode], T0.[ItemCode], T0.[U_EASIRcpt], T0.[Price], T0.[Quantity], " + " T0.[LineTotal], T0.[Project] FROM RDR1 T0 where T0.[DocEntry] in (" + oDataTable.Rows[0]["DocEntry"].ToString() + ")  ";

        //            oDataAdapter = new SqlDataAdapter(sQuery, sConnectionString);
        //            oDataTable = new DataTable();
        //            oDataAdapter.Fill(oDataTable);
        //            oDataSet.Tables.Add(oDataTable);

        //            //if (p_iDebugMode == DEBUG_ON)
        //            //    WriteToLogFile_Debug("Completed with SUCCESS", sFuncName);
        //            return oDataSet;
        //        }
        //        else
        //        {
        //            //if (p_iDebugMode == DEBUG_ON)
        //            //    WriteToLogFile_Debug("Completed with SUCCESS", sFuncName);
        //            return null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //if (p_iDebugMode == DEBUG_ON)
        //        //    WriteToLogFile_Debug("Completed with ERROR", sFuncName);
        //        //WriteToLogFile(ex.Message, sFuncName);
        //        throw ex;
        //    }

        //}

        #endregion  Public Methods
    }
}
