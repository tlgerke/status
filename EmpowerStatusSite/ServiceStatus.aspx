<%@ Page Title="" Language="C#" MasterPageFile="~/EmpowerStatusSite.Master" AutoEventWireup="true" CodeBehind="ServiceStatus.aspx.cs" Inherits="EmpowerStatusSite.ServiceStatus" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <H1>Empower Service Status</H1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <br />
    <asp:Label runat="server">Choose An Environment</asp:Label>
    <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="true" OnSelectedIndexChanged ="DropDownList1_SelectedIndexChanged" >
        <asp:ListItem Text="Dev" Value="Dev" />
        <asp:ListItem Text="Test" Value="Test" />
        <asp:ListItem Text="QA" Value="QA" />
        <asp:ListItem Text="Prod" Value ="Prod" />
        <asp:ListItem Text="Training" Value="Training" />
    </asp:DropDownList>
    <asp:HiddenField ID="hfSelectedValue" runat="server" />
    <br />
    <h2><asp:Label ID="EnvName" runat="server" /></h2>
    <asp:Table ID="ResultTable" runat="server">
        
        <asp:TableHeaderRow>
            <asp:TableHeaderCell>Service Name</asp:TableHeaderCell>
            <asp:TableHeaderCell>Status</asp:TableHeaderCell>
        </asp:TableHeaderRow>
    </asp:Table>
</asp:Content>
