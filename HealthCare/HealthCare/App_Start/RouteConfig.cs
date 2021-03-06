﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HealthCare
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Clientes", action = "Login", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Clientes",
                url: "{controller}/{action}/{id}"
            );

            routes.MapRoute(
                name: "Empresas",
                url: "{controller}/{action}/{id}"
            );

            routes.MapRoute(
                name: "Datos",
                url: "{controller}/{action}/{id}"
            );
        }
    }
}
