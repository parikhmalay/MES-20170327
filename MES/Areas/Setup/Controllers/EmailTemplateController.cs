using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MES.Controllers;


namespace MES.Areas.Setup.Controllers
{
    public class EmailTemplateController : SecuredWebControllerBase
    {
        public ActionResult Index()
        {
            ViewBag.log = "Email Template";
            return View();
        }
    }
}