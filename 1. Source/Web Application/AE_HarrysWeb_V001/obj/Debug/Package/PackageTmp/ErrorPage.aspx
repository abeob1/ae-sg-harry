<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true"
    CodeBehind="ErrorPage.aspx.cs" Inherits="AE_HarrysWeb_V001.ErrorPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="height: 400px; width: 100%;">
        <tr>
            <td align="center">
                <p style="text-align: center">
                    <b><font face="Trebuchet MS" color="#CC0000" size="3">Sorry, Something went wrong. Go
                        to <a href="Homepage.aspx" style ="color:Black">Home page </a>and try again.</font></b></p>
            </td>
        </tr>
    </table>
</asp:Content>
