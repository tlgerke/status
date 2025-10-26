using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using EmpowerStatusSite.Helpers;
using System.Threading.Tasks;
using System.Configuration;
using System.Diagnostics;

namespace EmpowerStatusSite
{
    public partial class EventLogs : System.Web.UI.Page
    {
        //public EventProps[] allVaues = new EventProps[] { };
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        private async Task BindServerDataAsync(string server, string log, string env)
        {
            EnvName.Text = $"{env} Logs";
            
            string logType = DropDownList2.SelectedValue;

            try
            {
                string result = await HttpClientProvider.Instance.GetStringAsync($"http://{server}:8081/api/EventLog?logName={logType}&maxEvents=50");
                EventProps[] allValues = ParseResults(result);
                AddResultToTable(server, allValues);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error fetching event logs from {server}: {ex}");
            }
        }

        private List<string> GetServerListFromDatabase(string env)
        {
            List<string> serverList = new List<string>();

            string connectionString = ConfigurationManager.ConnectionStrings["EmpowerDb"]?.ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Server FROM EmpowerStatus WHERE env=@env";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
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

        private EventProps[] ParseResults(string result)
        {
            EventProps[] parsedValues = new EventProps[] {};
            JavaScriptSerializer s = new JavaScriptSerializer();
            EventProps[] eventprops = s.Deserialize<EventProps[]>(result);

            parsedValues = parsedValues.Concat(eventprops).ToArray();

            return parsedValues;
        }

        private void AddResultToTable(string server, EventProps[] allValues)
        {
            PlaceHolder1.Controls.Add(new LiteralControl("<br /><br />"));
            Label serverName = new Label();
            serverName.Text = server;
            serverName.Font.Size = 14;
            PlaceHolder1.Controls.Add(serverName);
            PlaceHolder1.Controls.Add(new LiteralControl("<br />"));
            Table tbl = new Table();
            tbl.ID = $"{server}Events";
            tbl.Font.Size = 9;
            PlaceHolder1.Controls.Add(tbl);
            TableHeaderRow header = new TableHeaderRow();
            TableHeaderCell dateHeader = new TableHeaderCell();
            dateHeader.Text = "Date";
            dateHeader.BorderStyle = BorderStyle.Solid;
            header.Cells.Add(dateHeader);
            TableHeaderCell sourceHeader = new TableHeaderCell();
            sourceHeader.Text = "Source";   
            sourceHeader.BorderStyle = BorderStyle.Solid;
            header.Cells.Add(sourceHeader);
            TableHeaderCell typeHeader = new TableHeaderCell();
            typeHeader.Text = "Type";
            typeHeader.BorderStyle = BorderStyle.Solid;
            header.Cells.Add(typeHeader);
            TableHeaderCell messageHeader = new TableHeaderCell();
            messageHeader.Text = "Message";
            messageHeader.BorderStyle = BorderStyle.Solid;
            header.Cells.Add(messageHeader);
            tbl.Rows.Add(header);
            for (int i = 0; i < allValues.Length; i++)
            {
                TableRow row = new TableRow();
                TableCell dateCell = new TableCell();
                dateCell.Text = allValues[i].TimeGenerated.ToString();
                dateCell.BorderStyle = BorderStyle.Solid;
                row.Cells.Add(dateCell);
                TableCell sourceCell = new TableCell();
                sourceCell.Text = allValues[i].Source.ToString();
                sourceCell.BorderStyle = BorderStyle.Solid;
                row.Cells.Add(sourceCell);
                TableCell typeCell = new TableCell();
                typeCell.Text = allValues[i].EntryType.ToString();
                typeCell.BorderStyle = BorderStyle.Solid;
                if (allValues[i].EntryType.ToString() == "Warning")
                {
                    typeCell.BackColor = System.Drawing.Color.Yellow;
                }
                else if (allValues[i].EntryType.ToString() == "Error")
                {
                    typeCell.BackColor = System.Drawing.Color.Red;
                }
                row.Cells.Add(typeCell);
                TableCell messageCell = new TableCell();
                messageCell.Text = allValues[i].Message.ToString();
                messageCell.BorderStyle = BorderStyle.Solid;
                row.Cells.Add(messageCell);
                tbl.Rows.Add(row);
            }
        }
        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<string> serverList = GetServerListFromDatabase(DropDownList1.SelectedValue);
            foreach(string server in serverList)
            {
                Serverddl.Items.Add(server);
            }
            Serverddl.SelectedIndex = -1;
        }

        protected void submit_Click(object sender, EventArgs e)
        {
            var _ = BindServerDataAsync(Serverddl.SelectedValue, DropDownList2.SelectedValue, DropDownList1.SelectedValue);
        }
    }
}