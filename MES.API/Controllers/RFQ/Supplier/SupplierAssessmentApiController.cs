using MES.API.Attributes;
using MES.API.Extensions;
using MES.Business.Repositories.RFQ.Supplier;
using Ninject;
using NPE.Core;
using NPE.Core.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MES.API.Controllers.RFQ.Supplier
{
    [AdminPrefix("SupplierAssessmentApi")]
    public class SupplierAssessmentApiController : SecuredApiControllerBase
    {
        [Inject]
        public ISupplierAssessmentRepository SupplierAssessmentRepository { get; set; }

        #region Methods
        /// <summary>
        /// Get Assessment Detail(Add mode)..
        /// </summary>
        /// <returns></returns>
        [HttpGetRoute("GetAssessmentDetail")]
        public ITypedResponse<MES.DTO.Library.RFQ.Supplier.SupplierAssessment> GetAssessmentDetail(int id)
        {
            var type = this.Resolve<ISupplierAssessmentRepository>(SupplierAssessmentRepository).GetAssessmentDetail(id);
            return type;
        }

        /// <summary>
        /// Get supplier assessment detail..
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("GetAssessment")]
        public ITypedResponse<MES.DTO.Library.RFQ.Supplier.SupplierAssessment> Get(int Id)
        {
            var type = this.Resolve<ISupplierAssessmentRepository>(SupplierAssessmentRepository).FindById(Id);
            return type;
        }

        ///// <summary>
        ///// Get Suppliers list.
        ///// </summary>
        ///// <param name="page"></param>
        ///// <returns></returns>
        //[HttpPostRoute("GetSupplierAssessmentList")]
        //public ITypedResponse<List<MES.DTO.Library.RFQ.Supplier.SupplierAssessment>> GetSupplierAssessmentList(GenericPage<MES.DTO.Library.RFQ.Supplier.SearchCriteria> paging)
        //{
        //    var type = this.Resolve<ISupplierAssessmentRepository>(SupplierAssessmentRepository).GetSupplierAssessmentList(paging);
        //    return type;
        //}

        /// <summary>
        /// Save supplier assessment detail..
        /// </summary>
        /// <param name="supplierObj"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.RFQ.Supplier.SupplierAssessment supplierObj)
        {
            var type = this.Resolve<ISupplierAssessmentRepository>(SupplierAssessmentRepository).Save(supplierObj);
            return type;
        }

        /// <summary>
        /// Delete assessment detail..
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int assessmentId)
        {
            var type = this.Resolve<ISupplierAssessmentRepository>(SupplierAssessmentRepository).Delete(assessmentId);
            return type;
        }

        /// <summary>
        /// Create revision..
        /// </summary>
        /// <param name="supplierObj"></param>
        /// <returns></returns>
        [HttpPostRoute("CreateRevision")]
        public ITypedResponse<int?> CreateRevision(int assessmentId)
        {
            var type = this.Resolve<ISupplierAssessmentRepository>(SupplierAssessmentRepository).CreateRevision(assessmentId);
            return type;
        }

        /// <summary>
        /// Get Contacts list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetSupplierAssessmentList")]
        public List<MES.DTO.Library.RFQ.Supplier.AssessmentListDetail> GetSupplierAssessmentList(int supplierId)
        {
            var type = this.Resolve<ISupplierAssessmentRepository>(SupplierAssessmentRepository).GetSupplierAssessmentList(supplierId);
            return type;
        }
        #endregion

    }
}
