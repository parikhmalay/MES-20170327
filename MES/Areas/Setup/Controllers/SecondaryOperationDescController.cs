using MES.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Areas.Setup.Controllers
{
    public class SecondaryOperationDescController : SecuredWebControllerBase
    {
        // GET: Setup/SecondaryOperationDesc
        public ActionResult Index()
        {
            ViewBag.log = "Secondary Operation Description";
            return View();
        }
    }
}
