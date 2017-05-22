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
    public partial class MaterialRequest : System.Web.UI.Page
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
                        var data = new MasterService.MasterSoapClient();
                        //Calling the webmethod
                        DataSet ds = data.Get_MRSubmit_ApprovedDetails(Session[AppConstants.PurchaseNo].ToString(), Session[AppConstants.DocType].ToString(),
                            Session[AppConstants.U_ConnString].ToString());
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            Session["PurchaseDetails"] = ds;
                            //Load the Header Info
                            LoadHeaderData(ds);

                        }
                        //Load the gridview
                        LoadData(this.lblOutlet.Text.ToString());
                    }


                }
                else
                {
                    //  Response.Redirect(AppConstants.LoginURL);
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
                //Response.Redirect("~/ErrorPage.aspx");
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
                GridView grvSupplierItemList = e.Row.FindControl("grvSupplierItemList") as GridView;
                grvSupplierItemList.ToolTip = SuppCode;
                grvSupplierItemList.DataSource = GetItemData(SuppCode);
                grvSupplierItemList.DataBind();

            }
        }

        protected void grvSupplierItemList_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //DataTable dt = (DataTable)Session["supplierCount"]; 
                e.Row.Attributes.Add("onmouseover", "MouseEvents(this, event)");
                e.Row.Attributes.Add("onmouseout", "MouseEvents(this, event)");
                e.Row.Attributes["style"] = "cursor:pointer";
                TotalAmt += Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "Total"));
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[13].Text = String.Format("${0}", TotalAmt);
                e.Row.Cells[13].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Font.Bold = true;
            }
        }

        private void LoadHeaderData(DataSet ds)
        {

            lblOutlet.Text = ds.Tables[0].Rows[0]["WhsName"].ToString();
            lblUserName.Text = Session[Utils.AppConstants.UserCode].ToString().ToUpper();
            lblOrderDate.Text = ds.Tables[0].Rows[0]["DocDate"].ToString().Substring(0, 10);
            lblDeliveryDate.Text = ds.Tables[0].Rows[0]["DocDueDate"].ToString().Substring(0, 10);
            lblStatus.Text = ds.Tables[0].Rows[0]["Status"].ToString();
            lblPrNO.Text = Session[AppConstants.DocType].ToString() +" NO " + Session[AppConstants.PurchaseNo].ToString();
            Session[AppConstants.OrderDate] = lblOrderDate.Text;
            lblGrandTotal.Text = ds.Tables[0].Rows[0]["DocTotal"].ToString();
        }

        private void LoadData(string sOutlet)
        {
            try
            {
                Session["MRPurchase"] = null;
                var objC = new MasterService.MasterSoapClient();
                DataSet ds = objC.Get_MRSubmit_ApprovedDetails(Session[AppConstants.PurchaseNo].ToString(), Session[AppConstants.DocType].ToString(),
                                Session[AppConstants.U_ConnString].ToString());
                DataTable dt = ds.Tables[0];
                Session["MRPurchase"] = dt;
                DataTable supdt = (DataTable)dt.DefaultView.ToTable(true, "CardCode", "CardName", "MinSpend");
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

        private void LoadSuppItemListData(string sOutlet)
        {
            try
            {
                var objC = new MasterService.MasterSoapClient();
                DataSet ds = objC.Get_MRSubmit_ApprovedDetails(Session[AppConstants.PurchaseNo].ToString(), Session[AppConstants.DocType].ToString(),
                                    Session[AppConstants.U_ConnString].ToString());
                Session["MRPurchase"] = ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void grvSupplierItemList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView gvwChild = (sender as GridView);
            gvwChild.PageIndex = e.NewPageIndex;
            gvwChild.DataSource = GetItemData(gvwChild.ToolTip);
            gvwChild.DataBind();
        }

        protected void DisplayCalendar(CheckBoxList chkCalendar, string SuppCode)
        {

            DataTable dt = (DataTable)Session["MRPurchase"];
            DataTable dtList;

            DataTable supdt = (DataTable)dt.DefaultView.ToTable(true, "CardCode", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun");

            DataRow[] ItemRows = supdt.Select("CardCode='" + SuppCode + "'");
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

        private object GetItemData(string SuppCode)
        {
            try
            {
                DataTable dt = new DataTable();

                DataTable tblMRPurchase = CreateTabelFormat();
                DataTable dtItem = (DataTable)Session["MRPurchase"];
                DataTable dtItemList = null;
                dtItemList = dtItem.Clone();
                dtItemList.Clear();
                DataRow[] ItemRows = dtItem.Select("CardCode='" + SuppCode + "'");
                dtItemList = ItemRows.CopyToDataTable();
                if (dtItemList.Rows.Count > 0)
                {
                    int i = 1;
                    foreach (DataRow row in dtItemList.Rows)
                    {
                        DataRow rowNew = tblMRPurchase.NewRow();
                        rowNew["No"] = i;
                        rowNew["SupplierCode"] = row["CardCode"];
                        rowNew["SupplierName"] = row["CardName"];
                        rowNew["ItemCode"] = row["ItemCode"];
                        rowNew["Description"] = row["ItemName"];
                        rowNew["UserRemarks"] = row["UserRemarks"];
                        rowNew["ApprRemarks"] = row["ApprRemarks"];
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

                        tblMRPurchase.Rows.Add(rowNew);
                        i++;
                    }
                    DataView dv = tblMRPurchase.DefaultView;
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

        private DataTable CreateTabelFormat()
        {
            DataTable tbTemp = new DataTable();
            tbTemp.Columns.Add("No");
            tbTemp.Columns.Add("SupplierCode");
            tbTemp.Columns.Add("SupplierName");
            tbTemp.Columns.Add("ItemCode");
            tbTemp.Columns.Add("Description");
            tbTemp.Columns.Add("UserRemarks");
            tbTemp.Columns.Add("ApprRemarks");
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
            return tbTemp;
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