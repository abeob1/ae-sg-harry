<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListOfInventoryTransfer.aspx.cs"
    Inherits="AE_HarrysWeb_V001.ListOfInventoryTransfer" MasterPageFile="~/Main.Master" %>

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
                    <asp:Label ID="lblTitle" runat="server" Text="List of Inventory Transfers"></asp:Label>
                    <%--<asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>--%>
                </h2>
                <div style="margin-left: 5px; width: 99%;">
                    <hr />
                    <table border="0" width="100%" style="background-color: #D1D4D8; font-weight: bold;">
                        <tr>
                            <td class="style16">
                                Transfer From Outlet
                            </td>
                            <td class="style17">
                                :
                            </td>
                            <td style="width: 250px;">
                                <asp:DropDownList ID="ddlFromOutlet" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlFromOutlet_SelectedIndexChanged"
                                    CssClass="dropdownlist" Style="width: 250px;">
                                    <asp:ListItem Value=""> --- Select Outlet --- </asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td class="style16">
                                Transfer To Outlet
                            </td>
                            <td class="style17">
                                :
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlToOutlet" runat="server" AutoPostBack="false" CssClass="dropdownlist"
                                    Style="width: 250px;">
                                    <asp:ListItem Value=""> --- Select Outlet --- </asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td align="right" rowspan="5">
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
                                    Style="width: 250px;">
                                    <asp:ListItem Value="S" Selected="True"> --- Select Status --- </asp:ListItem>
                                    <asp:ListItem Value="O">Open</asp:ListItem>
                                    <asp:ListItem Value="C">Closed</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="style16">
                                Transfer Date From
                            </td>
                            <td class="style17">
                                :
                            </td>
                            <td>
                                <asp:TextBox ID="txtFromDate" runat="server" AutoPostBack="false" CssClass="txtbox"
                                    MaxLength="10" OnKeyPress="return isNumberKey(this, event);"></asp:TextBox>
                                <asp:ImageButton ID="imgFromDate" runat="Server" Style="width: 15px;" AlternateText="Click to show calendar"
                                    ImageUrl="Images/Calender.jpg" />
                                &nbsp;
                                <cc1:CalendarExtender ID="clnFromDate" runat="server" TargetControlID="txtFromDate"
                                    PopupButtonID="imgFromDate" Format="dd/MM/yyyy">
                                </cc1:CalendarExtender>
                            </td>
                            <td class="style16">
                                Transfer Date To
                            </td>
                            <td class="style17">
                                :
                            </td>
                            <td>
                                <asp:TextBox ID="txtToDate" runat="server" AutoPostBack="false" CssClass="txtbox"
                                    MaxLength="10" OnKeyPress="return isNumberKey(this, event);"></asp:TextBox>
                                <asp:ImageButton ID="imgToDate" runat="Server" AlternateText="Click to show calendar"
                                    ImageUrl="Images/Calender.jpg" Style="width: 15px;" />
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <cc1:CalendarExtender ID="ClnToDate" runat="server" TargetControlID="txtToDate" PopupButtonID="imgToDate"
                                    Format="dd/MM/yyyy">
                                </cc1:CalendarExtender>
                                <asp:Button ID="btnSearch" runat="server" Height="28px" Style="text-align: center;"
                                    OnClick="btnSearch_Click" Text="Search" Width="60px" />
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Label ID="lblMessage" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>
                <hr />
                <div style="width: 100%;">
                    <table border="0" width="100%" style="background-color: #D1D4D8; font-weight: bold;">
                        <asp:GridView ID="grvIT" CssClass="GridInner" runat="server" Width="100%" BorderColor="White"
                            BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                            HeaderStyle-Height="25px" CellSpacing="2" HeaderStyle-VerticalAlign="Middle"
                            AllowPaging="true" PageSize="50" OnPageIndexChanging="grvIT_PageIndexChanging">
                            <PagerSettings Mode="NumericFirstLast" />
                            <PagerStyle BackColor="#1B3B5F" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle"
                                CssClass="pager-row" />
                            <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px"
                                CssClass="row" />
                            <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                            <Columns>
                                <asp:TemplateField HeaderText="Transfer Request No">
                                    <ItemStyle HorizontalAlign="Center" Width="30px" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkITReqNO" runat="server" Font-Bold="true" Text='<%# Bind("RequestNo") %>'
                                            OnClick="lnkITReqNO_Click" CommandArgument='<%#Eval("DocEntry") + ";" + Eval("RequestNo") + ";" + Eval("Remarks") + ";" + Eval("Status") + ";" + Eval("RequestedBy") + ";" %>'></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="From Outlet">
                                    <ItemStyle HorizontalAlign="Center" Width="30px" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblFromOutlet" runat="server" Text='<%# Bind("FromOutlet") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="To Outlet">
                                    <ItemStyle HorizontalAlign="Center" Width="30px" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblToOutlet" runat="server" Text='<%# Bind("ToOutlet") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Request Date">
                                    <ItemStyle HorizontalAlign="Center" Width="20px" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblRequestDate" runat="server" Text='<%# Bind("RequestDate","{0:dd/MM/yyyy}") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Transfer Date">
                                    <ItemStyle HorizontalAlign="Center" Width="20px" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblTransferDate" runat="server" Text='<%# Bind("TransferDate","{0:dd/MM/yyyy}") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Request By">
                                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblRequestedBy" runat="server" Text='<%# Bind("RequestedBy") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Accepted By" Visible="false">
                                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblApprovedBy" runat="server" Text='<%# Bind("ApprovedBy") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Status">
                                    <ItemStyle HorizontalAlign="Center" Width="30px" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblStatus" runat="server" Text='<%# Bind("Status") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Remarks" Visible="false">
                                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblRemarks" runat="server" Text='<%# Bind("Remarks") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Remarks">
                                    <ItemStyle HorizontalAlign="Left" Width="180px" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblDisplayRemarks" runat="server" Text='<%# Bind("DisplayRemarks") %>'></asp:Label>
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
                                        <th style="width: 130px;">
                                            <span>Transfer Request No</span>
                                        </th>
                                        <th style="width: 230px;">
                                            <span>From Outlet</span>
                                        </th>
                                        <th style="width: 230px;">
                                            <span>To Outlet</span>
                                        </th>
                                        <th>
                                            <span>Request Date</span>
                                        </th>
                                        <th>
                                            <span>Transfer Date</span>
                                        </th>
                                        <th>
                                            <span>Request By</span>
                                        </th>
                                        <%--<th>
                                            <span>Accepted By</span>
                                        </th>--%>
                                        <th>
                                            <span>Status</span>
                                        </th>
                                        <th>
                                            <span>Remarks</span>
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
