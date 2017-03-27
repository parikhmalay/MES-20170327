using Account.DTO.Library;
using MES.Business.Repositories.Dashboard;
using NPE.Core;
using NPE.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace MES.Business.Library.BO.Dashboard
{
    class Dashboard : ContextBusinessBase, IDashboardRepository
    {
        public Dashboard()
            : base("Dashboard")
        { }
        #region get Dashboard summary
        public NPE.Core.ITypedResponse<MES.DTO.Library.Dashboard.Dashboard> GetDashboardSummary(NPE.Core.IPage<MES.DTO.Library.Dashboard.SearchCriteria> paging)
        {
            string errMSg = null;
            DTO.Library.Dashboard.Dashboard dashboard = null;
            this.RunOnDB(context =>
            {
                var data = context.GetDashboardSummary(paging.Criteria.DateFrom, paging.Criteria.DateTo).SingleOrDefault();
                if (data == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    dashboard = new DTO.Library.Dashboard.Dashboard();
                    dashboard.RFQsCount = data.RFQsCount;
                    dashboard.QuotesCount = data.QuotesCount;
                    dashboard.APQPCount = data.APQPCount;
                    dashboard.DefectTrackingCount = data.DefectTrackingCount;
                    dashboard.lstRFQAnalysisChart = GetRFQAnalysisReportQTC(paging);
                    dashboard.lstQuotesDoneChart = GetQuotesDoneReportChart(paging);
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.Dashboard.Dashboard>(errMSg, dashboard);
            return response;
        }

        public List<DTO.Library.RFQ.RFQReports.RFQAnalysisChart> GetRFQAnalysisReportQTC(NPE.Core.IPage<DTO.Library.Dashboard.SearchCriteria> searchInfo)
        {
            string errMSg = null;
            List<DTO.Library.RFQ.RFQReports.RFQAnalysisChart> lstRFQAnalysisChart = new List<DTO.Library.RFQ.RFQReports.RFQAnalysisChart>();
            DTO.Library.RFQ.RFQReports.RFQAnalysisChart rFQAnalysisChart;
            this.RunOnDB(context =>
            {
                //var chartData = context.RFQAnalysisChartForAlina(null, null, searchInfo.Criteria.DateFrom, searchInfo.Criteria.DateTo, null, null, null).ToList();
                var chartData = context.BusinessAnalysisChartQTC(null, null, searchInfo.Criteria.DateFrom, searchInfo.Criteria.DateTo, null, null, null, null, null).ToList();
                if (chartData == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    foreach (var chart in chartData)
                    {
                        rFQAnalysisChart = new DTO.Library.RFQ.RFQReports.RFQAnalysisChart();
                        rFQAnalysisChart.DisplayName = chart.DisplayName;
                        rFQAnalysisChart.NoOfRfq = chart.NoOfQuote;
                        lstRFQAnalysisChart.Add(rFQAnalysisChart);
                    }
                }
            });
            return lstRFQAnalysisChart;
        }

        public List<DTO.Library.RFQ.RFQReports.QuotesDoneChart> GetQuotesDoneReportChart(NPE.Core.IPage<DTO.Library.Dashboard.SearchCriteria> searchInfo)
        {
            string errMSg = null;
            List<DTO.Library.RFQ.RFQReports.QuotesDoneChart> lstQuotesDoneChart = new List<DTO.Library.RFQ.RFQReports.QuotesDoneChart>();
            DTO.Library.RFQ.RFQReports.QuotesDoneChart quotesDoneChart;
            this.RunOnDB(context =>
            {
                var chartData = context.GetQuotesByCriteriaChart(null, null, searchInfo.Criteria.DateFrom, searchInfo.Criteria.DateTo, "Commodity").ToList();
                if (chartData == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    foreach (var chart in chartData)
                    {
                        quotesDoneChart = new DTO.Library.RFQ.RFQReports.QuotesDoneChart();
                        quotesDoneChart.DisplayName = chart.DisplayName;
                        quotesDoneChart.Amount = chart.Amount;
                        lstQuotesDoneChart.Add(quotesDoneChart);
                    }
                }
            });
            return lstQuotesDoneChart;
        }
        #endregion
    }
}
