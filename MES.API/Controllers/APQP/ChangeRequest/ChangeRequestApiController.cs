using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.APQP.ChangeRequest;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.APQP.ChangeRequest
{
    [AdminPrefix("ChangeRequestApi")]
    public class ChangeRequestApiController : SecuredApiControllerBase
    {
        [Inject]
        public IChangeRequestRepository ChangeRequestRepository { get; set; }

        #region Methods

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.APQP.ChangeRequest.ChangeRequest> Get(int Id)
        {
            var type = this.Resolve<IChangeRequestRepository>(ChangeRequestRepository).FindById(Id);
            return type;
        }

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("GetOnChangeOfPartNumber")]
        public ITypedResponse<MES.DTO.Library.APQP.ChangeRequest.ChangeRequest> GetOnChangeOfPartNumber(int Id)
        {
            var type = this.Resolve<IChangeRequestRepository>(ChangeRequestRepository).OnChangeOfPartNumber(Id);
            return type;
        }


        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("AddToCRFromAPQP")]
        public ITypedResponse<MES.DTO.Library.APQP.ChangeRequest.ChangeRequest> AddToCRFromAPQP(int Id)
        {
            var type = this.Resolve<IChangeRequestRepository>(ChangeRequestRepository).AddToCRFromAPQP(Id);
            return type;
        }

        /// <summary>
        /// save the CR data.
        /// </summary>
        /// <param name="APQP"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.APQP.ChangeRequest.ChangeRequest changeRequest)
        {
            var type = this.Resolve<IChangeRequestRepository>(ChangeRequestRepository).Save(changeRequest);
            return type;
        }

        /// <summary>
        /// save the APQP data.
        /// </summary>
        /// <param name="APQP"></param>
        /// <returns></returns>
        [HttpPostRoute("AddToAPQP")]
        public ITypedResponse<int?> AddToAPQP(MES.DTO.Library.APQP.ChangeRequest.ChangeRequest changeRequest)
        {
            var type = this.Resolve<IChangeRequestRepository>(ChangeRequestRepository).AddToAPQP(changeRequest);
            return type;
        }
        /// <summary>
        /// delete the CR data.
        /// </summary>
        /// <param name="APQPId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int id)
        {
            var type = this.Resolve<IChangeRequestRepository>(ChangeRequestRepository).Delete(id);
            return type;
        }

        /// <summary>
        /// Get ChangeRequest list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetChangeRequestList")]
        public ITypedResponse<List<MES.DTO.Library.APQP.ChangeRequest.ChangeRequest>> GetChangeRequestList(GenericPage<MES.DTO.Library.APQP.ChangeRequest.SearchCriteria> paging)
        {
            var type = this.Resolve<IChangeRequestRepository>(ChangeRequestRepository).GetChangeRequestList(paging);
            return type;
        }

        [HttpPostRoute("SearchFromSAPRecords")]
        public ITypedResponse<List<MES.DTO.Library.APQP.APQP.SAPItem>> SearchFromSAPRecords(GenericPage<MES.DTO.Library.APQP.ChangeRequest.SearchCriteria> paging)
        {
            var type = this.Resolve<IChangeRequestRepository>(ChangeRequestRepository).SearchFromSAPRecords(paging);
            return type;
        }

        [HttpPostRoute("InsertFromSAPRecords")]
        public ITypedResponse<int?> InsertFromSAPRecords(string ItemIds)
        {
            var type = this.Resolve<IChangeRequestRepository>(ChangeRequestRepository).InsertFromSAPRecords(ItemIds);
            return type;
        }

        [HttpGetRoute("GetFromSAPAndInsertInLocalSAPTable")]
        public ITypedResponse<int?> GetFromSAPAndInsertInLocalSAPTable()
        {
            var type = this.Resolve<IChangeRequestRepository>(ChangeRequestRepository).GetFromSAPAndInsertInLocalSAPTable();
            return type;
        }

        /// <summary>
        /// Get log data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("GetChangeLog")]
        public NPE.Core.ITypedResponse<List<MES.DTO.Library.AuditLogs.AuditLogs>> GetChangeLog(int Id)
        {
            var type = this.Resolve<IChangeRequestRepository>(ChangeRequestRepository).crHistoryChangeLog(Id);
            return type;
        }


        /// <summary>
        /// delete the document data.
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [HttpPostRoute("DeleteDocument")]
        public ITypedResponse<bool?> DeleteDocument(int documentId)
        {
            var type = this.Resolve<IChangeRequestRepository>(ChangeRequestRepository).DeleteDocument(documentId);
            return type;
        }
        #endregion Methods
    }
}
