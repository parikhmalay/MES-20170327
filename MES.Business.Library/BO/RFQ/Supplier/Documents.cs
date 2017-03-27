using Account.DTO.Library;
using MES.Business.Repositories.RFQ.Supplier;
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

namespace MES.Business.Library.BO.RFQ.Supplier
{
    class Documents : ContextBusinessBase, IDocumentsRepository
    {
        public Documents()
            : base("Documents")
        { }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.RFQ.Supplier.Documents documents)
        {
            string errMSg = null;
            string successMsg = null;
            string fileName = string.Empty;
            var recordToBeUpdated = new MES.Data.Library.Document();
            recordToBeUpdated.CreatedBy = recordToBeUpdated.UpdatedBy = CurrentUser;
            recordToBeUpdated.CreatedDate = AuditUtils.GetCurrentDateTime();
            recordToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
            this.DataContext.Documents.Add(recordToBeUpdated);

            if (string.IsNullOrEmpty(errMSg))
            {
                recordToBeUpdated.SupplierId = documents.SupplierId;
                recordToBeUpdated.DocumentTypeId = documents.DocumentTypeId;
                if (!string.IsNullOrEmpty(documents.DocumentFilePath))
                    fileName = MES.Business.Library.Helper.BlobHelper.GetPhysicalPath(documents.DocumentFilePath);
                recordToBeUpdated.DocumentFilePath = fileName;
                recordToBeUpdated.DocumentFileName = documents.DocumentFileName.Replace(":", string.Empty)
                                                    .Replace("\\", string.Empty)
                                                    .Replace("/", string.Empty)
                                                    .Replace("|", string.Empty)
                                                    .Replace("*", string.Empty)
                                                    .Replace("<", string.Empty)
                                                    .Replace("?", string.Empty)
                                                    .Replace("\"", string.Empty)
                                                    .Replace(">", string.Empty) ?? "";
                recordToBeUpdated.Comment = documents.Comment;
                recordToBeUpdated.ExpirationDate = documents.ExpirationDate;
                this.DataContext.SaveChanges();
                documents.Id = recordToBeUpdated.Id;
                successMsg = Languages.GetResourceText("DocumentsSavedSuccess");
            }

            return SuccessOrFailedResponse<int?>(errMSg, documents.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.Supplier.Documents> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int documentsId)
        {
            var DocumentsToBeDeleted = this.DataContext.Documents.Where(a => a.Id == documentsId).SingleOrDefault();
            if (DocumentsToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("DocumentsNotExists"));
            }
            else
            {
                DocumentsToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                DocumentsToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(DocumentsToBeDeleted).State = EntityState.Modified;
                //DocumentsToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("DocumentsDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.Supplier.Documents>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public List<DTO.Library.RFQ.Supplier.Documents> GetDocumentsList(int supplierId)
        {
            List<DTO.Library.RFQ.Supplier.Documents> lstDocument = new List<DTO.Library.RFQ.Supplier.Documents>();
            DTO.Library.RFQ.Supplier.Documents documents;
            this.RunOnDB(context =>
            {
                var DocumentsList = context.Documents.Where(c => c.SupplierId == supplierId).OrderByDescending(a => a.CreatedDate).ToList();
                if (DocumentsList != null)
                {
                    foreach (var item in DocumentsList)
                    {
                        documents = new DTO.Library.RFQ.Supplier.Documents();
                        documents.Id = item.Id;
                        documents.SupplierId = item.SupplierId;
                        documents.DocumentFilePath = !string.IsNullOrEmpty(item.DocumentFilePath)
                                                    ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"]) + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + item.DocumentFilePath
                                                    : item.DocumentFilePath;
                        documents.DocumentFileName = item.DocumentFileName;
                        documents.Comment = item.Comment;
                        documents.ExpirationDate = item.ExpirationDate;
                        documents.DocumentTypeId = item.DocumentTypeId;
                        documents.DocumentTypeItem = new DTO.Library.Common.LookupFields
                        {
                            Id = item.DocumentTypeId,
                            Name = item.DocumentType.DocumentType1
                        };
                        lstDocument.Add(documents);
                    }
                }
            });
            return lstDocument;
        }
    }
}
