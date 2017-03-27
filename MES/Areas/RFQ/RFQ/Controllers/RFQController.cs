using MES.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace MES.Areas.RFQ.RFQ.Controllers
{
    public class RFQController : SecuredWebControllerBase
    {
        // GET: RFQ/RFQ
        public ActionResult Index()
        {
            ViewData.Add(new KeyValuePair<string, object>("ParentId", MES.Business.Library.Enums.Pages.RFQ));
            ViewBag.log = "log is here";
            return View("~/Areas/RFQ/RFQ/Views/RFQ/Index.cshtml");
        }

        public ActionResult DownloadFile(string filePath, string fileName)
        {
            string ext = Path.GetExtension(filePath);
            Stream fileStream = MES.Business.Library.Helper.BlobHelper.AttachFile_ConvertBlobToStream(filePath);
            return File(fileStream, ext, fileName);
        }
    }
}