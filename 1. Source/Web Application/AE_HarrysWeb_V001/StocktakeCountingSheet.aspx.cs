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
    public partial class StocktakeCounting : System.Web.UI.Page
    {
        public static decimal totalValue = 0;
        public static string sItemDescription = string.Empty;


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session[Utils.AppConstants.UserCode].ToString() != string.Empty)
                {
                    if (!IsPostBack)
                    {
                        Session[AppConstants.IsBackPage] = 0;
                        lblUserName.Text = Session[Utils.AppConstants.UserCode].ToString().ToUpper();
                        lblStatus.Text = AppConstants.DefaultStatus;
                        loadDropdowndata();
                        if (Convert.ToString(Session[AppConstants.WhsCode_countingSheet]) != "" && Convert.ToString(Session[AppConstants.WhsCode_countingSheet]) != null)
                        {
                            lblStatus.Text = Session[AppConstants.Status_countingSheet].ToString();
                            ddlWareHouse.SelectedValue = Session[AppConstants.WhsCode_countingSheet].ToString();
                            txtCountDate.Text = Session[AppConstants.CountDate_countingSheet].ToString();
                            Session[AppConstants.Status_countingSheet] = string.Empty;
                            Session[AppConstants.WhsCode_countingSheet] = string.Empty;
                            Session[AppConstants.CountDate_countingSheet] = string.Empty;
                            string sreturn = LoadData(ddlWareHouse.SelectedValue);
                            if (sreturn == "")
                            {
                                if (totalValue == 0)
                                {
                                    btnSubmit.Enabled = false;
                                    btnSubmit.Style["background-image"] = "url('Images/bgButton_disable.png')";
                                }
                            }
                            else
                            {
                                btnSaveDraft.Enabled = false;
                                btnSaveDraft.Style["background-image"] = "url('Images/bgButton_disable.png')";
                                btnSubmit.Enabled = false;
                                btnSubmit.Style["background-image"] = "url('Images/bgButton_disable.png')";
                            }
                        }
                        //string sreturn = LoadData(this.ddlWareHouse.SelectedValue.ToString());
                        //if (sreturn == "")
                        //{
                        if (totalValue == 0)
                        {
                            btnSubmit.Enabled = false;
                            btnSubmit.Style["background-image"] = "url('Images/bgButton_disable.png')";
                        }
                        mouseOverandMouseOut();
                        grvStkCnt.DataBind();
                        UpdateSessionTable();
                        //}
                        //else
                        //{
                        //btnSaveDraft.Enabled = false;
                        //btnSaveDraft.Style["background-image"] = "url('Images/bgButton_disable.png')";
                        //btnSubmit.Enabled = false;
                        //btnSubmit.Style["background-image"] = "url('Images/bgButton_disable.png')";
                        //}
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

        private void loadDropdowndata()
        {
            try
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

        //protected void txtCountedUOM1_OnTextChanged(object sender, EventArgs e)
        //{
        //    lblError.Text = string.Empty;
        //    GridViewRow row = (GridViewRow)((DataControlFieldCell)((TextBox)sender).Parent).Parent;
        //    ViewState["rowindex"] = row.RowIndex;
        //    TextBox txtCountedUOM1 = (TextBox)row.FindControl("txtCountedUOM1");
        //    Label lblStockTakeCon1 = (Label)row.FindControl("lblStockTakeCon1");

        //    TextBox txtCountedUOM2 = (TextBox)row.FindControl("txtCountedUOM2");
        //    Label lblStockTakeCon2 = (Label)row.FindControl("lblStockTakeCon2");

        //    if (txtCountedUOM1.Text != null && txtCountedUOM1.Text != "")
        //    {
        //        totalValue = totalValue + Convert.ToDecimal(txtCountedUOM1.Text);
        //        btnSubmit.Enabled = true;
        //        btnSubmit.Style["background-image"] = "url('Images/bgButton.png')";
        //    }
        //    Label lblItemCode = (Label)row.FindControl("lblItemCode");
        //    Label lblInStock = (Label)row.FindControl("lblInStock");
        //    Label lblTotalUOM1 = (Label)row.FindControl("lblTotalUOM1");
        //    Label lblTotalUOM2 = (Label)row.FindControl("lblTotalUOM2");
        //    if (txtCountedUOM1.Text != string.Empty)
        //    {
        //        lblTotalUOM1.Text = (double.Parse(txtCountedUOM1.Text) * double.Parse(lblStockTakeCon1.Text)).ToString();
        //    }
        //    else
        //    {
        //        lblTotalUOM1.Text = "0";
        //    }
        //    if (txtCountedUOM2.Text != string.Empty)
        //    {
        //        lblTotalUOM2.Text = (double.Parse(txtCountedUOM2.Text) * double.Parse(lblStockTakeCon2.Text)).ToString();
        //    }
        //    else
        //    {
        //        lblTotalUOM2.Text = "0";
        //    }
        //    DataTable tb = (DataTable)Session["STCS"];
        //    if (tb != null)
        //    {
        //        DataRow[] rupdate = tb.Select("ItemCode='" + lblItemCode.Text + "'");
        //        if (rupdate.Length > 0)
        //        {
        //            rupdate[0]["CountedUOM1"] = txtCountedUOM1.Text;
        //            rupdate[0]["TotalUOM1"] = lblTotalUOM1.Text;

        //            rupdate[0]["CountedUOM2"] = txtCountedUOM2.Text;
        //            rupdate[0]["TotalUOM2"] = lblTotalUOM2.Text;

        //            rupdate[0]["CountedInvUOM"] = Convert.ToDouble(rupdate[0]["TotalUOM1"]) + Convert.ToDouble(rupdate[0]["TotalUOM2"]);
        //            rupdate[0]["Variance"] = Convert.ToDouble(rupdate[0]["CountedInvUOM"]) - Convert.ToDouble(lblInStock.Text);
        //        }

        //    }
        //    this.grvStkCnt.EditIndex = -1;
        //    BindData(tb);
        //    btnSubmit.Enabled = false;
        //    btnSubmit.Style["background-image"] = "url('Images/bgButton_disable.png')";
        //    lblError.Text = string.Empty;
        //}

        //protected void txtCountedUOM2_OnTextChanged(object sender, EventArgs e)
        //{
        //    lblError.Text = string.Empty;
        //    GridViewRow row = (GridViewRow)((DataControlFieldCell)((TextBox)sender).Parent).Parent;
        //    ViewState["rowindex"] = row.RowIndex;
        //    Session["CountUOM2"] = "txtCountUOM2";
        //    TextBox txtCountedUOM1 = (TextBox)row.FindControl("txtCountedUOM1");
        //    Label lblStockTakeCon1 = (Label)row.FindControl("lblStockTakeCon1");

        //    TextBox txtCountedUOM2 = (TextBox)row.FindControl("txtCountedUOM2");
        //    Label lblStockTakeCon2 = (Label)row.FindControl("lblStockTakeCon2");

        //    if (txtCountedUOM2.Text != null && txtCountedUOM2.Text != "")
        //    {
        //        totalValue = totalValue + Convert.ToDecimal(txtCountedUOM2.Text);
        //        btnSubmit.Enabled = true;
        //        btnSubmit.Style["background-image"] = "url('Images/bgButton.png')";
        //    }
        //    Label lblItemCode = (Label)row.FindControl("lblItemCode");
        //    Label lblInStock = (Label)row.FindControl("lblInStock");
        //    Label lblTotalUOM1 = (Label)row.FindControl("lblTotalUOM1");
        //    Label lblTotalUOM2 = (Label)row.FindControl("lblTotalUOM2");
        //    if (txtCountedUOM1.Text != string.Empty)
        //    {
        //        lblTotalUOM1.Text = (double.Parse(txtCountedUOM1.Text) * double.Parse(lblStockTakeCon1.Text)).ToString();
        //    }
        //    else
        //    {
        //        lblTotalUOM1.Text = "0";
        //    }
        //    if (txtCountedUOM2.Text != string.Empty)
        //    {
        //        lblTotalUOM2.Text = (double.Parse(txtCountedUOM2.Text) * double.Parse(lblStockTakeCon2.Text)).ToString();
        //    }
        //    else
        //    {
        //        lblTotalUOM2.Text = "0";
        //    }
        //    DataTable tb = (DataTable)Session["STCS"];
        //    if (tb != null)
        //    {
        //        DataRow[] rupdate = tb.Select("ItemCode='" + lblItemCode.Text + "'");
        //        if (rupdate.Length > 0)
        //        {

        //            rupdate[0]["CountedUOM1"] = txtCountedUOM1.Text;
        //            rupdate[0]["TotalUOM1"] = lblTotalUOM1.Text;

        //            rupdate[0]["CountedUOM2"] = txtCountedUOM2.Text;
        //            rupdate[0]["TotalUOM2"] = lblTotalUOM2.Text;

        //            rupdate[0]["CountedInvUOM"] = Convert.ToDouble(rupdate[0]["TotalUOM1"]) + Convert.ToDouble(rupdate[0]["TotalUOM2"]);
        //            rupdate[0]["Variance"] = Convert.ToDouble(rupdate[0]["CountedInvUOM"]) - Convert.ToDouble(lblInStock.Text);
        //        }

        //    }
        //    this.grvStkCnt.EditIndex = -1;
        //    BindData(tb);
        //    btnSubmit.Enabled = false;
        //    btnSubmit.Style["background-image"] = "url('Images/bgButton_disable.png')";
        //    lblError.Text = string.Empty;
        //}

        private string LoadData(string sOutlet)
        {
            try
            {
                DataTable tblSTCS = CreateTableFormat();
                var data = new MasterService.MasterSoapClient();
                //Calling the webmethod
                if (sOutlet == " --- Select Outlet --- ")
                {
                    txtCountDate.Text = string.Empty;
                    this.grvStkCnt.DataBind();
                    return "";
                }
                else
                {
                    string sLabelStatus = "Draft,Pending Approval,Approved,Draft Recount";
                    //DataSet ds = data.Get_StockTakeCounting(Session[AppConstants.DBName].ToString(), sOutlet, Session[AppConstants.UserRole].ToString(), lblStatus.Text, string.Empty);
                    DataSet ds = data.Get_StockTakeCountingDetails(Session[AppConstants.DBName].ToString(), sOutlet
                                , Session[AppConstants.UserRole].ToString(), sLabelStatus, string.Empty
                                , Session[AppConstants.UserID].ToString(), Session[AppConstants.UserName].ToString(), txtCountDate.Text);

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        var sStatus = ds.Tables[0].Rows[0]["StockTakeStatus"].ToString().Trim();

                        if (sStatus.ToString().ToUpper() == "APPROVED" || sStatus.ToString().ToUpper() == "PENDING APPROVAL")
                        {
                            lblError.Visible = true;
                            lblError.Text = "StockTake Counting Sheet Already Created for Today.";
                            return lblError.Text;
                        }

                        int i = 1;
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            DataRow rowNew = tblSTCS.NewRow();
                            rowNew["No"] = i;
                            rowNew["DocEntry"] = row["DocEntry"];
                            rowNew["CardCode"] = row["CardCode"];
                            rowNew["CardName"] = row["CardName"];
                            rowNew["ItemCode"] = row["ItemCode"];
                            rowNew["Description"] = row["Description"];
                            rowNew["LineNum"] = row["LineNum"];
                            rowNew["InStock"] = row["InStock"];
                            rowNew["Variance"] = row["Variance"];

                            //rowNew["StockTakeUoM"] = row["Stocktake UoM"];
                            //if (Convert.ToDouble(row["Counted"]) == 0)
                            //{ }
                            //else
                            //{ rowNew["Counted"] = row["Counted"]; }
                            //rowNew["StockTakeConv"] = row["StockTakeConv"];
                            //rowNew["Adjust in Inventory UoM"] = row["Adjust In InventoryUOM"];

                            txtCountDate.Text = row["DocDate"].ToString();


                            rowNew["StockTakeUoM1"] = row["StockTakeUoM1"];
                            rowNew["StockTakeCon1"] = row["StockTakeCon1"];
                            rowNew["CountedUOM1"] = row["CountedUOM1"];
                            rowNew["TotalUOM1"] = row["TotalUOM1"];
                            rowNew["StockTakeUOM2"] = row["StockTakeUOM2"];
                            rowNew["StockTakeCon2"] = row["StockTakeCon2"];
                            rowNew["CountedUOM2"] = row["CountedUOM2"];
                            rowNew["TotalUOM2"] = row["TotalUOM2"];
                            rowNew["InventoryUOM"] = row["InventoryUOM"];
                            rowNew["CountedInvUOM"] = row["CountedInvUOM"];
                            rowNew["DocDate"] = row["DocDate"];
                            rowNew["WhsName"] = row["WhsName"];


                            tblSTCS.Rows.Add(rowNew);
                            i++;
                        }
                        Session["STCS"] = tblSTCS;
                        DataView dv = tblSTCS.DefaultView;
                        //dv.Sort = "Description ASC";
                        int ApprovalLevel;
                        ApprovalLevel = Convert.ToInt32(Session[AppConstants.ApprovalLevel]);
                        if (ApprovalLevel == 0)
                        {
                            this.grvStkCnt.Columns[12].Visible = false;
                            this.grvStkCnt.Columns[13].Visible = false;
                            this.grvStkCnt.Columns[14].Visible = false;
                            this.grvStkCnt.Columns[15].Visible = false;
                            this.grvStkCnt.Columns[16].Visible = true;
                            this.grvStkCnt.Columns[17].Visible = true;
                            this.grvStkCnt.Columns[18].Visible = true;
                            this.grvStkCnt.Columns[19].Visible = true;
                        }
                        this.grvStkCnt.DataSource = dv.ToTable();
                        this.grvStkCnt.DataBind();
                        return "";
                    }
                    else
                    {
                        mStkPopup.Show();
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
                return "";
            }
        }

        private DataTable CreateTableFormat()
        {
            DataTable tbTemp = new DataTable();
            tbTemp.Columns.Add("No");
            tbTemp.Columns.Add("DocEntry");
            tbTemp.Columns.Add("CardCode");
            tbTemp.Columns.Add("CardName");
            tbTemp.Columns.Add("ItemCode");
            tbTemp.Columns.Add("Description");
            tbTemp.Columns.Add("LineNum");
            tbTemp.Columns.Add("InStock");
            tbTemp.Columns.Add("Variance");

            //tbTemp.Columns.Add("StockTakeUoM");
            //tbTemp.Columns.Add("Counted");
            //tbTemp.Columns.Add("StockTakeConv");
            //tbTemp.Columns.Add("Adjust in Inventory UoM");

            tbTemp.Columns.Add("StockTakeUoM1");
            tbTemp.Columns.Add("StockTakeCon1");
            tbTemp.Columns.Add("CountedUOM1");
            tbTemp.Columns.Add("TotalUOM1");
            tbTemp.Columns.Add("StockTakeUOM2");
            tbTemp.Columns.Add("StockTakeCon2");
            tbTemp.Columns.Add("CountedUOM2");
            tbTemp.Columns.Add("TotalUOM2");
            tbTemp.Columns.Add("InventoryUOM");
            tbTemp.Columns.Add("CountedInvUOM");
            tbTemp.Columns.Add("DocDate");
            tbTemp.Columns.Add("WhsName");
            //tbTemp.Columns.Add("IsModified");

            return tbTemp;
        }

        protected void ddlWareHouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.ddlWareHouse.Text.Trim().Length > 0)
                {
                    btnSaveDraft.Enabled = true;
                    btnSaveDraft.Style["background-image"] = "url('Images/bgButton.png')";
                    lblError.Visible = false;
                    lblError.Text = string.Empty;
                    txtCountDate.Text = string.Empty;
                    this.grvStkCnt.DataSource = null;
                    this.grvStkCnt.DataBind();
                    lblError.Text = string.Empty;
                    //string sReturn = LoadData(this.ddlWareHouse.SelectedValue.ToString());
                    //if (sReturn == "")
                    //{
                    if (totalValue == 0)
                    {
                        btnSubmit.Enabled = false;
                        btnSubmit.Style["background-image"] = "url('Images/bgButton_disable.png')";
                    }
                    //}
                    //else
                    //{
                    //    btnSaveDraft.Enabled = false;
                    //    btnSaveDraft.Style["background-image"] = "url('Images/bgButton_disable.png')";
                    //    btnSubmit.Enabled = false;
                    //    btnSubmit.Style["background-image"] = "url('Images/bgButton_disable.png')";
                    //}
                }
                else
                {
                    grvStkCnt.DataBind();
                    btnSubmit.Enabled = false;
                    btnSubmit.Style["background-image"] = "url('Images/bgButton_disable.png')";
                }
            }

            catch (Exception ex)
            {
                Response.Redirect(AppConstants.LoginURL);
                throw ex;
            }
        }

        //protected void grvStkCnt_OnRowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Row.RowType == DataControlRowType.DataRow)
        //        {
        //            e.Row.Attributes.Add("onmouseover", "MouseEvents(this, event)");
        //            e.Row.Attributes.Add("onmouseout", "MouseEvents(this, event)");
        //            e.Row.Attributes["style"] = "cursor:pointer";
        //            int index = 0;
        //            TextBox txt1 = e.Row.FindControl("txtCountedUOM1") as TextBox;
        //            TextBox txt2 = e.Row.FindControl("txtCountedUOM2") as TextBox;

        //            if (Convert.ToString(Session["CountUOM2"]) == "txtCountUOM2")
        //            {
        //                txt1.Attributes.Add("onfocus", "JavaScript:document.getElementById('" + txt1.ClientID + "').select();");
        //                if (txt2.Text != null && txt2.Text != "" && txt2.Text != "0.000000")
        //                {
        //                    totalValue = totalValue + Convert.ToDecimal(txt2.Text);
        //                    btnSubmit.Enabled = true;
        //                    btnSubmit.Style["background-image"] = "url('Images/bgButton.png')";
        //                }
        //                if (ViewState["rowindex"] != null && Session["STCS"] != null)
        //                {
        //                    index = Convert.ToInt32(ViewState["rowindex"].ToString());
        //                    int rowCount = ((DataTable)Session["STCS"]).Rows.Count;
        //                    if ((index + 1) <= this.grvStkCnt.Rows.Count)
        //                    {
        //                        if (e.Row.RowIndex == index + 1)
        //                        {
        //                            txt1.Focus();
        //                            Session["CountUOM2"] = string.Empty;
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                txt2.Attributes.Add("onfocus", "JavaScript:document.getElementById('" + txt2.ClientID + "').select();");
        //                if (txt1.Text != null && txt1.Text != "" && txt1.Text != "0.000000")
        //                {
        //                    totalValue = totalValue + Convert.ToDecimal(txt1.Text);
        //                    btnSubmit.Enabled = true;
        //                    btnSubmit.Style["background-image"] = "url('Images/bgButton.png')";
        //                }
        //                if (ViewState["rowindex"] != null && Session["STCS"] != null)
        //                {
        //                    index = Convert.ToInt32(ViewState["rowindex"].ToString());
        //                    int rowCount = ((DataTable)Session["STCS"]).Rows.Count;
        //                    if ((index) <= this.grvStkCnt.Rows.Count)
        //                    {
        //                        if (e.Row.RowIndex == index)
        //                        {
        //                            txt2.Focus();
        //                        }
        //                    }
        //                }
        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        this.lblError.Text = ex.Message;
        //        this.lblError.Visible = true;

        //    }


        //}

        protected void grvStkCnt_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            var var1 = e.NewPageIndex;
            var var2 = this.grvStkCnt.PageIndex;

            UpdateSessionTable();

            this.grvStkCnt.PageIndex = e.NewPageIndex;
            DataTable tblSTCS = (DataTable)Session["STCS"];
            BindData(tblSTCS);
        }

        public void UpdateSessionTable()
        {
            foreach (GridViewRow row in grvStkCnt.Rows)
            {
                TextBox txtCountedUOM1 = (TextBox)row.FindControl("txtCountedUOM1");
                TextBox txtCountedUOM2 = (TextBox)row.FindControl("txtCountedUOM2");
                Label lblStockTakeCon1 = (Label)row.FindControl("lblStockTakeCon1");
                Label lblStockTakeCon2 = (Label)row.FindControl("lblStockTakeCon2");
                Label lblTotalUOM1 = (Label)row.FindControl("lblTotalUOM1");
                Label lblTotalUOM2 = (Label)row.FindControl("lblTotalUOM2");
                Label lblItemCode = (Label)row.FindControl("lblItemCode");
                Label lblInStock = (Label)row.FindControl("lblInStock");
                if (txtCountedUOM1.Text != string.Empty)
                {
                    lblTotalUOM1.Text = (double.Parse(txtCountedUOM1.Text) * double.Parse(lblStockTakeCon1.Text)).ToString();
                }
                else
                {
                    lblTotalUOM1.Text = "0";
                }
                if (txtCountedUOM2.Text != string.Empty)
                {
                    lblTotalUOM2.Text = (double.Parse(txtCountedUOM2.Text) * double.Parse(lblStockTakeCon2.Text)).ToString();
                }
                else
                {
                    lblTotalUOM2.Text = "0";
                }

                if (txtCountedUOM1.Text != null && txtCountedUOM1.Text != "")
                {
                    totalValue = totalValue + Convert.ToDecimal(txtCountedUOM1.Text);
                    btnSubmit.Enabled = true;
                    btnSubmit.Style["background-image"] = "url('Images/bgButton.png')";
                }

                if (txtCountedUOM2.Text != null && txtCountedUOM2.Text != "")
                {
                    totalValue = totalValue + Convert.ToDecimal(txtCountedUOM2.Text);
                    btnSubmit.Enabled = true;
                    btnSubmit.Style["background-image"] = "url('Images/bgButton.png')";
                }

                DataTable tb = (DataTable)Session["STCS"];
                if (tb != null)
                {
                    DataRow[] rupdate = tb.Select("ItemCode='" + lblItemCode.Text + "'");
                    rupdate[0]["CountedUOM1"] = txtCountedUOM1.Text;
                    rupdate[0]["TotalUOM1"] = lblTotalUOM1.Text;

                    rupdate[0]["CountedUOM2"] = txtCountedUOM2.Text;
                    rupdate[0]["TotalUOM2"] = lblTotalUOM2.Text;

                    rupdate[0]["CountedInvUOM"] = Convert.ToDouble(rupdate[0]["TotalUOM1"]) + Convert.ToDouble(rupdate[0]["TotalUOM2"]);
                    rupdate[0]["Variance"] = Convert.ToDouble(rupdate[0]["CountedInvUOM"]) - Convert.ToDouble(lblInStock.Text);
                }
                // here you'll get all rows with RowType=DataRow
                // others like Header are omitted in a foreach
            }
        }

        private void BindData(DataTable tblData)
        {
            Session["STCS"] = tblData;

            DataView dv = tblData.DefaultView;

            //dv.Sort = "Description ASC";

            this.grvStkCnt.DataSource = dv.ToTable();
            this.grvStkCnt.DataBind();
        }

        public void mouseOverandMouseOut()
        {
            btnSaveDraft.Attributes.Add("class", "static");
            btnSaveDraft.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnSaveDraft.Attributes.Add("onMouseOut", "this.className='static'");

            btnSubmit.Attributes.Add("class", "static");
            btnSubmit.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnSubmit.Attributes.Add("onMouseOut", "this.className='static'");

            btnSearch.Attributes.Add("class", "static");
            btnSearch.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnSearch.Attributes.Add("onMouseOut", "this.className='static'");

            btnStkOk.Attributes.Add("class", "static");
            btnStkOk.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnStkOk.Attributes.Add("onMouseOut", "this.className='static'");

            btnStkCancel.Attributes.Add("class", "static");
            btnStkCancel.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnStkCancel.Attributes.Add("onMouseOut", "this.className='static'");
        }

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            Session[AppConstants.Status_countingSheet] = string.Empty;
            Session[AppConstants.WhsCode_countingSheet] = string.Empty;
            if (ddlWareHouse.Text != " --- Select Outlet --- ")
            {
                if (txtCountDate.Text != string.Empty)
                {
                    //if (totalValue != 0)
                    //{
                    btnSubmit.Enabled = true;
                    btnSubmit.Style["background-image"] = "url('Images/bgButton.png')";
                    LoadHeaderData();

                    DataTable HeaderTable = (DataTable)Session["STCSHeader"];

                    if (HeaderTable != null && HeaderTable.Rows.Count > 0)
                    {
                        HeaderTable.TableName = "tblHeader";
                        DataSet ds = new DataSet();
                        ds.Tables.Add(HeaderTable);
                        var ObjC = new MasterService.MasterSoapClient();
                        //var returnResult = ObjC.UpdateDOStatus(ds, Session[AppConstants.DBName].ToString(), lblStatus.Text, Session[AppConstants.UserRole].ToString(), false);
                        var returnResult = ObjC.Update_StockTakeApprove(ds, Session[AppConstants.DBName].ToString(), lblStatus.Text, Session[AppConstants.UserRole].ToString(), false, Session[AppConstants.UserID].ToString(), Session[AppConstants.UserName].ToString(), ddlWareHouse.SelectedValue.ToString().Trim());

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
                            lblError.Text = "StockTake Counting Sheet Updated Successfully.";
                            totalValue = 0;
                            // clearFields();
                        }
                        ds.Tables.Remove(HeaderTable);
                        ds.Clear();

                    }
                    // }
                    //else
                    //{
                    //    lblError.Visible = true;
                    //    lblError.Text = "Counted Quantity cannot be empty.";
                    //    btnSubmit.Enabled = false;
                    //    btnSubmit.Style["background-image"] = "url('Images/bgButton_disable.png')";
                    //    return;
                    //}
                }
                else
                {
                    lblError.Visible = true;
                    lblError.Text = "Kinldy select the Count Date";
                    btnSubmit.Enabled = false;
                    btnSubmit.Style["background-image"] = "url('Images/bgButton_disable.png')";
                    return;
                }
            }
            else
            {
                lblError.Visible = true;
                lblError.Text = "Kindly select the outlet.";
                btnSubmit.Enabled = false;
                btnSubmit.Style["background-image"] = "url('Images/bgButton_disable.png')";
                return;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (ddlWareHouse.SelectedValue == " --- Select Outlet --- ")
            {
                lblError.Visible = true;
                lblError.Text = "Kindly Select Outlet.";
            }
            else if (txtCountDate.Text == string.Empty)
            {
                lblError.Visible = true;
                lblError.Text = "Kindly Select the Count Date.";
            }
            else
            {
                // this part of code is to clear the Grid view once the search button is clicked
                DataTable tbl = CreateTableFormat();
                grvStkCnt.DataSource = tbl;
                grvStkCnt.DataBind();
                // End

                lblError.Visible = false;
                lblError.Text = string.Empty;
                string sreturn = LoadData(this.ddlWareHouse.SelectedValue.ToString());
                if (sreturn != "")
                {
                    btnSaveDraft.Enabled = false;
                    btnSaveDraft.Style["background-image"] = "url('Images/bgButton_disable.png')";
                    btnSubmit.Enabled = false;
                    btnSubmit.Style["background-image"] = "url('Images/bgButton_disable.png')";
                }
                else if (totalValue == 0)
                {
                    btnSubmit.Enabled = false;
                    btnSubmit.Style["background-image"] = "url('Images/bgButton_disable.png')";
                }
            }
        }

        protected void btnStkOk_Click(object sender, EventArgs e)
        {
            DataTable tblSTCS = CreateTableFormat();
            var data = new MasterService.MasterSoapClient();
            string sLabelStatus = "Draft,Pending Approval,Approved,Draft Recount";
            DataSet ds = data.Create_StockTakeCounting(Session[AppConstants.DBName].ToString(), ddlWareHouse.SelectedValue.Trim()
                                , Session[AppConstants.UserRole].ToString(), sLabelStatus, string.Empty
                                , Session[AppConstants.UserID].ToString(), Session[AppConstants.UserName].ToString(), txtCountDate.Text);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                int i = 1;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    DataRow rowNew = tblSTCS.NewRow();
                    rowNew["No"] = i;
                    rowNew["DocEntry"] = row["DocEntry"];
                    rowNew["ItemCode"] = row["ItemCode"];
                    rowNew["Description"] = row["Description"];
                    rowNew["CardCode"] = row["CardCode"];
                    rowNew["CardName"] = row["CardName"];
                    rowNew["LineNum"] = row["LineNum"];
                    rowNew["InStock"] = row["InStock"];
                    rowNew["Variance"] = row["Variance"];
                    txtCountDate.Text = row["DocDate"].ToString();
                    rowNew["StockTakeUoM1"] = row["StockTakeUoM1"];
                    rowNew["StockTakeCon1"] = row["StockTakeCon1"];
                    rowNew["CountedUOM1"] = row["CountedUOM1"];
                    rowNew["TotalUOM1"] = row["TotalUOM1"];
                    rowNew["StockTakeUOM2"] = row["StockTakeUOM2"];
                    rowNew["StockTakeCon2"] = row["StockTakeCon2"];
                    rowNew["CountedUOM2"] = row["CountedUOM2"];
                    rowNew["TotalUOM2"] = row["TotalUOM2"];
                    rowNew["InventoryUOM"] = row["InventoryUOM"];
                    rowNew["CountedInvUOM"] = row["CountedInvUOM"];
                    rowNew["DocDate"] = row["DocDate"];
                    rowNew["WhsName"] = row["WhsName"];


                    tblSTCS.Rows.Add(rowNew);
                    i++;
                }
            }
            Session["STCS"] = tblSTCS;
            DataView dv = tblSTCS.DefaultView;
            this.grvStkCnt.DataSource = dv.ToTable();
            this.grvStkCnt.DataBind();

            int ApprovalLevel;
            ApprovalLevel = Convert.ToInt32(Session[AppConstants.ApprovalLevel]);
            if (ApprovalLevel == 0)
            {
                this.grvStkCnt.Columns[12].Visible = false;
                this.grvStkCnt.Columns[13].Visible = false;
                this.grvStkCnt.Columns[14].Visible = false;
                this.grvStkCnt.Columns[15].Visible = false;
                this.grvStkCnt.Columns[16].Visible = true;
                this.grvStkCnt.Columns[17].Visible = true;
                this.grvStkCnt.Columns[18].Visible = true;
                this.grvStkCnt.Columns[19].Visible = true;
            }

            //LoadData(ddlWareHouse.SelectedValue.ToString());
            btnSaveDraft.Enabled = true;
            btnSaveDraft.Style["background-image"] = "url('Images/bgButton.png')";
            btnSubmit.Enabled = false;
            btnSubmit.Style["background-image"] = "url('Images/bgButton_disable.png')";
            mStkPopup.Hide();
        }

        protected void btnStkCancel_Click(object sender, EventArgs e)
        {
            mStkPopup.Hide();
        }

        private void LoadHeaderData()
        {
            try
            {
                DataTable tbNew = new DataTable();
                tbNew = CreateTable();
                UpdateSessionTable();
                DataTable tblSTCS = (DataTable)Session["STCS"];
                //foreach (GridViewRow row in grvStkCnt.Rows)
                foreach (DataRow item in tblSTCS.Rows)
                {
                    DataRow rowNew = tbNew.NewRow();
                    rowNew["#"] = item["No"];
                    rowNew["CardCode"] = item["CardCode"];
                    rowNew["CardName"] = item["CardName"];
                    rowNew["ItemCode"] = item["ItemCode"];
                    rowNew["DocEntry"] = item["DocEntry"];
                    rowNew["LineNum"] = item["LineNum"];
                    rowNew["Description"] = item["Description"];
                    rowNew["InStock"] = item["InStock"];

                    //rowNew["Stocktake UoM"] = item["StocktakeUoM"];
                    //rowNew["Counted"] = item["Counted"];

                    rowNew["Variance"] = item["Variance"];

                    //rowNew["StockTakeConv"] = item["StockTakeConv"];
                    //rowNew["Adjust in Inventory UoM"] = item["Adjust in Inventory UoM"];

                    rowNew["StockTakeUOM1"] = item["StockTakeUOM1"];
                    rowNew["StockTakeCon1"] = item["StockTakeCon1"];
                    rowNew["CountedUOM1"] = item["CountedUOM1"];
                    rowNew["TotalUOM1"] = item["TotalUOM1"];

                    rowNew["StockTakeUOM2"] = item["StockTakeUOM2"];
                    rowNew["StockTakeCon2"] = item["StockTakeCon2"];
                    rowNew["CountedUOM2"] = item["CountedUOM2"];
                    rowNew["TotalUOM2"] = item["TotalUOM2"];

                    rowNew["InventoryUOM"] = item["InventoryUOM"];
                    rowNew["CountedInvUOM"] = item["CountedInvUOM"];


                    rowNew["CountDate"] = Convert.ToDateTime(txtCountDate.Text).Date;
                    rowNew["CreatedBy"] = Session[AppConstants.UserCode].ToString();
                    rowNew["ApprovedDate"] = "";
                    rowNew["ApprovedBy"] = "";
                    tbNew.Rows.Add(rowNew);
                }
                Session["STCSHeader"] = tbNew;
                //{
                //    Label lblNo = (Label)row.FindControl("lblNo");
                //    Label lblItemCode = (Label)row.FindControl("lblItemCode");
                //    Label lblDocEntry = (Label)row.FindControl("lblDocEntry");
                //    Label lblLineNum = (Label)row.FindControl("lblLineNum");
                //    // string CardCode = lblSupplierCode.Text;
                //    Label lblDescription = (Label)row.FindControl("lblDescription");
                //    Label lblInStock = (Label)row.FindControl("lblInStock");
                //    Label lblStockTake = (Label)row.FindControl("lblStockTake");
                //    TextBox txtCounted = (TextBox)row.FindControl("txtCounted");
                //    Label lblVariance = (Label)row.FindControl("lblVariance");
                //    Label lblStockTakeConv = (Label)row.FindControl("lblStockTakeConv");
                //    Label lblAdjustInventory = (Label)row.FindControl("lblAdjustInventory");


                //    Session["STCSHeader"] = tbNew;
                //}


            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private DataTable CreateTable()
        {
            DataTable tblCount = new DataTable();
            tblCount.Columns.Add("#");
            tblCount.Columns.Add("ItemCode");
            tblCount.Columns.Add("CardCode");
            tblCount.Columns.Add("CardName");
            tblCount.Columns.Add("DocEntry");
            tblCount.Columns.Add("LineNum");
            tblCount.Columns.Add("Description");
            tblCount.Columns.Add("InStock");
            //tblCount.Columns.Add("Stocktake UoM");
            //tblCount.Columns.Add("Counted");
            tblCount.Columns.Add("Variance");
            //tblCount.Columns.Add("StockTakeConv");
            //tblCount.Columns.Add("Adjust in Inventory UoM");

            tblCount.Columns.Add("StockTakeUOM1");
            tblCount.Columns.Add("StockTakeCon1");
            tblCount.Columns.Add("CountedUOM1");
            tblCount.Columns.Add("TotalUOM1");

            tblCount.Columns.Add("StockTakeUOM2");
            tblCount.Columns.Add("StockTakeCon2");
            tblCount.Columns.Add("CountedUOM2");
            tblCount.Columns.Add("TotalUOM2");

            tblCount.Columns.Add("InventoryUOM");
            tblCount.Columns.Add("CountedInvUOM");

            tblCount.Columns.Add("CountDate");
            tblCount.Columns.Add("CreatedBy");
            tblCount.Columns.Add("ApprovedBy");
            tblCount.Columns.Add("ApprovedDate");
            //tbTemp.Columns.Add("IsModified");

            return tblCount;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Session[AppConstants.Status_countingSheet] = string.Empty;
            Session[AppConstants.WhsCode_countingSheet] = string.Empty;
            if (txtCountDate.Text != string.Empty)
            {
                LoadHeaderData();

                DataTable HeaderTable = (DataTable)Session["STCSHeader"];

                if (HeaderTable != null && HeaderTable.Rows.Count > 0)
                {
                    bool sResult = CheckNullValues(HeaderTable);
                    if (sResult == true)
                    {
                        HeaderTable.TableName = "tblHeader";
                        DataSet ds = new DataSet();
                        ds.Tables.Add(HeaderTable);
                        var ObjC = new MasterService.MasterSoapClient();
                        //var returnResult = ObjC.UpdateDOStatus(ds, Session[AppConstants.DBName].ToString(), "Pending Approval", Session[AppConstants.UserRole].ToString(), false);
                        var returnResult = ObjC.Update_StockTakeApprove(ds, Session[AppConstants.DBName].ToString(), "Pending Approval", Session[AppConstants.UserRole].ToString(), false, Session[AppConstants.UserID].ToString(), Session[AppConstants.UserName].ToString(), ddlWareHouse.SelectedValue.ToString().Trim());

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
                            lblError.Text = "StockTake Counting Sheet Submitted Successfully.";
                            btnSaveDraft.Enabled = false;
                            btnSubmit.Enabled = false;
                            btnSubmit.Style["background-image"] = "url('Images/bgButton_disable.png')";
                            btnSaveDraft.Style["background-image"] = "url('Images/bgButton_disable.png')";
                            // clearFields();
                        }
                        ds.Tables.Remove(HeaderTable);
                        ds.Clear();
                    }
                    else
                    {
                        lblError.Visible = true;
                        lblError.Text = "Counted Quantity cannot be Null or Empty. Item Description : " + sItemDescription;
                        return;
                    }
                }
            }
            else
            {
                lblError.Visible = true;
                lblError.Text = "Kindly select the Count Date";
                return;
            }
        }

        public bool CheckNullValues(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                object value1 = row["CountedUOM1"];
                object value2 = row["CountedUOM2"];

                if (value1.ToString() == "")
                {
                    sItemDescription = row["Description"].ToString().Trim();
                    return false;
                }   // do something
                if (value2.ToString() == "")
                {
                    sItemDescription = row["Description"].ToString().Trim();
                    return false;
                }
            }
            return true;
        }

    }
}