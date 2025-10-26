using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using EmpowerStatusSite.Helpers;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EmpowerStatusSite
{
    public partial class SchedTask : System.Web.UI.Page
    {
        //public SortedList<string, string> allValues = new SortedList<string, string>();
        public List<TasksProp> allValues = new List<TasksProp>();
        protected void Page_Load(object sender, EventArgs e)
        {
            var _ = BindServerDataAsync(DropDownList1.SelectedValue);
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
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceWarning($"Failed to fetch scheduled tasks from {server}: {ex.Message}");
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

        private string MakeWebServiceCall(string url)
        {
            // kept for compatibility but prefer async methods
            using (WebClient client = new WebClient())
            {
                return client.DownloadString(url);
            }
        }

        private void ParseResults(string result, string server)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            TasksProp [] taskprops = s.Deserialize<TasksProp[]>(result);

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
                    nameCell.Text = allValues[i].Name;
                    nameCell.BorderStyle = BorderStyle.Solid;
                    row.Cells.Add(nameCell);

                    TableCell stateCell = new TableCell();
                    stateCell.Text = allValues[i].State.ToString();
                    stateCell.BorderStyle = BorderStyle.Solid;
                    if (allValues[i].State.ToString() == "Ready" || allValues[i].State.ToString() == "Running")
                    {
                        stateCell.BackColor = System.Drawing.Color.Green;
                    }
                    row.Cells.Add(stateCell);
                    TableCell nextCell = new TableCell();
                    nextCell.Text = allValues[i].NextRunTime.ToString();
                    nextCell.BorderStyle = BorderStyle.Solid;
                    row.Cells.Add(nextCell);
                    TableCell lastCell = new TableCell();
                    lastCell.Text = allValues[i].LastRunTime.ToString();
                    lastCell.BorderStyle = BorderStyle.Solid;
                    row.Cells.Add(lastCell);
                    TableCell scheduleCell = new TableCell();
                    scheduleCell.Text = allValues[i].Schedule.ToString();
                    scheduleCell.BorderStyle = BorderStyle.Solid;
                    scheduleCell.CssClass = "wordwrap";
                    row.Cells.Add(scheduleCell);



                    ResultTable.Rows.Add(row);
                }
            }
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            hfSelectedValue.Value = DropDownList1.SelectedItem.Text;
        }
       
    }
}