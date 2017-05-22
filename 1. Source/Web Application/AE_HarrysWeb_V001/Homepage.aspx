<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Homepage.aspx.cs" Inherits="AE_HarrysWeb_V001.Homepage"
    MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">
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

    </script>
    <div style="font-family: Trebuchet MS; margin-top: 50px;">
        <table border="0" cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td align="center" valign="top">
                    <div style="margin-top: 40px;">
                        <table>
                            <tr>
                                <td align="left">
                                    <asp:Image ID="ImgLogo" ImageUrl="Images/Logo_Small.png" runat="server" AlternateText="SAP Logo"
                                        Width="140px" Height="72px" />
                                </td>
                            </tr>
                        </table>
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
                    <table style="width: 300px;" align="left" border="0" class="sukiMenu">
                        <tr style="height: 25px; border: 0px solid #4297d7; background-color: #1B3B61; font-weight: bold;
                            color: White;">
                            <td align="left">
                                <a href="MaterialRequestBySupplier.aspx">Material Request by Supplier</a>
                            </td>
                        </tr>
                        <tr style="height: 25px; border: 0px solid #4297d7; background-color: #1B3B61; font-weight: bold;
                            color: White;">
                            <td align="left">
                                <a href="MaterialRequestByItem.aspx">Material Request by Item</a>
                            </td>
                        </tr>
                        <tr style="height: 25px; border: 0px solid #4297d7; background-color: #1B3B61; font-weight: bold;
                            color: White;">
                            <td align="left">
                                <a href="ListOfMaterialRequestDraft.aspx">List of Material Request Draft</a>
                            </td>
                        </tr>
                        <tr id="Submenu4" runat="server" style="height: 25px; border: 0px solid #4297d7;
                            background-color: #1B3B61; font-weight: bold; color: White;">
                            <td align="left">
                                <a href="OutletListPendingApproval.aspx">Material Request Pending Approval</a>
                            </td>
                        </tr>
                        <tr style="height: 25px; border: 0px solid #4297d7; background-color: #1B3B61; font-weight: bold;
                            color: White;">
                            <td align="left">
                                <a href="ListOfMaterialRequest.aspx">List of Material Request Approved</a>
                            </td>
                        </tr>
                        <tr style="height: 25px; border: 0px solid #4297d7; background-color: #1B3B61; font-weight: bold;
                            color: White;">
                            <td align="left">
                                <a href="RecieveIntoOutlet.aspx">Receive</a>
                            </td>
                        </tr>
                        <tr style="height: 25px; border: 0px solid #4297d7; background-color: #1b3b61; font-weight: bold;
                            color: white;">
                            <td align="left">
                                <a href="StocktakeCountingSheet.aspx">Stocktake Counting Sheet</a>
                            </td>
                        </tr>
                        <tr id="Submenu8" runat="server" style="height: 25px; border: 0px solid #4297d7;
                            background-color: #1B3B61; font-weight: bold; color: White;">
                            <td align="left">
                                <a href="StocktakeApproval.aspx">Stocktake Listing Pending Approval</a>
                            </td>
                        </tr>
                        <tr id="Submenu9" runat="server" style="height: 25px; border: 0px solid #4297d7;
                            background-color: #1B3B61; font-weight: bold; color: White;">
                            <td align="left">
                                <a href="ApprovalStatusSummary.aspx">Approval Status Summary</a>
                            </td>
                        </tr>
                        <tr id="Submenu10" runat="server" style="height: 25px; border: 0px solid #4297d7;
                            background-color: #1B3B61; font-weight: bold; color: White;">
                            <td align="left">
                                <a href="InventoryTransferRequest.aspx">Inventory Transfer Request</a>
                            </td>
                        </tr>
                        <tr id="Tr1" runat="server" style="height: 25px; border: 0px solid #4297d7;
                            background-color: #1B3B61; font-weight: bold; color: White;">
                            <td align="left">
                                <a href="ListOfInventoryTransfer.aspx">List of Transfer Report</a>
                            </td>
                        </tr>
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
