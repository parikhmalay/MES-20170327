using MES.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Areas.UserManagement.Controllers
{
    public class UserManagementController : SecuredWebControllerBase
    {
        // GET: UserManagement/UserManagement
        public ActionResult Index()
        {
            ViewData.Add(new KeyValuePair<string, object>("ParentId", MES.Business.Library.Enums.Pages.UserList));
            ViewBag.log = "User Management";
            return View();
        }
    }
}