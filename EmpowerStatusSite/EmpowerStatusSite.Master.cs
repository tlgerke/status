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

namespace EmpowerStatusSite
{
    public partial class EmpowerStatusSite : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            serviceAdminItem.Visible = false;
            taskAdminItem.Visible = false;
            
            string name = "";
            using (var context = new PrincipalContext(ContextType.Domain))
            {
                var usr = UserPrincipal.FindByIdentity(context, HttpContext.Current.User.Identity.Name);
                if (usr != null)
                {
                    name = usr.DisplayName;
                }

                UserID.Text = $"Hi, {name}";

                if (usr.DisplayName.ToLower() == "tracy gerke" || usr.DisplayName.ToLower() == "john currie" || usr.DisplayName.ToLower() =="amy glenn" || usr.DisplayName.ToLower() == "rik danner" || usr.DisplayName.ToLower() == "casey blackman" || usr.DisplayName.ToLower() == "kevin small" || usr.DisplayName.ToLower() == "trevor massey" || usr.DisplayName.ToLower() == "eric stabile" || usr.DisplayName.ToLower() == "kira pedroza")
                //if (HttpContext.Current.User.Identity.Name.ToLower() == "corp\\y5cn2" || HttpContext.Current.User.Identity.Name.ToLower() == "corp\\c1wc4" || HttpContext.Current.User.Identity.Name.ToLower() == "corp\\x5px9" || HttpContext.Current.User.Identity.Name.ToLower() == "corp\\y5bw3" || HttpContext.Current.User.Identity.Name.ToLower() == "corp\\y5cn2" || HttpContext.Current.User.Identity.Name.ToLower() == "corp\\y9yw8")
                {
                    serviceAdminItem.Visible = true;
                    taskAdminItem.Visible = true;

                }
            }
        }
    }
}