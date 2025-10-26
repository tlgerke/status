using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.DirectoryServices.AccountManagement;
using System.Web.Security;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web.UI.WebControls;
using EmpowerStatusSite.Helpers;

namespace EmpowerStatusSite
{
    public partial class EmpowerStatusSite : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            serviceAdminItem.Visible = false;
            taskAdminItem.Visible = false;

            string name = "";
            try
            {
                using (var context = new PrincipalContext(ContextType.Domain))
                {
                    var usr = UserPrincipal.FindByIdentity(context, HttpContext.Current.User.Identity.Name);
                    if (usr != null)
                    {
                        name = usr.DisplayName;
                    }

                    UserID.Text = $"Hi, {name}";

                    if (AuthorizationHelper.IsUserAdmin(HttpContext.Current.User.Identity.Name))
                    {
                        serviceAdminItem.Visible = true;
                        taskAdminItem.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.Logging.LogWarning("Error in master page Page_Load: " + ex);
            }
        }
    }
}