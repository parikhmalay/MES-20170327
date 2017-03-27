app.controller('RFQAnalysisReportCtrl', ['$scope', 'common', 'RFQReportSvc', '$modal', '$filter', 'LookupSvc', '$timeout',
    function ($scope, common, RFQReportSvc, $modal, $filter, LookupSvc, $timeout) {
        $scope.Init = function () {
            $scope.RFQAnalysisReportList = {};
            $scope.SelectionCriteria = { RFQDateFrom: '', RFQDateTo: '', SAMItems: [], CustomerItems: [], CommodityItems: [], RFQSourceItems: [], GroupBy: '' };
            $scope.Paging = GetDefaultPageObject();
            $scope.Paging.Criteria = $scope.SearchCriteria;
            $scope.GetRFQAnalysisReport();
        };
        $scope.GetRFQAnalysisReport = function () {
            common.usSpinnerService.spin('spnReports');
            $scope.SAMIds = [];
            $scope.CustomerIds = [];
            $scope.CommodityIds = [];
            $scope.RFQSourceIds = [];

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

            RFQReportSvc.GetRFQAnalysisReport($scope.Paging).then(
                function (response) {
                    common.usSpinnerService.stop('spnReports');
                    if (response.data.StatusCode == 200) {
                        $scope.RFQAnalysisReportList = response.data.Result;
                        $scope.Paging = response.data.PageInfo;
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
            $scope.GetRFQAnalysisReport();
        };

        $scope.pageChanged = function (PageNo) {
            $scope.Paging.PageNo = PageNo;
            $scope.GetRFQAnalysisReport();
        };

        $scope.Init();
    }]);