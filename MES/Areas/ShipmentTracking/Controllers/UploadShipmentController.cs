using MES.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
    
namespace MES.Areas.ShipmentTracking.Controllers
{
    public class UploadShipmentController : SecuredWebControllerBase
    {
        // GET: ShipmentTracking/UploadShipment
        public ActionResult Index()
        {
            ViewData.Add(new KeyValuePair<string, object>("ParentId", MES.Business.Library.Enums.Pages.Shipment));
            ViewBag.log = "Upload Shipment";
            return View();
        }
    }
}