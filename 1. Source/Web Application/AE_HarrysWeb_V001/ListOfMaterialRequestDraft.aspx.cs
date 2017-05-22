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
    public partial class ListOfMaterialRequestDraft : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session[Utils.AppConstants.UserCode].ToString() != string.Empty)
                {
                    if (!IsPostBack)
                    {
                        Session[AppConstants.IsBackPage] = 0;
                        BindOutletData();
                        LoadData(ddlOutlet.SelectedValue.ToString());
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
                this.lblError.Text = " Failed to load Data.." + ex.Message;
                this.lblError.Visible = true;
                //Response.Redirect("~/ErrorPage.aspx");
            }
        }

        private void BindOutletData()
        {
            try
            {
                var data = new MasterService.MasterSoapClient();
                DataSet dsOutlet = data.Get_Outlet_Details(Session[AppConstants.SUPERUSER].ToString(), Session[AppConstants.WhsCode].ToString(),
                    Convert.ToInt16(Session[AppConstants.ApprovalLevel]), Session[AppConstants.U_ConnString].ToString());
                if (dsOutlet != null && dsOutlet.Tables[0].Rows.Count != 0)
                {
                    ddlOutlet.DataSource = dsOutlet.Tables[0];
                    ddlOutlet.DataTextField = dsOutlet.Tables[0].Columns["WhsName"].ColumnName.ToString();
                    ddlOutlet.DataValueField = dsOutlet.Tables[0].Columns["WhsCode"].ColumnName.ToString();
                    ddlOutlet.DataBind();
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = "problem in loading the Outlets" + ex.Message;
                this.lblError.Visible = true;
            }

        }

        protected void ddlOutlet_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.ddlOutlet.Text.Trim().Length > 0)
                {
                    lblError.Visible = false;
                    lblError.Text = string.Empty;
                    LoadData(this.ddlOutlet.SelectedValue.ToString());
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }

        private void LoadData(string outlet)
        {
            try
            {
                var objC = new MasterService.MasterSoapClient();
                DataSet ds = objC.Get_MaterialReqDraft(outlet, Session[AppConstants.U_ConnString].ToString());
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    Session["MRDraftList"] = ds.Tables[0];
                    this.grvMRDraft.DataSource = ds.Tables[0];
                    this.grvMRDraft.DataBind();
                }
                else
                {
                    Session["MRDraftList"] = ds;
                    this.grvMRDraft.DataSource = ds;
                    this.grvMRDraft.DataBind();
                }
            }
            catch (Exception ex)
            {
                this.lblError.Visible = true;
                this.lblError.Text = "Failed to load data in Grid" + ex.Message;
            }
        }

        protected void lnkDrfNO_Click(object sender, System.EventArgs e)
        {
            LinkButton lnkRowSelection = (LinkButton)sender;
            string[] arguments = lnkRowSelection.CommandArgument.Split(';');
            Session[AppConstants.DraftNo] = arguments[0];
            Session[AppConstants.DocType] = arguments[1];
            Session[AppConstants.IsBackPage] = 1;
            Response.Redirect(AppConstants.MaterialRequestDraftURL);
        }

        protected void btndeleteDraft_Click(object sender, EventArgs e)
        {
            var data = new MasterService.MasterSoapClient();


            DataSet oDSSelectedDrafts = Get_SelectedDrafts();
            DataSet oDSPRFinal = new DataSet();
            DataSet oDSPQFinal = new DataSet();
            DataTable oDTFinal = new DataTable();

            var returnResult = string.Empty;
            DataView oDVSelectedDrafts = new DataView();


            if (oDSSelectedDrafts != null && oDSSelectedDrafts.Tables[0].Rows.Count > 0)
            {
                ////if (Get_SelectedDrafts() != null && Get_SelectedDrafts().Tables[0].Rows.Count > 0)
                ////{
                ////    var returnResult = string.Empty;


                ////    returnResult = data.Delete_Drafts(Get_SelectedDrafts(), Session[AppConstants.U_DBName].ToString());


                ////    //if (ddlSupplier.SelectedValue == AppConstants.HoldingSupplier)
                ////    //{
                ////    //  //  returnResult = data.Create_PurchaseRequest(NonZeroDataset, true, Session[AppConstants.U_DBName].ToString(), true, false, false);
                ////    //    returnResult = data.Delete_Drafts(Get_SelectedDrafts(), Session[AppConstants.U_DBName].ToString());
                ////    //}
                ////    //else
                ////    //{
                ////    //returnResult = data.Insert_PurchaseRequest(NonZeroDataset, true, Session[AppConstants.U_DBName].ToString(), true, false, false);

                ////    returnResult = data.Delete_PurchaseRequest(Get_SelectedDrafts(), Session[AppConstants.U_DBName].ToString());

                ////    // }


                oDVSelectedDrafts = oDSSelectedDrafts.Tables[0].DefaultView;

                oDVSelectedDrafts.RowFilter = "DocType='PR'";

                if (oDVSelectedDrafts.Count > 0)
                {
                    oDTFinal = oDVSelectedDrafts.ToTable();

                    oDSPRFinal.Tables.Add(oDTFinal.Copy());

                    returnResult = data.Delete_PurchaseRequest(oDSPRFinal, Session[AppConstants.U_DBName].ToString());
                }



                oDVSelectedDrafts.RowFilter = "DocType='PQ'";

                if (oDVSelectedDrafts.Count > 0)
                {
                    string finalResult = string.Empty;
                    oDTFinal = oDVSelectedDrafts.ToTable();

                    oDSPQFinal.Tables.Add(oDTFinal.Copy());
                    if (returnResult != "SUCCESS")
                    {
                        finalResult = returnResult;
                    }
                    returnResult = data.Delete_Drafts(oDSPQFinal, Session[AppConstants.U_DBName].ToString());
                    returnResult = finalResult + returnResult;
                }


                //Display thee Results Based on Web Services.

                if (returnResult != "SUCCESS")
                {
                    lblError.Visible = true;
                    lblError.Text = returnResult;
                }
                else
                {
                    lblError.Visible = true;
                    lblError.Text = "Material Request Draft Deleted Successfully.";
                }

                LoadData(ddlOutlet.SelectedValue.ToString());
            }
            else
            {
                lblError.Visible = true;
                lblError.Text = "No Records selected to delete !!!";
            }
        }

        private DataTable CreateHeaderTable()
        {
            DataTable tbHeader = new DataTable();
            tbHeader.Columns.Add("DraftNo");
            tbHeader.Columns.Add("DocType");
            return tbHeader;
        }

        private DataSet Get_SelectedDrafts()
        {
            DataTable tblHeader = CreateHeaderTable();
            DataTable dt = new DataTable();
            DataSet dsSelectedDraft = new DataSet();
            foreach (GridViewRow gvr in this.grvMRDraft.Rows)
            {
                if (((CheckBox)gvr.FindControl("chkSelect")).Checked == true)
                {
                    DataRow rowNew = tblHeader.NewRow();
                    LinkButton lnkValue = (LinkButton)gvr.FindControl("lnkDrfNO");

                    string[] arguments = lnkValue.CommandArgument.Split(';');
                    rowNew["DraftNo"] = arguments[0];
                    Session[AppConstants.DocType] = arguments[1];
                    rowNew["DocType"] = arguments[1];
                    tblHeader.Rows.Add(rowNew);
                }
            }
            dsSelectedDraft.Tables.Add(tblHeader);
            return dsSelectedDraft;
        }

        public void mouseOverandMouseOut()
        {
            btndeleteDraft.Attributes.Add("class", "static");
            btndeleteDraft.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btndeleteDraft.Attributes.Add("onMouseOut", "this.className='static'");
        }
    }
}