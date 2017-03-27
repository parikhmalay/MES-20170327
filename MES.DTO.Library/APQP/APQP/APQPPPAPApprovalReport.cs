using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.APQP
{
    public class APQPWeeklyMeetingReport
    {
        public string APQPEngineerId { get; set; }
        public string APQPEngineer { get; set; }
        public string CustomerName { get; set; }
        public string PartNumber { get; set; }
        public int PartNumberCount { get; set; }
        public string KickOffComments { get; set; }
        public string ToolingLaunchComments { get; set; }
        public string ProjectTrackingComments { get; set; }
        public string ProjectTrackingRemarks { get; set; }
        public string PPAPComments { get; set; }
        public List<CustomerNameValue> objCustomerNameValue { get; set; }
    }
    public class APQPWeeklyMeetingReportViewModel
    {
        public string APQPEngineerId { get; set; }
        public string APQPEngineer { get; set; }
        public List<APQPWeeklyMeetingReport> lstAPQPWeeklyMeetingReportPartA { get; set; }
        public List<APQPWeeklyMeetingReport> lstAPQPWeeklyMeetingReportPartB { get; set; }
        public List<APQPWeeklyMeetingReport> lstAPQPWeeklyMeetingReportPartC { get; set; }
        public List<APQPWeeklyMeetingReport> lstAPQPWeeklyMeetingReportPartD { get; set; }
    }
    public class APQPWeeklyMeetingReportSearchCriteria
    {
        public Nullable<System.DateTime> DateFrom { get; set; }
        public Nullable<System.DateTime> DateTo { get; set; }
        public string SearchHeading { get; set; }
    }
    public class CustomerNameValue
    {
        public string CustomerName { get; set; }
        public List<NameValue> lstNameValue { get; set; }
    }
    public class NameValue
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
