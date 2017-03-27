using Account.DTO.Library;
using MES.Business.Repositories.Setup.DocumentType;
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

namespace MES.Business.Library.BO.Setup.DocumentType
{
    class DocumentType : ContextBusinessBase, IDocumentTypeRepository
    {
        public DocumentType()
            : base("DocumentType")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.Setup.DocumentType.DocumentType documentType)
        {
            string errMSg = null;
            string successMsg = null;
            try
            {
                //check for the uniqueness
                if (this.DataContext.DocumentTypes.AsNoTracking().Any(a => a.DocumentType1 == documentType.documentType && a.IsDeleted == false && a.Id != documentType.Id))
                {
                    errMSg = Languages.GetResourceText("DocumentTypeExists");
                }
                else
                {
                    var recordToBeUpdated = new MES.Data.Library.DocumentType();

                    if (documentType.Id > 0)
                    {
                        recordToBeUpdated = this.DataContext.DocumentTypes.Where(a => a.Id == documentType.Id).SingleOrDefault();

                        if (recordToBeUpdated == null)
                            errMSg = Languages.GetResourceText("DocumentTypeNotExists");
                        else
                        {
                            #region "Delete Document associated to Details"
                            var deleteDocumentTypeAssociatedToList = this.DataContext.DocumentTypeAssociatedToes.Where(a => a.DocumentTypeId == documentType.Id).ToList();
                            foreach (var item in deleteDocumentTypeAssociatedToList)
                            {
                                this.DataContext.DocumentTypeAssociatedToes.Remove(item);
                            }
                            #endregion

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
                        this.DataContext.DocumentTypes.Add(recordToBeUpdated);
                    }
                    if (string.IsNullOrEmpty(errMSg))
                    {
                        recordToBeUpdated.DocumentType1 = documentType.documentType;
                        recordToBeUpdated.IsConfidential = documentType.IsConfidential;
                        this.DataContext.SaveChanges();
                        documentType.Id = recordToBeUpdated.Id;

                        #region "Save Document associated to Details"
                        MES.Data.Library.DocumentTypeAssociatedTo dboDocumentTypeAssociatedTo = null;
                        if (documentType.DocumentTypeAssociatedToList != null && documentType.DocumentTypeAssociatedToList.Count > 0)
                        {
                            bool AnyDocumentTypeAssociatedTo = false;
                            foreach (var documentTypeAssociatedTo in documentType.DocumentTypeAssociatedToList)
                            {
                                if (documentTypeAssociatedTo.Id != 0)
                                {
                                    AnyDocumentTypeAssociatedTo = true;
                                    dboDocumentTypeAssociatedTo = new MES.Data.Library.DocumentTypeAssociatedTo();
                                    dboDocumentTypeAssociatedTo.DocumentTypeId = documentType.Id;
                                    dboDocumentTypeAssociatedTo.AssociatedToId = documentTypeAssociatedTo.Id;
                                    dboDocumentTypeAssociatedTo.CreatedBy = CurrentUser;
                                    dboDocumentTypeAssociatedTo.CreatedDate = AuditUtils.GetCurrentDateTime();
                                    this.DataContext.DocumentTypeAssociatedToes.Add(dboDocumentTypeAssociatedTo);
                                }
                            }
                            if (AnyDocumentTypeAssociatedTo)
                                this.DataContext.SaveChanges();
                        }
                        #endregion

                        successMsg = Languages.GetResourceText("DocumentTypeSavedSuccess");
                    }
                }
                return SuccessOrFailedResponse<int?>(errMSg, documentType.Id, successMsg);
            }
            catch (Exception ex)
            {
                errMSg = ex.Message.ToString();
                return SuccessOrFailedResponse<int?>(errMSg, documentType.Id, successMsg);
            }
        }

        public NPE.Core.ITypedResponse<DTO.Library.Setup.DocumentType.DocumentType> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int documentTypeId)
        {
            var documentTypeToBeDeleted = this.DataContext.DocumentTypes.Where(a => a.Id == documentTypeId).SingleOrDefault();
            if (documentTypeToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("DocumentTypeNotExists"));
            }
            else
            {
                documentTypeToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                documentTypeToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(documentTypeToBeDeleted).State = EntityState.Modified;
                documentTypeToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("DocumentTypeDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.DocumentType.DocumentType>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.DocumentType.DocumentType>> GetDocumentTypesList(NPE.Core.IPage<DTO.Library.Setup.DocumentType.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.Setup.DocumentType.DocumentType> lstdocumentType = new List<DTO.Library.Setup.DocumentType.DocumentType>();
            DTO.Library.Setup.DocumentType.DocumentType documentType;
            this.RunOnDB(context =>
             {
                 var documentTypeList = context.SearchDocumentType(paging.Criteria.documentType, paging.Criteria.IsConfidential, paging.Criteria.AssociatedToId, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                 if (documentTypeList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                     foreach (var item in documentTypeList)
                     {
                         documentType = new DTO.Library.Setup.DocumentType.DocumentType();
                         documentType.Id = item.Id;
                         documentType.documentType = item.DocumentType;
                         documentType.IsConfidential = item.IsConfidential;
                         documentType.DocumentTypeAssociatedToList = new List<DTO.Library.Setup.DocumentType.DocumentTypeAssociatedTo>();
                         context.DocumentTypeAssociatedToes.Where(a => a.DocumentTypeId == item.Id).ToList().ForEach(tl => documentType.DocumentTypeAssociatedToList.Add(
                             new DTO.Library.Setup.DocumentType.DocumentTypeAssociatedTo()
                             {
                                 Id = Convert.ToInt32(tl.AssociatedToId),
                                 Name = tl.AssociatedTo.AssociatedTo1,
                                 DocumentTypeId = Convert.ToInt32(tl.DocumentTypeId),
                                 AssociatedToId = Convert.ToInt32(tl.AssociatedToId),
                                 AssociatedTo = tl.AssociatedTo.AssociatedTo1,
                                 Description = tl.AssociatedTo.Description
                             }));
                         lstdocumentType.Add(documentType);
                     }
                 }
             });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Setup.DocumentType.DocumentType>>(errMSg, lstdocumentType);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }
    }
}
