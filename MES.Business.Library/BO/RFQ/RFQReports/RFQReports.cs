using Account.DTO.Library;
using MES.Business.Repositories.RFQ.RFQReports;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using NPE.Core.Extensions;
using NPE.Core;
using System.Data.Entity;
using System.IO;
using System.Web;
using Winnovative.ExcelLib;
using GemBox.Spreadsheet;
using MES.Business.Library.Extensions;

namespace MES.Business.Library.BO.RFQ.RFQReports
{
    public class RFQReports : ContextBusinessBase, IRFQReportsRepository
    {
        public RFQReports()
            : base("RFQReports")
        { }

        #region General
        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.RFQ.RFQReports.RFQReports rFQParts)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.RFQReports.RFQReports> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int RFQPartId)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQReports.RFQReports>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region RFQ Analysis and Business(Alina) report with chart
        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQReports.RFQAnalysisReport>> GetRFQAnalysisReport(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.RFQAnalysisReportSearch> searchInfo)
        {
            if (searchInfo.Criteria.ReportId == 9)
                return GetRFQAnalysisReportData(searchInfo);
            else if (searchInfo.Criteria.ReportId == 16)
                return GetRFQAnalysisReportQTC(searchInfo);
            else
                return SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQAnalysisReport>>("", new List<DTO.Library.RFQ.RFQReports.RFQAnalysisReport>());
        }

        private ITypedResponse<List<DTO.Library.RFQ.RFQReports.RFQAnalysisReport>> GetRFQAnalysisReportQTC(IPage<DTO.Library.RFQ.RFQReports.RFQAnalysisReportSearch> searchInfo)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.RFQ.RFQReports.RFQAnalysisReport> lstRFQAnalysisReport = new List<DTO.Library.RFQ.RFQReports.RFQAnalysisReport>();
            DTO.Library.RFQ.RFQReports.RFQAnalysisReport rFQAnalysisReport;
            DTO.Library.RFQ.RFQReports.RFQAnalysisReportDetails rFQAnalysisReportDetails;
            List<DTO.Library.RFQ.RFQReports.RFQAnalysisChart> lstRFQAnalysisChart = new List<DTO.Library.RFQ.RFQReports.RFQAnalysisChart>();
            DTO.Library.RFQ.RFQReports.RFQAnalysisChart rFQAnalysisChart;
            this.RunOnDB(context =>
            {
                var firstPartData = context.BusinessAnalysisReportQTCFirstPart(searchInfo.Criteria.SAMIds, searchInfo.Criteria.CustomerIds, searchInfo.Criteria.DateFrom, searchInfo.Criteria.DateTo, searchInfo.Criteria.CommodityIds, searchInfo.Criteria.RFQSourceIds, searchInfo.Criteria.IndustryTypeIds).ToList();
                var secondPartData = context.BusinessAnalysisReportQTCSecondPart(searchInfo.Criteria.SAMIds, searchInfo.Criteria.CustomerIds, searchInfo.Criteria.DateFrom, searchInfo.Criteria.DateTo, searchInfo.Criteria.CommodityIds, searchInfo.Criteria.RFQSourceIds, searchInfo.Criteria.RFQTypeIds, searchInfo.Criteria.IndustryTypeIds).ToList();
                var chartData = context.BusinessAnalysisChartQTC(searchInfo.Criteria.SAMIds, searchInfo.Criteria.CustomerIds, searchInfo.Criteria.DateFrom, searchInfo.Criteria.DateTo, searchInfo.Criteria.CommodityIds, searchInfo.Criteria.RFQSourceIds, searchInfo.Criteria.RFQTypeIds, searchInfo.Criteria.IndustryTypeIds, searchInfo.Criteria.GroupBy).ToList();
                if (firstPartData == null || secondPartData == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records

                    foreach (var chart in chartData)
                    {
                        rFQAnalysisChart = new DTO.Library.RFQ.RFQReports.RFQAnalysisChart();
                        rFQAnalysisChart.DisplayName = chart.DisplayName;
                        rFQAnalysisChart.NoOfRfq = chart.NoOfQuote;
                        lstRFQAnalysisChart.Add(rFQAnalysisChart);
                    }

                    foreach (var firstPart in firstPartData)
                    {
                        rFQAnalysisReport = new DTO.Library.RFQ.RFQReports.RFQAnalysisReport();
                        rFQAnalysisReport.RfqDate = firstPart.QuoteDate;
                        rFQAnalysisReport.SortOrder = firstPart.SortOrder;
                        rFQAnalysisReport.lstRFQAnalysisReportDetails = new List<DTO.Library.RFQ.RFQReports.RFQAnalysisReportDetails>();
                        foreach (var secondPart in secondPartData.Where(a => a.QuoteDate == firstPart.QuoteDate).ToList())
                        {
                            rFQAnalysisReportDetails = new DTO.Library.RFQ.RFQReports.RFQAnalysisReportDetails();
                            rFQAnalysisReportDetails.RFQ = secondPart.QuoteNumber;
                            rFQAnalysisReportDetails.RfqDate = secondPart.QuoteDate;
                            rFQAnalysisReportDetails.QuoteDate = Convert.ToDateTime(secondPart.Date).ToString("dd-MMM-yy");
                            rFQAnalysisReportDetails.Customer = secondPart.Customer;
                            rFQAnalysisReportDetails.AccountManager = secondPart.AccountManager;
                            rFQAnalysisReportDetails.totalquoted = secondPart.totalquoted ?? 0;
                            rFQAnalysisReportDetails.ToolingCost = secondPart.ToolingCost ?? 0;
                            rFQAnalysisReportDetails.Commodity = secondPart.Commodity;
                            rFQAnalysisReportDetails.Process = secondPart.Process;
                            rFQAnalysisReportDetails.sortorder = secondPart.sortorder;
                            rFQAnalysisReportDetails.RFQSource = secondPart.RFQSource;
                            rFQAnalysisReportDetails.RFQType = secondPart.RFQTypeName;
                            rFQAnalysisReportDetails.IndustryType = secondPart.IndustryType;
                            //extra properties for grouping
                            if (!string.IsNullOrEmpty(searchInfo.Criteria.GroupBy))
                            {
                                if (searchInfo.Criteria.GroupBy == "Commodity")
                                    rFQAnalysisReportDetails.GroupByValue = string.IsNullOrEmpty(secondPart.Commodity) == true ? "Commodity Not Assigned" : secondPart.Commodity;
                                else if (searchInfo.Criteria.GroupBy == "SAM")
                                    rFQAnalysisReportDetails.GroupByValue = string.IsNullOrEmpty(secondPart.AccountManager) == true ? "Account Manager Not Assigned" : secondPart.AccountManager;
                                else if (searchInfo.Criteria.GroupBy == "RFQSource")
                                    rFQAnalysisReportDetails.GroupByValue = string.IsNullOrEmpty(secondPart.RFQSource) == true ? "RFQ Source Not Assigned" : secondPart.RFQSource;
                            }
                            else
                                rFQAnalysisReportDetails.GroupByValue = "";
                            rFQAnalysisReport.lstRFQAnalysisReportDetails.Add(rFQAnalysisReportDetails);
                        }
                        rFQAnalysisReport.RFQDateTotalQuoted = rFQAnalysisReport.lstRFQAnalysisReportDetails.Sum(a => a.totalquoted);
                        rFQAnalysisReport.RFQDateTotalToolingCost = rFQAnalysisReport.lstRFQAnalysisReportDetails.Sum(a => a.ToolingCost);
                        rFQAnalysisReport.lstRFQAnalysisChart = lstRFQAnalysisChart;
                        lstRFQAnalysisReport.Add(rFQAnalysisReport);
                    }
                }
            });
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQAnalysisReport>>(errMSg, lstRFQAnalysisReport);
            response.PageInfo = searchInfo.ToPage();
            return response;
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQReports.RFQAnalysisReport>> GetRFQAnalysisReportData(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.RFQAnalysisReportSearch> searchInfo)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.RFQ.RFQReports.RFQAnalysisReport> lstRFQAnalysisReport = new List<DTO.Library.RFQ.RFQReports.RFQAnalysisReport>();
            DTO.Library.RFQ.RFQReports.RFQAnalysisReport rFQAnalysisReport;
            DTO.Library.RFQ.RFQReports.RFQAnalysisReportDetails rFQAnalysisReportDetails;
            List<DTO.Library.RFQ.RFQReports.RFQAnalysisChart> lstRFQAnalysisChart = new List<DTO.Library.RFQ.RFQReports.RFQAnalysisChart>();
            DTO.Library.RFQ.RFQReports.RFQAnalysisChart rFQAnalysisChart;
            this.RunOnDB(context =>
            {
                var firstPartData = context.RFQAnalysisReportFirstPart(searchInfo.Criteria.SAMIds, searchInfo.Criteria.CustomerIds, searchInfo.Criteria.DateFrom, searchInfo.Criteria.DateTo, searchInfo.Criteria.CommodityIds, searchInfo.Criteria.RFQSourceIds).ToList();
                var secondPartData = context.RFQAnalysisReportSecondPart(searchInfo.Criteria.SAMIds, searchInfo.Criteria.CustomerIds, searchInfo.Criteria.DateFrom, searchInfo.Criteria.DateTo, searchInfo.Criteria.CommodityIds, searchInfo.Criteria.RFQSourceIds).ToList();
                var chartData = context.RfqAnalysisChart(searchInfo.Criteria.SAMIds, searchInfo.Criteria.CustomerIds, searchInfo.Criteria.DateFrom, searchInfo.Criteria.DateTo, searchInfo.Criteria.CommodityIds, searchInfo.Criteria.RFQSourceIds, searchInfo.Criteria.GroupBy).ToList();
                if (firstPartData == null || secondPartData == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records

                    foreach (var chart in chartData)
                    {
                        rFQAnalysisChart = new DTO.Library.RFQ.RFQReports.RFQAnalysisChart();
                        rFQAnalysisChart.DisplayName = chart.DisplayName;
                        rFQAnalysisChart.NoOfRfq = chart.NoOfRfq;
                        lstRFQAnalysisChart.Add(rFQAnalysisChart);
                    }

                    foreach (var firstPart in firstPartData)
                    {
                        rFQAnalysisReport = new DTO.Library.RFQ.RFQReports.RFQAnalysisReport();
                        rFQAnalysisReport.RfqDate = firstPart.RfqDate;
                        rFQAnalysisReport.NoOfRfq = firstPart.NoOfRfq;
                        rFQAnalysisReport.NewRFQ = firstPart.NewRFQ;
                        rFQAnalysisReport.NewCustomer = firstPart.NewCustomer;
                        rFQAnalysisReport.SortOrder = firstPart.SortOrder;
                        rFQAnalysisReport.lstRFQAnalysisReportDetails = new List<DTO.Library.RFQ.RFQReports.RFQAnalysisReportDetails>();
                        foreach (var secondPart in secondPartData.Where(a => a.RfqDate == firstPart.RfqDate).ToList())
                        {
                            rFQAnalysisReportDetails = new DTO.Library.RFQ.RFQReports.RFQAnalysisReportDetails();
                            rFQAnalysisReportDetails.RFQ = secondPart.RFQ;
                            rFQAnalysisReportDetails.RfqDate = secondPart.RfqDate;
                            rFQAnalysisReportDetails.Customer = secondPart.Customer;
                            rFQAnalysisReportDetails.AccountManager = secondPart.AccountManager;
                            rFQAnalysisReportDetails.totalquoted = secondPart.totalquoted ?? 0;
                            rFQAnalysisReportDetails.ToolingCost = secondPart.ToolingCost ?? 0;
                            rFQAnalysisReportDetails.Commodity = secondPart.Commodity;
                            rFQAnalysisReportDetails.Process = secondPart.Process;
                            rFQAnalysisReportDetails.sortorder = secondPart.sortorder;
                            rFQAnalysisReportDetails.RFQSource = secondPart.RFQSource;
                            rFQAnalysisReportDetails.RFQType = secondPart.RFQTypeName;
                            //extra properties for grouping
                            if (!string.IsNullOrEmpty(searchInfo.Criteria.GroupBy))
                            {
                                if (searchInfo.Criteria.GroupBy == "Commodity")
                                    rFQAnalysisReportDetails.GroupByValue = string.IsNullOrEmpty(secondPart.Commodity) == true ? "Commodity Not Assigned" : secondPart.Commodity;
                                else if (searchInfo.Criteria.GroupBy == "SAM")
                                    rFQAnalysisReportDetails.GroupByValue = string.IsNullOrEmpty(secondPart.AccountManager) == true ? "Account Manager Not Assigned" : secondPart.AccountManager;
                                else if (searchInfo.Criteria.GroupBy == "RFQSource")
                                    rFQAnalysisReportDetails.GroupByValue = string.IsNullOrEmpty(secondPart.RFQSource) == true ? "RFQ Source Not Assigned" : secondPart.RFQSource;
                            }
                            else
                                rFQAnalysisReportDetails.GroupByValue = "";
                            rFQAnalysisReport.lstRFQAnalysisReportDetails.Add(rFQAnalysisReportDetails);
                        }
                        rFQAnalysisReport.RFQDateTotalQuoted = rFQAnalysisReport.lstRFQAnalysisReportDetails.Sum(a => a.totalquoted);
                        rFQAnalysisReport.RFQDateTotalToolingCost = rFQAnalysisReport.lstRFQAnalysisReportDetails.Sum(a => a.ToolingCost);
                        rFQAnalysisReport.lstRFQAnalysisChart = lstRFQAnalysisChart;
                        lstRFQAnalysisReport.Add(rFQAnalysisReport);
                    }
                }
            });
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQAnalysisReport>>(errMSg, lstRFQAnalysisReport);
            response.PageInfo = searchInfo.ToPage();
            return response;
        }

        public NPE.Core.ITypedResponse<bool?> exportRFQAnalysisReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.RFQAnalysisReportSearch> paging)
        {
            string filePath = string.Empty;
            var httpcontext = HttpContext.Current;
            string strBody = string.Empty;
            filePath = httpcontext.Server.MapPath("~/" + Constants.REPORTSTEMPLATEFOLDER) + "RFQAnalysisTemplate.xls";

            if (File.Exists(filePath))
            {
                try
                {
                    System.IO.FileStream sourceXlsDataStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    ExcelWorkbook ewb = new ExcelWorkbook(sourceXlsDataStream);
                    ewb.LicenseKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["wnvkey"]);
                    Winnovative.ExcelLib.ExcelWorksheet ews1 = ewb.Worksheets[0];
                    Winnovative.ExcelLib.ExcelWorksheet ews2 = ewb.Worksheets[1];

                    #region RFQ Analysis Report Chart Create

                    if (ews2.UsedRangeCells.Count > 0)
                        ews2.UsedRange.Clear();

                    ews2[1, 1].Value = "SAM"; //1. SAM
                    ews2[1, 2].Value = "No. Of RFQ"; //2. No. Of RFQ

                    int counter = 1, count = 0;
                    string GroupByType = string.Empty;
                    short groupBy = 1;
                    if (groupBy == 1)
                    {
                        GroupByType = "Commodity";
                        ews2[1, 1].Value = "Commodity"; //1. Commodity
                    }
                    else if (groupBy == 2)
                    {
                        GroupByType = "SAM";
                        ews2[1, 1].Value = "SAM"; //2. SAM                        
                    }
                    else if (groupBy == 3)
                    {
                        GroupByType = "RFQSource";
                        ews2[1, 1].Value = "RFQ Source"; //3. RFQ Source                        
                    }
                    else
                        ews2[1, 1].Value = "Month";

                    ews2[1, 2].Value = "No. Of RFQ"; //2. No. Of RFQ
                    List<DTO.Library.RFQ.RFQReports.RFQAnalysisReport> lstRFQAnalysisReport = new List<DTO.Library.RFQ.RFQReports.RFQAnalysisReport>();

                    lstRFQAnalysisReport = GetRFQAnalysisReport(paging).Result;

                    List<DTO.Library.RFQ.RFQReports.RFQAnalysisChart> lstRFQAnalysisChart = new List<DTO.Library.RFQ.RFQReports.RFQAnalysisChart>();

                    lstRFQAnalysisChart = lstRFQAnalysisReport[0].lstRFQAnalysisChart;
                    if (lstRFQAnalysisChart.Count > 0)
                    {
                        ExcelRange er;
                        count = lstRFQAnalysisChart.Count + 1;
                        foreach (var item in lstRFQAnalysisChart)
                        {
                            counter++;
                            ews2[counter, 1].Value = item.DisplayName.ToString() + " (" + Convert.ToDouble(item.NoOfRfq.ToString()) + "%)";
                            ews2[counter, 2].NumberValue = Convert.ToDouble(item.NoOfRfq.ToString());
                        }

                        er = ews2[1, 1, count, 2];
                        ews1.Charts[0].DataSourceRange = er;
                    }

                    #endregion

                    #region  RFQ Analysis Report List Create
                    int intRFQRowCounter = 1;

                    foreach (DTO.Library.RFQ.RFQReports.RFQAnalysisReport drRFQ in lstRFQAnalysisReport)
                    {
                        decimal dTotalQuoted = 0;
                        decimal dToolingCost = 0;
                        int intMainRfqRowCount = intRFQRowCounter + 1;
                        int intMergeRowCount = 0;

                        foreach (MES.DTO.Library.RFQ.RFQReports.RFQAnalysisReportDetails drRFQItem in drRFQ.lstRFQAnalysisReportDetails)
                        {
                            intRFQRowCounter++;
                            intMergeRowCount++;
                            ews1[intRFQRowCounter, 3].Text = drRFQItem.RFQ;
                            ews1[intRFQRowCounter, 4].Text = drRFQItem.Customer;
                            ews1[intRFQRowCounter, 5].Text = !string.IsNullOrEmpty(drRFQItem.AccountManager) ? drRFQItem.AccountManager : string.Empty;
                            ews1[intRFQRowCounter, 6].Text = !string.IsNullOrEmpty(drRFQItem.Commodity) ? drRFQItem.Commodity : string.Empty;
                            ews1[intRFQRowCounter, 7].Text = !string.IsNullOrEmpty(drRFQItem.Process) ? drRFQItem.Process : string.Empty;
                            ews1[intRFQRowCounter, 8].Text = !string.IsNullOrEmpty(drRFQItem.RFQSource) ? drRFQItem.RFQSource : string.Empty;
                            ews1[intRFQRowCounter, 9].Text = !string.IsNullOrEmpty(drRFQItem.RFQType) ? drRFQItem.RFQType : string.Empty;
                            if (!string.IsNullOrEmpty(drRFQItem.totalquoted.ToString()))
                            {
                                ews1[intRFQRowCounter, 10].Text = string.Format("${0:#,##0.000}", Convert.ToDecimal(drRFQItem.totalquoted.ToString()));
                                dTotalQuoted = dTotalQuoted + Convert.ToDecimal(drRFQItem.totalquoted.ToString());
                            }
                            else
                            {
                                ews1[intRFQRowCounter, 10].Text = string.Format("${0:#,##0.000}", 0);
                            }
                            ews1[intRFQRowCounter, 10].Style.Alignment.HorizontalAlignment = ExcelCellHorizontalAlignmentType.Right;
                            if (!string.IsNullOrEmpty(drRFQItem.ToolingCost.ToString()))
                            {
                                ews1[intRFQRowCounter, 11].Text = string.Format("${0:#,##0.000}", Convert.ToDecimal(drRFQItem.ToolingCost.ToString()));
                                dToolingCost = dToolingCost + Convert.ToDecimal(drRFQItem.ToolingCost.ToString());
                            }
                            else
                            {
                                ews1[intRFQRowCounter, 11].Text = string.Format("${0:#,##0.000}", 0);
                            }
                            ews1[intRFQRowCounter, 11].Style.Alignment.HorizontalAlignment = ExcelCellHorizontalAlignmentType.Right;
                        }
                        ews1[intMainRfqRowCount, 1, intMainRfqRowCount + intMergeRowCount, 1].Merge();
                        ews1[intMainRfqRowCount, 1].Text = drRFQ.RfqDate.ToString();
                        ews1[intMainRfqRowCount, 1].Style.Alignment.VerticalAlignment = ExcelCellVerticalAlignmentType.Top;

                        ews1[intMainRfqRowCount, 2, intMainRfqRowCount + intMergeRowCount, 2].Merge();
                        ews1[intMainRfqRowCount, 2].Style.Alignment.WrapText = true;
                        ews1[intMainRfqRowCount, 2].ColumnWidthInChars = 22;
                        ews1[intMainRfqRowCount, 2].Text = "Total RFQ(s): " + drRFQ.NoOfRfq.ToString() + Environment.NewLine + "New RFQ(s): " + drRFQ.NewRFQ.ToString() + Environment.NewLine + "New Customer(s): " + drRFQ.NewCustomer.ToString();
                        ews1[intMainRfqRowCount, 2].AutofitRows();
                        ews1[intMainRfqRowCount, 2].Style.Alignment.VerticalAlignment = ExcelCellVerticalAlignmentType.Top;

                        intRFQRowCounter++;

                        ews1[intRFQRowCounter, 3, intRFQRowCounter, 10].Merge();
                        ews1[intRFQRowCounter, 3].Text = "Monthly Total: " + string.Format("${0:#,##0.000}", dTotalQuoted);
                        ews1[intRFQRowCounter, 3].Style.Alignment.HorizontalAlignment = ExcelCellHorizontalAlignmentType.Right;
                        ews1[intRFQRowCounter, 3].Style.Alignment.VerticalAlignment = ExcelCellVerticalAlignmentType.Top;
                        ews1[intRFQRowCounter, 3].Style.Font.Bold = true;

                        ews1[intRFQRowCounter, 11].Text = "Monthly Total: " + string.Format("${0:#,##0.000}", dToolingCost);
                        ews1[intRFQRowCounter, 11].Style.Alignment.HorizontalAlignment = ExcelCellHorizontalAlignmentType.Right;
                        ews1[intRFQRowCounter, 11].Style.Alignment.VerticalAlignment = ExcelCellVerticalAlignmentType.Top;
                        ews1[intRFQRowCounter, 11].Style.Font.Bold = true;

                        ews1[intRFQRowCounter, 1].AutofitColumns();
                        ews1[intRFQRowCounter, 3].AutofitColumns();
                        ews1[intRFQRowCounter, 10].AutofitColumns();
                        ews1[intRFQRowCounter, 11].AutofitColumns();
                    }

                    lstRFQAnalysisReport = null;
                    #endregion

                    #region  Excel Protected
                    ews1.WorksheetSecurity.ProtectWorksheet(System.Configuration.ConfigurationManager.AppSettings["ShipmentTemplatePswd"]);
                    ews2.WorksheetSecurity.ProtectWorksheet(System.Configuration.ConfigurationManager.AppSettings["ShipmentTemplatePswd"]);
                    #endregion

                    #region "Download"
                    string generatedFileName = string.Empty;

                    if (paging.Criteria.ReportId == 9)
                    {
                        generatedFileName = Languages.GetResourceText("RFQAnalysisReportFileName") + ".xls";// "RFQAnalysisReport.xls";
                        ewb.Worksheets[0].Name = Languages.GetResourceText("RFQAnalysisReportSheetName"); //"RFQ Analysis Report";
                    }

                    else if (paging.Criteria.ReportId == 12)
                    {
                        generatedFileName = Languages.GetResourceText("RFQBusinessAnalysisReportFileName") + ".xls";// "RFQBusinessAnalysisReport.xls";
                        ewb.Worksheets[0].Name = Languages.GetResourceText("RFQBusinessAnalysisReportSheetName");// "RFQ Business Analysis Report";
                    }

                    filePath = httpcontext.Server.MapPath(@"~") + Constants.REPORTSTEMPLATEFILEPATH + @"\" + generatedFileName;

                    if (!System.IO.Directory.Exists(httpcontext.Server.MapPath(@"~") + (Constants.REPORTSTEMPLATEFILEPATH)))
                        System.IO.Directory.CreateDirectory(httpcontext.Server.MapPath(@"~") + (Constants.REPORTSTEMPLATEFILEPATH));
                    else
                    {
                        if (File.Exists(filePath))
                            File.Delete(filePath);
                    }
                    try
                    {
                        ewb.Save(filePath);
                        filePath = System.Configuration.ConfigurationManager.AppSettings["ApiURL"] + Constants.REPORTSTEMPLATEFILEPATH + generatedFileName;
                    }
                    catch (Exception ex) //Error
                    {
                        return FailedBoolResponse(ex.Message);
                    }
                    finally
                    {
                        sourceXlsDataStream.Close();
                        ewb.Close();
                    }

                    #endregion
                }
                catch (Exception ex) //Error
                {
                    return FailedBoolResponse(ex.Message);
                }
            }
            else
            {
                if (paging.Criteria.ReportId == 9)
                    return FailedBoolResponse(Languages.GetResourceText("RFQAnalysisReportTemplateNotExists"));
                else if (paging.Criteria.ReportId == 12)
                    return FailedBoolResponse(Languages.GetResourceText("BusinessAnalysisReportTemplateNotExists"));
            }

            return SuccessBoolResponse(filePath);
        }

        public NPE.Core.ITypedResponse<bool?> exportBusinessAnalysisQTCReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.RFQAnalysisReportSearch> paging)
        {
            string filePath = string.Empty;
            var httpcontext = HttpContext.Current;
            string strBody = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(paging.Criteria.GroupBy))
                {
                    filePath = httpcontext.Server.MapPath("~/" + Constants.REPORTSTEMPLATEFOLDER) + "BusinessAnalysisQTCTemplate-Standard.xls";

                    if (File.Exists(filePath))
                    {
                        filePath = exportForBAQTCStandardReport(paging);
                    }
                    else
                    {
                        return FailedBoolResponse(Languages.GetResourceText("BusinessAnalysisQTCReportTemplateNotExists"));
                    }

                }
                else
                {
                    filePath = httpcontext.Server.MapPath("~/" + Constants.REPORTSTEMPLATEFOLDER) + "BusinessAnalysisQTCTemplate.xls";

                    if (File.Exists(filePath))
                    {
                        filePath = exportForBAQTCReport(paging);
                    }
                    else
                    {
                        return FailedBoolResponse(Languages.GetResourceText("BusinessAnalysisQTCReportTemplateNotExists"));
                    }

                }
            }
            catch (Exception ex)
            {
                return FailedBoolResponse(ex.Message);
            }
            return SuccessBoolResponse(filePath);
        }

        public string exportForBAQTCStandardReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.RFQAnalysisReportSearch> paging)
        {
            string filePath = string.Empty;
            var httpcontext = HttpContext.Current;
            string strBody = string.Empty;

            filePath = httpcontext.Server.MapPath("~/" + Constants.REPORTSTEMPLATEFOLDER) + "BusinessAnalysisQTCTemplate-Standard.xls";

            try
            {
                System.IO.FileStream sourceXlsDataStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                ExcelWorkbook ewb = new ExcelWorkbook(sourceXlsDataStream);
                ewb.LicenseKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["wnvkey"]);
                Winnovative.ExcelLib.ExcelWorksheet ews1 = ewb.Worksheets[0];
                Winnovative.ExcelLib.ExcelWorksheet ews2 = ewb.Worksheets[1];

                #region RFQ Analysis Report Chart Create

                if (ews2.UsedRangeCells.Count > 0)
                    ews2.UsedRange.Clear();

                ews2[1, 1].Value = "SAM"; //1. SAM
                ews2[1, 2].Value = "No. Of RFQ"; //2. No. Of RFQ

                int counter = 1, count = 0;
                string GroupByType = string.Empty;
                short groupBy = 1;
                if (groupBy == 1)
                {
                    GroupByType = "Commodity";
                    ews2[1, 1].Value = "Commodity"; //1. Commodity
                }
                else if (groupBy == 2)
                {
                    GroupByType = "SAM";
                    ews2[1, 1].Value = "SAM"; //2. SAM                        
                }
                else if (groupBy == 3)
                {
                    GroupByType = "RFQSource";
                    ews2[1, 1].Value = "RFQ Source"; //3. RFQ Source                        
                }
                else
                    ews2[1, 1].Value = "Month";

                ews2[1, 2].Value = "No. Of RFQ"; //2. No. Of RFQ
                List<DTO.Library.RFQ.RFQReports.RFQAnalysisReport> lstRFQAnalysisReport = new List<DTO.Library.RFQ.RFQReports.RFQAnalysisReport>();

                lstRFQAnalysisReport = GetRFQAnalysisReport(paging).Result;

                List<DTO.Library.RFQ.RFQReports.RFQAnalysisChart> lstRFQAnalysisChart = new List<DTO.Library.RFQ.RFQReports.RFQAnalysisChart>();

                lstRFQAnalysisChart = lstRFQAnalysisReport[0].lstRFQAnalysisChart;
                if (lstRFQAnalysisChart.Count > 0)
                {
                    ExcelRange er;
                    count = lstRFQAnalysisChart.Count + 1;
                    foreach (var item in lstRFQAnalysisChart)
                    {
                        counter++;
                        ews2[counter, 1].Value = item.DisplayName.ToString() + " (" + Convert.ToDouble(item.NoOfRfq.ToString()) + "%)";
                        ews2[counter, 2].NumberValue = Convert.ToDouble(item.NoOfRfq.ToString());
                    }

                    er = ews2[1, 1, count, 2];
                    ews1.Charts[0].DataSourceRange = er;
                }

                #endregion

                #region  RFQ Analysis Report List Create
                int intRFQRowCounter = 1;

                foreach (DTO.Library.RFQ.RFQReports.RFQAnalysisReport drRFQ in lstRFQAnalysisReport)
                {
                    decimal dTotalQuoted = 0;
                    decimal dToolingCost = 0;
                    int intMainRfqRowCount = intRFQRowCounter + 1;
                    int intMergeRowCount = 0;

                    foreach (MES.DTO.Library.RFQ.RFQReports.RFQAnalysisReportDetails drRFQItem in drRFQ.lstRFQAnalysisReportDetails)
                    {
                        intRFQRowCounter++;
                        intMergeRowCount++;
                        ews1[intRFQRowCounter, 1].Text = drRFQItem.RFQ;
                        ews1[intRFQRowCounter, 2].Text = drRFQItem.QuoteDate;
                        ews1[intRFQRowCounter, 3].Text = drRFQItem.Customer;
                        ews1[intRFQRowCounter, 4].Text = !string.IsNullOrEmpty(drRFQItem.AccountManager) ? drRFQItem.AccountManager : string.Empty;
                        ews1[intRFQRowCounter, 5].Text = !string.IsNullOrEmpty(drRFQItem.Commodity) ? drRFQItem.Commodity : string.Empty;
                        ews1[intRFQRowCounter, 6].Text = !string.IsNullOrEmpty(drRFQItem.Process) ? drRFQItem.Process : string.Empty;
                        ews1[intRFQRowCounter, 7].Text = !string.IsNullOrEmpty(drRFQItem.RFQSource) ? drRFQItem.RFQSource : string.Empty;
                        ews1[intRFQRowCounter, 8].Text = !string.IsNullOrEmpty(drRFQItem.RFQType) ? drRFQItem.RFQType : string.Empty;
                        ews1[intRFQRowCounter, 9].Text = !string.IsNullOrEmpty(drRFQItem.IndustryType) ? drRFQItem.RFQType : string.Empty;
                        if (!string.IsNullOrEmpty(drRFQItem.totalquoted.ToString()))
                        {
                            ews1[intRFQRowCounter, 10].Text = string.Format("${0:#,##0.000}", Convert.ToDecimal(drRFQItem.totalquoted.ToString()));
                            dTotalQuoted = dTotalQuoted + Convert.ToDecimal(drRFQItem.totalquoted.ToString());
                        }
                        else
                        {
                            ews1[intRFQRowCounter, 10].Text = string.Format("${0:#,##0.000}", 0);
                        }
                        ews1[intRFQRowCounter, 10].Style.Alignment.HorizontalAlignment = ExcelCellHorizontalAlignmentType.Right;
                        if (!string.IsNullOrEmpty(drRFQItem.ToolingCost.ToString()))
                        {
                            ews1[intRFQRowCounter, 11].Text = string.Format("${0:#,##0.000}", Convert.ToDecimal(drRFQItem.ToolingCost.ToString()));
                            dToolingCost = dToolingCost + Convert.ToDecimal(drRFQItem.ToolingCost.ToString());
                        }
                        else
                        {
                            ews1[intRFQRowCounter, 11].Text = string.Format("${0:#,##0.000}", 0);
                        }
                        ews1[intRFQRowCounter, 11].Style.Alignment.HorizontalAlignment = ExcelCellHorizontalAlignmentType.Right;
                    }

                    if (!string.IsNullOrEmpty(paging.Criteria.GroupBy))
                    {
                        ews1[intMainRfqRowCount, 1, intMainRfqRowCount + intMergeRowCount, 1].Merge();
                        ews1[intMainRfqRowCount, 1].Text = drRFQ.RfqDate.ToString();
                        ews1[intMainRfqRowCount, 1].Style.Alignment.VerticalAlignment = ExcelCellVerticalAlignmentType.Top;
                    }



                    if (!string.IsNullOrEmpty(paging.Criteria.GroupBy))
                    {
                        intRFQRowCounter++;
                        ews1[intRFQRowCounter, 2, intRFQRowCounter, 11].Merge();
                        ews1[intRFQRowCounter, 2].Text = "Monthly Total: " + string.Format("${0:#,##0.000}", dTotalQuoted);
                        ews1[intRFQRowCounter, 2].Style.Alignment.HorizontalAlignment = ExcelCellHorizontalAlignmentType.Right;
                        ews1[intRFQRowCounter, 2].Style.Alignment.VerticalAlignment = ExcelCellVerticalAlignmentType.Top;
                        ews1[intRFQRowCounter, 2].Style.Font.Bold = true;

                        ews1[intRFQRowCounter, 12].Text = "Monthly Total: " + string.Format("${0:#,##0.000}", dToolingCost);
                        ews1[intRFQRowCounter, 12].Style.Alignment.HorizontalAlignment = ExcelCellHorizontalAlignmentType.Right;
                        ews1[intRFQRowCounter, 12].Style.Alignment.VerticalAlignment = ExcelCellVerticalAlignmentType.Top;
                        ews1[intRFQRowCounter, 12].Style.Font.Bold = true;
                    }
                    ews1[intRFQRowCounter, 1].AutofitColumns();
                    ews1[intRFQRowCounter, 2].AutofitColumns();
                    ews1[intRFQRowCounter, 10].AutofitColumns();
                    ews1[intRFQRowCounter, 11].AutofitColumns();

                }

                lstRFQAnalysisReport = null;
                #endregion

                #region  Excel Protected
                ews1.WorksheetSecurity.ProtectWorksheet(System.Configuration.ConfigurationManager.AppSettings["ShipmentTemplatePswd"]);
                ews2.WorksheetSecurity.ProtectWorksheet(System.Configuration.ConfigurationManager.AppSettings["ShipmentTemplatePswd"]);
                #endregion

                #region "Download"
                string generatedFileName = string.Empty;

                generatedFileName = Languages.GetResourceText("BusinessAnalysisReportQTCFileName") + ".xls";// "RFQBusinessAnalysisReport.xls";
                ewb.Worksheets[0].Name = Languages.GetResourceText("BusinessAnalysisReportQTCSheetName");// "Business Analysis Report QTC";

                filePath = httpcontext.Server.MapPath(@"~") + Constants.REPORTSTEMPLATEFILEPATH + @"\" + generatedFileName;

                if (!System.IO.Directory.Exists(httpcontext.Server.MapPath(@"~") + (Constants.REPORTSTEMPLATEFILEPATH)))
                    System.IO.Directory.CreateDirectory(httpcontext.Server.MapPath(@"~") + (Constants.REPORTSTEMPLATEFILEPATH));
                else
                {
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
                try
                {
                    ewb.Save(filePath);
                    filePath = System.Configuration.ConfigurationManager.AppSettings["ApiURL"] + Constants.REPORTSTEMPLATEFILEPATH + generatedFileName;
                }
                catch (Exception ex) //Error
                {
                    throw ex;
                }
                finally
                {
                    sourceXlsDataStream.Close();
                    ewb.Close();
                }

                #endregion
            }
            catch (Exception ex) //Error
            {
                throw ex;
            }

            return filePath;
        }
        public string exportForBAQTCReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.RFQAnalysisReportSearch> paging)
        {
            string filePath = string.Empty;
            var httpcontext = HttpContext.Current;
            string strBody = string.Empty;

            filePath = httpcontext.Server.MapPath("~/" + Constants.REPORTSTEMPLATEFOLDER) + "BusinessAnalysisQTCTemplate.xls";


            try
            {
                System.IO.FileStream sourceXlsDataStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                ExcelWorkbook ewb = new ExcelWorkbook(sourceXlsDataStream);
                ewb.LicenseKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["wnvkey"]);
                Winnovative.ExcelLib.ExcelWorksheet ews1 = ewb.Worksheets[0];
                Winnovative.ExcelLib.ExcelWorksheet ews2 = ewb.Worksheets[1];

                #region RFQ Analysis Report Chart Create

                if (ews2.UsedRangeCells.Count > 0)
                    ews2.UsedRange.Clear();

                ews2[1, 1].Value = "SAM"; //1. SAM
                ews2[1, 2].Value = "No. Of RFQ"; //2. No. Of RFQ

                int counter = 1, count = 0;
                string GroupByType = string.Empty;
                short groupBy = 1;
                if (groupBy == 1)
                {
                    GroupByType = "Commodity";
                    ews2[1, 1].Value = "Commodity"; //1. Commodity
                }
                else if (groupBy == 2)
                {
                    GroupByType = "SAM";
                    ews2[1, 1].Value = "SAM"; //2. SAM                        
                }
                else if (groupBy == 3)
                {
                    GroupByType = "RFQSource";
                    ews2[1, 1].Value = "RFQ Source"; //3. RFQ Source                        
                }
                else
                    ews2[1, 1].Value = "Month";

                ews2[1, 2].Value = "No. Of RFQ"; //2. No. Of RFQ
                List<DTO.Library.RFQ.RFQReports.RFQAnalysisReport> lstRFQAnalysisReport = new List<DTO.Library.RFQ.RFQReports.RFQAnalysisReport>();

                lstRFQAnalysisReport = GetRFQAnalysisReport(paging).Result;

                List<DTO.Library.RFQ.RFQReports.RFQAnalysisChart> lstRFQAnalysisChart = new List<DTO.Library.RFQ.RFQReports.RFQAnalysisChart>();

                lstRFQAnalysisChart = lstRFQAnalysisReport[0].lstRFQAnalysisChart;
                if (lstRFQAnalysisChart.Count > 0)
                {
                    ExcelRange er;
                    count = lstRFQAnalysisChart.Count + 1;
                    foreach (var item in lstRFQAnalysisChart)
                    {
                        counter++;
                        ews2[counter, 1].Value = item.DisplayName.ToString() + " (" + Convert.ToDouble(item.NoOfRfq.ToString()) + "%)";
                        ews2[counter, 2].NumberValue = Convert.ToDouble(item.NoOfRfq.ToString());
                    }

                    er = ews2[1, 1, count, 2];
                    ews1.Charts[0].DataSourceRange = er;
                }

                #endregion

                #region  RFQ Analysis Report List Create
                int intRFQRowCounter = 1;

                foreach (DTO.Library.RFQ.RFQReports.RFQAnalysisReport drRFQ in lstRFQAnalysisReport)
                {
                    decimal dTotalQuoted = 0;
                    decimal dToolingCost = 0;
                    int intMainRfqRowCount = intRFQRowCounter + 1;
                    int intMergeRowCount = 0;

                    foreach (MES.DTO.Library.RFQ.RFQReports.RFQAnalysisReportDetails drRFQItem in drRFQ.lstRFQAnalysisReportDetails)
                    {
                        intRFQRowCounter++;
                        intMergeRowCount++;
                        ews1[intRFQRowCounter, 2].Text = drRFQItem.RFQ;
                        ews1[intRFQRowCounter, 3].Text = drRFQItem.QuoteDate;
                        ews1[intRFQRowCounter, 4].Text = drRFQItem.Customer;
                        ews1[intRFQRowCounter, 5].Text = !string.IsNullOrEmpty(drRFQItem.AccountManager) ? drRFQItem.AccountManager : string.Empty;
                        ews1[intRFQRowCounter, 6].Text = !string.IsNullOrEmpty(drRFQItem.Commodity) ? drRFQItem.Commodity : string.Empty;
                        ews1[intRFQRowCounter, 7].Text = !string.IsNullOrEmpty(drRFQItem.Process) ? drRFQItem.Process : string.Empty;
                        ews1[intRFQRowCounter, 8].Text = !string.IsNullOrEmpty(drRFQItem.RFQSource) ? drRFQItem.RFQSource : string.Empty;
                        ews1[intRFQRowCounter, 9].Text = !string.IsNullOrEmpty(drRFQItem.RFQType) ? drRFQItem.RFQType : string.Empty;
                        ews1[intRFQRowCounter, 10].Text = !string.IsNullOrEmpty(drRFQItem.IndustryType) ? drRFQItem.IndustryType : string.Empty;
                        if (!string.IsNullOrEmpty(drRFQItem.totalquoted.ToString()))
                        {
                            ews1[intRFQRowCounter, 11].Text = string.Format("${0:#,##0.000}", Convert.ToDecimal(drRFQItem.totalquoted.ToString()));
                            dTotalQuoted = dTotalQuoted + Convert.ToDecimal(drRFQItem.totalquoted.ToString());
                        }
                        else
                        {
                            ews1[intRFQRowCounter, 11].Text = string.Format("${0:#,##0.000}", 0);
                        }
                        ews1[intRFQRowCounter, 11].Style.Alignment.HorizontalAlignment = ExcelCellHorizontalAlignmentType.Right;
                        if (!string.IsNullOrEmpty(drRFQItem.ToolingCost.ToString()))
                        {
                            ews1[intRFQRowCounter, 12].Text = string.Format("${0:#,##0.000}", Convert.ToDecimal(drRFQItem.ToolingCost.ToString()));
                            dToolingCost = dToolingCost + Convert.ToDecimal(drRFQItem.ToolingCost.ToString());
                        }
                        else
                        {
                            ews1[intRFQRowCounter, 12].Text = string.Format("${0:#,##0.000}", 0);
                        }
                        ews1[intRFQRowCounter, 12].Style.Alignment.HorizontalAlignment = ExcelCellHorizontalAlignmentType.Right;
                    }

                    if (!string.IsNullOrEmpty(paging.Criteria.GroupBy))
                    {
                        ews1[intMainRfqRowCount, 1, intMainRfqRowCount + intMergeRowCount, 1].Merge();
                        ews1[intMainRfqRowCount, 1].Text = drRFQ.RfqDate.ToString();
                        ews1[intMainRfqRowCount, 1].Style.Alignment.VerticalAlignment = ExcelCellVerticalAlignmentType.Top;
                    }



                    if (!string.IsNullOrEmpty(paging.Criteria.GroupBy))
                    {
                        intRFQRowCounter++;
                        ews1[intRFQRowCounter, 2, intRFQRowCounter, 11].Merge();
                        ews1[intRFQRowCounter, 2].Text = "Monthly Total: " + string.Format("${0:#,##0.000}", dTotalQuoted);
                        ews1[intRFQRowCounter, 2].Style.Alignment.HorizontalAlignment = ExcelCellHorizontalAlignmentType.Right;
                        ews1[intRFQRowCounter, 2].Style.Alignment.VerticalAlignment = ExcelCellVerticalAlignmentType.Top;
                        ews1[intRFQRowCounter, 2].Style.Font.Bold = true;

                        ews1[intRFQRowCounter, 12].Text = "Monthly Total: " + string.Format("${0:#,##0.000}", dToolingCost);
                        ews1[intRFQRowCounter, 12].Style.Alignment.HorizontalAlignment = ExcelCellHorizontalAlignmentType.Right;
                        ews1[intRFQRowCounter, 12].Style.Alignment.VerticalAlignment = ExcelCellVerticalAlignmentType.Top;
                        ews1[intRFQRowCounter, 12].Style.Font.Bold = true;
                    }
                    ews1[intRFQRowCounter, 1].AutofitColumns();
                    ews1[intRFQRowCounter, 2].AutofitColumns();
                    ews1[intRFQRowCounter, 11].AutofitColumns();
                    ews1[intRFQRowCounter, 12].AutofitColumns();

                }

                lstRFQAnalysisReport = null;
                #endregion

                #region  Excel Protected
                ews1.WorksheetSecurity.ProtectWorksheet(System.Configuration.ConfigurationManager.AppSettings["ShipmentTemplatePswd"]);
                ews2.WorksheetSecurity.ProtectWorksheet(System.Configuration.ConfigurationManager.AppSettings["ShipmentTemplatePswd"]);
                #endregion

                #region "Download"
                string generatedFileName = string.Empty;

                generatedFileName = Languages.GetResourceText("BusinessAnalysisReportQTCFileName") + ".xls";// "RFQBusinessAnalysisReport.xls";
                ewb.Worksheets[0].Name = Languages.GetResourceText("BusinessAnalysisReportQTCSheetName");// "Business Analysis Report QTC";

                filePath = httpcontext.Server.MapPath(@"~") + Constants.REPORTSTEMPLATEFILEPATH + @"\" + generatedFileName;

                if (!System.IO.Directory.Exists(httpcontext.Server.MapPath(@"~") + (Constants.REPORTSTEMPLATEFILEPATH)))
                    System.IO.Directory.CreateDirectory(httpcontext.Server.MapPath(@"~") + (Constants.REPORTSTEMPLATEFILEPATH));
                else
                {
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
                try
                {
                    ewb.Save(filePath);
                    filePath = System.Configuration.ConfigurationManager.AppSettings["ApiURL"] + Constants.REPORTSTEMPLATEFILEPATH + generatedFileName;
                }
                catch (Exception ex) //Error
                {
                    throw ex;
                }
                finally
                {
                    sourceXlsDataStream.Close();
                    ewb.Close();
                }

                #endregion
            }
            catch (Exception ex) //Error
            {
                throw ex;
            }
            return filePath;
        }

        #endregion

        #region  RFQ "non award reason report" with chart
        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQReports.RFQNonAwardReasonReport>> GetRFQNonAwardReasonReport(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.RFQNonAwardReasonReportSearch> searchInfo)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.RFQ.RFQReports.RFQNonAwardReasonReport> lstRFQNonAwardReasonReport = new List<DTO.Library.RFQ.RFQReports.RFQNonAwardReasonReport>();
            DTO.Library.RFQ.RFQReports.RFQNonAwardReasonReport rFQNonAwardReasonReport;
            DTO.Library.RFQ.RFQReports.RFQNonAwardReasonReportDetails rFQNonAwardReasonReportDetails;
            List<DTO.Library.RFQ.RFQReports.RFQNonAwardReasonChart> lstRFQNonAwardReasonChart = new List<DTO.Library.RFQ.RFQReports.RFQNonAwardReasonChart>();
            DTO.Library.RFQ.RFQReports.RFQNonAwardReasonChart rFQNonAwardReasonChart;
            this.RunOnDB(context =>
            {
                var firstPartData = context.RFQNonAwardReasonReportFirstPart(searchInfo.Criteria.SAMIds, searchInfo.Criteria.CustomerIds, searchInfo.Criteria.DateFrom, searchInfo.Criteria.DateTo, searchInfo.Criteria.CommodityIds, searchInfo.Criteria.RFQSourceIds).ToList();
                var secondPartData = context.RFQNonAwardReasonReportSecondPart(searchInfo.Criteria.SAMIds, searchInfo.Criteria.CustomerIds, searchInfo.Criteria.DateFrom, searchInfo.Criteria.DateTo, searchInfo.Criteria.CommodityIds, searchInfo.Criteria.RFQSourceIds).ToList();
                var chartData = context.RFQNonAwardReasonChart(searchInfo.Criteria.SAMIds, searchInfo.Criteria.CustomerIds, searchInfo.Criteria.DateFrom, searchInfo.Criteria.DateTo, searchInfo.Criteria.CommodityIds, searchInfo.Criteria.RFQSourceIds, searchInfo.Criteria.GroupBy).ToList();
                if (firstPartData == null || secondPartData == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records

                    foreach (var chart in chartData)
                    {
                        rFQNonAwardReasonChart = new DTO.Library.RFQ.RFQReports.RFQNonAwardReasonChart();
                        rFQNonAwardReasonChart.DisplayName = chart.DisplayName;
                        rFQNonAwardReasonChart.NoOfRfq = chart.NoOfRfq;
                        lstRFQNonAwardReasonChart.Add(rFQNonAwardReasonChart);
                    }

                    foreach (var firstPart in firstPartData)
                    {
                        rFQNonAwardReasonReport = new DTO.Library.RFQ.RFQReports.RFQNonAwardReasonReport();
                        rFQNonAwardReasonReport.RfqDate = firstPart.RfqDate;
                        rFQNonAwardReasonReport.NoOfRfq = firstPart.NoOfRfq;
                        rFQNonAwardReasonReport.SortOrder = firstPart.SortOrder;
                        rFQNonAwardReasonReport.lstRFQNonAwardReasonReportDetails = new List<DTO.Library.RFQ.RFQReports.RFQNonAwardReasonReportDetails>();
                        foreach (var secondPart in secondPartData.Where(a => a.RfqDate == firstPart.RfqDate).ToList())
                        {
                            rFQNonAwardReasonReportDetails = new DTO.Library.RFQ.RFQReports.RFQNonAwardReasonReportDetails();
                            rFQNonAwardReasonReportDetails.NonAwardFeedback = secondPart.NonAwardFeedback;
                            rFQNonAwardReasonReportDetails.NonAwardDetailedFeedback = secondPart.NonAwardDetailedFeedback;
                            rFQNonAwardReasonReportDetails.RFQ = secondPart.RFQ;
                            rFQNonAwardReasonReportDetails.RfqDate = secondPart.RfqDate;
                            rFQNonAwardReasonReportDetails.Customer = secondPart.Customer;
                            rFQNonAwardReasonReportDetails.AccountManager = secondPart.AccountManager;
                            rFQNonAwardReasonReportDetails.totalquoted = secondPart.totalquoted ?? 0;
                            rFQNonAwardReasonReportDetails.ToolingCost = secondPart.ToolingCost ?? 0;
                            rFQNonAwardReasonReportDetails.Commodity = secondPart.Commodity;
                            rFQNonAwardReasonReportDetails.Process = secondPart.Process;
                            rFQNonAwardReasonReportDetails.sortorder = secondPart.sortorder;
                            rFQNonAwardReasonReportDetails.RFQSource = secondPart.RFQSource;
                            rFQNonAwardReasonReportDetails.RFQType = secondPart.RFQType;
                            //extra properties for grouping
                            if (!string.IsNullOrEmpty(searchInfo.Criteria.GroupBy))
                            {
                                if (searchInfo.Criteria.GroupBy == "SAM")
                                    rFQNonAwardReasonReportDetails.GroupByValue = string.IsNullOrEmpty(secondPart.AccountManager) == true ? "Account Manager Not Assigned" : secondPart.AccountManager;
                                else if (searchInfo.Criteria.GroupBy == "NonAwardFeedback")
                                    rFQNonAwardReasonReportDetails.GroupByValue = string.IsNullOrEmpty(secondPart.NonAwardFeedback) == true ? "Non-Award Reason Not Assigned" : secondPart.NonAwardFeedback;
                            }
                            else
                                rFQNonAwardReasonReportDetails.GroupByValue = "";
                            rFQNonAwardReasonReport.lstRFQNonAwardReasonReportDetails.Add(rFQNonAwardReasonReportDetails);
                        }
                        rFQNonAwardReasonReport.RFQDateTotalQuoted = rFQNonAwardReasonReport.lstRFQNonAwardReasonReportDetails.Sum(a => a.totalquoted);
                        rFQNonAwardReasonReport.RFQDateTotalToolingCost = rFQNonAwardReasonReport.lstRFQNonAwardReasonReportDetails.Sum(a => a.ToolingCost);
                        rFQNonAwardReasonReport.lstRFQNonAwardReasonChart = lstRFQNonAwardReasonChart;
                        lstRFQNonAwardReasonReport.Add(rFQNonAwardReasonReport);
                    }
                }
            });
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQNonAwardReasonReport>>(errMSg, lstRFQNonAwardReasonReport);
            response.PageInfo = searchInfo.ToPage();
            return response;
        }
        public NPE.Core.ITypedResponse<bool?> exportRFQNonAwardReasonReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.RFQNonAwardReasonReportSearch> paging)
        {
            string filePath = string.Empty;
            var httpcontext = HttpContext.Current;
            filePath = httpcontext.Server.MapPath("~/" + Constants.REPORTSTEMPLATEFOLDER) + "NonAwardReasonTemplate.xls";

            if (File.Exists(filePath))
            {
                try
                {
                    System.IO.FileStream sourceXlsDataStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    ExcelWorkbook ewb = new ExcelWorkbook(sourceXlsDataStream);
                    ewb.LicenseKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["wnvkey"]);
                    Winnovative.ExcelLib.ExcelWorksheet ews1 = ewb.Worksheets[0];
                    Winnovative.ExcelLib.ExcelWorksheet ews2 = ewb.Worksheets[1];

                    #region RFQ Non Award Reason Report Chart Create

                    if (ews2.UsedRangeCells.Count > 0)
                        ews2.UsedRange.Clear();

                    ews2[1, 1].Value = "SAM"; //1. SAM
                    ews2[1, 2].Value = "No. Of RFQ"; //2. No. Of RFQ

                    int counter = 1, count = 0;
                    string GroupByType = "";
                    short groupBy = 1;
                    if (groupBy == 1)
                    {
                        GroupByType = "NonAwardFeedback";
                        ews2[1, 1].Value = "Non-Award Reason"; //1. NonAwardFeedback
                    }
                    else if (groupBy == 2)
                    {
                        GroupByType = "SAM";
                        ews2[1, 1].Value = "SAM"; //2. SAM                        
                    }

                    ews2[1, 2].Value = "No. Of RFQ"; //2. No. Of RFQ
                    List<DTO.Library.RFQ.RFQReports.RFQNonAwardReasonReport> lstRFQNonAwardReasonReport = new List<DTO.Library.RFQ.RFQReports.RFQNonAwardReasonReport>();

                    lstRFQNonAwardReasonReport = GetRFQNonAwardReasonReport(paging).Result;

                    List<DTO.Library.RFQ.RFQReports.RFQNonAwardReasonChart> lstRFQNonAwardReasonChart = new List<DTO.Library.RFQ.RFQReports.RFQNonAwardReasonChart>();

                    lstRFQNonAwardReasonChart = lstRFQNonAwardReasonReport[0].lstRFQNonAwardReasonChart;
                    if (lstRFQNonAwardReasonChart.Count > 0)
                    {
                        ExcelRange er;
                        count = lstRFQNonAwardReasonChart.Count + 1;
                        foreach (var item in lstRFQNonAwardReasonChart)
                        {
                            counter++;
                            ews2[counter, 1].Value = item.DisplayName.ToString() + " (" + Convert.ToDouble(item.NoOfRfq.ToString()) + "%)";
                            ews2[counter, 2].NumberValue = Convert.ToDouble(item.NoOfRfq.ToString());
                        }

                        er = ews2[1, 1, count, 2];
                        ews1.Charts[0].DataSourceRange = er;
                    }

                    #endregion

                    #region  RFQ Non Award Reason Report List Create
                    int intRFQRowCounter = 1;

                    foreach (DTO.Library.RFQ.RFQReports.RFQNonAwardReasonReport drRFQ in lstRFQNonAwardReasonReport)
                    {
                        decimal dTotalQuoted = 0;
                        decimal dToolingCost = 0;
                        int intMainRfqRowCount = intRFQRowCounter + 1;
                        int intMergeRowCount = 0;

                        foreach (MES.DTO.Library.RFQ.RFQReports.RFQNonAwardReasonReportDetails drRFQItem in drRFQ.lstRFQNonAwardReasonReportDetails)
                        {
                            intRFQRowCounter++;
                            intMergeRowCount++;
                            ews1[intRFQRowCounter, 3].Text = drRFQItem.RFQ;
                            ews1[intRFQRowCounter, 4].Text = drRFQItem.Customer;
                            ews1[intRFQRowCounter, 5].Text = drRFQItem.AccountManager;
                            ews1[intRFQRowCounter, 6].Text = !string.IsNullOrEmpty(drRFQItem.NonAwardFeedback) ? drRFQItem.NonAwardFeedback : string.Empty;
                            ews1[intRFQRowCounter, 7].Text = !string.IsNullOrEmpty(drRFQItem.NonAwardDetailedFeedback) ? drRFQItem.NonAwardDetailedFeedback : string.Empty;
                            ews1[intRFQRowCounter, 8].Text = !string.IsNullOrEmpty(drRFQItem.Commodity) ? drRFQItem.Commodity : string.Empty;
                            ews1[intRFQRowCounter, 9].Text = !string.IsNullOrEmpty(drRFQItem.Process) ? drRFQItem.Process : string.Empty;
                            ews1[intRFQRowCounter, 10].Text = !string.IsNullOrEmpty(drRFQItem.RFQSource) ? drRFQItem.RFQSource : string.Empty;
                            ews1[intRFQRowCounter, 11].Text = !string.IsNullOrEmpty(drRFQItem.RFQType) ? drRFQItem.RFQType : string.Empty;
                            if (!string.IsNullOrEmpty(drRFQItem.totalquoted.ToString()))
                            {
                                ews1[intRFQRowCounter, 12].Text = string.Format("${0:#,##0.000}", Convert.ToDecimal(drRFQItem.totalquoted.ToString()));
                                dTotalQuoted = dTotalQuoted + Convert.ToDecimal(drRFQItem.totalquoted.ToString());
                            }
                            else
                            {
                                ews1[intRFQRowCounter, 12].Text = string.Format("${0:#,##0.000}", 0);
                            }
                            ews1[intRFQRowCounter, 12].Style.Alignment.HorizontalAlignment = ExcelCellHorizontalAlignmentType.Right;
                            if (!string.IsNullOrEmpty(drRFQItem.ToolingCost.ToString()))
                            {
                                ews1[intRFQRowCounter, 13].Text = string.Format("${0:#,##0.000}", Convert.ToDecimal(drRFQItem.ToolingCost.ToString()));
                                dToolingCost = dToolingCost + Convert.ToDecimal(drRFQItem.ToolingCost.ToString());
                            }
                            else
                            {
                                ews1[intRFQRowCounter, 13].Text = string.Format("${0:#,##0.000}", 0);
                            }
                            ews1[intRFQRowCounter, 13].Style.Alignment.HorizontalAlignment = ExcelCellHorizontalAlignmentType.Right;
                        }
                        ews1[intMainRfqRowCount, 1, intMainRfqRowCount + intMergeRowCount, 1].Merge();
                        ews1[intMainRfqRowCount, 1].Text = drRFQ.RfqDate.ToString();
                        ews1[intMainRfqRowCount, 1].Style.Alignment.VerticalAlignment = ExcelCellVerticalAlignmentType.Top;

                        ews1[intMainRfqRowCount, 2, intMainRfqRowCount + intMergeRowCount, 2].Merge();
                        ews1[intMainRfqRowCount, 2].Style.Alignment.WrapText = true;
                        ews1[intMainRfqRowCount, 2].ColumnWidthInChars = 22;
                        ews1[intMainRfqRowCount, 2].Text = "Total RFQ(s): " + drRFQ.NoOfRfq.ToString() + Environment.NewLine + "New RFQ(s): " + drRFQ.NoOfRfq.ToString();
                        ews1[intMainRfqRowCount, 2].AutofitRows();
                        ews1[intMainRfqRowCount, 2].Style.Alignment.VerticalAlignment = ExcelCellVerticalAlignmentType.Top;

                        intRFQRowCounter++;

                        ews1[intRFQRowCounter, 3, intRFQRowCounter, 12].Merge();
                        ews1[intRFQRowCounter, 3].Text = "Monthly Total: " + string.Format("${0:#,##0.000}", dTotalQuoted);
                        ews1[intRFQRowCounter, 3].Style.Alignment.HorizontalAlignment = ExcelCellHorizontalAlignmentType.Right;
                        ews1[intRFQRowCounter, 3].Style.Alignment.VerticalAlignment = ExcelCellVerticalAlignmentType.Top;
                        ews1[intRFQRowCounter, 3].Style.Font.Bold = true;

                        ews1[intRFQRowCounter, 13].Text = "Monthly Total: " + string.Format("${0:#,##0.000}", dToolingCost);
                        ews1[intRFQRowCounter, 13].Style.Alignment.HorizontalAlignment = ExcelCellHorizontalAlignmentType.Right;
                        ews1[intRFQRowCounter, 13].Style.Alignment.VerticalAlignment = ExcelCellVerticalAlignmentType.Top;
                        ews1[intRFQRowCounter, 13].Style.Font.Bold = true;

                        ews1[intRFQRowCounter, 1].AutofitColumns();
                        ews1[intRFQRowCounter, 3].AutofitColumns();
                        ews1[intRFQRowCounter, 8].AutofitColumns();
                        ews1[intRFQRowCounter, 12].AutofitColumns();
                        ews1[intRFQRowCounter, 13].AutofitColumns();
                    }

                    lstRFQNonAwardReasonReport = null;
                    #endregion

                    #region  Excel Protected
                    ews1.WorksheetSecurity.ProtectWorksheet(System.Configuration.ConfigurationManager.AppSettings["ShipmentTemplatePswd"]);
                    ews2.WorksheetSecurity.ProtectWorksheet(System.Configuration.ConfigurationManager.AppSettings["ShipmentTemplatePswd"]);
                    #endregion

                    #region "Download"
                    string generatedFileName = string.Empty;

                    generatedFileName = Languages.GetResourceText("RFQNonAwardReasonReportFileName") + ".xls";// "RFQNonAwardReasonReport.xls";
                    ewb.Worksheets[0].Name = Languages.GetResourceText("RFQNonAwardReasonReportSheetName"); //"RFQ Non-Award Reason Report";

                    if (!System.IO.Directory.Exists(httpcontext.Server.MapPath(@"~\") + (Constants.REPORTSTEMPLATEFILEPATH)))
                        System.IO.Directory.CreateDirectory(httpcontext.Server.MapPath(@"~\") + (Constants.REPORTSTEMPLATEFILEPATH));

                    filePath = httpcontext.Server.MapPath(@"~\") + Constants.REPORTSTEMPLATEFILEPATH + @"\" + generatedFileName;

                    if (File.Exists(filePath))
                        File.Delete(filePath);
                    try
                    {
                        ewb.Save(filePath);
                        filePath = System.Configuration.ConfigurationManager.AppSettings["ApiURL"] + Constants.REPORTSTEMPLATEFILEPATH + generatedFileName;
                    }
                    catch (Exception ex) //Error
                    {
                        return FailedBoolResponse(ex.Message);
                    }
                    finally
                    {
                        sourceXlsDataStream.Close();
                        ewb.Close();
                    }

                    #endregion

                }
                catch (Exception ex) //Error
                {
                    return FailedBoolResponse(ex.Message);
                }
            }
            else
            {
                return FailedBoolResponse(Languages.GetResourceText("RFQNonAwardReasonReportTemplateNotExists"));
            }

            return SuccessBoolResponse(filePath);
        }
        #endregion

        #region Quote's Total Dollar Quoted Report
        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReport>> GetQuoteTotalDollarQuotedReport(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReportSearch> searchInfo)
        {
            if (searchInfo.Criteria.ReportId == 14)
                return GetQuoteTotalDollarQuotedReportData(searchInfo);
            else if (searchInfo.Criteria.ReportId == 15)
                return GetQuoteTotalDollarQuotedReportBaseQuotes(searchInfo);
            else
                return SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReport>>("", new List<DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReport>());
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReport>> GetQuoteTotalDollarQuotedReportData(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReportSearch> searchInfo)
        {
            string errMSg = null;
            List<DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReport> lstQuoteTotalDollarQuotedReport = new List<DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReport>();
            DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReport quoteTotalDollarQuotedReport;
            this.RunOnDB(context =>
            {
                var reportData = context.GetTotalDollarQuoted(searchInfo.Criteria.DateFrom, searchInfo.Criteria.DateTo).ToList();
                if (reportData == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    ////// prepare the object over here that contains date range,missing date column etc
                    List<string> lstMonthYearName = new List<string>();
                    if (searchInfo.Criteria.DateFrom != null && searchInfo.Criteria.DateFrom.HasValue && searchInfo.Criteria.DateTo != null && searchInfo.Criteria.DateTo.HasValue)
                    {
                        DateTime dtFrom, dtTo;
                        string AbbreviatedMonthYearName = string.Empty;
                        dtFrom = Convert.ToDateTime(searchInfo.Criteria.DateFrom);
                        dtTo = Convert.ToDateTime(searchInfo.Criteria.DateTo);
                        dtFrom = new DateTime(dtFrom.Year, dtFrom.Month, 1);

                        do
                        {
                            AbbreviatedMonthYearName = string.Empty;
                            AbbreviatedMonthYearName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(dtFrom.Month) + '-' + Convert.ToString(dtFrom.Year);
                            lstMonthYearName.Add(AbbreviatedMonthYearName);
                            dtFrom = dtFrom.AddMonths(1);
                        } while (dtFrom <= dtTo);
                    }
                    foreach (var item in reportData)
                    {
                        quoteTotalDollarQuotedReport = new DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReport();
                        quoteTotalDollarQuotedReport.SortOrder = item.SortOrder;
                        quoteTotalDollarQuotedReport.Customer = item.Customer;
                        quoteTotalDollarQuotedReport.MonthYearName = item.MonthYearName;
                        quoteTotalDollarQuotedReport.ToolingCost = item.ToolingCost ?? 0;
                        quoteTotalDollarQuotedReport.TotalAnnualCost = item.TotalAnnualCost ?? 0;
                        quoteTotalDollarQuotedReport.TotalQuoted = item.TotalQuoted ?? 0;
                        quoteTotalDollarQuotedReport.lstMonthName = lstMonthYearName;
                        lstQuoteTotalDollarQuotedReport.Add(quoteTotalDollarQuotedReport);
                    }
                    var tempList = lstQuoteTotalDollarQuotedReport;
                    bool IsMonthNameExist = false;
                    foreach (var item in tempList.GroupBy(a => a.Customer).ToList())
                    {
                        for (int i = 0; i < lstMonthYearName.Count; i++)
                        {
                            IsMonthNameExist = false;
                            foreach (var CusInfo in item)
                            {
                                if (CusInfo.MonthYearName.ToUpper() == lstMonthYearName[i].ToUpper())
                                {
                                    CusInfo.SortOrder = i + 1;
                                    IsMonthNameExist = true;
                                }
                            }
                            if (!IsMonthNameExist)
                            {
                                quoteTotalDollarQuotedReport = new DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReport();
                                quoteTotalDollarQuotedReport.SortOrder = i + 1;
                                quoteTotalDollarQuotedReport.Customer = item.Key;
                                quoteTotalDollarQuotedReport.MonthYearName = lstMonthYearName[i];
                                quoteTotalDollarQuotedReport.ToolingCost = 0;
                                quoteTotalDollarQuotedReport.TotalAnnualCost = 0;
                                quoteTotalDollarQuotedReport.TotalQuoted = 0;
                                quoteTotalDollarQuotedReport.lstMonthName = lstMonthYearName;
                                lstQuoteTotalDollarQuotedReport.Add(quoteTotalDollarQuotedReport);
                            }
                        }
                    }
                    lstQuoteTotalDollarQuotedReport = lstQuoteTotalDollarQuotedReport.OrderBy(a => a.SortOrder).OrderBy(b => b.Customer).ToList();

                }
            });
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReport>>(errMSg, lstQuoteTotalDollarQuotedReport);
            response.PageInfo = searchInfo.ToPage();
            return response;
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReport>> GetQuoteTotalDollarQuotedReportBaseQuotes(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReportSearch> searchInfo)
        {
            string errMSg = null;
            List<DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReport> lstQuoteTotalDollarQuotedReport = new List<DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReport>();
            DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReport quoteTotalDollarQuotedReport;
            this.RunOnDB(context =>
            {
                var reportData = context.GetTotalDollarQuotedOnlyBaseQuotes(searchInfo.Criteria.DateFrom, searchInfo.Criteria.DateTo).ToList();
                if (reportData == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    ////// prepare the object over here that contains date range,missing date column etc
                    List<string> lstMonthYearName = new List<string>();
                    if (searchInfo.Criteria.DateFrom != null && searchInfo.Criteria.DateFrom.HasValue && searchInfo.Criteria.DateTo != null && searchInfo.Criteria.DateTo.HasValue)
                    {
                        DateTime dtFrom, dtTo;
                        string AbbreviatedMonthYearName = string.Empty;
                        dtFrom = Convert.ToDateTime(searchInfo.Criteria.DateFrom);
                        dtTo = Convert.ToDateTime(searchInfo.Criteria.DateTo);
                        dtFrom = new DateTime(dtFrom.Year, dtFrom.Month, 1);

                        do
                        {
                            AbbreviatedMonthYearName = string.Empty;
                            AbbreviatedMonthYearName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(dtFrom.Month) + '-' + Convert.ToString(dtFrom.Year);
                            lstMonthYearName.Add(AbbreviatedMonthYearName);
                            dtFrom = dtFrom.AddMonths(1);
                        } while (dtFrom <= dtTo);
                    }
                    foreach (var item in reportData)
                    {
                        quoteTotalDollarQuotedReport = new DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReport();
                        quoteTotalDollarQuotedReport.SortOrder = item.SortOrder;
                        quoteTotalDollarQuotedReport.Customer = item.Customer;
                        quoteTotalDollarQuotedReport.MonthYearName = item.MonthYearName;
                        quoteTotalDollarQuotedReport.ToolingCost = item.ToolingCost ?? 0;
                        quoteTotalDollarQuotedReport.TotalAnnualCost = item.TotalAnnualCost ?? 0;
                        quoteTotalDollarQuotedReport.TotalQuoted = item.TotalQuoted ?? 0;
                        quoteTotalDollarQuotedReport.lstMonthName = lstMonthYearName;
                        lstQuoteTotalDollarQuotedReport.Add(quoteTotalDollarQuotedReport);
                    }
                    var tempList = lstQuoteTotalDollarQuotedReport;
                    bool IsMonthNameExist = false;
                    foreach (var item in tempList.GroupBy(a => a.Customer).ToList())
                    {
                        for (int i = 0; i < lstMonthYearName.Count; i++)
                        {
                            IsMonthNameExist = false;
                            foreach (var CusInfo in item)
                            {
                                if (CusInfo.MonthYearName.ToUpper() == lstMonthYearName[i].ToUpper())
                                {
                                    CusInfo.SortOrder = i + 1;
                                    IsMonthNameExist = true;
                                }
                            }
                            if (!IsMonthNameExist)
                            {
                                quoteTotalDollarQuotedReport = new DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReport();
                                quoteTotalDollarQuotedReport.SortOrder = i + 1;
                                quoteTotalDollarQuotedReport.Customer = item.Key;
                                quoteTotalDollarQuotedReport.MonthYearName = lstMonthYearName[i];
                                quoteTotalDollarQuotedReport.ToolingCost = 0;
                                quoteTotalDollarQuotedReport.TotalAnnualCost = 0;
                                quoteTotalDollarQuotedReport.TotalQuoted = 0;
                                quoteTotalDollarQuotedReport.lstMonthName = lstMonthYearName;
                                lstQuoteTotalDollarQuotedReport.Add(quoteTotalDollarQuotedReport);
                            }
                        }
                    }
                    lstQuoteTotalDollarQuotedReport = lstQuoteTotalDollarQuotedReport.OrderBy(a => a.SortOrder).OrderBy(b => b.Customer).ToList();

                }
            });
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReport>>(errMSg, lstQuoteTotalDollarQuotedReport);
            response.PageInfo = searchInfo.ToPage();
            return response;
        }

        public NPE.Core.ITypedResponse<bool?> exportQuoteTotalDollarQuotedReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReportSearch> searchCriteria)
        {
            string filePath = string.Empty;
            try
            {
                filePath = CreateExcelForQuoteTotalDollarQuotedReport(searchCriteria);
            }
            catch (Exception ex)//Error
            {
                return FailedBoolResponse(ex.Message);
            }
            return SuccessBoolResponse(filePath);
        }
        private string CreateExcelForQuoteTotalDollarQuotedReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReportSearch> searchCriteria)
        {
            string filepath = string.Empty;
            StringBuilder strBodyContent = new StringBuilder();
            int noOfColumns = 0;
            decimal RowWiseTotal = 0;
            List<string> lstMonthYearName = new List<string>();
            List<MES.DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReport> lstQuoteTotalDollarQuotedReport = new List<MES.DTO.Library.RFQ.RFQReports.QuoteTotalDollarQuotedReport>();
            if (searchCriteria.Criteria.ReportId == 14)
                lstQuoteTotalDollarQuotedReport = GetQuoteTotalDollarQuotedReportData(searchCriteria).Result;
            else if (searchCriteria.Criteria.ReportId == 15)
                lstQuoteTotalDollarQuotedReport = GetQuoteTotalDollarQuotedReportBaseQuotes(searchCriteria).Result;

            if (lstQuoteTotalDollarQuotedReport.Count > 0)
            {
                noOfColumns = lstQuoteTotalDollarQuotedReport[0].lstMonthName.Count;
                lstMonthYearName = lstQuoteTotalDollarQuotedReport[0].lstMonthName;
            }


            strBodyContent.AppendLine("<table style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");

            #region Header HTML
            strBodyContent.AppendLine("<thead>");
            strBodyContent.AppendLine("    <tr>");
            strBodyContent.AppendLine("        <td  align='left' valign='top' style='background:#fff;height:70px; text-align:right;color:#454545;font-size:13px; padding:10px;' colspan='" + Convert.ToString(1 + noOfColumns + 1) + "'>");  // +1 for first column, +1 for last column
            strBodyContent.AppendLine("            <img style='float:left' id='img1' src='" + System.Configuration.ConfigurationManager.AppSettings["ApiURL"].ToString() + Constants.LOGOIMAGEFOLDERINAPI + "MESlogoPdf.png" + "' alt='MES' />");
            strBodyContent.AppendLine("            <font size='2'>");
            strBodyContent.AppendLine(Constants.MESFULLADDRESS);
            strBodyContent.AppendLine("            </font>");
            strBodyContent.AppendLine("        </td>");
            strBodyContent.AppendLine("    </tr>");
            strBodyContent.AppendLine("</thead>");

            strBodyContent.AppendLine("<tbody>");
            strBodyContent.AppendLine("<tr>");
            strBodyContent.AppendLine("  <td valign='top' height='30' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:0;font-weight:400;'>Customer Name</td>");

            foreach (var monthName in lstMonthYearName)
            {
                strBodyContent.AppendLine("  <td valign='top' height='30' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:0;font-weight:400;'>");
                strBodyContent.AppendLine("    <table width='100%' cellspacing='1' border='1' cellpadding='0'>");
                strBodyContent.AppendLine("      <tr>");
                strBodyContent.AppendLine("        <td colspan='3' valign='top' style='background:#e7e7e7;text-align:center;color:#e67757;font-size:13px;'>");
                strBodyContent.AppendLine(monthName ?? "");
                strBodyContent.AppendLine("        </td>");
                strBodyContent.AppendLine("      </tr>");
                strBodyContent.AppendLine("      <tr>");
                strBodyContent.AppendLine("        <td width='33%' valign='top' style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px;'>Tooling Cost</td>");
                strBodyContent.AppendLine("        <td width='33%' valign='top' style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px;'>Annual Cost</td>");
                strBodyContent.AppendLine("        <td width='33%' valign='top' style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px;'>Total</td>");
                strBodyContent.AppendLine("      </tr>");
                strBodyContent.AppendLine("    </table>");
                strBodyContent.AppendLine("  </td>");
            }
            strBodyContent.AppendLine("  <td valign='top' style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:0;font-weight:400;'>Total</td>");
            strBodyContent.AppendLine("</tr>");
            strBodyContent.AppendLine("</tbody>");

            #endregion

            #region Main body loop
            foreach (var mItem in lstQuoteTotalDollarQuotedReport.GroupBy(g => g.Customer).ToList())
            {
                RowWiseTotal = 0;
                strBodyContent.AppendLine("<tbody>");
                strBodyContent.AppendLine("<tr>");
                strBodyContent.AppendLine("<td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(mItem.Key ?? "");
                strBodyContent.AppendLine("</td>");
                foreach (var nestedItem in mItem)
                {
                    strBodyContent.AppendLine("  <td valign='top'>");
                    strBodyContent.AppendLine("     <table width='100%' cellspacing='1' border='1' cellpadding='0'>");
                    strBodyContent.AppendLine("      <tr>");
                    strBodyContent.AppendLine("        <td width='33%' valign='top' style='text-align:right;font-size:13px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                    strBodyContent.AppendLine(string.Format("{0:0,0.000}", (nestedItem.ToolingCost.HasValue ? nestedItem.ToolingCost.Value : 0)));
                    strBodyContent.AppendLine("        </td>");
                    strBodyContent.AppendLine("        <td width='33%' valign='top' style='text-align:right;font-size:13px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                    strBodyContent.AppendLine(string.Format("{0:0,0.000}", (nestedItem.TotalAnnualCost.HasValue ? nestedItem.TotalAnnualCost.Value : 0)));
                    strBodyContent.AppendLine("        </td>");
                    strBodyContent.AppendLine("        <td width='33%' valign='top' style='text-align:right;font-size:13px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                    strBodyContent.AppendLine(string.Format("{0:0,0.000}", (nestedItem.TotalQuoted.HasValue ? nestedItem.TotalQuoted.Value : 0)));
                    strBodyContent.AppendLine("        </td>");
                    strBodyContent.AppendLine("      </tr>");
                    strBodyContent.AppendLine("    </table>");
                    strBodyContent.AppendLine("  </td>");
                    if (nestedItem.TotalQuoted.HasValue)
                        RowWiseTotal = RowWiseTotal + nestedItem.TotalQuoted.Value;
                }
                strBodyContent.AppendLine("<td valign='top' style='background:#ffe276;text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                strBodyContent.AppendLine(string.Format("{0:0,0.000}", RowWiseTotal));
                strBodyContent.AppendLine("</td>");
                strBodyContent.AppendLine("</tr>");
                strBodyContent.AppendLine("</tbody>");
            }

            RowWiseTotal = 0;
            strBodyContent.AppendLine("<tbody>");
            strBodyContent.AppendLine("<tr>");
            strBodyContent.AppendLine("  <td valign='top' height='30' style='background:#ffe276;text-align:left;color:#454545;font-size:13px; padding:0;font-weight:400;'>Total</td>");
            foreach (var monthName in lstMonthYearName)
            {
                strBodyContent.AppendLine("  <td valign='top' style='background:#ffe276;text-align:left;color:#454545;font-size:13px; padding:0;font-weight:400;'>");
                strBodyContent.AppendLine("     <table width='100%' cellspacing='1' border='1' cellpadding='0'>");
                strBodyContent.AppendLine("      <tr>");
                strBodyContent.AppendLine("        <td width='33%' valign='top' style='text-align:right;font-size:13px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                strBodyContent.AppendLine(string.Format("{0:0,0.000}", (lstQuoteTotalDollarQuotedReport.Where(a => a.MonthYearName == monthName).ToList().Sum(b => b.ToolingCost))));
                strBodyContent.AppendLine("        </td>");
                strBodyContent.AppendLine("        <td width='33%' valign='top' style='text-align:right;font-size:13px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                strBodyContent.AppendLine(string.Format("{0:0,0.000}", (lstQuoteTotalDollarQuotedReport.Where(a => a.MonthYearName == monthName).ToList().Sum(b => b.TotalAnnualCost))));
                strBodyContent.AppendLine("        </td>");
                strBodyContent.AppendLine("        <td width='33%' valign='top' style='text-align:right;font-size:13px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                strBodyContent.AppendLine(string.Format("{0:0,0.000}", (lstQuoteTotalDollarQuotedReport.Where(a => a.MonthYearName == monthName).ToList().Sum(b => b.TotalQuoted))));
                strBodyContent.AppendLine("        </td>");
                strBodyContent.AppendLine("      </tr>");
                strBodyContent.AppendLine("    </table>");
                strBodyContent.AppendLine("  </td>");
                RowWiseTotal = RowWiseTotal + (lstQuoteTotalDollarQuotedReport.Where(a => a.MonthYearName == monthName).ToList().Sum(b => b.TotalQuoted).HasValue ? lstQuoteTotalDollarQuotedReport.Where(a => a.MonthYearName == monthName).ToList().Sum(b => b.TotalQuoted).Value : 0);
            }
            strBodyContent.AppendLine("  <td valign='top' style='background:#ffe276;text-align:right;font-size:13px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
            strBodyContent.AppendLine(string.Format("{0:0,0.000}", RowWiseTotal));
            strBodyContent.AppendLine("</td>");
            strBodyContent.AppendLine("</tr>");
            strBodyContent.AppendLine("</tbody>");
            #endregion

            strBodyContent.AppendLine("</table>");

            using (var stringStream = strBodyContent.ToString().StringToStream())
            {
                filepath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                              , Constants.APQPTEMPORARYFILEFOLDER, "Total Dollar Quoted Report.xls", stringStream);
            }

            return filepath;
        }
        #endregion

        #region  Quotes done Report
        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQReports.QuotesDoneReport>> GetQuotesDoneReport(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.QuotesDoneReportSearch> searchInfo)
        {
            string errMSg = null;
            List<DTO.Library.RFQ.RFQReports.QuotesDoneReport> lstQuotesDoneReport = new List<DTO.Library.RFQ.RFQReports.QuotesDoneReport>();
            DTO.Library.RFQ.RFQReports.QuotesDoneReport quotesDoneReport;
            List<DTO.Library.RFQ.RFQReports.QuotesDoneChart> lstQuotesDoneChart = new List<DTO.Library.RFQ.RFQReports.QuotesDoneChart>();
            DTO.Library.RFQ.RFQReports.QuotesDoneChart quotesDoneChart;
            this.RunOnDB(context =>
            {
                var reportData = context.GetQuotesByCriteria(searchInfo.Criteria.SAMIds, searchInfo.Criteria.CustomerIds, searchInfo.Criteria.DateFrom, searchInfo.Criteria.DateTo).ToList();
                var chartData = context.GetQuotesByCriteriaChart(searchInfo.Criteria.SAMIds, searchInfo.Criteria.CustomerIds, searchInfo.Criteria.DateFrom, searchInfo.Criteria.DateTo, searchInfo.Criteria.GroupBy).ToList();
                if (reportData == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records

                    foreach (var chart in chartData)
                    {
                        quotesDoneChart = new DTO.Library.RFQ.RFQReports.QuotesDoneChart();
                        quotesDoneChart.DisplayName = chart.DisplayName;
                        quotesDoneChart.Amount = chart.Amount;
                        lstQuotesDoneChart.Add(quotesDoneChart);
                    }

                    foreach (var item in reportData)
                    {
                        quotesDoneReport = new DTO.Library.RFQ.RFQReports.QuotesDoneReport();
                        quotesDoneReport.QuoteNumber = item.QuoteNumber;
                        quotesDoneReport.RFQ = item.RFQ;
                        quotesDoneReport.Customer = item.Customer;
                        quotesDoneReport.AccountManager = item.AccountManager;
                        quotesDoneReport.Commodity = item.Commodity;
                        quotesDoneReport.Process = item.Process;
                        quotesDoneReport.Amount = item.Amount;
                        quotesDoneReport.QuoteDate = item.QuoteDate;
                        quotesDoneReport.QuoteResult = item.QuoteResult;
                        quotesDoneReport.AmountWon = item.AmountWon;
                        //extra properties for grouping
                        if (!string.IsNullOrEmpty(searchInfo.Criteria.GroupBy))
                        {
                            if (searchInfo.Criteria.GroupBy == "SAM")
                                quotesDoneReport.GroupByValue = string.IsNullOrEmpty(item.AccountManager) == true ? "Account Manager Not Assigned" : item.AccountManager;
                            else if (searchInfo.Criteria.GroupBy == "Commodity")
                                quotesDoneReport.GroupByValue = string.IsNullOrEmpty(item.Commodity) == true ? "Commodity Not Assigned" : item.Commodity;
                        }
                        else
                            quotesDoneReport.GroupByValue = "";
                        quotesDoneReport.lstQuotesDoneChart = lstQuotesDoneChart;
                        lstQuotesDoneReport.Add(quotesDoneReport);
                    }
                }
            });
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQReports.QuotesDoneReport>>(errMSg, lstQuotesDoneReport);
            response.PageInfo = searchInfo.ToPage();
            return response;
        }
        public NPE.Core.ITypedResponse<bool?> exportQuotesDoneReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.QuotesDoneReportSearch> paging)
        {
            string filePath = string.Empty;
            var httpcontext = HttpContext.Current;
            string strBody = string.Empty;
            filePath = httpcontext.Server.MapPath("~/" + Constants.REPORTSTEMPLATEFOLDER) + "QuoteDoneTemplate.xls";

            if (File.Exists(filePath))
            {
                try
                {
                    System.IO.FileStream sourceXlsDataStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    ExcelWorkbook ewb = new ExcelWorkbook(sourceXlsDataStream);
                    ewb.LicenseKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["wnvkey"]);
                    Winnovative.ExcelLib.ExcelWorksheet ews1 = ewb.Worksheets[0];
                    Winnovative.ExcelLib.ExcelWorksheet ews2 = ewb.Worksheets[1];

                    #region Quotes Done Report Chart Create

                    if (ews2.UsedRangeCells.Count > 0)
                        ews2.UsedRange.Clear();

                    ews2[1, 1].Value = "SAM"; //1. SAM
                    ews2[1, 2].Value = "No. Of RFQ"; //2. No. Of RFQ

                    int counter = 1, count = 0;
                    string GroupByType = "";
                    short groupBy = 1;
                    if (groupBy == 1)
                    {
                        ews2[1, 1].Value = "Commodity";
                        GroupByType = "Commodity";
                    }
                    else if (groupBy == 2)
                    {
                        ews2[1, 1].Value = "SAM";
                        GroupByType = "SAM";
                    }
                    else
                        ews2[1, 1].Value = "Month";

                    ews2[1, 2].Value = "Amount";

                    List<DTO.Library.RFQ.RFQReports.QuotesDoneReport> lstQuotesDoneReport = new List<DTO.Library.RFQ.RFQReports.QuotesDoneReport>();

                    lstQuotesDoneReport = GetQuotesDoneReport(paging).Result;

                    List<DTO.Library.RFQ.RFQReports.QuotesDoneChart> lstQuotesDoneChart = new List<DTO.Library.RFQ.RFQReports.QuotesDoneChart>();

                    lstQuotesDoneChart = lstQuotesDoneReport[0].lstQuotesDoneChart;
                    if (lstQuotesDoneChart.Count > 0)
                    {
                        ExcelRange er;
                        count = lstQuotesDoneChart.Count + 1;
                        foreach (var item in lstQuotesDoneChart)
                        {
                            counter++;
                            ews2[counter, 1].Value = item.DisplayName.ToString() + " (" + Convert.ToDouble(item.Amount.ToString()) + "%)";
                            ews2[counter, 2].NumberValue = Convert.ToDouble(item.Amount.ToString());
                        }

                        er = ews2[1, 1, count, 2];
                        ews1.Charts[0].DataSourceRange = er;
                    }
                    #endregion

                    #region  Quotes Done Report List Create
                    int intQuoteRowCounter = 1;
                    decimal dTotalAmount = 0;
                    foreach (DTO.Library.RFQ.RFQReports.QuotesDoneReport drQuote in lstQuotesDoneReport)
                    {
                        intQuoteRowCounter++;

                        ews1[intQuoteRowCounter, 1].Text = drQuote.QuoteNumber;
                        ews1[intQuoteRowCounter, 2].Text = drQuote.RFQ;
                        ews1[intQuoteRowCounter, 3].Text = drQuote.Customer;
                        ews1[intQuoteRowCounter, 4].Text = !string.IsNullOrEmpty(drQuote.AccountManager) ? drQuote.AccountManager : string.Empty;
                        ews1[intQuoteRowCounter, 5].Text = !string.IsNullOrEmpty(drQuote.Commodity) ? drQuote.Commodity : string.Empty;
                        ews1[intQuoteRowCounter, 6].Text = !string.IsNullOrEmpty(drQuote.Process) ? drQuote.Process : string.Empty;

                        if (!string.IsNullOrEmpty(drQuote.QuoteDate.ToString()))
                            ews1[intQuoteRowCounter, 7].Text = Convert.ToDateTime(drQuote.QuoteDate).ToString("dd-MMM-yy");

                        if (drQuote.Amount.HasValue)
                        {
                            ews1[intQuoteRowCounter, 8].Text = string.Format("${0:#,##0.000}", Convert.ToDecimal(drQuote.Amount));
                            dTotalAmount = dTotalAmount + Convert.ToDecimal(drQuote.Amount.ToString());
                        }
                        else
                        {
                            ews1[intQuoteRowCounter, 8].Text = string.Format("${0:#,##0.000}", 0);
                        }
                        ews1[intQuoteRowCounter, 8].Style.Alignment.HorizontalAlignment = ExcelCellHorizontalAlignmentType.Right;

                        ews1[intQuoteRowCounter, 9].Text = drQuote.QuoteResult ?? "";

                        if (!string.IsNullOrEmpty(drQuote.AmountWon.ToString()))
                            ews1[intQuoteRowCounter, 10].Text = string.Format("${0:#,##0.000}", Convert.ToDecimal(drQuote.AmountWon));
                        else
                            ews1[intQuoteRowCounter, 10].Text = "";

                        ews1[intQuoteRowCounter, 10].Style.Alignment.HorizontalAlignment = ExcelCellHorizontalAlignmentType.Right;
                    }

                    intQuoteRowCounter++;

                    ews1[intQuoteRowCounter, 1].AutofitColumns();
                    ews1[intQuoteRowCounter, 2].AutofitColumns();
                    ews1[intQuoteRowCounter, 3].AutofitColumns();
                    ews1[intQuoteRowCounter, 4].AutofitColumns();
                    ews1[intQuoteRowCounter, 5].AutofitColumns();
                    ews1[intQuoteRowCounter, 6].AutofitColumns();
                    ews1[intQuoteRowCounter, 7].AutofitColumns();
                    ews1[intQuoteRowCounter, 8].AutofitColumns();
                    ews1[intQuoteRowCounter, 9].AutofitColumns();
                    ews1[intQuoteRowCounter, 10].AutofitColumns();

                    ews1[intQuoteRowCounter, 1, intQuoteRowCounter, 7].Merge();
                    ews1[intQuoteRowCounter, 1].Text = "Total: " + string.Format("${0:#,##0.000}", dTotalAmount);
                    ews1[intQuoteRowCounter, 1].Style.Alignment.HorizontalAlignment = ExcelCellHorizontalAlignmentType.Right;
                    ews1[intQuoteRowCounter, 1].Style.Alignment.VerticalAlignment = ExcelCellVerticalAlignmentType.Top;
                    ews1[intQuoteRowCounter, 1].Style.Font.Bold = true;
                    lstQuotesDoneReport = null;
                    #endregion

                    #region  Excel Protected
                    ews1.WorksheetSecurity.ProtectWorksheet(System.Configuration.ConfigurationManager.AppSettings["ShipmentTemplatePswd"]);
                    ews2.WorksheetSecurity.ProtectWorksheet(System.Configuration.ConfigurationManager.AppSettings["ShipmentTemplatePswd"]);
                    #endregion

                    #region "Download"
                    string generatedFileName = string.Empty;
                    generatedFileName = Languages.GetResourceText("QuoteDoneReportFileName") + ".xls";// "QuoteDoneReport.xls";
                    ewb.Worksheets[0].Name = Languages.GetResourceText("QuoteDoneReportSheetName"); //"Quote Done Report";

                    if (!System.IO.Directory.Exists(httpcontext.Server.MapPath(@"~\") + (Constants.REPORTSTEMPLATEFILEPATH)))
                        System.IO.Directory.CreateDirectory(httpcontext.Server.MapPath(@"~\") + (Constants.REPORTSTEMPLATEFILEPATH));

                    filePath = httpcontext.Server.MapPath(@"~\") + Constants.REPORTSTEMPLATEFILEPATH + @"\" + generatedFileName;

                    if (File.Exists(filePath))
                        File.Delete(filePath);
                    try
                    {
                        ewb.Save(filePath);
                        filePath = System.Configuration.ConfigurationManager.AppSettings["ApiURL"] + Constants.REPORTSTEMPLATEFILEPATH + generatedFileName;
                    }
                    catch (Exception ex) //Error
                    {
                        return FailedBoolResponse(ex.Message);
                    }
                    finally
                    {
                        sourceXlsDataStream.Close();
                        ewb.Close();
                    }

                    #endregion

                }
                catch (Exception ex) //Error
                {
                    return FailedBoolResponse(ex.Message);
                }
            }
            else
            {
                return FailedBoolResponse(Languages.GetResourceText("QuoteDoneReportTemplateNotExists"));
            }

            return SuccessBoolResponse(filePath);
        }
        #endregion

        #region RFQ Quote Report by Supplier
        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReport>> GetRFQPartsSupplierWiseReport(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReportSearch> searchInfo)
        {
            if (searchInfo.Criteria.ReportId == 1)
                return GetRFQPartsSupplierWiseReportData(searchInfo);
            else if (searchInfo.Criteria.ReportId == 8)
                return GetRFQPartsSupplierWiseReportIgnoreBlank(searchInfo);
            else
                return SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReport>>("", new List<DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReport>());
        }
        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReport>> GetRFQPartsSupplierWiseReportData(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReportSearch> searchInfo)
        {
            string errMSg = null;
            List<DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReport> lstRFQPartsSupplierWiseReport = new List<DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReport>();
            DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReport rFQPartsSupplierWiseReport;
            DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReportSecondPart rFQPartsSupplierWiseReportSecondPart;
            DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReportThirdPart rFQPartsSupplierWiseReportThirdPart;
            this.RunOnDB(context =>
            {
                var firstPartData = context.GetRfqPartsSupplierWiseFirstPart(searchInfo.Criteria.RFQIds).ToList();
                var secondPartData = context.GetRfqPartsSupplierWiseSecondPart(searchInfo.Criteria.RFQIds).ToList();
                var thirdPartData = context.GetRfqPartsSupplierWiseThirdPart(searchInfo.Criteria.RFQIds).ToList();
                if (firstPartData == null || secondPartData == null || thirdPartData == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    foreach (var item in firstPartData)
                    {
                        rFQPartsSupplierWiseReport = new DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReport();
                        rFQPartsSupplierWiseReport.RFQNo = item.RFQNo;
                        rFQPartsSupplierWiseReport.rfqDate = item.rfqDate;
                        rFQPartsSupplierWiseReport.rfqDateString = item.rfqDate.FormatDateInMediumDate();
                        rFQPartsSupplierWiseReport.Customer = item.Customer;
                        rFQPartsSupplierWiseReport.lstRFQPartsSupplierWiseReportSecondPart = new List<DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReportSecondPart>();
                        foreach (var second in secondPartData.Where(a => a.RFQNo == item.RFQNo).ToList())
                        {
                            rFQPartsSupplierWiseReportSecondPart = new DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReportSecondPart();
                            rFQPartsSupplierWiseReportSecondPart.RFQNo = second.RFQNo;
                            rFQPartsSupplierWiseReportSecondPart.RfqPartid = second.RfqPartid;
                            rFQPartsSupplierWiseReportSecondPart.CustomerPartNo = second.CustomerPartNo;
                            rFQPartsSupplierWiseReportSecondPart.PartDescription = second.PartDescription;
                            rFQPartsSupplierWiseReportSecondPart.AdditionalPartDescription = second.AdditionalPartDescription;
                            rFQPartsSupplierWiseReportSecondPart.PartWeightKG = second.PartWeightKG;
                            rFQPartsSupplierWiseReportSecondPart.EstimatedQty = second.EstimatedQty;
                            rFQPartsSupplierWiseReportSecondPart.TotalParts = second.totalparts;
                            rFQPartsSupplierWiseReportSecondPart.lstRFQPartsSupplierWiseReportThirdPart = new List<DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReportThirdPart>();
                            foreach (var third in thirdPartData.Where(a => a.RfqPartId == second.RfqPartid).ToList())
                            {
                                rFQPartsSupplierWiseReportThirdPart = new DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReportThirdPart();
                                rFQPartsSupplierWiseReportThirdPart.RFQNo = third.RFQNo;
                                rFQPartsSupplierWiseReportThirdPart.RfqPartId = third.RfqPartId;
                                rFQPartsSupplierWiseReportThirdPart.CustomerPartNo = third.CustomerPartNo;
                                rFQPartsSupplierWiseReportThirdPart.EstimatedQty = third.EstimatedQty;
                                rFQPartsSupplierWiseReportThirdPart.PartWeightKG = third.PartWeightKG;
                                rFQPartsSupplierWiseReportThirdPart.SupplierName = third.SupplierName;
                                rFQPartsSupplierWiseReportThirdPart.ToolingCost = third.ToolingCost;
                                rFQPartsSupplierWiseReportThirdPart.MaterialCost = third.MaterialCost;
                                rFQPartsSupplierWiseReportThirdPart.ProcessCost = third.ProcessCost;
                                rFQPartsSupplierWiseReportThirdPart.MachiningCost = third.MachiningCost;
                                rFQPartsSupplierWiseReportThirdPart.OtherProcessCost = third.OtherProcessCost;
                                rFQPartsSupplierWiseReportThirdPart.UnitPrice = third.UnitPrice.HasValue ? third.UnitPrice.Value : 0;
                                rFQPartsSupplierWiseReportThirdPart.UpdatedDate = third.UpdatedDate;
                                rFQPartsSupplierWiseReportThirdPart.QuoteDateString = third.UpdatedDate.HasValue ? third.UpdatedDate.Value.FormatDateInMediumDate() : "";
                                rFQPartsSupplierWiseReportThirdPart.SupplierCostPerKg = third.SupplierCostPerKg;
                                rFQPartsSupplierWiseReportSecondPart.lstRFQPartsSupplierWiseReportThirdPart.Add(rFQPartsSupplierWiseReportThirdPart);
                            }
                            rFQPartsSupplierWiseReport.lstRFQPartsSupplierWiseReportSecondPart.Add(rFQPartsSupplierWiseReportSecondPart);
                        }
                        lstRFQPartsSupplierWiseReport.Add(rFQPartsSupplierWiseReport);
                    }
                }
            });
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReport>>(errMSg, lstRFQPartsSupplierWiseReport);
            response.PageInfo = searchInfo.ToPage();
            return response;
        }
        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReport>> GetRFQPartsSupplierWiseReportIgnoreBlank(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReportSearch> searchInfo)
        {
            string errMSg = null;
            List<DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReport> lstRFQPartsSupplierWiseReport = new List<DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReport>();
            DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReport rFQPartsSupplierWiseReport;
            DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReportSecondPart rFQPartsSupplierWiseReportSecondPart;
            DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReportThirdPart rFQPartsSupplierWiseReportThirdPart;
            this.RunOnDB(context =>
            {
                var firstPartData = context.GetRfqPartsSupplierWiseFirstPart(searchInfo.Criteria.RFQIds).ToList();
                var secondPartData = context.GetRfqPartsSupplierWiseSecondPartIgnoreBlank(searchInfo.Criteria.RFQIds).ToList();
                var thirdPartData = context.GetRfqPartsSupplierWiseThirdPartIgnoreBlank(searchInfo.Criteria.RFQIds).ToList();
                if (firstPartData == null || secondPartData == null || thirdPartData == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    foreach (var item in firstPartData)
                    {
                        rFQPartsSupplierWiseReport = new DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReport();
                        rFQPartsSupplierWiseReport.RFQNo = item.RFQNo;
                        rFQPartsSupplierWiseReport.rfqDate = item.rfqDate;
                        rFQPartsSupplierWiseReport.rfqDateString = item.rfqDate.FormatDateInMediumDate();
                        rFQPartsSupplierWiseReport.Customer = item.Customer;
                        rFQPartsSupplierWiseReport.lstRFQPartsSupplierWiseReportSecondPart = new List<DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReportSecondPart>();
                        foreach (var second in secondPartData.Where(a => a.RFQNo == item.RFQNo).ToList())
                        {
                            rFQPartsSupplierWiseReportSecondPart = new DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReportSecondPart();
                            rFQPartsSupplierWiseReportSecondPart.RFQNo = second.RFQNo;
                            rFQPartsSupplierWiseReportSecondPart.RfqPartid = second.RfqPartid;
                            rFQPartsSupplierWiseReportSecondPart.CustomerPartNo = second.CustomerPartNo;
                            rFQPartsSupplierWiseReportSecondPart.PartDescription = second.PartDescription;
                            rFQPartsSupplierWiseReportSecondPart.AdditionalPartDescription = second.AdditionalPartDescription;
                            rFQPartsSupplierWiseReportSecondPart.PartWeightKG = second.PartWeightKG;
                            rFQPartsSupplierWiseReportSecondPart.EstimatedQty = second.EstimatedQty;
                            rFQPartsSupplierWiseReportSecondPart.TotalParts = second.totalparts;
                            rFQPartsSupplierWiseReportSecondPart.lstRFQPartsSupplierWiseReportThirdPart = new List<DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReportThirdPart>();
                            foreach (var third in thirdPartData.Where(a => a.RfqPartId == second.RfqPartid).ToList())
                            {
                                rFQPartsSupplierWiseReportThirdPart = new DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReportThirdPart();
                                rFQPartsSupplierWiseReportThirdPart.RFQNo = third.RFQNo;
                                rFQPartsSupplierWiseReportThirdPart.RfqPartId = third.RfqPartId;
                                rFQPartsSupplierWiseReportThirdPart.CustomerPartNo = third.CustomerPartNo;
                                rFQPartsSupplierWiseReportThirdPart.EstimatedQty = third.EstimatedQty;
                                rFQPartsSupplierWiseReportThirdPart.PartWeightKG = third.PartWeightKG;
                                rFQPartsSupplierWiseReportThirdPart.SupplierName = third.SupplierName;
                                rFQPartsSupplierWiseReportThirdPart.ToolingCost = third.ToolingCost;
                                rFQPartsSupplierWiseReportThirdPart.MaterialCost = third.MaterialCost;
                                rFQPartsSupplierWiseReportThirdPart.ProcessCost = third.ProcessCost;
                                rFQPartsSupplierWiseReportThirdPart.MachiningCost = third.MachiningCost;
                                rFQPartsSupplierWiseReportThirdPart.OtherProcessCost = third.OtherProcessCost;
                                rFQPartsSupplierWiseReportThirdPart.UnitPrice = third.UnitPrice.HasValue ? third.UnitPrice.Value : 0;
                                rFQPartsSupplierWiseReportThirdPart.UpdatedDate = third.UpdatedDate;
                                rFQPartsSupplierWiseReportThirdPart.QuoteDateString = third.UpdatedDate.HasValue ? third.UpdatedDate.Value.FormatDateInMediumDate() : "";
                                rFQPartsSupplierWiseReportThirdPart.SupplierCostPerKg = third.SupplierCostPerKg;
                                rFQPartsSupplierWiseReportSecondPart.lstRFQPartsSupplierWiseReportThirdPart.Add(rFQPartsSupplierWiseReportThirdPart);
                            }
                            rFQPartsSupplierWiseReport.lstRFQPartsSupplierWiseReportSecondPart.Add(rFQPartsSupplierWiseReportSecondPart);
                        }
                        lstRFQPartsSupplierWiseReport.Add(rFQPartsSupplierWiseReport);
                    }
                }
            });
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReport>>(errMSg, lstRFQPartsSupplierWiseReport);
            response.PageInfo = searchInfo.ToPage();
            return response;
        }
        public NPE.Core.ITypedResponse<bool?> exportRFQPartsSupplierWiseReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReportSearch> searchCriteria)
        {
            string filePath = string.Empty;
            try
            {
                filePath = CreateExcelForRFQPartsSupplierWiseReport(searchCriteria);
            }
            catch (Exception ex)//Error
            {
                return FailedBoolResponse(ex.Message);
            }
            return SuccessBoolResponse(filePath);
        }
        private string CreateExcelForRFQPartsSupplierWiseReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReportSearch> searchCriteria)
        {
            string filepath = string.Empty;
            StringBuilder strBodyContent = new StringBuilder();
            List<DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReport> lstRFQPartsSupplierWiseReport = new List<DTO.Library.RFQ.RFQReports.RFQPartsSupplierWiseReport>();

            if (searchCriteria.Criteria.ReportId == 1)
                lstRFQPartsSupplierWiseReport = GetRFQPartsSupplierWiseReportData(searchCriteria).Result;
            else if (searchCriteria.Criteria.ReportId == 8)
                lstRFQPartsSupplierWiseReport = GetRFQPartsSupplierWiseReportIgnoreBlank(searchCriteria).Result;

            strBodyContent.AppendLine("<table style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
            #region Header HTML
            strBodyContent.AppendLine("    <thead>");
            strBodyContent.AppendLine("        <tr>");
            strBodyContent.AppendLine("            <td  align='left' valign='top' style='background:#fff;height:70px; text-align:right;color:#454545;font-size:13px; padding:10px;' colspan='13'>");
            strBodyContent.AppendLine("                <img style='float:left' id='img1' src='" + System.Configuration.ConfigurationManager.AppSettings["ApiURL"].ToString() + Constants.LOGOIMAGEFOLDERINAPI + "MESlogoPdf.png" + "' alt='MES' />");
            strBodyContent.AppendLine("                <font size='2'>");
            strBodyContent.AppendLine(Constants.MESFULLADDRESS);
            strBodyContent.AppendLine("                </font>");
            strBodyContent.AppendLine("            </td>");
            strBodyContent.AppendLine("        </tr>");
            strBodyContent.AppendLine("        <tr style=''>");
            strBodyContent.AppendLine("              <th height='30' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Part No.</th>");
            strBodyContent.AppendLine("              <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Description</th>");
            strBodyContent.AppendLine("              <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Annual Quantity</th>");
            strBodyContent.AppendLine("              <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Weight</th>");
            strBodyContent.AppendLine("              <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Supplier Name</th>");
            strBodyContent.AppendLine("              <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Quote Date</th>");
            strBodyContent.AppendLine("              <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Tooling Price</th>");
            strBodyContent.AppendLine("              <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Material Cost</th>");
            strBodyContent.AppendLine("              <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Conversion Cost</th>");
            strBodyContent.AppendLine("               <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Machining Cost</th>");
            strBodyContent.AppendLine("              <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Other Process Cost</th>");
            strBodyContent.AppendLine("              <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Piece Price</th>");
            strBodyContent.AppendLine("              <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Supplier Cost/kg</th>");
            strBodyContent.AppendLine("        </tr>");
            strBodyContent.AppendLine("    </thead>");
            #endregion
            strBodyContent.AppendLine("    <tbody>");
            #region main body loop
            foreach (var item in lstRFQPartsSupplierWiseReport)
            {
                //strBodyContent.AppendLine("<tr>");
                strBodyContent.AppendLine("    <tr>");
                strBodyContent.AppendLine("        <td colspan='13' style='background: #d6d6d6;'>");
                strBodyContent.AppendLine("            <label style='width:100% !important;font-size: 14px; color: #e67757;font-weight: 600;'>");
                strBodyContent.AppendLine("                  <i style='font-size: 14px;font-style: normal; color: #e67757;'>MES RFQ#: </i>");
                strBodyContent.AppendLine("                  <i style='font-size: 14px;font-style: normal; color: #000;font-weight: 400;'>" + item.RFQNo ?? "" + "</i>&nbsp;&nbsp;");
                strBodyContent.AppendLine("                  <i style='font-size: 14px;font-style: normal; color: #e67757;'>Customer Name: </i>");
                strBodyContent.AppendLine("                  <i style='font-size: 14px;font-style: normal; color: #000;font-weight: 400;'>" + item.Customer ?? "" + "</i>&nbsp;&nbsp;");
                strBodyContent.AppendLine("                  <i style='font-size: 14px;font-style: normal; color: #e67757;'>RFQ Date: </i>");
                strBodyContent.AppendLine("                  <i style='font-size: 14px;font-style: normal; color: #000;font-weight: 400;'>" + item.rfqDateString ?? "" + "</i>");
                strBodyContent.AppendLine("            </label>");
                strBodyContent.AppendLine("        </td>");
                strBodyContent.AppendLine("    </tr>");
                foreach (var second in item.lstRFQPartsSupplierWiseReportSecondPart)
                {
                    strBodyContent.AppendLine("    <tr>");
                    strBodyContent.AppendLine("        <td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>");
                    strBodyContent.AppendLine(second.CustomerPartNo ?? "");
                    strBodyContent.AppendLine("        </td>");
                    strBodyContent.AppendLine("        <td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>");
                    strBodyContent.AppendLine((second.PartDescription ?? "") + " " + (second.AdditionalPartDescription ?? ""));
                    strBodyContent.AppendLine("        </td>");
                    strBodyContent.AppendLine("        <td valign='top' style='text-align:right;color:#454545;font-size:13px; padding:0;'>");
                    strBodyContent.AppendLine(Convert.ToString(second.EstimatedQty.HasValue ? second.EstimatedQty.Value : 0));
                    strBodyContent.AppendLine("        </td>");
                    strBodyContent.AppendLine("        <td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>  ");
                    strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>");
                    foreach (var third in second.lstRFQPartsSupplierWiseReportThirdPart)
                    {
                        strBodyContent.AppendLine("               <tr>");
                        strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\#\\,\\#\\#0\\.000;'>");
                        strBodyContent.AppendLine(string.Format("{0:0,0.000}", (third.PartWeightKG.HasValue ? third.PartWeightKG.Value : 0)));
                        strBodyContent.AppendLine("                   </td>");
                        strBodyContent.AppendLine("               </tr>");
                    }
                    strBodyContent.AppendLine("           </table>                                                                                      ");
                    strBodyContent.AppendLine("        </td>                                                                                            ");
                    strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>  ");
                    strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>                                            ");
                    foreach (var third in second.lstRFQPartsSupplierWiseReportThirdPart)
                    {
                        strBodyContent.AppendLine("               <tr>");
                        strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;'>");
                        strBodyContent.AppendLine(third.SupplierName ?? "");
                        strBodyContent.AppendLine("                   </td>");
                        strBodyContent.AppendLine("               </tr>");
                    }
                    strBodyContent.AppendLine("           </table>                                                                                      ");
                    strBodyContent.AppendLine("        </td>                                                                                            ");
                    strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>  ");
                    strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>                                            ");
                    foreach (var third in second.lstRFQPartsSupplierWiseReportThirdPart)
                    {
                        strBodyContent.AppendLine("               <tr>");
                        strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;'>");
                        strBodyContent.AppendLine(third.QuoteDateString ?? "");
                        strBodyContent.AppendLine("                   </td>");
                        strBodyContent.AppendLine("               </tr>");
                    }
                    strBodyContent.AppendLine("           </table>                                                                                      ");
                    strBodyContent.AppendLine("        </td>                                                                                            ");
                    strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>  ");
                    strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>                                            ");
                    foreach (var third in second.lstRFQPartsSupplierWiseReportThirdPart)
                    {
                        strBodyContent.AppendLine("               <tr>");
                        strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                        strBodyContent.AppendLine(string.Format("{0:0,0.000}", (third.ToolingCost.HasValue ? third.ToolingCost.Value : 0)));
                        strBodyContent.AppendLine("                   </td>");
                        strBodyContent.AppendLine("               </tr>");
                    }
                    strBodyContent.AppendLine("           </table>                                                                                      ");
                    strBodyContent.AppendLine("        </td>                                                                                            ");
                    strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>  ");
                    strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>                                            ");
                    foreach (var third in second.lstRFQPartsSupplierWiseReportThirdPart)
                    {
                        strBodyContent.AppendLine("               <tr>");
                        strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                        strBodyContent.AppendLine(string.Format("{0:0,0.000}", (third.MaterialCost.HasValue ? third.MaterialCost.Value : 0)));
                        strBodyContent.AppendLine("                   </td>");
                        strBodyContent.AppendLine("               </tr>");
                    }
                    strBodyContent.AppendLine("           </table>                                                                                      ");
                    strBodyContent.AppendLine("        </td>                                                                                            ");
                    strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>  ");
                    strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>                                            ");
                    foreach (var third in second.lstRFQPartsSupplierWiseReportThirdPart)
                    {
                        strBodyContent.AppendLine("               <tr>");
                        strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                        strBodyContent.AppendLine(string.Format("{0:0,0.000}", (third.ProcessCost.HasValue ? third.ProcessCost.Value : 0)));
                        strBodyContent.AppendLine("                   </td>");
                        strBodyContent.AppendLine("               </tr>");
                    }
                    strBodyContent.AppendLine("           </table>                                                                                      ");
                    strBodyContent.AppendLine("        </td>                                                                                            ");
                    strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>  ");
                    strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>                                            ");
                    foreach (var third in second.lstRFQPartsSupplierWiseReportThirdPart)
                    {
                        strBodyContent.AppendLine("               <tr>");
                        strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                        strBodyContent.AppendLine(string.Format("{0:0,0.000}", (third.MachiningCost.HasValue ? third.MachiningCost.Value : 0)));
                        strBodyContent.AppendLine("                   </td>");
                        strBodyContent.AppendLine("               </tr>");
                    }
                    strBodyContent.AppendLine("           </table>                                                                                      ");
                    strBodyContent.AppendLine("        </td>                                                                                            ");
                    strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>  ");
                    strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>                                            ");
                    foreach (var third in second.lstRFQPartsSupplierWiseReportThirdPart)
                    {
                        strBodyContent.AppendLine("               <tr>");
                        strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                        strBodyContent.AppendLine(string.Format("{0:0,0.000}", (third.OtherProcessCost.HasValue ? third.OtherProcessCost.Value : 0)));
                        strBodyContent.AppendLine("                   </td>");
                        strBodyContent.AppendLine("               </tr>");
                    }
                    strBodyContent.AppendLine("           </table>                                                                                      ");
                    strBodyContent.AppendLine("        </td>                                                                                            ");
                    strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>  ");
                    strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>                                            ");
                    foreach (var third in second.lstRFQPartsSupplierWiseReportThirdPart)
                    {
                        strBodyContent.AppendLine("               <tr>");
                        strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                        strBodyContent.AppendLine(string.Format("{0:0,0.000}", third.UnitPrice));
                        strBodyContent.AppendLine("                   </td>");
                        strBodyContent.AppendLine("               </tr>");
                    }
                    strBodyContent.AppendLine("           </table>                                                                                      ");
                    strBodyContent.AppendLine("        </td>                                                                                            ");
                    strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>  ");
                    strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>                                            ");
                    foreach (var third in second.lstRFQPartsSupplierWiseReportThirdPart)
                    {
                        strBodyContent.AppendLine("               <tr>");
                        strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                        strBodyContent.AppendLine(string.Format("{0:0,0.000}", (third.SupplierCostPerKg.HasValue ? third.SupplierCostPerKg.Value : 0)));
                        strBodyContent.AppendLine("                   </td>");
                        strBodyContent.AppendLine("               </tr>");
                    }
                    strBodyContent.AppendLine("           </table>");
                    strBodyContent.AppendLine("        </td>");
                    strBodyContent.AppendLine("    </tr>");
                }
                if (item.lstRFQPartsSupplierWiseReportSecondPart.Count <= 0)
                {
                    strBodyContent.AppendLine("    <tr>");
                    strBodyContent.AppendLine("        <td colspan='13'>");
                    strBodyContent.AppendLine("            <label style='width:100% !important;font-size: 14px; color: #e67757;font-weight: 600;'>");
                    strBodyContent.AppendLine("                  No Records Found.");
                    strBodyContent.AppendLine("            </label>");
                    strBodyContent.AppendLine("        </td>");
                    strBodyContent.AppendLine("    </tr>");
                }
                //strBodyContent.AppendLine(" /tr>");
            }
            #endregion
            strBodyContent.AppendLine("    </tbody>");
            strBodyContent.AppendLine("</table>");

            using (var stringStream = strBodyContent.ToString().StringToStream())
            {
                filepath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                              , Constants.APQPTEMPORARYFILEFOLDER, "RfqPartsBySupplier.xls", stringStream);
            }
            return filepath;
        }
        #endregion

        #region Supplier Parts Quote Report
        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQReports.RFQSupplierPartsQuoteReport>> GetRFQSupplierPartsQuoteReport(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.RFQSupplierPartsQuoteReportSearch> searchInfo)
        {
            string errMSg = null;
            List<DTO.Library.RFQ.RFQReports.RFQSupplierPartsQuoteReport> lstRFQSupplierPartsQuoteReport = new List<DTO.Library.RFQ.RFQReports.RFQSupplierPartsQuoteReport>();
            DTO.Library.RFQ.RFQReports.RFQSupplierPartsQuoteReport rFQSupplierPartsQuoteReport;
            DTO.Library.RFQ.RFQReports.RFQSupplierPartsQuoteReportSecondPart rFQSupplierPartsQuoteReportSecondPart;
            DTO.Library.RFQ.RFQReports.RFQSupplierPartsQuoteReportThirdPart rFQSupplierPartsQuoteReportThirdPart;
            this.RunOnDB(context =>
            {
                var firstPartData = context.GetRFQSupplierPartsQuoteReportFirstPart(searchInfo.Criteria.RFQIds).ToList();
                var secondPartData = context.GetRFQSupplierPartsQuoteReportSecondPart(searchInfo.Criteria.RFQIds).ToList();
                var thirdPartData = context.GetRFQSupplierPartsQuoteReportThirdPart(searchInfo.Criteria.RFQIds).ToList();
                if (firstPartData == null || secondPartData == null || thirdPartData == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    foreach (var item in firstPartData)
                    {
                        rFQSupplierPartsQuoteReport = new DTO.Library.RFQ.RFQReports.RFQSupplierPartsQuoteReport();
                        rFQSupplierPartsQuoteReport.RFQNo = item.RFQNo;
                        rFQSupplierPartsQuoteReport.rfqDate = item.rfqDate;
                        rFQSupplierPartsQuoteReport.rfqDateString = item.rfqDate.FormatDateInMediumDate();
                        rFQSupplierPartsQuoteReport.Customer = item.Customer;
                        rFQSupplierPartsQuoteReport.lstRFQSupplierPartsQuoteReportSecondPart = new List<DTO.Library.RFQ.RFQReports.RFQSupplierPartsQuoteReportSecondPart>();
                        foreach (var second in secondPartData.Where(a => a.RFQNo == item.RFQNo).ToList())
                        {
                            rFQSupplierPartsQuoteReportSecondPart = new DTO.Library.RFQ.RFQReports.RFQSupplierPartsQuoteReportSecondPart();
                            rFQSupplierPartsQuoteReportSecondPart.RFQNo = second.RFQNo;
                            rFQSupplierPartsQuoteReportSecondPart.Id = second.Id;
                            rFQSupplierPartsQuoteReportSecondPart.SupplierName = second.SupplierName;
                            rFQSupplierPartsQuoteReportSecondPart.NoQuote = second.NoQuote;
                            rFQSupplierPartsQuoteReportSecondPart.totalparts = second.totalparts;
                            rFQSupplierPartsQuoteReportSecondPart.lstRFQSupplierPartsQuoteReportThirdPart = new List<DTO.Library.RFQ.RFQReports.RFQSupplierPartsQuoteReportThirdPart>();
                            foreach (var third in thirdPartData.Where(a => a.Id == second.Id).ToList())
                            {
                                rFQSupplierPartsQuoteReportThirdPart = new DTO.Library.RFQ.RFQReports.RFQSupplierPartsQuoteReportThirdPart();
                                rFQSupplierPartsQuoteReportThirdPart.RFQNo = third.RFQNo;
                                rFQSupplierPartsQuoteReportThirdPart.Id = third.Id;
                                rFQSupplierPartsQuoteReportThirdPart.SupplierName = third.SupplierName;
                                rFQSupplierPartsQuoteReportThirdPart.CustomerPartNo = third.CustomerPartNo;
                                rFQSupplierPartsQuoteReportThirdPart.RFQSupplirePartQuoteID = third.RFQSupplirePartQuoteID;
                                rFQSupplierPartsQuoteReportThirdPart.Remarks = third.Remarks;
                                rFQSupplierPartsQuoteReportThirdPart.ToolingCost = third.ToolingCost;
                                rFQSupplierPartsQuoteReportThirdPart.MaterialCost = third.MaterialCost;
                                rFQSupplierPartsQuoteReportThirdPart.ProcessCost = third.ProcessCost;
                                rFQSupplierPartsQuoteReportThirdPart.MachiningCost = third.MachiningCost;
                                rFQSupplierPartsQuoteReportThirdPart.OtherProcessCost = third.OtherProcessCost;
                                rFQSupplierPartsQuoteReportThirdPart.UnitPrice = third.UnitPrice.HasValue ? third.UnitPrice.Value : 0;
                                rFQSupplierPartsQuoteReportThirdPart.EstimatedQty = third.EstimatedQty;
                                rFQSupplierPartsQuoteReportThirdPart.PartWeightKG = third.PartWeightKG;
                                rFQSupplierPartsQuoteReportThirdPart.UpdatedDate = third.UpdatedDate;
                                rFQSupplierPartsQuoteReportThirdPart.QuoteDateString = third.UpdatedDate.HasValue ? third.UpdatedDate.Value.FormatDateInMediumDate() : "";
                                rFQSupplierPartsQuoteReportThirdPart.Currency = third.Currency;
                                rFQSupplierPartsQuoteReportThirdPart.ExchangeRate = third.ExchangeRate;
                                rFQSupplierPartsQuoteReportThirdPart.RawMaterialPriceAssumed = third.RawMaterialPriceAssumed;
                                rFQSupplierPartsQuoteReportThirdPart.QuoteAttachmentFilePath = third.QuoteAttachmentFilePath;
                                rFQSupplierPartsQuoteReportThirdPart.SupplierCostPerKg = third.SupplierCostPerKg;
                                rFQSupplierPartsQuoteReportSecondPart.lstRFQSupplierPartsQuoteReportThirdPart.Add(rFQSupplierPartsQuoteReportThirdPart);
                            }
                            rFQSupplierPartsQuoteReport.lstRFQSupplierPartsQuoteReportSecondPart.Add(rFQSupplierPartsQuoteReportSecondPart);
                        }
                        lstRFQSupplierPartsQuoteReport.Add(rFQSupplierPartsQuoteReport);
                    }
                }
            });
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQSupplierPartsQuoteReport>>(errMSg, lstRFQSupplierPartsQuoteReport);
            response.PageInfo = searchInfo.ToPage();
            return response;
        }
        public NPE.Core.ITypedResponse<bool?> exportRFQSupplierPartsQuoteReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.RFQSupplierPartsQuoteReportSearch> searchCriteria)
        {
            string filePath = string.Empty;
            try
            {
                filePath = CreateExcelForRFQSupplierPartsQuoteReport(searchCriteria);
            }
            catch (Exception ex)//Error
            {
                return FailedBoolResponse(ex.Message);
            }
            return SuccessBoolResponse(filePath);
        }
        private string CreateExcelForRFQSupplierPartsQuoteReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.RFQSupplierPartsQuoteReportSearch> searchCriteria)
        {
            string filepath = string.Empty;
            StringBuilder strBodyContent = new StringBuilder();
            List<DTO.Library.RFQ.RFQReports.RFQSupplierPartsQuoteReport> lstRFQSupplierPartsQuoteReport = new List<DTO.Library.RFQ.RFQReports.RFQSupplierPartsQuoteReport>();
            lstRFQSupplierPartsQuoteReport = GetRFQSupplierPartsQuoteReport(searchCriteria).Result;

            strBodyContent.AppendLine("<table style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
            #region Header HTML
            strBodyContent.AppendLine("    <thead>");
            strBodyContent.AppendLine("        <tr>");
            strBodyContent.AppendLine("            <td  align='left' valign='top' style='background:#fff;height:70px; text-align:right;color:#454545;font-size:13px; padding:10px;' colspan='11'>");
            strBodyContent.AppendLine("                <img style='float:left' id='img1' src='" + System.Configuration.ConfigurationManager.AppSettings["ApiURL"].ToString() + Constants.LOGOIMAGEFOLDERINAPI + "MESlogoPdf.png" + "' alt='MES' />");
            strBodyContent.AppendLine("                <font size='2'>");
            strBodyContent.AppendLine(Constants.MESFULLADDRESS);
            strBodyContent.AppendLine("                </font>");
            strBodyContent.AppendLine("            </td>");
            strBodyContent.AppendLine("        </tr>");
            strBodyContent.AppendLine("        <tr style=''>");
            strBodyContent.AppendLine("              <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Supplier Name</th>");
            strBodyContent.AppendLine("              <th height='30' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Part No.</th>");
            strBodyContent.AppendLine("              <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Part Weight (KG)</th>");
            strBodyContent.AppendLine("              <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Quote Date</th>");
            strBodyContent.AppendLine("              <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Tooling Price</th>");
            strBodyContent.AppendLine("              <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Material Cost</th>");
            strBodyContent.AppendLine("              <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Conversion Cost</th>");
            strBodyContent.AppendLine("               <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Machining Cost</th>");
            strBodyContent.AppendLine("              <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Other Process Cost</th>");
            strBodyContent.AppendLine("              <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Piece Price</th>");
            strBodyContent.AppendLine("              <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Supplier Cost/kg</th>");
            strBodyContent.AppendLine("        </tr>");
            strBodyContent.AppendLine("    </thead>");
            #endregion
            strBodyContent.AppendLine("    <tbody>");
            #region main body loop
            foreach (var item in lstRFQSupplierPartsQuoteReport)
            {
                //strBodyContent.AppendLine("<tr>");
                strBodyContent.AppendLine("    <tr>");
                strBodyContent.AppendLine("        <td colspan='11' style='background: #d6d6d6;'>");
                strBodyContent.AppendLine("            <label style='width:100% !important;font-size: 14px; color: #e67757;font-weight: 600;'>");
                strBodyContent.AppendLine("                  <i style='font-size: 14px;font-style: normal; color: #e67757;'>MES RFQ#: </i>");
                strBodyContent.AppendLine("                  <i style='font-size: 14px;font-style: normal; color: #000;font-weight: 400;'>" + item.RFQNo ?? "" + "</i>&nbsp;&nbsp;");
                strBodyContent.AppendLine("                  <i style='font-size: 14px;font-style: normal; color: #e67757;'>Customer Name: </i>");
                strBodyContent.AppendLine("                  <i style='font-size: 14px;font-style: normal; color: #000;font-weight: 400;'>" + item.Customer ?? "" + "</i>&nbsp;&nbsp;");
                strBodyContent.AppendLine("                  <i style='font-size: 14px;font-style: normal; color: #e67757;'>RFQ Date: </i>");
                strBodyContent.AppendLine("                  <i style='font-size: 14px;font-style: normal; color: #000;font-weight: 400;'>" + item.rfqDateString ?? "" + "</i>");
                strBodyContent.AppendLine("            </label>");
                strBodyContent.AppendLine("        </td>");
                strBodyContent.AppendLine("    </tr>");
                foreach (var second in item.lstRFQSupplierPartsQuoteReportSecondPart)
                {
                    strBodyContent.AppendLine("    <tr>");
                    strBodyContent.AppendLine("        <td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>");
                    strBodyContent.AppendLine(second.SupplierName ?? "");
                    strBodyContent.AppendLine("        </td>");
                    strBodyContent.AppendLine("        <td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>  ");
                    strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>");
                    foreach (var third in second.lstRFQSupplierPartsQuoteReportThirdPart)
                    {
                        strBodyContent.AppendLine("               <tr>");
                        strBodyContent.AppendLine("                   <td style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                        strBodyContent.AppendLine(third.CustomerPartNo ?? "");
                        strBodyContent.AppendLine("                   </td>");
                        strBodyContent.AppendLine("               </tr>");
                    }
                    strBodyContent.AppendLine("           </table>");
                    strBodyContent.AppendLine("        </td>");
                    strBodyContent.AppendLine("        <td valign='top' style='text-align:right;color:#454545;font-size:13px; padding:0;'>  ");
                    strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>");
                    foreach (var third in second.lstRFQSupplierPartsQuoteReportThirdPart)
                    {
                        strBodyContent.AppendLine("               <tr>");
                        strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\#\\,\\#\\#0\\.000;'>");
                        strBodyContent.AppendLine(string.Format("{0:0,0.000}", (third.PartWeightKG.HasValue ? third.PartWeightKG.Value : 0)));
                        strBodyContent.AppendLine("                   </td>");
                        strBodyContent.AppendLine("               </tr>");
                    }
                    strBodyContent.AppendLine("           </table>                                                                                      ");
                    strBodyContent.AppendLine("        </td>                                                                                            ");

                    strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>  ");
                    strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>                                            ");
                    foreach (var third in second.lstRFQSupplierPartsQuoteReportThirdPart)
                    {
                        strBodyContent.AppendLine("               <tr>");
                        strBodyContent.AppendLine("                   <td style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                        strBodyContent.AppendLine(third.QuoteDateString ?? "");
                        strBodyContent.AppendLine("                   </td>");
                        strBodyContent.AppendLine("               </tr>");
                    }
                    strBodyContent.AppendLine("           </table>                                                                                      ");
                    strBodyContent.AppendLine("        </td>                                                                                            ");
                    strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>  ");
                    strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>                                            ");
                    foreach (var third in second.lstRFQSupplierPartsQuoteReportThirdPart)
                    {
                        strBodyContent.AppendLine("               <tr>");
                        strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                        strBodyContent.AppendLine(string.Format("{0:0,0.000}", (third.ToolingCost.HasValue ? third.ToolingCost.Value : 0)));
                        strBodyContent.AppendLine("                   </td>");
                        strBodyContent.AppendLine("               </tr>");
                    }
                    strBodyContent.AppendLine("           </table>                                                                                      ");
                    strBodyContent.AppendLine("        </td>                                                                                            ");
                    strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>  ");
                    strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>                                            ");
                    foreach (var third in second.lstRFQSupplierPartsQuoteReportThirdPart)
                    {
                        strBodyContent.AppendLine("               <tr>");
                        strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                        strBodyContent.AppendLine(string.Format("{0:0,0.000}", (third.MaterialCost.HasValue ? third.MaterialCost.Value : 0)));
                        strBodyContent.AppendLine("                   </td>");
                        strBodyContent.AppendLine("               </tr>");
                    }
                    strBodyContent.AppendLine("           </table>                                                                                      ");
                    strBodyContent.AppendLine("        </td>                                                                                            ");
                    strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>  ");
                    strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>                                            ");
                    foreach (var third in second.lstRFQSupplierPartsQuoteReportThirdPart)
                    {
                        strBodyContent.AppendLine("               <tr>");
                        strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                        strBodyContent.AppendLine(string.Format("{0:0,0.000}", (third.ProcessCost.HasValue ? third.ProcessCost.Value : 0)));
                        strBodyContent.AppendLine("                   </td>");
                        strBodyContent.AppendLine("               </tr>");
                    }
                    strBodyContent.AppendLine("           </table>                                                                                      ");
                    strBodyContent.AppendLine("        </td>                                                                                            ");
                    strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>  ");
                    strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>                                            ");
                    foreach (var third in second.lstRFQSupplierPartsQuoteReportThirdPart)
                    {
                        strBodyContent.AppendLine("               <tr>");
                        strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                        strBodyContent.AppendLine(string.Format("{0:0,0.000}", (third.MachiningCost.HasValue ? third.MachiningCost.Value : 0)));
                        strBodyContent.AppendLine("                   </td>");
                        strBodyContent.AppendLine("               </tr>");
                    }
                    strBodyContent.AppendLine("           </table>                                                                                      ");
                    strBodyContent.AppendLine("        </td>                                                                                            ");
                    strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>  ");
                    strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>                                            ");
                    foreach (var third in second.lstRFQSupplierPartsQuoteReportThirdPart)
                    {
                        strBodyContent.AppendLine("               <tr>");
                        strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                        strBodyContent.AppendLine(string.Format("{0:0,0.000}", (third.OtherProcessCost.HasValue ? third.OtherProcessCost.Value : 0)));
                        strBodyContent.AppendLine("                   </td>");
                        strBodyContent.AppendLine("               </tr>");
                    }
                    strBodyContent.AppendLine("           </table>                                                                                      ");
                    strBodyContent.AppendLine("        </td>                                                                                            ");
                    strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>  ");
                    strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>                                            ");
                    foreach (var third in second.lstRFQSupplierPartsQuoteReportThirdPart)
                    {
                        strBodyContent.AppendLine("               <tr>");
                        strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                        strBodyContent.AppendLine(string.Format("{0:0,0.000}", third.UnitPrice));
                        strBodyContent.AppendLine("                   </td>");
                        strBodyContent.AppendLine("               </tr>");
                    }
                    strBodyContent.AppendLine("           </table>                                                                                      ");
                    strBodyContent.AppendLine("        </td>                                                                                            ");
                    strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>  ");
                    strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>                                            ");
                    foreach (var third in second.lstRFQSupplierPartsQuoteReportThirdPart)
                    {
                        strBodyContent.AppendLine("               <tr>");
                        strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                        strBodyContent.AppendLine(string.Format("{0:0,0.000}", (third.SupplierCostPerKg.HasValue ? third.SupplierCostPerKg.Value : 0)));
                        strBodyContent.AppendLine("                   </td>");
                        strBodyContent.AppendLine("               </tr>");
                    }
                    strBodyContent.AppendLine("           </table>");
                    strBodyContent.AppendLine("        </td>");
                    strBodyContent.AppendLine("    </tr>");
                }
                if (item.lstRFQSupplierPartsQuoteReportSecondPart.Count <= 0)
                {
                    strBodyContent.AppendLine("    <tr>");
                    strBodyContent.AppendLine("        <td colspan='11'>");
                    strBodyContent.AppendLine("            <label style='width:100% !important;font-size: 14px; color: #e67757;font-weight: 600;'>");
                    strBodyContent.AppendLine("                  No Records Found.");
                    strBodyContent.AppendLine("            </label>");
                    strBodyContent.AppendLine("        </td>");
                    strBodyContent.AppendLine("    </tr>");
                }
                //strBodyContent.AppendLine(" /tr>");
            }
            #endregion
            strBodyContent.AppendLine("    </tbody>");
            strBodyContent.AppendLine("</table>");

            using (var stringStream = strBodyContent.ToString().StringToStream())
            {
                filepath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                              , Constants.APQPTEMPORARYFILEFOLDER, "RfqSupplierPartsQuote.xls", stringStream);
            }
            return filepath;
        }
        #endregion

        #region RFQ Parts Cost Comparison Report
        public ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQ>> GetRFQPartCostComparisonReport(IPage<DTO.Library.RFQ.RFQReports.RFQPartCostComparisonReportSearch> searchInfo)
        {
            string errMSg = null;
            List<DTO.Library.RFQ.RFQ.RFQ> lstRFQ = new List<DTO.Library.RFQ.RFQ.RFQ>();
            DTO.Library.RFQ.RFQ.RFQ rFQDetails = null;
            RFQ.RFQ rfqObj = new RFQ.RFQ();
            DTO.Library.RFQ.RFQ.RFQ rfqInfo = null;
            List<DTO.Library.RFQ.RFQ.RFQPartCostComparision> lstrFQPartCostComparison = new List<DTO.Library.RFQ.RFQ.RFQPartCostComparision>();
            MES.DTO.Library.RFQ.RFQ.RFQPartCostComparision rFQPartCostComparison = null;
            List<DTO.Library.RFQ.RFQ.RFQParts> lstRFQPart = null;
            DTO.Library.RFQ.RFQ.RFQParts rfqPartItem = null;
            List<DTO.Library.RFQ.RFQ.RFQSuppliers> lstrfqSuppliers = new List<DTO.Library.RFQ.RFQ.RFQSuppliers>();
            //searchInfo.Criteria.RFQIds
            foreach (string rfqId in searchInfo.Criteria.RFQIds.Split(','))
            {
                rFQDetails = new DTO.Library.RFQ.RFQ.RFQ();
                rFQDetails.Id = rfqId;

                rfqInfo = rfqObj.FindById(rfqId).Result;
                rFQDetails.CustomerName = rfqInfo.CustomerName;
                rFQDetails.rfqDateString = rfqInfo.Date.FormatDateInMediumDate();
                int i = 0;
                this.RunOnDB(context =>
                {
                    MES.Business.Library.BO.RFQ.RFQ.RFQSuppliers rfqSuppliersObj = new MES.Business.Library.BO.RFQ.RFQ.RFQSuppliers();
                    rFQDetails.lstQuotedSuppliers = rfqSuppliersObj.GetRFQSuppliers(rfqId).Result.Where(item => item.SupplierId != 5).ToList();
                    //.Where(item => item.QuoteDate != null)

                    int sId = 0;
                    if (rFQDetails.lstQuotedSuppliers.Count > 0)
                    {
                        sId = rFQDetails.lstQuotedSuppliers[0].SupplierId;
                        while (i < rFQDetails.lstQuotedSuppliers.Count)
                        {
                            lstRFQPart = new List<DTO.Library.RFQ.RFQ.RFQParts>();
                            var rfqPartsList = context.Parts.Where(p => p.RFQId == rfqId && p.IsDeleted == false).ToList();
                            if (rfqPartsList != null)
                            {
                                foreach (var item in rfqPartsList)
                                {
                                    rfqPartItem = new DTO.Library.RFQ.RFQ.RFQParts();
                                    rfqPartItem.Id = item.Id;
                                    rfqPartItem.CustomerPartNo = item.CustomerPartNo;
                                    rfqPartItem.PartWeightKG = item.PartWeightKG;
                                    rfqPartItem.EstimatedQty = item.EstimatedQty;
                                    rfqPartItem.lstRFQPartCostComparison = new List<DTO.Library.RFQ.RFQ.RFQPartCostComparision>();

                                    foreach (var sItem in rFQDetails.lstQuotedSuppliers)
                                    {
                                        var rFQPartCostComparisonlst = context.GetRfqPartCostingComparisons(rfqId, rfqPartItem.Id, sItem.SupplierId).ToList();
                                        if (rFQPartCostComparisonlst != null && rFQPartCostComparisonlst.Count != 0)
                                        {
                                            foreach (var cItem in rFQPartCostComparisonlst)
                                            {
                                                rFQPartCostComparison = new DTO.Library.RFQ.RFQ.RFQPartCostComparision();
                                                rFQPartCostComparison.UpdatedDate = cItem.UpdatedDate;
                                                rFQPartCostComparison.ToolingCost = cItem.ToolingCost;
                                                rFQPartCostComparison.PiecePrice = cItem.UnitPrice.HasValue ? cItem.UnitPrice.Value : 0;
                                                rFQPartCostComparison.SupplierCostPerKg = cItem.SupplierCostPerKg.HasValue ? cItem.SupplierCostPerKg.Value : 0;
                                                rFQPartCostComparison.SupplierId = sItem.SupplierId;
                                                rFQPartCostComparison.rfqPartId = rfqPartItem.Id.Value;
                                                if (sId == sItem.SupplierId)
                                                {
                                                    rFQPartCostComparison.rdoSelect = true;
                                                    rFQPartCostComparison.rdoSelectValue = rFQPartCostComparison.SupplierId + "_" + rFQPartCostComparison.rfqPartId;
                                                }
                                                else
                                                    rFQPartCostComparison.rdoSelect = false;

                                                rfqPartItem.lstRFQPartCostComparison.Add(rFQPartCostComparison);
                                            }
                                        }
                                    }
                                    lstRFQPart.Add(rfqPartItem);
                                }
                                rFQDetails.lstRFQPart = lstRFQPart;
                            }
                            i++;
                        }
                    }
                });
                lstRFQ.Add(rFQDetails);
            }
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQ>>(errMSg, lstRFQ);
            response.PageInfo = searchInfo.ToPage();
            return response;
        }

        public ITypedResponse<bool?> exportRFQPartCostComparisonReport(IPage<DTO.Library.RFQ.RFQReports.RFQPartCostComparisonReportSearch> searchCriteria)
        {
            string filePath = string.Empty;
            try
            {
                filePath = CreateExcelForRFQPartCostComparisonReport(searchCriteria);
            }
            catch (Exception ex)//Error
            {
                return FailedBoolResponse(ex.Message);
            }
            return SuccessBoolResponse(filePath);
        }
        private string CreateExcelForRFQPartCostComparisonReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.RFQPartCostComparisonReportSearch> searchCriteria)
        {
            string filepath = string.Empty;
            StringBuilder strBodyContent = new StringBuilder();
            int noOfColumns = 0;
            List<DTO.Library.RFQ.RFQ.RFQ> lstRFQ = new List<DTO.Library.RFQ.RFQ.RFQ>();
            lstRFQ = GetRFQPartCostComparisonReport(searchCriteria).Result;
            if (lstRFQ.Count > 0)
            {
                noOfColumns = lstRFQ.Max(obj => obj.lstQuotedSuppliers.Count);
            }


            strBodyContent.AppendLine("<table style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
            #region Header HTML
            strBodyContent.AppendLine("<thead>");
            strBodyContent.AppendLine("    <tr>");
            strBodyContent.AppendLine("        <td  align='left' valign='top' style='background:#fff;height:70px; text-align:right;color:#454545;font-size:13px; padding:10px;' colspan='" + Convert.ToString(noOfColumns + 1) + "'>");
            strBodyContent.AppendLine("            <img style='float:left' id='img1' src='" + System.Configuration.ConfigurationManager.AppSettings["ApiURL"].ToString() + Constants.LOGOIMAGEFOLDERINAPI + "MESlogoPdf.png" + "' alt='MES' />");
            strBodyContent.AppendLine("            <font size='2'>");
            strBodyContent.AppendLine(Constants.MESFULLADDRESS);
            strBodyContent.AppendLine("            </font>");
            strBodyContent.AppendLine("        </td>");
            strBodyContent.AppendLine("    </tr>");
            strBodyContent.AppendLine("</thead>");
            #endregion

            #region Main body loop
            foreach (var mItem in lstRFQ)
            {
                strBodyContent.AppendLine("<tbody>");

                strBodyContent.AppendLine("    <tr>");
                strBodyContent.AppendLine("        <td valign='top' colspan='" + Convert.ToString(noOfColumns + 1) + "' style='background: #d6d6d6;'>");
                strBodyContent.AppendLine("            <label style='width:100% !important;font-size: 14px; color: #e67757;font-weight: 600;'>");
                strBodyContent.AppendLine("                  <i style='font-size: 14px;font-style: normal; color: #e67757;'>MES RFQ#: </i>");
                strBodyContent.AppendLine("                  <i style='font-size: 14px;font-style: normal; color: #000;font-weight: 400;'>" + mItem.Id ?? "" + "</i>&nbsp;&nbsp;");
                strBodyContent.AppendLine("                  <i style='font-size: 14px;font-style: normal; color: #e67757;'>Customer Name: </i>");
                strBodyContent.AppendLine("                  <i style='font-size: 14px;font-style: normal; color: #000;font-weight: 400;'>" + mItem.CustomerName ?? "" + "</i>&nbsp;&nbsp;");
                strBodyContent.AppendLine("                  <i style='font-size: 14px;font-style: normal; color: #e67757;'>RFQ Date: </i>");
                strBodyContent.AppendLine("                  <i style='font-size: 14px;font-style: normal; color: #000;font-weight: 400;'>" + mItem.rfqDateString ?? "" + "</i>");
                strBodyContent.AppendLine("            </label>");
                strBodyContent.AppendLine("        </td>");
                strBodyContent.AppendLine("    </tr>");

                strBodyContent.AppendLine("<tr>");
                strBodyContent.AppendLine("  <td valign='top' height='30' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:0;font-weight:400;'>");
                strBodyContent.AppendLine("    <table width='100%' cellspacing='1' border='1' cellpadding='0'>");
                strBodyContent.AppendLine("      <tr>");
                strBodyContent.AppendLine("        <td valign='top' colspan='3' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px;'>&nbsp;</td>");
                strBodyContent.AppendLine("      </tr>");
                strBodyContent.AppendLine("      <tr>");
                strBodyContent.AppendLine("        <td valign='top' width='33%' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px;'>Part No.</td> ");
                strBodyContent.AppendLine("        <td valign='top' width='33%' style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px;'>Estimated Qty</td>");
                strBodyContent.AppendLine("        <td valign='top' width='33%' style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px;'>Weight in KG</td>");
                strBodyContent.AppendLine("      </tr>");
                strBodyContent.AppendLine("    </table>");
                strBodyContent.AppendLine("  </td>");
                foreach (var sItem in mItem.lstQuotedSuppliers)
                {
                    strBodyContent.AppendLine("  <td valign='top' height='30' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:0;font-weight:400;'>");
                    strBodyContent.AppendLine("    <table width='100%' cellspacing='1' border='1' cellpadding='0'>");
                    strBodyContent.AppendLine("      <tr>");
                    strBodyContent.AppendLine("        <td colspan='4' valign='top' style='background:#e7e7e7;text-align:center;color:#e67757;font-size:13px;'>");
                    strBodyContent.AppendLine(sItem.CompanyName ?? "");
                    strBodyContent.AppendLine("        </td>");
                    strBodyContent.AppendLine("      </tr>");
                    strBodyContent.AppendLine("      <tr>");
                    strBodyContent.AppendLine("        <td width='25%' valign='top' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px;'>Quote Date</td>");
                    strBodyContent.AppendLine("        <td width='25%' valign='top' style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px;'>Piece Price</td>");
                    strBodyContent.AppendLine("        <td width='25%' valign='top' style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px;'>Tooling Price</td>");
                    strBodyContent.AppendLine("        <td width='25%' valign='top' style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px;'>Supplier Cost/kg</td>");
                    strBodyContent.AppendLine("      </tr>");
                    strBodyContent.AppendLine("    </table>");
                    strBodyContent.AppendLine("  </td>");
                }
                strBodyContent.AppendLine("</tr>");

                strBodyContent.AppendLine("<tr>");
                strBodyContent.AppendLine("   <td valign='top' style='background: #fff;'>");

                strBodyContent.AppendLine("      <table width='100%' cellspacing='1' border='1' cellpadding='0'>");
                if (mItem.lstRFQPart != null)
                {
                    foreach (var item in mItem.lstRFQPart)
                    {
                        strBodyContent.AppendLine("        <tr>");
                        strBodyContent.AppendLine("          <td width='33%' valign='top' style='text-align:left;font-size:13px;'>");
                        strBodyContent.AppendLine(item.CustomerPartNo ?? "");
                        strBodyContent.AppendLine("          </td>");
                        strBodyContent.AppendLine("          <td width='33%' valign='top' style='text-align:right;font-size:13px;'>");
                        strBodyContent.AppendLine(Convert.ToString(item.EstimatedQty.HasValue ? item.EstimatedQty.Value : 0));
                        strBodyContent.AppendLine("          </td>");
                        strBodyContent.AppendLine("          <td width='33%' valign='top' style='text-align:right;font-size:13px;mso-number-format:\\#\\,\\#\\#0\\.000;'>");
                        strBodyContent.AppendLine(string.Format("{0:0,0.000}", (item.PartWeightKG.HasValue ? item.PartWeightKG.Value : 0)));
                        strBodyContent.AppendLine("          </td>");
                        strBodyContent.AppendLine("        </tr>");
                    }
                }
                else
                {
                    strBodyContent.AppendLine("        <tr>");
                    strBodyContent.AppendLine("          <td colspan='3' width='100%' valign='top' style='text-align:left;font-size:13px;'>");
                    strBodyContent.AppendLine("No Records Found.");
                    strBodyContent.AppendLine("          </td>");
                    strBodyContent.AppendLine("        </tr>");
                }
                strBodyContent.AppendLine("      </table>");

                strBodyContent.AppendLine("  </td>");

                foreach (var tItem in mItem.lstQuotedSuppliers)
                {
                    strBodyContent.AppendLine("  <td valign='top'>");
                    foreach (var pItem in mItem.lstRFQPart)
                    {
                        strBodyContent.AppendLine("     <table width='100%' cellspacing='1' border='1' cellpadding='0'>");
                        foreach (var cItem in pItem.lstRFQPartCostComparison.Where(tt => tt.SupplierId == tItem.SupplierId && tt.rfqPartId == pItem.Id))
                        {
                            strBodyContent.AppendLine("      <tr>");
                            strBodyContent.AppendLine("        <td width='25%' valign='top' style='text-align:left;font-size:13px;'>");
                            strBodyContent.AppendLine(cItem.UpdatedDate.HasValue ? cItem.UpdatedDate.Value.FormatDateInMediumDate() : "");
                            strBodyContent.AppendLine("        </td>");
                            strBodyContent.AppendLine("        <td width='25%' valign='top' style='text-align:right;font-size:13px;mso-number-format:\\$\\#\\,\\#\\#0\\.000'>");
                            strBodyContent.AppendLine(string.Format("{0:0,0.000}", (cItem.PiecePrice)));
                            strBodyContent.AppendLine("        </td>");
                            strBodyContent.AppendLine("        <td width='25%' valign='top' style='text-align:right;font-size:13px;mso-number-format:\\$\\#\\,\\#\\#0\\.000'>");
                            strBodyContent.AppendLine(string.Format("{0:0,0.000}", (cItem.ToolingCost.HasValue ? cItem.ToolingCost.Value : 0)));
                            strBodyContent.AppendLine("        </td>");
                            strBodyContent.AppendLine("        <td width='25%' valign='top' style='text-align:right;font-size:13px;mso-number-format:\\$\\#\\,\\#\\#0\\.000'>");
                            strBodyContent.AppendLine(string.Format("{0:0,0.000}", (cItem.SupplierCostPerKg)));
                            strBodyContent.AppendLine("        </td>");
                            strBodyContent.AppendLine("      </tr>");
                        }
                        strBodyContent.AppendLine("    </table>");
                    }
                    strBodyContent.AppendLine("  </td>");
                }

                strBodyContent.AppendLine("</tr>");

                strBodyContent.AppendLine("</tbody>");
            }
            #endregion

            strBodyContent.AppendLine("</table>");

            using (var stringStream = strBodyContent.ToString().StringToStream())
            {
                filepath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                              , Constants.APQPTEMPORARYFILEFOLDER, "RfqPartCostingComparisions.xls", stringStream);
            }

            return filepath;
        }
        #endregion

        #region  RFQs Quoted By Supplier
        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQReports.RFQQuotedBySupplierReport>> GetRFQQuotedBySupplierReport(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.RFQQuotedBySupplierReportSearch> searchInfo)
        {
            string errMSg = null;
            List<DTO.Library.RFQ.RFQReports.RFQQuotedBySupplierReport> lstRFQQuotedBySupplierReport = new List<DTO.Library.RFQ.RFQReports.RFQQuotedBySupplierReport>();
            DTO.Library.RFQ.RFQReports.RFQQuotedBySupplierReport rFQQuotedBySupplierReport;
            this.RunOnDB(context =>
            {
                var reportData = context.GetRFQsQuotedBySupplier(searchInfo.Criteria.SupplierIds, searchInfo.Criteria.DateFrom, searchInfo.Criteria.DateTo, searchInfo.Criteria.SQIds, searchInfo.Criteria.CountryIds).ToList();
                if (reportData == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    foreach (var item in reportData)
                    {
                        rFQQuotedBySupplierReport = new DTO.Library.RFQ.RFQReports.RFQQuotedBySupplierReport();
                        rFQQuotedBySupplierReport.RFQID = item.RFQID;
                        rFQQuotedBySupplierReport.Customer = item.Customer;
                        rFQQuotedBySupplierReport.RFQDate = item.RFQDate;
                        rFQQuotedBySupplierReport.RFQDateString = item.RFQDate.FormatDateInMediumDate();
                        rFQQuotedBySupplierReport.ProjectName = item.ProjectName;
                        rFQQuotedBySupplierReport.Quoted = item.Quoted;
                        rFQQuotedBySupplierReport.CustomerContactName = item.CustomerContactName;
                        rFQQuotedBySupplierReport.Email = item.Email;
                        lstRFQQuotedBySupplierReport.Add(rFQQuotedBySupplierReport);
                    }
                }
            });
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQQuotedBySupplierReport>>(errMSg, lstRFQQuotedBySupplierReport);
            response.PageInfo = searchInfo.ToPage();
            return response;
        }
        public NPE.Core.ITypedResponse<bool?> exportRFQQuotedBySupplierReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.RFQQuotedBySupplierReportSearch> searchCriteria)
        {
            string filePath = string.Empty;
            try
            {
                filePath = CreateExcelForRFQQuotedBySupplierReport(searchCriteria);
            }
            catch (Exception ex)//Error
            {
                return FailedBoolResponse(ex.Message);
            }
            return SuccessBoolResponse(filePath);
        }
        private string CreateExcelForRFQQuotedBySupplierReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.RFQQuotedBySupplierReportSearch> searchCriteria)
        {
            string filepath = string.Empty;
            StringBuilder strBodyContent = new StringBuilder();
            List<MES.DTO.Library.RFQ.RFQReports.RFQQuotedBySupplierReport> lstRFQQuotedBySupplierReport = new List<MES.DTO.Library.RFQ.RFQReports.RFQQuotedBySupplierReport>();
            lstRFQQuotedBySupplierReport = GetRFQQuotedBySupplierReport(searchCriteria).Result;

            strBodyContent.AppendLine("<table style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
            #region Header HTML
            strBodyContent.AppendLine("<thead>");
            strBodyContent.AppendLine("    <tr>");
            strBodyContent.AppendLine("        <td  align='left' valign='top' style='background:#fff;height:70px; text-align:right;color:#454545;font-size:13px; padding:10px;' colspan='7'>");
            strBodyContent.AppendLine("            <img style='float:left' id='img1' src='" + System.Configuration.ConfigurationManager.AppSettings["ApiURL"].ToString() + Constants.LOGOIMAGEFOLDERINAPI + "MESlogoPdf.png" + "' alt='MES' />");
            strBodyContent.AppendLine("            <font size='2'>");
            strBodyContent.AppendLine(Constants.MESFULLADDRESS);
            strBodyContent.AppendLine("            </font>");
            strBodyContent.AppendLine("        </td>");
            strBodyContent.AppendLine("    </tr>");
            strBodyContent.AppendLine("    <tr style=''>");
            strBodyContent.AppendLine("        <th height='30' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>RFQ ID</th>");
            strBodyContent.AppendLine("        <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Customer Name</th>");
            strBodyContent.AppendLine("        <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Project Name</th>");
            strBodyContent.AppendLine("        <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>RFQ Date</th>");
            strBodyContent.AppendLine("        <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Quoted</th>");
            strBodyContent.AppendLine("        <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Customer Contact Name</th>");
            strBodyContent.AppendLine("        <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Email</th>");
            strBodyContent.AppendLine("    </tr>");
            strBodyContent.AppendLine("</thead>");
            #endregion
            strBodyContent.AppendLine("<tbody>");
            #region Main body loop
            foreach (var item in lstRFQQuotedBySupplierReport)
            {
                strBodyContent.AppendLine("<tr>");
                strBodyContent.AppendLine("    <td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.RFQID ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.Customer ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.ProjectName ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.RFQDateString ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.Quoted ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.CustomerContactName ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.Email ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("</tr>");
            }
            #endregion
            strBodyContent.AppendLine("</tbody>");
            strBodyContent.AppendLine("</table>");

            using (var stringStream = strBodyContent.ToString().StringToStream())
            {
                filepath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                              , Constants.APQPTEMPORARYFILEFOLDER, "RFQs Quoted By Supplier Report.xls", stringStream);
            }

            return filepath;
        }
        #endregion

        #region RFQ Supplier list report
        public NPE.Core.ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQSupplierReport>> GetRFQSupplierReport(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.RFQSupplierReportSearch> searchInfo)
        {
            string errMSg = null;
            List<DTO.Library.RFQ.RFQReports.RFQSupplierReport> lstRFQSupplierReport = new List<DTO.Library.RFQ.RFQReports.RFQSupplierReport>();
            DTO.Library.RFQ.RFQReports.RFQSupplierReport rFQSupplierReport;
            this.RunOnDB(context =>
            {
                var reportData = context.GetSuppliersByCriteria(searchInfo.Criteria.SQIds, searchInfo.Criteria.CountryIds).ToList();
                if (reportData == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    foreach (var item in reportData)
                    {
                        rFQSupplierReport = new DTO.Library.RFQ.RFQReports.RFQSupplierReport();
                        rFQSupplierReport.CompanyName = item.CompanyName;
                        rFQSupplierReport.Address1 = item.Address1;
                        rFQSupplierReport.Address2 = item.Address2;
                        rFQSupplierReport.City = item.City;
                        rFQSupplierReport.State = item.State;
                        rFQSupplierReport.Country = item.Country;
                        rFQSupplierReport.Zip = item.Zip;
                        rFQSupplierReport.Website = item.Website;
                        rFQSupplierReport.CompanyPhone1 = item.CompanyPhone1;
                        rFQSupplierReport.CompanyPhone2 = item.CompanyPhone2;
                        rFQSupplierReport.CompanyFax = item.CompanyFax;
                        rFQSupplierReport.Comments = item.Comments;
                        rFQSupplierReport.PrimaryContact = item.PrimaryContact;
                        rFQSupplierReport.Email = item.Email;
                        rFQSupplierReport.DirectPhone = item.DirectPhone;
                        lstRFQSupplierReport.Add(rFQSupplierReport);
                    }
                }
            });
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQSupplierReport>>(errMSg, lstRFQSupplierReport);
            response.PageInfo = searchInfo.ToPage();
            return response;
        }
        public NPE.Core.ITypedResponse<bool?> exportRFQSupplierReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.RFQSupplierReportSearch> searchCriteria)
        {
            string filePath = string.Empty;
            try
            {
                filePath = CreateExcelForRFQSupplierReport(searchCriteria);
            }
            catch (Exception ex)//Error
            {
                return FailedBoolResponse(ex.Message);
            }
            return SuccessBoolResponse(filePath);
        }
        private string CreateExcelForRFQSupplierReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.RFQSupplierReportSearch> searchCriteria)
        {
            string filepath = string.Empty;
            StringBuilder strBodyContent = new StringBuilder();
            List<MES.DTO.Library.RFQ.RFQReports.RFQSupplierReport> lstRFQSupplierReport = new List<MES.DTO.Library.RFQ.RFQReports.RFQSupplierReport>();
            lstRFQSupplierReport = GetRFQSupplierReport(searchCriteria).Result;

            strBodyContent.AppendLine("<table style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
            #region Header HTML
            strBodyContent.AppendLine("<thead>");
            strBodyContent.AppendLine("    <tr>");
            strBodyContent.AppendLine("        <td  align='left' valign='top' style='background:#fff;height:70px; text-align:right;color:#454545;font-size:13px; padding:10px;' colspan='15'>");
            strBodyContent.AppendLine("            <img style='float:left' id='img1' src='" + System.Configuration.ConfigurationManager.AppSettings["ApiURL"].ToString() + Constants.LOGOIMAGEFOLDERINAPI + "MESlogoPdf.png" + "' alt='MES' />");
            strBodyContent.AppendLine("            <font size='2'>");
            strBodyContent.AppendLine(Constants.MESFULLADDRESS); //strBodyContent.AppendLine("625 Bear Run Lane,<br />"); strBodyContent.AppendLine("Lewis Center, OH 43035<br />");strBodyContent.AppendLine("(740) 201-8112, sales@mesinc.net");            
            strBodyContent.AppendLine("            </font>");
            strBodyContent.AppendLine("        </td>");
            strBodyContent.AppendLine("    </tr>");
            strBodyContent.AppendLine("    <tr style=''>");
            strBodyContent.AppendLine("        <th height='30' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Company Name</th>");
            strBodyContent.AppendLine("        <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Address1</th>");
            strBodyContent.AppendLine("        <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Address2</th>");
            strBodyContent.AppendLine("        <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>City</th>");
            strBodyContent.AppendLine("        <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>State</th>");
            strBodyContent.AppendLine("        <th width='200' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Country</th>");
            strBodyContent.AppendLine("        <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Zip</th>");
            strBodyContent.AppendLine("        <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Website</th>");
            strBodyContent.AppendLine("        <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Company Phone1</th>");
            strBodyContent.AppendLine("        <th width='200' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Company Phone2</th>");
            strBodyContent.AppendLine("        <th width='200' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Company Fax</th>");
            strBodyContent.AppendLine("        <th width='200' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Comments</th>");
            strBodyContent.AppendLine("        <th width='200' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Primary Contact</th>");
            strBodyContent.AppendLine("        <th width='200' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Email</th>");
            strBodyContent.AppendLine("        <th width='200' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Direct Phone</th>");
            strBodyContent.AppendLine("    </tr>");
            strBodyContent.AppendLine("</thead>");
            #endregion
            strBodyContent.AppendLine("<tbody>");
            #region Main body loop
            foreach (var item in lstRFQSupplierReport)
            {
                strBodyContent.AppendLine("<tr>");
                strBodyContent.AppendLine("    <td align='left' valign='top' align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.CompanyName ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.Address1 ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.Address2 ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.City ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.State ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.Country ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.Zip ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.Website ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.CompanyPhone1 ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.CompanyPhone2 ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.CompanyFax ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.Comments ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.PrimaryContact ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.Email ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.DirectPhone ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("</tr>");
            }
            #endregion
            strBodyContent.AppendLine("</tbody>");
            strBodyContent.AppendLine("</table>");

            using (var stringStream = strBodyContent.ToString().StringToStream())
            {
                filepath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                              , Constants.APQPTEMPORARYFILEFOLDER, "Supplier List.xls", stringStream);
            }

            return filepath;
        }
        #endregion

        #region RFQ Supplier Activity Level Report
        public NPE.Core.ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQSupplierActivityLevelReport>> GetRFQSupplierActivityLevelReport(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.RFQSupplierActivityLevelReportSearch> searchInfo)
        {
            string errMSg = null;
            List<DTO.Library.RFQ.RFQReports.RFQSupplierActivityLevelReport> lstRFQSupplierActivityLevelReport = new List<DTO.Library.RFQ.RFQReports.RFQSupplierActivityLevelReport>();
            DTO.Library.RFQ.RFQReports.RFQSupplierActivityLevelReport rFQSupplierActivityLevelReport;
            this.RunOnDB(context =>
            {
                var reportData = context.GetSupplierActivityLevel(searchInfo.Criteria.SupplierIds, searchInfo.Criteria.SQIds, searchInfo.Criteria.CountryIds, searchInfo.Criteria.CommodityIds).ToList();
                if (reportData == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    foreach (var item in reportData)
                    {
                        rFQSupplierActivityLevelReport = new DTO.Library.RFQ.RFQReports.RFQSupplierActivityLevelReport();
                        rFQSupplierActivityLevelReport.Supplier = item.Supplier;
                        rFQSupplierActivityLevelReport.SystemDate = item.SystemDate;
                        rFQSupplierActivityLevelReport.SystemDateString = item.SystemDate.FormatDateInMediumDate();
                        rFQSupplierActivityLevelReport.SentAnyRFQ = item.SentAnyRFQ;
                        rFQSupplierActivityLevelReport.NoOfRFQSent = item.NoOfRFQSent;
                        rFQSupplierActivityLevelReport.QuotedAnyProject = item.QuotedAnyProject;
                        rFQSupplierActivityLevelReport.NoOfRFQsQuoted = item.NoOfRFQsQuoted;
                        rFQSupplierActivityLevelReport.LastRFQQuoted = item.LastRFQQuoted;
                        rFQSupplierActivityLevelReport.DateLastQuoted = item.DateLastQuoted;
                        if (item.DateLastQuoted.HasValue)
                            rFQSupplierActivityLevelReport.DateLastQuotedString = item.DateLastQuoted.Value.FormatDateInMediumDate();
                        lstRFQSupplierActivityLevelReport.Add(rFQSupplierActivityLevelReport);
                    }
                }
            });
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQReports.RFQSupplierActivityLevelReport>>(errMSg, lstRFQSupplierActivityLevelReport);
            response.PageInfo = searchInfo.ToPage();
            return response;
        }
        public NPE.Core.ITypedResponse<bool?> exportRFQSupplierActivityLevelReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.RFQSupplierActivityLevelReportSearch> searchCriteria)
        {
            string filePath = string.Empty;
            try
            {
                filePath = CreateExcelForRFQSupplierActivityLevelReport(searchCriteria);
            }
            catch (Exception ex)//Error
            {
                return FailedBoolResponse(ex.Message);
            }
            return SuccessBoolResponse(filePath);
        }
        private string CreateExcelForRFQSupplierActivityLevelReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.RFQSupplierActivityLevelReportSearch> searchCriteria)
        {
            string filepath = string.Empty;
            StringBuilder strBodyContent = new StringBuilder();
            List<MES.DTO.Library.RFQ.RFQReports.RFQSupplierActivityLevelReport> lstRFQSupplierActivityLevelReport = new List<MES.DTO.Library.RFQ.RFQReports.RFQSupplierActivityLevelReport>();
            lstRFQSupplierActivityLevelReport = GetRFQSupplierActivityLevelReport(searchCriteria).Result;

            strBodyContent.AppendLine("<table style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
            #region Header HTML
            strBodyContent.AppendLine("<thead>");
            strBodyContent.AppendLine("    <tr>");
            strBodyContent.AppendLine("        <td  align='left' valign='top' style='background:#fff;height:70px; text-align:right;color:#454545;font-size:13px; padding:10px;' colspan='8'>");
            strBodyContent.AppendLine("            <img style='float:left' id='img1' src='" + System.Configuration.ConfigurationManager.AppSettings["ApiURL"].ToString() + Constants.LOGOIMAGEFOLDERINAPI + "MESlogoPdf.png" + "' alt='MES' />");
            strBodyContent.AppendLine("            <font size='2'>");
            strBodyContent.AppendLine(Constants.MESFULLADDRESS);
            strBodyContent.AppendLine("            </font>");
            strBodyContent.AppendLine("        </td>");
            strBodyContent.AppendLine("    </tr>");
            strBodyContent.AppendLine("    <tr style=''>");
            strBodyContent.AppendLine("        <th height='30' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Supplier</th>");
            strBodyContent.AppendLine("        <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>System Created Date</th>");
            strBodyContent.AppendLine("        <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Sent any RFQ?</th>");
            strBodyContent.AppendLine("        <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>No. of RFQ sent</th>");
            strBodyContent.AppendLine("        <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Quoted any Project?</th>");
            strBodyContent.AppendLine("        <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>No. of RFQ's Quoted</th>");
            strBodyContent.AppendLine("        <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Last RFQ Quoted</th>");
            strBodyContent.AppendLine("        <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Date Last Quoted</th>");
            strBodyContent.AppendLine("    </tr>");
            strBodyContent.AppendLine("</thead>");
            #endregion
            strBodyContent.AppendLine("<tbody>");
            #region Main body loop
            foreach (var item in lstRFQSupplierActivityLevelReport)
            {
                strBodyContent.AppendLine("<tr>");
                strBodyContent.AppendLine("    <td align='left' valign='top' align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.Supplier ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.SystemDateString ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.SentAnyRFQ ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:right;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(Convert.ToString(item.NoOfRFQSent.HasValue ? item.NoOfRFQSent.Value : 0));
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.QuotedAnyProject ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:right;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(Convert.ToString(item.NoOfRFQsQuoted.HasValue ? item.NoOfRFQsQuoted.Value : 0));
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.LastRFQQuoted ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.DateLastQuotedString ?? "");
                strBodyContent.AppendLine("    </td>");

                strBodyContent.AppendLine("</tr>");
            }
            #endregion
            strBodyContent.AppendLine("</tbody>");
            strBodyContent.AppendLine("</table>");

            using (var stringStream = strBodyContent.ToString().StringToStream())
            {
                filepath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                              , Constants.APQPTEMPORARYFILEFOLDER, "Supplier Activity Level List.xls", stringStream);
            }

            return filepath;
        }
        #endregion

        #region open RFQ report
        public NPE.Core.ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.OpenRFQsReport>> GetOpenRFQsReport(NPE.Core.IPage<DTO.Library.RFQ.RFQReports.OpenRFQsReportSearch> searchInfo)
        {
            string errMSg = null;
            List<DTO.Library.RFQ.RFQReports.OpenRFQsReport> lstOpenRFQsReport = new List<DTO.Library.RFQ.RFQReports.OpenRFQsReport>();
            DTO.Library.RFQ.RFQReports.OpenRFQsReport openRFQsReport;
            this.RunOnDB(context =>
            {
                var reportData = context.GetOpenRFQs(searchInfo.Criteria.DateFrom, searchInfo.Criteria.DateTo, searchInfo.Criteria.QuoteDateFrom, searchInfo.Criteria.QuoteDateTo, searchInfo.Criteria.SAMIds,
                    searchInfo.Criteria.CustomerIds, searchInfo.Criteria.ProjectName, searchInfo.Criteria.CountryIds, searchInfo.Criteria.SupQuoted, searchInfo.Criteria.RFQTypeIds).ToList();
                if (reportData == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    foreach (var item in reportData)
                    {
                        openRFQsReport = new DTO.Library.RFQ.RFQReports.OpenRFQsReport();
                        openRFQsReport.RFQNumber = item.RFQNumber;
                        openRFQsReport.Customer = item.Customer;
                        openRFQsReport.RFQDate = item.RFQDate;
                        openRFQsReport.RFQDateString = item.RFQDate.FormatDateInMediumDate();
                        openRFQsReport.QuoteDueDate = item.QuoteDueDate;
                        if (item.QuoteDueDate.HasValue)
                            openRFQsReport.QuoteDueDateString = item.QuoteDueDate.Value.FormatDateInMediumDate();
                        openRFQsReport.SalesAccountManager = item.SalesAccountManager;
                        openRFQsReport.ProjectName = item.ProjectName;
                        openRFQsReport.SupplierQuoted = item.SupplierQuoted;
                        openRFQsReport.NumberOfPartNumbers = item.NumberOfPartNumbers;
                        openRFQsReport.Commodity = item.Commodity;
                        openRFQsReport.Process = item.Process;
                        openRFQsReport.Commodity = item.Commodity;
                        openRFQsReport.RFQPriority = item.RFQPriority;
                        openRFQsReport.SupplierRequirement = item.SupplierRequirement;
                        if (!string.IsNullOrEmpty(item.QuoteFloated))
                        {
                            if (item.QuoteFloated.Contains(','))
                                openRFQsReport.QuoteFloated = "BOTH";
                            else
                                openRFQsReport.QuoteFloated = item.QuoteFloated;
                        }
                        openRFQsReport.Rfqcoordinator = item.Rfqcoordinator;
                        openRFQsReport.RFQTypeName = item.RFQTypeName;
                        lstOpenRFQsReport.Add(openRFQsReport);
                    }
                }
            });
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQReports.OpenRFQsReport>>(errMSg, lstOpenRFQsReport);
            response.PageInfo = searchInfo.ToPage();
            return response;
        }
        public NPE.Core.ITypedResponse<bool?> exportOpenRFQsReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.OpenRFQsReportSearch> searchCriteria)
        {
            string filePath = string.Empty;
            try
            {
                filePath = CreateExcelForOpenRFQsReport(searchCriteria);
            }
            catch (Exception ex)//Error
            {
                return FailedBoolResponse(ex.Message);
            }
            return SuccessBoolResponse(filePath);
        }
        private string CreateExcelForOpenRFQsReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.OpenRFQsReportSearch> searchCriteria)
        {
            string filepath = string.Empty;
            StringBuilder strBodyContent = new StringBuilder();
            List<MES.DTO.Library.RFQ.RFQReports.OpenRFQsReport> lstOpenRFQsReport = new List<MES.DTO.Library.RFQ.RFQReports.OpenRFQsReport>();
            lstOpenRFQsReport = GetOpenRFQsReport(searchCriteria).Result;

            strBodyContent.AppendLine("<table style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
            #region Header HTML
            strBodyContent.AppendLine("    <thead>");
            strBodyContent.AppendLine("        <tr>");
            strBodyContent.AppendLine("            <td  align='left' valign='top' style='background:#fff;height:70px; text-align:right;color:#454545;font-size:13px; padding:10px;' colspan='15'>");
            strBodyContent.AppendLine("                <img style='float:left' id='img1' src='" + System.Configuration.ConfigurationManager.AppSettings["ApiURL"].ToString() + Constants.LOGOIMAGEFOLDERINAPI + "MESlogoPdf.png" + "' alt='MES' />");
            strBodyContent.AppendLine("                <font size='2'>");
            strBodyContent.AppendLine(Constants.MESFULLADDRESS);
            strBodyContent.AppendLine("                </font>");
            strBodyContent.AppendLine("            </td>");
            strBodyContent.AppendLine("        </tr>");
            strBodyContent.AppendLine("        <tr style=''>");
            strBodyContent.AppendLine("            <th  height='30' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>RFQ Number</th>");
            strBodyContent.AppendLine("            <th width='250' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Customer</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>RFQ Date</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Quote Due Date</th>");
            strBodyContent.AppendLine("            <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Sales Account Manager</th>");
            strBodyContent.AppendLine("            <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>RFQ Coordinator</th>");
            strBodyContent.AppendLine("            <th width='200' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Project Name</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Supplier Quoted?</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Quote Floated</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Number of Part Numbers</th>");
            strBodyContent.AppendLine("            <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Commodity</th>");
            strBodyContent.AppendLine("            <th width='150' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Process</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>RFQ Type</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>RFQ Priority</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Supplier Requirement</th>");
            strBodyContent.AppendLine("        </tr>");
            strBodyContent.AppendLine("    </thead>");
            #endregion
            strBodyContent.AppendLine("    <tbody>");
            #region main body loop
            foreach (var item in lstOpenRFQsReport)
            {
                strBodyContent.AppendLine("<tr>");
                strBodyContent.AppendLine("    <td align='left' valign='top' align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.RFQNumber ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.Customer ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.RFQDateString ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.QuoteDueDateString ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.SalesAccountManager ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.Rfqcoordinator ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.ProjectName ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.SupplierQuoted ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.QuoteFloated ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:right;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(Convert.ToString(item.NumberOfPartNumbers.HasValue ? item.NumberOfPartNumbers.Value : 0));
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.Commodity ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.Process ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.RFQTypeName ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.RFQPriority ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("    <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.SupplierRequirement ?? "");
                strBodyContent.AppendLine("    </td>");
                strBodyContent.AppendLine("</tr>");
            }
            #endregion
            strBodyContent.AppendLine("    </tbody>");
            strBodyContent.AppendLine("</table>");

            using (var stringStream = strBodyContent.ToString().StringToStream())
            {
                filepath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                              , Constants.APQPTEMPORARYFILEFOLDER, "Open RFQs Report.xls", stringStream);
            }
            return filepath;
        }
        #endregion

        #region Detailed Supplier Quote report

        public ITypedResponse<List<MES.DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteReport>> GetDetailedSupplierQuoteReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteSearch> searchInfo)
        {
            string errMSg = null;
            List<DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteReport> lstDetailedSupplierQuoteReport = new List<DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteReport>();
            DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteReport detailedSupplierQuoteReport;
            this.RunOnDB(context =>
            {
                var reportData = context.dqGetRfqPartsBySupplier(searchInfo.Criteria.SupplierIds, searchInfo.Criteria.DateFrom, searchInfo.Criteria.DateTo).ToList();
                if (reportData == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    foreach (var item in reportData)
                    {
                        detailedSupplierQuoteReport = new DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteReport();
                        detailedSupplierQuoteReport.SupplierId = item.Id;
                        detailedSupplierQuoteReport.SupplierName = item.SupplierName;
                        detailedSupplierQuoteReport.State = item.SupplierState;
                        detailedSupplierQuoteReport.Country = item.Country;
                        detailedSupplierQuoteReport.SentAnyRFQ = item.SentAnyRFQ;
                        detailedSupplierQuoteReport.NoOfRFQSent = item.NoOfRFQSent;
                        detailedSupplierQuoteReport.NoOfRFQsQuoted = item.NoOfRFQQuoted;
                        detailedSupplierQuoteReport.NoOfParts = item.NoOfParts;

                        detailedSupplierQuoteReport.ToolingCost = item.ToolingPrice;
                        detailedSupplierQuoteReport.MaterialCost = item.MaterialCost;
                        detailedSupplierQuoteReport.ProcessCost = item.ConversionCost;
                        detailedSupplierQuoteReport.MachiningCost = item.MachiningCost;
                        detailedSupplierQuoteReport.OtherProcessCost = item.OtherProcessCost;
                        detailedSupplierQuoteReport.TotalQuoted = item.TotalDolarQuoted;
                        detailedSupplierQuoteReport.QuoteWon = item.QuoteWon;
                        lstDetailedSupplierQuoteReport.Add(detailedSupplierQuoteReport);
                    }
                }
            });
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteReport>>(errMSg, lstDetailedSupplierQuoteReport);
            response.PageInfo = searchInfo.ToPage();
            return response;
        }
        public ITypedResponse<List<DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteSubReportFirstPart>> GetRfqPartQuoteDetailsReport(IPage<DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteSearch> searchInfo)
        {
            string errMSg = null;

            List<DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteSubReportFirstPart> lstRfq = new List<DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteSubReportFirstPart>();
            DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteSubReportFirstPart detailedSupplierQuoteSubReportFirstPart;
            DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteSubReportSecondPart detailedSupplierQuoteSubReportSecondPart;
            this.RunOnDB(context =>
            {
                var firstPartData = context.dqGetRfqPartsSupplierViseFirstPart(searchInfo.Criteria.SupplierId, searchInfo.Criteria.DateFrom, searchInfo.Criteria.DateTo).ToList();
                var secondPartData = context.dqGetRfqPartsSupplierViseSecondPart(searchInfo.Criteria.SupplierId, searchInfo.Criteria.DateFrom, searchInfo.Criteria.DateTo).ToList();

                if (firstPartData == null || secondPartData == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    foreach (var item in firstPartData)
                    {
                        detailedSupplierQuoteSubReportFirstPart = new DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteSubReportFirstPart();
                        detailedSupplierQuoteSubReportFirstPart.RFQNo = item.RFQId;

                        detailedSupplierQuoteSubReportFirstPart.lstDetailedSupplierQuoteSubReportSecondPart = new List<DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteSubReportSecondPart>();
                        foreach (var second in secondPartData.Where(a => a.RFQId == item.RFQId).ToList())
                        {
                            detailedSupplierQuoteSubReportSecondPart = new DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteSubReportSecondPart();
                            detailedSupplierQuoteSubReportSecondPart.CustomerPartNo = second.CustomerPartNo;
                            detailedSupplierQuoteSubReportSecondPart.Description = second.PartDescription;
                            detailedSupplierQuoteSubReportSecondPart.AddlDescription = second.AdditionalPartDescription;
                            detailedSupplierQuoteSubReportSecondPart.EstimatedQty = second.EstimatedQty;
                            detailedSupplierQuoteSubReportSecondPart.PartWeightKG = second.PartWeightKG;
                            detailedSupplierQuoteSubReportSecondPart.QuoteDateString = second.UpdatedDate.HasValue ? second.UpdatedDate.Value.FormatDateInMediumDate() : string.Empty;
                            detailedSupplierQuoteSubReportSecondPart.ToolingCost = second.ToolingCost;
                            detailedSupplierQuoteSubReportSecondPart.MaterialCost = second.MaterialCost;
                            detailedSupplierQuoteSubReportSecondPart.ProcessCost = second.ProcessCost;
                            detailedSupplierQuoteSubReportSecondPart.MachiningCost = second.MachiningCost;
                            detailedSupplierQuoteSubReportSecondPart.OtherProcessCost = second.OtherProcessCost;
                            detailedSupplierQuoteSubReportSecondPart.UnitPrice = second.UnitPrice.HasValue ? second.UnitPrice.Value : 0;
                            detailedSupplierQuoteSubReportSecondPart.SupplierCostPerKg = second.SupplierCostPerKg;

                            detailedSupplierQuoteSubReportFirstPart.lstDetailedSupplierQuoteSubReportSecondPart.Add(detailedSupplierQuoteSubReportSecondPart);
                        }

                        lstRfq.Add(detailedSupplierQuoteSubReportFirstPart);
                    }
                }
            });
            var response = SuccessOrFailedResponse<List<DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteSubReportFirstPart>>(errMSg, lstRfq);
            response.PageInfo = searchInfo.ToPage();
            return response;

        }
        public ITypedResponse<bool?> exportDetailedSupplierQuoteReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteSearch> searchCriteria)
        {
            string filePath = string.Empty;
            try
            {
                if (!searchCriteria.Criteria.isInnerReport)
                    filePath = CreateExcelForDetailedSupplierQuoteReport(searchCriteria);   // main report
                else
                    filePath = CreateExcelForDetailedSupplierInnerReport(searchCriteria);   //inner report
            }
            catch (Exception ex)//Error
            {
                return FailedBoolResponse(ex.Message);
            }
            return SuccessBoolResponse(filePath);
        }
        private string CreateExcelForDetailedSupplierQuoteReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteSearch> searchCriteria)
        {
            string filepath = string.Empty;
            StringBuilder strBodyContent = new StringBuilder();
            List<DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteReport> lstDetailedSupplierQuoteReport = new List<DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteReport>();
            lstDetailedSupplierQuoteReport = GetDetailedSupplierQuoteReport(searchCriteria).Result;

            strBodyContent.AppendLine("<table style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
            #region Header HTML
            strBodyContent.AppendLine("    <thead>");
            strBodyContent.AppendLine("        <tr>");
            strBodyContent.AppendLine("            <td  align='left' valign='top' style='background:#fff;height:70px; text-align:right;color:#454545;font-size:13px; padding:10px;' colspan='14'>");
            strBodyContent.AppendLine("                <img style='float:left' id='img1' src='" + System.Configuration.ConfigurationManager.AppSettings["ApiURL"].ToString() + Constants.LOGOIMAGEFOLDERINAPI + "MESlogoPdf.png" + "' alt='MES' />");
            strBodyContent.AppendLine("                <font size='2'>");
            strBodyContent.AppendLine(Constants.MESFULLADDRESS);
            strBodyContent.AppendLine("                </font>");
            strBodyContent.AppendLine("            </td>");
            strBodyContent.AppendLine("        </tr>");
            strBodyContent.AppendLine("        <tr style=''>");
            strBodyContent.AppendLine("            <th height='30' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Supplier Name</th");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Supplier State</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Supplier Country</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Sent RFQ To Supplier?</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:center;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Number of RFQ sent</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:center;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Number of RFQ Quoted</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:center;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Number of parts</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Tooling Price</th>");
            strBodyContent.AppendLine("            <th width='150' style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Material Cost</th");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Conversion Cost</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Machining Cost</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Other Process Cost</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Total $ Quoted</th>");
            strBodyContent.AppendLine("            <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Quote Won</th>");
            strBodyContent.AppendLine("        </tr>");
            strBodyContent.AppendLine("    </thead>");
            #endregion
            strBodyContent.AppendLine("    <tbody>");
            #region main body loop
            foreach (var item in lstDetailedSupplierQuoteReport)
            {
                strBodyContent.AppendLine("<tr>");
                strBodyContent.AppendLine("      <td align='left' valign='top' align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.SupplierName ?? "");
                strBodyContent.AppendLine("      </td>");
                strBodyContent.AppendLine("      <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.State ?? "");
                strBodyContent.AppendLine("      </td>                                                                                             ");
                strBodyContent.AppendLine("      <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.Country ?? "");
                strBodyContent.AppendLine("      </td>                                                                                             ");
                strBodyContent.AppendLine("      <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.SentAnyRFQ ?? "");
                strBodyContent.AppendLine("      </td>                                                                                             ");
                strBodyContent.AppendLine("      <td align='left' valign='top' style='text-align:center;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(Convert.ToString(item.NoOfRFQSent.HasValue ? item.NoOfRFQSent.Value : 0));
                strBodyContent.AppendLine("      </td>                                                                                             ");
                strBodyContent.AppendLine("      <td align='left' valign='top' style='text-align:center;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(Convert.ToString(item.NoOfRFQsQuoted.HasValue ? item.NoOfRFQsQuoted.Value : 0));
                strBodyContent.AppendLine("      </td>                                                                                             ");
                strBodyContent.AppendLine("      <td align='left' valign='top' style='text-align:center;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(Convert.ToString(item.NoOfParts.HasValue ? item.NoOfParts.Value : 0));
                strBodyContent.AppendLine("      </td>                                                                                             ");
                strBodyContent.AppendLine("      <td align='left' valign='top' style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                strBodyContent.AppendLine(string.Format("${0:0,0.000}", (item.ToolingCost.HasValue ? item.ToolingCost.Value : 0)));
                strBodyContent.AppendLine("      </td>                                                                                             ");
                strBodyContent.AppendLine("      <td align='left' valign='top' style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                strBodyContent.AppendLine(string.Format("${0:0,0.000}", (item.MaterialCost.HasValue ? item.MaterialCost.Value : 0)));
                strBodyContent.AppendLine("      </td>                                                                                             ");
                strBodyContent.AppendLine("      <td align='left' valign='top' style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                strBodyContent.AppendLine(string.Format("${0:0,0.000}", (item.ProcessCost.HasValue ? item.ProcessCost.Value : 0)));
                strBodyContent.AppendLine("      </td>                                                                                             ");
                strBodyContent.AppendLine("      <td align='left' valign='top' style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                strBodyContent.AppendLine(string.Format("${0:0,0.000}", (item.MachiningCost.HasValue ? item.MachiningCost.Value : 0)));
                strBodyContent.AppendLine("      </td>                                                                                             ");
                strBodyContent.AppendLine("      <td align='left' valign='top' style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                strBodyContent.AppendLine(string.Format("${0:0,0.000}", (item.OtherProcessCost.HasValue ? item.OtherProcessCost.Value : 0)));
                strBodyContent.AppendLine("      </td>                                                                                             ");
                strBodyContent.AppendLine("      <td align='left' valign='top' style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                strBodyContent.AppendLine(string.Format("${0:0,0.000}", (item.TotalQuoted.HasValue ? item.TotalQuoted.Value : 0)));
                strBodyContent.AppendLine("      </td>                                                                                             ");
                strBodyContent.AppendLine("      <td align='left' valign='top' style='text-align:right;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(Convert.ToString(item.QuoteWon.HasValue ? item.QuoteWon.Value : 0));
                strBodyContent.AppendLine("      </td>                                                                                             ");
                strBodyContent.AppendLine("</tr>");
            }
            #endregion
            strBodyContent.AppendLine("    </tbody>");
            strBodyContent.AppendLine("</table>");

            using (var stringStream = strBodyContent.ToString().StringToStream())
            {
                filepath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                              , Constants.APQPTEMPORARYFILEFOLDER, "DetailedSupplierReport.xls", stringStream);
            }
            return filepath;
        }
        private string CreateExcelForDetailedSupplierInnerReport(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteSearch> searchCriteria)
        {
            string filepath = string.Empty;
            StringBuilder strBodyContent = new StringBuilder();
            List<DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteSubReportFirstPart> lstRfq = new List<DTO.Library.RFQ.RFQReports.DetailedSupplierQuoteSubReportFirstPart>();
            lstRfq = GetRfqPartQuoteDetailsReport(searchCriteria).Result;

            strBodyContent.AppendLine("<table style='width:100%;' cellspacing='1' border='1' cellpadding='0'>");
            #region Header HTML
            strBodyContent.AppendLine("    <thead>");
            strBodyContent.AppendLine("        <tr>");
            strBodyContent.AppendLine("            <td  align='left' valign='top' style='background:#fff;height:70px; text-align:right;color:#454545;font-size:13px; padding:10px;' colspan='13'>");
            strBodyContent.AppendLine("                <img style='float:left' id='img1' src='" + System.Configuration.ConfigurationManager.AppSettings["ApiURL"].ToString() + Constants.LOGOIMAGEFOLDERINAPI + "MESlogoPdf.png" + "' alt='MES' />");
            strBodyContent.AppendLine("                <font size='2'>");
            strBodyContent.AppendLine(Constants.MESFULLADDRESS);
            strBodyContent.AppendLine("                </font>");
            strBodyContent.AppendLine("            </td>");
            strBodyContent.AppendLine("        </tr>");
            strBodyContent.AppendLine("        <tr style=''>");
            strBodyContent.AppendLine("             <th height='30' style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>RFQ #</th>");
            strBodyContent.AppendLine("             <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Part No.</th>");
            strBodyContent.AppendLine("             <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Description</th>");
            strBodyContent.AppendLine("             <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Annual Quantity</th>");
            strBodyContent.AppendLine("             <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Weight</th>");
            strBodyContent.AppendLine("             <th style='background:#e7e7e7;text-align:left;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Quote Date</th>");
            strBodyContent.AppendLine("             <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Tooling Price</th>");
            strBodyContent.AppendLine("             <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Material Cost</th>");
            strBodyContent.AppendLine("             <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Conversion Cost</th>");
            strBodyContent.AppendLine("             <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Machining Cost</th>");
            strBodyContent.AppendLine("             <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Other Process Cost</th>");
            strBodyContent.AppendLine("             <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Piece Price</th>");
            strBodyContent.AppendLine("             <th style='background:#e7e7e7;text-align:right;color:#e67757;font-size:13px; padding:10px;font-weight:400;'>Supplier Cost/kg</th>");
            strBodyContent.AppendLine("        </tr>");
            strBodyContent.AppendLine("    </thead>");
            #endregion
            strBodyContent.AppendLine("    <tbody>");
            #region main body loop
            foreach (var item in lstRfq)
            {
                strBodyContent.AppendLine("     <tr>");
                strBodyContent.AppendLine("        <td style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                strBodyContent.AppendLine(item.RFQNo ?? "");
                strBodyContent.AppendLine("        </td>");
                strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>");
                strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>");
                foreach (var pItem in item.lstDetailedSupplierQuoteSubReportSecondPart)
                {
                    strBodyContent.AppendLine("               <tr>");
                    strBodyContent.AppendLine("                   <td style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                    strBodyContent.AppendLine(pItem.CustomerPartNo ?? "");
                    strBodyContent.AppendLine("                   </td>");
                    strBodyContent.AppendLine("               </tr>");
                }
                strBodyContent.AppendLine("           </table>");
                strBodyContent.AppendLine("        </td>");
                strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>");
                strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>");
                foreach (var pItem in item.lstDetailedSupplierQuoteSubReportSecondPart)
                {
                    strBodyContent.AppendLine("               <tr>");
                    strBodyContent.AppendLine("                   <td style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                    strBodyContent.AppendLine(pItem.Description + " " + pItem.AddlDescription);
                    strBodyContent.AppendLine("                   </td>");
                    strBodyContent.AppendLine("               </tr>");
                }
                strBodyContent.AppendLine("           </table>");
                strBodyContent.AppendLine("        </td>");
                strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>");
                strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>");
                foreach (var pItem in item.lstDetailedSupplierQuoteSubReportSecondPart)
                {
                    strBodyContent.AppendLine("               <tr>");
                    strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;'>");
                    strBodyContent.AppendLine(Convert.ToString(pItem.EstimatedQty.HasValue ? pItem.EstimatedQty : 0));
                    strBodyContent.AppendLine("                   </td>");
                    strBodyContent.AppendLine("               </tr>");
                }
                strBodyContent.AppendLine("           </table>");
                strBodyContent.AppendLine("        </td>");
                strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:right;color:#454545;font-size:13px; padding:0;'>");
                strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>");
                foreach (var pItem in item.lstDetailedSupplierQuoteSubReportSecondPart)
                {
                    strBodyContent.AppendLine("               <tr>");
                    strBodyContent.AppendLine("                   <td style='text-align:left;color:#454545;font-size:13px; padding:10px;mso-number-format:\\#\\,\\#\\#0\\.000;'>");
                    strBodyContent.AppendLine(string.Format("{0:0.000}", (pItem.PartWeightKG.HasValue ? pItem.PartWeightKG.Value : 0)));
                    strBodyContent.AppendLine("                   </td>");
                    strBodyContent.AppendLine("               </tr>");
                }
                strBodyContent.AppendLine("           </table>");
                strBodyContent.AppendLine("        </td>");
                strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>");
                strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>");
                foreach (var pItem in item.lstDetailedSupplierQuoteSubReportSecondPart)
                {
                    strBodyContent.AppendLine("               <tr>");
                    strBodyContent.AppendLine("                   <td style='text-align:left;color:#454545;font-size:13px; padding:10px;'>");
                    strBodyContent.AppendLine(pItem.QuoteDateString ?? "");
                    strBodyContent.AppendLine("                   </td>");
                    strBodyContent.AppendLine("               </tr>");
                }
                strBodyContent.AppendLine("           </table>");
                strBodyContent.AppendLine("        </td>");
                strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>");
                strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>");
                foreach (var pItem in item.lstDetailedSupplierQuoteSubReportSecondPart)
                {
                    strBodyContent.AppendLine("               <tr>");
                    strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                    strBodyContent.AppendLine(string.Format("${0:0,0.000}", (pItem.ToolingCost.HasValue ? pItem.ToolingCost.Value : 0)));
                    strBodyContent.AppendLine("                   </td>");
                    strBodyContent.AppendLine("               </tr>");
                }
                strBodyContent.AppendLine("           </table>");
                strBodyContent.AppendLine("        </td>");
                strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>");
                strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>");
                foreach (var pItem in item.lstDetailedSupplierQuoteSubReportSecondPart)
                {

                    strBodyContent.AppendLine("               <tr>");
                    strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                    strBodyContent.AppendLine(string.Format("${0:0.000}", (pItem.MaterialCost.HasValue ? pItem.MaterialCost.Value : 0)));
                    strBodyContent.AppendLine("                   </td>");
                    strBodyContent.AppendLine("               </tr>");
                }
                strBodyContent.AppendLine("           </table>");
                strBodyContent.AppendLine("        </td>");
                strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>");
                strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>");
                foreach (var pItem in item.lstDetailedSupplierQuoteSubReportSecondPart)
                {
                    strBodyContent.AppendLine("               <tr>");
                    strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                    strBodyContent.AppendLine(string.Format("${0:0.000}", (pItem.ProcessCost.HasValue ? pItem.ProcessCost.Value : 0)));
                    strBodyContent.AppendLine("                   </td>");
                    strBodyContent.AppendLine("               </tr>");
                }
                strBodyContent.AppendLine("           </table>");
                strBodyContent.AppendLine("        </td>");
                strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>");
                strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>");
                foreach (var pItem in item.lstDetailedSupplierQuoteSubReportSecondPart)
                {
                    strBodyContent.AppendLine("               <tr>");
                    strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                    strBodyContent.AppendLine(string.Format("${0:0.000}", (pItem.MachiningCost.HasValue ? pItem.MachiningCost.Value : 0)));
                    strBodyContent.AppendLine("                   </td>");
                    strBodyContent.AppendLine("               </tr>");
                }

                strBodyContent.AppendLine("           </table>");
                strBodyContent.AppendLine("        </td>");
                strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>");
                strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>");
                foreach (var pItem in item.lstDetailedSupplierQuoteSubReportSecondPart)
                {
                    strBodyContent.AppendLine("               <tr>");
                    strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                    strBodyContent.AppendLine(string.Format("${0:0.000}", (pItem.OtherProcessCost.HasValue ? pItem.OtherProcessCost.Value : 0)));
                    strBodyContent.AppendLine("                   </td>");
                    strBodyContent.AppendLine("               </tr>");
                }
                strBodyContent.AppendLine("           </table>");
                strBodyContent.AppendLine("        </td>");
                strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>");
                strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>");
                foreach (var pItem in item.lstDetailedSupplierQuoteSubReportSecondPart)
                {
                    strBodyContent.AppendLine("               <tr>");
                    strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                    strBodyContent.AppendLine(string.Format("${0:0.000}", pItem.UnitPrice));
                    strBodyContent.AppendLine("                   </td>");
                    strBodyContent.AppendLine("               </tr>");
                }
                strBodyContent.AppendLine("           </table>");
                strBodyContent.AppendLine("        </td>");
                strBodyContent.AppendLine("        <td align='left' valign='top' style='text-align:left;color:#454545;font-size:13px; padding:0;'>");
                strBodyContent.AppendLine("           <table cellspacing='1' border='1' cellpadding='0'>");
                foreach (var pItem in item.lstDetailedSupplierQuoteSubReportSecondPart)
                {
                    strBodyContent.AppendLine("               <tr>");
                    strBodyContent.AppendLine("                   <td style='text-align:right;color:#454545;font-size:13px; padding:10px;mso-number-format:\\$\\#\\,\\#\\#0\\.000;'>");
                    strBodyContent.AppendLine(string.Format("${0:0.000}", (pItem.SupplierCostPerKg.HasValue ? pItem.SupplierCostPerKg.Value : 0)));
                    strBodyContent.AppendLine("                   </td>");
                    strBodyContent.AppendLine("               </tr>");
                }
                strBodyContent.AppendLine("           </table>");
                strBodyContent.AppendLine("        </td>");
                strBodyContent.AppendLine("    </tr>");
            }
            #endregion
            strBodyContent.AppendLine("    </tbody>");
            strBodyContent.AppendLine("</table>");

            using (var stringStream = strBodyContent.ToString().StringToStream())
            {
                filepath = Helper.BlobHelper.UploadFileToBlob(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"])
                              , Constants.APQPTEMPORARYFILEFOLDER, "DetailedSupplierReport_RFQ.xls", stringStream);
            }
            return filepath;
        }
        #endregion

    }
}
