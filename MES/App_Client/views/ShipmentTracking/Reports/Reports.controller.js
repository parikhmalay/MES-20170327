app.controller('ReportCtrl', ['$rootScope', '$scope', 'common', 'ShipmentSvc', '$modal', '$filter', 'LookupSvc', 'ExportToExcel', '$timeout', function ($rootScope, $scope, common, ShipmentSvc, $modal, $filter, LookupSvc, ExportToExcel, $timeout) {
    $scope.Shipment = [];
    $scope.ShipmentList = [];
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;
    $rootScope.PageHeader = ($filter('translate')('_ReportSectionOne_'));
    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        $scope.SetLookupData();
        $scope.ShipmentModeList =
                     [{ 'name': 'Air', 'value': '1' }
                    , { 'name': 'Land', 'value': '2' }
                    , { 'name': 'Sea - Container', 'value': '3' }
                    , { 'name': 'Sea - LCL', 'value': '4' }
                    , { 'name': 'Sea - LCL - Express', 'value': '5' }];

        $scope.QualityReviewStatusList =
                 [{ 'name': 'Not Reviewed', 'value': '1' }
                , { 'name': 'Approved', 'value': '2' }
                , { 'name': 'Conditionally Approved', 'value': '3' }
                , { 'name': 'Rejected', 'value': '4' }];

        $scope.StatusList =
                [{ 'name': 'Open', 'value': true }
                , { 'name': 'Complete', 'value': false }];

    };

    $scope.SetLookupData = function () {
        $scope.LookUps = [
          {
              "Name": "Destinations", "Parameters": {}
          },
          {
              "Name": "Origins", "Parameters": {}
          },
          {
              "Name": "Forwarders", "Parameters": {}
          },
           {
               "Name": "Suppliers", "Parameters": {}
           },
        ];
        $scope.getLookupData();
    };

    $scope.getLookupData = function () {
        common.usSpinnerService.spin('spnShipment');
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            angular.forEach(data.data, function (o) {
                if (o.Name === "Suppliers") {
                    $scope.SupplierList = o.Data;
                }
                else if (o.Name === "Destinations") {
                    $scope.DestinationList = o.Data;
                }
                else if (o.Name === "Origins") {
                    $scope.OriginList = o.Data;
                }
                else if (o.Name === "Forwarders") {
                    $scope.ForwarderList = o.Data;
                }
            });
            common.usSpinnerService.stop('spnShipment');
        }, function (error) {
            common.usSpinnerService.stop('spnShipment');
        });
    }

    $scope.GetShipmentList = function () {
        common.usSpinnerService.spin('spnShipment');
        ShipmentSvc.GetShipmentList($scope.Paging).then(
             function (response) {
                 common.usSpinnerService.stop('spnShipment');
                 if (response.data.StatusCode == 200) {
                     $scope.ShipmentList = response.data.Result;
                     if ($scope.ShipmentList.length > 0)
                         advanceSearch.close();
                     angular.forEach($scope.ShipmentList, function (o) {
                         o.EstShpmntDate = convertUTCDateToLocalDate(o.EstShpmntDate);
                         o.ActShpmntDate = convertUTCDateToLocalDate(o.ActShpmntDate);
                         o.ETAAtWarehouseAtDest = convertUTCDateToLocalDate(o.ETAAtWarehouseAtDest);
                         o.ActArrDateAtWarehouseAtDest = convertUTCDateToLocalDate(o.ActArrDateAtWarehouseAtDest);
                         o.EstForwarderPickupDate = convertUTCDateToLocalDate(o.EstForwarderPickupDate);
                         o.ActForwarderPickupDate = convertUTCDateToLocalDate(o.ActForwarderPickupDate);
                         o.InspectionDate = convertUTCDateToLocalDate(o.InspectionDate);

                     });

                     $scope.Paging = response.data.PageInfo;
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
             function (error) {
                 common.usSpinnerService.stop('spnShipment');
                 //common.aaNotify.error(error);
             });
    };

    $scope.Search = function () {
        $scope.Init();
        $scope.ShowReport = true;
        $scope.GetShipmentList();
    };

    $scope.ResetSearch = function () {
        $scope.SearchCriteria = {};
        $scope.Init();
    }

    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.GetShipmentList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.GetShipmentList();
    };
    $scope.Init();

    //$scope.exportToExcel = function (tableId) { // ex: '#my-table'
    //    $scope.exportHref = ExportToExcel.tableToExcel(tableId, 'Shipment Report');
    //    $timeout(function () {
    //        location.href = $scope.exportHref;
    //    }, 100);
    //}

    $scope.exportToExcel = function () {
        common.usSpinnerService.spin('spnShipment');
        ShipmentSvc.exportToExcelShipmentReport($scope.Paging).then(
           function (response) {
               common.usSpinnerService.stop('spnShipment');
               if (response.data.StatusCode == 200) {
                   window.open(response.data.SuccessMessage, '_blank');
               }
               else {
                   common.aaNotify.error(response.data.ErrorText);
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnShipment');
           });
    };

}]);