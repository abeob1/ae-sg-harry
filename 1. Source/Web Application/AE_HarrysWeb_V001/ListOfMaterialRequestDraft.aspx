<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListOfMaterialRequestDraft.aspx.cs"
    Inherits="AE_HarrysWeb_V001.ListOfMaterialRequestDraft" MasterPageFile="~/Main.Master" %>

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
                    <asp:Label ID="lblTitle" runat="server" Text="List of Material Request Draft "></asp:Label>
                </h2>
                <div style="margin-left: 5px; width: 99%;">
                    <hr />
                    <table border="0" width="100%" style="background-color: #D1D4D8; font-weight: bold;">
                        <tr>
                            <td class="style16">
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
                            </td>
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
                    <asp:GridView ID="grvMRDraft" CssClass="GridInner" runat="server" Width="100%" BorderColor="White"
                        BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                        HeaderStyle-Height="25px" CellSpacing="2" HeaderStyle-VerticalAlign="Middle"
                        AllowPaging="true" PageSize="50">
                        <PagerSettings Mode="NumericFirstLast" />
                        <PagerStyle BackColor="#1B3B5F" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle"
                            CssClass="pager-row" />
                        <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px"
                            CssClass="row" />
                        <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                        <Columns>
                            <asp:TemplateField HeaderText="DELETE">
                                <ItemStyle HorizontalAlign="Center" Width="50px" />
                                <HeaderStyle VerticalAlign="Middle" Width="20px" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkSelect" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--<asp:TemplateField HeaderText="Type" Visible="false">
                                <ItemStyle HorizontalAlign="Center" Width="50px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkDrfNO1" runat="server" Text='<%# Bind("Type") %>' OnClick="lnkDrfNO1_Click"
                                        CommandArgument='<%#Eval("Type") + ";"  %>' Font-Bold="true"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>--%>
                            <asp:TemplateField HeaderText="DRF NO">
                                <ItemStyle HorizontalAlign="Center" Width="50px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkDrfNO" runat="server" Text='<%# Bind("DocEntry") %>' OnClick="lnkDrfNO_Click"
                                        CommandArgument='<%#Eval("DocEntry") + ";"  + Eval("Type") + ";" %>' Font-Bold="true"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="OUTLET">
                                <ItemStyle HorizontalAlign="Center" Width="50px" />
                                <HeaderStyle VerticalAlign="Middle" Width="100px" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblOutlet" runat="server" Text='<%# Bind("Outlet") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="ORDER DATE">
                                <ItemStyle HorizontalAlign="Center" Width="50px" />
                                <HeaderStyle VerticalAlign="Middle" Width="50px" />
                                <ItemStyle HorizontalAlign="Center" Width="80px" />
                                <ItemTemplate>
                                    <asp:Label ID="lblOrderDate" runat="server" Text='<%# Bind("OrderDate","{0:dd/MM/yyyy}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="URGENT">
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemStyle HorizontalAlign="Center" Width="80px" />
                                <ItemTemplate>
                                    <asp:Label ID="lblUrgent" runat="server" Text='<%# Bind("Urgent") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="TOTAL SPEND">
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemStyle HorizontalAlign="Center" Width="80px" />
                                <ItemTemplate>
                                    <asp:Label ID="lblTotalSpend" runat="server" Text='<%# Bind("TotalSpend") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="STATUS">
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemStyle HorizontalAlign="Center" Width="80px" />
                                <ItemTemplate>
                                    <asp:Label ID="lblStatus" runat="server" Text='<%# Bind("Status") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="USER NAME">
                                <ItemStyle HorizontalAlign="Center" />
                                <HeaderStyle VerticalAlign="Middle" Width="50px" />
                                <ItemTemplate>
                                    <asp:Label ID="lblUserName" runat="server" Text='<%# Bind("UserName") %>'></asp:Label>
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
                                        <span>DRF NO</span>
                                    </th>
                                    <th>
                                        <span>OUTLET</span>
                                    </th>
                                    <th>
                                        <span>ORDER DATE</span>
                                    </th>
                                    <th>
                                        <span>URGENT</span>
                                    </th>
                                    <th>
                                        <span>TOTAL SPEND</span>
                                    </th>
                                    <th>
                                        <span>STATUS</span>
                                    </th>
                                    <th>
                                        <span>USER NAME</span>
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
                <table style="width: 100%">
                    <tr align="right">
                        <td>
                            <asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                        </td>
                        <td style="width: 400px;">
                            <asp:Button ID="btndeleteDraft" runat="server" Text="Delete Draft" Visible="true"
                                Style="background-image: url('Images/bgButton.png'); background-repeat: no-repeat;
                                width: 85px; color: White;" BorderStyle="Solid" OnClick="btndeleteDraft_Click"
                                OnClientClick="return confirm('Are you sure you want to delete?')" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
