using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AE_Harrys.Common;
namespace AE_HarrysWeb_V001.Utils
{
    public class AppConstants
    {
        #region Session Objects
        public const string CompanyName = "CompanyName";
        public const string CompanyCode = "CompanyCode";
        public const string UserCode = "UserCode";
        public const string Pwd = "Pwd";
        public const string UserID = "UserID";
        public const string DATE = "dd/MM/yyyy";
        public const string UserAccess = "U_AE_Access";
        public const string CardCode = "CardCode";
        public const string CardName = "CardName";
        public const string WhsCode = "WhsCode";
        public const string WhsName = "WhsName";
        public const string SUPERUSER = "SUPERUSER";
        public const string UserRole = "U_UserRole";
        public const string DefaultStatus = "Draft";
        public const string LoginURL = "Login.aspx";
        public const string HomepageURL = "Homepage.aspx";
        public const string MaterialRequestDraftURL = "MaterialRequestDraft.aspx";
        public const string MaterialRequestApprovedURL = "MaterialRequestApproved.aspx";
        public const string ListOfMaterialRequestDraftURL = "ListOfMaterialRequestDraft.aspx";
        public const string ListOfMaterialRequestURL = "ListOfMaterialRequest.aspx";
        public const string RecieveIntoOutletURL = "RecieveIntoOutlet.aspx";
        public const string StocktakeListingApprovalURL = "StocktakeCounting.aspx";
        public const string StocktakeCountingSheetURL = "StocktakeCountingSheet.aspx";
        public const string StocktakeApprovalURL = "StocktakeApproval.aspx";
        public const string InventoryTransferReqApprovalURL = "InventoryTransferApproval.aspx";
        public const string ListOfInventoryTransferReqURL = "ListOfInventoryTransfer.aspx";
        public const string MaterialRequestApprovalNewURL = "MaterialRequestApprovalNew.aspx";
        public const string OutletListPendingApprovalURL = "OutletListPendingApproval.aspx";
        public const string DBConnect = "DBConnect";
        public const string Priority = "Urgent";
        public const string Name = "Name";
        public const string Value = "Value";
        public const string Yes = "Y";
        public const string No = "N";
        public const string DayName = "DayName";
        public const string NextDayName = "NextDayName";
        public const string Bar = "BAR";
        public const string Kitchen = "KITCHEN";
        public const string All = "ALL";
        public const string ApprovalLevel = "ApprovalLevel";
        public const string NUMBER_FORMAT = "#,##0.00";
        public const string NUMBER_FORMAT_1 = "#,##0.0000";
        public const string MinSpend = "MinSpend";
        public const string OrderDate = "OrderDate";
        public const string DBName = "DBName";
        public const string UserName = "UserName";
        public const string UserInfo = "UserInfo";
        public const string DraftNo = "DraftNo";
        public const string PurchaseNo = "PurchaseNo";
        public const string DocType = "DocType";
        public const string Status = "Status";
        public const string IsBackPage = "IsBackPage";
        public const string SupplierCode = "SupplierCode";
        public const string SupplierName = "SupplierName";
        public const string SupplierType = "SupplierType";
        public const string Approved = "Approved";
        public const string Draft = "Draft";
        public const string delChargeItemCode = "SDELIVERY";
        public const string CountDate = "CountDate";
        public const string WhsCode_countingSheet = "WhsCode_countingSheet";
        public const string Status_countingSheet = "Status_countingSheet";
        public const string PendingApproval = "Pending Approval";
        public const string ReceiveMessage = "ReceiveMessage";
        public const string ApprovedDate = "ApprovedDate";
        public const string TryAfterSometime = "System is under another Request. Kindly try after sometime";
        public const string HoldingSupplier = "VA-HARHO";
        public const string StockTakeDocEntry = "StockTakeDocEntry";
        public const string ITReqNo = "ITReqNo";
        public const string ITDocEntry = "ITDocEntry";
        public const string ITRemarks = "ITRemarks";
        public const string ITStatus = "ITStatus";
        public const string ITRequestor = "ITRequestor";
        public const string CountDate_countingSheet = "CountDate_countingSheet";
        public const string PRWhsCode = "PRWhsCode";

        #endregion

        #region Server Session Objects
        public static string U_DBName = "U_DBName";
        public static string U_CompName = "U_CompName";
        public static string U_SAPUserName = "U_SAPUserName";
        public static string U_SAPPassword = "U_SAPPassword";
        public static string U_DBUserName = "U_DBUserName";
        public static string U_DBPassword = "U_DBPassword";
        public static string U_ConnString = "U_ConnString";
        public static string U_Server = "U_Server";
        public static string U_LicenseServer = "U_LicenceServer";

        #endregion

        #region Log File Variables

        public const Int16 RTN_SUCCESS = 1;
        public const Int16 RTN_ERROR = 0;
        public const Int16 DEBUG_ON = 1;
        public const Int16 DEBUG_OFF = 0;
        public const Int16 p_iDebugMode = DEBUG_ON;
        
        #endregion



    }
}