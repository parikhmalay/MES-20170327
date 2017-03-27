using MES.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Areas.UserManagement.Controllers
{
    public class PreferencesController : SecuredWebControllerBase
    {
        // GET: UserManagement/Preferences
        public ActionResult Index()
        {
            ViewBag.log = "User Preferences";
            return View();
        }
    }
}