using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Configuration;

using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

using AE_Harrys.BLL;
using AE_Harrys.Common;


namespace AE_Harrys.WebService
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Master : System.Web.Services.WebService
    {
        #region Fields
        public string sConnString = string.Empty;

        clsLog oLog = new clsLog();

        public Int16 p_iDebugMode = DEBUG_ON;

        public const Int16 RTN_SUCCESS = 1;
        public const Int16 RTN_ERROR = 0;
        public const Int16 DEBUG_ON = 1;
        public const Int16 DEBUG_OFF = 0;
        public string sErrDesc = string.Empty;

        #endregion Fields

        #region objects
        clsLogin oLogin = new clsLogin();
        clsSupplier oSupplier = new clsSupplier();
        clsOutlet oOutlet = new clsOutlet();
        clsMaterialReqBySupplier oMatReqByBar = new clsMaterialReqBySupplier();
        clsMaterialReqByItem oMatReqByItem = new clsMaterialReqByItem();
        clsMaterialReqDraft oMatReqDraft = new clsMaterialReqDraft();
        clsMaterialRequestSubmit_Approved oMatReqSub_App = new clsMaterialRequestSubmit_Approved();
        clsCommon oCommon = new clsCommon();
        clsInventory oInventory = new clsInventory();
        clsPurchaseOrder oPurchaseOrder = new clsPurchaseOrder();
        clsCopyMarketingDocument oCopyDocument = new clsCopyMarketingDocument();
        clsDeliveryOrder oDeliveryOrder = new clsDeliveryOrder();
        clsPurchaseRequest oPurchaseReq = new clsPurchaseRequest();

        clsInventoryTransferRequest oInvRequest = new clsInventoryTransferRequest();
        clsOutletListPRApproval oOutletList = new clsOutletListPRApproval();
        clsStockTakeCounting oStockTakeCount = new clsStockTakeCounting();

        SAPbobsCOM.Company oDIComapny;

        #endregion objects

        #region Methods

        //[WebMethod(EnableSession = true)]
        //public void SAPCompany(SAPbobsCOM.Company oCompany, string sConnString)
        //{

        //    Session["SAPCompany"] = oCompany;
        //    Session["ConnString"] = sConnString;

        //}


        [WebMethod]
        public string Create_InventoryTransferRequest(string sCompanyDB, DataSet oDSInvReqData)
        {

            DataSet oDTCompanyList = new DataSet();
            SAPbobsCOM.Company oDICompany;
            string sReturnValue = string.Empty;
            string sConnString = string.Empty;
            string sFuncName = string.Empty;

            try
            {
                sFuncName = "Create_InventoryTransferRequest()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                var oReceiveInOutlet = new clsCopyMarketingDocument();

                oDTCompanyList = Get_Company_Details();

                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                oDICompany = ConnectToTargetCompany(sCompanyDB);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling oInvRequest.Create_InventoryTransferRequest() ", sFuncName);

                //sReturnValue = oInvRequest.Create_InventoryTransferRequest(oDIComapny, oDSInvReqData, sConnString, sErrDesc);
                sReturnValue = oInvRequest.Create_InventoryTransferRequest(oDICompany, oDSInvReqData, sConnString, sErrDesc);

                oDICompany.Disconnect();
                oDICompany = null;
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                oReceiveInOutlet = null;

                return sReturnValue;

            }

            catch (Exception ex)
            {
                sErrDesc = ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                return sErrDesc;
            }
            finally
            {

                oDTCompanyList.Dispose();
                oCommon = null;
                //if (oDIComapny != null)
                //{
                //    oDIComapny.Disconnect();
                //    oDIComapny = null;
                //}

            }

        }


        [WebMethod]
        public DataSet Get_OpenTransferRequest(string sCompanyDB, string sFromOutlet, string sToOutlet, string sStatus
                                            , string sFromDate, string sToDate)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDSResult = new DataSet();
            DataSet oDTCompanyList = new DataSet();

            try
            {
                sFuncName = "Get_OpenTransferRequest()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);

                oDTCompanyList = Get_Company_Details();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);

                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling oInvRequest .Get_OpenTransferRequest() ", sFuncName);

                oDSResult = oInvRequest.Get_OpenTransferRequest(sFromOutlet, sToOutlet, sStatus, sFromDate, sToDate, sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return oDSResult;
            }

            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oDSResult.Dispose();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
            finally
            {
                oDTCompanyList.Dispose();
                oCommon = null;
                oInvRequest = null;

            }
        }

        [WebMethod]
        public DataSet Get_OpenTransferRequestDetails(string sCompanyDB, string sDocEntry)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDSResult = new DataSet();
            DataSet oDTCompanyList = new DataSet();

            try
            {
                sFuncName = "Get_OpenTransferRequestDetails()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);

                oDTCompanyList = Get_Company_Details();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);

                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);
                oDTCompanyList.Dispose();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling oInvRequest.Get_OpenTransferRequestDetails ", sFuncName);

                oDSResult = oInvRequest.Get_OpenTransferRequestDetails(sDocEntry, sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return oDSResult;
            }

            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oDSResult.Dispose();
                oDTCompanyList.Dispose();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
            finally
            {
                oDTCompanyList.Dispose();
                oCommon = null;
                oInvRequest = null;
            }

        }

        [WebMethod]
        public DataSet Get_OutletListPR(string sGroupType, string sCompanyDB)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDSResult = new DataSet();
            DataSet oDTCompanyList = new DataSet();
            try
            {
                sFuncName = "Get_OutletListPR()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);

                oDTCompanyList = Get_Company_Details();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);

                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling oOutletList.Get_OutletListPR ", sFuncName);

                oDSResult = oOutletList.Get_OutletListPR(sGroupType, sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return oDSResult;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        [WebMethod]
        public string Approve_InventoryTransferRequest(string sCompanyDB, DataSet oDSInvTransData)
        {

            DataSet oDTCompanyList = new DataSet();
            SAPbobsCOM.Company oDICompany;
            string sReturnValue = string.Empty;
            string sConnString = string.Empty;
            string sFuncName = string.Empty;

            try
            {
                sFuncName = "Approve_InventoryTransferRequest()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                var oReceiveInOutlet = new clsCopyMarketingDocument();

                oDTCompanyList = Get_Company_Details();

                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                oDICompany = ConnectToTargetCompany(sCompanyDB);


                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling oInvRequest.Approve_InventoryTransferRequest() ", sFuncName);

                //sReturnValue = oInvRequest.Approve_InventoryTransferRequest(oDIComapny, oDSInvTransData, sConnString, sErrDesc);
                sReturnValue = oInvRequest.Approve_InventoryTransferRequest(oDICompany, oDSInvTransData, sConnString, sErrDesc);

                oDICompany.Disconnect();
                oDICompany = null;

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return sReturnValue;

            }

            catch (Exception ex)
            {
                sErrDesc = ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                return sErrDesc;
            }

        }

        [WebMethod]
        public string Reject_InventoryTransferRequest(string sCompanyDB, DataSet oDSInvTransData)
        {

            DataSet oDTCompanyList = new DataSet();
            SAPbobsCOM.Company oDICompany;
            string sReturnValue = string.Empty;
            string sConnString = string.Empty;
            string sFuncName = string.Empty;

            try
            {
                sFuncName = "Reject_InventoryTransferRequest()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                var oReceiveInOutlet = new clsCopyMarketingDocument();

                oDTCompanyList = Get_Company_Details();

                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);
                oDTCompanyList.Dispose();

                oDICompany = ConnectToTargetCompany(sCompanyDB);


                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling oInvRequest.Reject_InventoryTransferRequest() ", sFuncName);

                //sReturnValue = oInvRequest.Reject_InventoryTransferRequest(oDIComapny, oDSInvTransData, sConnString, sErrDesc);
                sReturnValue = oInvRequest.Reject_InventoryTransferRequest(oDICompany, oDSInvTransData, sConnString, sErrDesc);

                oDICompany.Disconnect();
                oDICompany = null;
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return sReturnValue;

            }

            catch (Exception ex)
            {
                sErrDesc = ex.Message.ToString();
                oDTCompanyList.Dispose();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                return sErrDesc;
            }

        }

        [WebMethod]
        public DataSet Get_InventoryTransferRequest_ItemSearch(string sCompanyDB, string sFromOutlet, string sToOutlet, string sGroupType)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDSResult = new DataSet();
            DataSet oDTCompanyList = new DataSet();

            try
            {
                sFuncName = "Get_InventoryTransferRequest_ItemSearch()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);

                oDTCompanyList = Get_Company_Details();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);

                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling oInvRequest.Get_InventoryTransferRequest_ItemSearch() ", sFuncName);

                oDSResult = oInvRequest.Get_InventoryTransferRequest_ItemSearch(sFromOutlet, sToOutlet, sGroupType, sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return oDSResult;
            }

            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oDSResult.Dispose();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        //============================= PHASE II WEB METHODS FINISHED ==========================================================


        //PHASE 1 WEB METHODS START:


        [WebMethod]
        public string Insert_PurchaseRequest(DataSet oDataset, bool IsDraft, string sCompanyDB, bool IsAdd, bool bIsSubmit, bool bIsDelCharge)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDSResult = new DataSet();
            DataSet oDTCompanyList = new DataSet();
            SAPbobsCOM.Company oDIComapny = new SAPbobsCOM.Company();
            //string sConnString = string.Empty;
            string sINTConnString = string.Empty;

            try
            {
                sFuncName = "Insert_PurchaseRequest()";
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);
                oDTCompanyList = Get_Company_Details();

                //if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling ConnectToTargetCompany() ", sFuncName);
                //oDIComapny = ConnectToTargetCompany(sCompanyDB);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);
                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                sINTConnString = System.Configuration.ConfigurationManager.AppSettings["INTDBWEB"].ToString();


                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Insert_PurchaseRequest() ", sFuncName);

                sReturnValue = oPurchaseReq.Insert_PurchaseRequest(oDataset, IsDraft, oDIComapny, IsAdd, bIsSubmit, bIsDelCharge, sConnString, sINTConnString, sCompanyDB, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return sReturnValue;


            }

            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                return sErrDesc;
            }
            finally
            {
                oDSResult.Dispose();
                oDTCompanyList.Dispose();
                oPurchaseReq = null;
                oCommon = null;
            }

        }

        [WebMethod]
        public string Update_PurchaseRequest(DataSet oDataset, bool IsDraft, string sCompanyDB, bool IsAdd, bool bIsSubmit, bool bIsDelCharge)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDSResult = new DataSet();
            DataSet oDTCompanyList = new DataSet();
            SAPbobsCOM.Company oDIComapny = new SAPbobsCOM.Company();
            //string sConnString = string.Empty;
            string sINTConnString = string.Empty;

            try
            {
                sFuncName = "Update_PurchaseRequest()";
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);
                oDTCompanyList = Get_Company_Details();

                //if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling ConnectToTargetCompany() ", sFuncName);
                //oDIComapny = ConnectToTargetCompany(sCompanyDB);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);
                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                sINTConnString = System.Configuration.ConfigurationManager.AppSettings["INTDBWEB"].ToString();


                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Update_PurchaseRequest() ", sFuncName);

                sReturnValue = oPurchaseReq.Update_PurchaseRequest(oDataset, IsDraft, oDIComapny, IsAdd, bIsSubmit, bIsDelCharge, sConnString, sINTConnString, sCompanyDB, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return sReturnValue;

            }

            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                return sErrDesc;
            }
            finally
            {
                oPurchaseReq = null;
            }

        }

        [WebMethod]
        public string Submit_PurchaseRequest(DataSet oDataset, string sCompanyDB)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDTCompanyList = new DataSet();
            string sINTConnString = string.Empty;
            //string sConnString = string.Empty;
            try
            {
                sFuncName = "Submit_PurchaseRequest()";
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                sINTConnString = System.Configuration.ConfigurationManager.AppSettings["INTDBWEB"].ToString();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);
                oDTCompanyList = Get_Company_Details();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);
                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);
                oDTCompanyList.Dispose();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Submit_PurchaseRequest() ", sFuncName);

                sReturnValue = oPurchaseReq.Submit_PurchaseRequest(oDataset, sCompanyDB, sConnString, sINTConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return sReturnValue;

            }

            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oDTCompanyList.Dispose();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                return sErrDesc;
            }

        }

        [WebMethod]
        public string Approve_PurchaseRequest(DataSet oDataset, string sCompanyDB)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDTCompanyList = new DataSet();
            DataSet oDSResult = new DataSet();
            // string sConnString = string.Empty;
            string sINTConnString = string.Empty;
            try
            {
                sFuncName = "Approve_PurchaseRequest()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);
                oDTCompanyList = Get_Company_Details();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);
                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                sINTConnString = System.Configuration.ConfigurationManager.AppSettings["INTDBWEB"].ToString();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Approve_PurchaseRequest() ", sFuncName);
                sReturnValue = oPurchaseReq.Approve_PurchaseRequest(oDataset, sCompanyDB, sConnString, sINTConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);
                return sReturnValue;
            }

            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                return sErrDesc;
            }
        }

        [WebMethod]
        public string Delete_PurchaseRequest(DataSet oDataset, string sCompanyDB)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            string sINTConnString = string.Empty;
            string sConnString = string.Empty;
            try
            {
                sFuncName = "Delete_PurchaseRequest()";
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                sINTConnString = System.Configuration.ConfigurationManager.AppSettings["INTDBWEB"].ToString();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling DeleteDrafts() ", sFuncName);

                sReturnValue = oPurchaseReq.Delete_PurchaseRequest(oDataset, sINTConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return sReturnValue;

            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                return sErrDesc;
            }

        }

        [WebMethod]
        public DataSet Get_ApprovalStatus_Summary(string sFromDate, string sToDate, string sOutlet, string sConnString)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDSResult = new DataSet();
            try
            {

                sFuncName = "Get_ApprovalStatus_Summary()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ApprovalStatus_Summary() ", sFuncName);

                oDSResult = oPurchaseReq.Get_ApprovalStatus_Summary(sFromDate, sToDate, sOutlet, sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return oDSResult;

            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        [WebMethod]
        public string Login(string sUserName, string sPassword, string sDBName, string sServer, string sLicServerName
                                , string sDBUserName, string sDBPassword)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDTCompanyList = new DataSet();
            try
            {
                sFuncName = "Login()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);
                oDTCompanyList = Get_Company_Details();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);
                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sDBName, sErrDesc);


                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling UserValidation() ", sFuncName);
                sReturnValue = oLogin.UserValidation(sUserName, sPassword, sConnString);


                //sReturnValue = oLogin.SAPLogin(sUserName, sPassword, sDBName, sServer, sLicServerName, sDBUserName, sDBPassword, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);
                return sReturnValue;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oDTCompanyList.Dispose();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                return sErrDesc;
            }

        }

        [WebMethod]
        public DataSet Get_UserInformation(string sUserName, string sConnString)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDSResult = new DataSet();
            try
            {
                sFuncName = "Get_UserInformation()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_UserInformation() ", sFuncName);

                oDSResult = oLogin.Get_UserInformation(sUserName, sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return oDSResult;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }

        }

        [WebMethod]
        public DataSet Get_Outlet_Details(string sSuperUser, string sOutletCode, int iApprovalLevel, string sConnString)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDSResult = new DataSet();
            try
            {
                sFuncName = "Get_Outlet_Details()";
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Outlets() ", sFuncName);

                oDSResult = oOutlet.Get_Outlets(sSuperUser, sOutletCode, sConnString, iApprovalLevel, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return oDSResult;
            }
            catch (Exception Ex)
            {

                sErrDesc = Ex.Message.ToString();
                oDSResult.Dispose();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }

        }

        [WebMethod]
        public DataSet Get_Supplier_Details(string sUserRole, string sConnString)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDSResult = new DataSet();
            try
            {
                sFuncName = "Get_Supplier_Details()";
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_UserInformation() ", sFuncName);

                oDSResult = oSupplier.Get_Supplier(sUserRole, sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return oDSResult;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oDSResult.Dispose();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }

        }

        [WebMethod]
        public DataSet Get_Company_Details()
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDSResult = new DataSet();
            clsCompanyList oCompanyList = new clsCompanyList();
            try
            {
                sFuncName = "Get_Company_Details()";
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_NewConnectionString() ", sFuncName);

                sConnString = Get_NewConnectionString();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_CompanyList() ", sFuncName);

                oDSResult = oCompanyList.Get_CompanyList(sConnString);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return oDSResult;

            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oDSResult.Dispose();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }

        }

        [WebMethod]
        public DataSet Get_MaterialReqBySupplier(string sSupplier, string sOutlet, string userRole, string sConnString)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDSResult = new DataSet();

            try
            {
                sFuncName = "Get_MaterialReqBySupplier()";
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_MaterialReqBySupplier() ", sFuncName);

                oDSResult = oMatReqByBar.Get_MaterialReqBySupplier(sSupplier, sOutlet, userRole, sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return oDSResult;

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
                oMatReqByBar = null;
                oDSResult.Dispose();
            }


        }

        [WebMethod]
        public DataSet Get_MaterialReqByItem(string sGroupType, string sOutlet, string sConnString)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDSResult = new DataSet();
            try
            {
                sFuncName = "Get_MaterialReqByItem()";
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_MaterialReqByItem() ", sFuncName);

                oDSResult = oMatReqByItem.Get_MaterialReqByItem(sGroupType, sOutlet, sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return oDSResult;

            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oDSResult.Dispose();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        [WebMethod]
        public string Create_PurchaseRequest(DataSet oDataset, bool IsDraft, string sCompanyDB, bool IsAdd, bool bIsSubmit, bool bIsDelCharge)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            //DataSet oDSResult = new DataSet();
            //DataSet oDTCompanyList = new DataSet();
            SAPbobsCOM.Company oDIComapny;
            //string sConnString = string.Empty;

            try
            {
                sFuncName = "Create_PurchaseRequest()";
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                //if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);
                //oDTCompanyList = Get_Company_Details();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling ConnectToTargetCompany() ", sFuncName);
                oDIComapny = ConnectToTargetCompany(sCompanyDB);

                ////if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);
                // //sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Create_PurchaseRequest() ", sFuncName);

                sReturnValue = oMatReqByBar.Create_PurchaseRequest(oDataset, IsDraft, oDIComapny, IsAdd, bIsSubmit, bIsDelCharge, sConnString, sErrDesc);
                oDIComapny.Disconnect();
                oDIComapny = null;
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return sReturnValue;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oDIComapny = null;
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                return sErrDesc;
            }
        }

        [WebMethod]
        public string Delete_Drafts(DataSet oDataset, string sCompanyDB)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDTCompanyList = new DataSet();

            string sConnString = string.Empty;
            try
            {
                sFuncName = "Delete_Drafts()";
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                SAPbobsCOM.Company oDIComapny;
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling ConnectToTargetCompany() ", sFuncName);
                oDIComapny = ConnectToTargetCompany(sCompanyDB);


                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling DeleteDrafts() ", sFuncName);

                sReturnValue = oCommon.DeleteDrafts(oDataset, oDIComapny, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return sReturnValue;

            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                return sErrDesc;
            }

        }

        [WebMethod]
        public string PRDraft_Submit(DataSet oDataset, string sCompanyDB)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            //DataSet oDTCompanyList = new DataSet();

            //string sConnString = string.Empty;
            try
            {
                sFuncName = "PRDraft_Submit()";
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                //if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);

                //oDTCompanyList = Get_Company_Details();

                //if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);

                //sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                SAPbobsCOM.Company oDIComapny;

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling ConnectToTargetCompany() ", sFuncName);
                oDIComapny = ConnectToTargetCompany(sCompanyDB);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling PR_DraftSubmit() ", sFuncName);

                sReturnValue = oMatReqByBar.PR_DraftSubmit(oDataset, oDIComapny, sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return sReturnValue;

            }

            //catch (CommunicationException ce)
            //{
            //    sErrDesc = ce.Message.ToString();
            //    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
            //    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
            //    return sErrDesc;
            //}
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                return sErrDesc;
            }

        }

        [WebMethod]
        public DataSet Get_MaterialReqDraft(string sOutlet, string sConnString)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDSResult = new DataSet();
            try
            {
                sFuncName = "Get_MaterialReqDraft()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_MaterialReqDraft() ", sFuncName);

                oDSResult = oMatReqDraft.Get_MaterialReqDraft(sOutlet, sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return oDSResult;

            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oDSResult.Dispose();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }

        }

        [WebMethod]
        public DataSet Get_MaterialReq_Submitted_Approval(string sOutlet, int iStatus, string fromDate, string todate, string sUserRole, string sConnString)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDSResult = new DataSet();
            try
            {

                sFuncName = "Get_MaterialReq_Submitted_Approval()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_MaterialReqSubmit_Approved() ", sFuncName);

                oDSResult = oMatReqSub_App.Get_MaterialReqSubmit_Approved(sOutlet, iStatus, fromDate, todate, sUserRole, sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return oDSResult;

            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        [WebMethod]
        public DataSet Get_DraftDetails(string sDocEntry, string sConnString, string sDocType)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDSResult = new DataSet();
            try
            {
                sFuncName = "Get_DraftDetails()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_DraftDetails() ", sFuncName);

                oDSResult = oMatReqDraft.Get_DraftDetails(sDocEntry, sConnString, sDocType, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return oDSResult;

            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }

        }

        [WebMethod]
        public DataSet Get_MRSubmit_ApprovedDetails(string sDocEntry, string sDocType, string sConnString)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDSResult = new DataSet();
            try
            {
                sFuncName = "Get_MRSubmit_ApprovedDetails()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Submit_ApprovedDetails() ", sFuncName);

                oDSResult = oMatReqSub_App.Get_Submit_ApprovedDetails(sDocEntry, sDocType, sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return oDSResult;

            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oDSResult.Dispose();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }

        }

        [WebMethod]
        public DataSet Get_MaterialReqBySupplierItemList(string sOutlet, string sConnString)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDSResult = new DataSet();
            try
            {
                sFuncName = "Get_MaterialReqBySupplierItemList()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_MaterialReqBySupplierItemList() ", sFuncName);

                oDSResult = oMatReqByItem.Get_MaterialReqBySupplierItemList(sOutlet, sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return oDSResult;

            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oDSResult.Dispose();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        [WebMethod]
        public DataSet Get_ItemList(string sCompanyDB)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDSResult = new DataSet();
            DataSet oDTCompanyList = new DataSet();
            // string sConnString = string.Empty;

            try
            {
                sFuncName = "Get_ItemList()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);

                oDTCompanyList = Get_Company_Details();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);

                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ItemList() ", sFuncName);

                oDSResult = oInventory.Get_ItemList(sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return oDSResult;

            }

            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        [WebMethod]
        public DataSet Get_ReasonDetails(string sCompanyDB, string sOutlet)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDSResult = new DataSet();
            DataSet oDTCompanyList = new DataSet();
            // string sConnString = string.Empty;

            try
            {
                sFuncName = "Get_ReasonDetails()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);

                oDTCompanyList = Get_Company_Details();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);

                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ReasonDetails() ", sFuncName);

                oDSResult = oInventory.Get_ReasonDetails(sOutlet, sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return oDSResult;
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
                oDSResult.Dispose();
                oDTCompanyList.Dispose();
            }
        }

        [WebMethod]
        public string ConvertDraftToDocument(DataSet oDataset, string sCompanyDB)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDTCompanyList = new DataSet();

            try
            {
                sFuncName = "ConvertDraftToDocument()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                SAPbobsCOM.Company oDIComapny;

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling ConnectToTargetCompany() ", sFuncName);

                oDIComapny = ConnectToTargetCompany(sCompanyDB);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);

                oDTCompanyList = Get_Company_Details();

                //if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);

                //sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling ConvertDraftToDocument() ", sFuncName);

                sReturnValue = oCommon.ConvertDraftToDocument(oDataset, SAPbobsCOM.BoObjectTypes.oDrafts, oDIComapny, sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return sReturnValue;
            }

            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                return sErrDesc;
            }
        }

        [WebMethod]
        public DataSet Get_InventoryRequest(string sCompanyDB, string sOutlet)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDTCompanyList = new DataSet();
            DataSet oDSResult = new DataSet();
            // string sConnString = string.Empty;
            try
            {
                sFuncName = "Get_InventoryRequest()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);

                oDTCompanyList = Get_Company_Details();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);
                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_InventoryRequest() ", sFuncName);

                oDSResult = oInventory.Get_InventoryRequest(sOutlet, sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return oDSResult;
            }

            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        [WebMethod]
        public DataSet Get_InventoryRequestDetails(string sCompanyDB, string sRequestNo)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDTCompanyList = new DataSet();
            DataSet oDSResult = new DataSet();
            // string sConnString = string.Empty;
            try
            {

                sFuncName = "Get_InventoryRequestDetails()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);

                oDTCompanyList = Get_Company_Details();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);
                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_InventoryRequestDetails() ", sFuncName);
                oDSResult = oInventory.Get_InventoryRequestDetails(sRequestNo, sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return oDSResult;
            }

            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        [WebMethod]
        public DataSet Get_OpenPOList(string sCompanyDB, string sSupplier, string sSupplierType, string sAccessType, string sOutletCode)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDTCompanyList = new DataSet();
            DataSet oDSResult = new DataSet();
            // string sConnString = string.Empty;
            try
            {
                sFuncName = "Get_OpenPOList()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);
                oDTCompanyList = Get_Company_Details();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);
                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_OpenPOList() ", sFuncName);
                oDSResult = oPurchaseOrder.Get_OpenPOList(sSupplier, sSupplierType, sAccessType, sOutletCode, sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);
                return oDSResult;
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
                oDSResult.Dispose();
                oDTCompanyList.Dispose();
            }
        }

        [WebMethod]
        public DataSet Get_ReceiveInOutlet_Details(string sCompanyDB, string sOutlet, string sAccessType, string sSupplier)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDTCompanyList = new DataSet();
            DataSet oDSResult = new DataSet();
            // string sConnString = string.Empty;
            try
            {
                sFuncName = "Get_ReceiveInOutlet_Details()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);
                oDTCompanyList = Get_Company_Details();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);
                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_OpenPOListDetails() ", sFuncName);
                oDSResult = oPurchaseOrder.Get_OpenPOListDetails(sSupplier, sOutlet, sAccessType, sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);
                return oDSResult;
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
                oDTCompanyList.Dispose();
                oDSResult.Dispose();
            }
        }

        [WebMethod]
        public DataSet Get_GetPendingDrafts(string sCompanyDB, string sOutlet, string sUserRole, string sApprLevel)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDTCompanyList = new DataSet();
            DataSet oDSResult = new DataSet();
            // string sConnString = string.Empty;
            try
            {
                sFuncName = "Get_GetPendingDrafts()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);
                oDTCompanyList = Get_Company_Details();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);
                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_GetPendingDrafts() ", sFuncName);
                oDSResult = oMatReqDraft.Get_GetPendingDrafts(sOutlet, sUserRole, sApprLevel, sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);
                return oDSResult;
            }

            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oDSResult.Dispose();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        [WebMethod]
        public string Update_ApprovalStatus(DataSet oDataset, string sCompanyDB)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            //DataSet oDTCompanyList = new DataSet();
            DataSet oDSResult = new DataSet();
            // string sConnString = string.Empty;
            try
            {
                sFuncName = "Update_ApprovalStatus()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                //if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);
                //oDTCompanyList = Get_Company_Details();

                SAPbobsCOM.Company oDIComapny;
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling ConnectToTargetCompany() ", sFuncName);
                oDIComapny = ConnectToTargetCompany(sCompanyDB);

                //if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);
                //sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Update_ApprovalStatus() ", sFuncName);
                sReturnValue = oPurchaseOrder.Update_ApprovalStatus(oDataset, oDIComapny, sConnString, sErrDesc);
                oDIComapny.Disconnect();
                oDIComapny = null;
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);
                oDSResult.Dispose();
                return sReturnValue;
            }

            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oDSResult.Dispose();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                return sErrDesc;
            }
        }

        [WebMethod]
        public string ReceiveInOutlet(DataSet oDataset, string sCompanyDB)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            //DataSet oDTCompanyList = new DataSet();
            //DataSet oDSResult = new DataSet();
            // string sConnString = string.Empty;
            SAPbobsCOM.Company oDIComapny;
            try
            {
                //this.Session.Timeout = 600000;

                sFuncName = "ReceiveInOutlet()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                //if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);
                //oDTCompanyList = Get_Company_Details();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling ConnectToTargetCompany() ", sFuncName);
                oDIComapny = ConnectToTargetCompany(sCompanyDB);

                //if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);
                //sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling ReceiveInOutlet() ", sFuncName);
                sReturnValue = oCopyDocument.ReceiveInOutlet(oDataset, oDIComapny, sConnString, sErrDesc);
                oDIComapny.Disconnect();
                oDIComapny = null;
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);
                return sReturnValue;
            }

            catch (Exception Ex)
            {
                oDIComapny = null;
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                return sErrDesc;
            }
        }

        [WebMethod]
        public DataSet Get_SalesTakingCountList(string sCompanyDB, string sOutlet, string sUserRole)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDTCompanyList = new DataSet();
            DataSet oDSResult = new DataSet();
            // string sConnString = string.Empty;
            try
            {
                sFuncName = "Get_SalesTakingCountList()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);
                oDTCompanyList = Get_Company_Details();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);
                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);
                oDTCompanyList.Dispose();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_StockTakeCountingList() ", sFuncName);
                oDSResult = oStockTakeCount.Get_StockTakeCountingList(sOutlet, sUserRole, sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);
                return oDSResult;

            }

            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oDSResult.Dispose();
                oDTCompanyList.Dispose();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        [WebMethod]
        public DataSet Get_StockTakeCounting(string sCompanyDB, string sOutlet, string sUserRole, string sStatus, string sDocEntry)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDTCompanyList = new DataSet();
            DataSet oDSResult = new DataSet();
            // string sConnString = string.Empty;
            try
            {
                sFuncName = "Get_StocTakeCounting()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);
                oDTCompanyList = Get_Company_Details();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);
                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                oDSResult = oDeliveryOrder.Get_StockTakeCounting(sOutlet, sUserRole, sStatus, sDocEntry, sConnString, sErrDesc);

                if (oDSResult != null && oDSResult.Tables[0].Rows.Count != 0)
                {
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);
                    return oDSResult;
                }
                else
                {
                    SAPbobsCOM.Company oDIComapny;
                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling ConnectToTargetCompany() ", sFuncName);
                    oDIComapny = ConnectToTargetCompany(sCompanyDB);

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling CreateDeliverOrderDraft() ", sFuncName);
                    if (oDeliveryOrder.CreateDeliverOrderDraft(oDIComapny, sUserRole, sOutlet, sConnString, sErrDesc) != "SUCCESS") throw new ArgumentException();

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling 2nd Get_StockTakeCounting() ", sFuncName);
                    oDSResult = oDeliveryOrder.Get_StockTakeCounting(sOutlet, sUserRole, sStatus, sDocEntry, sConnString, sErrDesc);

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed 2nd Get_StockTakeCounting() ", sFuncName);

                    if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);
                    return oDSResult;
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


        [WebMethod]
        public string StockTakeApprove(DataSet oDataset, string sOutlet, string sCompanyDB)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDTCompanyList = new DataSet();
            DataSet oDSResult = new DataSet();
            // string sConnString = string.Empty;
            try
            {
                sFuncName = "StockTakeApprove()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);
                sConnString = System.Configuration.ConfigurationManager.AppSettings["INTDBConnect"].ToString();


                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling StockTakeApprove() ", sFuncName);

                sReturnValue = oDeliveryOrder.StockTakeApprove(oDataset, sOutlet, sCompanyDB, sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                return sReturnValue;

            }

            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                return sErrDesc;
            }
        }

        [WebMethod]
        public DataSet Get_StockTakeCountingDetails(string sCompanyDB, string sOutlet, string sUserRole, string sStatus, string sDocEntry
                                                    , string sDocOwner, string sRequester, string sDocDate)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDTCompanyList = new DataSet();
            DataSet oDSResult = new DataSet();
            string sINTConnString = string.Empty;
            try
            {
                sFuncName = "Get_StockTakeCountingDetails()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);
                oDTCompanyList = Get_Company_Details();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);
                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);
                oDTCompanyList.Dispose();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling oStockTakeCount.Get_StockTakeCounting() ", sFuncName);
                oDSResult = oStockTakeCount.Get_StockTakeCounting(sOutlet, sUserRole, sStatus, sDocEntry, sDocDate, sConnString, sErrDesc);

                return oDSResult;
            }

            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oDTCompanyList.Dispose();
                oDSResult.Dispose();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        [WebMethod]
        public DataSet Create_StockTakeCounting(string sCompanyDB, string sOutlet, string sUserRole, string sStatus, string sDocEntry
                                                    , string sDocOwner, string sRequester, string sDocDate)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDTCompanyList = new DataSet();
            DataSet oDSResult = new DataSet();
            string sINTConnString = string.Empty;
            try
            {
                sFuncName = "Create_StockTakeCounting()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);
                oDTCompanyList = Get_Company_Details();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);
                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                oDTCompanyList.Dispose();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Assign the Connection String from Web Config File ", sFuncName);
                sINTConnString = System.Configuration.ConfigurationManager.AppSettings["INTDBWEB"].ToString();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling oStockTakeCount.Create_DeliverOrderDraft() ", sFuncName);

                if (oStockTakeCount.Create_DeliverOrderDraft(sUserRole, sOutlet, sCompanyDB, sDocOwner, sConnString, sINTConnString
                                                , sRequester, sDocDate, sErrDesc) != "SUCCESS") throw new ArgumentException(sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling oStockTakeCount.Get_StockTakeCounting() ", sFuncName);
                oDSResult = oStockTakeCount.Get_StockTakeCounting(sOutlet, sUserRole, sStatus, sDocEntry, sDocDate, sConnString, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed 2nd Get_StockTakeCounting() ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);
                return oDSResult;

            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oDTCompanyList.Dispose();
                oDSResult.Dispose();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }
        [WebMethod]
        public string Update_StockTakeApprove(DataSet oDataset, string sCompanyDB, string sStatus, string sUserRole, Boolean bIsApprove
                                                , string sDocOwner, string sRequester, string sOutlet)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            //DataSet oDTCompanyList = new DataSet();
            //DataSet oDSResult = new DataSet();
            string sINTConnString = string.Empty;
            try
            {
                sFuncName = "Update_StockTakeApprove()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Assign the Connection String from Web Config File ", sFuncName);
                sINTConnString = System.Configuration.ConfigurationManager.AppSettings["INTDBWEB"].ToString();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling oStockTakeCount.Save_DeliveryOrderDraft() ", sFuncName);

                sReturnValue = oStockTakeCount.Save_DeliveryOrderDraft(oDataset, sStatus, sUserRole, sOutlet, sDocOwner, sRequester, sINTConnString, sCompanyDB, bIsApprove, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);
                return sReturnValue;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                return sErrDesc;
            }
        }

        public SAPbobsCOM.Company ConnectToTargetCompany(string sCompanyDB)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDTCompanyList = new DataSet();
            DataSet oDSResult = new DataSet();
            //// string sConnString = string.Empty;
            DataView oDTView = new DataView();

            try
            {
                sFuncName = "ConnectToTargetCompany()";


                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);
                oDTCompanyList = Get_Company_Details();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Filter Based on Company DB() ", sFuncName);
                oDTView = oDTCompanyList.Tables[0].DefaultView;
                oDTView.RowFilter = "U_DBName= '" + sCompanyDB + "'";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling ConnectToTargetCompany() ", sFuncName);

                sConnString = oDTView[0]["U_ConnString"].ToString();

                oDIComapny = oCommon.ConnectToTargetCompany(oDIComapny, oDTView[0]["U_SAPUserName"].ToString(), oDTView[0]["U_SAPPassword"].ToString()
                                   , oDTView[0]["U_DBName"].ToString(), oDTView[0]["U_Server"].ToString(), oDTView[0]["U_LicenseServer"].ToString()
                                   , oDTView[0]["U_DBUserName"].ToString(), oDTView[0]["U_DBPassword"].ToString(), sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);

                //SAPCompany(oDIComapny, sConnString);

                oDTCompanyList.Dispose();
                oDSResult.Dispose();
                return oDIComapny;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                throw Ex;
            }

        }

        [WebMethod]
        public string UpdateDOStatus(DataSet oDataset, string sCompanyDB, string sStatus, string sUserRole, Boolean bIsApprove)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDTCompanyList = new DataSet();
            DataSet oDSResult = new DataSet();
            // string sConnString = string.Empty;
            try
            {
                sFuncName = "UpdateDOStatus()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);
                oDTCompanyList = Get_Company_Details();

                SAPbobsCOM.Company oDIComapny;

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling ConnectToTargetCompany() ", sFuncName);
                oDIComapny = ConnectToTargetCompany(sCompanyDB);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling UpdateDOStatus() ", sFuncName);
                sReturnValue = oDeliveryOrder.UpdateDOStatus(oDataset, oDIComapny, sStatus, sUserRole, bIsApprove, sErrDesc);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);
                return sReturnValue;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                return sErrDesc;
            }
        }

        public string Get_NewConnectionString()
        {
            string sFuncName = string.Empty;
            try
            {
                sFuncName = "Get_NewConnectionString()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                string DatabaseName = string.Empty, SAPUserID = string.Empty, SAPPassWord = string.Empty,
                                                  SQLUser = string.Empty, SQLPwd = string.Empty, SQLServer = string.Empty, LicenseServer = string.Empty;
                Int32 sqlType = 0;
                string newConStr = string.Empty;
                string con = System.Configuration.ConfigurationManager.AppSettings.Get("DBConnect");
                string[] MyArr = con.Split(';');
                if (MyArr.Length > 0)
                {

                    DatabaseName = MyArr[0].ToString();
                    SAPUserID = MyArr[1].ToString();
                    SAPPassWord = MyArr[2].ToString();
                    SQLServer = MyArr[3].ToString();
                    SQLUser = MyArr[4].ToString();
                    SQLPwd = MyArr[5].ToString();
                    LicenseServer = MyArr[6].ToString();
                    sqlType = int.Parse(MyArr[7]);
                }

                newConStr = "server= " + SQLServer + ";database=" + DatabaseName + " ;uid=" + SQLUser + "; pwd=" + SQLPwd + ";";
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS  ", sFuncName);
                return newConStr;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                return sErrDesc;
            }

        }

        #endregion Methods

    }
}

