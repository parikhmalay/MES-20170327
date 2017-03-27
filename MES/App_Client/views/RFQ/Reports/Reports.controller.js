app.controller('ReportsCtrl', ['$rootScope', '$scope', 'common', 'RFQReportSvc', '$modal', '$filter', 'LookupSvc', '$timeout', '$parse',
    function ($rootScope, $scope, common, RFQReportSvc, $modal, $filter, LookupSvc, $timeout, $parse) {
        $rootScope.PageHeader = ($filter('translate')('_ReportSectionOne_'));
        $scope.ViewReport = true;
        $scope.ShowRFQPartQuoteDetailGridReport = false;
        $scope.Init = function () {
            $scope.SearchCriteria = {};
            $scope.Paging = GetDefaultPageObject();
            $scope.Paging.Criteria = $scope.SearchCriteria;

            $scope.SearchCriteria.ReportId = '0';
            $scope.SearchCriteria.ReportViewPath = '';
            $scope.SearchCriteria.ReportSearchResultHeading = '';
            $scope.ShowExportButton = false;
            $scope.CopySearchCriteria = {};
        };
        $scope.setDefaultValues = function () {
            $scope.Paging = GetDefaultPageObject();
            $scope.Paging.Criteria = $scope.SearchCriteria;
        };
        $scope.fillDropdowns = function (id) {
            $scope.ShowRFQPartQuoteDetailGridReport = false;
            $scope.CopySearchCriteria = angular.copy($scope.SearchCriteria);
            $scope.SearchCriteria = {
                ReportId: id,
                RFQDateFrom: $scope.CopySearchCriteria.RFQDateFrom,
                RFQDateTo: $scope.CopySearchCriteria.RFQDateTo,
                QuoteDateFrom: $scope.CopySearchCriteria.QuoteDateFrom,
                QuoteDateTo: $scope.CopySearchCriteria.QuoteDateTo,
                QuoteDueDateFrom: $scope.CopySearchCriteria.QuoteDueDateFrom,
                QuoteDueDateTo: $scope.CopySearchCriteria.QuoteDueDateTo,
                RFQSentDateFrom: $scope.CopySearchCriteria.RFQSentDateFrom,
                RFQSentDateTo: $scope.CopySearchCriteria.RFQSentDateTo,
                RFQItems: []
            };      //.ReportId = id; 

            $scope.ShowExportButton = false;
            switch (id) {
                case '1':
                    $scope.SetLookupDataRFQPartsSupplierWiseReport();
                    $scope.setDefaultValues();
                    break;
                case '2':
                    $scope.SetLookupDataRFQPartsSupplierWiseReport();   //lookup is same for RFQ Supplier Parts Quote Report
                    $scope.setDefaultValues();
                    break;
                case '3':
                    $scope.SetLookupDataRFQPartsSupplierWiseReport();
                    $scope.setDefaultValues();
                    break;
                case '4':
                    $scope.SetLookupDataRFQQuotedBySupplierReport();
                    $scope.setDefaultValues();
                case '5':
                    $scope.SetLookupDataRFQSupplierReport();
                    $scope.setDefaultValues();
                    break;
                case '6':
                    $scope.SetLookupDataRFQSupplierActivityLevelReport();
                    $scope.setDefaultValues();
                    break;
                case '7':
                    $scope.SetLookupDataOpenRFQsReport();
                    $scope.setDefaultValues();
                    break;
                case '8':
                    $scope.SetLookupDataRFQPartsSupplierWiseReport();
                    $scope.setDefaultValues();
                    break;
                case '9':
                    $scope.SetLookupDataAnalysisReport();
                    $scope.setDefaultValues();
                    break;
                case '10':
                    $scope.SetLookupDataRFQDetailedSupplierReport();
                    $scope.setDefaultValues();
                    break;
                case '11':
                    $scope.SetLookupDataAnalysisReport(); //only the group by dropdown is different , rest are same so using Analysis report lookup function
                    $scope.setDefaultValues();
                    break;
                case '12':
                    $scope.SetLookupDataAnalysisReport();
                    $scope.setDefaultValues();
                    break;
                case '13':
                    $scope.SetLookupDataQuotesDoneReport();
                    $scope.setDefaultValues();
                    break;
                case '14':
                    $scope.setDefaultValues();
                    break;
                case '15':
                    $scope.setDefaultValues();
                    break;
                case '16':
                    $scope.SetLookupDataAnalysisReport();
                    $scope.setDefaultValues();
                    break;
                default:
                    break;
            }
        };
        $scope.SearchAndLoadReportView = function () {
            $scope.ShowExportButton = false;
            $scope.SearchCriteria.ReportViewPath = '';
            $scope.SearchCriteria.ReportSearchResultHeading = '';
            switch ($scope.SearchCriteria.ReportId) {
                case '1':
                    $scope.GetRFQPartsSupplierWiseReport();
                    $scope.SearchCriteria.ReportViewPath = "/App_Client/views/RFQ/Reports/RFQPartsSupplierWiseReport.html";
                    $scope.SearchCriteria.ReportSearchResultHeading = ($filter('translate')('_RFQQuoteReportBySupplier_'));
                    break;
                case '2':
                    $scope.GetRFQSupplierPartsQuoteReport();
                    $scope.SearchCriteria.ReportViewPath = "/App_Client/views/RFQ/Reports/RFQSupplierPartsQuoteReport.html";
                    $scope.SearchCriteria.ReportSearchResultHeading = ($filter('translate')('_SupplierPartQuoteReport_'));
                    break;
                case '3':
                    $scope.GetRFQPartCostComparisonReport();
                    $scope.SearchCriteria.ReportViewPath = "/App_Client/views/RFQ/Reports/RFQPartsCostComparisonReport.html";
                    $scope.SearchCriteria.ReportSearchResultHeading = ($filter('translate')('_RFQPartsCostComparison_'));
                    break;
                case '4':
                    $scope.GetRFQQuotedBySupplierReport();
                    $scope.SearchCriteria.ReportViewPath = "/App_Client/views/RFQ/Reports/RFQQuotedBySupplierReport.html";
                    $scope.SearchCriteria.ReportSearchResultHeading = ($filter('translate')('_RFQQuotedBySupplierReport_'));
                    break;
                case '5':
                    $scope.GetRFQSupplierReport();
                    $scope.SearchCriteria.ReportViewPath = "/App_Client/views/RFQ/Reports/RFQSupplierReport.html";
                    $scope.SearchCriteria.ReportSearchResultHeading = ($filter('translate')('_RFQSupplierReport_'));
                    break;
                case '6':
                    $scope.GetRFQSupplierActivityLevelReport();
                    $scope.SearchCriteria.ReportViewPath = "/App_Client/views/RFQ/Reports/RFQSupplierActivityLevelReport.html";
                    $scope.SearchCriteria.ReportSearchResultHeading = ($filter('translate')('_RFQSupplierActivityLevelReport_'));
                    break;
                case '7':
                    $scope.GetOpenRFQsReport();
                    $scope.SearchCriteria.ReportViewPath = "/App_Client/views/RFQ/Reports/OpenRFQsReport.html";
                    $scope.SearchCriteria.ReportSearchResultHeading = ($filter('translate')('_OpenRFQsReport_'));
                    break;
                case '8':
                    $scope.GetRFQPartsSupplierWiseReport();
                    $scope.SearchCriteria.ReportViewPath = "/App_Client/views/RFQ/Reports/RFQPartsSupplierWiseReport.html";
                    $scope.SearchCriteria.ReportSearchResultHeading = ($filter('translate')('_RFQQuoteReportBySupplier_'));
                    break;
                case '9':
                    $scope.GetRFQAnalysisReport();
                    $scope.SearchCriteria.ReportViewPath = "/App_Client/views/RFQ/Reports/RFQAnalysisReport.html";
                    $scope.SearchCriteria.ReportSearchResultHeading = ($filter('translate')('_RFQAnalysisReport_'));
                    break;
                case '10':
                    $scope.GetRFQDetailedSupplierReport();
                    $scope.SearchCriteria.ReportViewPath = "/App_Client/views/RFQ/Reports/RFQDetailedSupplierReport.html";
                    $scope.SearchCriteria.ReportSearchResultHeading = ($filter('translate')('_RFQDetailedSupplierReport_'));
                    break;
                case '11':
                    $scope.GetRFQNonAwardReasonReport();
                    $scope.SearchCriteria.ReportViewPath = "/App_Client/views/RFQ/Reports/RFQNonAwardReasonReport.html";
                    $scope.SearchCriteria.ReportSearchResultHeading = ($filter('translate')('_NonAwardReasonReport_'));
                    break;
                case '12':
                    $scope.GetRFQAnalysisReport();
                    $scope.SearchCriteria.ReportViewPath = "/App_Client/views/RFQ/Reports/RFQAnalysisReport.html";
                    $scope.SearchCriteria.ReportSearchResultHeading = ($filter('translate')('_RFQBusinessAnalysisReport_'));
                    break;
                case '13':
                    $scope.GetQuotesDoneReport();
                    $scope.SearchCriteria.ReportViewPath = "/App_Client/views/RFQ/Reports/QuotesDoneReport.html";
                    $scope.SearchCriteria.ReportSearchResultHeading = ($filter('translate')('_QuotesDoneReport_'));
                    break;
                case '14':
                    $scope.GetQuoteTotalDollarQuotedReport();
                    $scope.SearchCriteria.ReportViewPath = "/App_Client/views/RFQ/Reports/TotalDollarQuotedReport.html";
                    $scope.SearchCriteria.ReportSearchResultHeading = ($filter('translate')('_TotalDollarQuoted_'));
                    break;
                case '15':
                    $scope.GetQuoteTotalDollarQuotedReport();
                    $scope.SearchCriteria.ReportViewPath = "/App_Client/views/RFQ/Reports/TotalDollarQuotedReport.html";
                    $scope.SearchCriteria.ReportSearchResultHeading = ($filter('translate')('_TotalDollarQuotedBaseQuotes_'));
                    break;
                case '16':
                    $scope.GetRFQAnalysisReport();
                    $scope.SearchCriteria.ReportViewPath = "/App_Client/views/RFQ/Reports/RFQAnalysisReport.html";
                    $scope.SearchCriteria.ReportSearchResultHeading = ($filter('translate')('_BusinessAnalysisReportQTC_'));
                    break;
                default:
                    $scope.SearchCriteria.ReportViewPath = '';
                    $scope.SearchCriteria.ReportSearchResultHeading = '';
                    break;
            }
        };
        $scope.exportToExcel = function () {
            switch ($scope.SearchCriteria.ReportId) {
                case '1':
                    $scope.exportRFQPartsSupplierWiseReport();
                    break;
                case '2':
                    $scope.exportRFQSupplierPartsQuoteReport();
                    break;
                case '3':
                    $scope.exportRFQPartCostComparisonReport();
                    break;
                case '4':
                    $scope.exportRFQQuotedBySupplierReport();
                    break;
                case '5':
                    $scope.exportRFQSupplierReport();
                    break;
                case '6':
                    $scope.exportRFQSupplierActivityLevelReport();
                    break;
                case '7':
                    $scope.exportOpenRFQsReport();
                    break;
                case '8':
                    $scope.exportRFQPartsSupplierWiseReport();
                    break;
                case '9':
                    $scope.exportRFQAnalysisReport();
                    break;
                case '10':
                    $scope.exportRFQDetailedSupplierReport();
                    break;
                case '11':
                    $scope.exportRFQNonAwardReasonReport();
                    break;
                case '12':
                    $scope.exportRFQAnalysisReport();
                    break;
                case '13':
                    $scope.exportQuotesDoneReport();
                    break;
                case '14':
                    $scope.exportQuoteTotalDollarQuotedReport();
                    break;
                case '15':
                    $scope.exportQuoteTotalDollarQuotedReport();
                    break;
                case '16':
                    $scope.exportRFQAnalysisReport();
                    break;
                default:
                    break;
            }
        };
        $scope.rdoChangeFunction = function () {
            $scope.Init();
        };
        //start common for analysis, business, non-award reason report
        $scope.InitializeHighChart = function () {
            $scope.highchartsNG = {
                chart: {
                    renderTo: 'container'
                },
                title: {
                    enabled: false,
                    text: '',
                },
                tooltip: {
                    pointFormat: '<b>{point.percentage:.1f}%</b>'
                },
                options: {
                    chart: {
                        type: 'pie'
                    }
                },
                colors: ["#00d388", "#3896ff", "#ff4d4d", "#ffc600", "#ac6cff", "#c2c2c2", "#5cd0c9", "#cd6767", "#575e6c", "#ff7048"],
                plotOptions: {
                    pie: {
                        allowPointSelect: false,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: false,
                            format: '{point.percentage:.1f} %',
                            style: {
                                color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                            }
                        },
                        showInLegend: true
                    }
                },
                series: [],
                size: {
                    height: 350
                },
                loading: false,
                legend: {
                    enabled: true
                },
                exporting: {
                    enabled: false
                }
                //series: [{
                //    name: 'Brands',
                //    colorByPoint: true,
                //    data: [{
                //        name: 'Chrome',
                //        y: 24.03,
                //        sliced: true,
                //        selected: true
                //    }, {
                //        name: 'Proprietary or Undetectable',
                //        y: 0.2
                //    }]
                //}]
            }
        };
        $scope.SetValues = function () {
            //Start chart objects 
            $scope.InitializeHighChart();
            //End chart objects 

            $scope.SelectionCriteria = { RFQDateFrom: null, RFQDateTo: null, SAMItems: [], CustomerItems: [], CommodityItems: [], RFQSourceItems: [], RFQTypeItems: [], IndustryTypeItems: [], GroupBy: '', ReportId: '0' };
            common.usSpinnerService.spin('spnReports');
            $scope.SAMIds = [];
            $scope.CustomerIds = [];
            $scope.CommodityIds = [];
            $scope.RFQSourceIds = [];
            $scope.RFQTypeIds = [];
            $scope.IndustryTypeIds = [];
            $scope.SearchCriteria.DateFrom = convertUTCDateToLocalDate($scope.SearchCriteria.RFQDateFrom);
            $scope.SearchCriteria.DateTo = convertUTCDateToLocalDate($scope.SearchCriteria.RFQDateTo);
            $scope.SelectionCriteria.RFQDateFrom = $scope.SearchCriteria.RFQDateFrom;
            $scope.SelectionCriteria.RFQDateTo = $scope.SearchCriteria.RFQDateTo;
            $scope.SelectionCriteria.GroupBy = IsUndefinedNullOrEmpty($scope.SearchCriteria.GroupBy) ? '' : $scope.SearchCriteria.GroupBy;
            $scope.SelectionCriteria.ReportId = IsUndefinedNullOrEmpty($scope.SearchCriteria.ReportId) ? '0' : $scope.SearchCriteria.ReportId;

            angular.forEach($scope.SearchCriteria.SAMItems, function (item) {
                if (!Isundefinedornull(item.Id)) {
                    $scope.SAMIds.push(item.Id);
                    $scope.SelectionCriteria.SAMItems.push(item.Name);
                }
            });
            $scope.SearchCriteria.SAMIds = $scope.SAMIds.join(",");
            $scope.SelectionCriteria.SAMItems = $scope.SelectionCriteria.SAMItems.join(", ");

            angular.forEach($scope.SearchCriteria.CustomerItems, function (item) {
                if (!Isundefinedornull(item.Id) && item.Id > 0) {
                    $scope.CustomerIds.push(item.Id);
                    $scope.SelectionCriteria.CustomerItems.push(item.Name);
                }

            });
            $scope.SearchCriteria.CustomerIds = $scope.CustomerIds.join(",");
            $scope.SelectionCriteria.CustomerItems = $scope.SelectionCriteria.CustomerItems.join(", ");

            angular.forEach($scope.SearchCriteria.CommodityItems, function (item) {
                if (!Isundefinedornull(item.Id) && item.Id > 0) {
                    $scope.CommodityIds.push(item.Id);
                    $scope.SelectionCriteria.CommodityItems.push(item.Name);
                }
            });
            $scope.SearchCriteria.CommodityIds = $scope.CommodityIds.join(",");
            $scope.SelectionCriteria.CommodityItems = $scope.SelectionCriteria.CommodityItems.join(", ");

            angular.forEach($scope.SearchCriteria.RFQSourceItems, function (item) {
                if (!Isundefinedornull(item.Id) && item.Id > 0) {
                    $scope.RFQSourceIds.push(item.Id);
                    $scope.SelectionCriteria.RFQSourceItems.push(item.Name);
                }
            });
            $scope.SearchCriteria.RFQSourceIds = $scope.RFQSourceIds.join(",");
            $scope.SelectionCriteria.RFQSourceItems = $scope.SelectionCriteria.RFQSourceItems.join(", ");

            angular.forEach($scope.SearchCriteria.RFQTypeItems, function (item) {
                if (!Isundefinedornull(item.Id) && item.Id > 0) {
                    $scope.RFQTypeIds.push(item.Id);
                    $scope.SelectionCriteria.RFQTypeItems.push(item.Name);
                }
            });
            $scope.SearchCriteria.RFQTypeIds = $scope.RFQTypeIds.join(",");
            $scope.SelectionCriteria.RFQTypeItems = $scope.SelectionCriteria.RFQTypeItems.join(", ");

            angular.forEach($scope.SearchCriteria.IndustryTypeItems, function (item) {
                if (!Isundefinedornull(item.Id) && item.Id > 0) {
                    $scope.IndustryTypeIds.push(item.Id);
                    $scope.SelectionCriteria.IndustryTypeItems.push(item.Name);
                }
            });
            $scope.SearchCriteria.IndustryTypeIds = $scope.IndustryTypeIds.join(",");
            $scope.SelectionCriteria.IndustryTypeItems = $scope.SelectionCriteria.IndustryTypeItems.join(", ");

            $scope.Paging.Criteria = $scope.SearchCriteria;
        }
        //End common for analysis, business, non-award reason report

        //Start analysis report
        $scope.SetLookupDataAnalysisReport = function () {
            $scope.LookUps = [
               {
                   "Name": "CommodityTypes", "Parameters": {}
               },
               {
                   "Name": "Commodity", "Parameters": {}
               },
               {
                   "Name": "RFQSources", "Parameters": {}
               },
               {
                   "Name": "RFQTypes", "Parameters": {}
               },
               {
                   "Name": "IndustryTypes", "Parameters": {}
               },
               {
                   "Name": "SAM", "Parameters": {}
               },
               {
                   "Name": "Customers", "Parameters": {}
               }
            ];
            $scope.getLookupDataAnalysisReport();
        };
        $scope.getLookupDataAnalysisReport = function () {
            common.usSpinnerService.spin('spnReports');
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                common.usSpinnerService.stop('spnReports');
                angular.forEach(data.data, function (o) {
                    if (o.Name === "CommodityTypes") {
                        $scope.CommodityTypeList = o.Data;
                    }
                    else if (o.Name === "Commodity") {
                        $scope.CommodityList = o.Data;
                    }
                    else if (o.Name === "RFQSources") {
                        $scope.RFQSourceList = o.Data;
                    }
                    else if (o.Name === "RFQTypes") {
                        $scope.RFQTypeList = o.Data;
                    }
                    else if (o.Name === "IndustryTypes") {
                        $scope.IndustryTypeList = o.Data;
                    }
                    else if (o.Name === "SAM") {
                        $scope.SAMList = o.Data;
                    }
                    else if (o.Name === "Customers") {
                        $scope.CustomerList = o.Data;
                    }
                });
            }, function (error) {
                common.usSpinnerService.stop('spnReports');
            });
        }
        $scope.GetRFQAnalysisReport = function () {
            $scope.RFQAnalysisReportList = {};
            $scope.SetValues();
            RFQReportSvc.GetRFQAnalysisReport($scope.Paging).then(
                function (response) {
                    common.usSpinnerService.stop('spnReports');
                    if (response.data.StatusCode == 200) {
                        $scope.RFQAnalysisReportList = response.data.Result;
                        ///set chart values
                        $scope.$broadcast("loadScrollerPanel");
                        var rnd = [];
                        $scope.highchartsNG.series = [];
                        if ($scope.RFQAnalysisReportList.length > 0) {
                            $scope.ShowExportButton = true;
                            angular.forEach($scope.RFQAnalysisReportList[0].lstRFQAnalysisChart, function (o, index) {
                                rnd.push({ name: o.DisplayName, y: o.NoOfRfq });
                            });
                        }
                        $scope.highchartsNG.series.push({
                            name: 'RFQ',
                            colorByPoint: true,
                            data: rnd
                        });
                        ///End set chart values
                        if (!IsUndefinedNullOrEmpty($scope.SearchCriteria.GroupBy) && $scope.RFQAnalysisReportList.length > 0) {
                            angular.forEach($scope.RFQAnalysisReportList, function (item, index) {
                                item.lstRFQAnalysisReportDetails = _.groupBy(item.lstRFQAnalysisReportDetails, 'GroupByValue');
                            });
                        }

                        $scope.Paging = response.data.PageInfo;
                        $scope.Paging.Criteria = $scope.SearchCriteria;
                        $scope.$broadcast("loadScrollerPanel");
                    }
                    else {
                        console.log(response.data.ErrorText);
                    }
                }, function (error) {
                    common.usSpinnerService.stop('spnReports');
                });
        }
        $scope.getGroupByTotal = function (Value) {
            var total = 0;
            angular.forEach(Value, function (item, index) {
                if (!isNaN(parseFloat(item.totalquoted)))
                    total = total + parseFloat(item.totalquoted);
            });
            return total;
        };
        $scope.exportRFQAnalysisReport = function () {
            common.usSpinnerService.spin('spnReports');
            RFQReportSvc.exportRFQAnalysisReport($scope.Paging).then(
               function (response) {
                   common.usSpinnerService.stop('spnReports');
                   if (response.data.StatusCode == 200) {
                       window.open(response.data.SuccessMessage, '_blank');
                   }
                   else {
                       common.aaNotify.error(response.data.ErrorText);
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnReports');
               });
        };
        //End  analysis report

        //start non-award reason report
        $scope.GetRFQNonAwardReasonReport = function () {
            $scope.RFQNonAwardReasonReportList = {};
            $scope.SetValues();
            RFQReportSvc.GetRFQNonAwardReasonReport($scope.Paging).then(
                function (response) {
                    common.usSpinnerService.stop('spnReports');
                    if (response.data.StatusCode == 200) {
                        $scope.RFQNonAwardReasonReportList = response.data.Result;
                        ///set chart values
                        $scope.$broadcast("loadScrollerPanel");
                        var rnd = [];
                        $scope.highchartsNG.series = [];
                        if ($scope.RFQNonAwardReasonReportList.length > 0) {
                            $scope.ShowExportButton = true;
                            angular.forEach($scope.RFQNonAwardReasonReportList[0].lstRFQNonAwardReasonChart, function (o, index) {
                                rnd.push({ name: o.DisplayName, y: o.NoOfRfq });
                            });
                        }
                        $scope.highchartsNG.series.push({
                            name: 'RFQ',
                            colorByPoint: true,
                            data: rnd
                        });
                        ///End set chart values
                        if (!IsUndefinedNullOrEmpty($scope.SearchCriteria.GroupBy) && $scope.RFQNonAwardReasonReportList.length > 0) {
                            angular.forEach($scope.RFQNonAwardReasonReportList, function (item, index) {
                                item.lstRFQNonAwardReasonReportDetails = _.groupBy(item.lstRFQNonAwardReasonReportDetails, 'GroupByValue');
                            });
                        }

                        $scope.Paging = response.data.PageInfo;
                        $scope.Paging.Criteria = $scope.SearchCriteria;
                        $scope.$broadcast("loadScrollerPanel");
                    }
                    else {
                        console.log(response.data.ErrorText);
                    }
                }, function (error) {
                    common.usSpinnerService.stop('spnReports');
                });
        }
        $scope.exportRFQNonAwardReasonReport = function () {
            common.usSpinnerService.spin('spnReports');
            RFQReportSvc.exportRFQNonAwardReasonReport($scope.Paging).then(
               function (response) {
                   common.usSpinnerService.stop('spnReports');
                   if (response.data.StatusCode == 200) {
                       window.open(response.data.SuccessMessage, '_blank');
                   }
                   else {
                       common.aaNotify.error(response.data.ErrorText);
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnReports');
               });
        };
        //End non-award reason report

        //start Quote's Total $ Quoted report
        $scope.GetQuoteTotalDollarQuotedReport = function () {
            common.usSpinnerService.spin('spnReports');
            $scope.QuoteTotalDollarQuotedReportList = {};
            $scope.SelectionCriteria = { QuoteDateFrom: null, QuoteDateTo: null, ReportId: '0' };
            $scope.SearchCriteria.DateFrom = convertUTCDateToLocalDate($scope.SearchCriteria.QuoteDateFrom);
            $scope.SearchCriteria.DateTo = convertUTCDateToLocalDate($scope.SearchCriteria.QuoteDateTo);
            $scope.SelectionCriteria.QuoteDateFrom = $scope.SearchCriteria.QuoteDateFrom;
            $scope.SelectionCriteria.QuoteDateTo = $scope.SearchCriteria.QuoteDateTo;
            $scope.SelectionCriteria.ReportId = IsUndefinedNullOrEmpty($scope.SearchCriteria.ReportId) ? '0' : $scope.SearchCriteria.ReportId;
            RFQReportSvc.GetQuoteTotalDollarQuotedReport($scope.Paging).then(
                function (response) {
                    common.usSpinnerService.stop('spnReports');
                    if (response.data.StatusCode == 200) {
                        $scope.DataList = {};
                        $scope.lstMonthName = {};
                        $scope.CustomerNameList = {};
                        $scope.MonthYearNameList = {};
                        $scope.DataList = response.data.Result;
                        if ($scope.DataList.length > 0) {
                            $scope.ShowExportButton = true;
                            $scope.lstMonthName = $scope.DataList[0].lstMonthName;
                            $scope.CustomerNameList = _.groupBy($scope.DataList, 'Customer');
                            $scope.MonthYearNameList = _.groupBy($scope.DataList, 'MonthYearName');
                            $scope.QuoteTotalDollarQuotedReportList = $scope.DataList;
                        }

                        $scope.Paging = response.data.PageInfo;
                        $scope.Paging.Criteria = $scope.SearchCriteria;
                    }
                    else {
                        console.log(response.data.ErrorText);
                    }
                }, function (error) {
                    common.usSpinnerService.stop('spnReports');
                });
        }
        $scope.exportQuoteTotalDollarQuotedReport = function () {
            common.usSpinnerService.spin('spnReports');
            RFQReportSvc.exportQuoteTotalDollarQuotedReport($scope.Paging).then(
               function (response) {
                   common.usSpinnerService.stop('spnReports');
                   if (response.data.StatusCode == 200) {
                       window.open(response.data.SuccessMessage, '_blank');
                   }
                   else {
                       common.aaNotify.error(response.data.ErrorText);
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnReports');
               });
        };
        $scope.getCustomerWiseTotalQuoted = function (Value) {
            var total = 0;
            angular.forEach(Value, function (item, index) {
                if (!isNaN(parseFloat(item.TotalQuoted)))
                    total = total + parseFloat(item.TotalQuoted);
            });
            return total;
        };
        $scope.getTotalToolingCost = function (monthName) {
            var total = 0;
            angular.forEach($scope.MonthYearNameList, function (item, index) {
                if (index == monthName) {
                    angular.forEach(item, function (obj, index) {
                        if (!isNaN(parseFloat(obj.ToolingCost)))
                            total = total + parseFloat(obj.ToolingCost);
                    });
                }
            });
            return total;
        };
        $scope.getTotalAnnualCost = function (monthName) {
            var total = 0;
            angular.forEach($scope.MonthYearNameList, function (item, index) {
                if (index == monthName) {
                    angular.forEach(item, function (obj, index) {
                        if (!isNaN(parseFloat(obj.TotalAnnualCost)))
                            total = total + parseFloat(obj.TotalAnnualCost);
                    });
                }
            });
            return total;
        };
        $scope.getTotalQuoted = function (monthName) {
            var total = 0;
            angular.forEach($scope.MonthYearNameList, function (item, index) {
                if (index == monthName) {
                    angular.forEach(item, function (obj, index) {
                        if (!isNaN(parseFloat(obj.TotalQuoted)))
                            total = total + parseFloat(obj.TotalQuoted);
                    });
                }
            });
            return total;
        };
        $scope.getFinalTotal = function () {
            var total = 0;
            var rowObject = $filter('filter')($scope.QuoteTotalDollarQuotedReportList, function (rw) { return rw.TotalQuoted > 0 });
            angular.forEach(rowObject, function (obj, index) {
                if (!isNaN(parseFloat(obj.TotalQuoted)))
                    total = total + parseFloat(obj.TotalQuoted);
            });
            return total;
        };
        //End Quote's Total $ Quoted report

        //  start Quotes done Report
        $scope.SetLookupDataQuotesDoneReport = function () {
            $scope.LookUps = [
               {
                   "Name": "SAM", "Parameters": {}
               },
                {
                    "Name": "Customers", "Parameters": {}
                }
            ];
            $scope.getLookupDataQuotesDoneReport();
        };
        $scope.getLookupDataQuotesDoneReport = function () {
            common.usSpinnerService.spin('spnReports');
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                common.usSpinnerService.stop('spnReports');
                angular.forEach(data.data, function (o) {
                    if (o.Name === "SAM") {
                        $scope.SAMList = o.Data;
                    }
                    else if (o.Name === "Customers") {
                        $scope.CustomerList = o.Data;
                    }
                });
            }, function (error) {
                common.usSpinnerService.stop('spnReports');
            });
        }
        $scope.GetQuotesDoneReport = function () {
            common.usSpinnerService.spin('spnReports');
            $scope.QuotesDoneReportList = {};
            $scope.DataList = {};
            $scope.InitializeHighChartForQuoteDone();
            $scope.SAMIds = [];
            $scope.CustomerIds = [];
            $scope.SelectionCriteria = { QuoteDateFrom: null, QuoteDateTo: null, SAMItems: [], CustomerItems: [], GroupBy: '', ReportId: '0' };
            $scope.SearchCriteria.DateFrom = convertUTCDateToLocalDate($scope.SearchCriteria.QuoteDateFrom);
            $scope.SearchCriteria.DateTo = convertUTCDateToLocalDate($scope.SearchCriteria.QuoteDateTo);
            $scope.SelectionCriteria.QuoteDateFrom = $scope.SearchCriteria.QuoteDateFrom;
            $scope.SelectionCriteria.QuoteDateTo = $scope.SearchCriteria.QuoteDateTo;
            $scope.SelectionCriteria.GroupBy = IsUndefinedNullOrEmpty($scope.SearchCriteria.GroupBy) ? '' : $scope.SearchCriteria.GroupBy;
            $scope.SelectionCriteria.ReportId = IsUndefinedNullOrEmpty($scope.SearchCriteria.ReportId) ? '0' : $scope.SearchCriteria.ReportId;

            angular.forEach($scope.SearchCriteria.SAMItems, function (item) {
                if (!Isundefinedornull(item.Id)) {
                    $scope.SAMIds.push(item.Id);
                    $scope.SelectionCriteria.SAMItems.push(item.Name);
                }
            });
            $scope.SearchCriteria.SAMIds = $scope.SAMIds.join(",");
            $scope.SelectionCriteria.SAMItems = $scope.SelectionCriteria.SAMItems.join(", ");

            angular.forEach($scope.SearchCriteria.CustomerItems, function (item) {
                if (!Isundefinedornull(item.Id) && item.Id > 0) {
                    $scope.CustomerIds.push(item.Id);
                    $scope.SelectionCriteria.CustomerItems.push(item.Name);
                }
            });
            $scope.SearchCriteria.CustomerIds = $scope.CustomerIds.join(",");
            $scope.SelectionCriteria.CustomerItems = $scope.SelectionCriteria.CustomerItems.join(", ");

            $scope.Paging.Criteria = $scope.SearchCriteria;
            RFQReportSvc.GetQuotesDoneReport($scope.Paging).then(
                function (response) {
                    common.usSpinnerService.stop('spnReports');
                    if (response.data.StatusCode == 200) {
                        $scope.DataList = response.data.Result;
                        var rnd = [];
                        $scope.highchartsNG.series = [];
                        if ($scope.DataList.length > 0) {
                            $scope.ShowExportButton = true;
                            angular.forEach($scope.DataList, function (item) {
                                item.QuoteDate = convertUTCDateToLocalDate(item.QuoteDate);
                            });
                            angular.forEach($scope.DataList[0].lstQuotesDoneChart, function (o, index) {
                                rnd.push({ name: o.DisplayName, y: o.Amount });
                            });
                        }
                        $scope.highchartsNG.series.push({
                            name: 'Quote',
                            colorByPoint: true,
                            data: rnd
                        });
                        if (!IsUndefinedNullOrEmpty($scope.SearchCriteria.GroupBy) && $scope.DataList.length > 0) {
                            $scope.QuotesDoneReportList = _.groupBy($scope.DataList, 'GroupByValue');
                        }
                        else
                            $scope.QuotesDoneReportList = $scope.DataList;
                        $scope.Paging = response.data.PageInfo;
                        $scope.Paging.Criteria = $scope.SearchCriteria;
                    }
                    else {
                        console.log(response.data.ErrorText);
                    }
                }, function (error) {
                    common.usSpinnerService.stop('spnReports');
                });
        }
        $scope.exportQuotesDoneReport = function () {
            common.usSpinnerService.spin('spnReports');
            RFQReportSvc.exportQuotesDoneReport($scope.Paging).then(
               function (response) {
                   common.usSpinnerService.stop('spnReports');
                   if (response.data.StatusCode == 200) {
                       window.open(response.data.SuccessMessage, '_blank');
                   }
                   else {
                       common.aaNotify.error(response.data.ErrorText);
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnReports');
               });
        };
        $scope.getGroupByTotalForQuoteDone = function (Value) {
            var total = 0;
            angular.forEach(Value, function (item, index) {
                if (!isNaN(parseFloat(item.Amount)))
                    total = total + parseFloat(item.Amount);
            });
            return total;
        };
        $scope.getTotalForQuoteDone = function () {
            var total = 0;
            angular.forEach($scope.DataList, function (item, index) {
                if (!isNaN(parseFloat(item.Amount)))
                    total = total + parseFloat(item.Amount);
            });
            return total;
        };
        $scope.InitializeHighChartForQuoteDone = function () {
            $scope.highchartsNG = {
                chart: {
                    renderTo: 'container'
                },
                title: {
                    enabled: false,
                    text: '',
                },
                tooltip: {
                    pointFormat: '<b>${point.y:.3f}</b>'
                },
                options: {
                    chart: {
                        type: 'pie'
                    }
                },
                colors: ["#00d388", "#3896ff", "#ff4d4d", "#ffc600", "#ac6cff", "#c2c2c2", "#5cd0c9", "#cd6767", "#575e6c", "#ff7048"],
                plotOptions: {
                    pie: {
                        allowPointSelect: false,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: false,
                            format: '{point.percentage:.1f} %',
                            style: {
                                color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                            }
                        },
                        showInLegend: true
                    }
                },
                series: [],
                size: {
                    height: 350
                },
                loading: false,
                legend: {
                    enabled: true
                },
                exporting: {
                    enabled: false
                }
            }
        };
        // End Quotes done Report

        //  start RFQ Quote Report by Supplier
        $scope.SetLookupDataRFQPartsSupplierWiseReport = function () {
            $scope.LookUps = [
               {
                   "Name": "RFQForSupplierQuote", "Parameters": { "CustomerId": 0 }
               }
            ];
            $scope.getLookupDataRFQPartsSupplierWiseReport();
        };
        $scope.getLookupDataRFQPartsSupplierWiseReport = function () {
            common.usSpinnerService.spin('spnReports');
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                common.usSpinnerService.stop('spnReports');
                angular.forEach(data.data, function (o) {
                    if (o.Name === "RFQForSupplierQuote") {
                        $scope.RFQList = o.Data;
                    }
                });
            }, function (error) {
                common.usSpinnerService.stop('spnReports');
            });
        }
        $scope.GetRFQPartsSupplierWiseReport = function () {
            common.usSpinnerService.spin('spnReports');
            $scope.RFQPartsSupplierWiseReportList = {};
            $scope.DataList = {};
            $scope.RFQIds = [];
            $scope.SelectionCriteria = { RFQItems: [], ReportId: '0' };
            $scope.SelectionCriteria.ReportId = IsUndefinedNullOrEmpty($scope.SearchCriteria.ReportId) ? '0' : $scope.SearchCriteria.ReportId;

            angular.forEach($scope.SearchCriteria.RFQItems, function (item) {
                if (!IsUndefinedNullOrEmpty(item.Id)) {
                    $scope.RFQIds.push(item.Id);
                    $scope.SelectionCriteria.RFQItems.push(item.Name);
                }
            });
            $scope.SearchCriteria.RFQIds = $scope.RFQIds.join(",");
            $scope.SelectionCriteria.RFQItems = $scope.SelectionCriteria.RFQItems.join(", ");
            if ($scope.RFQIds.length <= 0) {
                alert($filter('translate')('_SelectRFQ_'));
                //alert('Please select RFQ.');
                common.usSpinnerService.stop('spnReports');
                return false;
            }
            else if ($scope.RFQIds.length > 100) {
                alert($filter('translate')('_SelectedRFQExceededLimit_'));
                //alert('If you are seeing this message, it means that the no. of items selected are much more than the report could process. Please de-select some and try again.');
                common.usSpinnerService.stop('spnReports');
                return false;
            }
            $scope.Paging.Criteria = $scope.SearchCriteria;
            RFQReportSvc.GetRFQPartsSupplierWiseReport($scope.Paging).then(
                function (response) {
                    common.usSpinnerService.stop('spnReports');
                    if (response.data.StatusCode == 200) {
                        $scope.DataList = response.data.Result;
                        if ($scope.DataList.length > 0) {
                            $scope.ShowExportButton = true;
                        }
                        $scope.RFQPartsSupplierWiseReportList = $scope.DataList;
                        $scope.Paging = response.data.PageInfo;
                        $scope.Paging.Criteria = $scope.SearchCriteria;
                    }
                    else {
                        console.log(response.data.ErrorText);
                    }
                }, function (error) {
                    common.usSpinnerService.stop('spnReports');
                });
        }
        $scope.exportRFQPartsSupplierWiseReport = function () {
            common.usSpinnerService.spin('spnReports');
            RFQReportSvc.exportRFQPartsSupplierWiseReport($scope.Paging).then(
               function (response) {
                   common.usSpinnerService.stop('spnReports');
                   if (response.data.StatusCode == 200) {
                       window.open(response.data.SuccessMessage, '_blank');
                   }
                   else {
                       common.aaNotify.error(response.data.ErrorText);
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnReports');
               });
        };
        // End RFQ Quote Report by Supplier

        //  start Supplier Parts Quote Report
        $scope.GetRFQSupplierPartsQuoteReport = function () {
            common.usSpinnerService.spin('spnReports');
            $scope.RFQSupplierPartsQuoteReportList = {};
            $scope.DataList = {};
            $scope.RFQIds = [];
            $scope.SelectionCriteria = { RFQItems: [], ReportId: '0' };
            $scope.SelectionCriteria.ReportId = IsUndefinedNullOrEmpty($scope.SearchCriteria.ReportId) ? '0' : $scope.SearchCriteria.ReportId;

            angular.forEach($scope.SearchCriteria.RFQItems, function (item) {
                if (!IsUndefinedNullOrEmpty(item.Id)) {
                    $scope.RFQIds.push(item.Id);
                    $scope.SelectionCriteria.RFQItems.push(item.Name);
                }
            });
            $scope.SearchCriteria.RFQIds = $scope.RFQIds.join(",");
            $scope.SelectionCriteria.RFQItems = $scope.SelectionCriteria.RFQItems.join(", ");
            if ($scope.RFQIds.length <= 0) {
                alert($filter('translate')('_SelectRFQ_'));
                //alert('Please select RFQ.');
                common.usSpinnerService.stop('spnReports');
                return false;
            }
            else if ($scope.RFQIds.length > 100) {
                alert($filter('translate')('_SelectedRFQExceededLimit_'));
                //alert('If you are seeing this message, it means that the no. of items selected are much more than the report could process. Please de-select some and try again.');
                common.usSpinnerService.stop('spnReports');
                return false;
            }
            $scope.Paging.Criteria = $scope.SearchCriteria;
            RFQReportSvc.GetRFQSupplierPartsQuoteReport($scope.Paging).then(
                function (response) {
                    common.usSpinnerService.stop('spnReports');
                    if (response.data.StatusCode == 200) {
                        $scope.DataList = response.data.Result;
                        if ($scope.DataList.length > 0) {
                            $scope.ShowExportButton = true;
                        }
                        $scope.RFQSupplierPartsQuoteReportList = $scope.DataList;
                        $scope.Paging = response.data.PageInfo;
                        $scope.Paging.Criteria = $scope.SearchCriteria;
                    }
                    else {
                        console.log(response.data.ErrorText);
                    }
                }, function (error) {
                    common.usSpinnerService.stop('spnReports');
                });
        }
        $scope.exportRFQSupplierPartsQuoteReport = function () {
            common.usSpinnerService.spin('spnReports');
            RFQReportSvc.exportRFQSupplierPartsQuoteReport($scope.Paging).then(
               function (response) {
                   common.usSpinnerService.stop('spnReports');
                   if (response.data.StatusCode == 200) {
                       window.open(response.data.SuccessMessage, '_blank');
                   }
                   else {
                       common.aaNotify.error(response.data.ErrorText);
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnReports');
               });
        };
        // End Supplier Parts Quote Report

        //  start RFQPartCostComparison Report
        $scope.GetRFQPartCostComparisonReport = function () {
            common.usSpinnerService.spin('spnReports');
            $scope.RFQPartCostComparisonReportList = {};
            $scope.DataList = {};
            $scope.lstRFQ = [];
            $scope.RFQIds = [];
            $scope.SelectionCriteria = { RFQItems: [], ReportId: '0' };
            $scope.SelectionCriteria.ReportId = IsUndefinedNullOrEmpty($scope.SearchCriteria.ReportId) ? '0' : $scope.SearchCriteria.ReportId;

            angular.forEach($scope.SearchCriteria.RFQItems, function (item) {
                if (!IsUndefinedNullOrEmpty(item.Id)) {
                    $scope.RFQIds.push(item.Id);
                    $scope.SelectionCriteria.RFQItems.push(item.Name);
                }
            });
            $scope.SearchCriteria.RFQIds = $scope.RFQIds.join(",");
            $scope.SelectionCriteria.RFQItems = $scope.SelectionCriteria.RFQItems.join(", ");
            if ($scope.RFQIds.length <= 0) {
                alert($filter('translate')('_SelectRFQ_'));
                //alert('Please select RFQ.');
                common.usSpinnerService.stop('spnReports');
                return false;
            }
            else if ($scope.RFQIds.length > 100) {
                alert($filter('translate')('_SelectedRFQExceededLimit_'));
                //alert('If you are seeing this message, it means that the no. of items selected are much more than the report could process. Please de-select some and try again.');
                common.usSpinnerService.stop('spnReports');
                return false;
            }
            $scope.Paging.Criteria = $scope.SearchCriteria;
            RFQReportSvc.GetRFQPartCostComparisonReport($scope.Paging).then(
                function (response) {
                    common.usSpinnerService.stop('spnReports');
                    if (response.data.StatusCode == 200) {
                        $scope.DataList = response.data.Result;
                        if ($scope.DataList.length > 0) {
                            $scope.ShowExportButton = true;
                        }
                        $scope.lstRFQ = $scope.DataList;

                        angular.forEach($scope.lstRFQ, function (n) {
                            angular.forEach(n.lstRFQPart, function (o) {
                                angular.forEach(o.lstRFQPartCostComparison, function (p) {
                                    p.UpdatedDate = convertUTCDateToLocalDate(p.UpdatedDate);
                                });
                            });
                        });

                        $scope.Paging = response.data.PageInfo;
                        $scope.Paging.Criteria = $scope.SearchCriteria;
                    }
                    else {
                        console.log(response.data.ErrorText);
                    }
                }, function (error) {
                    common.usSpinnerService.stop('spnReports');
                });
        }
        $scope.exportRFQPartCostComparisonReport = function () {
            common.usSpinnerService.spin('spnReports');
            RFQReportSvc.exportRFQPartCostComparisonReport($scope.Paging).then(
               function (response) {
                   common.usSpinnerService.stop('spnReports');
                   if (response.data.StatusCode == 200) {
                       window.open(response.data.SuccessMessage, '_blank');
                   }
                   else {
                       common.aaNotify.error(response.data.ErrorText);
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnReports');
               });
        };
        // End RFQPartCostComparison Report

        //  start RFQs Quoted By Supplier
        $scope.SetLookupDataRFQQuotedBySupplierReport = function () {
            $scope.LookUps = [
                   {
                       "Name": "SupplierItems", "Parameters": {}
                   },
                   {
                       "Name": "SQItems", "Parameters": { "SupplierIds": '' }
                   },
                   {
                       "Name": "CountryItems", "Parameters": { "SupplierIds": '', "SQIds": '' }
                   }
            ];
            $scope.getLookupDataRFQQuotedBySupplierReport();
        };
        $scope.getLookupDataRFQQuotedBySupplierReport = function () {
            common.usSpinnerService.spin('spnReports');
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                common.usSpinnerService.stop('spnReports');
                angular.forEach(data.data, function (o) {
                    if (o.Name === "SupplierItems") {
                        $scope.SupplierItemsList = o.Data;
                    }
                    else if (o.Name === "SQItems") {
                        $scope.SQList = o.Data;
                    }
                    else if (o.Name === "CountryItems") {
                        $scope.CountryList = o.Data;
                    }
                });
            }, function (error) {
                common.usSpinnerService.stop('spnReports');
            });
        }
        $scope.GetRFQQuotedBySupplierReport = function () {
            common.usSpinnerService.spin('spnReports');
            $scope.RFQQuotedBySupplierReportList = {};
            $scope.DataList = {};
            $scope.SupplierIds = [];
            $scope.SQIds = [];
            $scope.CountryIds = [];
            $scope.SelectionCriteria = { RFQDateFrom: null, RFQDateTo: null, SupplierItems: [], SQItems: [], CountryItems: [], ReportId: '0' };
            $scope.SearchCriteria.DateFrom = convertUTCDateToLocalDate($scope.SearchCriteria.RFQDateFrom);
            $scope.SearchCriteria.DateTo = convertUTCDateToLocalDate($scope.SearchCriteria.RFQDateTo);
            $scope.SelectionCriteria.RFQDateFrom = $scope.SearchCriteria.RFQDateFrom;
            $scope.SelectionCriteria.RFQDateTo = $scope.SearchCriteria.RFQDateTo;
            $scope.SelectionCriteria.ReportId = IsUndefinedNullOrEmpty($scope.SearchCriteria.ReportId) ? '0' : $scope.SearchCriteria.ReportId;

            angular.forEach($scope.SearchCriteria.SupplierItems, function (item) {
                if (!IsUndefinedNullOrEmpty(item.Id) && item.Id > 0) {
                    $scope.SupplierIds.push(item.Id);
                    $scope.SelectionCriteria.SupplierItems.push(item.Name);
                }
            });
            $scope.SearchCriteria.SupplierIds = $scope.SupplierIds.join(",");
            $scope.SelectionCriteria.SupplierItems = $scope.SelectionCriteria.SupplierItems.join(", ");
            angular.forEach($scope.SearchCriteria.SQItems, function (item) {
                if (!IsUndefinedNullOrEmpty(item.Id)) {
                    $scope.SQIds.push(item.Id);
                    $scope.SelectionCriteria.SQItems.push(item.Name);
                }
            });
            $scope.SearchCriteria.SQIds = $scope.SQIds.join(",");
            $scope.SelectionCriteria.SQItems = $scope.SelectionCriteria.SQItems.join(", ");
            angular.forEach($scope.SearchCriteria.CountryItems, function (item) {
                if (!IsUndefinedNullOrEmpty(item.Id)) {
                    $scope.CountryIds.push(item.Id);
                    $scope.SelectionCriteria.CountryItems.push(item.Name);
                }
            });
            $scope.SearchCriteria.CountryIds = $scope.CountryIds.join(",");
            $scope.SelectionCriteria.CountryItems = $scope.SelectionCriteria.CountryItems.join(", ");

            $scope.Paging.Criteria = $scope.SearchCriteria;
            RFQReportSvc.GetRFQQuotedBySupplierReport($scope.Paging).then(
                function (response) {
                    common.usSpinnerService.stop('spnReports');
                    if (response.data.StatusCode == 200) {
                        $scope.DataList = response.data.Result;
                        if ($scope.DataList.length > 0) {
                            $scope.ShowExportButton = true;
                        }
                        $scope.RFQQuotedBySupplierReportList = $scope.DataList;
                        $scope.Paging = response.data.PageInfo;
                        $scope.Paging.Criteria = $scope.SearchCriteria;
                    }
                    else {
                        console.log(response.data.ErrorText);
                    }
                }, function (error) {
                    common.usSpinnerService.stop('spnReports');
                });
        }
        $scope.exportRFQQuotedBySupplierReport = function () {
            common.usSpinnerService.spin('spnReports');
            RFQReportSvc.exportRFQQuotedBySupplierReport($scope.Paging).then(
               function (response) {
                   common.usSpinnerService.stop('spnReports');
                   if (response.data.StatusCode == 200) {
                       window.open(response.data.SuccessMessage, '_blank');
                   }
                   else {
                       common.aaNotify.error(response.data.ErrorText);
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnReports');
               });
        };
        $scope.getSQANDCountryBySuppliers = function () {
            $scope.SearchCriteria.SQItems = [];
            $scope.SearchCriteria.CountryItems = [];
            var Supids = [], SupIdsString = "";
            angular.forEach($scope.SearchCriteria.SupplierItems, function (item) {
                if (!IsUndefinedNullOrEmpty(item.Id) && item.Id > 0) {
                    Supids.push(item.Id);
                }
            });
            SupIdsString = Supids.join(",");
            $scope.LookUps = [
              {
                  "Name": "SQItems", "Parameters": { "SupplierIds": SupIdsString }
              },
                  {
                      "Name": "CountryItems", "Parameters": { "SupplierIds": SupIdsString, "SQIds": '' }
                  }
            ];
            common.usSpinnerService.spin('spnReports');
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                common.usSpinnerService.stop('spnReports');
                angular.forEach(data.data, function (o) {
                    if (o.Name === "SQItems") {
                        $scope.SQList = o.Data;
                    }
                    else if (o.Name === "CountryItems") {
                        $scope.CountryList = o.Data;
                    }
                });
            }, function (error) {
                common.usSpinnerService.stop('spnReports');
            });
        };
        // End RFQs Quoted By Supplier

        //  start RFQ Supplier List Report
        $scope.SetLookupDataRFQSupplierReport = function () {
            $scope.LookUps = [
                   {
                       "Name": "SQItems", "Parameters": { "SupplierIds": '' }
                   },
                   {
                       "Name": "CountryItems", "Parameters": { "SupplierIds": '', "SQIds": '' }
                   }
            ];
            $scope.getLookupDataRFQSupplierReport();
        };
        $scope.getLookupDataRFQSupplierReport = function () {
            common.usSpinnerService.spin('spnReports');
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                common.usSpinnerService.stop('spnReports');
                angular.forEach(data.data, function (o) {
                    if (o.Name === "SQItems") {
                        $scope.SQList = o.Data;
                    }
                    else if (o.Name === "CountryItems") {
                        $scope.CountryList = o.Data;
                    }
                });
            }, function (error) {
                common.usSpinnerService.stop('spnReports');
            });
        }
        $scope.GetRFQSupplierReport = function () {
            common.usSpinnerService.spin('spnReports');
            $scope.RFQSupplierReportList = {};
            $scope.DataList = {};
            $scope.SQIds = [];
            $scope.CountryIds = [];
            $scope.SelectionCriteria = { SQItems: [], CountryItems: [], ReportId: '0' };
            $scope.SelectionCriteria.ReportId = IsUndefinedNullOrEmpty($scope.SearchCriteria.ReportId) ? '0' : $scope.SearchCriteria.ReportId;

            angular.forEach($scope.SearchCriteria.SQItems, function (item) {
                if (!IsUndefinedNullOrEmpty(item.Id)) {
                    $scope.SQIds.push(item.Id);
                    $scope.SelectionCriteria.SQItems.push(item.Name);
                }
            });
            $scope.SearchCriteria.SQIds = $scope.SQIds.join(",");
            $scope.SelectionCriteria.SQItems = $scope.SelectionCriteria.SQItems.join(", ");
            angular.forEach($scope.SearchCriteria.CountryItems, function (item) {
                if (!IsUndefinedNullOrEmpty(item.Id)) {
                    $scope.CountryIds.push(item.Id);
                    $scope.SelectionCriteria.CountryItems.push(item.Name);
                }
            });
            $scope.SearchCriteria.CountryIds = $scope.CountryIds.join(",");
            $scope.SelectionCriteria.CountryItems = $scope.SelectionCriteria.CountryItems.join(", ");

            $scope.Paging.Criteria = $scope.SearchCriteria;
            RFQReportSvc.GetRFQSupplierReport($scope.Paging).then(
                function (response) {
                    common.usSpinnerService.stop('spnReports');
                    if (response.data.StatusCode == 200) {
                        $scope.DataList = response.data.Result;
                        if ($scope.DataList.length > 0) {
                            $scope.ShowExportButton = true;
                        }
                        $scope.RFQSupplierReportList = $scope.DataList;
                        $scope.Paging = response.data.PageInfo;
                        $scope.Paging.Criteria = $scope.SearchCriteria;
                    }
                    else {
                        console.log(response.data.ErrorText);
                    }
                }, function (error) {
                    common.usSpinnerService.stop('spnReports');
                });
        }
        $scope.exportRFQSupplierReport = function () {
            common.usSpinnerService.spin('spnReports');
            RFQReportSvc.exportRFQSupplierReport($scope.Paging).then(
               function (response) {
                   common.usSpinnerService.stop('spnReports');
                   if (response.data.StatusCode == 200) {
                       window.open(response.data.SuccessMessage, '_blank');
                   }
                   else {
                       common.aaNotify.error(response.data.ErrorText);
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnReports');
               });
        };
        $scope.getCountryBySQs = function () {
            $scope.SearchCriteria.CountryItems = [];
            var SQIds = [], SQIdsString = "";
            angular.forEach($scope.SearchCriteria.SQItems, function (item) {
                if (!IsUndefinedNullOrEmpty(item.Id)) {
                    SQIds.push(item.Id);
                }
            });
            SQIdsString = SQIds.join(",");
            $scope.LookUps = [
              {
                  "Name": "CountryItems", "Parameters": { "SupplierIds": '', "SQIds": SQIdsString }
              }
            ];
            common.usSpinnerService.spin('spnReports');
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                common.usSpinnerService.stop('spnReports');
                angular.forEach(data.data, function (o) {
                    if (o.Name === "CountryItems") {
                        $scope.CountryList = o.Data;
                    }
                });
            }, function (error) {
                common.usSpinnerService.stop('spnReports');
            });
        };
        // End RFQ Supplier List Report

        //  start RFQ Supplier Activity Level Report
        $scope.SetLookupDataRFQSupplierActivityLevelReport = function () {
            $scope.LookUps = [
                    {
                        "Name": "SupplierItems", "Parameters": {}
                    },
                   {
                       "Name": "SQItems", "Parameters": { "SupplierIds": '' }
                   },
                   {
                       "Name": "CountryItems", "Parameters": { "SupplierIds": '', "SQIds": '' }
                   },
                   {
                       "Name": "CommodityTypesSALR", "Parameters": { "SupplierIds": '' }
                   }
            ];
            $scope.getLookupDataRFQSupplierActivityLevelReport();
        };
        $scope.getLookupDataRFQSupplierActivityLevelReport = function () {
            common.usSpinnerService.spin('spnReports');
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                common.usSpinnerService.stop('spnReports');
                angular.forEach(data.data, function (o) {
                    if (o.Name === "SupplierItems") {
                        $scope.SupplierItemsList = o.Data;
                    }
                    else if (o.Name === "SQItems") {
                        $scope.SQList = o.Data;
                    }
                    else if (o.Name === "CountryItems") {
                        $scope.CountryList = o.Data;
                    }
                    else if (o.Name === "CommodityTypesSALR") {
                        $scope.CommodityTypeList = o.Data;
                    }
                });
            }, function (error) {
                common.usSpinnerService.stop('spnReports');
            });
        }
        $scope.GetRFQSupplierActivityLevelReport = function () {
            common.usSpinnerService.spin('spnReports');
            $scope.RFQSupplierActivityLevelReportList = {};
            $scope.DataList = {};
            $scope.SupplierIds = [];
            $scope.SQIds = [];
            $scope.CountryIds = [];
            $scope.CommodityIds = [];
            $scope.SelectionCriteria = { SupplierItems: [], SQItems: [], CountryItems: [], CommodityItems: [], ReportId: '0' };
            $scope.SelectionCriteria.ReportId = IsUndefinedNullOrEmpty($scope.SearchCriteria.ReportId) ? '0' : $scope.SearchCriteria.ReportId;

            angular.forEach($scope.SearchCriteria.SupplierItems, function (item) {
                if (!IsUndefinedNullOrEmpty(item.Id) && item.Id > 0) {
                    $scope.SupplierIds.push(item.Id);
                    $scope.SelectionCriteria.SupplierItems.push(item.Name);
                }
            });
            $scope.SearchCriteria.SupplierIds = $scope.SupplierIds.join(",");
            $scope.SelectionCriteria.SupplierItems = $scope.SelectionCriteria.SupplierItems.join(", ");
            angular.forEach($scope.SearchCriteria.SQItems, function (item) {
                if (!IsUndefinedNullOrEmpty(item.Id)) {
                    $scope.SQIds.push(item.Id);
                    $scope.SelectionCriteria.SQItems.push(item.Name);
                }
            });
            $scope.SearchCriteria.SQIds = $scope.SQIds.join(",");
            $scope.SelectionCriteria.SQItems = $scope.SelectionCriteria.SQItems.join(", ");
            angular.forEach($scope.SearchCriteria.CountryItems, function (item) {
                if (!IsUndefinedNullOrEmpty(item.Id)) {
                    $scope.CountryIds.push(item.Id);
                    $scope.SelectionCriteria.CountryItems.push(item.Name);
                }
            });
            $scope.SearchCriteria.CountryIds = $scope.CountryIds.join(",");
            $scope.SelectionCriteria.CountryItems = $scope.SelectionCriteria.CountryItems.join(", ");
            angular.forEach($scope.SearchCriteria.CommodityItems, function (item) {
                if (!Isundefinedornull(item.Id) && item.Id > 0) {
                    $scope.CommodityIds.push(item.Id);
                    $scope.SelectionCriteria.CommodityItems.push(item.Name);
                }
            });
            $scope.SearchCriteria.CommodityIds = $scope.CommodityIds.join(",");
            $scope.SelectionCriteria.CommodityItems = $scope.SelectionCriteria.CommodityItems.join(", ");

            $scope.Paging.Criteria = $scope.SearchCriteria;
            RFQReportSvc.GetRFQSupplierActivityLevelReport($scope.Paging).then(
                function (response) {
                    common.usSpinnerService.stop('spnReports');
                    if (response.data.StatusCode == 200) {
                        $scope.DataList = response.data.Result;
                        if ($scope.DataList.length > 0) {
                            $scope.ShowExportButton = true;
                        }
                        $scope.RFQSupplierActivityLevelReportList = $scope.DataList;
                        $scope.Paging = response.data.PageInfo;
                        $scope.Paging.Criteria = $scope.SearchCriteria;
                    }
                    else {
                        console.log(response.data.ErrorText);
                    }
                }, function (error) {
                    common.usSpinnerService.stop('spnReports');
                });
        }
        $scope.exportRFQSupplierActivityLevelReport = function () {
            common.usSpinnerService.spin('spnReports');
            RFQReportSvc.exportRFQSupplierActivityLevelReport($scope.Paging).then(
               function (response) {
                   common.usSpinnerService.stop('spnReports');
                   if (response.data.StatusCode == 200) {
                       window.open(response.data.SuccessMessage, '_blank');
                   }
                   else {
                       common.aaNotify.error(response.data.ErrorText);
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnReports');
               });
        };
        $scope.getCountrySQCTBySupplierForSALR = function () {
            $scope.SearchCriteria.SQItems = [];
            $scope.SearchCriteria.CountryItems = [];
            $scope.SearchCriteria.CommodityItems = [];

            var SupIds = [], SupIdsString = "";
            angular.forEach($scope.SearchCriteria.SupplierItems, function (item) {
                if (!IsUndefinedNullOrEmpty(item.Id) && item.Id > 0) {
                    SupIds.push(item.Id);
                }
            });
            SupIdsString = SupIds.join(",");
            $scope.LookUps = [
                {
                    "Name": "SQItems", "Parameters": { "SupplierIds": SupIdsString }
                },
                   {
                       "Name": "CountryItems", "Parameters": { "SupplierIds": SupIdsString, "SQIds": '' }
                   },
                   {
                       "Name": "CommodityTypesSALR", "Parameters": { "SupplierIds": SupIdsString }
                   }
            ];
            common.usSpinnerService.spin('spnReports');
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                common.usSpinnerService.stop('spnReports');
                angular.forEach(data.data, function (o) {
                    if (o.Name === "SQItems") {
                        $scope.SQList = o.Data;
                    }
                    else if (o.Name === "CountryItems") {
                        $scope.CountryList = o.Data;
                    }
                    else if (o.Name === "CommodityTypesSALR") {
                        $scope.CommodityTypeList = o.Data;
                    }
                });
            }, function (error) {
                common.usSpinnerService.stop('spnReports');
            });
        };
        $scope.getCountryBySQForSALR = function () {
            $scope.SearchCriteria.CountryItems = [];
            var SupIds = [], SQIds = [], SupIdsString = "", SQIdsString = "";
            angular.forEach($scope.SearchCriteria.SupplierItems, function (item) {
                if (!IsUndefinedNullOrEmpty(item.Id) && item.Id > 0) {
                    SupIds.push(item.Id);
                }
            });
            SupIdsString = SupIds.join(",");
            angular.forEach($scope.SearchCriteria.SQItems, function (item) {
                if (!IsUndefinedNullOrEmpty(item.Id)) {
                    SQIds.push(item.Id);
                }
            });
            SQIdsString = SQIds.join(",");

            $scope.LookUps = [
                {
                    "Name": "CountryItems", "Parameters": { "SupplierIds": SupIdsString, "SQIds": SQIdsString }
                }
            ];
            common.usSpinnerService.spin('spnReports');
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                common.usSpinnerService.stop('spnReports');
                angular.forEach(data.data, function (o) {
                    if (o.Name === "CountryItems") {
                        $scope.CountryList = o.Data;
                    }
                });
            }, function (error) {
                common.usSpinnerService.stop('spnReports');
            });
        };
        // End RFQ Supplier Activity Level Report

        //  start open RFQ report
        $scope.SetLookupDataOpenRFQsReport = function () {
            $scope.LookUps = [
                    {
                        "Name": "SAM", "Parameters": {}
                    },
                    {
                        "Name": "Customers", "Parameters": {}
                    },
                    {
                        "Name": "Countries", "Parameters": {}
                    },
                     {
                         "Name": "RFQTypes", "Parameters": {}
                     }
            ];
            $scope.getLookupDataOpenRFQsReport();
        };
        $scope.getLookupDataOpenRFQsReport = function () {
            common.usSpinnerService.spin('spnReports');
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                common.usSpinnerService.stop('spnReports');
                angular.forEach(data.data, function (o) {
                    if (o.Name === "SAM") {
                        $scope.SAMList = o.Data;
                    }
                    else if (o.Name === "Customers") {
                        $scope.CustomerList = o.Data;
                    }
                    else if (o.Name === "Countries") {
                        $scope.CountryList = o.Data;
                    }
                    else if (o.Name === "RFQTypes") {
                        $scope.RFQTypeList = o.Data;
                    }
                });
            }, function (error) {
                common.usSpinnerService.stop('spnReports');
            });
        }
        $scope.GetOpenRFQsReport = function () {
            common.usSpinnerService.spin('spnReports');
            $scope.OpenRFQsReportList = {};
            $scope.DataList = {};
            $scope.SAMIds = [];
            $scope.CustomerIds = [];
            $scope.CountryIds = [];
            $scope.RFQTypeIds = [];
            $scope.SelectionCriteria = {
                SAMItems: [], CustomerItems: [],
                RFQDateFrom: null, RFQDateTo: null,
                QuoteDueDateFrom: null, QuoteDueDateTo: null,
                ProjectName: '', RFQTypeItems: [], CountryItems: [],
                SupQuoted: '', ReportId: '0'
            };
            $scope.SelectionCriteria.ReportId = IsUndefinedNullOrEmpty($scope.SearchCriteria.ReportId) ? '0' : $scope.SearchCriteria.ReportId;
            $scope.SearchCriteria.DateFrom = convertUTCDateToLocalDate($scope.SearchCriteria.RFQDateFrom);
            $scope.SearchCriteria.DateTo = convertUTCDateToLocalDate($scope.SearchCriteria.RFQDateTo);
            $scope.SelectionCriteria.RFQDateFrom = $scope.SearchCriteria.RFQDateFrom;
            $scope.SelectionCriteria.RFQDateTo = $scope.SearchCriteria.RFQDateTo;
            $scope.SearchCriteria.QuoteDateFrom = convertUTCDateToLocalDate($scope.SearchCriteria.QuoteDueDateFrom);
            $scope.SearchCriteria.QuoteDateTo = convertUTCDateToLocalDate($scope.SearchCriteria.QuoteDueDateTo);
            $scope.SelectionCriteria.QuoteDueDateFrom = $scope.SearchCriteria.QuoteDueDateFrom;
            $scope.SelectionCriteria.QuoteDueDateTo = $scope.SearchCriteria.QuoteDueDateTo;
            $scope.SelectionCriteria.SupQuoted = IsUndefinedNullOrEmpty($scope.SearchCriteria.SupQuoted) ? '' : $scope.SearchCriteria.SupQuoted;
            $scope.SelectionCriteria.ProjectName = IsUndefinedNullOrEmpty($scope.SearchCriteria.ProjectName) ? '' : $scope.SearchCriteria.ProjectName;

            angular.forEach($scope.SearchCriteria.SAMItems, function (item) {
                if (!Isundefinedornull(item.Id)) {
                    $scope.SAMIds.push(item.Id);
                    $scope.SelectionCriteria.SAMItems.push(item.Name);
                }
            });
            $scope.SearchCriteria.SAMIds = $scope.SAMIds.join(",");
            $scope.SelectionCriteria.SAMItems = $scope.SelectionCriteria.SAMItems.join(", ");
            angular.forEach($scope.SearchCriteria.CustomerItems, function (item) {
                if (!Isundefinedornull(item.Id) && item.Id > 0) {
                    $scope.CustomerIds.push(item.Id);
                    $scope.SelectionCriteria.CustomerItems.push(item.Name);
                }
            });
            $scope.SearchCriteria.CustomerIds = $scope.CustomerIds.join(",");
            $scope.SelectionCriteria.CustomerItems = $scope.SelectionCriteria.CustomerItems.join(", ");

            angular.forEach($scope.SearchCriteria.RFQTypeItems, function (item) {
                if (!Isundefinedornull(item.Id) && item.Id > 0) {
                    $scope.RFQTypeIds.push(item.Id);
                    $scope.SelectionCriteria.RFQTypeItems.push(item.Name);
                }
            });
            $scope.SearchCriteria.RFQTypeIds = $scope.RFQTypeIds.join(",");
            $scope.SelectionCriteria.RFQTypeItems = $scope.SelectionCriteria.RFQTypeItems.join(", ");

            angular.forEach($scope.SearchCriteria.CountryItems, function (item) {
                if (!IsUndefinedNullOrEmpty(item.Id)) {
                    $scope.CountryIds.push(item.Id);
                    $scope.SelectionCriteria.CountryItems.push(item.Name);
                }
            });
            $scope.SearchCriteria.CountryIds = $scope.CountryIds.join(",");
            $scope.SelectionCriteria.CountryItems = $scope.SelectionCriteria.CountryItems.join(", ");

            $scope.Paging.Criteria = $scope.SearchCriteria;
            RFQReportSvc.GetOpenRFQsReport($scope.Paging).then(
                function (response) {
                    common.usSpinnerService.stop('spnReports');
                    if (response.data.StatusCode == 200) {
                        $scope.DataList = response.data.Result;
                        if ($scope.DataList.length > 0) {
                            $scope.ShowExportButton = true;
                        }
                        $scope.OpenRFQsReportList = $scope.DataList;
                        $scope.Paging = response.data.PageInfo;
                        $scope.Paging.Criteria = $scope.SearchCriteria;
                    }
                    else {
                        console.log(response.data.ErrorText);
                    }
                }, function (error) {
                    common.usSpinnerService.stop('spnReports');
                });
        }
        $scope.exportOpenRFQsReport = function () {
            common.usSpinnerService.spin('spnReports');
            RFQReportSvc.exportOpenRFQsReport($scope.Paging).then(
               function (response) {
                   common.usSpinnerService.stop('spnReports');
                   if (response.data.StatusCode == 200) {
                       window.open(response.data.SuccessMessage, '_blank');
                   }
                   else {
                       common.aaNotify.error(response.data.ErrorText);
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnReports');
               });
        };
        // End open RFQ report

        //  start RFQ Detailed Supplier Report
        $scope.SetLookupDataRFQDetailedSupplierReport = function () {
            $scope.LookUps = [
                {
                    "Name": "SupplierItems", "Parameters": {}
                }
            ];
            $scope.getLookupDataRFQDetailedSupplierReport();
        };
        $scope.getLookupDataRFQDetailedSupplierReport = function () {
            common.usSpinnerService.spin('spnReports');
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                common.usSpinnerService.stop('spnReports');
                angular.forEach(data.data, function (o) {
                    if (o.Name === "SupplierItems") {
                        $scope.SupplierItemsList = o.Data;
                    }
                });
            }, function (error) {
                common.usSpinnerService.stop('spnReports');
            });
        }
        $scope.GetRFQDetailedSupplierReport = function () {
            common.usSpinnerService.spin('spnReports');
            $scope.ShowRFQPartQuoteDetailGridReport = false;
            $scope.SearchCriteria.isInnerReport = false;
            $scope.RFQDetailedSupplierReportList = {};
            $scope.DataList = {};
            $scope.SupplierIds = [];
            $scope.SelectionCriteria = { SupplierItems: [], SupplierId: 0, RFQSentDateFrom: null, RFQSentDateTo: null, ReportId: '0' };
            $scope.SelectionCriteria.ReportId = IsUndefinedNullOrEmpty($scope.SearchCriteria.ReportId) ? '0' : $scope.SearchCriteria.ReportId;
            $scope.SearchCriteria.DateFrom = convertUTCDateToLocalDate($scope.SearchCriteria.RFQSentDateFrom);
            $scope.SearchCriteria.DateTo = convertUTCDateToLocalDate($scope.SearchCriteria.RFQSentDateTo);
            $scope.SelectionCriteria.RFQSentDateFrom = $scope.SearchCriteria.RFQSentDateFrom;
            $scope.SelectionCriteria.RFQSentDateTo = $scope.SearchCriteria.RFQSentDateTo;

            angular.forEach($scope.SearchCriteria.SupplierItems, function (item) {
                if (!IsUndefinedNullOrEmpty(item.Id) && item.Id > 0) {
                    $scope.SupplierIds.push(item.Id);
                    $scope.SelectionCriteria.SupplierItems.push(item.Name);
                }
            });
            $scope.SearchCriteria.SupplierIds = $scope.SupplierIds.join(",");
            $scope.SelectionCriteria.SupplierItems = $scope.SelectionCriteria.SupplierItems.join(", ");

            if ($scope.SupplierIds.length <= 0) {
                alert($filter('translate')('_AlertMessageSupplier_'));
                common.usSpinnerService.stop('spnReports');
                return false;
            }
            else if ($scope.SupplierIds.length > 100) {
                alert($filter('translate')('_SelectedSupplierExceededLimit_'));
                common.usSpinnerService.stop('spnReports');
                return false;
            }

            $scope.Paging.Criteria = $scope.SearchCriteria;
            RFQReportSvc.GetRFQDetailedSupplierReport($scope.Paging).then(
                function (response) {
                    common.usSpinnerService.stop('spnReports');
                    if (response.data.StatusCode == 200) {
                        $scope.DataList = response.data.Result;
                        if ($scope.DataList.length > 0) {
                            $scope.ShowExportButton = true;
                        }
                        $scope.RFQDetailedSupplierReportList = $scope.DataList;
                        $scope.Paging = response.data.PageInfo;
                        $scope.Paging.Criteria = $scope.SearchCriteria;
                    }
                    else {
                        console.log(response.data.ErrorText);
                    }
                }, function (error) {
                    common.usSpinnerService.stop('spnReports');
                });
        }
        $scope.ShowRFQPartQuoteDetailGrid = function (Id, SupplierName) {
            common.usSpinnerService.spin('spnReports');
            $scope.ShowRFQPartQuoteDetailGridReport = true;
            $scope.SearchCriteria.isInnerReport = true;
            $scope.RfqDataList = {};
            $scope.lstRFQ = {};
            $scope.SelectionCriteria.SupplierName = SupplierName;
            $scope.SearchCriteria.SupplierId = Id;
            $scope.Paging.Criteria = $scope.SearchCriteria;

            RFQReportSvc.GetRfqPartQuoteDetailsReport($scope.Paging).then(
                function (response) {
                    common.usSpinnerService.stop('spnReports');
                    if (response.data.StatusCode == 200) {
                        $scope.RfqDataList = response.data.Result;
                        if ($scope.RfqDataList.length > 0) {
                            $scope.ShowExportButton = true;
                            $scope.lstRFQ = $scope.RfqDataList; //_.groupBy($scope.RfqDataList, 'RFQNo');
                        }
                    }
                    else {
                        console.log(response.data.ErrorText);
                    }
                }, function (error) {
                    common.usSpinnerService.stop('spnReports');
                });
        }
        $scope.exportRFQDetailedSupplierReport = function () {
            common.usSpinnerService.spin('spnReports');
            RFQReportSvc.exportRFQDetailedSupplierReport($scope.Paging).then(
               function (response) {
                   common.usSpinnerService.stop('spnReports');
                   if (response.data.StatusCode == 200) {
                       window.open(response.data.SuccessMessage, '_blank');
                   }
                   else {
                       common.aaNotify.error(response.data.ErrorText);
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnReports');
               });
        };
        $scope.resetShowRFQPartQuoteDetailGridReport = function () {
            $scope.ShowRFQPartQuoteDetailGridReport = false;
            $scope.SearchCriteria.isInnerReport = false;
        };
        // End RFQ Detailed Supplier Report      

        $scope.fnCallBack = function (callbackfor) {
            if (callbackfor == "RFQQuotedBySupplierReport")
                $scope.getSQANDCountryBySuppliers();
            else if (callbackfor == "RFQSupplierReport")
                $scope.getCountryBySQs();
            else if (callbackfor == "SALRSupplier")
                $scope.getCountrySQCTBySupplierForSALR();
            else if (callbackfor == "SALRSQ")
                $scope.getCountryBySQForSALR();
        };
        $scope.ResetSearch = function () {
            common.$route.reload();
        };
        $scope.Init();
    }]);
