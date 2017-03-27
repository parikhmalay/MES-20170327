using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.APQP.DocumentManagement;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.APQP.DocumentManagement
{
    [AdminPrefix("DocumentManagementApi")]
    public class DocumentManagementApiController : SecuredApiControllerBase
    {
        [Inject]
        public IDocumentManagementRepository DocumentManagementRepository { get; set; }

        #region Methods

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.APQP.DocumentManagement.DocumentManagement> Get(int Id)
        {
            var type = this.Resolve<IDocumentManagementRepository>(DocumentManagementRepository).FindById(Id);
            return type;
        }

        /// <summary>
        /// Get DocumentManagement list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetDocumentManagementList")]
        public ITypedResponse<List<MES.DTO.Library.APQP.DocumentManagement.DocumentManagement>> GetDocumentManagementList(GenericPage<MES.DTO.Library.APQP.DocumentManagement.SearchCriteria> paging)
        {
            var type = this.Resolve<IDocumentManagementRepository>(DocumentManagementRepository).GetDocumentManagementList(paging);
            return type;
        }

        [HttpPostRoute("GetDocumentList")]
        public ITypedResponse<List<MES.DTO.Library.APQP.DocumentManagement.Document>> GetDocumentList(int DocumentManagementId)
        {
            var type = this.Resolve<IDocumentManagementRepository>(DocumentManagementRepository).GetDocumentList(DocumentManagementId);
            return type;
        }
        [HttpPostRoute("DownloadDocuments")]
        public ITypedResponse<string> DownloadDocuments(List<string> DocumentFilePathList)
        {
            var type = this.Resolve<IDocumentManagementRepository>(DocumentManagementRepository).DownloadDocuments(DocumentFilePathList);
            return type;
        }
        
        #endregion Methods
    }
}
