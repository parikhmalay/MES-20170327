using Account.DTO.Library;
using MES.Business.Repositories.RFQ.RFQ;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPE.Core.Extensions;
using NPE.Core;
using System.Data.Entity;
using System.Net.Mail;
using System.IO;
using System.Web;
using Ionic.Zip;
using MES.DTO.Library.Identity;
using MES.Business.Repositories.UserManagement;

namespace MES.Business.Library.BO.RFQ.RFQ
{
    class RFQSuppliers : ContextBusinessBase, IRFQSuppliersRepository
    {
        public RFQSuppliers()
            : base("RFQSuppliers")
        { }

        /// <summary>
        /// Gets the RFQ suppliers.
        /// </summary>
        /// <param name="paging">The paging.</param>
        /// <returns></returns>
        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQSuppliers>> GetRFQSuppliers(string rfqNo)
        {
            //set the out put param
            string errMSg = null;
            List<DTO.Library.RFQ.RFQ.RFQSuppliers> lstRFQSuppliers = new List<DTO.Library.RFQ.RFQ.RFQSuppliers>();
            DTO.Library.RFQ.RFQ.RFQSuppliers rFQSuppliers;

            this.RunOnDB(context =>
            {
                var rfqSupplierList = context.SearchRFQSuppliers(rfqNo).ToList();
                int totalRecords = rfqSupplierList.Count;
                if (rfqSupplierList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    foreach (var item in rfqSupplierList)
                    {
                        rFQSuppliers = new DTO.Library.RFQ.RFQ.RFQSuppliers();
                        rFQSuppliers.Id = item.Id;
                        rFQSuppliers.RFQId = item.RFQId;
                        rFQSuppliers.SupplierId = Convert.ToInt32(item.SupplierId);

                        rFQSuppliers.UniqueURL = item.UniqueURL;
                        rFQSuppliers.OriginalURL = item.OriginalURL;
                        rFQSuppliers.IsQuoteTypeDQ = item.IsQuoteTypeDQ;
                        rFQSuppliers.CompanyName = item.CompanyName;
                        rFQSuppliers.Email = item.Email;
                        rFQSuppliers.City = item.City;
                        rFQSuppliers.State = item.State;
                        rFQSuppliers.Country = item.Country;
                        rFQSuppliers.Website = item.Website;
                        rFQSuppliers.CompanyPhone1 = item.CompanyPhone1;
                        rFQSuppliers.QuoteDate = item.QuoteDate;
                        lstRFQSuppliers.Add(rFQSuppliers);
                    }
                }

            });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSuppliers>>(errMSg, lstRFQSuppliers);

            return response;
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.RFQ.RFQSuppliers> FindById(int id)
        {
            string errMSg = string.Empty;
            DTO.Library.RFQ.RFQ.RFQSuppliers rFQSuppliers = new DTO.Library.RFQ.RFQ.RFQSuppliers();
            this.RunOnDB(context =>
            {
                var rfqSupplierItem = context.rfqSuppliers.Where(r => r.Id == id).SingleOrDefault();
                if (rfqSupplierItem == null)
                    errMSg = Languages.GetResourceText("RFQSupplierNotExists");
                else
                {

                    rFQSuppliers = new DTO.Library.RFQ.RFQ.RFQSuppliers();
                    rFQSuppliers.Id = rfqSupplierItem.Id;
                    rFQSuppliers.RFQId = rfqSupplierItem.RFQId;
                    rFQSuppliers.SupplierId = Convert.ToInt32(rfqSupplierItem.SupplierId);

                    rFQSuppliers.UniqueURL = rfqSupplierItem.UniqueURL;
                    rFQSuppliers.OriginalURL = rfqSupplierItem.OriginalURL;
                    rFQSuppliers.IsQuoteTypeDQ = rfqSupplierItem.IsQuoteTypeDQ;
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.RFQ.RFQ.RFQSuppliers>(errMSg, rFQSuppliers);
            return response;

        }

        public NPE.Core.ITypedResponse<bool?> DeleteRFQSuppliers(int RFQSupplierId)
        {
            var RFQToBeDeleted = this.DataContext.rfqSuppliers.Where(a => a.Id == RFQSupplierId).SingleOrDefault();
            if (RFQToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("RFQSupplierNotExists"));
            }
            else
            {
                RFQSupplierPartQuote rspqObj = new RFQSupplierPartQuote();
                List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuote> lstrspq = rspqObj.GetRfqSupplierPartsQuote(RFQToBeDeleted.RFQId).Result;

                foreach (var item in lstrspq)
                {
                    rspqObj.Delete(item.Id);
                }

                RFQToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                RFQToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(RFQToBeDeleted).State = EntityState.Modified;
                RFQToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("RFQSupplierDeletedSuccess"));
            }
        }


        /// <summary>
        /// Send Email To RFQ suppliers from Send Email Popup.
        /// </summary>
        /// <param name="paging">The paging.</param>
        /// <returns></returns>
        public NPE.Core.ITypedResponse<bool?> SendEmail(MES.DTO.Library.Common.EmailData emailData)
        {
            bool IsSuccess = false;
            try
            {
                string rfqId = emailData.RFQId;

                MES.Business.Repositories.Email.IEmailRepository emailRepository = new MES.Business.Library.BO.Email.Email();
                List<Attachment> attachments = new List<Attachment>();
                List<string> lstToAddress = new List<string>();
                //string emailFrom = new System.Net.Mail.MailAddress(UserManagement.UserManagement CurrentUserInfo.GetEmail);

                string footer = @"<br /><br /><hr />
                                    <span style='color: #FF0000; font-size: 10px; font-family: Tahoma, Geneva, sans-serif;' align='left'> 
                                        Electronic Privacy Notice & Email Disclaimer:
                                    </span><br />
                                    <span style='color: #626465; font-size: 10px; font-family: Tahoma, Geneva, sans-serif;' align='left'>  
                                        This email, and any attachments, contains information that is, or may be, covered by electronic communications privacy laws,
                                        and is also confidential and proprietary in nature. If you are not the intended recipient, please be advised that you are 
                                        legally prohibited from retaining, using, copying, distributing, or otherwise disclosing this information in any manner.
                                        Instead, please reply to the sender that you have received this communication in error and then immediately delete it. 
                                        MES Inc. is not liable for any damage caused by any virus transmitted by this email. Thank you for your co operation.
                                    </span>";


                //set attachments
                //If IsTrue, attach RFQ PDF wiith the email
                if (emailData.AttachRFQPDF.HasValue && emailData.AttachRFQPDF.Value)
                {
                    string rfqFileName = "Rfq_" + rfqId + ".pdf";
                    string filepath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"])
                                + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                                + Constants.RFQFILESFOLDER
                                + rfqFileName;

                    if (Helper.BlobHelper.CheckFileExists(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                     , filepath))
                    {
                        Stream memoryStream = MES.Business.Library.Helper.BlobHelper.AttachFile_ConvertBlobToStream(filepath);
                        attachments.Add(new System.Net.Mail.Attachment(memoryStream, filepath));
                    }
                }

                if (emailData.lstEmailDocumentAttachment != null && emailData.lstEmailDocumentAttachment.Count > 0)
                {
                    foreach (var item in emailData.lstEmailDocumentAttachment)
                    {
                        if (!string.IsNullOrEmpty(item.FilePath) && !string.IsNullOrEmpty(item.FileName))
                        {
                            Stream memoryStream = MES.Business.Library.Helper.BlobHelper.AttachFile_ConvertBlobToStream(item.FilePath);
                            attachments.Add(new System.Net.Mail.Attachment(memoryStream, item.FileName));
                        }
                    }
                }

                foreach (string emailId in emailData.EmailIdsList)
                {
                    lstToAddress.Add(emailId);
                }

                //20170222:  MESHA-43
                List<string> lstCCEmail = new List<string>();
                IUserManagementRepository userObj = new UserManagement.UserManagement();
                LoginUser usrInfo = userObj.GetCurrentUserInfo().Result;
                if (!string.IsNullOrEmpty(usrInfo.Email))
                    lstCCEmail.Add(usrInfo.Email);

                emailRepository.SendEmail(lstToAddress, "", emailData.EmailSubject, emailData.EmailBody + footer, out IsSuccess, attachments, lstCCEmail);

                /** Send Copy of Email To Carol 28-Mar-2013**/
                /**change Carol to messupplierquote@mesinc.net 20150619**/
                if (IsSuccess)
                {
                    lstToAddress = new List<string>();
                    lstToAddress.Add(System.Configuration.ConfigurationManager.AppSettings["RFQEmailCopiesTo"]);

                    string SuppliersList = string.Empty
                    , emailid = string.Empty
                    , emailBody = string.Empty
                    , downloadText = string.Empty;
                    Supplier.Suppliers supplierObj;
                    foreach (string id in emailData.Ids)
                    {
                        supplierObj = new Supplier.Suppliers();
                        DTO.Library.RFQ.Supplier.Suppliers supplierItem = supplierObj.FindById(Convert.ToInt32(id)).Result;
                        MES.DTO.Library.RFQ.Supplier.Contacts supplierContact = supplierItem.lstContact.Where(item => item.IsDefault == true).FirstOrDefault();

                        SuppliersList += "<tr><td align='left' style='font-family: Tahoma, Geneva, sans-serif;color: #626465;font-size: 11px;'>"
                                   + supplierItem.CompanyName
                                   + "</td><td align='left' style='font-family: Tahoma, Geneva, sans-serif;color: #626465;font-size: 11px;'>"
                                   + supplierContact.FirstName + " " + supplierContact.LastName
                                   + "</td></tr>";
                    }
                    supplierObj = null;

                    emailBody = GetRfqEmailBody("SupplierEmailCopy.htm")
                           .Replace("<%Name%>", System.Configuration.ConfigurationManager.AppSettings["RFQEmailCopiesToName"].Trim())
                           .Replace("<%RFQId%>", rfqId)
                           .Replace("<%SuppliersList%>", SuppliersList)
                           .Replace("<%Body%>", emailData.EmailBody);

                    emailRepository.SendEmail(lstToAddress, "", emailData.EmailSubject, emailBody + footer, out IsSuccess, attachments, null, null);
                    /**End Here**/
                }
            }
            catch (Exception ex)
            {
            }
            if (IsSuccess)
                return SuccessBoolResponse(Languages.GetResourceText("SendEmailSuccess"));
            else
                return SuccessBoolResponse(Languages.GetResourceText("SendEmailFail"));
        }

        /// <summary>
        /// Gets the RFQ email body.
        /// </summary>
        /// <returns></returns>
        private string GetRfqEmailBody(string fileName)
        {
            string templatePath = System.Web.HttpContext.Current.Server.MapPath("~/") + Constants.EmailTemplateFolder + fileName;
            string emailBody = string.Empty;
            using (StreamReader reader = new StreamReader(templatePath))
            {
                emailBody = reader.ReadToEnd();
                reader.Close();
            }
            return emailBody;

        }

        /// <summary>
        /// Gets the RFQ suppliers.
        /// </summary>
        /// <param name="paging">The paging.</param>
        /// <returns></returns>
        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQSuppliers>> GetRFQSuppliersList(NPE.Core.IPage<DTO.Library.RFQ.RFQ.RfqSupplierSearchCriteria> paging)
        {
            //set the out put param
            string errMSg = null;
            List<DTO.Library.RFQ.RFQ.RFQSuppliers> lstRFQSuppliers = new List<DTO.Library.RFQ.RFQ.RFQSuppliers>();
            DTO.Library.RFQ.RFQ.RFQSuppliers rFQSuppliers;

            this.RunOnDB(context =>
            {
                var rfqSupplierList = context.SearchRFQSuppliers(paging.Criteria.RFQId).ToList();
                int totalRecords = rfqSupplierList.Count;
                if (rfqSupplierList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    foreach (var item in rfqSupplierList)
                    {
                        rFQSuppliers = new DTO.Library.RFQ.RFQ.RFQSuppliers();
                        rFQSuppliers.Id = item.Id;
                        rFQSuppliers.RFQId = item.RFQId;
                        rFQSuppliers.SupplierId = Convert.ToInt32(item.SupplierId);

                        rFQSuppliers.UniqueURL = item.UniqueURL;
                        rFQSuppliers.OriginalURL = item.OriginalURL;
                        rFQSuppliers.IsQuoteTypeDQ = item.IsQuoteTypeDQ;
                        rFQSuppliers.CompanyName = item.CompanyName;
                        rFQSuppliers.Email = item.Email;
                        rFQSuppliers.City = item.City;
                        rFQSuppliers.State = item.State;
                        rFQSuppliers.Country = item.Country;
                        rFQSuppliers.Website = item.Website;
                        rFQSuppliers.CompanyPhone1 = item.CompanyPhone1;
                        rFQSuppliers.QuoteDate = item.QuoteDate;
                        lstRFQSuppliers.Add(rFQSuppliers);
                    }
                }
                paging.TotalRecords = totalRecords;
            });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSuppliers>>(errMSg, lstRFQSuppliers);
            response.PageInfo = paging.ToPage();
            return response;
        }

        /// <summary>
        /// Gets the available RFQ suppliers.
        /// </summary>
        /// <param name="paging">The paging.</param>
        /// <returns></returns>

        public ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQSuppliers>> GetAvailableSuppliersList(NPE.Core.IPage<DTO.Library.RFQ.RFQ.RfqSupplierSearchCriteria> paging)
        {
            //set the out put param
            string errMSg = null;
            List<DTO.Library.RFQ.RFQ.RFQSuppliers> lstRFQSuppliers = new List<DTO.Library.RFQ.RFQ.RFQSuppliers>();
            DTO.Library.RFQ.RFQ.RFQSuppliers rFQSuppliers;

            string countryIds = null, commodityItemIds = null;
            if (paging.Criteria.CountryIds != null)
            {
                countryIds = string.Empty;
                foreach (var countryItem in paging.Criteria.CountryIds)
                {
                    countryIds += countryItem.Id + ",";
                }
                countryIds = countryIds.Trim(',');
            }
            if (paging.Criteria.CommodityItemIds != null)
            {
                commodityItemIds = string.Empty;
                foreach (var commodityItem in paging.Criteria.CommodityItemIds)
                {
                    commodityItemIds += commodityItem.Id + ",";
                }
                commodityItemIds = commodityItemIds.Trim(',');
            }

            this.RunOnDB(context =>
            {
                var rfqSupplierList = context.SearchAvailableSuppliers(paging.Criteria.RFQId, countryIds, commodityItemIds, paging.Criteria.Supplier).ToList();
                if (rfqSupplierList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    foreach (var item in rfqSupplierList)
                    {
                        rFQSuppliers = new DTO.Library.RFQ.RFQ.RFQSuppliers();
                        rFQSuppliers.SupplierId = Convert.ToInt32(item.SupplierId);
                        rFQSuppliers.CompanyName = item.CompanyName;
                        rFQSuppliers.City = item.City;
                        rFQSuppliers.State = item.State;
                        rFQSuppliers.Country = item.Country;
                        rFQSuppliers.Website = item.Website;
                        rFQSuppliers.CompanyPhone1 = item.CompanyPhone1;
                        lstRFQSuppliers.Add(rFQSuppliers);
                    }
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSuppliers>>(errMSg, lstRFQSuppliers);
            return response;
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.RFQ.RFQSuppliers> Save(DTO.Library.RFQ.RFQ.RFQSuppliers rFQSuppliers)
        {
            string errMSg = null;
            string successMsg = null;
            string uniqueId = Convert.ToString(Guid.NewGuid());
            var recordToBeUpdated = new MES.Data.Library.rfqSupplier();
            if (rFQSuppliers.Id > 0)
            {
                recordToBeUpdated = this.DataContext.rfqSuppliers.Where(a => a.Id == rFQSuppliers.Id).SingleOrDefault();
                if (recordToBeUpdated == null)
                    errMSg = Languages.GetResourceText("RFQSupplierNotExists");
                else
                {
                    recordToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
                    recordToBeUpdated.UpdatedBy = CurrentUser;
                    this.DataContext.Entry(recordToBeUpdated).State = EntityState.Modified;
                }
            }
            else
            {
                recordToBeUpdated = this.DataContext.rfqSuppliers.Where(a => a.SupplierId == rFQSuppliers.SupplierId && a.RFQId == rFQSuppliers.RFQId && a.IsDeleted == true).SingleOrDefault();
                if (recordToBeUpdated == null)
                {
                    recordToBeUpdated = new MES.Data.Library.rfqSupplier();
                    recordToBeUpdated.CreatedBy = recordToBeUpdated.UpdatedBy = CurrentUser;
                    recordToBeUpdated.CreatedDate = AuditUtils.GetCurrentDateTime();
                    recordToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
                    this.DataContext.rfqSuppliers.Add(recordToBeUpdated);

                }
                else
                {
                    recordToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
                    recordToBeUpdated.UpdatedBy = CurrentUser;
                    this.DataContext.Entry(recordToBeUpdated).State = EntityState.Modified;

                }
            }
            if (string.IsNullOrEmpty(errMSg))
            {
                #region Save general details
                recordToBeUpdated.UniqueURL = recordToBeUpdated.OriginalURL = uniqueId;
                recordToBeUpdated.RFQId = rFQSuppliers.RFQId;
                recordToBeUpdated.SupplierId = rFQSuppliers.SupplierId;
                recordToBeUpdated.IsQuoteTypeDQ = rFQSuppliers.IsQuoteTypeDQ;
                recordToBeUpdated.IsDeleted = false;
                this.DataContext.SaveChanges();
                rFQSuppliers.Id = recordToBeUpdated.Id;
                #endregion

                successMsg = Languages.GetResourceText("RFQSupplierSavedSuccess");
            }
            var response = SuccessOrFailedResponse<MES.DTO.Library.RFQ.RFQ.RFQSuppliers>(errMSg, rFQSuppliers, successMsg);
            return response;
        }

        #region "SEND RFQ"
        #region "SEND RFQ to Available Suppliers"
        /// <summary>
        ///Send RFQ To suppliers.
        /// </summary>
        /// <param name="paging">The paging.</param>
        /// <returns></returns>
        public ITypedResponse<bool?> SendRFQToSuppliers(DTO.Library.RFQ.RFQ.RFQSuppliers rfqSupplierData)
        {
            List<string> supplierNameList = new List<string>();
            Supplier.Suppliers supplierObj = new Supplier.Suppliers();
            DTO.Library.RFQ.Supplier.Suppliers supplierItem = null;
            DTO.Library.RFQ.RFQ.RFQSuppliers rfqSupplierToBeSaved = null;
            foreach (string id in rfqSupplierData.SupplierIdList)
            {
                rfqSupplierToBeSaved = new DTO.Library.RFQ.RFQ.RFQSuppliers();
                //Insert RFQSuppliers 
                rfqSupplierToBeSaved.SupplierId = Convert.ToInt32(id);
                rfqSupplierToBeSaved.RFQId = rfqSupplierData.RFQId;
                rfqSupplierToBeSaved.IsQuoteTypeDQ = rfqSupplierData.IsQuoteTypeDQ;
                DTO.Library.RFQ.RFQ.RFQSuppliers ret = Save(rfqSupplierToBeSaved).Result;
                if (ret.Id > 0)
                {
                    //Send RFQ email with attachments
                    SendRFQ(ret.SupplierId, ret.RFQId, ret.Id);
                    supplierItem = supplierObj.FindById(ret.SupplierId).Result;
                    supplierNameList.Add(supplierItem.CompanyName);
                }
            }
            // SendEmailToSQs(rfqSupplierData);
            SendMasterEmail(rfqSupplierData);

            return SuccessBoolResponse(Languages.GetResourceText("RFQSentSuccess").Replace("%supplierCompanyNames%", string.Join(", ", supplierNameList).TrimEnd(',')));
        }
        #endregion
        #region "Resend RFQ To RFQSuppliers"
        /// <summary>
        ///Send RFQ To suppliers.
        /// </summary>
        /// <param name="paging">The paging.</param>
        /// <returns></returns>
        public ITypedResponse<bool?> ResendRFQToRfqSuppliers(DTO.Library.RFQ.RFQ.RFQSuppliers rfqSupplierData)
        {
            List<string> supplierNameList = new List<string>();
            Supplier.Suppliers supplierObj = new Supplier.Suppliers();
            DTO.Library.RFQ.Supplier.Suppliers supplierItem = null;
            DTO.Library.RFQ.RFQ.RFQSuppliers rfqSupplierToBeSaved = null;

            if (rfqSupplierData.RFQSupplierIdList != null)
            {
                foreach (string id in rfqSupplierData.RFQSupplierIdList)
                {

                    rfqSupplierToBeSaved = new DTO.Library.RFQ.RFQ.RFQSuppliers();
                    rfqSupplierToBeSaved = FindById(Convert.ToInt32(id)).Result;
                    //Insert RFQSuppliers 
                    DTO.Library.RFQ.RFQ.RFQSuppliers ret = Save(rfqSupplierToBeSaved).Result;
                    if (ret.Id > 0)
                    {
                        //Send RFQ email with attachments
                        SendRFQ(ret.SupplierId, ret.RFQId, ret.Id);

                        supplierItem = supplierObj.FindById(ret.SupplierId).Result;
                        supplierNameList.Add(supplierItem.CompanyName);
                    }
                }
                // SendEmailToSQs(rfqSupplierData);
                SendMasterEmail(rfqSupplierData);
            }
            else
            {

                rfqSupplierToBeSaved = new DTO.Library.RFQ.RFQ.RFQSuppliers();
                rfqSupplierToBeSaved = FindById(rfqSupplierData.Id).Result;
                DTO.Library.RFQ.RFQ.RFQSuppliers ret = Save(rfqSupplierToBeSaved).Result;
                if (ret.Id > 0)
                {
                    //Send RFQ email with attachments
                    SendRFQ(ret.SupplierId, ret.RFQId, ret.Id);
                    // SendEmailToSQs(ret.SupplierId, ret.RFQId);
                    SendMasterEmail(ret.SupplierId, ret.RFQId);

                    supplierItem = supplierObj.FindById(ret.SupplierId).Result;
                    supplierNameList.Add(supplierItem.CompanyName);
                }
            }
            return SuccessBoolResponse(Languages.GetResourceText("RFQSentSuccess").Replace("%supplierCompanyNames%", string.Join(", ", supplierNameList).TrimEnd(',')));
        }
        #endregion
        #region "Send Email(the list of Suppliers) To Supplier Quality whenever RFQ email is suppliers"
        private bool SendEmailToSQs(DTO.Library.RFQ.RFQ.RFQSuppliers rfqSupplierData)
        {
            bool IsSuccess = false;
            // TODO: 1. Purpose : Send Email(the list of Suppliers) To Supplier Quality whenever RFQ email is suppliers.
            // TODO: Get SQ
            Supplier.Suppliers supplierObj = new Supplier.Suppliers();
            DTO.Library.RFQ.Supplier.Suppliers supplierItem = null;
            Dictionary<Guid?, int> SQ_SupplierList = new Dictionary<Guid?, int>();
            foreach (string id in rfqSupplierData.SupplierIdList)
            {
                supplierItem = supplierObj.FindById(Convert.ToInt32(id)).Result;
                if (supplierItem.SQId.HasValue)
                    SQ_SupplierList.Add(supplierItem.SQId.Value, Convert.ToInt32(id));

            }
            string SupplierList = string.Empty;
            if (SQ_SupplierList.Count() > 0)
            {
                foreach (var item in SQ_SupplierList.GroupBy(item => item.Key))
                {


                }
            }
            return IsSuccess;
        }

        private bool SendEmailToSQs(int supplierId, string rfqId)
        {
            bool IsSuccess = false;
            // TODO: 1. Purpose : Send Email(the list of Suppliers) To Supplier Quality whenever RFQ email is suppliers.
            // TODO: Get SQ
            Supplier.Suppliers supplierObj = new Supplier.Suppliers();
            DTO.Library.RFQ.Supplier.Suppliers supplierItem = null;
            MES.DTO.Library.RFQ.Supplier.Contacts contactItem = null;
            try
            {
                supplierItem = supplierObj.FindById(supplierId).Result;
                if (supplierItem.lstContact.Count > 0)
                {
                    contactItem = supplierItem.lstContact.Where(item => item.IsDefault == true).First();
                }
                Guid? SQId = null;
                supplierItem = supplierObj.FindById(supplierId).Result;
                if (supplierItem.SQId.HasValue)
                    SQId = supplierItem.SQId.Value;


                string SupplierList = string.Empty;
                if (SQId.HasValue)
                {
                    UserManagement.UserManagement userObj = new UserManagement.UserManagement();
                    MES.DTO.Library.Identity.LoginUser sqItem = userObj.FindById(SQId.Value.ToString()).Result;

                    SupplierList += "<tr><td align='left' style='font-family: Tahoma, Geneva, sans-serif;color: #626465;font-size: 11px;'>"
                             + sqItem.UserName
                             + "</td><td align='left' style='font-family: Tahoma, Geneva, sans-serif;color: #626465;font-size: 11px;'>"
                             + contactItem.FirstName + " " + contactItem.LastName
                             + "</td></tr>";


                    string emailBody = string.Empty;
                    DTO.Library.Common.EmailData emailData = new DTO.Library.Common.EmailData();

                    emailBody = GetRfqEmailBody("SQEmailTemplate.txt");
                    emailBody = emailBody.Replace("<%SQName%>", sqItem.UserName)
                                   .Replace("<%RFQId%>", rfqId)
                                   .Replace("<%SupplierList%>", SupplierList);
                    SupplierList = string.Empty;

                    emailData.EmailBody = emailBody;
                    MES.Business.Repositories.Email.IEmailRepository emailRepository = new MES.Business.Library.BO.Email.Email();
                    List<string> lstToAddress = new List<string>();
                    emailData.EmailSubject = Languages.GetResourceText("rfqMasterEmailSubject").Replace("%rfqNo%", rfqId); ;//"Quote Request for RFQ # " + RfqDetails.RfqId + " sent to Suppliers";
                    lstToAddress.Add(sqItem.Email);
                    emailRepository.SendEmail(lstToAddress, "", emailData.EmailSubject, emailData.EmailBody, out IsSuccess, null, null);

                }
                return IsSuccess;
            }
            catch (Exception ex)
            {
                return IsSuccess;
            }
        }
        #endregion
        #region "Send RFQ to Suppliers"
        private bool SendRFQ(int supplierId, string rfqId, int rfqSupplierId)
        {
            bool IsSuccess = false;

            Supplier.Suppliers supplierObj = new Supplier.Suppliers();
            DTO.Library.RFQ.Supplier.Suppliers supplierItem = null;
            MES.DTO.Library.RFQ.Supplier.Contacts contactItem = null;
            try
            {
                supplierItem = supplierObj.FindById(supplierId).Result;
                if (supplierItem.lstContact.Count > 0)
                {
                    contactItem = supplierItem.lstContact.Where(item => item.IsDefault == true).First();
                }
                RFQ rfqObj = new RFQ();
                DTO.Library.RFQ.RFQ.RFQ rfqItem = rfqObj.FindById(rfqId).Result;

                HttpContext context = HttpContext.Current;

                //Zip - RFQ PDF & Part Specs
                string rfqFilePath = string.Empty;
                if (!string.IsNullOrEmpty(rfqItem.RfqFilePath))
                    rfqFilePath = rfqItem.RfqFilePath;

                string rfqDirectoryPath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"]) + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + Constants.RFQFILESFOLDER;
                string rfqTempFilePath = string.Empty;
                string zipFileNavigationURL = string.Empty;

                string zipFileName = rfqId + ".zip";
                //Delete Zip if exists
                bool isSuccess = Helper.BlobHelper.DeleteBlobfile(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), rfqDirectoryPath + zipFileName);
                ZipFile zipFile = new ZipFile();
                if (!string.IsNullOrEmpty(rfqItem.RfqFilePath))
                {
                    if (Helper.BlobHelper.CheckFileExists(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), rfqFilePath))
                    {
                        byte[] fileBytes = Helper.BlobHelper.GetBlobStreamByUrl(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), rfqFilePath);
                        using (var fileStream = new MemoryStream(fileBytes))
                        {
                            fileStream.Seek(0, SeekOrigin.Begin);
                            zipFile.AddEntry(Path.GetFileName(rfqFilePath), fileBytes);
                        }
                    }
                }
                int i = 1;
                RFQParts rfqPartsObj = new RFQParts();
                List<DTO.Library.RFQ.RFQ.RFQParts> rfqPartList = rfqPartsObj.GetRFQPartsList(rfqId);
                foreach (var item in rfqPartList)
                {
                    foreach (MES.DTO.Library.RFQ.RFQ.RFQPartAttachment partattachment in item.RfqPartAttachmentList)
                    {
                        if (Helper.BlobHelper.CheckFileExists(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), partattachment.AttachmentPathOnServer))
                        {
                            byte[] fileBytes = Helper.BlobHelper.GetBlobStreamByUrl(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), partattachment.AttachmentPathOnServer);
                            using (var fileStream = new MemoryStream(fileBytes))
                            {
                                fileStream.Seek(0, SeekOrigin.Begin);
                                zipFile.AddEntry(i + "_" + partattachment.AttachmentName, fileBytes); i++;
                            }
                        }
                    }
                }
                using (var zipStream = new MemoryStream())
                {
                    zipFile.Save(zipStream);
                    zipStream.Seek(0, SeekOrigin.Begin);
                    zipFileNavigationURL = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), Constants.RFQFILESFOLDERNAME, zipFileName, zipStream);
                }

                RFQSuppliers rs = new RFQSuppliers();
                DTO.Library.RFQ.RFQ.RfqSupplierSearchCriteria rscriteria = new DTO.Library.RFQ.RFQ.RfqSupplierSearchCriteria();
                rscriteria.RFQId = rfqId;

                DTO.Library.RFQ.RFQ.RFQSuppliers rsItem = rs.FindById(rfqSupplierId).Result;
                string supplierNavigationURL = string.Empty;

                if (rsItem.IsQuoteTypeDQ.HasValue && rsItem.IsQuoteTypeDQ.Value)
                    supplierNavigationURL = System.Configuration.ConfigurationManager.AppSettings["redirectURLDQSubmitQuote"] + rsItem.UniqueURL;
                else
                    supplierNavigationURL = System.Configuration.ConfigurationManager.AppSettings["redirectURLSubmitQuote"] + rsItem.UniqueURL;

                string attachmentText = "Please <a href='" + zipFileNavigationURL + "'>click here</a> to download attachment."
                                   , noAttachmentText = "No documents available for download.";

                BO.Setup.EmailTemplate.EmailTemplate etObj = new Setup.EmailTemplate.EmailTemplate();
                DTO.Library.Setup.EmailTemplate.EmailTemplate emailTemplate = etObj.FindByShortCode(Constants.RFQINVITETOSUPPLIER.ToString()).Result;

                string emailBody = emailTemplate.EmailBody.Trim();
                if (zipFile.Count > 0)
                {
                    emailBody = emailBody
                          .Replace("%RFQNo%", rfqItem.Id)
                          .Replace("%QuoteDueDate%", !rfqItem.QuoteDueDate.HasValue ? "not mentioned" : rfqItem.QuoteDueDate.Value.ToString("dd-MMM-yy"))
                          .Replace("%AttachmentUrl%", attachmentText)
                          .Replace("%QuoteUrl%", supplierNavigationURL);
                }
                else
                {
                    emailBody = emailBody
                          .Replace("%RFQNo%", rfqItem.Id)
                          .Replace("%QuoteDueDate%", !rfqItem.QuoteDueDate.HasValue ? "not mentioned" : rfqItem.QuoteDueDate.Value.ToString("dd-MMM-yy"))
                          .Replace("%AttachmentUrl%", noAttachmentText)
                          .Replace("%QuoteUrl%", supplierNavigationURL);
                }

                DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria criteria = new DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria();
                criteria.RFQId = rfqId;
                criteria.SupplierId = supplierId;
                criteria.UniqueUrl = rsItem.UniqueURL;
                RFQSupplierPartQuote rspqObj = new RFQSupplierPartQuote();
                string filePath = string.Empty;
                List<System.Net.Mail.Attachment> attachments = new List<System.Net.Mail.Attachment>();

                if (rsItem.IsQuoteTypeDQ.HasValue && rsItem.IsQuoteTypeDQ.Value)
                    filePath = rspqObj.CreateDetailSubmitQuoteFile(criteria);
                else
                    filePath = rspqObj.CreateSubmitQuoteFile(criteria);
                if (!string.IsNullOrEmpty(filePath))
                {
                    Stream memoryStream = MES.Business.Library.Helper.BlobHelper.AttachFile_ConvertBlobToStream(filePath);
                    attachments.Add(new System.Net.Mail.Attachment(memoryStream, Path.GetFileName(filePath)));

                }
                emailTemplate.EmailSubject = emailTemplate.EmailSubject.Trim()
                                .Replace("%RFQNo%", rfqItem.Id)
                                .Replace("%ProjectName%", (string.IsNullOrEmpty(context.Server.HtmlDecode(rfqItem.ProjectName)) ? string.Empty : context.Server.HtmlDecode(rfqItem.ProjectName)));

                MES.Business.Repositories.Email.IEmailRepository emailRepository = new MES.Business.Library.BO.Email.Email();
                List<string> lstToAddress = new List<string>();
                lstToAddress.Add(contactItem.Email);
                emailRepository.SendEmail(lstToAddress, "", emailTemplate.EmailSubject, emailBody, out IsSuccess, attachments, null);

                return IsSuccess;
            }
            catch (Exception ex)
            {
                return IsSuccess;
            }
        }
        #endregion

