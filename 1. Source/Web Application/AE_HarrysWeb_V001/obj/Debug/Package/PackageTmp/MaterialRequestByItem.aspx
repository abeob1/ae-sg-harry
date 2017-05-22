<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MaterialRequestByItem.aspx.cs"
    Inherits="AE_HarrysWeb_V001.MaterialRequestByItem" MasterPageFile="~/Main.Master" %>

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
        //        function keyPressListener(e) {
        //            if (e.keyCode == 13) {
        //                 do something
        //            }
        //        }
    </script>
    <div>
        <asp:UpdatePanel ID="updatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div style="height: 5px;">
                </div>
                <h2>
                    <asp:Label ID="lblTitle" runat="server" Text="Material Request By Item"></asp:Label>
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
                                    OnSelectedIndexChanged="ddlWareHouse_SelectedIndexChanged" Style="width: 450px;">
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
                            <td style="width: 200px">
                                Delivery Date
                            </td>
                            <td>
                                :
                            </td>
                            <td>
                                <asp:TextBox ID="txtDeliveryDate" runat="server" AutoPostBack="True" CssClass="txtbox"></asp:TextBox>
                                <asp:ImageButton ID="Image1" runat="Server" Style="width: 15px;" AlternateText="Click to show calendar"
                                    ImageUrl="Images/Calender.jpg" />
                                <cc1:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtDeliveryDate"
                                    PopupButtonID="Image1" Format="dd/MM/yyyy">
                                </cc1:CalendarExtender>
                            </td>
                        </tr>
                        <%--                        <tr>
                            <td>
                                Total Outlet Spend FOR ORDER
                            </td>
                            <td>
                                :
                            </td>
                            <td>
                                <asp:Label ID="lblTotalOutlet" runat="server"></asp:Label>
                            </td>
                        </tr>--%>
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
                <table>
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
                        </td>
                        <td>
                            <asp:Button ID="btnSearch" runat="server" Text="::" Style="text-align: center;" Height="18px"
                                Width="35px" OnClick="btnSearch_Click" />
                        </td>
                    </tr>
                </table>
                <div style="margin-left: 0px; width: 100%">
                    <asp:GridView ID="grvParentGrid" runat="server" CssClass="GridInner" Width="100%"
                        DataKeyNames="SupplierCode" OnRowDataBound="OnRowDataBound" AllowSorting="True"
                        AllowPaging="false" AutoGenerateColumns="False" CellPadding="4" HeaderStyle-Height="27px"
                        HeaderStyle-VerticalAlign="Middle" EnableModelValidation="True" ForeColor="#333333"
                        GridLines="None">
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
                            <asp:BoundField DataField="SupplierCode" HeaderText="Supplier Code" HeaderStyle-HorizontalAlign="Left"
                                ItemStyle-Width="150px">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle Width="150px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="SupplierName" HeaderText="Supplier Name" HeaderStyle-HorizontalAlign="Left"
                                ItemStyle-Width="150px">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle Width="300px" />
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
                                                OnRowDataBound="grvSupplierItemList_OnRowDataBound" OnPageIndexChanging="grvSupplierItemList_PageIndexChanging"
                                                AutoGenerateColumns="False" CellPadding="2" HeaderStyle-Height="27px" CellSpacing="2"
                                                HeaderStyle-VerticalAlign="Middle" PageSize="20" ShowFooter="true">
                                                <PagerSettings Mode="NumericFirstLast" />
                                                <PagerStyle BackColor="#1B3B5F" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle"
                                                    CssClass="pager-row" />
                                                <%-- <PagerStyle BackColor="#70A0D0" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle" />--%>
                                                <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px"
                                                    CssClass="row" />
                                                <FooterStyle BackColor="#7E7E7E" Font-Bold="True" ForeColor="White" />
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
                                                    <asp:TemplateField HeaderText="Description">
                                                        <ItemStyle HorizontalAlign="left" Width="500px" />
                                                        <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblDescription" runat="server" Text='<%# Bind("Description") %>' BorderStyle="none" />
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
                                                            <asp:TextBox ID="txtOrderQuantity" runat="server" Width="97%" Text='<%# Eval("OrderQuantity")%>'
                                                                CssClass="txtbox" OnTextChanged="txtOrderQuantity_OnTextChanged" AutoPostBack="true"
                                                                TabIndex="1" OnKeyPress="return isNumberKey(this, event);" Style="text-align: right" />
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
                                                    <asp:TemplateField HeaderText="Remarks">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                        <HeaderStyle VerticalAlign="Middle" Width="350px" />
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtRemarks" runat="server" Width="97%" Text='<%# Eval("Remarks")%>'
                                                                OnTextChanged="txtRemarks_OnTextChanged" MaxLength="250" CssClass="txtbox" AutoPostBack="true"
                                                                Style="text-align: left; text-indent: 5px;" />
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
                                                            <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                                                <span>Remarks</span>
                                                            </th>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="13">
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
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Remarks</span>
                                    </th>
                                </tr>
                                <tr>
                                    <td colspan="13">
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
                    </tr>
                    <tr>
                        <td colspan="2">
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
                        </td>
                    </tr>
                </table>
                <asp:Timer ID="Timer1" runat="server" Interval="1" OnTick="Timer1_Tick" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
