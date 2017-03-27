using MES.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace MES.API.Areas.RFQ.Customer.Controllers
{
    public class CustomersController : SecuredWebControllerBase
    {
        public ActionResult Index()
        {
            ViewData.Add(new KeyValuePair<string, object>("ParentId", MES.Business.Library.Enums.Pages.CustomerList));
            ViewBag.log = "log is here";
            return View("~/Areas/RFQ/Customer/Views/Customers/Index.cshtml");
        }
    }
}