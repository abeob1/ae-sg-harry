using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using AE_HarrysWeb_V001.Utils;
using AE_Harrys.Common;

namespace AE_HarrysWeb_V001
{
    public partial class Recieve_Into_Outlet : System.Web.UI.Page
    {
        //clsLog oLog = new clsLog();
        //string sErrDesc = string.Empty;
        //string sFuncName = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (Session[Utils.AppConstants.UserCode].ToString() != string.Empty)
                {
                    if (!IsPostBack)
                    {
                        //sFuncName = "RecieveIntoOutlet_PageLoad";
                        //if (AppConstants.p_iDebugMode == AppConstants.DEBUG_ON) oLog.WriteToLogFile_Debug("Starting function", sFuncName);
                        lblReceiptDate.Text = DateTime.Now.ToString();
                        Session[AppConstants.IsBackPage] = 0;
                        lblUserName.Text = Session[Utils.AppConstants.UserCode].ToString().ToUpper();
                        //if (AppConstants.p_iDebugMode == AppConstants.DEBUG_ON) oLog.WriteToLogFile_Debug("Calling LoadDropDownData()", sFuncName);
                        loadDropdowndata();
                        //if (AppConstants.p_iDebugMode == AppConstants.DEBUG_ON) oLog.WriteToLogFile_Debug("Completed LoadDropDownData() With SUCCESS", sFuncName);
                        //if (AppConstants.p_iDebugMode == AppConstants.DEBUG_ON) oLog.WriteToLogFile_Debug("Loading the selected outlet in session variable", sFuncName);
                        Session["wareHouseCode"] = ddlWareHouse.SelectedValue;
                        //if (AppConstants.p_iDebugMode == AppConstants.DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS", sFuncName);

                        //if (AppConstants.p_iDebugMode == AppConstants.DEBUG_ON) oLog.WriteToLogFile_Debug("Calling mouseOverandMouseOut()", sFuncName);
                        mouseOverandMouseOut();
                        grvRIO.DataBind();
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
                Response.Redirect("~/ErrorPage.aspx");
                //sErrDesc = ex.Message.ToString();
                //if (AppConstants.p_iDebugMode == AppConstants.DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                //if (AppConstants.p_iDebugMode == AppConstants.DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);

            }
        }

