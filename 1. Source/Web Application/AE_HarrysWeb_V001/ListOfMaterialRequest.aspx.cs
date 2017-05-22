using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using AE_HarrysWeb_V001.Utils;
using System.Globalization;

namespace AE_HarrysWeb_V001
{
    public partial class ListOfMaterialRequest : System.Web.UI.Page
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
                        // Loading Outlet List
                        LoadOutletData();
                        LoadData(this.ddlOutlet.SelectedValue.ToString());
                        mouseOverandMouseOut();
                    }
                }
                else
                {
                    //Response.Redirect(AppConstants.LoginURL);
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = " Failed to load Data" + ex.Message;
                this.lblError.Visible = true;
                Response.Redirect("~/ErrorPage.aspx");
            }
        }

        private void LoadOutletData()
        {
            try
            {
                var data = new MasterService.MasterSoapClient();
                DataSet dsOutlet = data.Get_Outlet_Details(Session[AppConstants.SUPERUSER].ToString(), Session[AppConstants.WhsCode].ToString(),
                                    Convert.ToInt16(Session[AppConstants.ApprovalLevel]), Session[AppConstants.U_ConnString].ToString());
                if (dsOutlet != null && dsOutlet.Tables[0].Rows.Count != 0)
                {
                    ddlOutlet.DataSource = dsOutlet.Tables[0];
                    ddlOutlet.DataTextField = dsOutlet.Tables[0].Columns["WhsName"].ColumnName.ToString();
                    ddlOutlet.DataValueField = dsOutlet.Tables[0].Columns["WhsCode"].ColumnName.ToString();
                    ddlOutlet.DataBind();
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = "problem in loading the Outlets" + ex.Message;
                this.lblError.Visible = true;
            }
        }

        private void LoadData(string outlet)
        {
            try
            {
                //string[] fromDate = new string[] { }; string[] toDate = new string[] { };
                //if (txtFromDate.Text != string.Empty)
                //{
                //    fromDate = txtFromDate.Text.Split(' ');
                //    DateTime d = Convert.ToDateTime(txtFromDate.Text);
                //    string s = String.Format("{0:MM/dd/yy}", d);
                //}
                //else
                //{
                //    fromDate = new string[] { "" };
                //}
                //if (txtToDate.Text != string.Empty)
                //{
                //    toDate = txtToDate.Text.Split(' ');
                //}
                //else
                //{
                //    toDate = new string[] { "" };
                //}
                string sFromDate = string.Empty; string sToDate = string.Empty;
                if (txtFromDate.Text != string.Empty && txtToDate.Text != string.Empty)
                {
                    DateTime fromdate = Convert.ToDateTime(txtFromDate.Text);
                    DateTime todate = Convert.ToDateTime(txtToDate.Text);
                    sFromDate = String.Format("{0:MM/dd/yyyy}", fromdate);
                    sToDate = String.Format("{0:MM/dd/yyyy}", todate);
                }
                var objC = new MasterService.MasterSoapClient();

                DataSet ds = objC.Get_MaterialReq_Submitted_Approval(outlet, Convert.ToInt32(ddlStatus.SelectedValue), sFromDate, sToDate
                                                , Session[AppConstants.UserRole].ToString(), Session[AppConstants.U_ConnString].ToString());
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    Session["MRPurchaseList"] = dt;
                    dt.Columns.Add("TotalSpendValue");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i <= dt.Rows.Count - 1; i++)
                        {

                            dt.Rows[i]["TotalSpendValue"] = string.Format("${0}", Convert.ToDouble(dt.Rows[i]["TotalSpend"]).ToString(Utils.AppConstants.NUMBER_FORMAT));
                        }
                    }
                    this.grvMR.DataSource = dt;
                }
                this.grvMR.DataBind();

            }

            catch (Exception ex)
            {
                this.lblError.Visible = true;
                this.lblError.Text = "Failed to load data in Grid : " + ex.Message;
            }
        }

        protected void ddlOutlet_SelectedIndexChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (this.ddlOutlet.Text.Trim().Length > 0)
            //    {
            //        LoadData(this.ddlOutlet.SelectedValue.ToString());
            //    }
            //}
            //catch (Exception ex)
            //{
            //    this.lblError.Text = ex.Message;
            //    this.lblError.Visible = true;
            //}
        }

        protected void lnkPrNO_Click(object sender, System.EventArgs e)
        {
            LinkButton lnkRowSelection = (LinkButton)sender;
            string[] arguments = lnkRowSelection.CommandArgument.Split(';');
            Session[AppConstants.PurchaseNo] = arguments[0];
            Session[AppConstants.DocType] = arguments[1].ToUpper().Substring(0, 2);
            Session[AppConstants.IsBackPage] = 2;
            Response.Redirect(AppConstants.MaterialRequestApprovedURL);
        }

        protected void grvMR_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.grvMR.PageIndex = e.NewPageIndex;
            DataTable tblILOMR = (DataTable)Session["MRPurchaseList"];
            BindData(tblILOMR);
        }

        private void BindData(DataTable tblILOMR)
        {
            Session["MRPurchaseList"] = tblILOMR;
            DataView dv = tblILOMR.DefaultView;
            this.grvMR.DataSource = dv.ToTable();
            this.grvMR.DataBind();
        }

        //protected void txtSearch_OnTextChanged(object sender, EventArgs e)
        //{
        //    var searchKey = txtSearch.Text;
        //    DataTable dt = (DataTable)Session["MRPurchaseList"];
        //    var rows = from row in dt.AsEnumerable()
        //               where row.Field<string>("Status").Contains(UppercaseFirst(searchKey))
        //               select row;
        //    if (rows.Count() > 0)
        //    {
        //        grvMR.DataSource = rows.CopyToDataTable();
        //        grvMR.DataBind();
        //    }
        //    else
        //    {
        //        grvMR.DataSource = new DataTable();
        //        grvMR.DataBind();
        //    }
        //    txtSearch.Focus();
        //}

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                // check if two dates are empty, then no problem
                if (txtFromDate.Text == string.Empty && txtToDate.Text == string.Empty)
                {

                }
                // check if any one of the date is not selected,you should prompt the user to select the particular date, return
                else if (txtToDate.Text == string.Empty && txtFromDate.Text != string.Empty)
                {
                    lblMessage.Visible = true;
                    lblMessage.Text = "ToDate Should not be empty";
                    grvMR.DataBind();
                    return;
                }
                else if (txtFromDate.Text == string.Empty && txtToDate.Text != string.Empty)
                {
                    lblMessage.Visible = true;
                    lblMessage.Text = "FromDate Should not be empty";
                    grvMR.DataBind();
                    return;
                }
                // compare whether to date is less than from date
                if (txtFromDate.Text != string.Empty && txtToDate.Text != string.Empty)
                {
                    if (Convert.ToDateTime(txtToDate.Text).Date < Convert.ToDateTime(txtFromDate.Text).Date)
                    {
                        lblMessage.Text = "To date should not be less than From date.";
                        lblMessage.Visible = true;
                        grvMR.DataBind();
                        return;

                    }
                    else if (Convert.ToDateTime(txtToDate.Text).Date > Convert.ToDateTime(txtFromDate.Text).Date)
                    {
                        lblMessage.Text = string.Empty;
                        lblMessage.Visible = false;
                    }
                }
                if (ddlOutlet.Text == " --- Select Outlet --- ")
                {
                    lblMessage.Visible = true;
                    lblMessage.Text = "Kindly select the outlet";
                    grvMR.DataBind();
                }
                else if (this.ddlOutlet.Text.Trim().Length > 0)
                {
                    lblMessage.Text = string.Empty;
                    LoadData(this.ddlOutlet.SelectedValue.ToString());
                }

                
                
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }

        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public void mouseOverandMouseOut()
        {
            btnSearch.Attributes.Add("class", "static");
            btnSearch.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnSearch.Attributes.Add("onMouseOut", "this.className='static'");
        }
    }

}