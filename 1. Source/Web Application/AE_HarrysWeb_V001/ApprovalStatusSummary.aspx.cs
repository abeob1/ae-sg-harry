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
    public partial class ApprovalStatusSummary : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtFromDate.Text = DateTime.Now.ToShortDateString();
                txtToDate.Text = DateTime.Now.ToShortDateString();
                LoadOutletData();
                LoadData();
                mouseOverandMouseOut();
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

        private void LoadData()
        {
            try
            {

                string sFromDate = string.Empty; string sToDate = string.Empty; string sOutlet = string.Empty;
                if (txtFromDate.Text != string.Empty && txtToDate.Text != string.Empty)
                {
                    DateTime fromdate = Convert.ToDateTime(txtFromDate.Text);
                    DateTime todate = Convert.ToDateTime(txtToDate.Text);
                    sFromDate = String.Format("{0:MM/dd/yyyy}", fromdate);
                    sToDate = String.Format("{0:MM/dd/yyyy}", todate);
                }
                sOutlet = ddlOutlet.SelectedValue.ToString();
                if (sOutlet == " --- Select Outlet --- ")
                {
                    sOutlet = string.Empty;
                }

                if (sFromDate.ToString() != string.Empty && sToDate.ToString() != string.Empty)
                {
                    var objC = new MasterService.MasterSoapClient();

                    DataSet ds = objC.Get_ApprovalStatus_Summary(sFromDate, sToDate, sOutlet, Session[AppConstants.U_ConnString].ToString());
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];
                        Session["ApprovalStatus"] = dt;

                    }
                    this.grvAppr.DataSource = (DataTable)Session["ApprovalStatus"];
                    this.grvAppr.DataBind();
                }
                else
                {
                    this.grvAppr.DataBind();
                }

            }

            catch (Exception ex)
            {
                this.lblError.Visible = true;
                this.lblError.Text = "Failed to load data in Grid : " + ex.Message;
            }
        }

        protected void grvAppr_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.grvAppr.PageIndex = e.NewPageIndex;
            DataTable tblApproval = (DataTable)Session["ApprovalStatus"];
            BindData(tblApproval);
        }

        private void BindData(DataTable tblApproval)
        {
            Session["ApprovalStatus"] = tblApproval;
            DataView dv = tblApproval.DefaultView;
            this.grvAppr.DataSource = dv.ToTable();
            this.grvAppr.DataBind();
        }

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
                    grvAppr.DataBind();
                    return;
                }
                else if (txtFromDate.Text == string.Empty && txtToDate.Text != string.Empty)
                {
                    lblMessage.Visible = true;
                    lblMessage.Text = "FromDate Should not be empty";
                    grvAppr.DataBind();
                    return;
                }
                // compare whether to date is less than from date
                if (txtFromDate.Text != string.Empty && txtToDate.Text != string.Empty)
                {
                    if (Convert.ToDateTime(txtToDate.Text).Date < Convert.ToDateTime(txtFromDate.Text).Date)
                    {
                        lblMessage.Text = "To date should not be less than From date.";
                        lblMessage.Visible = true;
                        grvAppr.DataBind();
                        return;

                    }
                    else if (Convert.ToDateTime(txtToDate.Text).Date > Convert.ToDateTime(txtFromDate.Text).Date)
                    {
                        lblMessage.Text = string.Empty;
                        lblMessage.Visible = false;
                    }
                }
                LoadData();
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }

        protected void ddlOutlet_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblError.Visible = false;
            this.lblError.Text = string.Empty;
        }

        public void mouseOverandMouseOut()
        {
            btnSearch.Attributes.Add("class", "static");
            btnSearch.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnSearch.Attributes.Add("onMouseOut", "this.className='static'");
        }
    }
}