using Account.DTO.Library;
using MES.Business.Repositories.RFQ.RFQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NPE.Core.Extensions;
using NPE.Core;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.IO;
using MES.Business.Library.Extensions;
using GemBox.Spreadsheet;
using System.Drawing;
using System.Web;
using System.Text.RegularExpressions;

namespace MES.Business.Library.BO.RFQ.RFQ
{
    class RFQSupplierPartQuote : ContextBusinessBase, IRFQSupplierPartQuoteRepository
    {
        public RFQSupplierPartQuote()
            : base("RFQSupplierPartQuote")
        { }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuote>> GetRfqSupplierPartsQuote(string rfqId)
        {
            string errMSg = string.Empty;

            List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuote> lstrFQSuppliersPartQuote = new List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuote>();
            DTO.Library.RFQ.RFQ.RFQSupplierPartQuote rfqSPQ = new DTO.Library.RFQ.RFQ.RFQSupplierPartQuote();
            this.RunOnDB(context =>
            {
                var rFQSPQList = context.GetRfqSupplierPartsQuote(rfqId).ToList();
                if (rFQSPQList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    foreach (var item in rFQSPQList)
                    {
                        if (!string.IsNullOrEmpty(item.QuoteAttachmentFilePath))
                            rfqSPQ.QuoteAttachmentFilePath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"]) + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + item.QuoteAttachmentFilePath;
                        else
                            rfqSPQ.QuoteAttachmentFilePath = string.Empty;
                        rfqSPQ.Id = item.RFQSupplirePartQuoteID;
                        lstrFQSuppliersPartQuote.Add(rfqSPQ);
                    }
                }
            });

            //get hold of response
            var response = SuccessOrFailedResponse<List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuote>>(errMSg, lstrFQSuppliersPartQuote);
            return response;
        }

        #region "Submit Simplified quote"
        public NPE.Core.ITypedResponse<bool?> SaveSubmitQuote(List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> RFQSupplierPartQuoteList)
        {
            if (RFQSupplierPartQuoteList != null && RFQSupplierPartQuoteList.Count > 0)
            {
                string fileName = string.Empty;
                string UniqueURL = string.Empty;
                bool isFileProcessed = false;
                DateTime dtTime = AuditUtils.GetCurrentDateTime();
                foreach (var rFQSupplierPartQuote in RFQSupplierPartQuoteList)
                {
                    UniqueURL = RFQSupplierPartQuoteList[0].UniqueUrl;
                    rFQSupplierPartQuote.Remarks = RFQSupplierPartQuoteList[0].Remarks;
                    rFQSupplierPartQuote.ExchangeRate = RFQSupplierPartQuoteList[0].ExchangeRate;
                    rFQSupplierPartQuote.Currency = RFQSupplierPartQuoteList[0].Currency;
                    rFQSupplierPartQuote.RawMaterialPriceAssumed = RFQSupplierPartQuoteList[0].RawMaterialPriceAssumed;

                    if (!isFileProcessed)
                    {
                        isFileProcessed = true;
                        if (!string.IsNullOrEmpty(RFQSupplierPartQuoteList[0].QuoteAttachmentFilePath))
                            fileName = MES.Business.Library.Helper.BlobHelper.GetPhysicalPath(RFQSupplierPartQuoteList[0].QuoteAttachmentFilePath);
                    }

                    rFQSupplierPartQuote.QuoteAttachmentFilePath = fileName;


                    rFQSupplierPartQuote.CreatedDate = rFQSupplierPartQuote.UpdatedDate = dtTime;
                    var obj = Save(rFQSupplierPartQuote);
                    if (obj == null || obj.StatusCode != 200)
                    {
                        return FailedBoolResponse(Languages.GetResourceText("SupplierpartQuoteError"));
                    }
                }
                Data.Library.rfqSupplier rfqSupplier = null;
                /////update quote date
                this.RunOnDB(context =>
                {
                    rfqSupplier = context.rfqSuppliers.Where(a => a.OriginalURL.ToUpper() == UniqueURL.ToUpper() && a.IsDeleted == false).SingleOrDefault();
                    if (rfqSupplier != null)
                    {
                        rfqSupplier.UniqueURL = Convert.ToString(Guid.NewGuid());
                        if (!rfqSupplier.QuoteDate.HasValue)
                            rfqSupplier.QuoteDate = AuditUtils.GetCurrentDateTime();
                        this.DataContext.Entry(rfqSupplier).State = EntityState.Modified;
                        this.DataContext.SaveChanges();

                    }
                });
                if (rfqSupplier != null)
                {
                    DTO.Library.RFQ.RFQ.RFQSuppliers rsItem = new DTO.Library.RFQ.RFQ.RFQSuppliers();
                    rsItem.SupplierId = rfqSupplier.SupplierId;
                    rsItem.RFQId = rfqSupplier.RFQId;
                    rsItem.Id = rfqSupplier.Id;

                    //Send notification via email to the Supplier and admin here
                    SendEmail('A', rsItem);
                    SendEmail('S', rsItem);
                }
            }
            return SuccessBoolResponse(Languages.GetResourceText("SupplierPartQuoteSavedSuccess"));
        }

        private bool SendEmail(char user, DTO.Library.RFQ.RFQ.RFQSuppliers rfqSupplier)
        {
            bool IsSuccess = false;
            Supplier.Suppliers supObj = new Supplier.Suppliers();
            DTO.Library.RFQ.Supplier.Suppliers supplierItem = supObj.FindById(rfqSupplier.SupplierId).Result;
            DTO.Library.RFQ.Supplier.Contacts contactItem = new DTO.Library.RFQ.Supplier.Contacts();
            if (supplierItem.lstContact.Count > 0)
            {
                contactItem = supplierItem.lstContact.Where(item => item.IsDefault == true).First();

                DTO.Library.Common.EmailData emailData = new DTO.Library.Common.EmailData();
                emailData.EmailBody = string.Format(GetRfqEmailBody(user), supplierItem.CompanyName, rfqSupplier.RFQId);

                MES.Business.Repositories.Email.IEmailRepository emailRepository = new MES.Business.Library.BO.Email.Email();
                List<string> lstToAddress = new List<string>();

                if (user == 'A')
                {
                    emailData.EmailSubject = Languages.GetResourceText("submitQuoteEmailAdminSubj") + rfqSupplier.RFQId;
                    lstToAddress.Add(System.Configuration.ConfigurationManager.AppSettings["RfqEmailReceiver"]);
                }
                else
                {
                    emailData.EmailSubject = Languages.GetResourceText("submitQuoteEmailSupplierSubj") + rfqSupplier.RFQId;
                    lstToAddress.Add(contactItem.Email);
                }

                emailRepository.SendEmail(lstToAddress, "", emailData.EmailSubject, emailData.EmailBody, out IsSuccess, null);
            }
            return IsSuccess;
        }

        /// <summary>
        /// Gets the RFQ email body.
        /// </summary>
        /// <returns></returns>
        private string GetRfqEmailBody(char user)
        {
            string templatePath = string.Empty;
            if (user == 'A')
                templatePath = System.Web.HttpContext.Current.Server.MapPath("~/") + Constants.EmailTemplateFolder + "SubmitQuoteAdmin.txt";
            else
                templatePath = System.Web.HttpContext.Current.Server.MapPath("~/") + Constants.EmailTemplateFolder + "SubmitQuoteSupplier.txt";

            string emailBody = string.Empty;
            using (StreamReader reader = new StreamReader(templatePath))
            {
                emailBody = reader.ReadToEnd();
                reader.Close();
            }
            return emailBody;

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ rFQSupplierPartQuote)
        {
            string errMSg = null;
            string successMsg = null;
            ObjectParameter rFQSupplierPartQuoteId = new ObjectParameter("rFQSupplierPartQuoteId", 0);
            this.RunOnDB(context =>
            {
                int result = context.InsertRFQSupplierPartQuote(rFQSupplierPartQuote.RFQSupplierId, rFQSupplierPartQuote.RFQPartId,
                        rFQSupplierPartQuote.ManufacturerId, rFQSupplierPartQuote.MaterialCost, rFQSupplierPartQuote.ProcessCost,
                        rFQSupplierPartQuote.UnitPrice, rFQSupplierPartQuote.ToolingCost, rFQSupplierPartQuote.Currency, rFQSupplierPartQuote.RawMaterialPriceAssumed,
                        rFQSupplierPartQuote.QuoteAttachmentFilePath, rFQSupplierPartQuote.MachiningCost, rFQSupplierPartQuote.OtherProcessCost,
                        rFQSupplierPartQuote.Remarks, rFQSupplierPartQuote.SupplierToolingLeadtime, rFQSupplierPartQuote.ToolingWarranty,
                        rFQSupplierPartQuote.NoOfCavities, rFQSupplierPartQuote.MinOrderQty, rFQSupplierPartQuote.MOQConfirmation,
                        rFQSupplierPartQuote.Manufacturer, rFQSupplierPartQuote.ExchangeRate, rFQSupplierPartQuote.CompanyName,
                        rFQSupplierPartQuote.CreatedDate, rFQSupplierPartQuote.CompanyName, rFQSupplierPartQuote.UpdatedDate, rFQSupplierPartQuoteId);
                if (result > 0)
                    rFQSupplierPartQuote.Id = Convert.ToInt32(rFQSupplierPartQuoteId.Value);
                if (rFQSupplierPartQuote.Id > 0)
                {
                    #region "Save dqRawMaterial"
                    if (rFQSupplierPartQuote.rFQdqRawMaterial != null)
                    {
                        MES.Business.Repositories.RFQ.RFQ.IRFQdqRawMaterialRepository objIRFQdqRawMaterialRepository = new MES.Business.Library.BO.RFQ.RFQ.RFQdqRawMaterial();
                        rFQSupplierPartQuote.rFQdqRawMaterial.RFQSupplierPartDQId = rFQSupplierPartQuote.Id;
                        objIRFQdqRawMaterialRepository.Save(rFQSupplierPartQuote.rFQdqRawMaterial);
                    }
                    #endregion
                }
                successMsg = Languages.GetResourceText("SupplierPartQuoteSavedSuccess");
            });
            return SuccessOrFailedResponse<int?>(errMSg, rFQSupplierPartQuote.Id, successMsg);
        }
        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.RFQ.RFQ.RFQSupplierPartQuote rFQSupplierPartQuote)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> SaveSubmitNoQuote(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging)
        {
            //check for the existance
            if (!this.DataContext.rfqSuppliers.AsNoTracking().Any(a => a.UniqueURL.ToUpper() == paging.Criteria.UniqueUrl.ToUpper() && a.IsDeleted == false))
            {
                return FailedBoolResponse(Languages.GetResourceText("RecordNotExist"));
            }
            else
            {
                var rfqSupplier = this.DataContext.rfqSuppliers.Where(a => a.UniqueURL.ToUpper() == paging.Criteria.UniqueUrl.ToUpper()).SingleOrDefault();
                if (rfqSupplier != null)
                {
                    // rfqSupplier.QuoteDate = AuditUtils.GetCurrentDateTime();
                    rfqSupplier.UniqueURL = Convert.ToString(Guid.NewGuid());
                    rfqSupplier.NoQuote = true;
                    this.DataContext.Entry(rfqSupplier).State = EntityState.Modified;
                    this.DataContext.SaveChanges();
                }
            }
            return SuccessBoolResponse(Languages.GetResourceText("SupplierPartQuoteSavedSuccess"));
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.RFQ.RFQSupplierPartQuote> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int RFQSupplierpartQuoteId)
        {
            var RFQToBeDeleted = this.DataContext.SupplierPartQuotes.Where(a => a.Id == RFQSupplierpartQuoteId).SingleOrDefault();
            if (RFQToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("SupplierpartQuoteNotExists"));
            }
            else
            {
                RFQToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                RFQToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(RFQToBeDeleted).State = EntityState.Modified;
                RFQToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("SupplierpartQuoteDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuote>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>> GetRFQSupplierPartQuoteList(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging)
        {
            string errMSg = null;
            List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> lstRFQSupplierPartQuote = new List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>();
            DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ rFQSupplierPartQuote;
            //Part Attachments
            DTO.Library.RFQ.RFQ.RFQPartAttachment rFQPartAttachments;
            List<MES.Data.Library.PartAttachment> lstPartAttachment;

            this.RunOnDB(context =>
            {
                #region "Check Unique id exist or not, Quote is submitted or not etc"
                var rfqSupplier = context.rfqSuppliers.Where(a => a.OriginalURL.ToUpper() == paging.Criteria.UniqueUrl.ToUpper()).SingleOrDefault();
                if (rfqSupplier == null)
                {
                    errMSg = Languages.GetResourceText("UniqueIdNotExist");
                }
                else
                {
                    if ((rfqSupplier.UniqueURL != rfqSupplier.OriginalURL) || string.IsNullOrEmpty(rfqSupplier.OriginalURL))
                    {
                        errMSg = Languages.GetResourceText("QuoteAlreadySubmitted");
                    }
                    else if (Convert.ToBoolean(rfqSupplier.IsQuoteTypeDQ))
                    {
                        errMSg = Languages.GetResourceText("UniqueIdNotExist");
                    }
                }
                #endregion


                if (errMSg == null)
                {
                    var rfqSupplierList = context.SearchSupplierPartToSubmitQuote(paging.Criteria.UniqueUrl).ToList();
                    if (rfqSupplierList == null)
                        errMSg = Languages.GetResourceText("RecordNotExist");
                    else
                    {
                        //setup total records
                        foreach (var item in rfqSupplierList)
                        {
                            rFQSupplierPartQuote = new DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ();
                            #region "Extra properties"
                            rFQSupplierPartQuote.CustomerPartNo = item.CustomerPartNo;
                            rFQSupplierPartQuote.PartDescription = item.PartDescription;
                            rFQSupplierPartQuote.AdditionalPartDesc = item.AdditionalPartDescription;
                            rFQSupplierPartQuote.EstimatedQty = item.EstimatedQty;
                            #region get part wise attachments here
                            rFQSupplierPartQuote.Specifications = new List<DTO.Library.RFQ.RFQ.RFQPartAttachment>();
                            lstPartAttachment = new List<Data.Library.PartAttachment>();
                            lstPartAttachment = context.PartAttachments.Where(c => c.RFQPartId == item.Id).OrderByDescending(a => a.CreatedDate).ToList();
                            if (lstPartAttachment != null)
                            {
                                foreach (var partAttachmentitem in lstPartAttachment)
                                {
                                    rFQPartAttachments = new DTO.Library.RFQ.RFQ.RFQPartAttachment();
                                    rFQPartAttachments.Id = partAttachmentitem.Id;
                                    rFQPartAttachments.RfqPartId = Convert.ToInt32(item.Id);
                                    rFQPartAttachments.AttachmentName = partAttachmentitem.AttachmentName;
                                    rFQPartAttachments.AttachmentDetail = partAttachmentitem.AttachmentDetail;
                                    if (!string.IsNullOrEmpty(partAttachmentitem.AttachmentPathOnServer))
                                        rFQPartAttachments.AttachmentPathOnServer = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"]) + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + partAttachmentitem.AttachmentPathOnServer;
                                    else
                                        rFQPartAttachments.AttachmentPathOnServer = partAttachmentitem.AttachmentPathOnServer;
                                    rFQSupplierPartQuote.Specifications.Add(rFQPartAttachments);
                                }
                            }

                            #endregion
                            rFQSupplierPartQuote.MaterialType = item.MaterialType;
                            rFQSupplierPartQuote.PartWeightKG = item.PartWeightKG;
                            rFQSupplierPartQuote.RfqId = item.RFQId;
                            rFQSupplierPartQuote.RFQDate = item.RFQDate;
                            rFQSupplierPartQuote.QuoteDueDate = item.QuoteDueDate;
                            rFQSupplierPartQuote.SupplierRequirement = item.SupplierRequirement;
                            rFQSupplierPartQuote.RFQRemarks = item.RFQRemarks;
                            rFQSupplierPartQuote.SupplierId = item.SupplierId;
                            rFQSupplierPartQuote.CompanyName = item.CompanyName;
                            rFQSupplierPartQuote.IsMandatory = false;
                            rFQSupplierPartQuote.Minimum = 0;
                            rFQSupplierPartQuote.UniqueUrl = paging.Criteria.UniqueUrl;
                            #endregion

                            rFQSupplierPartQuote.RFQSupplierId = Convert.ToInt32(item.RFQSupplierId);
                            rFQSupplierPartQuote.RFQPartId = item.Id;
                            rFQSupplierPartQuote.Currency = string.Empty;// Constants.DEFAULTCURRENCY;
                            rFQSupplierPartQuote.MaterialCost = Convert.ToDecimal(0.000);
                            rFQSupplierPartQuote.ProcessCost = Convert.ToDecimal(0.000);
                            rFQSupplierPartQuote.UnitPrice = Convert.ToDecimal(0.000);
                            rFQSupplierPartQuote.ToolingCost = Convert.ToDecimal(0.000);
                            rFQSupplierPartQuote.RawMaterialPriceAssumed = string.Empty;
                            rFQSupplierPartQuote.QuoteAttachmentFilePath = string.Empty;
                            rFQSupplierPartQuote.MachiningCost = Convert.ToDecimal(0.000);
                            rFQSupplierPartQuote.OtherProcessCost = Convert.ToDecimal(0.000);
                            rFQSupplierPartQuote.Remarks = string.Empty;
                            rFQSupplierPartQuote.SupplierToolingLeadtime = string.Empty;
                            rFQSupplierPartQuote.ToolingWarranty = string.Empty;
                            rFQSupplierPartQuote.NoOfCavities = 0;
                            rFQSupplierPartQuote.MinOrderQty = null;
                            rFQSupplierPartQuote.MOQConfirmation = false;
                            rFQSupplierPartQuote.Manufacturer = string.Empty;
                            rFQSupplierPartQuote.ExchangeRate = Convert.ToDecimal(0.000);
                            rFQSupplierPartQuote.rFQdqRawMaterial = new DTO.Library.RFQ.RFQ.RFQdqRawMaterial();
                            lstRFQSupplierPartQuote.Add(rFQSupplierPartQuote);
                        }
                    }
                }
            });
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>>(errMSg, lstRFQSupplierPartQuote);
            response.PageInfo = paging.ToPage();
            return response;
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>> GetRFQSupplierPartQuoteListbyUniqueURL(MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria criteria)
        {
            string errMSg = null;
            List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> lstRFQSupplierPartQuote = new List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>();
            DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ rFQSupplierPartQuote;
            this.RunOnDB(context =>
            {
                var rfqSupplierList = context.SearchSupplierPartToSubmitQuote(criteria.UniqueUrl).ToList();
                if (rfqSupplierList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    foreach (var item in rfqSupplierList)
                    {
                        rFQSupplierPartQuote = new DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ();
                        #region "Extra properties"
                        rFQSupplierPartQuote.CustomerPartNo = item.CustomerPartNo;
                        rFQSupplierPartQuote.PartDescription = item.PartDescription;
                        rFQSupplierPartQuote.AdditionalPartDesc = item.AdditionalPartDescription;
                        rFQSupplierPartQuote.EstimatedQty = item.EstimatedQty;
                        rFQSupplierPartQuote.Specifications = new List<DTO.Library.RFQ.RFQ.RFQPartAttachment>(); // may be file path
                        rFQSupplierPartQuote.MaterialType = item.MaterialType;
                        rFQSupplierPartQuote.PartWeightKG = item.PartWeightKG;
                        rFQSupplierPartQuote.RfqId = item.RFQId;
                        rFQSupplierPartQuote.RFQDate = item.RFQDate;
                        rFQSupplierPartQuote.QuoteDueDate = item.QuoteDueDate;
                        rFQSupplierPartQuote.SupplierRequirement = item.SupplierRequirement;
                        rFQSupplierPartQuote.RFQRemarks = item.RFQRemarks;
                        rFQSupplierPartQuote.SupplierId = item.SupplierId;
                        rFQSupplierPartQuote.CompanyName = item.CompanyName;
                        rFQSupplierPartQuote.IsMandatory = false;
                        rFQSupplierPartQuote.Minimum = 0;
                        rFQSupplierPartQuote.UniqueUrl = criteria.UniqueUrl;
                        #endregion

                        rFQSupplierPartQuote.RFQSupplierId = Convert.ToInt32(item.RFQSupplierId);
                        rFQSupplierPartQuote.RFQPartId = item.Id;
                        rFQSupplierPartQuote.Currency = string.Empty;// Constants.DEFAULTCURRENCY;
                        rFQSupplierPartQuote.MaterialCost = Convert.ToDecimal(0.000);
                        rFQSupplierPartQuote.ProcessCost = Convert.ToDecimal(0.000);
                        rFQSupplierPartQuote.UnitPrice = Convert.ToDecimal(0.000);
                        rFQSupplierPartQuote.ToolingCost = Convert.ToDecimal(0.000);
                        rFQSupplierPartQuote.RawMaterialPriceAssumed = string.Empty;
                        rFQSupplierPartQuote.QuoteAttachmentFilePath = string.Empty;
                        rFQSupplierPartQuote.MachiningCost = Convert.ToDecimal(0.000);
                        rFQSupplierPartQuote.OtherProcessCost = Convert.ToDecimal(0.000);
                        rFQSupplierPartQuote.Remarks = string.Empty;
                        rFQSupplierPartQuote.SupplierToolingLeadtime = string.Empty;
                        rFQSupplierPartQuote.ToolingWarranty = string.Empty;
                        rFQSupplierPartQuote.NoOfCavities = 0;
                        rFQSupplierPartQuote.MinOrderQty = null;
                        rFQSupplierPartQuote.MOQConfirmation = false;
                        rFQSupplierPartQuote.Manufacturer = string.Empty;
                        rFQSupplierPartQuote.ExchangeRate = Convert.ToDecimal(0.000);
                        lstRFQSupplierPartQuote.Add(rFQSupplierPartQuote);
                    }
                }

            });
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>>(errMSg, lstRFQSupplierPartQuote);

            return response;
        }

        public NPE.Core.ITypedResponse<int> RFQSupplierPartsCount(string rfqId)
        {
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);

            string errMSg = string.Empty;
            int rfqSupplierCount = 0;
            DTO.Library.RFQ.RFQ.RFQSuppliers rFQSuppliers = new DTO.Library.RFQ.RFQ.RFQSuppliers();

            this.RunOnDB(context =>
            {
                context.GetSupplierPartQuoteCount(rfqId, totalRecords);
            });

            rfqSupplierCount = Convert.ToInt32(totalRecords.Value);
            //get hold of response
            var response = SuccessOrFailedResponse<int>(errMSg, rfqSupplierCount);
            return response;
        }
        #endregion

        #region "Submit Detail Quote DQ"
        public NPE.Core.ITypedResponse<bool?> SaveSubmitDQ(List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> RFQSupplierPartQuoteList)
        {
            if (RFQSupplierPartQuoteList != null && RFQSupplierPartQuoteList.Count > 0)
            {
                string fileName = string.Empty;
                string UniqueURL = string.Empty;
                bool isFileProcessed = false;
                DateTime dtTime = AuditUtils.GetCurrentDateTime();
                foreach (var rFQSupplierPartQuote in RFQSupplierPartQuoteList)
                {
                    UniqueURL = RFQSupplierPartQuoteList[0].UniqueUrl;
                    rFQSupplierPartQuote.Remarks = RFQSupplierPartQuoteList[0].Remarks;
                    rFQSupplierPartQuote.ExchangeRate = RFQSupplierPartQuoteList[0].ExchangeRate;
                    rFQSupplierPartQuote.Currency = RFQSupplierPartQuoteList[0].Currency;
                    rFQSupplierPartQuote.RawMaterialPriceAssumed = RFQSupplierPartQuoteList[0].RawMaterialPriceAssumed;
                    if (!isFileProcessed)
                    {
                        isFileProcessed = true;
                        if (!string.IsNullOrEmpty(RFQSupplierPartQuoteList[0].QuoteAttachmentFilePath))
                            fileName = MES.Business.Library.Helper.BlobHelper.GetPhysicalPath(RFQSupplierPartQuoteList[0].QuoteAttachmentFilePath);
                    }
                    rFQSupplierPartQuote.QuoteAttachmentFilePath = fileName;
                    rFQSupplierPartQuote.CreatedDate = rFQSupplierPartQuote.UpdatedDate = dtTime;
                    var obj = SaveDQ(rFQSupplierPartQuote);
                    if (obj == null || obj.StatusCode != 200)
                    {
                        return FailedBoolResponse(Languages.GetResourceText("SupplierpartQuoteError"));
                    }
                }
                Data.Library.rfqSupplier rfqSupplier = null;
                /////update quote date
                this.RunOnDB(context =>
                {
                    rfqSupplier = context.rfqSuppliers.Where(a => a.OriginalURL.ToUpper() == UniqueURL.ToUpper() && a.IsDeleted == false).SingleOrDefault();
                    if (rfqSupplier != null)
                    {
                        rfqSupplier.UniqueURL = Convert.ToString(Guid.NewGuid());
                        if (!rfqSupplier.QuoteDate.HasValue)
                            rfqSupplier.QuoteDate = AuditUtils.GetCurrentDateTime();
                        this.DataContext.Entry(rfqSupplier).State = EntityState.Modified;
                        this.DataContext.SaveChanges();
                    }
                });
                if (rfqSupplier != null)
                {
                    DTO.Library.RFQ.RFQ.RFQSuppliers rsItem = new DTO.Library.RFQ.RFQ.RFQSuppliers();
                    rsItem.SupplierId = rfqSupplier.SupplierId;
                    rsItem.RFQId = rfqSupplier.RFQId;
                    rsItem.Id = rfqSupplier.Id;

                    //Send notification via email to the Supplier and admin here
                    SendEmail('A', rsItem);
                    SendEmail('S', rsItem);
                }
            }
            return SuccessBoolResponse(Languages.GetResourceText("SupplierPartQuoteSavedSuccess"));
        }

        public NPE.Core.ITypedResponse<int?> SaveDQ(DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ rFQSupplierPartQuote)
        {
            string errMSg = null;
            string successMsg = null;
            var recordToBeUpdated = new MES.Data.Library.SupplierPartQuote();
            ObjectParameter rFQSupplierPartQuoteId = new ObjectParameter("rFQSupplierPartQuoteId", 0);
            this.RunOnDB(context =>
            {
                int result = context.InsertRFQSupplierPartQuoteDQ(rFQSupplierPartQuote.Id, rFQSupplierPartQuote.RFQSupplierId, rFQSupplierPartQuote.RFQPartId,
                        rFQSupplierPartQuote.ManufacturerId, rFQSupplierPartQuote.MaterialCost, rFQSupplierPartQuote.ProcessCost,
                        rFQSupplierPartQuote.UnitPrice, rFQSupplierPartQuote.ToolingCost, rFQSupplierPartQuote.Currency, rFQSupplierPartQuote.RawMaterialPriceAssumed,
                        rFQSupplierPartQuote.QuoteAttachmentFilePath, rFQSupplierPartQuote.MachiningCost, rFQSupplierPartQuote.OtherProcessCost,
                        rFQSupplierPartQuote.Remarks, rFQSupplierPartQuote.SupplierToolingLeadtime, rFQSupplierPartQuote.ToolingWarranty,
                        rFQSupplierPartQuote.NoOfCavities, rFQSupplierPartQuote.MinOrderQty, rFQSupplierPartQuote.MOQConfirmation,
                        rFQSupplierPartQuote.Manufacturer, rFQSupplierPartQuote.ExchangeRate, rFQSupplierPartQuote.CompanyName,
                        rFQSupplierPartQuote.CreatedDate, rFQSupplierPartQuote.CompanyName, rFQSupplierPartQuote.UpdatedDate, rFQSupplierPartQuoteId);
                if (result > 0)
                    rFQSupplierPartQuote.Id = Convert.ToInt32(rFQSupplierPartQuoteId.Value);
                if (rFQSupplierPartQuote.Id > 0)
                {
                    #region "Save dqRawMaterial"
                    if (rFQSupplierPartQuote.rFQdqRawMaterial != null)
                    {
                        MES.Business.Repositories.RFQ.RFQ.IRFQdqRawMaterialRepository objIRFQdqRawMaterialRepository = new MES.Business.Library.BO.RFQ.RFQ.RFQdqRawMaterial();
                        rFQSupplierPartQuote.rFQdqRawMaterial.RFQSupplierPartDQId = rFQSupplierPartQuote.Id;
                        objIRFQdqRawMaterialRepository.Save(rFQSupplierPartQuote.rFQdqRawMaterial);
                    }
                    #endregion

                    #region "Save dqPrimaryProcessConversion"
                    if (rFQSupplierPartQuote.rFQdqPrimaryProcessConversion != null)
                    {
                        MES.Business.Repositories.RFQ.RFQ.IRFQdqPrimaryProcessConversionRepository objIRFQdqPrimaryProcessConversionRepository =
                            new MES.Business.Library.BO.RFQ.RFQ.RFQdqPrimaryProcessConversion();
                        rFQSupplierPartQuote.rFQdqPrimaryProcessConversion.RFQSupplierPartDQId = rFQSupplierPartQuote.Id;
                        objIRFQdqPrimaryProcessConversionRepository.Save(rFQSupplierPartQuote.rFQdqPrimaryProcessConversion);
                    }
                    #endregion

                    #region "Save dqMachining"
                    if (rFQSupplierPartQuote.rFQdqMachining != null)
                    {
                        MES.Business.Repositories.RFQ.RFQ.IRFQdqMachiningRepository objIRFQdqMachiningRepository = new MES.Business.Library.BO.RFQ.RFQ.RFQdqMachining();
                        rFQSupplierPartQuote.rFQdqMachining.RFQSupplierPartDQId = rFQSupplierPartQuote.Id;
                        objIRFQdqMachiningRepository.Save(rFQSupplierPartQuote.rFQdqMachining);
                    }
                    #endregion

                    #region "Save dqMachiningSecondaryOperation"
                    if (rFQSupplierPartQuote.rFQdqMachiningSecondaryOperation != null)
                    {
                        MES.Business.Repositories.RFQ.RFQ.IRFQdqMachiningSecondaryOperationRepository objIRFQdqMachiningSecondaryOperationRepository = new MES.Business.Library.BO.RFQ.RFQ.RFQdqMachiningSecondaryOperation();
                        rFQSupplierPartQuote.rFQdqMachiningSecondaryOperation.RFQSupplierPartDQId = rFQSupplierPartQuote.Id;
                        objIRFQdqMachiningSecondaryOperationRepository.Save(rFQSupplierPartQuote.rFQdqMachiningSecondaryOperation);
                    }
                    #endregion

                    #region "Save dqMachiningOtherOperation"
                    if (rFQSupplierPartQuote.rFQdqMachiningOtherOperation != null)
                    {
                        MES.Business.Repositories.RFQ.RFQ.IRFQdqMachiningOtherOperationRepository objIRFQdqMachiningOtherOperationRepository = new MES.Business.Library.BO.RFQ.RFQ.RFQdqMachiningOtherOperation();
                        rFQSupplierPartQuote.rFQdqMachiningOtherOperation.RFQSupplierPartDQId = rFQSupplierPartQuote.Id;
                        objIRFQdqMachiningOtherOperationRepository.Save(rFQSupplierPartQuote.rFQdqMachiningOtherOperation);
                    }
                    #endregion

                    #region "Save dqSurfaceTreatment"
                    if (rFQSupplierPartQuote.rFQdqSurfaceTreatment != null)
                    {
                        MES.Business.Repositories.RFQ.RFQ.IRFQdqSurfaceTreatmentRepository objIRFQdqSurfaceTreatmentRepository = new MES.Business.Library.BO.RFQ.RFQ.RFQdqSurfaceTreatment();
                        rFQSupplierPartQuote.rFQdqSurfaceTreatment.RFQSupplierPartDQId = rFQSupplierPartQuote.Id;
                        objIRFQdqSurfaceTreatmentRepository.Save(rFQSupplierPartQuote.rFQdqSurfaceTreatment);
                    }
                    #endregion

                    #region "Save dqOverhead"
                    if (rFQSupplierPartQuote.rFQdqOverhead != null)
                    {
                        MES.Business.Repositories.RFQ.RFQ.IRFQdqOverheadRepository objIRFQdqOverheadRepository = new MES.Business.Library.BO.RFQ.RFQ.RFQdqOverhead();
                        rFQSupplierPartQuote.rFQdqOverhead.RFQSupplierPartDQId = rFQSupplierPartQuote.Id;
                        objIRFQdqOverheadRepository.Save(rFQSupplierPartQuote.rFQdqOverhead);
                    }
                    #endregion
                }

                successMsg = Languages.GetResourceText("SupplierPartQuoteSavedSuccess");
            });
            return SuccessOrFailedResponse<int?>(errMSg, rFQSupplierPartQuote.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<bool?> SaveSubmitNoDQ(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging)
        {
            //check for the existance
            if (!this.DataContext.rfqSuppliers.AsNoTracking().Any(a => a.UniqueURL.ToUpper() == paging.Criteria.UniqueUrl.ToUpper() && a.IsDeleted == false))
            {
                return FailedBoolResponse(Languages.GetResourceText("RecordNotExist"));
            }
            else
            {
                var rfqSupplier = this.DataContext.rfqSuppliers.Where(a => a.UniqueURL.ToUpper() == paging.Criteria.UniqueUrl.ToUpper()).SingleOrDefault();
                if (rfqSupplier != null)
                {
                    //rfqSupplier.QuoteDate = AuditUtils.GetCurrentDateTime();
                    rfqSupplier.UniqueURL = Convert.ToString(Guid.NewGuid());
                    rfqSupplier.NoQuote = true;
                    this.DataContext.Entry(rfqSupplier).State = EntityState.Modified;
                    this.DataContext.SaveChanges();
                }
            }
            return SuccessBoolResponse(Languages.GetResourceText("SupplierPartQuoteSavedSuccess"));
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>> GetDQRFQSupplierPartQuoteList(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging)
        {
            string errMSg = null;
            List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> lstRFQSupplierPartQuote = new List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>();
            DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ rFQSupplierPartQuote;
            //Part Attachments
            DTO.Library.RFQ.RFQ.RFQPartAttachment rFQPartAttachments;
            List<MES.Data.Library.PartAttachment> lstPartAttachment;

            this.RunOnDB(context =>
            {
                #region "Check Unique id exist or not, Quote is submitted or not etc"
                var rfqSupplier = context.rfqSuppliers.Where(a => a.OriginalURL.ToUpper() == paging.Criteria.UniqueUrl.ToUpper()).SingleOrDefault();
                if (rfqSupplier == null)
                {
                    errMSg = Languages.GetResourceText("UniqueIdNotExist");
                }
                else
                {
                    if ((rfqSupplier.UniqueURL != rfqSupplier.OriginalURL) || string.IsNullOrEmpty(rfqSupplier.OriginalURL))
                    {
                        errMSg = Languages.GetResourceText("QuoteAlreadySubmitted");
                    }
                    else if (!Convert.ToBoolean(rfqSupplier.IsQuoteTypeDQ))
                    {
                        errMSg = Languages.GetResourceText("UniqueIdNotExist");
                    }
                }
                #endregion


                if (errMSg == null)
                {
                    var rfqSupplierList = context.SearchSupplierPartToSubmitDetailQuote(paging.Criteria.UniqueUrl).ToList();
                    if (rfqSupplierList == null)
                        errMSg = Languages.GetResourceText("RecordNotExist");
                    else
                    {
                        //setup total records
                        foreach (var item in rfqSupplierList)
                        {
                            rFQSupplierPartQuote = new DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ();
                            #region "Extra properties"
                            rFQSupplierPartQuote.CustomerPartNo = item.PartNo;
                            rFQSupplierPartQuote.PartDescription = item.PartDescription;
                            rFQSupplierPartQuote.AdditionalPartDesc = item.AdditionalPartDescription;
                            rFQSupplierPartQuote.EstimatedQty = item.EstimatedQty;
                            #region get part wise attachments here
                            rFQSupplierPartQuote.Specifications = new List<DTO.Library.RFQ.RFQ.RFQPartAttachment>();
                            lstPartAttachment = new List<Data.Library.PartAttachment>();
                            lstPartAttachment = context.PartAttachments.Where(c => c.RFQPartId == item.Id).OrderByDescending(a => a.CreatedDate).ToList();
                            if (lstPartAttachment != null)
                            {
                                foreach (var partAttachmentitem in lstPartAttachment)
                                {
                                    rFQPartAttachments = new DTO.Library.RFQ.RFQ.RFQPartAttachment();
                                    rFQPartAttachments.Id = partAttachmentitem.Id;
                                    rFQPartAttachments.RfqPartId = Convert.ToInt32(item.Id);
                                    rFQPartAttachments.AttachmentName = partAttachmentitem.AttachmentName;
                                    rFQPartAttachments.AttachmentDetail = partAttachmentitem.AttachmentDetail;
                                    if (!string.IsNullOrEmpty(partAttachmentitem.AttachmentPathOnServer))
                                        rFQPartAttachments.AttachmentPathOnServer = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"]) + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + partAttachmentitem.AttachmentPathOnServer;
                                    else
                                        rFQPartAttachments.AttachmentPathOnServer = partAttachmentitem.AttachmentPathOnServer;
                                    rFQSupplierPartQuote.Specifications.Add(rFQPartAttachments);
                                }
                            }

                            #endregion
                            rFQSupplierPartQuote.MaterialType = item.MaterialType;
                            rFQSupplierPartQuote.PartWeightKG = item.PartWeightKG;
                            rFQSupplierPartQuote.RfqId = item.RFQId;
                            rFQSupplierPartQuote.RFQDate = item.RFQDate;
                            rFQSupplierPartQuote.QuoteDueDate = item.QuoteDueDate;
                            rFQSupplierPartQuote.SupplierRequirement = item.SupplierRequirement;
                            rFQSupplierPartQuote.RFQRemarks = item.RFQRemarks;
                            rFQSupplierPartQuote.SupplierId = item.SupplierId;
                            rFQSupplierPartQuote.CompanyName = item.CompanyName;
                            rFQSupplierPartQuote.IsMandatory = false;
                            rFQSupplierPartQuote.Minimum = 0;
                            rFQSupplierPartQuote.UniqueUrl = paging.Criteria.UniqueUrl;
                            #endregion

                            rFQSupplierPartQuote.RFQSupplierId = Convert.ToInt32(item.RFQSupplierId);
                            rFQSupplierPartQuote.RFQPartId = item.Id;
                            rFQSupplierPartQuote.Currency = string.Empty;// Constants.DEFAULTCURRENCY;
                            rFQSupplierPartQuote.MaterialCost = Convert.ToDecimal(0.000);
                            rFQSupplierPartQuote.ProcessCost = Convert.ToDecimal(0.000);
                            rFQSupplierPartQuote.UnitPrice = Convert.ToDecimal(0.000);
                            rFQSupplierPartQuote.ToolingCost = Convert.ToDecimal(0.000);
                            rFQSupplierPartQuote.RawMaterialPriceAssumed = string.Empty;
                            rFQSupplierPartQuote.QuoteAttachmentFilePath = string.Empty;
                            rFQSupplierPartQuote.MachiningCost = Convert.ToDecimal(0.000);
                            rFQSupplierPartQuote.OtherProcessCost = Convert.ToDecimal(0.000);
                            rFQSupplierPartQuote.Remarks = string.Empty;
                            rFQSupplierPartQuote.SupplierToolingLeadtime = string.Empty;
                            rFQSupplierPartQuote.ToolingWarranty = string.Empty;
                            rFQSupplierPartQuote.NoOfCavities = 0;
                            rFQSupplierPartQuote.MinOrderQty = null;
                            rFQSupplierPartQuote.MOQConfirmation = false;
                            rFQSupplierPartQuote.Manufacturer = string.Empty;
                            rFQSupplierPartQuote.ExchangeRate = Convert.ToDecimal(0.000);
                            rFQSupplierPartQuote.rFQdqRawMaterial = new DTO.Library.RFQ.RFQ.RFQdqRawMaterial();
                            rFQSupplierPartQuote.rFQdqPrimaryProcessConversion = new DTO.Library.RFQ.RFQ.RFQdqPrimaryProcessConversion();
                            rFQSupplierPartQuote.rFQdqMachining = new DTO.Library.RFQ.RFQ.RFQdqMachining();
                            rFQSupplierPartQuote.rFQdqMachiningSecondaryOperation = new DTO.Library.RFQ.RFQ.RFQdqMachiningSecondaryOperation();
                            rFQSupplierPartQuote.rFQdqMachiningOtherOperation = new DTO.Library.RFQ.RFQ.RFQdqMachiningOtherOperation();
                            rFQSupplierPartQuote.rFQdqSurfaceTreatment = new DTO.Library.RFQ.RFQ.RFQdqSurfaceTreatment();
                            rFQSupplierPartQuote.rFQdqOverhead = new DTO.Library.RFQ.RFQ.RFQdqOverhead();
                            lstRFQSupplierPartQuote.Add(rFQSupplierPartQuote);
                        }
                    }
                }
            });
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>>(errMSg, lstRFQSupplierPartQuote);
            response.PageInfo = paging.ToPage();
            return response;
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>> GetDQRFQSupplierPartQuoteListbyUniqueURL(MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria criteria)
        {
            string errMSg = null;
            List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> lstRFQSupplierPartQuote = new List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>();
            DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ rFQSupplierPartQuote;
            this.RunOnDB(context =>
            {
                if (errMSg == null)
                {
                    var rfqSupplierList = context.SearchSupplierPartToSubmitDetailQuote(criteria.UniqueUrl).ToList();
                    if (rfqSupplierList == null)
                        errMSg = Languages.GetResourceText("RecordNotExist");
                    else
                    {
                        //setup total records
                        foreach (var item in rfqSupplierList)
                        {
                            rFQSupplierPartQuote = new DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ();
                            #region "Extra properties"
                            rFQSupplierPartQuote.CustomerPartNo = item.PartNo;
                            rFQSupplierPartQuote.PartDescription = item.PartDescription;
                            rFQSupplierPartQuote.AdditionalPartDesc = item.AdditionalPartDescription;
                            rFQSupplierPartQuote.EstimatedQty = item.EstimatedQty;
                            rFQSupplierPartQuote.Specifications = new List<DTO.Library.RFQ.RFQ.RFQPartAttachment>();
                            rFQSupplierPartQuote.MaterialType = item.MaterialType;
                            rFQSupplierPartQuote.PartWeightKG = item.PartWeightKG;
                            rFQSupplierPartQuote.RfqId = item.RFQId;
                            rFQSupplierPartQuote.RFQDate = item.RFQDate;
                            rFQSupplierPartQuote.QuoteDueDate = item.QuoteDueDate;
                            rFQSupplierPartQuote.SupplierRequirement = item.SupplierRequirement;
                            rFQSupplierPartQuote.RFQRemarks = item.RFQRemarks;
                            rFQSupplierPartQuote.SupplierId = item.SupplierId;
                            rFQSupplierPartQuote.CompanyName = item.CompanyName;
                            rFQSupplierPartQuote.IsMandatory = false;
                            rFQSupplierPartQuote.Minimum = 0;
                            rFQSupplierPartQuote.UniqueUrl = criteria.UniqueUrl;
                            #endregion

                            rFQSupplierPartQuote.RFQSupplierId = Convert.ToInt32(item.RFQSupplierId);
                            rFQSupplierPartQuote.RFQPartId = item.Id;
                            rFQSupplierPartQuote.Currency = string.Empty;// Constants.DEFAULTCURRENCY;
                            rFQSupplierPartQuote.MaterialCost = Convert.ToDecimal(0.000);
                            rFQSupplierPartQuote.ProcessCost = Convert.ToDecimal(0.000);
                            rFQSupplierPartQuote.UnitPrice = Convert.ToDecimal(0.000);
                            rFQSupplierPartQuote.ToolingCost = Convert.ToDecimal(0.000);
                            rFQSupplierPartQuote.RawMaterialPriceAssumed = string.Empty;
                            rFQSupplierPartQuote.QuoteAttachmentFilePath = string.Empty;
                            rFQSupplierPartQuote.MachiningCost = Convert.ToDecimal(0.000);
                            rFQSupplierPartQuote.OtherProcessCost = Convert.ToDecimal(0.000);
                            rFQSupplierPartQuote.Remarks = string.Empty;
                            rFQSupplierPartQuote.SupplierToolingLeadtime = string.Empty;
                            rFQSupplierPartQuote.ToolingWarranty = string.Empty;
                            rFQSupplierPartQuote.NoOfCavities = 0;
                            rFQSupplierPartQuote.MinOrderQty = null;
                            rFQSupplierPartQuote.MOQConfirmation = false;
                            rFQSupplierPartQuote.Manufacturer = string.Empty;
                            rFQSupplierPartQuote.ExchangeRate = Convert.ToDecimal(0.000);
                            rFQSupplierPartQuote.rFQdqRawMaterial = new DTO.Library.RFQ.RFQ.RFQdqRawMaterial();
                            rFQSupplierPartQuote.rFQdqPrimaryProcessConversion = new DTO.Library.RFQ.RFQ.RFQdqPrimaryProcessConversion();
                            rFQSupplierPartQuote.rFQdqMachining = new DTO.Library.RFQ.RFQ.RFQdqMachining();
                            rFQSupplierPartQuote.rFQdqMachiningSecondaryOperation = new DTO.Library.RFQ.RFQ.RFQdqMachiningSecondaryOperation();
                            rFQSupplierPartQuote.rFQdqMachiningOtherOperation = new DTO.Library.RFQ.RFQ.RFQdqMachiningOtherOperation();
                            rFQSupplierPartQuote.rFQdqSurfaceTreatment = new DTO.Library.RFQ.RFQ.RFQdqSurfaceTreatment();
                            rFQSupplierPartQuote.rFQdqOverhead = new DTO.Library.RFQ.RFQ.RFQdqOverhead();
                            lstRFQSupplierPartQuote.Add(rFQSupplierPartQuote);
                        }
                    }
                }
            });
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>>(errMSg, lstRFQSupplierPartQuote);

            return response;
        }

        #endregion

        #region "Supplier quote (Admin side for simplified and detailed quote)"
        public NPE.Core.ITypedResponse<bool?> SaveSupplierQuoteList(List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> RFQSupplierPartQuoteList)
        {
            if (RFQSupplierPartQuoteList != null && RFQSupplierPartQuoteList.Count > 0)
            {
                string fileName = string.Empty;
                bool isFileProcessed = false;
                DateTime dtTime = AuditUtils.GetCurrentDateTime();
                foreach (var rFQSupplierPartQuote in RFQSupplierPartQuoteList.Where(a => a.IsHistory == false).ToList())
                {
                    rFQSupplierPartQuote.Remarks = RFQSupplierPartQuoteList[0].Remarks;
                    rFQSupplierPartQuote.ExchangeRate = RFQSupplierPartQuoteList[0].ExchangeRate;
                    rFQSupplierPartQuote.Currency = RFQSupplierPartQuoteList[0].Currency;
                    rFQSupplierPartQuote.RawMaterialPriceAssumed = RFQSupplierPartQuoteList[0].RawMaterialPriceAssumed;
                    if (!isFileProcessed)
                    {
                        isFileProcessed = true;
                        if (!string.IsNullOrEmpty(RFQSupplierPartQuoteList[0].QuoteAttachmentFilePath))
                            fileName = MES.Business.Library.Helper.BlobHelper.GetPhysicalPath(RFQSupplierPartQuoteList[0].QuoteAttachmentFilePath);
                    }
                    rFQSupplierPartQuote.QuoteAttachmentFilePath = fileName;
                    rFQSupplierPartQuote.CreatedDate = rFQSupplierPartQuote.UpdatedDate = dtTime;
                    var obj = SaveSupplierQuote(rFQSupplierPartQuote);
                    if (obj == null || obj.StatusCode != 200)
                    {
                        return FailedBoolResponse(Languages.GetResourceText("SupplierQuoteError"));
                    }
                }
            }
            return SuccessBoolResponse(Languages.GetResourceText("SupplierQuoteSavedSuccess"));
        }

        public NPE.Core.ITypedResponse<int?> SaveSupplierQuote(DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ rFQSupplierPartQuote)
        {
            string errMSg = null;
            string successMsg = null;
            var recordToBeUpdated = new MES.Data.Library.SupplierPartQuote();
            ObjectParameter rFQSupplierPartQuoteId = new ObjectParameter("rFQSupplierPartQuoteId", 0);
            this.RunOnDB(context =>
            {
                int result = context.InsertRFQSupplierPartQuoteDQ(rFQSupplierPartQuote.Id, rFQSupplierPartQuote.RFQSupplierId, rFQSupplierPartQuote.RFQPartId,
                        rFQSupplierPartQuote.ManufacturerId, rFQSupplierPartQuote.MaterialCost, rFQSupplierPartQuote.ProcessCost,
                        rFQSupplierPartQuote.UnitPrice, rFQSupplierPartQuote.ToolingCost, rFQSupplierPartQuote.Currency, rFQSupplierPartQuote.RawMaterialPriceAssumed,
                        rFQSupplierPartQuote.QuoteAttachmentFilePath, rFQSupplierPartQuote.MachiningCost, rFQSupplierPartQuote.OtherProcessCost,
                        rFQSupplierPartQuote.Remarks, rFQSupplierPartQuote.SupplierToolingLeadtime, rFQSupplierPartQuote.ToolingWarranty,
                        rFQSupplierPartQuote.NoOfCavities, rFQSupplierPartQuote.MinOrderQty, rFQSupplierPartQuote.MOQConfirmation,
                        rFQSupplierPartQuote.Manufacturer, rFQSupplierPartQuote.ExchangeRate, CurrentUser,
                        rFQSupplierPartQuote.CreatedDate, CurrentUser, rFQSupplierPartQuote.UpdatedDate, rFQSupplierPartQuoteId);
                if (result > 0)
                    rFQSupplierPartQuote.Id = Convert.ToInt32(rFQSupplierPartQuoteId.Value);
                if (rFQSupplierPartQuote.Id > 0)
                {
                    #region "Save dqRawMaterial"
                    if (rFQSupplierPartQuote.rFQdqRawMaterial != null)
                    {
                        MES.Business.Repositories.RFQ.RFQ.IRFQdqRawMaterialRepository objIRFQdqRawMaterialRepository = new MES.Business.Library.BO.RFQ.RFQ.RFQdqRawMaterial();
                        rFQSupplierPartQuote.rFQdqRawMaterial.RFQSupplierPartDQId = rFQSupplierPartQuote.Id;
                        objIRFQdqRawMaterialRepository.Save(rFQSupplierPartQuote.rFQdqRawMaterial);
                    }
                    #endregion
                    if (rFQSupplierPartQuote.IsQuoteTypeDQ)
                    {
                        #region "Save dqPrimaryProcessConversion"
                        if (rFQSupplierPartQuote.rFQdqPrimaryProcessConversion != null)
                        {
                            MES.Business.Repositories.RFQ.RFQ.IRFQdqPrimaryProcessConversionRepository objIRFQdqPrimaryProcessConversionRepository =
                                new MES.Business.Library.BO.RFQ.RFQ.RFQdqPrimaryProcessConversion();
                            rFQSupplierPartQuote.rFQdqPrimaryProcessConversion.RFQSupplierPartDQId = rFQSupplierPartQuote.Id;
                            objIRFQdqPrimaryProcessConversionRepository.Save(rFQSupplierPartQuote.rFQdqPrimaryProcessConversion);
                        }
                        #endregion

                        #region "Save dqMachining"
                        if (rFQSupplierPartQuote.rFQdqMachining != null)
                        {
                            MES.Business.Repositories.RFQ.RFQ.IRFQdqMachiningRepository objIRFQdqMachiningRepository = new MES.Business.Library.BO.RFQ.RFQ.RFQdqMachining();
                            rFQSupplierPartQuote.rFQdqMachining.RFQSupplierPartDQId = rFQSupplierPartQuote.Id;
                            objIRFQdqMachiningRepository.Save(rFQSupplierPartQuote.rFQdqMachining);
                        }
                        #endregion

                        #region "Save dqMachiningSecondaryOperation"
                        if (rFQSupplierPartQuote.rFQdqMachiningSecondaryOperation != null)
                        {
                            MES.Business.Repositories.RFQ.RFQ.IRFQdqMachiningSecondaryOperationRepository objIRFQdqMachiningSecondaryOperationRepository = new MES.Business.Library.BO.RFQ.RFQ.RFQdqMachiningSecondaryOperation();
                            rFQSupplierPartQuote.rFQdqMachiningSecondaryOperation.RFQSupplierPartDQId = rFQSupplierPartQuote.Id;
                            objIRFQdqMachiningSecondaryOperationRepository.Save(rFQSupplierPartQuote.rFQdqMachiningSecondaryOperation);
                        }
                        #endregion

                        #region "Save dqMachiningOtherOperation"
                        if (rFQSupplierPartQuote.rFQdqMachiningOtherOperation != null)
                        {
                            MES.Business.Repositories.RFQ.RFQ.IRFQdqMachiningOtherOperationRepository objIRFQdqMachiningOtherOperationRepository = new MES.Business.Library.BO.RFQ.RFQ.RFQdqMachiningOtherOperation();
                            rFQSupplierPartQuote.rFQdqMachiningOtherOperation.RFQSupplierPartDQId = rFQSupplierPartQuote.Id;
                            objIRFQdqMachiningOtherOperationRepository.Save(rFQSupplierPartQuote.rFQdqMachiningOtherOperation);
                        }
                        #endregion

                        #region "Save dqSurfaceTreatment"
                        if (rFQSupplierPartQuote.rFQdqSurfaceTreatment != null)
                        {
                            MES.Business.Repositories.RFQ.RFQ.IRFQdqSurfaceTreatmentRepository objIRFQdqSurfaceTreatmentRepository = new MES.Business.Library.BO.RFQ.RFQ.RFQdqSurfaceTreatment();
                            rFQSupplierPartQuote.rFQdqSurfaceTreatment.RFQSupplierPartDQId = rFQSupplierPartQuote.Id;
                            objIRFQdqSurfaceTreatmentRepository.Save(rFQSupplierPartQuote.rFQdqSurfaceTreatment);
                        }
                        #endregion

                        #region "Save dqOverhead"
                        if (rFQSupplierPartQuote.rFQdqOverhead != null)
                        {
                            MES.Business.Repositories.RFQ.RFQ.IRFQdqOverheadRepository objIRFQdqOverheadRepository = new MES.Business.Library.BO.RFQ.RFQ.RFQdqOverhead();
                            rFQSupplierPartQuote.rFQdqOverhead.RFQSupplierPartDQId = rFQSupplierPartQuote.Id;
                            objIRFQdqOverheadRepository.Save(rFQSupplierPartQuote.rFQdqOverhead);
                        }
                        #endregion
                    }
                }
                Data.Library.rfqSupplier rfqSupplier = null;
                /////update quote date

                rfqSupplier = context.rfqSuppliers.Where(a => a.Id == rFQSupplierPartQuote.RFQSupplierId && a.IsDeleted == false).SingleOrDefault();
                if (rfqSupplier != null)
                {
                    if (rfqSupplier.UniqueURL == rfqSupplier.OriginalURL)
                        rfqSupplier.UniqueURL = Convert.ToString(Guid.NewGuid());

                    if (!rfqSupplier.QuoteDate.HasValue)
                        rfqSupplier.QuoteDate = AuditUtils.GetCurrentDateTime();

                    this.DataContext.Entry(rfqSupplier).State = EntityState.Modified;
                    this.DataContext.SaveChanges();
                }

                successMsg = Languages.GetResourceText("SupplierQuoteSavedSuccess");
            });
            return SuccessOrFailedResponse<int?>(errMSg, rFQSupplierPartQuote.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>> GetSupplierQuoteList(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging)
        {
            string errMSg = null;
            List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> lstRFQSupplierPartQuote = new List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>();
            this.RunOnDB(context =>
            {

                var rfqSupplierList = context.SearchSupplierPartForSupplierQuote(paging.Criteria.RFQId, paging.Criteria.SupplierId).ToList();
                if (rfqSupplierList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    lstRFQSupplierPartQuote = setSupplierQuoteProperties(lstRFQSupplierPartQuote, rfqSupplierList, false);
                }

                //var rfqSupplierListHistory = context.SearchSupplierPartForSupplierQuoteHistory(paging.Criteria.RFQId, paging.Criteria.SupplierId).ToList();
                //if (rfqSupplierListHistory == null)
                //{
                //    if (errMSg == null)
                //        errMSg = Languages.GetResourceText("RecordNotExist");
                //}
                //else
                //{
                //    lstRFQSupplierPartQuote = setSupplierQuoteProperties(lstRFQSupplierPartQuote, rfqSupplierListHistory, true);
                //}
            });
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>>(errMSg, lstRFQSupplierPartQuote);
            response.PageInfo = paging.ToPage();
            return response;
        }

        private static List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> setSupplierQuoteProperties(List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> lstRFQSupplierPartQuote, List<Data.Library.SearchSupplierPartForSupplierQuote_Result> rfqSupplierList, bool IsHistory)
        {
            DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ rFQSupplierPartQuote;
            //setup total records
            foreach (var item in rfqSupplierList)
            {
                rFQSupplierPartQuote = new DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ();
                #region "Extra properties"
                rFQSupplierPartQuote.CustomerPartNo = item.CustomerPartNo;
                rFQSupplierPartQuote.PartDescription = item.PartDescription;
                rFQSupplierPartQuote.AdditionalPartDesc = item.AdditionalPartDescription;
                rFQSupplierPartQuote.EstimatedQty = item.EstimatedQty;
                rFQSupplierPartQuote.Specifications = new List<DTO.Library.RFQ.RFQ.RFQPartAttachment>();
                rFQSupplierPartQuote.MaterialType = item.MaterialType;
                rFQSupplierPartQuote.PartWeightKG = item.PartWeightKG;
                rFQSupplierPartQuote.RfqId = item.RFQId;
                rFQSupplierPartQuote.RFQDate = item.RFQDate;
                rFQSupplierPartQuote.QuoteDueDate = item.QuoteDueDate;
                rFQSupplierPartQuote.SupplierRequirement = item.SupplierRequirement;
                rFQSupplierPartQuote.RFQRemarks = item.RFQRemarks;
                rFQSupplierPartQuote.SupplierId = item.SupplierId;
                rFQSupplierPartQuote.CompanyName = item.CompanyName;
                rFQSupplierPartQuote.IsMandatory = false;
                rFQSupplierPartQuote.Minimum = 0;
                rFQSupplierPartQuote.IsQuoteTypeDQ = Convert.ToBoolean(item.IsQuoteTypeDQ);
                rFQSupplierPartQuote.NoQuote = item.NoQuote;
                rFQSupplierPartQuote.IsHistory = IsHistory;
                #endregion

                #region Supplier part quote tables data
                rFQSupplierPartQuote.RFQSupplierId = Convert.ToInt32(item.RFQSupplierId);
                rFQSupplierPartQuote.RFQPartId = item.Id;
                rFQSupplierPartQuote.Id = item.SupplierPartQuoteId;
                rFQSupplierPartQuote.Currency = item.Currency;
                rFQSupplierPartQuote.MaterialCost = item.MaterialCost;
                rFQSupplierPartQuote.ProcessCost = item.ProcessCost ?? 0;
                rFQSupplierPartQuote.UnitPrice = item.UnitPrice;
                rFQSupplierPartQuote.ToolingCost = item.ToolingCost;
                rFQSupplierPartQuote.RawMaterialPriceAssumed = item.RawMaterialPriceAssumed;
                rFQSupplierPartQuote.QuoteAttachmentFilePath = !string.IsNullOrEmpty(item.QuoteAttachmentFilePath) ?
                                                                Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"]) + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + item.QuoteAttachmentFilePath
                                                                : string.Empty;
                rFQSupplierPartQuote.MachiningCost = item.MachiningCost;
                rFQSupplierPartQuote.OtherProcessCost = item.OtherProcessCost;
                rFQSupplierPartQuote.Remarks = item.Remarks;
                rFQSupplierPartQuote.SupplierToolingLeadtime = item.SupplierToolingLeadtime;
                rFQSupplierPartQuote.ToolingWarranty = item.ToolingWarranty;
                rFQSupplierPartQuote.NoOfCavities = item.NoOfCavities ?? 0;
                rFQSupplierPartQuote.MinOrderQty = item.MinOrderQty;
                rFQSupplierPartQuote.MOQConfirmation = item.MOQConfirmation;


                rFQSupplierPartQuote.Manufacturer = item.Manufacturer;
                rFQSupplierPartQuote.ManufacturerId = item.ManufacturerId;


                rFQSupplierPartQuote.ExchangeRate = item.ExchangeRate;
                rFQSupplierPartQuote.UpdatedDate = item.UpdatedDate;
                #endregion

                #region "Assign details for the rfq raw material"
                rFQSupplierPartQuote.rFQdqRawMaterial = new DTO.Library.RFQ.RFQ.RFQdqRawMaterial();
                rFQSupplierPartQuote.rFQdqRawMaterial.Id = Convert.ToInt32(item.RawMaterialId);
                rFQSupplierPartQuote.rFQdqRawMaterial.RFQSupplierPartDQId = item.SupplierPartQuoteId;
                rFQSupplierPartQuote.rFQdqRawMaterial.RawMaterialDesc = item.RawMaterialDesc;
                rFQSupplierPartQuote.rFQdqRawMaterial.RawMaterialIndexUsed = item.RawMaterialIndexUsed;
                rFQSupplierPartQuote.rFQdqRawMaterial.RawMatInputInKg = item.RawMatInputInKg ?? 0;
                rFQSupplierPartQuote.rFQdqRawMaterial.RawMatCostPerKg = item.RawMatCostPerKg ?? 0;
                rFQSupplierPartQuote.rFQdqRawMaterial.RawMatTotal = item.RawMatTotal;
                rFQSupplierPartQuote.rFQdqRawMaterial.MfgRejectRate = item.MfgRejectRate;
                rFQSupplierPartQuote.rFQdqRawMaterial.MaterialLoss = item.MaterialLoss;
                #endregion

                if (Convert.ToBoolean(item.IsQuoteTypeDQ))
                {
                    #region Assign details quote properties here

                    rFQSupplierPartQuote.rFQdqPrimaryProcessConversion = new DTO.Library.RFQ.RFQ.RFQdqPrimaryProcessConversion();
                    rFQSupplierPartQuote.rFQdqPrimaryProcessConversion.Id = Convert.ToInt32(item.PrimaryProcessId);
                    rFQSupplierPartQuote.rFQdqPrimaryProcessConversion.RFQSupplierPartDQId = item.SupplierPartQuoteId;
                    rFQSupplierPartQuote.rFQdqPrimaryProcessConversion.MachineDescId = item.PPCMachineDescId;
                    rFQSupplierPartQuote.rFQdqPrimaryProcessConversion.MachineDescription = item.PPCMachineDescription;
                    rFQSupplierPartQuote.rFQdqPrimaryProcessConversion.MachineSize = item.PPCMachineSize;
                    rFQSupplierPartQuote.rFQdqPrimaryProcessConversion.CycleTime = item.PPCCycleTime ?? 0;
                    rFQSupplierPartQuote.rFQdqPrimaryProcessConversion.ManPlusMachineRatePerHour = item.PPCManPlusMachineRatePerHour ?? 0;
                    rFQSupplierPartQuote.rFQdqPrimaryProcessConversion.ProcessConversionCostPerPart = item.ProcessConversionCostPerPart;

                    rFQSupplierPartQuote.rFQdqMachining = new DTO.Library.RFQ.RFQ.RFQdqMachining();
                    rFQSupplierPartQuote.rFQdqMachining.Id = Convert.ToInt32(item.MachiningId);
                    rFQSupplierPartQuote.rFQdqMachining.RFQSupplierPartDQId = item.SupplierPartQuoteId;
                    rFQSupplierPartQuote.rFQdqMachining.MachiningDescId = item.MMachiningDescId;
                    rFQSupplierPartQuote.rFQdqMachining.MachiningDescription = item.MMachiningDescription;
                    rFQSupplierPartQuote.rFQdqMachining.CycleTime = item.MCycleTime;
                    rFQSupplierPartQuote.rFQdqMachining.ManPlusMachineRatePerHour = item.MManPlusMachineRatePerHour;
                    rFQSupplierPartQuote.rFQdqMachining.MachiningCostPerPart = item.MachiningCostPerPart;

                    rFQSupplierPartQuote.rFQdqMachiningSecondaryOperation = new DTO.Library.RFQ.RFQ.RFQdqMachiningSecondaryOperation();
                    rFQSupplierPartQuote.rFQdqMachiningSecondaryOperation.Id = Convert.ToInt32(item.MachiningSecOperationId);
                    rFQSupplierPartQuote.rFQdqMachiningSecondaryOperation.RFQSupplierPartDQId = item.SupplierPartQuoteId;
                    rFQSupplierPartQuote.rFQdqMachiningSecondaryOperation.SecondaryOperationDescId = item.MSOSecondaryOperationDescId;
                    rFQSupplierPartQuote.rFQdqMachiningSecondaryOperation.SecondaryOperationDescription = item.MSOSecondaryOperationDescription;
                    rFQSupplierPartQuote.rFQdqMachiningSecondaryOperation.CycleTime = item.MSOCycleTime;
                    rFQSupplierPartQuote.rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour = item.MSOManPlusMachineRatePerHour;
                    rFQSupplierPartQuote.rFQdqMachiningSecondaryOperation.SecondaryCostPerPart = item.MSOSecondaryCostPerPart;

                    rFQSupplierPartQuote.rFQdqMachiningOtherOperation = new DTO.Library.RFQ.RFQ.RFQdqMachiningOtherOperation();
                    rFQSupplierPartQuote.rFQdqMachiningOtherOperation.Id = Convert.ToInt32(item.MachiningOtherOperationId);
                    rFQSupplierPartQuote.rFQdqMachiningOtherOperation.RFQSupplierPartDQId = item.SupplierPartQuoteId;
                    rFQSupplierPartQuote.rFQdqMachiningOtherOperation.SecondaryOperationDescId = item.MOOSecondaryOperationDescId;
                    rFQSupplierPartQuote.rFQdqMachiningOtherOperation.SecondaryOperationDescription = item.MOOSecondaryOperationDescription;
                    rFQSupplierPartQuote.rFQdqMachiningOtherOperation.CycleTime = item.MOOCycleTime;
                    rFQSupplierPartQuote.rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour = item.MOOManPlusMachineRatePerHour;
                    rFQSupplierPartQuote.rFQdqMachiningOtherOperation.SecondaryCostPerPart = item.MOOSecondaryCostPerPart;

                    rFQSupplierPartQuote.rFQdqSurfaceTreatment = new DTO.Library.RFQ.RFQ.RFQdqSurfaceTreatment();
                    rFQSupplierPartQuote.rFQdqSurfaceTreatment.Id = Convert.ToInt32(item.SurfaceTreatmentId);
                    rFQSupplierPartQuote.rFQdqSurfaceTreatment.RFQSupplierPartDQId = item.SupplierPartQuoteId;
                    rFQSupplierPartQuote.rFQdqSurfaceTreatment.CoatingTypeId = item.STCoatingTypeId;
                    rFQSupplierPartQuote.rFQdqSurfaceTreatment.CoatingType = item.STCoatingType;
                    rFQSupplierPartQuote.rFQdqSurfaceTreatment.PartsPerHour = item.STPartsPerHour;
                    rFQSupplierPartQuote.rFQdqSurfaceTreatment.ManPlusMachineRatePerHour = item.STManPlusMachineRatePerHour;
                    rFQSupplierPartQuote.rFQdqSurfaceTreatment.CoatingCostPerHour = item.STCoatingCostPerHour;

                    rFQSupplierPartQuote.rFQdqOverhead = new DTO.Library.RFQ.RFQ.RFQdqOverhead();
                    rFQSupplierPartQuote.rFQdqOverhead.Id = Convert.ToInt32(item.OverheadId);
                    rFQSupplierPartQuote.rFQdqOverhead.RFQSupplierPartDQId = item.SupplierPartQuoteId;
                    rFQSupplierPartQuote.rFQdqOverhead.InventoryCarryingCost = item.OverheadInventoryCarryingCost;
                    rFQSupplierPartQuote.rFQdqOverhead.Packing = item.OverheadPacking;
                    rFQSupplierPartQuote.rFQdqOverhead.LocalFreightToPort = item.OverheadLocalFreightToPort;
                    rFQSupplierPartQuote.rFQdqOverhead.ProfitAndSGA = item.OverheadProfitAndSGA;
                    rFQSupplierPartQuote.rFQdqOverhead.OverheadPercentPiecePrice = item.OverheadPercentPiecePrice;
                    rFQSupplierPartQuote.rFQdqOverhead.PackagingMaterial = item.OverheadPackagingMaterial;
                    #endregion
                }
                lstRFQSupplierPartQuote.Add(rFQSupplierPartQuote);
            }
            return lstRFQSupplierPartQuote;
        }

        private static List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> setSupplierQuoteProperties(List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> lstRFQSupplierPartQuote, List<Data.Library.SearchSupplierPartForSupplierQuoteHistory_Result> rfqSupplierListHistory, bool IsHistory)
        {
            DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ rFQSupplierPartQuote;
            //setup total records
            foreach (var item in rfqSupplierListHistory)
            {
                rFQSupplierPartQuote = new DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ();
                #region "Extra properties"
                rFQSupplierPartQuote.CustomerPartNo = item.CustomerPartNo;
                rFQSupplierPartQuote.PartDescription = item.PartDescription;
                rFQSupplierPartQuote.AdditionalPartDesc = item.AdditionalPartDescription;
                rFQSupplierPartQuote.EstimatedQty = item.EstimatedQty;
                rFQSupplierPartQuote.Specifications = new List<DTO.Library.RFQ.RFQ.RFQPartAttachment>();
                rFQSupplierPartQuote.MaterialType = item.MaterialType;
                rFQSupplierPartQuote.PartWeightKG = item.PartWeightKG;
                rFQSupplierPartQuote.RfqId = item.RFQId;
                rFQSupplierPartQuote.RFQDate = item.RFQDate;
                rFQSupplierPartQuote.QuoteDueDate = item.QuoteDueDate;
                rFQSupplierPartQuote.RFQRemarks = item.RFQRemarks;
                rFQSupplierPartQuote.SupplierId = item.SupplierId;
                rFQSupplierPartQuote.CompanyName = item.CompanyName;
                rFQSupplierPartQuote.IsMandatory = false;
                rFQSupplierPartQuote.Minimum = 0;
                rFQSupplierPartQuote.IsQuoteTypeDQ = Convert.ToBoolean(item.IsQuoteTypeDQ);
                rFQSupplierPartQuote.NoQuote = item.NoQuote;
                rFQSupplierPartQuote.IsHistory = IsHistory;
                #endregion

                #region Supplier part quote tables data
                rFQSupplierPartQuote.RFQSupplierId = Convert.ToInt32(item.RFQSupplierId);
                rFQSupplierPartQuote.RFQPartId = item.Id;
                rFQSupplierPartQuote.Id = item.SupplierPartQuoteId;
                rFQSupplierPartQuote.Currency = item.Currency;
                rFQSupplierPartQuote.MaterialCost = item.MaterialCost;
                rFQSupplierPartQuote.ProcessCost = item.ProcessCost;
                rFQSupplierPartQuote.UnitPrice = item.UnitPrice;
                rFQSupplierPartQuote.ToolingCost = item.ToolingCost;
                rFQSupplierPartQuote.RawMaterialPriceAssumed = item.RawMaterialPriceAssumed;
                rFQSupplierPartQuote.QuoteAttachmentFilePath = !string.IsNullOrEmpty(item.QuoteAttachmentFilePath) ?
                                                                Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"]) + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + item.QuoteAttachmentFilePath
                                                                : string.Empty;
                rFQSupplierPartQuote.MachiningCost = item.MachiningCost;
                rFQSupplierPartQuote.OtherProcessCost = item.OtherProcessCost;
                rFQSupplierPartQuote.Remarks = item.Remarks;
                rFQSupplierPartQuote.SupplierToolingLeadtime = item.SupplierToolingLeadtime;
                rFQSupplierPartQuote.ToolingWarranty = item.ToolingWarranty;
                rFQSupplierPartQuote.NoOfCavities = item.NoOfCavities;
                rFQSupplierPartQuote.MinOrderQty = item.MinOrderQty;
                rFQSupplierPartQuote.MOQConfirmation = item.MOQConfirmation;
                rFQSupplierPartQuote.Manufacturer = item.Manufacturer;
                rFQSupplierPartQuote.ManufacturerId = item.ManufacturerId;
                rFQSupplierPartQuote.ExchangeRate = item.ExchangeRate;
                rFQSupplierPartQuote.UpdatedDate = item.UpdatedDate;
                rFQSupplierPartQuote.UpdatedDateString = item.UpdatedDate.Value.FormatDateInMediumDate();
                #endregion

                #region "Assign details for the rfq raw material"
                rFQSupplierPartQuote.rFQdqRawMaterial = new DTO.Library.RFQ.RFQ.RFQdqRawMaterial();
                rFQSupplierPartQuote.rFQdqRawMaterial.Id = Convert.ToInt32(item.RawMaterialId);
                rFQSupplierPartQuote.rFQdqRawMaterial.RFQSupplierPartDQId = item.SupplierPartQuoteId;
                rFQSupplierPartQuote.rFQdqRawMaterial.RawMaterialDesc = item.RawMaterialDesc;
                rFQSupplierPartQuote.rFQdqRawMaterial.RawMaterialIndexUsed = item.RawMaterialIndexUsed;
                rFQSupplierPartQuote.rFQdqRawMaterial.RawMatInputInKg = item.RawMatInputInKg;
                rFQSupplierPartQuote.rFQdqRawMaterial.RawMatCostPerKg = item.RawMatCostPerKg;
                rFQSupplierPartQuote.rFQdqRawMaterial.RawMatTotal = item.RawMatTotal;
                rFQSupplierPartQuote.rFQdqRawMaterial.MfgRejectRate = item.MfgRejectRate;
                rFQSupplierPartQuote.rFQdqRawMaterial.MaterialLoss = item.MaterialLoss;
                #endregion

                if (Convert.ToBoolean(item.IsQuoteTypeDQ))
                {
                    #region Assign details quote properties here
                    rFQSupplierPartQuote.rFQdqRawMaterial = new DTO.Library.RFQ.RFQ.RFQdqRawMaterial();
                    rFQSupplierPartQuote.rFQdqRawMaterial.Id = Convert.ToInt32(item.RawMaterialId);
                    rFQSupplierPartQuote.rFQdqRawMaterial.RFQSupplierPartDQId = item.SupplierPartQuoteId;
                    rFQSupplierPartQuote.rFQdqRawMaterial.RawMaterialDesc = item.RawMaterialDesc;
                    rFQSupplierPartQuote.rFQdqRawMaterial.RawMaterialIndexUsed = item.RawMaterialIndexUsed;
                    rFQSupplierPartQuote.rFQdqRawMaterial.RawMatInputInKg = item.RawMatInputInKg;
                    rFQSupplierPartQuote.rFQdqRawMaterial.RawMatCostPerKg = item.RawMatCostPerKg;
                    rFQSupplierPartQuote.rFQdqRawMaterial.RawMatTotal = item.RawMatTotal;
                    rFQSupplierPartQuote.rFQdqRawMaterial.MfgRejectRate = item.MfgRejectRate;
                    rFQSupplierPartQuote.rFQdqRawMaterial.MaterialLoss = item.MaterialLoss;

                    rFQSupplierPartQuote.rFQdqPrimaryProcessConversion = new DTO.Library.RFQ.RFQ.RFQdqPrimaryProcessConversion();
                    rFQSupplierPartQuote.rFQdqPrimaryProcessConversion.Id = Convert.ToInt32(item.PrimaryProcessId);
                    rFQSupplierPartQuote.rFQdqPrimaryProcessConversion.RFQSupplierPartDQId = item.SupplierPartQuoteId;
                    rFQSupplierPartQuote.rFQdqPrimaryProcessConversion.MachineDescId = item.PPCMachineDescId;
                    rFQSupplierPartQuote.rFQdqPrimaryProcessConversion.MachineDescription = item.PPCMachineDescription;
                    rFQSupplierPartQuote.rFQdqPrimaryProcessConversion.MachineSize = item.PPCMachineSize;
                    rFQSupplierPartQuote.rFQdqPrimaryProcessConversion.CycleTime = item.PPCCycleTime;
                    rFQSupplierPartQuote.rFQdqPrimaryProcessConversion.ManPlusMachineRatePerHour = item.PPCManPlusMachineRatePerHour;
                    rFQSupplierPartQuote.rFQdqPrimaryProcessConversion.ProcessConversionCostPerPart = item.ProcessConversionCostPerPart;

                    rFQSupplierPartQuote.rFQdqMachining = new DTO.Library.RFQ.RFQ.RFQdqMachining();
                    rFQSupplierPartQuote.rFQdqMachining.Id = Convert.ToInt32(item.MachiningId);
                    rFQSupplierPartQuote.rFQdqMachining.RFQSupplierPartDQId = item.SupplierPartQuoteId;
                    rFQSupplierPartQuote.rFQdqMachining.MachiningDescId = item.MMachiningDescId;
                    rFQSupplierPartQuote.rFQdqMachining.MachiningDescription = item.MMachiningDescription;
                    rFQSupplierPartQuote.rFQdqMachining.CycleTime = item.MCycleTime;
                    rFQSupplierPartQuote.rFQdqMachining.ManPlusMachineRatePerHour = item.MManPlusMachineRatePerHour;
                    rFQSupplierPartQuote.rFQdqMachining.MachiningCostPerPart = item.MachiningCostPerPart;

                    rFQSupplierPartQuote.rFQdqMachiningSecondaryOperation = new DTO.Library.RFQ.RFQ.RFQdqMachiningSecondaryOperation();
                    rFQSupplierPartQuote.rFQdqMachiningSecondaryOperation.Id = Convert.ToInt32(item.MachiningSecOperationId);
                    rFQSupplierPartQuote.rFQdqMachiningSecondaryOperation.RFQSupplierPartDQId = item.SupplierPartQuoteId;
                    rFQSupplierPartQuote.rFQdqMachiningSecondaryOperation.SecondaryOperationDescId = item.MSOSecondaryOperationDescId;
                    rFQSupplierPartQuote.rFQdqMachiningSecondaryOperation.SecondaryOperationDescription = item.MSOSecondaryOperationDescription;
                    rFQSupplierPartQuote.rFQdqMachiningSecondaryOperation.CycleTime = item.MSOCycleTime;
                    rFQSupplierPartQuote.rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour = item.MSOManPlusMachineRatePerHour;
                    rFQSupplierPartQuote.rFQdqMachiningSecondaryOperation.SecondaryCostPerPart = item.MSOSecondaryCostPerPart;

                    rFQSupplierPartQuote.rFQdqMachiningOtherOperation = new DTO.Library.RFQ.RFQ.RFQdqMachiningOtherOperation();
                    rFQSupplierPartQuote.rFQdqMachiningOtherOperation.Id = Convert.ToInt32(item.MachiningOtherOperationId);
                    rFQSupplierPartQuote.rFQdqMachiningOtherOperation.RFQSupplierPartDQId = item.SupplierPartQuoteId;
                    rFQSupplierPartQuote.rFQdqMachiningOtherOperation.SecondaryOperationDescId = item.MOOSecondaryOperationDescId;
                    rFQSupplierPartQuote.rFQdqMachiningOtherOperation.SecondaryOperationDescription = item.MOOSecondaryOperationDescription;
                    rFQSupplierPartQuote.rFQdqMachiningOtherOperation.CycleTime = item.MOOCycleTime;
                    rFQSupplierPartQuote.rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour = item.MOOManPlusMachineRatePerHour;
                    rFQSupplierPartQuote.rFQdqMachiningOtherOperation.SecondaryCostPerPart = item.MOOSecondaryCostPerPart;

                    rFQSupplierPartQuote.rFQdqSurfaceTreatment = new DTO.Library.RFQ.RFQ.RFQdqSurfaceTreatment();
                    rFQSupplierPartQuote.rFQdqSurfaceTreatment.Id = Convert.ToInt32(item.SurfaceTreatmentId);
                    rFQSupplierPartQuote.rFQdqSurfaceTreatment.RFQSupplierPartDQId = item.SupplierPartQuoteId;
                    rFQSupplierPartQuote.rFQdqSurfaceTreatment.CoatingTypeId = item.STCoatingTypeId;
                    rFQSupplierPartQuote.rFQdqSurfaceTreatment.CoatingType = item.STCoatingType;
                    rFQSupplierPartQuote.rFQdqSurfaceTreatment.PartsPerHour = item.STPartsPerHour;
                    rFQSupplierPartQuote.rFQdqSurfaceTreatment.ManPlusMachineRatePerHour = item.STManPlusMachineRatePerHour;
                    rFQSupplierPartQuote.rFQdqSurfaceTreatment.CoatingCostPerHour = item.STCoatingCostPerHour;

                    rFQSupplierPartQuote.rFQdqOverhead = new DTO.Library.RFQ.RFQ.RFQdqOverhead();
                    rFQSupplierPartQuote.rFQdqOverhead.Id = Convert.ToInt32(item.OverheadId);
                    rFQSupplierPartQuote.rFQdqOverhead.RFQSupplierPartDQId = item.SupplierPartQuoteId;
                    rFQSupplierPartQuote.rFQdqOverhead.InventoryCarryingCost = item.OverheadInventoryCarryingCost;
                    rFQSupplierPartQuote.rFQdqOverhead.Packing = item.OverheadPacking;
                    rFQSupplierPartQuote.rFQdqOverhead.LocalFreightToPort = item.OverheadLocalFreightToPort;
                    rFQSupplierPartQuote.rFQdqOverhead.ProfitAndSGA = item.OverheadProfitAndSGA;
                    rFQSupplierPartQuote.rFQdqOverhead.OverheadPercentPiecePrice = item.OverheadPercentPiecePrice;
                    rFQSupplierPartQuote.rFQdqOverhead.PackagingMaterial = item.OverheadPackagingMaterial;
                    #endregion
                }
                lstRFQSupplierPartQuote.Add(rFQSupplierPartQuote);
            }
            return lstRFQSupplierPartQuote;
        }

        #region Create SQ / DQ file
        public NPE.Core.ITypedResponse<bool?> exportToExcelSupplierQuote(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging)
        {
            string filePath = string.Empty;
            try
            {
                if (paging.Criteria.IsQuoteTypeDQ)
                {
                    filePath = exportToExcelSupplierQuoteDQ(paging);
                }
                else
                {
                    var context = HttpContext.Current;

                    Supplier.Suppliers supplierObj = new Supplier.Suppliers();
                    DTO.Library.RFQ.Supplier.Suppliers supplierItem = supplierObj.FindById(paging.Criteria.SupplierId).Result;

                    List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> rspqDetails = this.GetSupplierQuoteList(paging).Result.Where(a => a.IsHistory == false).ToList();

                    ExcelFile ef = new ExcelFile();
                    ExcelFile myExcelFile = new ExcelFile();
                    ExcelWorksheet excWsheet = myExcelFile.Worksheets.Add("Parts List");

                    try
                    {
                        // Frozen Columns (first column is frozen)
                        excWsheet.Panes = new WorksheetPanes(PanesState.Frozen, 1, 0, "B1", PanePosition.TopRight);

                        CellRange cr = excWsheet.Cells.GetSubrange("A5", "A6");
                        cr.Merged = true;
                        cr.Style.Font.Weight = ExcelFont.BoldWeight;
                        cr.Merged = false;
                        cr = null;

                        excWsheet.Cells[4, 0].Value = "P O BOX 401";
                        excWsheet.Cells[5, 0].Value = "Lewis Center, OH 43035";

                        cr = excWsheet.Cells.GetSubrange("A8", "B9");
                        cr.Merged = true;
                        cr.Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                        cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                        cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                        cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                        cr.Merged = false;
                        cr = null;

                        excWsheet.Cells[7, 0].Value = "VENDOR NAME:";
                        excWsheet.Cells[7, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                        excWsheet.Cells[7, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                        excWsheet.Cells[7, 1].Value = supplierItem.CompanyName.ToUpper();

                        excWsheet.Cells[8, 0].Value = "RFQ # ";
                        excWsheet.Cells[8, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                        excWsheet.Cells[8, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                        excWsheet.Cells[8, 1].Value = paging.Criteria.RFQId;

                        cr = excWsheet.Cells.GetSubrange("A11", "U11");
                        cr.Merged = true;
                        cr.Style.Font.Weight = ExcelFont.BoldWeight;
                        cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                        cr.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                        cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                        cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                        cr = null;

                        excWsheet.Cells[10, 0].Value = "Supplier Quote";

                        cr = excWsheet.Cells.GetSubrange("A13", "B16");
                        cr.Merged = true;
                        cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                        cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                        cr.Style.WrapText = true;
                        cr.Merged = false;
                        cr = null;

                        excWsheet.Cells[12, 0].Value = "Exchange Rate 1 USD =";
                        excWsheet.Cells[12, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                        excWsheet.Cells[12, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                        excWsheet.Cells[12, 1].Style.NumberFormat = "0.000";
                        excWsheet.Cells[12, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        excWsheet.Cells[12, 1].Value = rspqDetails[0].ExchangeRate;

                        excWsheet.Cells[13, 0].Value = "Currency =";
                        excWsheet.Cells[13, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                        excWsheet.Cells[13, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                        excWsheet.Cells[13, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        excWsheet.Cells[13, 1].Value = rspqDetails[0].Currency;

                        excWsheet.Cells[14, 0].Value = "Raw Material Price Assumed \n(Please include unit of measure…i.e. $1.000/kg) =";
                        excWsheet.Cells[14, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                        excWsheet.Cells[14, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                        excWsheet.Cells[14, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        excWsheet.Cells[14, 1].Value = rspqDetails[0].RawMaterialPriceAssumed;

                        excWsheet.Cells[15, 0].Value = "Supplier Remarks";
                        excWsheet.Cells[15, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                        excWsheet.Cells[15, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                        excWsheet.Cells[15, 1].Value = rspqDetails[0].Remarks;


                        cr = excWsheet.Cells.GetSubrange("A18", "U18");
                        cr.Merged = true;
                        cr.Style.Font.Weight = ExcelFont.BoldWeight;
                        cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                        cr.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                        cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                        cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                        cr = null;

                        excWsheet.Cells[17, 0].Value = "Part(s) List";

                        cr = excWsheet.Cells.GetSubrange("A19", "U19");
                        cr.Merged = true;
                        cr.Style.Font.Weight = ExcelFont.BoldWeight;
                        cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                        cr.Style.WrapText = true;
                        cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                        cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                        cr.Merged = false;
                        cr = null;

                        excWsheet.Cells[18, 0].Value = "Part No.";
                        excWsheet.Cells[18, 1].Value = "Description";
                        excWsheet.Cells[18, 2].Value = "Additional Description";
                        excWsheet.Cells[18, 3].Value = "Annual Quantity";
                        excWsheet.Cells[18, 4].Value = "Manufacturer";
                        excWsheet.Cells[18, 5].Value = "Min. Order Qty";
                        excWsheet.Cells[18, 6].Value = "Raw Material Description";
                        excWsheet.Cells[18, 7].Value = "Raw Material Index Used";
                        excWsheet.Cells[18, 8].Value = "Raw Mat'l Input(Gross wt in kg)";
                        excWsheet.Cells[18, 9].Value = "Part Weight (KG)";
                        excWsheet.Cells[18, 10].Value = "Raw Mat'l Cost (USD)(Per kg)";
                        excWsheet.Cells[18, 11].Value = "Material Loss (%)";
                        excWsheet.Cells[18, 12].Value = "Raw Mat'l Total";
                        excWsheet.Cells[18, 13].Value = "Conversion Cost (USD)";
                        excWsheet.Cells[18, 14].Value = "Machining Cost (USD)";
                        excWsheet.Cells[18, 15].Value = "Other Process Cost (USD)";
                        excWsheet.Cells[18, 16].Value = "Final Piece Price / Part";
                        excWsheet.Cells[18, 17].Value = "No. Of Cavities";
                        excWsheet.Cells[18, 18].Value = "Tooling Cost (USD) \n(Must include cost of 6 PPAP samples)";
                        excWsheet.Cells[18, 19].Value = "Tooling Warranty";
                        excWsheet.Cells[18, 20].Value = "Supplier Tooling Leadtime";


                        excWsheet.Cells[18, 3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        excWsheet.Cells[18, 5].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        excWsheet.Cells[18, 8].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        excWsheet.Cells[18, 9].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        excWsheet.Cells[18, 10].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        excWsheet.Cells[18, 11].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        excWsheet.Cells[18, 12].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        excWsheet.Cells[18, 13].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        excWsheet.Cells[18, 14].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        excWsheet.Cells[18, 15].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        excWsheet.Cells[18, 16].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        excWsheet.Cells[18, 17].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                        excWsheet.Cells[18, 18].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;

                        excWsheet.Columns[0].Width = 35 * 256;

                        int counter = 19;
                        int lastrow = counter + rspqDetails.Count;

                        if (rspqDetails.Count > 0)
                        {
                            foreach (var item in rspqDetails)
                            {

                                cr = excWsheet.Cells.GetSubrange("A" + (counter + 1), "U" + (counter + 1)); cr.Merged = true;
                                cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                                cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, Color.Black, LineStyle.Thin);
                                cr.Merged = false; cr = null; //Vertical Allignment Style set to "Top"

                                excWsheet.Cells[counter, 3].Style.NumberFormat = "0";
                                excWsheet.Cells[counter, 5].Style.NumberFormat = "0";
                                excWsheet.Cells[counter, 8].Style.NumberFormat = "0.000";
                                excWsheet.Cells[counter, 9].Style.NumberFormat = "0.000";
                                excWsheet.Cells[counter, 10].Style.NumberFormat = "0.000";
                                excWsheet.Cells[counter, 11].Style.NumberFormat = "0.000";
                                excWsheet.Cells[counter, 12].Style.NumberFormat = "0.000";
                                excWsheet.Cells[counter, 14].Style.NumberFormat = "0.000";
                                excWsheet.Cells[counter, 15].Style.NumberFormat = "0.000";
                                excWsheet.Cells[counter, 16].Style.NumberFormat = "0.000";
                                excWsheet.Cells[counter, 17].Style.NumberFormat = "0";
                                excWsheet.Cells[counter, 18].Style.NumberFormat = "0.000";


                                excWsheet.Cells[counter, 3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                excWsheet.Cells[counter, 5].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                excWsheet.Cells[counter, 8].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                excWsheet.Cells[counter, 9].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                excWsheet.Cells[counter, 10].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                excWsheet.Cells[counter, 11].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                excWsheet.Cells[counter, 12].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                excWsheet.Cells[counter, 13].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                excWsheet.Cells[counter, 14].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                excWsheet.Cells[counter, 15].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                excWsheet.Cells[counter, 16].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                excWsheet.Cells[counter, 17].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                                excWsheet.Cells[counter, 18].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;

                                excWsheet.Cells[counter, 0].Value = item.CustomerPartNo;
                                excWsheet.Cells[counter, 0].Style.WrapText = true;
                                excWsheet.Cells[counter, 1].Value = item.PartDescription;
                                excWsheet.Cells[counter, 1].Style.WrapText = true;
                                excWsheet.Cells[counter, 2].Value = item.AdditionalPartDesc;
                                excWsheet.Cells[counter, 2].Style.WrapText = true;
                                excWsheet.Cells[counter, 3].Value = item.EstimatedQty;
                                excWsheet.Cells[counter, 4].Value = item.Manufacturer;
                                excWsheet.Cells[counter, 5].Value = item.MinOrderQty;
                                excWsheet.Cells[counter, 6].Value = item.MaterialType;
                                excWsheet.Cells[counter, 7].Value = item.rFQdqRawMaterial.RawMaterialIndexUsed;
                                excWsheet.Cells[counter, 8].Value = item.rFQdqRawMaterial.RawMatInputInKg;
                                excWsheet.Cells[counter, 9].Value = item.PartWeightKG;
                                excWsheet.Cells[counter, 10].Value = item.rFQdqRawMaterial.RawMatCostPerKg;
                                excWsheet.Cells[counter, 11].Value = item.rFQdqRawMaterial.MaterialLoss;
                                excWsheet.Cells[counter, 12].Value = item.rFQdqRawMaterial.RawMatTotal;
                                excWsheet.Cells[counter, 13].Value = item.ProcessCost;
                                excWsheet.Cells[counter, 14].Value = item.MachiningCost;
                                excWsheet.Cells[counter, 15].Value = item.OtherProcessCost;
                                excWsheet.Cells[counter, 16].Value = item.UnitPrice;
                                excWsheet.Cells[counter, 17].Value = item.NoOfCavities;
                                excWsheet.Cells[counter, 18].Value = item.ToolingCost;
                                excWsheet.Cells[counter, 19].Value = item.ToolingWarranty;
                                excWsheet.Cells[counter, 19].Style.WrapText = true;
                                excWsheet.Cells[counter, 20].Value = item.SupplierToolingLeadtime;
                                excWsheet.Cells[counter, 20].Style.WrapText = true;

                                counter++;
                            }
                        }

                        excWsheet.Columns[1].AutoFit();
                        for (int i = 2; i <= 20; i++)
                        {
                            excWsheet.Columns[i].Width = 20 * 10 * 30;
                        }

                        //MES Logo
                        cr = excWsheet.Cells.GetSubrange("A1", "U4"); cr.Merged = true;
                        cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.White, System.Drawing.Color.White);
                        cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;
                        excWsheet.Pictures.Add(System.Web.HttpContext.Current.Server.MapPath(Constants.IMAGEFOLDER) + "MESlogoPdf.png", GemBox.Spreadsheet.PositioningMode.FreeFloating, new AnchorCell(excWsheet.Columns[0], excWsheet.Rows[0], true), new AnchorCell(excWsheet.Columns[0], excWsheet.Rows[3], false));
                        //ends here
                        string generatedFileName = supplierItem.CompanyName.Trim() + "_" + paging.Criteria.RFQId + "_" + System.DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx";

                        filePath = System.Web.HttpContext.Current.Server.MapPath(Constants.RFQSUPPLIERQUOTEFOLDER) + generatedFileName;
                        if (!System.IO.Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(Constants.RFQSUPPLIERQUOTEFOLDER)))
                            System.IO.Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(Constants.RFQSUPPLIERQUOTEFOLDER));
                        else
                        {
                            if (File.Exists(filePath))
                                File.Delete(filePath);
                        }
                        myExcelFile.SaveXlsx(filePath);
                        filePath = System.Configuration.ConfigurationManager.AppSettings["ApiURL"] + (Constants.RFQSUPPLIERQUOTEFILEPATH) + generatedFileName;
                    }
                    catch (Exception ex)
                    {
                        return FailedBoolResponse(ex.Message);
                    }
                    finally
                    {
                        excWsheet = null;
                        ef.ClosePreservedXlsx();
                    }
                }

            }
            catch (Exception ex)//Error
            {
                return FailedBoolResponse(ex.Message);
            }
            return SuccessBoolResponse(filePath);
        }

        public string exportToExcelSupplierQuoteDQ(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging)
        {
            var context = HttpContext.Current;
            string filepath = string.Empty;
            try
            {
                Supplier.Suppliers supplierObj = new Supplier.Suppliers();
                DTO.Library.RFQ.Supplier.Suppliers supplierItem = supplierObj.FindById(paging.Criteria.SupplierId).Result;

                List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> rspqDetails = this.GetSupplierQuoteList(paging).Result.Where(a => a.IsHistory == false).ToList();

                ExcelFile ef = new ExcelFile();
                ExcelFile myExcelFile = new ExcelFile();
                ExcelWorksheet excWsheet = myExcelFile.Worksheets.Add("Parts List");

                try
                {
                    // Frozen Columns (first column is frozen)
                    excWsheet.Panes = new WorksheetPanes(PanesState.Frozen, 1, 0, "B1", PanePosition.TopRight);

                    CellRange cr = excWsheet.Cells.GetSubrange("A5", "A6"); cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight; cr.Merged = false; cr = null;

                    excWsheet.Cells[4, 0].Value = "P O BOX 401";
                    excWsheet.Cells[5, 0].Value = "Lewis Center, OH 43035";

                    cr = excWsheet.Cells.GetSubrange("A8", "B9");
                    cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Merged = false;
                    cr = null;

                    excWsheet.Cells[7, 0].Value = "VENDOR NAME:";
                    excWsheet.Cells[7, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[7, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[7, 1].Value = supplierItem.CompanyName.ToUpper();

                    excWsheet.Cells[8, 0].Value = "RFQ # ";
                    excWsheet.Cells[8, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[8, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[8, 1].Value = paging.Criteria.RFQId;

                    cr = excWsheet.Cells.GetSubrange("A11", "AS11"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    excWsheet.Cells[10, 0].Value = "Supplier Quote";

                    cr = excWsheet.Cells.GetSubrange("A13", "B16");
                    cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Merged = false;
                    cr = null;

                    excWsheet.Cells[12, 0].Value = "Exchange Rate 1 USD =";
                    excWsheet.Cells[12, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[12, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[12, 1].Style.NumberFormat = "0.000";
                    excWsheet.Cells[12, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[12, 1].Value = rspqDetails[0].ExchangeRate;

                    excWsheet.Cells[13, 0].Value = "Currency =";
                    excWsheet.Cells[13, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[13, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[13, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[13, 1].Value = rspqDetails[0].Currency;

                    excWsheet.Cells[14, 0].Value = "Raw Material Price Assumed \n(Please include unit of measure…i.e. $1.000/kg) =";
                    excWsheet.Cells[14, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[14, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[14, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[14, 1].Value = rspqDetails[0].RawMaterialPriceAssumed;

                    excWsheet.Cells[15, 0].Value = "Supplier Remarks";
                    excWsheet.Cells[15, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[15, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[15, 1].Value = rspqDetails[0].Remarks;

                    cr = excWsheet.Cells.GetSubrange("A18", "AS18");
                    cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr = null;
                    excWsheet.Cells[17, 0].Value = "Part(s) List";


                    /*Part Detail*/
                    cr = excWsheet.Cells.GetSubrange("A19", "F19");
                    cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr = null;
                    excWsheet.Cells[18, 0].Value = "Part Detail";

                    /*Raw Material*/
                    cr = excWsheet.Cells.GetSubrange("G19", "M19");
                    cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr = null;
                    excWsheet.Cells[18, 6].Value = "Raw Material";

                    /*Primary Process/Conversion*/
                    cr = excWsheet.Cells.GetSubrange("N19", "R19");
                    cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr = null;
                    excWsheet.Cells[18, 13].Value = "Primary Process/Conversion";


                    /*Machining*/
                    cr = excWsheet.Cells.GetSubrange("S19", "V19");
                    cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr = null;
                    excWsheet.Cells[18, 18].Value = "Machining";


                    /*Machining 2/Secondary Operation*/
                    cr = excWsheet.Cells.GetSubrange("W19", "Z19");
                    cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr = null;
                    excWsheet.Cells[18, 22].Value = "Machining 2/Secondary Operation";


                    /*Machining 3/Other Operation*/
                    cr = excWsheet.Cells.GetSubrange("AA19", "AD19");
                    cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr = null;
                    excWsheet.Cells[18, 26].Value = "Machining 3/Other Operation";

                    /*Surface Treatment*/
                    cr = excWsheet.Cells.GetSubrange("AE19", "AH19");
                    cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr = null;
                    excWsheet.Cells[18, 30].Value = "Surface Treatment";

                    /*Overhead*/
                    cr = excWsheet.Cells.GetSubrange("AI19", "AO19");
                    cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr = null;
                    excWsheet.Cells[18, 34].Value = "Overhead";

                    /*Overhead*/
                    cr = excWsheet.Cells.GetSubrange("AP19", "AS19");
                    cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr = null;
                    excWsheet.Cells[18, 41].Value = "Tooling";

                    cr = excWsheet.Cells.GetSubrange("A20", "AS20");
                    cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.WrapText = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond); cr.Merged = false;
                    cr = null;


                    excWsheet.Cells[19, 0].Value = "Part No.";
                    excWsheet.Cells[19, 1].Value = "Description";
                    excWsheet.Cells[19, 2].Value = "Additional Description";
                    excWsheet.Cells[19, 3].Value = "Annual Quantity";
                    excWsheet.Cells[19, 4].Value = "Manufacturer";
                    excWsheet.Cells[19, 5].Value = "Min. Order Qty";

                    //Raw Material
                    excWsheet.Cells[19, 6].Value = "Raw Material Description";
                    excWsheet.Cells[19, 7].Value = "Raw Material Index Used";
                    excWsheet.Cells[19, 8].Value = "Raw Mat'l Input (Gross wt in kg)";
                    excWsheet.Cells[19, 9].Value = "Part Weight (kg)";
                    excWsheet.Cells[19, 10].Value = "Raw Mat'l Cost (per kg)";
                    excWsheet.Cells[19, 11].Value = "Material Loss (%)";
                    excWsheet.Cells[19, 12].Value = "Raw Mat'l TOTAL";

                    //Primary Process/Conversion
                    excWsheet.Cells[19, 13].Value = "Machine Description";
                    excWsheet.Cells[19, 14].Value = "Machine Size (Tons)";
                    excWsheet.Cells[19, 15].Value = "Cycle Time (sec)";
                    excWsheet.Cells[19, 16].Value = "Man + Machine Rate/Hour";
                    excWsheet.Cells[19, 17].Value = "Process/ Conversion Cost Per Part";

                    //Machining
                    excWsheet.Cells[19, 18].Value = "Machining Description";
                    excWsheet.Cells[19, 19].Value = "Cycle Time (sec)";
                    excWsheet.Cells[19, 20].Value = "Man + Machine Rate/Hour";
                    excWsheet.Cells[19, 21].Value = "Machining Cost/Part";

                    //Machining 2/Secondary Opr.
                    excWsheet.Cells[19, 22].Value = "Secondary Operation Description";
                    excWsheet.Cells[19, 23].Value = "Cycle Time (sec)";
                    excWsheet.Cells[19, 24].Value = "Man + Machine Rate/Hour";
                    excWsheet.Cells[19, 25].Value = "Secondary Cost/Part";

                    //Machining 3/Other Opr.
                    excWsheet.Cells[19, 26].Value = "Secondary Operation Description";
                    excWsheet.Cells[19, 27].Value = "Cycle Time (sec)";
                    excWsheet.Cells[19, 28].Value = "Man + Machine Rate/Hour";
                    excWsheet.Cells[19, 29].Value = "Secondary Cost/Part";

                    //Surface Treatment
                    excWsheet.Cells[19, 30].Value = "Coating Type";
                    excWsheet.Cells[19, 31].Value = "Parts Per Hour";
                    excWsheet.Cells[19, 32].Value = "Man + Machine Rate/Hour";
                    excWsheet.Cells[19, 33].Value = "Coating / Painting Cost Per Hour";

                    //Overhead
                    excWsheet.Cells[19, 34].Value = "Inventory Carrying Cost (if applicable)";
                    excWsheet.Cells[19, 35].Value = "Packaging Material";
                    excWsheet.Cells[19, 36].Value = "Packing Labour";
                    excWsheet.Cells[19, 37].Value = "FOB Port(Shipping Cost Per PC.)";
                    excWsheet.Cells[19, 38].Value = "Profit and S, G & A";
                    excWsheet.Cells[19, 39].Value = "Overhead as % of Piece Price";
                    excWsheet.Cells[19, 40].Value = "Final Piece Price / Part";

                    //Tooling
                    excWsheet.Cells[19, 41].Value = "No. of Cavities";
                    excWsheet.Cells[19, 42].Value = "Tooling Cost (USD) \n(Must include cost of 6 PPAP samples)";
                    excWsheet.Cells[19, 43].Value = "Tooling Warranty";
                    excWsheet.Cells[19, 44].Value = "Supplier Tooling Leadtime";

                    excWsheet.Cells[19, 3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 5].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 8].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 9].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 10].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 11].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 12].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 14].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 15].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 16].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 17].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 19].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 20].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 21].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 23].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 24].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 25].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 27].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 28].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 29].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;

                    excWsheet.Cells[19, 31].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 32].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 33].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 34].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 35].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 36].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 37].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 38].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 39].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 40].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 41].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[19, 42].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;

                    excWsheet.Columns[0].Width = 35 * 256;

                    int counter = 20;

                    if (rspqDetails.Count > 0)
                    {
                        foreach (var supplierDetailQuotePart in rspqDetails)
                        {
                            cr = excWsheet.Cells.GetSubrange("A" + (counter + 1), "AS" + (counter + 1));
                            cr.Merged = true;
                            cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                            cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, Color.Black, LineStyle.Thin);
                            cr.Merged = false;
                            cr = null; //Vertical Allignment Style set to "Top"

                            #region Cell Format
                            //Part Detail
                            excWsheet.Cells[counter, 3].Style.NumberFormat = "0";
                            excWsheet.Cells[counter, 5].Style.NumberFormat = "0";

                            //Raw Material
                            excWsheet.Cells[counter, 8].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 9].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 10].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 11].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 12].Style.NumberFormat = "0.000";

                            //Primary Process/Conversion
                            excWsheet.Cells[counter, 14].Style.NumberFormat = "0";
                            excWsheet.Cells[counter, 15].Style.NumberFormat = "0";
                            excWsheet.Cells[counter, 16].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 17].Style.NumberFormat = "0.000";

                            //Machining
                            excWsheet.Cells[counter, 19].Style.NumberFormat = "0";
                            excWsheet.Cells[counter, 20].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 21].Style.NumberFormat = "0.000";

                            //Machining 2/Secondary Opr.
                            excWsheet.Cells[counter, 23].Style.NumberFormat = "0";
                            excWsheet.Cells[counter, 24].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 25].Style.NumberFormat = "0.000";

                            //Machining 3/Other Opr.
                            excWsheet.Cells[counter, 27].Style.NumberFormat = "0";
                            excWsheet.Cells[counter, 28].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 29].Style.NumberFormat = "0.000";

                            //Surface Treatment
                            excWsheet.Cells[counter, 31].Style.NumberFormat = "0";
                            excWsheet.Cells[counter, 32].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 33].Style.NumberFormat = "0.000";

                            //Overhead
                            excWsheet.Cells[counter, 34].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 35].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 36].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 37].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 38].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 39].Style.NumberFormat = "0.00";
                            excWsheet.Cells[counter, 40].Style.NumberFormat = "0.000";

                            //Tooling
                            excWsheet.Cells[counter, 41].Style.NumberFormat = "0";
                            excWsheet.Cells[counter, 42].Style.NumberFormat = "0.000";
                            #endregion

                            #region Alignment

                            excWsheet.Cells[counter, 3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 5].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 8].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 9].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 10].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 11].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 12].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 14].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 15].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 16].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 17].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 19].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 20].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 21].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 23].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 24].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 25].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 27].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 28].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 29].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;

                            excWsheet.Cells[counter, 31].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 32].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 33].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 34].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 35].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 36].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 37].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 38].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 39].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 40].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 41].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                            excWsheet.Cells[counter, 42].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;

                            #endregion

                            #region assign data
                            excWsheet.Cells[counter, 0].Style.WrapText = true;
                            excWsheet.Cells[counter, 1].Style.WrapText = true;
                            excWsheet.Cells[counter, 2].Style.WrapText = true;
                            excWsheet.Cells[counter, 6].Style.WrapText = true;

                            excWsheet.Cells[counter, 0].Value = supplierDetailQuotePart.CustomerPartNo;
                            excWsheet.Cells[counter, 1].Value = supplierDetailQuotePart.PartDescription;
                            excWsheet.Cells[counter, 2].Value = supplierDetailQuotePart.AdditionalPartDesc;
                            excWsheet.Cells[counter, 3].Value = supplierDetailQuotePart.EstimatedQty;
                            excWsheet.Cells[counter, 4].Value = supplierDetailQuotePart.Manufacturer;
                            excWsheet.Cells[counter, 5].Value = supplierDetailQuotePart.MinOrderQty;

                            excWsheet.Cells[counter, 6].Value = supplierDetailQuotePart.MaterialType;
                            excWsheet.Cells[counter, 7].Value = supplierDetailQuotePart.rFQdqRawMaterial.RawMaterialIndexUsed;
                            excWsheet.Cells[counter, 8].Value = supplierDetailQuotePart.rFQdqRawMaterial.RawMatInputInKg;
                            excWsheet.Cells[counter, 9].Value = supplierDetailQuotePart.PartWeightKG;
                            excWsheet.Cells[counter, 10].Value = supplierDetailQuotePart.rFQdqRawMaterial.RawMatCostPerKg;
                            excWsheet.Cells[counter, 11].Value = supplierDetailQuotePart.rFQdqRawMaterial.MaterialLoss;
                            excWsheet.Cells[counter, 12].Value = supplierDetailQuotePart.rFQdqRawMaterial.RawMatTotal;

                            excWsheet.Cells[counter, 13].Value = supplierDetailQuotePart.rFQdqPrimaryProcessConversion.MachineDescription; //MachineDescId
                            excWsheet.Cells[counter, 14].Value = supplierDetailQuotePart.rFQdqPrimaryProcessConversion.MachineSize;
                            excWsheet.Cells[counter, 15].Value = supplierDetailQuotePart.rFQdqPrimaryProcessConversion.CycleTime;
                            excWsheet.Cells[counter, 16].Value = supplierDetailQuotePart.rFQdqPrimaryProcessConversion.ManPlusMachineRatePerHour;
                            excWsheet.Cells[counter, 17].Value = supplierDetailQuotePart.rFQdqPrimaryProcessConversion.ProcessConversionCostPerPart;

                            excWsheet.Cells[counter, 18].Value = supplierDetailQuotePart.rFQdqMachining.MachiningDescription; //MachiningDescId
                            excWsheet.Cells[counter, 19].Value = supplierDetailQuotePart.rFQdqMachining.CycleTime;
                            excWsheet.Cells[counter, 20].Value = supplierDetailQuotePart.rFQdqMachining.ManPlusMachineRatePerHour;
                            excWsheet.Cells[counter, 21].Value = supplierDetailQuotePart.rFQdqMachining.MachiningCostPerPart;

                            excWsheet.Cells[counter, 22].Value = supplierDetailQuotePart.rFQdqMachiningSecondaryOperation.SecondaryOperationDescription; //SecondaryOperationDescId
                            excWsheet.Cells[counter, 23].Value = supplierDetailQuotePart.rFQdqMachiningSecondaryOperation.CycleTime;
                            excWsheet.Cells[counter, 24].Value = supplierDetailQuotePart.rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour;
                            excWsheet.Cells[counter, 25].Value = supplierDetailQuotePart.rFQdqMachiningSecondaryOperation.SecondaryCostPerPart;

                            excWsheet.Cells[counter, 26].Value = supplierDetailQuotePart.rFQdqMachiningOtherOperation.SecondaryOperationDescription; //SecondaryOperationDescId
                            excWsheet.Cells[counter, 27].Value = supplierDetailQuotePart.rFQdqMachiningOtherOperation.CycleTime;
                            excWsheet.Cells[counter, 28].Value = supplierDetailQuotePart.rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour;
                            excWsheet.Cells[counter, 29].Value = supplierDetailQuotePart.rFQdqMachiningOtherOperation.SecondaryCostPerPart;

                            excWsheet.Cells[counter, 30].Value = supplierDetailQuotePart.rFQdqSurfaceTreatment.CoatingType; //CoatingTypeId
                            excWsheet.Cells[counter, 31].Value = supplierDetailQuotePart.rFQdqSurfaceTreatment.PartsPerHour;
                            excWsheet.Cells[counter, 32].Value = supplierDetailQuotePart.rFQdqSurfaceTreatment.ManPlusMachineRatePerHour;
                            excWsheet.Cells[counter, 33].Value = supplierDetailQuotePart.rFQdqSurfaceTreatment.CoatingCostPerHour;

                            excWsheet.Cells[counter, 34].Value = supplierDetailQuotePart.rFQdqOverhead.InventoryCarryingCost;
                            excWsheet.Cells[counter, 35].Value = supplierDetailQuotePart.rFQdqOverhead.PackagingMaterial;
                            excWsheet.Cells[counter, 36].Value = supplierDetailQuotePart.rFQdqOverhead.Packing;
                            excWsheet.Cells[counter, 37].Value = supplierDetailQuotePart.rFQdqOverhead.LocalFreightToPort;
                            excWsheet.Cells[counter, 38].Value = supplierDetailQuotePart.rFQdqOverhead.ProfitAndSGA;
                            excWsheet.Cells[counter, 39].Value = supplierDetailQuotePart.rFQdqOverhead.OverheadPercentPiecePrice;
                            excWsheet.Cells[counter, 40].Value = supplierDetailQuotePart.UnitPrice;

                            excWsheet.Cells[counter, 41].Value = supplierDetailQuotePart.NoOfCavities;
                            excWsheet.Cells[counter, 42].Value = supplierDetailQuotePart.ToolingCost;
                            excWsheet.Cells[counter, 43].Value = supplierDetailQuotePart.ToolingWarranty;
                            excWsheet.Cells[counter, 44].Value = supplierDetailQuotePart.SupplierToolingLeadtime;

                            #endregion

                            counter++;
                        }
                    }

                    excWsheet.Columns[1].AutoFit();
                    for (int index = 2; index <= 44; index++)
                    {
                        excWsheet.Columns[index].Width = 20 * 10 * 25;
                    }

                    //MES Logo
                    cr = excWsheet.Cells.GetSubrange("A1", "AS4"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.White, System.Drawing.Color.White);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;
                    excWsheet.Pictures.Add(System.Web.HttpContext.Current.Server.MapPath(Constants.IMAGEFOLDER) + "MESlogoPdf.png", GemBox.Spreadsheet.PositioningMode.FreeFloating, new AnchorCell(excWsheet.Columns[0], excWsheet.Rows[0], true), new AnchorCell(excWsheet.Columns[0], excWsheet.Rows[3], false));
                    //ends here

                    string generatedFileName = supplierItem.CompanyName.Trim() + "_" + paging.Criteria.RFQId + "_" + System.DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx";
                    filepath = System.Web.HttpContext.Current.Server.MapPath(Constants.RFQSUPPLIERQUOTEFOLDER) + generatedFileName;
                    if (!System.IO.Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(Constants.RFQSUPPLIERQUOTEFOLDER)))
                        System.IO.Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(Constants.RFQSUPPLIERQUOTEFOLDER));
                    else
                    {
                        if (File.Exists(filepath))
                            File.Delete(filepath);
                    }
                    myExcelFile.SaveXlsx(filepath);

                    filepath = System.Configuration.ConfigurationManager.AppSettings["ApiURL"] + (Constants.RFQSUPPLIERQUOTEFILEPATH) + generatedFileName;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    excWsheet = null;
                    ef.ClosePreservedXlsx();
                }
            }
            catch (Exception ex)//Error
            {
                throw ex;
            }
            return filepath;
        }
        #endregion

        public NPE.Core.ITypedResponse<MES.DTO.Library.RFQ.RFQ.RFQ> getRFQDetails(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging)
        {
            string errMSg = null;
            DTO.Library.RFQ.RFQ.RFQ RFQDetail = new DTO.Library.RFQ.RFQ.RFQ();

            this.RunOnDB(context =>
            {
                var rfqDetails = context.RFQs.Where(a => a.Id == paging.Criteria.RFQId && a.IsDeleted == false).SingleOrDefault();
                if (rfqDetails != null)
                {
                    RFQDetail.CustomerName = rfqDetails.Customer.CompanyName;
                    RFQDetail.CustomerId = rfqDetails.CustomerId;
                    RFQDetail.Id = rfqDetails.Id;
                    RFQDetail.SupplierName = "";
                    RFQDetail.Date = rfqDetails.Date;
                    RFQDetail.QuoteDueDate = rfqDetails.QuoteDueDate;
                    RFQDetail.SupplierRequirement = rfqDetails.SupplierRequirement;
                    RFQDetail.Remarks = rfqDetails.Remarks;

                }
            });

            var response = SuccessOrFailedResponse<MES.DTO.Library.RFQ.RFQ.RFQ>(errMSg, RFQDetail);
            response.PageInfo = paging.ToPage();
            return response;
        }

        #endregion

        #region Load SQ / DQ file

        public ITypedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>> UploadRfqSupplierPartQuote(IPage<DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging)
        {
            ITypedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>> retVal = null;
            List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> varlt = new List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>();
            string errMSg = null;
            var recordToBeUpdated = new MES.Data.Library.Part();

            string filePath = string.Empty;
            try
            {
                Business.Library.BO.RFQ.RFQ.RFQ rfqObj = new Business.Library.BO.RFQ.RFQ.RFQ();
                DTO.Library.RFQ.RFQ.RFQ rfqItem = rfqObj.FindById(paging.Criteria.RFQId).Result;

                if (string.IsNullOrEmpty(paging.Criteria.UniqueUrl))
                {
                    List<DTO.Library.RFQ.RFQ.RFQParts> lstRFQParts = new List<DTO.Library.RFQ.RFQ.RFQParts>();

                    this.RunOnDB(dbcontext =>
                    {
                        var rsItem = dbcontext.rfqSuppliers.Where(c => c.RFQId == paging.Criteria.RFQId && c.SupplierId == paging.Criteria.SupplierId).FirstOrDefault();
                        if (rsItem == null)
                            errMSg = Languages.GetResourceText("RecordNotExist");
                        else
                        {
                            paging.Criteria.UniqueUrl = rsItem.OriginalURL;
                            paging.Criteria.IsQuoteTypeDQ = rsItem.IsQuoteTypeDQ.HasValue ? rsItem.IsQuoteTypeDQ.Value : false;
                        }
                    });

                    if (paging.Criteria.IsQuoteTypeDQ)
                    {
                        retVal = LoadDetailQuoteExcel(paging.Criteria);
                    }
                    else
                    {
                        retVal = LoadExcel(paging.Criteria);
                    }
                }
                else
                {
                    List<DTO.Library.RFQ.RFQ.RFQParts> lstRFQParts = new List<DTO.Library.RFQ.RFQ.RFQParts>();

                    this.RunOnDB(dbcontext =>
                    {
                        var rsItem = dbcontext.rfqSuppliers.Where(c => c.UniqueURL == paging.Criteria.UniqueUrl).FirstOrDefault();
                        if (rsItem == null)
                            errMSg = Languages.GetResourceText("RecordNotExist");
                        else
                        {
                            paging.Criteria.RFQId = rsItem.RFQId;
                            paging.Criteria.SupplierId = rsItem.SupplierId;
                            paging.Criteria.IsQuoteTypeDQ = rsItem.IsQuoteTypeDQ.HasValue ? rsItem.IsQuoteTypeDQ.Value : false;
                        }
                    });


                    if (paging.Criteria.IsQuoteTypeDQ)
                    {
                        retVal = LoadDetailQuoteExcel_SubmitQuote(paging.Criteria);
                    }
                    else
                    {
                        retVal = LoadExcel_SubmitQuote(paging.Criteria);
                    }
                }

            }
            catch (Exception ex)//Error
            {
                errMSg = ex.Message;
            }

            return retVal;
        }
        /// <summary>
        /// Loads the detail quote excel.
        /// </summary>
        /// <param name="paging">The paging.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>       
        public ITypedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>> LoadExcel(DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria criteria)
        {
            List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> SupplierQuoteDetails = new List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>();
            SupplierQuoteDetails = GetDQRFQSupplierPartQuoteListbyUniqueURL(criteria).Result;
            Supplier.Suppliers supplierObj = new Supplier.Suppliers();
            DTO.Library.RFQ.Supplier.Suppliers supplierItem = supplierObj.FindById(criteria.SupplierId).Result;

            var context = HttpContext.Current;
            string errMsg = string.Empty;
            string successMessage = string.Empty;

            try
            {
                Stream memoryStream = MES.Business.Library.Helper.BlobHelper.AttachFile_ConvertBlobToStream(criteria.UploadQuoteFilePath);
                ExcelFile ef = new ExcelFile();
                ef.LoadXlsx(memoryStream, XlsxOptions.None);
                ExcelWorksheet ws = ef.Worksheets[0];
                var isNumber = new Regex(@"^\d{0,7}(\.\d{0,50})?$");
                try
                {
                    // Find the last real row
                    int nLastRow = 0;
                    bool isvalid = true;

                    if (ws.GetUsedCellRange(true) != null
                        && (nLastRow = ws.GetUsedCellRange(true).LastRowIndex) > 0
                        && ws.Cells["A13"].Value != null && ws.Cells["A13"].Value.ToString().ToLower().Trim() == "supplier quote"
                        && ws.Cells["A21"].Value != null && ws.Cells["A21"].Value.ToString().ToLower().Trim() == "part detail")
                    {
                        if (ws.Cells[7, 1].Value != null && supplierItem.CompanyName != ws.Cells[7, 1].Value.ToString() && ws.Cells[8, 1].Value != null && criteria.RFQId != ws.Cells[8, 1].Value.ToString())
                        {
                            errMsg = Languages.GetResourceText("UploadQuoteErrMsg1"); //"It seems that in this uploaded file, the information, other than the required ones have been modified.<br />Please download the RFQ file again and fill in the required values and upload it.";
                        }
                        else
                        {
                            List<MES.Data.Library.SearchSuppliersQuotedNotQuoted_Result> manufacturerList = this.DataContext.SearchSuppliersQuotedNotQuoted(criteria.RFQId).ToList();

                            if (ws.Cells[14, 1].Value == null || ws.Cells[15, 1].Value == null)
                                isvalid = false;
                            else if (ws.Cells[14, 1].Value != null && !isNumber.IsMatch(ws.Cells[14, 1].Value.ToString()))
                                isvalid = false;
                            else
                            {
                                for (int i = 22; i <= nLastRow; i++)
                                {
                                    if ((ws.Cells[i, 5].Value != null && !isNumber.IsMatch(ws.Cells[i, 5].Value.ToString().Trim()))// Part Detail Tab

                                        || (ws.Cells[i, 8].Value != null && !isNumber.IsMatch(ws.Cells[i, 8].Value.ToString().Trim()))// Raw Material Tab
                                        || (ws.Cells[i, 10].Value != null && !isNumber.IsMatch(ws.Cells[i, 10].Value.ToString().Trim()))

                                        || (ws.Cells[i, 13].Value != null && !isNumber.IsMatch(ws.Cells[i, 11].Value.ToString().Trim()))//Other Tab
                                        || (ws.Cells[i, 14].Value != null && !isNumber.IsMatch(ws.Cells[i, 12].Value.ToString().Trim()))
                                        || (ws.Cells[i, 15].Value != null && !isNumber.IsMatch(ws.Cells[i, 13].Value.ToString().Trim()))

                                        || (ws.Cells[i, 17].Value != null && !isNumber.IsMatch(ws.Cells[i, 14].Value.ToString().Trim()))//Tooling Tab
                                        || (ws.Cells[i, 18].Value != null && !isNumber.IsMatch(ws.Cells[i, 15].Value.ToString().Trim()))
                                    )
                                    {
                                        isvalid = false;
                                        break;
                                    }

                                    if ((ws.Cells[i, 5].Value == null || getExcelCellIntValue(ws.Cells[i, 5].Value) == 0) && getExcelCellIntValue(ws.Cells[i, 8].Value) == 0
                                        && getExcelCellIntValue(ws.Cells[i, 10].Value) == 0 && getExcelCellIntValue(ws.Cells[i, 13].Value) == 0
                                        && getExcelCellIntValue(ws.Cells[i, 14].Value) == 0 && getExcelCellIntValue(ws.Cells[i, 15].Value) == 0
                                        && getExcelCellIntValue(ws.Cells[i, 17].Value) == 0)
                                    {
                                        continue;
                                    }
                                    else if ((ws.Cells[i, 5].Value != null && (ws.Cells[i, 5].Value.ToString().Trim() == "" || Convert.ToDecimal(ws.Cells[i, 5].Value.ToString().Trim()) == 0))
                                        && (ws.Cells[i, 8].Value != null && (ws.Cells[i, 8].Value.ToString().Trim() == "" || Convert.ToDecimal(ws.Cells[i, 8].Value.ToString().Trim()) == 0))
                                        && (ws.Cells[i, 10].Value != null && (ws.Cells[i, 10].Value.ToString().Trim() == "" || Convert.ToDecimal(ws.Cells[i, 10].Value.ToString().Trim()) == 0))
                                        && (ws.Cells[i, 13].Value != null && (ws.Cells[i, 13].Value.ToString().Trim() == "" || Convert.ToDecimal(ws.Cells[i, 13].Value.ToString().Trim()) == 0))
                                        && (ws.Cells[i, 14].Value != null && (ws.Cells[i, 14].Value.ToString().Trim() == "" || Convert.ToDecimal(ws.Cells[i, 14].Value.ToString().Trim()) == 0))
                                        && (ws.Cells[i, 15].Value != null && (ws.Cells[i, 15].Value.ToString().Trim() == "" || Convert.ToDecimal(ws.Cells[i, 15].Value.ToString().Trim()) == 0))
                                        && (ws.Cells[i, 17].Value != null && (ws.Cells[i, 17].Value.ToString().Trim() == "" || Convert.ToDecimal(ws.Cells[i, 17].Value.ToString().Trim()) == 0))
                                 )
                                    {
                                        continue;
                                    }


                                    if (ws.Cells[i, 8].Value == null || ws.Cells[i, 10].Value == null || ws.Cells[i, 13].Value == null || ws.Cells[i, 17].Value == null)
                                    {
                                        isvalid = false; break;
                                    }
                                    else if ((ws.Cells[i, 8].Value != null && Convert.ToDecimal(ws.Cells[i, 8].Value.ToString().Trim()) <= 0)
                                     || (ws.Cells[i, 10].Value != null && Convert.ToDecimal(ws.Cells[i, 10].Value.ToString().Trim()) <= 0)
                                     || (ws.Cells[i, 13].Value != null && Convert.ToDecimal(ws.Cells[i, 13].Value.ToString().Trim()) <= 0))
                                    {
                                        if (!((ws.Cells[i, 18].Value != null && Convert.ToDecimal(ws.Cells[i, 18].Value.ToString().Trim()) == 0)
                                            && (ws.Cells[i, 14].Value != null && Convert.ToDecimal(ws.Cells[i, 14].Value.ToString().Trim()) == 0)
                                            && (ws.Cells[i, 15].Value != null && Convert.ToDecimal(ws.Cells[i, 15].Value.ToString().Trim()) == 0)))
                                        {
                                            isvalid = false; break;
                                        }
                                    }

                                    #region Part Detail

                                    if (ws.Cells[i, 5].Value == null || ws.Cells[i, 5].Value.ToString() == string.Empty || Convert.ToDecimal(ws.Cells[i, 5].Value) < 0)
                                    {
                                        isvalid = false;
                                        break;
                                    }

                                    if ((ws.Cells[i, 5].Value != null && Convert.ToDecimal(ws.Cells[i, 5].Value) > 0)

                                        && (
                                        (ws.Cells[i, 8].Value != null && Convert.ToDecimal(ws.Cells[i, 8].Value.ToString().Trim()) <= 0)
                                        || (ws.Cells[i, 10].Value != null && Convert.ToDecimal(ws.Cells[i, 10].Value.ToString().Trim()) <= 0)
                                        || (ws.Cells[i, 13].Value != null && Convert.ToDecimal(ws.Cells[i, 13].Value.ToString().Trim()) <= 0)
                                        || (ws.Cells[i, 17].Value != null && Convert.ToDecimal(ws.Cells[i, 17].Value.ToString().Trim()) <= 0)))
                                    {
                                        isvalid = false;
                                        break;
                                    }

                                    #endregion
                                }
                            }


                            if (isvalid)
                            {
                                decimal exchangeRate = Math.Round(Convert.ToDecimal(ws.Cells[14, 1].Value), 3);
                                string currency = ws.Cells[15, 1].Value.ToString();
                                string rawmaterial = ws.Cells[16, 1].Value != null ? ws.Cells[16, 1].Value.ToString() : string.Empty;
                                string remarks = string.Empty;
                                if (ws.Cells[17, 1].Value != null && !string.IsNullOrEmpty(ws.Cells[17, 1].Value.ToString().Trim()))
                                    remarks = ws.Cells[17, 1].Value.ToString();

                                int counter = 0;
                                decimal varexchangeRate;
                                decimal sum;
                                decimal dRawMaterialTotal = 0;

                                for (int i = 22; i <= nLastRow; i++)
                                {
                                    sum = 0;

                                    if (ws.Cells[i, 4].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 4].Value.ToString().Trim()))
                                    {
                                        if (manufacturerList.Where(rec => rec.CompanyName.ToLower().Trim() == ws.Cells[i, 4].Value.ToString().Trim().ToLower()).Count() > 0)
                                        {
                                            SupplierQuoteDetails[counter].ManufacturerId = manufacturerList.Where(rec => rec.CompanyName.ToLower().Trim() == ws.Cells[i, 4].Value.ToString().Trim().ToLower()).Single().SupplierId;
                                            SupplierQuoteDetails[counter].Manufacturer = manufacturerList.Where(rec => rec.CompanyName.ToLower().Trim() == ws.Cells[i, 4].Value.ToString().Trim().ToLower()).Single().CompanyName;
                                        }
                                        else if (manufacturerList.Where(rec => rec.CompanyName.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 4].Value.ToString().Trim().ToLower()).Count() > 0)
                                        {
                                            SupplierQuoteDetails[counter].ManufacturerId = manufacturerList.Where(rec => rec.CompanyName.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 4].Value.ToString().Trim().ToLower()).Single().SupplierId;
                                            SupplierQuoteDetails[counter].Manufacturer = manufacturerList.Where(rec => rec.CompanyName.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 4].Value.ToString().Trim().ToLower()).Single().CompanyName;
                                        }
                                        else
                                        {
                                            SupplierQuoteDetails[counter].ManufacturerId = (int?)null;
                                            SupplierQuoteDetails[counter].Manufacturer = ws.Cells[i, 4].Value.ToString().Trim();
                                        }
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].ManufacturerId = (int?)null;
                                        SupplierQuoteDetails[counter].Manufacturer = "";
                                    }
                                    #region Part Detail
                                    if (ws.Cells[i, 5].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 5].Value.ToString().Trim()))
                                        SupplierQuoteDetails[counter].MinOrderQty = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 5].Value), 0);
                                    #endregion

                                    #region Raw Material
                                    dRawMaterialTotal = 0;
                                    SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMaterialIndexUsed = ws.Cells[i, 7].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 7].Value.ToString().Trim()) ? ws.Cells[i, 7].Value.ToString() : string.Empty;
                                    if (ws.Cells[i, 8].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 8].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 8].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatInputInKg = Math.Round(Convert.ToDecimal(ws.Cells[i, 8].Value), 3);
                                        dRawMaterialTotal = Convert.ToDecimal(ws.Cells[i, 8].Value);
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatInputInKg = 0;
                                    }

                                    if (ws.Cells[i, 10].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 10].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 10].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatCostPerKg = Math.Round(Convert.ToDecimal(ws.Cells[i, 10].Value), 3);
                                        dRawMaterialTotal = dRawMaterialTotal * Convert.ToDecimal(ws.Cells[i, 10].Value);
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatCostPerKg = 0;
                                    }

                                    if (ws.Cells[i, 8].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 8].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 8].Value) > 0)
                                    {
                                        if (SupplierQuoteDetails[counter].PartWeightKG.HasValue)
                                            SupplierQuoteDetails[counter].rFQdqRawMaterial.MaterialLoss = (SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatInputInKg - SupplierQuoteDetails[counter].PartWeightKG) / SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatInputInKg;
                                        else
                                            SupplierQuoteDetails[counter].rFQdqRawMaterial.MaterialLoss = 1;
                                    }
                                    else
                                        SupplierQuoteDetails[counter].rFQdqRawMaterial.MaterialLoss = 0;

                                    SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatTotal = Math.Round(dRawMaterialTotal, 3);

                                    #endregion
                                    #region Other
                                    if (ws.Cells[i, 13].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 13].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 13].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].ProcessCost = Math.Round(Convert.ToDecimal(ws.Cells[i, 13].Value), 3, MidpointRounding.AwayFromZero);
                                        sum += Convert.ToDecimal(ws.Cells[i, 13].Value);
                                    }
                                    if (ws.Cells[i, 14].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 14].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 14].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].MachiningCost = Math.Round(Convert.ToDecimal(ws.Cells[i, 14].Value), 3, MidpointRounding.AwayFromZero);
                                        sum += Convert.ToDecimal(ws.Cells[i, 14].Value);
                                    }
                                    else
                                        SupplierQuoteDetails[counter].MachiningCost = 0;

                                    if (ws.Cells[i, 15].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 15].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 15].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].OtherProcessCost = Math.Round(Convert.ToDecimal(ws.Cells[i, 15].Value), 3, MidpointRounding.AwayFromZero);
                                        sum += Convert.ToDecimal(ws.Cells[i, 15].Value);
                                    }
                                    else
                                        SupplierQuoteDetails[counter].OtherProcessCost = 0;

                                    if (sum > 0)
                                        SupplierQuoteDetails[counter].UnitPrice = sum + Math.Round(dRawMaterialTotal, 3);
                                    #endregion

                                    #region Tooling
                                    if (ws.Cells[i, 17].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 17].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 17].Value) > 0)
                                        SupplierQuoteDetails[counter].NoOfCavities = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 17].Value), 0);
                                    else
                                        SupplierQuoteDetails[counter].NoOfCavities = 0;

                                    if (ws.Cells[i, 18].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 18].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 18].Value) > 0)
                                        SupplierQuoteDetails[counter].ToolingCost = Math.Round(Convert.ToDecimal(ws.Cells[i, 18].Value), 3, MidpointRounding.AwayFromZero);

                                    SupplierQuoteDetails[counter].ToolingWarranty = ws.Cells[i, 19].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 19].Value.ToString().Trim()) ? ws.Cells[i, 19].Value.ToString() : string.Empty;
                                    SupplierQuoteDetails[counter].SupplierToolingLeadtime = ws.Cells[i, 20].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 20].Value.ToString().Trim()) ? ws.Cells[i, 20].Value.ToString() : string.Empty;
                                    #endregion

                                    SupplierQuoteDetails[counter].Remarks = remarks;
                                    varexchangeRate = exchangeRate;
                                    SupplierQuoteDetails[counter].ExchangeRate = varexchangeRate;
                                    SupplierQuoteDetails[counter].Currency = currency;
                                    SupplierQuoteDetails[counter].RawMaterialPriceAssumed = rawmaterial; //MESUtilities.RemoveAmountFormat(rawmaterial);
                                    SupplierQuoteDetails[counter].IsQuoteTypeDQ = criteria.IsQuoteTypeDQ;
                                    counter++;
                                }

                                SupplierQuoteDetails[0].CreatedBy = supplierItem.CompanyName;
                                SupplierQuoteDetails[0].Remarks = remarks;
                                successMessage = Languages.GetResourceText("QuoteUploadSuccess");
                            }
                            else
                            {
                                errMsg = Languages.GetResourceText("UploadQuoteErrMsg2"); //"Please fill the mandatory data in the excel file and upload.";
                            }
                        }
                    }

                    else
                    {
                        errMsg = Languages.GetResourceText("UploadQuoteErrMsg1");// "It seems that in this uploaded file, the information, other than the required ones have been modified.<br />Please download the RFQ file again and fill in the required values and upload it.";
                    }
                }


                catch (Exception ex) //Error
                {
                    //"Error - Loading xlsx",
                    throw ex;
                }
                finally
                {
                    ws = null;
                    ef.ClosePreservedXlsx();
                }
            }
            catch (Exception ex) //Error
            {
                //"Error - LoadCSV",	
                throw ex;
            }

            return SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>>(errMsg, SupplierQuoteDetails, successMessage);
        }
        /// <summary>
        /// Loads the detail quote excel.
        /// </summary>
        /// <param name="paging">The paging.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>       
        public ITypedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>> LoadExcel_SubmitQuote(DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria criteria)
        {
            List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> SupplierQuoteDetails = new List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>();
            SupplierQuoteDetails = GetDQRFQSupplierPartQuoteListbyUniqueURL(criteria).Result;
            Supplier.Suppliers supplierObj = new Supplier.Suppliers();
            DTO.Library.RFQ.Supplier.Suppliers supplierItem = supplierObj.FindById(criteria.SupplierId).Result;

            var context = HttpContext.Current;
            string errMsg = string.Empty;
            string successMessage = string.Empty;

            try
            {
                Stream memoryStream = MES.Business.Library.Helper.BlobHelper.AttachFile_ConvertBlobToStream(criteria.UploadQuoteFilePath);
                ExcelFile ef = new ExcelFile();
                ef.LoadXlsx(memoryStream, XlsxOptions.None);
                ExcelWorksheet ws = ef.Worksheets[0];
                var isNumber = new Regex(@"^\d{0,7}(\.\d{0,50})?$");
                try
                {
                    // Find the last real row
                    int nLastRow = 0;
                    bool isvalid = true;

                    if (ws.GetUsedCellRange(true) != null
                        && (nLastRow = ws.GetUsedCellRange(true).LastRowIndex) > 0
                        && ws.Cells["A13"].Value != null && ws.Cells["A13"].Value.ToString().ToLower().Trim() == "request for quote (rfq)"
                        && ws.Cells["A21"].Value != null && ws.Cells["A21"].Value.ToString().ToLower().Trim() == "part detail")
                    {
                        if (ws.Cells[7, 1].Value != null && supplierItem.CompanyName != ws.Cells[7, 1].Value.ToString() && ws.Cells[8, 1].Value != null && criteria.RFQId != ws.Cells[8, 1].Value.ToString())
                        {
                            errMsg = Languages.GetResourceText("UploadQuoteErrMsg1"); //"It seems that in this uploaded file, the information, other than the required ones have been modified.<br />Please download the RFQ file again and fill in the required values and upload it.";
                        }
                        else
                        {
                            if (ws.Cells[14, 1].Value == null || ws.Cells[15, 1].Value == null)
                                isvalid = false;
                            else if (ws.Cells[14, 1].Value != null && !isNumber.IsMatch(ws.Cells[14, 1].Value.ToString()))
                                isvalid = false;
                            else
                            {
                                for (int i = 22; i <= nLastRow; i++)
                                {
                                    if ((ws.Cells[i, 4].Value != null && !isNumber.IsMatch(ws.Cells[i, 4].Value.ToString().Trim())) // Part Detail Tab

                                        || (ws.Cells[i, 7].Value != null && !isNumber.IsMatch(ws.Cells[i, 7].Value.ToString().Trim()))// Raw Material Tab
                                        || (ws.Cells[i, 9].Value != null && !isNumber.IsMatch(ws.Cells[i, 9].Value.ToString().Trim()))

                                        || (ws.Cells[i, 12].Value != null && !isNumber.IsMatch(ws.Cells[i, 12].Value.ToString().Trim()))//Other Tab
                                        || (ws.Cells[i, 13].Value != null && !isNumber.IsMatch(ws.Cells[i, 13].Value.ToString().Trim()))
                                        || (ws.Cells[i, 14].Value != null && !isNumber.IsMatch(ws.Cells[i, 14].Value.ToString().Trim()))
                                        //|| (ws.Cells[i, 15].Value != null && !isNumber.IsMatch(ws.Cells[i, 15].Value.ToString().Trim()))

                                        || (ws.Cells[i, 16].Value != null && !isNumber.IsMatch(ws.Cells[i, 16].Value.ToString().Trim()))//Tooling Tab
                                        || (ws.Cells[i, 17].Value != null && !isNumber.IsMatch(ws.Cells[i, 17].Value.ToString().Trim()))
                                    )
                                    {
                                        isvalid = false;
                                        break;
                                    }

                                    if ((ws.Cells[i, 4].Value == null || getExcelCellIntValue(ws.Cells[i, 4].Value) == 0) && getExcelCellIntValue(ws.Cells[i, 7].Value) == 0
                                        && getExcelCellIntValue(ws.Cells[i, 9].Value) == 0 && getExcelCellIntValue(ws.Cells[i, 12].Value) == 0
                                        && getExcelCellIntValue(ws.Cells[i, 13].Value) == 0 && getExcelCellIntValue(ws.Cells[i, 14].Value) == 0
                                        && getExcelCellIntValue(ws.Cells[i, 16].Value) == 0)
                                    {
                                        continue;
                                    }
                                    else if ((ws.Cells[i, 4].Value != null && (ws.Cells[i, 4].Value.ToString().Trim() == "" || Convert.ToDecimal(ws.Cells[i, 4].Value.ToString().Trim()) == 0))
                                        && (ws.Cells[i, 7].Value != null && (ws.Cells[i, 7].Value.ToString().Trim() == "" || Convert.ToDecimal(ws.Cells[i, 7].Value.ToString().Trim()) == 0))
                                        && (ws.Cells[i, 9].Value != null && (ws.Cells[i, 9].Value.ToString().Trim() == "" || Convert.ToDecimal(ws.Cells[i, 9].Value.ToString().Trim()) == 0))
                                        && (ws.Cells[i, 12].Value != null && (ws.Cells[i, 12].Value.ToString().Trim() == "" || Convert.ToDecimal(ws.Cells[i, 12].Value.ToString().Trim()) == 0))
                                        && (ws.Cells[i, 13].Value != null && (ws.Cells[i, 13].Value.ToString().Trim() == "" || Convert.ToDecimal(ws.Cells[i, 13].Value.ToString().Trim()) == 0))
                                        && (ws.Cells[i, 14].Value != null && (ws.Cells[i, 14].Value.ToString().Trim() == "" || Convert.ToDecimal(ws.Cells[i, 14].Value.ToString().Trim()) == 0))
                                        && (ws.Cells[i, 16].Value != null && (ws.Cells[i, 16].Value.ToString().Trim() == "" || Convert.ToDecimal(ws.Cells[i, 16].Value.ToString().Trim()) == 0))
                                    )
                                    {
                                        continue;
                                    }

                                    if (ws.Cells[i, 7].Value == null || ws.Cells[i, 9].Value == null || ws.Cells[i, 12].Value == null || ws.Cells[i, 16].Value == null)
                                    {
                                        isvalid = false; break;
                                    }
                                    else if ((ws.Cells[i, 7].Value != null && Convert.ToDecimal(ws.Cells[i, 7].Value.ToString().Trim()) <= 0)
                                     || (ws.Cells[i, 9].Value != null && Convert.ToDecimal(ws.Cells[i, 9].Value.ToString().Trim()) <= 0)
                                     || (ws.Cells[i, 12].Value != null && Convert.ToDecimal(ws.Cells[i, 12].Value.ToString().Trim()) <= 0)
                                        //|| (ws.Cells[i, 16].Value != null && Convert.ToDecimal(ws.Cells[i, 16].Value.ToString().Trim()) <= 0)
                                     )
                                    {
                                        if (!((ws.Cells[i, 17].Value != null && Convert.ToDecimal(ws.Cells[i, 17].Value.ToString().Trim()) == 0)
                                            && (ws.Cells[i, 13].Value != null && Convert.ToDecimal(ws.Cells[i, 13].Value.ToString().Trim()) == 0)
                                            && (ws.Cells[i, 14].Value != null && Convert.ToDecimal(ws.Cells[i, 14].Value.ToString().Trim()) == 0)))
                                        {
                                            isvalid = false; break;
                                        }
                                    }

                                    #region Part Detail

                                    if (ws.Cells[i, 4].Value == null || ws.Cells[i, 4].Value.ToString() == string.Empty || Convert.ToDecimal(ws.Cells[i, 4].Value) < 0)
                                    {
                                        isvalid = false;
                                        break;
                                    }

                                    if ((ws.Cells[i, 4].Value != null && Convert.ToDecimal(ws.Cells[i, 4].Value) > 0)

                                        && (
                                        (ws.Cells[i, 7].Value != null && Convert.ToDecimal(ws.Cells[i, 7].Value.ToString().Trim()) <= 0)
                                        || (ws.Cells[i, 9].Value != null && Convert.ToDecimal(ws.Cells[i, 9].Value.ToString().Trim()) <= 0)
                                        || (ws.Cells[i, 12].Value != null && Convert.ToDecimal(ws.Cells[i, 12].Value.ToString().Trim()) <= 0)
                                        || (ws.Cells[i, 16].Value != null && Convert.ToDecimal(ws.Cells[i, 16].Value.ToString().Trim()) <= 0)))
                                    {
                                        isvalid = false;
                                        break;
                                    }

                                    #endregion
                                }
                            }


                            if (isvalid)
                            {
                                decimal exchangeRate = Math.Round(Convert.ToDecimal(ws.Cells[14, 1].Value), 3);
                                string currency = ws.Cells[15, 1].Value.ToString();
                                string rawmaterial = ws.Cells[16, 1].Value != null ? ws.Cells[16, 1].Value.ToString() : string.Empty;
                                string remarks = string.Empty;
                                if (ws.Cells[17, 1].Value != null && !string.IsNullOrEmpty(ws.Cells[17, 1].Value.ToString().Trim()))
                                    remarks = ws.Cells[17, 1].Value.ToString();

                                int counter = 0;
                                decimal varexchangeRate;
                                decimal sum;
                                decimal dRawMaterialTotal = 0;
                                for (int i = 22; i <= nLastRow; i++)
                                {
                                    sum = 0;

                                    #region Part Detail
                                    if (ws.Cells[i, 4].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 4].Value.ToString().Trim()))
                                        SupplierQuoteDetails[counter].MinOrderQty = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 4].Value), 0);
                                    #endregion

                                    #region Raw Material
                                    dRawMaterialTotal = 0;
                                    SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMaterialIndexUsed = ws.Cells[i, 6].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 6].Value.ToString().Trim()) ? ws.Cells[i, 6].Value.ToString() : string.Empty;
                                    if (ws.Cells[i, 7].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 7].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 7].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatInputInKg = Math.Round(Convert.ToDecimal(ws.Cells[i, 7].Value), 3);
                                        dRawMaterialTotal = Convert.ToDecimal(ws.Cells[i, 7].Value);
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatInputInKg = 0;
                                    }

                                    if (ws.Cells[i, 9].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 9].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 9].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatCostPerKg = Math.Round(Convert.ToDecimal(ws.Cells[i, 9].Value), 3);
                                        dRawMaterialTotal = dRawMaterialTotal * Convert.ToDecimal(ws.Cells[i, 9].Value);
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatCostPerKg = 0;
                                    }

                                    if (ws.Cells[i, 7].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 7].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 7].Value) > 0)
                                    {
                                        if (SupplierQuoteDetails[counter].PartWeightKG.HasValue)
                                            SupplierQuoteDetails[counter].rFQdqRawMaterial.MaterialLoss = (SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatInputInKg - SupplierQuoteDetails[counter].PartWeightKG) / SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatInputInKg;
                                        else
                                            SupplierQuoteDetails[counter].rFQdqRawMaterial.MaterialLoss = 1;
                                    }
                                    else
                                        SupplierQuoteDetails[counter].rFQdqRawMaterial.MaterialLoss = 0;

                                    SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatTotal = Math.Round(dRawMaterialTotal, 3);

                                    #endregion

                                    #region Other
                                    if (ws.Cells[i, 12].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 12].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 12].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].ProcessCost = Math.Round(Convert.ToDecimal(ws.Cells[i, 12].Value), 3, MidpointRounding.AwayFromZero);
                                        sum += Convert.ToDecimal(ws.Cells[i, 12].Value);
                                    }
                                    if (ws.Cells[i, 13].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 13].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 13].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].MachiningCost = Math.Round(Convert.ToDecimal(ws.Cells[i, 13].Value), 3, MidpointRounding.AwayFromZero);
                                        sum += Convert.ToDecimal(ws.Cells[i, 13].Value);
                                    }
                                    else
                                        SupplierQuoteDetails[counter].MachiningCost = 0;

                                    if (ws.Cells[i, 14].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 14].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 14].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].OtherProcessCost = Math.Round(Convert.ToDecimal(ws.Cells[i, 14].Value), 3, MidpointRounding.AwayFromZero);
                                        sum += Convert.ToDecimal(ws.Cells[i, 14].Value);
                                    }
                                    else
                                        SupplierQuoteDetails[counter].OtherProcessCost = 0;

                                    if (sum > 0)
                                        SupplierQuoteDetails[counter].UnitPrice = sum + Math.Round(dRawMaterialTotal, 3);
                                    #endregion

                                    #region Tooling
                                    if (ws.Cells[i, 16].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 16].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 16].Value) > 0)
                                        SupplierQuoteDetails[counter].NoOfCavities = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 16].Value), 0);
                                    else
                                        SupplierQuoteDetails[counter].NoOfCavities = 0;

                                    if (ws.Cells[i, 17].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 17].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 17].Value) > 0)
                                        SupplierQuoteDetails[counter].ToolingCost = Math.Round(Convert.ToDecimal(ws.Cells[i, 17].Value), 3, MidpointRounding.AwayFromZero);

                                    SupplierQuoteDetails[counter].ToolingWarranty = ws.Cells[i, 18].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 18].Value.ToString().Trim()) ? ws.Cells[i, 18].Value.ToString() : string.Empty;
                                    SupplierQuoteDetails[counter].SupplierToolingLeadtime = ws.Cells[i, 19].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 19].Value.ToString().Trim()) ? ws.Cells[i, 19].Value.ToString() : string.Empty;
                                    #endregion

                                    SupplierQuoteDetails[counter].Remarks = remarks;
                                    varexchangeRate = exchangeRate;
                                    SupplierQuoteDetails[counter].ExchangeRate = varexchangeRate;
                                    SupplierQuoteDetails[counter].Currency = currency;
                                    SupplierQuoteDetails[counter].RawMaterialPriceAssumed = rawmaterial; //MESUtilities.RemoveAmountFormat(rawmaterial);

                                    counter++;
                                }

                                SupplierQuoteDetails[0].CreatedBy = supplierItem.CompanyName;
                                SupplierQuoteDetails[0].Remarks = remarks;
                                successMessage = Languages.GetResourceText("QuoteUploadSuccess");
                            }
                            else
                            {
                                errMsg = Languages.GetResourceText("UploadQuoteErrMsg2"); //"Please fill the mandatory data in the excel file and upload.";
                            }
                        }
                    }

                    else
                    {
                        errMsg = Languages.GetResourceText("UploadQuoteErrMsg1");// "It seems that in this uploaded file, the information, other than the required ones have been modified.<br />Please download the RFQ file again and fill in the required values and upload it.";
                    }
                }


                catch (Exception ex) //Error
                {
                    //"Error - Loading xlsx",
                    throw ex;
                }
                finally
                {
                    ws = null;
                    ef.ClosePreservedXlsx();
                }
            }
            catch (Exception ex) //Error
            {
                //"Error - LoadCSV",	
                throw ex;
            }

            return SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>>(errMsg, SupplierQuoteDetails, successMessage);
        }

        private ITypedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>> LoadDetailQuoteExcel(DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria criteria)
        {
            List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> SupplierQuoteDetails = new List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>();
            SupplierQuoteDetails = GetDQRFQSupplierPartQuoteListbyUniqueURL(criteria).Result;
            Supplier.Suppliers supplierObj = new Supplier.Suppliers();
            DTO.Library.RFQ.Supplier.Suppliers supplierItem = supplierObj.FindById(criteria.SupplierId).Result;

            var context = HttpContext.Current;
            string errMsg = string.Empty;
            string successMessage = string.Empty;

            try
            {
                Stream memoryStream = MES.Business.Library.Helper.BlobHelper.AttachFile_ConvertBlobToStream(criteria.UploadQuoteFilePath);
                ExcelFile ef = new ExcelFile();
                ef.LoadXlsx(memoryStream, XlsxOptions.None);
                ExcelWorksheet ws = ef.Worksheets[0];
                var isNumber = new Regex(@"^\d{0,7}(\.\d{0,50})?$");

                try
                {
                    // Find the last real row
                    int nLastRow = 0;
                    bool isvalid = true;
                    if (ws.GetUsedCellRange(true) != null && (nLastRow = ws.GetUsedCellRange(true).LastRowIndex) > 0
                        && ws.Cells["A13"].Value != null && ws.Cells["A13"].Value.ToString().ToLower().Trim() == "supplier quote"
                        && ws.Cells["A21"].Value != null && ws.Cells["A21"].Value.ToString().ToLower().Trim() == "part detail")
                    {
                        if (ws.Cells[7, 1].Value != null && supplierItem.CompanyName != ws.Cells[7, 1].Value.ToString() && ws.Cells[8, 1].Value != null && criteria.RFQId != ws.Cells[8, 1].Value.ToString())
                        {
                            errMsg = Languages.GetResourceText("UploadQuoteErrMsg1");// "It seems that in this uploaded file, the information, other than the required ones have been modified.<br />Please download the RFQ file again and fill in the required values and upload it.";

                        }
                        else
                        {
                            List<MES.Data.Library.SearchSuppliersQuotedNotQuoted_Result> manufacturerList = this.DataContext.SearchSuppliersQuotedNotQuoted(criteria.RFQId).ToList();

                            if (ws.Cells[14, 1].Value == null || ws.Cells[15, 1].Value == null)
                                isvalid = false;
                            else if (ws.Cells[14, 1].Value != null && !isNumber.IsMatch(ws.Cells[14, 1].Value.ToString()))
                                isvalid = false;
                            else
                            {
                                for (int i = 22; i <= nLastRow; i++)
                                {
                                    if ((ws.Cells[i, 5].Value != null && !isNumber.IsMatch(ws.Cells[i, 5].Value.ToString().Trim()))  // Part Detail Tab
                                          || (ws.Cells[i, 41].Value != null && !isNumber.IsMatch(ws.Cells[i, 41].Value.ToString().Trim()))

                                          || (ws.Cells[i, 7].Value != null && !isNumber.IsMatch(ws.Cells[i, 7].Value.ToString().Trim())) // Raq Material Tab
                                          || (ws.Cells[i, 8].Value != null && !isNumber.IsMatch(ws.Cells[i, 8].Value.ToString().Trim()))
                                          || (ws.Cells[i, 10].Value != null && !isNumber.IsMatch(ws.Cells[i, 10].Value.ToString().Trim()))
                                          || (ws.Cells[i, 11].Value != null && !isNumber.IsMatch(ws.Cells[i, 11].Value.ToString().Trim()))

                                          || (ws.Cells[i, 14].Value != null && !isNumber.IsMatch(ws.Cells[i, 14].Value.ToString().Trim())) // Raq Material Tab
                                          || (ws.Cells[i, 15].Value != null && !isNumber.IsMatch(ws.Cells[i, 15].Value.ToString().Trim()))
                                          || (ws.Cells[i, 16].Value != null && !isNumber.IsMatch(ws.Cells[i, 16].Value.ToString().Trim()))
                                          || (ws.Cells[i, 17].Value != null && !isNumber.IsMatch(ws.Cells[i, 17].Value.ToString().Trim()))

                                          //|| (ws.Cells[i, 16].Value != null && string.IsNullOrEmpty(ws.Cells[i, 16].Value.ToString().Trim())) //Primary Process/Conversion Machine Description
                                          || (ws.Cells[i, 19].Value != null && !isNumber.IsMatch(ws.Cells[i, 19].Value.ToString().Trim()))
                                          || (ws.Cells[i, 20].Value != null && !isNumber.IsMatch(ws.Cells[i, 20].Value.ToString().Trim()))
                                          || (ws.Cells[i, 21].Value != null && !isNumber.IsMatch(ws.Cells[i, 21].Value.ToString().Trim()))

                                          || (ws.Cells[i, 23].Value != null && !isNumber.IsMatch(ws.Cells[i, 23].Value.ToString().Trim()))// Machining Tab
                                          || (ws.Cells[i, 24].Value != null && !isNumber.IsMatch(ws.Cells[i, 24].Value.ToString().Trim()))
                                          || (ws.Cells[i, 25].Value != null && !isNumber.IsMatch(ws.Cells[i, 25].Value.ToString().Trim()))

                                          || (ws.Cells[i, 27].Value != null && !isNumber.IsMatch(ws.Cells[i, 27].Value.ToString().Trim())) // Machining Secondary Operation Tab
                                          || (ws.Cells[i, 28].Value != null && !isNumber.IsMatch(ws.Cells[i, 28].Value.ToString().Trim()))
                                          || (ws.Cells[i, 29].Value != null && !isNumber.IsMatch(ws.Cells[i, 29].Value.ToString().Trim()))

                                          || (ws.Cells[i, 31].Value != null && !isNumber.IsMatch(ws.Cells[i, 31].Value.ToString().Trim())) // Machining Other Operation Tab
                                          || (ws.Cells[i, 32].Value != null && !isNumber.IsMatch(ws.Cells[i, 32].Value.ToString().Trim()))
                                          || (ws.Cells[i, 33].Value != null && !isNumber.IsMatch(ws.Cells[i, 33].Value.ToString().Trim()))

                                          || (ws.Cells[i, 34].Value != null && !isNumber.IsMatch(ws.Cells[i, 34].Value.ToString().Trim())) // Surface Treament Tab
                                          || (ws.Cells[i, 35].Value != null && !isNumber.IsMatch(ws.Cells[i, 35].Value.ToString().Trim()))
                                          || (ws.Cells[i, 36].Value != null && !isNumber.IsMatch(ws.Cells[i, 36].Value.ToString().Trim()))

                                          || (ws.Cells[i, 37].Value != null && !isNumber.IsMatch(ws.Cells[i, 37].Value.ToString().Trim())) // Overhead Tab
                                          || (ws.Cells[i, 38].Value != null && !isNumber.IsMatch(ws.Cells[i, 38].Value.ToString().Trim()))
                                          || (ws.Cells[i, 39].Value != null && !isNumber.IsMatch(ws.Cells[i, 39].Value.ToString().Trim()))
                                          || (ws.Cells[i, 40].Value != null && !isNumber.IsMatch(ws.Cells[i, 40].Value.ToString().Trim()))
                                      )
                                    {
                                        isvalid = false;
                                        break;
                                    }

                                    if (getExcelCellIntValue(ws.Cells[i, 5].Value) == 0 && getExcelCellIntValue(ws.Cells[i, 41].Value) == 0
                                     && getExcelCellDecimalValue(ws.Cells[i, 8].Value) == 0 && getExcelCellDecimalValue(ws.Cells[i, 10].Value) == 0 && getExcelCellDecimalValue(ws.Cells[i, 11].Value) == 0

                                     && (ws.Cells[i, 13].Value == null || string.IsNullOrEmpty(Convert.ToString(ws.Cells[i, 13].Value).Trim()))
                                     && getExcelCellIntValue(ws.Cells[i, 14].Value) == 0 && getExcelCellIntValue(ws.Cells[i, 15].Value) == 0 && getExcelCellDecimalValue(ws.Cells[i, 16].Value) == 0

                                     && getExcelCellIntValue(ws.Cells[i, 18].Value) == 0 && getExcelCellDecimalValue(ws.Cells[i, 19].Value) == 0
                                     && getExcelCellIntValue(ws.Cells[i, 22].Value) == 0 && getExcelCellDecimalValue(ws.Cells[i, 23].Value) == 0
                                     && getExcelCellIntValue(ws.Cells[i, 26].Value) == 0 && getExcelCellDecimalValue(ws.Cells[i, 27].Value) == 0
                                     && getExcelCellIntValue(ws.Cells[i, 30].Value) == 0 && getExcelCellDecimalValue(ws.Cells[i, 31].Value) == 0

                                     && getExcelCellDecimalValue(ws.Cells[i, 32].Value) == 0 && getExcelCellDecimalValue(ws.Cells[i, 33].Value) == 0 && getExcelCellDecimalValue(ws.Cells[i, 34].Value) == 0
                                     && getExcelCellDecimalValue(ws.Cells[i, 35].Value) == 0 && getExcelCellDecimalValue(ws.Cells[i, 36].Value) == 0
                                    )
                                    {
                                        continue;
                                    }

                                    #region Part Detail

                                    if (ws.Cells[i, 5].Value == null || ws.Cells[i, 5].Value.ToString() == string.Empty || Convert.ToDecimal(ws.Cells[i, 5].Value) < 0)
                                    {
                                        isvalid = false;
                                        break;
                                    }

                                    if ((ws.Cells[i, 5].Value != null && Convert.ToDecimal(ws.Cells[i, 5].Value) > 0)
                                        && (
                                               (ws.Cells[i, 41].Value != null && Convert.ToDecimal(ws.Cells[i, 41].Value.ToString().Trim()) <= 0)
                                            || (ws.Cells[i, 8].Value != null && Convert.ToDecimal(ws.Cells[i, 8].Value.ToString().Trim()) <= 0)
                                            || (ws.Cells[i, 10].Value != null && Convert.ToDecimal(ws.Cells[i, 10].Value.ToString().Trim()) <= 0)
                                            || (string.IsNullOrEmpty(Convert.ToString(ws.Cells[i, 13])))
                                            || (ws.Cells[i, 14].Value != null && Convert.ToDecimal(ws.Cells[i, 14].Value.ToString().Trim()) <= 0)
                                            || (ws.Cells[i, 15].Value != null && Convert.ToDecimal(ws.Cells[i, 15].Value.ToString().Trim()) <= 0)
                                            )
                                    )
                                    {
                                        isvalid = false;
                                        break;
                                    }
                                    #endregion

                                    #region Machining Tab Validation

                                    if (ws.Cells[i, 19].Value != null && Convert.ToDecimal(ws.Cells[i, 19].Value) > 0)
                                    {
                                        if (ws.Cells[i, 20].Value == null || Convert.ToDecimal(ws.Cells[i, 20].Value) <= 0)
                                        {
                                            isvalid = false;
                                            break;
                                        }
                                    }

                                    if (ws.Cells[i, 20].Value != null && Convert.ToDecimal(ws.Cells[i, 20].Value) > 0)
                                    {
                                        if (ws.Cells[i, 19].Value == null || Convert.ToDecimal(ws.Cells[i, 19].Value) <= 0)
                                        {
                                            isvalid = false;
                                            break;
                                        }
                                    }

                                    #endregion

                                    #region Machining Secondary Operation Tab Validation

                                    if (ws.Cells[i, 23].Value != null && Convert.ToDecimal(ws.Cells[i, 23].Value) > 0)
                                    {
                                        if (ws.Cells[i, 24].Value == null || Convert.ToDecimal(ws.Cells[i, 24].Value) <= 0)
                                        {
                                            isvalid = false;
                                            break;
                                        }
                                    }

                                    if (ws.Cells[i, 24].Value != null && Convert.ToDecimal(ws.Cells[i, 24].Value) > 0)
                                    {
                                        if (ws.Cells[i, 23].Value == null || Convert.ToDecimal(ws.Cells[i, 23].Value) <= 0)
                                        {
                                            isvalid = false;
                                            break;
                                        }
                                    }

                                    #endregion

                                    #region Machining Other Operation Tab Validation

                                    if (ws.Cells[i, 27].Value != null && Convert.ToDecimal(ws.Cells[i, 27].Value) > 0)
                                    {
                                        if (ws.Cells[i, 28].Value == null || Convert.ToDecimal(ws.Cells[i, 28].Value) <= 0)
                                        {
                                            isvalid = false;
                                            break;
                                        }
                                    }

                                    if (ws.Cells[i, 28].Value != null && Convert.ToDecimal(ws.Cells[i, 28].Value) > 0)
                                    {
                                        if (ws.Cells[i, 27].Value == null || Convert.ToDecimal(ws.Cells[i, 27].Value) <= 0)
                                        {
                                            isvalid = false;
                                            break;
                                        }
                                    }

                                    #endregion

                                    #region Surface Treatment

                                    if (ws.Cells[i, 31].Value != null && Convert.ToDecimal(ws.Cells[i, 31].Value) > 0)
                                    {
                                        if (ws.Cells[i, 32].Value == null || Convert.ToDecimal(ws.Cells[i, 32].Value) <= 0)
                                        {
                                            isvalid = false;
                                            break;
                                        }
                                    }

                                    if (ws.Cells[i, 32].Value != null && Convert.ToDecimal(ws.Cells[i, 32].Value) > 0)
                                    {
                                        if (ws.Cells[i, 31].Value == null || Convert.ToDecimal(ws.Cells[i, 31].Value) <= 0)
                                        {
                                            isvalid = false;
                                            break;
                                        }
                                    }

                                    #endregion
                                }
                            }

                            if (isvalid)
                            {
                                decimal exchangeRate = Math.Round(Convert.ToDecimal(ws.Cells[14, 1].Value), 3);
                                string currency = ws.Cells[15, 1].Value.ToString();
                                string rawmaterial = ws.Cells[16, 1].Value != null ? ws.Cells[16, 1].Value.ToString() : string.Empty;
                                string remarks = string.Empty;
                                if (ws.Cells[17, 1].Value != null && !string.IsNullOrEmpty(ws.Cells[17, 1].Value.ToString().Trim()))
                                    remarks = ws.Cells[17, 1].Value.ToString();

                                int counter = 0;
                                decimal varexchangeRate;
                                decimal sum;
                                decimal dRawMaterialTotal = 0;
                                decimal dPrimaryProcessConversionTotal = 0;
                                decimal dMachiningTotal = 0;
                                decimal dMachiningSecondaryOperationTotal = 0;
                                decimal dMachiningOtherOperationTotal = 0;
                                decimal dSurfaceTreatmentTotal = 0;
                                decimal dOverheadTotal = 0;
                                Dictionary<int, string> ppcMachineDescriptionList = new Dictionary<int, string>();

                                this.RunOnDB(DataContext =>
                                {

                                    foreach (var item in DataContext.MachineDescs)
                                    {
                                        ppcMachineDescriptionList.Add(item.Id, item.MachineDescription);
                                    }

                                }, true);
                                Dictionary<int, string> mMachiningDescriptionList = new Dictionary<int, string>();

                                this.RunOnDB(DataContext =>
                                {

                                    foreach (var item in DataContext.MachiningDescs)
                                    {
                                        mMachiningDescriptionList.Add(item.Id, item.MachiningDescription);
                                    }

                                }, true);
                                Dictionary<int, string> msoSecOprDescriptionList = new Dictionary<int, string>();

                                this.RunOnDB(DataContext =>
                                {

                                    foreach (var item in DataContext.SecondaryOperationDescs)
                                    {
                                        msoSecOprDescriptionList.Add(item.Id, item.SecondaryOperationDescription);
                                    }

                                }, true);
                                Dictionary<int, string> stCoatingTypeList = new Dictionary<int, string>();

                                this.RunOnDB(DataContext =>
                                {

                                    foreach (var item in DataContext.CoatingTypes)
                                    {
                                        stCoatingTypeList.Add(item.Id, item.CoatingType1);
                                    }

                                }, true);
                                for (int i = 22; i <= nLastRow; i++)
                                {
                                    #region Part Detail

                                    if (ws.Cells[i, 4].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 4].Value.ToString().Trim()))
                                    {
                                        if (manufacturerList.Where(rec => rec.CompanyName.ToLower().Trim() == ws.Cells[i, 4].Value.ToString().Trim().ToLower()).Count() > 0)
                                        {
                                            SupplierQuoteDetails[counter].ManufacturerId = manufacturerList.Where(rec => rec.CompanyName.ToLower().Trim() == ws.Cells[i, 4].Value.ToString().Trim().ToLower()).Single().Id;
                                            SupplierQuoteDetails[counter].Manufacturer = manufacturerList.Where(rec => rec.CompanyName.ToLower().Trim() == ws.Cells[i, 4].Value.ToString().Trim().ToLower()).Single().CompanyName;
                                        }
                                        else if (manufacturerList.Where(rec => rec.CompanyName.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 4].Value.ToString().Trim().ToLower()).Count() > 0)
                                        {
                                            SupplierQuoteDetails[counter].ManufacturerId = manufacturerList.Where(rec => rec.CompanyName.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 4].Value.ToString().Trim().ToLower()).Single().Id;
                                            SupplierQuoteDetails[counter].Manufacturer = manufacturerList.Where(rec => rec.CompanyName.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 4].Value.ToString().Trim().ToLower()).Single().CompanyName;
                                        }
                                        else
                                        {
                                            SupplierQuoteDetails[counter].ManufacturerId = null;
                                            SupplierQuoteDetails[counter].Manufacturer = ws.Cells[i, 4].Value.ToString().Trim();
                                        }
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].ManufacturerId = null;
                                        SupplierQuoteDetails[counter].Manufacturer = "";
                                    }
                                    #region Part Detail
                                    if (ws.Cells[i, 5].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 5].Value.ToString().Trim()))
                                        SupplierQuoteDetails[counter].MinOrderQty = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 5].Value), 0);
                                    #endregion
                                    #region Tooling
                                    if (ws.Cells[i, 42].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 42].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 42].Value) > 0)
                                        SupplierQuoteDetails[counter].ToolingCost = Math.Round(Convert.ToDecimal(ws.Cells[i, 42].Value), 3);
                                    else
                                        SupplierQuoteDetails[counter].ToolingCost = 0;

                                    SupplierQuoteDetails[counter].ToolingWarranty = ws.Cells[i, 43].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 43].Value.ToString().Trim()) ? ws.Cells[i, 43].Value.ToString() : string.Empty;
                                    SupplierQuoteDetails[counter].SupplierToolingLeadtime = ws.Cells[i, 44].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 44].Value.ToString().Trim()) ? ws.Cells[i, 44].Value.ToString() : string.Empty;

                                    if (ws.Cells[i, 41].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 41].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 41].Value) > 0)
                                        SupplierQuoteDetails[counter].NoOfCavities = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 41].Value), 0);
                                    else
                                        SupplierQuoteDetails[counter].NoOfCavities = 0;
                                    #endregion
                                    SupplierQuoteDetails[counter].Remarks = remarks;
                                    varexchangeRate = exchangeRate;
                                    SupplierQuoteDetails[counter].ExchangeRate = varexchangeRate;
                                    SupplierQuoteDetails[counter].Currency = currency;
                                    SupplierQuoteDetails[counter].RawMaterialPriceAssumed = rawmaterial;
                                    SupplierQuoteDetails[counter].IsQuoteTypeDQ = criteria.IsQuoteTypeDQ;
                                    #endregion

                                    #region Raw Material

                                    dRawMaterialTotal = 0;
                                    if (ws.Cells[i, 8].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 8].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 8].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatInputInKg = Math.Round(Convert.ToDecimal(ws.Cells[i, 8].Value), 3);
                                        dRawMaterialTotal = Convert.ToDecimal(ws.Cells[i, 8].Value);
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatInputInKg = 0;
                                    }

                                    if (ws.Cells[i, 10].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 10].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 10].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatCostPerKg = Math.Round(Convert.ToDecimal(ws.Cells[i, 10].Value), 3);
                                        dRawMaterialTotal = dRawMaterialTotal * Convert.ToDecimal(ws.Cells[i, 10].Value);
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatCostPerKg = 0;
                                    }

                                    if (ws.Cells[i, 8].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 8].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 8].Value) > 0)
                                    {
                                        if (SupplierQuoteDetails[counter].PartWeightKG.HasValue)
                                            SupplierQuoteDetails[counter].rFQdqRawMaterial.MaterialLoss = (SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatInputInKg - SupplierQuoteDetails[counter].PartWeightKG) / SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatInputInKg;
                                        else
                                            SupplierQuoteDetails[counter].rFQdqRawMaterial.MaterialLoss = 1;
                                    }
                                    else
                                        SupplierQuoteDetails[counter].rFQdqRawMaterial.MaterialLoss = 0;


                                    SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatTotal = Math.Round(dRawMaterialTotal, 3);

                                    #endregion

                                    #region Primary Process/Conversion

                                    if (ws.Cells[i, 13].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 13].Value.ToString().Trim()))
                                    {
                                        if (ppcMachineDescriptionList.Where(rec => rec.Value.ToLower().Trim() == ws.Cells[i, 13].Value.ToString().Trim().ToLower()).Count() > 0)
                                        {
                                            SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.MachineDescId = ppcMachineDescriptionList.Where(rec => rec.Value.ToLower().Trim() == ws.Cells[i, 13].Value.ToString().Trim().ToLower()).FirstOrDefault().Key;
                                            SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.MachineDescription = ws.Cells[i, 13].Value.ToString().Trim();
                                        }
                                        else if (ppcMachineDescriptionList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 13].Value.ToString().Trim().ToLower()).Count() > 0)
                                        {
                                            SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.MachineDescId = ppcMachineDescriptionList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 13].Value.ToString().Trim().ToLower()).FirstOrDefault().Key;
                                            SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.MachineDescription = ppcMachineDescriptionList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 13].Value.ToString().Trim().ToLower()).FirstOrDefault().Value;
                                        }
                                        else
                                        {
                                            SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.MachineDescId = null;
                                            SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.MachineDescription = ws.Cells[i, 13].Value.ToString().Trim();
                                        }
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.MachineDescId = null;
                                        SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.MachineDescription = "";
                                    }

                                    if (ws.Cells[i, 14].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 14].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 14].Value) > 0)
                                        SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.MachineSize = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 14].Value), 0);
                                    else
                                        SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.MachineSize = 0;

                                    dPrimaryProcessConversionTotal = 0;
                                    if (ws.Cells[i, 16].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 16].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 16].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.ManPlusMachineRatePerHour = Math.Round(Convert.ToDecimal(ws.Cells[i, 16].Value), 3);
                                        dPrimaryProcessConversionTotal = Math.Round(Convert.ToDecimal(ws.Cells[i, 16].Value), 3) / 3600;
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.ManPlusMachineRatePerHour = 0;
                                    }

                                    if (ws.Cells[i, 15].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 15].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 15].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.CycleTime = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 15].Value), 0);
                                        dPrimaryProcessConversionTotal = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 15].Value), 0) * dPrimaryProcessConversionTotal;
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.CycleTime = 0;
                                        dPrimaryProcessConversionTotal = 0;
                                    }
                                    SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.ProcessConversionCostPerPart = Math.Round(dPrimaryProcessConversionTotal, 3);

                                    #endregion

                                    #region Machining

                                    if (ws.Cells[i, 18].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 18].Value.ToString().Trim()))
                                    {
                                        if (mMachiningDescriptionList.Where(rec => rec.Value.ToLower().Trim() == ws.Cells[i, 18].Value.ToString().Trim().ToLower()).Count() > 0)
                                        {
                                            SupplierQuoteDetails[counter].rFQdqMachining.MachiningDescId = mMachiningDescriptionList.Where(rec => rec.Value.ToLower().Trim() == ws.Cells[i, 18].Value.ToString().Trim().ToLower()).FirstOrDefault().Key;
                                            SupplierQuoteDetails[counter].rFQdqMachining.MachiningDescription = ws.Cells[i, 18].Value.ToString().Trim();
                                        }
                                        else if (mMachiningDescriptionList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 18].Value.ToString().Trim().ToLower()).Count() > 0)
                                        {
                                            SupplierQuoteDetails[counter].rFQdqMachining.MachiningDescId = mMachiningDescriptionList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 18].Value.ToString().Trim().ToLower()).FirstOrDefault().Key;
                                            SupplierQuoteDetails[counter].rFQdqMachining.MachiningDescription = mMachiningDescriptionList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 18].Value.ToString().Trim().ToLower()).FirstOrDefault().Value;
                                        }
                                        else
                                        {
                                            SupplierQuoteDetails[counter].rFQdqMachining.MachiningDescId = null;
                                            SupplierQuoteDetails[counter].rFQdqMachining.MachiningDescription = ws.Cells[i, 18].Value.ToString().Trim();
                                        }
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachining.MachiningDescId = null;
                                        SupplierQuoteDetails[counter].rFQdqMachining.MachiningDescription = "";
                                    }

                                    dMachiningTotal = 0;
                                    if (ws.Cells[i, 20].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 20].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 20].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachining.ManPlusMachineRatePerHour = Math.Round(Convert.ToDecimal(ws.Cells[i, 20].Value), 3);
                                        dMachiningTotal = Convert.ToDecimal(ws.Cells[i, 20].Value) / 3600;
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachining.ManPlusMachineRatePerHour = 0;
                                    }

                                    if (ws.Cells[i, 19].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 19].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 19].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachining.CycleTime = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 19].Value), 0);
                                        dMachiningTotal = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 19].Value), 0) * dMachiningTotal;
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachining.CycleTime = 0;
                                        dMachiningTotal = 0;
                                    }
                                    SupplierQuoteDetails[counter].rFQdqMachining.MachiningCostPerPart = Math.Round(dMachiningTotal, 3);

                                    #endregion

                                    #region Machining Secondary Operation

                                    if (ws.Cells[i, 22].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 22].Value.ToString().Trim()))
                                    {
                                        if (msoSecOprDescriptionList.Where(rec => rec.Value.ToLower().Trim() == ws.Cells[i, 22].Value.ToString().Trim().ToLower()).Count() > 0)
                                        {
                                            SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.SecondaryOperationDescId = msoSecOprDescriptionList.Where(rec => rec.Value.ToLower().Trim() == ws.Cells[i, 22].Value.ToString().Trim().ToLower()).FirstOrDefault().Key;
                                            SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.SecondaryOperationDescription = ws.Cells[i, 22].Value.ToString().Trim();
                                        }
                                        else if (msoSecOprDescriptionList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 22].Value.ToString().Trim().ToLower()).Count() > 0)
                                        {
                                            SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.SecondaryOperationDescId = msoSecOprDescriptionList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 22].Value.ToString().Trim().ToLower()).FirstOrDefault().Key;
                                            SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.SecondaryOperationDescription = msoSecOprDescriptionList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 25].Value.ToString().Trim().ToLower()).FirstOrDefault().Value;
                                        }
                                        else
                                        {
                                            SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.SecondaryOperationDescId = null;
                                            SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.SecondaryOperationDescription = ws.Cells[i, 22].Value.ToString().Trim();
                                        }
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.SecondaryOperationDescId = null;
                                        SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.SecondaryOperationDescription = "";
                                    }

                                    dMachiningSecondaryOperationTotal = 0;
                                    if (ws.Cells[i, 24].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 24].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 24].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour = Math.Round(Convert.ToDecimal(ws.Cells[i, 24].Value), 3);
                                        dMachiningSecondaryOperationTotal = Convert.ToDecimal(ws.Cells[i, 24].Value) / 3600;
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour = 0;
                                    }

                                    if (ws.Cells[i, 23].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 23].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 23].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.CycleTime = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 23].Value), 0);
                                        dMachiningSecondaryOperationTotal = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 23].Value), 0) * dMachiningSecondaryOperationTotal;
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.CycleTime = 0;
                                        dMachiningSecondaryOperationTotal = 0;
                                    }
                                    SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.SecondaryCostPerPart = Math.Round(dMachiningSecondaryOperationTotal, 3);

                                    #endregion

                                    #region Machining Other Operation

                                    if (ws.Cells[i, 26].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 26].Value.ToString().Trim()))
                                    {
                                        if (msoSecOprDescriptionList.Where(rec => rec.Value.ToLower().Trim() == ws.Cells[i, 26].Value.ToString().Trim().ToLower()).Count() > 0)
                                        {
                                            SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.SecondaryOperationDescId = msoSecOprDescriptionList.Where(rec => rec.Value.ToLower().Trim() == ws.Cells[i, 26].Value.ToString().Trim().ToLower()).FirstOrDefault().Key;
                                            SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.SecondaryOperationDescription = ws.Cells[i, 26].Value.ToString().Trim();
                                        }
                                        else if (msoSecOprDescriptionList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 26].Value.ToString().Trim().ToLower()).Count() > 0)
                                        {
                                            SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.SecondaryOperationDescId = msoSecOprDescriptionList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 26].Value.ToString().Trim().ToLower()).FirstOrDefault().Key;
                                            SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.SecondaryOperationDescription = msoSecOprDescriptionList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 26].Value.ToString().Trim().ToLower()).FirstOrDefault().Value;
                                        }
                                        else
                                        {
                                            SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.SecondaryOperationDescId = null;
                                            SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.SecondaryOperationDescription = ws.Cells[i, 26].Value.ToString().Trim();
                                        }
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.SecondaryOperationDescId = null;
                                        SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.SecondaryOperationDescription = "";
                                    }

                                    dMachiningOtherOperationTotal = 0;
                                    if (ws.Cells[i, 28].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 28].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 28].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour = Math.Round(Convert.ToDecimal(ws.Cells[i, 28].Value), 3);
                                        dMachiningOtherOperationTotal = Convert.ToDecimal(ws.Cells[i, 28].Value) / 3600;
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour = 0;
                                    }

                                    if (ws.Cells[i, 27].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 27].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 27].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.CycleTime = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 27].Value), 0);
                                        dMachiningOtherOperationTotal = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 27].Value), 0) * dMachiningOtherOperationTotal;
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.CycleTime = 0;
                                        dMachiningOtherOperationTotal = 0;
                                    }
                                    SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.SecondaryCostPerPart = Math.Round(dMachiningOtherOperationTotal, 3);

                                    #endregion

                                    #region Surface Treatment

                                    if (ws.Cells[i, 30].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 30].Value.ToString().Trim()))
                                    {
                                        if (stCoatingTypeList.Where(rec => rec.Value.ToLower().Trim() == ws.Cells[i, 30].Value.ToString().Trim().ToLower()).Count() > 0)
                                        {
                                            SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.CoatingTypeId = stCoatingTypeList.Where(rec => rec.Value.ToLower().Trim() == ws.Cells[i, 30].Value.ToString().Trim().ToLower()).FirstOrDefault().Key;
                                            SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.CoatingType = ws.Cells[i, 30].Value.ToString().Trim();
                                        }
                                        else if (stCoatingTypeList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 30].Value.ToString().Trim().ToLower()).Count() > 0)
                                        {
                                            SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.CoatingTypeId = stCoatingTypeList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 30].Value.ToString().Trim().ToLower()).FirstOrDefault().Key;
                                            SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.CoatingType = stCoatingTypeList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 30].Value.ToString().Trim().ToLower()).FirstOrDefault().Value;
                                        }
                                        else
                                        {
                                            SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.CoatingTypeId = null;
                                            SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.CoatingType = ws.Cells[i, 30].Value.ToString().Trim();
                                        }
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.CoatingTypeId = null;
                                        SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.CoatingType = "";
                                    }

                                    dSurfaceTreatmentTotal = 0;
                                    if (ws.Cells[i, 32].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 32].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 32].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.ManPlusMachineRatePerHour = Math.Round(Convert.ToDecimal(ws.Cells[i, 32].Value), 3);
                                        dSurfaceTreatmentTotal = Convert.ToDecimal(ws.Cells[i, 32].Value);
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.ManPlusMachineRatePerHour = 0;
                                    }

                                    if (ws.Cells[i, 31].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 31].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 31].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.PartsPerHour = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 31].Value), 0);
                                        dSurfaceTreatmentTotal = dSurfaceTreatmentTotal / (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 31].Value), 0);
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.PartsPerHour = 0;
                                        dSurfaceTreatmentTotal = 0;
                                    }
                                    SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.CoatingCostPerHour = Math.Round(dSurfaceTreatmentTotal, 3);

                                    #endregion

                                    #region Overhead

                                    dOverheadTotal = 0;
                                    if (ws.Cells[i, 34].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 34].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 34].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqOverhead.InventoryCarryingCost = Math.Round(Convert.ToDecimal(ws.Cells[i, 34].Value), 3);
                                        dOverheadTotal = Convert.ToDecimal(ws.Cells[i, 34].Value);
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqOverhead.InventoryCarryingCost = 0;
                                    }

                                    if (ws.Cells[i, 35].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 35].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 35].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqOverhead.PackagingMaterial = Math.Round(Convert.ToDecimal(ws.Cells[i, 35].Value), 3);
                                        dOverheadTotal = dOverheadTotal + Convert.ToDecimal(ws.Cells[i, 35].Value);
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqOverhead.PackagingMaterial = 0;
                                    }

                                    if (ws.Cells[i, 36].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 36].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 36].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqOverhead.Packing = Math.Round(Convert.ToDecimal(ws.Cells[i, 36].Value), 3);
                                        dOverheadTotal = dOverheadTotal + Convert.ToDecimal(ws.Cells[i, 36].Value);
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqOverhead.Packing = 0;
                                    }

                                    if (ws.Cells[i, 37].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 37].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 37].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqOverhead.LocalFreightToPort = Math.Round(Convert.ToDecimal(ws.Cells[i, 37].Value), 3);
                                        dOverheadTotal = dOverheadTotal + Convert.ToDecimal(ws.Cells[i, 37].Value);
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqOverhead.LocalFreightToPort = 0;
                                    }

                                    if (ws.Cells[i, 38].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 38].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 38].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqOverhead.ProfitAndSGA = Math.Round(Convert.ToDecimal(ws.Cells[i, 38].Value), 3);
                                        dOverheadTotal = dOverheadTotal + Convert.ToDecimal(ws.Cells[i, 38].Value);
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqOverhead.ProfitAndSGA = 0;
                                    }

                                    sum = dRawMaterialTotal + dPrimaryProcessConversionTotal + dMachiningTotal + dMachiningSecondaryOperationTotal + dMachiningOtherOperationTotal + dSurfaceTreatmentTotal + dOverheadTotal;
                                    SupplierQuoteDetails[counter].UnitPrice = Math.Round(sum, 3);

                                    if (sum > 0 && dOverheadTotal > 0)
                                        SupplierQuoteDetails[counter].rFQdqOverhead.OverheadPercentPiecePrice = Math.Round(dOverheadTotal * 100 / sum, 2);
                                    else
                                        SupplierQuoteDetails[counter].rFQdqOverhead.OverheadPercentPiecePrice = Math.Round((decimal)0, 2);

                                    #endregion
                                    counter++;
                                }

                                SupplierQuoteDetails[0].CreatedBy = supplierItem.CompanyName;
                                SupplierQuoteDetails[0].Remarks = remarks;
                                successMessage = Languages.GetResourceText("QuoteUploadSuccess");
                            }
                            else
                            {
                                errMsg = Languages.GetResourceText("UploadQuoteErrMsg2"); //"Please fill the mandatory data in the excel file and upload.";                              
                            }

                        }
                    }
                    else
                    {
                        errMsg = Languages.GetResourceText("UploadQuoteErrMsg3");// "The uploaded file is blank. Please upload a valid file.";
                    }
                }
                catch (Exception ex) //Error
                {
                    //"Error - Loading xlsx",
                    throw ex;
                }
                finally
                {
                    ws = null;
                    ef.ClosePreservedXlsx();
                }
            }
            catch (Exception ex) //Error
            {
                //"Error - LoadCSV",	

                throw ex;
            }

            return SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>>(errMsg, SupplierQuoteDetails, successMessage);
        }

        private ITypedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>> LoadDetailQuoteExcel_SubmitQuote(DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria criteria)
        {
            List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> SupplierQuoteDetails = new List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>();
            SupplierQuoteDetails = GetDQRFQSupplierPartQuoteListbyUniqueURL(criteria).Result;
            Supplier.Suppliers supplierObj = new Supplier.Suppliers();
            DTO.Library.RFQ.Supplier.Suppliers supplierItem = supplierObj.FindById(criteria.SupplierId).Result;

            var context = HttpContext.Current;
            string errMsg = string.Empty;
            string successMessage = string.Empty;

            try
            {
                Stream memoryStream = MES.Business.Library.Helper.BlobHelper.AttachFile_ConvertBlobToStream(criteria.UploadQuoteFilePath);
                ExcelFile ef = new ExcelFile();
                ef.LoadXlsx(memoryStream, XlsxOptions.None);
                ExcelWorksheet ws = ef.Worksheets[0];
                var isNumber = new Regex(@"^\d{0,7}(\.\d{0,50})?$");

                try
                {
                    // Find the last real row
                    int nLastRow = 0;
                    bool isvalid = true;
                    if (ws.GetUsedCellRange(true) != null && (nLastRow = ws.GetUsedCellRange(true).LastRowIndex) > 0
                        && ws.Cells["A13"].Value != null && ws.Cells["A13"].Value.ToString().ToLower().Trim() == "request for quote (rfq)"
                        && ws.Cells["A21"].Value != null && ws.Cells["A21"].Value.ToString().ToLower().Trim() == "part detail")
                    {
                        if (ws.Cells[7, 1].Value != null && supplierItem.CompanyName != ws.Cells[7, 1].Value.ToString() && ws.Cells[8, 1].Value != null && criteria.RFQId != ws.Cells[8, 1].Value.ToString())
                        {
                            errMsg = Languages.GetResourceText("UploadQuoteErrMsg1");// "It seems that in this uploaded file, the information, other than the required ones have been modified.<br />Please download the RFQ file again and fill in the required values and upload it.";

                        }
                        else
                        {

                            if (ws.Cells[14, 1].Value == null || ws.Cells[15, 1].Value == null)
                                isvalid = false;
                            else if (ws.Cells[14, 1].Value != null && !isNumber.IsMatch(ws.Cells[14, 1].Value.ToString()))
                                isvalid = false;
                            else
                            {
                                for (int i = 22; i <= nLastRow; i++)
                                {
                                    if ((ws.Cells[i, 3].Value != null && !isNumber.IsMatch(ws.Cells[i, 3].Value.ToString().Trim()))  // Part Detail Tab
                                          || (ws.Cells[i, 40].Value != null && !isNumber.IsMatch(ws.Cells[i, 40].Value.ToString().Trim()))

                                          || (ws.Cells[i, 7].Value != null && !isNumber.IsMatch(ws.Cells[i, 7].Value.ToString().Trim())) // Raq Material Tab
                                          || (ws.Cells[i, 9].Value != null && !isNumber.IsMatch(ws.Cells[i, 9].Value.ToString().Trim()))
                                          || (ws.Cells[i, 6].Value != null && !isNumber.IsMatch(ws.Cells[i, 6].Value.ToString().Trim()))
                                          || (ws.Cells[i, 11].Value != null && !isNumber.IsMatch(ws.Cells[i, 11].Value.ToString().Trim()))

                                          || (ws.Cells[i, 13].Value != null && !isNumber.IsMatch(ws.Cells[i, 13].Value.ToString().Trim()))//Primary Process/Conversion Machine Description
                                          || (ws.Cells[i, 14].Value != null && !isNumber.IsMatch(ws.Cells[i, 14].Value.ToString().Trim()))
                                          || (ws.Cells[i, 15].Value != null && !isNumber.IsMatch(ws.Cells[i, 15].Value.ToString().Trim()))
                                          || (ws.Cells[i, 16].Value != null && !isNumber.IsMatch(ws.Cells[i, 16].Value.ToString().Trim()))

                                          || (ws.Cells[i, 18].Value != null && !isNumber.IsMatch(ws.Cells[i, 18].Value.ToString().Trim())) // Machining Tab
                                          || (ws.Cells[i, 19].Value != null && !isNumber.IsMatch(ws.Cells[i, 19].Value.ToString().Trim()))
                                          || (ws.Cells[i, 20].Value != null && !isNumber.IsMatch(ws.Cells[i, 20].Value.ToString().Trim()))

                                          || (ws.Cells[i, 22].Value != null && !isNumber.IsMatch(ws.Cells[i, 22].Value.ToString().Trim())) // Machining Secondary Operation Tab
                                          || (ws.Cells[i, 23].Value != null && !isNumber.IsMatch(ws.Cells[i, 23].Value.ToString().Trim()))
                                          || (ws.Cells[i, 24].Value != null && !isNumber.IsMatch(ws.Cells[i, 24].Value.ToString().Trim()))

                                          || (ws.Cells[i, 26].Value != null && !isNumber.IsMatch(ws.Cells[i, 26].Value.ToString().Trim())) // Machining Other Operation Tab
                                          || (ws.Cells[i, 27].Value != null && !isNumber.IsMatch(ws.Cells[i, 27].Value.ToString().Trim()))
                                          || (ws.Cells[i, 28].Value != null && !isNumber.IsMatch(ws.Cells[i, 28].Value.ToString().Trim()))

                                          || (ws.Cells[i, 30].Value != null && !isNumber.IsMatch(ws.Cells[i, 30].Value.ToString().Trim())) // Surface Treament Tab
                                          || (ws.Cells[i, 31].Value != null && !isNumber.IsMatch(ws.Cells[i, 31].Value.ToString().Trim()))
                                          || (ws.Cells[i, 32].Value != null && !isNumber.IsMatch(ws.Cells[i, 32].Value.ToString().Trim()))

                                          || (ws.Cells[i, 33].Value != null && !isNumber.IsMatch(ws.Cells[i, 33].Value.ToString().Trim())) // Overhead Tab
                                          || (ws.Cells[i, 34].Value != null && !isNumber.IsMatch(ws.Cells[i, 34].Value.ToString().Trim()))
                                          || (ws.Cells[i, 35].Value != null && !isNumber.IsMatch(ws.Cells[i, 35].Value.ToString().Trim()))
                                          || (ws.Cells[i, 36].Value != null && !isNumber.IsMatch(ws.Cells[i, 36].Value.ToString().Trim()))
                                          || (ws.Cells[i, 37].Value != null && !isNumber.IsMatch(ws.Cells[i, 37].Value.ToString().Trim()))
                                          || (ws.Cells[i, 38].Value != null && !isNumber.IsMatch(ws.Cells[i, 38].Value.ToString().Trim()))
                                          || (ws.Cells[i, 39].Value != null && !isNumber.IsMatch(ws.Cells[i, 39].Value.ToString().Trim()))
                                      )
                                    {
                                        isvalid = false;
                                        break;
                                    }


                                    if (getExcelCellIntValue(ws.Cells[i, 4].Value) == 0 && getExcelCellIntValue(ws.Cells[i, 40].Value) == 0
                                        && getExcelCellDecimalValue(ws.Cells[i, 7].Value) == 0 && getExcelCellDecimalValue(ws.Cells[i, 9].Value) == 0 && getExcelCellDecimalValue(ws.Cells[i, 10].Value) == 0

                                        && (ws.Cells[i, 12].Value == null || string.IsNullOrEmpty(Convert.ToString(ws.Cells[i, 12].Value).Trim()))
                                        && getExcelCellIntValue(ws.Cells[i, 13].Value) == 0 && getExcelCellIntValue(ws.Cells[i, 14].Value) == 0 && getExcelCellDecimalValue(ws.Cells[i, 15].Value) == 0

                                        && getExcelCellIntValue(ws.Cells[i, 17].Value) == 0 && getExcelCellDecimalValue(ws.Cells[i, 18].Value) == 0
                                        && getExcelCellIntValue(ws.Cells[i, 21].Value) == 0 && getExcelCellDecimalValue(ws.Cells[i, 22].Value) == 0
                                        && getExcelCellIntValue(ws.Cells[i, 25].Value) == 0 && getExcelCellDecimalValue(ws.Cells[i, 26].Value) == 0
                                        && getExcelCellIntValue(ws.Cells[i, 29].Value) == 0 && getExcelCellDecimalValue(ws.Cells[i, 30].Value) == 0

                                        && getExcelCellDecimalValue(ws.Cells[i, 31].Value) == 0 && getExcelCellDecimalValue(ws.Cells[i, 32].Value) == 0 && getExcelCellDecimalValue(ws.Cells[i, 33].Value) == 0
                                        && getExcelCellDecimalValue(ws.Cells[i, 34].Value) == 0 && getExcelCellDecimalValue(ws.Cells[i, 35].Value) == 0
                                    )
                                    {
                                        continue;
                                    }


                                    #region Part Detail

                                    if (ws.Cells[i, 4].Value == null || ws.Cells[i, 4].Value.ToString() == string.Empty || Convert.ToDecimal(ws.Cells[i, 4].Value) < 0)
                                    {
                                        isvalid = false;
                                        break;
                                    }

                                    if ((ws.Cells[i, 4].Value != null && Convert.ToDecimal(ws.Cells[i, 4].Value) > 0)
                                        && (
                                            (ws.Cells[i, 40].Value != null && Convert.ToDecimal(ws.Cells[i, 40].Value.ToString().Trim()) <= 0)
                                            || (ws.Cells[i, 7].Value != null && Convert.ToDecimal(ws.Cells[i, 7].Value.ToString().Trim()) <= 0)
                                            || (ws.Cells[i, 9].Value != null && Convert.ToDecimal(ws.Cells[i, 9].Value.ToString().Trim()) <= 0)
                                            || (string.IsNullOrEmpty(Convert.ToString(ws.Cells[i, 12])))
                                            || (ws.Cells[i, 13].Value != null && Convert.ToDecimal(ws.Cells[i, 13].Value.ToString().Trim()) <= 0)
                                            || (ws.Cells[i, 14].Value != null && Convert.ToDecimal(ws.Cells[i, 14].Value.ToString().Trim()) <= 0)
                                            )
                                    )
                                    {
                                        isvalid = false;
                                        break;
                                    }


                                    //}

                                    #endregion

                                    #region Machining Tab Validation

                                    if (ws.Cells[i, 18].Value != null && Convert.ToDecimal(ws.Cells[i, 18].Value) > 0)
                                    {
                                        if (ws.Cells[i, 19].Value == null || Convert.ToDecimal(ws.Cells[i, 19].Value) <= 0)
                                        {
                                            isvalid = false;
                                            break;
                                        }
                                    }

                                    if (ws.Cells[i, 19].Value != null && Convert.ToDecimal(ws.Cells[i, 19].Value) > 0)
                                    {
                                        if (ws.Cells[i, 18].Value == null || Convert.ToDecimal(ws.Cells[i, 18].Value) <= 0)
                                        {
                                            isvalid = false;
                                            break;
                                        }
                                    }

                                    #endregion

                                    #region Machining Secondary Operation Tab Validation

                                    if (ws.Cells[i, 22].Value != null && Convert.ToDecimal(ws.Cells[i, 22].Value) > 0)
                                    {
                                        if (ws.Cells[i, 23].Value == null || Convert.ToDecimal(ws.Cells[i, 23].Value) <= 0)
                                        {
                                            isvalid = false;
                                            break;
                                        }
                                    }

                                    if (ws.Cells[i, 23].Value != null && Convert.ToDecimal(ws.Cells[i, 23].Value) > 0)
                                    {
                                        if (ws.Cells[i, 22].Value == null || Convert.ToDecimal(ws.Cells[i, 22].Value) <= 0)
                                        {
                                            isvalid = false;
                                            break;
                                        }
                                    }

                                    #endregion

                                    #region Machining Other Operation Tab Validation

                                    if (ws.Cells[i, 26].Value != null && Convert.ToDecimal(ws.Cells[i, 26].Value) > 0)
                                    {
                                        if (ws.Cells[i, 27].Value == null || Convert.ToDecimal(ws.Cells[i, 27].Value) <= 0)
                                        {
                                            isvalid = false;
                                            break;
                                        }
                                    }

                                    if (ws.Cells[i, 27].Value != null && Convert.ToDecimal(ws.Cells[i, 27].Value) > 0)
                                    {
                                        if (ws.Cells[i, 26].Value == null || Convert.ToDecimal(ws.Cells[i, 26].Value) <= 0)
                                        {
                                            isvalid = false;
                                            break;
                                        }
                                    }

                                    #endregion

                                    #region Surface Treatment

                                    if (ws.Cells[i, 30].Value != null && Convert.ToDecimal(ws.Cells[i, 30].Value) > 0)
                                    {
                                        if (ws.Cells[i, 31].Value == null || Convert.ToDecimal(ws.Cells[i, 31].Value) <= 0)
                                        {
                                            isvalid = false;
                                            break;
                                        }
                                    }

                                    if (ws.Cells[i, 31].Value != null && Convert.ToDecimal(ws.Cells[i, 31].Value) > 0)
                                    {
                                        if (ws.Cells[i, 30].Value == null || Convert.ToDecimal(ws.Cells[i, 30].Value) <= 0)
                                        {
                                            isvalid = false;
                                            break;
                                        }
                                    }

                                    #endregion
                                }
                            }

                            if (isvalid)
                            {
                                decimal exchangeRate = Math.Round(Convert.ToDecimal(ws.Cells[14, 1].Value), 3);
                                string currency = ws.Cells[15, 1].Value.ToString();
                                string rawmaterial = ws.Cells[16, 1].Value != null ? ws.Cells[16, 1].Value.ToString() : string.Empty;
                                string remarks = string.Empty;
                                if (ws.Cells[17, 1].Value != null && !string.IsNullOrEmpty(ws.Cells[17, 1].Value.ToString().Trim()))
                                    remarks = ws.Cells[17, 1].Value.ToString();

                                int counter = 0;
                                decimal varexchangeRate;
                                decimal sum;
                                decimal dRawMaterialTotal = 0;
                                decimal dPrimaryProcessConversionTotal = 0;
                                decimal dMachiningTotal = 0;
                                decimal dMachiningSecondaryOperationTotal = 0;
                                decimal dMachiningOtherOperationTotal = 0;
                                decimal dSurfaceTreatmentTotal = 0;
                                decimal dOverheadTotal = 0;
                                Dictionary<int, string> ppcMachineDescriptionList = new Dictionary<int, string>();

                                this.RunOnDB(DataContext =>
                                {

                                    foreach (var item in DataContext.MachineDescs)
                                    {
                                        ppcMachineDescriptionList.Add(item.Id, item.MachineDescription);
                                    }

                                }, true);
                                Dictionary<int, string> mMachiningDescriptionList = new Dictionary<int, string>();

                                this.RunOnDB(DataContext =>
                                {

                                    foreach (var item in DataContext.MachiningDescs)
                                    {
                                        mMachiningDescriptionList.Add(item.Id, item.MachiningDescription);
                                    }

                                }, true);
                                Dictionary<int, string> msoSecOprDescriptionList = new Dictionary<int, string>();

                                this.RunOnDB(DataContext =>
                                {

                                    foreach (var item in DataContext.SecondaryOperationDescs)
                                    {
                                        msoSecOprDescriptionList.Add(item.Id, item.SecondaryOperationDescription);
                                    }

                                }, true);
                                Dictionary<int, string> stCoatingTypeList = new Dictionary<int, string>();

                                this.RunOnDB(DataContext =>
                                {

                                    foreach (var item in DataContext.CoatingTypes)
                                    {
                                        stCoatingTypeList.Add(item.Id, item.CoatingType1);
                                    }

                                }, true);

                                for (int i = 22; i <= nLastRow; i++)
                                {
                                    #region Part Detail

                                    if (ws.Cells[i, 4].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 4].Value.ToString().Trim()))
                                        SupplierQuoteDetails[counter].MinOrderQty = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 4].Value), 0);

                                    #endregion

                                    #region Tooling
                                    if (ws.Cells[i, 41].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 41].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 41].Value) > 0)
                                        SupplierQuoteDetails[counter].ToolingCost = Math.Round(Convert.ToDecimal(ws.Cells[i, 41].Value), 3);
                                    else
                                        SupplierQuoteDetails[counter].ToolingCost = 0;

                                    SupplierQuoteDetails[counter].ToolingWarranty = ws.Cells[i, 42].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 42].Value.ToString().Trim()) ? ws.Cells[i, 42].Value.ToString() : string.Empty;
                                    SupplierQuoteDetails[counter].SupplierToolingLeadtime = ws.Cells[i, 43].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 43].Value.ToString().Trim()) ? ws.Cells[i, 43].Value.ToString() : string.Empty;

                                    if (ws.Cells[i, 40].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 40].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 40].Value) > 0)
                                        SupplierQuoteDetails[counter].NoOfCavities = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 40].Value), 0);
                                    else
                                        SupplierQuoteDetails[counter].NoOfCavities = 0;
                                    #endregion

                                    #region Raw Material

                                    dRawMaterialTotal = 0;
                                    if (ws.Cells[i, 7].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 7].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 7].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatInputInKg = Math.Round(Convert.ToDecimal(ws.Cells[i, 7].Value), 3);
                                        dRawMaterialTotal = Convert.ToDecimal(ws.Cells[i, 7].Value);
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatInputInKg = 0;
                                    }

                                    if (ws.Cells[i, 9].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 9].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 9].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatCostPerKg = Math.Round(Convert.ToDecimal(ws.Cells[i, 9].Value), 3);
                                        dRawMaterialTotal = dRawMaterialTotal * Convert.ToDecimal(ws.Cells[i, 9].Value);
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatCostPerKg = 0;
                                    }

                                    if (ws.Cells[i, 7].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 7].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 7].Value) > 0)
                                    {
                                        if (SupplierQuoteDetails[counter].PartWeightKG.HasValue)
                                            SupplierQuoteDetails[counter].rFQdqRawMaterial.MaterialLoss = (SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatInputInKg - SupplierQuoteDetails[counter].PartWeightKG) / SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatInputInKg;
                                        else
                                            SupplierQuoteDetails[counter].rFQdqRawMaterial.MaterialLoss = 1;
                                    }
                                    else
                                        SupplierQuoteDetails[counter].rFQdqRawMaterial.MaterialLoss = 0;

                                    SupplierQuoteDetails[counter].rFQdqRawMaterial.RawMatTotal = Math.Round(dRawMaterialTotal, 3);

                                    #endregion

                                    #region Primary Process/Conversion

                                    if (ws.Cells[i, 12].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 12].Value.ToString().Trim()))
                                    {
                                        if (ppcMachineDescriptionList.Where(rec => rec.Value.ToLower().Trim() == ws.Cells[i, 12].Value.ToString().Trim().ToLower()).Count() > 0)
                                        {
                                            SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.MachineDescId = ppcMachineDescriptionList.Where(rec => rec.Value.ToLower().Trim() == ws.Cells[i, 12].Value.ToString().Trim().ToLower()).FirstOrDefault().Key;
                                            SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.MachineDescription = ws.Cells[i, 12].Value.ToString().Trim();
                                        }
                                        else if (ppcMachineDescriptionList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 12].Value.ToString().Trim().ToLower()).Count() > 0)
                                        {
                                            SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.MachineDescId = ppcMachineDescriptionList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 12].Value.ToString().Trim().ToLower()).FirstOrDefault().Key;
                                            SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.MachineDescription = ppcMachineDescriptionList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 12].Value.ToString().Trim().ToLower()).FirstOrDefault().Value;
                                        }
                                        else
                                        {
                                            SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.MachineDescId = null;
                                            SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.MachineDescription = ws.Cells[i, 12].Value.ToString().Trim();
                                        }
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.MachineDescId = null;
                                        SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.MachineDescription = "";
                                    }

                                    if (ws.Cells[i, 13].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 13].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 13].Value) > 0)
                                        SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.MachineSize = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 13].Value), 0);
                                    else
                                        SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.MachineSize = 0;

                                    dPrimaryProcessConversionTotal = 0;

                                    if (ws.Cells[i, 15].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 15].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 15].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.ManPlusMachineRatePerHour = Math.Round(Convert.ToDecimal(ws.Cells[i, 15].Value), 0);
                                        dPrimaryProcessConversionTotal = Math.Round(Convert.ToDecimal(ws.Cells[i, 15].Value), 3) / 3600;
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.ManPlusMachineRatePerHour = 0;
                                        dPrimaryProcessConversionTotal = 0;
                                    }

                                    if (ws.Cells[i, 14].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 14].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 14].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.CycleTime = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 14].Value), 3);
                                        dPrimaryProcessConversionTotal = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 14].Value), 0) * dPrimaryProcessConversionTotal;
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.CycleTime = 0;
                                    }

                                    SupplierQuoteDetails[counter].rFQdqPrimaryProcessConversion.ProcessConversionCostPerPart = Math.Round(dPrimaryProcessConversionTotal, 3);

                                    #endregion

                                    #region Machining

                                    if (ws.Cells[i, 17].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 17].Value.ToString().Trim()))
                                    {
                                        if (mMachiningDescriptionList.Where(rec => rec.Value.ToLower().Trim() == ws.Cells[i, 17].Value.ToString().Trim().ToLower()).Count() > 0)
                                        {
                                            SupplierQuoteDetails[counter].rFQdqMachining.MachiningDescId = mMachiningDescriptionList.Where(rec => rec.Value.ToLower().Trim() == ws.Cells[i, 17].Value.ToString().Trim().ToLower()).FirstOrDefault().Key;
                                            SupplierQuoteDetails[counter].rFQdqMachining.MachiningDescription = ws.Cells[i, 17].Value.ToString().Trim();
                                        }
                                        else if (mMachiningDescriptionList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 17].Value.ToString().Trim().ToLower()).Count() > 0)
                                        {
                                            SupplierQuoteDetails[counter].rFQdqMachining.MachiningDescId = mMachiningDescriptionList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 17].Value.ToString().Trim().ToLower()).FirstOrDefault().Key;
                                            SupplierQuoteDetails[counter].rFQdqMachining.MachiningDescription = mMachiningDescriptionList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 17].Value.ToString().Trim().ToLower()).FirstOrDefault().Value;
                                        }
                                        else
                                        {
                                            SupplierQuoteDetails[counter].rFQdqMachining.MachiningDescId = null;
                                            SupplierQuoteDetails[counter].rFQdqMachining.MachiningDescription = ws.Cells[i, 17].Value.ToString().Trim();
                                        }
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachining.MachiningDescId = null;
                                        SupplierQuoteDetails[counter].rFQdqMachining.MachiningDescription = "";
                                    }

                                    dMachiningTotal = 0;
                                    if (ws.Cells[i, 19].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 19].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 19].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachining.ManPlusMachineRatePerHour = Math.Round(Convert.ToDecimal(ws.Cells[i, 19].Value), 3);
                                        dMachiningTotal = Convert.ToDecimal(ws.Cells[i, 19].Value) / 3600;
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachining.ManPlusMachineRatePerHour = 0;
                                    }

                                    if (ws.Cells[i, 18].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 18].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 18].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachining.CycleTime = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 18].Value), 0);
                                        dMachiningTotal = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 18].Value), 0) * dMachiningTotal;
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachining.CycleTime = 0;
                                        dMachiningTotal = 0;
                                    }



                                    SupplierQuoteDetails[counter].rFQdqMachining.MachiningCostPerPart = Math.Round(dMachiningTotal, 3);

                                    #endregion

                                    #region Machining Secondary Operation

                                    if (ws.Cells[i, 21].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 21].Value.ToString().Trim()))
                                    {
                                        if (msoSecOprDescriptionList.Where(rec => rec.Value.ToLower().Trim() == ws.Cells[i, 21].Value.ToString().Trim().ToLower()).Count() > 0)
                                        {
                                            SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.SecondaryOperationDescId = msoSecOprDescriptionList.Where(rec => rec.Value.ToLower().Trim() == ws.Cells[i, 21].Value.ToString().Trim().ToLower()).FirstOrDefault().Key;
                                            SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.SecondaryOperationDescription = ws.Cells[i, 21].Value.ToString().Trim();
                                        }
                                        else if (msoSecOprDescriptionList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 21].Value.ToString().Trim().ToLower()).Count() > 0)
                                        {
                                            SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.SecondaryOperationDescId = msoSecOprDescriptionList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 21].Value.ToString().Trim().ToLower()).FirstOrDefault().Key;
                                            SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.SecondaryOperationDescription = msoSecOprDescriptionList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 21].Value.ToString().Trim().ToLower()).FirstOrDefault().Value;
                                        }
                                        else
                                        {
                                            SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.SecondaryOperationDescId = null;
                                            SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.SecondaryOperationDescription = ws.Cells[i, 21].Value.ToString().Trim();
                                        }
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.SecondaryOperationDescId = null;
                                        SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.SecondaryOperationDescription = "";
                                    }

                                    dMachiningSecondaryOperationTotal = 0;
                                    if (ws.Cells[i, 23].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 23].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 23].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour = Math.Round(Convert.ToDecimal(ws.Cells[i, 23].Value), 3);
                                        dMachiningSecondaryOperationTotal = Convert.ToDecimal(ws.Cells[i, 23].Value) / 3600;
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour = 0;
                                    }

                                    if (ws.Cells[i, 22].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 22].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 22].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.CycleTime = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 22].Value), 0);
                                        dMachiningSecondaryOperationTotal = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 22].Value), 0) * dMachiningSecondaryOperationTotal;
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.CycleTime = 0;
                                        dMachiningSecondaryOperationTotal = 0;
                                    }
                                    SupplierQuoteDetails[counter].rFQdqMachiningSecondaryOperation.SecondaryCostPerPart = Math.Round(dMachiningSecondaryOperationTotal, 3);

                                    #endregion

                                    #region Machining Other Operation

                                    if (ws.Cells[i, 25].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 25].Value.ToString().Trim()))
                                    {
                                        if (msoSecOprDescriptionList.Where(rec => rec.Value.ToLower().Trim() == ws.Cells[i, 25].Value.ToString().Trim().ToLower()).Count() > 0)
                                        {
                                            SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.SecondaryOperationDescId = msoSecOprDescriptionList.Where(rec => rec.Value.ToLower().Trim() == ws.Cells[i, 25].Value.ToString().Trim().ToLower()).FirstOrDefault().Key;
                                            SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.SecondaryOperationDescription = ws.Cells[i, 25].Value.ToString().Trim();
                                        }
                                        else if (msoSecOprDescriptionList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 25].Value.ToString().Trim().ToLower()).Count() > 0)
                                        {
                                            SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.SecondaryOperationDescId = msoSecOprDescriptionList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 25].Value.ToString().Trim().ToLower()).FirstOrDefault().Key;
                                            SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.SecondaryOperationDescription = msoSecOprDescriptionList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 25].Value.ToString().Trim().ToLower()).FirstOrDefault().Value;
                                        }
                                        else
                                        {
                                            SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.SecondaryOperationDescId = null;
                                            SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.SecondaryOperationDescription = ws.Cells[i, 25].Value.ToString().Trim();
                                        }
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.SecondaryOperationDescId = null;
                                        SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.SecondaryOperationDescription = "";
                                    }

                                    dMachiningOtherOperationTotal = 0;
                                    if (ws.Cells[i, 27].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 27].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 27].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour = Math.Round(Convert.ToDecimal(ws.Cells[i, 27].Value), 3);
                                        dMachiningOtherOperationTotal = Convert.ToDecimal(ws.Cells[i, 27].Value) / 3600;
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour = 0;
                                    }

                                    if (ws.Cells[i, 26].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 26].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 26].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.CycleTime = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 26].Value), 0);
                                        dMachiningOtherOperationTotal = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 26].Value), 0) * dMachiningOtherOperationTotal;
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.CycleTime = 0;
                                        dMachiningOtherOperationTotal = 0;
                                    }
                                    SupplierQuoteDetails[counter].rFQdqMachiningOtherOperation.SecondaryCostPerPart = Math.Round(dMachiningOtherOperationTotal, 3);

                                    #endregion

                                    #region Surface Treatment

                                    if (ws.Cells[i, 29].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 29].Value.ToString().Trim()))
                                    {
                                        if (stCoatingTypeList.Where(rec => rec.Value.ToLower().Trim() == ws.Cells[i, 29].Value.ToString().Trim().ToLower()).Count() > 0)
                                        {
                                            SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.CoatingTypeId = stCoatingTypeList.Where(rec => rec.Value.ToLower().Trim() == ws.Cells[i, 29].Value.ToString().Trim().ToLower()).FirstOrDefault().Key;
                                            SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.CoatingType = ws.Cells[i, 29].Value.ToString().Trim();
                                        }
                                        else if (stCoatingTypeList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 29].Value.ToString().Trim().ToLower()).Count() > 0)
                                        {
                                            SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.CoatingTypeId = stCoatingTypeList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 29].Value.ToString().Trim().ToLower()).FirstOrDefault().Key;
                                            SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.CoatingType = stCoatingTypeList.Where(rec => rec.Value.Replace(",", " ").ToLower().Trim() == ws.Cells[i, 29].Value.ToString().Trim().ToLower()).FirstOrDefault().Value;
                                        }
                                        else
                                        {
                                            SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.CoatingTypeId = null;
                                            SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.CoatingType = ws.Cells[i, 29].Value.ToString().Trim();
                                        }
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.CoatingTypeId = null;
                                        SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.CoatingType = "";
                                    }

                                    dSurfaceTreatmentTotal = 0;
                                    if (ws.Cells[i, 31].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 31].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 31].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.ManPlusMachineRatePerHour = Math.Round(Convert.ToDecimal(ws.Cells[i, 31].Value), 3);
                                        dSurfaceTreatmentTotal = Convert.ToDecimal(ws.Cells[i, 31].Value);
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.ManPlusMachineRatePerHour = 0;
                                    }

                                    if (ws.Cells[i, 30].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 30].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 30].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.PartsPerHour = (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 30].Value), 0);
                                        dSurfaceTreatmentTotal = dSurfaceTreatmentTotal / (int)Math.Round(Convert.ToDecimal(ws.Cells[i, 30].Value), 0);
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.PartsPerHour = 0;
                                        dSurfaceTreatmentTotal = 0;
                                    }
                                    SupplierQuoteDetails[counter].rFQdqSurfaceTreatment.CoatingCostPerHour = Math.Round(dSurfaceTreatmentTotal, 3);

                                    #endregion

                                    #region Overhead

                                    dOverheadTotal = 0;
                                    if (ws.Cells[i, 33].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 33].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 33].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqOverhead.InventoryCarryingCost = Math.Round(Convert.ToDecimal(ws.Cells[i, 33].Value), 3);
                                        dOverheadTotal = Convert.ToDecimal(ws.Cells[i, 33].Value);
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqOverhead.InventoryCarryingCost = 0;
                                    }

                                    if (ws.Cells[i, 34].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 34].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 34].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqOverhead.PackagingMaterial = Math.Round(Convert.ToDecimal(ws.Cells[i, 34].Value), 3);
                                        dOverheadTotal = dOverheadTotal + Convert.ToDecimal(ws.Cells[i, 34].Value);
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqOverhead.PackagingMaterial = 0;
                                    }

                                    if (ws.Cells[i, 35].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 35].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 35].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqOverhead.Packing = Math.Round(Convert.ToDecimal(ws.Cells[i, 35].Value), 3);
                                        dOverheadTotal = dOverheadTotal + Convert.ToDecimal(ws.Cells[i, 35].Value);
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqOverhead.Packing = 0;
                                    }

                                    if (ws.Cells[i, 36].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 36].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 36].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqOverhead.LocalFreightToPort = Math.Round(Convert.ToDecimal(ws.Cells[i, 36].Value), 3);
                                        dOverheadTotal = dOverheadTotal + Convert.ToDecimal(ws.Cells[i, 36].Value);
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqOverhead.LocalFreightToPort = 0;
                                    }

                                    if (ws.Cells[i, 37].Value != null && !string.IsNullOrEmpty(ws.Cells[i, 37].Value.ToString().Trim()) && Convert.ToDecimal(ws.Cells[i, 37].Value) > 0)
                                    {
                                        SupplierQuoteDetails[counter].rFQdqOverhead.ProfitAndSGA = Math.Round(Convert.ToDecimal(ws.Cells[i, 37].Value), 3);
                                        dOverheadTotal = dOverheadTotal + Convert.ToDecimal(ws.Cells[i, 37].Value);
                                    }
                                    else
                                    {
                                        SupplierQuoteDetails[counter].rFQdqOverhead.ProfitAndSGA = 0;
                                    }

                                    sum = dRawMaterialTotal + dPrimaryProcessConversionTotal + dMachiningTotal + dMachiningSecondaryOperationTotal + dMachiningOtherOperationTotal + dSurfaceTreatmentTotal + dOverheadTotal;
                                    SupplierQuoteDetails[counter].UnitPrice = Math.Round(sum, 3);

                                    if (sum > 0 && dOverheadTotal > 0)
                                        SupplierQuoteDetails[counter].rFQdqOverhead.OverheadPercentPiecePrice = Math.Round(dOverheadTotal * 100 / sum, 2);
                                    else
                                        SupplierQuoteDetails[counter].rFQdqOverhead.OverheadPercentPiecePrice = Math.Round((decimal)0, 2);

                                    #endregion

                                    SupplierQuoteDetails[counter].Remarks = remarks;
                                    varexchangeRate = exchangeRate;
                                    SupplierQuoteDetails[counter].ExchangeRate = varexchangeRate;
                                    SupplierQuoteDetails[counter].Currency = currency;
                                    SupplierQuoteDetails[counter].RawMaterialPriceAssumed = rawmaterial;

                                    counter++;
                                }

                                SupplierQuoteDetails[0].CreatedBy = supplierItem.CompanyName;
                                SupplierQuoteDetails[0].Remarks = remarks;
                                successMessage = Languages.GetResourceText("QuoteUploadSuccess");
                            }
                            else
                            {
                                errMsg = Languages.GetResourceText("UploadQuoteErrMsg2"); //"Please fill the mandatory data in the excel file and upload.";                              
                            }

                        }
                    }
                    else
                    {
                        errMsg = Languages.GetResourceText("UploadQuoteErrMsg3");// "The uploaded file is blank. Please upload a valid file.";
                    }
                }
                catch (Exception ex) //Error
                {
                    //"Error - Loading xlsx",
                    throw ex;
                }
                finally
                {
                    ws = null;
                    ef.ClosePreservedXlsx();
                }
            }
            catch (Exception ex) //Error
            {
                //"Error - LoadCSV",	

                throw ex;
            }

            return SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>>(errMsg, SupplierQuoteDetails, successMessage);
        }

        private int getExcelCellIntValue(object cellValue)
        {
            int dReturn = 0;
            if (cellValue != null)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(cellValue).Trim()))
                {
                    dReturn = (int)(Math.Round(Convert.ToDecimal(Convert.ToString(cellValue)), 0));
                }
            }

            return dReturn;
        }

        private decimal getExcelCellDecimalValue(object cellValue)
        {
            decimal dReturn = 0;
            if (cellValue != null)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(cellValue).Trim()))
                    dReturn = Convert.ToDecimal(Convert.ToString(cellValue).Trim());
            }

            return dReturn;
        }

        #endregion

        #region Download Quote File
        public ITypedResponse<bool?> DownloadRfqSupplierPartQuote(IPage<DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging)
        {
            string filePath = string.Empty;
            try
            {
                string errMSg = null;
                if (string.IsNullOrEmpty(paging.Criteria.UniqueUrl))
                {
                    List<DTO.Library.RFQ.RFQ.RFQParts> lstRFQParts = new List<DTO.Library.RFQ.RFQ.RFQParts>();

                    this.RunOnDB(dbcontext =>
                    {
                        var rsItem = dbcontext.rfqSuppliers.Where(c => c.RFQId == paging.Criteria.RFQId && c.SupplierId == paging.Criteria.SupplierId).FirstOrDefault();
                        if (rsItem == null)
                            errMSg = Languages.GetResourceText("RecordNotExist");
                        else
                        {
                            paging.Criteria.UniqueUrl = rsItem.OriginalURL;
                            paging.Criteria.IsQuoteTypeDQ = rsItem.IsQuoteTypeDQ.HasValue ? rsItem.IsQuoteTypeDQ.Value : false;
                        }
                    });
                    if (paging.Criteria.IsQuoteTypeDQ)
                        filePath = CreateDetailSupplierQuoteFile(paging.Criteria);
                    else
                        filePath = CreateSupplierQuoteFile(paging.Criteria);
                }
                else
                {
                    List<DTO.Library.RFQ.RFQ.RFQParts> lstRFQParts = new List<DTO.Library.RFQ.RFQ.RFQParts>();

                    this.RunOnDB(dbcontext =>
                    {
                        var rsItem = dbcontext.rfqSuppliers.Where(c => c.UniqueURL == paging.Criteria.UniqueUrl).FirstOrDefault();
                        if (rsItem == null)
                            errMSg = Languages.GetResourceText("RecordNotExist");
                        else
                        {
                            paging.Criteria.RFQId = rsItem.RFQId;
                            paging.Criteria.SupplierId = rsItem.SupplierId;
                            paging.Criteria.IsQuoteTypeDQ = rsItem.IsQuoteTypeDQ.HasValue ? rsItem.IsQuoteTypeDQ.Value : false;
                        }
                    });

                    if (paging.Criteria.IsQuoteTypeDQ)
                        filePath = CreateDetailSubmitQuoteFile(paging.Criteria);
                    else
                        filePath = CreateSubmitQuoteFile(paging.Criteria);
                }
            }
            catch (Exception ex)//Error
            {
                return FailedBoolResponse(ex.Message);
            }
            return SuccessBoolResponse(filePath);
        }

        public string CreateSubmitQuoteFile(DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria criteria)
        {
            List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> SupplierQuoteDetails = this.GetRFQSupplierPartQuoteListbyUniqueURL(criteria).Result;

            Supplier.Suppliers supplierObj = new Supplier.Suppliers();
            DTO.Library.RFQ.Supplier.Suppliers supplierItem = supplierObj.FindById(criteria.SupplierId).Result;

            string filePath = string.Empty;
            var context = HttpContext.Current;
            try
            {
                ExcelFile ef = new ExcelFile();
                ExcelFile myExcelFile = new ExcelFile();
                ExcelWorksheet excWsheet = myExcelFile.Worksheets.Add("Parts List");

                try
                {
                    // Frozen Columns (first column is frozen)
                    excWsheet.Panes = new WorksheetPanes(PanesState.Frozen, 1, 0, "B1", PanePosition.TopRight);

                    CellRange cr = excWsheet.Cells.GetSubrange("A1", "AB100");
                    cr.Merged = true; cr.Style.Locked = true;
                    cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A13", "T13"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    cr = excWsheet.Cells.GetSubrange("A20", "T20"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*Part Detail*/
                    cr = excWsheet.Cells.GetSubrange("A21", "E21"); cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.WrapText = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*Raw Material*/
                    cr = excWsheet.Cells.GetSubrange("F21", "L21"); cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.WrapText = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*OTHER*/
                    cr = excWsheet.Cells.GetSubrange("M21", "P21"); cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.WrapText = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*TOOLING*/
                    cr = excWsheet.Cells.GetSubrange("Q21", "T21"); cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.WrapText = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    cr = excWsheet.Cells.GetSubrange("A8", "B8"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A9", "B9"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top; cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A10", "B10"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top; cr.Merged = false; cr = null;
                    
                    cr = excWsheet.Cells.GetSubrange("A11", "B11"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top; cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A15", "B15"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A16", "B16"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A17", "B17"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.WrapText = true; cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A18", "B18"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.WrapText = true; cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A22", "T22"); cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.WrapText = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond); cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A5", "F5"); cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight; cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A6", "F6"); cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight; cr.Merged = false; cr = null;

                    excWsheet.Cells[7, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[7, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    excWsheet.Cells[7, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[7, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;

                    excWsheet.Cells[8, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[8, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    excWsheet.Cells[8, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[8, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;

                    excWsheet.Cells[9, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[9, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    excWsheet.Cells[9, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[9, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;

                    excWsheet.Cells[10, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[10, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    excWsheet.Cells[10, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[10, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;

                    excWsheet.Cells[12, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[12, 0].Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    excWsheet.Cells[12, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                    excWsheet.Cells[14, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[14, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[14, 1].Style.NumberFormat = "0.000";
                    excWsheet.Cells[14, 1].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);
                    excWsheet.Cells[14, 2].Style.Font.Color = System.Drawing.Color.Red;

                    excWsheet.Cells[15, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[15, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[15, 1].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);
                    excWsheet.Cells[15, 2].Style.Font.Color = System.Drawing.Color.Red;

                    excWsheet.Cells[16, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[16, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[16, 1].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);
                    excWsheet.Cells[16, 2].Style.Font.Color = System.Drawing.Color.Red;

                    excWsheet.Cells[17, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[17, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[17, 1].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);

                    excWsheet.Cells[19, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[19, 0].Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    excWsheet.Cells[19, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                    excWsheet.Columns[0].Width = 35 * 256;

                    int counter = 22;

                    if (SupplierQuoteDetails.Count > 0)
                    {
                        foreach (var supplierDetailQuotePart in SupplierQuoteDetails)
                        {
                            excWsheet.Cells[counter, 20].Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                            excWsheet.Cells[counter, 20].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 20].Style.Font.Color = Color.Red;
                            excWsheet.Cells[counter, 20].Style.WrapText = true;

                            cr = excWsheet.Cells.GetSubrange("A" + (counter + 1), "T" + (counter + 1)); cr.Merged = true;
                            cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                            cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, Color.Black, LineStyle.Thin);
                            cr.Merged = false;
                            cr = null; //Vertical Allignment Style set to Top And Border"

                            #region Back Color Yellow and Unlocked
                            cr = excWsheet.Cells.GetSubrange("E" + (counter + 1), "E" + (counter + 1)); cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.Yellow, Color.Yellow);
                            cr.Style.Locked = false; cr.Merged = false;
                            cr = null; //Unlock the editable cells

                            cr = excWsheet.Cells.GetSubrange("G" + (counter + 1), "H" + (counter + 1)); cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.Yellow, Color.Yellow);
                            cr.Style.Locked = false; cr.Merged = false;
                            cr = null; //Unlock the editable cells

                            cr = excWsheet.Cells.GetSubrange("J" + (counter + 1), "J" + (counter + 1)); cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.Yellow, Color.Yellow);
                            cr.Style.Locked = false; cr.Merged = false;
                            cr = null; //Unlock the editable cells
                            cr = excWsheet.Cells.GetSubrange("M" + (counter + 1), "O" + (counter + 1)); cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.Yellow, Color.Yellow);
                            cr.Style.Locked = false; cr.Merged = false;
                            cr = null; //Unlock the editable cells

                            cr = excWsheet.Cells.GetSubrange("Q" + (counter + 1), "T" + (counter + 1)); cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.Yellow, Color.Yellow);
                            cr.Style.Locked = false; cr.Merged = false;
                            cr = null; //Unlock the editable cells
                            #endregion

                            #region Cell Format

                            excWsheet.Cells[counter, 7].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 7].Value = 0.000;
                            excWsheet.Cells[counter, 8].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 8].Value = 0.000;
                            excWsheet.Cells[counter, 9].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 9].Value = 0.000;
                            excWsheet.Cells[counter, 10].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 10].Value = 0.000;
                            excWsheet.Cells[counter, 11].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 11].Value = 0.000;

                            excWsheet.Cells[counter, 12].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 12].Value = 0.000;
                            excWsheet.Cells[counter, 13].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 13].Value = 0.000;
                            excWsheet.Cells[counter, 14].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 14].Value = 0.000;
                            excWsheet.Cells[counter, 15].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 15].Value = 0.000;
                            excWsheet.Cells[counter, 16].Style.NumberFormat = "0";
                            excWsheet.Cells[counter, 16].Value = 0;
                            excWsheet.Cells[counter, 17].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 17].Value = 0.000;
                            #endregion

                            #region Cell Alignment
                            //PART
                            excWsheet.Cells[counter, 3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 4].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            //RAW MATERIAL
                            excWsheet.Cells[counter, 5].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 6].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 7].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 8].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 9].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 10].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            //OTHER
                            excWsheet.Cells[counter, 11].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 12].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 13].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 14].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 15].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            //TOOLING
                            excWsheet.Cells[counter, 16].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 17].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 18].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            #endregion

                            excWsheet.Cells[counter, 0].Style.WrapText = true;
                            excWsheet.Cells[counter, 1].Style.WrapText = true;
                            excWsheet.Cells[counter, 2].Style.WrapText = true;
                            excWsheet.Cells[counter, 6].Style.WrapText = true;
                            excWsheet.Cells[counter, 18].Style.WrapText = true;
                            excWsheet.Cells[counter, 19].Style.WrapText = true;
                            excWsheet.Cells[counter, 20].Style.WrapText = true;

                            excWsheet.Cells[counter, 0].Value = supplierDetailQuotePart.CustomerPartNo;
                            excWsheet.Cells[counter, 1].Value = supplierDetailQuotePart.PartDescription;
                            excWsheet.Cells[counter, 2].Value = supplierDetailQuotePart.AdditionalPartDesc;
                            excWsheet.Cells[counter, 3].Value = supplierDetailQuotePart.EstimatedQty;
                            excWsheet.Cells[counter, 5].Value = supplierDetailQuotePart.MaterialType;
                            excWsheet.Cells[counter, 8].Value = supplierDetailQuotePart.PartWeightKG.HasValue ? supplierDetailQuotePart.PartWeightKG.Value.ToString("0.000") : string.Empty;

                            /*I. Example: Input = 1kg  Raw Mat’l Cost=2.15/kg
                                       1x2.15= $2.15.  Loss would be calculation of Input wt. – RFQ wt./Input Wt.*/
                            excWsheet.Cells[counter, 10].Formula = "=IF(H" + (counter + 1) + ">0,IFERROR((H" + (counter + 1) + "-I" + (counter + 1) + ")/H" + (counter + 1) + ",1),0)";

                            /*II. Raw Mat'l TOTAL
                            * [Raw Mat'l TOTAL] = [Raw Mat'l Input (Gross wt in kg] * [Raw Mat'l Cost (per kg)]
                            * =(PRODUCT(H21,J21))
                            */
                            string rawMatFormula = "=ROUND(PRODUCT(H" + (counter + 1) + ", J" + (counter + 1) + "),3)";
                            excWsheet.Cells[counter, 11].Formula = rawMatFormula;

                            excWsheet.Cells[counter, 15].Formula = "=SUM(L" + (counter + 1) + ":O" + (counter + 1) + ")";

                            string formulaValidation = "=IF(";
                            formulaValidation += "AND(";
                            /*
                             * E - MIN. ORDER QTY          ----- E
                             * H - TOOLING COST            ----- R
                             * K - MATERIAL COST           ----- L
                             * L - CONVERSION COST         ----- M
                             * M - MACHINING COST          ----- N
                             * N - OTHER PROCESS COST      ----- O
                             * O - NO. OF CAVITIES         ----- Q
                             *   - RAW MAT'L INPUT         ----- H
                             *   - RAW MAT'L COST          ----- J                             
                             * G - PART WEIGHT             ----- I
                             * I - STL                     ----- T
                             */
                            formulaValidation += "OR(ISBLANK(E" + (counter + 1) + "), (E" + (counter + 1) + "=0)), OR(ISBLANK(Q" + (counter + 1) + "), (Q" + (counter + 1) + "=0)), OR(ISBLANK(R" + (counter + 1) + "), (R" + (counter + 1) + "=0))";
                            formulaValidation += ", OR(ISBLANK(G" + (counter + 1) + "), (G" + (counter + 1) + "=0)), OR(ISBLANK(H" + (counter + 1) + "), (H" + (counter + 1) + "=0)), OR(ISBLANK(J" + (counter + 1) + "), (J" + (counter + 1) + "=0))";
                            formulaValidation += ", OR(ISBLANK(M" + (counter + 1) + "), (M" + (counter + 1) + "=0)), OR(ISBLANK(N" + (counter + 1) + "), (N" + (counter + 1) + "=0)), OR(ISBLANK(O" + (counter + 1) + "), (O" + (counter + 1) + "=0))";
                            formulaValidation += "), \"\" ";

                            formulaValidation += ", IF(";
                            formulaValidation += "OR(";
                            formulaValidation += "ISTEXT(E" + (counter + 1) + "), ISTEXT(Q" + (counter + 1) + "), ISTEXT(R" + (counter + 1) + ")";
                            formulaValidation += ", ISTEXT(H" + (counter + 1) + "), ISTEXT(J" + (counter + 1) + ")";
                            formulaValidation += ", ISTEXT(M" + (counter + 1) + "), ISTEXT(N" + (counter + 1) + "), ISTEXT(O" + (counter + 1) + ")";
                            formulaValidation += "), \"Please enter numeric values.\" ";

                            formulaValidation += ", IF(";
                            formulaValidation += "OR(";
                            formulaValidation += "ISBLANK(E" + (counter + 1) + "), ISBLANK(Q" + (counter + 1) + "), (Q" + (counter + 1) + "=0)";
                            formulaValidation += ", ISBLANK(H" + (counter + 1) + "), (H" + (counter + 1) + "=0), ISBLANK(J" + (counter + 1) + "), (J" + (counter + 1) + "=0)";
                            formulaValidation += ", ISBLANK(M" + (counter + 1) + "), (M" + (counter + 1) + "=0)";
                            formulaValidation += "), \"Values for Min. Order Qty, Raw Mat'l Input, Raw Mat'l Cost, Conversion Cost & No. Of Cavities are mandatory.\" ";

                            formulaValidation += ", \"\" )";
                            formulaValidation += ")";
                            formulaValidation += ")";

                            excWsheet.Cells[counter, 20].Formula = formulaValidation;

                            counter++;
                        }
                    }

                    //** start ** Unprotect the sheet and lock all the cells
                    //Unlock the editable cells
             
                    excWsheet.Cells[13, 1].Style.Locked = false;
                    excWsheet.Cells[14, 1].Style.Locked = false;
                    excWsheet.Cells[15, 1].Style.Locked = false;
                    excWsheet.Cells[16, 1].Style.Locked = false;
                    excWsheet.Cells[17, 1].Style.Locked = false;
                    //** end **

                    excWsheet.Cells[4, 0].Value = "P O BOX 401";
                    excWsheet.Cells[5, 0].Value = "Lewis Center, OH 43035";
                    excWsheet.Cells[7, 0].Value = "VENDOR NAME:";
                    excWsheet.Cells[7, 1].Value = supplierItem.CompanyName.ToUpper();

                    excWsheet.Cells[8, 0].Value = "RFQ # ";
                    excWsheet.Cells[8, 1].Value = criteria.RFQId;

                    RFQ rfqObj = new RFQ();
                    DTO.Library.RFQ.RFQ.RFQ rfqInfo = rfqObj.FindById(criteria.RFQId).Result;

                    excWsheet.Cells[9, 0].Value = "Supplier Requirement";
                    excWsheet.Cells[9, 1].Value = rfqInfo.SupplierRequirement;
                    
                    excWsheet.Cells[10, 0].Value = "Remarks";
                    excWsheet.Cells[10, 1].Value = rfqInfo.Remarks;

                    excWsheet.Cells[12, 0].Value = "Request For Quote (RFQ)";

                    excWsheet.Cells[14, 0].Value = "Exchange Rate 1 USD =";
                    excWsheet.Cells[14, 2].Formula = "=IF(ISBLANK(B15),\"Value for Exchange Rate is mandatory.\",IF(ISTEXT(B15),\"Please enter numeric values.\",\"\"))";
                    excWsheet.Cells[14, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[15, 0].Value = "Currency =";
                    excWsheet.Cells[15, 2].Formula = "=IF(ISBLANK(B16),\"Value for Currency is mandatory.\",\"\")";
                    excWsheet.Cells[15, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;

                    excWsheet.Cells[16, 0].Value = "Raw Material Price Assumed \n(Please include unit of measure…i.e. $1.000/kg) =";
                    excWsheet.Cells[16, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;

                    excWsheet.Cells[17, 0].Value = "Supplier Remarks";

                    excWsheet.Cells[19, 0].Value = "Part(s) List";
                    excWsheet.Cells[20, 0].Value = "Part Detail";
                    excWsheet.Cells[20, 5].Value = "Raw Material";
                    excWsheet.Cells[20, 12].Value = "Other";
                    excWsheet.Cells[20, 16].Value = "Tooling";

                    excWsheet.Cells[21, 0].Value = "Part No.";
                    excWsheet.Cells[21, 1].Value = "Description";
                    excWsheet.Cells[21, 2].Value = "Additional Description";
                    excWsheet.Cells[21, 3].Value = "Annual Quantity";
                    excWsheet.Cells[21, 4].Value = "Min. Order Qty";

                    excWsheet.Cells[21, 5].Value = "Raw Material Description";
                    excWsheet.Cells[21, 6].Value = "Raw Material Index Used";
                    excWsheet.Cells[21, 7].Value = "Raw Mat'l Input (Gross wt in kg)";
                    excWsheet.Cells[21, 8].Value = "Part Weight (kg)";
                    excWsheet.Cells[21, 9].Value = "Raw Mat'l Cost (USD)(per kg)";
                    excWsheet.Cells[21, 10].Value = "Material Loss";
                    excWsheet.Cells[21, 11].Value = "Raw Mat'l TOTAL";

                    excWsheet.Cells[21, 12].Value = "Conversion Cost (USD)";
                    excWsheet.Cells[21, 13].Value = "Machining Cost (USD)";
                    excWsheet.Cells[21, 14].Value = "Other Process Cost (USD)";
                    excWsheet.Cells[21, 15].Value = "Final Piece Price / Part";

                    excWsheet.Cells[21, 16].Value = "No. Of Cavities";
                    excWsheet.Cells[21, 17].Value = "Tooling Cost";
                    excWsheet.Cells[21, 18].Value = "Tooling Warranty";
                    excWsheet.Cells[21, 19].Value = "Supplier Tooling Leadtime";

                    excWsheet.Columns[1].AutoFit();
                    excWsheet.Columns[2].AutoFit();
                    excWsheet.Columns[3].Width = 20 * 10 * 25;
                    excWsheet.Columns[4].Width = 20 * 10 * 20;
                    excWsheet.Columns[5].Width = 20 * 10 * 30;
                    excWsheet.Columns[6].Width = 20 * 10 * 30;
                    excWsheet.Columns[7].Width = 20 * 10 * 30;
                    excWsheet.Columns[8].Width = 20 * 10 * 30;
                    excWsheet.Columns[9].Width = 20 * 10 * 30;
                    excWsheet.Columns[10].Width = 20 * 10 * 30;
                    excWsheet.Columns[11].Width = 20 * 10 * 30;
                    excWsheet.Columns[12].Width = 20 * 10 * 30;
                    excWsheet.Columns[13].Width = 20 * 10 * 30;
                    excWsheet.Columns[14].Width = 20 * 10 * 30;
                    excWsheet.Columns[15].Width = 20 * 10 * 30;
                    excWsheet.Columns[16].Width = 20 * 10 * 30;
                    excWsheet.Columns[17].Width = 20 * 10 * 30;
                    excWsheet.Columns[18].Width = 20 * 10 * 30;
                    excWsheet.Columns[19].Width = 20 * 10 * 30;
                    excWsheet.Columns[20].Width = 20 * 30 * 40;

                    //MES Logo
                    cr = excWsheet.Cells.GetSubrange("A1", "T4"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.White, System.Drawing.Color.White);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;
                    excWsheet.Pictures.Add(context.Server.MapPath(Constants.IMAGEFOLDER) + "MESlogoPdf.png", GemBox.Spreadsheet.PositioningMode.FreeFloating, new AnchorCell(excWsheet.Columns[0], excWsheet.Rows[0], true), new AnchorCell(excWsheet.Columns[0], excWsheet.Rows[3], false));

                    //ends here

                    excWsheet.ProtectionSettings.PasswordHash = 1;
                    excWsheet.Protected = true;

                    string generatedFileName = supplierItem.CompanyName + "_" + criteria.RFQId + "_" + System.DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx";
                    string tempFilepath = context.Server.MapPath("~") + Constants.REPORTSTEMPLATEFILEPATH + generatedFileName;
                    if (!System.IO.Directory.Exists(context.Server.MapPath(@"~") + (Constants.REPORTSTEMPLATEFILEPATH)))
                        System.IO.Directory.CreateDirectory(context.Server.MapPath(@"~") + (Constants.REPORTSTEMPLATEFILEPATH));
                    else
                    {
                        if (File.Exists(tempFilepath))
                            File.Delete(tempFilepath);
                    }

                    myExcelFile.SaveXlsx(tempFilepath);

                    filePath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"])
                         + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                         + Constants.TEMPFILESEMAILATTACHMENTSFOLDER
                         + generatedFileName;

                    Helper.BlobHelper.DeleteBlobfile(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), filePath);

                    Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                                  , Constants.TEMPFILESEMAILATTACHMENTSFOLDERName
                                  , generatedFileName
                                  , tempFilepath);
                    File.Delete(tempFilepath);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    excWsheet = null;
                    ef.ClosePreservedXlsx();
                }
            }
            catch (Exception ex)//Error
            {
                throw ex;
            }
            return filePath;
        }

        public string CreateDetailSubmitQuoteFile(DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria criteria)
        {
            List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> SupplierQuoteDetails = this.GetRFQSupplierPartQuoteListbyUniqueURL(criteria).Result;
            Supplier.Suppliers supplierObj = new Supplier.Suppliers();
            DTO.Library.RFQ.Supplier.Suppliers supplierItem = supplierObj.FindById(criteria.SupplierId).Result;

            string filePath = string.Empty;
            var context = HttpContext.Current;
            try
            {
                ExcelFile myExcelFile = new ExcelFile();
                ExcelWorksheet excWsheet = myExcelFile.Worksheets.Add("Parts List");

                try
                {
                    // Frozen Columns (first column is frozen)
                    excWsheet.Panes = new WorksheetPanes(PanesState.Frozen, 1, 0, "B1", PanePosition.TopRight);

                    CellRange cr = excWsheet.Cells.GetSubrange("A1", "AQ100");
                    cr.Merged = true; cr.Style.Locked = true;
                    cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A13", "AR13"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    cr = excWsheet.Cells.GetSubrange("A20", "AR20"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*Part Detail*/
                    cr = excWsheet.Cells.GetSubrange("A21", "E21"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*Raw Material*/
                    cr = excWsheet.Cells.GetSubrange("F21", "L21"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*Primary Process/Conversion*/
                    cr = excWsheet.Cells.GetSubrange("M21", "Q21"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*Machining*/
                    cr = excWsheet.Cells.GetSubrange("R21", "U21"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*Machining 2/Secondary Operation*/
                    cr = excWsheet.Cells.GetSubrange("V21", "Y21"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*Machining 3/Other Operation*/
                    cr = excWsheet.Cells.GetSubrange("Z21", "AC21"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*Surface Treatment*/
                    cr = excWsheet.Cells.GetSubrange("AD21", "AG21"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*Overhead*/
                    cr = excWsheet.Cells.GetSubrange("AH21", "AN21"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*Tooling*/
                    cr = excWsheet.Cells.GetSubrange("AO21", "AR21"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    cr = excWsheet.Cells.GetSubrange("A8", "B8"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A9", "B9"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top; cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A10", "B10"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top; cr.Merged = false; cr = null;
                    
                    cr = excWsheet.Cells.GetSubrange("A11", "B11"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top; cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A15", "B15"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A16", "B16"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A17", "B17"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.WrapText = true; cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A18", "B18"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.WrapText = true; cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A22", "AR22"); cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.WrapText = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond); cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A5", "F5"); cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight; cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A6", "F6"); cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight; cr.Merged = false; cr = null;

                    excWsheet.Cells[7, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[7, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    excWsheet.Cells[7, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[7, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;

                    excWsheet.Cells[8, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[8, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    excWsheet.Cells[8, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[8, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;

                    excWsheet.Cells[9, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[9, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    excWsheet.Cells[9, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[9, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;

                    excWsheet.Cells[10, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[10, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    excWsheet.Cells[10, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[10, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    
                    excWsheet.Cells[12, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[12, 0].Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    excWsheet.Cells[12, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                    excWsheet.Cells[14, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[14, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[14, 1].Style.NumberFormat = "0.000";
                    excWsheet.Cells[14, 1].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);
                    excWsheet.Cells[14, 2].Style.Font.Color = System.Drawing.Color.Red;

                    excWsheet.Cells[15, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[15, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[15, 1].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);
                    excWsheet.Cells[15, 2].Style.Font.Color = System.Drawing.Color.Red;

                    excWsheet.Cells[16, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[16, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[16, 1].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);
                    excWsheet.Cells[16, 2].Style.Font.Color = System.Drawing.Color.Red;

                    excWsheet.Cells[17, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[17, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[17, 1].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);

                    excWsheet.Cells[19, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[19, 0].Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    excWsheet.Cells[19, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                    excWsheet.Columns[0].Width = 35 * 256;

                    int counter = 22;

                    List<string> ppcMachineDescriptionList = this.DataContext.MachineDescs.Select(rec => rec.MachineDescription.Replace(",", " ")).ToList();
                    List<string> mMachiningDescriptionList = this.DataContext.MachiningDescs.Select(rec => rec.MachiningDescription.Replace(",", " ")).ToList();
                    List<string> msoSecOprDescriptionList = this.DataContext.SecondaryOperationDescs.Select(rec => rec.SecondaryOperationDescription.Replace(",", " ")).ToList();
                    List<string> stCoatingTypeList = this.DataContext.CoatingTypes.Select(rec => rec.CoatingType1.Replace(",", " ")).ToList();

                    if (SupplierQuoteDetails.Count > 0)
                    {
                        foreach (var supplierDetailQuotePart in SupplierQuoteDetails)
                        {
                            /*Values for 
                              Tooling Cost
                             ,No. Of Cavities
                             ,Raw Mat'l Input, Raw Material Cost Per Kg
                             ,(Primary Process/Conversion)
                             ,Machine Description,
                             ,Machine Size, 
                             ,Cycle Time, 
                             ,Man + Machine Rate Per Hour 
                             & Process / Conversion Cost / Part
                             are mandatory.*/

                            excWsheet.Cells[counter, 44].Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                            excWsheet.Cells[counter, 44].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 44].Style.Font.Color = Color.Red;
                            excWsheet.Cells[counter, 44].Style.WrapText = true;
                            excWsheet.Rows[counter].AutoFit();

                            cr = excWsheet.Cells.GetSubrange("A" + (counter + 1), "AR" + (counter + 1));  //Vertical Allignment Style set to Top And Borders"
                            cr.Merged = true;
                            cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                            cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, Color.Black, LineStyle.Thin);
                            cr.Merged = false;
                            cr = null;

                            #region Back Color Yellow and Unlocked

                            cr = excWsheet.Cells.GetSubrange("E" + (counter + 1), "E" + (counter + 1));
                            cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);
                            cr.Style.Locked = false;
                            cr.Merged = false;
                            cr = null;

                            cr = excWsheet.Cells.GetSubrange("G" + (counter + 1), "H" + (counter + 1));
                            cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);
                            cr.Style.Locked = false;
                            cr.Merged = false;
                            cr = null;

                            cr = excWsheet.Cells.GetSubrange("J" + (counter + 1), "J" + (counter + 1));
                            cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);
                            cr.Style.Locked = false;
                            cr.Merged = false;
                            cr = null;

                            cr = excWsheet.Cells.GetSubrange("M" + (counter + 1), "P" + (counter + 1));
                            cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);
                            cr.Style.Locked = false;
                            cr.Merged = false;
                            cr = null;

                            cr = excWsheet.Cells.GetSubrange("R" + (counter + 1), "T" + (counter + 1));
                            cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);
                            cr.Style.Locked = false;
                            cr.Merged = false;
                            cr = null;

                            cr = excWsheet.Cells.GetSubrange("V" + (counter + 1), "X" + (counter + 1));
                            cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);
                            cr.Style.Locked = false;
                            cr.Merged = false;
                            cr = null;

                            cr = excWsheet.Cells.GetSubrange("Z" + (counter + 1), "AB" + (counter + 1));
                            cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);
                            cr.Style.Locked = false;
                            cr.Merged = false;
                            cr = null;

                            cr = excWsheet.Cells.GetSubrange("AD" + (counter + 1), "AF" + (counter + 1));
                            cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);
                            cr.Style.Locked = false;
                            cr.Merged = false;
                            cr = null;
                            
                            cr = excWsheet.Cells.GetSubrange("AH" + (counter + 1), "AL" + (counter + 1));
                            cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);
                            cr.Style.Locked = false;
                            cr.Merged = false;
                            cr = null;

                            cr = excWsheet.Cells.GetSubrange("AO" + (counter + 1), "AR" + (counter + 1));
                            cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);
                            cr.Style.Locked = false;
                            cr.Merged = false;
                            cr = null;

                            #endregion

                            #region Cell Format

                            excWsheet.Cells[counter, 41].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 41].Value = 0.000;
                            excWsheet.Cells[counter, 40].Style.NumberFormat = "0";
                            excWsheet.Cells[counter, 40].Value = 0;

                            excWsheet.Cells[counter, 7].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 7].Value = 0.000;
                            excWsheet.Cells[counter, 8].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 8].Value = 0.000;
                            excWsheet.Cells[counter, 9].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 9].Value = 0.000;
                            excWsheet.Cells[counter, 10].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 10].Value = 0.000;
                            excWsheet.Cells[counter, 11].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 11].Value = 0.00;

                            excWsheet.Cells[counter, 13].Style.NumberFormat = "0";
                            excWsheet.Cells[counter, 13].Value = 0;
                            excWsheet.Cells[counter, 14].Style.NumberFormat = "0";
                            excWsheet.Cells[counter, 14].Value = 0;
                            excWsheet.Cells[counter, 15].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 15].Value = 0.000;
                            excWsheet.Cells[counter, 16].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 16].Value = 0.000;

                            excWsheet.Cells[counter, 18].Style.NumberFormat = "0";
                            excWsheet.Cells[counter, 18].Value = 0;
                            excWsheet.Cells[counter, 19].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 19].Value = 0.000;
                            excWsheet.Cells[counter, 20].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 20].Value = 0.000;

                            excWsheet.Cells[counter, 22].Style.NumberFormat = "0";
                            excWsheet.Cells[counter, 22].Value = 0;
                            excWsheet.Cells[counter, 23].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 23].Value = 0.000;
                            excWsheet.Cells[counter, 24].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 24].Value = 0.000;

                            excWsheet.Cells[counter, 26].Style.NumberFormat = "0";
                            excWsheet.Cells[counter, 26].Value = 0;
                            excWsheet.Cells[counter, 27].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 27].Value = 0.000;
                            excWsheet.Cells[counter, 28].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 28].Value = 0.000;

                            excWsheet.Cells[counter, 30].Style.NumberFormat = "0";
                            excWsheet.Cells[counter, 30].Value = 0;
                            excWsheet.Cells[counter, 31].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 31].Value = 0.000;
                            excWsheet.Cells[counter, 32].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 32].Value = 0.000;

                            excWsheet.Cells[counter, 33].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 33].Value = 0.000;
                            excWsheet.Cells[counter, 34].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 34].Value = 0.000;
                            excWsheet.Cells[counter, 35].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 35].Value = 0.000;
                            excWsheet.Cells[counter, 36].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 36].Value = 0.000;
                            excWsheet.Cells[counter, 37].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 37].Value = 0.000;
                            excWsheet.Cells[counter, 38].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 38].Value = 0.000;
                            excWsheet.Cells[counter, 39].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 39].Value = 0.000;


                            #endregion

                            #region Cell Alignment

                            excWsheet.Cells[counter, 3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 4].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                            excWsheet.Cells[counter, 5].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 6].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 7].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 8].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 9].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 10].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 11].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                            excWsheet.Cells[counter, 12].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 13].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 14].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 15].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                            excWsheet.Cells[counter, 16].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 17].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 18].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 19].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                            excWsheet.Cells[counter, 20].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 21].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 22].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 23].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                            excWsheet.Cells[counter, 24].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 25].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 26].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 27].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                            excWsheet.Cells[counter, 28].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 29].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 30].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 31].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                            excWsheet.Cells[counter, 32].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 33].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 34].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 35].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                            excWsheet.Cells[counter, 36].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 37].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 38].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 39].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 40].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 41].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 42].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                            #endregion

                            excWsheet.Cells[counter, 0].Style.WrapText = true;
                            excWsheet.Cells[counter, 1].Style.WrapText = true;
                            excWsheet.Cells[counter, 2].Style.WrapText = true;
                            excWsheet.Cells[counter, 42].Style.WrapText = true;
                            excWsheet.Cells[counter, 43].Style.WrapText = true;

                            excWsheet.Cells[counter, 0].Value = supplierDetailQuotePart.CustomerPartNo;
                            excWsheet.Cells[counter, 1].Value = supplierDetailQuotePart.PartDescription;
                            excWsheet.Cells[counter, 2].Value = supplierDetailQuotePart.AdditionalPartDesc;
                            excWsheet.Cells[counter, 3].Value = supplierDetailQuotePart.EstimatedQty;
                            excWsheet.Cells[counter, 5].Value = supplierDetailQuotePart.MaterialType;
                            excWsheet.Cells[counter, 8].Value = supplierDetailQuotePart.PartWeightKG.HasValue ? supplierDetailQuotePart.PartWeightKG.Value.ToString("0.000") : string.Empty;

                            #region Drop Down Set

                            DataValidation dv;
                            /*Machine Description*/
                            cr = excWsheet.Cells.GetSubrange("M" + (counter + 1), "M" + (counter + 1));
                            cr.Merged = true;
                            dv = new DataValidation(cr);
                            dv.Type = DataValidationType.List;
                            dv.InputMessage = "Select Machine Description";
                            dv.Formula1 = ppcMachineDescriptionList;
                            dv.ShowErrorAlert = false;
                            excWsheet.DataValidations.Add(dv);
                            cr.Merged = false;
                            cr = null;
                            /*end here*/
                            /*Machining Description*/
                            cr = excWsheet.Cells.GetSubrange("R" + (counter + 1), "R" + (counter + 1));
                            cr.Merged = true;
                            dv = new DataValidation(cr);
                            dv.Type = DataValidationType.List;
                            dv.InputMessage = "Select Machining Description";
                            dv.Formula1 = mMachiningDescriptionList;
                            dv.ShowErrorAlert = false;
                            excWsheet.DataValidations.Add(dv);
                            cr.Merged = false;
                            cr = null;
                            /*end here*/
                            /*Secondary Operation*/
                            cr = excWsheet.Cells.GetSubrange("V" + (counter + 1), "V" + (counter + 1));
                            cr.Merged = true;
                            dv = new DataValidation(cr);
                            dv.Type = DataValidationType.List;
                            dv.InputMessage = "Select Secondary Operation Description";
                            dv.Formula1 = msoSecOprDescriptionList;
                            dv.ShowErrorAlert = false;
                            excWsheet.DataValidations.Add(dv);
                            cr.Merged = false;
                            cr = null;
                            /*end here*/
                            /*Secondary Operation Description*/
                            cr = excWsheet.Cells.GetSubrange("Z" + (counter + 1), "Z" + (counter + 1));
                            cr.Merged = true;
                            dv = new DataValidation(cr);
                            dv.Type = DataValidationType.List;
                            dv.InputMessage = "Select Secondary Operation Description";
                            dv.Formula1 = msoSecOprDescriptionList;
                            dv.ShowErrorAlert = false;
                            excWsheet.DataValidations.Add(dv);
                            cr.Merged = false;
                            cr = null;
                            /*end here*/
                            /*Coating Type*/
                            cr = excWsheet.Cells.GetSubrange("AD" + (counter + 1), "AD" + (counter + 1));
                            cr.Merged = true;
                            dv = new DataValidation(cr);
                            dv.Type = DataValidationType.List;
                            dv.InputMessage = "Select Coating Type";
                            dv.Formula1 = stCoatingTypeList;
                            dv.ShowErrorAlert = false;
                            excWsheet.DataValidations.Add(dv);
                            cr.Merged = false;
                            cr = null;
                            /*end here*/
                            #endregion

                            /*I.A Raw Mat'l TOTAL
                            * [Raw Mat'l TOTAL] = [Raw Mat'l Input (Gross wt in kg] * [Raw Mat'l Cost (per kg)]
                            * =(PRODUCT(H21,J21))
                            */
                            /*I.B Example: Input = 1kg  Raw Mat’l Cost=2.15/kg
                                     1x2.15= $2.15.  Loss would be calculation of Input wt. – RFQ wt./Input Wt.
                            */
                            excWsheet.Cells[counter, 10].Formula = "=IF(H" + (counter + 1) + ">0,IFERROR((H" + (counter + 1) + "-I" + (counter + 1) + ")/H" + (counter + 1) + ",1),0)";
                            string rawMatFormula = "=ROUND(PRODUCT(H" + (counter + 1) + ", J" + (counter + 1) + "),3)";
                            excWsheet.Cells[counter, 11].Formula = rawMatFormula;

                            /*II. Process / Conversion Cost / Part
                             * [Process / Conversion Cost / Part] =[Cycle Time (sec)] * ([Man + Machine Rate/Hour] / 3600)
                             * =(PRODUCT(O21,(P21/3600))) 
                             */
                            excWsheet.Cells[counter, 16].Formula = "=ROUND((PRODUCT(O" + (counter + 1) + ",(P" + (counter + 1) + "/3600))), 3)";

                            /*III. Machining Cost / Part
                             * [Machining Cost / Part]=[Cycle Time (sec) ]* ([Man + Machine Rate/Hour] / 3600)
                             * =(PRODUCT(S21,(T21/3600))) 
                             */
                            excWsheet.Cells[counter, 20].Formula = "=ROUND((PRODUCT(S" + (counter + 1) + ",(T" + (counter + 1) + "/3600))), 3)";

                            /*IV. Machining 2/Secondary Operation
                             * [Secondary Cost/Part]= [Cycle Time (sec)] * ([Man + Machine Rate/Hour] / 3600)
                             * =(PRODUCT(W21,(X21/3600)))
                             */
                            excWsheet.Cells[counter, 24].Formula = "=ROUND((PRODUCT(W" + (counter + 1) + ",(X" + (counter + 1) + "/3600))), 3)";

                            /*V. Machining 3/Secondary Operation
                             * [Secondary Cost/Part]= [Cycle Time (sec)] * ([Man + Machine Rate/Hour] / 3600)
                             * =(PRODUCT(AA21,(AB21/3600)))
                             */
                            excWsheet.Cells[counter, 28].Formula = "=ROUND((PRODUCT(AA" + (counter + 1) + ",(AB" + (counter + 1) + "/3600))), 3)";

                            /*VI. Surface Treatment Coating / Painting Cost Per Hour
                             * [Coating / Painting Cost per Hour] =[Man + Machine Rate/Hour] / [Parts Per Hour]
                             * =(AF21/AE21)
                             */
                            excWsheet.Cells[counter, 32].Formula = "=IF(OR(ISBLANK(AF" + (counter + 1) + "), ISBLANK(AE" + (counter + 1) + "), AE" + (counter + 1) + "=0, AF" + (counter + 1) + "=0), \"0\", ROUND((AF" + (counter + 1) + "/AE" + (counter + 1) + "), 3))";

                            /*IX. Final Piece Price / Part=
                                (
                                  [Raw Mat'l TOTAL]
                                + [Process / Conversion Cost / Part]
                                + [Machining Cost / Part]
                                + [Machining 2/Secondary Operation Secondary Cost/Part]
                                + [Machining 3/Other Operation Secondary Cost/Part]
                                + [Coating / Painting Cost per Hour]
                                + [Inventory Carrying Cost (If Applicable)]
                                + [Packing]
                                + [Local Freight to Port]
                                + [Profit and S, G & A]
                                )
                             */
                            excWsheet.Cells[counter, 39].Formula = "=ROUND((SUM(L" + (counter + 1) + ", Q" + (counter + 1) + ", U" + (counter + 1) + ", Y" + (counter + 1) + ", AC" + (counter + 1) + ", AG" + (counter + 1) + ", AH" + (counter + 1) + ", AI" + (counter + 1) + ", AJ" + (counter + 1) + ", AK" + (counter + 1) + ", AL" + (counter + 1) + ")), 3)";

                            excWsheet.Cells[counter, 38].Formula = "=IF(AND(OR(ISBLANK(AH" + (counter + 1) + "),AH" + (counter + 1) + "=0), OR(ISBLANK(AI" + (counter + 1) + "),AI" + (counter + 1) + "=0), OR(ISBLANK(AJ" + (counter + 1) + "),AJ" + (counter + 1) + "=0), OR(ISBLANK(AK" + (counter + 1) + "),AK" + (counter + 1) + "=0), OR(ISBLANK(AL" + (counter + 1) + "),AL" + (counter + 1) + "=0), OR(ISBLANK(AN" + (counter + 1) + "),AN" + (counter + 1) + "=0)), \"0\", ROUND((SUM(AH" + (counter + 1) + ", AI" + (counter + 1) + ", AJ" + (counter + 1) + ", AK" + (counter + 1) + ", AL" + (counter + 1) + ") * 100 /(AN" + (counter + 1) + ")), 2))";


                            #region Formula Validation

                            /*Surface Treatment Validation Start*/
                            string stValidation = "IF(";
                            stValidation += "AND(ISBLANK(AE" + (counter + 1) + "), ISBLANK(AF" + (counter + 1) + ")), \"\" ";
                            stValidation += ", IF(";
                            stValidation += "OR(ISBLANK(AE" + (counter + 1) + "), ISBLANK(AF" + (counter + 1) + ")), \"Please enter Surface Treatment Parts Per Hour and Man + Machine Rate/hour \" ";
                            stValidation += ", \"\" ))";
                            /*Surface Treatment Validation End*/

                            /*Machining Other Operation Validation Start*/
                            string mooValidation = "IF(";
                            mooValidation += "AND(ISBLANK(AA" + (counter + 1) + "), ISBLANK(AB" + (counter + 1) + ")), " + stValidation + " ";
                            mooValidation += ", IF(";
                            mooValidation += "OR(ISBLANK(AA" + (counter + 1) + "), ISBLANK(AB" + (counter + 1) + ")), \"Please enter Machining Other Operation Cycle Time(sec) and Man + Machine Rate/hour \" ";
                            mooValidation += ", " + stValidation + " ))";
                            /*Machining Other Operation Validation End*/

                            /*Machining Secondary Operation Validation Start*/
                            string msoValidation = "IF(";
                            msoValidation += "AND(ISBLANK(W" + (counter + 1) + "), ISBLANK(X" + (counter + 1) + ")), " + mooValidation + " ";
                            msoValidation += ", IF(";
                            msoValidation += "OR(ISBLANK(W" + (counter + 1) + "), ISBLANK(X" + (counter + 1) + ")), \"Please enter Machining Secondary Operation Cycle Time(sec) and Man + Machine Rate/hour \" ";
                            msoValidation += ", " + mooValidation + " ))";
                            /*Machining Secondary Operation Validation End*/

                            /*Machining Validation Start*/
                            string machiningValidation = "IF(";
                            machiningValidation += "AND(ISBLANK(S" + (counter + 1) + "), ISBLANK(T" + (counter + 1) + ")), " + msoValidation + " ";
                            machiningValidation += ", IF(";
                            machiningValidation += "OR(ISBLANK(S" + (counter + 1) + "), ISBLANK(T" + (counter + 1) + ")), \"Please enter Machining Cycle Time(sec) and Man + Machine Rate/hour \" ";
                            machiningValidation += ", " + msoValidation + " ))";
                            /*Machining Validation End*/

                            string formulaValidation = "=IF(";

                            formulaValidation += "AND(";

                            /*
                             * D - MIN ORDER QTY --------------      E  
                             * I - NO OF CAVITITES-------------      AO
                             * F - TOOLING COST  --------------      AP
                             * L - RAW MAT INPUT --------------      H
                             * M - RAW MAT COST  --------------      J
                             * N - MFG REJ RATE  --------------      K (LOSS)
                             * 
                             */

                            formulaValidation += "OR(ISBLANK(E" + (counter + 1) + "),(E" + (counter + 1) + ")=0), OR(ISBLANK(AO" + (counter + 1) + "),(AO" + (counter + 1) + ")=0)";
                            formulaValidation += ", OR(ISBLANK(H" + (counter + 1) + "),(H" + (counter + 1) + ")=0), OR(ISBLANK(J" + (counter + 1) + "),(J" + (counter + 1) + ")=0)";
                            formulaValidation += ", OR(ISBLANK(M" + (counter + 1) + "),(M" + (counter + 1) + ")=0), OR(ISBLANK(N" + (counter + 1) + "),(N" + (counter + 1) + ")=0), OR(ISBLANK(O" + (counter + 1) + "),(O" + (counter + 1) + ")=0), OR(ISBLANK(P" + (counter + 1) + "),(P" + (counter + 1) + ")=0)";
                            formulaValidation += ", OR(ISBLANK(S" + (counter + 1) + "),(S" + (counter + 1) + ")=0), OR(ISBLANK(T" + (counter + 1) + "),(T" + (counter + 1) + ")=0)";
                            formulaValidation += ", OR(ISBLANK(W" + (counter + 1) + "),(W" + (counter + 1) + ")=0), OR(ISBLANK(X" + (counter + 1) + "),(X" + (counter + 1) + "=0))";
                            formulaValidation += ", OR(ISBLANK(AA" + (counter + 1) + "),(AA" + (counter + 1) + ")=0), OR(ISBLANK(AB" + (counter + 1) + "),(AB" + (counter + 1) + ")=0)";
                            formulaValidation += ", OR(ISBLANK(AE" + (counter + 1) + "),(AE" + (counter + 1) + ")=0), OR(ISBLANK(AF" + (counter + 1) + "),(AF" + (counter + 1) + ")=0)";
                            formulaValidation += ", OR(ISBLANK(AH" + (counter + 1) + "),(AH" + (counter + 1) + ")=0), OR(ISBLANK(AI" + (counter + 1) + "),(AI" + (counter + 1) + ")=0), OR(ISBLANK(AJ" + (counter + 1) + "),(AJ" + (counter + 1) + ")=0), OR(ISBLANK(AK" + (counter + 1) + "),(AK" + (counter + 1) + ")=0), OR(ISBLANK(AL" + (counter + 1) + "),(AL" + (counter + 1) + ")=0)";
                            formulaValidation += "), \"\" ";

                            formulaValidation += ", IF(";
                            formulaValidation += "OR(";
                            formulaValidation += "ISTEXT(E" + (counter + 1) + "), ISTEXT(AO" + (counter + 1) + ")";
                            formulaValidation += ", ISTEXT(H" + (counter + 1) + "), ISTEXT(J" + (counter + 1) + ")";
                            formulaValidation += ", ISTEXT(N" + (counter + 1) + "), ISTEXT(N" + (counter + 1) + "), ISTEXT(O" + (counter + 1) + ")";
                            formulaValidation += ", ISTEXT(S" + (counter + 1) + "), ISTEXT(T" + (counter + 1) + ")";
                            formulaValidation += ", ISTEXT(W" + (counter + 1) + "), ISTEXT(X" + (counter + 1) + ")";
                            formulaValidation += ", ISTEXT(AA" + (counter + 1) + "), ISTEXT(AB" + (counter + 1) + ")";
                            formulaValidation += ", ISTEXT(AE" + (counter + 1) + "), ISTEXT(AF" + (counter + 1) + ")";
                            formulaValidation += ", ISTEXT(AH" + (counter + 1) + "), ISTEXT(AI" + (counter + 1) + "), ISTEXT(AJ" + (counter + 1) + "), ISTEXT(AK" + (counter + 1) + "), ISTEXT(AL" + (counter + 1) + ")";
                            formulaValidation += "), \"Please enter numeric or decimal values.\" ";

                            formulaValidation += ", IF(";
                            formulaValidation += "OR(";
                            formulaValidation += "ISBLANK(E" + (counter + 1) + "), ISBLANK(AO" + (counter + 1) + ")";
                            formulaValidation += ", ISBLANK(H" + (counter + 1) + "), ISBLANK(J" + (counter + 1) + ")";
                            formulaValidation += ", ISBLANK(M" + (counter + 1) + "), ISBLANK(N" + (counter + 1) + "), ISBLANK(O" + (counter + 1) + "), ISBLANK(P" + (counter + 1) + ")";
                            formulaValidation += "), \"Values of Min. Order Qty, No. Of Cavities, Raw Mat'l Input, Raw Mat'l Cost, Machine Description, Machine Size, Cycle Time  & Man + Machine Rate/Hour are mandatory.\" ";

                            formulaValidation += ", " + machiningValidation + " )))";

                            #endregion

                            excWsheet.Cells[counter, 44].Formula = formulaValidation;

                            counter++;
                        }
                    }

                    //** start ** Unprotect the sheet and lock all the cells
                    //Unlock the editable cells
                 
                    excWsheet.Cells[13, 1].Style.Locked = false;
                    excWsheet.Cells[14, 1].Style.Locked = false;
                    excWsheet.Cells[15, 1].Style.Locked = false;
                    excWsheet.Cells[16, 1].Style.Locked = false;
                    excWsheet.Cells[17, 1].Style.Locked = false;
                    //** end **

                    excWsheet.Cells[4, 0].Value = "P O BOX 401";
                    excWsheet.Cells[5, 0].Value = "Lewis Center, OH 43035";
                    excWsheet.Cells[7, 0].Value = "VENDOR NAME:";
                    excWsheet.Cells[7, 1].Value = supplierItem.CompanyName.ToUpper();

                    excWsheet.Cells[8, 0].Value = "RFQ # ";
                    excWsheet.Cells[8, 1].Value = criteria.RFQId;

                    RFQ rfqObj = new RFQ();
                    DTO.Library.RFQ.RFQ.RFQ rfqInfo = rfqObj.FindById(criteria.RFQId).Result;

                    excWsheet.Cells[9, 0].Value = "Supplier Requirement";
                    excWsheet.Cells[9, 1].Value = rfqInfo.SupplierRequirement;
                    
                    excWsheet.Cells[10, 0].Value = "Remarks";
                    excWsheet.Cells[10, 1].Value = rfqInfo.Remarks;

                    excWsheet.Cells[12, 0].Value = "Request For Quote (RFQ)";

                    excWsheet.Cells[14, 0].Value = "Exchange Rate 1 USD =";
                    excWsheet.Cells[14, 2].Formula = "=IF(ISBLANK(B15),\"Value for Exchange Rate is mandatory.\",IF(ISTEXT(B15),\"Please enter numeric values.\",\"\"))";
                    excWsheet.Cells[14, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[15, 0].Value = "Currency =";
                    excWsheet.Cells[15, 2].Formula = "=IF(ISBLANK(B16),\"Value for Currency is mandatory.\",\"\")";
                    excWsheet.Cells[15, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;

                    excWsheet.Cells[16, 0].Value = "Raw Material Price Assumed \n(Please include unit of measure…i.e. $1.000/kg) =";
                    excWsheet.Cells[16, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;

                    excWsheet.Cells[17, 0].Value = "Supplier Remarks";

                    excWsheet.Cells[19, 0].Value = "Part(s) List";
                    excWsheet.Cells[20, 0].Value = "Part Detail";
                    excWsheet.Cells[20, 5].Value = "Raw Material";
                    excWsheet.Cells[20, 12].Value = "Primary Process/Conversion";
                    excWsheet.Cells[20, 17].Value = "Machining";
                    excWsheet.Cells[20, 21].Value = "Machining 2/Secondary Operation";
                    excWsheet.Cells[20, 25].Value = "Machining 3/Other Operation";
                    excWsheet.Cells[20, 29].Value = "Surface Treatment";
                    excWsheet.Cells[20, 33].Value = "Overhead";
                    excWsheet.Cells[20, 40].Value = "Tooling";

                    excWsheet.Cells[21, 0].Value = "Part No.";
                    excWsheet.Cells[21, 1].Value = "Description";
                    excWsheet.Cells[21, 2].Value = "Additional Description";
                    excWsheet.Cells[21, 3].Value = "Annual Quantity";
                    excWsheet.Cells[21, 4].Value = "Min. Order Qty";

                    excWsheet.Cells[21, 5].Value = "Raw Material Description";
                    excWsheet.Cells[21, 6].Value = "Raw Material Index Used";
                    excWsheet.Cells[21, 7].Value = "Raw Mat'l Input (Gross wt in kg)";
                    excWsheet.Cells[21, 8].Value = "Part Weight (kg)";
                    excWsheet.Cells[21, 9].Value = "Raw Mat'l Cost (USD)(per kg)";
                    excWsheet.Cells[21, 10].Value = "Material Loss";
                    excWsheet.Cells[21, 11].Value = "Raw Mat'l TOTAL";

                    excWsheet.Cells[21, 12].Value = "Machine Description";
                    excWsheet.Cells[21, 13].Value = "Machine Size (Tons)";
                    excWsheet.Cells[21, 14].Value = "Cycle Time (sec)";
                    excWsheet.Cells[21, 15].Value = "Man + Machine Rate/Hour";
                    excWsheet.Cells[21, 16].Value = "Process/Conversion Cost/Part";

                    excWsheet.Cells[21, 17].Value = "Machining Description";
                    excWsheet.Cells[21, 18].Value = "Cycle Time (sec)";
                    excWsheet.Cells[21, 19].Value = "Man + Machine Rate/Hour";
                    excWsheet.Cells[21, 20].Value = "Machining Cost/Part";

                    excWsheet.Cells[21, 21].Value = "Secondary Operation Description";
                    excWsheet.Cells[21, 22].Value = "Cycle Time (sec)";
                    excWsheet.Cells[21, 23].Value = "Man + Machine Rate/Hour";
                    excWsheet.Cells[21, 24].Value = "Secondary Cost/Part";

                    excWsheet.Cells[21, 25].Value = "Secondary Operation Description";
                    excWsheet.Cells[21, 26].Value = "Cycle Time (sec)";
                    excWsheet.Cells[21, 27].Value = "Man + Machine Rate/Hour";
                    excWsheet.Cells[21, 28].Value = "Secondary Cost/Part";

                    excWsheet.Cells[21, 29].Value = "Coating Type";
                    excWsheet.Cells[21, 30].Value = "Parts Per Hour";
                    excWsheet.Cells[21, 31].Value = "Man + Machine Rate/Hour";
                    excWsheet.Cells[21, 32].Value = "Coating / Painting Cost Per Hour";

                    excWsheet.Cells[21, 33].Value = "Inventory Carrying Cost (if applicable)";
                    excWsheet.Cells[21, 34].Value = "Packaging Material";
                    excWsheet.Cells[21, 35].Value = "Packing Labour";
                    excWsheet.Cells[21, 36].Value = "FOB Port(Shipping Cost Per PC.)";
                    excWsheet.Cells[21, 37].Value = "Profit and S, G & A";
                    excWsheet.Cells[21, 38].Value = "Overhead as % of Piece Price";
                    excWsheet.Cells[21, 39].Value = "Final Piece Price / Part";

                    excWsheet.Cells[21, 40].Value = "No. Of Cavities";
                    excWsheet.Cells[21, 41].Value = "Tooling Cost";
                    excWsheet.Cells[21, 42].Value = "Tooling Warranty";
                    excWsheet.Cells[21, 43].Value = "Supplier Tooling Leadtime";

                    excWsheet.Columns[1].AutoFit();

                    for (int index = 2; index < 44; index++)
                    {
                        excWsheet.Columns[index].Width = 20 * 10 * 25;
                    }
                    excWsheet.Columns[44].Width = 20 * 30 * 40;
                    //MES Logo
                    cr = excWsheet.Cells.GetSubrange("A1", "AR4"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.White, System.Drawing.Color.White);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;
                    excWsheet.Pictures.Add(context.Server.MapPath(Constants.IMAGEFOLDER) + "MESlogoPdf.png", GemBox.Spreadsheet.PositioningMode.FreeFloating, new AnchorCell(excWsheet.Columns[0], excWsheet.Rows[0], true), new AnchorCell(excWsheet.Columns[0], excWsheet.Rows[3], false));


                    excWsheet.ProtectionSettings.PasswordHash = 1;
                    excWsheet.Protected = true;

                    string generatedFileName = supplierItem.CompanyName + "_" + criteria.RFQId + "_" + System.DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx";
                    string tempFilepath = context.Server.MapPath("~") + Constants.REPORTSTEMPLATEFILEPATH + generatedFileName;
                    if (!System.IO.Directory.Exists(context.Server.MapPath(@"~") + (Constants.REPORTSTEMPLATEFILEPATH)))
                        System.IO.Directory.CreateDirectory(context.Server.MapPath(@"~") + (Constants.REPORTSTEMPLATEFILEPATH));
                    else
                    {
                        if (File.Exists(tempFilepath))
                            File.Delete(tempFilepath);
                    }
                    myExcelFile.SaveXlsx(tempFilepath);

                    filePath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"])
                         + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                         + Constants.TEMPFILESEMAILATTACHMENTSFOLDER
                         + generatedFileName;

                    Helper.BlobHelper.DeleteBlobfile(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), filePath);

                    Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                                  , Constants.TEMPFILESEMAILATTACHMENTSFOLDERName
                                  , generatedFileName
                                  , tempFilepath);
                    File.Delete(tempFilepath);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    excWsheet = null;
                    myExcelFile.ClosePreservedXlsx();
                }
            }
            catch (Exception ex)//Error
            {
                throw ex;
            }
            return filePath;
        }

        private string CreateSupplierQuoteFile(DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria criteria)
        {
            List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> SupplierQuoteDetails = this.GetRFQSupplierPartQuoteListbyUniqueURL(criteria).Result;

            Supplier.Suppliers supplierObj = new Supplier.Suppliers();
            DTO.Library.RFQ.Supplier.Suppliers supplierItem = supplierObj.FindById(criteria.SupplierId).Result;

            string filePath = string.Empty;
            var context = HttpContext.Current;
            try
            {
                ExcelFile myExcelFile = new ExcelFile();
                ExcelWorksheet excWsheet = myExcelFile.Worksheets.Add("Parts List");
                ExcelWorksheet excWsheet2 = myExcelFile.Worksheets.Add("ManufacturerList");

                try
                {
                    //Lock sheet 2 - 'ManufacturerList'
                    CellRange cr = excWsheet2.Cells.GetSubrange("A1", "AB100");
                    cr.Merged = true; cr.Style.Locked = true;
                    cr.Merged = false; cr = null;

                    // Frozen Columns (first column is frozen)
                    excWsheet.Panes = new WorksheetPanes(PanesState.Frozen, 1, 0, "B1", PanePosition.TopRight);

                    cr = excWsheet.Cells.GetSubrange("A1", "AB100");
                    cr.Merged = true; cr.Style.Locked = true; cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A13", "U13"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    cr = excWsheet.Cells.GetSubrange("A20", "U20"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*Part Detail*/
                    cr = excWsheet.Cells.GetSubrange("A21", "F21"); cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.WrapText = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*Raw Material*/
                    cr = excWsheet.Cells.GetSubrange("G21", "M21"); cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.WrapText = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*OTHER*/
                    cr = excWsheet.Cells.GetSubrange("N21", "Q21"); cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.WrapText = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*TOOLING*/
                    cr = excWsheet.Cells.GetSubrange("R21", "U21"); cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.WrapText = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    cr = excWsheet.Cells.GetSubrange("A8", "B8"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A9", "B9"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top; cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A10", "B10"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top; cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A11", "B11"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top; cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A15", "B15"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A16", "B16"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A17", "B17"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.WrapText = true; cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A18", "B18"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.WrapText = true; cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A22", "U22"); cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.WrapText = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond); cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A5", "F5"); cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight; cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A6", "F6"); cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight; cr.Merged = false; cr = null;

                    excWsheet.Cells[7, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[7, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    excWsheet.Cells[7, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[7, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;

                    excWsheet.Cells[8, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[8, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    excWsheet.Cells[8, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[8, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;

                    excWsheet.Cells[9, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[9, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    excWsheet.Cells[9, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[9, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;

                    excWsheet.Cells[10, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[10, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    excWsheet.Cells[10, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[10, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    
                    excWsheet.Cells[12, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[12, 0].Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    excWsheet.Cells[12, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                    excWsheet.Cells[14, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[14, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[14, 1].Style.NumberFormat = "0.000";
                    excWsheet.Cells[14, 1].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);
                    excWsheet.Cells[14, 2].Style.Font.Color = System.Drawing.Color.Red;

                    excWsheet.Cells[15, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[15, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[15, 1].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);
                    excWsheet.Cells[15, 2].Style.Font.Color = System.Drawing.Color.Red;

                    excWsheet.Cells[16, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[16, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[16, 1].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);
                    excWsheet.Cells[16, 2].Style.Font.Color = System.Drawing.Color.Red;

                    excWsheet.Cells[17, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[17, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[17, 1].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);

                    excWsheet.Cells[19, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[19, 0].Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    excWsheet.Cells[19, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                    excWsheet.Columns[0].Width = 35 * 256;

                    int counter = 22;

                    if (SupplierQuoteDetails.Count > 0)
                    {
                        foreach (var supplierDetailQuotePart in SupplierQuoteDetails)
                        {
                            excWsheet.Cells[counter, 21].Style.Font.Color = Color.Red;
                            excWsheet.Cells[counter, 21].Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                            excWsheet.Cells[counter, 21].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 21].Style.WrapText = true;

                            cr = excWsheet.Cells.GetSubrange("A" + (counter + 1), "U" + (counter + 1)); cr.Merged = true;
                            cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                            cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, Color.Black, LineStyle.Thin);
                            cr.Merged = false; cr = null; //Vertical Allignment Style set to "Top"

                            #region Back Color Yellow and Unlocked
                            cr = excWsheet.Cells.GetSubrange("E" + (counter + 1), "F" + (counter + 1)); cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.Yellow, Color.Yellow);
                            cr.Style.Locked = false; cr.Merged = false; cr = null; //Unlock the editable cells

                            cr = excWsheet.Cells.GetSubrange("H" + (counter + 1), "I" + (counter + 1)); cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.Yellow, Color.Yellow);
                            cr.Style.Locked = false; cr.Merged = false; cr = null; //Unlock the editable cells

                            cr = excWsheet.Cells.GetSubrange("K" + (counter + 1), "K" + (counter + 1)); cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.Yellow, Color.Yellow);
                            cr.Style.Locked = false; cr.Merged = false; cr = null; //Unlock the editable cells

                            cr = excWsheet.Cells.GetSubrange("N" + (counter + 1), "P" + (counter + 1)); cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.Yellow, Color.Yellow);
                            cr.Style.Locked = false; cr.Merged = false; cr = null; //Unlock the editable cells

                            cr = excWsheet.Cells.GetSubrange("R" + (counter + 1), "U" + (counter + 1)); cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.Yellow, Color.Yellow);
                            cr.Style.Locked = false; cr.Merged = false; cr = null; //Unlock the editable cells
                            #endregion
                            #region Cell Format
                            excWsheet.Cells[counter, 8].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 8].Value = 0.000;
                            excWsheet.Cells[counter, 10].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 10].Value = 0.000;
                            excWsheet.Cells[counter, 11].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 11].Value = 0.000;
                            excWsheet.Cells[counter, 12].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 12].Value = 0.000;
                            excWsheet.Cells[counter, 13].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 13].Value = 0.000;
                            excWsheet.Cells[counter, 14].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 14].Value = 0.000;
                            excWsheet.Cells[counter, 15].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 15].Value = 0.000;
                            excWsheet.Cells[counter, 16].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 16].Value = 0.000;
                            excWsheet.Cells[counter, 17].Style.NumberFormat = "0";
                            excWsheet.Cells[counter, 17].Value = 0;
                            excWsheet.Cells[counter, 18].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 18].Value = 0.000;
                            #endregion
                            #region Cell Alignment
                            //PART
                            excWsheet.Cells[counter, 3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 5].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            //RAW MATERIAL
                            excWsheet.Cells[counter, 6].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 7].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 8].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 9].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 10].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 11].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 12].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            //OTHER
                            excWsheet.Cells[counter, 13].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 14].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 15].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 16].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            //TOOLING
                            excWsheet.Cells[counter, 17].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 18].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 19].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 20].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            #endregion

                            excWsheet.Cells[counter, 0].Style.WrapText = true;
                            excWsheet.Cells[counter, 1].Style.WrapText = true;
                            excWsheet.Cells[counter, 2].Style.WrapText = true;
                            excWsheet.Cells[counter, 6].Style.WrapText = true;
                            excWsheet.Cells[counter, 19].Style.WrapText = true;
                            excWsheet.Cells[counter, 20].Style.WrapText = true;
                            excWsheet.Cells[counter, 21].Style.WrapText = true;

                            excWsheet.Cells[counter, 0].Value = supplierDetailQuotePart.CustomerPartNo;
                            excWsheet.Cells[counter, 1].Value = supplierDetailQuotePart.PartDescription;
                            excWsheet.Cells[counter, 2].Value = supplierDetailQuotePart.AdditionalPartDesc;
                            excWsheet.Cells[counter, 3].Value = supplierDetailQuotePart.EstimatedQty;
                            excWsheet.Cells[counter, 6].Value = supplierDetailQuotePart.MaterialType;
                            excWsheet.Cells[counter, 9].Value = supplierDetailQuotePart.PartWeightKG.HasValue ? supplierDetailQuotePart.PartWeightKG.Value.ToString("0.000") : string.Empty;

                            #region Drop Down Set
                            DataValidation dv;

                            /*Manufacturer List*/
                            cr = excWsheet.Cells.GetSubrange("E" + (counter + 1), "E" + (counter + 1));
                            cr.Merged = true;
                            dv = new DataValidation(cr);

                            List<MES.Data.Library.SearchSuppliersQuotedNotQuoted_Result> manufacturerList = this.DataContext.SearchSuppliersQuotedNotQuoted(criteria.RFQId).ToList();
                            int startRow = 2, startCol = 2;
                            excWsheet2.Cells[0, startCol].Value = "Manufacturer(s)";
                            excWsheet2.Cells[0, startCol].Style.Font.Weight = ExcelFont.BoldWeight;
                            foreach (var listItem in manufacturerList)
                            {
                                excWsheet2.Cells[startRow, startCol].Value = listItem.CompanyName.ToString().Trim();
                                startRow++;
                            }

                            dv.Type = DataValidationType.List;
                            dv.InputMessage = "Select Manufacturer";
                            dv.Formula1 = "='ManufacturerList'!C2:C" + startRow;
                            dv.ShowErrorAlert = true;
                            excWsheet.DataValidations.Add(dv);
                            cr.Merged = false;
                            cr = null;
                            excWsheet2.Columns[2].AutoFit();
                            /*end here*/
                            #endregion

                            /*I.
                             * Example: Input = 1kg  Raw Mat’l Cost=2.15/kg
                                       1x2.15= $2.15.  Loss would be calculation of Input wt. – RFQ wt./Input Wt.
                             * =IF(I21 > 0 ),(I21-J21)/I21,""
                             */
                            excWsheet.Cells[counter, 11].Formula = "=IF(I" + (counter + 1) + ">0,IFERROR((I" + (counter + 1) + "-J" + (counter + 1) + ")/I" + (counter + 1) + ",1),0)";

                            /*II. Raw Mat'l TOTAL
                            * [Raw Mat'l TOTAL] = [Raw Mat'l Input (Gross wt in kg] * [Raw Mat'l Cost (per kg)]
                            * =(PRODUCT(I21,K21))
                            */
                            string rawMatFormula = "=ROUND(PRODUCT(I" + (counter + 1) + ", K" + (counter + 1) + "),3)";
                            excWsheet.Cells[counter, 12].Formula = rawMatFormula;

                            excWsheet.Cells[counter, 16].Formula = "=SUM(M" + (counter + 1) + ":P" + (counter + 1) + ")";
                            string formulaValidation = "=IF(";
                            formulaValidation += "AND(";

                            formulaValidation += "OR(ISBLANK(F" + (counter + 1) + "),(F" + (counter + 1) + "=0)), OR(ISBLANK(R" + (counter + 1) + "),(R" + (counter + 1) + "=0)), OR(ISBLANK(S" + (counter + 1) + "),(S" + (counter + 1) + "=0))";
                            formulaValidation += ", OR(ISBLANK(H" + (counter + 1) + "),(H" + (counter + 1) + "=0)), OR(ISBLANK(I" + (counter + 1) + "),(I" + (counter + 1) + "=0)), OR(ISBLANK(K" + (counter + 1) + "),(K" + (counter + 1) + "=0))";
                            formulaValidation += ", OR(ISBLANK(N" + (counter + 1) + "),(N" + (counter + 1) + "=0)), OR(ISBLANK(O" + (counter + 1) + "),(O" + (counter + 1) + "=0))";
                            formulaValidation += ", OR(ISBLANK(P" + (counter + 1) + "),(P" + (counter + 1) + "=0))";
                            formulaValidation += "), \"\" ";

                            formulaValidation += ", IF(";

                            formulaValidation += "OR(";
                            formulaValidation += "ISTEXT(F" + (counter + 1) + "), ISTEXT(R" + (counter + 1) + "), ISTEXT(S" + (counter + 1) + ")";
                            formulaValidation += ", ISTEXT(I" + (counter + 1) + "), ISTEXT(K" + (counter + 1) + ")";
                            formulaValidation += ", ISTEXT(N" + (counter + 1) + "), ISTEXT(O" + (counter + 1) + "), ISTEXT(P" + (counter + 1) + ")";
                            formulaValidation += "), \"Please enter numeric values.\" ";

                            formulaValidation += ", IF(";
                            formulaValidation += "OR(";
                            formulaValidation += "ISBLANK(F" + (counter + 1) + "), ISBLANK(R" + (counter + 1) + "), (R" + (counter + 1) + "=0)";
                            formulaValidation += ", ISBLANK(I" + (counter + 1) + "), (I" + (counter + 1) + "=0), ISBLANK(K" + (counter + 1) + "), (K" + (counter + 1) + "=0)";
                            formulaValidation += ", ISBLANK(N" + (counter + 1) + "), (N" + (counter + 1) + "=0)";
                            formulaValidation += "), \"Values for Min. Order Qty, Raw Mat'l Input, Raw Mat'l Cost, Conversion Cost & No. Of Cavities are mandatory.\" ";

                            formulaValidation += ", \"\" )";
                            formulaValidation += ")";
                            formulaValidation += ")";

                            excWsheet.Cells[counter, 21].Formula = formulaValidation;
                            //"=IF(AND(ISBLANK(H" + (counter + 1) + "), ISBLANK(K" + (counter + 1) + "), ISBLANK(L" + (counter + 1) + "), ISBLANK(M" + (counter + 1) + "), ISBLANK(N" + (counter + 1) + "), ISBLANK(O" + (counter + 1) + ")),\"\",IF(OR(ISTEXT(F" + (counter + 1) + "), ISTEXT(H" + (counter + 1) + "), ISTEXT(K" + (counter + 1) + "), ISTEXT(L" + (counter + 1) + "), ISTEXT(M" + (counter + 1) + "), ISTEXT(N" + (counter + 1) + "), ISTEXT(O" + (counter + 1) + ")),\"Please enter numeric values.\",IF(OR(ISBLANK(H" + (counter + 1) + "), ISBLANK(K" + (counter + 1) + "), ISBLANK(L" + (counter + 1) + "), ISBLANK(M" + (counter + 1) + "), ISBLANK(N" + (counter + 1) + "), ISBLANK(O" + (counter + 1) + ")),\"Values for Tooling Cost, Material Cost, Conversion Cost & No. Of Cavities are mandatory.\",\"\")))";

                            counter++;
                        }
                    }

                    //** start ** Unprotect the sheet and lock all the cells
                    //Unlock the editable cells
                    
                    excWsheet.Cells[13, 1].Style.Locked = false;
                    excWsheet.Cells[14, 1].Style.Locked = false;
                    excWsheet.Cells[15, 1].Style.Locked = false;
                    excWsheet.Cells[16, 1].Style.Locked = false;
                    excWsheet.Cells[17, 1].Style.Locked = false;
                    //** end **

                    excWsheet.Cells[4, 0].Value = "P O BOX 401";
                    excWsheet.Cells[5, 0].Value = "Lewis Center, OH 43035";
                    excWsheet.Cells[7, 0].Value = "VENDOR NAME:";
                    excWsheet.Cells[7, 1].Value = supplierItem.CompanyName.ToUpper();

                    excWsheet.Cells[8, 0].Value = "RFQ # ";
                    excWsheet.Cells[8, 1].Value = criteria.RFQId;

                    RFQ rfqObj = new RFQ();
                    DTO.Library.RFQ.RFQ.RFQ rfqInfo = rfqObj.FindById(criteria.RFQId).Result;

                    excWsheet.Cells[9, 0].Value = "Supplier Requirement";
                    excWsheet.Cells[9, 1].Value = rfqInfo.SupplierRequirement;

                    excWsheet.Cells[10, 0].Value = "Remarks";
                    excWsheet.Cells[10, 1].Value = rfqInfo.Remarks;
                    
                    excWsheet.Cells[12, 0].Value = "Supplier Quote";

                    excWsheet.Cells[14, 0].Value = "Exchange Rate 1 USD =";
                    excWsheet.Cells[14, 2].Formula = "=IF(ISBLANK(B15),\"Value for Exchange Rate is mandatory.\",IF(ISTEXT(B15),\"Please enter numeric values.\",\"\"))";
                    excWsheet.Cells[14, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excWsheet.Cells[15, 0].Value = "Currency =";
                    excWsheet.Cells[15, 2].Formula = "=IF(ISBLANK(B16),\"Value for Currency is mandatory.\",\"\")";
                    excWsheet.Cells[15, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;

                    excWsheet.Cells[16, 0].Value = "Raw Material Price Assumed \n(Please include unit of measure…i.e. $1.000/kg) =";
                    excWsheet.Cells[16, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;

                    excWsheet.Cells[17, 0].Value = "Supplier Remarks";

                    excWsheet.Cells[19, 0].Value = "Part(s) List";
                    excWsheet.Cells[20, 0].Value = "Part Detail";
                    excWsheet.Cells[20, 6].Value = "Raw Material";
                    excWsheet.Cells[20, 13].Value = "Other";
                    excWsheet.Cells[20, 17].Value = "Tooling";

                    excWsheet.Cells[21, 0].Value = "Part No.";
                    excWsheet.Cells[21, 1].Value = "Description";
                    excWsheet.Cells[21, 2].Value = "Additional Description";
                    excWsheet.Cells[21, 3].Value = "Annual Quantity";
                    excWsheet.Cells[21, 4].Value = "Manufacturer";
                    excWsheet.Cells[21, 5].Value = "Min. Order Qty";

                    excWsheet.Cells[21, 6].Value = "Raw Material Description";
                    excWsheet.Cells[21, 7].Value = "Raw Material Index Used";
                    excWsheet.Cells[21, 8].Value = "Raw Mat'l Input (Gross wt in kg)";
                    excWsheet.Cells[21, 9].Value = "Part Weight (kg)";
                    excWsheet.Cells[21, 10].Value = "Raw Mat'l Cost (USD)(per kg)";
                    excWsheet.Cells[21, 11].Value = "Material Loss";
                    excWsheet.Cells[21, 12].Value = "Raw Mat'l TOTAL";

                    excWsheet.Cells[21, 13].Value = "Conversion Cost (USD)";
                    excWsheet.Cells[21, 14].Value = "Machining Cost (USD)";
                    excWsheet.Cells[21, 15].Value = "Other Process Cost (USD)";
                    excWsheet.Cells[21, 16].Value = "Final Piece Price / Part";

                    excWsheet.Cells[21, 17].Value = "No. Of Cavities";
                    excWsheet.Cells[21, 18].Value = "Tooling Cost";
                    excWsheet.Cells[21, 19].Value = "Tooling Warranty";
                    excWsheet.Cells[21, 20].Value = "Supplier Tooling Leadtime";

                    excWsheet.Columns[1].AutoFit();
                    excWsheet.Columns[2].AutoFit();
                    excWsheet.Columns[3].Width = 20 * 10 * 25;
                    excWsheet.Columns[4].Width = 20 * 10 * 20;
                    excWsheet.Columns[5].Width = 20 * 10 * 30;
                    excWsheet.Columns[6].Width = 20 * 10 * 30;
                    excWsheet.Columns[7].Width = 20 * 10 * 30;
                    excWsheet.Columns[8].Width = 20 * 10 * 30;
                    excWsheet.Columns[9].Width = 20 * 10 * 30;
                    excWsheet.Columns[10].Width = 20 * 10 * 30;
                    excWsheet.Columns[11].Width = 20 * 10 * 30;
                    excWsheet.Columns[12].Width = 20 * 10 * 30;
                    excWsheet.Columns[13].Width = 20 * 10 * 30;
                    excWsheet.Columns[14].Width = 20 * 10 * 30;
                    excWsheet.Columns[15].Width = 20 * 10 * 30;
                    excWsheet.Columns[16].Width = 20 * 10 * 30;
                    excWsheet.Columns[17].Width = 20 * 10 * 30;
                    excWsheet.Columns[18].Width = 20 * 10 * 30;
                    excWsheet.Columns[19].Width = 20 * 10 * 30;
                    excWsheet.Columns[20].Width = 20 * 10 * 30;
                    excWsheet.Columns[21].Width = 20 * 30 * 40;
                    //MES Logo
                    cr = excWsheet.Cells.GetSubrange("A1", "U4"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.White, System.Drawing.Color.White);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;
                    excWsheet.Pictures.Add(context.Server.MapPath(Constants.IMAGEFOLDER) + "MESlogoPdf.png", GemBox.Spreadsheet.PositioningMode.FreeFloating, new AnchorCell(excWsheet.Columns[0], excWsheet.Rows[0], true), new AnchorCell(excWsheet.Columns[0], excWsheet.Rows[3], false));

                    //ends here

                    excWsheet.ProtectionSettings.PasswordHash = 1;
                    excWsheet.Protected = true;
                    excWsheet2.ProtectionSettings.PasswordHash = 1;
                    excWsheet2.Protected = true;


                    string generatedFileName = supplierItem.CompanyName + "_" + criteria.RFQId + "_" + System.DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx";
                    string tempFilepath = context.Server.MapPath("~") + Constants.REPORTSTEMPLATEFILEPATH + generatedFileName;
                    if (!System.IO.Directory.Exists(context.Server.MapPath(@"~") + (Constants.REPORTSTEMPLATEFILEPATH)))
                        System.IO.Directory.CreateDirectory(context.Server.MapPath(@"~") + (Constants.REPORTSTEMPLATEFILEPATH));
                    else
                    {
                        if (File.Exists(tempFilepath))
                            File.Delete(tempFilepath);
                    }
                    myExcelFile.SaveXlsx(tempFilepath);

                    filePath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"])
                         + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                         + Constants.TEMPFILESEMAILATTACHMENTSFOLDER
                         + generatedFileName;

                    Helper.BlobHelper.DeleteBlobfile(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), filePath);

                    Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                                  , Constants.TEMPFILESEMAILATTACHMENTSFOLDERName
                                  , generatedFileName
                                  , tempFilepath);
                    File.Delete(tempFilepath);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    excWsheet = null;
                    myExcelFile.ClosePreservedXlsx();
                }
            }
            catch (Exception ex)//Error
            {
                throw ex;
            }
            return filePath;
        }

        private string CreateDetailSupplierQuoteFile(DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria criteria)
        {
            List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> SupplierQuoteDetails = this.GetRFQSupplierPartQuoteListbyUniqueURL(criteria).Result;
            Supplier.Suppliers supplierObj = new Supplier.Suppliers();
            DTO.Library.RFQ.Supplier.Suppliers supplierItem = supplierObj.FindById(criteria.SupplierId).Result;

            string filePath = string.Empty;
            var context = HttpContext.Current;
            try
            {
                ExcelFile myExcelFile = new ExcelFile();
                ExcelWorksheet excWsheet = myExcelFile.Worksheets.Add("Parts List");
                ExcelWorksheet excWsheet2 = myExcelFile.Worksheets.Add("ManufacturerList");
                try
                {
                    //Lock sheet 2 - 'ManufacturerList'
                    CellRange cr = excWsheet2.Cells.GetSubrange("A1", "AB100");
                    cr.Merged = true; cr.Style.Locked = true;
                    cr.Merged = false; cr = null;

                    // Frozen Columns (first column is frozen)
                    excWsheet.Panes = new WorksheetPanes(PanesState.Frozen, 1, 0, "B1", PanePosition.TopRight);

                    cr = excWsheet.Cells.GetSubrange("A1", "AS100");
                    cr.Merged = true; cr.Style.Locked = true;
                    cr.Merged = false;
                    cr = null;

                    cr = excWsheet.Cells.GetSubrange("A13", "AS13"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    cr = excWsheet.Cells.GetSubrange("A20", "AS20"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*Part Detail*/
                    cr = excWsheet.Cells.GetSubrange("A21", "F21"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*Raw Material*/
                    cr = excWsheet.Cells.GetSubrange("G21", "M21"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*Primary Process/Conversion*/
                    cr = excWsheet.Cells.GetSubrange("N21", "R21"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*Machining*/
                    cr = excWsheet.Cells.GetSubrange("S21", "V21"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*Machining 2/Secondary Operation*/
                    cr = excWsheet.Cells.GetSubrange("W21", "Z21"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*Machining 3/Other Operation*/
                    cr = excWsheet.Cells.GetSubrange("AA21", "AD21"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*Surface Treatment*/
                    cr = excWsheet.Cells.GetSubrange("AE21", "AH21"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*Overhead*/
                    cr = excWsheet.Cells.GetSubrange("AI21", "AO21"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    /*Tooling*/
                    cr = excWsheet.Cells.GetSubrange("AP21", "AS21"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BurlyWood, System.Drawing.Color.BurlyWood);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;

                    cr = excWsheet.Cells.GetSubrange("A8", "B8"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A9", "B9"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top; cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A10", "B10"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top; cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A11", "B11"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top; cr.Merged = false; cr = null;
                    
                    cr = excWsheet.Cells.GetSubrange("A15", "B15"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A16", "B16"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A17", "B17"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.WrapText = true; cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A18", "B18"); cr.Merged = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.WrapText = true; cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A22", "AS22"); cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.WrapText = true;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond); cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A5", "F5"); cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight; cr.Merged = false; cr = null;

                    cr = excWsheet.Cells.GetSubrange("A6", "F6"); cr.Merged = true;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight; cr.Merged = false; cr = null;

                    excWsheet.Cells[7, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[7, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    excWsheet.Cells[7, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[7, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;

                    excWsheet.Cells[8, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[8, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    excWsheet.Cells[8, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[8, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;

                    excWsheet.Cells[9, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[9, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    excWsheet.Cells[9, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[9, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;

                    excWsheet.Cells[10, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[10, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    excWsheet.Cells[10, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[10, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;

                    excWsheet.Cells[12, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[12, 0].Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    excWsheet.Cells[12, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                    excWsheet.Cells[14, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[14, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[14, 1].Style.NumberFormat = "0.000";
                    excWsheet.Cells[14, 1].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);
                    excWsheet.Cells[14, 2].Style.Font.Color = System.Drawing.Color.Red;

                    excWsheet.Cells[15, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[15, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[15, 1].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);
                    excWsheet.Cells[15, 2].Style.Font.Color = System.Drawing.Color.Red;

                    excWsheet.Cells[16, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[16, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[16, 1].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);
                    excWsheet.Cells[16, 2].Style.Font.Color = System.Drawing.Color.Red;

                    excWsheet.Cells[17, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[17, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.BlanchedAlmond, System.Drawing.Color.BlanchedAlmond);
                    excWsheet.Cells[17, 1].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);

                    excWsheet.Cells[19, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[19, 0].Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    excWsheet.Cells[19, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                    excWsheet.Columns[0].Width = 35 * 256;

                    int counter = 22;
                    List<string> ppcMachineDescriptionList = this.DataContext.MachineDescs.Select(rec => rec.MachineDescription.Replace(",", " ")).ToList();
                    List<string> mMachiningDescriptionList = this.DataContext.MachiningDescs.Select(rec => rec.MachiningDescription.Replace(",", " ")).ToList();
                    List<string> msoSecOprDescriptionList = this.DataContext.SecondaryOperationDescs.Select(rec => rec.SecondaryOperationDescription.Replace(",", " ")).ToList();
                    List<string> stCoatingTypeList = this.DataContext.CoatingTypes.Select(rec => rec.CoatingType1.Replace(",", " ")).ToList();

                    if (SupplierQuoteDetails.Count > 0)
                    {
                        foreach (var supplierDetailQuotePart in SupplierQuoteDetails)
                        {
                            /*Values for 
                              Tooling Cost
                             ,No. Of Cavities
                             ,Raw Mat'l Input, Raw Material Cost Per Kg
                             ,(Primary Process/Conversion)
                             ,Machine Description,
                             ,Machine Size, 
                             ,Cycle Time, 
                             ,Man + Machine Rate Per Hour 
                             & Process / Conversion Cost / Part
                             are mandatory.*/


                            excWsheet.Cells[counter, 45].Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                            excWsheet.Cells[counter, 45].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 45].Style.Font.Color = Color.Red;
                            excWsheet.Cells[counter, 45].Style.WrapText = true;

                            cr = excWsheet.Cells.GetSubrange("A" + (counter + 1), "AS" + (counter + 1));
                            cr.Merged = true;
                            cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                            cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, Color.Black, LineStyle.Thin);
                            cr.Merged = false;
                            cr = null; //Vertical Allignment Style set to Top and Border"


                            #region Back Color Yellow and Unlocked

                            cr = excWsheet.Cells.GetSubrange("E" + (counter + 1), "F" + (counter + 1));
                            cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.Yellow, Color.Yellow);
                            cr.Style.Locked = false;
                            cr.Merged = false;
                            cr = null;

                            cr = excWsheet.Cells.GetSubrange("H" + (counter + 1), "I" + (counter + 1));
                            cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.Yellow, Color.Yellow);
                            cr.Style.Locked = false;
                            cr.Merged = false;
                            cr = null;

                            cr = excWsheet.Cells.GetSubrange("K" + (counter + 1), "K" + (counter + 1));
                            cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.Yellow, Color.Yellow);
                            cr.Style.Locked = false;
                            cr.Merged = false;
                            cr = null;

                            cr = excWsheet.Cells.GetSubrange("N" + (counter + 1), "Q" + (counter + 1));
                            cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.Yellow, Color.Yellow);
                            cr.Style.Locked = false;
                            cr.Merged = false;
                            cr = null;

                            cr = excWsheet.Cells.GetSubrange("S" + (counter + 1), "U" + (counter + 1));
                            cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.Yellow, Color.Yellow);
                            cr.Style.Locked = false;
                            cr.Merged = false;
                            cr = null;

                            cr = excWsheet.Cells.GetSubrange("W" + (counter + 1), "Y" + (counter + 1));
                            cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.Yellow, Color.Yellow);
                            cr.Style.Locked = false;
                            cr.Merged = false;
                            cr = null;

                            cr = excWsheet.Cells.GetSubrange("AA" + (counter + 1), "AC" + (counter + 1));
                            cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.Yellow, Color.Yellow);
                            cr.Style.Locked = false;
                            cr.Merged = false;
                            cr = null;

                            cr = excWsheet.Cells.GetSubrange("AE" + (counter + 1), "AG" + (counter + 1));
                            cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, Color.Yellow, Color.Yellow);
                            cr.Style.Locked = false;
                            cr.Merged = false;
                            cr = null;

                            cr = excWsheet.Cells.GetSubrange("AI" + (counter + 1), "AM" + (counter + 1));
                            cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);
                            cr.Style.Locked = false;
                            cr.Merged = false;
                            cr = null;

                            cr = excWsheet.Cells.GetSubrange("AP" + (counter + 1), "AS" + (counter + 1));
                            cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Yellow);
                            cr.Style.Locked = false;
                            cr.Merged = false;
                            cr = null;
                            #endregion

                            #region Cell Format

                            excWsheet.Cells[counter, 42].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 42].Value = 0.000;
                            excWsheet.Cells[counter, 41].Style.NumberFormat = "0";
                            excWsheet.Cells[counter, 41].Value = 0;

                            excWsheet.Cells[counter, 8].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 8].Value = 0.000;
                            excWsheet.Cells[counter, 9].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 9].Value = 0.000;
                            excWsheet.Cells[counter, 10].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 10].Value = 0.000;
                            excWsheet.Cells[counter, 11].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 11].Value = 0.000;
                            excWsheet.Cells[counter, 12].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 12].Value = 0.00;

                            excWsheet.Cells[counter, 14].Style.NumberFormat = "0";
                            excWsheet.Cells[counter, 14].Value = 0;
                            excWsheet.Cells[counter, 15].Style.NumberFormat = "0";
                            excWsheet.Cells[counter, 15].Value = 0;
                            excWsheet.Cells[counter, 16].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 16].Value = 0.000;
                            excWsheet.Cells[counter, 17].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 17].Value = 0.000;

                            excWsheet.Cells[counter, 19].Style.NumberFormat = "0";
                            excWsheet.Cells[counter, 19].Value = 0;
                            excWsheet.Cells[counter, 20].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 20].Value = 0.000;
                            excWsheet.Cells[counter, 21].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 21].Value = 0.000;

                            excWsheet.Cells[counter, 23].Style.NumberFormat = "0";
                            excWsheet.Cells[counter, 23].Value = 0;
                            excWsheet.Cells[counter, 24].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 24].Value = 0.000;
                            excWsheet.Cells[counter, 25].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 25].Value = 0.000;

                            excWsheet.Cells[counter, 27].Style.NumberFormat = "0";
                            excWsheet.Cells[counter, 27].Value = 0;
                            excWsheet.Cells[counter, 28].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 28].Value = 0.000;
                            excWsheet.Cells[counter, 29].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 29].Value = 0.000;

                            excWsheet.Cells[counter, 31].Style.NumberFormat = "0";
                            excWsheet.Cells[counter, 31].Value = 0;
                            excWsheet.Cells[counter, 32].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 32].Value = 0.000;
                            excWsheet.Cells[counter, 33].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 33].Value = 0.000;

                            excWsheet.Cells[counter, 34].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 34].Value = 0.000;
                            excWsheet.Cells[counter, 35].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 35].Value = 0.000;
                            excWsheet.Cells[counter, 36].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 36].Value = 0.000;
                            excWsheet.Cells[counter, 37].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 37].Value = 0.000;
                            excWsheet.Cells[counter, 38].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 38].Value = 0.000;
                            excWsheet.Cells[counter, 39].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 39].Value = 0.000;
                            excWsheet.Cells[counter, 40].Style.NumberFormat = "0.000";
                            excWsheet.Cells[counter, 40].Value = 0.000;

                            #endregion

                            #region Alignment

                            excWsheet.Cells[counter, 3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 4].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 5].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                            excWsheet.Cells[counter, 6].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 7].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 8].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 9].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 10].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 11].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 12].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                            excWsheet.Cells[counter, 13].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 14].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 15].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 16].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                            excWsheet.Cells[counter, 17].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 18].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 19].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 20].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                            excWsheet.Cells[counter, 21].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 22].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 23].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 24].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                            excWsheet.Cells[counter, 25].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 26].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 27].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 28].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                            excWsheet.Cells[counter, 29].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 30].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 31].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 32].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                            excWsheet.Cells[counter, 33].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 34].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 35].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 36].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                            excWsheet.Cells[counter, 37].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 38].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 39].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 40].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 41].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 42].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 43].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                            excWsheet.Cells[counter, 44].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                            excWsheet.Cells[counter, 45].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;

                            #endregion

                            excWsheet.Cells[counter, 0].Style.WrapText = true;
                            excWsheet.Cells[counter, 1].Style.WrapText = true;
                            excWsheet.Cells[counter, 2].Style.WrapText = true;
                            excWsheet.Cells[counter, 43].Style.WrapText = true;
                            excWsheet.Cells[counter, 44].Style.WrapText = true;


                            excWsheet.Cells[counter, 0].Value = supplierDetailQuotePart.CustomerPartNo;
                            excWsheet.Cells[counter, 1].Value = supplierDetailQuotePart.PartDescription;
                            excWsheet.Cells[counter, 2].Value = supplierDetailQuotePart.AdditionalPartDesc;
                            excWsheet.Cells[counter, 3].Value = supplierDetailQuotePart.EstimatedQty;
                            excWsheet.Cells[counter, 6].Value = supplierDetailQuotePart.MaterialType;
                            excWsheet.Cells[counter, 9].Value = supplierDetailQuotePart.PartWeightKG.HasValue ? supplierDetailQuotePart.PartWeightKG.Value.ToString("0.000") : string.Empty;
                            #region Drop Down Set

                            DataValidation dv;
                            /*Manufacturer List*/
                            cr = excWsheet.Cells.GetSubrange("E" + (counter + 1), "E" + (counter + 1));
                            cr.Merged = true;
                            dv = new DataValidation(cr);

                            List<MES.Data.Library.SearchSuppliersQuotedNotQuoted_Result> manufacturerList = this.DataContext.SearchSuppliersQuotedNotQuoted(criteria.RFQId).ToList();
                            int startRow = 2, startCol = 2;
                            excWsheet2.Cells[0, startCol].Value = "Manufacturer(s)";
                            excWsheet2.Cells[0, startCol].Style.Font.Weight = ExcelFont.BoldWeight;
                            foreach (var listItem in manufacturerList)
                            {
                                excWsheet2.Cells[startRow, startCol].Value = listItem.CompanyName.ToString().Trim();
                                startRow++;
                            }

                            dv.Type = DataValidationType.List;
                            dv.InputMessage = "Select Manufacturer";
                            dv.Formula1 = "='ManufacturerList'!C2:C" + startRow;
                            dv.ShowErrorAlert = true;
                            excWsheet.DataValidations.Add(dv);
                            cr.Merged = false;
                            cr = null;
                            excWsheet2.Columns[2].AutoFit();
                            /*end here*/

                            /*Machine Description*/
                            cr = excWsheet.Cells.GetSubrange("N" + (counter + 1), "N" + (counter + 1));
                            cr.Merged = true;
                            dv = new DataValidation(cr);
                            dv.Type = DataValidationType.List;
                            dv.InputMessage = "Select Machine Description";
                            dv.Formula1 = ppcMachineDescriptionList;
                            dv.ShowErrorAlert = false;
                            excWsheet.DataValidations.Add(dv);
                            cr.Merged = false;
                            cr = null;
                            /*end here*/
                            /*Machining Description*/
                            cr = excWsheet.Cells.GetSubrange("S" + (counter + 1), "S" + (counter + 1));
                            cr.Merged = true;
                            dv = new DataValidation(cr);
                            dv.Type = DataValidationType.List;
                            dv.InputMessage = "Select Machining Description";
                            dv.Formula1 = mMachiningDescriptionList;
                            dv.ShowErrorAlert = false;
                            excWsheet.DataValidations.Add(dv);
                            cr.Merged = false;
                            cr = null;
                            /*end here*/
                            /*Secondary Operation*/
                            cr = excWsheet.Cells.GetSubrange("W" + (counter + 1), "W" + (counter + 1));
                            cr.Merged = true;
                            dv = new DataValidation(cr);
                            dv.Type = DataValidationType.List;
                            dv.InputMessage = "Select Secondary Operation Description";
                            dv.Formula1 = msoSecOprDescriptionList;
                            dv.ShowErrorAlert = false;
                            excWsheet.DataValidations.Add(dv);
                            cr.Merged = false;
                            cr = null;
                            /*end here*/
                            /*Secondary Operation Description*/
                            cr = excWsheet.Cells.GetSubrange("AA" + (counter + 1), "AA" + (counter + 1));
                            cr.Merged = true;
                            dv = new DataValidation(cr);
                            dv.Type = DataValidationType.List;
                            dv.InputMessage = "Select Secondary Operation Description";
                            dv.Formula1 = msoSecOprDescriptionList;
                            dv.ShowErrorAlert = false;
                            excWsheet.DataValidations.Add(dv);
                            cr.Merged = false;
                            cr = null;
                            /*end here*/
                            /*Coating Type*/
                            cr = excWsheet.Cells.GetSubrange("AE" + (counter + 1), "AE" + (counter + 1));
                            cr.Merged = true;
                            dv = new DataValidation(cr);
                            dv.Type = DataValidationType.List;
                            dv.InputMessage = "Select Coating Type";
                            dv.Formula1 = stCoatingTypeList;
                            dv.ShowErrorAlert = false;
                            dv.IgnoreBlank = true;
                            excWsheet.DataValidations.Add(dv);
                            cr.Merged = false;
                            cr = null;
                            /*end here*/
                            #endregion

                            /*I.A Raw Mat'l TOTAL
                            * [Raw Mat'l TOTAL] = [Raw Mat'l Input (Gross wt in kg] * [Raw Mat'l Cost (per kg)]
                            * =(PRODUCT(H21,J21))
                            */
                            /*I.B Example: Input = 1kg  Raw Mat’l Cost=2.15/kg
                                     1x2.15= $2.15.  Loss would be calculation of Input wt. – RFQ wt./Input Wt.
                            */
                            excWsheet.Cells[counter, 11].Formula = "=IF(I" + (counter + 1) + ">0,IFERROR((I" + (counter + 1) + "-J" + (counter + 1) + ")/I" + (counter + 1) + ",1),0)";

                            string rawMatFormula = "=ROUND(PRODUCT(I" + (counter + 1) + ", K" + (counter + 1) + "),3)";
                            excWsheet.Cells[counter, 12].Formula = rawMatFormula;


                            /*II. Process / Conversion Cost / Part
                             * [Process / Conversion Cost / Part] =[Cycle Time (sec)] * ([Man + Machine Rate/Hour] / 3600)
                             * =(PRODUCT(P21,(Q21/3600))) 
                             */
                            excWsheet.Cells[counter, 17].Formula = "=ROUND((PRODUCT(P" + (counter + 1) + ",(Q" + (counter + 1) + "/3600))), 3)";

                            /*III. Machining Cost / Part
                             * [Machining Cost / Part]=[Cycle Time (sec) ]* ([Man + Machine Rate/Hour] / 3600)
                             * =(PRODUCT(T21,(U21/3600))) 
                             */
                            excWsheet.Cells[counter, 21].Formula = "=ROUND((PRODUCT(T" + (counter + 1) + ",(U" + (counter + 1) + "/3600))), 3)";

                            /*IV. Machining 2/Secondary Operation
                             * [Secondary Cost/Part]= [Cycle Time (sec)] * ([Man + Machine Rate/Hour] / 3600)
                             * =(PRODUCT(X21,(Y21/3600)))
                             */
                            excWsheet.Cells[counter, 25].Formula = "=ROUND((PRODUCT(X" + (counter + 1) + ",(Y" + (counter + 1) + "/3600))), 3)";

                            /*V. Machining 3/Secondary Operation
                             * [Secondary Cost/Part]= [Cycle Time (sec)] * ([Man + Machine Rate/Hour] / 3600)
                             * =(PRODUCT(AB21,(AC21/3600)))
                             */
                            excWsheet.Cells[counter, 29].Formula = "=ROUND((PRODUCT(AB" + (counter + 1) + ",(AC" + (counter + 1) + "/3600))), 3)";

                            /*VI. Surface Treatment Coating / Painting Cost Per Hour
                             * [Coating / Painting Cost per Hour] =[Man + Machine Rate/Hour] / [Parts Per Hour]
                             * =(AG21/AF21)
                             */
                            excWsheet.Cells[counter, 33].Formula = "=IF(OR(ISBLANK(AF" + (counter + 1) + "), ISBLANK(AG" + (counter + 1) + "),AF" + (counter + 1) + "=0,AG" + (counter + 1) + "=0), \"0\", ROUND((AF" + (counter + 1) + "/AG" + (counter + 1) + "), 3))";

                            /*IX. Final Piece Price / Part=
                                (
                                  [Raw Mat'l TOTAL]
                                + [Process / Conversion Cost / Part]
                                + [Machining Cost / Part]
                                + [Machining 2/Secondary Operation Secondary Cost/Part]
                                + [Machining 3/Other Operation Secondary Cost/Part]
                                + [Coating / Painting Cost per Hour]
                                + [Inventory Carrying Cost (If Applicable)]
                                + [Packing]
                                + [Local Freight to Port]
                                + [Profit and S, G & A]
                                )  
                             * 
                             */
                            excWsheet.Cells[counter, 40].Formula = "=ROUND((SUM(M" + (counter + 1) + ", R" + (counter + 1) + ", V" + (counter + 1) + ", Z" + (counter + 1) + ", AD" + (counter + 1) + ", AF" + (counter + 1) + ", AI" + (counter + 1) + ", AJ" + (counter + 1) + ", AK" + (counter + 1) + ", AL" + (counter + 1) + ", AM" + (counter + 1) + ")), 3)";

                            excWsheet.Cells[counter, 39].Formula = "=IF(AND(OR(ISBLANK(AI" + (counter + 1) + "),AI" + (counter + 1) + "=0), OR(ISBLANK(AJ" + (counter + 1) + "),AJ" + (counter + 1) + "=0), OR(ISBLANK(AK" + (counter + 1) + "),AK" + (counter + 1) + "=0),OR(ISBLANK(AL" + (counter + 1) + "),AL" + (counter + 1) + "=0),OR(ISBLANK(AM" + (counter + 1) + "),AM" + (counter + 1) + "=0),OR(ISBLANK(AO" + (counter + 1) + " ),AO" + (counter + 1) + "=0)), \"0\", ROUND((SUM(AI" + (counter + 1) + ", AJ" + (counter + 1) + ", AK" + (counter + 1) + ", AL" + (counter + 1) + ", AM" + (counter + 1) + ") * 100 /(AO" + (counter + 1) + " )), 2))";

                            #region Formula Validation

                            /*Surface Treatment Validation Start*/
                            string stValidation = "IF(";
                            stValidation += "AND(ISBLANK(AF" + (counter + 1) + "), ISBLANK(AG" + (counter + 1) + ")), \"\" ";
                            stValidation += ", IF(";
                            stValidation += "OR(ISBLANK(AF" + (counter + 1) + "), ISBLANK(AG" + (counter + 1) + ")), \"Please enter Surface Treatment Parts Per Hour and Man + Machine Rate/hour \" ";
                            stValidation += ", \"\" ))";
                            /*Surface Treatment Validation End*/

                            /*Machining Other Operation Validation Start*/
                            string mooValidation = "IF(";
                            mooValidation += "AND(ISBLANK(AB" + (counter + 1) + "), ISBLANK(AC" + (counter + 1) + ")), " + stValidation + " ";
                            mooValidation += ", IF(";
                            mooValidation += "OR(ISBLANK(AB" + (counter + 1) + "), ISBLANK(AC" + (counter + 1) + ")), \"Please enter Machining Other Operation Cycle Time(sec) and Man + Machine Rate/hour \" ";
                            mooValidation += ", " + stValidation + " ))";
                            /*Machining Other Operation Validation End*/

                            /*Machining Secondary Operation Validation Start*/
                            string msoValidation = "IF(";
                            msoValidation += "AND(ISBLANK(X" + (counter + 1) + "), ISBLANK(Y" + (counter + 1) + ")), " + mooValidation + " ";
                            msoValidation += ", IF(";
                            msoValidation += "OR(ISBLANK(X" + (counter + 1) + "), ISBLANK(Y" + (counter + 1) + ")), \"Please enter Machining Secondary Operation Cycle Time(sec) and Man + Machine Rate/hour \" ";
                            msoValidation += ", " + mooValidation + " ))";
                            /*Machining Secondary Operation Validation End*/

                            /*Machining Validation Start*/
                            string machiningValidation = "IF(";
                            machiningValidation += "AND(ISBLANK(T" + (counter + 1) + "), ISBLANK(U" + (counter + 1) + ")), " + msoValidation + " ";
                            machiningValidation += ", IF(";
                            machiningValidation += "OR(ISBLANK(T" + (counter + 1) + "), ISBLANK(U" + (counter + 1) + ")), \"Please enter Machining Cycle Time(sec) and Man + Machine Rate/hour \" ";
                            machiningValidation += ", " + msoValidation + " ))";
                            /*Machining Validation End*/

                            string formulaValidation = "=IF(";

                            formulaValidation += "AND(";

                            formulaValidation += "OR(ISBLANK(F" + (counter + 1) + "),(F" + (counter + 1) + ")=0), OR(ISBLANK(AP" + (counter + 1) + "),(AP" + (counter + 1) + ")=0)";
                            formulaValidation += ", OR(ISBLANK(I" + (counter + 1) + "),(I" + (counter + 1) + ")=0), OR(ISBLANK(K" + (counter + 1) + "),(K" + (counter + 1) + ")=0)";
                            formulaValidation += ", OR(ISBLANK(N" + (counter + 1) + "),(N" + (counter + 1) + ")=0), OR(ISBLANK(O" + (counter + 1) + "),(O" + (counter + 1) + ")=0), OR(ISBLANK(P" + (counter + 1) + "),(P" + (counter + 1) + ")=0), OR(ISBLANK(Q" + (counter + 1) + "),(Q" + (counter + 1) + ")=0)";
                            formulaValidation += ", OR(ISBLANK(T" + (counter + 1) + "),(T" + (counter + 1) + ")=0), OR(ISBLANK(U" + (counter + 1) + "),(U" + (counter + 1) + ")=0)";
                            formulaValidation += ", OR(ISBLANK(X" + (counter + 1) + "),(X" + (counter + 1) + ")=0), OR(ISBLANK(Y" + (counter + 1) + "),(Y" + (counter + 1) + "=0))";
                            formulaValidation += ", OR(ISBLANK(AB" + (counter + 1) + "),(AB" + (counter + 1) + ")=0), OR(ISBLANK(AC" + (counter + 1) + "),(AC" + (counter + 1) + ")=0)";
                            formulaValidation += ", OR(ISBLANK(AF" + (counter + 1) + "),(AF" + (counter + 1) + ")=0), OR(ISBLANK(AG" + (counter + 1) + "),(AG" + (counter + 1) + ")=0)";
                            formulaValidation += ", OR(ISBLANK(AI" + (counter + 1) + "),(AI" + (counter + 1) + ")=0), OR(ISBLANK(AJ" + (counter + 1) + "),(AJ" + (counter + 1) + ")=0), OR(ISBLANK(AK" + (counter + 1) + "),(AK" + (counter + 1) + ")=0), OR(ISBLANK(AL" + (counter + 1) + "),(AL" + (counter + 1) + ")=0), OR(ISBLANK(AM" + (counter + 1) + "),(AM" + (counter + 1) + ")=0)";
                            formulaValidation += "), \"\" ";

                            formulaValidation += ", IF(";
                            formulaValidation += "OR(";
                            formulaValidation += "ISTEXT(F" + (counter + 1) + "), ISTEXT(AP" + (counter + 1) + ")";
                            formulaValidation += ", ISTEXT(I" + (counter + 1) + "), ISTEXT(K" + (counter + 1) + ")";
                            formulaValidation += ", ISTEXT(O" + (counter + 1) + "), ISTEXT(O" + (counter + 1) + "), ISTEXT(P" + (counter + 1) + ")";
                            formulaValidation += ", ISTEXT(T" + (counter + 1) + "), ISTEXT(U" + (counter + 1) + ")";
                            formulaValidation += ", ISTEXT(X" + (counter + 1) + "), ISTEXT(Y" + (counter + 1) + ")";
                            formulaValidation += ", ISTEXT(AB" + (counter + 1) + "), ISTEXT(AC" + (counter + 1) + ")";
                            formulaValidation += ", ISTEXT(AF" + (counter + 1) + "), ISTEXT(AG" + (counter + 1) + ")";
                            formulaValidation += ", ISTEXT(AI" + (counter + 1) + "), ISTEXT(AJ" + (counter + 1) + "), ISTEXT(AK" + (counter + 1) + "), ISTEXT(AL" + (counter + 1) + "), ISTEXT(AM" + (counter + 1) + ")";
                            formulaValidation += "), \"Please enter numeric or decimal values.\" ";

                            formulaValidation += ", IF(";
                            formulaValidation += "OR(";
                            formulaValidation += "ISBLANK(F" + (counter + 1) + "), ISBLANK(AP" + (counter + 1) + "), (AP" + (counter + 1) + "=0)";
                            formulaValidation += ", ISBLANK(I" + (counter + 1) + "), (I" + (counter + 1) + "=0), ISBLANK(K" + (counter + 1) + "), (K" + (counter + 1) + "=0)";
                            formulaValidation += ", ISBLANK(N" + (counter + 1) + "), ISBLANK(O" + (counter + 1) + "), (O" + (counter + 1) + "=0), ISBLANK(P" + (counter + 1) + "), (P" + (counter + 1) + "=0), ISBLANK(Q" + (counter + 1) + "), (Q" + (counter + 1) + "=0)";
                            formulaValidation += "), \"Values of Min. Order Qty, No. Of Cavities, Raw Mat'l Input, Raw Mat'l Cost, Machine Description, Machine Size, Cycle Time  & Man + Machine Rate/Hour are mandatory.\" ";

                            formulaValidation += ", " + machiningValidation + " )))";

                            #endregion

                            excWsheet.Cells[counter, 45].Formula = formulaValidation;

                            counter++;
                        }
                    }

                    //** start ** Unprotect the sheet and lock all the cells
                    //Unlock the editable cells
                    
                    excWsheet.Cells[13, 1].Style.Locked = false;
                    excWsheet.Cells[14, 1].Style.Locked = false;
                    excWsheet.Cells[15, 1].Style.Locked = false;
                    excWsheet.Cells[16, 1].Style.Locked = false;
                    excWsheet.Cells[17, 1].Style.Locked = false;
                    //** end **

                    excWsheet.Cells[4, 0].Value = "P O BOX 401";
                    excWsheet.Cells[5, 0].Value = "Lewis Center, OH 43035";
                    excWsheet.Cells[7, 0].Value = "VENDOR NAME:";
                    excWsheet.Cells[7, 1].Value = supplierItem.CompanyName.ToUpper();

                    excWsheet.Cells[8, 0].Value = "RFQ # ";
                    excWsheet.Cells[8, 1].Value = criteria.RFQId;

                    RFQ rfqObj = new RFQ();
                    DTO.Library.RFQ.RFQ.RFQ rfqInfo = rfqObj.FindById(criteria.RFQId).Result;

                    excWsheet.Cells[9, 0].Value = "Supplier Requirement";
                    excWsheet.Cells[9, 1].Value = rfqInfo.SupplierRequirement;

                    excWsheet.Cells[10, 0].Value = "Remarks";
                    excWsheet.Cells[10, 1].Value = rfqInfo.Remarks;

                    excWsheet.Cells[12, 0].Value = "Supplier Quote";

                    excWsheet.Cells[14, 0].Value = "Exchange Rate 1 USD =";
                    excWsheet.Cells[14, 2].Formula = "=IF(ISBLANK(B15),\"Value for Exchange Rate is mandatory.\",IF(ISTEXT(B15),\"Please enter numeric values.\",\"\"))";
                    excWsheet.Cells[14, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;

                    cr = excWsheet.Cells.GetSubrange("B15", "B15");
                    cr.Merged = true;
                    excWsheet.DataValidations.Add(new DataValidation(cr)
                    {
                        Type = DataValidationType.Decimal,
                        Operator = DataValidationOperator.GreaterThan,
                        Formula1 = 0,
                        ErrorMessage = "Please enter numeric values.",
                        ErrorStyle = DataValidationErrorStyle.Stop
                    });
                    cr.Merged = false;
                    cr = null;

                    excWsheet.Cells[15, 0].Value = "Currency =";
                    excWsheet.Cells[15, 2].Formula = "=IF(ISBLANK(B16),\"Value for Currency is mandatory.\",\"\")";
                    excWsheet.Cells[15, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;

                    excWsheet.Cells[16, 0].Value = "Raw Material Price Assumed \n(Please include unit of measure…i.e. $1.000/kg) =";
                    excWsheet.Cells[16, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;

                    excWsheet.Cells[17, 0].Value = "Supplier Remarks";

                    excWsheet.Cells[19, 0].Value = "Part(s) List";
                    excWsheet.Cells[20, 0].Value = "Part Detail";
                    excWsheet.Cells[20, 6].Value = "Raw Material";
                    excWsheet.Cells[20, 13].Value = "Primary Process/Conversion";
                    excWsheet.Cells[20, 18].Value = "Machining";
                    excWsheet.Cells[20, 22].Value = "Machining 2/Secondary Operation";
                    excWsheet.Cells[20, 26].Value = "Machining 3/Other Operation";
                    excWsheet.Cells[20, 30].Value = "Surface Treatment";
                    excWsheet.Cells[20, 34].Value = "Overhead";
                    excWsheet.Cells[20, 41].Value = "Tooling";

                    excWsheet.Cells[21, 0].Value = "Part No.";
                    excWsheet.Cells[21, 1].Value = "Description";
                    excWsheet.Cells[21, 2].Value = "Additional Description";
                    excWsheet.Cells[21, 3].Value = "Annual Quantity";
                    excWsheet.Cells[21, 4].Value = "Manufacturer";
                    excWsheet.Cells[21, 5].Value = "Min. Order Qty";

                    excWsheet.Cells[21, 6].Value = "Raw Material Description";
                    excWsheet.Cells[21, 7].Value = "Raw Material Index Used";
                    excWsheet.Cells[21, 8].Value = "Raw Mat'l Input (Gross wt in kg)";
                    excWsheet.Cells[21, 9].Value = "Part Weight (kg)";
                    excWsheet.Cells[21, 10].Value = "Raw Mat'l Cost (USD)(per kg)";
                    excWsheet.Cells[21, 11].Value = "Material Loss";
                    excWsheet.Cells[21, 12].Value = "Raw Mat'l TOTAL";

                    excWsheet.Cells[21, 13].Value = "Machine Description";
                    excWsheet.Cells[21, 14].Value = "Machine Size (Tons)";
                    excWsheet.Cells[21, 15].Value = "Cycle Time (sec)";
                    excWsheet.Cells[21, 16].Value = "Man + Machine Rate/Hour";
                    excWsheet.Cells[21, 17].Value = "Process/Conversion Cost/Part";

                    excWsheet.Cells[21, 18].Value = "Machining Description";
                    excWsheet.Cells[21, 19].Value = "Cycle Time (sec)";
                    excWsheet.Cells[21, 20].Value = "Man + Machine Rate/Hour";
                    excWsheet.Cells[21, 21].Value = "Machining Cost/Part";

                    excWsheet.Cells[21, 22].Value = "Secondary Operation Description";
                    excWsheet.Cells[21, 23].Value = "Cycle Time (sec)";
                    excWsheet.Cells[21, 24].Value = "Man + Machine Rate/Hour";
                    excWsheet.Cells[21, 25].Value = "Secondary Cost/Part";

                    excWsheet.Cells[21, 26].Value = "Secondary Operation Description";
                    excWsheet.Cells[21, 27].Value = "Cycle Time (sec)";
                    excWsheet.Cells[21, 28].Value = "Man + Machine Rate/Hour";
                    excWsheet.Cells[21, 29].Value = "Secondary Cost/Part";

                    excWsheet.Cells[21, 30].Value = "Coating Type";
                    excWsheet.Cells[21, 31].Value = "Parts Per Hour";
                    excWsheet.Cells[21, 32].Value = "Man + Machine Rate/Hour";
                    excWsheet.Cells[21, 33].Value = "Coating / Painting Cost Per Hour";

                    excWsheet.Cells[21, 34].Value = "Inventory Carrying Cost (if applicable)";
                    excWsheet.Cells[21, 35].Value = "Packaging Material";
                    excWsheet.Cells[21, 36].Value = "Packing Labour";
                    excWsheet.Cells[21, 37].Value = "FOB Port(Shipping Cost Per PC.)";
                    excWsheet.Cells[21, 38].Value = "Profit and S, G & A";
                    excWsheet.Cells[21, 39].Value = "Overhead as % of Piece Price";
                    excWsheet.Cells[21, 40].Value = "Final Piece Price / Part";

                    excWsheet.Cells[21, 41].Value = "No. Of Cavities";
                    excWsheet.Cells[21, 42].Value = "Tooling Cost";
                    excWsheet.Cells[21, 43].Value = "Tooling Warranty";
                    excWsheet.Cells[21, 44].Value = "Supplier Tooling Leadtime";

                    excWsheet.Columns[1].AutoFit();

                    for (int index = 2; index <= 44; index++)
                    {
                        excWsheet.Columns[index].Width = 20 * 10 * 25;
                    }

                    excWsheet.Columns[45].Width = 20 * 30 * 40;

                    //MES Logo
                    cr = excWsheet.Cells.GetSubrange("A1", "AS4"); cr.Merged = true;
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.White, System.Drawing.Color.White);
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin); cr = null;
                    excWsheet.Pictures.Add(context.Server.MapPath(Constants.IMAGEFOLDER) + "MESlogoPdf.png", GemBox.Spreadsheet.PositioningMode.FreeFloating, new AnchorCell(excWsheet.Columns[0], excWsheet.Rows[0], true), new AnchorCell(excWsheet.Columns[0], excWsheet.Rows[3], false));
                    //ends here

                    excWsheet.ProtectionSettings.PasswordHash = 1;
                    excWsheet.Protected = true;
                    excWsheet2.ProtectionSettings.PasswordHash = 1;
                    excWsheet2.Protected = true;

                    string generatedFileName = supplierItem.CompanyName + "_" + criteria.RFQId + "_" + System.DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx";
                    string tempFilepath = context.Server.MapPath("~") + Constants.REPORTSTEMPLATEFILEPATH + generatedFileName;
                    if (!System.IO.Directory.Exists(context.Server.MapPath(@"~") + (Constants.REPORTSTEMPLATEFILEPATH)))
                        System.IO.Directory.CreateDirectory(context.Server.MapPath(@"~") + (Constants.REPORTSTEMPLATEFILEPATH));
                    else
                    {
                        if (File.Exists(tempFilepath))
                            File.Delete(tempFilepath);
                    }
                    myExcelFile.SaveXlsx(tempFilepath);

                    filePath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"])
                         + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                         + Constants.TEMPFILESEMAILATTACHMENTSFOLDER
                         + generatedFileName;

                    Helper.BlobHelper.DeleteBlobfile(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), filePath);

                    Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                                 , Constants.TEMPFILESEMAILATTACHMENTSFOLDERName
                                 , generatedFileName
                                 , tempFilepath);
                    File.Delete(tempFilepath);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    excWsheet = null;
                    myExcelFile.ClosePreservedXlsx();
                }
            }
            catch (Exception ex)//Error
            {
                throw ex;
            }
            return filePath;
        }
        #endregion

        public ITypedResponse<DTO.Library.RFQ.RFQ.RFQ> GetRFQPartCostComparisonList(string rfqId)
        {
            string errMSg = null;
            DTO.Library.RFQ.RFQ.RFQ rFQDetails = new DTO.Library.RFQ.RFQ.RFQ();
            List<DTO.Library.RFQ.RFQ.RFQPartCostComparision> lstrFQPartCostComparison = new List<DTO.Library.RFQ.RFQ.RFQPartCostComparision>();
            MES.DTO.Library.RFQ.RFQ.RFQPartCostComparision rFQPartCostComparison = null;
            List<DTO.Library.RFQ.RFQ.RFQParts> lstRFQPart = null;
            DTO.Library.RFQ.RFQ.RFQParts rfqPartItem = null;
            List<DTO.Library.RFQ.RFQ.RFQSuppliers> lstrfqSuppliers = new List<DTO.Library.RFQ.RFQ.RFQSuppliers>();

            rFQDetails.Id = rfqId;
            int i = 0;
            this.RunOnDB(context =>
            {
                MES.Business.Library.BO.RFQ.RFQ.RFQSuppliers rfqSuppliersObj = new MES.Business.Library.BO.RFQ.RFQ.RFQSuppliers();
                rFQDetails.lstQuotedSuppliers = rfqSuppliersObj.GetRFQSuppliers(rfqId).Result.Where(item => item.QuoteDate != null).ToList();

                int sId = 0;
                sId = rFQDetails.lstQuotedSuppliers[0].SupplierId;
                while (i < rFQDetails.lstQuotedSuppliers.Count)
                {
                    lstRFQPart = new List<DTO.Library.RFQ.RFQ.RFQParts>();
                    var rfqPartsList = context.Parts.Where(p => p.RFQId == rfqId && p.IsDeleted == false).ToList();
                    if (rfqPartsList != null)
                    {
                        foreach (var item in rfqPartsList)
                        {
                            rfqPartItem = new DTO.Library.RFQ.RFQ.RFQParts();
                            rfqPartItem.Id = item.Id;
                            rfqPartItem.CustomerPartNo = item.CustomerPartNo;
                            rfqPartItem.PartWeightKG = item.PartWeightKG;
                            rfqPartItem.EstimatedQty = item.EstimatedQty;
                            rfqPartItem.lstRFQPartCostComparison = new List<DTO.Library.RFQ.RFQ.RFQPartCostComparision>();

                            foreach (var sItem in rFQDetails.lstQuotedSuppliers)
                            {
                                var rFQPartCostComparisonlst = context.GetRfqPartCostingComparisons(rfqId, rfqPartItem.Id, sItem.SupplierId).ToList();
                                if (rFQPartCostComparisonlst != null && rFQPartCostComparisonlst.Count != 0)
                                {
                                    foreach (var cItem in rFQPartCostComparisonlst)
                                    {
                                        rFQPartCostComparison = new DTO.Library.RFQ.RFQ.RFQPartCostComparision();
                                        rFQPartCostComparison.UpdatedDate = cItem.UpdatedDate;
                                        rFQPartCostComparison.ToolingCost = cItem.ToolingCost;
                                        rFQPartCostComparison.PiecePrice = cItem.UnitPrice.HasValue ? cItem.UnitPrice.Value : 0;
                                        rFQPartCostComparison.SupplierCostPerKg = cItem.SupplierCostPerKg.HasValue ? cItem.SupplierCostPerKg.Value : 0;
                                        rFQPartCostComparison.SupplierId = sItem.SupplierId;
                                        rFQPartCostComparison.rfqPartId = rfqPartItem.Id.Value;
                                        if (sId == sItem.SupplierId)
                                        {
                                            rFQPartCostComparison.rdoSelect = true;
                                            rFQPartCostComparison.rdoSelectValue = rFQPartCostComparison.SupplierId + "_" + rFQPartCostComparison.rfqPartId;
                                        }
                                        else
                                            rFQPartCostComparison.rdoSelect = false;

                                        rfqPartItem.lstRFQPartCostComparison.Add(rFQPartCostComparison);
                                    }
                                }
                            }

                            lstRFQPart.Add(rfqPartItem);

                        }
                        rFQDetails.lstRFQPart = lstRFQPart;
                    }

                    i++;
                }
            });

            var response = SuccessOrFailedResponse<MES.DTO.Library.RFQ.RFQ.RFQ>(errMSg, rFQDetails);

            return response;
        }
    }
}
