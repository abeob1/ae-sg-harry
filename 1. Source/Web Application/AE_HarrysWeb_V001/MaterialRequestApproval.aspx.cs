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
    public partial class MaterialRequestApproval : System.Web.UI.Page
    {
        private decimal TotalAmt = (decimal)0.0;
        public CheckBoxList chkList = new CheckBoxList();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session[Utils.AppConstants.UserCode].ToString() != string.Empty)
                {
                    loadHeaderdata();
                    loadDropdowndata();
                    //Calling the webmethod
                    var data = new MasterService.MasterSoapClient();
                    DataSet ds = data.Get_DraftDetails("46", Session[AppConstants.U_ConnString].ToString(),"");
                    Session["ApprovalList"] = ds;
                    loadLinedata(ds);
                    mouseOverandMouseOut();
                    chkList.Attributes.Add("onclick", "radioMe(event);");
                }
                else
                {
                    Response.Redirect(AppConstants.LoginURL);
                }
            }
            catch (Exception)
            {

                Response.Redirect(AppConstants.LoginURL);
            }
        }

        protected void ddlWareHouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception)
            {
                Response.Redirect(AppConstants.LoginURL);
            }
        }

        protected void grvParentGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            TotalAmt = 0;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                string PRNo = grvParentGrid.DataKeys[e.Row.RowIndex].Value.ToString();
                chkList = e.Row.FindControl("chkCalendar") as CheckBoxList;
                GridView grvChildGrid = e.Row.FindControl("grvChildGrid") as GridView;
                grvChildGrid.ToolTip = PRNo;
                grvChildGrid.DataSource = GetItemData(PRNo);
                grvChildGrid.DataBind();

            }
        }

        protected void grvChildGrid_OnRowDataBound(object sender, GridViewRowEventArgs e)
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
                e.Row.Cells[11].Text = String.Format("${0}", TotalAmt);
                e.Row.Cells[11].HorizontalAlign = HorizontalAlign.Right;
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
            GridViewRow row = (GridViewRow)((DataControlFieldCell)((TextBox)sender).Parent).Parent;
            ViewState["rowindex"] = row.RowIndex;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {

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
        }

        private void loadHeaderdata()
        {
            lblOrderDate.Text = DateTime.Now.ToString();
            Session[AppConstants.OrderDate] = DateTime.Now;
            Session[AppConstants.IsBackPage] = 0;
            lblUserName.Text = Session[Utils.AppConstants.UserCode].ToString().ToUpper();
        }

        private void loadLinedata(DataSet ds)
        {
            DataTable dt = ds.Tables[0];
            DataTable prData = (DataTable)dt.DefaultView.ToTable(true, "DocEntry");
            grvParentGrid.DataSource = prData;
            grvParentGrid.DataBind();
        }

        private object GetItemData(string PRNo)
        {
            try
            {
                DataTable dt = new DataTable();

                DataTable tblApprovalList = CreateTableFormat();
                DataSet ds = new DataSet();
                ds = (DataSet)Session["ApprovalList"];
                DataTable dtItem = ds.Tables[0];
                DataTable dtItemList = null;
                dtItemList = dtItem.Clone();
                dtItemList.Clear();
                DataRow[] ItemRows = dtItem.Select("DocEntry='" + PRNo + "'");
                dtItemList = ItemRows.CopyToDataTable();
                if (dtItemList.Rows.Count > 1)
                {
                    int i = 1;
                    foreach (DataRow row in dtItemList.Rows)
                    {
                        DataRow rowNew = tblApprovalList.NewRow();
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
                        rowNew["Price"] = row["Price"];
                        rowNew["MinSpend"] = row["MinSpend"];
                        rowNew["OrderQuantity"] = row["OrderQuantity"];
                        rowNew["Total"] = double.Parse(row["Total"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);

                        tblApprovalList.Rows.Add(rowNew);
                        i++;
                    }
                    DataView dv = tblApprovalList.DefaultView;
                    dt = dv.ToTable();
                }
                return dt;

            }
            catch (Exception)
            {
                throw;
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
            tbTemp.Columns.Add("OrderQuantity");
            tbTemp.Columns.Add("UOM");
            tbTemp.Columns.Add("Price");
            tbTemp.Columns.Add("Total");
            tbTemp.Columns.Add("MinSpend");
            return tbTemp;
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
        
    }
}