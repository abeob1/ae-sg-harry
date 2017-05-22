using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AE_HarrysWeb_V001.Utils;

namespace AE_HarrysWeb_V001
{
    public partial class Homepage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[Utils.AppConstants.UserCode] != null)
            {
                if (!IsPostBack)
                {
                    this.lblCompany.Text = string.Format("{0} | {1} ", Session[Utils.AppConstants.UserCode].ToString().ToUpper(),
                                                        Session[Utils.AppConstants.CompanyName].ToString());
                    this.lblDate.Text = DateTime.Now.ToString(Utils.AppConstants.DATE);
                    EnableandDisable();
                    mouseOverandMouseOut();
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }

        protected void btnLogOut_Click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx");
        }

        protected void EnableandDisable()
        {
            // Hide and Show for the sub menu items based on ApprovalLevel
            if (Convert.ToInt16(Session[AppConstants.ApprovalLevel].ToString()) == 0)
            {
                methodVisibility(false);
            }
            else
            {
                methodVisibility(true);
            }
        }

        protected void methodVisibility(bool value)
        {
            Submenu4.Visible = value; // Pending approval
            Submenu8.Visible = value; // Stocktake approval
            if (value == false)
            {
            Submenu10.Visible = true; // Inventory Transfer Request
            }
            else
            {
                Submenu10.Visible = false; // Inventory Transfer Request
            }
        }

        public void mouseOverandMouseOut()
        {
            btnLogOut.Attributes.Add("class", "static");
            btnLogOut.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnLogOut.Attributes.Add("onMouseOut", "this.className='static'");
        }
    }
}