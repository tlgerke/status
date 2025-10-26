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

namespace EmpowerStatusSite
{
    public partial class SchedTask : System.Web.UI.Page
    {
        //public SortedList<string, string> allValues = new SortedList<string, string>();
        public TasksProp[] allVaues = new TasksProp[] {};
        protected void Page_Load(object sender, EventArgs e)
        {
            BindServerData(DropDownList1.SelectedValue);
        }

        private void BindServerData(string env)
        {
            EnvName.Text = $"{env} Scheduled Tasks";
            List<string> serverList = GetServerListFromDatabase(env);

            foreach (string server in serverList)
            {
                string result = MakeWebServiceCall($"http://{server}:8081/api/scheduledtasks/status");
                ParseResults(result);
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

        private void ParseResults(string result)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            TasksProp [] taskprops = s.Deserialize<TasksProp[]>(result);
            
            allVaues = allVaues.Concat(taskprops).ToArray();
               

            
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