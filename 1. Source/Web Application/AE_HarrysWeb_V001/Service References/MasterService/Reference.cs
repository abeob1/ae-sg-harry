﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AE_HarrysWeb_V001.MasterService {
    using System.Data;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="MasterService.MasterSoap")]
    public interface MasterSoap {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Create_InventoryTransferRequest", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string Create_InventoryTransferRequest(string sCompanyDB, System.Data.DataSet oDSInvReqData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_OpenTransferRequest", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_OpenTransferRequest(string sCompanyDB, string sFromOutlet, string sToOutlet, string sStatus, string sFromDate, string sToDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_OpenTransferRequestDetails", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_OpenTransferRequestDetails(string sCompanyDB, string sDocEntry);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_OutletListPR", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_OutletListPR(string sGroupType, string sCompanyDB);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Approve_InventoryTransferRequest", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string Approve_InventoryTransferRequest(string sCompanyDB, System.Data.DataSet oDSInvTransData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Reject_InventoryTransferRequest", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string Reject_InventoryTransferRequest(string sCompanyDB, System.Data.DataSet oDSInvTransData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_InventoryTransferRequest_ItemSearch", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_InventoryTransferRequest_ItemSearch(string sCompanyDB, string sFromOutlet, string sToOutlet, string sGroupType);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Insert_PurchaseRequest", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string Insert_PurchaseRequest(System.Data.DataSet oDataset, bool IsDraft, string sCompanyDB, bool IsAdd, bool bIsSubmit, bool bIsDelCharge);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Update_PurchaseRequest", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string Update_PurchaseRequest(System.Data.DataSet oDataset, bool IsDraft, string sCompanyDB, bool IsAdd, bool bIsSubmit, bool bIsDelCharge);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Submit_PurchaseRequest", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string Submit_PurchaseRequest(System.Data.DataSet oDataset, string sCompanyDB);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Approve_PurchaseRequest", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string Approve_PurchaseRequest(System.Data.DataSet oDataset, string sCompanyDB);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Delete_PurchaseRequest", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string Delete_PurchaseRequest(System.Data.DataSet oDataset, string sCompanyDB);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_ApprovalStatus_Summary", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_ApprovalStatus_Summary(string sFromDate, string sToDate, string sOutlet, string sConnString);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Login", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string Login(string sUserName, string sPassword, string sDBName, string sServer, string sLicServerName, string sDBUserName, string sDBPassword);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_UserInformation", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_UserInformation(string sUserName, string sConnString);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_Outlet_Details", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_Outlet_Details(string sSuperUser, string sOutletCode, int iApprovalLevel, string sConnString);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_Supplier_Details", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_Supplier_Details(string sUserRole, string sConnString);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_Company_Details", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_Company_Details();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_MaterialReqBySupplier", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_MaterialReqBySupplier(string sSupplier, string sOutlet, string userRole, string sConnString);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_MaterialReqByItem", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_MaterialReqByItem(string sGroupType, string sOutlet, string sConnString);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Create_PurchaseRequest", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string Create_PurchaseRequest(System.Data.DataSet oDataset, bool IsDraft, string sCompanyDB, bool IsAdd, bool bIsSubmit, bool bIsDelCharge);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Delete_Drafts", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string Delete_Drafts(System.Data.DataSet oDataset, string sCompanyDB);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/PRDraft_Submit", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string PRDraft_Submit(System.Data.DataSet oDataset, string sCompanyDB);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_MaterialReqDraft", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_MaterialReqDraft(string sOutlet, string sConnString);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_MaterialReq_Submitted_Approval", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_MaterialReq_Submitted_Approval(string sOutlet, int iStatus, string fromDate, string todate, string sUserRole, string sConnString);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_DraftDetails", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_DraftDetails(string sDocEntry, string sConnString, string sDocType);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_MRSubmit_ApprovedDetails", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_MRSubmit_ApprovedDetails(string sDocEntry, string sDocType, string sConnString);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_MaterialReqBySupplierItemList", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_MaterialReqBySupplierItemList(string sOutlet, string sConnString);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_ItemList", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_ItemList(string sCompanyDB);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_ReasonDetails", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_ReasonDetails(string sCompanyDB, string sOutlet);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ConvertDraftToDocument", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string ConvertDraftToDocument(System.Data.DataSet oDataset, string sCompanyDB);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_InventoryRequest", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_InventoryRequest(string sCompanyDB, string sOutlet);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_InventoryRequestDetails", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_InventoryRequestDetails(string sCompanyDB, string sRequestNo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_OpenPOList", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_OpenPOList(string sCompanyDB, string sSupplier, string sSupplierType, string sAccessType, string sOutletCode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_ReceiveInOutlet_Details", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_ReceiveInOutlet_Details(string sCompanyDB, string sOutlet, string sAccessType, string sSupplier);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_GetPendingDrafts", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_GetPendingDrafts(string sCompanyDB, string sOutlet, string sUserRole, string sApprLevel);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Update_ApprovalStatus", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string Update_ApprovalStatus(System.Data.DataSet oDataset, string sCompanyDB);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ReceiveInOutlet", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string ReceiveInOutlet(System.Data.DataSet oDataset, string sCompanyDB);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_SalesTakingCountList", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_SalesTakingCountList(string sCompanyDB, string sOutlet, string sUserRole);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_StockTakeCounting", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_StockTakeCounting(string sCompanyDB, string sOutlet, string sUserRole, string sStatus, string sDocEntry);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/StockTakeApprove", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string StockTakeApprove(System.Data.DataSet oDataset, string sOutlet, string sCompanyDB);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Get_StockTakeCountingDetails", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Get_StockTakeCountingDetails(string sCompanyDB, string sOutlet, string sUserRole, string sStatus, string sDocEntry, string sDocOwner, string sRequester, string sDocDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Create_StockTakeCounting", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Create_StockTakeCounting(string sCompanyDB, string sOutlet, string sUserRole, string sStatus, string sDocEntry, string sDocOwner, string sRequester, string sDocDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Update_StockTakeApprove", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string Update_StockTakeApprove(System.Data.DataSet oDataset, string sCompanyDB, string sStatus, string sUserRole, bool bIsApprove, string sDocOwner, string sRequester, string sOutlet);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UpdateDOStatus", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string UpdateDOStatus(System.Data.DataSet oDataset, string sCompanyDB, string sStatus, string sUserRole, bool bIsApprove);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface MasterSoapChannel : AE_HarrysWeb_V001.MasterService.MasterSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class MasterSoapClient : System.ServiceModel.ClientBase<AE_HarrysWeb_V001.MasterService.MasterSoap>, AE_HarrysWeb_V001.MasterService.MasterSoap {
        
        public MasterSoapClient() {
        }
        
        public MasterSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public MasterSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public MasterSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public MasterSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string Create_InventoryTransferRequest(string sCompanyDB, System.Data.DataSet oDSInvReqData) {
            return base.Channel.Create_InventoryTransferRequest(sCompanyDB, oDSInvReqData);
        }
        
        public System.Data.DataSet Get_OpenTransferRequest(string sCompanyDB, string sFromOutlet, string sToOutlet, string sStatus, string sFromDate, string sToDate) {
            return base.Channel.Get_OpenTransferRequest(sCompanyDB, sFromOutlet, sToOutlet, sStatus, sFromDate, sToDate);
        }
        
        public System.Data.DataSet Get_OpenTransferRequestDetails(string sCompanyDB, string sDocEntry) {
            return base.Channel.Get_OpenTransferRequestDetails(sCompanyDB, sDocEntry);
        }
        
        public System.Data.DataSet Get_OutletListPR(string sGroupType, string sCompanyDB) {
            return base.Channel.Get_OutletListPR(sGroupType, sCompanyDB);
        }
        
        public string Approve_InventoryTransferRequest(string sCompanyDB, System.Data.DataSet oDSInvTransData) {
            return base.Channel.Approve_InventoryTransferRequest(sCompanyDB, oDSInvTransData);
        }
        
        public string Reject_InventoryTransferRequest(string sCompanyDB, System.Data.DataSet oDSInvTransData) {
            return base.Channel.Reject_InventoryTransferRequest(sCompanyDB, oDSInvTransData);
        }
        
        public System.Data.DataSet Get_InventoryTransferRequest_ItemSearch(string sCompanyDB, string sFromOutlet, string sToOutlet, string sGroupType) {
            return base.Channel.Get_InventoryTransferRequest_ItemSearch(sCompanyDB, sFromOutlet, sToOutlet, sGroupType);
        }
        
        public string Insert_PurchaseRequest(System.Data.DataSet oDataset, bool IsDraft, string sCompanyDB, bool IsAdd, bool bIsSubmit, bool bIsDelCharge) {
            return base.Channel.Insert_PurchaseRequest(oDataset, IsDraft, sCompanyDB, IsAdd, bIsSubmit, bIsDelCharge);
        }
        
        public string Update_PurchaseRequest(System.Data.DataSet oDataset, bool IsDraft, string sCompanyDB, bool IsAdd, bool bIsSubmit, bool bIsDelCharge) {
            return base.Channel.Update_PurchaseRequest(oDataset, IsDraft, sCompanyDB, IsAdd, bIsSubmit, bIsDelCharge);
        }
        
        public string Submit_PurchaseRequest(System.Data.DataSet oDataset, string sCompanyDB) {
            return base.Channel.Submit_PurchaseRequest(oDataset, sCompanyDB);
        }
        
        public string Approve_PurchaseRequest(System.Data.DataSet oDataset, string sCompanyDB) {
            return base.Channel.Approve_PurchaseRequest(oDataset, sCompanyDB);
        }
        
        public string Delete_PurchaseRequest(System.Data.DataSet oDataset, string sCompanyDB) {
            return base.Channel.Delete_PurchaseRequest(oDataset, sCompanyDB);
        }
        
        public System.Data.DataSet Get_ApprovalStatus_Summary(string sFromDate, string sToDate, string sOutlet, string sConnString) {
            return base.Channel.Get_ApprovalStatus_Summary(sFromDate, sToDate, sOutlet, sConnString);
        }
        
        public string Login(string sUserName, string sPassword, string sDBName, string sServer, string sLicServerName, string sDBUserName, string sDBPassword) {
            return base.Channel.Login(sUserName, sPassword, sDBName, sServer, sLicServerName, sDBUserName, sDBPassword);
        }
        
        public System.Data.DataSet Get_UserInformation(string sUserName, string sConnString) {
            return base.Channel.Get_UserInformation(sUserName, sConnString);
        }
        
        public System.Data.DataSet Get_Outlet_Details(string sSuperUser, string sOutletCode, int iApprovalLevel, string sConnString) {
            return base.Channel.Get_Outlet_Details(sSuperUser, sOutletCode, iApprovalLevel, sConnString);
        }
        
        public System.Data.DataSet Get_Supplier_Details(string sUserRole, string sConnString) {
            return base.Channel.Get_Supplier_Details(sUserRole, sConnString);
        }
        
        public System.Data.DataSet Get_Company_Details() {
            return base.Channel.Get_Company_Details();
        }
        
        public System.Data.DataSet Get_MaterialReqBySupplier(string sSupplier, string sOutlet, string userRole, string sConnString) {
            return base.Channel.Get_MaterialReqBySupplier(sSupplier, sOutlet, userRole, sConnString);
        }
        
        public System.Data.DataSet Get_MaterialReqByItem(string sGroupType, string sOutlet, string sConnString) {
            return base.Channel.Get_MaterialReqByItem(sGroupType, sOutlet, sConnString);
        }
        
        public string Create_PurchaseRequest(System.Data.DataSet oDataset, bool IsDraft, string sCompanyDB, bool IsAdd, bool bIsSubmit, bool bIsDelCharge) {
            return base.Channel.Create_PurchaseRequest(oDataset, IsDraft, sCompanyDB, IsAdd, bIsSubmit, bIsDelCharge);
        }
        
        public string Delete_Drafts(System.Data.DataSet oDataset, string sCompanyDB) {
            return base.Channel.Delete_Drafts(oDataset, sCompanyDB);
        }
        
        public string PRDraft_Submit(System.Data.DataSet oDataset, string sCompanyDB) {
            return base.Channel.PRDraft_Submit(oDataset, sCompanyDB);
        }
        
        public System.Data.DataSet Get_MaterialReqDraft(string sOutlet, string sConnString) {
            return base.Channel.Get_MaterialReqDraft(sOutlet, sConnString);
        }
        
        public System.Data.DataSet Get_MaterialReq_Submitted_Approval(string sOutlet, int iStatus, string fromDate, string todate, string sUserRole, string sConnString) {
            return base.Channel.Get_MaterialReq_Submitted_Approval(sOutlet, iStatus, fromDate, todate, sUserRole, sConnString);
        }
        
        public System.Data.DataSet Get_DraftDetails(string sDocEntry, string sConnString, string sDocType) {
            return base.Channel.Get_DraftDetails(sDocEntry, sConnString, sDocType);
        }
        
        public System.Data.DataSet Get_MRSubmit_ApprovedDetails(string sDocEntry, string sDocType, string sConnString) {
            return base.Channel.Get_MRSubmit_ApprovedDetails(sDocEntry, sDocType, sConnString);
        }
        
        public System.Data.DataSet Get_MaterialReqBySupplierItemList(string sOutlet, string sConnString) {
            return base.Channel.Get_MaterialReqBySupplierItemList(sOutlet, sConnString);
        }
        
        public System.Data.DataSet Get_ItemList(string sCompanyDB) {
            return base.Channel.Get_ItemList(sCompanyDB);
        }
        
        public System.Data.DataSet Get_ReasonDetails(string sCompanyDB, string sOutlet) {
            return base.Channel.Get_ReasonDetails(sCompanyDB, sOutlet);
        }
        
        public string ConvertDraftToDocument(System.Data.DataSet oDataset, string sCompanyDB) {
            return base.Channel.ConvertDraftToDocument(oDataset, sCompanyDB);
        }
        
        public System.Data.DataSet Get_InventoryRequest(string sCompanyDB, string sOutlet) {
            return base.Channel.Get_InventoryRequest(sCompanyDB, sOutlet);
        }
        
        public System.Data.DataSet Get_InventoryRequestDetails(string sCompanyDB, string sRequestNo) {
            return base.Channel.Get_InventoryRequestDetails(sCompanyDB, sRequestNo);
        }
        
        public System.Data.DataSet Get_OpenPOList(string sCompanyDB, string sSupplier, string sSupplierType, string sAccessType, string sOutletCode) {
            return base.Channel.Get_OpenPOList(sCompanyDB, sSupplier, sSupplierType, sAccessType, sOutletCode);
        }
        
        public System.Data.DataSet Get_ReceiveInOutlet_Details(string sCompanyDB, string sOutlet, string sAccessType, string sSupplier) {
            return base.Channel.Get_ReceiveInOutlet_Details(sCompanyDB, sOutlet, sAccessType, sSupplier);
        }
        
        public System.Data.DataSet Get_GetPendingDrafts(string sCompanyDB, string sOutlet, string sUserRole, string sApprLevel) {
            return base.Channel.Get_GetPendingDrafts(sCompanyDB, sOutlet, sUserRole, sApprLevel);
        }
        
        public string Update_ApprovalStatus(System.Data.DataSet oDataset, string sCompanyDB) {
            return base.Channel.Update_ApprovalStatus(oDataset, sCompanyDB);
        }
        
        public string ReceiveInOutlet(System.Data.DataSet oDataset, string sCompanyDB) {
            return base.Channel.ReceiveInOutlet(oDataset, sCompanyDB);
        }
        
        public System.Data.DataSet Get_SalesTakingCountList(string sCompanyDB, string sOutlet, string sUserRole) {
            return base.Channel.Get_SalesTakingCountList(sCompanyDB, sOutlet, sUserRole);
        }
        
        public System.Data.DataSet Get_StockTakeCounting(string sCompanyDB, string sOutlet, string sUserRole, string sStatus, string sDocEntry) {
            return base.Channel.Get_StockTakeCounting(sCompanyDB, sOutlet, sUserRole, sStatus, sDocEntry);
        }
        
        public string StockTakeApprove(System.Data.DataSet oDataset, string sOutlet, string sCompanyDB) {
            return base.Channel.StockTakeApprove(oDataset, sOutlet, sCompanyDB);
        }
        
        public System.Data.DataSet Get_StockTakeCountingDetails(string sCompanyDB, string sOutlet, string sUserRole, string sStatus, string sDocEntry, string sDocOwner, string sRequester, string sDocDate) {
            return base.Channel.Get_StockTakeCountingDetails(sCompanyDB, sOutlet, sUserRole, sStatus, sDocEntry, sDocOwner, sRequester, sDocDate);
        }
        
        public System.Data.DataSet Create_StockTakeCounting(string sCompanyDB, string sOutlet, string sUserRole, string sStatus, string sDocEntry, string sDocOwner, string sRequester, string sDocDate) {
            return base.Channel.Create_StockTakeCounting(sCompanyDB, sOutlet, sUserRole, sStatus, sDocEntry, sDocOwner, sRequester, sDocDate);
        }
        
        public string Update_StockTakeApprove(System.Data.DataSet oDataset, string sCompanyDB, string sStatus, string sUserRole, bool bIsApprove, string sDocOwner, string sRequester, string sOutlet) {
            return base.Channel.Update_StockTakeApprove(oDataset, sCompanyDB, sStatus, sUserRole, bIsApprove, sDocOwner, sRequester, sOutlet);
        }
        
        public string UpdateDOStatus(System.Data.DataSet oDataset, string sCompanyDB, string sStatus, string sUserRole, bool bIsApprove) {
            return base.Channel.UpdateDOStatus(oDataset, sCompanyDB, sStatus, sUserRole, bIsApprove);
        }
    }
}
