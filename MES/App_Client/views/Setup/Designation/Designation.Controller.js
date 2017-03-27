app.controller('DesignationCtrl', ['$rootScope', '$scope', 'common', 'DesignationSvc', '$modal', '$filter', function ($rootScope, $scope, common, DesignationSvc, $modal, $filter) {
    $scope.Designation = {};
    $scope.DesignationList = {};
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;
    $rootScope.PageHeader = ($filter('translate')('_Designations_'));

    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        $scope.GetDesignationList();
    };
    $scope.GetDesignationList = function () {
        common.usSpinnerService.spin('spnDesignation');
        DesignationSvc.GetDesignationList($scope.Paging).then(
             function (response) {
                 common.usSpinnerService.stop('spnDesignation');
                 if (response.data.StatusCode == 200) {
                     $scope.DesignationList = response.data.Result;
                     if ($scope.DesignationList.length > 0)
                         advanceSearch.close();
                     $scope.Paging = response.data.PageInfo;
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
             function (error) {
                 common.usSpinnerService.stop('spnDesignation');
                 //common.aaNotify.error(error);
             });
    };

    $scope.Edit = function (designation) {
        $scope.ShowPopup();
        $scope.Designation = designation;
    };

    $scope.Delete = function (designationId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            DesignationSvc.Delete(designationId).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.GetDesignationList();
               }
           },
           function (error) {
               //common.aaNotify.error(error);
           });
        }
    };

    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.GetDesignationList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.GetDesignationList();
    };

    $scope.ShowPopup = function (designationId) {
        $scope.Designation = {};
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/Setup/Designation/AddDesignationPopup.html?v=' + Version,
            controller: AddDesignationCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            resolve: {
                DesignationId: function () {
                    return designationId;
                }
            }
        });
        modalInstance.result.then(function (designationId) {

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
        $scope.TableName = 'Designation';
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

var AddDesignationCtrl = function ($scope, common, $location, $modalInstance, DesignationId, DesignationSvc) {

    $scope.InitDesignationCtrl = function () {
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
    $scope.SaveDesignation = function () {
        common.usSpinnerService.spin('spnDesignation');
        DesignationSvc.Save($scope.Designation).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   if ($scope.Designation.Id != undefined && $scope.Designation.Id > 0) {
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   else {
                       $scope.Paging.PageNo = 1;
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   common.usSpinnerService.stop('spnDesignation');
                   $scope.ClosePopup();
               }
               else {
                   common.usSpinnerService.stop('spnDesignation');
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnDesignation');
               console.log(error);
           });
    }
    $scope.InitDesignationCtrl();
};

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
};