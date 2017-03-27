using Account.DTO.Library;
using MES.Business.Repositories.APQP.APQP;
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

namespace MES.Business.Library.BO.APQP.APQP
{
    class Document : ContextBusinessBase, IDocumentRepository
    {
        public Document()
            : base("Document")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.APQP.APQP.Document document)
        {
            string errMSg = null;
            string successMsg = null;
            string fileName = string.Empty;
            var recordToBeUpdated = new MES.Data.Library.apqpDocument();
            if (document.Id > 0)
            {
                recordToBeUpdated = this.DataContext.apqpDocuments.Where(a => a.Id == document.Id).SingleOrDefault();

                if (recordToBeUpdated == null)
                    errMSg = Languages.GetResourceText("APQPDocumentNotExists");
                else
                {
                    recordToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
                    recordToBeUpdated.UpdatedBy = CurrentUser;
                    this.DataContext.Entry(recordToBeUpdated).State = EntityState.Modified;
                }
            }
            else
            {
                recordToBeUpdated.CreatedBy = recordToBeUpdated.UpdatedBy = CurrentUser;
                recordToBeUpdated.CreatedDate = AuditUtils.GetCurrentDateTime();
                recordToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
                this.DataContext.apqpDocuments.Add(recordToBeUpdated);
            }
            if (string.IsNullOrEmpty(errMSg))
            {
                recordToBeUpdated.APQPItemId = document.APQPItemId;
                recordToBeUpdated.DocumentTypeId = document.DocumentTypeId;
                recordToBeUpdated.ReceivedDate = document.ReceivedDate;
                recordToBeUpdated.PreparedDate = document.PreparedDate;
                recordToBeUpdated.FileTitle = document.FileTitle;
                if (!string.IsNullOrEmpty(document.FilePath))
                    fileName = MES.Business.Library.Helper.BlobHelper.GetPhysicalPath(document.FilePath);
                recordToBeUpdated.FilePath = fileName;
                recordToBeUpdated.Comments = document.Comments;
                recordToBeUpdated.RevLevel = document.RevLevel;
                //recordToBeUpdated.crId = document.crId;
                recordToBeUpdated.crId = document.crId;
                recordToBeUpdated.UploadedFromStepId = document.UploadedFromStepId;
                this.DataContext.SaveChanges();
                document.Id = recordToBeUpdated.Id;
                successMsg = Languages.GetResourceText("APQPDocumentSavedSuccess");
            }
            return SuccessOrFailedResponse<int?>(errMSg, document.Id, successMsg);
        }
        public NPE.Core.ITypedResponse<DTO.Library.APQP.APQP.Document> FindById(int id)
        {
            string errMSg = string.Empty;
            DTO.Library.APQP.APQP.Document doc = new DTO.Library.APQP.APQP.Document();
            this.RunOnDB(context =>
            {
                var documentItem = context.apqpDocuments.Where(a => a.Id == id).SingleOrDefault();
                if (documentItem == null)
                    errMSg = Languages.GetResourceText("DocumentNotExists");
                else
                {   
                    doc.Id = documentItem.Id;
                    doc.FileTitle = documentItem.FileTitle;
                    doc.FilePath = !string.IsNullOrEmpty(documentItem.FilePath) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"]) +
                            Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + documentItem.FilePath : string.Empty;

                    doc.DocumentType = documentItem.DocumentType.DocumentType1;
                    doc.RevLevel = documentItem.RevLevel;
                    doc.CreatedDate = documentItem.CreatedDate;
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<DTO.Library.APQP.APQP.Document>(errMSg, doc);
            return response;
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int documentId)
        {
            var DocumentToBeDeleted = this.DataContext.apqpDocuments.Where(a => a.Id == documentId).SingleOrDefault();
            if (DocumentToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("APQPDocumentNotExists"));
            }
            else
            {
                this.DataContext.apqpDocuments.Remove(DocumentToBeDeleted);
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("APQPDocumentDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.APQP.Document>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }
    }
}
