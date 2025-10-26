using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.ServiceProcess;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using EmpowerStatusSite.Helpers;
using System.Threading.Tasks;

namespace EmpowerStatusSite
{
    public partial class ServiceStatusAdmin : System.Web.UI.Page
    {
        public List<string[]> allValues = new List<string[]>();
        public int i =0;
        protected async void Page_Load(object sender, EventArgs e)
        {
            // simple role/group check - adjust group name as appropriate
            try
            {
                if (!AuthorizationHelper.IsUserAdmin(HttpContext.Current.User.Identity.Name))
                {
                    HiddenLabel.Visible = true;
                    HiddenLabel.Text = "You don't have permission to view this page";
                    MasterTable.Visible = false;
                    selectLabel.Visible = false;
                    DropDownList1.Visible = false;
                    noteLbl.Visible = false;
                    Helpers.Logging.LogWarning($"Unauthorized access attempt by {HttpContext.Current.User.Identity.Name} to ServiceStatusAdmin.");
                    return;
                }

                if (hfSelectedValue.Value != "")
                {
                    DropDownList1.SelectedValue = hfSelectedValue.Value;
                }

                await BindServerDataAsync(DropDownList1.SelectedValue);
            }
            catch (Exception ex)
            {
                Helpers.Logging.LogError(ex, "Error in Page_Load ServiceStatusAdmin");
                HiddenLabel.Visible = true;
                HiddenLabel.Text = "An error occurred loading the page.";
            }
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
                    ParseResults(result,server);
                    result = await HttpClientProvider.Instance.GetStringAsync($"http://{server}:8081/api/servicestatus/get?serviceName=Regions");
                    ParseResults(result,server);
                    result = await HttpClientProvider.Instance.GetStringAsync($"http://{server}:8081/api/servicestatus/get?serviceName=EmailProcessorService");
                    ParseResults(result,server);
                }
                catch (HttpRequestException hre)
                {
                    Helpers.Logging.LogWarning($"Failed to fetch service status from {server}: {hre.Message}");
                }
                catch (TaskCanceledException tce)
                {
                    Helpers.Logging.LogWarning($"Timeout fetching service status from {server}: {tce.Message}");
                }
                catch (Exception ex)
                {
                    Helpers.Logging.LogError(ex, $"Unexpected error fetching service status from {server}");
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
                string query = "SELECT Server FROM EmpowerStatus WHERE (Type=@type1 or Type=@type2) AND env=@env";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@type1", "app");
                    cmd.Parameters.AddWithValue("@type2", "api");
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
            Dictionary<string, string> parsedValues = ParseResult(result);

            foreach (var kvp in parsedValues)
            {

                if (!kvp.Key.Contains("vsts") && !kvp.Key.Contains("Barcode") && !kvp.Key.Contains("Diagnostics") && !kvp.Key.Contains("Intraday") && !kvp.Key.Contains("Archiving") && !kvp.Key.Contains("Backfill") && !kvp.Key.Contains("Full Process") && !kvp.Key.Contains("Reload") && !kvp.Key.Contains("Fusion"))
                {
                    string[] newRow = new string[3];
                    newRow[0] = server;
                    newRow[1] = kvp.Key;
                    newRow[2] = kvp.Value;

                    allValues.Add(newRow);

                }
            }
        }

        private void AddResultToTable()
        {
            for (int j=0; j<allValues.Count; j++) 
            {
                TableRow row = new TableRow();
                TableCell serverCell = new TableCell();
                serverCell.Text = HttpUtility.HtmlEncode(allValues[j][0]);
                serverCell.BorderStyle = BorderStyle.Solid;
                row.Cells.Add(serverCell);

                TableCell serviceCell = new TableCell();
                serviceCell.Text = HttpUtility.HtmlEncode(allValues[j][1]);
                serviceCell.BorderStyle = BorderStyle.Solid;
                row.Cells.Add(serviceCell);

                TableCell statusCell = new TableCell();
                statusCell.Text = HttpUtility.HtmlEncode(allValues[j][2]);
                if (statusCell.Text == "Running")
                {
                    statusCell.BackColor = System.Drawing.Color.Green;
                    statusCell.BorderStyle = BorderStyle.Solid;
                }
                else
                {
                    statusCell.BackColor = System.Drawing.Color.Red;
                    statusCell.BorderStyle = BorderStyle.Solid;
                }
                row.Cells.Add(statusCell);

                TableCell startCell = new TableCell();
                // create a client-side button placeholder that the front-end JS will convert
                var startButton = new Button { Text = "Start" };
                startButton.Attributes["data-server"] = allValues[j][0];
                startButton.Attributes["data-service"] = allValues[j][1];
                startButton.CssClass = "ajax-action start-service";
                startCell.Controls.Add(startButton);
                row.Cells.Add(startCell);

                TableCell stopCell = new TableCell();
                var stopButton = new Button { Text = "Stop" };
                stopButton.Attributes["data-server"] = allValues[j][0];
                stopButton.Attributes["data-service"] = allValues[j][1];
                stopButton.CssClass = "ajax-action stop-service";
                stopCell.Controls.Add(stopButton);
                row.Cells.Add(stopCell);

                ResultTable.Rows.Add(row);
            }
        }


        private Dictionary<string, string> ParseResult(string result)
        {
            result = result.Replace("/n", "");
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}