        #region "Send Master Email"

        private bool SendMasterEmail(DTO.Library.RFQ.RFQ.RFQSuppliers rfqSupplierData)
        {
            // TODO: 3.
            /** Send Master Email To Carol 28-Mar-2013 **/
            /** change Carol to messupplierquote@mesinc.net 20150619**/
            /** Send MasterEmail to Countrywise Sourcing Manager **/
            /** Starts here **/
            string SuppliersList = string.Empty
               , downloadText = string.Empty;

            List<string> countries = new List<string>();

            Supplier.Suppliers supplierObj = new Supplier.Suppliers();
            DTO.Library.RFQ.Supplier.Suppliers supplierItem = null;
            MES.DTO.Library.RFQ.Supplier.Contacts contactItem = null;

            foreach (string id in rfqSupplierData.SupplierIdList)
            {
                supplierItem = supplierObj.FindById(Convert.ToInt32(id)).Result;

                if (supplierItem.lstContact.Count > 0)
                {
                    contactItem = supplierItem.lstContact.Where(item => item.IsDefault == true).First();
                }
                SuppliersList += "<tr><td align='left' style='font-family: Tahoma, Geneva, sans-serif;color: #626465;font-size: 11px;'>"
                           + supplierItem.CompanyName
                           + "</td><td align='left' style='font-family: Tahoma, Geneva, sans-serif;color: #626465;font-size: 11px;'>"
                           + ((contactItem != null) ? contactItem.FirstName + " " + contactItem.LastName : string.Empty)
                           + "</td></tr>";

                if (!string.IsNullOrEmpty(supplierItem.Country) && !countries.Contains(supplierItem.Country))
                    countries.Add(supplierItem.Country);
            }
            supplierObj = null;


            bool IsSuccess = false;
            MES.Business.Repositories.Email.IEmailRepository emailRepository = new MES.Business.Library.BO.Email.Email();
            DTO.Library.Common.EmailData emailData = new DTO.Library.Common.EmailData();
            List<string> lstToAddress = new List<string>();

            downloadText = "Please click here to download attachment.";
            RFQ rfqObj = new RFQ();
            DTO.Library.RFQ.RFQ.RFQ rfqItem = rfqObj.FindById(rfqSupplierData.RFQId).Result;
            emailData.EmailBody = GetRfqEmailBody("MasterEmailTemplate.htm")
                     .Replace("<%RFQId%>", rfqSupplierData.RFQId)
                     .Replace("<%Name%>", System.Configuration.ConfigurationManager.AppSettings["RFQEmailCopiesToName"].Trim())
                     .Replace("<%SuppliersList%>", SuppliersList)
                     .Replace("<%QuoteDueDate%>", !rfqItem.QuoteDueDate.HasValue ? "not mentioned" : rfqItem.QuoteDueDate.Value.ToString("dd-MMM-yy"))
                     .Replace("<%DownloadText%>", downloadText);


            emailData.EmailSubject = Languages.GetResourceText("rfqMasterEmailSubject").Replace("%rfqNo%", rfqSupplierData.RFQId);

            lstToAddress.Add(System.Configuration.ConfigurationManager.AppSettings["RFQEmailCopiesTo"]);

            string key = string.Empty;
            if (countries.Count() > 0)
            {
                if (countries.Contains("China"))
                {
                    key = (System.Configuration.ConfigurationManager.AppSettings["ChinaSourcingMangers"]);
                    lstToAddress.Add(key);
                }
                if (countries.Contains("India"))
                {
                    key = (System.Configuration.ConfigurationManager.AppSettings["IndiaSourcingMangers"]);
                    lstToAddress.Add(key);
                }
            }

            emailRepository.SendEmail(lstToAddress, "", emailData.EmailSubject, emailData.EmailBody, out IsSuccess, null);
            return IsSuccess;
        }
        private bool SendMasterEmail(int supplierId, string rfqId)
        {
            // TODO: 3.
            /** Send Master Email To Carol 28-Mar-2013 **/
            /** change Carol to messupplierquote@mesinc.net 20150619**/
            /** Send MasterEmail to Countrywise Sourcing Manager **/
            /** Starts here **/
            string SuppliersList = string.Empty
               , downloadText = string.Empty;

            string country = string.Empty;

            Supplier.Suppliers supplierObj = new Supplier.Suppliers();
            DTO.Library.RFQ.Supplier.Suppliers supplierItem = null;
            MES.DTO.Library.RFQ.Supplier.Contacts contactItem = null;

            supplierItem = supplierObj.FindById(supplierId).Result;

            if (supplierItem.lstContact.Count > 0)
            {
                contactItem = supplierItem.lstContact.Where(item => item.IsDefault == true).First();
            }
            SuppliersList += "<tr><td align='left' style='font-family: Tahoma, Geneva, sans-serif;color: #626465;font-size: 11px;'>"
                       + supplierItem.CompanyName
                       + "</td><td align='left' style='font-family: Tahoma, Geneva, sans-serif;color: #626465;font-size: 11px;'>"
                       + ((contactItem != null) ? contactItem.FirstName + " " + contactItem.LastName : string.Empty)
                       + "</td></tr>";

            if (!string.IsNullOrEmpty(supplierItem.Country))
                country = supplierItem.Country;

            bool IsSuccess = false;
            MES.Business.Repositories.Email.IEmailRepository emailRepository = new MES.Business.Library.BO.Email.Email();
            DTO.Library.Common.EmailData emailData = new DTO.Library.Common.EmailData();
            List<string> lstToAddress = new List<string>();

            downloadText = "Please click here to download attachment.";
            RFQ rfqObj = new RFQ();
            DTO.Library.RFQ.RFQ.RFQ rfqItem = rfqObj.FindById(rfqId).Result;
            emailData.EmailBody = GetRfqEmailBody("MasterEmailTemplate.htm")
                     .Replace("<%RFQId%>", rfqId)
                     .Replace("<%Name%>", System.Configuration.ConfigurationManager.AppSettings["RFQEmailCopiesToName"].Trim())
                     .Replace("<%SuppliersList%>", SuppliersList)
                     .Replace("<%QuoteDueDate%>", !rfqItem.QuoteDueDate.HasValue ? "not mentioned" : rfqItem.QuoteDueDate.Value.ToString("dd-MMM-yy"))
                     .Replace("<%DownloadText%>", downloadText);


            emailData.EmailSubject = Languages.GetResourceText("rfqMasterEmailSubject").Replace("%rfqNo%", rfqId);

            lstToAddress.Add(System.Configuration.ConfigurationManager.AppSettings["RFQEmailCopiesTo"]);

            string key = string.Empty;
            if (!string.IsNullOrEmpty(country))
            {
                if (country.ToLower() == "china")
                {
                    key = (System.Configuration.ConfigurationManager.AppSettings["ChinaSourcingMangers"]);
                    lstToAddress.Add(key);
                }
                else if (country.ToLower() == "india")
                {
                    key = (System.Configuration.ConfigurationManager.AppSettings["IndiaSourcingMangers"]);
                    lstToAddress.Add(key);
                }
            }

            emailRepository.SendEmail(lstToAddress, "", emailData.EmailSubject, emailData.EmailBody, out IsSuccess, null, null);
            return IsSuccess;
        }

