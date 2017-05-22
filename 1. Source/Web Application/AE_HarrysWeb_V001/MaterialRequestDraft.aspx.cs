using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AE_HarrysWeb_V001.Utils;
using System.Data;
using System.Drawing;

namespace AE_HarrysWeb_V001
{
    public partial class MaterialRequestDraft : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session[Utils.AppConstants.UserCode].ToString() != string.Empty)
                {
                    if (!IsPostBack || Convert.ToString(Session["ErrorMsg"]) != string.Empty)
                    {
                        Session["checkSubmit"] = "0";
                        Session["DeliveryChargeSupplier"] = string.Empty;
                        loadDropdowndata();
                        btnEDOnLoadandOnSaveClick();
                        var data = new MasterService.MasterSoapClient();
                        DataSet ds = data.Get_DraftDetails(Session[AppConstants.DraftNo].ToString(), Session[AppConstants.U_ConnString].ToString(), Session[AppConstants.DocType].ToString());
                        if (ds != null && ds.Tables.Count > 0)
                        {
                            Session["DraftDetails"] = ds;
                            loadDraftSupplier(ds);
                            LoadHeaderData(ds);
                            LoadLineData(ds);
                            //if (lblStatus.Text == AppConstants.Draft)
                            //{ btnAddItems.Enabled = false; btnSaveDraft.Enabled = false; btnSubmit.Enabled = true; }
                            //else if (lblStatus.Text != AppConstants.Approved)
                            //{ btnAddItems.Enabled = true; btnSaveDraft.Enabled = true; btnSubmit.Enabled = true; }
                            DataTable dtTable = (DataTable)Session["MRDraftTable"];
                            var rows = from row in dtTable.AsEnumerable()
                                       where row.Field<string>("SupplierCode") == ddlMainSupplier.SelectedValue
                                       select row;
                            grvMRDraft.DataSource = rows.CopyToDataTable();
                            grvMRDraft.DataBind();
                        }
                        grvMRDraft.DataBind();
                        mouseOverandMouseOut();
                        lblError.Visible = true;
                        lblError.Text = Convert.ToString(Session["ErrorMsg"]);
                        Session["ErrorMsg"] = string.Empty;
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
                //Response.Redirect(AppConstants.LoginURL);
            }
        }

        protected void ddlSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.ddlSupplier.Text.Trim().Length > 0)
                {
                    LoadPopupData(this.ddlSupplier.SelectedValue.ToString(), lblWareHouseCode.Text, Session[Utils.AppConstants.UserRole].ToString());
                }

                mpePopup.Show();
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;

            }
        }

        protected void ddlMainSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.ddlMainSupplier.Text.Trim().Length > 0)
                {
                    DataTable dtTable = (DataTable)Session["MRDraftTable"];

                    IEnumerable<DataRow> datatableRows = dtTable.AsEnumerable()
                        .Where(row => row.Field<string>("ItemCode") != AppConstants.delChargeItemCode);

                    if (datatableRows.Count() != 0)
                    {
                        dtTable = datatableRows.CopyToDataTable();
                    }

                    if (dtTable != null && dtTable.Rows.Count > 0)
                    {
                        IEnumerable<DataRow> rows = dtTable.AsEnumerable()
                        .Where(row => row.Field<string>("SupplierCode") == ddlMainSupplier.SelectedValue
                            && row.Field<string>("ItemCode") != AppConstants.delChargeItemCode);

                        DataTable dt = new DataTable();
                        if (rows.Count() != 0)
                        {
                            dt = rows.CopyToDataTable();
                            if (dt.Rows[0]["DelChargeUDF"].ToString() == "Y")
                            {
                                chkDeliveryCharge.Visible = true;
                                chkDeliveryCharge.Checked = true;
                            }
                            else
                            {
                                chkDeliveryCharge.Visible = true;
                                chkDeliveryCharge.Checked = false;
                            }

                            DateTime date = Convert.ToDateTime(dt.Rows[0]["DeliveryDate"]);
                            lblDeliveryDate.Text = String.Format("{0:dd/MM/yyyy}", date);
                        }
                        else
                        {
                            chkDeliveryCharge.Visible = false;
                        }
                        if (ddlMainSupplier.Text != "ALL")
                        {
                            lblDeliveryCalender.Visible = true;
                            lblSeperator.Visible = true;
                            chkCalendar.Visible = true;
                            lblDate.Visible = true;
                            lblSeperator1.Visible = true;
                            lblDeliveryDate.Visible = true;

                            grvMRDraft.DataSource = dt;
                            grvMRDraft.DataBind();
                            CalcTotal(dt, dtTable);
                            //DataSet ds = new DataSet();
                            //ds.Tables.Add((DataTable));
                            DisplayCalendar((DataSet)Session["DraftDetails"]);
                            CalcDeliveryDate();
                            //ds.Tables.Remove(rows.CopyToDataTable());
                            //ds.Clear();
                        }
                        else
                        {
                            lblDeliveryCalender.Visible = false;
                            lblSeperator.Visible = false;
                            chkCalendar.Visible = false;
                            lblDate.Visible = false;
                            lblSeperator1.Visible = false;
                            lblDeliveryDate.Visible = false;

                            chkDeliveryCharge.Visible = false;
                            grvMRDraft.DataSource = dtTable;
                            grvMRDraft.DataBind();
                            CalcTotal(dtTable, dtTable);
                            CalcDeliveryDate();
                            DisplayCalendar(new DataSet());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }

        private void LoadPopupData(string supplier, string outlet, string userRole)
        {
            try
            {
                DataTable tblAddItem = CreatePopHeaderFormat();
                var objC = new MasterService.MasterSoapClient();
                DataSet ds = objC.Get_MaterialReqBySupplier(supplier, outlet, userRole, Session[AppConstants.U_ConnString].ToString());
                Session["PopupDraftDetails"] = ds;
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
                        Session["PopupDraftDetails"] = ds;
                    }

                    int i = 1;
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        DataRow rowNew = tblAddItem.NewRow();
                        rowNew["No"] = i;
                        rowNew["GroupType"] = row["GroupType"];
                        rowNew["SupplierCode"] = row["SupplierCode"];
                        rowNew["SupplierName"] = row["SupplierName"];
                        rowNew["ItemCode"] = row["ItemCode"];
                        rowNew["Description"] = row["ItemName"];
                        rowNew["InStock"] = row["OnHand"];
                        rowNew["EventOrder"] = row["OnOrder"];
                        rowNew["Last7DaysAvg"] = row["Last 7 Days"];
                        rowNew["AlreadyOrdered"] = row["AlreadyOrdered"];
                        rowNew["RecommendedQuantity"] = row["RecommendedQuantity"];
                        rowNew["MinStock"] = row["MinLevel"];
                        rowNew["UOM"] = row["UOM"];
                        rowNew["Price"] = row["Price"];
                        rowNew["MinSpend"] = row["MinSpend"];
                        rowNew["ItemPerUnit"] = row["ItemPerUnit"];
                        //rowNew["Vendor"] = ddlSupplier.SelectedValue.ToString();
                        //rowNew["SupplierCode"] = ddlSupplier.SelectedValue.ToString();
                        tblAddItem.Rows.Add(rowNew);
                        i++;
                    }
                    Session["ItemPopupTable"] = tblAddItem;
                    DataView dv = tblAddItem.DefaultView;
                    this.grdVendor.DataSource = dv.ToTable();
                    this.grdVendor.DataBind();

                }
                else
                {
                    Session["ItemPopupTable"] = tblAddItem;
                    DataView dv = tblAddItem.DefaultView;
                    this.grdVendor.DataSource = dv.ToTable();
                    this.grdVendor.DataBind();
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = "Failed to load the POPUP Data.." + ex.Message;
                this.lblError.Visible = true;
            }
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
            Label lblNo = ((Label)row.FindControl("lblNo"));
            DataTable tb = (DataTable)Session["MRDraftTable"];
            if (tb != null)
            {
                DataRow[] rupdate1 = tb.Select("ItemCode='" + lblItemCode.Text + "'");
                if (rupdate1.Length > 1)
                {
                    DataRow[] rupdate;
                    if (txtQuantity.Text == string.Empty)
                    {
                        rupdate = tb.Select("ItemCode='" + lblItemCode.Text + "' AND OrderQuantity = '" + string.Empty + "'");
                    }
                    else
                    {
                        rupdate = tb.Select("ItemCode='" + lblItemCode.Text + "' AND No = '" + lblNo.Text + "'");
                    }
                    if (rupdate.Length > 0)
                    {
                        rupdate[0]["OrderQuantity"] = txtQuantity.Text == string.Empty ? "0" : txtQuantity.Text;
                        GetPrice(rupdate[0]);
                    }
                }
                else if (rupdate1.Length > 0)
                {
                    rupdate1[0]["OrderQuantity"] = txtQuantity.Text == string.Empty ? "0" : txtQuantity.Text;
                    GetPrice(rupdate1[0]);
                }
                this.grvMRDraft.EditIndex = -1;
                BindData(tb);
                //DataSet ds = new DataSet();
                //ds.Tables.Add(tb);
                CalcTotal(tb, tb);
                //ds.Tables.Remove(tb);
                if (Session["checkSubmit"].ToString() == "0")
                {
                    // this is to disable the submit button
                    btnEDOnLoadandOnSaveClick();
                    btnEDOnGridChange();
                }
            }
        }

        protected void btnAddItems_Click(object sender, EventArgs e)
        {
            rdbSuppliers_CheckedChanged(this, new System.EventArgs());
        }

        protected void btnSubmitItems_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)Session["MRDraftTable"];
            //DataTable selectedData = new DataTable();
            DataTable tblSelectedItem = CreateTableFormat();
            foreach (GridViewRow val in grdVendor.Rows)
            {
                CheckBox chkItems = (CheckBox)val.FindControl("chkItems");
                if (chkItems.Checked)
                {
                    DateTime date = new DateTime();
                    string DelChargeUDF = string.Empty;
                    DataRow rowNew = dt.NewRow();
                    Label lblSupplierName = (Label)val.FindControl("lblSupplierName");
                    Label lblSupplierCode = (Label)val.FindControl("lblSupplierCode");
                    Label lblGroupType = (Label)val.FindControl("lblGroupType");
                    Session["SelectedSupplier"] = lblSupplierCode.Text;
                    Label lblItemCode = (Label)val.FindControl("lblItemCode");
                    Label lblItemDesc = (Label)val.FindControl("lblItemDesc");
                    Label lblInStock = (Label)val.FindControl("lblInStock");
                    Label lblEventOrder = (Label)val.FindControl("lblEventOrder");
                    Label lblL7DaysAvg = (Label)val.FindControl("lblL7DaysAvg");
                    Label lblAlreadyOrdered = (Label)val.FindControl("lblAlreadyOrdered");
                    Label lblMinSpend = (Label)val.FindControl("lblMinSpendChild");
                    Label lblMinStock = (Label)val.FindControl("lblMinStock");
                    Label lblRecommendedQuantity = (Label)val.FindControl("lblRecommendedQuantity");
                    TextBox txtOrderQuantity = (TextBox)val.FindControl("txtOrderQuantity");
                    Label lblUoM = (Label)val.FindControl("lblUoM");
                    Label lblPrice = (Label)val.FindControl("lblPrice");
                    Label lblTotal = (Label)val.FindControl("lblTotal");
                    Label lblItemPerUnit = (Label)val.FindControl("lblItemPerUnit");
                    
                    rowNew["No"] = dt.Rows.Count + 1;
                    rowNew["GroupType"] = lblGroupType.Text;
                    rowNew["SupplierCode"] = lblSupplierCode.Text;
                    rowNew["SupplierName"] = lblSupplierName.Text;
                    rowNew["ItemCode"] = lblItemCode.Text;
                    rowNew["Description"] = lblItemDesc.Text;
                    rowNew["InStock"] = lblInStock.Text;
                    rowNew["EventOrder"] = lblEventOrder.Text;
                    rowNew["Last7DaysAvg"] = lblL7DaysAvg.Text;
                    rowNew["AlreadyOrdered"] = lblAlreadyOrdered.Text;
                    rowNew["RecommendedQuantity"] = lblRecommendedQuantity.Text;
                    rowNew["OrderQuantity"] = txtOrderQuantity.Text;
                    rowNew["MinStock"] = lblMinStock.Text;
                    rowNew["UOM"] = lblUoM.Text;
                    rowNew["Price"] = lblPrice.Text;
                    rowNew["MinSpend"] = lblMinSpend.Text;
                    rowNew["ItemPerUnit"] = lblItemPerUnit.Text;
                    rowNew["Total"] = lblTotal.Text.Trim() == string.Empty ? "0.0000" : lblTotal.Text;
                    IEnumerable<DataRow> rows = dt.AsEnumerable().Where(r => r.Field<string>("SupplierCode") == lblSupplierCode.Text);
                    if (rows.Count() > 0)
                    {
                        DataTable table = rows.CopyToDataTable();
                        DelChargeUDF = Convert.ToString(table.Rows[0]["DelChargeUDF"]);
                        date = Convert.ToDateTime(table.Rows[0]["DeliveryDate"]);
                    }
                    else
                    {
                        date = DateTime.Now.Date;
                        DelChargeUDF = "N";
                    }
                    rowNew["DelChargeUDF"] = DelChargeUDF;
                    rowNew["DeliveryDate"] = date;
                    dt.Rows.Add(rowNew.ItemArray);
                    tblSelectedItem.Rows.Add(rowNew.ItemArray);
                    //dt.Rows.Add(rowNew);
                    //tblSelectedItem.Rows.Add(rowNew);
                }
            }

            grvMRDraft.DataSource = dt;
            grvMRDraft.DataBind();
            DataSet ds = new DataSet();
            DataSet ds1 = new DataSet();
            DataTable dtclone = dt.Copy();
            ds.Tables.Add(dtclone);
            ds1.Tables.Add(tblSelectedItem);
            //DataTable ItemPopupData = new DataTable();
            //ItemPopupData = (DataTable)Session["ItemPopupTable"];
            //DataTable t1 = (DataTable)Session["DraftDetails"];
            //ds.Tables.Add(t1);
            Session["MRDraftTable"] = ds.Tables[0];
            loadDraftSupplier(ds);
            if (rdbSuppliers.Checked == true)
            {
                ddlMainSupplier.SelectedValue = ddlSupplier.SelectedValue;
            }
            else
            {
                ddlMainSupplier.SelectedValue = Session["SelectedSupplier"].ToString();
            }
            CalcDeliveryDate();
            //DataSet datasetold = (DataSet)Session["DraftDetails"];
            //DataSet datasetnew = (DataSet)Session["PopupDraftDetails"];
            //datasetold.Merge(datasetnew);
            DisplayCalendar((DataSet)Session["DraftDetails"]);
            ds.Tables.Remove(dtclone);
            ds1.Tables.Remove(tblSelectedItem);
            //ds.Tables.Remove(t1);
            //Session["MRDraftTable"] = dt;

            ddlMainSupplier_SelectedIndexChanged(this, new System.EventArgs());
            // this is to disable the submit button
            btnEDOnLoadandOnSaveClick();
            btnEDOnGridChange();
            lblError.Text = string.Empty;
            lblDeliveryCalender.Visible = false;
            lblSeperator.Visible = false;
            chkCalendar.Visible = false;
            lblDate.Visible = false;
            lblSeperator1.Visible = false;
            lblDeliveryDate.Visible = false;
            mpePopup.Hide();
        }

        protected void txtSearch_OnTextChanged(object sender, EventArgs e)
        {
            btnSearch_Click(this, new System.EventArgs());
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            var searchKey = txtSearch.Text;
            if (searchKey != null && searchKey != string.Empty)
            {
                searchKey = searchKey.ToUpper();
            }
            DataTable dt = (DataTable)Session["ItemPopupTable"];
            if (dt != null && dt.Rows.Count > 0)
            {
                if (searchKey != string.Empty)
                {
                    dt.DefaultView.RowFilter = "Description LIKE '%" + searchKey + "%'";
                    dt = dt.DefaultView.ToTable();
                }
                else
                {
                    dt.DefaultView.RowFilter = "";
                    dt = dt.DefaultView.ToTable();
                }

                if (dt != null)
                {
                    grdVendor.DataSource = dt;
                    grdVendor.DataBind();
                }
                else
                {
                    grdVendor.DataSource = new DataTable();
                    grdVendor.DataBind();
                }
                txtSearch.Focus();
                mpePopup.Show();
            }
        }

        protected void rdbSuppliers_CheckedChanged(object sender, EventArgs e)
        {
            rdbItems.Checked = false;
            rdbSuppliers.Checked = true;
            ddlSupplier.Enabled = true;
            ddlSupplier.SelectedIndex = 0;
            txtSearch.Text = string.Empty;
            grdVendor.DataBind();
            DataTable dtSupplier = (DataTable)Session["PopUpSupplierList"];
            RemoveSupplier(dtSupplier);
            mpePopup.Show();
        }

        protected void rdbItems_CheckedChanged(object sender, EventArgs e)
        {
            rdbItems.Checked = true;
            rdbSuppliers.Checked = false;
            ddlSupplier.Enabled = false;
            ddlSupplier.SelectedIndex = 0;
            txtSearch.Text = string.Empty;
            LoadPopupData(string.Empty, lblWareHouseCode.Text, Session[Utils.AppConstants.UserRole].ToString());
            mpePopup.Show();
        }

        protected void txtMainSearch_OnTextChanged(object sender, EventArgs e)
        {
            btnMainSearch_Click(this, new System.EventArgs());
        }

        protected void btnMainSearch_Click(object sender, EventArgs e)
        {
            var searchKey = txtMainSearch.Text;

            DataTable dt = (DataTable)Session["MRDraftTable"];

            if (searchKey != string.Empty)
            {
                dt.DefaultView.RowFilter = "Description LIKE '%" + searchKey + "%'";
                dt = dt.DefaultView.ToTable();
            }
            else
            {
                dt.DefaultView.RowFilter = "";
                dt = dt.DefaultView.ToTable();
            }

            if (dt != null)
            {
                grvMRDraft.DataSource = dt;
                grvMRDraft.DataBind();
            }
            else
            {
                grvMRDraft.DataSource = new DataTable();
                grvMRDraft.DataBind();
            }
            txtSearch.Focus();
        }

        protected void grvMRDraft_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridViewRow myRow = e.Row;
                Label lblNo = myRow.FindControl("lblNo") as Label;
                //lblNo.Text = (myRow.RowIndex + 1).ToString();
                //if (this.hdnStatus.Value == "C")
                //{
                //    TextBox txtQuantity = myRow.FindControl("txtOrderQuantity") as TextBox;
                //    txtQuantity.Enabled = false;
                //}
            }
        }

        protected void grvMRDraft_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.grvMRDraft.PageIndex = e.NewPageIndex;
            DataTable tblMRDraft = (DataTable)Session["MRDraftTable"];
            BindData(tblMRDraft);
        }

        protected void grvMRDraft_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("onmouseover", "MouseEvents(this, event)");
                e.Row.Attributes.Add("onmouseout", "MouseEvents(this, event)");
                e.Row.Attributes["style"] = "cursor:pointer";
                int index = 0;
                TextBox ddl1 = e.Row.FindControl("txtOrderQuantity") as TextBox;
                if (Convert.ToString(DataBinder.Eval(e.Row.DataItem, "GroupType")) == Session[AppConstants.UserRole].ToString().ToUpper())
                {
                    ddl1.Enabled = true;
                }
                else
                {
                    ddl1.Enabled = false;
                }
                ddl1.Attributes.Add("onfocus", "JavaScript:document.getElementById('" + ddl1.ClientID + "').select();");
                if (ViewState["rowindex"] != null && Session["MRDraftTable"] != null)
                {
                    index = Convert.ToInt32(ViewState["rowindex"].ToString());
                    int rowCount = ((DataTable)Session["MRDraftTable"]).Rows.Count;
                    if ((index + 1) <= this.grvMRDraft.Rows.Count)
                    {
                        if (e.Row.RowIndex == index + 1)
                        {
                            ddl1.Focus();
                        }
                    }
                }
                //if (lblStatus.Text == AppConstants.Approved)
                //{ ddl1.Enabled = false; }
                //else if (lblStatus.Text != AppConstants.Approved)
                //{ ddl1.Enabled = true; }
            }
        }

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            //LoadHeaderData();
            DataTable HeaderTable = (DataTable)Session["MRDraftHeaderTable"];
            DataTable LineTable = (DataTable)Session["MRDraftTable"];
            HeaderTable.TableName = "tblHeader";
            LineTable.TableName = "tblLine";
            DataSet ds = new DataSet();
            DataTable dtHeader = HeaderTable.Copy();
            DataTable dtLine = LineTable.Copy();
            ds.Tables.Add(dtHeader);
            ds.Tables.Add(dtLine);

            foreach (DataRow item in dtLine.Rows)
            {
                item["OrderQuantity"] = Convert.ToString(item["OrderQuantity"]) == string.Empty ? "0" : item["OrderQuantity"];
            }

            var data = new MasterService.MasterSoapClient();

            var returnResult = string.Empty;
            if (ds.Tables[1].Rows[0]["SupplierCode"].ToString().Trim().ToUpper() == AppConstants.HoldingSupplier.ToString().Trim().ToUpper())
            {
                //var returnResult = data.Create_PurchaseRequest(ds, true, Session[AppConstants.U_DBName].ToString(), false, false, false);
                //var returnResult = data.Update_PurchaseRequest(ds, true, Session[AppConstants.U_DBName].ToString(), false, false, false);
                returnResult = data.Create_PurchaseRequest(ds, true, Session[AppConstants.U_DBName].ToString(), false, false, false);
            }
            else
            {
                returnResult = data.Update_PurchaseRequest(ds, true, Session[AppConstants.U_DBName].ToString(), false, false, false);
            }


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
                if (dtLine.Rows.Count == 1 && Convert.ToDouble(dtLine.Rows[0]["OrderQuantity"]) == 0)
                {
                    lblError.Visible = true;
                    lblError.Text = "Material Request Draft Removed Successfully.";
                    btnEDOnLoadandOnSaveClick();
                }
                else
                {
                    lblError.Visible = true;
                    lblError.Text = "Material Request Draft Updated Successfully.";
                    btnEDOnLoadandOnSaveClick();
                }
            }
            ds.Tables.Remove(dtHeader);
            ds.Tables.Remove(dtLine);
            ds.Clear();
            Session["ErrorMsg"] = lblError.Text;
            Page_Load(this, new System.EventArgs());
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string supplier = string.Empty;
            int countForAddDeliveryCharge = 0;
            int AddDelChargeForOtherUser = 0;
            DataTable HeaderTable = (DataTable)Session["MRDraftHeaderTable"];
            DataTable LineTable = (DataTable)Session["MRDraftTable"];
            HeaderTable.TableName = "tblHeader";
            LineTable.TableName = "tblLine";
            DataSet ds = new DataSet();
            DataTable dtHeader = HeaderTable.Copy();
            ds.Tables.Add(dtHeader);

            //checking the validation for Total Amount greater than MinSpend 
            var result = from r in LineTable.AsEnumerable()
                         group r by new
                         {
                             Group = r["SupplierCode"],
                             MinSpend = r["MinSpend"] == DBNull.Value ? 0 : Convert.ToDouble(r["MinSpend"]),
                             SupName = r["SupplierName"],
                             GroupType = r["GroupType"],
                             DelChargeUDF = r["DelChargeUDF"],
                             //UpdateDeliveryCharge = r["UpdateDeliveryCharge"],
                             //DeliveryDate = Convert.ToDateTime(r["DeliveryDate"]).Date,

                         } into g
                         select new
                         {
                             Group = g.Key.Group,
                             SupName = g.Key.SupName,
                             MinSpend = g.Key.MinSpend,
                             DelChargeUDF = g.Key.DelChargeUDF,
                             //DeliveryDate = g.Key.DeliveryDate,
                             //UpdateDeliveryCharge = g.Key.UpdateDeliveryCharge,
                             GroupType = g.Key.GroupType,
                             Sum = g.Sum(x => x["Total"] == null ? 0 : Convert.ToDouble(x["Total"]))
                         };
            foreach (var item in result)
            {
                if (item.Sum < item.MinSpend && item.Sum > 0 && item.DelChargeUDF.ToString() == AppConstants.No && item.GroupType.ToString() == Convert.ToString(Session[AppConstants.UserRole]).ToUpper())
                {
                    countForAddDeliveryCharge = countForAddDeliveryCharge + 1;

                    if (countForAddDeliveryCharge == 1)
                    {
                        supplier = item.SupName.ToString();
                    }
                    else
                    {
                        supplier = supplier + "','" + item.SupName.ToString();
                    }
                }
                if (item.Sum < item.MinSpend && item.DelChargeUDF.ToString() == AppConstants.No && item.GroupType.ToString() != Convert.ToString(Session[AppConstants.UserRole]).ToUpper())
                {
                    Session["Otheruser"] = item.GroupType.ToString();
                    AddDelChargeForOtherUser = AddDelChargeForOtherUser + 1;
                }
            }

            DataTable dtLine = LineTable.Copy();
            ds.Tables.Add(dtLine);

            if (countForAddDeliveryCharge != 0)
            {
                Session["DeliveryChargeSupplier"] = supplier;
                lblError.Visible = true;
                lblError.Text = "Minimum spend not meet for suppliers:' " + supplier + "'.Kindly Add for the Delivery charge";
                return;
            }

            if (countForAddDeliveryCharge == 0 && AddDelChargeForOtherUser == 0)
            {
                var data = new MasterService.MasterSoapClient();
                //DataSet NonZeroDataset = FetchNotNullValues(ds);
                //dtLine = NonZeroDataset.Tables[1];
                var returnResult = string.Empty;
                if (ds.Tables[1].Rows[0]["SupplierCode"].ToString().Trim().ToUpper() == AppConstants.HoldingSupplier.Trim().ToString().ToUpper())
                {
                    //var sDocResult = data.Create_PurchaseRequest(ds, true, Session[AppConstants.DBName].ToString(), false, true, false);
                    returnResult = data.PRDraft_Submit(ds, Session[AppConstants.DBName].ToString());
                }
                else
                {
                    returnResult = data.Submit_PurchaseRequest(ds, Session[AppConstants.DBName].ToString());
                }

                //if (sDocResult.ToUpper() != "SUCCESS") throw new ArgumentException(sDocResult);

                //var returnResult = data.ConvertDraftToDocument(ds, Session[AppConstants.DBName].ToString());

                if (returnResult != "SUCCESS")
                {
                    if (dtLine.Rows.Count == 1 && Convert.ToDouble(dtLine.Rows[0]["OrderQuantity"]) == 0)
                    {
                        returnResult = "Material Request Draft Removed Successfully.";
                        lblError.Visible = true;
                        lblError.Text = returnResult;
                    }
                    else
                    {
                        if (returnResult == string.Empty)
                        {
                            returnResult = AppConstants.TryAfterSometime;
                        }
                        lblError.Visible = true;
                        lblError.Text = returnResult;
                    }
                    grdVendor.DataBind();
                    btnEDOnSubmitClick();
                }
                else
                {
                    lblError.Visible = true;
                    lblError.Text = "Material Request Document Created Successfully";
                    grdVendor.DataBind();
                    btnEDOnSubmitClick();
                }
                ds.Tables.Remove(dtHeader);
                ds.Tables.Remove(dtLine);
                ds.Clear();
            }
            else if (countForAddDeliveryCharge == 0 && AddDelChargeForOtherUser != 0)
            {
                // webservice to update the UDF in Line level for delivery charge.
                var data = new MasterService.MasterSoapClient();
                DataSet NonZeroDataset = FetchNotNullValues(ds);
                dtLine = NonZeroDataset.Tables[1];

                var sDocResult = string.Empty;
                if (ds.Tables[1].Rows[0]["SupplierCode"].ToString().Trim().ToUpper() == AppConstants.HoldingSupplier.ToString().Trim().ToUpper())
                {
                    //var sDocResult = data.Create_PurchaseRequest(ds, true, Session[AppConstants.DBName].ToString(), false, true, false);
                    sDocResult = data.PRDraft_Submit(NonZeroDataset, Session[AppConstants.DBName].ToString());
                }
                else
                {
                    sDocResult = data.Submit_PurchaseRequest(NonZeroDataset, Session[AppConstants.DBName].ToString());
                }

                //var sDocResult = data.Create_PurchaseRequest(NonZeroDataset, true, Session[AppConstants.DBName].ToString(), false, true, true);
                //var sDocResult = data.PRDraft_Submit(NonZeroDataset, Session[AppConstants.DBName].ToString());

                //if (sDocResult.ToUpper() != "SUCCESS") throw new ArgumentException(sDocResult);

                if (sDocResult.ToUpper() != "SUCCESS")
                {
                    lblError.Visible = true;
                    lblError.Text = sDocResult;
                    grdVendor.DataBind();
                    btnEDOnSubmitClick();
                }

                lblError.Visible = true;
                lblError.Text = "Some of the '" + Session["Otheruser"].ToString() + " items' needs to add Delivery charge for items. Kindly login as '" + Session["Otheruser"].ToString() + "' user and do submit.";
                grdVendor.DataBind();
                btnEDOnSubmitClick();
            }
            Session["checkSubmit"] = "1";
        }

        protected void chkDeliveryCharge_OnCheckedChanged(object sender, EventArgs e)
        {
            if (Session["DeliveryChargeSupplier"].ToString() != string.Empty)
            {
                DataTable LineTable = (DataTable)Session["MRDraftTable"];
                if (chkDeliveryCharge.Checked == true)
                {
                    foreach (DataRow item in LineTable.Rows)
                    {
                        if (item["SupplierCode"].ToString() == ddlMainSupplier.SelectedValue.ToString())
                        {
                            //item[17] = "Y";
                            item["DelChargeUDF"] = "Y";
                        }
                    }
                }
                else
                {
                    foreach (DataRow item in LineTable.Rows)
                    {
                        if (item["SupplierCode"].ToString() == ddlMainSupplier.SelectedValue.ToString())
                        {
                            //item[17] = "";
                            item["DelChargeUDF"] = "N";
                        }
                    }
                }
                Session["MRDraftTable"] = LineTable;
            }
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            mpePopup.Hide();
        }

        private void loadDropdowndata()
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

            DataSet dsSupplier = data.Get_Supplier_Details(Session[AppConstants.UserRole].ToString(), Session[AppConstants.U_ConnString].ToString());
            if (dsSupplier.Tables.Count != 0 && dsSupplier != null)
            {
                Session["PopUpSupplierList"] = dsSupplier.Tables[0];
                ddlSupplier.DataSource = dsSupplier.Tables[0];
                ddlSupplier.DataTextField = dsSupplier.Tables[0].Columns[AppConstants.CardName].ColumnName.ToString();
                ddlSupplier.DataValueField = dsSupplier.Tables[0].Columns[AppConstants.CardCode].ColumnName.ToString();
                ddlSupplier.DataBind();
            }
        }

        private void loadDraftSupplier(DataSet ds)
        {
            try
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataView view = new DataView(ds.Tables[0]);
                    //string[] columnNames = ds.Tables[0].Columns.Cast<DataColumn>()
                    //                 .Select(x => x.ColumnName)
                    //                 .ToArray();
                    //if (columnNames[1] != "SupplierCode")
                    //{
                    DataTable distinctValues = view.ToTable(true, AppConstants.SupplierName, AppConstants.SupplierCode);
                    DataRow dr = distinctValues.NewRow();
                    dr[AppConstants.SupplierName] = "ALL";
                    dr[AppConstants.SupplierCode] = "ALL";
                    distinctValues.Rows.Add(dr);
                    ddlMainSupplier.DataSource = distinctValues;
                    ddlMainSupplier.DataTextField = ds.Tables[0].Columns[AppConstants.SupplierName].ColumnName.ToString();
                    ddlMainSupplier.DataValueField = ds.Tables[0].Columns[AppConstants.SupplierCode].ColumnName.ToString();
                    ddlMainSupplier.DataBind();
                    //}
                    //else
                    //{
                    //    DataTable distinctValues = view.ToTable(true, AppConstants.SupplierName, AppConstants.SupplierCode);
                    //    DataRow dr = distinctValues.NewRow();
                    //    dr[AppConstants.SupplierName] = "ALL";
                    //    dr[AppConstants.SupplierCode] = "ALL";
                    //    distinctValues.Rows.Add(dr);
                    //    ddlMainSupplier.DataSource = distinctValues;
                    //    ddlMainSupplier.DataTextField = ds.Tables[0].Columns[AppConstants.SupplierName].ColumnName.ToString();
                    //    ddlMainSupplier.DataValueField = ds.Tables[0].Columns[AppConstants.SupplierCode].ColumnName.ToString();
                    //    ddlMainSupplier.DataBind();
                    //}
                }
            }
            catch (Exception ex)
            {

                this.lblError.Visible = true;
                this.lblError.Text = ex.Message;
            }
        }

        private DataTable CreatePopHeaderFormat()
        {
            DataTable tbTemp = new DataTable();
            tbTemp.Columns.Add("No");
            tbTemp.Columns.Add("GroupType");
            tbTemp.Columns.Add("SupplierCode");
            tbTemp.Columns.Add("SupplierName");
            tbTemp.Columns.Add("ItemCode");
            tbTemp.Columns.Add("Description");
            tbTemp.Columns.Add("InStock");
            tbTemp.Columns.Add("EventOrder");
            tbTemp.Columns.Add("Last7DaysAvg");
            tbTemp.Columns.Add("AlreadyOrdered");
            tbTemp.Columns.Add("MinStock");
            tbTemp.Columns.Add("RecommendedQuantity");
            tbTemp.Columns.Add("OrderQuantity");
            tbTemp.Columns.Add("UOM");
            tbTemp.Columns.Add("Price");
            tbTemp.Columns.Add("Total", typeof(decimal));
            tbTemp.Columns.Add("MinSpend");
            tbTemp.Columns.Add("ItemPerUnit");
            return tbTemp;
        }

        private DataTable CreateTableFormat()
        {
            DataTable tbTemp = new DataTable();
            tbTemp.Columns.Add("No");
            tbTemp.Columns.Add("GroupType");
            tbTemp.Columns.Add("SupplierCode");
            tbTemp.Columns.Add("SupplierName");
            tbTemp.Columns.Add("ItemCode");
            tbTemp.Columns.Add("Description");
            tbTemp.Columns.Add("InStock");
            tbTemp.Columns.Add("EventOrder");
            tbTemp.Columns.Add("Last7DaysAvg");
            tbTemp.Columns.Add("AlreadyOrdered");
            tbTemp.Columns.Add("MinStock");
            tbTemp.Columns.Add("RecommendedQuantity");
            tbTemp.Columns.Add("OrderQuantity");
            tbTemp.Columns.Add("UOM");
            tbTemp.Columns.Add("Price");
            tbTemp.Columns.Add("Total", typeof(double));
            tbTemp.Columns.Add("MinSpend");
            tbTemp.Columns.Add("DelChargeUDF");
            //tbTemp.Columns.Add("UpdateDeliveryCharge");
            tbTemp.Columns.Add("DeliveryDate");
            tbTemp.Columns.Add("Remarks");
            tbTemp.Columns.Add("ApproverRemarks");
            tbTemp.Columns.Add("LineId");
            tbTemp.Columns.Add("ItemPerUnit");
            return tbTemp;
        }

        private void LoadHeaderData(DataSet ds)
        {
            try
            {
                DataTable tblIMRBSHeader = CreateHeaderTable();
                DataRow rowNew = tblIMRBSHeader.NewRow();

                lblWareHouse.Text = ds.Tables[0].Rows[0]["WhsName"].ToString();
                lblWareHouseCode.Text = ds.Tables[0].Rows[0]["WhsCode"].ToString();
                lblUserName.Text = Session[Utils.AppConstants.UserCode].ToString().ToUpper();
                lblOrderDate.Text = ds.Tables[0].Rows[0]["DocDate"].ToString();
                //lblDeliveryDate.Text = ds.Tables[0].Rows[0]["DocDueDate"].ToString();

                if (ds.Tables[0].Rows[0]["DocDueDate"].ToString() != null)
                {
                    DateTime date = Convert.ToDateTime(ds.Tables[0].Rows[0]["DocDueDate"]);
                    lblDeliveryDate.Text = String.Format("{0:dd/MM/yyyy}", date);
                }
                lblStatus.Text = ds.Tables[0].Rows[0]["Status"].ToString();
                chkPriority.Checked = ds.Tables[0].Rows[0]["Urgent"].ToString() == AppConstants.Yes ? true : false;
                Session[AppConstants.OrderDate] = lblOrderDate.Text;
                lblDraftNo.Text = "DRF " + Session[AppConstants.DraftNo].ToString();

                rowNew["Id"] = 1;
                rowNew["PostingDate"] = Convert.ToDateTime(lblOrderDate.Text).Date;
                rowNew["Outlet"] = lblWareHouseCode.Text;
                rowNew["UserCode"] = Session[Utils.AppConstants.UserName].ToString().ToUpper();
                rowNew["DeliveryDate"] = Convert.ToDateTime(lblDeliveryDate.Text).Date;
                if (chkPriority.Checked == true) { rowNew["Urgent"] = AppConstants.Yes; } else { rowNew["Urgent"] = AppConstants.No; }
                rowNew["OrderTime"] = Convert.ToDateTime(Session[AppConstants.OrderDate]).TimeOfDay.Hours + ":" + Convert.ToDateTime(Session[AppConstants.OrderDate]).TimeOfDay.Minutes + " " + Convert.ToDateTime(Session[AppConstants.OrderDate]).ToString("tt");
                rowNew["UserId"] = Convert.ToInt16(Session[AppConstants.UserID]);
                rowNew["DocEntry"] = Session[AppConstants.DraftNo];
                tblIMRBSHeader.Rows.Add(rowNew);
                Session["MRDraftHeaderTable"] = tblIMRBSHeader;
                //DisplayCalendar();
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }

        private void LoadLineData(DataSet ds)
        {
            try
            {
                DataTable tblMRDraft = CreateTableFormat();
                if (ds != null && ds.Tables.Count > 0)
                {
                    int i = 1;
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        DataRow rowNew = tblMRDraft.NewRow();
                        rowNew["No"] = i;
                        rowNew["GroupType"] = row["GroupType"];
                        rowNew["SupplierCode"] = row["SupplierCode"];
                        rowNew["SupplierName"] = row["SupplierName"];
                        rowNew["ItemCode"] = row["ItemCode"];
                        rowNew["Description"] = row["ItemName"];
                        rowNew["InStock"] = row["OnHand"];
                        rowNew["EventOrder"] = row["OnOrder"];
                        rowNew["Last7DaysAvg"] = row["Last7DaysAvg"];
                        rowNew["AlreadyOrdered"] = row["AlreadyOrdered"];
                        rowNew["RecommendedQuantity"] = row["RecommendedQuantity"];
                        rowNew["OrderQuantity"] = row["OrderQuantity"];
                        rowNew["MinStock"] = row["MinLevel"];
                        rowNew["UOM"] = row["UOM"];
                        rowNew["Price"] = row["Price"];
                        rowNew["MinSpend"] = row["MinSpend"];
                        rowNew["Total"] = row["Total"].ToString() == string.Empty ? "0.0000" : row["Total"];
                        rowNew["DeliveryDate"] = row["DocDueDate"];
                        rowNew["DelChargeUDF"] = row["DelChargeUDF"];
                        rowNew["Remarks"] = row["Remarks"];
                        rowNew["ApproverRemarks"] = string.Empty;
                        rowNew["LineId"] = row["LineId"];
                        rowNew["ItemPerUnit"] = row["ItemPerUnit"];
                        tblMRDraft.Rows.Add(rowNew);
                        i++;
                    }
                    Session["MRDraftTable"] = tblMRDraft;
                    DataView dv = tblMRDraft.DefaultView;
                    this.grvMRDraft.DataSource = dv.ToTable();
                    this.grvMRDraft.DataBind();
                    CalcTotal(dv.ToTable(), dv.ToTable());
                    DisplayCalendar(ds);
                }
                else
                {
                    Session["MRDraftTable"] = tblMRDraft;
                    DataView dv = tblMRDraft.DefaultView;
                    this.grvMRDraft.DataSource = dv.ToTable();
                    this.grvMRDraft.DataBind();
                    CalcTotal(dv.ToTable(), dv.ToTable());
                    DisplayCalendar(ds);
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }

        private void BindData(DataTable tblData)
        {
            Session["MRDraftTable"] = tblData;

            DataView dv = tblData.DefaultView;
            if (ddlMainSupplier.Text != "ALL")
            {
                //dv.Sort = "ItemCode  ASC";
                var rows = from row in dv.ToTable().AsEnumerable()
                           where row.Field<string>("SupplierCode") == ddlMainSupplier.SelectedValue
                           select row;
                grvMRDraft.DataSource = rows.CopyToDataTable();
                grvMRDraft.DataBind();
            }
            else
            {
                grvMRDraft.DataSource = dv.ToTable();
                grvMRDraft.DataBind();
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

        public void mouseOverandMouseOut()
        {
            btnSaveDraft.Attributes.Add("class", "static");
            btnSaveDraft.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnSaveDraft.Attributes.Add("onMouseOut", "this.className='static'");

            btnAddItems.Attributes.Add("class", "static");
            btnAddItems.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnAddItems.Attributes.Add("onMouseOut", "this.className='static'");

            btnClose.Attributes.Add("class", "static");
            btnClose.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnClose.Attributes.Add("onMouseOut", "this.className='static'");

            btnMainSearch.Attributes.Add("class", "static");
            btnMainSearch.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnMainSearch.Attributes.Add("onMouseOut", "this.className='static'");

            btnSearch.Attributes.Add("class", "static");
            btnSearch.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnSearch.Attributes.Add("onMouseOut", "this.className='static'");

            btnSubmitItems.Attributes.Add("class", "static");
            btnSubmitItems.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnSubmitItems.Attributes.Add("onMouseOut", "this.className='static'");

            btnSubmit.Attributes.Add("class", "static");
            btnSubmit.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnSubmit.Attributes.Add("onMouseOut", "this.className='static'");
        }

        private void CalcTotal(DataTable dt, DataTable dtOverall)
        {
            try
            {
                if (dt != null)
                {
                    double overallTotal = 0;
                    foreach (DataRow row in dtOverall.Rows)
                    {
                        string get = row["Total"].ToString();
                        if (get.Trim() != string.Empty)
                        {
                            overallTotal += double.Parse(row["Total"].ToString());
                        }
                    }
                    double Total = 0;
                    if (ddlMainSupplier.Text != "ALL")
                    {
                        var rows = from row in dt.AsEnumerable()
                                   where row.Field<string>("SupplierCode") == ddlMainSupplier.SelectedValue
                                   select row;
                        foreach (DataRow row in rows.CopyToDataTable().Rows)
                        {
                            string get = row["Total"].ToString();
                            if (get.Trim() != string.Empty)
                            {
                                Total += double.Parse(row["Total"].ToString());
                            }
                        }
                    }
                    else
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            string get = row["Total"].ToString();
                            if (get.Trim() != string.Empty)
                            {
                                Total += double.Parse(row["Total"].ToString());
                            }
                        }
                    }
                    Session["Total"] = Total;
                    Session["OverallTotal"] = overallTotal;
                    this.lblGrandTotal.Text = string.Format("${0}", Total.ToString(Utils.AppConstants.NUMBER_FORMAT));
                    this.lblTotalOutlet.Text = string.Format("${0}", overallTotal.ToString(Utils.AppConstants.NUMBER_FORMAT));
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }

        private void DisplayCalendar(DataSet ds)
        {
            try
            {
                //Load Calendar 
                if (ds != null && ds.Tables.Count > 0)
                {
                    this.chkCalendar.Items.Clear();
                    Session["MinSpend"] = ds.Tables[0].Rows[0][AppConstants.MinSpend];
                    //this.lblMinSpend.Text = string.Format("${0}", Convert.ToDouble(ds.Tables[0].Rows[0][AppConstants.MinSpend]).ToString(Utils.AppConstants.NUMBER_FORMAT));
                    if (ddlMainSupplier.SelectedItem.ToString() != "ALL")
                    {
                        var rows = from row in ds.Tables[0].AsEnumerable()
                                   where row.Field<string>("SupplierCode") == ddlMainSupplier.SelectedValue
                                   select row;
                        DataTable dt = new DataTable();
                        if (rows.Count() > 0)
                        {
                            dt = rows.CopyToDataTable();
                        }
                        DataTable tbNew = new DataTable();
                        tbNew.Columns.Add(AppConstants.Name);
                        tbNew.Columns.Add(AppConstants.Value);
                        if (dt.Columns.Count > 0)
                        {
                            int colIndex = dt.Columns.Count - 7;
                            for (int i = colIndex; i < dt.Columns.Count; i++)
                            {
                                DataRow rNew = tbNew.NewRow();
                                rNew[AppConstants.Name] = dt.Columns[i].ColumnName;
                                rNew[AppConstants.Value] = dt.Rows[0][i];
                                tbNew.Rows.Add(rNew);
                            }
                        }
                        else
                        {
                            lblDeliveryCalender.Visible = false;
                            lblSeperator.Visible = false;
                            chkCalendar.Visible = false;
                            lblDate.Visible = false;
                            lblSeperator1.Visible = false;
                            lblDeliveryDate.Visible = false;
                        }

                        this.chkCalendar.DataSource = tbNew;
                        this.chkCalendar.DataTextField = AppConstants.Name;
                        this.chkCalendar.DataValueField = AppConstants.Value;
                        this.chkCalendar.DataBind();
                        if (this.chkCalendar.Items.Count > 0)
                        {
                            string day = string.Empty;
                            int count = 0;
                            for (int i = 0; i < this.chkCalendar.Items.Count; i++)
                            {
                                if (this.chkCalendar.Items[i].Value.ToString() == AppConstants.Yes)
                                {
                                    this.chkCalendar.Items[i].Selected = true;
                                    if (count == 0)
                                    {
                                        Session[AppConstants.DayName] = chkCalendar.Items[0].Text;
                                        count = count + 1;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (ddlMainSupplier.Items.Count > 2)
                        {
                            for (int i = 0; i < 7; i++)
                            {
                                this.chkCalendar.Items[i].Selected = false;
                            }
                            Session[AppConstants.DayName] = string.Empty;
                            Session["TotalDays"] = 0;
                        }
                    }
                }
                else
                {
                    if (ddlMainSupplier.Items.Count > 2)
                    {
                        if (chkCalendar.Items.Count != 0)
                        {
                            for (int i = 0; i < 7; i++)
                            {
                                this.chkCalendar.Items[i].Selected = false;
                            }
                        }
                        Session[AppConstants.DayName] = string.Empty;
                        Session["TotalDays"] = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }

        private void CalcDeliveryDate()
        {
            try
            {
                if (Session[AppConstants.DayName] != null && Session[AppConstants.DayName].ToString() != "")
                {
                    for (int i = 0; i <= 6; i++)
                    {
                        string dayOfWeek = string.Empty;
                        string sessionValue = (string)Session[AppConstants.DayName].ToString();
                        if (i == 0)
                        {
                            dayOfWeek = DateTime.Now.DayOfWeek.ToString();
                        }
                        else
                        {
                            dayOfWeek = DateTime.Now.AddDays(i).DayOfWeek.ToString();
                        }

                        if (dayOfWeek.Substring(0, 3).ToUpper().ToString() == sessionValue.ToUpper().ToString())
                        {
                            Session["TotalDays"] = i;
                            break;
                        }
                    }
                }
                else
                {
                    Session["TotalDays"] = 0;
                }
            }
            catch (Exception)
            {

                throw;
            }
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
            tbHeader.Columns.Add("DocEntry");
            return tbHeader;
        }

        public void RemoveSupplier(DataTable dtSupplier)
        {
            if (lblWareHouseCode.Text == "01CKT")
            {
                foreach (DataRow dr in dtSupplier.Rows)
                {
                    if (dr[AppConstants.CardCode].ToString() == "VA-HARCT")
                    {
                        dtSupplier.Rows.Remove(dr);
                        break;
                    }
                }
            }
            else
            {
                if (Session[AppConstants.UserRole].ToString().ToUpper() == AppConstants.Kitchen)
                {
                    string find = "CardCode = 'VA-HARCT'";
                    DataRow[] foundRows = dtSupplier.Select(find);
                    if (foundRows.Count() == 0)
                        dtSupplier.Rows.Add("VA-HARCT", "Harry`s Central Kitchen");
                }
            }
            DataView dv = new DataView(dtSupplier);
            dv.Sort = AppConstants.CardName;
            ddlSupplier.DataSource = dv.ToTable();
            ddlSupplier.DataTextField = dtSupplier.Columns[AppConstants.CardName].ColumnName.ToString();
            ddlSupplier.DataValueField = dtSupplier.Columns[AppConstants.CardCode].ColumnName.ToString();
            ddlSupplier.DataBind();
        }

        public void btnEDOnSubmitClick()
        {
            btnAddItems.Enabled = false;
            btnSaveDraft.Enabled = false;
            btnSubmit.Enabled = false;
            btnMainSearch.Enabled = false;

            btnAddItems.Style["background-image"] = "url('Images/bgButton_disable.png')";
            btnSaveDraft.Style["background-image"] = "url('Images/bgButton_disable.png')";
            btnSubmit.Style["background-image"] = "url('Images/bgButton_disable.png')";
            btnMainSearch.Style["background-image"] = "url('Images/bgButton_disable.png')";
        }

        public void btnEDOnLoadandOnSaveClick()
        {
            btnAddItems.Enabled = true;
            btnSaveDraft.Enabled = false;
            btnSubmit.Enabled = true;
            btnMainSearch.Enabled = true;
            btnSaveDraft.Style["background-image"] = "url('Images/bgButton_disable.png')";
            btnSubmit.Style["background-image"] = "url('Images/bgButton.png')";
        }

        public void btnEDOnGridChange()
        {
            btnAddItems.Enabled = true;
            btnSaveDraft.Enabled = true;
            btnSubmit.Enabled = false;
            btnMainSearch.Enabled = true;
            btnSubmit.Style["background-image"] = "url('Images/bgButton_disable.png')";
            btnSaveDraft.Style["background-image"] = "url('Images/bgButton.png')";
        }

        public DataSet FetchNotNullValues(DataSet ds)
        {
            DataTable dt = ds.Tables[1];

            IEnumerable<DataRow> rows = dt.AsEnumerable()
            .Where(r => r.Field<string>("GroupType") == Session[AppConstants.UserRole].ToString().ToUpper()
            || r.Field<string>("ItemCode") == AppConstants.delChargeItemCode);
            if (rows.Count() > 0)
            {
                dt = rows.CopyToDataTable();
            }
            dt = dt.DefaultView.ToTable();
            ds.Tables.Remove(ds.Tables[1]);
            ds.Tables.Add(dt.Copy());
            return ds;
        }
    }
}