using Account.DTO.Library;
using MES.Business.Repositories.APQP.DefectTracking;
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

namespace MES.Business.Library.BO.APQP.DefectTracking
{
    class PartDocument : ContextBusinessBase, IPartDocumentRepository
    {
        public PartDocument() : base("PartDocument") { }
        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.APQP.DefectTracking.PartDocument partDocument)
        {
            string errMSg = null;
            string successMsg = null;
            string fileName = string.Empty;
            var recordToBeUpdated = new MES.Data.Library.dtPartDocument();
            if (partDocument.Id > 0)
            {
                recordToBeUpdated = this.DataContext.dtPartDocuments.Where(a => a.Id == partDocument.Id).SingleOrDefault();

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
                this.DataContext.dtPartDocuments.Add(recordToBeUpdated);
            }
            if (string.IsNullOrEmpty(errMSg))
            {
                recordToBeUpdated.DefectTrackingDetailId = partDocument.DefectTrackingDetailId;
                recordToBeUpdated.DocumentTypeId = partDocument.DocumentTypeId;
                recordToBeUpdated.FileTitle = partDocument.FileTitle;
                if (!string.IsNullOrEmpty(partDocument.FilePath))
                    fileName = MES.Business.Library.Helper.BlobHelper.GetPhysicalPath(partDocument.FilePath);
                recordToBeUpdated.FilePath = fileName;
                recordToBeUpdated.Comments = partDocument.Comments;
                recordToBeUpdated.RevLevel = partDocument.RevLevel;
                recordToBeUpdated.AssociatedToId = partDocument.AssociatedToId;
                this.DataContext.SaveChanges();
                partDocument.Id = recordToBeUpdated.Id;
                successMsg = Languages.GetResourceText("APQPDocumentSavedSuccess");
            }
            return SuccessOrFailedResponse<int?>(errMSg, partDocument.Id, successMsg);
        }
        public NPE.Core.ITypedResponse<DTO.Library.APQP.DefectTracking.PartDocument> FindById(int id)
        {
            string errMSg = string.Empty;
            DTO.Library.APQP.DefectTracking.PartDocument objPartDocument = new DTO.Library.APQP.DefectTracking.PartDocument();
            this.RunOnDB(context =>
            {
                var partDocumentItem = context.dtGetPartDocumentById(id).SingleOrDefault();
                if (partDocumentItem == null)
                    errMSg = Languages.GetResourceText("APQPDocumentNotExists");
                else
                {
                    objPartDocument.Id = partDocumentItem.Id;
                    objPartDocument.DefectTrackingDetailId = partDocumentItem.DefectTrackingDetailId;
                    objPartDocument.DocumentTypeId = partDocumentItem.DocumentTypeId;
                    objPartDocument.DocumentType = partDocumentItem.DocumentType;
                    objPartDocument.IsConfidential = partDocumentItem.IsConfidential;
                    objPartDocument.FileTitle = partDocumentItem.FileTitle;
                    objPartDocument.FilePath = !string.IsNullOrEmpty(partDocumentItem.FilePath) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"]) +
                            Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + partDocumentItem.FilePath : string.Empty;
                    objPartDocument.Comments = partDocumentItem.Comments;
                    objPartDocument.RevLevel = partDocumentItem.RevLevel;
                    objPartDocument.AssociatedToId = partDocumentItem.AssociatedToId;
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<DTO.Library.APQP.DefectTracking.PartDocument>(errMSg, objPartDocument);
            return response;
        }
        public NPE.Core.ITypedResponse<bool?> Delete(int partDocumentId)
        {
            var PartDocumentToBeDeleted = this.DataContext.dtPartDocuments.Where(a => a.Id == partDocumentId).SingleOrDefault();
            if (PartDocumentToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("APQPDocumentNotExists"));
            }
            else
            {
                this.DataContext.dtPartDocuments.Remove(PartDocumentToBeDeleted);
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("APQPDocumentDeletedSuccess"));
            }
        }
        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.DefectTracking.PartDocument>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }
        public List<MES.DTO.Library.APQP.DefectTracking.PartDocument> GetPartDocumentList(int defectTrackingDetailId, int AssociatedToId)
        {
            List<DTO.Library.APQP.DefectTracking.PartDocument> lstPartDocument = new List<DTO.Library.APQP.DefectTracking.PartDocument>();
            DTO.Library.APQP.DefectTracking.PartDocument partDocument;
            this.RunOnDB(context =>
            {
                var PartDocumentList = context.dtGetPartDocuments(defectTrackingDetailId, AssociatedToId).ToList();
                if (PartDocumentList != null)
                {
                    foreach (var item in PartDocumentList)
                    {
                        partDocument = new DTO.Library.APQP.DefectTracking.PartDocument();
                        partDocument.Id = item.Id;
                        partDocument.DefectTrackingDetailId = item.DefectTrackingDetailId;
                        partDocument.DocumentTypeId = item.DocumentTypeId;
                        partDocument.DocumentType = item.DocumentType + " - " + item.AssociatedTo;
                        partDocument.IsConfidential = item.IsConfidential;
                        partDocument.FileTitle = item.FileTitle;
                        partDocument.FilePath = !string.IsNullOrEmpty(item.FilePath) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"]) +
                                Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + item.FilePath : string.Empty;
                        partDocument.Comments = item.Comments;
                        partDocument.RevLevel = item.RevLevel;
                        partDocument.AssociatedToId = item.AssociatedToId;
                        lstPartDocument.Add(partDocument);
                    }
                }
            });
            return lstPartDocument;
        }
    }
}
