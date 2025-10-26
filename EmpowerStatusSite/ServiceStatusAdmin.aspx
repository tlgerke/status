<%@ Page Async="true" Title="" Language="C#" EnableViewState="true" MasterPageFile="~/EmpowerStatusSite.Master" AutoEventWireup="true" CodeBehind="ServiceStatusAdmin.aspx.cs" Inherits="EmpowerStatusSite.ServiceStatusAdmin" EnableEventValidation="false"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <h1>Service Status Admin</h1>
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
    
            <asp:Label ID="selectLabel" runat="server">Choose An Environment</asp:Label>
            <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged" onchange="updateHiddenField();">
                <asp:ListItem Text="Dev" Value="Dev" />
                <asp:ListItem Text="Test" Value="Test" />
                <asp:ListItem Text="QA" Value="QA" />
                <asp:ListItem Text="Prod" Value ="Prod" />
                <asp:ListItem Text="Training" Value="Training" />
            </asp:DropDownList>
        
    <asp:HiddenField ID="hfSelectedValue" runat="server" />
    <br />

    <h2><asp:Label ID="EnvName" runat="server" /></h2>
    <div><asp:Label ID="noteLbl" runat="server">See notes to the right for list of where each service should be running</asp:Label></div>
    <asp:Table ID="MasterTable" runat="server">
        <asp:TableRow>
            <asp:TableCell>
                <asp:Table ID="ResultTable" runat="server">
                    <asp:TableHeaderRow>
                        <asp:TableHeaderCell>Service</asp:TableHeaderCell>
                        <asp:TableHeaderCell>Status</asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                </asp:Table>
            </asp:TableCell>
            <asp:TableCell>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</asp:TableCell>
            <asp:TableCell>
               <asp:Table ID="ServiceNotes" runat="server" BorderStyle="Solid">
                <asp:TableRow>
                    <asp:TableCell>Empower Document Server</asp:TableCell>
                    <asp:TableCell>Runs on Every Server</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>Empower Document Spooler</asp:TableCell>
                    <asp:TableCell>Runs On Every Server</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>Empower Lights Out Processor</asp:TableCell>
                    <asp:TableCell>Runs On Every Server</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>Empower ODS Change Log Monitor</asp:TableCell>
                    <asp:TableCell>Run On Every Server</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>Empower ODS Failover Process</asp:TableCell>
                    <asp:TableCell>Runs on Primary Server Only</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>Empower ODS Incremental Process</asp:TableCell>
                    <asp:TableCell>Runs On Primary Server Only</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>Empower Orchestration Server</asp:TableCell>
                    <asp:TableCell>Runs On Every Server</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>Empower Queueing Server</asp:TableCell>
                    <asp:TableCell>Runs On Every Server</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>Empower Scheduler</asp:TableCell>
                    <asp:TableCell>Runs On Primary Server Only</asp:TableCell>
                </asp:TableRow>
               </asp:Table>
                </asp:TableCell></asp:TableRow></asp:Table>
            
</asp:Content>

