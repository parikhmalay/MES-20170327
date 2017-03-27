using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MES.Controllers;
namespace MES.Areas.Dashboard.Controllers
{
    public class DashboardController : SecuredWebControllerBase
    {
        // GET: Dashboard/Dashboard
        public ActionResult Index()
        {
            ViewData.Add(new KeyValuePair<string, object>("ParentId", MES.Business.Library.Enums.Pages.Dashboard));           
            ViewBag.log="Dashboard";
            return View();
        }
    }
}