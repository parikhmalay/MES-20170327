app.controller('RFQQuoteReportBySupplierCtrl', ['$rootScope', '$scope', 'common', 'RFQReportSvc', '$modal', '$filter', 'LookupSvc', '$timeout',
    function ($rootScope, $scope, common, RFQReportSvc, $modal, $filter, LookupSvc, $timeout) {
        $rootScope.PageHeader = ($filter('translate')('_ReportSectionOne_'));

        $scope.Init = function () {
            $scope.Paging = GetDefaultPageObject();
            $scope.Paging.Criteria = $scope.SearchCriteria;

        };

        $scope.Init();
    }]);