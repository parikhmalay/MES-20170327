using MES.API.Attributes;
using MES.Business.Repositories.APQP.CAPA;
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

namespace MES.API.Controllers.APQP.CAPA
{
    [AdminPrefix("CAPAApi")]
    public class CAPAApiController : SecuredApiControllerBase
    {
        [Inject]
        public ICAPARepository CAPARepository { get; set; }

        #region Methods
        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.APQP.CAPA.CAPA> Get(int Id)
        {
            var type = this.Resolve<ICAPARepository>(CAPARepository).FindById(Id);
            return type;
        }
        [HttpPostRoute("CheckPartAssociationWithSupplier")]
        public ITypedResponse<string> CheckPartAssociationWithSupplier(DTO.Library.APQP.CAPA.CAPA capaData)
        {
            var type = this.Resolve<ICAPARepository>(CAPARepository).CheckPartAssociationWithSupplier(capaData);
            return type;
        }
        /// <summary>
        /// save the APQP data.
        /// </summary>
        /// <param name="APQP"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.APQP.CAPA.CAPA capaData)
        {
            var type = this.Resolve<ICAPARepository>(CAPARepository).Save(capaData);
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
            var type = this.Resolve<ICAPARepository>(CAPARepository).Delete(id);
            return type;
        }
        [HttpPostRoute("DeleteCAPAPartAffectedDetail")]
        public ITypedResponse<bool?> DeleteCAPAPartAffectedDetail(int CAPAPartAffectedDetailId)
        {
            var type = this.Resolve<ICAPARepository>(CAPARepository).DeleteCAPAPartAffectedDetail(CAPAPartAffectedDetailId);
            return type;
        }

        /// <summary>
        /// Get CAPA list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetCAPAList")]
        public ITypedResponse<List<MES.DTO.Library.APQP.CAPA.CAPA>> GetCAPAList(GenericPage<MES.DTO.Library.APQP.CAPA.SearchCriteria> paging)
        {
            var type = this.Resolve<ICAPARepository>(CAPARepository).GetCAPAList(paging);
            return type;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpPostRoute("getSAPPartsList")]
        public ITypedResponse<List<MES.DTO.Library.Common.LookupFields>> getSAPPartsList(string supplierName, string customerName)
        {
            var type = this.Resolve<ICAPARepository>(CAPARepository).getSAPPartsList(supplierName, customerName);
            return type;
        }
        [HttpPostRoute("GenerateCAPAForm")]
        public ITypedResponse<bool?> GenerateCAPAForm(DTO.Library.APQP.CAPA.CAPA capaData)
        {
            var type = this.Resolve<ICAPARepository>(CAPARepository).GenerateCAPAForm(capaData);
            return type;
        }

        [HttpPostRoute("GetPartDocumentList")]
        public ITypedResponse<List<MES.DTO.Library.APQP.CAPA.capaPartDocument>> GetPartDocumentList(int cAPAPartAffectedDetailId, string SectionName)
        {
            var type = this.Resolve<ICAPARepository>(CAPARepository).GetPartDocumentList(cAPAPartAffectedDetailId, SectionName);
            return type;
        }
        [HttpPostRoute("DeletePartDocument")]
        public ITypedResponse<bool?> DeletePartDocument(int documentId)
        {
            var type = this.Resolve<ICAPARepository>(CAPARepository).DeletePartDocument(documentId);
            return type;
        }
        [HttpPostRoute("SavePartDocument")]
        public ITypedResponse<int?> SavePartDocument(DTO.Library.APQP.CAPA.capaPartDocument document)
        {
            var type = this.Resolve<ICAPARepository>(CAPARepository).SavePartDocument(document);
            return type;
        }
        [HttpPostRoute("SendEmail")]
        public ITypedResponse<bool?> SendEmail(MES.DTO.Library.Common.EmailData EmailData)
        {
            var type = this.Resolve<ICAPARepository>(CAPARepository).SendEmail(EmailData);
            return type;
        }
        #endregion
    }
}
