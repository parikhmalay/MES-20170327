using MES.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Areas.Setup.Controllers
{
    public class RemarksController : SecuredWebControllerBase
    {
        // GET: Setup/Remarks
        public ActionResult Index()
        {
            ViewBag.log = "Remarks";
            return View();
        }
    }
}
