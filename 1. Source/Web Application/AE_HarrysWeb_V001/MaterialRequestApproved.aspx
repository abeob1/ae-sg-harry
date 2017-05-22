<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MaterialRequestApproved.aspx.cs"
    Inherits="AE_HarrysWeb_V001.MaterialRequest" MasterPageFile="~/Main.Master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
        <asp:UpdatePanel ID="updatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div style="height: 5px;">
                </div>
                <h2>
                    <asp:Label ID="lblTitle" runat="server" Text="Material Request Approved"></asp:Label>
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
                                <asp:Label ID="lblOutlet" runat="server"></asp:Label>
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
                            <td>
                                User Name
                            </td>
                            <td>
                                :
                            </td>
                            <td>
                                <asp:Label ID="lblUserName" runat="server"></asp:Label>
                            </td>
                            <td>
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
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 200px">
                                Delivery Date
                            </td>
                            <td>
                                :
                            </td>
                            <td>
                                <asp:Label ID="lblDeliveryDate" runat="server"></asp:Label>
                            </td>
                            <td>
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
                            <td style="vertical-align:top;">
                                <img src="Images/Dark Green.png" alt="User Remarks" style="background-color: transparent" />
                                User Remarks &nbsp;&nbsp;&nbsp;<img src="Images/Red.png" alt="Approver Remarks" style="background-color: transparent" />Approver
                                Remarks
                            </td>
                            <td align="right" style="padding-right: 3%; font-size: medium;">
                                <asp:Label ID="lblPrNO" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <hr />
                </div>
                <div style="margin-left: 0px; width: 100%">
                    <asp:GridView ID="grvParentGrid" runat="server" CssClass="GridInner" Width="100%"
                        DataKeyNames="CardCode" AllowSorting="True" OnRowDataBound="OnRowDataBound" AllowPaging="false"
                        AutoGenerateColumns="False" CellPadding="4" HeaderStyle-Height="27px" HeaderStyle-VerticalAlign="Middle"
                        EnableModelValidation="True" ForeColor="#333333" GridLines="None">
                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle Height="27px" VerticalAlign="Middle" BackColor="#5D7B9D" Font-Bold="True"
                            ForeColor="White" />
                        <PagerStyle BackColor="#284775" HorizontalAlign="Center" VerticalAlign="Middle" ForeColor="White"
                            CssClass="pager-row" />
                        <%-- <PagerStyle BackColor="#70A0D0" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle" />--%>
                        <RowStyle BackColor="#F7F6F3" BorderColor="White" BorderWidth="2px" Height="25px"
                            ForeColor="#333333" CssClass="row" />
                        <AlternatingRowStyle BackColor="White" BorderColor="White" BorderWidth="2px" ForeColor="#284775" />
                        <Columns>
                            <asp:TemplateField ItemStyle-Width="20px">
                                <ItemTemplate>
                                </ItemTemplate>
                                <ItemStyle Width="20px" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="CardCode" HeaderText="Supplier Code" HeaderStyle-HorizontalAlign="Left"
                                ItemStyle-Width="150px">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle Width="150px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="CardName" HeaderText="Supplier Name" HeaderStyle-HorizontalAlign="Left"
                                ItemStyle-Width="150px">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle Width="400px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="MinSpend" HeaderText="Min Spend" HeaderStyle-HorizontalAlign="Left"
                                Visible="false" ItemStyle-Width="150px">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle Width="150px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="MinSpendValue" HeaderText="Min Spend" HeaderStyle-HorizontalAlign="Left"
                                ItemStyle-Width="150px">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle Width="150px" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Delivery Calendar" HeaderStyle-HorizontalAlign="Left"
                                ItemStyle-Width="150px">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle Width="150px" />
                                <ItemTemplate>
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
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <tr>
                                        <td colspan="100%">
                                            <asp:GridView ID="grvSupplierItemList" CssClass="GridInner" runat="server" Width="100%"
                                                BorderColor="White" BackColor="White" AllowSorting="True" AllowPaging="true"
                                                OnPageIndexChanging="grvSupplierItemList_PageIndexChanging" OnRowDataBound="grvSupplierItemList_OnRowDataBound"
                                                AutoGenerateColumns="False" CellPadding="2" HeaderStyle-Height="27px" CellSpacing="2"
                                                HeaderStyle-VerticalAlign="Middle" PageSize="20" ShowFooter="true">
                                                <PagerSettings Mode="NumericFirstLast" />
                                                <PagerStyle BackColor="#1B3B5F" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                <%-- <PagerStyle BackColor="#70A0D0" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle" />--%>
                                                <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px" />
                                                <FooterStyle BackColor="#1B3B61" Font-Bold="True" ForeColor="White" />
                                                <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="#">
                                                        <ItemStyle HorizontalAlign="Center" Width="20px" />
                                                        <HeaderStyle VerticalAlign="Middle" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblNo" runat="server" Text='<%# Bind("No") %>' BorderStyle="none">
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="SupplierCode" Visible="false">
                                                        <ItemStyle HorizontalAlign="Center" Width="20px" />
                                                        <HeaderStyle VerticalAlign="Middle" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSupplierCode" runat="server" Text='<%# Bind("SupplierCode") %>'
                                                                BorderStyle="none">
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="ItemCode" Visible="false">
                                                        <ItemStyle HorizontalAlign="Center" Width="20px" />
                                                        <HeaderStyle VerticalAlign="Middle" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblItemCode" runat="server" Text='<%# Bind("ItemCode") %>' BorderStyle="none">
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Description & Remarks">
                                                        <ItemStyle HorizontalAlign="left" Width="500px" />
                                                        <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblDescription" runat="server" Text='<%# Bind("Description") %>' BorderStyle="none" /><br />
                                                            <asp:Label ID="lblUserRemarks" runat="server" ForeColor="DarkGreen" Text='<%# RemarksNullCheck(Eval("UserRemarks")) %>'
                                                                BorderStyle="none" /><br />
                                                            <asp:Label ID="lblApprRemarks" runat="server" ForeColor="Red" Text='<%# RemarksNullCheck(Eval("ApprRemarks")) %>'
                                                                BorderStyle="none" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="In Stock">
                                                        <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblInStock" runat="server" Text='<%# String.Format("{0,-15:#,##0.0000}",Eval("InStock")) %>'
                                                                BorderStyle="none" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Event Order">
                                                        <HeaderStyle VerticalAlign="Middle" />
                                                        <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblEventOrder" runat="server" Text='<%# String.Format("{0,-15:#,##0.0000}",Eval("EventOrder")) %>'
                                                                BorderStyle="none" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Last 7 Days Avg Req">
                                                        <HeaderStyle VerticalAlign="Middle" />
                                                        <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblL7DaysAvg" runat="server" Text='<%# String.Format("{0,-15:#,##0.0000}",Eval("Last7DaysAvg")) %>'
                                                                BorderStyle="none" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Already ordered">
                                                        <HeaderStyle VerticalAlign="Middle" />
                                                        <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblAlreadyOrdered" runat="server" Text='<%# String.Format("{0,-15:#,##0.0000}",Eval("AlreadyOrdered"))%>'
                                                                BorderStyle="none" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Min Stock">
                                                        <HeaderStyle VerticalAlign="Middle" />
                                                        <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblMinStock" runat="server" Text='<%# String.Format("{0,-15:#,##0.0000}",Eval("MinStock"))%>'
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
                                                            <asp:Label ID="lblOrderQuantity" runat="server" Text='<%# Bind("OrderQuantity") %>'
                                                                BorderStyle="none" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="UoM">
                                                        <HeaderStyle VerticalAlign="Middle" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblUoM" runat="server" Text='<%# Bind("UOM") %>' BorderStyle="none" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Price" FooterText="Total">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                        <HeaderStyle VerticalAlign="Middle" Width="120px" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblPrice" runat="server" Text='<%# String.Format("{0,-15:#,##0.0000}",Eval("Price"))%>'
                                                                BorderStyle="none" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Total">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                        <HeaderStyle VerticalAlign="Middle" Width="120px" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblTotal" runat="server" Text='<%#String.Format("{0,-15:#,##0.0000}", Eval("Total"))%>'
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
                                                            <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                                                <span>Description</span>
                                                            </th>
                                                            <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                                                <span>In Stock</span>
                                                            </th>
                                                            <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                                                <span>Event Order</span>
                                                            </th>
                                                            <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                                                <span>Last 7 days Avg Req</span>
                                                            </th>
                                                            <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                                                <span>Already Ordered</span>
                                                            </th>
                                                            <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                                                <span>Min Stock</span>
                                                            </th>
                                                            <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                                                <span>Recommended Quantity</span>
                                                            </th>
                                                            <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                                                <span>Ordered Quantity</span>
                                                            </th>
                                                            <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                                                <span>UOM</span>
                                                            </th>
                                                            <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                                                <span>Price</span>
                                                            </th>
                                                            <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
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
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EditRowStyle BackColor="#999999" />
                        <EmptyDataTemplate>
                            <table class="GridInner" style="width: 100%; border-color: White;" border="1" rules="all"
                                cellspacing="2" cellpadding="2">
                                <tr valign="middle" style="height: 27px; color: white; font-weight: bold; text-decoration: none;
                                    background-color: rgb(27,59,97);">
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Description</span>
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>In Stock</span>
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Event Order</span>
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Last 7 days Avg Req</span>
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Already Ordered</span>
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Min Stock</span>
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Ordered Quantity</span>
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>UOM</span>
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Price</span>
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
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
                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
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
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
