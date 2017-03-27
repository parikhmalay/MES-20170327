app.controller('MachineDescriptionCtrl', ['$rootScope', '$scope', 'common', 'MachineDescriptionSvc', '$modal', '$filter', function ($rootScope, $scope, common, MachineDescriptionSvc, $modal, $filter) {
    $scope.MachineDescription = {};
    $scope.MachineDescriptionList = {};
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;
    $rootScope.PageHeader = ($filter('translate')('_MachineDescriptions_'));

    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        $scope.GetMachineDescriptionList();
    };
    $scope.GetMachineDescriptionList = function () {
        common.usSpinnerService.spin('spnMachineDescription');
        MachineDescriptionSvc.GetMachineDescriptionList($scope.Paging).then(
             function (response) {
                 common.usSpinnerService.stop('spnMachineDescription');
                 if (response.data.StatusCode == 200) {
                     $scope.MachineDescriptionList = response.data.Result;
                     if ($scope.MachineDescriptionList.length > 0)
                         advanceSearch.close();
                     $scope.Paging = response.data.PageInfo;
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
             function (error) {
                 common.usSpinnerService.stop('spnMachineDescription');
                 //common.aaNotify.error(error);
             });
    };

    $scope.Edit = function (machineDescription) {
        $scope.ShowPopup();
        $scope.MachineDescription = machineDescription;
    };

    $scope.Delete = function (machineDescriptionId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            MachineDescriptionSvc.Delete(machineDescriptionId).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.GetMachineDescriptionList();
               }
           },
           function (error) {
               //common.aaNotify.error(error);
           });
        }
    };

    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.GetMachineDescriptionList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.GetMachineDescriptionList();
    };

    $scope.ShowPopup = function (machineDescriptionId) {
        $scope.MachineDescription = {};
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/Setup/MachineDescription/AddMachineDescriptionPopup.html?v=' + Version,
            controller: AddMachineDescriptionCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            resolve: {
                MachineDescriptionId: function () {
                    return machineDescriptionId;
                }
            }
        });
        modalInstance.result.then(function (machineDescriptionId) {

        }, function () {
        });
    };

    $scope.ResetSearch = function () {
        $scope.SearchCriteria = {};
        $scope.Init();
    }

    //popup for show audit log 
    $scope.ShowChangeLogPopup = function (transactionId) {
        $scope.TransactionId = transactionId;
        $scope.TableName = 'MachineDesc';
        $scope.SchemaName = 'Setup';
        $scope.SpinnerKeyName = $("[id^='spn']").first().attr('id');
        var modalTemplatePreviewOptions = $modal.open({
            templateUrl: '/App_Client/common/directives/AuditLog/AuditLogsPopup.html?v=' + Version,
            controller: ViewChangeLogPageInstance,
            keyboard: true,
            backdrop: true,
            scope: $scope,
            sizeclass: 'modal-fitScreen'
        });
        modalTemplatePreviewOptions.result.then(function () {
        }, function () {
        });
    };
    // audit logs popup end here

    $scope.Init();
}]);

var AddMachineDescriptionCtrl = function ($scope, common, $location, $modalInstance, MachineDescriptionId, MachineDescriptionSvc) {
    $scope.InitMachineDescriptionCtrl = function () {
    }
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
        $scope.destroyScope();
    }
    $scope.ClosePopup = function () {
        $modalInstance.close();
        $scope.destroyScope();
    }
    $scope.destroyScope = function () {
        var lclScope = $scope;
        lclScope.$destroy();
    };
    $scope.SaveMachineDescription = function () {
        common.usSpinnerService.spin('spnMachineDescription');
        MachineDescriptionSvc.Save($scope.MachineDescription).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   if ($scope.MachineDescription.Id != undefined && $scope.MachineDescription.Id > 0) {
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   else {
                       $scope.Paging.PageNo = 1;
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   common.usSpinnerService.stop('spnMachineDescription');
                   //$scope.Id = response.data.Result // Id of latest created record
                   $scope.ClosePopup();
               }
               else {
                   common.usSpinnerService.stop('spnMachineDescription');
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnMachineDescription');
               console.log(error);
           });
    }
    $scope.InitMachineDescriptionCtrl();
};

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
};