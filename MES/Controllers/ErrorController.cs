using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AccessDenied()
        {
            ViewBag.ErrorMessage = "Access denied.";
            return View();
        }

        public ActionResult InvalidAccess()
        {
            ViewBag.ErrorMessage = "Unauthorized request error.";

            return View();
        }

        public ActionResult NotFound()
        {
            ViewBag.ErrorMessage = "Page not found.";
            return View();
        }

        public ActionResult InternalError()
        {
            ViewBag.ErrorMessage = "Internal Server Error.";
            return View();
        }
        public ActionResult CommonError()
        {
            ViewBag.ErrorMessage = "Error";
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}