using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MES.Controllers;


namespace MES.Areas.Setup.Controllers
{
    public class ProjectStageController : SecuredWebControllerBase
    {
        public ActionResult Index()
        {
            ViewBag.log = "Project Stage";
            return View();
        }
    }
}