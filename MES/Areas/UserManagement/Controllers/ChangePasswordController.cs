using MES.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Areas.UserManagement.Controllers
{
    public class ChangePasswordController : SecuredWebControllerBase
    {
        // GET: UserManagement/ChangePassword
        public ActionResult Index()
        {
            ViewBag.log = "Change Password";
            return View();
        }
    }
}