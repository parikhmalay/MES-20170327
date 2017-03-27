using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NPE.Core;
using NPE.Core.Extended;
using MES.Business.Repositories.ShipmentTracking;

namespace MES.API.Controllers.ShipmentTracking
{
    [AdminPrefix("ShipmentApi")]
    public class ShipmentApiController : SecuredApiControllerBase
    {
        [Inject]
        public IShipmentsRepository ShipmentRepository { get; set; }

        #region Methods

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.ShipmentTracking.Shipments> Get(int Id)
        {
            var type = this.Resolve<IShipmentsRepository>(ShipmentRepository).FindById(Id);
            return type;
        }

        /// <summary>
        /// save the Supplier data.
        /// </summary>
        /// <param name="Suppliers"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.ShipmentTracking.Shipments shipment)
        {
            var type = this.Resolve<IShipmentsRepository>(ShipmentRepository).Save(shipment);
            return type;
        }

        /// <summary>
        /// delete the Shipment data.
        /// </summary>
        /// <param name="SuppliersId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int shipmentId)
        {
            var type = this.Resolve<IShipmentsRepository>(ShipmentRepository).Delete(shipmentId);
            return type;
        }

        /// <summary>
        /// Get Shipment list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetShipmentList")]
        public ITypedResponse<List<MES.DTO.Library.ShipmentTracking.Shipments>> GetShipmentList(GenericPage<MES.DTO.Library.ShipmentTracking.SearchCriteria> paging)
        {
            var type = this.Resolve<IShipmentsRepository>(ShipmentRepository).GetShipmentList(paging);
            return type;
        }

        /// <summary>
        /// Export to Excel.
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpPostRoute("DownloadShipment")]
        public ITypedResponse<bool?> DownloadShipment()
        {
            var type = this.Resolve<IShipmentsRepository>(ShipmentRepository).DownloadShipment();
            return type;
        }

        /// <summary>
        /// upload rspq.
        /// </summary>
        /// <param name="RFQ"></param>
        /// <returns></returns>
        [HttpPostRoute("UploadShipment")]
        public NPE.Core.ITypedResponse<int?> UploadShipment(string filePath)
        {
            var type = this.Resolve<IShipmentsRepository>(ShipmentRepository).UploadShipment(filePath);
            return type;
        }

        /// <summary>
        /// Get Shipment list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("exportToExcelShipmentReport")]
        public ITypedResponse<bool?> exportToExcelShipmentReport(GenericPage<MES.DTO.Library.ShipmentTracking.SearchCriteria> paging)
        {
            var type = this.Resolve<IShipmentsRepository>(ShipmentRepository).exportToExcelShipmentReport(paging);
            return type;
        }
        #endregion Methods
    }
}