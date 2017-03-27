app.controller('ShipmentCtrl', ['$rootScope', '$scope', 'common', 'ShipmentSvc', '$filter', function ($rootScope, $scope, common, ShipmentSvc, $filter) {
    //Start implement security role wise
    $scope.setRoleWisePrivilege = function () {
        $scope.currentSecurityObject = currentSecurityObject;
        if (!Isundefinedornull($scope.currentSecurityObject) && $scope.currentSecurityObject.length > 0) {
            angular.forEach($scope.currentSecurityObject, function (obj, index) {
                switch (obj.ObjectId) {
                    case 78:
                        $scope.setSecurityRoleCase(obj);
                        break;
                    case 84:
                        $scope.setSecurityRoleCase(obj);
                        break;
                    default:
                        break;
                }
            });
        }
        else {
            RedirectToAccessDenied();
        }
    }
    $scope.setSecurityRoleCase = function (obj) {
        switch (obj.PrivilegeId) {
            case 1:                           //none
                RedirectToAccessDenied();
                break;
            case 2:                           //read only
                $scope.IsReadOnlyPage = true;
                break;
        }
    };
    //End implement security role wise
    $scope.setRoleWisePrivilege();

    $rootScope.PageHeader = ($filter('translate')('_PageHeading_'));
    $scope.Shipment = [];

    $scope.Download = function () {
        common.usSpinnerService.spin('spnShipment');
        ShipmentSvc.downloadShipmentFile().then(
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

    $scope.Upload = function () {
        common.usSpinnerService.spin('spnShipment');
        if (IsUndefinedNullOrEmpty($scope.Shipment.uploadShipmentFilePath) || ($scope.Shipment.uploadShipmentFilePath == '')) {
            common.aaNotify.error($filter('translate')('_SelectFile_'));
            common.usSpinnerService.stop('spnShipment');
        }
        else {
            ShipmentSvc.uploadShipmentFile($scope.Shipment.uploadShipmentFilePath).then(
                 function (response) {
                     common.usSpinnerService.stop('spnShipment');

                     if (response.data.StatusCode == 200) {
                         $scope.Shipment.Id = response.data.Result;
                         common.aaNotify.success(response.data.SuccessMessage);
                     }
                     else {
                         common.aaNotify.error(response.data.ErrorText);
                     }
                 },
         function (error) {
             common.usSpinnerService.stop('spnShipment');
         });
        }
    };

    $scope.SetObjectvalues = function (file, name) {
        $scope.Shipment.uploadShipmentFilePath = file;
    };
}]);
