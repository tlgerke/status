using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Net;
using System.Web.Services;
using Newtonsoft.Json;
using System.Web.Http.Results;

namespace EmpowerStatusSite
{
    public partial class Default : System.Web.UI.Page
    {
        Dictionary<string, string> allValues;
        protected void Page_Load(object sender, EventArgs e)
        {
            

        }
        private void BindServerData()
        {
            string[,] serverList = GetServerListFromDatabase();
        }

        private string[,] GetServerListFromDatabase()
        {
            string[,] serverList = new string[,] {};

            string connectionString = "Data Source=wdmlesql01,31082;Initial Catalog=EMPOWER_DEV;User ID=EMPOWERDEV;Password=2=N%+Dn?8bWTHV~7q56jPkBJ";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT Server,Primary FROM EmpowerStatus";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        int i= 0;
                        while (reader.Read())
                        {
                            serverList[i, 0] = reader[0].ToString();
                            serverList[i,1] = reader[1].ToString();
                            i += 1;
                        }
                    }
                }
            }
            return serverList;
        }
    }
}