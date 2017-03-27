using Account.DTO.Library;
using MES.Business.Mapping.Extensions;
using MES.Business.Repositories.RFQ.Quote;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Library.BO.RFQ.Quote
{
    class QuoteDetails : ContextBusinessBase, IQuoteDetailsRepository
    {
        public QuoteDetails()
            : base("QuoteDetails")
        { }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.Quote.Quote> GetPartsToQuote(string rfqId)
        {
            string errMSg = string.Empty;

            DTO.Library.RFQ.Quote.Quote quoteItem = new DTO.Library.RFQ.Quote.Quote();
            List<DTO.Library.RFQ.Quote.QuoteDetails> lstPartsToQuote = new List<DTO.Library.RFQ.Quote.QuoteDetails>();
            DTO.Library.RFQ.Quote.QuoteDetails partItem = null;
            this.RunOnDB(context =>
            {
                RFQ.RFQ rfqObj = new RFQ.RFQ();
                DTO.Library.RFQ.RFQ.RFQ rfqInfo = rfqObj.FindById(rfqId).Result;
                if (rfqInfo != null)
                {
                    quoteItem.RfqId = rfqId;
                    quoteItem.CustomerId = rfqInfo.CustomerId;
                    quoteItem.SAMId = rfqInfo.SAMId;
                    quoteItem.ProjectName = rfqInfo.ProjectName;

                    var partList = context.GetPartsToQuote(rfqId).ToList();
                    if (partList == null)
                        errMSg = Languages.GetResourceText("RecordNotExist");
                    else
                    {
                        if (partList.Count > 0)
                        {
                            foreach (var item in partList)
                            {
                                partItem = new DTO.Library.RFQ.Quote.QuoteDetails();
                                partItem.PartId = item.RFQPartId;
                                partItem.CustomerPartNo = item.PartNo;
                                partItem.RevLevel = item.RevLevel;
                                partItem.PartDescription = item.PartDescription;
                                partItem.AdditionalPartDesc = item.AdditionalPartDescription;
                                partItem.EstimatedQty = item.EstimatedQty;
                                partItem.MaterialType = item.MaterialType;
                                partItem.PartWeightKG = item.PartWeightKG;

                                lstPartsToQuote.Add(partItem);
                            }
                            string revQuoteNumber = GenerateQuoteNumber(rfqId);
                            quoteItem.QuoteNumber = string.IsNullOrEmpty(revQuoteNumber) ? rfqId : revQuoteNumber;
                        }

                        quoteItem.lstQuoteDetails = lstPartsToQuote;
                    }
                }
            });


            //get hold of response
            var response = SuccessOrFailedResponse<DTO.Library.RFQ.Quote.Quote>(errMSg, quoteItem);
            return response;
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.Quote.Quote> GetQuoteDetails(string quoteId, bool isR)
        {
            string errMSg = string.Empty;

            DTO.Library.RFQ.Quote.Quote quoteItem = new DTO.Library.RFQ.Quote.Quote();

            List<DTO.Library.RFQ.Quote.QuoteDetails> lstQuoteDetails = new List<DTO.Library.RFQ.Quote.QuoteDetails>();
            DTO.Library.RFQ.Quote.QuoteDetails qdItem = null;
            this.RunOnDB(context =>
            {
                var quoteInfo = context.GetQuoteInfo(quoteId).First();
                if (quoteInfo != null)
                {
                    quoteItem.Id = quoteInfo.Id;
                    quoteItem.QuoteNumber = quoteInfo.QuoteNumber;
                    quoteItem.RfqId = quoteInfo.RFQId;
                    quoteItem.ProjectName = quoteInfo.ProjectName;
                    quoteItem.CustomerId = quoteInfo.CustomerId;
                    quoteItem.SAMId = quoteInfo.SAMId;
                    quoteItem.CompanyName = quoteInfo.CompanyName;
                    quoteItem.ContactFullName = quoteInfo.ContactFullName;
                    quoteItem.Date = quoteInfo.Date;
                    quoteItem.GeneralAssumption = quoteInfo.GeneralAssumption;
                    quoteItem.SAMUser = quoteInfo.SAMUser;
                    quoteItem.CustomDutiesPercent = quoteInfo.CustomDutiesPercent;
                    quoteItem.SalesCommissionPercent = quoteInfo.SalesCommissionPercent;
                    quoteItem.SGAProfitPercent = quoteInfo.SGAProfitPercent;
                    quoteItem.ShippingCostPercent = quoteInfo.ShippingCostPercent;
                    quoteItem.WarehousingPercent = quoteInfo.WarehousingPercent;
                    quoteItem.ToolingCostPercent = quoteInfo.ToolingCostPercent;
                    quoteItem.ShippingCostCalMethod = quoteInfo.ShippingCostCalMethod;
                    quoteItem.SupplierCost = quoteInfo.SupplierCost;
                    quoteItem.AmountWon = quoteInfo.AmountWon;
                    quoteItem.StatusId = quoteInfo.Status;
                    quoteItem.QuoteFilePath = quoteInfo.QuoteFilePath;
                    quoteItem.ExtQuoteFilePath = quoteInfo.ExtQuoteFilePath;
                    quoteItem.MESComments = quoteInfo.MESComments;


                    var qdList = context.GetQuoteDetails(quoteId).ToList();
                    if (qdList == null)
                        errMSg = Languages.GetResourceText("RecordNotExist");
                    else
                    {
                        //set details for the list of quote 
                        foreach (var item in qdList)
                        {
                            qdItem = new DTO.Library.RFQ.Quote.QuoteDetails();
                            qdItem.PartId = item.Id;
                            qdItem.CustomerPartNo = item.PartNo;
                            qdItem.RevLevel = item.RevLevel;
                            qdItem.PartDescription = item.PartDescription;
                            qdItem.AdditionalPartDesc = item.AdditionalPartDescription;
                            qdItem.EstimatedQty = item.EstimatedQty;
                            qdItem.MaterialType = item.MaterialType;
                            qdItem.PartWeightKG = item.PartWeightKG;

                            qdItem.QuoteRevision = item.QuoteRevision;
                            qdItem.Id = item.QuoteDetailsId.HasValue ? item.QuoteDetailsId.Value : 0;
                            qdItem.SupplierId = item.SupplierId;
                            qdItem.SupplierToolingCost = item.SupplierToolingCost;

                            var sqpItem = context.GetSubmitedQuotePartsDetails(quoteItem.RfqId, qdItem.SupplierId, qdItem.PartId).SingleOrDefault();
                            if (sqpItem != null)
                            {
                                qdItem.SupplierToolingCost = sqpItem.ToolingCost;
                            }
                            else
                                qdItem.SupplierToolingCost = 0;
                            qdItem.ToolingCost = item.ToolingCost;
                            qdItem.ShippingCost = item.ShippingCost;
                            qdItem.SupplierCostPerKg = item.SupplierCostPerKg;
                            qdItem.Warehousing = item.Warehousing;

                            qdItem.SGAProfit = item.SGAProfit;
                            qdItem.SalesCommission = item.SalesCommission;
                            qdItem.CustomDuties = item.CustomDuties;
                            qdItem.SupplierQuotedPrice = item.SupplierQuotedPrice;
                            qdItem.ToolingWarranty = item.ToolingWarranty;
                            qdItem.SupplierToolingLeadtime = item.SupplierToolingLeadtime;
                            qdItem.Leadtime = item.Leadtime;
                            qdItem.ShippingAssumption = item.ShippingAssumption;

                            qdItem.SupplierPriceUsed = item.SupplierPriceUsed;
                            qdItem.FinalMESPrice = item.FinalMESPrice;
                            qdItem.FinalMESPerKg = item.FinalMESCostPerKg;
                            qdItem.QuoteDate = item.QuoteDate;
                            qdItem.TotalAnnualCost = item.TotalAnnualCost;
                            qdItem.MinOrderQty = item.MinOrderQty;
                            //set last percent on which calculation was done
                            if (!isR)
                            {
                                //fetch details for the list of quote calculated history
                                IQuoteCalculationHistoryRepository objIQuoteCalculationHistoryRepository = new QuoteCalculationHistory();
                                quoteItem.lstQuoteCalculationHistory = objIQuoteCalculationHistoryRepository.GetQuoteCalculationHistory(quoteId);

                                var tempObj = quoteItem.lstQuoteCalculationHistory.Where(a => a.PartId == qdItem.PartId).OrderByDescending(b => b.UpdatedDate).FirstOrDefault();
                                if (tempObj != null)
                                {
                                    qdItem.CustomDutiesPercent = "(" + Convert.ToString(tempObj.CustomDutiesPercent) + "%)";
                                    qdItem.SalesCommissionPercent = "(" + Convert.ToString(tempObj.SalesCommissionPercent) + "%)";
                                    qdItem.SGAProfitPercent = "(" + Convert.ToString(tempObj.SGAProfitPercent) + "%)";
                                    qdItem.ShippingCostPercent = "(" + Convert.ToString(tempObj.ShippingCostPercent) + Convert.ToString(tempObj.ShippingCostCalMethod) + ")";
                                    qdItem.ShippingCostCalMethod = Convert.ToString(tempObj.ShippingCostCalMethod);
                                    qdItem.WarehousingPercent = "(" + Convert.ToString(tempObj.WarehousingPercent) + "%)";
                                    qdItem.ToolingCostPercent = "(" + Convert.ToString(tempObj.ToolingCostPercent) + "%)";
                                }
                            }
                            /* else
                             {
                                 //fetch details for the list of quote calculated history
                                 IQuoteCalculationHistoryRepository objIQuoteCalculationHistoryRepository = new QuoteCalculationHistory();
                                 quoteItem.lstQuoteCalculationHistory = new List<DTO.Library.RFQ.Quote.QuoteCalculationHistory>();
                                 //quoteItem.lstQuoteCalculationHistory = objIQuoteCalculationHistoryRepository.GetQuoteCalculationHistory(quoteId);

                                 var tempObj = objIQuoteCalculationHistoryRepository.GetQuoteCalculationHistory(quoteId).Where(a => a.PartId == qdItem.PartId).OrderByDescending(b => b.UpdatedDate).FirstOrDefault();
                                 if (tempObj != null)
                                 {
                                     qdItem.CustomDutiesPercent = "(" + Convert.ToString(tempObj.CustomDutiesPercent) + "%)";
                                     qdItem.SalesCommissionPercent = "(" + Convert.ToString(tempObj.SalesCommissionPercent) + "%)";
                                     qdItem.SGAProfitPercent = "(" + Convert.ToString(tempObj.SGAProfitPercent) + "%)";
                                     qdItem.ShippingCostPercent = "(" + Convert.ToString(tempObj.ShippingCostPercent) + Convert.ToString(tempObj.ShippingCostCalMethod) + ")";
                                     qdItem.ShippingCostCalMethod = Convert.ToString(tempObj.ShippingCostCalMethod);
                                     qdItem.WarehousingPercent = "(" + Convert.ToString(tempObj.WarehousingPercent) + "%)";
                                     qdItem.ToolingCostPercent = "(" + Convert.ToString(tempObj.ToolingCostPercent) + "%)";
                                     quoteItem.lstQuoteCalculationHistory.Add(tempObj);
                                 }                                
                             }
                             */

                            lstQuoteDetails.Add(qdItem);
                        }
                        quoteItem.lstQuoteDetails = lstQuoteDetails;
                        string revQuoteNumber = string.Empty;
                        if (isR)
                        {
                            revQuoteNumber = GenerateQuoteNumber(quoteItem.RfqId);
                            quoteItem.QuoteNumber = string.IsNullOrEmpty(revQuoteNumber) ? quoteItem.RfqId : revQuoteNumber;
                        }
                    }
                }
            });

            //get hold of response
            var response = SuccessOrFailedResponse<DTO.Library.RFQ.Quote.Quote>(errMSg, quoteItem);
            return response;
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.Quote.Quote> GetQuotesGeneralDetails(string quoteId, bool isR)
        {
            string errMSg = string.Empty;
            DTO.Library.RFQ.Quote.Quote quoteItem = new DTO.Library.RFQ.Quote.Quote();
            this.RunOnDB(context =>
            {
                var quoteInfo = context.GetQuoteInfo(quoteId).First();
                if (quoteInfo != null)
                {
                    quoteItem.Id = quoteInfo.Id;
                    quoteItem.QuoteNumber = quoteInfo.QuoteNumber;
                    quoteItem.RfqId = quoteInfo.RFQId;
                    quoteItem.ProjectName = quoteInfo.ProjectName;
                    quoteItem.CustomerId = quoteInfo.CustomerId;
                    quoteItem.SAMId = quoteInfo.SAMId;
                    quoteItem.CompanyName = quoteInfo.CompanyName;
                    quoteItem.ContactFullName = quoteInfo.ContactFullName;
                    quoteItem.Date = quoteInfo.Date;
                    quoteItem.GeneralAssumption = quoteInfo.GeneralAssumption;
                    quoteItem.SAMUser = quoteInfo.SAMUser;
                    quoteItem.CustomDutiesPercent = quoteInfo.CustomDutiesPercent;
                    quoteItem.SalesCommissionPercent = quoteInfo.SalesCommissionPercent;
                    quoteItem.SGAProfitPercent = quoteInfo.SGAProfitPercent;
                    quoteItem.ShippingCostPercent = quoteInfo.ShippingCostPercent;
                    quoteItem.WarehousingPercent = quoteInfo.WarehousingPercent;
                    quoteItem.ToolingCostPercent = quoteInfo.ToolingCostPercent;
                    quoteItem.ShippingCostCalMethod = quoteInfo.ShippingCostCalMethod;
                    quoteItem.SupplierCost = quoteInfo.SupplierCost;
                    quoteItem.AmountWon = quoteInfo.AmountWon;
                    quoteItem.StatusId = quoteInfo.Status;
                    quoteItem.QuoteFilePath = quoteInfo.QuoteFilePath;
                    quoteItem.ExtQuoteFilePath = quoteInfo.ExtQuoteFilePath;
                    quoteItem.MESComments = quoteInfo.MESComments;
                }
            });

            //get hold of response
            var response = SuccessOrFailedResponse<DTO.Library.RFQ.Quote.Quote>(errMSg, quoteItem);
            return response;
        }

        public NPE.Core.ITypedResponse<bool?> InsertQuoteDetails(DTO.Library.RFQ.Quote.QuoteDetails quoteDetails)
        {
            string successMsg = null;
            //set the out put param
            ObjectParameter ErrorNumber = new ObjectParameter("ErrorNumber", 0);
            this.RunOnDB(context =>
            {
                int result = context.InsertQuoteDetails(quoteDetails.QuoteId
                    , quoteDetails.QuoteRevision
                    , quoteDetails.SupplierId
                    , quoteDetails.PartId
                    , quoteDetails.SupplierCostPerKg
                    , quoteDetails.ShippingCost
                    , quoteDetails.Warehousing
                    , quoteDetails.SGAProfit
                    , quoteDetails.ToolingCost
                    , quoteDetails.SalesCommission
                    , quoteDetails.CustomDuties
                    , quoteDetails.SupplierQuotedPrice
                    , quoteDetails.SupplierPriceUsed
                    , quoteDetails.FinalMESPrice
                    , quoteDetails.FinalMESPerKg
                    , quoteDetails.ToolingWarranty
                    , quoteDetails.SupplierToolingLeadtime == null ? string.Empty : quoteDetails.SupplierToolingLeadtime
                    , quoteDetails.Leadtime == null ? string.Empty : quoteDetails.Leadtime
                    , quoteDetails.TotalAnnualCost
                    , quoteDetails.ShippingAssumption == null ? string.Empty : quoteDetails.ShippingAssumption
                    , quoteDetails.MinOrderQty.HasValue ? quoteDetails.MinOrderQty.Value : 0
                    , CurrentUser
                    , ErrorNumber
                    );

                if (result > 0)
                {
                    successMsg = Languages.GetResourceText("QuoteDetailsSavedSuccess");
                }
            });

            return SuccessBoolResponse(Languages.GetResourceText("QuoteDetailsSavedSuccess"));
        }

        public NPE.Core.ITypedResponse<bool?> UpdateQuoteDetails(DTO.Library.RFQ.Quote.QuoteDetails quoteDetails)
        {
            string successMsg = null;
            //set the out put param
            ObjectParameter ErrorNumber = new ObjectParameter("ErrorNumber", 0);
            this.RunOnDB(context =>
            {
                int result = context.UpdateQuoteDetails(quoteDetails.Id
                    , quoteDetails.QuoteRevision
                    , quoteDetails.SupplierId
                    , quoteDetails.SupplierCostPerKg
                    , quoteDetails.ShippingCost
                    , quoteDetails.Warehousing
                    , quoteDetails.SGAProfit
                    , quoteDetails.ToolingCost
                    , quoteDetails.SalesCommission
                    , quoteDetails.CustomDuties
                    , quoteDetails.SupplierQuotedPrice
                    , quoteDetails.SupplierPriceUsed
                    , quoteDetails.FinalMESPrice
                    , quoteDetails.FinalMESPerKg
                    , quoteDetails.ToolingWarranty
                    , quoteDetails.SupplierToolingLeadtime == null ? string.Empty : quoteDetails.SupplierToolingLeadtime
                    , quoteDetails.Leadtime == null ? string.Empty : quoteDetails.Leadtime
                    , quoteDetails.TotalAnnualCost
                    , quoteDetails.ShippingAssumption == null ? string.Empty : quoteDetails.ShippingAssumption
                    , quoteDetails.MinOrderQty.HasValue ? quoteDetails.MinOrderQty.Value : 0
                    , CurrentUser
                    , ErrorNumber
                    );

                if (result > 0)
                {
                    successMsg = Languages.GetResourceText("QuoteDetailsUpdatedSuccess");
                }
            });

            return SuccessBoolResponse(Languages.GetResourceText("QuoteDetailsUpdatedSuccess"));
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.Quote.QuoteDetails> GetSupplierQuotedDetails(DTO.Library.RFQ.Quote.QuoteDetails qdItem)
        {
            string errMSg = string.Empty;

            this.RunOnDB(context =>
            {
                var item = context.GetSubmitedQuotePartsDetails(qdItem.RfqId, qdItem.SupplierId, qdItem.PartId).SingleOrDefault();
                if (item == null)
                {
                    qdItem.SupplierQuotedPrice = 0;
                    qdItem.ToolingWarranty = string.Empty;
                    qdItem.SupplierToolingLeadtime = string.Empty;
                    qdItem.ToolingCost = 0;
                    qdItem.SupplierToolingCost = 0;
                    qdItem.SupplierPriceUsed = 0;
                    qdItem.MinOrderQty = 0;
                }
                else
                {
                    qdItem.SupplierQuotedPrice = item.SupplierQuotedPrice;
                    qdItem.ToolingWarranty = item.ToolingWarranty;
                    qdItem.SupplierToolingLeadtime = item.SupplierToolingLeadtime;
                    qdItem.ToolingCost = item.ToolingCost;
                    qdItem.SupplierToolingCost = item.ToolingCost;
                    qdItem.SupplierPriceUsed = item.SupplierQuotedPrice;
                    qdItem.MinOrderQty = item.MinOrderQty;
                }
            });

            //get hold of response
            var response = SuccessOrFailedResponse<DTO.Library.RFQ.Quote.QuoteDetails>(errMSg, qdItem);
            return response;
        }

        private string GenerateQuoteNumber(string rfqNo)
        {
            string newQuoteNumber = string.Empty;
            Quote quoteObj = new Quote();
            string errMSg = string.Empty;
            DTO.Library.RFQ.Quote.Quote quotes = new DTO.Library.RFQ.Quote.Quote();
            string newRevQuoteNumber = string.Empty, parentQuoteId = string.Empty;
            this.RunOnDB(context =>
            {
                var lstlatestQuote = context.GenerateRevisionQuoteNumber(rfqNo).ToList();

                if (lstlatestQuote == null || lstlatestQuote.Count == 0)
                    errMSg = Languages.GetResourceText("QuoteNotExists");
                else
                {
                    foreach (var item in lstlatestQuote)
                    {
                        newQuoteNumber = item.QuoteNumber;
                    }
                    /* var latestQuote = lstlatestQuote.First();
                     string varQuoteNumber = latestQuote.QuoteNumber.TrimEnd(new char[] { ' ' });
                     if (!varQuoteNumber.Contains("R"))
                         varQuoteNumber += "-R";
                     string tempvarQuoteNumber = varQuoteNumber.Substring(varQuoteNumber.IndexOf("R") + 1, varQuoteNumber.Length - varQuoteNumber.IndexOf("R") - 1);

                     int qn = !string.IsNullOrEmpty(tempvarQuoteNumber) ? Convert.ToInt16(tempvarQuoteNumber) + 1 : 1;*/
                    //newQuoteNumber = varQuoteNumber.Substring(0, varQuoteNumber.IndexOf("R") + 1) + qn.ToString().PadLeft(2, '0');

                }
            });
            return newQuoteNumber;
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.Quote.Quote> GetQuotePartsDetail(DTO.Library.RFQ.Quote.Quote qItem)
        {
            string errMSg = string.Empty;

            DTO.Library.RFQ.Quote.Quote quoteItem = new DTO.Library.RFQ.Quote.Quote();
            List<DTO.Library.RFQ.Quote.QuoteDetails> lstPartsToQuote = new List<DTO.Library.RFQ.Quote.QuoteDetails>();
            DTO.Library.RFQ.Quote.QuoteDetails partItem = null;
            this.RunOnDB(context =>
            {
                RFQ.RFQ rfqObj = new RFQ.RFQ();
                DTO.Library.RFQ.RFQ.RFQ rfqInfo = rfqObj.FindById(qItem.RfqId).Result;
                if (rfqInfo != null)
                {
                    quoteItem.RfqId = qItem.RfqId;
                    quoteItem.CustomerId = rfqInfo.CustomerId;
                    quoteItem.SAMId = rfqInfo.SAMId;
                    quoteItem.ProjectName = rfqInfo.ProjectName;

                    var partList = context.GetPartsToQuote(qItem.RfqId).ToList();
                    if (partList == null)
                        errMSg = Languages.GetResourceText("RecordNotExist");
                    else
                    {
                        if (partList.Count > 0)
                        {
                            foreach (var item in partList)
                            {
                                partItem = new DTO.Library.RFQ.Quote.QuoteDetails();
                                partItem.PartId = item.RFQPartId;
                                partItem.CustomerPartNo = item.PartNo;
                                partItem.RevLevel = item.RevLevel;
                                partItem.PartDescription = item.PartDescription;
                                partItem.AdditionalPartDesc = item.AdditionalPartDescription;
                                partItem.EstimatedQty = item.EstimatedQty;
                                partItem.MaterialType = item.MaterialType;
                                partItem.PartWeightKG = item.PartWeightKG;

                                lstPartsToQuote.Add(partItem);
                            }
                            string revQuoteNumber = GenerateQuoteNumber(qItem.RfqId);
                            quoteItem.QuoteNumber = string.IsNullOrEmpty(revQuoteNumber) ? qItem.RfqId : revQuoteNumber;
                        }
                        quoteItem.lstQuoteDetails = lstPartsToQuote;
                    }
                }
            });

            foreach (var item in quoteItem.lstQuoteDetails)
            {
                foreach (var jtem in qItem.lstQuoteDetails)
                {
                    if (jtem.PartId == item.PartId)
                    {
                        item.SupplierId = jtem.SupplierId;
                        item.RfqId = qItem.RfqId;
                        DTO.Library.RFQ.Quote.QuoteDetails qdItem = GetSupplierQuotedDetails(item).Result;
                        item.SupplierQuotedPrice = qdItem.SupplierQuotedPrice;
                        item.ToolingWarranty = qdItem.ToolingWarranty;
                        item.SupplierToolingLeadtime = qdItem.SupplierToolingLeadtime;
                        item.ToolingCost = qdItem.ToolingCost;
                        item.SupplierToolingCost = qdItem.ToolingCost;
                        item.SupplierPriceUsed = qdItem.SupplierQuotedPrice;
                        break;
                    }
                }
            }

            //get hold of response
            var response = SuccessOrFailedResponse<DTO.Library.RFQ.Quote.Quote>(errMSg, quoteItem);
            return response;
        }
        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.Quote.QuoteDetails>> FindByQuoteId(string quoteId)
        {
            string errMSg = string.Empty;
            List<DTO.Library.RFQ.Quote.QuoteDetails> lstQuotes = new List<DTO.Library.RFQ.Quote.QuoteDetails>();

            DTO.Library.RFQ.Quote.QuoteDetails quotes = new DTO.Library.RFQ.Quote.QuoteDetails();
            this.RunOnDB(context =>
            {
                var quoteItem = context.Details.Where(q => q.QuoteId == quoteId).ToList();
                if (quoteItem == null)
                    errMSg = Languages.GetResourceText("QuoteDetailNotExists");
                else
                {
                    foreach (var item in quoteItem)
                    {
                        quotes = new DTO.Library.RFQ.Quote.QuoteDetails();
                        quotes = ObjectLibExtensions.AutoConvert<DTO.Library.RFQ.Quote.QuoteDetails>(quoteItem);
                        lstQuotes.Add(quotes);
                    }
                }
            });

            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.Quote.QuoteDetails>>(errMSg, lstQuotes);
            return response;
        }

        public NPE.Core.ITypedResponse<bool?> Delete(string data)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.Quote.QuoteDetails> FindById(string id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<string> Save(DTO.Library.RFQ.Quote.QuoteDetails data)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.Quote.QuoteDetails>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }
    }
}
