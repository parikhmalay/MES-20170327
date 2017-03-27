using Account.DTO.Library;
using MES.Business.Repositories.APQP.CAPA;
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

namespace MES.Business.Library.BO.APQP.CAPA
{
    class capaPartDocument : ContextBusinessBase, IPartDocumentRepository
    {
        public capaPartDocument() : base("capaPartDocument") { }
        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.APQP.CAPA.capaPartDocument partDocument)
        {
            string errMSg = null;
            string successMsg = null;
            string fileName = string.Empty;
            var recordToBeUpdated = new MES.Data.Library.capaPartDocument();
            if (partDocument.Id > 0)
            {
                recordToBeUpdated = this.DataContext.capaPartDocuments.Where(a => a.Id == partDocument.Id).SingleOrDefault();

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
                this.DataContext.capaPartDocuments.Add(recordToBeUpdated);
            }
            if (string.IsNullOrEmpty(errMSg))
            {
                recordToBeUpdated.PartAffectedDetailsId = partDocument.PartAffectedDetailsId;
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
        public NPE.Core.ITypedResponse<DTO.Library.APQP.CAPA.capaPartDocument> FindById(int id)
        {
            throw new NotImplementedException();
        }
        public NPE.Core.ITypedResponse<bool?> Delete(int partDocumentId)
        {
            var PartDocumentToBeDeleted = this.DataContext.capaPartDocuments.Where(a => a.Id == partDocumentId).SingleOrDefault();
            if (PartDocumentToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("APQPDocumentNotExists"));
            }
            else
            {
                this.DataContext.capaPartDocuments.Remove(PartDocumentToBeDeleted);
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("APQPDocumentDeletedSuccess"));
            }
        }
        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.CAPA.capaPartDocument>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }
        public List<MES.DTO.Library.APQP.CAPA.capaPartDocument> GetPartDocumentList(int cAPAPartAffectedDetailId, int AssociatedToId)
        {
            List<DTO.Library.APQP.CAPA.capaPartDocument> lstPartDocument = new List<DTO.Library.APQP.CAPA.capaPartDocument>();
            DTO.Library.APQP.CAPA.capaPartDocument partDocument;
            this.RunOnDB(context =>
            {
                var PartDocumentList = context.capaGetPartDocuments(cAPAPartAffectedDetailId, AssociatedToId).ToList();
                if (PartDocumentList != null)
                {
                    foreach (var item in PartDocumentList)
                    {
                        partDocument = new DTO.Library.APQP.CAPA.capaPartDocument();
                        partDocument.Id = item.Id;
                        partDocument.PartAffectedDetailsId = item.PartAffectedDetailsId;
                        partDocument.DocumentTypeId = item.DocumentTypeId;
                        partDocument.DocumentType = item.DocumentType + " - " + item.AssociatedTo;
                        partDocument.IsConfidential = item.IsConfidential;
                        partDocument.FileTitle = item.FileTitle;
                        partDocument.FilePath = !string.IsNullOrEmpty(item.FilePath) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"]) +
                                Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + item.FilePath : string.Empty;
                        partDocument.Comments = item.Comments;
                        partDocument.RevLevel = item.RevLevel;
                        partDocument.AssociatedToId = item.AssociatedToId;
                        partDocument.DocumentTypeItem = new DTO.Library.Common.LookupFields();
                        partDocument.DocumentTypeItem.Id = partDocument.DocumentTypeId;
                        partDocument.DocumentTypeItem.Name = partDocument.DocumentType;
                        partDocument.DocumentTypeItem.ParentId = partDocument.AssociatedToId;
                        partDocument.DocumentTypeItem.Code = "";
                        lstPartDocument.Add(partDocument);
                    }
                }
            });
            return lstPartDocument;
        }
    }
}
