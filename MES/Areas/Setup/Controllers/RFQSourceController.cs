using MES.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Areas.Setup.Controllers
{
    public class RFQSourceController : SecuredWebControllerBase
    {
        // GET: Setup/RFQSource
        public ActionResult Index()
        {
            ViewBag.log = "RFQ Source";
            return View();
        }
    }
}
