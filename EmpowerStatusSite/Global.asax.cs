using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using EmpowerStatusSite.App_Start;

namespace EmpowerStatusSite
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            SerilogConfig.Configure();
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
