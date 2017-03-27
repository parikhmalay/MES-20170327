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
using System.IO;
using MES.Business.Repositories.RFQ.Quote;
using Account.DTO.Library;
using EvoPdf.HtmlToPdf;
using GemBox.Spreadsheet;
using System.Net.Mail;
using MES.Business.Mapping.Extensions;

namespace MES.Business.Library.BO.RFQ.Quote
{
    public class Quote : ContextBusinessBase, IQuoteRepository
    {
        public Quote()
            : base("Quote")
        { }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.Quote.Quote>> GetQuoteList(NPE.Core.IPage<DTO.Library.RFQ.Quote.SearchCriteria> paging)
        {
            var httpcontext = HttpContext.Current;

            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.RFQ.Quote.Quote> lstQuotes = new List<DTO.Library.RFQ.Quote.Quote>();
            DTO.Library.RFQ.Quote.Quote quotes;
            this.RunOnDB(context =>
            {
                var quoteList = context.SearchQuotes(paging.Criteria.QuoteId,
                    paging.Criteria.RfqId,
                    paging.Criteria.CustomerName,
                    paging.Criteria.ContactFullName,
                    paging.Criteria.rfqCoordinator,
                    paging.Criteria.SAM,
                    paging.Criteria.QuoteDateFrom,
                    paging.Criteria.QuoteDateTo,
                    paging.Criteria.PartNumber,
                    paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                if (quoteList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                    foreach (var item in quoteList)
                    {
                        quotes = new DTO.Library.RFQ.Quote.Quote();
                        quotes.Id = item.Id;
                        quotes.QuoteNumber = item.QuoteNumber;
                        quotes.StatusId = item.Status;
                        quotes.RfqId = item.RFQId;
                        quotes.SAMId = item.SAMId;
                        quotes.CompanyName = item.CompanyName;
                        quotes.ContactFullName = item.ContactFullName;
                        quotes.Date = item.Date;
                        quotes.GeneralAssumption = item.GeneralAssumption;
                        quotes.CreatedDate = item.createddate;

                        string filepath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"])
                                   + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                                   + Constants.QUOTEFILEPATH
                                   + item.QuoteFilePath;
                        quotes.QuoteFilePath = filepath;
                        /* if (Helper.BlobHelper.CheckFileExists(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                             , filepath))
                         {
                             quotes.QuoteFilePath = filepath;
                         }
                         else
                             quotes.QuoteFilePath = string.Empty;*/

                        filepath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"])
                                   + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                                   + Constants.QUOTEFILEPATH
                                   + item.ExtQuoteFilePath;
                        quotes.ExtQuoteFilePath = filepath;
                        /*if (Helper.BlobHelper.CheckFileExists(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                          , filepath))
                        {
                            quotes.ExtQuoteFilePath = filepath;
                        }
                        else
                            quotes.ExtQuoteFilePath = string.Empty;*/

                        quotes.Comments = item.MESComments;

                        string statusLegend = string.Empty;
                        /* Value    Status           Code                        
                         * 1        Quoted           Q
                         * 2        Full Win         F
                         * 3        Partial Win      P 
                         * 4        Loss             L
                         * 5        Cancelled        C
                         * 6        Obsolete         O*/

                        if (item.Status > 0)
                        {
                            switch (item.Status)
                            {
                                case 1:
                                    statusLegend = "Q";
                                    break;
                                case 2:
                                    statusLegend = "F";
                                    break;
                                case 3:
                                    statusLegend = "P";
                                    break;
                                case 4:
                                    statusLegend = "L";
                                    break;
                                case 5:
                                    statusLegend = "C";
                                    break;
                                case 6:
                                    statusLegend = "O";
                                    break;
                            }
                        }

                        quotes.StatusLegend = statusLegend;
                        lstQuotes.Add(quotes);
                    }
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.Quote.Quote>>(errMSg, lstQuotes);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }

        public NPE.Core.ITypedResponse<string> InsertQuote(DTO.Library.RFQ.Quote.Quote quoteData)
        {
            string errMSg = null;
            string successMsg = null;
            //set the out put param
            ObjectParameter ErrorNumber = new ObjectParameter("ErrorNumber", 0);
            ObjectParameter QuoteId = new ObjectParameter("QuoteId", 0);

            if (string.IsNullOrEmpty(quoteData.QuoteFilePath))
                quoteData.QuoteFilePath = "Quote_" + quoteData.QuoteNumber + ".pdf";

            if (string.IsNullOrEmpty(quoteData.ExtQuoteFilePath))
                quoteData.ExtQuoteFilePath = "MES Quote " + quoteData.QuoteNumber + ".pdf";

            this.RunOnDB(context =>
            {
                int result = context.InsertQuote(quoteData.QuoteNumber
                    , quoteData.pQuoteId
                    , quoteData.RfqId
                    , quoteData.SAMId
                    , quoteData.GeneralAssumption
                    , quoteData.Revision
                    , CurrentUser
                    , quoteData.CustomDutiesPercent
                    , quoteData.SalesCommissionPercent
                    , quoteData.SGAProfitPercent
                    , quoteData.ShippingCostPercent
                    , quoteData.SupplierCost
                    , quoteData.WarehousingPercent
                    , quoteData.ToolingCostPercent
                    , quoteData.ShippingCostCalMethod
                    , quoteData.StatusId
                    , quoteData.AmountWon
                    , quoteData.Amount
                    , "Quote_" + quoteData.QuoteNumber + ".pdf"
                    , "MES Quote " + quoteData.QuoteNumber + ".pdf"
                    , quoteData.MESComments
                    , ErrorNumber
                    , QuoteId
                    );

                if (Convert.ToInt32(QuoteId.Value) > 0)
                {
                    quoteData.Id = Convert.ToString(QuoteId.Value);

                    if (quoteData.lstQuoteDetails != null && quoteData.lstQuoteDetails.Count > 0)
                    {
                        QuoteDetails objQuoteDetails = null;

                        foreach (var qdObject in quoteData.lstQuoteDetails)
                        {
                            objQuoteDetails = new QuoteDetails();
                            qdObject.QuoteId = quoteData.Id;
                            objQuoteDetails.InsertQuoteDetails(qdObject);
                        }
                    }

                }
                successMsg = Languages.GetResourceText("QuoteSavedSuccess");
            });

            return SuccessOrFailedResponse<string>(errMSg, quoteData.Id, successMsg);
        }

        private void SendEmailWithExtPDF(DTO.Library.RFQ.Quote.Quote quoteData)
        {
            MES.DTO.Library.Common.EmailData emailData = null;

            bool IsSuccess = false;
            try
            {
                emailData = new DTO.Library.Common.EmailData();
                MES.Business.Repositories.Email.IEmailRepository emailRepository = new MES.Business.Library.BO.Email.Email();
                emailData.EmailBody = "Hello,<br /><br />MES Comments - " + (!string.IsNullOrEmpty(quoteData.MESComments) ? quoteData.MESComments : "-");
                string footer = @"<br /><br /><br /><hr />
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

                //Get SAM
                string samEmail = string.Empty;
                if (!string.IsNullOrEmpty(quoteData.SAMId))
                {
                    UserManagement.UserManagement userObj = new UserManagement.UserManagement();
                    MES.DTO.Library.Identity.LoginUser userInfo = userObj.FindById(quoteData.SAMId).Result;
                    samEmail = userInfo.Email;
                    lstToAddress.Add(samEmail);
                }
                //Get RfqCoordinator
                string rfqCoordinatorEmail = string.Empty;
                if (!string.IsNullOrEmpty(quoteData.RfqId))
                {
                    RFQ.RFQ rfqObj = new RFQ.RFQ();
                    DTO.Library.RFQ.RFQ.RFQ rfqInfo = rfqObj.FindById(quoteData.RfqId).Result;
                    if (!string.IsNullOrEmpty(rfqInfo.RFQCoordinatorId))
                    {
                        UserManagement.UserManagement userObj = new UserManagement.UserManagement();
                        MES.DTO.Library.Identity.LoginUser userInfo = userObj.FindById(rfqInfo.RFQCoordinatorId).Result;
                        if (userInfo != null)
                            rfqCoordinatorEmail = userInfo.Email;

                        lstToAddress.Add(rfqCoordinatorEmail);
                    }
                }
                //Get Managers
                string managersEmail = System.Configuration.ConfigurationManager.AppSettings["MESAccountManagers"].ToString();

                //create list for the cc emails
                List<string> lstCCEmail = new List<string>();
                foreach (var item in managersEmail.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.ToString()))
                        lstCCEmail.Add(item.ToString());
                }

                //TODO: Attachment-Ext Quote PDF
                List<Attachment> attachments = new List<Attachment>();
                if (!string.IsNullOrEmpty(quoteData.ExtQuoteFilePath))
                {
                    string filepath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"])
                               + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                               + Constants.QUOTEFILEPATH
                               + quoteData.ExtQuoteFilePath;

                    if (Helper.BlobHelper.CheckFileExists(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                      , filepath))
                    {
                        Stream memoryStream = MES.Business.Library.Helper.BlobHelper.AttachFile_ConvertBlobToStream(filepath);
                        attachments.Add(new System.Net.Mail.Attachment(memoryStream, quoteData.ExtQuoteFilePath));
                    }
                }
                emailData.EmailSubject = "Quote for Customer - " + quoteData.ContactFullName;
                emailRepository.SendEmail(lstToAddress, "", emailData.EmailSubject, emailData.EmailBody + footer, out IsSuccess, attachments, lstCCEmail, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Updates the quote details.
        /// </summary>
        /// <param name="quoteData">The quote data.</param>
        /// <returns></returns>
        public NPE.Core.ITypedResponse<string> UpdateQuote(DTO.Library.RFQ.Quote.Quote quoteData)
        {
            string errMSg = null;
            string successMsg = null;
            //set the out put param
            ObjectParameter ErrorNumber = new ObjectParameter("ErrorNumber", 0);
            ObjectParameter QuoteId = new ObjectParameter("QuoteId", 0);

            if (string.IsNullOrEmpty(quoteData.QuoteFilePath))
                quoteData.QuoteFilePath = "Quote_" + quoteData.QuoteNumber + ".pdf";

            if (string.IsNullOrEmpty(quoteData.ExtQuoteFilePath))
                quoteData.ExtQuoteFilePath = "MES Quote" + quoteData.QuoteNumber + ".pdf";

            this.RunOnDB(context =>
            {
                int result = context.UpdateQuote(quoteData.Id
                    , quoteData.QuoteNumber
                    , quoteData.SAMId
                    , quoteData.GeneralAssumption
                    , CurrentUser
                    , quoteData.CustomDutiesPercent
                    , quoteData.SalesCommissionPercent
                    , quoteData.SGAProfitPercent
                    , quoteData.ShippingCostPercent
                    , quoteData.SupplierCost
                    , quoteData.WarehousingPercent
                    , quoteData.ToolingCostPercent
                    , quoteData.ShippingCostCalMethod
                    , quoteData.StatusId
                    , quoteData.AmountWon
                    , quoteData.Amount
                    , quoteData.QuoteFilePath
                    , quoteData.ExtQuoteFilePath
                    , quoteData.MESComments
                    , ErrorNumber
                    );

                if (string.IsNullOrEmpty(ErrorNumber.Value.ToString()))
                {
                    if (quoteData.lstQuoteDetails != null && quoteData.lstQuoteDetails.Count > 0)
                    {
                        QuoteDetails objQuoteDetails = null;

                        foreach (var qdObject in quoteData.lstQuoteDetails)
                        {
                            objQuoteDetails = new QuoteDetails();
                            qdObject.QuoteId = quoteData.Id;

                            if (qdObject.Id > 0)
                                objQuoteDetails.UpdateQuoteDetails(qdObject);
                            else
                                objQuoteDetails.InsertQuoteDetails(qdObject);
                        }
                    }

                }
                successMsg = Languages.GetResourceText("QuoteUpdatedSuccess");
            });

            return SuccessOrFailedResponse<string>(errMSg, quoteData.Id, successMsg);
        }

        /// <summary>
        /// Creates the quote PDF.
        /// </summary>
        /// <param name="quoteData">The quote data.</param>
        private void CreateQuotePdf(DTO.Library.RFQ.Quote.Quote quoteData)
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

            string contents = File.ReadAllText(context.Server.MapPath("~/EmailTemplates/QuoteTemplate.htm"));

            //// Replace the placeholders with the user-specified text
            contents = contents.Replace("<%RFQNo%>", quoteData.quotesGeneralDetails.RfqId);
            contents = contents.Replace("<%QuoteNo%>", quoteData.quotesGeneralDetails.QuoteNumber);
            contents = contents.Replace("<%ProjectName%>", quoteData.quotesGeneralDetails.ProjectName);
            contents = contents.Replace("<%SAMName%>", quoteData.quotesGeneralDetails.SAMUser);
            contents = contents.Replace("<%Date%>", !quoteData.quotesGeneralDetails.Date.HasValue ? string.Empty : Convert.ToDateTime(quoteData.quotesGeneralDetails.Date).ToString("dd-MMM-yy"));
            contents = contents.Replace("<%Customer%>", quoteData.quotesGeneralDetails.CompanyName);
            contents = contents.Replace("<%CustomerContact%>", quoteData.quotesGeneralDetails.ContactFullName);
            if (quoteData.quotesGeneralDetails.ShippingCostPercent != null && quoteData.quotesGeneralDetails.ShippingCostPercent > 0)
            {
                if (quoteData.quotesGeneralDetails.ShippingCostCalMethod == "%")
                    contents = contents.Replace("<%ShippingCostCalMethod%>", " (% of SP)");
                else
                    contents = contents.Replace("<%ShippingCostCalMethod%>", " (per kg)");
            }
            if (!string.IsNullOrEmpty(quoteData.quotesGeneralDetails.GeneralAssumption))
            {
                string assumptions = quoteData.quotesGeneralDetails.GeneralAssumption.Replace("\n", "<br />");
                contents = contents.Replace("<%Assumptions%>", assumptions);
            }

            if (!string.IsNullOrEmpty(quoteData.quotesGeneralDetails.MESComments))
            {
                string comments = quoteData.quotesGeneralDetails.MESComments.Replace("\n", "<br />");
                contents = contents.Replace("<%Comments%>", comments);
            }

            var partlist = quoteData.lstQuoteDetails;
            string itemsTable = string.Empty;
            string calcPercentHistoryTable = string.Empty;

            bool hasHeader = false;

            if (partlist.Count > 0)
            {
                int counter = 0;
                decimal totalTooling = 0, TAC = 0;



                foreach (MES.DTO.Library.RFQ.Quote.QuoteDetails item in partlist)
                {
                    if (partlist.Count == counter)
                        break;
                    if (item.chkSelect)
                    {
                        if (counter % 2 == 0)
                        {
                            itemsTable += string.Format(@"<tr>
                                                <td bgcolor='#ffffff'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{0}</font></td>
                                                <td bgcolor='#ffffff'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{1}</font></td>
                                                <td bgcolor='#ffffff'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{2}</font></td>
                                                <td bgcolor='#ffffff'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{3}</font></td>
                                                <td bgcolor='#ffffff' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{4}</font></td>
                                                <td bgcolor='#ffffff' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{5}</font></td>
                                                <td bgcolor='#ffffff' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{6}</font></td>
                                                <td bgcolor='#ffffff' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{7}</font></td>
                                                <td bgcolor='#ffffff' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{8}</font></td>
                                                <td bgcolor='#ffffff' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{9}</font></td>
                                                <td bgcolor='#ffffff'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{10}</font></td>
                                                <td bgcolor='#ffffff' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{11}</font></td>
                                                <td bgcolor='#ffffff' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{12}</font></td>
                                                <td bgcolor='#ffffff' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{13}</font></td>
                                                <td bgcolor='#ffffff' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{14}</font></td>
                                                <td bgcolor='#ffffff' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{15}</font></td>
                                                <td bgcolor='#ffffff' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{16}</font></td>
                                                <td bgcolor='#ffffff' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{17}</font></td>                                                
                                                <td bgcolor='#ffffff' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{18}</font></td>                                                
                                                <td bgcolor='#ffffff' style='border-right: 1px solid #000;'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{19}</font></td>                                                
                                                </tr>"
                                    , item.CustomerPartNo
                                    , item.RevLevel
                                    , item.PartDescription + "<br />" + item.AdditionalPartDesc
                                    , item.MaterialType
                                    , item.EstimatedQty
                                    , item.SupplierQuotedPrice.HasValue && (item.SupplierQuotedPrice.ToString() != "0" || item.SupplierQuotedPrice.ToString() != "0.000") ? "$" + item.SupplierQuotedPrice.Value.ToString("#,##0.000") : item.SupplierQuotedPrice.ToString()
                                    , item.SupplierPriceUsed.HasValue && (item.SupplierPriceUsed.ToString() != "0" || item.SupplierPriceUsed.ToString() != "0.000") ? "$" + item.SupplierPriceUsed.Value.ToString("#,##0.000") : item.SupplierPriceUsed.ToString()
                                    , item.SupplierCostPerKg.HasValue && (item.SupplierCostPerKg.ToString() != "0" || item.SupplierCostPerKg.ToString() != "0.000") ? "$" + item.SupplierCostPerKg.Value.ToString("#,##0.000") : item.SupplierCostPerKg.ToString()
                                    , item.CustomDuties.HasValue && (item.CustomDuties.ToString() != "0" || item.CustomDuties.ToString() != "0.000") ? "$" + item.CustomDuties.Value.ToString("#,##0.000") : item.CustomDuties.ToString()
                                    , item.PartWeightKG.HasValue ? item.PartWeightKG.Value.ToString("#,##0.000") : item.PartWeightKG.ToString()
                                    , (!string.IsNullOrEmpty(item.ShippingAssumption) ? item.ShippingAssumption.Replace("\n", "<br />") : string.Empty)
                                    , item.ShippingCost.HasValue && (item.ShippingCost.ToString() != "0" || item.ShippingCost.ToString() != "0.000") ? "$" + item.ShippingCost.Value.ToString("#,##0.000") : item.ShippingCost.ToString()
                                    , item.Warehousing.HasValue && (item.Warehousing.ToString() != "0" || item.Warehousing.ToString() != "0.000") ? "$" + item.Warehousing.Value.ToString("#,##0.000") : item.Warehousing.ToString()
                                    , item.SGAProfit.HasValue && (item.SGAProfit.ToString() != "0" || item.SGAProfit.ToString() != "0.000") ? "$" + item.SGAProfit.Value.ToString("#,##0.000") : item.SGAProfit.ToString()
                                    , item.SalesCommission.HasValue && (item.SalesCommission.ToString() != "0" || item.SalesCommission.ToString() != "0.000") ? "$" + item.SalesCommission.Value.ToString("#,##0.000") : item.SalesCommission.ToString()
                                    , item.FinalMESPrice.HasValue && (item.FinalMESPrice.ToString() != "0" || item.FinalMESPrice.ToString() != "0.000") ? "$" + item.FinalMESPrice.Value.ToString("#,##0.000") : item.FinalMESPrice.ToString()
                                    , item.FinalMESPerKg.HasValue && (item.FinalMESPerKg.ToString() != "0" || item.FinalMESPerKg.ToString() != "0.000") ? "$" + item.FinalMESPerKg.Value.ToString("#,##0.000") : item.FinalMESPerKg.ToString()
                                    , item.ToolingCost.HasValue && (item.ToolingCost.ToString() != "0" || item.ToolingCost.ToString() != "0.000") ? "$" + item.ToolingCost.Value.ToString("#,##0.000") : item.ToolingCost.ToString()
                                    , item.TotalAnnualCost.HasValue && (item.TotalAnnualCost.ToString() != "0" || item.TotalAnnualCost.ToString() != "0.000") ? "$" + item.TotalAnnualCost.Value.ToString("#,##0.000") : item.TotalAnnualCost.ToString()
                                    , (!string.IsNullOrEmpty(item.Leadtime) ? item.Leadtime.Replace("\n", "<br />") : string.Empty)
                                    );
                            totalTooling += (item.ToolingCost.HasValue ? item.ToolingCost.Value : 0);
                            TAC += (item.TotalAnnualCost.HasValue ? item.TotalAnnualCost.Value : 0);
                        }
                        else
                        {
                            itemsTable += string.Format(@"<tr>
                                                <td bgcolor='#f6f4f2'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{0}</font></td>
                                                <td bgcolor='#f6f4f2'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{1}</font></td>
                                                <td bgcolor='#f6f4f2'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{2}</font></td>
                                                <td bgcolor='#f6f4f2'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{3}</font></td>
                                                <td bgcolor='#f6f4f2' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{4}</font></td>
                                                <td bgcolor='#f6f4f2' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{5}</font></td>
                                                <td bgcolor='#f6f4f2' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{6}</font></td>
                                                <td bgcolor='#f6f4f2' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{7}</font></td>
                                                <td bgcolor='#f6f4f2' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{8}</font></td>
                                                <td bgcolor='#f6f4f2' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{9}</font></td>
                                                <td bgcolor='#f6f4f2'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{10}</font></td>
                                                <td bgcolor='#f6f4f2' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{11}</font></td>
                                                <td bgcolor='#f6f4f2' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{12}</font></td>
                                                <td bgcolor='#f6f4f2' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{13}</font></td>
                                                <td bgcolor='#f6f4f2' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{14}</font></td>
                                                <td bgcolor='#f6f4f2' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{15}</font></td>
                                                <td bgcolor='#f6f4f2' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{16}</font></td>
                                                <td bgcolor='#f6f4f2' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{17}</font></td>                                                
                                                <td bgcolor='#f6f4f2' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{18}</font></td>
                                                <td bgcolor='#f6f4f2' style='border-right: 1px solid #000;'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{19}</font></td>    
                                                </tr>"
                                    , item.CustomerPartNo
                                    , item.RevLevel
                                    , item.PartDescription + "<br />" + item.AdditionalPartDesc
                                    , item.MaterialType
                                    , item.EstimatedQty
                                    , item.SupplierQuotedPrice.HasValue && (item.SupplierQuotedPrice.ToString() != "0" || item.SupplierQuotedPrice.ToString() != "0.000") ? "$" + item.SupplierQuotedPrice.Value.ToString("#,##0.000") : item.SupplierQuotedPrice.ToString()
                                    , item.SupplierPriceUsed.HasValue && (item.SupplierPriceUsed.ToString() != "0" || item.SupplierPriceUsed.ToString() != "0.000") ? "$" + item.SupplierPriceUsed.Value.ToString("#,##0.000") : item.SupplierPriceUsed.ToString()
                                    , item.SupplierCostPerKg.HasValue && (item.SupplierCostPerKg.ToString() != "0" || item.SupplierCostPerKg.ToString() != "0.000") ? "$" + item.SupplierCostPerKg.Value.ToString("#,##0.000") : item.SupplierCostPerKg.ToString()
                                    , item.CustomDuties.HasValue && (item.CustomDuties.ToString() != "0" || item.CustomDuties.ToString() != "0.000") ? "$" + item.CustomDuties.Value.ToString("#,##0.000") : item.CustomDuties.ToString()
                                    , item.PartWeightKG.HasValue ? item.PartWeightKG.Value.ToString("#,##0.000") : item.PartWeightKG.ToString()
                                    , (!string.IsNullOrEmpty(item.ShippingAssumption) ? item.ShippingAssumption.Replace("\n", "<br />") : string.Empty)
                                    , item.ShippingCost.HasValue && (item.ShippingCost.ToString() != "0" || item.ShippingCost.ToString() != "0.000") ? "$" + item.ShippingCost.Value.ToString("#,##0.000") : item.ShippingCost.ToString()
                                    , item.Warehousing.HasValue && (item.Warehousing.ToString() != "0" || item.Warehousing.ToString() != "0.000") ? "$" + item.Warehousing.Value.ToString("#,##0.000") : item.Warehousing.ToString()
                                    , item.SGAProfit.HasValue && (item.SGAProfit.ToString() != "0" || item.SGAProfit.ToString() != "0.000") ? "$" + item.SGAProfit.Value.ToString("#,##0.000") : item.SGAProfit.ToString()
                                    , item.SalesCommission.HasValue && (item.SalesCommission.ToString() != "0" || item.SalesCommission.ToString() != "0.000") ? "$" + item.SalesCommission.Value.ToString("#,##0.000") : item.SalesCommission.ToString()
                                    , item.FinalMESPrice.HasValue && (item.FinalMESPrice.ToString() != "0" || item.FinalMESPrice.ToString() != "0.000") ? "$" + item.FinalMESPrice.Value.ToString("#,##0.000") : item.FinalMESPrice.ToString()
                                    , item.FinalMESPerKg.HasValue && (item.FinalMESPerKg.ToString() != "0" || item.FinalMESPerKg.ToString() != "0.000") ? "$" + item.FinalMESPerKg.Value.ToString("#,##0.000") : item.FinalMESPerKg.ToString()
                                    , item.ToolingCost.HasValue && (item.ToolingCost.ToString() != "0" || item.ToolingCost.ToString() != "0.000") ? "$" + item.ToolingCost.Value.ToString("#,##0.000") : item.ToolingCost.ToString()
                                    , item.TotalAnnualCost.HasValue && (item.TotalAnnualCost.ToString() != "0" || item.TotalAnnualCost.ToString() != "0.000") ? "$" + item.TotalAnnualCost.Value.ToString("#,##0.000") : item.TotalAnnualCost.ToString()
                                    , (!string.IsNullOrEmpty(item.Leadtime) ? item.Leadtime.Replace("\n", "<br />") : string.Empty)
                                    );
                            totalTooling += (item.ToolingCost.HasValue ? item.ToolingCost.Value : 0);
                            TAC += (item.TotalAnnualCost.HasValue ? item.TotalAnnualCost.Value : 0);
                        }
                        #region lstQuoteCalculationHistory
                        var calcPercentlist = quoteData.lstQuoteCalculationHistory;
                        if (calcPercentlist.Count > 0)
                        {
                            if (!hasHeader)
                            {
                                calcPercentHistoryTable += @"<tr><td bgcolor='#ffffff' style='border-right: 1px solid #000;' colspan='20'>Calculation History</td></tr><tr><td style='border: 0px' colspan='20'><table cellpadding='2' cellspacing='0' width='99%' style='margin-top: -2px;'><tr>
                                <td align='left' bgcolor='#EBFFD6'>
                                    <font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'><b>Part No.</b></font>
                                </td>
                                <td align='left' bgcolor='#EBFFD6'>
                                    <font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'><b>Description</b></font>
                                </td>
                                <td align='center' bgcolor='#EBFFD6'>
                                    <font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'><b>History Date</b></font>
                                </td>
                                <td align='center' bgcolor='#EBFFD6'>
                                    <font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'><b>Custom Duties</b></font>
                                </td>
                                <td align='center' bgcolor='#EBFFD6'>
                                    <font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'><b>Shipping Cost/Pc</b></font>
                                </td>
                                <td align='center' bgcolor='#EBFFD6'>
                                    <font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'><b>Warehousing</b></font>
                                </td>
                                <td align='center' bgcolor='#EBFFD6'>
                                    <font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'><b>S, G, A & Profit</b></font>
                                </td>
                                <td align='center' bgcolor='#EBFFD6'>
                                    <font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'><b>Sales Commission</b></font>
                                </td>
                                <td align='center' bgcolor='#EBFFD6'style='border-right: 1px solid #000;'>
                                    <font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'><b>Tooling</b></font>
                                </td>
                                </tr>";
                                hasHeader = true;
                            }
                            foreach (MES.DTO.Library.RFQ.Quote.QuoteCalculationHistory hItem in calcPercentlist.Where(i => i.PartId == item.PartId))
                            {
                                if (calcPercentlist.Count == counter)
                                    break;

                                if (counter % 2 == 0)
                                {
                                    calcPercentHistoryTable += string.Format(@"<tr>
                                                 <td bgcolor='#ffffff'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{0}</font></td>
                                                 <td bgcolor='#ffffff'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{1}</font></td>
                                                 <td bgcolor='#ffffff' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{2}</font></td>
                                                 <td bgcolor='#ffffff' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{3}</font></td>
                                                 <td bgcolor='#ffffff' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{4}</font></td>
                                                 <td bgcolor='#ffffff' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{5}</font></td>
                                                 <td bgcolor='#ffffff' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{6}</font></td>
                                                 <td bgcolor='#ffffff' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{7}</font></td>
                                                 <td bgcolor='#ffffff' align='center' style='border-right: 1px solid #000;'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{8}</font></td></tr>"
                                                     , item.CustomerPartNo
                                                     , item.PartDescription
                                                     , hItem.UpdatedDate
                                                     , hItem.CustomDutiesPercent + (hItem.CustomDutiesPercent <= 0 ? string.Empty : "%") + " | " + hItem.CustomDuties
                                                     , hItem.ShippingCostPercent + (hItem.ShippingCostPercent <= 0 ? string.Empty : "%") + " | " + hItem.ShippingCost
                                                     , hItem.WarehousingPercent + (hItem.WarehousingPercent <= 0 ? string.Empty : "%") + " | " + hItem.Warehousing
                                                     , hItem.SGAProfitPercent + (hItem.SGAProfitPercent <= 0 ? string.Empty : "%") + " | " + hItem.SGAProfit
                                                     , hItem.SalesCommissionPercent + (hItem.SalesCommissionPercent <= 0 ? string.Empty : "%") + " | " + hItem.SalesCommission
                                                     , hItem.ToolingCostPercent + (hItem.ToolingCostPercent <= 0 ? string.Empty : "%") + " | " + hItem.ToolingCost
                                                     );
                                }
                                else
                                {
                                    calcPercentHistoryTable += string.Format(@"<tr>
                                                <td bgcolor='#f6f4f2'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{0}</font></td>
                                                <td bgcolor='#f6f4f2'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{1}</font></td>
                                                <td bgcolor='#f6f4f2' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{2}</font></td>
                                                <td bgcolor='#f6f4f2' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{3}</font></td>
                                                <td bgcolor='#f6f4f2' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{4}</font></td>
                                                <td bgcolor='#f6f4f2' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{5}</font></td>
                                                <td bgcolor='#f6f4f2' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{6}</font></td>
                                                <td bgcolor='#f6f4f2' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{7}</font></td>
                                                <td bgcolor='#f6f4f2' align='center' style='border-right: 1px solid #000;'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{8}</font></td></tr>"
                                                    , item.CustomerPartNo
                                                    , item.PartDescription
                                                    , hItem.UpdatedDate
                                                    , hItem.CustomDutiesPercent + (hItem.CustomDutiesPercent <= 0 ? string.Empty : "%") + " | " + hItem.CustomDuties
                                                    , hItem.ShippingCostPercent + (hItem.ShippingCostPercent <= 0 ? string.Empty : "%") + " | " + hItem.ShippingCost
                                                    , hItem.WarehousingPercent + (hItem.WarehousingPercent <= 0 ? string.Empty : "%") + " | " + hItem.Warehousing
                                                    , hItem.SGAProfitPercent + (hItem.SGAProfitPercent <= 0 ? string.Empty : "%") + " | " + hItem.SGAProfit
                                                    , hItem.SalesCommissionPercent + (hItem.SalesCommissionPercent > 0 ? string.Empty : "%") + " | " + hItem.SalesCommission
                                                    , hItem.ToolingCostPercent + (hItem.ToolingCostPercent <= 0 ? string.Empty : "%") + " | " + hItem.ToolingCost
                                                    );
                                }
                            }

                        }
                        #endregion

                    }
                    counter++;
                }
                if (!string.IsNullOrEmpty(calcPercentHistoryTable))
                    calcPercentHistoryTable += "</table></td></tr>";

                contents = contents.Replace("<%CalculationPercentHistory%>", calcPercentHistoryTable);

                if (!string.IsNullOrEmpty(itemsTable))
                    itemsTable += @"<tr><td colspan='17' bgcolor='#ffffff'></td>
                                    <td bgcolor='#ffffff'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>$" + totalTooling.ToString("#,##0.000") + @"</font></td>
                                    <td bgcolor='#ffffff'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>$" + TAC.ToString("#,##0.000") + @"</font></td>
                                    <td bgcolor='#ffffff' style='border-right: 1px solid #000;'>&nbsp;</td></tr>";
            }

            contents = contents.Replace("<%PartsList%>", itemsTable);

            // get the pdf bytes from html string
            byte[] pdfBytes = pdfConverter.GetPdfBytesFromHtmlString(contents);

            pdfConverter.PdfDocumentOptions.ShowFooter = true;
            pdfConverter.PdfDocumentOptions.ShowHeader = true;

            pdfConverter.PdfHeaderOptions.HeaderHeight = 75;
            string headerAndFooterHtmlUrl = context.Server.MapPath("~/EmailTemplates/Header.htm");
            pdfConverter.PdfHeaderOptions.HtmlToPdfArea = new HtmlToPdfArea(0, 0, 0, pdfConverter.PdfHeaderOptions.HeaderHeight,
                               headerAndFooterHtmlUrl, 1060, 100);

            pdfConverter.PdfHeaderOptions.DrawHeaderLine = false;

            pdfConverter.PdfFooterOptions.FooterHeight = 20;
            pdfConverter.PdfFooterOptions.DrawFooterLine = false;

            headerAndFooterHtmlUrl = context.Server.MapPath("~/EmailTemplates/Footer.htm");
            pdfConverter.PdfFooterOptions.HtmlToPdfArea = new HtmlToPdfArea(0, 0, 0, pdfConverter.PdfFooterOptions.FooterHeight,
                               headerAndFooterHtmlUrl, 1060, 100);

            pdfConverter.PdfFooterOptions.TextArea = new TextArea(737, 0, "Page &p; of &P; ",
            new System.Drawing.Font(new System.Drawing.FontFamily("Verdana"), 8, System.Drawing.GraphicsUnit.Point));

            string generatedFileName = quoteData.QuoteFilePath;
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
                           + Constants.QUOTEFILEPATH
                           + generatedFileName;

            Helper.BlobHelper.DeleteBlobfile(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), filepath);

            filepath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                          , Constants.QUOTEFILEFOLDER
                          , generatedFileName
                          , tempFilepath);
            File.Delete(tempFilepath);
        }

        private void CreateExtQuotePdf(DTO.Library.RFQ.Quote.Quote quoteData)
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

            string contents = File.ReadAllText(context.Server.MapPath("~/EmailTemplates/ExternalQuoteTemplate.htm"));

            //// Replace the placeholders with the user-specified text
            contents = contents.Replace("<%RFQNo%>", quoteData.quotesGeneralDetails.RfqId);
            contents = contents.Replace("<%QuoteNo%>", quoteData.quotesGeneralDetails.QuoteNumber);
            contents = contents.Replace("<%ProjectName%>", quoteData.quotesGeneralDetails.ProjectName);
            contents = contents.Replace("<%SAMName%>", quoteData.quotesGeneralDetails.SAMUser);
            contents = contents.Replace("<%Date%>", !quoteData.quotesGeneralDetails.Date.HasValue ? string.Empty : Convert.ToDateTime(quoteData.quotesGeneralDetails.Date).ToString("dd-MMM-yy"));
            contents = contents.Replace("<%Customer%>", quoteData.quotesGeneralDetails.CompanyName);
            contents = contents.Replace("<%CustomerContact%>", quoteData.quotesGeneralDetails.ContactFullName);

            if (!string.IsNullOrEmpty(quoteData.quotesGeneralDetails.GeneralAssumption))
            {
                string assumptions = quoteData.quotesGeneralDetails.GeneralAssumption.Replace("\n", "<br />");
                contents = contents.Replace("<%Assumptions%>", assumptions);
            }

            List<DTO.Library.RFQ.Quote.QuoteDetails> partlist = quoteData.lstQuoteDetails;
            string itemsTable = string.Empty;

            if (partlist.Count > 0)
            {
                int counter = 0;

                foreach (DTO.Library.RFQ.Quote.QuoteDetails item in partlist)
                {
                    if (partlist.Count == counter)
                        break;
                    if (item.chkSelect)
                    {
                        if (counter % 2 == 0)
                        {
                            itemsTable += string.Format(@"<tr>
                                                <td bgcolor='#ffffff'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{0}</font></td>
                                                <td bgcolor='#ffffff'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{1}</font></td>
                                                <td bgcolor='#ffffff'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{2}</font></td>
                                                <td bgcolor='#ffffff'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{3}</font></td>
                                                <td bgcolor='#ffffff' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{4}</font></td>
                                                <td bgcolor='#ffffff' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{5}</font></td>                                                
                                                <td bgcolor='#ffffff' align='left'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{6}</font></td>
                                                <td bgcolor='#ffffff' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{7}</font></td>                                                
                                                <td bgcolor='#ffffff' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{8}</font></td>
                                                <td bgcolor='#ffffff' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{9}</font></td>
                                                <td bgcolor='#ffffff' style='border-right: 1px solid #000;'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{10}</font></td>
                                                </tr>"
                                    , item.CustomerPartNo
                                    , item.RevLevel
                                    , item.PartDescription
                                    , item.MaterialType
                                    , item.EstimatedQty
                                    , item.PartWeightKG.HasValue ? item.PartWeightKG.Value.ToString("#,##0.000") : item.PartWeightKG.ToString()
                                    , (!string.IsNullOrEmpty(item.ShippingAssumption) ? item.ShippingAssumption.Replace("\n", "<br />") : string.Empty)
                                    , item.FinalMESPrice.HasValue && (item.FinalMESPrice.ToString() != "0" || item.FinalMESPrice.ToString() != "0.000") ? "$" + item.FinalMESPrice.Value.ToString("#,##0.000") : item.FinalMESPrice.ToString()
                                    , item.ToolingCost.HasValue && (item.ToolingCost.ToString() != "0" || item.ToolingCost.ToString() != "0.000") ? "$" + item.ToolingCost.Value.ToString("#,##0.000") : item.ToolingCost.ToString()
                                    , item.TotalAnnualCost.HasValue && (item.TotalAnnualCost.ToString() != "0" || item.TotalAnnualCost.ToString() != "0.000") ? "$" + item.TotalAnnualCost.Value.ToString("#,##0.000") : item.TotalAnnualCost.ToString()
                                    , (!string.IsNullOrEmpty(item.Leadtime) ? item.Leadtime.Replace("\n", "<br />") : string.Empty)
                                    );

                        }
                        else
                        {
                            itemsTable += string.Format(@"<tr>
                                                <td bgcolor='#f6f4f2'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{0}</font></td>
                                                <td bgcolor='#f6f4f2'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{1}</font></td>
                                                <td bgcolor='#f6f4f2'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{2}</font></td>
                                                <td bgcolor='#f6f4f2'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{3}</font></td>
                                                <td bgcolor='#f6f4f2' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{4}</font></td>
                                                <td bgcolor='#f6f4f2' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{5}</font></td>                                                
                                                <td bgcolor='#f6f4f2' align='left'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{6}</font></td>
                                                <td bgcolor='#f6f4f2' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{7}</font></td>                                                
                                                <td bgcolor='#f6f4f2' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{8}</font></td>
                                                <td bgcolor='#f6f4f2' align='center'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{9}</font></td>
                                                <td bgcolor='#f6f4f2' style='border-right: 1px solid #000;'><font face='Arial, Helvetica, sans-serif;' size='1' color='#2f1c14'>{10}</font></td>
                                                </tr>"
                                    , item.CustomerPartNo
                                    , item.RevLevel
                                    , item.PartDescription
                                    , item.MaterialType
                                    , item.EstimatedQty
                                    , item.PartWeightKG.HasValue ? item.PartWeightKG.Value.ToString("#,##0.000") : item.PartWeightKG.ToString()
                                    , (!string.IsNullOrEmpty(item.ShippingAssumption) ? item.ShippingAssumption.Replace("\n", "<br />") : string.Empty)
                                    , item.FinalMESPrice.HasValue && (item.FinalMESPrice.ToString() != "0" || item.FinalMESPrice.ToString() != "0.000") ? "$" + item.FinalMESPrice.Value.ToString("#,##0.000") : item.FinalMESPrice.ToString()
                                    , item.ToolingCost.HasValue && (item.ToolingCost.ToString() != "0" || item.ToolingCost.ToString() != "0.000") ? "$" + item.ToolingCost.Value.ToString("#,##0.000") : item.ToolingCost.ToString()
                                    , item.TotalAnnualCost.HasValue && (item.TotalAnnualCost.ToString() != "0" || item.TotalAnnualCost.ToString() != "0.000") ? "$" + item.TotalAnnualCost.Value.ToString("#,##0.000") : item.TotalAnnualCost.ToString()
                                    , (!string.IsNullOrEmpty(item.Leadtime) ? item.Leadtime.Replace("\n", "<br />") : string.Empty)
                                    );
                        }
                    }
                    counter++;
                }
            }

            contents = contents.Replace("<%PartsList%>", itemsTable);
            // get the pdf bytes from html string
            byte[] pdfBytes = pdfConverter.GetPdfBytesFromHtmlString(contents);

            pdfConverter.PdfDocumentOptions.ShowFooter = true;
            pdfConverter.PdfDocumentOptions.ShowHeader = true;

            pdfConverter.PdfHeaderOptions.HeaderHeight = 75;
            string headerAndFooterHtmlUrl = context.Server.MapPath("~/EmailTemplates/Header.htm");
            pdfConverter.PdfHeaderOptions.HtmlToPdfArea = new HtmlToPdfArea(0, 0, 0, pdfConverter.PdfHeaderOptions.HeaderHeight,
                               headerAndFooterHtmlUrl, 1060, 100);

            pdfConverter.PdfHeaderOptions.DrawHeaderLine = false;

            pdfConverter.PdfFooterOptions.FooterHeight = 20;
            pdfConverter.PdfFooterOptions.DrawFooterLine = false;

            headerAndFooterHtmlUrl = context.Server.MapPath("~/EmailTemplates/Footer.htm");
            pdfConverter.PdfFooterOptions.HtmlToPdfArea = new HtmlToPdfArea(0, 0, 0, pdfConverter.PdfFooterOptions.FooterHeight,
                               headerAndFooterHtmlUrl, 1060, 100);

            pdfConverter.PdfFooterOptions.TextArea = new TextArea(737, 0, "Page &p; of &P; ",
            new System.Drawing.Font(new System.Drawing.FontFamily("Verdana"), 8, System.Drawing.GraphicsUnit.Point));

            string generatedFileName = quoteData.ExtQuoteFilePath;
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
                             + Constants.QUOTEFILEPATH
                             + generatedFileName;

            Helper.BlobHelper.DeleteBlobfile(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]), filepath);

