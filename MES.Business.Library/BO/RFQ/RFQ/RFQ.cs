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
using System.Web;
using EvoPdf.HtmlToPdf;
using System.IO;
using MES.Business.Repositories.UserManagement;
using MES.DTO.Library.Identity;
using MES.Business.Library.Enums;
using MES.Business.Mapping.Extensions;
namespace MES.Business.Library.BO.RFQ.RFQ
{
    class RFQ : ContextBusinessBase, IRFQRepository
    {
        public RFQ()
            : base("RFQ")
        { }


        public NPE.Core.ITypedResponse<DTO.Library.RFQ.RFQ.RFQ> CreateRevision(string rfqNo)
        {

            string errMSg = string.Empty;
            DTO.Library.RFQ.RFQ.RFQ parentRfqItem = FindById(rfqNo).Result;
            DTO.Library.RFQ.RFQ.RFQ newrfqItem = new DTO.Library.RFQ.RFQ.RFQ();

            #region general details          

            newrfqItem = ObjectLibExtensions.AutoConvert<DTO.Library.RFQ.RFQ.RFQ>(parentRfqItem);
            newrfqItem.Id = rfqNo;
            newrfqItem.lstQuoteToCustomer = null;
            newrfqItem.lstQuotedSuppliers = null;
            newrfqItem.lstRFQPart = null;
            if (!string.IsNullOrEmpty(parentRfqItem.RfqFilePath))
                newrfqItem.RfqFilePath = CreateRFQFile(rfqNo, parentRfqItem.RfqFilePath);
            newrfqItem.isRevision = true;
            newrfqItem.Id = Save(newrfqItem).Result;
            #endregion
            newrfqItem.lstRFQPart = CreateRfqPartsRev(newrfqItem.Id, rfqNo);

            CreateRfqSupplierRev(newrfqItem.Id, rfqNo);
            CreatePDF(newrfqItem);
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.RFQ.RFQ.RFQ>(errMSg, newrfqItem);
            return response;
        }

