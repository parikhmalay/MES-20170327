using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.RFQ.RFQReports
{
    public interface IRFQReportsRepository : ICrudMethods<MES.DTO.Library.RFQ.RFQReports.RFQReports, int?, string,
           MES.DTO.Library.RFQ.RFQReports.RFQReports, int, bool?, int, MES.DTO.Library.RFQ.RFQReports.RFQReports>
    {
        #region RFQ Analysis and Business(Alina) report
        ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQAnalysisReport>> GetRFQAnalysisReport(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.RFQAnalysisReportSearch> searchInfo);
        ITypedResponse<bool?> exportRFQAnalysisReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.RFQAnalysisReportSearch> paging);
        ITypedResponse<bool?> exportBusinessAnalysisQTCReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.RFQAnalysisReportSearch> paging);
        #endregion
        #region RFQ Non-Award Reason Report
        ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQNonAwardReasonReport>> GetRFQNonAwardReasonReport(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.RFQNonAwardReasonReportSearch> searchInfo);
        ITypedResponse<bool?> exportRFQNonAwardReasonReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.RFQNonAwardReasonReportSearch> paging);
        #endregion
        #region Quote's Total Dollar Quoted Report
        ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReport>> GetQuoteTotalDollarQuotedReport(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReportSearch> searchInfo);
        ITypedResponse<bool?> exportQuoteTotalDollarQuotedReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReportSearch> paging);
        #endregion
        #region Quotes done Report
        ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.QuotesDoneReport>> GetQuotesDoneReport(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.QuotesDoneReportSearch> searchInfo);
        ITypedResponse<bool?> exportQuotesDoneReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.QuotesDoneReportSearch> paging);
        #endregion
        #region RFQ Quote Report by Supplier
        ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReport>> GetRFQPartsSupplierWiseReport(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReportSearch> searchInfo);
        ITypedResponse<bool?> exportRFQPartsSupplierWiseReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReportSearch> paging);
        #endregion
        #region Supplier Parts Quote Report
        ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQSupplierPartsQuoteReport>> GetRFQSupplierPartsQuoteReport(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.RFQSupplierPartsQuoteReportSearch> searchInfo);
        ITypedResponse<bool?> exportRFQSupplierPartsQuoteReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.RFQSupplierPartsQuoteReportSearch> paging);
        #endregion
        #region RFQ Part Cost Comparison Report
        ITypedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQ>> GetRFQPartCostComparisonReport(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.RFQPartCostComparisonReportSearch> searchInfo);
        ITypedResponse<bool?> exportRFQPartCostComparisonReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.RFQPartCostComparisonReportSearch> paging);
        #endregion
        #region RFQs Quoted By Supplier
        ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQQuotedBySupplierReport>> GetRFQQuotedBySupplierReport(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.RFQQuotedBySupplierReportSearch> searchInfo);
        ITypedResponse<bool?> exportRFQQuotedBySupplierReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.RFQQuotedBySupplierReportSearch> paging);
        #endregion
        #region RFQ Supplier list report
        ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQSupplierReport>> GetRFQSupplierReport(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.RFQSupplierReportSearch> searchInfo);
        ITypedResponse<bool?> exportRFQSupplierReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.RFQSupplierReportSearch> paging);
        #endregion
        #region RFQ Supplier Activity Level Report
        ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQSupplierActivityLevelReport>> GetRFQSupplierActivityLevelReport(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.RFQSupplierActivityLevelReportSearch> searchInfo);
        ITypedResponse<bool?> exportRFQSupplierActivityLevelReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.RFQSupplierActivityLevelReportSearch> paging);
        #endregion
        #region open RFQ report
        ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.OpenRFQsReport>> GetOpenRFQsReport(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.OpenRFQsReportSearch> searchInfo);
        ITypedResponse<bool?> exportOpenRFQsReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.OpenRFQsReportSearch> paging);
        #endregion
        #region Detailed Supplier Quote report

        ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteReport>> GetDetailedSupplierQuoteReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteSearch> searchInfo);
        ITypedResponse<List<DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteSubReportFirstPart>> GetRfqPartQuoteDetailsReport(IPage<DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteSearch> searchInfo);      
        ITypedResponse<bool?> exportDetailedSupplierQuoteReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteSearch> paging);

        #endregion
    }
}
