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
    public partial class InventoryTransferRequest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session[Utils.AppConstants.UserCode].ToString() != string.Empty)
                {
                    if (!IsPostBack)
                    {
                        txtTransferDate.Text = DateTime.Now.ToShortDateString();
                        txtSubmittedBy.Text = Session[Utils.AppConstants.UserName].ToString().ToUpper().Trim();
                        loadDropdowndata();
                        grvInvTransfer.DataBind();
                        mouseOverandMouseOut();
                        //txtRemarks.SelectionStart = txtRemarks.Text.Length -1; // add some logic if length is 0
                        //txtRemarks.SelectionLength = 0;
                    }
                }
                else
                {
                    // Response.Redirect(AppConstants.LoginURL);
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
                //Response.Redirect("~/ErrorPage.aspx");
            }

        }

        protected void btnItemSearch_Click(object sender, EventArgs e)
        {
            //test.Visible = true;
            lblError.Visible = false;
            lblError.Text = string.Empty;
            if (ddlFromOutlet.Text == " --- Select Outlet --- ")
            {
                lblError.Visible = true;
                lblError.Text = "Kindly select the From Outlet.";
                return;
            }
            else if (ddlToOutlet.Text == " --- Select Outlet --- ")
            {
                lblError.Visible = true;
                lblError.Text = "Kindly select the To Outlet.";
                return;
            }
            else
            {
                txtSearch.Text = string.Empty;
                mpePopup.Show();
                LoadPopupData(string.Empty, ddlFromOutlet.SelectedValue, ddlToOutlet.SelectedValue, Session[Utils.AppConstants.UserRole].ToString().Trim());
            }
            //test.Visible = false;
        }

        protected void ddlFromOutlet_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["AddDataToGrid"] = new DataTable();
            lblError.Visible = false;
            lblError.Text = string.Empty;
            grvInvTransfer.DataBind();
        }

        protected void txtOrderQuantity_OnTextChanged(object sender, EventArgs e)
        {
            GridViewRow row = (GridViewRow)((DataControlFieldCell)((TextBox)sender).Parent).Parent;
            ViewState["rowindex"] = row.RowIndex;
            TextBox txtQuantity = (TextBox)row.FindControl("txtOrderQuantity");
            Label lblItemCode = (Label)row.FindControl("lblItemCode");

            DataTable tb = (DataTable)Session["ITTable"];
            if (tb != null)
            {
                DataRow[] rupdate = tb.Select("ItemCode='" + lblItemCode.Text + "'");
                if (rupdate.Length > 0)
                {
                    rupdate[0]["OrderQuantity"] = txtQuantity.Text == string.Empty ? "0" : txtQuantity.Text; ;
                }
                this.grvInvTransfer.EditIndex = -1;
                BindData(tb);
                txtSearch.Text = string.Empty;
            }

            DataTable tb1 = (DataTable)Session["AddDataToGrid"];
            if (tb1 != null)
            {
                DataRow[] rupdate1 = tb1.Select("ItemCode='" + lblItemCode.Text + "'");
                if (rupdate1.Length > 0)
                {
                    rupdate1[0]["OrderQuantity"] = txtQuantity.Text == string.Empty ? "0" : txtQuantity.Text; ;
                }
                Session["AddDataToGrid"] = tb1;
            }
        }

        protected void grvInvTransfer_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("onmouseover", "MouseEvents(this, event)");
                e.Row.Attributes.Add("onmouseout", "MouseEvents(this, event)");
                e.Row.Attributes["style"] = "cursor:pointer";
                int index = 0;
                TextBox ddl1 = e.Row.FindControl("txtOrderQuantity") as TextBox;

                ddl1.Attributes.Add("onfocus", "JavaScript:document.getElementById('" + ddl1.ClientID + "').select();");
                if (ViewState["rowindex"] != null && Session["ITTable"] != null)
                {
                    index = Convert.ToInt32(ViewState["rowindex"].ToString());
                    int rowCount = ((DataTable)Session["ITTable"]).Rows.Count;
                    if ((index + 1) <= this.grvInvTransfer.Rows.Count)
                    {
                        if (e.Row.RowIndex == index + 1)
                        {
                            ddl1.Focus();
                        }
                    }
                }

            }
        }

        protected void grvInvTransfer_OnRowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridViewRow myRow = e.Row;
                Label lblNo = myRow.FindControl("lblNo") as Label;
            }
        }

        protected void grvInvTransfer_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.grvInvTransfer.PageIndex = e.NewPageIndex;
            DataTable tblIT = (DataTable)Session["ITTable"];
            BindData(tblIT);
        }

        protected void LoadPopupData(string supplier, string sFromOutlet, string sToOutlet, string userRole)
        {
            try
            {
                DataTable tblAddItem = CreatePopHeaderFormat();
                var objC = new MasterService.MasterSoapClient();
                DataSet ds = objC.Get_InventoryTransferRequest_ItemSearch(Session[Utils.AppConstants.U_DBName].ToString().Trim(), sFromOutlet, sToOutlet, userRole);

                Session["ITPopUpData"] = ds;
                if (ds != null && ds.Tables.Count > 0)
                {
                    int i = 1;
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        DataRow rowNew = tblAddItem.NewRow();
                        rowNew["No"] = i;
                        rowNew["ItemCode"] = row["ItemCode"];
                        rowNew["Description"] = row["Description"];
                        rowNew["UOM"] = row["UOM"];
                        rowNew["InStock"] = row["InStock"];
                        rowNew["QryGroup2"] = row["QryGroup2"];
                        rowNew["NumInBuy"] = row["NumInBuy"];
                        rowNew["NumInSale"] = row["NumInSale"];
                        tblAddItem.Rows.Add(rowNew);
                        i++;
                    }
                    Session["ItemPopupTable"] = tblAddItem;
                    DataView dv = tblAddItem.DefaultView;
                    this.grvItemList.DataSource = dv.ToTable();
                    this.grvItemList.DataBind();

                }
                else
                {
                    Session["ItemPopupTable"] = tblAddItem;
                    DataView dv = tblAddItem.DefaultView;
                    this.grvItemList.DataSource = dv.ToTable();
                    this.grvItemList.DataBind();
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = "Failed to load the POPUP Data.." + ex.Message;
                this.lblError.Visible = true;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string sFromOutlet = ddlFromOutlet.SelectedValue.Trim().ToUpper();
            string sToOutlet = ddlToOutlet.SelectedValue.Trim().ToUpper();
            string sSubmittedBy = txtSubmittedBy.Text.Trim();
            string sRemarks = txtRemarks.Text.Trim();
            string sRequestDate = txtTransferDate.Text.Trim();
            if (ddlFromOutlet.Text == " --- Select Outlet --- ")
            {
                lblError.Visible = true;
                lblError.Text = "Kindly select the From Outlet.";
                return;
            }
            else if (txtSubmittedBy.Text == string.Empty)
            {
                lblError.Visible = true;
                lblError.Text = "Kindly enter the Submitted by.";
                return;
            }
            else if (txtRemarks.Text == string.Empty)
            {
                lblError.Visible = true;
                lblError.Text = "Kindly enter the Remarks.";
                return;
            }
            else if (txtTransferDate.Text == string.Empty)
            {
                lblError.Visible = true;
                lblError.Text = "Kindly enter the Transfer Date.";
                return;
            }
            else if (grvInvTransfer.Rows.Count == 0)
            {
                lblError.Visible = true;
                lblError.Text = "Kindly add the Items and click Submit.";
                return;
            }
            else
            {
                DataTable dtResult = (DataTable)Session["ITTable"];
                if (dtResult != null && dtResult.Rows.Count > 0)
                {
                    foreach (DataRow item in dtResult.Rows)
                    {
                        item["FromOutlet"] = sFromOutlet;
                        item["ToOutlet"] = sToOutlet;
                        item["SubmittedBy"] = sSubmittedBy;
                        item["Remarks"] = sRemarks;
                        item["RequestDate"] = sRequestDate;
                        if (item["OrderQuantity"].ToString() != "")
                        {
                            if (item["QryGroup2"].ToString() == "N")
                            {
                                item["OrderQuantity"] = Convert.ToInt32(item["OrderQuantity"]) * Convert.ToDouble(item["NumInSale"]);
                            }
                            else
                            {
                                item["OrderQuantity"] = Convert.ToInt32(item["OrderQuantity"]) * Convert.ToDouble(item["NumInBuy"]);
                            }
                        }
                    }
                }

                // Call the Web Method

                DataSet oDS = new DataSet();
                DataTable dtCopy = dtResult.Copy();

                oDS.Tables.Add(dtCopy);

                var data = new MasterService.MasterSoapClient();
                var sReturnMessage = data.Create_InventoryTransferRequest(Session[AppConstants.U_DBName].ToString().Trim(), oDS);

                if (sReturnMessage != "SUCCESS")
                {
                    lblError.Visible = true;
                    lblError.Text = sReturnMessage;
                }
                else
                {
                    lblError.Visible = true;
                    lblError.Text = "Inventory Transfer Request Created Successfully.";
                    ClearFields();
                    Session["AddDataToGrid"] = new DataTable();
                }
                oDS.Tables.Remove(dtCopy);
                oDS.Clear();
            }
        }

        protected void btnSubmitItems_Click(object sender, EventArgs e)
        {
            DataSet dsPopUpData = (DataSet)Session["ITPopUpData"];
            if (dsPopUpData != null && dsPopUpData.Tables.Count > 0)
            {
                DataTable dt = dsPopUpData.Tables[0];

                DataTable tblSelectedItem;
                DataTable dtExistingData = (DataTable)Session["AddDataToGrid"];
                if (dtExistingData == null)
                {
                    tblSelectedItem = CreateTableFormat();
                }
                else if (dtExistingData.Rows.Count == 0)
                {
                    tblSelectedItem = CreateTableFormat();
                }
                else
                {
                    tblSelectedItem = dtExistingData;
                }
                foreach (GridViewRow val in grvItemList.Rows)
                {
                    CheckBox chkItems = (CheckBox)val.FindControl("chkItems");
                    if (chkItems.Checked)
                    {
                        string DelChargeUDF = string.Empty;
                        DataRow rowNew = tblSelectedItem.NewRow();
                        Label lblItemCode = (Label)val.FindControl("lblItemCode");
                        Label lblItemDesc = (Label)val.FindControl("lblItemDesc");
                        TextBox txtOrderQuantity = (TextBox)val.FindControl("txtOrderQuantity");
                        Label lblUoM = (Label)val.FindControl("lblUoM");
                        Label lblInStock = (Label)val.FindControl("lblInStock");
                        Label lblQryGroup2 = (Label)val.FindControl("lblQryGroup2");
                        Label lblNumInBuy = (Label)val.FindControl("lblNumInBuy");
                        Label lblNumInSale = (Label)val.FindControl("lblNumInSale");

                        rowNew["No"] = tblSelectedItem.Rows.Count + 1;
                        rowNew["ItemCode"] = lblItemCode.Text;
                        rowNew["Description"] = lblItemDesc.Text;
                        rowNew["OrderQuantity"] = string.Empty;
                        rowNew["UOM"] = lblUoM.Text;
                        rowNew["InStock"] = lblInStock.Text;
                        rowNew["QryGroup2"] = lblQryGroup2.Text;
                        rowNew["NumInBuy"] = lblNumInBuy.Text;
                        rowNew["NumInSale"] = lblNumInSale.Text;

                        tblSelectedItem.Rows.Add(rowNew.ItemArray);
                    }
                }

                DataView dv = tblSelectedItem.DefaultView;
                dv.Sort = "Description  ASC";
                grvInvTransfer.DataSource = dv.ToTable();
                grvInvTransfer.DataBind();
                Session["AddDataToGrid"] = dv.ToTable();

                DataSet ds = new DataSet();
                ds.Tables.Add(tblSelectedItem);
                Session["ITTable"] = ds.Tables[0];

                lblError.Text = string.Empty;
                mpePopup.Hide();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            lblError.Visible = true;
            lblError.Text = string.Empty;
            Session["AddDataToGrid"] = new DataTable();
            ClearFields();
        }

        protected void txtSearch_OnTextChanged(object sender, EventArgs e)
        {
            btnPopUpSearch_Click(this, new System.EventArgs());
        }

        protected void btnPopUpSearch_Click(object sender, EventArgs e)
        {
            var searchKey = txtSearch.Text;
            if (searchKey != null && searchKey != string.Empty)
            {
                searchKey = searchKey.ToUpper();
            }
            DataSet ds = (DataSet)Session["ITPopUpData"];
            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable tb = ds.Tables[0];
                if (searchKey != string.Empty)
                {
                    tb.DefaultView.RowFilter = "Description LIKE '%" + searchKey + "%'";
                    tb = tb.DefaultView.ToTable();
                }
                else
                {
                    tb.DefaultView.RowFilter = "";
                    tb = tb.DefaultView.ToTable();
                }

                if (tb != null)
                {
                    grvItemList.DataSource = tb;
                    grvItemList.DataBind();
                }
                else
                {
                    grvItemList.DataSource = new DataTable();
                    grvItemList.DataBind();
                }
                txtSearch.Focus();
                mpePopup.Show();
            }
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

        private void BindData(DataTable tblData)
        {
            Session["ITTable"] = tblData;

            DataView dv = tblData.DefaultView;

            dv.Sort = "Description  ASC";

            this.grvInvTransfer.DataSource = dv.ToTable();
            this.grvInvTransfer.DataBind();
        }

        private void loadDropdowndata()
        {
            var data = new MasterService.MasterSoapClient();

            DataSet dsFromOutlet = data.Get_Outlet_Details(Session[Utils.AppConstants.SUPERUSER].ToString(), Session[Utils.AppConstants.WhsCode].ToString(),
                1, Session[AppConstants.U_ConnString].ToString());

            if (dsFromOutlet.Tables.Count != 0 && dsFromOutlet != null)
            {
                RemoveOutletandBind(dsFromOutlet.Tables[0]);
            }

            DataSet dsOutlet = data.Get_Outlet_Details(Session[Utils.AppConstants.SUPERUSER].ToString(), Session[Utils.AppConstants.WhsCode].ToString(),
                Convert.ToInt16(Session[AppConstants.ApprovalLevel]), Session[AppConstants.U_ConnString].ToString());
            if (dsOutlet.Tables.Count != 0 && dsOutlet != null)
            {

                ddlToOutlet.DataSource = dsOutlet.Tables[0];
                ddlToOutlet.DataTextField = dsOutlet.Tables[0].Columns[AppConstants.WhsName].ColumnName.ToString();
                ddlToOutlet.DataValueField = dsOutlet.Tables[0].Columns[AppConstants.WhsCode].ColumnName.ToString();
                ddlToOutlet.DataBind();
            }
        }

        private void mouseOverandMouseOut()
        {
            btnItemSearch.Attributes.Add("class", "static");
            btnItemSearch.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnItemSearch.Attributes.Add("onMouseOut", "this.className='static'");

            btnPopUpSearch.Attributes.Add("class", "static");
            btnPopUpSearch.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnPopUpSearch.Attributes.Add("onMouseOut", "this.className='static'");

            btnSubmitItems.Attributes.Add("class", "static");
            btnSubmitItems.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnSubmitItems.Attributes.Add("onMouseOut", "this.className='static'");

            btnClose.Attributes.Add("class", "static");
            btnClose.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnClose.Attributes.Add("onMouseOut", "this.className='static'");

            btnSubmit.Attributes.Add("class", "static");
            btnSubmit.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnSubmit.Attributes.Add("onMouseOut", "this.className='static'");

            btnCancel.Attributes.Add("class", "static");
            btnCancel.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnCancel.Attributes.Add("onMouseOut", "this.className='static'");
        }

        private void ClearFields()
        {
            ddlFromOutlet.SelectedIndex = -1;
            //txtSubmittedBy.Text = string.Empty;
            txtRemarks.Text = string.Empty;
            txtTransferDate.Text = DateTime.Now.ToShortDateString();
            grvInvTransfer.DataBind();
        }

        private DataTable CreateTableFormat()
        {
            DataTable tbTemp = new DataTable();
            tbTemp.Columns.Add("No");
            tbTemp.Columns.Add("ItemCode");
            tbTemp.Columns.Add("Description");
            tbTemp.Columns.Add("OrderQuantity");
            tbTemp.Columns.Add("UOM");
            tbTemp.Columns.Add("InStock");
            tbTemp.Columns.Add("FromOutlet");
            tbTemp.Columns.Add("ToOutlet");
            tbTemp.Columns.Add("SubmittedBy");
            tbTemp.Columns.Add("Remarks");
            tbTemp.Columns.Add("RequestDate");
            tbTemp.Columns.Add("QryGroup2");
            tbTemp.Columns.Add("NumInBuy");
            tbTemp.Columns.Add("NumInSale");

            return tbTemp;
        }

        private DataTable CreatePopHeaderFormat()
        {
            DataTable tbTemp = new DataTable();
            tbTemp.Columns.Add("No");
            tbTemp.Columns.Add("ItemCode");
            tbTemp.Columns.Add("Description");
            tbTemp.Columns.Add("OrderQuantity");
            tbTemp.Columns.Add("UOM");
            tbTemp.Columns.Add("InStock");
            tbTemp.Columns.Add("QryGroup2");
            tbTemp.Columns.Add("NumInBuy");
            tbTemp.Columns.Add("NumInSale");
            return tbTemp;
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            mpePopup.Hide();
        }
    }
}