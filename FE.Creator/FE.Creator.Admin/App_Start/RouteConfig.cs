using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FE.Creator.Admin
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes for angular templates.
            routes.MapRoute(
                name: "AngularClientTemplates",
                url: "ngview/{module}/{name}",
               defaults: new { controller = "AngularView", action = "ClientTemplate" });


            routes.MapRoute(
                name: "AngularViewTemplates",
                url: "ngview/{action}/{module}/{name}/{Id}",
               defaults: new { controller = "AngularView", action = "EditOrDisplay", Id = UrlParameter.Optional });


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
