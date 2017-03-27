app.controller('APQPReportsCtrl', ['$rootScope', '$scope', 'common', 'APQPSvc', '$filter', '$modal', 'LookupSvc', function ($rootScope, $scope, common, APQPSvc, $filter, $modal, LookupSvc) {
    $rootScope.PageHeader = ($filter('translate')('_APQPReports_'));
    $scope.todayDate = new Date();
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
        $scope.CopySearchCriteria = angular.copy($scope.SearchCriteria);
        $scope.SearchCriteria = {
            ReportId: id,
            ProjectKickoffDateFrom: $scope.CopySearchCriteria.ProjectKickoffDateFrom,
            ProjectKickoffDateTo: $scope.CopySearchCriteria.ProjectKickoffDateTo,
            PPAPPartsApprovedDateFrom: $scope.CopySearchCriteria.PPAPPartsApprovedDateFrom,
            PPAPPartsApprovedDateTo: $scope.CopySearchCriteria.PPAPPartsApprovedDateTo,
            WeeklyDateFrom: $scope.CopySearchCriteria.WeeklyDateFrom,
            WeeklyDateTo: $scope.CopySearchCriteria.WeeklyDateTo,
            RMADateFrom: $scope.CopySearchCriteria.RMADateFrom,
            RMADateTo: $scope.CopySearchCriteria.RMADateTo
        };

        $scope.ShowExportButton = false;
        switch (id) {
            case '1':
                $scope.SetLookupDataAPQPProjectStatusReport();
                $scope.setDefaultValues();
                break;
            case '2':
                $scope.SetLookupDataAPQPDefectTrackingReport();
                $scope.setDefaultValues();
                $scope.SearchCriteria.RMADateTo = $scope.todayDate;
                $('body').removeClass('paginationFixedToBottom');
                break;
            case '3':
                //$scope.SetLookupDataAPQPWeeklyMeetingReport();
                $scope.setDefaultValues();
                $('body').removeClass('paginationFixedToBottom');
                break;
            case '4':
                $scope.SetLookupDataAPQPPPAPApprovalReport();
                $scope.setDefaultValues();
                break;
            case '5':
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
                $scope.GetAPQPProjectStatusReport();
                $scope.SearchCriteria.ReportViewPath = "/App_Client/views/APQP/APQP/APQPProjectStatusReport.html";
                $scope.SearchCriteria.ReportSearchResultHeading = ($filter('translate')('_ProjectStatus_'));
                break;
            case '2':
                $scope.GetDefectTrackingReport();
                $scope.SearchCriteria.ReportViewPath = "/App_Client/views/APQP/DefectTracking/DefectTrackingReport.html";
                $scope.SearchCriteria.ReportSearchResultHeading = ($filter('translate')('_DefectTrackingList_'));
                break;
            case '3':
                $scope.GetAPQPWeeklyMeetingReport();
                $scope.SearchCriteria.ReportViewPath = "/App_Client/views/APQP/APQP/APQPWeeklyMeetingReport.html";
                $scope.SearchCriteria.ReportSearchResultHeading = ($filter('translate')('_APQPWeeklyMeetingReport_'));
                break;
            case '4':
                $scope.GetAPQPPPAPApprovalReport();
                $scope.SearchCriteria.ReportViewPath = "/App_Client/views/APQP/APQP/APQPPPAPApprovalReport.html";
                $scope.SearchCriteria.ReportSearchResultHeading = ($filter('translate')('_APQPPPAPApprovalReport_'));
                break;
            case '5':
                $scope.GetAPQPNewBusinessAwardedReport();
                $scope.SearchCriteria.ReportViewPath = "/App_Client/views/APQP/APQP/APQPNewBusinessAwardReport.html";
                $scope.SearchCriteria.ReportSearchResultHeading = ($filter('translate')('_APQPNewBusinessAwardedReport_'));
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
                $scope.exportProjectStatusReport();
                break;
            case '2':
                $scope.exportDefectTrackingReport();
                break;
            case '3':
                $scope.exportAPQPWeeklyMeetingReport();
                break;
            case '4':
                $scope.exportAPQPPPAPApprovalReport();
                break;
            default:
                break;
        }
    };

    //start project status report
    $scope.SetLookupDataAPQPProjectStatusReport = function () {
        $scope.LookUps = [
           {
               "Name": "SAM", "Parameters": { "source": "SAM" }
           },
            {
                "Name": "Customers", "Parameters": {}
            },
            {
                "Name": "APQPEngineers", "Parameters": {}
            },
            {
                "Name": "SCUsers", "Parameters": {}
            },
             {
                 "Name": "APQPStatus", "Parameters": {
                     "source": "APQP"
                 }
             },
             {
                 "Name": "RFQForSupplierQuote", "Parameters": { "CustomerId": 0 }
             },
             {
                 "Name": "QuoteNumbers", "Parameters": {}
             }
        ];
        $scope.getLookupDataAPQPProjectStatusReport();
    };
    $scope.getLookupDataAPQPProjectStatusReport = function () {
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
                else if (o.Name === "APQPEngineers") {
                    $scope.APQPQualityEngineerList = o.Data;
                }
                else if (o.Name === "SCUsers") {
                    $scope.SupplyChainCoordinatorList = o.Data;
                }
                else if (o.Name === "APQPStatus") {
                    $scope.APQPStatusList = o.Data;
                }
                else if (o.Name === "RFQForSupplierQuote") {
                    $scope.RFQList = o.Data;
                }
                else if (o.Name === "QuoteNumbers") {
                    $scope.QuoteNumberList = o.Data;
                }

            });
        }, function (error) {
            common.usSpinnerService.stop('spnReports');
        });
    };
    $scope.GetAPQPProjectStatusReport = function () {
        common.usSpinnerService.spin('spnReports');
        $scope.ProjectStatusReportList = {};
        var APQPStatusIds = [];

        $scope.SelectionCriteria = {
            APQPStatusIds: [], ProjectKickoffDateFrom: null, ProjectKickoffDateTo: null, ReportId: '0'
        };

        $scope.SelectionCriteria.ReportId = IsUndefinedNullOrEmpty($scope.SearchCriteria.ReportId) ? '0' : $scope.SearchCriteria.ReportId;
        $scope.SearchCriteria.DateFrom = convertUTCDateToLocalDate($scope.SearchCriteria.ProjectKickoffDateFrom);
        $scope.SearchCriteria.DateTo = convertUTCDateToLocalDate($scope.SearchCriteria.ProjectKickoffDateTo);
        $scope.SelectionCriteria.ProjectKickoffDateFrom = $scope.SearchCriteria.ProjectKickoffDateFrom;
        $scope.SelectionCriteria.ProjectKickoffDateTo = $scope.SearchCriteria.ProjectKickoffDateTo;

        if (!IsUndefinedNullOrEmpty($scope.SearchCriteria.RFQNumber)) {
            $scope.SelectionCriteria.RFQNumber = $filter('filter')($scope.RFQList, function (rw) {
                return rw.Id == $scope.SearchCriteria.RFQNumber
            })[0].Name;
        }
        if (!IsUndefinedNullOrEmpty($scope.SearchCriteria.QuoteNumber)) {
            $scope.SelectionCriteria.QuoteNumber = $filter('filter')($scope.QuoteNumberList, function (rw) {
                return rw.Id == $scope.SearchCriteria.QuoteNumber
            })[0].Name;
        }
        $scope.SelectionCriteria.PartNo = $scope.SearchCriteria.PartNo;
        $scope.SelectionCriteria.ProjectName = $scope.SearchCriteria.ProjectName;

        if (!IsUndefinedNullOrEmpty($scope.SearchCriteria.CustomerId)) {
            $scope.SelectionCriteria.CustomerName = $filter('filter')($scope.CustomerList, function (rw) {
                return rw.Id == $scope.SearchCriteria.CustomerId
            })[0].Name;
            $scope.SearchCriteria.CustomerName = $scope.SelectionCriteria.CustomerName;
        }
        else {
            $scope.SearchCriteria.CustomerName = "";
        }

        if (!IsUndefinedNullOrEmpty($scope.SearchCriteria.SAMUserId)) {
            $scope.SelectionCriteria.SAMUserName = $filter('filter')($scope.SAMList, function (rw) {
                return rw.Id == $scope.SearchCriteria.SAMUserId
            })[0].Name;
        }
        $scope.SelectionCriteria.ManufacturerCode = $scope.SearchCriteria.ManufacturerCode;
        $scope.SelectionCriteria.ManufacturerName = $scope.SearchCriteria.ManufacturerName;

        if (!IsUndefinedNullOrEmpty($scope.SearchCriteria.APQPQualityEngineerId)) {
            $scope.SelectionCriteria.APQPQualityEngineerName = $filter('filter')($scope.APQPQualityEngineerList, function (rw) {
                return rw.Id == $scope.SearchCriteria.APQPQualityEngineerId
            })[0].Name;
        }
        if (!IsUndefinedNullOrEmpty($scope.SearchCriteria.SupplyChainCoordinatorId)) {
            $scope.SelectionCriteria.SupplyChainCoordinatorName = $filter('filter')($scope.SupplyChainCoordinatorList, function (rw) {
                return rw.Id == $scope.SearchCriteria.SupplyChainCoordinatorId
            })[0].Name;
        }

        angular.forEach($scope.SearchCriteria.APQPStatusItems, function (item, index) {
            if (!IsUndefinedNullOrEmpty(item.Id)) {
                APQPStatusIds.push(item.Id);
                $scope.SelectionCriteria.APQPStatusIds.push(item.Name);
            }
        });
        $scope.SearchCriteria.APQPStatusIds = APQPStatusIds.join(",");
        $scope.SelectionCriteria.APQPStatusIds = $scope.SelectionCriteria.APQPStatusIds.join(", ");

        $scope.Paging.Criteria = $scope.SearchCriteria;
        APQPSvc.GetAPQPProjectStatusReport($scope.Paging).then(
            function (response) {
                common.usSpinnerService.stop('spnReports');
                if (response.data.StatusCode == 200) {
                    $scope.ProjectStatusReportList = response.data.Result;
                    if ($scope.ProjectStatusReportList.length > 0) {
                        $scope.ShowExportButton = true;
                        angular.forEach($scope.ProjectStatusReportList, function (item, index) {
                            item.ProjectKickoffDate = convertUTCDateToLocalDate(item.ProjectKickoffDate);
                            item.ActualToolingKickoffDate = convertUTCDateToLocalDate(item.ActualToolingKickoffDate);
                            item.PlannedToolingCompletionDate = convertUTCDateToLocalDate(item.PlannedToolingCompletionDate);
                            item.ActualToolingCompletionDate = convertUTCDateToLocalDate(item.ActualToolingCompletionDate);
                            item.EstimatedSampleShipmentDate = convertUTCDateToLocalDate(item.EstimatedSampleShipmentDate);
                        });
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
    $scope.exportProjectStatusReport = function () {
        common.usSpinnerService.spin('spnReports');
        APQPSvc.ExportAPQPProjectStatusReport($scope.Paging).then(
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

    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.GetAPQPProjectStatusReport();
    };
    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.GetAPQPProjectStatusReport();
    };
    //End project status report

    //Start New Business Awarded Report

    $scope.GetAPQPNewBusinessAwardedReport = function () {
     
        common.usSpinnerService.spin('spnReports');
        $scope.APQPNewBusinessAwardedReportData = {};
        $scope.SelectionCriteria = {
            WeeklyDateFrom: null, WeeklyDateTo: null, ReportId: '0'
        };
        $scope.SelectionCriteria.ReportId = IsUndefinedNullOrEmpty($scope.SearchCriteria.ReportId) ? '0' : $scope.SearchCriteria.ReportId;
        //var d = new Date();
        //d.setFullYear(2016);
        //$scope.SearchCriteria.DateFrom = convertUTCDateToLocalDate(d);
        //var e = new Date();
        //$scope.SearchCriteria.DateTo = convertUTCDateToLocalDate(e);
        //$scope.SelectionCriteria.WeeklyDateFrom = d;
        //$scope.SelectionCriteria.WeeklyDateTo = e;
   
        $scope.SearchCriteria.DateFrom = convertUTCDateToLocalDate($scope.SearchCriteria.WeeklyDateFrom);
        $scope.SearchCriteria.DateTo = convertUTCDateToLocalDate($scope.SearchCriteria.WeeklyDateTo);
        $scope.SelectionCriteria.WeeklyDateFrom = $scope.SearchCriteria.WeeklyDateFrom;
        $scope.SelectionCriteria.WeeklyDateTo = $scope.SearchCriteria.WeeklyDateTo;
        $scope.Paging.Criteria = $scope.SearchCriteria;
        APQPSvc.GetAPQPNewBusinessAwardedReport($scope.Paging).then(
            function (response) {
                common.usSpinnerService.stop('spnReports');
                if (response.data.StatusCode == 200) {
                    $scope.APQPNewBusinessAwardedReportData = response.data.Result;
                    if (!Isundefinedornull($scope.APQPNewBusinessAwardedReportData) && $scope.APQPNewBusinessAwardedReportData.length > 0) {
                        $scope.ShowExportButton = true;
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
    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.GetAPQPProjectStatusReport();
    };
    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.GetAPQPProjectStatusReport();
    };

    //End New Business Awarded Report

    //Start defect Tracking report
    $scope.SetLookupDataAPQPDefectTrackingReport = function () {
        $scope.LookUps = [
            {
                "Name": "SAPItemByCustomer", "Parameters": { "CustomerCode": "" }
            },
            {
                "Name": "SAPCustomersName", "Parameters": {}
            },
             {
                 "Name": "SAPSuppliersName", "Parameters": {}
             },
             {
                 "Name": "Users", "Parameters": {}
             }
        ];
        $scope.getLookupDataAPQPDefectTrackingReport();
    };
    $scope.getLookupDataAPQPDefectTrackingReport = function () {
        common.usSpinnerService.spin('spnReports');
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            common.usSpinnerService.stop('spnReports');
            angular.forEach(data.data, function (o) {
                if (o.Name === "SAPCustomersName") {
                    $scope.CustomerList = o.Data;
                }
                else if (o.Name === "SAPItemByCustomer") {
                    $scope.PartNoList = o.Data;
                }
                else if (o.Name === "SAPSuppliersName") {
                    $scope.SupplierList = o.Data;
                }
                else if (o.Name === "Users") {
                    $scope.RMAInitiatedByList = o.Data;
                }
            });
        }, function (error) {
            common.usSpinnerService.stop('spnReports');
        });
    };
    $scope.GetDefectTrackingReport = function () {
        common.usSpinnerService.spin('spnReports');
        $scope.DefectTrackingReportList = {};
        $scope.SelectionCriteria = {
            ReportId: '0'
        };

        $scope.SelectionCriteria.ReportId = IsUndefinedNullOrEmpty($scope.SearchCriteria.ReportId) ? '0' : $scope.SearchCriteria.ReportId;
        $scope.SelectionCriteria.RMANumber = $scope.SearchCriteria.RMANumber;
        if (!IsUndefinedNullOrEmpty($scope.SearchCriteria.PartNo)) {
            $scope.SelectionCriteria.PartNo = $filter('filter')($scope.PartNoList, function (rw) {
                return rw.Id == $scope.SearchCriteria.PartNo
            })[0].Name;
        }

        if (!IsUndefinedNullOrEmpty($scope.SearchCriteria.CustomerId)) {
            $scope.SelectionCriteria.CustomerName = $filter('filter')($scope.CustomerList, function (rw) {
                return rw.Id == $scope.SearchCriteria.CustomerId
            })[0].Name;
            $scope.SearchCriteria.CustomerName = $scope.SelectionCriteria.CustomerName;
        }
        else
            $scope.SearchCriteria.CustomerName = "";

        if (!IsUndefinedNullOrEmpty($scope.SearchCriteria.RMAInitiatedBy)) {
            $scope.SelectionCriteria.RMAInitiatedByName = $filter('filter')($scope.RMAInitiatedByList, function (rw) {
                return rw.Id == $scope.SearchCriteria.RMAInitiatedBy
            })[0].Name;
        }
        $scope.SelectionCriteria.SupplierCode = $scope.SearchCriteria.SupplierCode;
        if (!IsUndefinedNullOrEmpty($scope.SearchCriteria.SupplierId)) {
            $scope.SelectionCriteria.SupplierName = $filter('filter')($scope.SupplierList, function (rw) {
                return rw.Id == $scope.SearchCriteria.SupplierId
            })[0].Name;
            $scope.SearchCriteria.SupplierName = $scope.SelectionCriteria.SupplierName;
        }
        else
            $scope.SearchCriteria.SupplierName = "";

        $scope.SearchCriteria.DateFrom = convertUTCDateToLocalDate($scope.SearchCriteria.RMADateFrom);
        $scope.SearchCriteria.DateTo = convertUTCDateToLocalDate($scope.SearchCriteria.RMADateTo);
        $scope.SearchCriteria.RMADateFrom = $scope.SearchCriteria.DateFrom;
        $scope.SearchCriteria.RMADateTo = $scope.SearchCriteria.DateTo;

        $scope.SelectionCriteria.CAPANumber = $scope.SearchCriteria.CAPANumber;
        $scope.SelectionCriteria.MESWarehouseLocation = $scope.SearchCriteria.MESWarehouseLocation;

        $scope.Paging.Criteria = $scope.SearchCriteria;
        APQPSvc.GetDefectTrackingReport($scope.Paging).then(
            function (response) {
                common.usSpinnerService.stop('spnReports');
                if (response.data.StatusCode == 200) {
                    $scope.DefectTrackingReportList = response.data.Result;
                    if ($scope.DefectTrackingReportList.length > 0) {
                        $scope.ShowExportButton = true;
                        angular.forEach($scope.DefectTrackingReportList, function (item, index) {
                            item.RMADate = convertUTCDateToLocalDate(item.RMADate);
                        });
                    }
                    //$scope.Paging = response.data.PageInfo;
                    $scope.Paging.Criteria = $scope.SearchCriteria;
                }
                else {
                    console.log(response.data.ErrorText);
                }
            }, function (error) {
                common.usSpinnerService.stop('spnReports');
            });
    };
    $scope.exportDefectTrackingReport = function () {
        common.usSpinnerService.spin('spnReports');
        APQPSvc.ExportDefectTrackingReport($scope.Paging).then(
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
    //End defect Tracking report

    //start Weekly Meeting Report
    $scope.GetAPQPWeeklyMeetingReport = function () {
        common.usSpinnerService.spin('spnReports');
        $scope.APQPWeeklyMeetingReportData = {};
        $scope.SelectionCriteria = {
            WeeklyDateFrom: null, WeeklyDateTo: null, ReportId: '0'
        };
        $scope.SelectionCriteria.ReportId = IsUndefinedNullOrEmpty($scope.SearchCriteria.ReportId) ? '0' : $scope.SearchCriteria.ReportId;
        $scope.SearchCriteria.DateFrom = convertUTCDateToLocalDate($scope.SearchCriteria.WeeklyDateFrom);
        $scope.SearchCriteria.DateTo = convertUTCDateToLocalDate($scope.SearchCriteria.WeeklyDateTo);
        $scope.SelectionCriteria.WeeklyDateFrom = $scope.SearchCriteria.WeeklyDateFrom;
        $scope.SelectionCriteria.WeeklyDateTo = $scope.SearchCriteria.WeeklyDateTo;

        $scope.Paging.Criteria = $scope.SearchCriteria;
        APQPSvc.GetAPQPWeeklyMeetingReport($scope.Paging).then(
            function (response) {
                common.usSpinnerService.stop('spnReports');
                if (response.data.StatusCode == 200) {
                    $scope.APQPWeeklyMeetingReportData = response.data.Result;
                    if (!Isundefinedornull($scope.APQPWeeklyMeetingReportData) && $scope.APQPWeeklyMeetingReportData.lstAPQPWeeklyMeetingReportPartA.length > 0) {
                        $scope.ShowExportButton = true;
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
    $scope.exportAPQPWeeklyMeetingReport = function () {
        common.usSpinnerService.spin('spnReports');
        APQPSvc.ExportAPQPWeeklyMeetingReport($scope.Paging).then(
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
    //End Weekly Meeting Report

    //start PPAP Approval Report
    $scope.SetLookupDataAPQPPPAPApprovalReport = function () {
        $scope.LookUps = [
           {
               "Name": "SAM", "Parameters": { "source": "SAM" }
           },
            {
                "Name": "APQPEngineers", "Parameters": {}
            },
            {
                "Name": "SCUsers", "Parameters": {}
            },
            {
                "Name": "ProjectStagesWithoutCategoryId", "Parameters": {}
            }
        ];
        $scope.getLookupDataAPQPPPAPApprovalReport();
    };
    $scope.getLookupDataAPQPPPAPApprovalReport = function () {
        common.usSpinnerService.spin('spnReports');
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            common.usSpinnerService.stop('spnReports');
            angular.forEach(data.data, function (o) {
                if (o.Name === "SAM") {
                    $scope.SAMList = o.Data;
                }
                else if (o.Name === "APQPEngineers") {
                    $scope.APQPQualityEngineerList = o.Data;
                }
                else if (o.Name === "SCUsers") {
                    $scope.SupplyChainCoordinatorList = o.Data;
                }
                else if (o.Name === "ProjectStagesWithoutCategoryId") {
                    $scope.ProjectStageList = o.Data;
                }
            });
        }, function (error) {
            common.usSpinnerService.stop('spnReports');
        });
    };
    $scope.GetAPQPPPAPApprovalReport = function () {
        common.usSpinnerService.spin('spnReports');
        $scope.APQPPPAPApprovalReportList = {};
        var APQPQualityEngineerIds = [], SupplyChainCoordinatorIds = [], SAMUserIds = [], ProjectStageIds = [];

        $scope.SelectionCriteria = {
            APQPQualityEngineerIds: [], SupplyChainCoordinatorIds: [], SAMUserIds: [], ProjectStageIds: [],
            PPAPPartsApprovedDateFrom: null, PPAPPartsApprovedDateTo: null, ReportId: '0'
        };

        $scope.SelectionCriteria.ReportId = IsUndefinedNullOrEmpty($scope.SearchCriteria.ReportId) ? '0' : $scope.SearchCriteria.ReportId;
        $scope.SearchCriteria.DateFrom = convertUTCDateToLocalDate($scope.SearchCriteria.PPAPPartsApprovedDateFrom);
        $scope.SearchCriteria.DateTo = convertUTCDateToLocalDate($scope.SearchCriteria.PPAPPartsApprovedDateTo);
        $scope.SelectionCriteria.PPAPPartsApprovedDateFrom = $scope.SearchCriteria.PPAPPartsApprovedDateFrom;
        $scope.SelectionCriteria.PPAPPartsApprovedDateTo = $scope.SearchCriteria.PPAPPartsApprovedDateTo;
        $scope.SelectionCriteria.CustomerName = $scope.SearchCriteria.CustomerName;

        angular.forEach($scope.SearchCriteria.APQPQualityEngineerItems, function (item, index) {
            if (!IsUndefinedNullOrEmpty(item.Id)) {
                APQPQualityEngineerIds.push(item.Id);
                $scope.SelectionCriteria.APQPQualityEngineerIds.push(item.Name);
            }
        });
        $scope.SearchCriteria.APQPQualityEngineerIds = APQPQualityEngineerIds.join(",");
        $scope.SelectionCriteria.APQPQualityEngineerIds = $scope.SelectionCriteria.APQPQualityEngineerIds.join(", ");

        angular.forEach($scope.SearchCriteria.SupplyChainCoordinatorItems, function (item, index) {
            if (!IsUndefinedNullOrEmpty(item.Id)) {
                SupplyChainCoordinatorIds.push(item.Id);
                $scope.SelectionCriteria.SupplyChainCoordinatorIds.push(item.Name);
            }
        });
        $scope.SearchCriteria.SupplyChainCoordinatorIds = SupplyChainCoordinatorIds.join(",");
        $scope.SelectionCriteria.SupplyChainCoordinatorIds = $scope.SelectionCriteria.SupplyChainCoordinatorIds.join(", ");

        angular.forEach($scope.SearchCriteria.SAMUserItems, function (item, index) {
            if (!IsUndefinedNullOrEmpty(item.Id)) {
                SAMUserIds.push(item.Id);
                $scope.SelectionCriteria.SAMUserIds.push(item.Name);
            }
        });
        $scope.SearchCriteria.SAMUserIds = SAMUserIds.join(",");
        $scope.SelectionCriteria.SAMUserIds = $scope.SelectionCriteria.SAMUserIds.join(", ");

        angular.forEach($scope.SearchCriteria.ProjectStageItems, function (item, index) {
            if (!IsUndefinedNullOrEmpty(item.Id) && item.Id > 0) {
                ProjectStageIds.push(item.Id);
                $scope.SelectionCriteria.ProjectStageIds.push(item.Name);
            }
        });
        $scope.SearchCriteria.ProjectStageIds = ProjectStageIds.join(",");
        $scope.SelectionCriteria.ProjectStageIds = $scope.SelectionCriteria.ProjectStageIds.join(", ");

        if (!IsUndefinedNullOrEmpty($scope.SearchCriteria.PPAPStatus)) {
            if ($scope.SearchCriteria.PPAPStatus == 1)
                $scope.SelectionCriteria.PPAPStatus = "On Time";
            else if ($scope.SearchCriteria.PPAPStatus == 2)
                $scope.SelectionCriteria.PPAPStatus = "Late";
        }
        $scope.SearchCriteria.PPAPStatusText = $scope.SelectionCriteria.PPAPStatus;

        $scope.Paging.Criteria = $scope.SearchCriteria;
        APQPSvc.GetAPQPPPAPApprovalReport($scope.Paging).then(
            function (response) {
                common.usSpinnerService.stop('spnReports');
                if (response.data.StatusCode == 200) {
                    $scope.APQPPPAPApprovalReportList = response.data.Result;
                    if ($scope.APQPPPAPApprovalReportList.length > 0) {
                        $scope.ShowExportButton = true;
                        angular.forEach($scope.APQPPPAPApprovalReportList, function (item, index) {
                            item.ProjectKickoffDate = convertUTCDateToLocalDate(item.ProjectKickoffDate);
                            item.ToolingKickoffDate = convertUTCDateToLocalDate(item.ToolingKickoffDate);
                            item.PlanToolingCompletionDate = convertUTCDateToLocalDate(item.PlanToolingCompletionDate);
                            item.ActualToolingCompletionDate = convertUTCDateToLocalDate(item.ActualToolingCompletionDate);
                            item.PSWDate = convertUTCDateToLocalDate(item.PSWDate);
                            item.ActualPSWDate = convertUTCDateToLocalDate(item.ActualPSWDate);
                            item.PPAPPartsApprovedDate = convertUTCDateToLocalDate(item.PPAPPartsApprovedDate);
                        });
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
    $scope.exportAPQPPPAPApprovalReport = function () {
        common.usSpinnerService.spin('spnReports');
        APQPSvc.ExportAPQPPPAPApprovalReport($scope.Paging).then(
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

    $scope.APQPPPAPApprovalReportPageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.GetAPQPPPAPApprovalReport();
    };
    $scope.APQPPPAPApprovalReportPageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.GetAPQPPPAPApprovalReport();
    };
    //End PPAP Approval Report

    $scope.ResetSearch = function () {
        common.$route.reload();
    };
    $scope.Init();
}]);