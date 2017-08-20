using FE.Creator.ObjectRepository;
using FE.Creator.ObjectRepository.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace FE.Creator.Admin.Controllers
{
    public class BaseController : Controller
    {
        IObjectService objectService = null;
        public BaseController(IObjectService objectService)
        {
            this.objectService = objectService;
        }

        private int GetAppObjectDefintionIdByName(string defName)
        {
            var objDefs = objectService.GetAllObjectDefinitions();
            var findObjDef = (from def in objDefs
                              where def.ObjectDefinitionName.Equals(defName, StringComparison.InvariantCultureIgnoreCase)
                              select def).FirstOrDefault();

            return findObjDef.ObjectDefinitionID;
        }

        private string getAppSettingsLang()
        {
            int settingsDefId = GetAppObjectDefintionIdByName("AppConfig");
            var settings = objectService.GetServiceObjects(settingsDefId,
                new string[] { "language" },
                1,
                1);
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

        protected string getLanguageJSFilePath()
        {
            string enUsPath = Server.MapPath("~/lang/applang.en_us.js");
            string zhCNPath = Server.MapPath("~/lang/applang.zh_cn.js");

            string lang = getAppSettingsLang();
            if (!string.IsNullOrEmpty(lang))
            {
                if ("zh-CN".Equals(lang, StringComparison.InvariantCultureIgnoreCase))
                {
                    return zhCNPath;
                }
            }

            //if language is not set in appsettings, apply chinese language if it's in chinese environment.
            if (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.Equals("zh-CN", StringComparison.InvariantCultureIgnoreCase))
            {
                return zhCNPath;
            }

            return enUsPath;
        }
    }
}