<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InventoryTransfer.aspx.cs"
    Inherits="AE_HarrysWeb_V001.InventoryTransfer" MasterPageFile="~/Main.Master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style17
        {
            width: 450px;
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
                    <asp:Label ID="lblTitle" runat="server" Text="Inventory Transfer"></asp:Label>
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
                                <asp:DropDownList ID="ddlFromOutlet" runat="server" AutoPostBack="true" CssClass="dropdownlist" OnSelectedIndexChanged = "ddlFromOutlet_SelectedIndexChanged"
                                    Style="width: 450px; margin-left: 0px;" TabIndex = "1" Visible="True">
                                </asp:DropDownList>
                            </td>
                            <td class="style20">
                                To Outlet
                            </td>
                            <td>
                                :
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlToOutlet" runat="server" AutoPostBack="true" CssClass="dropdownlist" OnSelectedIndexChanged = "ddlToOutlet_SelectedIndexChanged"
                                    Style="width: 450px;" TabIndex = "2" Visible="True">
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
                                <asp:TextBox ID="txtSubmittedBy" runat="server" TabIndex = "3" CssClass="txtbox"
                                    MaxLength="150"></asp:TextBox>
                            </td>
                            <td class="style19">
                                Approved By
                            </td>
                            <td>
                                :
                            </td>
                            <td class="style17">
                                <asp:TextBox ID="txtApprovedBy" runat="server" TabIndex = "4" CssClass="txtbox"
                                    MaxLength="150"></asp:TextBox>
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
                                <asp:TextBox ID="txtTransferDate" runat="server" MaxLength="10"
                                    CssClass="txtbox" OnKeyPress="return isNumberKey(this, event);"></asp:TextBox>
                                <asp:ImageButton ID="Image1" runat="Server" Style="width: 15px;" AlternateText="Click to show calendar"
                                    ImageUrl="Images/Calender.jpg" />
                                <cc1:CalendarExtender ID="CalendarExtender1" runat="server" OnClientDateSelectionChanged="checkDate"
                                    TargetControlID="txtTransferDate" PopupButtonID="Image1" Format="dd/MM/yyyy">
                                </cc1:CalendarExtender>
                                &nbsp; &nbsp;
                                <asp:Button ID="btnItemSearch" runat="server" Text = "Item Search" OnClick="btnItemSearch_Click"
                                    Style="background-image: url('Images/bgButton.png');
                                background-repeat: no-repeat; color: White;" BorderStyle="Solid"
                                Height="28px"/>
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
                        AllowPaging="true" PageSize="50">
                        <PagerSettings Mode="NumericFirstLast" />
                        <PagerStyle BackColor="#1B3B5F" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle" />
                        <%-- <PagerStyle BackColor="#70A0D0" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle" />--%>
                        <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px" />
                        <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                        <Columns>
                            <asp:TemplateField HeaderText="#">
                                <ItemStyle HorizontalAlign="Center" Width="20px" />
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <asp:Label ID="lblNo" runat="server" Text='<%# Bind("No") %>' BorderStyle="none"> </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Item">
                                <ItemStyle HorizontalAlign="left" Width="80px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblItem" runat="server" Text='<%# Bind("Item") %>' BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="UoM">
                                <ItemStyle HorizontalAlign="left" Width="80px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblUoM" runat="server" Text='<%# Bind("UoM") %>' BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quantity">
                                <ItemStyle HorizontalAlign="left" Width="80px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:TextBox ID="txtOrderQuantity" runat="server" Width="97%" Text='<%# Eval("OrderQuantity")%>'
                                        CssClass="txtbox" AutoPostBack="true" TabIndex="1" Style="text-align: right" />
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
                                    <th style="width: 400px;">
                                        <span>Item</span>
                                    </th>
                                    <th>
                                        <span>UoM</span>
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
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit" Style="background-image: url('Images/bgButton.png');
                                background-repeat: no-repeat; width: 85px; color: White;" BorderStyle="Solid"
                                Height="28px" Width="85px" OnClick = "btnSubmit_Click"/>
                        </td>
                    </tr>
                </table>
                <!-- This is for the Pop up on clicking the Item Search Button -->
                <asp:Button ID="btnHiddenOpen" runat="server" Style="display: none" />
                <cc1:ModalPopupExtender ID="mpePopup" runat="server" TargetControlID="btnHiddenOpen"
                    PopupControlID="panShow">
                </cc1:ModalPopupExtender>
                <asp:Panel ID="panShow" runat="server" Style="width: 500px; height: 530px; background-color: #F8F8F8;
                    border: 2px solid #C8C8C8">
                    <contenttemplate>
                        <table id="tblHead" style="width:494px; height:20px; background-color: #CCCCCC; 
                        margin-left :2px;margin-right:2px;margin-top:2px;margin-bottom:2px; border-style: solid;border-width: 1px; font-weight:bold;" >
                            <tr>
                            <td>Item Search</td>
                            </tr>
                        </table>
                        <table>
                        <tr>
                        <td> Item Search </td>
                        <td> : </td>
                        <td><asp:TextBox ID="txtSearch" runat="server" CssClass="txtbox" AutoPostBack="true"
                        ToolTip="Search Items..." ></asp:TextBox>
                        &nbsp;
                        <asp:Button ID="btnPopUpSearch" runat= "server" Text = "::" Style = "text-align:justify;"
                        Height="22px" Width="20px"/>
                        </td>
                        </tr>
                        </table>
                        <div style="width: 500px; height:420px; overflow:auto; border: 1px;" cellspacing="0">
                        <table>
                            <tr>
                            <td colspan="2" style="text-align: center; ">
                            <asp:GridView ID="grvItemList" runat="server" CssClass="GridInner" Width="480px" BorderColor="White"
                            BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                            HeaderStyle-Height="27px">
                            <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px" />
                            <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" Height="25px"/>

                            <Columns>
                                <asp:TemplateField HeaderText="#" SortExpression="CardCode">
                                    <ItemStyle HorizontalAlign="Center" Width="13%" />
                                    <HeaderStyle VerticalAlign="Middle" />
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkItems" runat ="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Item Name" SortExpression="CardName">
                                    <ItemStyle HorizontalAlign="left" Width="50%" />
                                    <HeaderStyle VerticalAlign="Middle" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblItemDesc" runat="server" Text='<%# Bind("Description") %>'  BorderStyle="none"> </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Item Code" SortExpression="CardCode">
                                    <ItemStyle HorizontalAlign="Center" Width="50%" />
                                    <HeaderStyle VerticalAlign="Middle" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblItemCode" runat="server"  Text='<%# Bind("ItemCode") %>'  BorderStyle="none"> </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="UoM"  Visible = "false">
                                    <HeaderStyle VerticalAlign="Middle" />
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblUoM" runat="server" Text='<%# Bind("UOM") %>' BorderStyle="none" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>

                            <PagerStyle BackColor="#1B3B5F" Font-Bold="True" ForeColor="White" HorizontalAlign="Right" VerticalAlign="Middle" />
                            <SelectedRowStyle BackColor="LightCyan" Font-Bold="true" ForeColor="DarkBlue" />
                            <HeaderStyle BackColor="#1B3B61" Font-Bold="true" ForeColor="#ffffff" Font-Overline="False"
                            Height="27px" VerticalAlign="Bottom" />
                            <EmptyDataTemplate> <b>No Data Found. </b></EmptyDataTemplate>
                            </asp:GridView>
                            </td>
                            </tr>
                            <tr>
                            <td colspan="2" style="text-align: center;"></td>
                            </tr>
                        </table>
                        </div>
                        <table style="width: 100%; margin-top:12px;">
                        <tr align="right">
                        <td></td>
                        <td style="width: 450px;"><asp:Button ID="btnSubmitItems" runat="server" Text="Choose Items" OnClick="btnSubmitItems_Click"
                        Style="background-image: url('Images/bgButton.png'); text-align:center;
                        background-repeat: no-repeat; width: 100px; color: White;" BorderStyle="Solid" />
                        <asp:Button ID="btnClose" runat="server" Text="Close" Style="background-image: url('Images/bgButton.png');
                        background-repeat: no-repeat; width: 85px; color: White;" BorderStyle="Solid" />
                        </td>
                        </tr>
                        </table>
                    </contenttemplate>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
