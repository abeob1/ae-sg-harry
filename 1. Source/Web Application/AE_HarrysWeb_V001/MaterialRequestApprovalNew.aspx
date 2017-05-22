<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MaterialRequestApprovalNew.aspx.cs"
    Inherits="AE_HarrysWeb_V001.MaterialRequestApprovalNew" MasterPageFile="~/Main.Master" %>

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
                    <asp:Label ID="lblTitle" runat="server" Text="Material Request Approval"></asp:Label>
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
                    </table>
                    <hr />
                </div>
                <div style="margin-left: 0px; width: 100%">
                    <asp:GridView ID="grvParentGrid" runat="server" CssClass="GridInner" Width="100%"
                        DataKeyNames="DocEntry" OnRowDataBound="OnRowDataBound" AllowSorting="True" AllowPaging="false"
                        AutoGenerateColumns="False" CellPadding="4" HeaderStyle-Height="27px" HeaderStyle-VerticalAlign="Middle"
                        EnableModelValidation="True" ForeColor="#333333" GridLines="None">
                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle Height="27px" VerticalAlign="Middle" BackColor="#5D7B9D" Font-Bold="True"
                            ForeColor="White" />
                        <PagerStyle BackColor="#284775" HorizontalAlign="Center" VerticalAlign="Middle" ForeColor="White" CssClass="pager-row"/>
                        <%-- <PagerStyle BackColor="#70A0D0" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle" />--%>
                        <RowStyle BackColor="#F7F6F3" BorderColor="White" BorderWidth="2px" Height="25px"
                            ForeColor="#333333"  CssClass="row"/>
                        <AlternatingRowStyle BackColor="White" BorderColor="White" BorderWidth="2px" ForeColor="#284775" />
                        <Columns>
                            <asp:TemplateField ItemStyle-Width="20px">
                                <ItemTemplate>
                                </ItemTemplate>
                                <ItemStyle Width="20px" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="DocEntry" HeaderText="PR Draft #" HeaderStyle-HorizontalAlign="Left"
                                ItemStyle-Width="150px">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle Width="150px" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="#" Visible="false">
                                <ItemStyle HorizontalAlign="Center" Width="20px" />
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <asp:Label ID="lblPRNo" runat="server" Text='<%# Bind("DocEntry") %>' BorderStyle="none">
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" ItemStyle-Width="140px">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle Width="80px" />
                                <ItemTemplate>
                                    <asp:CheckBoxList ID="chkApprove" AutoPostBack="true" runat="server" RepeatDirection="Horizontal"
                                        Font-Bold="True" OnSelectedIndexChanged="chkApprove_SelectedIndexChnaged">
                                        <asp:ListItem>Approve</asp:ListItem>
                                        <%--<asp:ListItem>Reject</asp:ListItem>--%>
                                    </asp:CheckBoxList>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" ItemStyle-Width="130px">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle Width="80px" />
                                <ItemTemplate>
                                    <asp:CheckBoxList ID="chkReject" AutoPostBack="true" runat="server" RepeatDirection="Horizontal" Visible="false"
                                        Font-Bold="True" OnSelectedIndexChanged="chkReject_SelectedIndexChnaged">
                                        <asp:ListItem>Reject</asp:ListItem>
                                    </asp:CheckBoxList>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" ItemStyle-Width="130px">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle Width="700px" />
                                <ItemTemplate>
                                    <asp:CheckBoxList ID="chkAddDeliveryCharge" runat="server" RepeatDirection="Horizontal"
                                        Font-Bold="True">
                                        <asp:ListItem>Add Delivery Charge if Minimum Spend not meet after Reduced Order Quantity</asp:ListItem>
                                    </asp:CheckBoxList>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <tr>
                                        <td colspan="100%">
                                            <asp:GridView ID="grvChildGrid" CssClass="GridInner" runat="server" Width="100%"
                                                BorderColor="White" BackColor="White" AllowSorting="True" AllowPaging="true"
                                                OnRowDataBound="grvChildGrid_OnRowDataBound" OnPageIndexChanging="grvChildGrid_PageIndexChanging"
                                                AutoGenerateColumns="False" CellPadding="2" HeaderStyle-Height="27px" CellSpacing="2"
                                                HeaderStyle-VerticalAlign="Middle" PageSize="50" ShowFooter="true">
                                                <PagerSettings Mode="NumericFirstLast" />
                                                <PagerStyle BackColor="#1B3B5F" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                <%-- <PagerStyle BackColor="#70A0D0" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle" />--%>
                                                <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px" />
                                                <FooterStyle BackColor="#7E7E7E" Font-Bold="True" ForeColor="White" />
                                                <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="#" Visible="false">
                                                        <ItemStyle HorizontalAlign="Center" Width="20px" />
                                                        <HeaderStyle VerticalAlign="Middle" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblDocEntry" runat="server" Text='<%# Bind("PRNo") %>' BorderStyle="none">
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="#" Visible="false">
                                                        <ItemStyle HorizontalAlign="Center" Width="20px" />
                                                        <HeaderStyle VerticalAlign="Middle" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblDocType" runat="server" Text='<%# Bind("DocType") %>' BorderStyle="none">
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
                                                    <asp:TemplateField HeaderText="Supplier Name">
                                                        <ItemStyle HorizontalAlign="left" Width="500px" />
                                                        <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSupplier" runat="server" Text='<%# Bind("SupplierName") %>' BorderStyle="none" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Description & Remarks">
                                                        <ItemStyle HorizontalAlign="left" Width="500px" />
                                                        <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("Description") %>' BorderStyle="none" /><br />
                                                            <asp:Label ID="lblRemarks" runat="server" ForeColor ="#00cc00"  Text='<%# RemarksNullCheck(Eval("Remarks")) %>' BorderStyle="none" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Min Spend">
                                                        <ItemStyle HorizontalAlign="left" Width="80px" />
                                                        <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblMinSpend" runat="server" Text='<%# Bind("MinSpend") %>' BorderStyle="none" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Delivery Date">
                                                        <ItemStyle HorizontalAlign="left" Width="80px" />
                                                        <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblDeliveryDate" runat="server" Text='<%# Bind("DeliveryDate") %>'
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
                                                            <asp:TextBox ID="txtOrderQuantity" runat="server" Width="97%" Text='<%# Eval("OrderQuantity")%>'
                                                                CssClass="txtbox" OnTextChanged="txtOrderQuantity_OnTextChanged" AutoPostBack="true"
                                                                OnKeyPress="return isNumberKey(this, event);" Style="text-align: right" />
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
                                                     <asp:TemplateField HeaderText="Approver Remarks">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                        <HeaderStyle VerticalAlign="Middle" Width="350px" />
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtApproverRemarks" runat="server" Width="97%" Text='<%# Eval("ApproverRemarks")%>'
                                                                OnTextChanged="txtApproverRemarks_OnTextChanged"  MaxLength = "250" CssClass="txtbox" AutoPostBack="true"
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
                                                                <span>Supplier Name</span>
                                                            </th>
                                                            <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                                                <span>Description & Remarks</span>
                                                            </th>
                                                            <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                                                <span>Min Spend</span>
                                                            </th>
                                                            <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                                                <span>Delivery Date</span>
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
                                                            <td colspan="15">
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
                                        <span>Supplier Name</span>
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Description & Remarks</span>
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Min Spend</span>
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Delivery Date</span>
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
                                    </th
                                </tr>
                                <tr>
                                    <td colspan="15">
                                        <span>No Data</span>
                                    </td>
                                </tr>
                            </table>
                        </EmptyDataTemplate>
                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                    </asp:GridView>
                </div>
                <hr />
                <table style="width: 100%">
                    <tr align="right">
                        <td>
                            <asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                        </td>
                        <td style="width: 400px;">
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit" Style="background-image: url('Images/bgButton.png');
                                background-repeat: no-repeat; width: 85px; color: White;" BorderStyle="Solid"
                                OnClick="btnSubmit_Click" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" Style="background-image: url('Images/bgButton.png');
                                background-repeat: no-repeat; width: 85px; color: White;" BorderStyle="Solid"
                                OnClick="btnCancel_Click" />
                        </td>
                    </tr>
                </table>
                <%--<asp:Timer ID="Timer1" runat="server" Interval="1" OnTick="Timer1_Tick" />--%>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