        protected void rbnOulets_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlWareHouse.Text != " --- Select Outlet --- ")
                {
                    //sFuncName = "RecieveIntoOutlet_RadioButtonEvent";
                    //if (AppConstants.p_iDebugMode == AppConstants.DEBUG_ON) oLog.WriteToLogFile_Debug("Starting function", sFuncName);
                    txtSearch.Text = string.Empty;
                    if (rbnCK.Checked)
                    {
                        lblError.Text = string.Empty;
                        Session[AppConstants.SupplierType] = "I";
                        Session["CKRIO"] = null;
                        var Ws = new MasterService.MasterSoapClient();
                        //if (AppConstants.p_iDebugMode == AppConstants.DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Webmethod Get_OpenPOList()", sFuncName);
                        DataSet ds = Ws.Get_OpenPOList(Session[AppConstants.DBName].ToString(), "", "I", Session[AppConstants.UserRole].ToString(), ddlWareHouse.SelectedValue.ToString());                        //if (AppConstants.p_iDebugMode == AppConstants.DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS on Webmethod call 'Get_OpenPOList'", sFuncName);
                        if (ds != null)
                        {
                            DataTable dt = ds.Tables[0];
                            Session["CKRIO"] = dt;
                            grvRIO.DataSource = dt;

                            grvRIO.DataBind();
                        }
                        else
                        {
                            grvRIO.DataBind();
                        }
                        Session[AppConstants.ReceiveMessage] = 0;
                        Session[AppConstants.IsBackPage] = 3;
                        txtSearch.Enabled = false;
                        btnSearch.Enabled = false;
                        lblError.Visible = false;
                        lblError.Text = string.Empty;
                    }

                    else if (rbnCS.Checked)
                    {
                        lblError.Text = string.Empty;
                        Session[AppConstants.SupplierType] = "E";
                        Session["CSRIO"] = null;
                        var Ws = new MasterService.MasterSoapClient();
                        //if (AppConstants.p_iDebugMode == AppConstants.DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Webmethod Get_OpenPOList()", sFuncName);
                        DataSet ds = Ws.Get_OpenPOList(Session[AppConstants.DBName].ToString(), txtSearch.Text.ToString() + "%", "E", Session[AppConstants.UserRole].ToString(), ddlWareHouse.SelectedValue.ToString());
                        //if (AppConstants.p_iDebugMode == AppConstants.DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS on Webmethod call 'Get_OpenPOList'", sFuncName);
                        if (ds != null)
                        {
                            DataTable dt = ds.Tables[0];
                            Session["CSRIO"] = dt;
                            grvRIO.DataSource = dt;
                            grvRIO.DataBind();
                        }
                        else
                        {
                            grvRIO.DataBind();
                        }
                        Session[AppConstants.ReceiveMessage] = 1;
                        Session[AppConstants.IsBackPage] = 4;
                        btnSearch.Enabled = true;
                        txtSearch.Enabled = true;
                    }
                }
                else
                {
                    lblError.Text = "Kindly select the outlet.";
                    lblError.Visible = true;
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = "problem in loading the data.." + ex.Message;
                this.lblError.Visible = true;
                //sErrDesc = ex.Message.ToString();
                //if (AppConstants.p_iDebugMode == AppConstants.DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                //if (AppConstants.p_iDebugMode == AppConstants.DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
            }
        }

        protected void txtSearch_OnTextChanged(object sender, EventArgs e)
        {
            btnSearch_Click(this, new System.EventArgs());
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (rbnCS.Checked)
                {
                    var searchKey = txtSearch.Text;
                    DataTable dt = (DataTable)Session["CSRIO"];

                    if (searchKey != string.Empty)
                    {
                        dt.DefaultView.RowFilter = "CardName LIKE '%" + searchKey.ToUpper() + "%'";
                        dt = dt.DefaultView.ToTable();
                    }
                    else
                    {
                        dt.DefaultView.RowFilter = "";
                        dt = dt.DefaultView.ToTable();
                    }

                    if (dt != null)
                    {
                        grvRIO.DataSource = dt;
                        grvRIO.DataBind();

                    }
                    else
                    {
                        grvRIO.DataSource = new DataTable();
                        grvRIO.DataBind();
                    }
                }

                txtSearch.Focus();
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }

        protected void ddlWareHouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblError.Text = string.Empty;
            lblError.Visible = false;
            rbnCK.Checked = false;
            rbnCS.Checked = false;
            grvRIO.DataSource = new DataTable();
            grvRIO.DataBind();
            if (ddlWareHouse.SelectedValue == "01CKT")
            {
                rbnCK.Enabled = false;
            }
            else
            {
                rbnCK.Enabled = true;
            }
            //sFuncName = "RcvIntoOutlet_ddlEvent";
            //if (AppConstants.p_iDebugMode == AppConstants.DEBUG_ON) oLog.WriteToLogFile_Debug("Starting function", sFuncName);
            //if (AppConstants.p_iDebugMode == AppConstants.DEBUG_ON) oLog.WriteToLogFile_Debug("Loading the selected outlet in session variable", sFuncName);
            Session["wareHouseCode"] = ddlWareHouse.SelectedValue;
            //if (AppConstants.p_iDebugMode == AppConstants.DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS", sFuncName);
        }

        protected void grvRIO_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (rbnCS.Checked)
            {
                this.grvRIO.PageIndex = e.NewPageIndex;
                DataTable tblIRIO = (DataTable)Session["CSRIO"];
                BindData(tblIRIO);
            }
            else if (rbnCK.Checked)
            {
                this.grvRIO.PageIndex = e.NewPageIndex;
                DataTable tblIRIO = (DataTable)Session["CKRIO"];
                BindData(tblIRIO);
            }
        }

        private void BindData(DataTable tblIRIO)
        {
            if (rbnCS.Checked)
            {
                Session["CSRIO"] = tblIRIO;
                DataView dv = tblIRIO.DefaultView;
                this.grvRIO.DataSource = dv.ToTable();
                this.grvRIO.DataBind();
            }
            else if (rbnCK.Checked)
            {
                Session["CKRIO"] = tblIRIO;
                DataView dv = tblIRIO.DefaultView;
                this.grvRIO.DataSource = dv.ToTable();
                this.grvRIO.DataBind();

            }
        }

        private void loadDropdowndata()
        {
            try
            {
                //sFuncName = "RecieveIntoOutlet_loadDropdowndata";
                //if (AppConstants.p_iDebugMode == AppConstants.DEBUG_ON) oLog.WriteToLogFile_Debug("Starting function", sFuncName);
                var data = new MasterService.MasterSoapClient();
                //if (AppConstants.p_iDebugMode == AppConstants.DEBUG_ON) oLog.WriteToLogFile_Debug("Calling Webmethod Get_Outlet_Details()", sFuncName);
                DataSet dsOutlet = data.Get_Outlet_Details(Session[Utils.AppConstants.SUPERUSER].ToString(), Session[Utils.AppConstants.WhsCode].ToString(),
                    Convert.ToInt16(Session[AppConstants.ApprovalLevel]), Session[AppConstants.U_ConnString].ToString());
                //if (AppConstants.p_iDebugMode == AppConstants.DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With SUCCESS on Webmethod call 'Get_Outlet_Details'", sFuncName);
                if (dsOutlet.Tables.Count != 0 && dsOutlet != null)
                {
                    ddlWareHouse.DataSource = dsOutlet.Tables[0];
                    ddlWareHouse.DataTextField = dsOutlet.Tables[0].Columns[AppConstants.WhsName].ColumnName.ToString();
                    ddlWareHouse.DataValueField = dsOutlet.Tables[0].Columns[AppConstants.WhsCode].ColumnName.ToString();
                    ddlWareHouse.DataBind();
                }
                else
                {
                    //if (AppConstants.p_iDebugMode == AppConstants.DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                    //if (AppConstants.p_iDebugMode == AppConstants.DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
                    this.lblError.Text = "Failed to Load Outlet list.";
                    this.lblError.Visible = true;
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = "Error Occured while loading.." + ex.Message;
                this.lblError.Visible = true;

                //sErrDesc = ex.Message.ToString();
                //if (AppConstants.p_iDebugMode == AppConstants.DEBUG_ON) oLog.WriteToLogFile(sErrDesc, sFuncName);
                //if (AppConstants.p_iDebugMode == AppConstants.DEBUG_ON) oLog.WriteToLogFile_Debug("Completed With ERROR  ", sFuncName);
            }
        }

        public void mouseOverandMouseOut()
        {
            btnSearch.Attributes.Add("class", "static");
            btnSearch.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnSearch.Attributes.Add("onMouseOut", "this.className='static'");

            btnSearch.Attributes.Add("class", "static");
            btnSearch.Attributes.Add("onMouseOver", "this.className='hoverbutton'");
            btnSearch.Attributes.Add("onMouseOut", "this.className='static'");
        }

    }
}