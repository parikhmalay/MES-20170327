using MES.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Areas.Setup.Controllers
{
    public class NonAwardFeedbackController : SecuredWebControllerBase
    {
        // GET: Setup/NonAwardFeedback
        public ActionResult Index()
        {
            ViewBag.log = "Non-Award Feedback";
            return View();
        }
    }
}
