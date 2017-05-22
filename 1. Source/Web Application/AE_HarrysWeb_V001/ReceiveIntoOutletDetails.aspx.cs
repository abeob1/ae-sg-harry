using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AE_HarrysWeb_V001.Utils;
using System.Data;


namespace AE_HarrysWeb_V001
{
    public partial class ReceiveIntoOutletDetails : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session[Utils.AppConstants.UserCode].ToString() != string.Empty)
                {
                    if (!IsPostBack)
                    {
                        Session["ReasonSelected"] = string.Empty;
                        Session["EmptyReasonCount"] = string.Empty;
                        Session["Alert"] = "";
                        //lblReceiptDate.Text = DateTime.Now.ToShortDateString();
                        //Session[AppConstants.IsBackPage] = 0;
                        lblUserName.Text = Session[Utils.AppConstants.UserCode].ToString().ToUpper();
                        lblWareHouse.Text = Session["wareHouseCode"].ToString();
                        lblSupplierCode.Text = Request.QueryString[0].ToString();
                        lblSupplier.Text = Request.QueryString[1].ToString();
                        lblNoOfOpenPO.Text = Request.QueryString[2].ToString();
                        btnAdd.Enabled = false;
                        btnAdd.Style["background-image"] = "url('Images/bgButton_disable.png')";
                        //LoadData(this.lblWareHouse.Text.ToString(), this.lblSupplier.Text.ToString());
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
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
                //Response.Redirect("~/ErrorPage.aspx");
                //Response.Redirect("ErrorPage.aspx");
            }
        }

        private void LoadData(string sOutlet, string sSupplier, string sPONumber)
        {
            try
            {
                var data = new MasterService.MasterSoapClient();
                //Calling the webmethod                
                sSupplier = sSupplier.Replace("'", "''");
                DataSet ds = data.Get_ReceiveInOutlet_Details(Session[AppConstants.DBName].ToString(), sOutlet, Session[AppConstants.UserRole].ToString(), sSupplier);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    // Search based on the PO Number
                    if (sPONumber.Trim() == string.Empty)
                    {
                        Session["RIOD"] = dt;
                    }
                    else
                    {
                        //dt = ds.Tables["student"];
                        dt.DefaultView.RowFilter = "DocNum='" + sPONumber.ToString().Trim() + "'";
                        dt = dt.DefaultView.ToTable();
                        Session["RIOD"] = dt;
                    }

                    DataSet dsreasonList = data.Get_ReasonDetails(Session[AppConstants.DBName].ToString(), this.lblWareHouse.Text.ToString());

                    DataRow dr = dsreasonList.Tables[0].NewRow();
                    dr["Code"] = "A";
                    dr["Name"] = " --- Select Reason --- ";
                    dsreasonList.Tables[0].Rows.Add(dr);
                    dsreasonList.Tables[0].DefaultView.Sort = "Code DESC";

                    Session["ReasonList"] = dsreasonList;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        btnAdd.Enabled = true;
                        btnAdd.Style["background-image"] = "url('Images/bgButton.png')";
                    }
                    else
                    {
                        btnAdd.Enabled = false;
                        btnAdd.Style["background-image"] = "url('Images/bgButton_disable.png')";
                    }
                    grvRIOD.DataSource = dt;
                    grvRIOD.DataBind();
                }
                else
                {
                    grvRIOD.DataBind();
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = "Failed to load data in grid ..." + ex.Message;
                this.lblError.Visible = true;

            }
        }

        public void mouseOverandMouseOut()
        {
            btnAdd.Attributes.Add("class", "static");
            btnAdd.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnAdd.Attributes.Add("onMouseOut", "this.className='static'");

            btnINVAlert.Attributes.Add("class", "static");
            btnINVAlert.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnINVAlert.Attributes.Add("onMouseOut", "this.className='static'");

            btnINVCancel.Attributes.Add("class", "static");
            btnINVCancel.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnINVCancel.Attributes.Add("onMouseOut", "this.className='static'");

            btnPOAlert.Attributes.Add("class", "static");
            btnPOAlert.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnPOAlert.Attributes.Add("onMouseOut", "this.className='static'");

            btnPOCancel.Attributes.Add("class", "static");
            btnPOCancel.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnPOCancel.Attributes.Add("onMouseOut", "this.className='static'");

            btnPOSearch.Attributes.Add("class", "static");
            btnPOSearch.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnPOSearch.Attributes.Add("onMouseOut", "this.className='static'");
        }

        protected void grvRIOD_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //int i = Convert.ToInt32(Session["EmptyReasonCount"]);
                    DropDownList ddlReason = e.Row.FindControl("ddlReasonCode") as DropDownList;
                    Label lblItemCode = (Label)e.Row.FindControl("lblItemCode");
                    Label lblDocEntry = (Label)e.Row.FindControl("lblDocEntry");
                    Label lblLineNum = (Label)e.Row.FindControl("lblLineNum");
                    Label lblOrderQty = (Label)e.Row.FindControl("lblOrderQty");
                    TextBox txtReceiptQty = (TextBox)e.Row.FindControl("txtReceiptQuantity");
                    if (txtReceiptQty.Text == string.Empty)
                    {
                        txtReceiptQty.Text = lblOrderQty.Text;
                    }
                    DataTable dt = (DataTable)Session["RIOD"];
                    DataRow[] rupdate = dt.Select("ItemCode='" + lblItemCode.Text + "' AND DocEntry = '" + lblDocEntry.Text + "' AND LineNum = '" + lblLineNum.Text + "'");
                    PopulateReasonCode(ddlReason);
                    //string Value = ddlReason.SelectedValue;
                    if (rupdate.Length > 0)
                    {
                        ddlReason.SelectedValue = rupdate[0]["ReasonCode"].ToString();
                        rupdate[0]["ReceiptQty"] = txtReceiptQty.Text;
                    }
                    Session["RIOD"] = dt;
                    Label lblImageURL = e.Row.FindControl("lblImageURL") as Label;
                    Image ItemImage = e.Row.FindControl("ItemImage") as Image;
                    ItemImage.ImageUrl = "/GetImage.ashx?file=" + GetFileFromPath(lblImageURL.Text);

                    //if (double.Parse(txtReceiptQty.Text).ToString(Utils.AppConstants.NUMBER_FORMAT_1) != double.Parse(lblOrderQty.Text).ToString(Utils.AppConstants.NUMBER_FORMAT_1) && Session["ReasonSelected"].ToString() == "0")
                    //{
                    //    Session["EmptyReasonCount"] = 1;
                    //    ddlReason.SelectedValue = "A";
                    //    DataRow[] rupdate1 = dt.Select("ItemCode='" + lblItemCode.Text + "' AND DocEntry = '" + lblDocEntry.Text + "' AND LineNum = '" + lblLineNum.Text + "'");
                    //    rupdate1[0]["ReasonCode"] = ddlReason.SelectedValue;
                    //    Session["RIOD"] = dt;
                    //    //Session["ReasonSelected"] = 1;
                    //}
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }

        private void PopulateReasonCode(DropDownList ddl)
        {
            try
            {
                //var ws = new MasterService.MasterSoapClient();
                //DataSet ds = ws.Get_ReasonDetails(Session[AppConstants.DBName].ToString(), this.lblWareHouse.Text.ToString());
                DataSet dsTable = new DataSet();
                dsTable = (DataSet)Session["ReasonList"];
                if (dsTable != null)
                {
                    //DataTable dt = ds.Tables[0];
                    ddl.DataSource = dsTable.Tables[0];
                    ddl.DataTextField = "Name";
                    ddl.DataValueField = "Code";
                    ddl.DataBind();
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = "Failed while loading Reason Code" + ex.Message;
                this.lblError.Visible = true;
            }
        }

        protected void grvRIOD_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

            this.grvRIOD.PageIndex = e.NewPageIndex;
            DataTable tblIRIOD = (DataTable)Session["RIOD"];
            BindData(tblIRIOD);

        }

        protected void txtReceiptQuantity_OnTextChanged(object sender, EventArgs e)
        {
            Session["ReasonSelected"] = "0";
            lblError.Text = string.Empty;
            lblError.Visible = false;

            GridViewRow row = (GridViewRow)((DataControlFieldCell)((TextBox)sender).Parent).Parent;
            ViewState["rowindex"] = row.RowIndex;
            TextBox txtQuantity = (TextBox)row.FindControl("txtReceiptQuantity");

            Label lblItemCode = (Label)row.FindControl("lblItemCode");
            Label lblDocEntry = (Label)row.FindControl("lblDocEntry");
            Label lblLineNum = (Label)row.FindControl("lblLineNum");
            DataTable tb = (DataTable)Session["RIOD"];
            if (tb != null)
            {
                DataRow[] rupdate = tb.Select("ItemCode='" + lblItemCode.Text + "' AND DocEntry = '" + lblDocEntry.Text + "' AND LineNum = '" + lblLineNum.Text + "'");
                if (rupdate.Length > 0)
                {
                    rupdate[0]["ReceiptQty"] = txtQuantity.Text == string.Empty ? "0" : txtQuantity.Text;
                }
                this.grvRIOD.EditIndex = -1;
                Label lblOrderQty = (Label)row.FindControl("lblOrderQty");
                if (double.Parse(txtQuantity.Text).ToString(Utils.AppConstants.NUMBER_FORMAT_1) != double.Parse(lblOrderQty.Text).ToString(Utils.AppConstants.NUMBER_FORMAT_1))
                {
                    Session["EmptyReasonCount"] = 1;
                    rupdate[0]["ReasonCode"] = "A";
                }
                BindData(tb);
            }
        }

        protected void ddlReasonCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ReasonSelected"] = "1";
            lblError.Text = string.Empty;
            lblError.Visible = false;
            GridViewRow row = (GridViewRow)((DataControlFieldCell)((DropDownList)sender).Parent).Parent;
            ViewState["rowindex"] = row.RowIndex;
            DropDownList ddlReason = (DropDownList)row.FindControl("ddlReasonCode");
            Label lblItemCode = (Label)row.FindControl("lblItemCode");
            Label lblDocEntry = (Label)row.FindControl("lblDocEntry");
            Label lblLineNum = (Label)row.FindControl("lblLineNum");
            DataTable tb = (DataTable)Session["RIOD"];
            if (tb != null)
            {
                DataRow[] rupdate = tb.Select("ItemCode='" + lblItemCode.Text + "' AND DocEntry = '" + lblDocEntry.Text + "'AND LineNum = '" + lblLineNum.Text + "'");
                if (rupdate.Length > 0)
                {
                    rupdate[0]["ReasonCode"] = ddlReason.SelectedValue;
                    Session["EmptyReasonCount"] = 0;
                }
                if (ddlReason.SelectedValue == "A")
                {
                    Session["EmptyReasonCount"] = 1;
                }
                this.grvRIOD.EditIndex = -1;
                BindData(tb);
            }
        }

        private void BindData(DataTable tblIRIOD)
        {
            Session["RIOD"] = tblIRIOD;
            DataView dv = tblIRIOD.DefaultView;
            this.grvRIOD.DataSource = dv.ToTable();
            this.grvRIOD.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                lblError.Text = string.Empty;
                int count = 0;
                DataTable tbNew = new DataTable();
                DataTable dtResult = new DataTable();
                dtResult = (DataTable)Session["RIOD"];
                tbNew = CreateTable();

                if (txtReceiptDate.Text == string.Empty)
                {
                    lblError.Text = "Receipt Date cannot be Empty.";
                    lblError.Visible = true;
                    return;
                }

                foreach (DataRow item in dtResult.Rows)
                {
                    DataRow rowNew = tbNew.NewRow();

                    rowNew["LineNum"] = item["LineNum"].ToString();
                    rowNew["DocEntry"] = item["DocEntry"].ToString();
                    rowNew["DocNum"] = item["DocNum"].ToString();
                    rowNew["SupplierCode"] = lblSupplierCode.Text;
                    rowNew["Description"] = item["Dscription"].ToString();
                    rowNew["Order Qty"] = item["Quantity"].ToString();
                    rowNew["Receipt Qty"] = item["ReceiptQty"].ToString() == string.Empty ? "0.00" : item["ReceiptQty"].ToString();
                    rowNew["Reason Code"] = item["ReasonCode"].ToString();
                    if (item["ReasonCode"].ToString() == "A")
                    {
                        count = count + 1;
                    }
                    rowNew["ItemCode"] = item["ItemCode"].ToString();
                    rowNew["Outlet"] = lblWareHouse.Text;
                    if (Session["Alert"].ToString() == "Y")
                    {
                        rowNew["CloseStatus"] = "Y";
                    }
                    else if (Session["Alert"].ToString() == "N")
                    {
                        rowNew["CloseStatus"] = "N";
                    }
                    rowNew["OutletReceivedBy"] = Convert.ToString(Session[AppConstants.UserName]);
                    rowNew["ReceiptDate"] = txtReceiptDate.Text.ToString();
                    tbNew.Rows.Add(rowNew);
                }

                //foreach (GridViewRow row in grvRIOD.Rows)
                //{
                //    Label lblLineNum = (Label)row.FindControl("lblLineNum");
                //    Label lblDocNum = (Label)row.FindControl("lblDocNum");
                //    Label lblDocEntry = (Label)row.FindControl("lblDocEntry");
                //    string CardCode = lblSupplierCode.Text;
                //    Label lblDescription = (Label)row.FindControl("lblDescription");
                //    Label lblItemCode = (Label)row.FindControl("lblItemCode");
                //    Label lblOrderQty = (Label)row.FindControl("lblOrderQty");
                //    TextBox txtReceiptQuantity = (TextBox)row.FindControl("txtReceiptQuantity");
                //    DropDownList ddl1 = row.FindControl("ddlReasonCode") as DropDownList;
                //    string ReasonCode = ddl1.SelectedValue.ToString();

                //    DataRow rowNew = tbNew.NewRow();

                //    rowNew["LineNum"] = lblLineNum.Text;
                //    rowNew["DocEntry"] = lblDocEntry.Text;
                //    rowNew["DocNum"] = lblDocNum.Text;
                //    rowNew["SupplierCode"] = CardCode;
                //    rowNew["Description"] = lblDescription.Text;
                //    rowNew["Order Qty"] = lblOrderQty.Text;
                //    rowNew["Receipt Qty"] = txtReceiptQuantity.Text == string.Empty ? "0.000000" : txtReceiptQuantity.Text;
                //    rowNew["Reason Code"] = ReasonCode;
                //    rowNew["ItemCode"] = lblItemCode.Text;
                //    rowNew["Outlet"] = lblWareHouse.Text;
                //    if (Session["Alert"].ToString() == "Y")
                //    {
                //        rowNew["CloseStatus"] = "Y";
                //    }
                //    else if (Session["Alert"].ToString() == "N")
                //    {
                //        rowNew["CloseStatus"] = "N";
                //    }
                //    tbNew.Rows.Add(rowNew);
                //}

                // var result = from r in tbNew.AsEnumerable().Sum(r=>r.Field<decimal>("Receipt Qty"));
                var result = tbNew.AsEnumerable().Sum(x => x["Receipt Qty"] == null ? 0 : Convert.ToDouble(x["Receipt Qty"]));
                if (result <= 0)
                {
                    lblError.Text = "Receipt quantity cannot be Zero/Empty.";
                    lblError.Visible = true;
                    return;
                }

                if (count != 0)
                {
                    lblError.Text = "Kindly select the Reason Code.";
                    lblError.Visible = true;
                    return;
                }

                if (Session["Alert"].ToString() == "")
                {
                    //if (Convert.ToInt32(Session[AppConstants.ReceiveMessage]) == 0)
                    //{
                    //    mINVPopup.Show();
                    //}else
                    if (Convert.ToInt32(Session[AppConstants.ReceiveMessage]) == 1)
                    {
                        if (Convert.ToInt32(lblNoOfOpenPO.Text.ToString()) != 0)
                        {
                            mPOPopup.Show();
                        }
                        else
                        {
                            mPOPopup.Hide();
                            Session["Alert"] = "N";
                        }
                    }
                }

                // if the user chooses central kitchen, then if part will be executed otherwise else part will be executed.
                if (Convert.ToInt32(Session[AppConstants.ReceiveMessage]) == 0)
                {
                    if (Convert.ToInt32(Session[AppConstants.ReceiveMessage]) == 1)
                    {
                        mPOPopup.Hide();
                    }
                    DataSet ds = new DataSet();
                    ds.Tables.Add(tbNew.Copy());
                    // pass this dataset to the web service method
                    var data = new MasterService.MasterSoapClient();

                    var returnResult = data.ReceiveInOutlet(ds, Session[AppConstants.DBName].ToString());
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
                        if (Convert.ToInt32(Session[AppConstants.ReceiveMessage]) == 0)
                        {
                            grvRIOD.DataBind();
                            lblError.Visible = true;
                            lblError.Text = "Inventory Transfer Document Created Successfully.";
                        }
                        else if (Convert.ToInt32(Session[AppConstants.ReceiveMessage]) == 1)
                        {
                            grvRIOD.DataBind();
                            lblError.Visible = true;
                            lblError.Text = "GRPO Document is Created Successfully.";
                        }
                    }
                    ds.Clear();
                }
                else
                {
                    if (Session["Alert"].ToString() == "Y" || Session["Alert"].ToString() == "N")
                    {
                        //if (Convert.ToInt32(Session[AppConstants.ReceiveMessage]) == 0)
                        //{
                        //    mINVPopup.Hide();
                        //}
                        //else 

                        if (Convert.ToInt32(Session[AppConstants.ReceiveMessage]) == 1)
                        {
                            mPOPopup.Hide();
                        }
                        DataSet ds = new DataSet();
                        ds.Tables.Add(tbNew.Copy());
                        // pass this dataset to the web service method
                        var data = new MasterService.MasterSoapClient();

                        var returnResult = data.ReceiveInOutlet(ds, Session[AppConstants.DBName].ToString());
                        if (returnResult != "SUCCESS")
                        {
                            lblError.Visible = true;
                            lblError.Text = returnResult;
                        }

                        else
                        {
                            if (Convert.ToInt32(Session[AppConstants.ReceiveMessage]) == 0)
                            {
                                grvRIOD.DataBind();
                                lblError.Visible = true;
                                lblError.Text = "Inventory Transfer Document Created Successfully.";
                            }
                            else if (Convert.ToInt32(Session[AppConstants.ReceiveMessage]) == 1)
                            {
                                grvRIOD.DataBind();
                                lblError.Visible = true;
                                lblError.Text = "GRPO Document is Created Successfully.";
                            }
                        }
                        ds.Clear();
                    }
                }
            }
            catch (Exception ex)
            {

                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }

        }

        protected void btnPOSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadData(this.lblWareHouse.Text.ToString(), this.lblSupplier.Text.ToString(), txtPOSearch.Text.Trim());
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        protected void btnPOAlert_Click(object sender, EventArgs e)
        {
            Session["Alert"] = "Y";
            mPOPopup.Hide();
            btnAdd_Click(this, new System.EventArgs());
        }

        protected void btnINVAlert_Click(object sender, EventArgs e)
        {
            Session["Alert"] = "Y";
            mINVPopup.Hide();
            btnAdd_Click(this, new System.EventArgs());
        }

        protected void btnPOCancel_Click(object sender, EventArgs e)
        {
            Session["Alert"] = "N";
            mPOPopup.Hide();
            btnAdd_Click(this, new System.EventArgs());
        }

        protected void btnINVCancel_Click(object sender, EventArgs e)
        {
            Session["Alert"] = "N";
            mINVPopup.Hide();
            btnAdd_Click(this, new System.EventArgs());
        }

        public static string GetFileFromPath(string strPath)
        {
            string fileName = string.Empty;

            if (!string.IsNullOrEmpty(strPath))
            {
                fileName = System.IO.Path.GetFileName(strPath);
            }

            return fileName;
        }

        private DataTable CreateTable()
        {
            DataTable tbReceipt = new DataTable();
            tbReceipt.Columns.Add("LineNum");
            tbReceipt.Columns.Add("DocNum");
            tbReceipt.Columns.Add("DocEntry");
            tbReceipt.Columns.Add("SupplierCode");
            tbReceipt.Columns.Add("Description");
            tbReceipt.Columns.Add("Order Qty");
            tbReceipt.Columns.Add("Receipt Qty");
            tbReceipt.Columns.Add("Reason Code");
            tbReceipt.Columns.Add("ItemCode");
            tbReceipt.Columns.Add("Outlet");
            tbReceipt.Columns.Add("CloseStatus");
            tbReceipt.Columns.Add("OutletReceivedBy");
            tbReceipt.Columns.Add("ReceiptDate");
            return tbReceipt;
        }

    }

}