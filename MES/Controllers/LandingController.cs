using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


namespace MES.Controllers
{
    public class LandingController : Controller
    {
        // GET: Landing
        public ActionResult Index()
        {
            return View();
        }
    }
}