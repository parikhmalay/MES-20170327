using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MES.Controllers;


namespace MES.Areas.Setup.Controllers
{
    public class MachineDescriptionController : SecuredWebControllerBase
    {
        public ActionResult Index()
        {
            ViewBag.log = "Machine Description";
            return View();
        }
    }
}