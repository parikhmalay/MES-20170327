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
using System.Data.Entity.Validation;
using System.IO;

namespace MES.Business.Library.BO.RFQ.Supplier
{
    class Suppliers : ContextBusinessBase, ISuppliersRepository
    {
        public Suppliers()
            : base("Suppliers")
        { }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.RFQ.Supplier.Suppliers suppliers)
        {
            string errMSg = null;
            string successMsg = null;
            var recordToBeUpdated = new MES.Data.Library.Supplier();
            // bool IsNewRecord = true;
            if (suppliers.Id > 0)
            {
                recordToBeUpdated = this.DataContext.Suppliers.Where(a => a.Id == suppliers.Id).SingleOrDefault();

                if (recordToBeUpdated == null)
                    errMSg = Languages.GetResourceText("SuppliersNotExists");
                else
                {
                    //IsNewRecord = false;
                    #region "Delete Suppliers from CommodityTypeSupplier for this supplier"
                    var deleteCommodityTypeSupplierList = this.DataContext.CommodityTypeSuppliers.Where(a => a.SupplierId == suppliers.Id).ToList();
                    foreach (var item in deleteCommodityTypeSupplierList)
                    {
                        this.DataContext.CommodityTypeSuppliers.Remove(item);
                    }
                    #endregion
                    #region "Delete Supplier Ids from CommoditySupplier"
                    var deleteCommoditySupplierList = this.DataContext.CommoditySuppliers.Where(a => a.SupplierId == suppliers.Id).ToList();
                    foreach (var item in deleteCommoditySupplierList)
                    {
                        this.DataContext.CommoditySuppliers.Remove(item);
                    }
                    #endregion
                    #region "Delete Contact(s) from Supplier.Contacts for this supplier"
                    var deleteContactsList = this.DataContext.Contacts1.Where(a => a.SupplierId == suppliers.Id).ToList();
                    foreach (var item in deleteContactsList)
                    {
                        this.DataContext.Contacts1.Remove(item);
                    }
                    #endregion
                    #region "Delete Documents from Supplier.Documents for this supplier"
                    var deleteDocumentsList = this.DataContext.Documents.Where(a => a.SupplierId == suppliers.Id).ToList();
                    foreach (var item in deleteDocumentsList)
                    {
                        this.DataContext.Documents.Remove(item);
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
                this.DataContext.Suppliers.Add(recordToBeUpdated);
            }
            if (string.IsNullOrEmpty(errMSg))
            {
                #region Save general details
                recordToBeUpdated.SupplierCode = suppliers.SupplierCode;
                recordToBeUpdated.Description = suppliers.Description;
                recordToBeUpdated.CompanyName = suppliers.CompanyName;
                recordToBeUpdated.Address1 = suppliers.Address1;
                recordToBeUpdated.Address2 = suppliers.Address2;
                recordToBeUpdated.City = suppliers.City;
                recordToBeUpdated.State = suppliers.State;
                recordToBeUpdated.CountryId = suppliers.CountryId;
                recordToBeUpdated.Zip = suppliers.Zip;
                recordToBeUpdated.Website = suppliers.Website;
                recordToBeUpdated.CompanyPhone1 = suppliers.CompanyPhone1;
                recordToBeUpdated.CompanyPhone2 = suppliers.CompanyPhone2;
                recordToBeUpdated.CompanyFax = suppliers.CompanyFax;
                recordToBeUpdated.Comments = suppliers.Comments;
                recordToBeUpdated.WorkQualityRating = suppliers.WorkQualityRating;
                recordToBeUpdated.TimelineRating = suppliers.TimelineRating;
                recordToBeUpdated.CostingRating = suppliers.CostingRating;
                recordToBeUpdated.SQId = suppliers.SQId ?? Guid.Empty;
                recordToBeUpdated.Status = suppliers.Status;
                recordToBeUpdated.IsCurrentSupplier = suppliers.IsCurrentSupplier;
                recordToBeUpdated.AssessmentDate = suppliers.AssessmentDate;
                recordToBeUpdated.Score = suppliers.Score;
                recordToBeUpdated.SQCertificationFilePath = suppliers.SQCertificationFilePath;
                recordToBeUpdated.AssessmentScore = suppliers.AssessmentScore;
                this.DataContext.SaveChanges();
                suppliers.Id = recordToBeUpdated.Id;
                #endregion


                #region "Save CommoditySuppliers Detail"
                MES.Data.Library.CommoditySupplier dboCommoditySupplier = null;
                if (suppliers.CommoditySuppliers != null && suppliers.CommoditySuppliers.Count > 0)
                {
                    bool AnyCommoditySupplier = false;
                    foreach (var commoditySupplier in suppliers.CommoditySuppliers)
                    {
                        if (Convert.ToInt32(commoditySupplier.Id) != 0)
                        {
                            AnyCommoditySupplier = true;
                            dboCommoditySupplier = new MES.Data.Library.CommoditySupplier();
                            dboCommoditySupplier.SupplierId = suppliers.Id;
                            dboCommoditySupplier.CommodityId = Convert.ToInt16(commoditySupplier.Id);
                            dboCommoditySupplier.CreatedBy = CurrentUser;
                            dboCommoditySupplier.CreatedDate = AuditUtils.GetCurrentDateTime();
                            this.DataContext.CommoditySuppliers.Add(dboCommoditySupplier);
                        }
                    }
                    if (AnyCommoditySupplier)
                        this.DataContext.SaveChanges();
                }
                #endregion

                #region "Save supplier contacts Detail"

                //if (IsNewRecord && suppliers.lstContact != null && suppliers.lstContact.Count > 0)
                if (suppliers.lstContact != null && suppliers.lstContact.Count > 0)
                {
                    MES.Business.Repositories.RFQ.Supplier.IContactsRepository objIContactsRepository = null;
                    foreach (var objContact in suppliers.lstContact)
                    {

                        objIContactsRepository = new MES.Business.Library.BO.RFQ.Supplier.Contacts();
                        objContact.SupplierId = suppliers.Id;
                        objContact.Id = 0;
                        objIContactsRepository.Save(objContact);

                    }
                }
                #endregion

                #region "Save supplier Documents Detail"
                if (suppliers.lstDocument != null && suppliers.lstDocument.Count > 0)
                {
                    MES.Business.Repositories.RFQ.Supplier.IDocumentsRepository objIDocumentsRepository = null;
                    foreach (var objDocument in suppliers.lstDocument)
                    {
                        if (objDocument.DocumentTypeId != 0)
                        {
                            objIDocumentsRepository = new MES.Business.Library.BO.RFQ.Supplier.Documents();
                            objDocument.SupplierId = suppliers.Id;
                            objIDocumentsRepository.Save(objDocument);
                        }
                    }
                }
                #endregion

                successMsg = Languages.GetResourceText("SuppliersSavedSuccess");
            }

            return SuccessOrFailedResponse<int?>(errMSg, suppliers.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.Supplier.Suppliers> FindById(int id)
        {
            string errMSg = string.Empty;

            DTO.Library.RFQ.Supplier.Suppliers suppliers = new DTO.Library.RFQ.Supplier.Suppliers();
            this.RunOnDB(context =>
            {
                var Suppliers = context.Suppliers.Where(s => s.Id == id).SingleOrDefault();
                if (Suppliers == null)
                    errMSg = Languages.GetResourceText("SuppliersNotExists");
                else
                {
                    string countryName = string.Empty;
                    if (Suppliers.CountryId.HasValue)
                    {
                        var country = context.Countries.Where(s => s.Id == Suppliers.CountryId.Value).SingleOrDefault();
                        countryName = country.Value;
                    }
                    #region general details
                    suppliers.Id = Suppliers.Id;
                    suppliers.SupplierCode = Suppliers.SupplierCode;
                    suppliers.Description = Suppliers.Description;
                    suppliers.CompanyName = Suppliers.CompanyName;
                    suppliers.Address1 = Suppliers.Address1;
                    suppliers.Address2 = Suppliers.Address2;
                    suppliers.City = Suppliers.City;
                    suppliers.State = Suppliers.State;
                    suppliers.CountryId = Suppliers.CountryId;
                    suppliers.Country = countryName;
                    suppliers.Zip = Suppliers.Zip;
                    suppliers.Website = Suppliers.Website;
                    suppliers.CompanyPhone1 = Suppliers.CompanyPhone1;
                    suppliers.CompanyPhone2 = Suppliers.CompanyPhone2;
                    suppliers.CompanyFax = Suppliers.CompanyFax;
                    suppliers.Comments = Suppliers.Comments;
                    suppliers.WorkQualityRating = Suppliers.WorkQualityRating;
                    suppliers.TimelineRating = Suppliers.TimelineRating;
                    suppliers.CostingRating = Suppliers.CostingRating;
                    suppliers.SQId = Suppliers.SQId;
                    suppliers.Status = Suppliers.Status;
                    suppliers.IsCurrentSupplier = Suppliers.IsCurrentSupplier;
                    suppliers.AssessmentDate = Suppliers.AssessmentDate;
                    suppliers.Score = Suppliers.Score;
                    suppliers.SQCertificationFilePath = Suppliers.SQCertificationFilePath;
                    suppliers.AssessmentScore = Suppliers.AssessmentScore;
                    #endregion

                    #region Bind Commodity
                    suppliers.CommoditySuppliers = new List<DTO.Library.Common.LookupFields>();
                    context.CommoditySuppliers.Where(a => a.SupplierId == id).ToList().ForEach(tl => suppliers.CommoditySuppliers.Add(
                        new DTO.Library.Common.LookupFields()
                        {
                            Id = Convert.ToInt32(tl.CommodityId),
                            Name = tl.Commodity.CommodityName
                        }));
                    #endregion
                    #region Bind contact details
                    MES.Business.Repositories.RFQ.Supplier.IContactsRepository objIContactsRepository = new MES.Business.Library.BO.RFQ.Supplier.Contacts();
                    suppliers.lstContact = objIContactsRepository.GetContactsList(id);
                    #endregion
                    #region Bind document details
                    MES.Business.Repositories.RFQ.Supplier.IDocumentsRepository objIDocumentsRepository = new MES.Business.Library.BO.RFQ.Supplier.Documents();
                    suppliers.lstDocument = objIDocumentsRepository.GetDocumentsList(id);
                    #endregion

                    #region Bind assessment details
                    MES.Business.Repositories.RFQ.Supplier.ISupplierAssessmentRepository objIAssessmentRepository = new MES.Business.Library.BO.RFQ.Supplier.SupplierAssessment();
                    suppliers.lstAssessment = objIAssessmentRepository.GetSupplierAssessmentList(id);
                    #endregion
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.RFQ.Supplier.Suppliers>(errMSg, suppliers);
            return response;
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int suppliersId)
        {
            return DeleteMultiple(Convert.ToString(suppliersId));

            //var SuppliersToBeDeleted = this.DataContext.Suppliers.Where(a => a.Id == suppliersId).SingleOrDefault();
            //if (SuppliersToBeDeleted == null)
            //{
            //    return FailedBoolResponse(Languages.GetResourceText("SuppliersNotExists"));
            //}
            //else
            //{
            //    SuppliersToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
            //    SuppliersToBeDeleted.UpdatedBy = CurrentUser;
            //    this.DataContext.Entry(SuppliersToBeDeleted).State = EntityState.Modified;
            //    SuppliersToBeDeleted.IsDeleted = true;
            //    this.DataContext.SaveChanges();
            //    return SuccessBoolResponse(Languages.GetResourceText("SuppliersDeletedSuccess"));
            //}
        }

        public NPE.Core.ITypedResponse<bool?> DeleteMultiple(string SupplierIds)
        {
            //set the out put param
            ObjectParameter Result = new ObjectParameter("Result", 0);
            this.RunOnDB(context =>
            {
                context.DeleteMultipleSupplier(SupplierIds, CurrentUser, Result);
            });
            if (Convert.ToInt32(Result.Value) > 0)
                return SuccessBoolResponse(Languages.GetResourceText("SuppliersDeletedSuccess"));
            else
                return FailedBoolResponse(Languages.GetResourceText("SupplierDeleteFail"));
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.Supplier.Suppliers>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.Supplier.Suppliers>> GetSuppliersList(NPE.Core.IPage<DTO.Library.RFQ.Supplier.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.RFQ.Supplier.Suppliers> lstSuppliers = new List<DTO.Library.RFQ.Supplier.Suppliers>();
            DTO.Library.RFQ.Supplier.Suppliers suppliers;
            this.RunOnDB(context =>
             {
                 var SuppliersList = context.SearchSuppliers(paging.Criteria.CompanyName,
                     paging.Criteria.City,
                     paging.Criteria.State,
                     paging.Criteria.Website,
                     paging.Criteria.CompanyPhone1,
                     paging.Criteria.Status,
                     paging.Criteria.CommodityIds,
                     paging.Criteria.WorkQualityRating,
                     paging.Criteria.TimelineRating,
                     paging.Criteria.CostingRating,
                     paging.Criteria.IsCurrentSupplier,
                     paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                 if (SuppliersList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                     foreach (var item in SuppliersList)
                     {
                         suppliers = new DTO.Library.RFQ.Supplier.Suppliers();
                         suppliers.Id = item.Id;
                         suppliers.CompanyName = item.CompanyName;
                         suppliers.SQId = item.SQId;
                         suppliers.SupplierQuality = item.SupplierQuality;
                         suppliers.City = item.City;
                         suppliers.State = item.State;
                         suppliers.Email = item.Email;
                         suppliers.Website = item.Website;
                         suppliers.CompanyPhone1 = item.CompanyPhone1;
                         suppliers.IsCurrentSupplierText = (item.IsCurrentSupplier.HasValue && item.IsCurrentSupplier.Value) ? "Yes" : "No";
                         lstSuppliers.Add(suppliers);
                     }
                 }
             });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.Supplier.Suppliers>>(errMSg, lstSuppliers);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }

        public NPE.Core.ITypedResponse<bool?> SendEmail(MES.DTO.Library.Common.EmailData emailData)
        {
            bool IsSuccess = false;
            try
            {
                MES.Business.Repositories.Email.IEmailRepository emailRepository = new MES.Business.Library.BO.Email.Email();
                List<string> lstToAddress = new List<string>();
                foreach (var item in emailData.EmailIdsList)
                {
                    lstToAddress.Add(item);

                }

                List<System.Net.Mail.Attachment> Attachments = new List<System.Net.Mail.Attachment>();
                if (!string.IsNullOrEmpty(emailData.EmailAttachment))
                {
                    Stream memoryStream = MES.Business.Library.Helper.BlobHelper.AttachFile_ConvertBlobToStream(emailData.EmailAttachment);
                    Attachments.Add(new System.Net.Mail.Attachment(memoryStream, emailData.EmailFileName));
                }
                emailRepository.SendEmail(lstToAddress, "", emailData.EmailSubject, emailData.EmailBody, out IsSuccess, Attachments, null);
            }
            catch (Exception ex)
            {
            }
            if (IsSuccess)
                return SuccessBoolResponse(Languages.GetResourceText("SendEmailSuccess"));
            else
                return SuccessBoolResponse(Languages.GetResourceText("SendEmailFail"));
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.Supplier.Suppliers> FindByCode(string supplierCode)
        {
            string errMSg = string.Empty;

            DTO.Library.RFQ.Supplier.Suppliers supplier = new DTO.Library.RFQ.Supplier.Suppliers();
            this.RunOnDB(context =>
            {
                var SupplierItem = context.GetSupplierDetailsByCode(supplierCode).FirstOrDefault();
                if (SupplierItem == null)
                    errMSg = Languages.GetResourceText("SuppliersNotExists");
                else
                {
                    #region general details
                    supplier.Id = SupplierItem.Id;
                    supplier.SupplierCode = supplierCode;
                    supplier.CompanyName = SupplierItem.CompanyName;
                    supplier.SCEmail = SupplierItem.SCEmail;
                    supplier.SCName = SupplierItem.SCName;
                    supplier.SCPhone = SupplierItem.SCPhone;
                    supplier.SQEmail = SupplierItem.SQEmail;
                    supplier.SQName = SupplierItem.SQName;
                    #endregion


                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.RFQ.Supplier.Suppliers>(errMSg, supplier);
            return response;
        }
    }
}
