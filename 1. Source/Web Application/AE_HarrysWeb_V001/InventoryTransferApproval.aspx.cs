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
    public partial class InventoryTransferApproval : System.Web.UI.Page
    {
        string sDocEntry = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    Session[AppConstants.IsBackPage] = 7;
                    loadDropdowndata();
                    sDocEntry = Convert.ToString(Session[AppConstants.ITDocEntry]);
                    LoadData(sDocEntry);
                    mouseOverandMouseOut();
                }
            }
            catch (Exception ex)
            {
                lblError.Visible = true;
                lblError.Text = ex.Message.ToString();
            }
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)Session["ITApprovalData"];

            if (dt != null && dt.Rows.Count > 0)
            {

                DataSet oDS = new DataSet();
                DataTable dtCopy = dt.Copy();
                oDS.Tables.Add(dtCopy);

                var ObjC = new MasterService.MasterSoapClient();
                var returnResult = ObjC.Approve_InventoryTransferRequest(Session[AppConstants.DBName].ToString(), oDS);

                if (returnResult != "SUCCESS")
                {
                    if (returnResult == string.Empty)
                    {
                        returnResult = AppConstants.TryAfterSometime;
                    }
                    lblError.Visible = true;
                    lblError.Text = returnResult;
                }
                else
                {
                    lblError.Visible = true;
                    lblError.Text = "Inventory Transfer Document Created Successfully.";
                    btnApprove.Enabled = false;
                    btnApprove.Style["background-image"] = "url('Images/bgButton_disable.png')";

                    btnReject.Enabled = false;
                    btnReject.Style["background-image"] = "url('Images/bgButton_disable.png')";
                }
                oDS.Tables.Remove(dtCopy);
                oDS.Clear();
            }

            // Call the Approve web method
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)Session["ITApprovalData"];

            // Call the Reject web method

            if (dt != null && dt.Rows.Count > 0)
            {

                DataSet oDS = new DataSet();
                DataTable dtCopy = dt.Copy();
                oDS.Tables.Add(dtCopy);

                var ObjC = new MasterService.MasterSoapClient();
                var returnResult = ObjC.Reject_InventoryTransferRequest(Session[AppConstants.DBName].ToString(), oDS);

                if (returnResult != "SUCCESS")
                {
                    if (returnResult == string.Empty)
                    {
                        returnResult = AppConstants.TryAfterSometime;
                    }
                    lblError.Visible = true;
                    lblError.Text = returnResult;
                }
                else
                {
                    lblError.Visible = true;
                    lblError.Text = "Inventory Transfer Request Rejected.";
                    btnApprove.Enabled = false;
                    btnApprove.Style["background-image"] = "url('Images/bgButton_disable.png')";

                    btnReject.Enabled = false;
                    btnReject.Style["background-image"] = "url('Images/bgButton_disable.png')";
                }
                oDS.Tables.Remove(dtCopy);
                oDS.Clear();
            }

        }

        protected void grvInvTransfer_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.grvInvTransfer.PageIndex = e.NewPageIndex;
            DataTable tblIT = (DataTable)Session["ITApprovalData"];
            grvInvTransfer.DataSource = tblIT;
            grvInvTransfer.DataBind();
        }

        private void LoadData(string sDocEntry)
        {
            var objC = new MasterService.MasterSoapClient();
            DataSet ds = objC.Get_OpenTransferRequestDetails(Session[AppConstants.DBName].ToString(), sDocEntry);
            if (ds != null && ds.Tables.Count > 0)
            {
                Session["ITApprovalData"] = ds.Tables[0];
                ddlFromOutlet.SelectedValue = ds.Tables[0].Rows[0]["FromOutlet"].ToString();
                ddlToOutlet.SelectedValue = ds.Tables[0].Rows[0]["ToOutlet"].ToString();
                txtSubmittedBy.Text = ds.Tables[0].Rows[0]["SubmittedBy"].ToString();
                txtRemarks.Text = ds.Tables[0].Rows[0]["Remarks"].ToString();
                txtTransferDate.Text = ds.Tables[0].Rows[0]["RequestDate"].ToString();
                grvInvTransfer.DataSource = ds;
                grvInvTransfer.DataBind();
            }
        }

        private void mouseOverandMouseOut()
        {
            if (Session[Utils.AppConstants.UserName].ToString().ToUpper().Trim() != Session[AppConstants.ITRequestor].ToString().ToUpper())
            {
                if (Convert.ToInt32(Session[AppConstants.ApprovalLevel]) == 0)
                {
                    if (Session[AppConstants.ITStatus].ToString() != "Closed" && Session[AppConstants.ITRemarks].ToString() != "Rejected")
                    {
                        btnApprove.Attributes.Add("class", "static");
                        btnApprove.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
                        btnApprove.Attributes.Add("onMouseOut", "this.className='static'");

                        btnReject.Attributes.Add("class", "static");
                        btnReject.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
                        btnReject.Attributes.Add("onMouseOut", "this.className='static'");
                    }
                    else
                    {
                        btnApprove.Enabled = false;
                        btnApprove.Style["background-image"] = "url('Images/bgButton_disable.png')";

                        btnReject.Enabled = false;
                        btnReject.Style["background-image"] = "url('Images/bgButton_disable.png')";
                    }
                }

                else
                {
                    btnApprove.Enabled = false;
                    btnApprove.Style["background-image"] = "url('Images/bgButton_disable.png')";

                    btnReject.Enabled = false;
                    btnReject.Style["background-image"] = "url('Images/bgButton_disable.png')";
                }
            }
            else
            {
                btnApprove.Enabled = false;
                btnApprove.Style["background-image"] = "url('Images/bgButton_disable.png')";

                btnReject.Enabled = false;
                btnReject.Style["background-image"] = "url('Images/bgButton_disable.png')";
            }
        }

        private void loadDropdowndata()
        {
            var data = new MasterService.MasterSoapClient();

            DataSet ds = data.Get_Outlet_Details(Session[Utils.AppConstants.SUPERUSER].ToString(), Session[Utils.AppConstants.WhsCode].ToString(),
                1, Session[AppConstants.U_ConnString].ToString());

            if (ds.Tables.Count != 0 && ds != null)
            {
                Session["Outlets"] = ds.Tables[0];
                DataView dv = new DataView(ds.Tables[0]);
                dv.Sort = AppConstants.WhsCode;
                ddlFromOutlet.DataSource = dv.ToTable();
                ddlFromOutlet.DataTextField = dv.ToTable().Columns[AppConstants.WhsName].ColumnName.ToString();
                ddlFromOutlet.DataValueField = dv.ToTable().Columns[AppConstants.WhsCode].ColumnName.ToString();
                ddlFromOutlet.DataBind();

                ddlToOutlet.DataSource = dv.ToTable();
                ddlToOutlet.DataTextField = dv.ToTable().Columns[AppConstants.WhsName].ColumnName.ToString();
                ddlToOutlet.DataValueField = dv.ToTable().Columns[AppConstants.WhsCode].ColumnName.ToString();
                ddlToOutlet.DataBind();
            }
        }
    }
}