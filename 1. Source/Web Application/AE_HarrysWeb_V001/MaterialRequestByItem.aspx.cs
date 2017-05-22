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
    public partial class MaterialRequestByItem : System.Web.UI.Page
    {
        private decimal TotalAmt = (decimal)0.0;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session[Utils.AppConstants.UserCode].ToString() != string.Empty)
                {
                    if (!IsPostBack)
                    {
                        lblOrderDate.Text = DateTime.Now.ToShortDateString();
                        Session[AppConstants.IsBackPage] = 0;
                        Session[AppConstants.OrderDate] = DateTime.Now;
                        lblUserName.Text = Session[Utils.AppConstants.UserCode].ToString().ToUpper();
                        lblStatus.Text = AppConstants.DefaultStatus;
                        // Loading outlet data
                        LoadOutletDropDown();
                        // Loading in Grid view
                        LoadData(this.ddlWareHouse.SelectedValue.ToString());
                        mouseOverandMouseOut();
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

        protected void ddlWareHouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.ddlWareHouse.Text.Trim().Length > 0)
                {
                    lblError.Visible = false;
                    lblError.Text = string.Empty;
                    txtDeliveryDate.Text = string.Empty;
                    txtSearch.Text = string.Empty;
                    this.grvParentGrid.DataSource = null;
                    this.grvParentGrid.DataBind();
                    LoadData(this.ddlWareHouse.SelectedValue.ToString());
                }
                else
                {
                    this.lblError.Text = "Can not get Outlet list.";
                    this.lblError.Visible = true;

                }
            }

            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }

        protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            TotalAmt = 0;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string SuppCode = grvParentGrid.DataKeys[e.Row.RowIndex].Value.ToString();
                CheckBoxList chkCalendar = e.Row.FindControl("chkCalendar") as CheckBoxList;
                DisplayCalendar(chkCalendar, SuppCode);
                //e.Row.Attributes.Add("onmouseover", "MouseEvents(this, event)");
                //e.Row.Attributes.Add("onmouseout", "MouseEvents(this, event)");
                e.Row.Attributes["style"] = "cursor:pointer";

                GridView grvSupplierItemList = e.Row.FindControl("grvSupplierItemList") as GridView;
                grvSupplierItemList.ToolTip = SuppCode;
                if (Session["MRBISearch"] == null)
                {
                    grvSupplierItemList.DataSource = GetItemData(SuppCode);
                    grvSupplierItemList.DataBind();
                    Session["supplierCount"] = grvSupplierItemList.DataSource;
                }
                else
                {

                    grvSupplierItemList.DataSource = GetItemData_Search(SuppCode);
                    grvSupplierItemList.DataBind();
                    Session["supplierCount"] = grvSupplierItemList.DataSource;
                }
            }
        }

        protected void grvSupplierItemList_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("onmouseover", "MouseEvents(this, event)");
                e.Row.Attributes.Add("onmouseout", "MouseEvents(this, event)");
                e.Row.Attributes["style"] = "cursor:pointer";
                int index = 0;
                TextBox ddl1 = e.Row.FindControl("txtOrderQuantity") as TextBox;
                TextBox ddl2 = e.Row.FindControl("txtRemarks") as TextBox;
                GridView grvSupplierItemList = e.Row.FindControl("grvSupplierItemList") as GridView;

                if (Convert.ToString(Session["Remarks"]) == "txtRemarks")
                {
                    //if (e.Row.RowIndex > 0)
                    //{
                    //    TextBox txtNowQuantity = e.Row.FindControl("txtOrderQuantity") as TextBox;
                    //    TextBox txtPreviousQuantity = e.Row.FindControl("txtOrderQuantity") as TextBox;
                    //    txtPreviousQuantity.Attributes.Add("onkeyup", "FindNextRow('" + txtNowQuantity.ClientID + "');");
                    //}

                    ddl1.Attributes.Add("onfocus", "JavaScript:document.getElementById('" + ddl1.ClientID + "').select();");
                    if (ViewState["rowindex"] != null && Session["MRBI"] != null)
                    {
                        index = Convert.ToInt32(ViewState["rowindex"].ToString());
                        int rowCount = ((DataTable)Session["MRBI"]).Rows.Count;
                        if ((index + 1) <= this.grvParentGrid.Rows.Count)
                        {
                            if (e.Row.RowIndex == index + 1)
                            {
                                ddl1.Focus();
                                Session["Remarks"] = string.Empty;
                            }
                        }
                    }
                }
                else
                {
                    DataTable dtnew = (DataTable)Session["supplierCount"];
                    ddl2.Attributes.Add("onfocus", "JavaScript:document.getElementById('" + ddl2.ClientID + "').select();");
                    // ddl1.Attributes.Add("onkeydown", "return (event.keyCode!=13);");
                    if (ViewState["rowindex"] != null && Session["MRBI"] != null)
                    {
                        index = Convert.ToInt32(ViewState["rowindex"].ToString());
                        int rowCount = ((DataTable)Session["MRBI"]).Rows.Count;
                        if ((index + 1) == this.grvParentGrid.Rows.Count)
                        {
                            if (e.Row.RowIndex == index + 1)
                            {
                                ddl2.Focus();
                            }
                        }
                    }
                }
                TotalAmt += Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "Total"));
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[13].Text = String.Format("${0}", TotalAmt);
                e.Row.Cells[13].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Font.Bold = true;
            }
        }

        protected void grvSupplierItemList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView gvwChild = (sender as GridView);
            gvwChild.PageIndex = e.NewPageIndex;
            gvwChild.DataSource = GetItemData(gvwChild.ToolTip);
            gvwChild.DataBind();
        }

        protected void txtOrderQuantity_OnTextChanged(object sender, EventArgs e)
        {
            GridViewRow row = (GridViewRow)((DataControlFieldCell)((TextBox)sender).Parent).Parent;
            ViewState["rowindex"] = row.RowIndex;
            TextBox txtQuantity = (TextBox)row.FindControl("txtOrderQuantity");
            //if (txtQuantity.Text.Trim().Length == 0)
            //{
            //    ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", "alert('Pls input quantity');", true);
            //    txtQuantity.Focus();
            //    return;
            //}
            Label lblItemCode = (Label)row.FindControl("lblItemCode");
            Label lblSupplierCode = (Label)row.FindControl("lblSupplierCode");
            DataTable tb = (DataTable)Session["MRBI"];
            if (tb != null)
            {
                DataRow[] rupdate = tb.Select("SupplierCode='" + lblSupplierCode.Text + "' and ItemCode='" + lblItemCode.Text + "'");
                if (rupdate.Length > 0)
                {
                    rupdate[0]["OrderQuantity"] = txtQuantity.Text.Trim() == string.Empty ? "0" : txtQuantity.Text.Trim();
                    GetPrice(rupdate[0]);
                }
                this.grvParentGrid.EditIndex = -1;
                Session["MRBI"] = null;
                Session["MRBI"] = tb;
                BindData(tb, string.Empty);
                txtSearch.Text = string.Empty;
                //CalcTotal(tb);
            }
        }

        protected void txtRemarks_OnTextChanged(object sender, EventArgs e)
        {
            GridViewRow row = (GridViewRow)((DataControlFieldCell)((TextBox)sender).Parent).Parent;
            ViewState["rowindex"] = row.RowIndex;
            Session["Remarks"] = "txtRemarks";
            TextBox txtRemaks = (TextBox)row.FindControl("txtRemarks");

            Label lblItemCode = (Label)row.FindControl("lblItemCode");
            Label lblSupplierCode = (Label)row.FindControl("lblSupplierCode");

            DataTable tb = (DataTable)Session["MRBI"];
            if (tb != null)
            {
                DataRow[] rupdate = tb.Select("SupplierCode='" + lblSupplierCode.Text + "' and ItemCode='" + lblItemCode.Text + "'");
                if (rupdate.Length > 0)
                {
                    rupdate[0]["Remarks"] = txtRemaks.Text == string.Empty ? "0" : txtRemaks.Text; ;
                    //GetPrice(rupdate[0]);
                }
                this.grvParentGrid.EditIndex = -1;
                Session["MRBI"] = null;
                Session["MRBI"] = tb;
                BindData(tb, string.Empty);
                txtSearch.Text = string.Empty;
                //CalcTotal(tb);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {

            var searchKey = txtSearch.Text;

            DataTable tb = (DataTable)Session["MRBI"];
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
                Session["MRBISearch"] = tb;
                BindData(tb, "Search");
                Session["MRBISearch"] = null;
            }
            else
            {
                this.grvParentGrid.DataSource = new DataTable();
                this.grvParentGrid.DataBind();
            }
            txtSearch.Focus();
        }

        protected void txtSearch_OnTextChanged(object sender, EventArgs e)
        {

            btnSearch_Click(this, new System.EventArgs());
        }

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            LoadHeaderTableData();
            DataTable HeaderTable = (DataTable)Session["MRBIHeaderTable"];
            DataTable LineTable = (DataTable)Session["MRBI"];
            var result = LineTable.AsEnumerable().Sum(x => x["OrderQuantity"] == DBNull.Value ? 0 : Convert.ToDecimal(x["OrderQuantity"]));

            if ((HeaderTable != null && HeaderTable.Rows.Count > 0) && (LineTable != null && LineTable.Rows.Count > 0) && txtDeliveryDate.Text != string.Empty &&
                ddlWareHouse.Text != " --- Select Outlet --- ")
            {
                if (Convert.ToDateTime(lblOrderDate.Text).Date == Convert.ToDateTime(txtDeliveryDate.Text).Date)
                {
                    lblError.Visible = true;
                    lblError.Text = "Order date and Delivery date should not be same.";
                    return;
                }
                else if (Convert.ToDateTime(txtDeliveryDate.Text).Date < Convert.ToDateTime(lblOrderDate.Text).Date)
                {
                    lblError.Visible = true;
                    lblError.Text = "Delivery date should not be less than Order date.";
                    return;
                }
                if (result > 0)
                {
                    foreach (DataRow row in LineTable.Rows)
                    {
                        if (row["DeliveryDate"].ToString() == string.Empty)
                        {
                            row["DeliveryDate"] = Convert.ToDateTime(txtDeliveryDate.Text).Date;
                        }
                    }
                    HeaderTable.TableName = "tblHeader";
                    LineTable.TableName = "tblLine";
                    DataSet ds = new DataSet();
                    DataTable table1 = HeaderTable.Copy();
                    DataTable table2 = LineTable.Copy();
                    ds.Tables.Add(table1);
                    ds.Tables.Add(table2);
                    var data = new MasterService.MasterSoapClient();
                    // Remove the Rows, in which the quantity is Non Zero
                    DataSet NonZeroDataset = FetchNotNullValues(ds);
                    table2 = NonZeroDataset.Tables[1];

                    //var returnResult = data.Create_PurchaseRequest(ds, true, Session[AppConstants.U_DBName].ToString(), true,false ,false);

                    var returnResult = data.Insert_PurchaseRequest(ds, true, Session[AppConstants.U_DBName].ToString(), true, false, false);

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
                        lblError.Text = "Material Request Draft Created Successfully.";
                        ClearFields();
                    }
                    ds.Tables.Remove(table1);
                    ds.Tables.Remove(table2);
                    ds.Clear();
                    ds.Dispose();
                }
                else
                {
                    lblError.Visible = true;
                    lblError.Text = "Order Quantity cannot be Zero/Empty.";
                }
            }
            else
            {
                if (ddlWareHouse.Text == " --- Select Outlet --- ")
                {
                    lblError.Visible = true;
                    lblError.Text = "Please select the Outlet.";
                }
                else if (txtDeliveryDate.Text == string.Empty)
                {
                    lblError.Visible = true;
                    lblError.Text = "Please select the Delivery Date.";
                }
            }


        }

        private DataTable GetItemData(string supplier)
        {
            try
            {
                DataTable dt = new DataTable();
                DataTable tblIMRBI = CreateTabelFormat();
                DataTable dtItem = (DataTable)Session["MRBI"];
                DataTable dtItemList = null;
                dtItemList = dtItem.Clone();
                dtItemList.Clear();
                DataRow[] ItemRows = dtItem.Select("SupplierCode='" + supplier + "'");
                dtItemList = ItemRows.CopyToDataTable();
                if (dtItemList.Rows.Count > 0)
                {
                    int i = 1;
                    foreach (DataRow row in dtItemList.Rows)
                    {
                        DataRow rowNew = tblIMRBI.NewRow();
                        rowNew["No"] = i;
                        rowNew["SupplierCode"] = row["SupplierCode"];
                        rowNew["ItemCode"] = row["ItemCode"];
                        rowNew["Description"] = row["Description"];
                        rowNew["InStock"] = row["OnHand"];
                        rowNew["EventOrder"] = row["OnOrder"];
                        rowNew["Last7DaysAvg"] = row["Last7DaysAvg"];
                        rowNew["AlreadyOrdered"] = row["AlreadyOrdered"];
                        rowNew["MinStock"] = row["MinLevel"];
                        rowNew["UOM"] = row["UOM"];
                        rowNew["Price"] = row["Price"];
                        rowNew["MinSpend"] = row["MinSpend"];
                        rowNew["RecommendedQuantity"] = row["RecommendedQuantity"];
                        rowNew["OrderQuantity"] = row["OrderQuantity"];
                        rowNew["Total"] = double.Parse(row["Total"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);
                        rowNew["Vendor"] = row["SupplierCode"];
                        if (txtDeliveryDate.Text != "" && txtDeliveryDate.Text != string.Empty && txtDeliveryDate != null)
                        {
                            rowNew["DeliveryDate"] = Convert.ToDateTime(txtDeliveryDate.Text).Date;
                        }
                        else
                        { rowNew["DeliveryDate"] = new DateTime(); }
                        rowNew["Remarks"] = row["Remarks"];
                        rowNew["ApproverRemarks"] = string.Empty;
                        rowNew["ItemPerUnit"] = row["ItemPerUnit"];

                        tblIMRBI.Rows.Add(rowNew);
                        i++;
                    }
                    DataView dv = tblIMRBI.DefaultView;
                    dv.Sort = "Description  ASC";
                    dt = dv.ToTable();
                }
                return dt;

            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
                throw ex;
            }
        }

        private void BindData(DataTable tblData, string type)
        {
            DataTable supdt = (DataTable)tblData.DefaultView.ToTable(true, "SupplierCode", "SupplierName", "MinSpend");
            supdt.Columns.Add("MinSpendValue");
            if (tblData != null && tblData.Rows.Count > 0)
            {
                for (int i = 0; i <= supdt.Rows.Count - 1; i++)
                {
                    supdt.Rows[i]["MinSpendValue"] = string.Format("${0}", Convert.ToDouble(supdt.Rows[i]["MinSpend"]).ToString(Utils.AppConstants.NUMBER_FORMAT));
                }
            }
            grvParentGrid.DataSource = supdt;
            grvParentGrid.DataBind();
        }

        private DataTable GetItemData_Search(string supplier)
        {
            try
            {
                DataTable dt = new DataTable();
                DataTable tblIMRBI = CreateTabelFormat();
                DataTable dtItem = (DataTable)Session["MRBISearch"];
                DataTable dtItemList = null;
                dtItemList = dtItem.Clone();
                dtItemList.Clear();
                DataRow[] ItemRows = dtItem.Select("SupplierCode='" + supplier + "'");

                dtItemList = ItemRows.CopyToDataTable();

                if (dtItemList.Rows.Count > 0)
                {
                    int i = 1;
                    foreach (DataRow row in dtItemList.Rows)
                    {
                        DataRow rowNew = tblIMRBI.NewRow();
                        rowNew["No"] = i;
                        rowNew["SupplierCode"] = row["SupplierCode"];
                        rowNew["ItemCode"] = row["ItemCode"];
                        rowNew["Description"] = row["Description"];
                        rowNew["InStock"] = row["OnHand"];
                        rowNew["EventOrder"] = row["OnOrder"];
                        rowNew["Last7DaysAvg"] = row["Last7DaysAvg"];
                        rowNew["AlreadyOrdered"] = row["AlreadyOrdered"];
                        rowNew["MinStock"] = row["MinLevel"];
                        rowNew["UOM"] = row["UOM"];
                        rowNew["Price"] = row["Price"];
                        rowNew["MinSpend"] = row["MinSpend"];
                        rowNew["RecommendedQuantity"] = row["RecommendedQuantity"];
                        rowNew["OrderQuantity"] = row["OrderQuantity"];
                        rowNew["Total"] = double.Parse(row["Total"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);
                        rowNew["Vendor"] = row["SupplierCode"];
                        if (txtDeliveryDate.Text != "" && txtDeliveryDate.Text != string.Empty && txtDeliveryDate != null)
                        {
                            rowNew["DeliveryDate"] = Convert.ToDateTime(txtDeliveryDate.Text).Date;
                        }
                        else
                        { rowNew["DeliveryDate"] = new DateTime(); }
                        rowNew["Remarks"] = row["Remarks"];
                        rowNew["ApproverRemarks"] = string.Empty;
                        tblIMRBI.Rows.Add(rowNew);
                        i++;
                    }

                    DataView dv = tblIMRBI.DefaultView;
                    dv.Sort = "Description  ASC";
                    dt = dv.ToTable();

                }
                return dt;
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
                throw ex;
            }
        }

        private void GetPrice(DataRow r)
        {
            try
            {
                //r["Price"].ToString()
                r["Total"] = (double.Parse(r["Price"].ToString()) * double.Parse(r["OrderQuantity"].ToString())).ToString(Utils.AppConstants.NUMBER_FORMAT);
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }

        private void CalcTotal(DataTable tb)
        {
            try
            {
                if (tb != null)
                {
                    double Total = 0;
                    foreach (DataRow row in tb.Rows)
                    {
                        if (row["Total"] != null && row["Total"].ToString().Length > 0)
                        {
                            Total += double.Parse(row["Total"].ToString());
                        }
                    }
                    //Session["Total"] = Total;

                    //this.lblGrandTotal.Text = string.Format("${0}", Total.ToString(Utils.AppConstants.NUMBER_FORMAT));

                    //this.lblTotalOutlet.Text = this.lblGrandTotal.Text;
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }

        protected void DisplayCalendar(CheckBoxList chkCalendar, string SuppCode)
        {

            DataTable dt = (DataTable)Session["MRBI"];
            DataTable dtList;

            DataTable supdt = (DataTable)dt.DefaultView.ToTable(true, "SupplierCode", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun");

            DataRow[] ItemRows = supdt.Select("SupplierCode='" + SuppCode + "'");
            dtList = ItemRows.CopyToDataTable();

            var CurrentRow = dtList.Rows[0];
            DataTable tbNew = new DataTable();
            tbNew.Columns.Add(AppConstants.Name);
            tbNew.Columns.Add(AppConstants.Value);
            if (CurrentRow.Table.Columns.Count > 0)
            {
                int colIndex = 1;
                for (int i = colIndex; i < CurrentRow.Table.Columns.Count; i++)
                {
                    DataRow rNew = tbNew.NewRow();
                    rNew[AppConstants.Name] = CurrentRow.Table.Columns[i].ColumnName;
                    rNew[AppConstants.Value] = CurrentRow.ItemArray[i];
                    tbNew.Rows.Add(rNew);
                }
            }

            chkCalendar.DataSource = tbNew;
            chkCalendar.DataTextField = AppConstants.Name;
            chkCalendar.DataValueField = AppConstants.Value;
            chkCalendar.DataBind();
            if (chkCalendar.Items.Count > 0)
            {
                string day = string.Empty;

                for (int i = 0; i < chkCalendar.Items.Count; i++)
                {
                    if (chkCalendar.Items[i].Value.ToString() == AppConstants.Yes)
                    {
                        chkCalendar.Items[i].Selected = true;

                    }
                    else
                    {
                        chkCalendar.Items[i].Selected = false;
                    }
                }
            }

        }

        private void LoadSuppItemListData(string sOutlet)
        {
            try
            {
                var objC = new MasterService.MasterSoapClient();
                DataSet ds = objC.Get_MaterialReqBySupplierItemList(sOutlet, Session[AppConstants.U_ConnString].ToString());
                Session["MRBI"] = ds.Tables[0];
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }

        private void LoadData(string sOutlet)
        {
            try
            {
                Session["MRBI"] = null;
                var objC = new MasterService.MasterSoapClient();
                DataSet ds = objC.Get_MaterialReqByItem(Session[AppConstants.UserRole].ToString(), sOutlet, Session[AppConstants.U_ConnString].ToString());
                DataTable dt = ds.Tables[0];
                // if outlet is central kitchen, remove the central kitchen supplier
                if (ddlWareHouse.SelectedValue == "01CKT")
                {
                    string code = "VA-HARCT";
                    var rows = from row in dt.AsEnumerable()
                               where !row.Field<string>(AppConstants.SupplierCode).Contains(code.ToUpper())
                               select row;
                    if (rows.Count() > 0)
                    {
                        dt = rows.CopyToDataTable();
                    }
                }
                Session["MRBI"] = dt;
                DataTable supdt = (DataTable)dt.DefaultView.ToTable(true, "SupplierCode", "SupplierName", "MinSpend");
                supdt.Columns.Add("MinSpendValue");
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i <= supdt.Rows.Count - 1; i++)
                    {
                        supdt.Rows[i]["MinSpendValue"] = string.Format("${0}", Convert.ToDouble(supdt.Rows[i]["MinSpend"]).ToString(Utils.AppConstants.NUMBER_FORMAT));
                    }
                }
                grvParentGrid.DataSource = supdt;
                grvParentGrid.DataBind();
            }
            catch (Exception ex)
            {
                this.lblError.Text = "Error Occured While loading the Data in Grid..." + ex.Message;
                this.lblError.Visible = true;
            }
        }

        private DataTable CreateTabelFormat()
        {
            DataTable tbTemp = new DataTable();
            tbTemp.Columns.Add("No");
            tbTemp.Columns.Add("SupplierCode");
            tbTemp.Columns.Add("ItemCode");
            tbTemp.Columns.Add("Description");
            tbTemp.Columns.Add("InStock");
            tbTemp.Columns.Add("EventOrder");
            tbTemp.Columns.Add("Last7DaysAvg");
            tbTemp.Columns.Add("AlreadyOrdered");
            tbTemp.Columns.Add("MinStock");
            tbTemp.Columns.Add("RecommendedQuantity");
            tbTemp.Columns.Add("OrderQuantity", typeof(double));
            tbTemp.Columns.Add("UOM");
            tbTemp.Columns.Add("Price");
            tbTemp.Columns.Add("Total", typeof(double));
            tbTemp.Columns.Add("MinSpend");
            tbTemp.Columns.Add("Vendor");
            tbTemp.Columns.Add("DeliveryDate");
            tbTemp.Columns.Add("Remarks");
            tbTemp.Columns.Add("ApproverRemarks");
            tbTemp.Columns.Add("ItemPerUnit");
            return tbTemp;
        }

        private void LoadHeaderTableData()
        {
            DataTable tblIMRBIHeader = CreateHeaderTable();
            DataRow rowNew = tblIMRBIHeader.NewRow();
            rowNew["Id"] = 1;
            rowNew["PostingDate"] = Convert.ToDateTime(Session[AppConstants.OrderDate]).Date;
            rowNew["Outlet"] = ddlWareHouse.SelectedValue.ToString();
            rowNew["UserCode"] = Session[Utils.AppConstants.UserName].ToString().ToUpper();
            if (txtDeliveryDate.Text != string.Empty)
            {
                rowNew["DeliveryDate"] = Convert.ToDateTime(txtDeliveryDate.Text).Date;
                lblError.Visible = true;
                lblError.Text = string.Empty;
            }
            else
            {
                lblError.Visible = true;
                lblError.Text = "Delivery Date should not be Empty";
                return;
            }
            rowNew["OrderTime"] = Convert.ToDateTime(Session[AppConstants.OrderDate]).TimeOfDay.Hours + ":" + Convert.ToDateTime(Session[AppConstants.OrderDate]).TimeOfDay.Minutes + " " + Convert.ToDateTime(Session[AppConstants.OrderDate]).ToString("tt");
            rowNew["UserId"] = Convert.ToInt16(Session[AppConstants.UserID]);
            if (chkPriority.Checked == true) { rowNew["Urgent"] = AppConstants.Yes; } else { rowNew["Urgent"] = AppConstants.No; }
            tblIMRBIHeader.Rows.Add(rowNew);
            Session["MRBIHeaderTable"] = tblIMRBIHeader;
        }

        private DataTable CreateHeaderTable()
        {
            DataTable tbHeader = new DataTable();
            tbHeader.Columns.Add("Id");
            tbHeader.Columns.Add("Outlet");
            tbHeader.Columns.Add("UserCode");
            tbHeader.Columns.Add("PostingDate");
            tbHeader.Columns.Add("DeliveryDate");
            tbHeader.Columns.Add("Urgent");
            tbHeader.Columns.Add("OrderTime");
            tbHeader.Columns.Add("UserId");
            return tbHeader;
        }

        public void mouseOverandMouseOut()
        {
            btnSaveDraft.Attributes.Add("class", "static");
            btnSaveDraft.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnSaveDraft.Attributes.Add("onMouseOut", "this.className='static'");

            btnSearch.Attributes.Add("class", "static");
            btnSearch.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnSearch.Attributes.Add("onMouseOut", "this.className='static'");
        }

        private void LoadOutletDropDown()
        {
            try
            {
                var data = new MasterService.MasterSoapClient();
                DataSet dsOutlet = data.Get_Outlet_Details(Session[Utils.AppConstants.SUPERUSER].ToString(), Session[Utils.AppConstants.WhsCode].ToString(),
                Convert.ToInt16(Session[AppConstants.ApprovalLevel]), Session[AppConstants.U_ConnString].ToString());
                if (dsOutlet.Tables[0].Rows.Count != 0 && dsOutlet != null)
                {
                    ddlWareHouse.DataSource = dsOutlet.Tables[0];
                    ddlWareHouse.DataTextField = dsOutlet.Tables[0].Columns[AppConstants.WhsName].ColumnName.ToString();
                    ddlWareHouse.DataValueField = dsOutlet.Tables[0].Columns[AppConstants.WhsCode].ColumnName.ToString();
                    ddlWareHouse.DataBind();
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = "Failed to Load Outlet List " + ex.Message;
                this.lblError.Visible = true;
            }
        }

        public void ClearFields()
        {
            ddlWareHouse.SelectedIndex = 0;
            lblOrderDate.Text = DateTime.Now.ToString();
            Session[AppConstants.OrderDate] = lblOrderDate.Text;
            txtDeliveryDate.Text = string.Empty;
            txtSearch.Text = string.Empty;
            grvParentGrid.DataBind();
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            LoadData(this.ddlWareHouse.SelectedValue.ToString());
            Timer1.Enabled = false;
        }

        public DataSet FetchNotNullValues(DataSet ds)
        {
            //ds.Tables[1].DefaultView.RowFilter = "OrderQuantity > 0";
            //return ds;
            DataTable dt = ds.Tables[1];
            dt.DefaultView.RowFilter = "OrderQuantity > 0";
            dt = dt.DefaultView.ToTable();
            ds.Tables.Remove(ds.Tables[1]);
            ds.Tables.Add(dt.Copy());
            return ds;
        }
    }
}