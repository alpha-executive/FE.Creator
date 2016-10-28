using FE.Creator.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace FE.Creator.Admin.Controllers
{
    public class AngularViewController : Controller
    {
        public ActionResult Template(string module, string name)
        {
            if (name == null || !Regex.IsMatch(name, @"^[-\w]+$"))
                throw new ArgumentException("Illegal template name", "name");

            if (module == null || !Regex.IsMatch(module, @"^[-\w]+$"))
                throw new ArgumentException("Illegal template name", "name");

            return View("Template", new AngularUrl
            {
               ModuleName = module,
               TemplateName = name
            });
        }
    }
}