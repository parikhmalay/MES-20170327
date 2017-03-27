app.controller('MachiningDescriptionCtrl', ['$rootScope', '$scope', 'common', 'MachiningDescriptionSvc', '$modal', '$filter', function ($rootScope, $scope, common, MachiningDescriptionSvc, $modal, $filter) {
    $scope.MachiningDescription = {};
    $scope.MachiningDescriptionList = {};
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;

    $rootScope.PageHeader = ($filter('translate')('_MachiningDescriptions_'));
    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        $scope.GetMachiningDescriptionList();
    };
    $scope.GetMachiningDescriptionList = function () {
        common.usSpinnerService.spin('spnMachiningDescription');
        MachiningDescriptionSvc.GetMachiningDescriptionList($scope.Paging).then(
             function (response) {
                 common.usSpinnerService.stop('spnMachiningDescription');
                 if (response.data.StatusCode == 200) {
                     $scope.MachiningDescriptionList = response.data.Result;
                     if ($scope.MachiningDescriptionList.length > 0)
                         advanceSearch.close();
                     $scope.Paging = response.data.PageInfo;
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
             function (error) {
                 common.usSpinnerService.stop('spnMachiningDescription');
                 //common.aaNotify.error(error);
             });
    };

    $scope.Edit = function (machiningDescription) {
        $scope.ShowPopup();
        $scope.MachiningDescription = machiningDescription;
    };

    $scope.Delete = function (machiningDescriptionId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            MachiningDescriptionSvc.Delete(machiningDescriptionId).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.GetMachiningDescriptionList();
               }
           },
           function (error) {
               //common.aaNotify.error(error);
           });
        }
    };

    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.GetMachiningDescriptionList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.GetMachiningDescriptionList();
    };

    $scope.ShowPopup = function (machiningDescriptionId) {
        $scope.MachiningDescription = {};
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/Setup/MachiningDescription/AddMachiningDescriptionPopup.html?v=' + Version,
            controller: AddMachiningDescriptionCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            resolve: {
                MachiningDescriptionId: function () {
                    return machiningDescriptionId;
                }
            }
        });
        modalInstance.result.then(function (machiningDescriptionId) {

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
        $scope.TableName = 'MachiningDesc';
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

var AddMachiningDescriptionCtrl = function ($scope, common, $location, $modalInstance, MachiningDescriptionId, MachiningDescriptionSvc) {
    $scope.InitMachiningDescriptionCtrl = function () {
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
    $scope.SaveMachiningDescription = function () {
        common.usSpinnerService.spin('spnMachiningDescription');
        MachiningDescriptionSvc.Save($scope.MachiningDescription).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   if ($scope.MachiningDescription.Id != undefined && $scope.MachiningDescription.Id > 0) {
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   else {
                       $scope.Paging.PageNo = 1;
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   common.usSpinnerService.stop('spnMachiningDescription');
                   //$scope.Id = response.data.Result // Id of latest created record
                   $scope.ClosePopup();
               }
               else {
                   common.usSpinnerService.stop('spnMachiningDescription');
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnMachiningDescription');
               console.log(error);
           });
    }
    $scope.InitMachiningDescriptionCtrl();
};

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
};