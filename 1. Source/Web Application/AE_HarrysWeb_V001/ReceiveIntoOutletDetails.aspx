<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReceiveIntoOutletDetails.aspx.cs"
    Inherits="AE_HarrysWeb_V001.ReceiveIntoOutletDetails" MasterPageFile="~/Main.Master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style16
        {
            width: 166px;
        }
        .style17
        {
            width: 8px;
        }
        .style18
        {
            width: 67px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">
        function isNumberKey(sender, evt) {
            var txt = sender.value;
            var dotcontainer = txt.split('.');
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (!(dotcontainer.length == 1 && charCode == 46) && charCode > 31 && (charCode < 48 || charCode > 57))
                return false;

            return true;
        }
        
    </script>
    <div>
        <asp:UpdatePanel ID="updatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div style="height: 5px;">
                </div>
                <h2>
                    <asp:Label ID="lblTitle" runat="server" Text="Receive into Outlet Details "></asp:Label>
                </h2>
                <div style="margin-left: 5px; width: 99%;">
                    <hr />
                    <table border="0" width="100%" style="background-color: #D1D4D8; font-weight: bold;">
                        <tr>
                            <td class="style16">
                                User Name
                            </td>
                            <td class="style17">
                                :
                            </td>
                            <td>
                                <asp:Label ID="lblUserName" runat="server"></asp:Label>
                            </td>
                            <td align="right" rowspan="4">
                                <img src="Images/Logo_Small.png" alt="Harrys Logo" />
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td class="style16">
                                Outlet
                            </td>
                            <td class="style17">
                                :
                            </td>
                            <td>
                                <asp:Label ID="lblWareHouse" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="style16">
                                Receipt Date
                            </td>
                            <td class="style17">
                                :
                            </td>
                            <td>
                                <asp:Label ID="lblReceiptDate" runat="server" Visible="false"></asp:Label>
                                <asp:TextBox ID="txtReceiptDate" runat="server" AutoPostBack="True" CssClass="txtbox"
                                    MaxLength="10" OnKeyPress="return isNumberKey(this, event);" Style="text-align: left;
                                    text-indent: 5px;"></asp:TextBox>
                                <asp:ImageButton ID="Image1" runat="Server" Style="width: 15px;" AlternateText="Click to show calendar"
                                    ImageUrl="Images/Calender.jpg" />
                                <cc1:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtReceiptDate"
                                    PopupButtonID="Image1" Format="dd/MM/yyyy">
                                </cc1:CalendarExtender>
                            </td>
                        </tr>
                    </table>
                    <hr />
                    <table width="100%" style="font-weight: bold;">
                        <tr>
                            <td style="width: 10px">
                                Supplier
                            </td>
                            <td style="width: 10px">
                                :
                            </td>
                            <td>
                                <asp:Label ID="lblSupplier" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblSupplierCode" runat="server" Visible="false"></asp:Label>
                                <asp:Label ID="lblNoOfOpenPO" runat="server" Visible="false"></asp:Label>
                            </td>
                            <td class="style18">
                                Search PO
                            </td>
                            <td style="width: 10px">
                                :
                            </td>
                            <td>
                                <asp:TextBox ID="txtPOSearch" runat="server" OnKeyPress="return isNumberKey(this, event);"
                                    MaxLength="10"></asp:TextBox>
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Button ID="btnPOSearch" runat="server" Text="Search" Style="background-image: url('Images/bgButton.png');
                                    background-repeat: no-repeat; width: 85px; color: White;" BorderStyle="Solid"
                                    OnClick="btnPOSearch_Click" />
                            </td>
                            <td>
                            </td>
                        </tr>
                    </table>
                    <div style="width: 100%;">
                        <table border="0" style="background-color: #D1D4D8; font-weight: bold;">
                            <asp:GridView ID="grvRIOD" runat="server" CssClass="GridInner" Width="100%" BorderColor="White"
                                BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                                HeaderStyle-Height="25px" CellSpacing="2" HeaderStyle-VerticalAlign="Middle"
                                OnRowDataBound="grvRIOD_OnRowDataBound" AllowPaging="true" PageSize="20" OnPageIndexChanging="grvRIOD_PageIndexChanging">
                                <PagerSettings Mode="NumericFirstLast" />
                                <PagerStyle BackColor="#1B3B5F" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle"
                                    CssClass="pager-row" />
                                <%-- <PagerStyle BackColor="#70A0D0" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle" />--%>
                                <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px"
                                    CssClass="row" />
                                <FooterStyle BackColor="#7E7E7E" Font-Bold="True" ForeColor="White" />
                                <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                                <Columns>
                                    <asp:TemplateField HeaderText="LineNum#" Visible="false">
                                        <HeaderStyle VerticalAlign="Middle" />
                                        <ItemStyle HorizontalAlign="left" Width="5px" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblLineNum" runat="server" Text='<%# Bind("LineNum") %>' BorderStyle="none">
                                            </asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="PO#">
                                        <HeaderStyle VerticalAlign="Middle" />
                                        <ItemStyle HorizontalAlign="left" Width="5px" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblDocNum" runat="server" Text='<%# Bind("DocNum") %>' BorderStyle="none">
                                            </asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="PO#" Visible="false">
                                        <HeaderStyle VerticalAlign="Middle" />
                                        <ItemStyle HorizontalAlign="left" Width="5px" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblDocEntry" runat="server" Text='<%# Bind("DocEntry") %>' BorderStyle="none">
                                            </asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Description" ItemStyle-Width="50px">
                                        <HeaderStyle VerticalAlign="Middle" />
                                        <ItemStyle HorizontalAlign="left" Width="90px" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblDescription" runat="server" Text='<%# Bind("Dscription") %>' BorderStyle="none">
                                            </asp:Label>
                                            <%--<asp:Label ID="lblImageURL" Visible="false" runat="server" Text='<%# Bind("ImageURL") %>'
                                BorderStyle="none">
                            </asp:Label>
                            <asp:Image ID="ItemImage" runat="server" Width="20%" />--%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Item Image">
                                        <HeaderStyle VerticalAlign="Middle" />
                                        <ItemStyle HorizontalAlign="left" Width="30px" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblImageURL" Visible="false" runat="server" Text='<%# Bind("ImageURL") %>'
                                                BorderStyle="none">
                                            </asp:Label>
                                            <asp:Image ID="ItemImage" runat="server" Width="35%" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="ItemCode" Visible="false">
                                        <HeaderStyle VerticalAlign="Middle" />
                                        <ItemStyle HorizontalAlign="left" Width="50px" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblItemCode" runat="server" Text='<%# Bind("ItemCode") %>' BorderStyle="none">
                                            </asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Order Qty" ItemStyle-Width="50px">
                                        <HeaderStyle VerticalAlign="Middle" />
                                        <ItemStyle HorizontalAlign="left" Width="50px" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblOrderQty" runat="server" Text='<%# Bind("Quantity") %>' BorderStyle="none">
                                            </asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Receipt Quantity" ItemStyle-Width="50px">
                                        <ItemStyle HorizontalAlign="left" />
                                        <HeaderStyle VerticalAlign="Middle" Width="50px" />
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtReceiptQuantity" runat="server" CssClass="txtbox" Width="97%"
                                                OnKeyPress="return isNumberKey(this, event);" AutoPostBack="true" Text='<%# Eval("ReceiptQty")%>'
                                                OnTextChanged="txtReceiptQuantity_OnTextChanged" Style="text-align: right" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="ReasonCode" ItemStyle-Width="50px">
                                        <ItemStyle HorizontalAlign="left" />
                                        <HeaderStyle VerticalAlign="Middle" />
                                        <ItemTemplate>
                                            <asp:DropDownList ID="ddlReasonCode" runat="server" CssClass="dropdownlist" AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlReasonCode_SelectedIndexChanged" Width="97%">
                                            </asp:DropDownList>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <SelectedRowStyle BackColor="LightCyan" ForeColor="DarkBlue" Font-Bold="true" />
                                <HeaderStyle BackColor="#1B3B61" Font-Bold="true" ForeColor="#ffffff" Font-Overline="False"
                                    Height="25px" VerticalAlign="Bottom" />
                                <EmptyDataTemplate>
                                    <table class="GridInner" style="width: 100%; border-color: White;" border="1" rules="all"
                                        cellspacing="2" cellpadding="2">
                                        <tr valign="middle" style="height: 27px; color: white; font-weight: bold; text-decoration: none;
                                            background-color: rgb(27,59,97);">
                                            <th>
                                                <span>PO #</span>
                                            </th>
                                            <th>
                                                <span>Description</span>
                                            </th>
                                            <th>
                                                <span>ItemCode</span>
                                            </th>
                                            <th>
                                                <span>Order Qty</span>
                                            </th>
                                            <th>
                                                <span>Receipt Qty</span>
                                            </th>
                                            <th>
                                                <span>Reason Code</span>
                                            </th>
                                        </tr>
                                        <tr>
                                            <td colspan="12">
                                                <span>No Data</span>
                                            </td>
                                        </tr>
                                        <tr>
                                    </table>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </table>
                    </div>
                </div>
                <%--POP UP to show the alert for PO lines close--%>
                <asp:Button ID="btnHiddenOpen" runat="server" Style="display: none" />
                <cc1:ModalPopupExtender ID="mPOPopup" runat="server" TargetControlID="btnHiddenOpen"
                    PopupControlID="poPopUpShow">
                </cc1:ModalPopupExtender>
                <asp:Panel ID="poPopUpShow" runat="server" Style="width: 330px; height: 160px; background-color: #F8F8F8;
                    border: 2px solid #C8C8C8">
                    <contenttemplate>
                    <table id="tblHead" style="width:325px; height:20px; background-color: #CCCCCC; 
                           margin-left :2px;margin-right:2px;margin-top:2px;margin-bottom:2px; border-style: solid;border-width: 1px; font-weight:bold;" >
                        <tr>
                            <td>Alert</td>
                        </tr>
                    </table>
                    <div style="width: 330px; height:90px; overflow:auto; border: 1px;">
                            <table>
                                <tr>
                                    <td style='padding:15px 10px 5px 20px'><h3><b>Do you want to close the PO Lines ?</b></h3></td>
                                </tr>
                            </table>
                    </div>
                    <table style="width: 100%; margin-top:5px;">
                            <tr align="right">
                                <td></td>
                                <td style="width: 330px;"><asp:Button ID="btnPOAlert" runat="server" Text="Yes" OnClick="btnPOAlert_Click"
                                     Style="background-image: url('Images/bgButton.png'); text-align:center;
                                    background-repeat: no-repeat; width: 50px; color: White;" BorderStyle="Solid" />
                                    <asp:Button ID="btnPOCancel" runat="server" Text="No" OnClick="btnPOCancel_Click" Style="background-image: url('Images/bgButton.png');
                                    background-repeat: no-repeat; width: 60px; color: White;" BorderStyle="Solid" />
                                </td>
                            </tr>
                    </table>
                    </contenttemplate>
                </asp:Panel>
                <%--POP UP to show the alert for PO lines close--%>
                <asp:Button ID="btnHiddenOpen1" runat="server" Style="display: none" />
                <cc1:ModalPopupExtender ID="mINVPopup" runat="server" TargetControlID="btnHiddenOpen1"
                    PopupControlID="invPopUpShow">
                </cc1:ModalPopupExtender>
                <asp:Panel ID="invPopUpShow" runat="server" Style="width: 330px; height: 160px; background-color: #F8F8F8;
                    border: 2px solid #C8C8C8">
                    <contenttemplate>
                    <table id="Table1" style="width:325px; height:20px; background-color: #CCCCCC; 
                           margin-left :2px;margin-right:2px;margin-top:2px;margin-bottom:2px; border-style: solid;border-width: 1px; font-weight:bold;" >
                        <tr>
                            <td>Alert</td>
                        </tr>
                    </table>
                    <div style="width: 330px; height:90px; overflow:auto; border: 1px;">
                            <table>
                                <tr>
                                    <td style='padding:15px 10px 5px 20px'><h3><b>Do you want to close the Inventory Transfer Request Lines ?</b></h3></td>
                                </tr>
                            </table>
                    </div>
                    <table style="width: 100%; margin-top:5px;">
                            <tr align="right">
                                <td></td>
                                <td style="width: 300px;"><asp:Button ID="btnINVAlert" runat="server" Text="Yes" OnClick="btnINVAlert_Click"
                                     Style="background-image: url('Images/bgButton.png'); text-align:center;
                                    background-repeat: no-repeat; width: 50px; color: White;" BorderStyle="Solid" />
                                    <asp:Button ID="btnINVCancel" runat="server" Text="No"  OnClick="btnINVCancel_Click" Style="background-image: url('Images/bgButton.png');
                                    background-repeat: no-repeat; width: 60px; color: White;" BorderStyle="Solid" />
                                </td>
                            </tr>
                    </table>
                    </contenttemplate>
                </asp:Panel>
                <div>
                <table style="width: 100%">
                    <tr align="right">
                        <td>
                            <asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                        </td>
                        <td style="width: 400px;">
                            <asp:Button ID="btnAdd" runat="server" Text="Add" Style="background-image: url('Images/bgButton.png');
                                background-repeat: no-repeat; width: 85px; color: White;" BorderStyle="Solid"
                                OnClick="btnAdd_Click" />
                        </td>
                    </tr>
                </table>
            </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
