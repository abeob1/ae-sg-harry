<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RecieveIntoOutlet.aspx.cs"
    Inherits="AE_HarrysWeb_V001.Recieve_Into_Outlet" MasterPageFile="~/Main.Master" %>

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
                    <asp:Label ID="lblTitle" runat="server" Text="Receive into Outlet "></asp:Label>
                </h2>
                <div style="margin-left: 5px; width: 99%;">
                    <hr />
                    <table border="0" width="100%" style="background-color: #D1D4D8; font-weight: bold;">
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
                            <td align="right" rowspan="4">
                                <img src="Images/Logo_Small.png" alt="Harrys Logo" />
                            </td>
                            <td>
                            </td>
                        </tr>
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
                        </tr>
                        <tr>
                            <td>
                                Receipt Date
                            </td>
                            <td>
                                :
                            </td>
                            <td>
                                <asp:Label ID="lblReceiptDate" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <hr />
                </div>
                <div>
                    <table width="100%" style="font-weight: bold;">
                        <tr>
                            <td>
                                <asp:RadioButton ID="rbnCK" runat="server" Text="Central Kitchen" AutoPostBack="True"
                                    GroupName="Outlets" OnCheckedChanged="rbnOulets_CheckedChanged" Visible = "false"/>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:RadioButton ID="rbnCS" runat="server" Text="Choose Supplier" AutoPostBack="True"
                                    GroupName="Outlets" OnCheckedChanged="rbnOulets_CheckedChanged" />
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Vendor Search :
                                <asp:TextBox ID="txtSearch" runat="server" CssClass="txtbox" AutoPostBack="true"
                                    ToolTip="Search Items..." OnTextChanged="txtSearch_OnTextChanged"></asp:TextBox>
                                <asp:Button ID="btnSearch" runat="server" Text="::" Style="text-align: center;" Height="18px"
                                    Width="35px" OnClick="btnSearch_Click" />
                                &nbsp;
                                <asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                            </td>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                </div>
                <div style="width: 100%;">
                    <table border="0" width="50%" style="background-color: #D1D4D8; font-weight: bold;">
                        <asp:GridView ID="grvRIO" runat="server" CssClass="GridInner" Width="100%" BorderColor="White"
                            BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                            HeaderStyle-Height="25px" CellSpacing="2" HeaderStyle-VerticalAlign="Middle"
                            AllowPaging="true" PageSize="20" OnPageIndexChanging="grvRIO_PageIndexChanging">
                            <PagerSettings Mode="NumericFirstLast" />
                            <PagerStyle BackColor="#1B3B5F" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle"
                                CssClass="pager-row" />
                            <%-- <PagerStyle BackColor="#70A0D0" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle" />--%>
                            <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px"
                                CssClass="row" />
                            <FooterStyle BackColor="#7E7E7E" Font-Bold="True" ForeColor="White" />
                            <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                            <Columns>
                                <asp:TemplateField HeaderText="Vendor" ItemStyle-Width="150px">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle Width="150px" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hlnk" ControlStyle-ForeColor="Firebrick" Text='<%# Bind("CardName") %>'
                                            runat="server" NavigateUrl='<%# "ReceiveIntoOutletDetails.aspx?CardCode=" + Eval("CardCode") + "&CardName=" + Server.UrlEncode(Eval("CardName").ToString()) + "&NoOfOpenPO=" + Eval("NoOfOpenPO") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="NoOfOpenPO" HeaderText="Open POs" HeaderStyle-HorizontalAlign="Left"
                                    ItemStyle-Width="150px">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle Width="150px" />
                                </asp:BoundField>
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
                                            <span>Vendor</span>
                                        </th>
                                        <th>
                                            <span>Open POs</span>
                                        </th>
                                        <tr>
                                            <td colspan="12">
                                                <span>No Data</span>
                                            </td>
                                        </tr>
                                </table>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </table>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <hr />
</asp:Content>
