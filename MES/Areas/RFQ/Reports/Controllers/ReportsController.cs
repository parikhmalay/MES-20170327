using MES.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace MES.API.Areas.RFQ.Reports.Controllers
{
    public class ReportsController : SecuredWebControllerBase
    {
        public ActionResult Index()
        {
            ViewBag.log = "log is here";
            return View("~/Areas/RFQ/Reports/Views/Reports/Index.cshtml");
        }
    }
}