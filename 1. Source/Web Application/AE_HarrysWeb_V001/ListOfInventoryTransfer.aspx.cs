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
    public partial class ListOfInventoryTransfer : System.Web.UI.Page
    {
        string sFromDate = string.Empty; string sToDate = string.Empty;
        DateTime fromdate = new DateTime();
        DateTime todate = new DateTime();
        string sStatus = string.Empty;
        string sFromOutlet = string.Empty;
        string sToOutlet = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session[Utils.AppConstants.UserCode].ToString() != string.Empty)
                {
                    if (!IsPostBack)
                    {
                        Session[AppConstants.IsBackPage] = 6;
                        loadDropdowndata();
                        mouseOverandMouseOut();
                        grvIT.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Visible = true;
                lblError.Text = ex.Message.ToString();
            }
        }

        protected void ddlFromOutlet_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblError.Visible = true;
            lblError.Text = string.Empty;
            //DataTable dt = new DataTable();
            //dt = (DataTable) Session["Outlets"];
            //RemoveToOutlet(dt);
        }

        protected void grvIT_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.grvIT.PageIndex = e.NewPageIndex;
            DataTable tblITReq = (DataTable)Session["ListofITRequest"];
            grvIT.DataSource = tblITReq;
            grvIT.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                lblError.Visible = true;
                lblError.Text = string.Empty;
                lblMessage.Visible = true;
                lblMessage.Text = string.Empty;
                var objC = new MasterService.MasterSoapClient();

                // compare whether to date is less than from date
                if (txtFromDate.Text != string.Empty && txtToDate.Text != string.Empty)
                {
                    if (Convert.ToDateTime(txtToDate.Text).Date < Convert.ToDateTime(txtFromDate.Text).Date)
                    {
                        lblMessage.Text = "To date should not be less than From date.";
                        lblMessage.Visible = true;
                        grvIT.DataBind();
                        return;

                    }
                    else if (Convert.ToDateTime(txtToDate.Text).Date > Convert.ToDateTime(txtFromDate.Text).Date)
                    {
                        lblMessage.Text = string.Empty;
                        lblMessage.Visible = false;
                    }
                }

                if (txtFromDate.Text != string.Empty)
                {
                    fromdate = Convert.ToDateTime(txtFromDate.Text);
                    sFromDate = String.Format("{0:MM/dd/yyyy}", fromdate);
                }
                if (txtToDate.Text != string.Empty)
                {
                    todate = Convert.ToDateTime(txtToDate.Text);
                    sToDate = String.Format("{0:MM/dd/yyyy}", todate);
                }
                if (ddlStatus.Text == "S")
                {
                    sStatus = string.Empty;
                }
                else
                {
                    sStatus = ddlStatus.SelectedValue;
                }

                if (ddlFromOutlet.SelectedValue == " --- Select Outlet --- ")
                {
                    sFromOutlet = string.Empty;
                }
                else
                {
                    sFromOutlet = ddlFromOutlet.SelectedValue;
                }

                if (ddlToOutlet.SelectedValue == " --- Select Outlet --- ")
                {
                    sToOutlet = string.Empty;
                }
                else
                {
                    sToOutlet = ddlToOutlet.SelectedValue;
                }

                DataSet ds = objC.Get_OpenTransferRequest(Session[AppConstants.DBName].ToString(), sFromOutlet, sToOutlet, sStatus, sFromDate, sToDate);
                if (ds != null && ds.Tables.Count > 0)
                {
                    Session["ListofITRequest"] = ds.Tables[0];
                    grvIT.DataSource = ds.Tables[0];
                    grvIT.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
                lblError.Visible = true;
            }
            
        }

        protected void lnkITReqNO_Click(object sender, EventArgs e)
        {
            LinkButton lnkRowSelection = (LinkButton)sender;
            string[] arguments = lnkRowSelection.CommandArgument.Split(';');
            Session[AppConstants.ITDocEntry] = arguments[0];
            Session[AppConstants.ITReqNo] = arguments[1];
            Session[AppConstants.ITRemarks] = arguments[2];
            Session[AppConstants.ITStatus] = arguments[3];
            Session[AppConstants.ITRequestor] = arguments[4];
            Response.Redirect(AppConstants.InventoryTransferReqApprovalURL);
        }

        private void loadDropdowndata()
        {
            var data = new MasterService.MasterSoapClient();

            DataSet dsFromOutlet = data.Get_Outlet_Details(Session[Utils.AppConstants.SUPERUSER].ToString(), Session[Utils.AppConstants.WhsCode].ToString(),
                1, Session[AppConstants.U_ConnString].ToString());
            ////if (Convert.ToInt32(Session[AppConstants.ApprovalLevel]) == 0)
            ////{
            //    //if (dsFromOutlet.Tables.Count != 0 && dsFromOutlet != null)
            //    //{
            //    //    RemoveOutletandBind(dsFromOutlet.Tables[0]);
            //    //}

            //    DataSet dsOutlet = data.Get_Outlet_Details(Session[Utils.AppConstants.SUPERUSER].ToString(), Session[Utils.AppConstants.WhsCode].ToString(),
            //        Convert.ToInt16(Session[AppConstants.ApprovalLevel]), Session[AppConstants.U_ConnString].ToString());
            //    if (dsOutlet.Tables.Count != 0 && dsOutlet != null)
            //    {
            //        ddlToOutlet.Enabled = false;
            //        ddlToOutlet.DataSource = dsOutlet.Tables[0];
            //        ddlToOutlet.DataTextField = dsOutlet.Tables[0].Columns[AppConstants.WhsName].ColumnName.ToString();
            //        ddlToOutlet.DataValueField = dsOutlet.Tables[0].Columns[AppConstants.WhsCode].ColumnName.ToString();
            //        ddlToOutlet.DataBind();
            //    }
            //}
            //else
            //{
            if (dsFromOutlet.Tables.Count != 0 && dsFromOutlet != null)
            {
                DataView dv = new DataView(dsFromOutlet.Tables[0]);
                dv.Sort = AppConstants.WhsCode;
                ddlFromOutlet.DataSource = dv.ToTable();
                ddlFromOutlet.DataTextField = dv.ToTable().Columns[AppConstants.WhsName].ColumnName.ToString();
                ddlFromOutlet.DataValueField = dv.ToTable().Columns[AppConstants.WhsCode].ColumnName.ToString();
                ddlFromOutlet.DataBind();

                //                    ddlToOutlet.Enabled = true;
                ddlToOutlet.DataSource = dv.ToTable();
                ddlToOutlet.DataTextField = dv.ToTable().Columns[AppConstants.WhsName].ColumnName.ToString();
                ddlToOutlet.DataValueField = dv.ToTable().Columns[AppConstants.WhsCode].ColumnName.ToString();
                ddlToOutlet.DataBind();
            }
            //}
        }

        private void mouseOverandMouseOut()
        {
            btnSearch.Attributes.Add("class", "static");
            btnSearch.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnSearch.Attributes.Add("onMouseOut", "this.className='static'");
        }

        private void RemoveToOutlet(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[AppConstants.WhsCode].ToString() == ddlFromOutlet.SelectedValue.ToString())
                {
                    dt.Rows.Remove(dr);
                    break;
                }
            }

            DataView dv = new DataView(dt);
            dv.Sort = AppConstants.WhsCode;
            ddlToOutlet.DataSource = dv.ToTable();
            ddlToOutlet.DataTextField = dt.Columns[AppConstants.WhsName].ColumnName.ToString();
            ddlToOutlet.DataValueField = dt.Columns[AppConstants.WhsCode].ColumnName.ToString();
            ddlToOutlet.DataBind();
        }

        private void RemoveFromOutlet(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[AppConstants.WhsCode].ToString() == ddlFromOutlet.SelectedValue.ToString())
                {
                    dt.Rows.Remove(dr);
                    break;
                }
            }

            DataView dv = new DataView(dt);
            dv.Sort = AppConstants.WhsCode;
            ddlFromOutlet.DataSource = dv.ToTable();
            ddlFromOutlet.DataTextField = dt.Columns[AppConstants.WhsName].ColumnName.ToString();
            ddlFromOutlet.DataValueField = dt.Columns[AppConstants.WhsCode].ColumnName.ToString();
            ddlFromOutlet.DataBind();
        }

        private void RemoveOutletandBind(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[AppConstants.WhsCode].ToString() == Session[Utils.AppConstants.WhsCode].ToString())
                {
                    dt.Rows.Remove(dr);
                    break;
                }
            }
            DataView dv = new DataView(dt);
            dv.Sort = AppConstants.WhsCode;
            ddlFromOutlet.DataSource = dv.ToTable();
            ddlFromOutlet.DataTextField = dt.Columns[AppConstants.WhsName].ColumnName.ToString();
            ddlFromOutlet.DataValueField = dt.Columns[AppConstants.WhsCode].ColumnName.ToString();
            ddlFromOutlet.DataBind();
        }
    }
}