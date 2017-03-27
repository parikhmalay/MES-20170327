app.controller('PartQuoteReportFromQuoteCtrl', ['$rootScope', '$scope', 'common', 'RFQReportSvc', '$filter', '$routeParams',
    function ($rootScope, $scope, common, RFQReportSvc, $filter, $routeParams) {

        $scope.GetRFQPartsQuoteBySupplierReport = function () {
            common.usSpinnerService.spin('spnReports');
            $scope.Paging = {};
            $scope.SearchCriteria = { ReportId: '8' };  // ignore by zero
            $scope.Paging = GetDefaultPageObject();
            $scope.Paging.Criteria = $scope.SearchCriteria;
            $scope.RFQPartsSupplierWiseReportList = {};

            if (!IsUndefinedNullOrEmpty($routeParams.Id) && $routeParams.Id != '') {
                $scope.SearchCriteria.RFQIds = $routeParams.Id;
            }
            if (IsUndefinedNullOrEmpty($scope.SearchCriteria.RFQIds)) {
                alert($filter('translate')('_SelectRFQ_'));
                common.usSpinnerService.stop('spnReports');
                return false;
            }
            $scope.Paging.Criteria = $scope.SearchCriteria;
            RFQReportSvc.GetRFQPartsSupplierWiseReport($scope.Paging).then(
                function (response) {
                    common.usSpinnerService.stop('spnReports');
                    if (response.data.StatusCode == 200) {
                        $scope.RFQPartsSupplierWiseReportList = response.data.Result;
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
        $scope.GetRFQPartsQuoteBySupplierReport();
    }]);
