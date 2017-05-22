using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using AE_Harrys.Common;
using AE_Harrys.BLL;
using System.Web.Script.Services;
using System.Web.Script.Serialization;

namespace AE_Harrys.WebService
{
    /// <summary>
    /// Summary description for Mobile
    /// </summary>
    /// 
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Mobile : System.Web.Services.WebService
    {
        string sServer = string.Empty;
        string sLicenseServer = string.Empty;
        string sSQLUserName = string.Empty;
        string sSQLPassword = string.Empty;
        public string sErrDesc = string.Empty;

        public Int16 p_iDebugMode = DEBUG_ON;

        public const Int16 RTN_SUCCESS = 1;
        public const Int16 RTN_ERROR = 0;
        public const Int16 DEBUG_ON = 1;
        public const Int16 DEBUG_OFF = 0;


        Master oMaster = new Master();
        clsCommon oCommon = new clsCommon();
        clsInventory oInventory = new clsInventory();
        clsPurchaseOrder oPurchaseOrder = new clsPurchaseOrder();
        clsLog oLog = new clsLog();
        clsLogin oUserInformation = new clsLogin();


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string MGet_Login(string sUserName, string sPassword, string sCompanyDB)
        {
            string sResult = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDTCompanyList = new DataSet();
            string sConnString = string.Empty;
            string sFuncName = string.Empty;

            try
            {
                sFuncName = "MGet_Login";
                oDTCompanyList = oMaster.Get_Company_Details();
                sReturnValue = oCommon.MAcknowledgment(oDTCompanyList, sUserName, sPassword, sCompanyDB, sErrDesc);

                if (sReturnValue.ToString().ToUpper() == "SUCCESS")
                {
                    sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                    DataSet oDSUserInfo = oUserInformation.Get_UserInformation(sUserName, sConnString, sErrDesc);

                    if (oDSUserInfo != null && oDSUserInfo.Tables[0].Rows.Count > 0)
                    {
                        oLog.WriteToLogFile_Debug("Dataset is not null ", sFuncName);

                        List<UserInformation> lst = new List<UserInformation>();
                        foreach (DataRow r in oDSUserInfo.Tables[0].Rows)
                        {
                            UserInformation oUserInfo = new UserInformation();

                            oUserInfo.EmpID = r["EmpID"].ToString();
                            oUserInfo.EmpName = r["EmployeeName"].ToString();
                            oUserInfo.Outlet = r["WhsCode"].ToString();
                            oUserInfo.OutletName = r["WhsName"].ToString();
                            oUserInfo.UserAccess = r["U_AE_Access"].ToString();
                            oUserInfo.SuperUser = r["SUPERUSER"].ToString();
                            oUserInfo.UserRole = r["U_UserRole"].ToString();
                            oUserInfo.ApprovalLevel = r["U_ApprovalLevel"].ToString();
                            oUserInfo.CompanyName = r["CompanyName"].ToString();

                            lst.Add(oUserInfo);

                            break;

                        }
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        oLog.WriteToLogFile_Debug("Completed With SUCCESS", sFuncName);
                        sResult = js.Serialize(lst);
                    }

                }
                else
                {
                    sResult = sReturnValue;
                }

            }
            catch (Exception ex)
            {
                sResult = ex.Message.ToString();
            }
            finally
            {
                oDTCompanyList.Dispose();
            }

            return sResult;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string MGet_UserAcknowledgement(string sUserName, string sPassword, string sOutlet, string sCompanyDB)
        {
            string sResult = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDTCompanyList = new DataSet();
            string sConnString = string.Empty;
            string sFuncName = string.Empty;

            try
            {
                sFuncName = "MGet_UserAcknowledgement";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);
                oDTCompanyList = oMaster.Get_Company_Details();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);
                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);
                oDTCompanyList.Dispose();
                sReturnValue = oUserInformation.MUser_Acknowlwdge(sUserName, sPassword, sOutlet, sConnString, sErrDesc);

                List<ReceiveInOutlet> lst = new List<ReceiveInOutlet>();

                ReceiveInOutlet _ReceiveInOutlet = new ReceiveInOutlet();
                _ReceiveInOutlet.sReturnValue = sReturnValue.ToString();

                lst.Add(_ReceiveInOutlet);

                JavaScriptSerializer js = new JavaScriptSerializer();
                sResult = js.Serialize(lst);
            }

