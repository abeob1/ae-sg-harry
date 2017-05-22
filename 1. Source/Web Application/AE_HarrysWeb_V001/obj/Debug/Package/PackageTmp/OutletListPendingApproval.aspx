<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OutletListPendingApproval.aspx.cs"
    Inherits="AE_HarrysWeb_V001.OutletListPendingApproval" MasterPageFile="~/Main.Master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .style16
        {
            width: 127px;
        }
        .style17
        {
            width: 10px;
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
                    <asp:Label ID="lblTitle" runat="server" Text="Outlet List of PR Pending Approval "></asp:Label>
                </h2>
                <div style="margin-left: 5px; width: 99%;">
                    <hr />
                    <table border="0" width="100%" style="background-color: #D1D4D8; font-weight: bold;">
                        <tr>
                            <%--<td class="style16">
                                Filter Outlet
                            </td>
                            <td class="style17">
                                :
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlOutlet" runat="server" AutoPostBack="true" CssClass="dropdownlist"
                                    Style="width: 450px;" OnSelectedIndexChanged="ddlOutlet_SelectedIndexChanged">
                                    <asp:ListItem Value="">Select Outlet</asp:ListItem>
                                </asp:DropDownList>
                            </td>--%>
                            <td align="right" rowspan="4">
                                <img src="Images/Logo_Small.png" alt="Harrys Logo" />
                            </td>
                            <td>
                            </td>
                        </tr>
                    </table>
                </div>
                <hr />
                <div style="width: 100%;">
                    <asp:GridView ID="grvPendingAppr" runat="server" CssClass="GridInner" Width="100%"
                        BorderColor="White" BackColor="White" AllowSorting="True" AutoGenerateColumns="False"
                        CellPadding="2" HeaderStyle-Height="25px" CellSpacing="2" HeaderStyle-VerticalAlign="Middle"
                        AllowPaging="true" PageSize="50" OnRowDataBound="grvPendingAppr_RowDataBound">
                        <PagerSettings Mode="NumericFirstLast" />
                        <PagerStyle BackColor="#1B3B5F" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle"
                            CssClass="pager-row" />
                        <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px"
                            CssClass="row" />
                        <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                        <Columns>
                            <asp:TemplateField HeaderText="Outlet Code" Visible="false">
                                <ItemStyle HorizontalAlign="Center" Width="150px" />
                                <HeaderStyle VerticalAlign="Middle" Width="200px" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblOutletCode" runat="server" Text='<%# Bind("WhsCode") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Outlet">
                                <ItemStyle HorizontalAlign="Center" Width="150px" />
                                <HeaderStyle VerticalAlign="Middle" Width="200px" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblOutletName" runat="server" Text='<%# Bind("WhsName") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Number of PR Pending Level1 Approval">
                                <ItemStyle HorizontalAlign="Center" Width="50px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkNoPRL1Approval" runat="server" Text='<%# Bind("L1ApprovalStatus") %>'
                                        OnClick="lnkL1App_Click" CommandArgument='<%#Eval("WhsCode") + ";" %>' Font-Bold="true"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Number of PR Pending Level2 Approval">
                                <ItemStyle HorizontalAlign="Center" Width="50px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkNoPRL2Approval" runat="server" Text='<%# Bind("L2ApprovalStatus") %>'
                                        OnClick="lnkL2App_Click" CommandArgument='<%#Eval("WhsCode") + ";" %>' Font-Bold="true"></asp:LinkButton>
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
                                        <span>Outlet Name</span>
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Number of PR Pending Level1 Approval</span>
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Number of PR Pending Level2 Approval</span>
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
                        <%--<td style="width: 400px;">
                            <asp:Button ID="btndeleteDraft" runat="server" Text="Delete Draft" Visible="true"
                                Style="background-image: url('Images/bgButton.png'); background-repeat: no-repeat;
                                width: 85px; color: White;" BorderStyle="Solid" OnClick="btndeleteDraft_Click"
                                OnClientClick="return confirm('Are you sure you want to delete?')" />
                        </td>--%>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
