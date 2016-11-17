using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FE.Creator.Admin.Controllers.ViewController
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Page(int id)
        {
            return View("ErrorPage", id);
        }
    }
}