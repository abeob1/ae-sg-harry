<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StocktakeApproval.aspx.cs"
    Inherits="AE_HarrysWeb_V001.StocktakeApproval" MasterPageFile="~/Main.Master" %>

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
                    <asp:Label ID="lblTitle" runat="server" Text="List Of Stock Take Counting Pending Approval "></asp:Label>
                    <asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
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
                                <asp:DropDownList ID="ddlWareHouse" runat="server" AutoPostBack="true" CssClass="dropdownlist"
                                    Style="width: 450px;" OnSelectedIndexChanged="ddlWareHouse_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                            <td align="right" rowspan="4">
                                <img src="Images/Logo_Small.png" alt="Harrys Logo" />
                            </td>
                        </tr>
                    </table>
                    <hr />
                </div>
                <div style="width: 100%">
                    <asp:GridView ID="grvStkApp" runat="server" CssClass="GridInner" Width="100%" BorderColor="White"
                        BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                        HeaderStyle-Height="25px" CellSpacing="2" HeaderStyle-VerticalAlign="Middle"
                        AllowPaging="true" PageSize="400">
                        <PagerSettings Mode="NumericFirstLast" />
                        <PagerStyle BackColor="#1B3B5F" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle"
                            CssClass="pager-row" />
                        <%-- <PagerStyle BackColor="#70A0D0" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle" />--%>
                        <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px"
                            CssClass="row" />
                        <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                        <Columns>
                            <asp:TemplateField HeaderText="DocEntry" Visible="false">
                                <ItemStyle HorizontalAlign="left" Width="10px" />
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <asp:Label ID="lblDocEntry" runat="server" Text='<%# Bind("DocEntry") %>' BorderStyle="none">
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Outlet">
                                <ItemStyle HorizontalAlign="left" Width="10px" />
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <asp:Label ID="lblOutlet" runat="server" Text='<%# Bind("WhsCode") %>' BorderStyle="none">
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Count Date">
                                <ItemStyle HorizontalAlign="left" Width="10px" />
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <asp:Label ID="lblCountDate" runat="server" Text='<%# Bind("DocDate") %>' BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Status">
                                <ItemStyle HorizontalAlign="left" Width="10px" />
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkStatus" runat="server" Text='<%# Bind("U_StockTakeStatus") %>'
                                        OnClick="lnkStatus_Click" CommandArgument='<%#Eval("U_StockTakeStatus") + ";" + Eval("DocDate") + ";"  + Eval("WhsCode") + ";" + Eval("U_ApprovedDate") + ";" + Eval("U_ApprovedBy") + ";" + Eval("DocEntry") + ";" %>'
                                        Font-Bold="true"></asp:LinkButton>
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
                                        <span>Outlet</span>
                                    </th>
                                    <th>
                                        <span>Count Date</span>
                                    </th>
                                    <th>
                                        <span>Status</span>
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
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
