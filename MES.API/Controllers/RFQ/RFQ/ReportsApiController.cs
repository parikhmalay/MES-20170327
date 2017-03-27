using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.RFQ.RFQReports;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.RFQ.RFQReports
{
    [AdminPrefix("ReportsApi")]
    public class ReportsApiController : SecuredApiControllerBase
    {
        [Inject]
        public IRFQReportsRepository RFQReportsRepository { get; set; }

        #region RFQ Analysis and Business(Alina) report with chart
        [HttpPostRoute("GetRFQAnalysisReport")]
        public ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQAnalysisReport>> GetRFQAnalysisReport(GenericPage<MES.DTO.Library.RFQ.RFQReports.RFQAnalysisReportSearch> paging)
        {
            var type = this.Resolve<IRFQReportsRepository>(RFQReportsRepository).GetRFQAnalysisReport(paging);
            return type;
        }
        [HttpPostRoute("exportRFQAnalysisReport")]
        public ITypedResponse<bool?> exportRFQAnalysisReport(GenericPage<MES.DTO.Library.RFQ.RFQReports.RFQAnalysisReportSearch> paging)
        {

            if (paging.Criteria.ReportId == 16)
                return this.Resolve<IRFQReportsRepository>(RFQReportsRepository).exportBusinessAnalysisQTCReport(paging);

            else
                return this.Resolve<IRFQReportsRepository>(RFQReportsRepository).exportRFQAnalysisReport(paging);

        }
        #endregion

        #region  RFQ "non award reason report" with chart
        [HttpPostRoute("GetRFQNonAwardReasonReport")]
        public ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQNonAwardReasonReport>> GetRFQNonAwardReasonReport(GenericPage<MES.DTO.Library.RFQ.RFQReports.RFQNonAwardReasonReportSearch> paging)
        {
            var type = this.Resolve<IRFQReportsRepository>(RFQReportsRepository).GetRFQNonAwardReasonReport(paging);
            return type;
        }
        [HttpPostRoute("exportRFQNonAwardReasonReport")]
        public ITypedResponse<bool?> exportRFQNonAwardReasonReport(GenericPage<MES.DTO.Library.RFQ.RFQReports.RFQNonAwardReasonReportSearch> paging)
        {
            var type = this.Resolve<IRFQReportsRepository>(RFQReportsRepository).exportRFQNonAwardReasonReport(paging);
            return type;
        }
        #endregion

        #region Quote's Total Dollar Quoted Report
        [HttpPostRoute("GetQuoteTotalDollarQuotedReport")]
        public ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReport>> GetQuoteTotalDollarQuotedReport(GenericPage<MES.DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReportSearch> paging)
        {
            var type = this.Resolve<IRFQReportsRepository>(RFQReportsRepository).GetQuoteTotalDollarQuotedReport(paging);
            return type;
        }
        [HttpPostRoute("exportQuoteTotalDollarQuotedReport")]
        public ITypedResponse<bool?> exportQuoteTotalDollarQuotedReport(GenericPage<MES.DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReportSearch> paging)
        {
            var type = this.Resolve<IRFQReportsRepository>(RFQReportsRepository).exportQuoteTotalDollarQuotedReport(paging);
            return type;
        }
        #endregion

        #region Quotes done Report
        [HttpPostRoute("GetQuotesDoneReport")]
        public ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.QuotesDoneReport>> GetQuotesDoneReport(GenericPage<MES.DTO.Library.RFQ.RFQReports.QuotesDoneReportSearch> paging)
        {
            var type = this.Resolve<IRFQReportsRepository>(RFQReportsRepository).GetQuotesDoneReport(paging);
            return type;
        }
        [HttpPostRoute("exportQuotesDoneReport")]
        public ITypedResponse<bool?> exportQuotesDoneReport(GenericPage<MES.DTO.Library.RFQ.RFQReports.QuotesDoneReportSearch> paging)
        {
            var type = this.Resolve<IRFQReportsRepository>(RFQReportsRepository).exportQuotesDoneReport(paging);
            return type;
        }
        #endregion

        #region RFQ Quote Report by Supplier
        [HttpPostRoute("GetRFQPartsSupplierWiseReport")]
        public ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReport>> GetRFQPartsSupplierWiseReport(GenericPage<MES.DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReportSearch> paging)
        {
            var type = this.Resolve<IRFQReportsRepository>(RFQReportsRepository).GetRFQPartsSupplierWiseReport(paging);
            return type;
        }
        [HttpPostRoute("exportRFQPartsSupplierWiseReport")]
        public ITypedResponse<bool?> exportRFQPartsSupplierWiseReport(GenericPage<MES.DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReportSearch> paging)
        {
            var type = this.Resolve<IRFQReportsRepository>(RFQReportsRepository).exportRFQPartsSupplierWiseReport(paging);
            return type;
        }
        #endregion

        #region Supplier Parts Quote Report
        [HttpPostRoute("GetRFQSupplierPartsQuoteReport")]
        public ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQSupplierPartsQuoteReport>> GetRFQSupplierPartsQuoteReport(GenericPage<MES.DTO.Library.RFQ.RFQReports.RFQSupplierPartsQuoteReportSearch> paging)
        {
            var type = this.Resolve<IRFQReportsRepository>(RFQReportsRepository).GetRFQSupplierPartsQuoteReport(paging);
            return type;
        }
        [HttpPostRoute("exportRFQSupplierPartsQuoteReport")]
        public ITypedResponse<bool?> exportRFQSupplierPartsQuoteReport(GenericPage<MES.DTO.Library.RFQ.RFQReports.RFQSupplierPartsQuoteReportSearch> paging)
        {
            var type = this.Resolve<IRFQReportsRepository>(RFQReportsRepository).exportRFQSupplierPartsQuoteReport(paging);
            return type;
        }
        #endregion

        #region RFQ Part Cost Comparison Report
        [HttpPostRoute("GetRFQPartCostComparisonReport")]
        public ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQ>> GetRFQPartCostComparisonReport(GenericPage<MES.DTO.Library.RFQ.RFQReports.RFQPartCostComparisonReportSearch> paging)
        {
            var type = this.Resolve<IRFQReportsRepository>(RFQReportsRepository).GetRFQPartCostComparisonReport(paging);
            return type;
        }
        [HttpPostRoute("exportRFQPartCostComparisonReport")]
        public ITypedResponse<bool?> exportRFQPartCostComparisonReport(GenericPage<MES.DTO.Library.RFQ.RFQReports.RFQPartCostComparisonReportSearch> paging)
        {
            var type = this.Resolve<IRFQReportsRepository>(RFQReportsRepository).exportRFQPartCostComparisonReport(paging);
            return type;
        }
        #endregion

        #region  RFQs Quoted By Supplier
        [HttpPostRoute("GetRFQQuotedBySupplierReport")]
        public ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQQuotedBySupplierReport>> GetRFQQuotedBySupplierReport(GenericPage<MES.DTO.Library.RFQ.RFQReports.RFQQuotedBySupplierReportSearch> paging)
        {
            var type = this.Resolve<IRFQReportsRepository>(RFQReportsRepository).GetRFQQuotedBySupplierReport(paging);
            return type;
        }
        [HttpPostRoute("exportRFQQuotedBySupplierReport")]
        public ITypedResponse<bool?> exportRFQQuotedBySupplierReport(GenericPage<MES.DTO.Library.RFQ.RFQReports.RFQQuotedBySupplierReportSearch> paging)
        {
            var type = this.Resolve<IRFQReportsRepository>(RFQReportsRepository).exportRFQQuotedBySupplierReport(paging);
            return type;
        }
        #endregion

        #region RFQ Supplier list report
        [HttpPostRoute("GetRFQSupplierReport")]
        public ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQSupplierReport>> GetRFQSupplierReport(GenericPage<MES.DTO.Library.RFQ.RFQReports.RFQSupplierReportSearch> paging)
        {
            var type = this.Resolve<IRFQReportsRepository>(RFQReportsRepository).GetRFQSupplierReport(paging);
            return type;
        }
        [HttpPostRoute("exportRFQSupplierReport")]
        public ITypedResponse<bool?> exportRFQSupplierReport(GenericPage<MES.DTO.Library.RFQ.RFQReports.RFQSupplierReportSearch> paging)
        {
            var type = this.Resolve<IRFQReportsRepository>(RFQReportsRepository).exportRFQSupplierReport(paging);
            return type;
        }
        #endregion

        #region RFQ Supplier Activity Level Report
        [HttpPostRoute("GetRFQSupplierActivityLevelReport")]
        public ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQSupplierActivityLevelReport>> GetRFQSupplierActivityLevelReport(GenericPage<MES.DTO.Library.RFQ.RFQReports.RFQSupplierActivityLevelReportSearch> paging)
        {
            var type = this.Resolve<IRFQReportsRepository>(RFQReportsRepository).GetRFQSupplierActivityLevelReport(paging);
            return type;
        }
        [HttpPostRoute("exportRFQSupplierActivityLevelReport")]
        public ITypedResponse<bool?> exportRFQSupplierActivityLevelReport(GenericPage<MES.DTO.Library.RFQ.RFQReports.RFQSupplierActivityLevelReportSearch> paging)
        {
            var type = this.Resolve<IRFQReportsRepository>(RFQReportsRepository).exportRFQSupplierActivityLevelReport(paging);
            return type;
        }
        #endregion

        #region open RFQ report
        [HttpPostRoute("GetOpenRFQsReport")]
        public ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.OpenRFQsReport>> GetOpenRFQsReport(GenericPage<MES.DTO.Library.RFQ.RFQReports.OpenRFQsReportSearch> paging)
        {
            var type = this.Resolve<IRFQReportsRepository>(RFQReportsRepository).GetOpenRFQsReport(paging);
            return type;
        }
        [HttpPostRoute("exportOpenRFQsReport")]
        public ITypedResponse<bool?> exportOpenRFQsReport(GenericPage<MES.DTO.Library.RFQ.RFQReports.OpenRFQsReportSearch> paging)
        {
            var type = this.Resolve<IRFQReportsRepository>(RFQReportsRepository).exportOpenRFQsReport(paging);
            return type;
        }
        #endregion

        #region Detailed Supplier Quote report
        [HttpPostRoute("GetDetailedSupplierQuoteReport")]
        public ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteReport>> GetDetailedSupplierQuoteReport(GenericPage<MES.DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteSearch> paging)
        {
            var type = this.Resolve<IRFQReportsRepository>(RFQReportsRepository).GetDetailedSupplierQuoteReport(paging);
            return type;
        }
        [HttpPostRoute("GetRfqPartQuoteDetailsReport")]
        public ITypedResponse<List<DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteSubReportFirstPart>> GetRfqPartQuoteDetailsReport(GenericPage<MES.DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteSearch> paging)
        {
            var type = this.Resolve<IRFQReportsRepository>(RFQReportsRepository).GetRfqPartQuoteDetailsReport(paging);
            return type;
        }
        [HttpPostRoute("exportDetailedSupplierQuoteReport")]
        public ITypedResponse<bool?> exportDetailedSupplierQuoteReport(GenericPage<MES.DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteSearch> paging)
        {
            var type = this.Resolve<IRFQReportsRepository>(RFQReportsRepository).exportDetailedSupplierQuoteReport(paging);
            return type;
        }
        #endregion
    }
}
