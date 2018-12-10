using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace InterviewTest
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ValidatorPost",
                url: "Home/CustomValidator/{id}",
                defaults: new { controller = "Home", action = "CustomValidatorPost",}
            );

            routes.MapRoute(
                name: "Validator",
                url: "Home/CustomValidator",
                defaults: new { controller = "Home", action = "CustomValidator" }
            );

            //routes.MapRoute(
            //    name: "CustomValidator",
            //    url:  "Home/CustomValidatorPost",
            //    defaults: new {controller = "Home", action = "CustomValidator" }
            //);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            //routes.MapRoute(
            //    name: "Validator",
            //    url: "{controller}/{action}"
            //    );
            ///{standardId}
            //, standardId = UrlParameter.Optional 
        }
    }
}
