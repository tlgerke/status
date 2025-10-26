<%@ Page Title="" Language="C#" MasterPageFile="~/EmpowerStatusSite.Master" AutoEventWireup="true" CodeBehind="SchedTask.aspx.cs" Inherits="EmpowerStatusSite.SchedTask" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <h1>Empower Scheduled Task Info</h1>
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
    <asp:Table ID="ResultTable" runat="server" Font-Size="Small">
    
    <asp:TableHeaderRow>
        <asp:TableHeaderCell>Task Name</asp:TableHeaderCell>
        <asp:TableHeaderCell>State</asp:TableHeaderCell>
        <%--<asp:TableHeaderCell>Enabled</asp:TableHeaderCell>--%>
        <asp:TableHeaderCell>Next Run Time</asp:TableHeaderCell>
        <asp:TableHeaderCell>Last Run Time</asp:TableHeaderCell>
        <asp:TableHeaderCell Width="300">Schedule</asp:TableHeaderCell>
    </asp:TableHeaderRow>
</asp:Table>

<script>
document.addEventListener('click', function(e) {
 var t = e.target;
 if (t.classList && t.classList.contains('ajax-action')) {
 e.preventDefault();
 var srv = t.getAttribute('data-server');
 var name = t.getAttribute('data-service');
 if (t.classList.contains('start-task')) {
 fetch('/api/proxy/scheduledtask/run', { method: 'POST', headers: {'Content-Type':'application/json'}, body: JSON.stringify({ server: srv, taskName: name }) })
 .then(r => { if (r.ok) { t.innerText = 'Started'; t.disabled = true; } else alert('Failed'); });
 }
 if (t.classList.contains('disable-task')) {
 fetch('/api/proxy/scheduledtask/disable', { method: 'POST', headers: {'Content-Type':'application/json'}, body: JSON.stringify({ server: srv, taskName: name }) })
 .then(r => { if (r.ok) { t.innerText = 'Disabled'; t.disabled = true; } else alert('Failed'); });
 }
 if (t.classList.contains('enable-task')) {
 fetch('/api/proxy/scheduledtask/enable', { method: 'POST', headers: {'Content-Type':'application/json'}, body: JSON.stringify({ server: srv, taskName: name }) })
 .then(r => { if (r.ok) { t.innerText = 'Enabled'; t.disabled = true; } else alert('Failed'); });
 }
 }
});
</script>

</asp:Content>
