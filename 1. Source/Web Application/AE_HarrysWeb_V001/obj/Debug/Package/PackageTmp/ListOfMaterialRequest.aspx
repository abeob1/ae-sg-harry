<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListOfMaterialRequest.aspx.cs"
    Inherits="AE_HarrysWeb_V001.ListOfMaterialRequest" MasterPageFile="~/Main.Master" %>

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
                    <asp:Label ID="lblTitle" runat="server" Text="List of Material Request Approved"></asp:Label>
                    <%--<asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>--%>
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
                                <asp:DropDownList ID="ddlOutlet" runat="server" AutoPostBack="false" CssClass="dropdownlist"
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
                        <tr>
                            <td class="style16">
                                Status
                            </td>
                            <td class="style17">
                                :
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="false" CssClass="dropdownlist"
                                    Style="width: 450px;">
                                    <asp:ListItem Value="-1" Selected="True"> --- Select Status --- </asp:ListItem>
                                    <asp:ListItem Value="0">Approved</asp:ListItem>
                                    <asp:ListItem Value="1">Approved Not Required</asp:ListItem>
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
                <table width="100%" style="font-weight: bold;">
                    <%--td style="width: 80%">
                        </td>
                        <td>
                            Search
                        </td>
                        <td>
                            :
                        </td><tr>
                            <td>
                                <asp:TextBox ID="txtSearch" runat="server" AutoPostBack="true" CssClass="txtbox"
                                    OnTextChanged="txtSearch_OnTextChanged" ToolTip="Search Items..." Visible="false"></asp:TextBox>
                            </td>
                            <td align="right">
                                 <asp:Button ID="btnSearch" runat="server" Height="28px" OnClick="btnSearch_Click"
                                    Style="text-align: center;" Text="Search" Width="60px" />
                            </td>
                        </tr>
                </table>--%>
                    <div style="width: 100%;">
                        <table border="0" width="100%" style="background-color: #D1D4D8; font-weight: bold;">
                            <asp:GridView ID="grvMR" CssClass="GridInner" runat="server" Width="100%" BorderColor="White"
                                BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                                HeaderStyle-Height="25px" CellSpacing="2" HeaderStyle-VerticalAlign="Middle"
                                AllowPaging="true" PageSize="50" OnPageIndexChanging="grvMR_PageIndexChanging">
                                <PagerSettings Mode="NumericFirstLast" />
                                <PagerStyle BackColor="#1B3B5F" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle"
                                    CssClass="pager-row" />
                                <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px"
                                    CssClass="row" />
                                <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                                <Columns>
                                    <asp:TemplateField HeaderText="SAP PR/PQ NO">
                                        <ItemStyle HorizontalAlign="Center" Width="50px" />
                                        <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkPrNO" runat="server" Text='<%# Bind("PRNo") %>' OnClick="lnkPrNO_Click"
                                                CommandArgument='<%#Eval("DocEntry") + ";" + Eval("PRNo") + ";" %>' Font-Bold="true"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="WEB PR NO">
                                        <ItemStyle HorizontalAlign="Center" Width="50px" />
                                        <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblWebPRNo" runat="server" Text='<%# Bind("WebPRNo") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="OUTLET">
                                        <ItemStyle HorizontalAlign="Center" Width="50px" />
                                        <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblOutlet" runat="server" Text='<%# Bind("Outlet") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="ORDER DATE">
                                        <ItemStyle HorizontalAlign="Center" Width="50px" />
                                        <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblOrderDate" runat="server" Text='<%# Bind("OrderDate","{0:dd/MM/yyyy}") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="URGENT">
                                        <ItemStyle HorizontalAlign="Center" Width="50px" />
                                        <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblUrgent" runat="server" Text='<%# Bind("Urgent") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="TOTAL SPEND" Visible="false">
                                        <ItemStyle HorizontalAlign="Center" Width="50px" />
                                        <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblTotalSpend" runat="server" Text='<%# Bind("TotalSpend") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="TotalSpendValue" HeaderText="TOTAL SPEND" HeaderStyle-HorizontalAlign="Center">
                                        <ItemStyle HorizontalAlign="Center" Width="50px" />
                                        <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="STATUS">
                                        <ItemStyle HorizontalAlign="Center" Width="50px" />
                                        <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblStatus" runat="server" Text='<%# Bind("Status") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="USER NAME">
                                        <ItemStyle HorizontalAlign="Center" Width="50px" />
                                        <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
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
                                                <span>SAP PR/PQ NO</span>
                                            </th>
                                            <th>
                                                <span>WEB PR NO</span>
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
