using MES.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Areas.Setup.Controllers
{
    public class RFQPriorityController : SecuredWebControllerBase
    {
        // GET: Setup/RFQPriority
        public ActionResult Index()
        {
            ViewBag.log = "RFQPriority";
            return View();
        }
    }
}
