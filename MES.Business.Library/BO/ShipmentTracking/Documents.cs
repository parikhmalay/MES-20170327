using Account.DTO.Library;
using MES.Business.Repositories.ShipmentTracking;
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

namespace MES.Business.Library.BO.ShipmentTracking
{
    class Documents : ContextBusinessBase, IDocumentsRepository
    {
        public Documents()
            : base("Documents")
        { }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.ShipmentTracking.Documents documents)
        {
            string errMSg = null;
            string successMsg = null;
            string fileName = string.Empty;
            var recordToBeUpdated = new MES.Data.Library.Document1();
            recordToBeUpdated.CreatedBy = recordToBeUpdated.UpdatedBy = CurrentUser;
            recordToBeUpdated.CreatedDate = AuditUtils.GetCurrentDateTime();
            recordToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
            this.DataContext.Document1.Add(recordToBeUpdated);

            if (string.IsNullOrEmpty(errMSg))
            {
                recordToBeUpdated.ShipmentId = documents.ShipmentId;
                recordToBeUpdated.DocumentTypeId = documents.DocumentTypeId;

                if (!string.IsNullOrEmpty(documents.DocumentFilePath))
                {
                    fileName = MES.Business.Library.Helper.BlobHelper.GetPhysicalPath(documents.DocumentFilePath);
                }
                recordToBeUpdated.DocumentFileName = documents.DocumentFileName;
                recordToBeUpdated.DocumentFilePath = fileName;

                this.DataContext.SaveChanges();
                documents.Id = recordToBeUpdated.Id;
                successMsg = Languages.GetResourceText("DocumentsSavedSuccess");
            }
            return SuccessOrFailedResponse<int?>(errMSg, documents.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.ShipmentTracking.Documents> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int documentsId)
        {
            var DocumentsToBeDeleted = this.DataContext.Document1.Where(a => a.Id == documentsId).SingleOrDefault();
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

        public NPE.Core.ITypedResponse<List<DTO.Library.ShipmentTracking.Documents>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public List<DTO.Library.ShipmentTracking.Documents> GetDocumentsList(int shipmentId)
        {
            List<DTO.Library.ShipmentTracking.Documents> lstDocument = new List<DTO.Library.ShipmentTracking.Documents>();
            DTO.Library.ShipmentTracking.Documents documents;
            this.RunOnDB(context =>
            {
                var DocumentsList = context.Document1.Where(c => c.ShipmentId == shipmentId).OrderByDescending(a => a.CreatedDate).ToList();
                if (DocumentsList != null)
                {
                    foreach (var item in DocumentsList)
                    {
                        documents = new DTO.Library.ShipmentTracking.Documents();
                        documents.Id = item.Id;
                        documents.ShipmentId = item.ShipmentId;
                        documents.DocumentFilePath = !string.IsNullOrEmpty(item.DocumentFilePath) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"]) + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + item.DocumentFilePath : string.Empty;
                        documents.DocumentFileName = item.DocumentFileName;
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