        private List<DTO.Library.RFQ.RFQ.RFQParts> CreateRfqPartsRev(string newRfqId, string pRfqId)
        {
            #region RFQ Parts List
            DTO.Library.RFQ.RFQ.RFQParts rFQParts;
            List<DTO.Library.RFQ.RFQ.RFQParts> lstRfqParts = new List<DTO.Library.RFQ.RFQ.RFQParts>();
            RFQParts rfqpartsObj = new RFQParts();
            List<DTO.Library.RFQ.RFQ.RFQParts> lstpRfqParts = rfqpartsObj.GetRFQPartsList(pRfqId);

            if (lstpRfqParts.Count > 0)
            {
                foreach (var pPartitem in lstpRfqParts)
                {
                    rFQParts = new DTO.Library.RFQ.RFQ.RFQParts();

                    rFQParts.RfqId = newRfqId;
                    rFQParts.CustomerPartNo = pPartitem.CustomerPartNo;
                    rFQParts.RevLevel = pPartitem.RevLevel;
                    rFQParts.PartDescription = pPartitem.PartDescription;
                    rFQParts.AdditionalPartDesc = pPartitem.AdditionalPartDesc;
                    rFQParts.MaterialType = pPartitem.MaterialType;
                    rFQParts.EstimatedQty = pPartitem.EstimatedQty;
                    rFQParts.PartWeightKG = pPartitem.PartWeightKG;

                    //RFQ Part Attachments
                    if (pPartitem.RfqPartAttachmentList != null && pPartitem.RfqPartAttachmentList.Count > 0)
                    {
                        List<DTO.Library.RFQ.RFQ.RFQPartAttachment> lstRFQPartAttachments = new List<DTO.Library.RFQ.RFQ.RFQPartAttachment>();
                        DTO.Library.RFQ.RFQ.RFQPartAttachment rFQPartAttachments;

                        foreach (var pPartAttachmentitem in pPartitem.RfqPartAttachmentList)
                        {
                            rFQPartAttachments = new DTO.Library.RFQ.RFQ.RFQPartAttachment();

                            rFQPartAttachments.RfqPartId = Convert.ToInt32(rFQParts.Id);
                            rFQPartAttachments.AttachmentName = pPartAttachmentitem.AttachmentName;
                            rFQPartAttachments.AttachmentDetail = pPartAttachmentitem.AttachmentDetail;

                            if (Helper.BlobHelper.CheckFileExists(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), pPartAttachmentitem.AttachmentPathOnServer))
                            {
                                rFQPartAttachments.AttachmentPathOnServer = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                                       , Constants.RFQATTACHMENTFILEPATH + Guid.NewGuid() + Path.GetExtension(pPartAttachmentitem.AttachmentPathOnServer)
                                       , pPartAttachmentitem.AttachmentPathOnServer);
                            }
                        }
                    }

                    rFQParts.Id = rfqpartsObj.Save(rFQParts).Result;
                    lstRfqParts.Add(rFQParts);
                }
            }
            #endregion
            return lstRfqParts;

        }

        private void CreateRfqSupplierRev(string revRfqId, string pRfqId)
        {
            var context = HttpContext.Current;
            #region RFQ Supplier
            List<DTO.Library.RFQ.RFQ.RFQSuppliers> lstRFQSuppliers = new List<DTO.Library.RFQ.RFQ.RFQSuppliers>();
            DTO.Library.RFQ.RFQ.RFQSuppliers rFQSuppliers;

            RFQSuppliers rfqSupObj = new RFQSuppliers();

            foreach (var item in rfqSupObj.GetRFQSuppliers(pRfqId).Result)
            {
                rFQSuppliers = new DTO.Library.RFQ.RFQ.RFQSuppliers();

                rFQSuppliers.RFQId = revRfqId;
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

                rfqSupObj.Save(rFQSuppliers);
            }

            #endregion
        }

        private string CreateRFQFile(string pRfqId, string pRfqFilePath)
        {
            string rRfqFilePath = string.Empty;
            var context = HttpContext.Current;

            if (Helper.BlobHelper.CheckFileExists(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), pRfqFilePath))
            {
                rRfqFilePath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                              , Constants.RFQFILESFOLDERNAME + Guid.NewGuid() + Path.GetExtension(pRfqFilePath)
                              , pRfqFilePath);
            }


            return rRfqFilePath;
        }
        public NPE.Core.ITypedResponse<string> Save(DTO.Library.RFQ.RFQ.RFQ RFQdata)
        {
            string errMSg = null, action = "Create";
            string successMsg = null;

            //set the out put param
            ObjectParameter RfqId = new ObjectParameter("RfqId", 0);
            //1. Save RFQ
            if (!string.IsNullOrEmpty(RFQdata.Id))
            {
                if (RFQdata.isRevision)
                    action = "Create";
                else
                    action = "Update";
            }
            this.RunOnDB(context =>
            {
                string filePath = string.Empty;

                if (!string.IsNullOrEmpty(RFQdata.RfqFilePath))
                {
                    filePath = MES.Business.Library.Helper.BlobHelper.GetPhysicalPath(RFQdata.RfqFilePath);
                }
                int result = context.SaveRFQ(action, RFQdata.Id, RFQdata.CustomerId, RFQdata.CustomerContactId,
                      RFQdata.Date, Constants.DEFAULTCURRENCY, RFQdata.OtherAssumption, RFQdata.NonAwardFeedbackId,
                      RFQdata.NonAwardDetailedFeedback, RFQdata.Remarks, filePath, RFQdata.RfqFileName,
                      RFQdata.ProjectName, RFQdata.QuoteDueDate, RFQdata.Status, RFQdata.SAMId,
                      RFQdata.ProcessId, RFQdata.CommodityId, RFQdata.CommodityTypeId, RFQdata.RFQCoordinatorId,
                      RFQdata.RFQSourceId, RFQdata.RFQTypeId, RFQdata.SupplierRequirement, RFQdata.RFQPriorityId, RFQdata.IndustryTypeId, CurrentUser, CurrentUser, RfqId);

                if (result > 0)
                {
                    RFQdata.isRevision = false;
                    RFQdata.Id = Convert.ToString(RfqId.Value);
                    if (action != "Update")
                    {
                        if (RFQdata.lstRFQPart != null && RFQdata.lstRFQPart.Count > 0)
                        {
                            RFQParts rfqPartObj = new RFQParts();
                            List<RFQParts> lstRfqParts = new List<RFQParts>();//   RFQdata.lstRFQPart
                            foreach (MES.DTO.Library.RFQ.RFQ.RFQParts pItem in RFQdata.lstRFQPart)
                            {
                                pItem.RfqId = RFQdata.Id;
                                rfqPartObj.Save(pItem);
                            }
                        }
                    }
                }
                successMsg = Languages.GetResourceText("RFQSavedSuccess");
            });

            //2.Create RFQQ PDF
            if (action != "Create")
            {
                CreatePDF(RFQdata);
            }
            //3. Send Notification Email on RFQ Status change
            if (RFQdata.StatusUpdatedDate == null && !string.IsNullOrEmpty(RFQdata.Status) && RFQdata.Status.ToUpper() == "C")
            {
                NPE.Core.ITypedResponse<bool?> ret = SendNotificationEmail(RFQdata);
                if (ret.IsOK())
                {
                    MES.Data.Library.RFQ recordToBeUpdated = new MES.Data.Library.RFQ();
                    recordToBeUpdated = this.DataContext.RFQs.Where(a => a.Id == RFQdata.Id).SingleOrDefault();

                    recordToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
                    recordToBeUpdated.UpdatedBy = CurrentUser;
                    recordToBeUpdated.StatusUpdatedDate = AuditUtils.GetCurrentDateTime();
                    this.DataContext.Entry(recordToBeUpdated).State = EntityState.Modified;
                    this.DataContext.SaveChanges();
                }
            }

            return SuccessOrFailedResponse<string>(errMSg, RFQdata.Id, successMsg);
        }
        /// <summary>
        /// Send Notification Email To SAM user to notify -  RFQ has been marked as complete.
        /// </summary>
        /// <returns></returns>
        private NPE.Core.ITypedResponse<bool?> SendNotificationEmail(DTO.Library.RFQ.RFQ.RFQ RfqDetails)
        {
            bool IsSuccess = false;
            try
            {
                MES.Business.Repositories.Email.IEmailRepository emailRepository = new MES.Business.Library.BO.Email.Email();
                MES.DTO.Library.Common.EmailData emailData = new DTO.Library.Common.EmailData();

                string redirectURL = System.Configuration.ConfigurationManager.AppSettings["redirectURL"] + RfqDetails.Id;
                List<string> lstToAddress = new List<string>();

                if (!string.IsNullOrEmpty(RfqDetails.SAMId))
                {
                    UserManagement.UserManagement userObj = new UserManagement.UserManagement();
                    DTO.Library.Identity.LoginUser SAMInfo = userObj.FindById(RfqDetails.SAMId).Result;

                    emailData.EmailBody = GetRfqEmailBody("CompletedRFQTemplate.htm")
                        .Replace("<%FirstLineText%>", Languages.GetResourceText("rqStatusCompleteEmailBodyFirstLine"))
                                  .Replace("<%RFQNo%>", RfqDetails.Id)
                                  .Replace("<%CustomerName%>", RfqDetails.CustomerContactName)
                                  .Replace("<%CreatedDate%>", RfqDetails.CreatedDate.Value.ToShortDateString())
                                  .Replace("<%QuoteDueDate%>", RfqDetails.QuoteDueDate.Value.ToShortDateString()
                                  .Replace("<%redirectURL%>", redirectURL)
                                  .Replace("<%SAMName%>", SAMInfo.FirstName + " " + SAMInfo.LastName));

                    emailData.EmailSubject = Languages.GetResourceText("rfqStatusCompleteEmailSubject").Replace("<%rfqNo%>", RfqDetails.Id);// "RFQ #" + RfqDetails.RfqId + " is Completed";
                    lstToAddress.Add(SAMInfo.Email);

                    emailRepository.SendEmail(lstToAddress, "", emailData.EmailSubject, emailData.EmailBody, out IsSuccess, null);
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
        public void CreatePDF(DTO.Library.RFQ.RFQ.RFQ RfqDetails)
        {
            var context = HttpContext.Current;

            PdfConverter pdfConverter = new PdfConverter();
            /* Date : 27-Dec-2012
             * By : Roma
             * Purpose : LicenseKey key 'evopdfkey' added in web.config
             */
            pdfConverter.LicenseKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["evopdfkey"]);
            pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.Letter;
            pdfConverter.PdfDocumentOptions.PdfCompressionLevel = PdfCompressionLevel.Normal;
            pdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Landscape;

            string contents = File.ReadAllText(context.Server.MapPath("~/" + Constants.EmailTemplateFolder + "RfqDetailsPdfTemplate.htm"));

            // Replace the placeholders with the user-specified text
            contents = contents.Replace("<%RFQNo%>", RfqDetails.Id);
            contents = contents.Replace("<%ProjectName%>", RfqDetails.ProjectName);
            contents = contents.Replace("<%RFQDate%>", Convert.ToDateTime(RfqDetails.Date).ToString("dd-MMM-yy"));
            contents = contents.Replace("<%QuoteDueDate%>", (RfqDetails.QuoteDueDate == null) ? string.Empty : Convert.ToDateTime(RfqDetails.QuoteDueDate).ToString("dd-MMM-yy"));
            contents = contents.Replace("<%Remarks%>", RfqDetails.Remarks);
            contents = contents.Replace("<%OtherAssumptions%>", RfqDetails.OtherAssumption);

            RFQParts rfq = new RFQParts();
            List<DTO.Library.RFQ.RFQ.RFQParts> partslist = rfq.GetRFQPartsList(RfqDetails.Id);
            var itemsTable = string.Empty;
            int counter = 0;
            if (partslist.Count > 0)
            {
                foreach (var item in partslist)
                {
                    if (counter % 2 == 0)
                    {
                        if (partslist.Count == counter + 1)
                        {
                            itemsTable += string.Format(@"<tr>
                                                <td bgcolor='#ffffff' width='10%' align='center' style='border-bottom: 1px solid #000;'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{0}</font></td>
                                                <td bgcolor='#ffffff' width='45%' style='border-bottom: 1px solid #000;'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{1}</font></td>
                                                <td bgcolor='#ffffff' width='10%' style='border-bottom: 1px solid #000;'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{2}</font></td>
                                                <td bgcolor='#ffffff' width='15%' style='border-bottom: 1px solid #000;'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{3}</font></td>
                                                <td bgcolor='#ffffff' width='10%' align='right' style='border-bottom: 1px solid #000;'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{4}</font></td>
                                                <td bgcolor='#ffffff' width='10%' align='right' style='border-right: 1px solid #000;border-bottom: 1px solid #000;'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{5}</font></td>
                                                </tr>"
                                        , item.CustomerPartNo
                                        , item.PartDescription
                                        + "<br />" + item.AdditionalPartDesc
                                        , item.RevLevel
                                        , item.MaterialType
                                        , item.EstimatedQty
                                        , item.PartWeightKG.HasValue ? item.PartWeightKG.Value.ToString("#,##0.000") : item.PartWeightKG.ToString());
                        }
                        else
                        {
                            itemsTable += string.Format(@"<tr>
                                                <td bgcolor='#ffffff' width='10%' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{0}</font></td>
                                                <td bgcolor='#ffffff' width='45%'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{1}</font></td>
                                                <td bgcolor='#ffffff' width='10%'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{2}</font></td>
                                                <td bgcolor='#ffffff' width='15%'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{3}</font></td>
                                                <td bgcolor='#ffffff' width='10%' align='right'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{4}</font></td>
                                                <td bgcolor='#ffffff' width='10%' align='right' style='border-right: 1px solid #000;'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{5}</font></td>
                                                </tr>"
                                        , item.CustomerPartNo
                                        , item.PartDescription
                                        + "<br />" + item.AdditionalPartDesc
                                        , item.RevLevel
                                        , item.MaterialType
                                        , item.EstimatedQty
                                        , item.PartWeightKG.HasValue ? item.PartWeightKG.Value.ToString("#,##0.000") : item.PartWeightKG.ToString());
                        }
                        counter++;
                    }
                    else
                    {
                        if (partslist.Count == counter + 1)
                        {
                            itemsTable += string.Format(@"<tr>
                                                 <td bgcolor='#f6f4f2' width='10%' align='center' style='border-bottom: 1px solid #000;'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{0}</font></td>
                                                 <td bgcolor='#f6f4f2' width='45%' style='border-bottom: 1px solid #000;'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{1}</font></td>
                                                 <td bgcolor='#f6f4f2'width='10%' style='border-bottom: 1px solid #000;' ><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{2}</font></td>
                                                 <td bgcolor='#f6f4f2'width='15%' style='border-bottom: 1px solid #000;' ><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{3}</font></td>
                                                 <td bgcolor='#f6f4f2' width='10%' align='right' style='border-bottom: 1px solid #000;'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{4}</font></td>
                                                 <td bgcolor='#f6f4f2' width='10%' align='right' style='border-right: 1px solid #000;border-bottom: 1px solid #000;'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{5}</font></td>
                                                 </tr>"
                                        , item.CustomerPartNo
                                        , item.PartDescription
                                        + "<br />" + item.AdditionalPartDesc
                                        , item.RevLevel
                                        , item.MaterialType
                                        , item.EstimatedQty
                                        , item.PartWeightKG.HasValue ? item.PartWeightKG.Value.ToString("#,##0.000") : item.PartWeightKG.ToString());
                        }
                        else
                        {
                            itemsTable += string.Format(@"<tr>
                                                 <td bgcolor='#f6f4f2' width='10%' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{0}</font></td>
                                                 <td bgcolor='#f6f4f2' width='45%'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{1}</font></td>
                                                 <td bgcolor='#f6f4f2'width='10%' ><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{2}</font></td>
                                                 <td bgcolor='#f6f4f2'width='15%' ><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{3}</font></td>
                                                 <td bgcolor='#f6f4f2' width='10%' align='right'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{4}</font></td>
                                                 <td bgcolor='#f6f4f2' width='10%' align='right' style='border-right: 1px solid #000;'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{5}</font></td>
                                                 </tr>"
                                        , item.CustomerPartNo
                                        , item.PartDescription
                                        + "<br />" + item.AdditionalPartDesc
                                        , item.RevLevel
                                        , item.MaterialType
                                        , item.EstimatedQty
                                        , item.PartWeightKG.HasValue ? item.PartWeightKG.Value.ToString("#,##0.000") : item.PartWeightKG.ToString());
                        }
                        counter++;
                    }
                }
            }

            contents = contents.Replace("<%PartsList%>", itemsTable);
            // get the pdf bytes from html string
            byte[] pdfBytes = pdfConverter.GetPdfBytesFromHtmlString(contents);


            pdfConverter.PdfDocumentOptions.ShowFooter = true;
            pdfConverter.PdfDocumentOptions.ShowHeader = true;

            pdfConverter.PdfHeaderOptions.HeaderHeight = 75;
            string headerAndFooterHtmlUrl = context.Server.MapPath("~/" + Constants.EmailTemplateFolder + "Header.htm");
            pdfConverter.PdfHeaderOptions.HtmlToPdfArea = new HtmlToPdfArea(0, 0, 0, pdfConverter.PdfHeaderOptions.HeaderHeight,
                    headerAndFooterHtmlUrl, 1060, 100);

            pdfConverter.PdfHeaderOptions.DrawHeaderLine = false;

            pdfConverter.PdfFooterOptions.FooterHeight = 20;

            pdfConverter.PdfFooterOptions.TextArea = new TextArea(737, 0, "Page &p; of &P; ",
            new System.Drawing.Font(new System.Drawing.FontFamily("Verdana"), 8, System.Drawing.GraphicsUnit.Point));

            string generatedFileName = "Rfq_" + RfqDetails.Id + ".pdf";
            string tempFilepath = context.Server.MapPath("~") + Constants.REPORTSTEMPLATEFILEPATH + generatedFileName;

            if (!System.IO.Directory.Exists(context.Server.MapPath(@"~") + (Constants.REPORTSTEMPLATEFILEPATH)))
                System.IO.Directory.CreateDirectory(context.Server.MapPath(@"~") + (Constants.REPORTSTEMPLATEFILEPATH));
            else
            {
                if (File.Exists(tempFilepath))
                    File.Delete(tempFilepath);
            }

            pdfConverter.SavePdfFromHtmlStringToFile(contents, tempFilepath);
            string filepath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"])
                            + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                            + Constants.RFQFILESFOLDER
                            + generatedFileName;

            Helper.BlobHelper.DeleteBlobfile(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), filepath);
            Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                      , Constants.RFQFILESFOLDERNAME
                      , generatedFileName
                      , tempFilepath);

            File.Delete(tempFilepath);
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.RFQ.RFQ> FindById(string rfqNo)
        {
            string errMSg = string.Empty;
            DTO.Library.RFQ.RFQ.RFQ rfq = new DTO.Library.RFQ.RFQ.RFQ();
            this.RunOnDB(context =>
            {
                var rfqItem = context.RFQs.Where(r => r.Id == rfqNo).SingleOrDefault();
                if (rfqItem == null)
                    errMSg = Languages.GetResourceText("RFQNotExists");
                else
                {
                    #region general details
                    rfq.Id = rfqItem.Id;
                    rfq.CustomerId = rfqItem.CustomerId;
                    rfq.ProjectName = rfqItem.ProjectName;
                    rfq.CustomerName = rfqItem.Customer.CompanyName;
                    rfq.CustomerContactId = rfqItem.CustomerContactId;
                    if (rfqItem.CustomerContactId > 0)
                    {
                        Customer.Contacts custContactObj = new Customer.Contacts();
                        DTO.Library.RFQ.Customer.Contacts custInfo = custContactObj.FindById(rfqItem.CustomerContactId).Result;
                        if (custInfo != null)
                            rfq.CustomerContactEmail = custInfo.Email;

                    }
                    rfq.Date = rfqItem.Date;
                    rfq.QuoteDueDate = rfqItem.QuoteDueDate;
                    if (string.IsNullOrEmpty(rfqItem.Status))
                        rfq.Status = "R";
                    else
                        rfq.Status = rfqItem.Status;
                    rfq.SAMId = rfqItem.SAMId;
                    rfq.CommodityTypeId = rfqItem.CommodityTypeId;
                    rfq.CommodityId = rfqItem.CommodityId;
                    rfq.ProcessId = rfqItem.ProcessId;
                    rfq.RFQTypeId = rfqItem.RFQTypeId;
                    rfq.RFQCoordinatorId = rfqItem.RFQCoordinatorId;
                    rfq.RFQSourceId = rfqItem.RFQSourceId;
                    if (!string.IsNullOrEmpty(rfqItem.RfqFilePath))
                    {
                        rfq.RfqFilePath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"]) + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]) + rfqItem.RfqFilePath;
                        rfq.RfqFileName = rfqItem.RfqFileName;
                    }
                    else
                        rfq.RfqFilePath = rfqItem.RfqFilePath;
                    rfq.Remarks = rfqItem.Remarks;
                    rfq.OtherAssumption = rfqItem.OtherAssumption;
                    rfq.NonAwardFeedbackId = rfqItem.NonAwardFeedbackId;
                    rfq.NonAwardDetailedFeedback = rfqItem.NonAwardDetailedFeedback;

                    rfq.SupplierRequirement = rfqItem.SupplierRequirement;
                    rfq.RFQPriorityId = rfqItem.RFQPriorityId;
                    rfq.IndustryTypeId = rfqItem.IndustryTypeId;
                    #endregion
                    #region RFQ Parts List
                    RFQParts rfqparts = new RFQParts();
                    rfq.lstRFQPart = rfqparts.GetRFQPartsList(rfq.Id);
                    #endregion
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.RFQ.RFQ.RFQ>(errMSg, rfq);
            return response;
        }

        public NPE.Core.ITypedResponse<bool?> Delete(string RFQId)
        {
            var RFQToBeDeleted = this.DataContext.RFQs.Where(a => a.Id == RFQId).SingleOrDefault();
            if (RFQToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("RFQNotExists"));
            }
            else
            {
                //TODO: RFQ association check


                DeleteFiles(RFQId);

                RFQParts rfqPartsObj = new RFQParts();
                NPE.Core.ITypedResponse<bool?> ret = rfqPartsObj.DeleteByRFQId(RFQId);
                if (ret.StatusCode == 200)
                {
                    RFQSuppliers rfqSupplierObj = new RFQSuppliers();
                    ret = rfqSupplierObj.DeleteRFQSuppliersbyRFQId(RFQId);
                    if (ret.StatusCode == 200)
                    {
                        //RFQToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                        //RFQToBeDeleted.UpdatedBy = CurrentUser;
                        this.DataContext.Entry(RFQToBeDeleted).State = EntityState.Deleted;

                        this.DataContext.SaveChanges();
                        return SuccessBoolResponse(Languages.GetResourceText("RFQDeletedSuccess"));
                    }
                    else
                        return SuccessBoolResponse(Languages.GetResourceText("RFQDeletedFailure"));
                }
                else
                    return SuccessBoolResponse(Languages.GetResourceText("RFQDeletedFailure"));
            }
        }

        private void DeleteFiles(string RFQId)
        {
            bool isSuccess = false;

            //Step 1: RFQ file attachment
            RFQ rfqObj = new RFQ();
            DTO.Library.RFQ.RFQ.RFQ rfqItem = rfqObj.FindById(RFQId).Result;
            if (!string.IsNullOrEmpty(rfqItem.RfqFilePath))
            {
                isSuccess = Helper.BlobHelper.DeleteBlobfile(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                   , rfqItem.RfqFilePath);
            }
            //Step 2: RFQ parts file attachments
            List<MES.DTO.Library.RFQ.RFQ.RFQParts> rfqPartsLst = rfqItem.lstRFQPart;
            List<MES.DTO.Library.RFQ.RFQ.RFQPartAttachment> partAttachments = null;
            foreach (var parts in rfqPartsLst)
            {
                partAttachments = parts.RfqPartAttachmentList;
                foreach (var attachments in partAttachments)
                {
                    isSuccess = Helper.BlobHelper.DeleteBlobfile(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                    , attachments.AttachmentPathOnServer);
                }
            }


            //Step 3: RFQ file & parts attachment zip file
            string zipFileName = RFQId + ".zip";
            isSuccess = Helper.BlobHelper.DeleteBlobfile(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                    , Constants.RFQAttachmentFolder + zipFileName);

            //RFQ details pdf file
            string pdfFileName = "Rfq_" + RFQId + ".pdf";
            isSuccess = Helper.BlobHelper.DeleteBlobfile(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                   , Constants.RFQAttachmentFolder + pdfFileName);

            //RFQ supplier quote file attachment
            RFQSupplierPartQuote rspqObj = new RFQSupplierPartQuote();

            List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuote> rspqItem = rspqObj.GetRfqSupplierPartsQuote(RFQId).Result;
            foreach (var item in rspqItem)
            {
                isSuccess = Helper.BlobHelper.DeleteBlobfile(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                     , item.QuoteAttachmentFilePath);

            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQ>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQ>> GetRFQList(NPE.Core.IPage<DTO.Library.RFQ.RFQ.SearchCriteria> paging)
        {
            string generated_FileName = string.Empty;
            string filepath = string.Empty;

            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.RFQ.RFQ.RFQ> lstRFQs = new List<DTO.Library.RFQ.RFQ.RFQ>();
            DTO.Library.RFQ.RFQ.RFQ rfqs;
            this.RunOnDB(context =>
            {
                var rfqList = context.SearchRFQs(paging.Criteria.RfqId,
                    paging.Criteria.CompanyName,
                    paging.Criteria.ContactFullName,
                    paging.Criteria.ProjectName,
                    paging.Criteria.WorkType,
                    paging.Criteria.PartNumber,
                    paging.Criteria.rfqCoordinator,
                    paging.Criteria.rfqSource,
                    paging.Criteria.Process,
                    paging.Criteria.Commodity,
                    paging.Criteria.SAM,
                    paging.Criteria.rfqPriority,
                    paging.Criteria.PartDescription,
                    paging.Criteria.RfqDateFrom,
                    paging.Criteria.RfqDateTo,
                    paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                if (rfqList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                    foreach (var item in rfqList)
                    {
                        rfqs = new DTO.Library.RFQ.RFQ.RFQ();
                        rfqs.Id = item.RFQId;
                        rfqs.CustomerName = item.CompanyName;
                        rfqs.CustomerContactName = item.FirstName + " " + item.LastName;
                        rfqs.RFQCoordinator = item.RFQCoordinator;
                        rfqs.RFQType = item.RfqType;
                        rfqs.Commodity = item.CommodityName;
                        rfqs.Date = item.RFQDate;
                        rfqs.QuoteDueDate = item.QuoteDueDate;
                        rfqs.RFQType = item.RfqType;
                        rfqs.QuotesRcvd = item.QuotedCount.HasValue ? item.QuotedCount.Value : 0;
                        rfqs.QuotedSupplier = item.QuotedSupplier;
                        rfqs.Remarks = item.Remarks;
                        rfqs.CustomerId = item.CustomerId;
                        generated_FileName = "Rfq_" + item.RFQId + ".pdf";
                        filepath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"])
                                  + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                                  + Constants.RFQFILESFOLDER
                                  + generated_FileName;
                        rfqs.RfqPDFPath = filepath;
                        /*if (Helper.BlobHelper.CheckFileExists(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                            , filepath))
                        {
                            rfqs.RfqPDFPath = filepath;
                        }
                        else
                            rfqs.RfqPDFPath = string.Empty;*/
                        rfqs.daysOld = item.DaysOld;

                        string statusLegend = string.Empty;
                        /* Value    Status         Code
                         * 0        Blank           
                         * 1        Hold            H
                         * 2        Release         R
                         * 3        Completed       C 
                         * 4        Void            V*/

                        if (!string.IsNullOrEmpty(item.Status))
                        {
                            switch (item.Status.ToUpper())
                            {
                                case "V":
                                    statusLegend = "V";
                                    break;
                                case "C":
                                    statusLegend = "C";
                                    break;
                                case "H":
                                    statusLegend = "H";
                                    break;
                                case "NR":
                                    statusLegend = "NR";
                                    break;
                                case "R":
                                default:
                                    statusLegend = "R";
                                    break;
                            }
                        }
                        else
                        {
                            /*QD = Quote Due
                              IC = Incomplete*/

                            if (item.QuoteDueDate.HasValue && item.QuoteDueDate.Value <= DateTime.Today.AddDays(1))
                            {
                                statusLegend = "QD";
                            }
                            else
                            {
                                int totalrfqSuppliers = 0, supplierQuotesRecieved = 0;
                                RFQSuppliers rfqSupplierObj = new RFQSuppliers();
                                totalrfqSuppliers = rfqSupplierObj.RFQSuppliersCount(item.RFQId).Result;

                                RFQSupplierPartQuote rfqSupPartQuoteObj = new RFQSupplierPartQuote();
                                supplierQuotesRecieved = rfqSupPartQuoteObj.RFQSupplierPartsCount(item.RFQId).Result;

                                if (totalrfqSuppliers == supplierQuotesRecieved)
                                {

                                }
                                else
                                    statusLegend = "IC";
                            }
                        }
                        rfqs.StatusLegend = statusLegend;

                        /*RFQSuppliers rsObj = new RFQSuppliers();
                        List<DTO.Library.RFQ.RFQ.RFQSuppliers> lstQuotedSups = rsObj.GetQuotedSuppliers(rfqs.Id).Result;
                        rfqs.lstQuotedSuppliers = lstQuotedSups;


                        Quote.Quote qObj = new Quote.Quote();
                        List<MES.DTO.Library.RFQ.Quote.Quote> lstQuoteToCust = qObj.GetQuotesToCustomer(rfqs.Id).Result;
                        rfqs.lstQuoteToCustomer = lstQuoteToCust;
                        */
                        lstRFQs.Add(rfqs);
                    }
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQ>>(errMSg, lstRFQs);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }
        /// <summary>
        /// Send RFQ Close Out Email from RFQ last step.
        /// </summary>      
        /// <returns></returns>
        public NPE.Core.ITypedResponse<bool?> SendRFQCloseOutEmail(MES.DTO.Library.Common.EmailData emailData)
        {
            bool IsSuccess = false;
            try
            {
                MES.Business.Repositories.Email.IEmailRepository emailRepository = new MES.Business.Library.BO.Email.Email();
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
                //create a list for To address
                List<string> lstToAddress = new List<string>();
                foreach (var item in emailData.EmailIdsList)
                {
                    lstToAddress.Add(item);
                }
                //create list for the Bcc emails
                List<string> lstBCCEmail = new List<string>();

                if (!string.IsNullOrEmpty(emailData.BCCEmailIds))
                {
                    foreach (var item in emailData.BCCEmailIds.Split(','))
                    {
                        if (!string.IsNullOrEmpty(item.ToString()))
                            lstBCCEmail.Add(item.ToString());
                    }
                }

                //20170222:  MESHA-43
                List<string> lstCCEmail = new List<string>();
                IUserManagementRepository userObj = new UserManagement.UserManagement();
                LoginUser usrInfo = userObj.GetCurrentUserInfo().Result;
                if (!string.IsNullOrEmpty(usrInfo.Email))
                    lstCCEmail.Add(usrInfo.Email);

                List<System.Net.Mail.Attachment> Attachments = new List<System.Net.Mail.Attachment>();
                if (emailData.lstEmailDocumentAttachment != null && emailData.lstEmailDocumentAttachment.Count > 0)
                {
                    foreach (var item in emailData.lstEmailDocumentAttachment)
                    {
                        if (!string.IsNullOrEmpty(item.FilePath) && !string.IsNullOrEmpty(item.FileName))
                        {
                            Stream memoryStream = MES.Business.Library.Helper.BlobHelper.AttachFile_ConvertBlobToStream(item.FilePath);
                            Attachments.Add(new System.Net.Mail.Attachment(memoryStream, item.FileName.Replace(":", string.Empty)
                                                                           .Replace("\\", string.Empty)
                                                                           .Replace("/", string.Empty)
                                                                           .Replace("|", string.Empty)
                                                                           .Replace("*", string.Empty)
                                                                           .Replace("<", string.Empty)
                                                                           .Replace("?", string.Empty)
                                                                           .Replace("\"", string.Empty)
                                                                           .Replace(">", string.Empty)));
                        }
                    }
                }
                emailData.EmailBody = emailData.EmailBodyRFQCloseout;
                emailRepository.SendEmail(lstToAddress, "", emailData.EmailSubject, emailData.EmailBody + footer, out IsSuccess, Attachments, lstCCEmail, lstBCCEmail);
            }
            catch (Exception ex)
            {
            }
            if (IsSuccess)
                return SuccessBoolResponse(Languages.GetResourceText("SendEmailSuccess"));
            else
                return FailedBoolResponse(Languages.GetResourceText("SendEmailFail"));
        }

        //public void Att()
        //{
        //    if (attachments.Count > 0)
        //    {
        //        var localResouece = RoleEnvironment.GetLocalResource("FileStorage");
        //        string clientDirectory = localResouece.RootPath + "\\" + client.ClientId.ToString().PadLeft(10, '0');
        //        if (!Directory.Exists(clientDirectory))
        //        {
        //            Directory.CreateDirectory(clientDirectory);
        //        }
        //        Array.ForEach(Directory.GetFiles(clientDirectory), File.Delete);
        //        //attachments.ForEach(attachment => AttachFile_SendGrid(client.ClientId, emailAttachmentList, attachment));

        //        string filePath = string.Empty;
        //        foreach (var item in attachments)
        //        {

        //            Stream memoryStream = AttachFile_SendGrid(client.ClientId, item);
        //            //emailMessage.AddAttachment(item.FileUrl);
        //            emailMessage.AddAttachment(memoryStream, item.FileName);
        //        }
        //    }
        //}
    }
}
