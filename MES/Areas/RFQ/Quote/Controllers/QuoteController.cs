using MES.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Areas.RFQ.Quote.Controllers
{
    public class QuoteController : SecuredWebControllerBase
    {
        // GET: RFQ/Quote
        public ActionResult Index()
        {
            ViewData.Add(new KeyValuePair<string, object>("ParentId", MES.Business.Library.Enums.Pages.QuoteList));
            ViewBag.log = "log is here";
            return View("~/Areas/RFQ/Quote/Views/Quote/Index.cshtml");
        }

    }
}