            filepath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                          , Constants.QUOTEFILEFOLDER
                          , generatedFileName
                          , tempFilepath);
            File.Delete(tempFilepath);
        }
        public NPE.Core.ITypedResponse<bool?> exportToExcel(MES.DTO.Library.RFQ.Quote.Quote quote)
        {
            string filePath = string.Empty;
            try
            {
                if (quote.IsExcelTypeExt)
                    filePath = CreateExternalExcel(quote);
                else
                    filePath = CreateInternalExcel(quote);

            }
            catch (Exception ex)//Error
            {
                return FailedBoolResponse(ex.Message);
            }
            return SuccessBoolResponse(filePath);
        }

        private string CreateInternalExcel(DTO.Library.RFQ.Quote.Quote quote)
        {
            string filePath = string.Empty;
            var httpcontext = HttpContext.Current;
            try
            {
                ExcelFile ef = new ExcelFile();
                ExcelFile myExcelFile = new ExcelFile();
                ExcelWorksheet excWsheet = myExcelFile.Worksheets.Add("Internal Quote");

                try
                {
                    CellRange cr = excWsheet.Cells.GetSubrange("A1", "Z100");

                    /* Date : 27-Aug-2013
                     * By : Roma
                     * Purpose : As per client email dt: 17-Jul-2013 Subject: MES RFQ Update -> Could you make excel quote editable instead of just read- only 
                     */
                    //cr.Merged = true;
                    //cr.Style.Locked = true;
                    //cr.Merged = false; 
                    cr = null;

                    cr = excWsheet.Cells.GetSubrange("A6", "F6");
                    cr.Merged = true;
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.ColorTranslator.FromHtml("#EBFFD6"), System.Drawing.ColorTranslator.FromHtml("#EBFFD6"));
                    cr = null;

                    cr = excWsheet.Cells.GetSubrange("A7", "F9");
                    cr.Merged = true;
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.ColorTranslator.FromHtml("#e1dbd1"), System.Drawing.ColorTranslator.FromHtml("#e1dbd1"));
                    cr.Merged = false; cr = null;

                    excWsheet.Cells[5, 0].Value = "Quote Details";

                    excWsheet.Cells[6, 0].Value = "MES Quote #";
                    excWsheet.Cells[6, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[6, 1].Value = quote.QuoteNumber;

                    excWsheet.Cells[7, 0].Value = "Date";
                    excWsheet.Cells[7, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[7, 1].Value = (quote.Date.HasValue) ? Convert.ToDateTime(quote.Date).ToString("dd-MMM-yy") : string.Empty;

                    excWsheet.Cells[8, 0].Value = "Sales Account Manager";
                    excWsheet.Cells[8, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[8, 1].Value = quote.SAMUser;

                    excWsheet.Cells[6, 2].Value = "Ref. RFQ #";
                    excWsheet.Cells[6, 2].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[6, 3].Value = quote.RfqId;

                    excWsheet.Cells[6, 4].Value = "Project Name";
                    excWsheet.Cells[6, 4].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[6, 5].Value = quote.ProjectName;

                    excWsheet.Cells[7, 2].Value = "Customer Name";
                    excWsheet.Cells[7, 2].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[7, 3].Value = quote.CompanyName;

                    excWsheet.Cells[8, 2].Value = "Contact Full Name";
                    excWsheet.Cells[8, 2].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[8, 3].Value = quote.ContactFullName;


                    cr = excWsheet.Cells.GetSubrange("A11", "T11");
                    cr.Merged = true;
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.ColorTranslator.FromHtml("#EBFFD6"), System.Drawing.ColorTranslator.FromHtml("#EBFFD6"));
                    cr.Merged = false;
                    cr = null;

                    excWsheet.Cells[10, 0].Value = "Part No.";
                    excWsheet.Cells[10, 1].Value = "Rev Level";
                    excWsheet.Cells[10, 2].Value = "Description";
                    excWsheet.Cells[10, 3].Value = "Material";
                    excWsheet.Cells[10, 4].Value = "Annual Quantity";
                    excWsheet.Cells[10, 5].Value = "Supplier Quoted Price";
                    excWsheet.Cells[10, 6].Value = "Supplier Price Used";
                    excWsheet.Cells[10, 7].Value = "Supplier Cost/Kg";
                    excWsheet.Cells[10, 8].Value = "Custom Duties";
                    excWsheet.Cells[10, 9].Value = "Weight(KG)";
                    excWsheet.Cells[10, 10].Value = "Shipping Assumption";
                    excWsheet.Cells[10, 11].Value = "Shipping Cost/Pc";
                    excWsheet.Cells[10, 12].Value = "Warehousing";
                    excWsheet.Cells[10, 13].Value = "S, G, A & Profit";
                    excWsheet.Cells[10, 14].Value = "Sales Commission";
                    excWsheet.Cells[10, 15].Value = "Final MES Price";
                    excWsheet.Cells[10, 16].Value = "Final MES Price Per Kg";
                    excWsheet.Cells[10, 17].Value = "Tooling";
                    excWsheet.Cells[10, 18].Value = "Total Annual Cost";
                    excWsheet.Cells[10, 19].Value = "Leadtime / Comments";

                    excWsheet.Columns[0].AutoFit();
                    excWsheet.Columns[1].AutoFit();
                    excWsheet.Columns[2].AutoFit();
                    excWsheet.Columns[3].AutoFit();
                    excWsheet.Columns[4].AutoFit();
                    excWsheet.Columns[5].AutoFit();
                    excWsheet.Columns[6].AutoFit();
                    excWsheet.Columns[7].AutoFit();
                    excWsheet.Columns[8].AutoFit();
                    excWsheet.Columns[9].AutoFit();
                    excWsheet.Columns[10].AutoFit();
                    excWsheet.Columns[11].AutoFit();
                    excWsheet.Columns[12].AutoFit();
                    excWsheet.Columns[13].AutoFit();
                    excWsheet.Columns[14].AutoFit();
                    excWsheet.Columns[15].AutoFit();
                    excWsheet.Columns[16].AutoFit();
                    excWsheet.Columns[17].AutoFit();
                    excWsheet.Columns[18].AutoFit();
                    excWsheet.Columns[19].AutoFit();

                    excWsheet.Columns[8].Width = 1000 * 3;
                    int counter = 11, tempCounter = counter + 1;
                    if (quote.lstQuoteDetails != null && quote.lstQuoteDetails.Count > 0)
                    {
                        List<MES.DTO.Library.RFQ.Quote.QuoteDetails> quotePartsDetails = quote.lstQuoteDetails;

                        if (quotePartsDetails.Count > 0)
                        {
                            foreach (MES.DTO.Library.RFQ.Quote.QuoteDetails item in quotePartsDetails)
                            {

                                if (item.chkSelect)
                                {
                                    excWsheet.Cells[counter, 4].Style.NumberFormat = "0.000";
                                    excWsheet.Cells[counter, 5].Style.NumberFormat = "0.000";
                                    excWsheet.Cells[counter, 6].Style.NumberFormat = "0.000";
                                    excWsheet.Cells[counter, 7].Style.NumberFormat = "0.000";
                                    excWsheet.Cells[counter, 9].Style.NumberFormat = "0.000";
                                    excWsheet.Cells[counter, 10].Style.NumberFormat = "0.000";
                                    excWsheet.Cells[counter, 11].Style.NumberFormat = "0.000";
                                    excWsheet.Cells[counter, 12].Style.NumberFormat = "0.000";
                                    excWsheet.Cells[counter, 13].Style.NumberFormat = "0.000";
                                    excWsheet.Cells[counter, 14].Style.NumberFormat = "0.000";
                                    excWsheet.Cells[counter, 15].Style.NumberFormat = "0.000";
                                    excWsheet.Cells[counter, 16].Style.NumberFormat = "0.000";
                                    excWsheet.Cells[counter, 17].Style.NumberFormat = "0.000";
                                    excWsheet.Cells[counter, 18].Style.NumberFormat = "0.000";

                                    cr = excWsheet.Cells.GetSubrange("A" + tempCounter, "T" + tempCounter);
                                    cr.Merged = true;
                                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                                    cr.Merged = false;
                                    cr = null;

                                    excWsheet.Cells[counter, 0].Value = item.CustomerPartNo;
                                    excWsheet.Cells[counter, 1].Value = item.RevLevel;
                                    excWsheet.Cells[counter, 2].Value = item.PartDescription + "\n" + item.AdditionalPartDesc;
                                    excWsheet.Cells[counter, 3].Value = item.MaterialType;
                                    excWsheet.Cells[counter, 4].Value = item.EstimatedQty;
                                    excWsheet.Cells[counter, 4].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                                    excWsheet.Cells[counter, 5].Value = item.SupplierQuotedPrice.HasValue ? item.SupplierQuotedPrice.Value.ToString("#,##0.000") : item.SupplierQuotedPrice.ToString();
                                    excWsheet.Cells[counter, 5].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                                    excWsheet.Cells[counter, 6].Value = item.SupplierPriceUsed.HasValue ? item.SupplierPriceUsed.Value.ToString("#,##0.000") : item.SupplierPriceUsed.ToString();
                                    excWsheet.Cells[counter, 6].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                                    excWsheet.Cells[counter, 7].Value = item.SupplierCostPerKg.HasValue ? item.SupplierCostPerKg.Value.ToString("#,##0.000") : item.SupplierCostPerKg.ToString();
                                    excWsheet.Cells[counter, 7].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                                    excWsheet.Cells[counter, 8].Value = item.CustomDuties.HasValue ? item.CustomDuties.Value.ToString("#,##0.000") : item.CustomDuties.ToString();
                                    excWsheet.Cells[counter, 8].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                                    excWsheet.Cells[counter, 9].Value = item.PartWeightKG.HasValue ? item.PartWeightKG.Value.ToString("#,##0.000") : item.PartWeightKG.ToString();
                                    excWsheet.Cells[counter, 9].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                                    excWsheet.Cells[counter, 10].Value = item.ShippingAssumption;
                                    excWsheet.Cells[counter, 11].Value = item.ShippingCost.HasValue ? item.ShippingCost.Value.ToString("#,##0.000") : item.ShippingCost.ToString();
                                    excWsheet.Cells[counter, 11].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                                    excWsheet.Cells[counter, 12].Value = item.Warehousing.HasValue ? item.Warehousing.Value.ToString("#,##0.000") : item.Warehousing.ToString();
                                    excWsheet.Cells[counter, 12].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                                    excWsheet.Cells[counter, 13].Value = item.SGAProfit.HasValue ? item.SGAProfit.Value.ToString("#,##0.000") : item.SGAProfit.ToString();
                                    excWsheet.Cells[counter, 13].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                                    excWsheet.Cells[counter, 14].Value = item.SalesCommission.HasValue ? item.SalesCommission.Value.ToString("#,##0.000") : item.SalesCommission.ToString();
                                    excWsheet.Cells[counter, 14].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                                    excWsheet.Cells[counter, 15].Value = item.FinalMESPrice.HasValue && (item.FinalMESPrice.ToString() != "0" || item.FinalMESPrice.ToString() != "0.000") ? "$" + item.FinalMESPrice.Value.ToString("#,##0.000") : item.FinalMESPrice.ToString();
                                    excWsheet.Cells[counter, 15].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

                                    if (item.PartWeightKG > 0)
                                        excWsheet.Cells[counter, 16].Value = Math.Round(Convert.ToDecimal(item.FinalMESPrice / item.PartWeightKG), 3, MidpointRounding.AwayFromZero);

                                    //excWsheet.Cells[counter, 16].Value = item.FinalMESPerKg.HasValue && (item.FinalMESPerKg.ToString() != "0" || item.FinalMESPerKg.ToString() != "0.000") ? "$" + item.FinalMESPerKg.Value.ToString("#,##0.000") : item.FinalMESPerKg.ToString();
                                    excWsheet.Cells[counter, 16].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                                    excWsheet.Cells[counter, 17].Value = item.ToolingCost.HasValue && (item.ToolingCost.ToString() != "0" || item.ToolingCost.ToString() != "0.000") ? "$" + item.ToolingCost.Value.ToString("#,##0.000") : item.ToolingCost.ToString();
                                    excWsheet.Cells[counter, 17].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                                    excWsheet.Cells[counter, 18].Value = item.TotalAnnualCost.HasValue && (item.TotalAnnualCost.ToString() != "0" || item.TotalAnnualCost.ToString() != "0.000") ? "$" + item.TotalAnnualCost.Value.ToString("#,##0.000") : item.TotalAnnualCost.ToString();
                                    excWsheet.Cells[counter, 18].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                                    excWsheet.Cells[counter, 19].Value = item.Leadtime;

                                    counter++;
                                    tempCounter = counter + 1;
                                }
                            }

                            excWsheet.Cells[counter, 0].Value = "Assumptions / Basic Quote Terms";
                            excWsheet.Cells[counter + 1, 0].Value = quote.GeneralAssumption;
                            excWsheet.Cells[counter, 10].Value = "MES Comments";
                            excWsheet.Cells[counter + 1, 10].Value = quote.MESComments;

                            //Assumptions/Basic Quote Terms
                            tempCounter = counter + 1;
                            cr = excWsheet.Cells.GetSubrange("A" + tempCounter, "K" + tempCounter);
                            cr.Merged = true;
                            cr.Style.Font.Weight = ExcelFont.BoldWeight;
                            cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                            cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.ColorTranslator.FromHtml("#EBFFD6"), System.Drawing.ColorTranslator.FromHtml("#EBFFD6"));
                            cr = null;
                            //MES Comments
                            cr = excWsheet.Cells.GetSubrange("L" + tempCounter, "T" + tempCounter);
                            cr.Merged = true;
                            cr.Style.Font.Weight = ExcelFont.BoldWeight;
                            cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                            cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.ColorTranslator.FromHtml("#EBFFD6"), System.Drawing.ColorTranslator.FromHtml("#EBFFD6"));
                            cr = null;

                            string str = quote.GeneralAssumption;
                            int cnt = 0;
                            if (!string.IsNullOrEmpty(str))
                                foreach (var item in str.Split('\r'))
                                {
                                    cnt++;
                                }
                            excWsheet.Rows[counter + 1].Height = cnt * 20 * 20;

                            //Assumptions/Basic Quote Terms
                            tempCounter = tempCounter + 1;
                            cr = excWsheet.Cells.GetSubrange("A" + tempCounter, "K" + tempCounter);
                            cr.Merged = true;
                            cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                            cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                            cr = null;

                            //MES Comments
                            cr = excWsheet.Cells.GetSubrange("L" + tempCounter, "T" + tempCounter);
                            cr.Merged = true;
                            cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                            cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                            cr = null;

                            //MES Logo
                            cr = excWsheet.Cells.GetSubrange("A1", "L4"); cr.Merged = true;
                            cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.White, System.Drawing.Color.White);
                            cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                            cr = null;
                            excWsheet.Pictures.Add(httpcontext.Server.MapPath(Constants.IMAGEFOLDER) + "MESlogoPdf.png", PositioningMode.FreeFloating, new AnchorCell(excWsheet.Columns[0], excWsheet.Rows[0], true), new AnchorCell(excWsheet.Columns[0], excWsheet.Rows[3], false));

                            excWsheet.PrintOptions.FitWorksheetWidthToPages = 1;

                            /* Date : 27-Aug-2013
                             * By : Roma
                             * Purpose : As per client email dt: 17-Jul-2013 Subject: MES RFQ Update -> Could you make excel quote editable instead of just read- only 
                             */
                            //excWsheet.ProtectionSettings.PasswordHash = 1;
                            //excWsheet.Protected = true;
                            string generatedFileName = quote.QuoteNumber + ".xlsx";
                            string tempFilepath = httpcontext.Server.MapPath("~") + Constants.REPORTSTEMPLATEFILEPATH + generatedFileName;
                            if (!System.IO.Directory.Exists(httpcontext.Server.MapPath(@"~") + (Constants.REPORTSTEMPLATEFILEPATH)))
                                System.IO.Directory.CreateDirectory(httpcontext.Server.MapPath(@"~") + (Constants.REPORTSTEMPLATEFILEPATH));
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
                    }
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                finally
                {
                    excWsheet = null;
                    ef.ClosePreservedXlsx();
                }
            }
            catch (Exception ex)//Error
            {
                //"Error - LoadCSV",			
                throw (ex);
            }
            return filePath;
        }

        private string CreateExternalExcel(DTO.Library.RFQ.Quote.Quote quote)
        {
            string filePath = string.Empty;
            var httpcontext = HttpContext.Current;
            try
            {
                ExcelFile ef = new ExcelFile();
                ExcelFile myExcelFile = new ExcelFile();
                ExcelWorksheet excWsheet = myExcelFile.Worksheets.Add("External Quote");

                try
                {
                    CellRange cr = excWsheet.Cells.GetSubrange("A1", "Z100");

                    /* Date : 27-Aug-2013
                     * By : Roma
                     * Purpose : As per client email dt: 17-Jul-2013 Subject: MES RFQ Update -> Could you make excel quote editable instead of just read- only 
                     */
                    //cr.Merged = true;
                    //cr.Style.Locked = true;
                    //cr.Merged = false;
                    cr = null;

                    cr = excWsheet.Cells.GetSubrange("A6", "D6");
                    cr.Merged = true;
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.ColorTranslator.FromHtml("#EBFFD6"), System.Drawing.ColorTranslator.FromHtml("#EBFFD6"));
                    cr = null;

                    cr = excWsheet.Cells.GetSubrange("A7", "D9");
                    cr.Merged = true;
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.ColorTranslator.FromHtml("#e1dbd1"), System.Drawing.ColorTranslator.FromHtml("#e1dbd1"));
                    cr.Merged = false; cr = null;

                    excWsheet.Cells[5, 0].Value = "Quote Details";

                    excWsheet.Cells[6, 0].Value = "MES Quote #";
                    excWsheet.Cells[6, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[6, 1].Value = quote.QuoteNumber;

                    excWsheet.Cells[7, 0].Value = "Date";
                    excWsheet.Cells[7, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[7, 1].Value = (quote.Date.HasValue) ? Convert.ToDateTime(quote.Date).ToString("dd-MMM-yy") : string.Empty;

                    excWsheet.Cells[8, 0].Value = "Sales Account Manager";
                    excWsheet.Cells[8, 0].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[8, 1].Value = quote.SAMUser;

                    excWsheet.Cells[6, 2].Value = "Ref. RFQ #";
                    excWsheet.Cells[6, 2].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[6, 3].Value = quote.RfqId;

                    excWsheet.Cells[6, 4].Value = "Project Name";
                    excWsheet.Cells[6, 4].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[6, 5].Value = quote.ProjectName;

                    excWsheet.Cells[7, 2].Value = "Customer Name";
                    excWsheet.Cells[7, 2].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[7, 3].Value = quote.CompanyName;

                    excWsheet.Cells[8, 2].Value = "Contact Full Name";
                    excWsheet.Cells[8, 2].Style.Font.Weight = ExcelFont.BoldWeight;
                    excWsheet.Cells[8, 3].Value = quote.ContactFullName;

                    cr = excWsheet.Cells.GetSubrange("A11", "K11");
                    cr.Merged = true;
                    cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                    cr.Style.Font.Weight = ExcelFont.BoldWeight;
                    cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                    cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.ColorTranslator.FromHtml("#EBFFD6"), System.Drawing.ColorTranslator.FromHtml("#EBFFD6"));
                    cr.Merged = false;
                    cr = null;

                    excWsheet.Cells[10, 0].Value = "Part No.";
                    excWsheet.Cells[10, 1].Value = "Rev Level";
                    excWsheet.Cells[10, 2].Value = "Description";
                    excWsheet.Cells[10, 3].Value = "Material";
                    excWsheet.Cells[10, 4].Value = "Annual Quantity";
                    excWsheet.Cells[10, 5].Value = "Weight(KG)";
                    excWsheet.Cells[10, 6].Value = "Shipping Assumption";
                    excWsheet.Cells[10, 7].Value = "Final MES Price";
                    excWsheet.Cells[10, 8].Value = "Tooling";
                    excWsheet.Cells[10, 9].Value = "Total Annual Cost";
                    excWsheet.Cells[10, 10].Value = "Leadtime / Comments";

                    excWsheet.Columns[0].AutoFit();
                    excWsheet.Columns[1].AutoFit();
                    excWsheet.Columns[2].AutoFit();
                    excWsheet.Columns[3].AutoFit();
                    excWsheet.Columns[4].AutoFit();
                    excWsheet.Columns[5].AutoFit();
                    excWsheet.Columns[6].AutoFit();
                    excWsheet.Columns[7].AutoFit();
                    excWsheet.Columns[8].AutoFit();
                    excWsheet.Columns[9].AutoFit();
                    excWsheet.Columns[10].AutoFit();

                    excWsheet.Columns[8].Width = 1000 * 3;
                    int counter = 11
                        , tempCounter = counter + 1;
                    List<MES.DTO.Library.RFQ.Quote.QuoteDetails> quotePartsDetails = quote.lstQuoteDetails;

                    if (quotePartsDetails.Count > 0)
                    {
                        foreach (MES.DTO.Library.RFQ.Quote.QuoteDetails item in quotePartsDetails)
                        {
                            if (item.chkSelect)
                            {
                                excWsheet.Cells[counter, 5].Style.NumberFormat = "0.00";
                                excWsheet.Cells[counter, 6].Style.NumberFormat = "0.00";
                                excWsheet.Cells[counter, 7].Style.NumberFormat = "0.00";
                                excWsheet.Cells[counter, 8].Style.NumberFormat = "0.00";
                                excWsheet.Cells[counter, 9].Style.NumberFormat = "0.00";
                                excWsheet.Cells[counter, 10].Style.NumberFormat = "0.00";

                                cr = excWsheet.Cells.GetSubrange("A" + tempCounter, "K" + tempCounter);
                                cr.Merged = true;
                                cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                                cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                                cr.Merged = false;
                                cr = null;

                                excWsheet.Cells[counter, 0].Value = item.CustomerPartNo;
                                excWsheet.Cells[counter, 1].Value = item.RevLevel;
                                excWsheet.Cells[counter, 2].Value = item.PartDescription;
                                excWsheet.Cells[counter, 3].Value = item.MaterialType;
                                excWsheet.Cells[counter, 4].Value = item.EstimatedQty;
                                excWsheet.Cells[counter, 4].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                                excWsheet.Cells[counter, 5].Value = item.PartWeightKG.HasValue ? item.PartWeightKG.Value.ToString("#,##0.000") : item.PartWeightKG.ToString();
                                excWsheet.Cells[counter, 5].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                                excWsheet.Cells[counter, 6].Value = item.ShippingAssumption;
                                excWsheet.Cells[counter, 7].Value = item.FinalMESPrice.HasValue && (item.FinalMESPrice.ToString() != "0" || item.FinalMESPrice.ToString() != "0.000") ? "$" + item.FinalMESPrice.Value.ToString("#,##0.000") : item.FinalMESPrice.ToString();
                                excWsheet.Cells[counter, 7].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                                excWsheet.Cells[counter, 8].Value = item.ToolingCost.HasValue && (item.ToolingCost.ToString() != "0" || item.ToolingCost.ToString() != "0.000") ? "$" + item.ToolingCost.Value.ToString("#,##0.000") : item.ToolingCost.ToString();
                                excWsheet.Cells[counter, 8].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                                excWsheet.Cells[counter, 9].Value = item.TotalAnnualCost.HasValue && (item.TotalAnnualCost.ToString() != "0" || item.TotalAnnualCost.ToString() != "0.000") ? "$" + item.TotalAnnualCost.Value.ToString("#,##0.000") : item.TotalAnnualCost.ToString();
                                excWsheet.Cells[counter, 9].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                                excWsheet.Cells[counter, 10].Value = item.Leadtime;
                                counter++;
                                tempCounter = counter + 1;
                            }
                        }
                        excWsheet.Cells[counter, 0].Value = "Assumptions / Basic Quote Terms";
                        excWsheet.Cells[counter + 1, 0].Value = quote.GeneralAssumption;
                        tempCounter = counter + 1;
                        cr = excWsheet.Cells.GetSubrange("A" + tempCounter, "K" + tempCounter);
                        cr.Merged = true;
                        cr.Style.Font.Weight = ExcelFont.BoldWeight;
                        cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                        cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                        cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.ColorTranslator.FromHtml("#EBFFD6"), System.Drawing.ColorTranslator.FromHtml("#EBFFD6"));
                        cr = null;

                        string str = quote.GeneralAssumption;
                        int cnt = 0;
                        if (!string.IsNullOrEmpty(str))
                            foreach (var item in str.Split('\r'))
                            {
                                cnt++;
                            }
                        excWsheet.Rows[counter + 1].Height = cnt * 20 * 20;

                        tempCounter = tempCounter + 1;
                        cr = excWsheet.Cells.GetSubrange("A" + tempCounter, "K" + tempCounter);
                        cr.Merged = true;
                        cr.Style.VerticalAlignment = VerticalAlignmentStyle.Top;
                        cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                        cr = null;

                        cr = excWsheet.Cells.GetSubrange("A1", "K4"); cr.Merged = true;
                        cr.Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.White, System.Drawing.Color.White);
                        cr.SetBorders(MultipleBorders.Top | MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                        cr = null;

                        excWsheet.Pictures.Add(httpcontext.Server.MapPath(Constants.IMAGEFOLDER) + "MESlogoPdf.png", PositioningMode.FreeFloating, new AnchorCell(excWsheet.Columns[0], excWsheet.Rows[0], true), new AnchorCell(excWsheet.Columns[0], excWsheet.Rows[3], false));

                        excWsheet.PrintOptions.FitWorksheetWidthToPages = 1;

                        /* Date : 27-Aug-2013
                         * By : Roma
                         * Purpose : As per client email dt: 17-Jul-2013 Subject: MES RFQ Update -> Could you make excel quote editable instead of just read- only 
                         */
                        //excWsheet.ProtectionSettings.PasswordHash = 1;
                        //excWsheet.Protected = true;

                        string generatedFileName = "Ext_" + quote.QuoteNumber + ".xlsx";
                        string tempFilepath = httpcontext.Server.MapPath("~") + Constants.REPORTSTEMPLATEFILEPATH + generatedFileName;
                        if (!System.IO.Directory.Exists(httpcontext.Server.MapPath(@"~") + (Constants.REPORTSTEMPLATEFILEPATH)))
                            System.IO.Directory.CreateDirectory(httpcontext.Server.MapPath(@"~") + (Constants.REPORTSTEMPLATEFILEPATH));
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
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                finally
                {
                    excWsheet = null;
                    ef.ClosePreservedXlsx();
                }
            }
            catch (Exception ex)//Error
            {
                //"Error - LoadCSV",
                throw (ex);
            }
            return filePath;
        }
        public ITypedResponse<bool?> Delete(string data)
        {
            throw new NotImplementedException();
        }

        public ITypedResponse<DTO.Library.RFQ.Quote.Quote> FindById(string id)
        {

            string errMSg = string.Empty;
            DTO.Library.RFQ.Quote.Quote quotes = new DTO.Library.RFQ.Quote.Quote();
            this.RunOnDB(context =>
            {
                var quoteItem = context.Quotes.Where(q => q.Id == id).SingleOrDefault();
                if (quoteItem == null)
                    errMSg = Languages.GetResourceText("QuoteNotExists");
                else
                {
                    quotes = new DTO.Library.RFQ.Quote.Quote();
                    quotes = ObjectLibExtensions.AutoConvert<DTO.Library.RFQ.Quote.Quote>(quoteItem);

                    IQuoteDetailsRepository quoteDetailRep = new QuoteDetails();
                    quotes.lstQuoteDetails = quoteDetailRep.FindByQuoteId(quotes.Id).Result;
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.RFQ.Quote.Quote>(errMSg, quotes);
            return response;
        }

        public ITypedResponse<string> Save(DTO.Library.RFQ.Quote.Quote data)
        {
            string errMSg = null, successMsg = null;
            string quoteId = string.Empty;
            if (string.IsNullOrEmpty(data.Id) || data.isRevision)
            {
                data.QuoteFilePath = string.Empty;
                data.ExtQuoteFilePath = string.Empty;
                quoteId = InsertQuote(data).Result;
            }
            else
            {
                quoteId = UpdateQuote(data).Result;
            }
            data.Id = quoteId;

            if (!string.IsNullOrEmpty(quoteId) && data.lstQuoteCalculationHistory != null && data.lstQuoteCalculationHistory.Count > 0)
            {
                IQuoteCalculationHistoryRepository objQuoteCalculationHistoryRepository = null;
                var objDatetime = AuditUtils.GetCurrentDateTime();
                foreach (var item in data.lstQuoteCalculationHistory)
                {
                    objQuoteCalculationHistoryRepository = new QuoteCalculationHistory();
                    item.QuoteId = quoteId;
                    item.CreatedDate = objDatetime;
                    item.UpdatedDate = objDatetime;
                    objQuoteCalculationHistoryRepository.Save(item);
                }
            }

            QuoteCalculationHistory quoteCalculationHistoryobj = new QuoteCalculationHistory();
            data.lstQuoteCalculationHistory = quoteCalculationHistoryobj.GetQuoteCalculationHistory(data.Id);

            IQuoteDetailsRepository quoteRep = new QuoteDetails();
            data.quotesGeneralDetails = new DTO.Library.RFQ.Quote.Quote();
            data.quotesGeneralDetails = quoteRep.GetQuotesGeneralDetails(data.Id, false).Result;

            CreateQuotePdf(data);
            CreateExtQuotePdf(data);

            /*if (IsInsert || data.isRevision)
              {
                  SendEmailWithExtPDF(data);
              }*/
            return SuccessOrFailedResponse<string>(errMSg, quoteId, successMsg);
        }

        public ITypedResponse<List<DTO.Library.RFQ.Quote.Quote>> Search(IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public ITypedResponse<List<DTO.Library.RFQ.Quote.Quote>> GetQuotesToCustomer(string rfqId)
        {
            string errMSg = null;
            List<DTO.Library.RFQ.Quote.Quote> lstQuotes = new List<DTO.Library.RFQ.Quote.Quote>();
            DTO.Library.RFQ.Quote.Quote quotes;
            var httpcontext = HttpContext.Current;

            this.RunOnDB(context =>
            {
                var quoteList = context.GetQuotesToCustomer(rfqId).ToList();
                if (quoteList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    foreach (var item in quoteList)
                    {
                        quotes = new DTO.Library.RFQ.Quote.Quote();
                        quotes.Id = item.Id;
                        quotes.QuoteNumber = item.QuoteNumber;
                        quotes.CreatedDate = item.CreatedDate;

                        string filepath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"])
                                   + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                                   + Constants.QUOTEFILEPATH
                                   + item.QuoteFilePath;

                        if (Helper.BlobHelper.CheckFileExists(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                            , filepath))
                        {
                            quotes.QuoteFilePath = filepath;
                        }
                        else
                            quotes.QuoteFilePath = string.Empty;

                        filepath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["StorageAccountName"])
                                   + Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                                   + Constants.QUOTEFILEPATH
                                   + item.ExtQuoteFilePath;

                        if (Helper.BlobHelper.CheckFileExists(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                          , filepath))
                        {
                            quotes.ExtQuoteFilePath = filepath;
                        }
                        else
                            quotes.ExtQuoteFilePath = string.Empty;

                        lstQuotes.Add(quotes);
                    }
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.Quote.Quote>>(errMSg, lstQuotes);
            return response;
        }

        public ITypedResponse<string> SaveQuoteCalculationHistory(DTO.Library.RFQ.Quote.Quote quoteData)
        {
            string errMSg = null;
            DTO.Library.RFQ.Quote.QuoteCalculationHistory quoteCalculationHistory = null;
            IQuoteCalculationHistoryRepository objQuoteCalculationHistoryRepository = null;
            var objDatetime = AuditUtils.GetCurrentDateTime();
            foreach (var item in quoteData.lstQuoteDetails.Where(a => a.chkSelect == true))
            {
                quoteCalculationHistory = new DTO.Library.RFQ.Quote.QuoteCalculationHistory();
                quoteCalculationHistory.QuoteId = quoteData.Id;
                quoteCalculationHistory.PartId = item.PartId;
                quoteCalculationHistory.CustomDutiesPercent = quoteData.CustomDutiesPercent;
                quoteCalculationHistory.CustomDuties = item.CustomDuties;
                quoteCalculationHistory.ShippingCostPercent = quoteData.ShippingCostPercent;
                quoteCalculationHistory.ShippingCostCalMethod = quoteData.ShippingCostCalMethod;
                quoteCalculationHistory.ShippingCost = item.ShippingCost;
                quoteCalculationHistory.WarehousingPercent = quoteData.WarehousingPercent;
                quoteCalculationHistory.Warehousing = item.Warehousing;
                quoteCalculationHistory.SGAProfitPercent = quoteData.SGAProfitPercent;
                quoteCalculationHistory.SGAProfit = item.SGAProfit;
                quoteCalculationHistory.SalesCommissionPercent = quoteData.SalesCommissionPercent;
                quoteCalculationHistory.SalesCommission = item.SalesCommission;
                quoteCalculationHistory.ToolingCostPercent = quoteData.ToolingCostPercent;
                quoteCalculationHistory.ToolingCost = item.ToolingCost;
                quoteCalculationHistory.CreatedDate = objDatetime;
                quoteCalculationHistory.UpdatedDate = objDatetime;
                objQuoteCalculationHistoryRepository = new QuoteCalculationHistory();
                objQuoteCalculationHistoryRepository.Save(quoteCalculationHistory);
            }

            return SuccessOrFailedResponse<string>(errMSg, "");
        }

    }
}