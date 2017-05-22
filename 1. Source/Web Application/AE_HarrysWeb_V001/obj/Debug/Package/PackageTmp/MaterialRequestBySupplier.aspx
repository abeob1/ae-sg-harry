<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MaterialRequestBySupplier.aspx.cs"
    Inherits="AE_HarrysWeb_V001.MaterialRequestBySupplier" MasterPageFile="~/Main.Master" %>

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
        function FindNextRow(txtQuantityID) {
            if (window.event) { e = window.event; }
            if (e.keyCode == 13) {
                document.getElementById(txtQuantityID).select();
            }
            return false;
        }
    </script>
    <div>
        <asp:UpdatePanel ID="updatePanel" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div style="height: 5px;">
                </div>
                <h2>
                    <asp:Label ID="lblTitle" runat="server" Text="Material Request By Supplier"></asp:Label>
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
                                <asp:Label ID="lblPriority" runat="server"></asp:Label>
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
                            <asp:DropDownList ID="ddlSupplier" runat="server" AutoPostBack="true" CssClass="dropdownlist"
                                Style="width: 450px;" OnSelectedIndexChanged="ddlSupplier_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td style="width: 100px">
                            Min Spend
                        </td>
                        <td>
                            :
                        </td>
                        <td style="width: 100px">
                            <asp:Label ID="lblMinSpend" runat="server"></asp:Label>
                        </td>
                        <td style="width: 100px">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Delivery Calendar
                        </td>
                        <td>
                            :
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
                <div style="width: 100%">
                    <asp:GridView ID="grvMRBS" CssClass="GridInner" runat="server" Width="100%" BorderColor="White"
                        BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                        HeaderStyle-Height="25px" CellSpacing="2" HeaderStyle-VerticalAlign="Middle"
                        AllowPaging="true" PageSize="10" OnRowCreated="grvMRBS_RowCreated" OnPageIndexChanging="grvMRBS_PageIndexChanging"
                        OnRowDataBound="grvMRBS_RowDataBound">
                        <PagerSettings Mode="NumericFirstLast" />
                        <PagerStyle BackColor="#1B3B5F" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle"
                            CssClass="pager-row" />
                        <%-- <PagerStyle BackColor="#70A0D0" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle" />--%>
                        <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px"
                            CssClass="row" />
                        <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                        <Columns>
                            <asp:TemplateField HeaderText="#" Visible="false">
                                <ItemStyle HorizontalAlign="Center" Width="20px" />
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <asp:Label ID="lblNo" runat="server" Text='<%# Bind("No") %>' BorderStyle="none">
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
                                    <asp:Label ID="txtDescription" runat="server" Text='<%# Bind("Description") %>' BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="In Stock">
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="txtInStock" runat="server" Text='<%# String.Format("{0,-15:#,##0.00}",Eval("InStock")) %>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Event Order">
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemStyle HorizontalAlign="Right" Width="80px" />
                                <ItemTemplate>
                                    <asp:Label ID="txtEventOrder" runat="server" Text='<%# String.Format("{0,-15:#,##0.00}",Eval("EventOrder")) %>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Last 7 Days Avg Req">
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemStyle HorizontalAlign="Right" Width="80px" />
                                <ItemTemplate>
                                    <asp:Label ID="txtL7DaysAvg" runat="server" Text='<%# String.Format("{0,-15:#,##0.00}",Eval("Last7DaysAvg")) %>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Already ordered">
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemStyle HorizontalAlign="Right" Width="80px" />
                                <ItemTemplate>
                                    <asp:Label ID="txtAlreadyOrdered" runat="server" Text='<%# String.Format("{0,-15:#,##0.00}",Eval("AlreadyOrdered"))%>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Min Stock">
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemStyle HorizontalAlign="Right" Width="80px" />
                                <ItemTemplate>
                                    <asp:Label ID="txtMinStock" runat="server" Text='<%# String.Format("{0,-15:#,##0.00}",Eval("MinStock"))%>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Recommended Quantity">
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemStyle HorizontalAlign="Right" Width="80px" />
                                <ItemTemplate>
                                    <asp:Label ID="lblRecommendedQuantity" runat="server" Text='<%# String.Format("{0,-15:#,##0.00}",Eval("RecommendedQuantity"))%>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Order Quantity">
                                <ItemStyle HorizontalAlign="Right" />
                                <HeaderStyle VerticalAlign="Middle" Width="100px" />
                                <ItemTemplate>
                                    <asp:TextBox ID="txtOrderQuantity" runat="server" Width="97%" Text='<%# Eval("OrderQuantity")%>'
                                        CssClass="txtbox" AutoPostBack="true" OnKeyPress="return isNumberKey(this, event);"
                                        OnTextChanged="txtOrderQuantity_OnTextChanged" Style="text-align: right;" />
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
                                    <asp:Label ID="txtTotal" runat="server" Text='<%#String.Format("{0,-15:#,##0.00}", Eval("Total"))%>'
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
                                    <th>
                                        <span>Min Stock</span>
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
                                    <th>
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
                            <div style="margin-left: 47.4%;">
                                <table border="0">
                                    <tr valign="middle" style="height: 27px; font-weight: bold; text-decoration: none;
                                        background-color: #D9E0ED;">
                                        <td style="width: 150px" align="right">
                                            Total:
                                        </td>
                                        <td align="right" style="width: 90px;">
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
                            <asp:Button ID="btndeleteDraft" runat="server" Text="Delete Draft" Visible="false"
                                Style="background-image: url('Images/bgButton.png'); background-repeat: no-repeat;
                                width: 85px; color: White;" BorderStyle="Solid" />
                            <asp:Button ID="btnSaveDraft" runat="server" Text="Save" Style="background-image: url('Images/bgButton.png');
                                background-repeat: no-repeat; width: 85px; color: White;" BorderStyle="Solid"
                                OnClick="btnSaveDraft_Click" />
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit" Visible="false" Style="background-image: url('Images/bgButton.png');
                                background-repeat: no-repeat; width: 85px; color: White;" BorderStyle="Solid"
                                OnClick="btnSubmit_Click" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
