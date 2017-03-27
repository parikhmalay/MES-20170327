using MES.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Areas.Setup.Controllers
{
    public class ProcessController : SecuredWebControllerBase
    {
        // GET: Setup/Process
        public ActionResult Index()
        {
            ViewBag.log = "Process";
            return View();
        }
    }
}
