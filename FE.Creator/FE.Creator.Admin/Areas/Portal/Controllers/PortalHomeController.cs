using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FE.Creator.Admin.Areas.Portal.Controllers
{
    public class PortalHomeController : Controller
    {
        // GET: Portal/PortalHome
        public ActionResult Index()
        {
            return View();
        }
    }
}