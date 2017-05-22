<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApprovalStatusSummary.aspx.cs"
    Inherits="AE_HarrysWeb_V001.ApprovalStatusSummary" MasterPageFile="~/Main.Master" %>

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
                    <asp:Label ID="lblTitle" runat="server" Text="Approval Status Summary"></asp:Label>
                    <%--<asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>--%>
                </h2>
                <div style="margin-left: 5px; width: 99%;">
                    <hr />
                    <table border="0" width="100%" style="background-color: #D1D4D8; font-weight: bold;">
                        <tr>
                            <td class="style16">
                            </td>
                            <td class="style17">
                            </td>
                            <td>
                            </td>
                            <td align="right" rowspan="4">
                                <img src="Images/Logo_Small.png" alt="Harrys Logo" />
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td class="style16">
                                Filter Outlet
                            </td>
                            <td class="style17">
                                :
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlOutlet" runat="server" AutoPostBack="false" CssClass="dropdownlist"
                                    Style="width: 450px;" OnSelectedIndexChanged="ddlOutlet_SelectedIndexChanged">
                                    <asp:ListItem Value=" --- Select Outlet --- "> --- Select Outlet --- </asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="style16">
                                OrderDate From
                            </td>
                            <td class="style17">
                                :
                            </td>
                            <td>
                                <asp:TextBox ID="txtFromDate" runat="server" AutoPostBack="false" CssClass="txtbox"></asp:TextBox>
                                <asp:ImageButton ID="imgFromDate" runat="Server" Style="width: 15px;" AlternateText="Click to show calendar"
                                    ImageUrl="Images/Calender.jpg" />
                                &nbsp;
                                <cc1:CalendarExtender ID="clnFromDate" runat="server" TargetControlID="txtFromDate"
                                    PopupButtonID="imgFromDate" Format="dd/MM/yyyy">
                                </cc1:CalendarExtender>
                                OrderDate To:&nbsp;
                                <asp:TextBox ID="txtToDate" runat="server" AutoPostBack="false" CssClass="txtbox"></asp:TextBox>
                                <asp:ImageButton ID="imgToDate" runat="Server" AlternateText="Click to show calendar"
                                    ImageUrl="Images/Calender.jpg" Style="width: 15px;" />
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <cc1:CalendarExtender ID="ClnToDate" runat="server" TargetControlID="txtToDate" PopupButtonID="imgToDate"
                                    Format="dd/MM/yyyy">
                                </cc1:CalendarExtender>
                                <asp:Button ID="btnSearch" runat="server" Height="28px" OnClick="btnSearch_Click"
                                    Style="text-align: center;" Text="Search" Width="60px" />
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Label ID="lblMessage" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>
                <hr />
                <div style="width: 100%;">
                    <table border="0" width="100%" style="background-color: #D1D4D8; font-weight: bold;">
                        <asp:GridView ID="grvAppr" CssClass="GridInner" runat="server" Width="100%" BorderColor="White"
                            BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                            HeaderStyle-Height="25px" CellSpacing="2" HeaderStyle-VerticalAlign="Middle"
                            AllowPaging="true" PageSize="50" OnPageIndexChanging="grvAppr_PageIndexChanging">
                            <PagerSettings Mode="NumericFirstLast" />
                            <PagerStyle BackColor="#1B3B5F" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle"
                                CssClass="pager-row" />
                            <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px"
                                CssClass="row" />
                            <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                            <Columns>
                                <asp:TemplateField HeaderText="WEB Doc. Num">
                                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblDocEntry" runat="server" Text='<%# Bind("docentry") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="SAP Doc. Num">
                                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblSAPDocEntry" runat="server" Text='<%# Bind("SAPDocEntry") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Date">
                                    <ItemStyle HorizontalAlign="Center" Width="75px" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblOrderDate" runat="server" Text='<%# Bind("docdate","{0:dd/MM/yyyy}") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Request">
                                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblRequest" runat="server" Text='<%# Bind("Requester") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Outlet">
                                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblOutlet" runat="server" Text='<%# Bind("Outlet") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Approval">
                                    <ItemStyle HorizontalAlign="Center" Width="200px" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblApproval" runat="server" Text='<%# Bind("Approval") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="L1ApprovalStatus">
                                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblL1ApprovalStatus" runat="server" Text='<%# Bind("U_L1ApprovalStatus") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="L1Approval">
                                    <ItemStyle HorizontalAlign="Center" Width="250px" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblL1Approval" runat="server" Text='<%# Bind("U_L1Approver") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="L2ApprovalStatus">
                                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblL2ApprovalStatus" runat="server" Text='<%# Bind("U_L2ApprovalStatus") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="L2Approval">
                                    <ItemStyle HorizontalAlign="Center" Width="250px" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblL2Approval" runat="server" Text='<%# Bind("U_L2Approver") %>'></asp:Label>
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
                                            <span>WEB Doc. Num</span>
                                        </th>
                                        <th>
                                            <span>SAP Doc. Num</span>
                                        </th>
                                        <th>
                                            <span>Order Date</span>
                                        </th>
                                        <th>
                                            <span>Request</span>
                                        </th>
                                        <th>
                                            <span>Outlet</span>
                                        </th>
                                        <th>
                                            <span>Approval</span>
                                        </th>
                                        <th>
                                            <span>L1ApprovalStatus</span>
                                        </th>
                                        <th>
                                            <span>L1Approval</span>
                                        </th>
                                        <th>
                                            <span>L2ApprovalStatus</span>
                                        </th>
                                        <th>
                                            <span>L2Approval</span>
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
                    </table>
                    <table style="width: 100%">
                        <tr align="center">
                            <td>
                                <asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
