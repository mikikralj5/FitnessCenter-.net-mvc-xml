using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebApplication1.HelperClasses;
using WebApplication1.Models;

namespace WebApplication1
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            List<User> users = ReadXML.UsersRead();
            HttpContext.Current.Application["users"] = users;
            //HttpContext.Current.Application["users"] = new List<User>();
            //var r = GroupTrainingData.GetAllFutureTraining(new List<User>());
        }
    }
}
