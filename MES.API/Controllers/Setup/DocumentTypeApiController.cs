using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.Setup.DocumentType;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.Setup
{
    [AdminPrefix("DocumentTypeApi")]
    public class DocumentTypeApiController : SecuredApiControllerBase
    {
        [Inject]
        public IDocumentTypeRepository DocumentTypeRepository { get; set; }

        #region Methods

        /// <summary>
        /// save the DocumentType data.
        /// </summary>
        /// <param name="documentType"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.Setup.DocumentType.DocumentType documentType)
        {
            var type = this.Resolve<IDocumentTypeRepository>(DocumentTypeRepository).Save(documentType);
            return type;
        }

        /// <summary>
        /// delete the DocumentType data.
        /// </summary>
        /// <param name="documentTypeId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int documentTypeId)
        {
            var type = this.Resolve<IDocumentTypeRepository>(DocumentTypeRepository).Delete(documentTypeId);
            return type;
        }
       
        /// <summary>
        /// Get DocumentType list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetDocumentTypesList")]
        public ITypedResponse<List<MES.DTO.Library.Setup.DocumentType.DocumentType>> GetDocumentTypesList(GenericPage<MES.DTO.Library.Setup.DocumentType.SearchCriteria> paging)
        {
            var type = this.Resolve<IDocumentTypeRepository>(DocumentTypeRepository).GetDocumentTypesList(paging);
            return type;
        }

        #endregion Methods
    }
}