using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using AE_HarrysWeb_V001.Utils;

namespace AE_HarrysWeb_V001
{
    public partial class StocktakeApproval : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e) 
        {
            try
            {
                if (Session[Utils.AppConstants.UserCode].ToString() != string.Empty)
                {
                    if (!IsPostBack)
                    {
                        Session[AppConstants.IsBackPage] = 0;
                        Session[AppConstants.ApprovedDate] = string.Empty;
                        loadDropdowndata();
                        grvStkApp.DataBind();
                        //Session["wareHouseCode"] = ddlWareHouse.SelectedValue;

                    }
                }
                else
                {
                    //Response.Redirect(AppConstants.LoginURL);
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = " Failed to load Data..." + ex.Message;
                this.lblError.Visible = true;
                //Response.Redirect("~/ErrorPage.aspx");
               // Response.Redirect(AppConstants.LoginURL);
            }
        }

        private void loadDropdowndata()
        {
            try
            {
                var data = new MasterService.MasterSoapClient();
                DataSet dsOutlet = data.Get_Outlet_Details(Session[Utils.AppConstants.SUPERUSER].ToString(), Session[Utils.AppConstants.WhsCode].ToString(),
                    Convert.ToInt16(Session[AppConstants.ApprovalLevel]), Session[AppConstants.U_ConnString].ToString());
                if (dsOutlet.Tables.Count != 0 && dsOutlet != null)
                {
                    ddlWareHouse.DataSource = dsOutlet.Tables[0];
                    ddlWareHouse.DataTextField = dsOutlet.Tables[0].Columns[AppConstants.WhsName].ColumnName.ToString();
                    ddlWareHouse.DataValueField = dsOutlet.Tables[0].Columns[AppConstants.WhsCode].ColumnName.ToString();
                    ddlWareHouse.DataBind();
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = "Error Occured while Loading the Outlets... " + ex.Message;
                this.lblError.Visible = true;
            }
        }

        protected void lnkStatus_Click(object sender, System.EventArgs e)
        {
            LinkButton lnkRowSelection = (LinkButton)sender;
            string[] arguments = lnkRowSelection.CommandArgument.Split(';');
            Session[AppConstants.Status] = arguments[0];
            Session[AppConstants.CountDate] = arguments[1];
            Session[AppConstants.WhsCode] = arguments[2];
            Session[AppConstants.ApprovedDate] = arguments[3];
            Session[AppConstants.StockTakeDocEntry] = arguments[5];
            Session[AppConstants.IsBackPage] = 5;
            if (arguments[0] == AppConstants.PendingApproval || arguments[0] == AppConstants.Approved)
            {
                Response.Redirect(AppConstants.StocktakeListingApprovalURL);
            }
            else
            {
                Session[AppConstants.Status_countingSheet] = arguments[0];
                Session[AppConstants.CountDate_countingSheet] = arguments[1];
                Session[AppConstants.WhsCode_countingSheet] = arguments[2];
                Response.Redirect(AppConstants.StocktakeCountingSheetURL);
            }
        }

        protected void ddlWareHouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Session["wareHouseCode"] = ddlWareHouse.SelectedValue;
            if (this.ddlWareHouse.Text.Trim().Length > 0)
            {
                LoadData(Session[AppConstants.DBName].ToString(), ddlWareHouse.SelectedValue.ToString());
            }
        }

        private void LoadData(string companyDB, string outlet)
        {
            try
            {
                var objGetPending = new MasterService.MasterSoapClient();
                DataSet ds = objGetPending.Get_SalesTakingCountList(Session[AppConstants.DBName].ToString(), ddlWareHouse.SelectedValue.ToString(),Session[AppConstants.UserRole].ToString());
                if (ds != null)
                {
                    DataTable dt = ds.Tables[0];
                    Session["StockCountingAppList"] = dt;
                    grvStkApp.DataSource = dt;
                }
                grvStkApp.DataBind();
            }
            catch (Exception ex)
            {
                this.lblError.Text = "Problem in loading the Data..." + ex.Message;
                this.lblError.Visible = true;
            }
        }

    }
}