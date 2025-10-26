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

namespace EmpowerStatusSite
{
    public partial class ServiceStatusAdmin : System.Web.UI.Page
    {
        public List<string[]> allValues = new List<string[]>();
        public int i = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            using (var context = new PrincipalContext(ContextType.Domain))
            {
                var usr = UserPrincipal.FindByIdentity(context, HttpContext.Current.User.Identity.Name);
                //if (usr.DisplayName.ToLower() == "tracy gerke" || usr.DisplayName.ToLower() == "john currie" || usr.DisplayName.ToLower() == "amy glenn" || usr.DisplayName.ToLower() == "rik danner" || usr.DisplayName.ToLower() == "casey blackman")
                if (User.Identity.Name == "CORP\\c1wc4" || User.Identity.Name == "CORP\\x5px9" || User.Identity.Name == "CORP\\y5bw3" || User.Identity.Name == "CORP\\y9yw8" || User.Identity.Name == "CORP\\r7vr4" || User.Identity.Name == "CORP\\v9kb5" || User.Identity.Name == "CORP\\y5cn2" || User.Identity.Name == "CORP\\a7aw0" || User.Identity.Name == "CORP\\g3cn0")
                    {

                        if (hfSelectedValue.Value != "")
                        {
                            DropDownList1.SelectedValue = hfSelectedValue.Value;
                        }

                        BindServerData(DropDownList1.SelectedValue);
                    }
                    else
                    {
                        HiddenLabel.Visible = true;
                        HiddenLabel.Text = "You don't have permission to view this page";
                        MasterTable.Visible = false;
                        selectLabel.Visible = false;
                        DropDownList1.Visible = false;
                        noteLbl.Visible = false;

                    }
            }
            
        }


        private void BindServerData(string env)
        {
            EnvName.Text = $"{env} Services";
            List<string> serverList = GetServerListFromDatabase(env);

            foreach (string server in serverList)
            {
                string result = MakeWebServiceCall($"http://{server}:8081/api/servicestatus/get?serviceName=Empower");
                ParseResults(result,server);
                result = MakeWebServiceCall($"http://{server}:8081/api/servicestatus/get?serviceName=Regions");
                ParseResults(result,server);
                result = MakeWebServiceCall($"http://{server}:8081/api/servicestatus/get?serviceName=EmailProcessorService");
                ParseResults(result,server);
            }
            AddResultToTable();
        }

        private List<string> GetServerListFromDatabase(string env)
        {
            List<string> serverList = new List<string>();

            string connectionString = "Data Source=wdmlesql01,31082;Initial Catalog=EMPOWER_DEV; User ID=EMPOWERDEV;Password=2=N%+Dn?8bWTHV~7q56jPkBJ";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = $"SELECT Server FROM EmpowerStatus WHERE (Type='app' or Type='api') AND env='{env}'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
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
                serverCell.Text = allValues[j][0];
                serverCell.BorderStyle = BorderStyle.Solid;
                row.Cells.Add(serverCell);

                TableCell serviceCell = new TableCell();
                serviceCell.Text = allValues[j][1];
                serviceCell.BorderStyle = BorderStyle.Solid;
                row.Cells.Add(serviceCell);

                TableCell statusCell = new TableCell();
                statusCell.Text = allValues[j][2];
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
                Button startButton = new Button();
                startButton.Click += new EventHandler(startButton_Click);
                startButton.Text = $"Start";
                startButton.ID = $"Start,{allValues[j][0]},{allValues[j][1]}";
                startButton.PostBackUrl = "ServiceStatusAdmin.aspx";
                


                startCell.Controls.Add(startButton);
                row.Cells.Add(startCell);

                TableCell stopCell = new TableCell();
                Button stopButton = new Button();
                stopButton.Click += new EventHandler(stopButton_Click);
                stopButton.Text = $"Stop";
                stopButton.ID = $"Stop,{allValues[j][0]},{allValues[j][1]}";
                stopButton.PostBackUrl = "ServiceStatusAdmin.aspx";
                stopCell.Controls.Add(stopButton);
                row.Cells.Add(stopCell);

                ResultTable.Rows.Add(row);
            }
        }

        void startButton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string[] elements = btn.ID.Split(',');
            string server = elements[1];
            string serviceName = elements[2];
            StartService(serviceName,server);
            
        }

        void stopButton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string[] elements = btn.ID.Split(',');
            string server = elements[1];
            string serviceName = elements[2];
            StopService(serviceName,server);
            
        }

        private Dictionary<string, string> ParseResult(string result)
        {
            result = result.Replace("/n", "");
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
        }

        private async void StopService(string serviceName,string server)
        {
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("serviceName",serviceName)
            });
            var response = await client.PostAsync($"http://{server}:8081/api/servicestatus/stop?serviceName={serviceName}", content);
            var responseString = await response.Content.ReadAsStringAsync();
            //Response.Redirect("ServiceStatusAdmin.aspx");
            

        }

        public async void StartService(string serviceName,string server)
        {
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("serviceName",serviceName)
            });
            var response = await client.PostAsync($"http://{server}:8081/api/servicestatus/start?serviceName={serviceName}", content);
            var responseString = await response.Content.ReadAsStringAsync();
            //Response.Redirect("ServiceStatusAdmin.aspx");
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
        protected void DropDownList1_OnUnload(object sender, EventArgs e)
        {
            
        }
    }
}