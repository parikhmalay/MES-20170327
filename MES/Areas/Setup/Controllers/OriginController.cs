using MES.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Areas.Setup.Controllers
{
    public class OriginController : SecuredWebControllerBase
    {
        // GET: Setup/Origin
        public ActionResult Index()
        {
            ViewBag.log = "Origin";
            return View();
        }
    }
}
