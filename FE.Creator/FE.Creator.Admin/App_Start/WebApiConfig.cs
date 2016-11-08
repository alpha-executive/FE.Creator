using FE.Creator.Admin.MVCExtension;
using FE.Creator.ObjectRepository;
using Microsoft.Practices.Unity;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace FE.Creator.Admin
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Use camel case for JSON data.
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Web API routes
            config.MapHttpAttributeRoutes();

           
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                   name: "DefaultCustomApi",
                   routeTemplate: "api/custom/{controller}/{action}/{id}",
                   defaults: new { id = RouteParameter.Optional }
               );


            //uncomment this will avoid return xml serialization output to client.
            //config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.Clear();

            config.Formatters.Add(new ObjectDefintionFormatter());
            Newtonsoft.Json.JsonConvert.DefaultSettings = () => new Newtonsoft.Json.JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var container = new UnityContainer();
            container.RegisterType<IObjectService, DefaultObjectService>(new HierarchicalLifetimeManager());
            config.DependencyResolver = new UnityDependencyResolver(container);

        }
    }
}
