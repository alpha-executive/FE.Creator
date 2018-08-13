using FE.Creator.Admin.Areas.Portal.Models;
using FE.Creator.ObjectRepository;
using FE.Creator.ObjectRepository.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace FE.Creator.Admin.Areas.Portal.Controllers
{
    public class PortalHomeController : Controller
    {
        private IObjectService objectService = null;
        public PortalHomeController(IObjectService objectService)
        {
            this.objectService = objectService;
        }

        private int GetAppObjectDefintionIdByName(string defName)
        {
            var objDefs = objectService.GetAllObjectDefinitions();
            if (objDefs == null
                || objDefs.Count == 0)
                return -1;

            var findObjDef = (from def in objDefs
                              where def.ObjectDefinitionName.Equals(defName, StringComparison.InvariantCultureIgnoreCase)
                              select def).FirstOrDefault();

            return findObjDef.ObjectDefinitionID;
        }


        protected string getAppSettingsLang()
        {
            int settingsDefId = GetAppObjectDefintionIdByName("AppConfig");
            var settings = objectService.GetServiceObjects(settingsDefId,
                new string[] { "language" },
                1,
                1,
                null);
            if (settings.Count > 0)
            {
                string lang = settings
                     .First()
                     .GetPropertyValue<PrimeObjectField>("language")
                     .GetStrongTypeValue<string>();

                return lang;
            }

            return string.Empty;
        }

        // GET: Portal/PortalHome
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AngularLightBoxTemplate()
        {
            return View();
        }

        public ActionResult ViewArticleContent(string id)
        {
            PostViewModel viewModel = new PostViewModel();
            try
            {
                int objectId = int.Parse(id);
                ServiceObject article = objectService.GetServiceObjectById(objectId,
                      new string[] {
                        "articleDesc",
                        "articleContent",
                        "isOriginal",
                        "articleImage",
                        "articleGroup",
                        "articleSharedLevel" },
                      null
                  );

               int sharedLevel = article.GetPropertyValue<PrimeObjectField>("articleSharedLevel")
                     .GetStrongTypeValue<Int32>();

               if(sharedLevel == 1)
                {
                    viewModel.ObjectId = article.ObjectID;
                    viewModel.PostTitle = article.ObjectName;
                    viewModel.PostDesc = article.GetPropertyValue<PrimeObjectField>("articleDesc")
                        .GetStrongTypeValue<string>();
                    viewModel.PostContent = article.GetPropertyValue<PrimeObjectField>("articleContent")
                        .GetStrongTypeValue<string>();
                    viewModel.IsOriginal = article.GetPropertyValue<PrimeObjectField>("isOriginal")
                        .GetStrongTypeValue<int>() == 1;
                    viewModel.Created = article.Created;
                    viewModel.Author = article.CreatedBy;
                }
            }
            catch(Exception ex)
            {

            }
            
            return View(viewModel);
        }

        public PartialViewResult PortalFlexSlider()
        {
            string lang = getAppSettingsLang();
            if (!string.IsNullOrEmpty(lang))
            {
                if ("zh-CN".Equals(lang, StringComparison.InvariantCultureIgnoreCase))
                {
                    return PartialView("PortalFlexSlider_ZH_CN");
                }
            }
            else
            {
                //if language is not set in appsettings, apply chinese language if it's in chinese environment.
                if (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.Equals("zh-CN", StringComparison.InvariantCultureIgnoreCase))
                {
                    return PartialView("PortalFlexSlider_ZH_CN");
                }
            }

            return PartialView("PortalFlexSlider");
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string lang = getAppSettingsLang();
            if (!string.IsNullOrEmpty(lang))
            {
                if ("zh-CN".Equals(lang, StringComparison.InvariantCultureIgnoreCase))
                {
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");
                }
                else
                {
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                }
            }
            else
            {
                //if it's a none chinese environment, set english as the default language.
                if (!Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.Equals("zh-CN", StringComparison.InvariantCultureIgnoreCase))
                {
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                }
            }

            return base.BeginExecuteCore(callback, state);
        }
    }
}