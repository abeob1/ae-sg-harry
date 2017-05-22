using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AE_HarrysWeb_V001.Utils;

namespace AE_HarrysWeb_V001
{
    public partial class Main : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[Utils.AppConstants.UserCode] != null
                && Session[Utils.AppConstants.CompanyName] != null)
                //&& Session[Utils.AppConstants.CompanyCode] != null
                //&& Session[Utils.AppConstants.Pwd] != null
            {
                lblUser.Text = Session[Utils.AppConstants.UserCode].ToString().ToUpper();
                lblOutlet.Text = Session[Utils.AppConstants.CompanyName].ToString();
                lblUserRole.Text = Session[Utils.AppConstants.UserRole].ToString().ToUpper();
                lblApprovalLevel.Text = Session[Utils.AppConstants.ApprovalLevel].ToString().ToUpper();
            }
            else
            {
                Response.Redirect(AppConstants.LoginURL);
            }
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            try
            {
                Session.Abandon();
                Response.Redirect(AppConstants.LoginURL);
            }
            catch (Exception)
            {

            }
        }
        protected void lnkHome_Click(object sender, EventArgs e)
        {
            Response.Redirect(AppConstants.HomepageURL);
        }

        protected void lnkBack_Click(object sender, EventArgs e)
        {
            if (Session[AppConstants.IsBackPage] != null)
            {
                if (Convert.ToInt16(Session[AppConstants.IsBackPage]) == 1)
                {
                    Response.Redirect(AppConstants.ListOfMaterialRequestDraftURL);
                }
                else if (Convert.ToInt16(Session[AppConstants.IsBackPage]) == 2)
                {
                    Response.Redirect(AppConstants.ListOfMaterialRequestURL);
                }
                else if (Convert.ToInt16(Session[AppConstants.IsBackPage]) == 3)
                {
                    Response.Redirect(AppConstants.RecieveIntoOutletURL);
                }
                else if (Convert.ToInt16(Session[AppConstants.IsBackPage]) == 4)
                {
                    Response.Redirect(AppConstants.RecieveIntoOutletURL);
                }
                else if (Convert.ToInt16(Session[AppConstants.IsBackPage]) ==5)
                {
                    Response.Redirect(AppConstants.StocktakeApprovalURL);
                }
                else if (Convert.ToInt16(Session[AppConstants.IsBackPage]) == 7)
                {
                    Response.Redirect(AppConstants.ListOfInventoryTransferReqURL);
                }
                else if (Convert.ToInt16(Session[AppConstants.IsBackPage]) == 9)
                {
                    Response.Redirect(AppConstants.OutletListPendingApprovalURL);
                }
                else
                {
                    Response.Redirect(AppConstants.HomepageURL);
                }
            }
            else
            {
                Response.Redirect(AppConstants.HomepageURL);
            }
        }
    }
}