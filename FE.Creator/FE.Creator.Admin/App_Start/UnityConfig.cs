using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc5;
using FE.Creator.ObjectRepository;
using FE.Creator.FileStorage;
using Microsoft.Owin.Security;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using FE.Creator.Admin.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FE.Creator.Admin
{
    public static class UnityConfig
    {
        public static UnityContainer getUnityContainer()
        {
            var container = new UnityContainer();
            container.RegisterType<IObjectService, DefaultObjectService>(new HierarchicalLifetimeManager());

            string rootPath = System.IO.Path.Combine(System.Web.HttpRuntime.AppDomainAppPath, "App_Data");
            container.RegisterType<IFileStorageService, LocalFileSystemStorage>(
                new InjectionFactory(
                    c => new LocalFileSystemStorage(rootPath)));

            //register the ownin context.
            container.RegisterType<HttpContextBase>(new InjectionFactory(c => new HttpContextWrapper(HttpContext.Current)));
            container.RegisterType<ApplicationSignInManager>(
                new InjectionFactory(c => HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>()));
            container.RegisterType<ApplicationUserManager, ApplicationUserManager>(
                new HierarchicalLifetimeManager()
                , new InjectionFactory(c => HttpContext.Current.GetOwinContext().Get<ApplicationUserManager>()));

            return container;
        }
        public static void RegisterComponents()
        {
            var dependencyResolver = getUnityContainer();
            DependencyResolver.SetResolver(new UnityDependencyResolver(dependencyResolver));
        }
    }
}