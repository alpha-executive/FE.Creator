using FE.Creator.Admin.Models;
using FE.Creator.ObjectRepository;
using FE.Creator.ObjectRepository.EntityModels;
using FE.Creator.ObjectRepository.ServiceModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace FE.Creator.Admin.MVCExtension
{
    public class UnknownErrorFilterAttribute : ExceptionFilterAttribute
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        private void LogErrorEvent(IObjectService objectService, HttpActionExecutedContext actionExecutedContext)
        {
            logger.Debug("Log Error Event start.");
            string owner = actionExecutedContext.ActionContext.RequestContext.Principal.Identity.Name;
            logger.Debug("owner = " + owner);

            ServiceObject svObject = new ServiceObject();
            svObject.ObjectName = FE.Creator.Admin.lang.AppLang.EVENT_APP_ERROR;
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
                    Value = lang.AppLang.EVENT_APP_UNKOWN_ERROR
                }
            });
            svObject.Properties.Add(new ObjectKeyValuePair()
            {
                KeyName = "eventDetails",
                Value = new PrimeObjectField()
                {
                    PrimeDataType = PrimeFieldDataType.String,
                    Value = string.Format(lang.AppLang.EVENT_APP_ACCESS_URL_ERROR, actionExecutedContext.ActionContext.Request.RequestUri.PathAndQuery)
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
                    PrimeDataType = PrimeFieldDataType.String,
                    Value = (int)AppEventModel.EnumEventLevel.Error
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

            logger.Info("Create event object : " + objId);
            logger.Debug("Log Error Event Done.");
        }
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            try
            {
               IObjectService objectService = actionExecutedContext
                    .ActionContext
                    .ControllerContext
                    .Configuration
                    .DependencyResolver
                    .GetService(typeof(IObjectService)) as IObjectService;

                LogErrorEvent(objectService, actionExecutedContext);
            }
            catch(Exception ex)
            {
                logger.Error(ex);
            }

            actionExecutedContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
        }
    }
}