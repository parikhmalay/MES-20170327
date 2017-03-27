using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.Dashboard
{
    public class Dashboard : CreateEditPropBase
    {
        #region Model property
        public Nullable<int> RFQsCount { get; set; }
        public Nullable<int> QuotesCount { get; set; }
        public Nullable<int> APQPCount { get; set; }
        public Nullable<int> DefectTrackingCount { get; set; }
        public List<DTO.Library.RFQ.RFQReports.RFQAnalysisChart> lstRFQAnalysisChart { get; set; }
        public List<DTO.Library.RFQ.RFQReports.QuotesDoneChart> lstQuotesDoneChart { get; set; }
        #endregion
    }

    public class SearchCriteria
    {
        public Nullable<System.DateTime> DateFrom { get; set; }
        public Nullable<System.DateTime> DateTo { get; set; }
    }
}
