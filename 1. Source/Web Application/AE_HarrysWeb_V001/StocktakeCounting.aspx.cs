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
    public partial class StocktakeCounting1 : System.Web.UI.Page
    {
        public static string sItemDescription = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session[Utils.AppConstants.UserCode].ToString() != string.Empty)
                {
                    if (!IsPostBack)
                    {
                        var data = new MasterService.MasterSoapClient();
                        //Calling the webmethod
                        DataSet ds = data.Get_SalesTakingCountList(Session[AppConstants.DBName].ToString(), Session[AppConstants.WhsCode].ToString(), Session[AppConstants.UserRole].ToString());
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            Session["PurchaseDetails"] = ds;
                            //Load the Header Info
                            LoadHeaderData(ds);

                        }
                        mouseOverandMouseOut();
                        LoadData(this.lblOutlet.Text.ToString());
                        if (Convert.ToString(Session[AppConstants.ApprovedDate]) != string.Empty)
                        {
                            btnApprove.Enabled = false;
                            btnReject.Enabled = false;
                            btnApprove.Style["background-image"] = "url('Images/bgButton_disable.png')";
                            btnReject.Style["background-image"] = "url('Images/bgButton_disable.png')";
                        }
                        UpdateSessionTable();
                    }
                }
                else
                {
                    //Response.Redirect(AppConstants.LoginURL);
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = "Problem in loading the Data..." + ex.Message;
                this.lblError.Visible = true;
                //Response.Redirect("~/ErrorPage.aspx");
                // Response.Redirect(AppConstants.LoginURL);
            }
        }

        private void LoadHeaderData(DataSet ds)
        {
            int i = 0;
            foreach (DataRow item in ds.Tables[0].Rows)
            {
                if (item[2].ToString() == Session[AppConstants.Status].ToString() && item[1].ToString() == Session[AppConstants.CountDate].ToString()
                    && item[0].ToString() == Session[AppConstants.WhsCode].ToString())
                {
                    break;
                }
                i = i + 1;
            }
            lblUserName.Text = ds.Tables[0].Rows[i]["CreatedBy"].ToString();
            lblStatus.Text = Session[AppConstants.Status].ToString();
            lblCountDate.Text = Session[AppConstants.CountDate].ToString();
            lblOutlet.Text = ds.Tables[0].Rows[i]["WhsName"].ToString();
            lblApprovedBy.Text = Session[Utils.AppConstants.UserCode].ToString().ToUpper();
            if (Convert.ToString(Session[AppConstants.ApprovedDate]) != string.Empty)
            {
                txtApprovedDate.Visible = false;
                Image1.Visible = false;
                lblApprovedDate.Visible = true;
                lblApprovedDate.Text = ds.Tables[0].Rows[i]["U_ApprovedDate"].ToString();
                lblApprovedBy.Text = ds.Tables[0].Rows[i]["U_ApprovedBy"].ToString();
            }
            else
            {
                txtApprovedDate.Visible = true;
                Image1.Visible = true;
                lblApprovedDate.Visible = false;
                txtApprovedDate.Text = DateTime.Now.ToShortDateString();
            }
        }

        private void LoadData(string sOutlet)
        {
            try
            {
                DataTable tblSTCS = CreateTableFormat();
                var data = new MasterService.MasterSoapClient();
                //Calling the webmethod
                //DataSet ds = data.Get_StockTakeCounting(Session[AppConstants.DBName].ToString(), Session[AppConstants.WhsCode].ToString(),
                //                                    Session[AppConstants.UserRole].ToString(), Session[AppConstants.Status].ToString(), Session[AppConstants.StockTakeDocEntry].ToString());

                DataSet ds = data.Get_StockTakeCountingDetails(Session[AppConstants.DBName].ToString(), Session[AppConstants.WhsCode].ToString(),
                                                    Session[AppConstants.UserRole].ToString(), Session[AppConstants.Status].ToString()
                                                    , Session[AppConstants.StockTakeDocEntry].ToString(), Session[AppConstants.UserID].ToString()
                                                    , Session[AppConstants.UserName].ToString(), string.Empty);

                Session["StockTakeOriginal"] = ds;
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

                        //rowNew["Counted"] = row["Counted"];
                        //rowNew["StockTakeUoM"] = row["Stocktake UoM"];
                        //rowNew["StockTakeConv"] = row["StockTakeConv"];
                        //rowNew["Adjust in Inventory UoM"] = row["Adjust In InventoryUOM"];
                        //rowNew["Event Order Commitment"] = row["Event Order Commitment"];

                        rowNew["CountDate"] = row["DocDate"];

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

                        rowNew["CreatedBy"] = Session[AppConstants.UserCode].ToString();
                        if (txtApprovedDate.Text != string.Empty)
                        {
                            rowNew["ApprovedDate"] = Convert.ToDateTime(txtApprovedDate.Text);
                        }
                        else
                        {
                            rowNew["ApprovedDate"] = "";
                        }
                        rowNew["ApprovedBy"] = lblApprovedBy.Text;
                        tblSTCS.Rows.Add(rowNew);
                        i++;
                    }
                    Session["STCS"] = tblSTCS;
                    DataView dv = tblSTCS.DefaultView;
                    //dv.Sort = "ItemCode ASC";
                    int ApprovalLevel;
                    ApprovalLevel = Convert.ToInt32(Session[AppConstants.ApprovalLevel]);
                    if (ApprovalLevel == 0)
                    {
                        this.grvStkCnt.Columns[12].Visible = false;
                        this.grvStkCnt.Columns[13].Visible = false;
                        this.grvStkCnt.Columns[14].Visible = false;
                        this.grvStkCnt.Columns[15].Visible = false;
                        this.grvStkCnt.Columns[12].Visible = true;
                        this.grvStkCnt.Columns[13].Visible = true;
                        this.grvStkCnt.Columns[14].Visible = true;
                        this.grvStkCnt.Columns[15].Visible = true;
                    }
                    this.grvStkCnt.DataSource = dv.ToTable();
                    this.grvStkCnt.DataBind();

                }
                else
                {
                    Session["STCS"] = tblSTCS;
                    DataView dv = tblSTCS.DefaultView;
                    this.grvStkCnt.DataSource = dv.ToTable();
                    this.grvStkCnt.DataBind();
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = "Error Occured while Loading data." + ex.Message;
                this.lblError.Visible = true;
            }
        }

        private DataTable CreateTableFormat()
        {
            DataTable tbTemp = new DataTable();
            tbTemp.Columns.Add("No");
            tbTemp.Columns.Add("DocEntry");
            tbTemp.Columns.Add("ItemCode");
            tbTemp.Columns.Add("Description");
            tbTemp.Columns.Add("CardCode");
            tbTemp.Columns.Add("CardName");
            tbTemp.Columns.Add("LineNum");
            tbTemp.Columns.Add("InStock");

            //tbTemp.Columns.Add("StockTakeUoM");
            //tbTemp.Columns.Add("StockTakeConv");
            //tbTemp.Columns.Add("Adjust in Inventory UoM");
            //tbTemp.Columns.Add("Event Order Commitment");
            //tbTemp.Columns.Add("Counted");

            tbTemp.Columns.Add("Variance");

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

            tbTemp.Columns.Add("CountDate");
            tbTemp.Columns.Add("CreatedBy");
            tbTemp.Columns.Add("ApprovedBy");
            tbTemp.Columns.Add("ApprovedDate");
            return tbTemp;
        }

        //protected void grvStkCnt_OnRowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        e.Row.Attributes.Add("onmouseover", "MouseEvents(this, event)");
        //        e.Row.Attributes.Add("onmouseout", "MouseEvents(this, event)");
        //        e.Row.Attributes["style"] = "cursor:pointer";
        //        int index = 0;
        //        TextBox txt1 = e.Row.FindControl("txtCountedUOM1") as TextBox;
        //        TextBox txt2 = e.Row.FindControl("txtCountedUOM2") as TextBox;

        //        if (Convert.ToString(Session["CountUOM2"]) == "txtCountUOM2")
        //        {
        //            txt1.Attributes.Add("onfocus", "JavaScript:document.getElementById('" + txt1.ClientID + "').select();");
        //            if (Convert.ToString(Session[AppConstants.ApprovedDate]) != string.Empty)
        //            {
        //                txt1.Enabled = false;
        //            }
        //            else
        //            {
        //                txt1.Enabled = true;
        //            }
        //            if (ViewState["rowindex"] != null && Session["STCS"] != null)
        //            {
        //                index = Convert.ToInt32(ViewState["rowindex"].ToString());
        //                int rowCount = ((DataTable)Session["STCS"]).Rows.Count;
        //                if ((index + 1) <= this.grvStkCnt.Rows.Count)
        //                {
        //                    if (e.Row.RowIndex == index + 1)
        //                    {
        //                        txt1.Focus();
        //                        Session["CountUOM2"] = string.Empty;
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            txt2.Attributes.Add("onfocus", "JavaScript:document.getElementById('" + txt2.ClientID + "').select();");
        //            if (Convert.ToString(Session[AppConstants.ApprovedDate]) != string.Empty)
        //            {
        //                txt2.Enabled = false;
        //                txt1.Enabled = false;
        //            }
        //            else
        //            {
        //                txt1.Enabled = true;
        //                txt2.Enabled = true;
        //            }
        //            if (ViewState["rowindex"] != null && Session["STCS"] != null)
        //            {
        //                index = Convert.ToInt32(ViewState["rowindex"].ToString());
        //                int rowCount = ((DataTable)Session["STCS"]).Rows.Count;
        //                if ((index) <= this.grvStkCnt.Rows.Count)
        //                {
        //                    if (e.Row.RowIndex == index)
        //                    {
        //                        txt2.Focus();
        //                    }
        //                }
        //            }
        //        }
        //    }

        //}

        //protected void txtCountedUOM1_OnTextChanged(object sender, EventArgs e)
        //{
        //    lblError.Text = string.Empty;
        //    GridViewRow row = (GridViewRow)((DataControlFieldCell)((TextBox)sender).Parent).Parent;
        //    ViewState["rowindex"] = row.RowIndex;
        //    TextBox txtCountedUOM1 = (TextBox)row.FindControl("txtCountedUOM1");
        //    Label lblStockTakeCon1 = (Label)row.FindControl("lblStockTakeCon1");

        //    TextBox txtCountedUOM2 = (TextBox)row.FindControl("txtCountedUOM2");
        //    Label lblStockTakeCon2 = (Label)row.FindControl("lblStockTakeCon2");

        //    //if (txtCountedUOM1.Text != null && txtCountedUOM1.Text != "")
        //    //{
        //    //    totalValue = totalValue + Convert.ToDecimal(txtCountedUOM1.Text);
        //    //    btnSubmit.Enabled = true;
        //    //    btnSubmit.Style["background-image"] = "url('Images/bgButton.png')";
        //    //}
        //    Label lblItemCode = (Label)row.FindControl("lblItemCode");
        //    Label lblInStock = (Label)row.FindControl("lblInStock");
        //    Label lblTotalUOM1 = (Label)row.FindControl("lblTotalUOM1"); 9443959033
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

        //    //if (txtCountedUOM2.Text != null && txtCountedUOM2.Text != "")
        //    //{
        //    //    totalValue = totalValue + Convert.ToDecimal(txtCountedUOM1.Text);
        //    //    btnSubmit.Enabled = true;
        //    //    btnSubmit.Style["background-image"] = "url('Images/bgButton.png')";
        //    //}

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
        //}

        protected void grvStkCnt_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            var var1 = e.NewPageIndex;
            var var2 = this.grvStkCnt.PageIndex;

            UpdateSessionTable();

            this.grvStkCnt.PageIndex = e.NewPageIndex;
            DataTable tblSTCS = (DataTable)Session["STCS"];
            BindData(tblSTCS);
            disableTextBoxes();        
        }

        public void disableTextBoxes()
        {
            foreach (GridViewRow row in grvStkCnt.Rows)
            {
                TextBox txtCountedUOM1 = (TextBox)row.FindControl("txtCountedUOM1");
                TextBox txtCountedUOM2 = (TextBox)row.FindControl("txtCountedUOM2");

                if (Convert.ToString(Session[AppConstants.ApprovedDate]) != string.Empty)
                {
                    txtCountedUOM2.Enabled = false;
                    txtCountedUOM1.Enabled = false;
                }
                else
                {
                    txtCountedUOM1.Enabled = true;
                    txtCountedUOM2.Enabled = true;
                }
            }
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

                //if (Convert.ToString(Session[AppConstants.ApprovedDate]) != string.Empty)
                //{
                //    txtCountedUOM1.Enabled = false;
                //}
                //else
                //{
                //    txtCountedUOM1.Enabled = true;
                //}

                if (Convert.ToString(Session[AppConstants.ApprovedDate]) != string.Empty)
                {
                    txtCountedUOM2.Enabled = false;
                    txtCountedUOM1.Enabled = false;
                }
                else
                {
                    txtCountedUOM1.Enabled = true;
                    txtCountedUOM2.Enabled = true;
                }

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

            //dv.Sort = "ItemCode  ASC";

            this.grvStkCnt.DataSource = dv.ToTable();
            this.grvStkCnt.DataBind();
        }

        public void mouseOverandMouseOut()
        {
            btnApprove.Enabled = true;
            btnApprove.Style["background-image"] = "url('Images/bgButton.png')";
            btnApprove.Attributes.Add("class", "static");
            btnApprove.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnApprove.Attributes.Add("onMouseOut", "this.className='static'");

            btnReject.Attributes.Add("class", "static");
            btnReject.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnReject.Attributes.Add("onMouseOut", "this.className='static'");
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            if (txtApprovedDate.Text != string.Empty)
            {
                if (Convert.ToDateTime(txtApprovedDate.Text).Date < Convert.ToDateTime(lblCountDate.Text).Date)
                {
                    lblError.Visible = true;
                    lblError.Text = "Approved date should not be less than Count date.";
                    return;
                }
                UpdateSessionTable();
                DataTable dt = (DataTable)Session["STCS"];
                DataSet ds = new DataSet();
                foreach (DataRow d in dt.Rows)
                {
                    d["ApprovedDate"] = txtApprovedDate.Text;
                }
                DataTable dtCol = dt;
                dtCol.TableName = "tblHeader";
                DataTable table1 = dtCol.Copy();
                ds.Tables.Add(table1);

                //Need to pass that dataset to web service
                bool sResult = CheckNullValues(table1);
                if (sResult == true)
                {
                    var ObjC = new MasterService.MasterSoapClient();
                    //var updateStatus = ObjC.UpdateDOStatus(ds, Session[AppConstants.DBName].ToString(), "Approved", Session[AppConstants.UserRole].ToString(), true);
                    var returnResult = ObjC.Update_StockTakeApprove(ds, Session[AppConstants.DBName].ToString(), "Approved", Session[AppConstants.UserRole].ToString(), true, Session[AppConstants.UserID].ToString(), Session[AppConstants.UserName].ToString(), Session[AppConstants.WhsCode].ToString());
                    //if (updateStatus == "SUCCESS")
                    //{
                    //    var returnResult = ObjC.StockTakeApprove(ds, Session[AppConstants.WhsCode].ToString(), Session[AppConstants.DBName].ToString());
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
                        lblError.Text = "StockTake Counting Sheet Approved Successfully.";
                        btnApprove.Enabled = false;
                        btnReject.Enabled = false;
                        btnApprove.Style["background-image"] = "url('Images/bgButton_disable.png')";
                        btnReject.Style["background-image"] = "url('Images/bgButton_disable.png')";
                    }
                    ds.Tables.Remove(table1);
                    ds.Clear();
                }
                else
                {
                    lblError.Visible = true;
                    lblError.Text = "Counted Quantity cannot be Null or Empty. Item Description : " + sItemDescription;
                    return;
                }
                //}
                //else
                //{
                //    lblError.Visible = true;
                //    lblError.Text = "Error : " + updateStatus.ToString();
                //}
            }
            else
            {
                lblError.Visible = true;
                lblError.Text = "Kindly select the approved date";
            }
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            UpdateSessionTable();
            DataTable dt = (DataTable)Session["STCS"];
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            //Need to pass that dataset to web service

            var ObjC = new MasterService.MasterSoapClient();
            //var returnResult = ObjC.UpdateDOStatus(ds, Session[AppConstants.DBName].ToString(), "Draft Recount", Session[AppConstants.UserRole].ToString(), false);
            var returnResult = ObjC.Update_StockTakeApprove(ds, Session[AppConstants.DBName].ToString(), "Draft Recount", Session[AppConstants.UserRole].ToString(), false, Session[AppConstants.UserID].ToString(), Session[AppConstants.UserName].ToString(), Session[AppConstants.WhsCode].ToString());

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
                lblError.Text = "StockTake Counting Sheet Rejected Successfully.";
                grvStkCnt.DataBind();
                btnApprove.Enabled = false;
                btnApprove.Style["background-image"] = "url('Images/bgButton_disable.png')";
            }
            ds.Tables.Remove(dt);
            ds.Clear();
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