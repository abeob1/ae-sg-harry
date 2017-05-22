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
    public partial class OutletListPendingApproval : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[Utils.AppConstants.UserCode].ToString() != string.Empty)
            {
                if (!IsPostBack)
                {
                    LoadData();
                }
            }
        }

        protected void LoadData()
        {
            Session[AppConstants.IsBackPage] = 8;
            var objC = new MasterService.MasterSoapClient();
            DataSet ds = objC.Get_OutletListPR(Session[AppConstants.UserRole].ToString().Trim() ,Session[AppConstants.DBName].ToString().Trim());
            if (ds != null && ds.Tables.Count > 0)
            {
                grvPendingAppr.DataSource = ds.Tables[0];
                grvPendingAppr.DataBind();
            }
        }

        protected void grvPendingAppr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton hlk1 = e.Row.FindControl("lnkNoPRL1Approval") as LinkButton;
                LinkButton hlk2 = e.Row.FindControl("lnkNoPRL2Approval") as LinkButton;

                if (Convert.ToInt32(Session[AppConstants.ApprovalLevel]) == 1)
                {
                    hlk1.Enabled = true;
                    hlk2.Enabled = false;
                }
                else if (Convert.ToInt32(Session[AppConstants.ApprovalLevel]) == 2)
                {
                    hlk1.Enabled = false;
                    hlk2.Enabled = true;
                }
            }
        }

        protected void lnkL1App_Click(object sender, EventArgs e)
        {
            LinkButton lnkRowSelection = (LinkButton)sender;
            string[] arguments = lnkRowSelection.CommandArgument.Split(';');
            Session[AppConstants.PRWhsCode] = arguments[0];
            Response.Redirect(AppConstants.MaterialRequestApprovalNewURL);
        }

        protected void lnkL2App_Click(object sender, EventArgs e)
        {
            LinkButton lnkRowSelection = (LinkButton)sender;
            string[] arguments = lnkRowSelection.CommandArgument.Split(';');
            Session[AppConstants.PRWhsCode] = arguments[0];
            Response.Redirect(AppConstants.MaterialRequestApprovalNewURL);
        }
    }
}