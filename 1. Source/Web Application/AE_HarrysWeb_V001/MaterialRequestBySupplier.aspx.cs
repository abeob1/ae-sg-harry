using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using AE_HarrysWeb_V001.MasterService;
using System.Data;
using AE_HarrysWeb_V001.Utils;

namespace AE_HarrysWeb_V001
{
    public partial class MaterialRequestBySupplier : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToString(Session[Utils.AppConstants.UserCode]) != string.Empty)
                {
                    if (!IsPostBack)
                    {
                        lblOrderDate.Text = DateTime.Now.ToShortDateString();
                        Session[AppConstants.OrderDate] = DateTime.Now;
                        Session[AppConstants.IsBackPage] = 0;
                        lblUserName.Text = Session[Utils.AppConstants.UserCode].ToString().ToUpper();
                        lblPriority.Text = AppConstants.Priority;
                        lblStatus.Text = AppConstants.DefaultStatus;
                        loadDropdowndata();
                        DataTable dt = (DataTable)Session["SupplierList"];
                        if (dt.Rows.Count != 0)
                        {
                            RemoveSupplier(dt);
                        }
                        if (ddlSupplier.SelectedValue.ToString() != " --- Select Supplier --- ")
                        {
                            LoadData(this.ddlSupplier.SelectedValue.ToString(), this.ddlWareHouse.SelectedValue.ToString(), Session[Utils.AppConstants.UserRole].ToString());
                            //DisplayCalendar();
                        }
                        else
                        {
                            grvMRBS.DataBind();
                        }
                        mouseOverandMouseOut();
                    }
                }
                else
                {
                    this.lblError.Text = "Logged in User was expired";
                    this.lblError.Visible = true;
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
                lblError.Visible = false;
                lblError.Text = string.Empty;
                DataTable dtSupplier = (DataTable)Session["SupplierList"];
                RemoveSupplier(dtSupplier);
                if (ddlWareHouse.Text == " --- Select Outlet --- ")
                {
                    clearFields();
                }
            }
            catch (Exception ex)
            {
                lblError.Visible = true;
                lblError.Text = ex.Message;
            }

        }

        protected void ddlSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.ddlSupplier.Text.Trim().Length > 0)
                {
                    //if (ddlSupplier.Text == " --- Select Supplier --- ")
                    //{
                    txtDeliveryDate.Text = string.Empty;
                    txtSearch.Text = string.Empty;
                    Session["singleDayCount"] = 0;
                    Session["TotalDays"] = 0;
                    //Session["noDeliveryDate"] = 0;
                    //}
                    lblError.Visible = false;
                    lblError.Text = string.Empty;
                    LoadData(this.ddlSupplier.SelectedValue.ToString(), this.ddlWareHouse.SelectedValue.ToString(), Session[Utils.AppConstants.UserRole].ToString());
                    if (Session["DelDateCount"].ToString() != "0")
                    {
                        txtDeliveryDate.Text = String.Format("{0:dd/MM/yyyy}", DateTime.Now.AddDays(Convert.ToInt32(Session["DelDateCount"])).Date);
                    }
                    //CalcDeliveryDate();
                    //int count = (int)Session["TotalDays"];
                    //if (count != 0)
                    //{
                    //    //this.txtDeliveryDate.Text = DateTime.Now.AddDays(count - 1).ToString();
                    //    this.txtDeliveryDate.Text = String.Format("{0:dd/MM/yyyy}", DateTime.Now.AddDays(count - 1).Date);
                    //}
                    //else
                    //{
                    //    //this.txtDeliveryDate.Text = string.Empty;
                    //}
                    //if (Convert.ToInt16(Session["singleDayCount"]) == 1)
                    //{
                    //    this.txtDeliveryDate.Text = String.Format("{0:dd/MM/yyyy}", DateTime.Now.AddDays(7).Date);
                    //}

                    //if (Convert.ToInt32(Session["noDeliveryDate"]) == 1)
                    //{
                    //    this.txtDeliveryDate.Text = string.Empty;

                    //}


                    CalcTotal((DataTable)Session["MRBSTable"]);
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = "Error Occured while Loading Suppliers" + ex.Message;
            }
        }

        protected void grvMRBS_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                this.grvMRBS.PageIndex = e.NewPageIndex;
                DataTable tblIMRBS = (DataTable)Session["MRBSTable"];
                BindData(tblIMRBS);
            }
            catch (Exception ex)
            {
                lblError.Visible = true;
                lblError.Text = ex.Message;
            }
        }

        protected void grvMRBS_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("onmouseover", "MouseEvents(this, event)");
                e.Row.Attributes.Add("onmouseout", "MouseEvents(this, event)");
                e.Row.Attributes["style"] = "cursor:pointer";
                int index = 0;
                TextBox ddl1 = e.Row.FindControl("txtOrderQuantity") as TextBox;
                TextBox ddl2 = e.Row.FindControl("txtRemarks") as TextBox;//

                if (Convert.ToString(Session["Remarks"]) == "txtRemarks")
                {
                    //if (e.Row.RowIndex > 0)
                    //{
                    //    TextBox txtNowQuantity = e.Row.FindControl("txtOrderQuantity") as TextBox;
                    //    TextBox txtPreviousQuantity = e.Row.FindControl("txtOrderQuantity") as TextBox;
                    //    txtPreviousQuantity.Attributes.Add("onkeyup", "FindNextRow('" + txtNowQuantity.ClientID + "');");
                    //}

                    ddl1.Attributes.Add("onfocus", "JavaScript:document.getElementById('" + ddl1.ClientID + "').select();");
                    if (ViewState["rowindex"] != null && Session["MRBSTable"] != null)
                    {
                        index = Convert.ToInt32(ViewState["rowindex"].ToString());
                        int rowCount = ((DataTable)Session["MRBSTable"]).Rows.Count;
                        if ((index + 1) <= this.grvMRBS.Rows.Count)
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
                    ddl2.Attributes.Add("onfocus", "JavaScript:document.getElementById('" + ddl2.ClientID + "').select();");
                    if (ViewState["rowindex"] != null && Session["MRBSTable"] != null)
                    {
                        index = Convert.ToInt32(ViewState["rowindex"].ToString());
                        int rowCount = ((DataTable)Session["MRBSTable"]).Rows.Count;
                        if ((index) <= this.grvMRBS.Rows.Count)
                        {
                            if (e.Row.RowIndex == index)
                            {
                                ddl2.Focus();
                            }
                        }
                    }
                }
            }
        }

        protected void grvMRBS_RowCreated(object sender, GridViewRowEventArgs e)
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

        protected void txtSearch_OnTextChanged(object sender, EventArgs e)
        {
            btnSearch_Click(this, new System.EventArgs());
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
            DataTable tb = (DataTable)Session["MRBSTable"];
            if (tb != null)
            {
                DataRow[] rupdate = tb.Select("ItemCode='" + lblItemCode.Text + "'");
                if (rupdate.Length > 0)
                {
                    rupdate[0]["OrderQuantity"] = txtQuantity.Text.Trim() == string.Empty ? "0" : txtQuantity.Text.Trim();
                    GetPrice(rupdate[0]);
                }
                this.grvMRBS.EditIndex = -1;
                BindData(tb);
                txtSearch.Text = string.Empty;
                CalcTotal(tb);
            }
        }

        protected void txtRemarks_OnTextChanged(object sender, EventArgs e)
        {
            GridViewRow row = (GridViewRow)((DataControlFieldCell)((TextBox)sender).Parent).Parent;
            ViewState["rowindex"] = row.RowIndex;
            Session["Remarks"] = "txtRemarks";
            TextBox txtRemaks = (TextBox)row.FindControl("txtRemarks");
            //if (txtQuantity.Text.Trim().Length == 0)
            //{
            //    ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", "alert('Pls input quantity');", true);
            //    txtQuantity.Focus();
            //    return;
            //}
            Label lblItemCode = (Label)row.FindControl("lblItemCode");
            DataTable tb = (DataTable)Session["MRBSTable"];
            if (tb != null)
            {
                DataRow[] rupdate = tb.Select("ItemCode='" + lblItemCode.Text + "'");
                if (rupdate.Length > 0)
                {
                    rupdate[0]["Remarks"] = txtRemaks.Text == string.Empty ? "" : txtRemaks.Text;
                    //GetPrice(rupdate[0]);
                }
                this.grvMRBS.EditIndex = -1;
                BindData(tb);
                txtSearch.Text = string.Empty;
                //CalcTotal(tb);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            var searchKey = txtSearch.Text;
            DataTable dt = (DataTable)Session["MRBSTable"];

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
                grvMRBS.DataSource = dt;
                grvMRBS.DataBind();
            }
            else
            {
                grvMRBS.DataSource = new DataTable();
                grvMRBS.DataBind();
            }
            txtSearch.Focus();
        }

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            try
            {
                LoadHeaderTableData();
                DataTable HeaderTable = (DataTable)Session["MRBSHeaderTable"];
                DataTable LineTable = (DataTable)Session["MRBSTable"];
                var result = LineTable.AsEnumerable().Sum(x => x["OrderQuantity"] == DBNull.Value ? 0 : Convert.ToDecimal(x["OrderQuantity"]));

                if ((HeaderTable != null && HeaderTable.Rows.Count > 0) && (LineTable != null && LineTable.Rows.Count > 0)
                    && txtDeliveryDate.Text != string.Empty && ddlWareHouse.Text != " --- Select Outlet --- " && ddlSupplier.Text != " --- Select Supplier --- ")
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
                            if (row["DeliveryDate"].ToString() == new DateTime().ToString())
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
                        var returnResult = string.Empty;

                        if (ddlSupplier.SelectedValue.Trim().ToUpper() == AppConstants.HoldingSupplier.Trim().ToUpper())
                        {
                            returnResult = data.Create_PurchaseRequest(NonZeroDataset, true, Session[AppConstants.U_DBName].ToString(), true, false, false);
                            NonZeroDataset.Dispose();
                        }
                        else
                        {
                            returnResult = data.Insert_PurchaseRequest(NonZeroDataset, true, Session[AppConstants.U_DBName].ToString(), true, false, false);
                            NonZeroDataset.Dispose();
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
                            lblError.Visible = true;
                            if (ddlSupplier.SelectedValue.Trim() != AppConstants.HoldingSupplier.Trim())
                            {
                                lblError.Text = "Material Request Draft Created Successfully.";
                            }
                            else
                            {
                                lblError.Text = "PQ Draft Created Successfully.";
                            }
                            clearFields();
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
                    else if (ddlSupplier.Text == " --- Select Supplier --- ")
                    {
                        lblError.Visible = true;
                        lblError.Text = "Please select the Supplier.";
                    }

                }
            }
            catch (Exception ex)
            {
                lblError.Visible = true;
                lblError.Text = ex.Message.ToString();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Session["Total"].ToString()) && !string.IsNullOrEmpty(Session["MinSpend"].ToString()))
                {
                    if (Convert.ToDouble(Session["Total"]) < Convert.ToDouble(Session["MinSpend"]))
                    {
                        lblError.Visible = true;
                        lblError.Text = "Total Spend is less than the minimum spend";
                    }
                    else
                    {
                        LoadHeaderTableData();
                        DataTable HeaderTable = (DataTable)Session["MRBSHeaderTable"];
                        DataTable LineTable = (DataTable)Session["MRBSTable"];
                        //HeaderTable.TableName = "tblHeader";
                        //LineTable.TableName = "tblLine";
                        foreach (DataRow row in LineTable.Rows)
                        {
                            if (row["DeliveryDate"].ToString() == new DateTime().ToString())
                            {
                                row["DeliveryDate"] = Convert.ToDateTime(txtDeliveryDate.Text).Date;
                            }
                        }
                        DataSet ds = new DataSet();
                        ds.Tables.Add(HeaderTable);
                        ds.Tables.Add(LineTable);
                        var data = new MasterService.MasterSoapClient();
                        var returnResult = data.Create_PurchaseRequest(ds, false, Session[AppConstants.U_DBName].ToString(), true, false, false);
                        if (returnResult != "SUCCESS")
                        {
                            lblError.Visible = true;
                            lblError.Text = returnResult;
                        }
                        else
                        {
                            lblError.Visible = true;
                            lblError.Text = "Document Created Successfully.";
                        }
                        ds.Tables.Remove(HeaderTable);
                        ds.Tables.Remove(LineTable);
                        ds.Clear();
                    }
                }
                else
                {
                    lblError.Text = "Total Spend is less than the minimum spend";
                }
            }
            catch (Exception ex)
            {
                lblError.Visible = true;
                lblError.Text = ex.Message.ToString();
            }

        }

        private void LoadHeaderTableData()
        {
            DataTable tblIMRBSHeader = CreateHeaderTable();
            DataRow rowNew = tblIMRBSHeader.NewRow();
            rowNew["Id"] = 1;
            rowNew["PostingDate"] = Convert.ToDateTime(Session[AppConstants.OrderDate]).Date;
            rowNew["Outlet"] = ddlWareHouse.SelectedValue.ToString(); ;
            rowNew["UserCode"] = Session[Utils.AppConstants.UserName].ToString().ToUpper().Trim();
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
            if (chkPriority.Checked == true) { rowNew["Urgent"] = AppConstants.Yes; } else { rowNew["Urgent"] = AppConstants.No; }
            rowNew["OrderTime"] = Convert.ToDateTime(Session[AppConstants.OrderDate]).TimeOfDay.Hours + ":" + Convert.ToDateTime(Session[AppConstants.OrderDate]).TimeOfDay.Minutes + " " + Convert.ToDateTime(Session[AppConstants.OrderDate]).ToString("tt");
            rowNew["UserId"] = Convert.ToInt16(Session[AppConstants.UserID]);
            tblIMRBSHeader.Rows.Add(rowNew);
            Session["MRBSHeaderTable"] = tblIMRBSHeader;
        }

        private void loadDropdowndata()
        {
            var data = new MasterService.MasterSoapClient();
            DataSet dsOutlet = data.Get_Outlet_Details(Session[Utils.AppConstants.SUPERUSER].ToString(), Session[Utils.AppConstants.WhsCode].ToString(),
                Convert.ToInt16(Session[AppConstants.ApprovalLevel]), Session[AppConstants.U_ConnString].ToString());
            if (dsOutlet.Tables.Count != 0 && dsOutlet != null)
            {
                ddlWareHouse.DataSource = dsOutlet.Tables[0];
                ddlWareHouse.DataTextField = dsOutlet.Tables[0].Columns[AppConstants.WhsName].ColumnName.ToString();
                ddlWareHouse.DataValueField = dsOutlet.Tables[0].Columns[AppConstants.WhsCode].ColumnName.ToString();
                ddlWareHouse.DataBind();
            }

            DataSet dsSupplier = data.Get_Supplier_Details(Session[AppConstants.UserRole].ToString(), Session[AppConstants.U_ConnString].ToString());
            if (dsSupplier.Tables.Count != 0 && dsSupplier != null)
            {
                Session["SupplierList"] = dsSupplier.Tables[0];
                ddlSupplier.DataSource = dsSupplier.Tables[0];
                ddlSupplier.DataTextField = dsSupplier.Tables[0].Columns[AppConstants.CardName].ColumnName.ToString();
                ddlSupplier.DataValueField = dsSupplier.Tables[0].Columns[AppConstants.CardCode].ColumnName.ToString();
                ddlSupplier.DataBind();
            }
        }

        private void DisplayCalendar(DataSet ds)
        {
            try
            {
                //Load Calendar 
                //var objC = new MasterService.MasterSoapClient();
                //DataSet ds = objC.Get_MaterialReqBySupplier(this.ddlSupplier.SelectedValue.ToString(), this.ddlWareHouse.SelectedValue.ToString(),
                //    Session[Utils.AppConstants.UserRole].ToString(), Session[AppConstants.U_ConnString].ToString());
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    this.chkCalendar.Items.Clear();
                    Session["MinSpend"] = ds.Tables[0].Rows[0][AppConstants.MinSpend];
                    this.lblMinSpend.Text = string.Format("${0}", Convert.ToDouble(ds.Tables[0].Rows[0][AppConstants.MinSpend]).ToString(Utils.AppConstants.NUMBER_FORMAT));
                    var CurrentRow = ds.Tables[0].Rows[0];
                    DataTable tbNew = new DataTable();
                    tbNew.Columns.Add(AppConstants.Name);
                    tbNew.Columns.Add(AppConstants.Value);
                    if (CurrentRow.Table.Columns.Count > 0)
                    {
                        int colIndex = CurrentRow.Table.Columns.Count - 7;
                        for (int i = colIndex; i < CurrentRow.Table.Columns.Count; i++)
                        {
                            DataRow rNew = tbNew.NewRow();
                            rNew[AppConstants.Name] = CurrentRow.Table.Columns[i].ColumnName;
                            rNew[AppConstants.Value] = CurrentRow.ItemArray[i];
                            tbNew.Rows.Add(rNew);
                        }
                    }

                    this.chkCalendar.DataSource = tbNew;
                    this.chkCalendar.DataTextField = AppConstants.Name;
                    this.chkCalendar.DataValueField = AppConstants.Value;
                    this.chkCalendar.DataBind();
                    if (this.chkCalendar.Items.Count > 0)
                    {
                        int singleDayCount = 0;
                        string day = string.Empty;
                        int count = 0; int firstday = 0; int secondDay = 0;
                        for (int i = 0; i < this.chkCalendar.Items.Count; i++)
                        {
                            if (this.chkCalendar.Items[i].Value.ToString() == AppConstants.Yes)
                            {
                                if (DateTime.Now.DayOfWeek.ToString().Substring(0, 3) == chkCalendar.Items[i].Text)
                                {
                                    singleDayCount = singleDayCount + 1;
                                    Session["singleDayCount"] = singleDayCount;
                                }

                                this.chkCalendar.Items[i].Selected = true;

                            }
                            //else
                            //{
                            //    Session["noDeliveryDate"] = i;
                            //}
                            if (DateTime.Now.DayOfWeek.ToString().Substring(0, 3) == chkCalendar.Items[i].Text)
                            {
                                for (int k = i; k <= 6; k++)
                                {
                                    if (this.chkCalendar.Items[k].Value.ToString() == AppConstants.Yes)
                                    {
                                        if (firstday == 0)
                                        {
                                            secondDay = secondDay + 1;
                                            Session[AppConstants.DayName] = chkCalendar.Items[k].Text;
                                            firstday = firstday + 1;
                                        }
                                        else if (firstday > 0)
                                        {
                                            count = count + 1;
                                            Session[AppConstants.NextDayName] = chkCalendar.Items[k].Text;
                                            break;
                                        }
                                        //break;
                                    }
                                }
                                if (count == 0)
                                {
                                    for (int m = 0; m < i; m++)
                                    {
                                        if (this.chkCalendar.Items[m].Value.ToString() == AppConstants.Yes)
                                        {
                                            if (secondDay == 0)
                                            {
                                                Session[AppConstants.DayName] = chkCalendar.Items[m].Text;
                                                secondDay = secondDay + 1;
                                            }
                                            else if (secondDay > 0)
                                            {
                                                Session[AppConstants.NextDayName] = chkCalendar.Items[m].Text;
                                                break;
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 7; i++)
                    {
                        this.chkCalendar.Items[i].Selected = false;
                    }
                    txtDeliveryDate.Text = string.Empty;
                    Session[AppConstants.DayName] = string.Empty;
                    Session[AppConstants.NextDayName] = string.Empty;
                    Session["TotalDays"] = 0;
                    Session["singleDayCount"] = 0;
                    //Session["noDeliveryDate"] = 0;
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
                string nextDay = string.Empty;
                if (Session[AppConstants.DayName] != null && Session[AppConstants.DayName].ToString() != "")
                {
                    for (int i = 0; i <= 6; i++)
                    {
                        string dayOfWeek = string.Empty;
                        nextDay = Convert.ToString(Session[AppConstants.NextDayName]);
                        string sessionValue = (string)Session[AppConstants.DayName].ToString();
                        if (i == 0)
                        {
                            dayOfWeek = DateTime.Now.DayOfWeek.ToString();
                        }
                        else
                        {
                            dayOfWeek = DateTime.Now.AddDays(i).DayOfWeek.ToString();
                        }

                        if (i == 0 && dayOfWeek.Substring(0, 3).ToUpper().ToString() == sessionValue.ToUpper().ToString())
                        {

                            for (int d = 0; d <= 6; d++)
                            {
                                if (d == 0)
                                {
                                    dayOfWeek = DateTime.Now.DayOfWeek.ToString();
                                }
                                else
                                {
                                    dayOfWeek = DateTime.Now.AddDays(d).DayOfWeek.ToString();
                                }

                                if (dayOfWeek.Substring(0, 3).ToUpper().ToString() == nextDay.ToUpper().ToString())
                                {
                                    Session["TotalDays"] = d + 1;
                                    return;
                                }
                            }
                        }
                        else
                        {
                            if (dayOfWeek.Substring(0, 3).ToUpper().ToString() == sessionValue.ToUpper().ToString())
                            {
                                Session["TotalDays"] = i + 1;
                                break;
                            }
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

        private void BindData(DataTable tblData)
        {
            Session["MRBSTable"] = tblData;

            DataView dv = tblData.DefaultView;

            dv.Sort = "Description  ASC";

            this.grvMRBS.DataSource = dv.ToTable();
            this.grvMRBS.DataBind();
        }

        private DataTable CreateTableFormat()
        {
            DataTable tbTemp = new DataTable();
            tbTemp.Columns.Add("No");
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
            tbTemp.Columns.Add("SupplierCode");
            tbTemp.Columns.Add("DeliveryDate");
            tbTemp.Columns.Add("Remarks");
            tbTemp.Columns.Add("ApproverRemarks");
            tbTemp.Columns.Add("ItemPerUnit");
            return tbTemp;
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

        private void LoadData(string supplier, string outlet, string userRole)
        {
            try
            {
                DataTable tblIMRBS = CreateTableFormat();
                var objC = new MasterService.MasterSoapClient();
                DataSet ds = objC.Get_MaterialReqBySupplier(supplier, outlet, userRole, Session[AppConstants.U_ConnString].ToString());
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    int i = 1;
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        DataRow rowNew = tblIMRBS.NewRow();
                        rowNew["No"] = i;
                        rowNew["ItemCode"] = row["ItemCode"];
                        rowNew["Description"] = row["ItemName"];
                        rowNew["InStock"] = row["OnHand"];
                        rowNew["EventOrder"] = row["OnOrder"];
                        rowNew["Last7DaysAvg"] = row["Last 7 Days"];
                        rowNew["AlreadyOrdered"] = row["AlreadyOrdered"];
                        rowNew["MinStock"] = row["MinLevel"];
                        rowNew["UOM"] = row["UOM"];
                        rowNew["RecommendedQuantity"] = row["RecommendedQuantity"];
                        rowNew["Price"] = row["Price"];
                        rowNew["MinSpend"] = row["MinSpend"];
                        //rowNew["Vendor"] = ddlSupplier.SelectedValue.ToString();
                        rowNew["SupplierCode"] = ddlSupplier.SelectedValue.ToString();
                        Session["DelDateCount"] = row["DelDate"];
                        if (txtDeliveryDate.Text != "" && txtDeliveryDate.Text != string.Empty && txtDeliveryDate != null)
                        {
                            rowNew["DeliveryDate"] = Convert.ToDateTime(txtDeliveryDate.Text).Date;
                        }
                        else
                        { rowNew["DeliveryDate"] = new DateTime(); }
                        rowNew["Remarks"] = "";
                        rowNew["ApproverRemarks"] = "";

                        rowNew["ItemPerUnit"] = row["ItemPerUnit"];
                        tblIMRBS.Rows.Add(rowNew);
                        i++;
                    }
                    Session["MRBSTable"] = tblIMRBS;
                    DataView dv = tblIMRBS.DefaultView;
                    dv.Sort = "Description ASC";
                    this.grvMRBS.DataSource = dv.ToTable();
                    this.grvMRBS.DataBind();
                    DisplayCalendar(ds);
                    CalcDeliveryDate();
                }
                else
                {
                    Session["MRBSTable"] = tblIMRBS;
                    Session["DelDateCount"] = 0;
                    DataView dv = tblIMRBS.DefaultView;
                    this.grvMRBS.DataSource = dv.ToTable();
                    this.grvMRBS.DataBind();
                    DisplayCalendar(ds);
                    CalcDeliveryDate();
                }
            }
            catch (Exception)
            {
                throw;
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
                    Session["Total"] = Total;
                    this.lblGrandTotal.Text = string.Format("${0}", Total.ToString(Utils.AppConstants.NUMBER_FORMAT));
                    this.lblTotalOutlet.Text = this.lblGrandTotal.Text;
                }
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

            btnSearch.Attributes.Add("class", "static");
            btnSearch.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnSearch.Attributes.Add("onMouseOut", "this.className='static'");
        }

        public void clearFields()
        {
            lblOrderDate.Text = DateTime.Now.ToString();
            Session[AppConstants.OrderDate] = lblOrderDate.Text;
            txtDeliveryDate.Text = string.Empty;
            txtSearch.Text = string.Empty;
            lblMinSpend.Text = string.Empty;
            lblTotalOutlet.Text = string.Empty;
            chkPriority.Checked = false;
            ddlWareHouse.SelectedIndex = 0;
            ddlSupplier.SelectedIndex = 0;
            for (int i = 0; i < this.chkCalendar.Items.Count; i++)
            {
                if (this.chkCalendar.Items[i].Selected == true)
                {
                    this.chkCalendar.Items[i].Selected = false;
                }
            }
            lblGrandTotal.Text = string.Empty;
            grvMRBS.DataBind();
        }

        public void RemoveSupplier(DataTable dtSupplier)
        {
            if (ddlWareHouse.SelectedValue == "01CKT")
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

        public DataSet FetchNotNullValues(DataSet ds)
        {
            DataTable dt = ds.Tables[1];
            dt.DefaultView.RowFilter = "OrderQuantity > 0";
            dt = dt.DefaultView.ToTable();
            ds.Tables.Remove(ds.Tables[1]);
            ds.Tables.Add(dt.Copy());
            return ds;
        }
    }
}