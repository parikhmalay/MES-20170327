using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.APQP.DefectTracking;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.APQP.DefectTracking
{
    [AdminPrefix("DefectTrackingApi")]
    public class DefectTrackingApiController : SecuredApiControllerBase
    {
        [Inject]
        public IDefectTrackingRepository DefectTrackingRepository { get; set; }

        #region Methods

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.APQP.DefectTracking.DefectTracking> Get(int Id)
        {
            var type = this.Resolve<IDefectTrackingRepository>(DefectTrackingRepository).FindById(Id);
            return type;
        }

        /// <summary>
        /// save the APQP data.
        /// </summary>
        /// <param name="APQP"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.APQP.DefectTracking.DefectTracking defectTracking)
        {
            var type = this.Resolve<IDefectTrackingRepository>(DefectTrackingRepository).Save(defectTracking);
            return type;
        }

        /// <summary>
        /// delete the APQP data.
        /// </summary>
        /// <param name="APQPId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int id)
        {
            var type = this.Resolve<IDefectTrackingRepository>(DefectTrackingRepository).Delete(id);
            return type;
        }

        /// <summary>
        /// Get DefectTracking list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetDefectTrackingList")]
        public ITypedResponse<List<MES.DTO.Library.APQP.DefectTracking.DefectTracking>> GetDefectTrackingList(GenericPage<MES.DTO.Library.APQP.DefectTracking.SearchCriteria> paging)
        {
            var type = this.Resolve<IDefectTrackingRepository>(DefectTrackingRepository).GetDefectTrackingList(paging);
            return type;
        }
        [HttpGetRoute("GetNewRMANumber")]
        public ITypedResponse<string> GetNewRMANumber()
        {
            var type = this.Resolve<IDefectTrackingRepository>(DefectTrackingRepository).GetNewRMANumber();
            return type;
        }
        [HttpPostRoute("GenerateCAPAForm")]
        public ITypedResponse<bool?> GenerateCAPAForm(DTO.Library.APQP.DefectTracking.DefectTracking defectTracking)
        {
            var type = this.Resolve<IDefectTrackingRepository>(DefectTrackingRepository).GenerateCAPAForm(defectTracking);
            return type;
        }
        [HttpPostRoute("GetPartDocumentList")]
        public ITypedResponse<List<MES.DTO.Library.APQP.DefectTracking.PartDocument>> GetPartDocumentList(int defectTrackingDetailId, string SectionName)
        {
            var type = this.Resolve<IDefectTrackingRepository>(DefectTrackingRepository).GetPartDocumentList(defectTrackingDetailId, SectionName);
            return type;
        }
        [HttpPostRoute("DeletePartDocument")]
        public ITypedResponse<bool?> DeletePartDocument(int documentId)
        {
            var type = this.Resolve<IDefectTrackingRepository>(DefectTrackingRepository).DeletePartDocument(documentId);
            return type;
        }
        [HttpPostRoute("SavePartDocument")]
        public ITypedResponse<int?> SavePartDocument(DTO.Library.APQP.DefectTracking.PartDocument document)
        {
            var type = this.Resolve<IDefectTrackingRepository>(DefectTrackingRepository).SavePartDocument(document);
            return type;
        }
        [HttpPostRoute("DeleteDefectTrackingDetail")]
        public ITypedResponse<bool?> DeleteDefectTrackingDetail(int DefectTrackingDetailId)
        {
            var type = this.Resolve<IDefectTrackingRepository>(DefectTrackingRepository).DeleteDefectTrackingDetail(DefectTrackingDetailId);
            return type;
        }
        #endregion Methods
    }
}
