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
    public partial class InventoryTransfer : System.Web.UI.Page
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
                        loadDropdowndata();
                        grvInvTransfer.DataBind();
                        mouseOverandMouseOut();
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Visible = true;
                lblError.Text = ex.Message.ToString();
            }
        }

        protected void btnItemSearch_Click(object sender, EventArgs e)
        {
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
                LoadPopupData(string.Empty, ddlFromOutlet.Text, Session[Utils.AppConstants.UserRole].ToString());
            }
        }

        protected void event_textChange(object sender, EventArgs e)
        {
            var data = txtTransferDate.Text;
            if (data != string.Empty)
            {

            }
        }

        private void loadDropdowndata()
        {
            var data = new MasterService.MasterSoapClient();
            DataSet dsOutlet = data.Get_Outlet_Details(Session[Utils.AppConstants.SUPERUSER].ToString(), Session[Utils.AppConstants.WhsCode].ToString(),
                Convert.ToInt16(Session[AppConstants.ApprovalLevel]), Session[AppConstants.U_ConnString].ToString());
            if (dsOutlet.Tables.Count != 0 && dsOutlet != null)
            {
                ddlFromOutlet.DataSource = dsOutlet.Tables[0];
                ddlFromOutlet.DataTextField = dsOutlet.Tables[0].Columns[AppConstants.WhsName].ColumnName.ToString();
                ddlFromOutlet.DataValueField = dsOutlet.Tables[0].Columns[AppConstants.WhsCode].ColumnName.ToString();
                ddlFromOutlet.DataBind();

                ddlToOutlet.DataSource = dsOutlet.Tables[0];
                ddlToOutlet.DataTextField = dsOutlet.Tables[0].Columns[AppConstants.WhsName].ColumnName.ToString();
                ddlToOutlet.DataValueField = dsOutlet.Tables[0].Columns[AppConstants.WhsCode].ColumnName.ToString();
                ddlToOutlet.DataBind();
            }
        }

        private void LoadPopupData(string supplier, string outlet, string userRole)
        {
            try
            {
                DataTable tblAddItem = CreatePopHeaderFormat();
                var objC = new MasterService.MasterSoapClient();
                DataSet ds = objC.Get_MaterialReqBySupplier(supplier, outlet, userRole, Session[AppConstants.U_ConnString].ToString());
                Session["ITPopUpData"] = ds;
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (supplier == string.Empty && outlet == "01CKT")
                    {
                        DataTable dt = new DataTable();
                        string code = "VA-HARCT";
                        var rows = from row in ds.Tables[0].AsEnumerable()
                                   where !row.Field<string>(AppConstants.SupplierCode).Contains(code.ToUpper())
                                   select row;
                        if (rows.Count() > 0)
                        {
                            dt = rows.CopyToDataTable();
                        }

                        ds = new DataSet();
                        ds.Tables.Add(dt.Copy());
                        Session["ITPopUpData"] = ds;
                    }

                    int i = 1;
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        DataRow rowNew = tblAddItem.NewRow();
                        rowNew["No"] = i;
                        rowNew["ItemCode"] = row["ItemCode"];
                        rowNew["Description"] = row["ItemName"];
                        rowNew["UOM"] = row["UOM"];
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

        private DataTable CreatePopHeaderFormat()
        {
            DataTable tbTemp = new DataTable();
            tbTemp.Columns.Add("No");
            tbTemp.Columns.Add("ItemCode");
            tbTemp.Columns.Add("Description");
            tbTemp.Columns.Add("OrderQuantity");
            tbTemp.Columns.Add("UOM");
            return tbTemp;
        }

        public void mouseOverandMouseOut()
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
        }

        protected void ddlFromOutlet_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblError.Visible = false;
            lblError.Text = string.Empty;
            grvInvTransfer.DataBind();
        }

        protected void ddlToOutlet_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblError.Visible = false;
            lblError.Text = string.Empty;
            grvInvTransfer.DataBind();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (txtSubmittedBy.Text == string.Empty)
            {
                lblError.Visible = true;
                lblError.Text = "Kindly enter the Submitted by.";
                return;
            }
            else if (txtApprovedBy.Text == string.Empty)
            {
                lblError.Visible = true;
                lblError.Text = "Kindly enter the Approved by.";
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
        }

        protected void btnSubmitItems_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)Session["ITPopUpData"];
            //DataTable selectedData = new DataTable();
            DataTable tblSelectedItem = CreateTableFormat();
            foreach (GridViewRow val in grvItemList.Rows)
            {
                CheckBox chkItems = (CheckBox)val.FindControl("chkItems");
                if (chkItems.Checked)
                {
                    string DelChargeUDF = string.Empty;
                    DataRow rowNew = dt.NewRow();
                    Label lblItemCode = (Label)val.FindControl("lblItemCode");
                    Label lblItemDesc = (Label)val.FindControl("lblItemDesc");
                    TextBox txtOrderQuantity = (TextBox)val.FindControl("txtOrderQuantity");
                    Label lblUoM = (Label)val.FindControl("lblUoM");

                    rowNew["No"] = dt.Rows.Count + 1;
                    rowNew["ItemCode"] = lblItemCode.Text;
                    rowNew["Description"] = lblItemDesc.Text;
                    rowNew["OrderQuantity"] = txtOrderQuantity.Text;
                    rowNew["UOM"] = lblUoM.Text;
                    dt.Rows.Add(rowNew.ItemArray);
                    tblSelectedItem.Rows.Add(rowNew.ItemArray);
                    //dt.Rows.Add(rowNew);
                    //tblSelectedItem.Rows.Add(rowNew);
                }
            }

            grvInvTransfer.DataSource = dt;
            grvInvTransfer.DataBind();
            DataSet ds = new DataSet();
            DataSet ds1 = new DataSet();
            ds.Tables.Add(dt);
            ds1.Tables.Add(tblSelectedItem);
            //DataTable ItemPopupData = new DataTable();
            //ItemPopupData = (DataTable)Session["ItemPopupTable"];
            //DataTable t1 = (DataTable)Session["DraftDetails"];
            //ds.Tables.Add(t1);
            Session["ITTable"] = ds.Tables[0];
            
            // this is to disable the submit button
            lblError.Text = string.Empty;
        }

        private DataTable CreateTableFormat()
        {
            DataTable tbTemp = new DataTable();
            tbTemp.Columns.Add("No");
            tbTemp.Columns.Add("ItemCode");
            tbTemp.Columns.Add("Description");
            tbTemp.Columns.Add("OrderQuantity");
            tbTemp.Columns.Add("UOM");
            return tbTemp;
        }
    }
}