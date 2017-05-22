<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StocktakeCountingSheet.aspx.cs"
    Inherits="AE_HarrysWeb_V001.StocktakeCounting" MasterPageFile="~/Main.Master" %>

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
        .pop
        {
            background-color: transparent;
            position: absolute;
            top: -230px;
            left: -280px;
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
        <asp:UpdatePanel ID="updatePanel" runat="server">
            <ContentTemplate>
                <div style="height: 5px;">
                </div>
                <h2>
                    <asp:Label ID="lblTitle" runat="server" Text="Stock Take Counting Sheet "></asp:Label>
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
                            <td style="width: 200px">
                                Count Date
                            </td>
                            <td>
                                :
                            </td>
                            <td>
                                <asp:TextBox ID="txtCountDate" runat="server" AutoPostBack="True" CssClass="txtbox"
                                    MaxLength="10"></asp:TextBox>
                                <asp:ImageButton ID="Image1" runat="Server" Style="width: 15px;" AlternateText="Click to show calendar"
                                    ImageUrl="Images/Calender.jpg" />
                                <cc1:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtCountDate"
                                    PopupButtonID="Image1" Format="dd/MM/yyyy">
                                </cc1:CalendarExtender>
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
                                <asp:Label ID="lblStatus" runat="server"></asp:Label>
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                &nbsp;
                                <asp:Button ID="btnSearch" runat="server" Text="Search" Style="background-image: url('Images/bgButton.png');
                                    background-repeat: no-repeat; width: 85px; color: White;" BorderStyle="Solid"
                                    OnClick="btnSearch_Click" />
                            </td>
                        </tr>
                    </table>
                    <hr />
                </div>
                <div style="width: 100%">
                    <asp:GridView ID="grvStkCnt" CssClass="GridInner" runat="server" Width="100%" BorderColor="White"
                        BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                        HeaderStyle-Height="25px" CellSpacing="2" HeaderStyle-VerticalAlign="Middle"
                        AllowPaging="true" PageSize = "400" OnPageIndexChanging="grvStkCnt_PageIndexChanging">
                        <PagerSettings Mode="NumericFirstLast" />
                        <PagerStyle BackColor="#1B3B5F" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle"
                            CssClass="pager-row" />
                        <%-- <PagerStyle BackColor="#70A0D0" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle" />--%>
                        <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px"
                            CssClass="row" />
                        <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                        <Columns>
                            <asp:TemplateField HeaderText="CardCode" Visible="false">
                                <ItemStyle HorizontalAlign="left" Width="300px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblCardCode" runat="server" Text='<%# Bind("CardCode") %>' BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Supplier Name">
                                <ItemStyle HorizontalAlign="left" Width="300px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblCardName" runat="server" Text='<%# Bind("CardName") %>' BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="ItemCode" Visible="false">
                                <ItemStyle HorizontalAlign="left" Width="300px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblItemCode" runat="server" Text='<%# Bind("ItemCode") %>' BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Description">
                                <ItemStyle HorizontalAlign="left" Width="300px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblDescription" runat="server" Text='<%# Bind("Description") %>' BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Stocktake UoM1" Visible="True">
                                <ItemStyle HorizontalAlign="left" Width="300px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblStockTakeUOM1" runat="server" Text='<%# Bind("StockTakeUOM1") %>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Stocktake Con1" Visible="false">
                                <ItemStyle HorizontalAlign="left" Width="300px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblStockTakeCon1" runat="server" Text='<%# Bind("StockTakeCon1") %>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Total UOM1" Visible="false">
                                <ItemStyle HorizontalAlign="left" Width="300px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblTotalUOM1" runat="server" Text='<%# Bind("TotalUOM1") %>' BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Counted in Stocktake UoM1" Visible="True">
                                <ItemStyle HorizontalAlign="left" Width="300px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:TextBox ID="txtCountedUOM1" runat="server" Width="97%" Text='<%# Bind("CountedUOM1")%>'
                                        CssClass="txtbox" AutoPostBack="false" OnKeyPress="return isNumberKey(this, event);"
                                        Style="text-align:left ; text-indent : 5px;" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Stocktake UoM2" Visible="True">
                                <ItemStyle HorizontalAlign="left" Width="300px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblStockTakeUOM2" runat="server" Text='<%# Bind("StockTakeUOM2") %>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Stocktake Con2" Visible="false">
                                <ItemStyle HorizontalAlign="left" Width="300px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblStockTakeCon2" runat="server" Text='<%# Bind("StockTakeCon2") %>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Total UOM2" Visible="false">
                                <ItemStyle HorizontalAlign="left" Width="300px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblTotalUOM2" runat="server" Text='<%# Bind("TotalUOM2") %>' BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Counted in Stocktake UoM2" Visible="True">
                                <ItemStyle HorizontalAlign="left" Width="300px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:TextBox ID="txtCountedUOM2" runat="server" Width="97%" Text='<%# Bind("CountedUOM2")%>'
                                        CssClass="txtbox" AutoPostBack="false" OnKeyPress="return isNumberKey(this, event);"
                                        Style="text-align: left ; text-indent : 5px;" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Inventory UoM" Visible="True">
                                <ItemStyle HorizontalAlign="left" Width="300px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblInventoryUOM" runat="server" Text='<%# Bind("InventoryUOM") %>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Counted in Inventory UoM" Visible="True">
                                <ItemStyle HorizontalAlign="left" Width="300px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblCountedInvUOM" runat="server" Text='<%# Bind("CountedInvUOM") %>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="In Stock">
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemStyle HorizontalAlign="left" Width="100px" />
                                <ItemTemplate>
                                    <asp:Label ID="lblInStock" runat="server" Text='<%# String.Format("{0,-15:#,##0.0000}",Eval("InStock")) %>'
                                        BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Variance">
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemStyle HorizontalAlign="left" Width="80px" />
                                <ItemTemplate>
                                    <asp:Label ID="lblVariance" runat="server" BorderStyle="none" Text='<%# Bind("Variance") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Inventory UoM" Visible="false">
                                <ItemStyle HorizontalAlign="left" Width="300px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblInventoryUOM1" runat="server" BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Counted in Inventory UoM" Visible="false">
                                <ItemStyle HorizontalAlign="left" Width="300px" />
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemTemplate>
                                    <asp:Label ID="lblCountedInvUOM1" runat="server" BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="In Stock" Visible="false">
                                <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                <ItemStyle HorizontalAlign="left" Width="100px" />
                                <ItemTemplate>
                                    <asp:Label ID="lblInStock1" runat="server" BorderStyle="none" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Variance" Visible="false">
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemStyle HorizontalAlign="left" Width="80px" />
                                <ItemTemplate>
                                    <asp:Label ID="lblVariance1" runat="server" BorderStyle="none" />
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
                                        <span>Description</span>
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Stocktake UoM1</span>
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Counted in Stocktake UoM1</span>
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Stocktake UoM2</span>
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Counted in Stocktake UoM2</span>
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Inventory UOM</span>
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Counted in Inventory UOM</span>
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>In Stock</span>
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        <span>Variance</span>
                                    </th>
                                </tr>
                                <tr>
                                    <td colspan="10">
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
                            <asp:Button ID="btnSaveDraft" runat="server" Text="Save" Style="background-image: url('Images/bgButton.png');
                                background-repeat: no-repeat; width: 85px; color: White;" BorderStyle="Solid"
                                OnClick="btnSaveDraft_Click" />
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit" Style="background-image: url('Images/bgButton.png');
                                background-repeat: no-repeat; width: 85px; color: White;" BorderStyle="Solid"
                                OnClick="btnSubmit_Click" />
                        </td>
                    </tr>
                </table>
                <%--POP UP to show the alert for create the Stock Sheet--%>
                <asp:Button ID="btnHiddenOpen" runat="server" Style="display: none" />
                <cc1:ModalPopupExtender ID="mStkPopup" runat="server" TargetControlID="btnHiddenOpen"
                    PopupControlID="StkPopUpShow">
                </cc1:ModalPopupExtender>
                <asp:Panel ID="StkPopUpShow" runat="server" Style="width: 330px; height: 160px; background-color: #F8F8F8;
                    border: 2px solid #C8C8C8">
                    <asp:UpdatePanel runat="server" ID="updatepnl" UpdateMode="Conditional">
                        <ContentTemplate>
                            <table id="tblHead" style="width: 325px; height: 20px; background-color: #CCCCCC;
                                margin-left: 2px; margin-right: 2px; margin-top: 2px; margin-bottom: 2px; border-style: solid;
                                border-width: 1px; font-weight: bold;">
                                <tr>
                                    <td>
                                        Alert
                                    </td>
                                </tr>
                            </table>
                            <div style="width: 330px; height: 90px; overflow: auto; border: 1px;">
                                <table>
                                    <tr>
                                        <td style='padding: 15px 10px 5px 20px'>
                                            <h3>
                                                <b>Stock Counting sheet does not exists for Selected Date. Do you want to Create a new
                                                    ?</b></h3>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <table style="width: 100%; margin-top: 5px;">
                                <tr align="right">
                                    <td>
                                    </td>
                                    <td style="width: 300px;">
                                        <asp:Button ID="btnStkOk" runat="server" Text="Yes" Style="background-image: url('Images/bgButton.png');
                                            text-align: center; background-repeat: no-repeat; width: 50px; color: White;"
                                            BorderStyle="Solid" OnClick="btnStkOk_Click" />
                                        <asp:Button ID="btnStkCancel" runat="server" Text="No" Style="background-image: url('Images/bgButton.png');
                                            background-repeat: no-repeat; width: 60px; color: White;" BorderStyle="Solid"
                                            OnClick="btnStkCancel_Click" />
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdateProgress ID="UpdateProg2" DisplayAfter="0" runat="server">
                        <ProgressTemplate>
                            <asp:Panel ID="Panel2" runat="server" CssClass="pop">
                                <div style="text-align: center; padding-top: 350px; padding-left: 50px; width: 800px;
                                    height: 700px;">
                                    <asp:Image ID="img" runat="server" Height="50px" ImageUrl="~/Images/loading3.gif"
                                        Width="50px" />
                                </div>
                            </asp:Panel>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
