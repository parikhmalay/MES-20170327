using MES.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Areas.Setup.Controllers
{
    public class IndustryTypeController : SecuredWebControllerBase
    {
        // GET: Setup/IndustryType
        public ActionResult Index()
        {
            ViewBag.log = "Industry Type";
            return View();
        }
    }
}
