using MES.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Areas.RoleManagement.Controllers
{
    public class RoleManagementController : SecuredWebControllerBase
    {
        // GET: RoleManagement/RoleManagement
        public ActionResult Index()
        {
            ViewData.Add(new KeyValuePair<string, object>("ParentId", MES.Business.Library.Enums.Pages.RoleList));           
            ViewBag.log = "Role Management";
            return View();
        }
    }
}