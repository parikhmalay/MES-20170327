using MES.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Areas.Setup.Controllers
{
    public class StatusController : SecuredWebControllerBase
    {
        // GET: Setup/Status
        public ActionResult Index()
        {
            ViewBag.log = "Status";
            return View();
        }
    }
}
