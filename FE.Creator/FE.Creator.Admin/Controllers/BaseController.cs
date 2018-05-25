using FE.Creator.Admin.Models;
using FE.Creator.ObjectRepository;
using FE.Creator.ObjectRepository.EntityModels;
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
        protected IObjectService objectService = null;
        public BaseController(IObjectService objectService)
        {
            this.objectService = objectService;
        }

        protected int LogEvent(IObjectService objectService, string owner, AppEventModel.EnumEventLevel level, string title, string description)
        {
            ServiceObject svObject = new ServiceObject();
            svObject.ObjectName = FE.Creator.Admin.lang.AppLang.EVENT_APP_EVENT;
            svObject.ObjectOwner = owner;
            svObject.OnlyUpdateProperties = false;
            svObject.UpdatedBy = owner;
            svObject.CreatedBy = owner;
            svObject.ObjectDefinitionId = objectService
                .GetObjectDefinitionByName("AppEvent")
                .ObjectDefinitionID;

            svObject.Properties.Add(new ObjectKeyValuePair()
            {
                KeyName = "eventTitle",
                Value = new PrimeObjectField()
                {
                    PrimeDataType = PrimeFieldDataType.String,
                    Value = title
                }
            });
            svObject.Properties.Add(new ObjectKeyValuePair()
            {
                KeyName = "eventDetails",
                Value = new PrimeObjectField()
                {
                    PrimeDataType = PrimeFieldDataType.String,
                    Value = description
                }
            });

            svObject.Properties.Add(new ObjectKeyValuePair()
            {
                KeyName = "eventDateTime",
                Value = new PrimeObjectField()
                {
                    PrimeDataType = PrimeFieldDataType.Datetime,
                    Value = DateTime.Now
                }
            });
            svObject.Properties.Add(new ObjectKeyValuePair()
            {
                KeyName = "eventLevel",
                Value = new PrimeObjectField()
                {
                    PrimeDataType = PrimeFieldDataType.Integer,
                    Value = (int)level
                }
            });

            svObject.Properties.Add(new ObjectKeyValuePair()
            {
                KeyName = "eventOwner",
                Value = new PrimeObjectField()
                {
                    PrimeDataType = PrimeFieldDataType.String,
                    Value = owner
                }
            });

            int objId = objectService.CreateORUpdateGeneralObject(svObject);

            return objId;
        }

        protected int GetAppObjectDefintionIdByName(string defName)
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
            else
            {
                //if language is not set in appsettings, apply chinese language if it's in chinese environment.
                if (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.Equals("zh-CN", StringComparison.InvariantCultureIgnoreCase))
                {
                    return zhCNPath;
                }
            }
 
            return enUsPath;
        }
    }
}