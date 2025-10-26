<%@ Page Title="" Language="C#" MasterPageFile="~/EmpowerStatusSite.Master" AutoEventWireup="true" CodeBehind="EventLogs.aspx.cs" Inherits="EmpowerStatusSite.EventLogs" EnableViewState="true"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <h1>Event Logs</h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
        <table>
            <tr>
                <td>Step 1. Choose An Environment&nbsp;&nbsp;

                
                    <asp:DropDownList ID="DropDownList1" runat="server" AutoPostback="true" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
                        <asp:ListItem Text="--Select One--" />
                        <asp:ListItem Text="Dev" Value="Dev" />
                        <asp:ListItem Text="Test" Value="Test" />
                        <asp:ListItem Text="QA" Value="QA" />
                        <asp:ListItem Text="Prod" Value ="Prod" />
                        <asp:ListItem Text="Training" Value="Training" />
                    </asp:DropDownList>
                    </td>
                </tr>
            <tr>
                <td>Step 2.  Select the server and Log Name</td>
            </tr>
                <tr>
                <td>
                    &nbsp;&nbsp;&nbsp;
                    <asp:Label runat="server">Server Name</asp:Label>
                    <asp:DropDownList ID="Serverddl" runat="server"></asp:DropDownList>
                    &nbsp;&nbsp;&nbsp;
                    <asp:Label runat="server">Log Type</asp:Label>
                    <asp:DropDownList ID="DropDownList2" runat="server">
                        <asp:ListItem Text="Application" Value="Application" />
                        <asp:ListItem Text="System" Value="System" />
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:Button ID="submit" runat="server" Text="Get Logs" OnClick="submit_Click" />
                </td>
            </tr>
        </table>
        
        <asp:HiddenField ID="hfSelectedValue" runat="server" />
        <br />
        <h2><asp:Label ID="EnvName" runat="server" /></h2>

        <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
</asp:Content>