            catch (Exception ex)
            {
                List<ReceiveInOutlet> lst = new List<ReceiveInOutlet>();

                ReceiveInOutlet _ReceiveInOutlet = new ReceiveInOutlet();
                _ReceiveInOutlet.sReturnValue = ex.Message;

                lst.Add(_ReceiveInOutlet);

                JavaScriptSerializer js = new JavaScriptSerializer();
                sResult = js.Serialize(lst);
            }
            return sResult;
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string MGet_OutletList(string sUserName, string sPassword, string sCompanyDB)
        {
            string sResult = string.Empty;
            clsLog oLog = new clsLog();
            try
            {
                oLog.WriteToLogFile_Debug("Starting Function", "MGet_OutletList");

                DataSet oDTCompanyList = new DataSet();
                oLog.WriteToLogFile_Debug("Calling Company Details", "MGet_OutletList");

                oDTCompanyList = oMaster.Get_Company_Details();

                oLog.WriteToLogFile_Debug("Completed company details", "MGet_OutletList");

                DataSet ds = oCommon.MGetOutlet(oDTCompanyList, sUserName, sPassword, sCompanyDB, sErrDesc);
                oDTCompanyList.Dispose();

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    oLog.WriteToLogFile_Debug("Dataset is not null ", "MGet_OutletList");

                    List<Outlet> lst = new List<Outlet>();
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        Outlet _outlet = new Outlet();
                        _outlet.WhsCode = r["WhsCode"].ToString();
                        _outlet.WhsName = r["WhsName"].ToString();
                        lst.Add(_outlet);
                    }
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    oLog.WriteToLogFile_Debug("Completed With SUCCESS", "MGet_OutletList");
                    sResult = js.Serialize(lst);
                }
                ds.Dispose();
            }

            catch (Exception ex)
            {
                oLog.WriteToLogFile_Debug("Completed with ERROR", "MGet_OutletList");
                //throw ex;
                sResult = ex.Message.ToString();
            }
            return sResult;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string MGet_CompanyList()
        {
            string sResult = string.Empty;
            try
            {
                DataSet ds = oMaster.Get_Company_Details();
                if (ds != null && ds.Tables.Count > 0)
                {
                    List<Company> lst = new List<Company>();
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        Company _company = new Company();
                        _company.U_DBName = r["U_DBName"].ToString();
                        _company.U_CompName = r["U_CompName"].ToString();
                        _company.U_SAPUserName = r["U_SAPUserName"].ToString();
                        _company.U_SAPPassword = r["U_SAPPassword"].ToString();
                        _company.U_DBUserName = r["U_DBUserName"].ToString();
                        _company.U_DBPassword = r["U_DBPassword"].ToString();
                        _company.U_ConnString = r["U_ConnString"].ToString();
                        _company.U_Server = r["U_Server"].ToString();
                        _company.U_LicenseServer = r["U_LicenseServer"].ToString();
                        lst.Add(_company);
                    }
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    sResult = js.Serialize(lst);
                }
                ds.Dispose();
            }

            catch (Exception ex)
            {
                List<ReceiveInOutlet> lst = new List<ReceiveInOutlet>();

                ReceiveInOutlet _ReceiveInOutlet = new ReceiveInOutlet();
                _ReceiveInOutlet.sReturnValue = ex.Message;

                lst.Add(_ReceiveInOutlet);

                JavaScriptSerializer js = new JavaScriptSerializer();
                sResult = js.Serialize(lst);

            }
            return sResult;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string MGet_InventoryRequest(string sCompanyDB, string sOutlet)
        {
            string sResult = string.Empty;
            DataSet oDTCompanyList = new DataSet();
            string sConnString = string.Empty;

            try
            {
                oDTCompanyList = oMaster.Get_Company_Details();

                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                oDTCompanyList.Dispose();
                //DataSet ds = oMaster.Get_InventoryRequest(sCompanyDB, sOutlet);
                DataSet ds = oInventory.Get_OpenTRList(sOutlet, sConnString, sErrDesc);// Get_InventoryRequest(sCompanyDB, sOutlet);

                if (ds != null && ds.Tables.Count > 0)
                {
                    List<InventoryRequest> lst = new List<InventoryRequest>();
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        InventoryRequest _invRequest = new InventoryRequest();
                        _invRequest.RequestNo = r["RequestNo"] == DBNull.Value ? 0 : Convert.ToInt32(r["RequestNo"]);
                        _invRequest.Date = Convert.ToDateTime(r["Date"]);
                        _invRequest.ToOutlet = r["ToOutlet"].ToString();
                        _invRequest.SYS2RequestNo = r["SYS2RequestNo"] == DBNull.Value ? 0 : Convert.ToInt32(r["SYS2RequestNo"]);
                        // _invRequest.DocStatus = r["DocStatus"].ToString();
                        lst.Add(_invRequest);
                    }
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    sResult = js.Serialize(lst);
                }
                ds.Dispose();
            }

            catch (Exception ex)
            {
                sResult = ex.Message.ToString();
            }
            return sResult;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string MGet_InventoryRequestDetails(string sCompanyDB, string sRequestNo)
        {
            string sResult = string.Empty;
            DataSet oDTCompanyList = new DataSet();
            string sConnString = string.Empty;

            try
            {
                oDTCompanyList = oMaster.Get_Company_Details();

                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                oDTCompanyList.Dispose();
                DataSet ds = oInventory.Get_TransferRequest_ItemList(sRequestNo, sConnString, sErrDesc);


                if (ds != null && ds.Tables.Count > 0)
                {
                    List<InventoryRequestDetails> lst = new List<InventoryRequestDetails>();
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        InventoryRequestDetails _invRequest = new InventoryRequestDetails();
                        _invRequest.RequestNo = r["RequestNo"] == DBNull.Value ? 0 : Convert.ToInt32(r["RequestNo"]);
                        _invRequest.ItemCode = r["ItemCode"].ToString();
                        _invRequest.ItemName = r["ItemName"].ToString();
                        _invRequest.OpenQty = Convert.ToDecimal(r["OpenQty"]);
                        //_invRequest.BatchNum = r["BatchNum"].ToString();
                        //_invRequest.BaseEntry = r["BaseEntry"] == DBNull.Value ? 0 : Convert.ToInt32(r["BaseEntry"]);
                        //_invRequest.BaseLine = r["BaseLine"] == DBNull.Value ? 0 : Convert.ToInt32(r["BaseLine"]);
                        lst.Add(_invRequest);
                    }
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    sResult = js.Serialize(lst);
                }
                ds.Dispose();
            }

            catch (Exception ex)
            {
                sResult = ex.Message.ToString();
            }
            return sResult;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string MGet_OpenPOList(string sCompanyDB, string sSupplier, string sSupplierType, string sAccessType, string sOutlet)
        {
            string sResult = string.Empty;
            try
            {
                DataSet ds = oMaster.Get_OpenPOList(sCompanyDB, sSupplier, sSupplierType, sAccessType, sOutlet);
                if (ds != null && ds.Tables.Count > 0)
                {
                    List<openPOList> lst = new List<openPOList>();
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        openPOList _openPOList = new openPOList();
                        _openPOList.CardCode = r["CardCode"].ToString();
                        _openPOList.CardName = r["CardName"].ToString();
                        _openPOList.NoOfOpenPO = r["NoOfOpenPO"] == DBNull.Value ? 0 : Convert.ToInt32(r["NoOfOpenPO"]);
                        lst.Add(_openPOList);
                    }
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    sResult = js.Serialize(lst);
                }
                ds.Dispose();
            }

            catch (Exception ex)
            {
                sResult = ex.Message.ToString();
            }
            return sResult;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string MGet_ReceiveInOutlet_Details(string sCompanyDB, string sOutlet, string sAccessType, string sSupplier)
        {
            string sResult = string.Empty;
            try
            {
                DataSet ds = oMaster.Get_ReceiveInOutlet_Details(sCompanyDB, sOutlet, sAccessType, sSupplier);
                if (ds != null && ds.Tables.Count > 0)
                {
                    List<ReceiveIntoOutlet> lst = new List<ReceiveIntoOutlet>();
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        ReceiveIntoOutlet _receiveIntoOutlet = new ReceiveIntoOutlet();
                        _receiveIntoOutlet.DocEntry = r["DocEntry"] == DBNull.Value ? 0 : Convert.ToInt32(r["DocEntry"]);
                        _receiveIntoOutlet.DocNum = r["DocNum"] == DBNull.Value ? 0 : Convert.ToInt32(r["DocNum"]);
                        _receiveIntoOutlet.ItemCode = r["ItemCode"].ToString();
                        _receiveIntoOutlet.Dscription = r["Dscription"].ToString();
                        _receiveIntoOutlet.OpenQty = Convert.ToDecimal(r["Quantity"]);
                        _receiveIntoOutlet.ImageURL = r["ImageURL"].ToString();
                        _receiveIntoOutlet.BaseType = r["BaseType"] == DBNull.Value ? 0 : Convert.ToInt32(r["BaseType"]);
                        _receiveIntoOutlet.BaseEntry = r["BaseEntry"] == DBNull.Value ? 0 : Convert.ToInt32(r["BaseEntry"]);
                        _receiveIntoOutlet.BaseLine = r["BaseLine"] == DBNull.Value ? 0 : Convert.ToInt32(r["BaseLine"]);
                        _receiveIntoOutlet.LineNum = r["LineNum"] == DBNull.Value ? 0 : Convert.ToInt32(r["LineNum"]);
                        lst.Add(_receiveIntoOutlet);
                    }
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    sResult = js.Serialize(lst);
                }
                ds.Dispose();
            }

            catch (Exception ex)
            {
                sResult = ex.Message.ToString();
            }
            return sResult;
        }

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public string MGet_ItemList_WithOutPO(string sCompanyDB)
        //{
        //    try
        //    {
        //        DataSet ds = oMaster.Get_ItemList(sCompanyDB);
        //        if (ds != null && ds.Tables.Count > 0)
        //        {
        //            List<ItemListWithoutPO> lst = new List<ItemListWithoutPO>();
        //            foreach (DataRow r in ds.Tables[0].Rows)
        //            {
        //                ItemListWithoutPO _itemList = new ItemListWithoutPO();
        //                _itemList.ItemCode = r["ItemCode"].ToString();
        //                _itemList.ItemName = r["ItemName"].ToString();
        //                _itemList.frozenFor = r["frozenFor"].ToString();
        //                lst.Add(_itemList);
        //            }
        //            JavaScriptSerializer js = new JavaScriptSerializer();
        //            return js.Serialize(lst);
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return string.Empty;
        //}

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string MGet_ReasonCode(string sCompanyDB, string sOutlet)
        {
            string sResult = string.Empty;
            try
            {
                DataSet ds = oMaster.Get_ReasonDetails(sCompanyDB, sOutlet);
                if (ds != null && ds.Tables.Count > 0)
                {
                    List<ReasonCode> lst = new List<ReasonCode>();
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        ReasonCode _reasonCode = new ReasonCode();
                        _reasonCode.Code = r["Code"].ToString();
                        _reasonCode.Name = r["Name"].ToString();
                        lst.Add(_reasonCode);
                    }
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    sResult = js.Serialize(lst);
                }
                ds.Dispose();
            }

            catch (Exception ex)
            {
                //  throw ex;
                sResult = ex.Message.ToString();
            }
            return sResult;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string MGet_ReceiveInOutlet(string sCompanyDB, string sJSONFile)
        {
            //DataTable oDTRecInOutlet = new DataTable();
            DataSet oDSRecInOutlet = new DataSet();
            DataSet oDTCompanyList = new DataSet();
            SAPbobsCOM.Company oDICompany;
            string sReturnValue = string.Empty;
            string sConnString = string.Empty;
            string sFuncName = string.Empty;
            string sResult = string.Empty;

            try
            {
                sFuncName = "MGet_ReceiveInOutlet()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("JSON File : " + sJSONFile, sFuncName);

                var oReceiveInOutlet = new clsCopyMarketingDocument();


                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_Company_Details() ", sFuncName);
                oDTCompanyList = oMaster.Get_Company_Details();

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Get_ConnectionString() ", sFuncName);
                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                oDICompany = oMaster.ConnectToTargetCompany(sCompanyDB);

                //Convert JSON to Array

                JavaScriptSerializer jss = new JavaScriptSerializer();
                List<GRPO> grpo = jss.Deserialize<List<GRPO>>(sJSONFile);


                if (grpo.Count > 0)
                {
                    GRPO grpo1 = grpo[0];
                    List<GrpoLine> grpoLine1 = grpo1.GrpoLine;


                    string sCardCode = grpo1.CardCode;
                    string sRequestNo = grpo1.RequestNo;
                    string sOutlet = grpo1.WhsCode;
                    string CreatedUser = grpo1.CreatedUser;
                    string sReceiptDate = grpo1.ReceiptDate;

                    DataTable FormatJsontable = ConvertJSONtoDataTable(grpoLine1, sCardCode, sOutlet, sRequestNo, CreatedUser, sReceiptDate);

                    oDSRecInOutlet.Tables.Add(FormatJsontable);


                }

                sReturnValue = oReceiveInOutlet.ReceiveInOutlet(oDSRecInOutlet, oDICompany, sConnString, sErrDesc);
                oDICompany.Disconnect();
                oDICompany = null;

                List<ReceiveInOutlet> lst = new List<ReceiveInOutlet>();

                ReceiveInOutlet _ReceiveInOutlet = new ReceiveInOutlet();
                _ReceiveInOutlet.sReturnValue = sReturnValue.ToString();

                lst.Add(_ReceiveInOutlet);

                JavaScriptSerializer js = new JavaScriptSerializer();
                sResult = js.Serialize(lst);

            }

            catch (Exception ex)
            {
                oDSRecInOutlet.Dispose();
                oDTCompanyList.Dispose();
                List<ReceiveInOutlet> lst = new List<ReceiveInOutlet>();

                ReceiveInOutlet _ReceiveInOutlet = new ReceiveInOutlet();
                _ReceiveInOutlet.sReturnValue = ex.Message;

                lst.Add(_ReceiveInOutlet);

                JavaScriptSerializer js = new JavaScriptSerializer();
                sResult = js.Serialize(lst);
            }
            return sResult;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string MGet_ReceiveInVAN(string sCompanyDB, string sJSONFile)
        {
            //DataTable oDTRecInOutlet = new DataTable();
            DataSet oDSRecIntoVAN = new DataSet();
            DataSet oDTCompanyList = new DataSet();
            SAPbobsCOM.Company oDICompany;
            string sReturnValue = string.Empty;
            string sConnString = string.Empty;
            string sFuncName = string.Empty;

            try
            {
                sFuncName = "MGet_ReceiveInVAN()";

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("Starting Function ", sFuncName);

                var oReceiveInOutlet = new clsCopyMarketingDocument();

                oDTCompanyList = oMaster.Get_Company_Details();

                sConnString = oCommon.Get_ConnectionString(oDTCompanyList, sCompanyDB, sErrDesc);

                oDICompany = oMaster.ConnectToTargetCompany(sCompanyDB);

                if (p_iDebugMode == DEBUG_ON) oLog.WriteToLogFile_Debug("JSON File : " + sJSONFile, sFuncName);

                //Convert JSON to Array

                JavaScriptSerializer jss = new JavaScriptSerializer();
                List<Inventory> Inventory = jss.Deserialize<List<Inventory>>(sJSONFile);

                if (Inventory.Count > 0)
                {
                    Inventory Inventory1 = Inventory[0];
                    List<InventoryLine> InventoryLine1 = Inventory1.InventoryTransfer;

                    string sCardCode = Inventory1.CardCode;
                    string sRequestNo = Inventory1.RequestNo;
                    string sOutlet = Inventory1.WhsCode;
                    string sReceiptDate = Inventory1.ReceiptDate;

                    DataTable FormatJsontable = ConvertJSONtoDataTable_VAN(InventoryLine1, sCardCode, sOutlet, sRequestNo, sReceiptDate);

                    oDSRecIntoVAN.Tables.Add(FormatJsontable);
                    FormatJsontable.Dispose();
                }

                sReturnValue = oReceiveInOutlet.ReceiveIntoVAN(oDSRecIntoVAN, oDICompany, sConnString, sErrDesc);

                oDICompany.Disconnect();
                oDICompany = null;

                List<ReceiveInOutlet> lst = new List<ReceiveInOutlet>();

                ReceiveInOutlet _ReceiveInOutlet = new ReceiveInOutlet();
                _ReceiveInOutlet.sReturnValue = sReturnValue.ToString();

                lst.Add(_ReceiveInOutlet);

                JavaScriptSerializer js = new JavaScriptSerializer();
                return js.Serialize(lst);

            }

            catch (Exception ex)
            {
                oDSRecIntoVAN.Dispose();
                oDTCompanyList.Dispose();
                List<ReceiveInOutlet> lst = new List<ReceiveInOutlet>();

                ReceiveInOutlet _ReceiveInOutlet = new ReceiveInOutlet();
                _ReceiveInOutlet.sReturnValue = ex.Message;

                lst.Add(_ReceiveInOutlet);

                JavaScriptSerializer js = new JavaScriptSerializer();
                return js.Serialize(lst);
                //throw ex;
            }
            //return string.Empty;
        }

        private DataTable CreateTable()
        {
            DataTable tbReceipt = new DataTable();
            tbReceipt.Columns.Add("LineNum");
            tbReceipt.Columns.Add("DocEntry");
            tbReceipt.Columns.Add("SupplierCode");
            tbReceipt.Columns.Add("Description");
            tbReceipt.Columns.Add("Order Qty");
            tbReceipt.Columns.Add("Receipt Qty");
            tbReceipt.Columns.Add("Reason Code");
            tbReceipt.Columns.Add("ItemCode");
            tbReceipt.Columns.Add("Outlet");
            tbReceipt.Columns.Add("RequestNo");
            tbReceipt.Columns.Add("CloseStatus");
            tbReceipt.Columns.Add("OutletReceivedBy");
            tbReceipt.Columns.Add("RequestDate");
            return tbReceipt;
        }

        private DataTable ConvertJSONtoDataTable_VAN(List<InventoryLine> Line, string sCardCode, string sOutlet, string sRequestNum, string sRequestDate)
        {
            DataTable tbNew = new DataTable();
            tbNew = CreateTable();
            int RowCount = 0;

            foreach (InventoryLine Item in Line)
            {
                DataRow rowNew = tbNew.NewRow();
                rowNew["LineNum"] = RowCount;
                rowNew["DocEntry"] = Item.DocEntry;
                rowNew["SupplierCode"] = sCardCode;
                rowNew["Description"] = Item.Dscription;

                // rowNew["Order Qty"] = item["Quantity"];
                rowNew["Receipt Qty"] = Item.ReceiptQty;
                rowNew["Reason Code"] = Item.ReasonCode;
                rowNew["ItemCode"] = Item.ItemCode;
                rowNew["Outlet"] = sOutlet;
                rowNew["RequestNo"] = sRequestNum;
                rowNew["RequestDate"] = sRequestDate;
                rowNew["CloseStatus"] = Item.CloseStatus;

                tbNew.Rows.Add(rowNew);
                RowCount = RowCount + 1;

            }



            return tbNew.Copy();
        }

        private DataTable ConvertJSONtoDataTable(List<GrpoLine> Line, string sCardCode, string sOutlet,
                                                 string sRequestNum, string CreatedUser, string sReceiptDate)
        {
            DataTable tbNew = new DataTable();
            tbNew = CreateTable();
            int RowCount = 0;

            foreach (GrpoLine Item in Line)
            {
                DataRow rowNew = tbNew.NewRow();
                rowNew["LineNum"] = Item.LineNum;
                rowNew["DocEntry"] = Item.DocEntry;
                rowNew["SupplierCode"] = sCardCode;
                rowNew["Description"] = Item.Dscription;

                // rowNew["Order Qty"] = item["Quantity"];
                rowNew["Receipt Qty"] = Item.ReceiptQty;
                rowNew["Reason Code"] = Item.ReasonCode;
                rowNew["ItemCode"] = Item.ItemCode;
                rowNew["Outlet"] = sOutlet;
                rowNew["RequestNo"] = sRequestNum;
                rowNew["CloseStatus"] = Item.CloseStatus;
                rowNew["OutletReceivedBy"] = CreatedUser;
                rowNew["RequestDate"] = sReceiptDate;

                tbNew.Rows.Add(rowNew);
                RowCount = RowCount + 1;

            }



            return tbNew.Copy();
        }

        public DataTable ConvertJsontable(DataTable dt)
        {
            DataTable tbNew = new DataTable();
            tbNew = CreateTable();
            int RowCount = 0;
            foreach (DataRow item in dt.Rows)
            {
                DataRow rowNew = tbNew.NewRow();
                rowNew["LineNum"] = RowCount;
                rowNew["DocEntry"] = item["DocEntry"];
                rowNew["SupplierCode"] = item["CardCode"];
                rowNew["Description"] = item["Dscription"];

                // rowNew["Order Qty"] = item["Quantity"];
                rowNew["Receipt Qty"] = item["ReceiptQty"];
                rowNew["Reason Code"] = item["ReasonCode"];
                rowNew["ItemCode"] = item["ItemCode"];
                rowNew["Outlet"] = item["WhsCode"];

                if (dt.Columns["RequestNo"] != null)
                    rowNew["RequestNo"] = item["RequestNo"];

                if (dt.Columns["CloseStatus"] != null)
                    rowNew["CloseStatus"] = item["CloseStatus"];

                tbNew.Rows.Add(rowNew);
                RowCount = RowCount + 1;
            }
            return tbNew.Copy();
        }
    }

    class Company
    {
        public string U_DBName { get; set; }
        public string U_CompName { get; set; }
        public string U_SAPUserName { get; set; }
        public string U_SAPPassword { get; set; }
        public string U_DBUserName { get; set; }
        public string U_DBPassword { get; set; }
        public string U_ConnString { get; set; }
        public string U_Server { get; set; }
        public string U_LicenseServer { get; set; }
    }

    class UserInformation
    {
        public string EmpID { get; set; }
        public string EmpName { get; set; }
        public string Outlet { get; set; }
        public string OutletName { get; set; }
        public string UserAccess { get; set; }
        public string SuperUser { get; set; }
        public string UserRole { get; set; }
        public string ApprovalLevel { get; set; }
        public string CompanyName { get; set; }
    }

    class Outlet
    {
        public string WhsCode { get; set; }
        public string WhsName { get; set; }
    }

    class InventoryRequest
    {
        public int RequestNo { get; set; }
        public DateTime Date { get; set; }
        public string ToOutlet { get; set; }
        public int SYS2RequestNo { get; set; }
        //public string DocStatus { get; set; }
    }

    class GRPO
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string NoOfOpenPO { get; set; }
        public string RequestNo { get; set; }
        public string WhsCode { get; set; }
        public string CreatedUser { get; set; }
        public string ReceiptDate { get; set; }

        public List<GrpoLine> GrpoLine { get; set; }
    }

    class GrpoLine
    {
        public string DocEntry { get; set; }
        public string ItemCode { get; set; }
        public string LineNum { get; set; }
        public string Dscription { get; set; }
        public string Quantity { get; set; }
        public string ImageUrl { get; set; }
        public string BatchNum { get; set; }
        public string BaseEntry { get; set; }
        public string BaseLine { get; set; }
        public string ReceiptQty { get; set; }
        public string LineStatus { get; set; }
        public string ReasonCode { get; set; }
        public string ReasonName { get; set; }
        public string CloseStatus { get; set; }
    }

    class Inventory
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string RequestNo { get; set; }
        public string WhsCode { get; set; }
        public string ReceiptDate { get; set; }
        public List<InventoryLine> InventoryTransfer { get; set; }
    }

    class InventoryLine
    {

        public string DocEntry { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string Quantity { get; set; }
        public string ImageUrl { get; set; }
        public string BatchNum { get; set; }
        public string BaseEntry { get; set; }
        public string BaseLine { get; set; }
        public string ReceiptQty { get; set; }
        public string LineStatus { get; set; }
        public string ReasonCode { get; set; }
        public string ReasonName { get; set; }
        public string CloseStatus { get; set; }
    }

    class InventoryRequestDetails
    {
        public int RequestNo { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal OpenQty { get; set; }

        //public string BatchNum { get; set; }
        //public int BaseEntry { get; set; }
        //public int BaseLine { get; set; }
    }

    class openPOList
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public int NoOfOpenPO { get; set; }
    }

    class ReceiveIntoOutlet
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public decimal OpenQty { get; set; }
        public string ImageURL { get; set; }
        public int BaseType { get; set; }
        public int BaseEntry { get; set; }
        public int BaseLine { get; set; }
        public int LineNum { get; set; }
    }

    //class ItemListWithoutPO
    //{
    //    public string ItemCode { get; set; }
    //    public string ItemName { get; set; }
    //    public string frozenFor { get; set; }
    //}

    class ReasonCode
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    class ReceiveInOutlet
    {
        public string sReturnValue { get; set; }
    }
}
