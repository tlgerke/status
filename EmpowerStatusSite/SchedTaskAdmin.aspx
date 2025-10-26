<%@ Page Title="" Language="C#" MasterPageFile="~/EmpowerStatusSite.Master" AutoEventWireup="true" Async="true" CodeBehind="SchedTaskAdmin.aspx.cs" Inherits="EmpowerStatusSite.SchedTaskAdmin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">
    function updateHiddenField() {
        var selectedValue = document.getElementById('<%= DropDownList1.ClientID %>').value;
        document.getElementById('<%= hfSelectedValue.ClientID %>').value = selectedValue;
    }
    </script>
    <asp:Label id="HiddenLabel" runat="server" Visible="false"/>
    <br />
    <br />
    <asp:Label runat="server">Choose An Environment</asp:Label>
    <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="true" OnSelectedIndexChanged ="DropDownList1_SelectedIndexChanged" onchange="updateHiddenField();" >
        <asp:ListItem Text="Dev" Value="Dev" />
        <asp:ListItem Text="Test" Value="Test" />
        <asp:ListItem Text="QA" Value="QA" />
        <asp:ListItem Text="Prod" Value ="Prod" />
        <asp:ListItem Text="Training" Value="Training" />
    </asp:DropDownList>
    
    <asp:HiddenField ID="hfSelectedValue" runat="server" />
    <br />

    <h2><asp:Label ID="EnvName" runat="server" /></h2>
    <asp:Table ID="ResultTable" runat="server" Font-Size="Small">
    
    <asp:TableHeaderRow>
        <asp:TableHeaderCell>Task Name</asp:TableHeaderCell>
        <asp:TableHeaderCell>State</asp:TableHeaderCell>
        <%--<asp:TableHeaderCell>Enabled</asp:TableHeaderCell>--%>
        <asp:TableHeaderCell>Next Run Time</asp:TableHeaderCell>
        <asp:TableHeaderCell>Last Run Time</asp:TableHeaderCell>
        <asp:TableHeaderCell width="300">Schedule</asp:TableHeaderCell>
    </asp:TableHeaderRow>
</asp:Table>
</asp:Content>
