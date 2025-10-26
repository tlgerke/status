using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using EmpowerStatusSite.Helpers;
using System.Threading.Tasks;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;

namespace EmpowerStatusSite
{
 public partial class SchedTaskAdmin : System.Web.UI.Page
 {
 public List<TasksProp> allValues = new List<TasksProp>();
 protected void Page_Load(object sender, EventArgs e)
 {
 try
 {
 using (var context = new PrincipalContext(ContextType.Domain))
 {
 var usr = UserPrincipal.FindByIdentity(context, HttpContext.Current.User.Identity.Name);
 var isAdmin = false;
 if (usr != null)
 {
 isAdmin = usr.IsMemberOf(context, IdentityType.Name, "EmpowerAdmins");
 }

 if (!isAdmin && !(User.Identity.Name == "CORP\\c1wc4" || User.Identity.Name == "CORP\\y5cn2" || User.Identity.Name == "CORP\\x5px9" || User.Identity.Name == "CORP\\y5bw3" || User.Identity.Name == "CORP\\y9yw8" || User.Identity.Name == "CORP\\R7VR4" || User.Identity.Name == "CORP\\v9kb5" || User.Identity.Name == "CORP\\a7aw0" || User.Identity.Name == "CORP\\g3cn0"))
 {
 HiddenLabel.Visible = true;
 HiddenLabel.Text = "You don't have permission to view this page";
 System.Diagnostics.Trace.TraceWarning($"Unauthorized access attempt by {User.Identity.Name} to SchedTaskAdmin.");
 return;
 }
 }

 if (!string.IsNullOrEmpty(hfSelectedValue.Value))
 {
 DropDownList1.SelectedValue = hfSelectedValue.Value;
 }

 var _ = BindServerDataAsync(DropDownList1.SelectedValue);
 }
 catch (Exception ex)
 {
 System.Diagnostics.Trace.TraceError("Error in Page_Load SchedTaskAdmin: " + ex);
 HiddenLabel.Visible = true;
 HiddenLabel.Text = "An error occurred loading the page.";
 }
 }

 private async Task BindServerDataAsync(string env)
 {
 EnvName.Text = $"{env} Scheduled Tasks";
 List<string> serverList = GetServerListFromDatabase(env);

 foreach (string server in serverList)
 {
 try
 {
 string result = await HttpClientProvider.Instance.GetStringAsync($"http://{server}:8081/api/scheduledtasks/status");
 ParseResults(result, server);
 }
 catch (HttpRequestException hre)
 {
 System.Diagnostics.Trace.TraceWarning($"Failed to fetch scheduled tasks from {server}: {hre.Message}");
 }
 catch (TaskCanceledException tce)
 {
 System.Diagnostics.Trace.TraceWarning($"Timeout fetching scheduled tasks from {server}: {tce.Message}");
 }
 catch (Exception ex)
 {
 System.Diagnostics.Trace.TraceError($"Unexpected error fetching scheduled tasks from {server}: {ex}");
 }
 }
 AddResultToTable();
 }

 private List<string> GetServerListFromDatabase(string env)
 {
 List<string> serverList = new List<string>();

 string connectionString = ConfigurationManager.ConnectionStrings["EmpowerDb"]?.ConnectionString;

 using (SqlConnection conn = new SqlConnection(connectionString))
 {
 conn.Open();
 string query = "SELECT Server FROM EmpowerStatus WHERE Type=@type AND env=@env";

 using (SqlCommand cmd = new SqlCommand(query, conn))
 {
 cmd.Parameters.AddWithValue("@type", "scheduler");
 cmd.Parameters.AddWithValue("@env", env);
 using (SqlDataReader reader = cmd.ExecuteReader())
 {
 while (reader.Read())
 {
 serverList.Add(reader["Server"].ToString());
 }
 }
 }
 }
 return serverList;
 }

 private void ParseResults(string result, string server)
 {
 JavaScriptSerializer s = new JavaScriptSerializer();
 TasksProp[] taskprops = s.Deserialize<TasksProp[]>(result);

 foreach (var tp in taskprops)
 {
 tp.Server = server;
 allValues.Add(tp);
 }
 }

