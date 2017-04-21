using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace FE.Creator.Admin.MVCExtension
{
    public class UnknownErrorFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
        }
    }
}