using MES.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Areas.RFQ.DQ.Controllers
{
    public class SubmitQuoteController : WebControllerBase
    {
        // GET: RFQ/DQ
        public ActionResult Index()
        {
            ViewBag.log = "log is here";
            return View("~/Areas/RFQ/DQ/Views/SubmitQuote/Index.cshtml");
        }
    }
}