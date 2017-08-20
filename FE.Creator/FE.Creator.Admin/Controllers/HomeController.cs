using FE.Creator.ObjectRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FE.Creator.Admin.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public HomeController(IObjectService objectService) : base(objectService) { }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LocalizationJS()
        {
            string languageFile = getLanguageJSFilePath();
            return File(languageFile, "application/javascript");
        }
    }
}