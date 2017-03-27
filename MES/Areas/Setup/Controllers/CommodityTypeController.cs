using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MES.Controllers;


namespace MES.Areas.Setup.Controllers
{
    public class CommodityTypeController : SecuredWebControllerBase
    {
        public ActionResult Index()
        {
            ViewBag.log = "Commodity Type";
            return View();
        }
    }
}