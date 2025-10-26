using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EmpowerStatusSite
{
    public partial class SchedTaskAdmin : System.Web.UI.Page
    {
        public TasksProp[] allVaues = new TasksProp[] { };
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.Name == "CORP\\c1wc4" || User.Identity.Name == "CORP\\y5cn2" || User.Identity.Name == "CORP\\x5px9" || User.Identity.Name == "CORP\\y5bw3" || User.Identity.Name == "CORP\\y9yw8" || User.Identity.Name == "CORP\\R7VR4" || User.Identity.Name == "CORP\\v9kb5" || User.Identity.Name == "CORP\\a7aw0" || User.Identity.Name == "CORP\\g3cn0")
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
            }
        }
        private void BindServerData(string env)
        {
            EnvName.Text = $"{env} Scheduled Tasks";
            List<string> serverList = GetServerListFromDatabase(env);
            
            foreach (string server in serverList)
            {
                string result = MakeWebServiceCall($"http://{server}:8081/api/scheduledtasks/status");
                ParseResults(result, server);
                
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
                string query = $"SELECT Server FROM EmpowerStatus WHERE Type='scheduler' AND env='{env}'";

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
            JavaScriptSerializer s = new JavaScriptSerializer();
            TasksProp[] taskprops = s.Deserialize<TasksProp[]>(result);

            allVaues = allVaues.Concat(taskprops).ToArray();
            
            foreach(TasksProp allValue in  allVaues)
            {
                if (allValue.Server is null)
                    { allValue.Server = server; }

            }



        }

        private void AddResultToTable()
        {
            for (int i = 0; i < allVaues.Length; i++)
            {
                if (!allVaues[i].Name.Contains("npcap") && !allVaues[i].Name.Contains("SensorFramework") && !allVaues[i].Name.Contains("User_Feed") && !allVaues[i].Name.StartsWith("IIS") && !allVaues[i].Name.Contains("AutoRepair") && !allVaues[i].Name.Equals("Register DNS") && !allVaues[i].Name.Equals("Move SNOW Certificate"))
                {
                    TableRow row = new TableRow();
                    TableCell nameCell = new TableCell();
                    nameCell.Text = allVaues[i].Name;
                    nameCell.BorderStyle = BorderStyle.Solid;
                    row.Cells.Add(nameCell);

                    TableCell stateCell = new TableCell();
                    stateCell.Text = allVaues[i].State.ToString();
                    stateCell.BorderStyle = BorderStyle.Solid;
                    if (allVaues[i].State.ToString() == "Ready" || allVaues[i].State.ToString() == "Running")
                    {
                        stateCell.BackColor = System.Drawing.Color.Green;
                    }
                    row.Cells.Add(stateCell);
                    //TableCell enabledCell = new TableCell();
                    //enabledCell.Text = allVaues[i].Enabled.ToString();
                    //enabledCell.BorderStyle = BorderStyle.Solid;
                    //row.Cells.Add(enabledCell);
                    TableCell nextCell = new TableCell();
                    nextCell.Text = allVaues[i].NextRunTime.ToString();
                    nextCell.BorderStyle = BorderStyle.Solid;
                    row.Cells.Add(nextCell);
                    TableCell lastCell = new TableCell();
                    lastCell.Text = allVaues[i].LastRunTime.ToString();
                    lastCell.BorderStyle = BorderStyle.Solid;
                    row.Cells.Add(lastCell);
                    TableCell scheduleCell = new TableCell();
                    scheduleCell.Text = allVaues[i].Schedule.ToString();
                    scheduleCell.BorderStyle = BorderStyle.Solid;
                    scheduleCell.CssClass = "wordwrap";
                    row.Cells.Add(scheduleCell);
                    TableCell enableCell = new TableCell();
                    Button enableButton = new Button();
                    enableButton.Click += new EventHandler(enableButton_Click);
                    enableButton.Text = $"Enable";
                    string currentServer = allVaues[i].Server;
                    string currentName = allVaues[i].Name;
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
            enableTask(taskName, server);
        }
        void disableButton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string[] elements = btn.ID.Split(',');
            string server = elements[1];
            string taskName = elements[2];
            disableTask(taskName, server);
        }
        void startButton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string[] elements = btn.ID.Split(',');
            string server = elements[1];
            string taskName = elements[2];
            startTask(taskName, server);
        }
        public async void enableTask(string taskName, string server)
        {
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("serviceName",taskName)
            });
            var response = await client.PostAsync($"http://{server}:8081/api/scheduledtask/{taskName}/enable", content);
            var responseString = await response.Content.ReadAsStringAsync();
            //Response.Redirect("SchedTaskAdmin.aspx");
        }
        public async void disableTask(string taskName, string server)
        {
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("serviceName",taskName)
            });
            var response = await client.PostAsync($"http://{server}:8081/api/scheduledtask/{taskName}/disable", content);
            var responseString = await response.Content.ReadAsStringAsync();
            //Response.Redirect("SchedTaskAdmin.aspx");
        }
        public async void startTask(string taskName, string server)
        {
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("serviceName",taskName)
            });
            var response = await client.PostAsync($"http://{server}:8081/api/scheduledtask/{taskName}/run", content);
            var responseString = await response.Content.ReadAsStringAsync();
            //Response.Redirect("SchedTaskAdmin.aspx");
        }
    }
}