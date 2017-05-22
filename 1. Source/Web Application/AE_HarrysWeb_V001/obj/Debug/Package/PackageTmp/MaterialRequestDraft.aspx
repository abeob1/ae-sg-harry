<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MaterialRequestDraft.aspx.cs"
    Inherits="AE_HarrysWeb_V001.MaterialRequestDraft" MasterPageFile="~/Main.Master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style16
        {
            width: 178px;
        }
        .pop
        {
            background-color:transparent;
            position: absolute;
            top: -100px;
            left: -200px;
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
        <asp:UpdatePanel ID="updatePanel" runat="server">
            <ContentTemplate>
                <div style="height: 5px;">
                </div>
                <h2>
                    <asp:Label ID="lblTitle" runat="server" Text="Material Request Draft"></asp:Label>
                </h2>
                <div style="margin-left: 5px; width: 99%;">
                    <hr />
                    <table border="0" width="100%" style="background-color: #D1D4D8; font-weight: bold;">
                        <tr>
                            <td style="width: 200px">
                                Outlet
                            </td>
                            <td>
                                :
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlWareHouse" runat="server" AutoPostBack="true" CssClass="dropdownlist"
                                    Style="width: 450px;" Visible="false">
                                </asp:DropDownList>
                                <asp:Label ID="lblWareHouse" runat="server"></asp:Label>
                                <asp:Label ID="lblWareHouseCode" runat="server" Visible="false"></asp:Label>
                            </td>
                            <td align="right" rowspan="3">
                                <img src="Images/Logo_Small.png" alt="Harrys Logo" />
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                User Name
                            </td>
                            <td>
                                :
                            </td>
                            <td>
                                <asp:Label ID="lblUserName" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Order Date
                            </td>
                            <td>
                                :
                            </td>
                            <td>
                                <asp:Label ID="lblOrderDate" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Total Outlet Spend FOR ORDER
                            </td>
                            <td>
                                :
                            </td>
                            <td>
                                <asp:Label ID="lblTotalOutlet" runat="server"></asp:Label>
                            </td>
                            <td align="right" style="padding-right: 3%; font-size: medium;">
                                <asp:Label ID="lblDraftNo" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Status
                            </td>
                            <td>
                                :
                            </td>
                            <td>
                                <asp:Label ID="lblStatus" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblPriority" runat="server">Urgent</asp:Label>
                            </td>
                            <td>
                                :
                            </td>
                            <td>
                                <asp:CheckBox ID="chkPriority" runat="server" />
                            </td>
                        </tr>
                    </table>
                    <hr />
                </div>
                <table width="100%" style="font-weight: bold;">
                    <tr>
                        <td style="width: 205px">
                            Supplier
                        </td>
                        <td style="width: 15px">
                            :
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlMainSupplier" runat="server" AutoPostBack="true" CssClass="dropdownlist"
                                Style="width: 450px;" OnSelectedIndexChanged="ddlMainSupplier_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:Button ID="btnAddItems" runat="server" Text="Add Items" OnClick="btnAddItems_Click"
                                Style="background-image: url('Images/bgButton.png'); background-repeat: no-repeat;
                                width: 85px; color: White;" BorderStyle="Solid" />
                        </td>
                        <td style="width: 100px">
                            Item Search
                        </td>
                        <td>
                            :
                        </td>
                        <td class="style16">
                            <asp:TextBox ID="txtMainSearch" runat="server" CssClass="txtbox" AutoPostBack="true"
                                ToolTip="Search Items..." OnTextChanged="txtMainSearch_OnTextChanged"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Button ID="btnMainSearch" runat="server" Text="::" Style="text-align: center;"
                                OnClick="btnMainSearch_Click" Width="22px" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkDeliveryCharge" runat="server" Text="Add Delivery Charge if Minimum Spend not meet"
                                OnCheckedChanged="chkDeliveryCharge_OnCheckedChanged" AutoPostBack="true" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblDeliveryCalender" runat="server">Delivery Calendar</asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblSeperator" runat="server">:</asp:Label>
                        </td>
                        <td>
                            <asp:CheckBoxList ID="chkCalendar" runat="server" RepeatDirection="Horizontal" Enabled="False"
                                Font-Bold="True">
                                <asp:ListItem>Mon</asp:ListItem>
                                <asp:ListItem>Tue</asp:ListItem>
                                <asp:ListItem>Wed</asp:ListItem>
                                <asp:ListItem>Thu</asp:ListItem>
                                <asp:ListItem>Fri</asp:ListItem>
                                <asp:ListItem>Sat</asp:ListItem>
                                <asp:ListItem>Sun</asp:ListItem>
                            </asp:CheckBoxList>
                        </td>
                        <td>
                            <asp:Label ID="lblDate" runat="server">Delivery Date</asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblSeperator1" runat="server">:</asp:Label>
                        </td>
                        <td class="style16">
                            <asp:Label ID="lblDeliveryDate" runat="server"></asp:Label>
                        </td>
                        <td>
                        </td>
                    </tr>
                </table>
                <div style="width: 100%">
                    <asp:GridView ID="grvMRDraft" CssClass="GridInner" runat="server" Width="100%" BorderColor="White"
                        BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                        HeaderStyle-Height="25px" CellSpacing="2" HeaderStyle-VerticalAlign="Middle"
                        AllowPaging="true" PageSize="50" OnRowCreated="grvMRDraft_RowCreated" OnPageIndexChanging="grvMRDraft_PageIndexChanging"
                        OnRowDataBound="grvMRDraft_RowDataBound">
                        <PagerSettings Mode="NumericFirstLast" />
                        <PagerStyle BackColor="#1B3B5F" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle"
                            CssClass="pager-row" />
                        <%-- <PagerStyle BackColor="#70A0D0" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle" />--%>
                        <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px"
                            CssClass="row" />
                        <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                        <Columns>
                            <asp:TemplateField HeaderText="#">
                                <ItemStyle HorizontalAlign="Center" Width="20px" />
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <asp:Label ID="lblNo" runat="server" Text='<%# Bind("No") %>' BorderStyle="none"> </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="ItemCode" Visible="false">
                                <ItemStyle HorizontalAlign="Center" Width="20px" />
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <asp:Label ID="lblItemCode" runat="server" Text='<%# Bind("ItemCode") %>' BorderStyle="none"> </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="GroupType" Visible="false">
                                <ItemStyle HorizontalAlign="Center" Width="20px" />
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <asp:Label ID="lblGroupType" runat="server" Text='<%# Bind("GroupType") %>' BorderStyle="none"> </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="SupplierCode" Visible="false">
                                <ItemStyle HorizontalAlign="left" Width="20px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblCardCode" runat="server" Text='<%# Bind("SupplierCode") %>' BorderStyle="none" />
                                    <asp:TextBox ID="txtCardCode" runat="server" ReadOnly="True" Visible="false" Width="96px"
                                        BackColor="#E6E6E6"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="SupplierName">
                                <ItemStyle HorizontalAlign="left" Width="300px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblCardName" runat="server" Text='<%# Bind("SupplierName") %>' BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Min Spend">
                                <ItemStyle HorizontalAlign="left" Width="80px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblMinSpend" runat="server" Text='<%# Bind("MinSpend") %>' BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Description">
                                <ItemStyle HorizontalAlign="left" Width="300px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="txtDescription" runat="server" Text='<%# Bind("Description") %>' BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="In Stock">
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="txtInStock" runat="server" Text='<%# String.Format("{0,-15:#,##0.0000}",Eval("InStock")) %>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Event Order">
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemStyle HorizontalAlign="Right" Width="80px" />
                                <ItemTemplate>
                                    <asp:Label ID="txtEventOrder" runat="server" Text='<%# String.Format("{0,-15:#,##0.0000}",Eval("EventOrder")) %>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Last 7 Days Avg Req">
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemStyle HorizontalAlign="Right" Width="80px" />
                                <ItemTemplate>
                                    <asp:Label ID="txtL7DaysAvg" runat="server" Text='<%# String.Format("{0,-15:#,##0.0000}",Eval("Last7DaysAvg")) %>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Already ordered">
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemStyle HorizontalAlign="Right" Width="80px" />
                                <ItemTemplate>
                                    <asp:Label ID="txtAlreadyOrdered" runat="server" Text='<%# String.Format("{0,-15:#,##0.0000}",Eval("AlreadyOrdered"))%>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Min Stock">
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemStyle HorizontalAlign="Right" Width="80px" />
                                <ItemTemplate>
                                    <asp:Label ID="txtMinStock" runat="server" Text='<%# String.Format("{0,-15:#,##0.0000}",Eval("MinStock"))%>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Recommended Quantity">
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemStyle HorizontalAlign="Right" Width="80px" />
                                <ItemTemplate>
                                    <asp:Label ID="lblRecommendedQuantity" runat="server" Text='<%# String.Format("{0,-15:#,##0.0000}",Eval("RecommendedQuantity"))%>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Order Quantity">
                                <ItemStyle HorizontalAlign="Right" />
                                <HeaderStyle VerticalAlign="Middle" Width="100px" />
                                <ItemTemplate>
                                    <asp:TextBox ID="txtOrderQuantity" runat="server" Width="97%" Text='<%# Eval("OrderQuantity")%>'
                                        CssClass="txtbox" AutoPostBack="true" OnTextChanged="txtOrderQuantity_OnTextChanged"
                                        OnKeyPress="return isNumberKey(this, event);" Style="text-align: right" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="UoM">
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemStyle HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:Label ID="txtUoM" runat="server" Text='<%# Bind("UOM") %>' BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Price">
                                <ItemStyle HorizontalAlign="Right" />
                                <HeaderStyle VerticalAlign="Middle" Width="120px" />
                                <ItemTemplate>
                                    <asp:Label ID="txtPrice" runat="server" Text='<%# String.Format("{0,-15:#,##0.0000}",Eval("Price"))%>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Total">
                                <ItemStyle HorizontalAlign="Right" />
                                <HeaderStyle VerticalAlign="Middle" Width="120px" />
                                <ItemTemplate>
                                    <asp:Label ID="txtTotal" runat="server" Text='<%#String.Format("{0,-15:#,##0.0000}", Eval("Total"))%>'
                                        BorderStyle="none" />
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
                                        <span>Supplier</span>
                                    </th>
                                    <th>
                                        <span>Min Spend</span>
                                    </th>
                                    <th>
                                        <span>Description</span>
                                    </th>
                                    <th>
                                        <span>In Stock</span>
                                    </th>
                                    <th>
                                        <span>Event Order</span>
                                    </th>
                                    <th>
                                        <span>Last 7 days Avg Req</span>
                                    </th>
                                    <th>
                                        <span>Already Ordered</span>
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Recommended Quantity</span>
                                    </th>
                                    <th>
                                        <span>Ordered Quantity</span>
                                    </th>
                                    <th>
                                        <span>UOM</span>
                                    </th>
                                    <th>
                                        <span>Price</span>
                                    </th>
                                    <th>
                                        <span>Total</span>
                                    </th>
                                </tr>
                                <tr>
                                    <td colspan="12">
                                        <span>No Data</span>
                                    </td>
                                </tr>
                            </table>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
                <hr />
                <table width="100%" border="0">
                    <tr>
                        <td valign="top" style="width: 300px;">
                            <table>
                                <tr>
                                    <td>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="width: 100px;">
                        </td>
                        <td>
                            <div style="margin-left: 75%; margin-right: 5px;">
                                <table border="0">
                                    <tr valign="middle" style="height: 27px; font-weight: bold; text-decoration: none;
                                        background-color: #D9E0ED;">
                                        <td style="width: 150px" align="right">
                                            Total:
                                        </td>
                                        <td align="right">
                                            <asp:Label ID="lblGrandTotal" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <div style="width: 100%;">
                                <hr />
                            </div>
                        </td>
                    </tr>
                </table>
                <table style="width: 100%">
                    <tr align="right">
                        <td>
                            <asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                        </td>
                        <td style="width: 400px;">
                            <asp:Button ID="btnSaveDraft" runat="server" Text="Save" Style="background-image: url('Images/bgButton.png');
                                background-repeat: no-repeat; width: 85px; color: White;" BorderStyle="Solid"
                                OnClick="btnSaveDraft_Click" />
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit" Style="background-image: url('Images/bgButton.png');
                                background-repeat: no-repeat; width: 85px; color: White;" BorderStyle="Solid"
                                OnClick="btnSubmit_Click" />
                        </td>
                    </tr>
                </table>
                <%-- THis part of code is to display the pop up--%>
                <asp:Button ID="btnHiddenOpen" runat="server" Style="display: none" />
                <cc1:ModalPopupExtender ID="mpePopup" runat="server" TargetControlID="btnHiddenOpen"
                    PopupControlID="panShow">
                </cc1:ModalPopupExtender>
                <asp:Panel ID="panShow" runat="server" Style="width: 500px; height: 570px; background-color: #F8F8F8;
                    border: 2px solid #C8C8C8">
                    <asp:UpdatePanel runat="server" ID="updatepnl" UpdateMode="Conditional">
                        <ContentTemplate>
                            <table id="tblHead" style="width: 494px; height: 20px; background-color: #CCCCCC;
                                margin-left: 2px; margin-right: 2px; margin-top: 2px; margin-bottom: 2px; border-style: solid;
                                border-width: 1px; font-weight: bold;">
                                <tr>
                                    <td>
                                        Add Items
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td style="width: 205px">
                                        Search By
                                    </td>
                                    <td style="width: 15px">
                                        :
                                    </td>
                                    <td>
                                        <asp:RadioButton ID="rdbSuppliers" runat="server" Text="Suppliers" AutoPostBack="true"
                                            Checked="true" OnCheckedChanged="rdbSuppliers_CheckedChanged" />
                                        <asp:RadioButton ID="rdbItems" runat="server" Text="Items" AutoPostBack="true" OnCheckedChanged="rdbItems_CheckedChanged" />
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Supplier
                                    </td>
                                    <td>
                                        :
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlSupplier" runat="server" AutoPostBack="true" CssClass="dropdownlist"
                                            OnSelectedIndexChanged="ddlSupplier_SelectedIndexChanged" Style="width: 350px;">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Item Search
                                    </td>
                                    <td>
                                        :
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtSearch" runat="server" CssClass="txtbox" AutoPostBack="true"
                                            ToolTip="Search Items..." OnTextChanged="txtSearch_OnTextChanged"></asp:TextBox>
                                        &nbsp;
                                        <asp:Button ID="btnSearch" runat="server" Text="::" Style="height: 20px; width: 5%;
                                            text-align: justify;" OnClick="btnSearch_Click" />
                                    </td>
                                </tr>
                            </table>
                            <div style="width: 500px; height: 420px; overflow: auto; border: 1px;">
                                <table>
                                    <tr>
                                        <td colspan="2" style="text-align: center;">
                                            <asp:GridView ID="grdVendor" runat="server" CssClass="GridInner" Width="480px" BorderColor="White"
                                                BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                                                HeaderStyle-Height="27px">
                                                <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px" />
                                                <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" Height="25px" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="#" SortExpression="CardCode">
                                                        <ItemStyle HorizontalAlign="Center" Width="13%" />
                                                        <HeaderStyle VerticalAlign="Middle" />
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="chkItems" runat="server" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="SupplierName">
                                                        <ItemStyle HorizontalAlign="left" Width="40%" />
                                                        <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSupplierName" runat="server" Text='<%# Bind("SupplierName") %>'
                                                                BorderStyle="none" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="GroupType" Visible="false">
                                                        <ItemStyle HorizontalAlign="left" Width="40%" />
                                                        <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblGroupType" runat="server" Text='<%# Bind("GroupType") %>' BorderStyle="none" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="SupplierCode" Visible="false">
                                                        <ItemStyle HorizontalAlign="left" Width="40%" />
                                                        <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSupplierCode" runat="server" Text='<%# Bind("SupplierCode") %>'
                                                                BorderStyle="none" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Item Name" SortExpression="CardName">
                                                        <ItemStyle HorizontalAlign="left" Width="70%" />
                                                        <HeaderStyle VerticalAlign="Middle" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblItemDesc" runat="server" Text='<%# Bind("Description") %>' BorderStyle="none"> </asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Item Code" SortExpression="CardCode">
                                                        <ItemStyle HorizontalAlign="Center" Width="13%" />
                                                        <HeaderStyle VerticalAlign="Middle" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblItemCode" runat="server" Text='<%# Bind("ItemCode") %>' BorderStyle="none"> </asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="MinSpend" Visible="false">
                                                        <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblMinSpendChild" runat="server" Text='<%# String.Format("{0,-15:#,##0.0000}",Eval("MinSpend")) %>'
                                                                BorderStyle="none" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="In Stock" Visible="false">
                                                        <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblInStock" runat="server" Text='<%# String.Format("{0,-15:#,##0.0000}",Eval("InStock")) %>'
                                                                BorderStyle="none" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Event Order" Visible="false">
                                                        <HeaderStyle VerticalAlign="Middle" />
                                                        <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblEventOrder" runat="server" Text='<%# String.Format("{0,-15:#,##0.0000}",Eval("EventOrder")) %>'
                                                                BorderStyle="none" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Last 7 Days Avg Req" Visible="false">
                                                        <HeaderStyle VerticalAlign="Middle" />
                                                        <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblL7DaysAvg" runat="server" Text='<%# String.Format("{0,-15:#,##0.0000}",Eval("Last7DaysAvg")) %>'
                                                                BorderStyle="none" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Already ordered" Visible="false">
                                                        <HeaderStyle VerticalAlign="Middle" />
                                                        <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblAlreadyOrdered" runat="server" Text='<%# String.Format("{0,-15:#,##0.0000}",Eval("AlreadyOrdered"))%>'
                                                                BorderStyle="none" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Min Stock" Visible="false">
                                                        <HeaderStyle VerticalAlign="Middle" />
                                                        <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblMinStock" runat="server" Text='<%# String.Format("{0,-15:#,##0.0000}",Eval("MinStock"))%>'
                                                                BorderStyle="none" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Recommended Quantity" Visible="false">
                                                        <HeaderStyle VerticalAlign="Middle" />
                                                        <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblRecommendedQuantity" runat="server" Text='<%# String.Format("{0,-15:#,##0.0000}",Eval("RecommendedQuantity"))%>'
                                                                BorderStyle="none" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Order Quantity" Visible="false">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                        <HeaderStyle VerticalAlign="Middle" Width="100px" />
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtOrderQuantity" runat="server" Width="97%" Text='<%# Eval("OrderQuantity")%>'
                                                                CssClass="txtbox" AutoPostBack="true" Style="text-align: right" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="UoM" Visible="false">
                                                        <HeaderStyle VerticalAlign="Middle" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblUoM" runat="server" Text='<%# Bind("UOM") %>' BorderStyle="none" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Price" Visible="false">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                        <HeaderStyle VerticalAlign="Middle" Width="120px" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblPrice" runat="server" Text='<%# String.Format("{0,-15:#,##0.0000}",Eval("Price"))%>'
                                                                BorderStyle="none" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Total" Visible="false">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                        <HeaderStyle VerticalAlign="Middle" Width="120px" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblTotal" runat="server" Text='<%#String.Format("{0,-15:#,##0.0000}", Eval("Total"))%>'
                                                                BorderStyle="none" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="ItemPerUnit" Visible="false">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                        <HeaderStyle VerticalAlign="Middle" Width="120px" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblItemPerUnit" runat="server" Text='<%# Eval("ItemPerUnit")%>'
                                                                BorderStyle="none" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <PagerStyle BackColor="#1B3B5F" Font-Bold="True" ForeColor="White" HorizontalAlign="Right"
                                                    VerticalAlign="Middle" />
                                                <SelectedRowStyle BackColor="LightCyan" Font-Bold="true" ForeColor="DarkBlue" />
                                                <HeaderStyle BackColor="#1B3B61" Font-Bold="true" ForeColor="#ffffff" Font-Overline="False"
                                                    Height="27px" VerticalAlign="Bottom" />
                                                <EmptyDataTemplate>
                                                    <b>No Data Found. </b>
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" style="text-align: center;">
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <table style="width: 100%; margin-top: 12px;">
                                <tr align="right">
                                    <td>
                                    </td>
                                    <td style="width: 450px;">
                                        <asp:Button ID="btnSubmitItems" runat="server" Text="Choose Items" OnClick="btnSubmitItems_Click"
                                            Style="background-image: url('Images/bgButton.png'); text-align: center; background-repeat: no-repeat;
                                            width: 100px; color: White;" BorderStyle="Solid" />
                                        <asp:Button ID="btnClose" runat="server" Text="Close" Style="background-image: url('Images/bgButton.png');
                                            background-repeat: no-repeat; width: 85px; color: White;" BorderStyle="Solid"
                                            OnClick="btnClose_Click" />
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdateProgress ID="UpdateProg2" DisplayAfter="0" runat="server">
                        <ProgressTemplate>
                          <asp:Panel ID="Panel2" runat="server" CssClass="pop">
                                    <div style="text-align: center; padding-top:350px; padding-left:50px; width: 800px; height: 700px;">
                                        <asp:Image ID="Image1" runat="server" Height="50px" ImageUrl="~/Images/loading3.gif" Width="50px" />
                                    </div>
                            </asp:Panel>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
