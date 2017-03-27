using MES.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Areas.RFQ.RFQ.Controllers
{
    public class SubmitQuoteController : WebControllerBase
    {
        // GET: RFQ/RFQ
        public ActionResult Index()
        {
            ViewBag.log = "log is here";
            return View("~/Areas/RFQ/RFQ/Views/SubmitQuote/Index.cshtml");
        }
    }
}