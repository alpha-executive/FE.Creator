using FE.Creator.ObjectRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FE.Creator.Admin.Controllers.ViewController
{
    public class PrintController : Controller
    {
        IObjectService objectService = null;

        //public PrintController()
        //{
        //    this.objectService = new DefaultObjectService();
        //}

        public PrintController(IObjectService objectService)
        {
            this.objectService = objectService;
        }

        // GET: Print
        public ActionResult PrintArticle(int id)
        {
            var serviceObject = objectService.GetServiceObjectById(id, 
                new string[]{ "articleSharedLevel", "isOriginal", "articleDesc", "articleContent" });
            return View(serviceObject);
        }
    }
}