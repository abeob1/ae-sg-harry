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
    public partial class MaterialRequestApprovalNew : System.Web.UI.Page
    {
        private decimal TotalAmt = (decimal)0.0;
        public DataTable tblApprove = new DataTable();
        //public CheckBoxList chkList = new CheckBoxList();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session[Utils.AppConstants.UserCode].ToString() != string.Empty)
                {
                    if (!IsPostBack)
                    {
                        Session[AppConstants.IsBackPage] = 9;
                        tblApprove = CreateApproveTable();
                        Session["tblApprove"] = tblApprove;
                        LoadHeaderTableData();
                        LoadOutletDropDown();
                        //LoadData(this.ddlWareHouse.SelectedValue.ToString());

                        ddlWareHouse.SelectedValue = Session[AppConstants.PRWhsCode].ToString();
                        btnSubmit.Enabled = true;
                        this.grvParentGrid.DataSource = null;
                        this.grvParentGrid.DataBind();
                        lblError.Text = string.Empty;
                        LoadData(this.ddlWareHouse.SelectedValue.ToString());

                        mouseOverandMouseOut();
                        ddlWareHouse.Enabled = false;
                        if (grvParentGrid.Rows.Count == 0)
                        {
                            btnSubmit.Enabled = false;
                            btnSubmit.Style["background-image"] = "url('Images/bgButton_disable.png')";
                        }
                    }
                }
                else
                {
                    Response.Redirect(AppConstants.LoginURL);
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }

        protected void ddlWareHouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.ddlWareHouse.Text.Trim().Length > 0)
                {
                    btnSubmit.Enabled = true;
                    this.grvParentGrid.DataSource = null;
                    this.grvParentGrid.DataBind();
                    lblError.Text = string.Empty;
                    LoadData(this.ddlWareHouse.SelectedValue.ToString());
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
                string PRNo = grvParentGrid.DataKeys[e.Row.RowIndex].Value.ToString();
                CheckBoxList chkCalendar = e.Row.FindControl("chkAddDeliveryCharge") as CheckBoxList;
                CheckBoxList chkApprove = e.Row.FindControl("chkApprove") as CheckBoxList;
                DisplayCalendar(chkCalendar, chkApprove, PRNo);
                //chkList = e.Row.FindControl("chkCalendar") as CheckBoxList;
                //Session["Check1"] = chkList.Items[0].Selected;
                //Session["Check2"] = chkList.Items[1].Selected;
                e.Row.Attributes["style"] = "cursor:pointer";

                GridView grvChildGrid = e.Row.FindControl("grvChildGrid") as GridView;
                grvChildGrid.ToolTip = PRNo;
                DataTable DocEntrydt = new DataTable();
                DocEntrydt.Columns.Add("DocEntry");
                if (Session["MRASearch"] == null)
                {
                    grvChildGrid.DataSource = GetItemData(PRNo);

                    grvChildGrid.DataBind();
                    Session["supplierCount"] = grvChildGrid.DataSource;

                    DataTable dt = (DataTable)Session["supplierCount"];
                    //if (dt.Rows.Count == 0)
                    //{
                    //    grvParentGrid.DataSource = DocEntrydt;
                    //    grvParentGrid.DataBind();
                    //}
                }
                else
                {

                    grvChildGrid.DataSource = GetItemData_Search(PRNo);
                    grvChildGrid.DataBind();
                    Session["supplierCount"] = grvChildGrid.DataSource;
                    DataTable dt = (DataTable)Session["supplierCount"];
                    //if (dt.Rows.Count == 0)
                    //{
                    //    grvParentGrid.DataSource = DocEntrydt;
                    //    grvParentGrid.DataBind();
                    //}
                }
            }
        }

        protected void grvChildGrid_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("onmouseover", "MouseEvents(this, event)");
                e.Row.Attributes.Add("onmouseout", "MouseEvents(this, event)");
                e.Row.Attributes["style"] = "cursor:pointer";
                TotalAmt += Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "Total"));
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[17].Text = String.Format("${0}", TotalAmt);
                e.Row.Cells[17].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Font.Bold = true;
            }
        }

        protected void grvChildGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView gvwChild = (sender as GridView);
            gvwChild.PageIndex = e.NewPageIndex;
            gvwChild.DataSource = GetItemData(gvwChild.ToolTip);
            gvwChild.DataBind();
        }

        protected void txtOrderQuantity_OnTextChanged(object sender, EventArgs e)
        {
            lblError.Text = string.Empty;
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
            Label lblDocEntry = (Label)row.FindControl("lblDocEntry");
            DataTable tb = (DataTable)Session["ApprovalList"];
            if (tb != null)
            {
                DataRow[] rupdate = tb.Select("SupplierCode='" + lblSupplierCode.Text + "' and ItemCode='" + lblItemCode.Text + "' and DocEntry = '" + lblDocEntry.Text + "'");
                if (rupdate.Length > 0)
                {
                    rupdate[0]["OrderQuantity"] = txtQuantity.Text == string.Empty ? "0.0000" : txtQuantity.Text;
                    GetPrice(rupdate[0]);
                }
                this.grvParentGrid.EditIndex = -1;
                Session["ApprovalList"] = null;
                Session["ApprovalList"] = tb;
                BindData(tb, string.Empty);
                //txtSearch.Text = string.Empty;
                //CalcTotal(tb);
            }
        }

        protected void txtApproverRemarks_OnTextChanged(object sender, EventArgs e)
        {
            lblError.Text = string.Empty;
            GridViewRow row = (GridViewRow)((DataControlFieldCell)((TextBox)sender).Parent).Parent;
            ViewState["rowindex"] = row.RowIndex;
            TextBox txtAppRemarks = (TextBox)row.FindControl("txtApproverRemarks");
            //if (txtQuantity.Text.Trim().Length == 0)
            //{
            //    ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", "alert('Pls input quantity');", true);
            //    txtQuantity.Focus();
            //    return;
            //}
            Label lblItemCode = (Label)row.FindControl("lblItemCode");
            Label lblSupplierCode = (Label)row.FindControl("lblSupplierCode");
            Label lblDocEntry = (Label)row.FindControl("lblDocEntry");
            DataTable tb = (DataTable)Session["ApprovalList"];
            if (tb != null)
            {
                DataRow[] rupdate = tb.Select("SupplierCode='" + lblSupplierCode.Text + "' and ItemCode='" + lblItemCode.Text + "' and DocEntry = '" + lblDocEntry.Text + "'");
                if (rupdate.Length > 0)
                {
                    rupdate[0]["ApproverRemarks"] = txtAppRemarks.Text == string.Empty ? "" : txtAppRemarks.Text;
                    //GetPrice(rupdate[0]);
                }
                this.grvParentGrid.EditIndex = -1;
                Session["ApprovalList"] = null;
                Session["ApprovalList"] = tb;
                BindData(tb, string.Empty);
                //txtSearch.Text = string.Empty;
                //CalcTotal(tb);
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (ddlWareHouse.Text != " --- Select Outlet --- ")
            {
                lblError.Text = string.Empty; string supplier = string.Empty; string prNo = string.Empty; string allPRNo = string.Empty; int k = 0;
                DataSet ds = new DataSet();
                int Approvecount = 0; int DeliveryCount = 0; int countForAddDeliveryCharge = 0;
                string checkItem = string.Empty; bool checkValue = false;
                DataTable lineItems = new DataTable();
                DataTable ApprovedLineItems = new DataTable();
                DataTable tblDeliveryCharge = CreateTableForDeliveryCharge();
                lineItems = (DataTable)Session["ApprovalList"];
                DataTable tblResult = CreateResultTable();
                DataTable dtCopy = new DataTable();
                DataTable dtCopylineItems = new DataTable();
                foreach (GridViewRow gvr in this.grvParentGrid.Rows)
                {
                    countForAddDeliveryCharge = 0;
                    Label lblPRNo = (Label)gvr.FindControl("lblPRNo");
                    //Label lblDocType = (Label)gvr.FindControl("lblDocType");
                    CheckBoxList cblApprove = (CheckBoxList)gvr.FindControl("chkApprove");
                    CheckBoxList cblReject = (CheckBoxList)gvr.FindControl("chkReject");
                    CheckBoxList cblDeliveryCharge = (CheckBoxList)gvr.FindControl("chkAddDeliveryCharge");

                    if (cblApprove != null)
                    {
                        if (cblApprove.Items[0].Selected)
                        {
                            Approvecount = Approvecount + 1;
                            checkItem = cblApprove.Items[0].Value;
                            if (k != 0)
                            {
                                k = k - 1;
                            }
                            else
                            {
                                k = 0;
                            }
                        }
                        else
                        {
                            Approvecount = 0;
                            if (k == 0)
                            {
                                allPRNo = lblPRNo.Text;
                                k = k + 1;
                            }
                            else
                            { allPRNo = allPRNo + "," + lblPRNo.Text; }
                        }
                    }

                    if (cblDeliveryCharge != null)
                    {
                        for (int i = 0; i < cblDeliveryCharge.Items.Count; i++)
                        {
                            if (cblDeliveryCharge.Items[i].Selected)
                            {
                                DeliveryCount = DeliveryCount + 1;
                                checkValue = cblDeliveryCharge.Items[i].Selected;
                            }
                        }
                    }

                    if (DeliveryCount == 0)
                    {
                        DataTable dt = (DataTable)Session["ApprovalList"];
                        dt.DefaultView.RowFilter = "ItemName IS NOT NULL";
                        dt = dt.DefaultView.ToTable();
                        var result = from r in dt.AsEnumerable()
                                     group r by new
                                     {
                                         Group = r["SupplierCode"],
                                         MinSpend = r["MinSpend"],
                                         PRNo = r["DocEntry"],
                                         SupName = r["SupplierName"],
                                         DelChargeUDF = r["DelChargeUDF"]
                                     } into g
                                     select new
                                     {
                                         Group = g.Key.Group,
                                         SupName = g.Key.SupName,
                                         MinSpend = Convert.ToDouble(g.Key.MinSpend),
                                         PRNo = g.Key.PRNo,
                                         DelChargeUDF = g.Key.DelChargeUDF,
                                         IsApproval = g.Sum(x => Convert.ToInt32(x["Approval"])),
                                         Sum = g.Sum(x => Convert.ToDouble(x["Total"]))
                                     };
                        foreach (var item in result)
                        {
                            if (item.IsApproval > 0)
                            {
                                if (item.Sum < item.MinSpend && item.Sum > 0)
                                {
                                    if (item.DelChargeUDF.ToString() == "N")
                                    {
                                        //cblDeliveryCharge.Enabled = true;
                                        countForAddDeliveryCharge = countForAddDeliveryCharge + 1;
                                        //if (Loopcount == 0)
                                        //{
                                        if (countForAddDeliveryCharge == 1)
                                        {
                                            supplier = item.SupName.ToString();
                                            prNo = item.PRNo.ToString();
                                        }
                                        else
                                        {
                                            supplier = supplier + "','" + item.SupName.ToString();
                                            prNo = prNo + "," + item.PRNo.ToString();
                                        }
                                        //}
                                    }
                                    else
                                    {
                                        //cblDeliveryCharge.Enabled = false;
                                    }
                                }
                            }
                        }
                        //Loopcount = 1;
                        Session["ApprovalList"] = dt;
                        lineItems = (DataTable)Session["ApprovalList"];
                    }
                    DataRow rowNew = tblResult.NewRow();
                    rowNew["DocEntry"] = lblPRNo.Text;
                    rowNew["Approved"] = (checkItem == "Approve") ? "Y" : "N";
                    rowNew["DeliveryCharge"] = (checkValue == true) ? "Y" : "N";
                    rowNew["UserName"] = Session[AppConstants.UserCode].ToString();
                    rowNew["UserRole"] = Session[AppConstants.ApprovalLevel].ToString();
                    IEnumerable<DataRow> rows1 = lineItems.AsEnumerable().Where(r => r.Field<string>("DocEntry") == lblPRNo.Text);
                    if (rows1.Count() > 0)
                    {
                        DataTable dt = rows1.CopyToDataTable();
                        rowNew["DocType"] = dt.Rows[0]["DocType"].ToString();
                    }

                    if (checkValue == true)
                    {
                        DeliveryCount = DeliveryCount - 1;
                    }
                    //    IEnumerable<DataRow> rows = lineItems.AsEnumerable().Where(r => r.Field<string>("DocEntry") == lblPRNo.Text);
                    IEnumerable<DataRow> rows = lineItems.AsEnumerable().Where(r => r.Field<string>("DocEntry") == lblPRNo.Text);
                    if (rows.Count() > 0)
                    {

                        DataTable dt = rows.CopyToDataTable();
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (checkValue == true)
                            {
                                dr["DelChargeUDF"] = "Y";
                            }
                        }
                        ApprovedLineItems.Merge(dt);
                    }

                    DataTable newTable = (DataTable)Session["ApprovalList"];
                    foreach (DataRow dr in newTable.Rows)
                    {
                        foreach (DataRow item in ApprovedLineItems.Rows)
                        {
                            if (dr["ItemCode"] == item["ItemCode"])
                            {
                                dr["DelChargeUDF"] = item["DelChargeUDF"];
                            }
                        }
                    }
                    tblResult.Rows.Add(rowNew);
                    tblResult.TableName = "tblHeader";
                    lineItems.TableName = "tblLine";
                    dtCopy = tblResult.Copy();
                    dtCopylineItems = lineItems.Copy();
                }

                if (Approvecount != this.grvParentGrid.Rows.Count)
                {
                    lblError.Visible = true;
                    lblError.Text = "Kindly Check the Approve for PR Draft No : " + allPRNo;
                    return;
                }
                if (countForAddDeliveryCharge >= 0)
                {
                    if (prNo != string.Empty)
                    {
                        lblError.Visible = true;
                        lblError.Text = "Minimum spend not meet for the PR Draft No:' " + prNo + "' for suppliers:' " + supplier + "'. Kindly Add for the Delivery charge";
                        return;
                    }
                }
                ds.Tables.Add(dtCopy);
                dtCopylineItems.DefaultView.RowFilter = "Approval > 0";
                dtCopylineItems = dtCopylineItems.DefaultView.ToTable();
                ds.Tables.Add(dtCopylineItems);
                Session["ResultTable"] = ds;
                //var sumOfQty = dtCopylineItems.AsEnumerable().Sum(x => x["OrderQuantity"] == DBNull.Value ? 0 : Convert.ToDecimal(x["OrderQuantity"]));
                //if (sumOfQty > 0)
                //{
                //Call the web service method to approve
                var data = new MasterService.MasterSoapClient();
                // var returnResult = data.Update_ApprovalStatus(ds, Session[AppConstants.DBName].ToString());

                var returnResult = string.Empty;
                DataSet oDSPRFinal = new DataSet();
                DataSet oDSPQFinal = new DataSet();


                oDSPQFinal = returnPQDataSet(ds);
                oDSPRFinal = returnPRDataSet(ds);

                if (oDSPQFinal != null && oDSPQFinal.Tables.Count > 0)
                {
                    if (oDSPQFinal.Tables[0].Rows.Count > 0)
                    {
                        returnResult = data.Update_ApprovalStatus(oDSPQFinal, Session[AppConstants.DBName].ToString());
                    }
                }

                if (oDSPRFinal != null && oDSPRFinal.Tables.Count > 0)
                {
                    string finalResult = string.Empty;
                    if (oDSPRFinal.Tables[0].Rows.Count > 0)
                    {
                        if (returnResult != "SUCCESS")
                        {
                            finalResult = returnResult;
                        }
                        returnResult = data.Approve_PurchaseRequest(oDSPRFinal, Session[AppConstants.DBName].ToString());
                        returnResult = finalResult + returnResult;
                    }
                }


                //   returnResult = data.Approve_PurchaseRequest(ds, Session[AppConstants.DBName].ToString());

                if (returnResult != "SUCCESS")
                {
                    if (returnResult == string.Empty)
                    {
                        returnResult = AppConstants.TryAfterSometime;
                    }
                    ClearFields();
                    lblError.Visible = true;
                    lblError.Text = returnResult;
                    btnSubmit.Enabled = false;
                }
                //else
                //{
                //    var result = data.ConvertDraftToDocument(ds, Session[AppConstants.DBName].ToString());
                //    if (result != "SUCCESS")
                //    {
                //        ClearFields();
                //        lblError.Visible = true;
                //        lblError.Text = result;
                //        btnSubmit.Enabled = false;
                //    }
                else
                {
                    ClearFields();
                    lblError.Visible = true;
                    lblError.Text = "Material Request Document Created Successfully";
                    btnSubmit.Enabled = false;
                }
                //  }
                //}
                //else
                //{
                //    lblError.Visible = true;
                //    lblError.Text = "Order Quantity cannot be Zero/Empty.";
                //}
                //ClearFields();
                ds.Tables.Remove(dtCopy);
                ds.Tables.Remove(dtCopylineItems);
                ds.Clear();
            }
            else
            {
                lblError.Visible = true;
                //lblError.Text = "Kindly select the Outlet.";
                lblError.Text = string.Empty;
                btnSubmit.Enabled = false;
                btnSubmit.Style["background-image"] = "url('Images/bgButton_disable.png')";

            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(AppConstants.OutletListPendingApprovalURL);
            //ClearFields();
        }

        private DataTable GetItemData(string supplier)
        {
            try
            {
                DataTable dt = new DataTable();
                DataTable tblMRA = CreateTableFormat();
                DataTable dtItem = (DataTable)Session["ApprovalList"];
                DataTable dtItemList = null;
                dtItemList = dtItem.Clone();
                dtItemList.Clear();
                DataRow[] ItemRows = dtItem.Select("DocEntry ='" + supplier + "'");
                dtItemList = ItemRows.CopyToDataTable();
                if (dtItemList.Rows.Count > 0)
                {
                    dtItemList.DefaultView.RowFilter = "ItemName IS NOT NULL";
                    dtItemList = dtItemList.DefaultView.ToTable();
                    int i = 1;
                    foreach (DataRow row in dtItemList.Rows)
                    {
                        if (Convert.ToInt32(row["Approval"]) > 0)
                        {
                            DataRow rowNew = tblMRA.NewRow();
                            rowNew["No"] = i;
                            rowNew["PRNo"] = row["DocEntry"];
                            rowNew["SupplierCode"] = row["SupplierCode"];
                            rowNew["SupplierName"] = row["SupplierName"];
                            rowNew["ItemCode"] = row["ItemCode"];
                            rowNew["Description"] = row["ItemName"];
                            rowNew["InStock"] = row["OnHand"];
                            rowNew["EventOrder"] = row["OnOrder"];
                            rowNew["Last7DaysAvg"] = row["Last7DaysAvg"];
                            rowNew["AlreadyOrdered"] = row["AlreadyOrdered"];
                            rowNew["MinStock"] = row["MinLevel"];
                            rowNew["UOM"] = row["UOM"];
                            rowNew["RecommendedQuantity"] = row["RecommendedQuantity"];
                            rowNew["Price"] = row["Price"];
                            rowNew["MinSpend"] = row["MinSpend"];
                            rowNew["OrderQuantity"] = row["OrderQuantity"];
                            rowNew["Total"] = double.Parse(row["Total"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);
                            rowNew["UserApprovalLevel"] = Session[AppConstants.ApprovalLevel];
                            rowNew["DeliveryDate"] = row["DocDueDate"].ToString().Substring(0, 10);
                            rowNew["DocType"] = row["DocType"];
                            rowNew["Remarks"] = row["Remarks"];
                            rowNew["ApproverRemarks"] = row["ApproverRemarks"];
                            //rowNew["DeliveryDate"] = row["DocDueDate"].ToString().Substring(0, 10);
                            tblMRA.Rows.Add(rowNew);
                            i++;
                        }
                    }
                    DataView dv = tblMRA.DefaultView;
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
            DataTable supdt = (DataTable)tblData.DefaultView.ToTable(true, "DocEntry");
            grvParentGrid.DataSource = supdt;
            grvParentGrid.DataBind();
        }

        private DataTable GetItemData_Search(string supplier)
        {
            try
            {
                DataTable dt = new DataTable();
                DataTable tblMRA = CreateTableFormat();
                DataTable dtItem = (DataTable)Session["MRASearch"];
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
                        DataRow rowNew = tblMRA.NewRow();
                        rowNew["No"] = i;
                        rowNew["PRNo"] = row["DocEntry"];
                        rowNew["SupplierCode"] = row["SupplierCode"];
                        rowNew["ItemCode"] = row["ItemCode"];
                        rowNew["Description"] = row["Description"];
                        rowNew["InStock"] = row["OnHand"];
                        rowNew["EventOrder"] = row["OnOrder"];
                        rowNew["Last7DaysAvg"] = row["Last7Days"];
                        rowNew["AlreadyOrdered"] = row["AlreadyOrdered"];
                        rowNew["MinStock"] = row["MinLevel"];
                        rowNew["UOM"] = row["UOM"];
                        rowNew["Price"] = row["Price"];
                        rowNew["MinSpend"] = row["MinSpend"];
                        rowNew["RecommendedQuantity"] = row["RecommendedQuantity"];
                        rowNew["OrderQuantity"] = row["OrderQuantity"];
                        rowNew["Total"] = double.Parse(row["Total"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);
                        rowNew["Vendor"] = row["SupplierCode"];
                        rowNew["UserApprovalLevel"] = Session[AppConstants.ApprovalLevel];
                        rowNew["DeliveryDate"] = row["DocDueDate"].ToString().Substring(0, 10);
                        rowNew["DocType"] = row["DocType"];
                        rowNew["Remarks"] = row["Remarks"];
                        rowNew["ApproverRemarks"] = row["ApproverRemarks"];
                        tblMRA.Rows.Add(rowNew);
                        i++;
                    }

                    DataView dv = tblMRA.DefaultView;
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

        private void LoadData(string sOutlet)
        {
            try
            {
                Session["ApprovalList"] = null;
                DataTable DocEntrydt = new DataTable();
                DocEntrydt.Columns.Add("DocEntry");
                var objC = new MasterService.MasterSoapClient();

                DataSet ds = objC.Get_GetPendingDrafts(Session[AppConstants.DBName].ToString(), ddlWareHouse.SelectedValue.ToString()
                    , Session[AppConstants.UserRole].ToString(), Session[AppConstants.ApprovalLevel].ToString());
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    Session["ApprovalList"] = dt;
                    dt.DefaultView.RowFilter = "Approval > 0";
                    dt = dt.DefaultView.ToTable();
                    DataTable supdt = (DataTable)dt.DefaultView.ToTable(true, "DocEntry");

                    DataTable dtApprovl = (DataTable)dt.DefaultView.ToTable(true, "Approval");
                    if (supdt.Rows.Count > 0 && dtApprovl.Rows.Count > 0)
                    {
                        Int32 sum = Convert.ToInt32(dtApprovl.Compute("Sum(Approval)", ""));
                        if (sum > 0)
                        {
                            grvParentGrid.DataSource = supdt;
                            grvParentGrid.DataBind();
                        }
                        else
                        {
                            grvParentGrid.DataSource = DocEntrydt;
                            grvParentGrid.DataBind();
                        }
                    }
                    else
                    {
                        grvParentGrid.DataSource = DocEntrydt;
                        grvParentGrid.DataBind();
                    }
                }
                else
                {
                    grvParentGrid.DataSource = DocEntrydt;
                    grvParentGrid.DataBind();
                }

            }
            catch (Exception ex)
            {
                this.lblError.Text = "Error Occured while loading the data in Grid..." + ex.Message;
                this.lblError.Visible = true;
            }
        }

        private DataTable CreateTableFormat()
        {
            DataTable tbTemp = new DataTable();
            tbTemp.Columns.Add("No");
            tbTemp.Columns.Add("PRNo");
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
            tbTemp.Columns.Add("Total");
            tbTemp.Columns.Add("MinSpend");
            tbTemp.Columns.Add("UserApprovalLevel");
            tbTemp.Columns.Add("DeliveryDate");
            tbTemp.Columns.Add("DocType");
            tbTemp.Columns.Add("Remarks");
            tbTemp.Columns.Add("ApproverRemarks");
            //tbTemp.Columns.Add("DeliveryCharge");
            //tbTemp.Columns.Add("Approval");
            return tbTemp;
        }

        private DataTable CreateResultTable()
        {
            DataTable tbTemp = new DataTable();
            tbTemp.Columns.Add("DocEntry");
            tbTemp.Columns.Add("Approved");
            tbTemp.Columns.Add("DeliveryCharge");
            tbTemp.Columns.Add("UserName");
            tbTemp.Columns.Add("UserRole");
            tbTemp.Columns.Add("DocType");
            return tbTemp;
        }

        public DataTable CreateApproveTable()
        {
            DataTable dtApprove = new DataTable();
            dtApprove.Columns.Add("PRNo");
            dtApprove.Columns.Add("Approve");
            return dtApprove;
        }

        private void LoadHeaderTableData()
        {
            lblOrderDate.Text = DateTime.Now.ToShortDateString();
            Session[AppConstants.OrderDate] = DateTime.Now;
            //Session[AppConstants.IsBackPage] = 0;
            lblUserName.Text = Session[Utils.AppConstants.UserCode].ToString().ToUpper();
        }

        public void mouseOverandMouseOut()
        {
            btnSubmit.Attributes.Add("class", "static");
            btnSubmit.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnSubmit.Attributes.Add("onMouseOut", "this.className='static'");

            btnCancel.Attributes.Add("class", "static");
            btnCancel.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnCancel.Attributes.Add("onMouseOut", "this.className='static'");
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
                this.lblError.Text = "Problem in loading the outlets" + ex.Message;
                this.lblError.Visible = true;
            }
        }

        public void ClearFields()
        {
            ddlWareHouse.SelectedIndex = 0;
            lblOrderDate.Text = DateTime.Now.ToString();
            Session[AppConstants.OrderDate] = DateTime.Now;
            //Session[AppConstants.IsBackPage] = 0;
            lblUserName.Text = Session[Utils.AppConstants.UserCode].ToString().ToUpper();
            grvParentGrid.DataBind();
            lblError.Text = string.Empty;

            foreach (GridViewRow gvr in this.grvParentGrid.Rows)
            {
                CheckBoxList cblApprove = (CheckBoxList)gvr.FindControl("chkApprove");
                CheckBoxList cblReject = (CheckBoxList)gvr.FindControl("chkReject");
                CheckBoxList cblDeliveryCharge = (CheckBoxList)gvr.FindControl("chkAddDeliveryCharge");
                if (cblApprove != null)
                {
                    if (cblApprove.Items[0].Selected)
                    {
                        cblApprove.Items[0].Selected = false;
                    }
                }
                if (cblReject != null)
                {
                    if (cblReject.Items[0].Selected)
                    {
                        cblReject.Items[0].Selected = false;
                    }
                }
                if (cblDeliveryCharge != null)
                {
                    for (int i = 0; i < cblDeliveryCharge.Items.Count; i++)
                    {
                        if (cblDeliveryCharge.Items[i].Selected)
                        {
                            cblDeliveryCharge.Items[i].Selected = false;
                        }
                    }
                }
            }
        }

        public DataTable CreateTableForDeliveryCharge()
        {
            DataTable tbTemp = new DataTable();
            tbTemp.Columns.Add("PRNo");
            tbTemp.Columns.Add("SupplierCode");
            tbTemp.Columns.Add("SupplierName");
            tbTemp.Columns.Add("ItemCode");
            return tbTemp;
        }

        protected void chkApprove_SelectedIndexChnaged(object sender, System.EventArgs e)
        {

            foreach (GridViewRow gvr in this.grvParentGrid.Rows)
            {
                int i = 0;
                //DataTable dtItem = new DataTable();
                Label lblPRNo = (Label)gvr.FindControl("lblPRNo");
                CheckBoxList cbl = (CheckBoxList)gvr.FindControl("chkApprove");
                if (cbl.Items[i].Selected == true)
                {
                    DataTable dt = (DataTable)Session["tblApprove"];
                    DataRow rowNew = dt.NewRow();
                    rowNew["PRNo"] = lblPRNo.Text;
                    rowNew["Approve"] = cbl.Items[i].Selected;
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["PRNo"].ToString() == lblPRNo.Text)
                        {
                            dt.Rows.Remove(dr);
                            break;
                        }
                    }
                    dt.Rows.Add(rowNew);
                    Session["tblApprove"] = dt;
                }
                else
                {
                    DataTable dt = (DataTable)Session["tblApprove"];
                    DataRow[] ItemRows = dt.Select("PRNo ='" + lblPRNo.Text + "'");
                    if (ItemRows.Count() > 0)
                    {
                        //dtItem = ItemRows.CopyToDataTable();
                        ItemRows[0][1] = false;
                        Session["tblApprove"] = dt;
                    }
                }
                i = i + 1;
            }
        }

        protected void chkReject_SelectedIndexChnaged(object sender, System.EventArgs e)
        {
            foreach (GridViewRow gvr in this.grvParentGrid.Rows)
            {
                int i = 0;
                CheckBoxList cbl = (CheckBoxList)gvr.FindControl("chkApprove");
                if (cbl.Items[i].Selected == true)
                {
                    cbl.Items[i].Selected = false;
                }
                i = i + 1;
            }
        }

        protected void DisplayCalendar(CheckBoxList chkCalendar, CheckBoxList chkApprove, string PRNo)
        {

            DataTable dt = (DataTable)Session["ApprovalList"];
            DataTable dtList;

            DataTable prTable = (DataTable)dt.DefaultView.ToTable(true, "DocEntry", "DelChargeUDF");

            DataRow[] ItemRows = prTable.Select("DocEntry ='" + PRNo + "'");
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
                    rNew[AppConstants.Name] = "Add Delivery Charge if Minimum Spend not meet after Reduced Order Quantity";
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
                        chkCalendar.Enabled = false;

                    }
                    else
                    {
                        chkCalendar.Items[i].Selected = false;
                        chkCalendar.Enabled = true;
                    }
                }
            }

            if (chkApprove.Items.Count > 0)
            {
                DataTable dtNew = (DataTable)Session["tblApprove"];
                for (int j = 0; j < chkApprove.Items.Count; j++)
                {
                    if (dtNew != null && dtNew.Rows.Count > 0)
                    {
                        DataRow[] dr = dtNew.Select("PRNo ='" + PRNo + "'");
                        if (dr.Count() > 0)
                        {
                            dtList = dr.CopyToDataTable();
                            bool checkApprove = Convert.ToBoolean(dtList.Rows[0][1]);

                            if (checkApprove == true)
                            {
                                chkApprove.Items[j].Selected = true;
                            }
                            else
                            {
                                chkApprove.Items[j].Selected = false;
                            }
                        }
                    }
                }
            }
        }

        public DataSet returnPRDataSet(DataSet ds)
        {
            DataView oDVSelectedDrafts = new DataView();
            DataTable oDTFinal = new DataTable();
            DataSet oDSPRFinal = new DataSet();

            oDVSelectedDrafts = ds.Tables[0].DefaultView;

            oDVSelectedDrafts.RowFilter = "DocType='PR'";

            if (oDVSelectedDrafts.Count > 0)
            {
                oDTFinal = oDVSelectedDrafts.ToTable();

                oDSPRFinal.Tables.Add(oDTFinal.Copy());
            }


            oDVSelectedDrafts = ds.Tables[1].DefaultView;

            oDVSelectedDrafts.RowFilter = "DocType='PR'";

            if (oDVSelectedDrafts.Count > 0)
            {
                oDTFinal = oDVSelectedDrafts.ToTable();

                oDSPRFinal.Tables.Add(oDTFinal.Copy());
            }
            return oDSPRFinal;
        }

        public DataSet returnPQDataSet(DataSet ds)
        {
            DataView oDVSelectedDrafts = new DataView();
            DataTable oDTFinal = new DataTable();
            DataSet oDSPQFinal = new DataSet();

            oDVSelectedDrafts = ds.Tables[0].DefaultView;

            oDVSelectedDrafts.RowFilter = "DocType='PQ'";

            if (oDVSelectedDrafts.Count > 0)
            {
                oDTFinal = oDVSelectedDrafts.ToTable();

                oDSPQFinal.Tables.Add(oDTFinal.Copy());
            }


            oDVSelectedDrafts = ds.Tables[1].DefaultView;

            oDVSelectedDrafts.RowFilter = "DocType='PQ'";

            if (oDVSelectedDrafts.Count > 0)
            {
                oDTFinal = oDVSelectedDrafts.ToTable();

                oDSPQFinal.Tables.Add(oDTFinal.Copy());
            }
            return oDSPQFinal;
        }

        public string RemarksNullCheck(object oRemarks)
        {
            string sReturnResult = string.Empty;
            if (oRemarks != System.DBNull.Value) //oRemarks != "" || oRemarks != string.Empty || oRemarks != null)
            {
                sReturnResult = Convert.ToString(oRemarks);
            }
            return sReturnResult;
        }
    }
}