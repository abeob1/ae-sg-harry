<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Homepage_old.aspx.cs" Inherits="AE_HarrysWeb_V001.Homepage"
    MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%--<script type="text/javascript">
        var key = "";
        var isNS = (navigator.appName == "Netscape") ? 1 : 0;
        if (navigator.appName == "Netscape") document.captureEvents(Event.MOUSEDOWN || Event.MOUSEUP);
        function mischandler() {
            return false;
        }

        function keyhandler(en) {
            key = en.keyCode;
        }

        function mousehandler(e) {
            var myevent = (isNS) ? e : event;
            var eventbutton = (isNS) ? myevent.which : myevent.button;
            if ((eventbutton == 3)) { alert("Right-click Not Allowed"); return false; }
            else if ((eventbutton == 2)) { alert("Mouse center-click Not Allowed"); return false; }
            else if ((key == 17) && (eventbutton == 1)) { alert("Control + Left-click Not Allowed"); key = 0; return false; }
        }
        document.oncontextmenu = mischandler;
        document.onmousedown = mousehandler;
        document.onmouseup = mousehandler;
        document.onkeydown = keyhandler;

    </script>--%>
    <div style="font-family: Trebuchet MS; margin-top: 50px;">
        <table border="0" cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td align="center" valign="top">
                    <div style="margin-top: 40px;">
                        <asp:Image ID="ImgLogo" ImageUrl="Images/Logo_Small.png" runat="server" AlternateText="SAP Logo"
                            Width="100px" Height="72px" />
                    </div>
                    <table border="1" cellpadding="0" cellspacing="0" style="width: 100%; margin-top: 30px;
                        border-color: #DDDDDD">
                        <tr>
                            <td style="height: 25px; border: 0px solid #4297d7; background-color: #1B3B61; color: #fff;
                                font-weight: bold;" align="left">
                                <strong>
                                    <asp:Label ID="lblCompany" runat="server" Font-Size="9pt"></asp:Label></strong>
                            </td>
                            <td style="height: 25px; border: 0px solid #4297d7; background-color: #1B3B61; color: #fff;
                                font-weight: bold;" align="right">
                                <strong>
                                    <asp:Label ID="lblDate" runat="server" Font-Size="9pt"></asp:Label></strong>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <table style="width: 100%;">
            <tr>
                <td>
                    <table style="width: 300px;" align="left" border="0">
                        <div id="cssmenu">
                            <ul>
                                <li><a href="#" id="M1">Menus</a> </li>
                                <li id="LeftMenuBarItems" runat="server"><a href="#"><span>Web Order</span></a>
                                    <ul id="LeftMenuBarItemsul">
                                        <li><a id="SubMenu1" runat="server" href="MaterialRequestBySupplier.aspx">Material Request
                                            by Supplier </a></li>
                                        <li><a id="SubMenu2" runat="server" href="MaterialRequestByItem.aspx">Material Request
                                            by Item List </a></li>
                                        <li><a id="SubMenu3" runat="server" href="ListOfMaterialRequestDraft.aspx">List of Material
                                            Request (Draft) </a></li>
                                        <li><a id="SubMenu5" runat="server" href="MaterialRequestApprovalNew.aspx">Material
                                            Request Pending Approval </a></li>
                                        <li><a id="SubMenu4" runat="server" href="ListOfMaterialRequest.aspx">List of Material
                                            Request Approved </a></li>
                                        <li><a id="SubMenu6" runat="server" href="RecieveIntoOutlet.aspx">Receive </a></li>
                                        <li><a id="SubMenu7" runat="server" href="StocktakeCountingSheet.aspx">Stocktake Counting
                                            Sheet </a></li>
                                        <li><a id="SubMenu8" runat="server" href="StocktakeApproval.aspx">Stocktake Listing
                                            Pending Approval</a></li>
                                    </ul>
                                </li>
                            </ul>
                        </div>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <p>
                    </p>
                    <table style="width: 100%;">
                        <tr style="height: 40px; border: 0px solid #4297d7; background-color: #D2D2D2; color: #000;
                            font-weight: bold;" align="center">
                            <td>
                                <asp:UpdatePanel ID="updatePanel" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Button ID="btnLogOut" runat="server" Text="Log Out" OnClick="btnLogOut_Click"
                                            Style="background-image: Url('Images/bgButton.png'); background-repeat: no-repeat;"
                                            BorderStyle="Solid" Width="100px" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