 private void AddResultToTable()
 {
 for (int i =0; i < allValues.Count; i++)
 {
 if (!allValues[i].Name.Contains("npcap") && !allValues[i].Name.Contains("SensorFramework") && !allValues[i].Name.Contains("User_Feed") && !allValues[i].Name.StartsWith("IIS") && !allValues[i].Name.Contains("AutoRepair") && !allValues[i].Name.Equals("Register DNS") && !allValues[i].Name.Equals("Move SNOW Certificate"))
 {
 TableRow row = new TableRow();
 TableCell nameCell = new TableCell();
 nameCell.Text = HttpUtility.HtmlEncode(allValues[i].Name);
 nameCell.BorderStyle = BorderStyle.Solid;
 row.Cells.Add(nameCell);

 TableCell stateCell = new TableCell();
 stateCell.Text = HttpUtility.HtmlEncode(allValues[i].State.ToString());
 stateCell.BorderStyle = BorderStyle.Solid;
 if (allValues[i].State.ToString() == "Ready" || allValues[i].State.ToString() == "Running")
 {
 stateCell.BackColor = System.Drawing.Color.Green;
 }
 row.Cells.Add(stateCell);
 //TableCell enabledCell = new TableCell();
 //enabledCell.Text = allValues[i].Enabled.ToString();
 //enabledCell.BorderStyle = BorderStyle.Solid;
 //row.Cells.Add(enabledCell);
 TableCell nextCell = new TableCell();
 nextCell.Text = HttpUtility.HtmlEncode(allValues[i].NextRunTime.ToString());
 nextCell.BorderStyle = BorderStyle.Solid;
 row.Cells.Add(nextCell);
 TableCell lastCell = new TableCell();
 lastCell.Text = HttpUtility.HtmlEncode(allValues[i].LastRunTime.ToString());
 lastCell.BorderStyle = BorderStyle.Solid;
 row.Cells.Add(lastCell);
 TableCell scheduleCell = new TableCell();
 scheduleCell.Text = HttpUtility.HtmlEncode(allValues[i].Schedule.ToString());
 scheduleCell.BorderStyle = BorderStyle.Solid;
 scheduleCell.CssClass = "wordwrap";
 row.Cells.Add(scheduleCell);
 TableCell enableCell = new TableCell();
 Button enableButton = new Button();
 enableButton.Click += new EventHandler(enableButton_Click);
 enableButton.Text = $"Enable";
 string currentServer = allValues[i].Server;
 string currentName = allValues[i].Name;
 enableButton.ID = $"Enable,{currentServer},{currentName}";
 enableButton.PostBackUrl = "SchedTaskAdmin.aspx";
 enableCell.Controls.Add(enableButton);

 row.Cells.Add(enableCell);
 TableCell disableCell = new TableCell();
 Button disableButton = new Button();
 disableButton.Click += new EventHandler(disableButton_Click);
 disableButton.Text = $"Disable";
 disableButton.ID = $"Disable,{currentServer},{currentName}";
 disableButton.PostBackUrl = "SchedTaskAdmin.aspx";
 disableCell.Controls.Add(disableButton);

 row.Cells.Add(disableCell);
 TableCell runCell = new TableCell();
 Button startButton = new Button();
 startButton.Click += new EventHandler(startButton_Click);
 startButton.Text = $"Start";
 startButton.ID = $"Start,{currentServer},{currentName}";
 startButton.PostBackUrl = "SchedTaskAdmin.aspx";
 runCell.Controls.Add(startButton);

 row.Cells.Add(runCell);

 ResultTable.Rows.Add(row);
 }
 }
 }
 protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
 {
 }
 void enableButton_Click(object sender, EventArgs e)
 {
 Button btn = (Button)sender;
 string[] elements = btn.ID.Split(',');
 string server = elements[1];
 string taskName = elements[2];
 var _ = enableTask(taskName, server);
 }
 void disableButton_Click(object sender, EventArgs e)
 {
 Button btn = (Button)sender;
 string[] elements = btn.ID.Split(',');
 string server = elements[1];
 string taskName = elements[2];
 var _ = disableTask(taskName, server);
 }
 void startButton_Click(object sender, EventArgs e)
 {
 Button btn = (Button)sender;
 string[] elements = btn.ID.Split(',');
 string server = elements[1];
 string taskName = elements[2];
 var _ = startTask(taskName, server);
 }
 public async Task enableTask(string taskName, string server)
 {
 try
 {
 var content = new FormUrlEncodedContent(new[]
 {
 new KeyValuePair<string, string>("serviceName",taskName)
 });
 var response = await HttpClientProvider.Instance.PostAsync($"http://{server}:8081/api/scheduledtask/{taskName}/enable", content);
 response.EnsureSuccessStatusCode();
 System.Diagnostics.Trace.TraceInformation($"Enabled scheduled task {taskName} on {server}");
 }
 catch (Exception ex)
 {
 System.Diagnostics.Trace.TraceError($"Error enabling scheduled task {taskName} on {server}: {ex}");
 }
 //Response.Redirect("SchedTaskAdmin.aspx");
 }
 public async Task disableTask(string taskName, string server)
 {
 try
 {
 var content = new FormUrlEncodedContent(new[]
 {
 new KeyValuePair<string, string>("serviceName",taskName)
 });
 var response = await HttpClientProvider.Instance.PostAsync($"http://{server}:8081/api/scheduledtask/{taskName}/disable", content);
 response.EnsureSuccessStatusCode();
 System.Diagnostics.Trace.TraceInformation($"Disabled scheduled task {taskName} on {server}");
 }
 catch (Exception ex)
 {
 System.Diagnostics.Trace.TraceError($"Error disabling scheduled task {taskName} on {server}: {ex}");
 }
 //Response.Redirect("SchedTaskAdmin.aspx");
 }
 public async Task startTask(string taskName, string server)
 {
 try
 {
 var content = new FormUrlEncodedContent(new[]
 {
 new KeyValuePair<string, string>("serviceName",taskName)
 });
 var response = await HttpClientProvider.Instance.PostAsync($"http://{server}:8081/api/scheduledtask/{taskName}/run", content);
 response.EnsureSuccessStatusCode();
 System.Diagnostics.Trace.TraceInformation($"Started scheduled task {taskName} on {server}");
 }
 catch (Exception ex)
 {
 System.Diagnostics.Trace.TraceError($"Error starting scheduled task {taskName} on {server}: {ex}");
 }
 //Response.Redirect("SchedTaskAdmin.aspx");
 }
 }
}