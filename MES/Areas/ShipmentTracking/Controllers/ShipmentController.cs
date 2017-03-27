using MES.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Areas.ShipmentTracking.Controllers
{
    public class ShipmentController : SecuredWebControllerBase
    {
        // GET: ShipmentTracking/Shipment
        public ActionResult Index()
        {
            ViewData.Add(new KeyValuePair<string, object>("ParentId", MES.Business.Library.Enums.Pages.ShipmentList));
            ViewBag.log = "Shipment Tracking";
            return View();
        }
        public ActionResult DownloadFile(string filePath, string fileName)
        {
            string ext = Path.GetExtension(filePath);
            Stream fileStream = MES.Business.Library.Helper.BlobHelper.AttachFile_ConvertBlobToStream(filePath);
            return File(fileStream, ext, fileName);
        }
    }
}