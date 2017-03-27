using MES.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MES.Business.Library.BO;

namespace MES.API.Areas.APQP.APQP.Controllers
{
    public class APQPController : SecuredWebControllerBase
    {
        public ActionResult Index()
        {
            ViewData.Add(new KeyValuePair<string, object>("ParentId", MES.Business.Library.Enums.Pages.APQP));
            ViewBag.log = "log is here";
            return View("~/Areas/APQP/APQP/Views/APQP/Index.cshtml");
        }

        public ActionResult DownloadFile(string filePath, string fileName)
        {
            string ext = Path.GetExtension(filePath);
            Stream fileStream = MES.Business.Library.Helper.BlobHelper.AttachFile_ConvertBlobToStream(filePath);
            return File(fileStream, ext, fileName);
        }
    }
}