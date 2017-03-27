using Account.DTO.Library;
using MES.Business.Repositories.APQP.ChangeRequest;
using NPE.Core;
using NPE.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPE.Core.Extensions;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;

namespace MES.Business.Library.BO.APQP.ChangeRequest
{
    class Document : ContextBusinessBase, IDocumentRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Document"/> class.
        /// </summary>
        public Document() : base("Document") { }
        /// <summary>
        /// Saves the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.APQP.ChangeRequest.Document document)
        {
            string errMSg = null;
            string successMsg = null;
            string fileName = string.Empty;
            ObjectParameter Id = new ObjectParameter("Id", document.Id);
            ObjectParameter ErrorKey = new ObjectParameter("ErrorKey", "");
            if (!string.IsNullOrEmpty(document.FilePath))
                fileName = MES.Business.Library.Helper.BlobHelper.GetPhysicalPath(document.FilePath);
            this.RunOnDB(context =>
            {
                int result = context.crSaveDocument(document.ChangeRequestId, document.DocumentTypeId, document.FileTitle, fileName, document.Comments, CurrentUser, Id, ErrorKey);
                if (Convert.ToInt32(Id.Value) <= 0 || !string.IsNullOrEmpty(Convert.ToString(ErrorKey.Value)))
                {
                    errMSg = Languages.GetResourceText("ChangeRequestSaveFail");
                }
            });
            if (string.IsNullOrEmpty(errMSg))
            {
                document.Id = Convert.ToInt32(Id.Value);
                successMsg = Languages.GetResourceText("ChangeRequestSavedSuccess");
            }
            return SuccessOrFailedResponse<int?>(errMSg, document.Id, successMsg);
        }
        /// <summary>
        /// Finds the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public NPE.Core.ITypedResponse<DTO.Library.APQP.ChangeRequest.Document> FindById(int id)
        {
            string errMSg = string.Empty;
            DTO.Library.APQP.ChangeRequest.Document objDocument = new DTO.Library.APQP.ChangeRequest.Document();
            this.RunOnDB(context =>
            {
                var documentItem = context.crGetDocumentById(id).SingleOrDefault();
                if (documentItem == null)
                    errMSg = Languages.GetResourceText("DocumentNotExists");
                else
                {
                    objDocument.Id = documentItem.Id;
                    objDocument.ChangeRequestId = documentItem.ChangeRequestId;
                    objDocument.DocumentTypeId = documentItem.DocumentTypeId;
                    objDocument.DocumentType = documentItem.DocumentType;
                    objDocument.IsConfidential = documentItem.IsConfidential;
                    objDocument.Comments = documentItem.Comments;
                    objDocument.FileTitle = documentItem.FileTitle;
                    objDocument.FilePath = !string.IsNullOrEmpty(documentItem.FilePath) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"]) +
                            Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + documentItem.FilePath : string.Empty;
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<DTO.Library.APQP.ChangeRequest.Document>(errMSg, objDocument);
            return response;
        }
        /// <summary>
        /// Deletes the specified document identifier.
        /// </summary>
        /// <param name="documentId">The document identifier.</param>
        /// <returns></returns>
        public NPE.Core.ITypedResponse<bool?> Delete(int documentId)
        {
            var DocumentToBeDeleted = this.DataContext.crDocuments.Where(a => a.Id == documentId).SingleOrDefault();
            if (DocumentToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("ChangeRequestDocumentNotExists"));
            }
            else
            {
                this.DataContext.crDocuments.Remove(DocumentToBeDeleted);
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("ChangeRequestDocumentDeletedSuccess"));
            }
        }
        /// <summary>
        /// Searches the specified search information.
        /// </summary>
        /// <param name="searchInfo">The search information.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.ChangeRequest.Document>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Gets the document list.
        /// </summary>
        /// <param name="changeRequestId">The change request identifier.</param>
        /// <returns></returns>
        public List<MES.DTO.Library.APQP.ChangeRequest.Document> GetDocumentList(int changeRequestId)
        {
            List<DTO.Library.APQP.ChangeRequest.Document> lstDocument = new List<DTO.Library.APQP.ChangeRequest.Document>();
            DTO.Library.APQP.ChangeRequest.Document document;
            this.RunOnDB(context =>
            {
                var DocumentList = context.crGetDocumentList(changeRequestId).ToList();
                if (DocumentList != null)
                {
                    foreach (var item in DocumentList)
                    {
                        document = new DTO.Library.APQP.ChangeRequest.Document();
                        document.Id = item.Id;
                        document.ChangeRequestId = item.ChangeRequestId;
                        document.DocumentTypeId = item.DocumentTypeId;
                        document.DocumentType = item.DocumentType;
                        document.IsConfidential = item.IsConfidential;
                        document.Comments = item.Comments;
                        document.FileTitle = item.FileTitle;
                        document.FilePath = !string.IsNullOrEmpty(item.FilePath) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"]) +
                                Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + item.FilePath : string.Empty;
                        document.DocumentTypeItem = new DTO.Library.Common.LookupFields
                        {
                            Id = item.DocumentTypeId,
                            Name = item.DocumentType
                        };
                        lstDocument.Add(document);
                    }
                }
            });
            return lstDocument;
        }
    }
}
