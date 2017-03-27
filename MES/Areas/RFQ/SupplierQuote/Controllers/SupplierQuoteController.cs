using MES.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Areas.RFQ.SupplierQuote.Controllers
{
    public class SupplierQuoteController : SecuredWebControllerBase
    {
        public ActionResult Index()
        {
            ViewData.Add(new KeyValuePair<string, object>("ParentId", MES.Business.Library.Enums.Pages.RFQM));
            ViewBag.log = "log is here";
            return View("~/Areas/RFQ/SupplierQuote/Views/SupplierQuote/Index.cshtml");
        }
    }
}