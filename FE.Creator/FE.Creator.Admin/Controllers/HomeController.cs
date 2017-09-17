using FE.Creator.ObjectRepository;
using NLog;
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
    }
}