<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StockCounting.aspx.cs"
    Inherits="AE_HarrysWeb_V001.StockCounting" MasterPageFile="~/Main.Master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style16
        {
            width: 163px;
        }
        .style17
        {
            width: 7px;
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
        <asp:UpdatePanel ID="updatePanel" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div style="height: 5px;">
                </div>
                <h2>
                    <asp:Label ID="lblTitle" runat="server" Text="Stock Counting "></asp:Label>
                </h2>
                <div style="margin-left: 5px; width: 99%;">
                    <hr />
                    <table border="0" width="100%" style="background-color: #D1D4D8; font-weight: bold;">
                        <tr>
                            <td>
                                Outlet
                            </td>
                            <td>
                                :
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlOutlet" runat="server" AutoPostBack="true" CssClass="dropdownlist"
                                    Style="width: 450px;">
                                </asp:DropDownList>
                            </td>
                            <td align="right" rowspan="4">
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
                            <td style="width: 200px">
                                Count Date
                            </td>
                            <td>
                                :
                            </td>
                            <td>
                                <asp:TextBox ID="txtCountDate" runat="server" AutoPostBack="True" CssClass="txtbox"></asp:TextBox>
                                <asp:ImageButton ID="Image1" runat="Server" Style="width: 15px;" AlternateText="Click to show calendar"
                                    ImageUrl="Images/Calender.jpg" />
                                <cc1:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtCountDate"
                                    PopupButtonID="Image1" Format="dd/MM/yyyy">
                                </cc1:CalendarExtender>
                            </td>
                        </tr>
                        <tr>
                            <td class="style16">
                                Status
                            </td>
                            <td class="style17">
                                :
                            </td>
                            <td>
                                <asp:Label ID="lblStatus" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <hr />
                </div>
                <div style="width: 100%">
                    <asp:GridView ID="grvStkCnt" CssClass="GridInner" Width="100%" BorderColor="White"
                        BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                        HeaderStyle-Height="25px" CellSpacing="2" HeaderStyle-VerticalAlign="Middle"
                        AllowPaging="true" PageSize="50">
                        <PagerSettings Mode="NumericFirstLast" />
                        <PagerStyle BackColor="#1B3B5F" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle" />
                        <%-- <PagerStyle BackColor="#70A0D0" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle" />--%>
                        <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px" />
                        <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                        <Columns>
                            <asp:TemplateField HeaderText="Description">
                                <ItemStyle HorizontalAlign="left" Width="300px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblDescription" runat="server" Text='<%# Bind("Description") %>' BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Stocktake UoM1" Visible="True">
                                <ItemStyle HorizontalAlign="left" Width="300px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblStocktakeUoM1" runat="server" Text='<%# Bind("StocktakeUoM1") %>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Counted in Stocktake UoM1" Visible="True">
                                <ItemStyle HorizontalAlign="left" Width="300px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblCntStkUoM1" runat="server" Text='<%# Bind("CntStocktakeUoM1") %>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Stocktake UoM2" Visible="True">
                                <ItemStyle HorizontalAlign="left" Width="300px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblStocktakeUoM2" runat="server" Text='<%# Bind("StocktakeUoM2") %>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Counted in Stocktake UoM2" Visible="True">
                                <ItemStyle HorizontalAlign="left" Width="300px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblCntStkUoM2" runat="server" Text='<%# Bind("CntStocktakeUoM2") %>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Inventory UoM" Visible="True">
                                <ItemStyle HorizontalAlign="left" Width="300px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblInventoryUoM" runat="server" Text='<%# Bind("InventoryUoM") %>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Counted in Inventory UoM" Visible="True">
                                <ItemStyle HorizontalAlign="left" Width="300px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblCntInventoryUoM" runat="server" Text='<%# Bind("CntInventoryUoM") %>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="In Stock">
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemStyle HorizontalAlign="left" Width="100px" />
                                <ItemTemplate>
                                    <asp:Label ID="lblInStock" runat="server" Text='<%# String.Format("{0,-15:#,##0.0000}",Eval("InStock")) %>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Variance">
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemStyle HorizontalAlign="left" Width="80px" />
                                <ItemTemplate>
                                    <asp:Label ID="lblVariance" runat="server" BorderStyle="none" Text='<%# Bind("Variance") %>' />
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
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Description</span>
                                    </th>
                                     <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Stocktake UoM1</span>
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Counted in Stocktake UoM1</span>
                                    </th>
                                     <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Stocktake UoM2</span>
                                    </th>
                                     <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Counted in Stocktake UoM2</span>
                                    </th>
                                     <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Inventory UOM</span>
                                    </th>
                                     <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Counted in Inventory UOM</span>
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>In Stock</span>
                                    </th>
                                   <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Variance</span>
                                    </th>
                                </tr>
                                <tr>
                                    <td colspan="9">
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
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
