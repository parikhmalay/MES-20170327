using MES.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Areas.Setup.Controllers
{
    public class RFQTypeController : SecuredWebControllerBase
    {
        // GET: Setup/RFQType
        public ActionResult Index()
        {
            ViewBag.log = "RFQ Type";
            return View();
        }
    }
}
