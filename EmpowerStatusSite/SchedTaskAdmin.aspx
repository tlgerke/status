<%@ Page Title="" Language="C#" MasterPageFile="~/EmpowerStatusSite.Master" AutoEventWireup="true" Async="true" CodeBehind="SchedTaskAdmin.aspx.cs" Inherits="EmpowerStatusSite.SchedTaskAdmin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
 <script type="text/javascript">
 function updateHiddenField() {
 var selectedValue = document.getElementById('<%= DropDownList1.ClientID %>').value;
 document.getElementById('<%= hfSelectedValue.ClientID %>').value = selectedValue;
 }

 async function postAction(url, data) {
 try {
 const resp = await fetch(url, {
 method: 'POST',
 headers: { 'Content-Type': 'application/json' },
 body: JSON.stringify(data)
 });
 return resp;
 } catch (err) {
 console.error('Error posting action', err);
 throw err;
 }
 }

 document.addEventListener('click', function (e) {
 var target = e.target;
 if (target.classList && target.classList.contains('ajax-action')) {
 e.preventDefault();
 var server = target.getAttribute('data-server');
 var service = target.getAttribute('data-service');
 if (target.classList.contains('start-service') || target.classList.contains('start-task')) {
 postAction('/api/proxy/scheduledtask/run', { server: server, taskName: service })
 .then(r => { if (r.ok) { target.innerText = 'Started'; target.disabled = true; } else { alert('Failed to start'); } });
 } else if (target.classList.contains('stop-service') || target.classList.contains('disable-task')) {
 postAction('/api/proxy/scheduledtask/disable', { server: server, taskName: service })
 .then(r => { if (r.ok) { target.innerText = 'Disabled'; target.disabled = true; } else { alert('Failed to disable'); } });
 } else if (target.classList.contains('enable-service') || target.classList.contains('enable-task')) {
 postAction('/api/proxy/scheduledtask/enable', { server: server, taskName: service })
 .then(r => { if (r.ok) { target.innerText = 'Enabled'; target.disabled = true; } else { alert('Failed to enable'); } });
 }
 }
 });
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
