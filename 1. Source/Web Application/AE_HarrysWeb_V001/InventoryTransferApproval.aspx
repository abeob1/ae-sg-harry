<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InventoryTransferApproval.aspx.cs"
    Inherits="AE_HarrysWeb_V001.InventoryTransferApproval" MasterPageFile="~/Main.Master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style17
        {
            width: 250px;
        }
        .style19
        {
            width: 92px;
        }
        .style20
        {
            width: 68px;
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

        function checkDate(sender, args) {
            if (sender._selectedDate < new Date()) {
                alert("You cannot select a day earlier than today!");
                sender._selectedDate = new Date();
                // set the date back to the current date
                sender._textbox.set_Value(sender._selectedDate.format(sender._format))
            }
        }

        function dateValidation() {

            var obj = document.getElementById("<%=txtTransferDate.ClientID%>");
            var day = obj.value.split("/")[0];
            var month = obj.value.split("/")[1];
            var year = obj.value.split("/")[2];

            if ((day < 1 || day > 31) || (month < 1 && month > 12) && (year.length != 4)) {
                alert("Invalid Format"); return false;
            }
            else {
                var dt = new Date(year, month - 1, day);
                var today = new Date();
                if ((dt.getDate() != day) || (dt.getMonth() != month - 1) || (dt.getFullYear() != year) || (dt > today)) {
                    alert("Invalid Date"); return false;
                }
            }
        }
    </script>
    <div>
        <asp:UpdatePanel ID="updatePanel" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div style="height: 5px;">
                </div>
                <h2>
                    <asp:Label ID="lblTitle" runat="server" Text="Inventory Transfer Approval"></asp:Label>
                </h2>
                <div style="margin-left: 5px; width: 99%;">
                    <hr />
                    <table border="0" width="100%" style="background-color: #D1D4D8; font-weight: bold;">
                        <tr>
                            <td class="style19">
                                From Outlet
                            </td>
                            <td>
                                :
                            </td>
                            <td class="style17">
                                <asp:DropDownList ID="ddlFromOutlet" runat="server" AutoPostBack="true" CssClass="dropdownlist"
                                    Style="width: 250px; margin-left: 0px;" TabIndex="1" Visible="True" Enabled="false">
                                </asp:DropDownList>
                            </td>
                            <td class="style20">
                                To Outlet
                            </td>
                            <td>
                                :
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlToOutlet" runat="server" AutoPostBack="true" CssClass="dropdownlist"
                                    Style="width: 250px;" TabIndex="2" Visible="True" Enabled="false">
                                </asp:DropDownList>
                            </td>
                            <td>
                                &nbsp;
                            </td>
                            <td align="right" rowspan="3">
                                <img src="Images/Logo_Small.png" alt="Harrys Logo" />
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td class="style19">
                                Submitted By
                            </td>
                            <td>
                                :
                            </td>
                            <td class="style17">
                                <asp:TextBox ID="txtSubmittedBy" runat="server" TabIndex="3" CssClass="txtbox" MaxLength="150"
                                    Style="text-align: left; text-indent: 5px;" Enabled="false"></asp:TextBox>
                            </td>
                            <td class="style19">
                                Remarks
                            </td>
                            <td>
                                :
                            </td>
                            <td class="style17" rowspan ="2">
                                <asp:TextBox ID="txtRemarks" runat="server" TabIndex="4" CssClass="txtbox" MaxLength="150"
                                    Style="text-align: left; text-indent: 5px; height : 45px;width: 380px;" Enabled="false"></asp:TextBox>
                            </td>
                            <td class="style20">
                                &nbsp;
                            </td>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="style19">
                                Transfer Date
                            </td>
                            <td>
                                :
                            </td>
                            <td class="style17">
                                <asp:TextBox ID="txtTransferDate" runat="server" MaxLength="10" CssClass="txtbox"
                                    OnKeyPress="return isNumberKey(this, event);" Style="text-align: left; text-indent: 5px;"
                                    Enabled="false"></asp:TextBox>
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                    <hr />
                </div>
                <div style="width: 100%">
                    <asp:GridView ID="grvInvTransfer" runat="server" CssClass="GridInner" Width="100%"
                        BorderColor="White" BackColor="White" AllowSorting="True" AutoGenerateColumns="False"
                        CellPadding="2" HeaderStyle-Height="25px" CellSpacing="2" HeaderStyle-VerticalAlign="Middle"
                        AllowPaging="true" PageSize="50" OnPageIndexChanging="grvInvTransfer_PageIndexChanging">
                        <PagerSettings Mode="NumericFirstLast" />
                        <PagerStyle BackColor="#1B3B5F" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle"
                            CssClass="pager-row" />
                        <%-- <PagerStyle BackColor="#70A0D0" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle" />--%>
                        <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px"
                            CssClass="row" />
                        <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                        <Columns>
                            <asp:TemplateField HeaderText="#" Visible="false">
                                <ItemStyle HorizontalAlign="Center" Width="20px" />
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <asp:Label ID="lblNo" runat="server" BorderStyle="none" Text='<%# Bind("No") %>'> </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="ItemCode" Visible="false">
                                <ItemStyle HorizontalAlign="left" Width="80px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblItemCode" runat="server" BorderStyle="none" Text='<%# Bind("ItemCode") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Item">
                                <ItemStyle HorizontalAlign="left" Width="280px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblDescription" runat="server" BorderStyle="none" Text='<%# Bind("Description") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Sales UoM">
                                <ItemStyle HorizontalAlign="left" Width="20px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblUoM" runat="server" BorderStyle="none" Text='<%# Bind("UOM") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="InStock">
                                <ItemStyle HorizontalAlign="left" Width="20px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblInStock" runat="server" BorderStyle="none" Text='<%# Bind("InStock") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quantity">
                                <ItemStyle HorizontalAlign="left" Width="20px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblOrderQuantity" runat="server" BorderStyle="none" Text='<%# String.Format("{0,-15:#,##0.00}", Eval("OrderQuantity"))%>' />
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
                                    <th style="width: 300px;">
                                        <span>Item</span>
                                    </th>
                                    <th>
                                        <span>Sales UoM</span>
                                    </th>
                                    <th>
                                        <span>InStock</span>
                                    </th>
                                    <th style="width: 400px;">
                                        <span>Quantity</span>
                                    </th>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <span>No Data</span>
                                    </td>
                                </tr>
                            </table>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
                <hr />
                <table style="width: 100%">
                    <tr align="right">
                        <td>
                            <asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                        </td>
                        <td style="width: 400px;">
                            <asp:Button ID="btnApprove" runat="server" Text="Approve" Style="background-image: url('Images/bgButton.png');
                                background-repeat: no-repeat; width: 85px; color: White;" BorderStyle="Solid"
                                Height="28px" Width="85px" OnClick="btnApprove_Click" />
                            <asp:Button ID="btnReject" runat="server" Text="Reject" Style="background-image: url('Images/bgButton.png');
                                background-repeat: no-repeat; width: 85px; color: White;" BorderStyle="Solid"
                                Height="28px" Width="85px" OnClick="btnReject_Click" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
