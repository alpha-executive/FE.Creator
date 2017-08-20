using FE.Creator.ObjectRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FE.Creator.Admin.Controllers.ViewController
{
    public class ErrorController : BaseController
    {
        public ErrorController(IObjectService objectService) : base(objectService) { }
        // GET: Error
        public ActionResult Page(int id)
        {
            return View("ErrorPage", id);
        }
    }
}