        #endregion
        #endregion


        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQSuppliers>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public ITypedResponse<bool?> Delete(int data)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> DeleteRFQSuppliersbyRFQId(string RFQId)
        {

            var RFQToBeDeleted = this.DataContext.rfqSuppliers.Where(a => a.RFQId == RFQId).ToList();
            if (RFQToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("RFQSupplierNotExists"));
            }
            else
            {
                foreach (var item in RFQToBeDeleted)
                {
                    //item.UpdatedDate = AuditUtils.GetCurrentDateTime();
                    //item.UpdatedBy = CurrentUser;
                    this.DataContext.Entry(item).State = EntityState.Deleted;
                    //item.IsDeleted = true;
                    this.DataContext.SaveChanges();
                }

                return SuccessBoolResponse(Languages.GetResourceText("RFQSupplierDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<int> RFQSuppliersCount(string rfqId)
        {
            string errMSg = string.Empty;
            int rfqSupplierCount = 0;
            DTO.Library.RFQ.RFQ.RFQSuppliers rFQSuppliers = new DTO.Library.RFQ.RFQ.RFQSuppliers();

            this.RunOnDB(context =>
            {
                rfqSupplierCount = context.rfqSuppliers.Where(r => r.RFQId == rfqId).Count();

            });
            //get hold of response
            var response = SuccessOrFailedResponse<int>(errMSg, rfqSupplierCount);
            return response;

        }


        ITypedResponse<int?> ICrudMethods<DTO.Library.RFQ.RFQ.RFQSuppliers, int?, string, DTO.Library.RFQ.RFQ.RFQSuppliers, int, bool?, int, DTO.Library.RFQ.RFQ.RFQSuppliers>.Save(DTO.Library.RFQ.RFQ.RFQSuppliers data)
        {
            throw new NotImplementedException();
        }


        public ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQSuppliers>> GetQuotedSuppliers(string rfqId)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.RFQ.RFQ.RFQSuppliers> lstRFQSuppliers = new List<DTO.Library.RFQ.RFQ.RFQSuppliers>();
            DTO.Library.RFQ.RFQ.RFQSuppliers rFQSuppliers;
            this.RunOnDB(context =>
            {
                var rfqSupplierList = context.GetQuotedSuppliers(rfqId);
                if (rfqSupplierList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    foreach (var item in rfqSupplierList)
                    {
                        rFQSuppliers = new DTO.Library.RFQ.RFQ.RFQSuppliers();
                        rFQSuppliers.SupplierId = item.SupplierId;
                        rFQSuppliers.CompanyName = item.CompanyName;
                        rFQSuppliers.QuoteDate = item.QuoteDate;
                        lstRFQSuppliers.Add(rFQSuppliers);
                    }
                }
            });
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSuppliers>>(errMSg, lstRFQSuppliers);
            return response;
        }
    }
}
