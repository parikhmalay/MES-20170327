using MES.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Areas.RFQ.PartQuoteReportFromQuote.Controllers
{
    public class PartQuoteReportFromQuoteController : SecuredWebControllerBase
    {
        // GET: RFQ/DQ
        public ActionResult Index()
        {
            ViewBag.log = "log is here";
            return View("~/Areas/RFQ/PartQuoteReportFromQuote/Views/PartQuoteReportFromQuote/Index.cshtml");
        }
    }
}