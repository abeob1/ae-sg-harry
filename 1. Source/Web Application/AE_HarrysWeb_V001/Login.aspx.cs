using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using AE_HarrysWeb_V001.MasterService;
using AE_HarrysWeb_V001.Utils;
//using AE_Harrys.Common;

namespace AE_HarrysWeb_V001
{
    public partial class Login : System.Web.UI.Page
    {
        public static string path;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    //clsLog.path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                    LoadCompany();
                    mouseOverandMouseOut();
                }
            }
            catch (Exception ex)
            {
                this.lblMessage.Text = ex.Message;
                this.lblMessage.Visible = true;
            }

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtUserName.Text) && !string.IsNullOrEmpty(txtPassword.Text))
                {
                    LoadServerCredentials();
                    var client = new MasterService.MasterSoapClient();
                    var result = client.Login(txtUserName.Text.Trim(), txtPassword.Text.Trim(), ddlCompany.SelectedValue.ToString(), Session[AppConstants.U_Server].ToString(),
                        Session[AppConstants.U_LicenseServer].ToString(), Session[AppConstants.U_DBUserName].ToString(), Session[AppConstants.U_DBPassword].ToString());
                    if (result == "SUCCESS")
                    {
                        var userDetail = client.Get_UserInformation(txtUserName.Text.Trim(), Session[AppConstants.U_ConnString].ToString());
                        if (userDetail != null && userDetail.Tables[0].Rows.Count != 0)
                        {
                            Session[AppConstants.UserCode] = userDetail.Tables[0].Rows[0]["EmployeeName"].ToString();
                            Session[AppConstants.CompanyName] = userDetail.Tables[0].Rows[0]["CompanyName"].ToString();
                            Session[AppConstants.DBName] = ddlCompany.SelectedValue.ToString();
                            Session[AppConstants.UserID] = userDetail.Tables[0].Rows[0]["EmpId"].ToString();
                            Session[AppConstants.UserName] = txtUserName.Text.ToString();//.Trim();
                            Session[AppConstants.Pwd] = txtPassword.Text.ToString();
                            Session[AppConstants.WhsCode] = userDetail.Tables[0].Rows[0]["WhsCode"].ToString();
                            Session[AppConstants.WhsName] = userDetail.Tables[0].Rows[0]["WhsName"].ToString();
                            Session[AppConstants.UserAccess] = userDetail.Tables[0].Rows[0]["U_AE_Access"].ToString();
                            Session[AppConstants.SUPERUSER] = userDetail.Tables[0].Rows[0]["SUPERUSER"].ToString();
                            Session[AppConstants.UserRole] = userDetail.Tables[0].Rows[0]["U_UserRole"].ToString();
                            Session[AppConstants.ApprovalLevel] = userDetail.Tables[0].Rows[0]["U_ApprovalLevel"];
                            if (Session[AppConstants.ApprovalLevel].ToString() == "0")
                            {
                                if (Session[AppConstants.WhsCode].ToString() != string.Empty && Session[AppConstants.WhsCode].ToString() != "" && 
                                    Session[AppConstants.WhsCode].ToString() != null)
                                {
                                    Response.Redirect(AppConstants.HomepageURL, false);
                                }
                                else
                                {
                                    this.lblMessage.Text = "Please Update the Outlet Code in SAP User Master !!!";
                                    clearFields();
                                }
                            }
                            else
                            {
                                Response.Redirect(AppConstants.HomepageURL, false);
                            }
                            
                        }
                        else
                        {
                            this.lblMessage.Text = "Access denied !!!";
                            clearFields();
                        }
                    }
                    else
                    {
                        this.lblMessage.Text = result;
                        clearFields();
                    }
                }
                else
                {
                    this.lblMessage.Text = "User ID or Password is incorrect.";
                    clearFields();
                }
            }
            catch (Exception ex)
            {
                this.lblMessage.Text = ex.Message.ToString();
                clearFields();
                //throw;
            }
        }

        private void LoadCompany()
        {
            try
            {
                // MasterService.MasterSoapClient client = new MasterService.MasterSoapClient("MasterSoap1");
                var client = new MasterService.MasterSoapClient();
                DataSet ds = client.Get_Company_Details();
                if (ds != null && ds.Tables.Count > 0)
                {
                    Session[AppConstants.UserInfo] = ds.Tables[0];
                    this.ddlCompany.DataSource = ds.Tables[0];
                    this.ddlCompany.DataTextField = "U_CompName";
                    this.ddlCompany.DataValueField = "U_DBName";
                    this.ddlCompany.DataBind();
                }
                else
                {
                    this.lblMessage.Text = "Please Configure the Initial Settings in this User";
                }
            }
            catch (Exception ex)
            {
                this.lblMessage.Text = "ERROR : " + ex.Message.ToString();
            }
        }

        public void clearFields()
        {
            txtUserName.Text = string.Empty;
            txtPassword.Text = string.Empty;
        }

        public void mouseOverandMouseOut()
        {
            btnLogin.Attributes.Add("class", "static");
            btnLogin.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnLogin.Attributes.Add("onMouseOut", "this.className='static'");
        }

        public void LoadServerCredentials()
        {
            DataTable dt = new DataTable();
            dt = (DataTable)Session[AppConstants.UserInfo];
            if (dt != null && dt.Rows.Count > 0)
            {
                var rows = from row in dt.AsEnumerable()
                           where row.Field<string>("U_DBName").Equals(ddlCompany.SelectedValue.ToString())
                           select row;
                if (rows.Count() > 0)
                {
                    DataTable newTable = new DataTable();
                    newTable = rows.CopyToDataTable();
                    if (newTable != null && newTable.Rows.Count > 0)
                    {
                        Session[AppConstants.U_DBName] = newTable.Rows[0]["U_DBName"].ToString();
                        Session[AppConstants.U_CompName] = newTable.Rows[0]["U_CompName"].ToString();
                        Session[AppConstants.U_SAPUserName] = newTable.Rows[0]["U_SAPUserName"].ToString();
                        Session[AppConstants.U_SAPPassword] = newTable.Rows[0]["U_SAPPassword"].ToString();
                        Session[AppConstants.U_DBUserName] = newTable.Rows[0]["U_DBUserName"].ToString();
                        Session[AppConstants.U_DBPassword] = newTable.Rows[0]["U_DBPassword"].ToString();
                        Session[AppConstants.U_ConnString] = newTable.Rows[0]["U_ConnString"].ToString();
                        Session[AppConstants.U_Server] = newTable.Rows[0]["U_Server"].ToString();
                        Session[AppConstants.U_LicenseServer] = newTable.Rows[0]["U_LicenseServer"].ToString();
                    }
                }
            }
        }
    }
}