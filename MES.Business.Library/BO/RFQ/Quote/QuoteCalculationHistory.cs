using Account.DTO.Library;
using MES.Business.Repositories.RFQ.Quote;
using NPE.Core;
using NPE.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using NPE.Core.Extensions;
using System.Data.Entity.Core.Objects;
using MES.Business.Library.Extensions;

namespace MES.Business.Library.BO.RFQ.Quote
{
    class QuoteCalculationHistory : ContextBusinessBase, IQuoteCalculationHistoryRepository
    {
        public QuoteCalculationHistory()
            : base("QuoteCalculationHistory")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.RFQ.Quote.QuoteCalculationHistory quoteCalculationHistory)
        {
            string errMSg = null;
            string successMsg = null;
            var recordToBeUpdated = new MES.Data.Library.QuoteCalculationHistory();

            if (quoteCalculationHistory.Id > 0)
            {
                recordToBeUpdated = this.DataContext.QuoteCalculationHistories.Where(a => a.Id == quoteCalculationHistory.Id).SingleOrDefault();

                if (recordToBeUpdated == null)
                    errMSg = Languages.GetResourceText("quoteCalculationHistoryNotExists");
                else
                {
                    recordToBeUpdated.UpdatedBy = CurrentUser;
                    this.DataContext.Entry(recordToBeUpdated).State = EntityState.Modified;
                }
            }
            else
            {
                recordToBeUpdated.CreatedBy = recordToBeUpdated.UpdatedBy = CurrentUser;
                this.DataContext.QuoteCalculationHistories.Add(recordToBeUpdated);
            }
            if (string.IsNullOrEmpty(errMSg))
            {
                recordToBeUpdated.QuoteId = quoteCalculationHistory.QuoteId;
                recordToBeUpdated.PartId = quoteCalculationHistory.PartId;
                recordToBeUpdated.CustomDutiesPercent = quoteCalculationHistory.CustomDutiesPercent;
                recordToBeUpdated.CustomDuties = quoteCalculationHistory.CustomDuties;
                recordToBeUpdated.ShippingCostPercent = quoteCalculationHistory.ShippingCostPercent;
                recordToBeUpdated.ShippingCostCalMethod = quoteCalculationHistory.ShippingCostCalMethod;
                recordToBeUpdated.ShippingCost = quoteCalculationHistory.ShippingCost;
                recordToBeUpdated.WarehousingPercent = quoteCalculationHistory.WarehousingPercent;
                recordToBeUpdated.Warehousing = quoteCalculationHistory.Warehousing;
                recordToBeUpdated.SGAProfitPercent = quoteCalculationHistory.SGAProfitPercent;
                recordToBeUpdated.SGAProfit = quoteCalculationHistory.SGAProfit;
                recordToBeUpdated.SalesCommissionPercent = quoteCalculationHistory.SalesCommissionPercent;
                recordToBeUpdated.SalesCommission = quoteCalculationHistory.SalesCommission;
                recordToBeUpdated.ToolingCostPercent = quoteCalculationHistory.ToolingCostPercent;
                recordToBeUpdated.ToolingCost = quoteCalculationHistory.ToolingCost;
                recordToBeUpdated.CreatedDate = Convert.ToDateTime(quoteCalculationHistory.CreatedDate);
                recordToBeUpdated.UpdatedDate = quoteCalculationHistory.UpdatedDate;
                this.DataContext.SaveChanges();
                quoteCalculationHistory.Id = Convert.ToInt32(recordToBeUpdated.Id);
                successMsg = Languages.GetResourceText("quoteCalculationHistorySavedSuccess");
            }
            return SuccessOrFailedResponse<int?>(errMSg, quoteCalculationHistory.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.Quote.QuoteCalculationHistory> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int QuoteCalculationHistoryId)
        {
            var RFQdqOverheadToBeDeleted = this.DataContext.QuoteCalculationHistories.Where(a => a.Id == QuoteCalculationHistoryId).SingleOrDefault();
            if (RFQdqOverheadToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("QuoteCalculationHistoryNotExists"));
            }
            else
            {
                RFQdqOverheadToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                RFQdqOverheadToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(RFQdqOverheadToBeDeleted).State = EntityState.Modified;
                //RFQdqOverheadToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("QuoteCalculationHistoryDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.Quote.QuoteCalculationHistory>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public List<DTO.Library.RFQ.Quote.QuoteCalculationHistory> GetQuoteCalculationHistory(string QuoteId)
        {
            List<DTO.Library.RFQ.Quote.QuoteCalculationHistory> lstQuoteCalculationHistory = new List<DTO.Library.RFQ.Quote.QuoteCalculationHistory>();
            DTO.Library.RFQ.Quote.QuoteCalculationHistory quoteCalculationHistory;
            this.RunOnDB(context =>
             {
                 var objQuoteCalculationHistory = context.GetQuoteCalculationHistory(QuoteId).ToList();
                 if (objQuoteCalculationHistory != null)
                 {
                     foreach (var item in objQuoteCalculationHistory)
                     {
                         quoteCalculationHistory = new DTO.Library.RFQ.Quote.QuoteCalculationHistory();
                         quoteCalculationHistory.Id = Convert.ToInt32(item.Id);
                         quoteCalculationHistory.QuoteId = item.QuoteId;
                         quoteCalculationHistory.PartId = item.PartId;
                         quoteCalculationHistory.CustomerPartNo = item.CustomerPartNo;
                         quoteCalculationHistory.PartDescription = item.PartDescription;
                         quoteCalculationHistory.AdditionalPartDesc = item.AdditionalPartDescription;
                         quoteCalculationHistory.UpdatedOn = item.UpdatedOn;
                         quoteCalculationHistory.UpdatedDateString = item.UpdatedDate.Value.FormatDateInMediumDateWithTime();
                         quoteCalculationHistory.CustomDutiesPercent = item.CustomDutiesPercent;
                         quoteCalculationHistory.CustomDuties = item.CustomDuties;
                         quoteCalculationHistory.ShippingCostPercent = item.ShippingCostPercent;
                         quoteCalculationHistory.ShippingCostCalMethod = item.ShippingCostCalMethod;
                         quoteCalculationHistory.ShippingCost = item.ShippingCost;
                         quoteCalculationHistory.WarehousingPercent = item.WarehousingPercent;
                         quoteCalculationHistory.Warehousing = item.Warehousing;
                         quoteCalculationHistory.SGAProfitPercent = item.SGAProfitPercent;
                         quoteCalculationHistory.SGAProfit = item.SGAProfit;
                         quoteCalculationHistory.SalesCommissionPercent = item.SalesCommissionPercent;
                         quoteCalculationHistory.SalesCommission = item.SalesCommission;
                         quoteCalculationHistory.ToolingCostPercent = item.ToolingCostPercent;
                         quoteCalculationHistory.ToolingCost = item.ToolingCost;
                         quoteCalculationHistory.UpdatedDate = item.UpdatedDate;
                         lstQuoteCalculationHistory.Add(quoteCalculationHistory);
                     }
                 }
             });
            //get hold of response
            return lstQuoteCalculationHistory;
        }
    }
}
