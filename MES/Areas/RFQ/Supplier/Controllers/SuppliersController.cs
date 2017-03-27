using MES.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace MES.API.Areas.RFQ.Supplier.Controllers
{
    public class SuppliersController : SecuredWebControllerBase
    {
        public ActionResult Index()
        {
            ViewData.Add(new KeyValuePair<string, object>("ParentId", MES.Business.Library.Enums.Pages.SupplierList));
            ViewBag.log = "log is here";
            return View("~/Areas/RFQ/Supplier/Views/Suppliers/Index.cshtml");
        }
    }
}