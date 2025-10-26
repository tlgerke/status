using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using EmpowerStatusSite.Helpers;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EmpowerStatusSite
{
    public partial class ServiceStatus : System.Web.UI.Page
    {
        public SortedList<string, string> allValues = new SortedList<string, string>();
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(hfSelectedValue.Value))
            {
                DropDownList1.ClearSelection();
                DropDownList1.Items.FindByText(hfSelectedValue.Value).Selected = true;
            }

            var _ = BindServerDataAsync(DropDownList1.SelectedValue);
            
        }


        private async Task BindServerDataAsync(string env)
        {
            EnvName.Text = $"{env} Services";
            List<string> serverList = GetServerListFromDatabase(env);

            foreach (string server in serverList)
            {
                try
                {
                    string result = await HttpClientProvider.Instance.GetStringAsync($"http://{server}:8081/api/servicestatus/get?serviceName=Empower");
                    ParseResults(result);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceWarning($"Failed to fetch service status from {server}: {ex.Message}");
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
                    cmd.Parameters.AddWithValue("@type", "app");
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
            using (WebClient client = new WebClient())
            {
                return client.DownloadString(url);
            }
        }

        private void ParseResults(string result)
        {
            Dictionary<string, string> parsedValues = ParseResult(result);
            foreach (var kvp in parsedValues)
            {
                try
                {
                    if (!kvp.Key.Contains("vsts") && !kvp.Key.Contains("Barcode") && !kvp.Key.Contains("Diagnostics") && !kvp.Key.Contains("Intraday") && !kvp.Key.Contains("Archiving") && !kvp.Key.Contains("Backfill") && !kvp.Key.Contains("Full Process") && !kvp.Key.Contains("Reload") && !kvp.Key.Contains("Fusion"))
                    {
                        allValues.Add(kvp.Key, kvp.Value);
                    }
                    
                }
                catch (System.ArgumentException)
                {
                    if (allValues[kvp.Key] == "Stopped")
                    {
                        if (kvp.Value == "Running")
                        {
                            allValues[kvp.Key] = kvp.Value;
                        }
                    }
                }
                
            }
        }

        private void AddResultToTable()
        {
            foreach (var kvp in allValues)
            {
                TableRow row = new TableRow();
                TableCell serviceCell = new TableCell();
                serviceCell.Text = kvp.Key;
                serviceCell.BorderStyle = BorderStyle.Solid;
                row.Cells.Add(serviceCell);

                TableCell statusCell = new TableCell();
                statusCell.Text = kvp.Value;
                if (statusCell.Text == "Running")
                {
                    statusCell.BackColor = System.Drawing.Color.Green;
                    statusCell.BorderStyle = BorderStyle.Solid;
                }
                else
                {
                    statusCell.BackColor= System.Drawing.Color.Red;
                    statusCell.BorderStyle = BorderStyle.Solid;
                }
                row.Cells.Add(statusCell);

                

                ResultTable.Rows.Add(row);
            }
        }

        private Dictionary<string,string> ParseResult(string result)
        {
            result = result.Replace("/n", "");
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            hfSelectedValue.Value = DropDownList1.SelectedItem.Text;
        }
    }
}