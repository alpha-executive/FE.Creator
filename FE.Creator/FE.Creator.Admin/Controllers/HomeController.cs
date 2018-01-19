using FE.Creator.Admin.Models;
using FE.Creator.ObjectRepository;
using FE.Creator.ObjectRepository.ServiceModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FE.Creator.Admin.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        ILogger logger = LogManager.GetCurrentClassLogger(typeof(HomeController));
        public HomeController(IObjectService objectService) : base(objectService) { }
        public ActionResult Index()
        {
            logger.Debug(string.Format("{0} access the home page", User.Identity.Name));
            return View();
        }

        public ActionResult LocalizationJS()
        {
            string languageFile = getLanguageJSFilePath();
            return File(languageFile, "application/javascript");
        }


        private async Task<List<ProviderNotification>> getProviderNotificationData()
        {
            logger.Debug("Start getProviderNotificationData");
            List<ProviderNotification> notifications = null; 

            int settingsDefId = GetAppObjectDefintionIdByName("AppConfig");
            logger.Debug("settingsDefId = " + settingsDefId);

            var settings = objectService.GetServiceObjects(settingsDefId,
                new string[] { "language", "pullMessageFromPublisher", "pullMessagePublisherUrl" },
                1,
                1);

            if (settings.Count > 0)
            {
                string lang = settings
                         .First()
                         .GetPropertyValue<PrimeObjectField>("language")
                         .GetStrongTypeValue<string>();
                logger.Debug("lang = " + lang);

                bool isPullMessageFromPubliser = settings
                     .First()
                     .GetPropertyValue<PrimeObjectField>("pullMessageFromPublisher")
                     .GetStrongTypeValue<int>() > 0;

                logger.Debug("isPullMessageFromPubliser = " + isPullMessageFromPubliser);

                if (isPullMessageFromPubliser)
                {
                    string publisherUrl = settings
                     .First()
                     .GetPropertyValue<PrimeObjectField>("pullMessagePublisherUrl")
                     .GetStrongTypeValue<string>();

                    logger.Debug("publisherUrl = " + publisherUrl);

                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("req_language", lang);
                    try
                    {
                        var response = await client.GetAsync(publisherUrl);
                        logger.Debug("request sent to " + publisherUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            notifications = await response
                                .Content
                                .ReadAsAsync<List<ProviderNotification>>();

                            logger.Debug("notification count: " + (notifications != null ? notifications.Count : 0));
                        }
                    }
                    catch(Exception e)
                    {
                        logger.Error(e);
                    }
                }
            }

            return notifications;
        }

        [HttpGet()]
        public async Task<ActionResult> ProviderNotification()
        {
            List<ProviderNotification> notifies = await getProviderNotificationData();
            if(notifies == null 
                || notifies.Count == 0)
            {
                logger.Debug("Not get any notification from provider site, will use the default one");
                notifies = new List<Models.ProviderNotification>();
                var notifyData = new ProviderNotification
                {
                    NotifyDesc = FE.Creator.Admin.lang.AppLang.INDEX_NOTIFY_DEFAULT_MSG,
                    ImageSrc = Url.Content("~/Content/adminlte-2.3.6/dist/img/alarm-50.png"),
                    ActionUrl = "#",
                    Notifier = FE.Creator.Admin.lang.AppLang.INDEX_NOTIFY_DEFAULT_PROVIDER,
                    EventTime = DateTime.Now.ToShortDateString()
                };

                notifies.Add(notifyData);
            }

            return Json(notifies, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LatestSystemEvent(int count)
        {
            int eventDefId = GetAppObjectDefintionIdByName("AppEvent");
            logger.Debug("eventDefId = " + eventDefId);

            var events = objectService.GetServiceObjects(eventDefId,
               new string[] { "eventTitle", "eventDetails", "eventDateTime", "eventLevel", "eventOwner" },
               1,
               count > 0 ? count : 10);

            List<AppEventModel> latestEvents = new List<AppEventModel>();
            if(events != null && events.Count > 0)
            {
                foreach(var evt in events)
                {
                    latestEvents.Add(new AppEventModel()
                    {
                        EventTitle = evt.GetPropertyValue<PrimeObjectField>("eventTitle").GetStrongTypeValue<string>(),
                        EventDetails = evt.GetPropertyValue<PrimeObjectField>("eventDetails").GetStrongTypeValue<string>(),
                        EventDateTime = evt.GetPropertyValue<PrimeObjectField>("eventDateTime").GetStrongTypeValue<DateTime>(),
                        EventLevel =  (AppEventModel.EnumEventLevel)evt.GetPropertyValue<PrimeObjectField>("eventLevel").GetStrongTypeValue<int>(),
                        EventOwner = evt.GetPropertyValue<PrimeObjectField>("eventOwner").GetStrongTypeValue<string>()
                    });
                }
            }

            return Json(latestEvents);
        }
    }
}