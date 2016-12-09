using FE.Creator.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace FE.Creator.Admin.ViewController.Controllers
{
    [Authorize]
    public class AngularViewController : Controller
    {
        public ActionResult ClientTemplate(string module, string name)
        {
            if (name == null || !Regex.IsMatch(name, @"^[-\w]+$"))
                throw new ArgumentException("Illegal template name", "name");

            if (module == null || !Regex.IsMatch(module, @"^[-\w]+$"))
                throw new ArgumentException("Illegal template name", "name");
            
            return View(string.Format("~/Views/AngularView/Client/{0}/{1}.cshtml", module, name), "_AngularLayout");
        }

        public ActionResult Index(string module, string name)
        {
            if (name == null || !Regex.IsMatch(name, @"^[-\w]+$"))
                throw new ArgumentException("Illegal template name", "name");

            if (module == null || !Regex.IsMatch(module, @"^[-\w]+$"))
                throw new ArgumentException("Illegal template name", "name");

            ViewBag.Title = module;
            return View(string.Format("~/Views/AngularView/Server/{0}/{1}.cshtml", module, name));
        }


        public ActionResult EditOrDisplay(string module, string name, int Id)
        {
            if (name == null || !Regex.IsMatch(name, @"^[-\w]+$"))
                throw new ArgumentException("Illegal template name", "name");

            if (module == null || !Regex.IsMatch(module, @"^[-\w]+$"))
                throw new ArgumentException("Illegal template name", "name");

            ViewBag.Title = module;
            return View(string.Format("~/Views/AngularView/Server/{0}/{1}.cshtml", module, name), Id);
        }
    }
}