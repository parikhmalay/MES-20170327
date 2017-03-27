app.controller('DefectTypeCtrl', ['$rootScope', '$scope', 'common', 'DefectTypeSvc', '$modal', '$filter', function ($rootScope, $scope, common, DefectTypeSvc, $modal, $filter) {
    $scope.DefectType = {};
    $scope.DefectTypeList = {};
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;
    $rootScope.PageHeader = ($filter('translate')('_DefectTypes_'));
    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        $scope.GetDefectTypeList();
    };
    $scope.GetDefectTypeList = function () {
        common.usSpinnerService.spin('spnDefectType');
        DefectTypeSvc.GetDefectTypeList($scope.Paging).then(
             function (response) {
                 common.usSpinnerService.stop('spnDefectType');
                 if (response.data.StatusCode == 200) {
                     $scope.DefectTypeList = response.data.Result;
                     if ($scope.DefectTypeList.length > 0)
                         advanceSearch.close();

                     $scope.Paging = response.data.PageInfo;
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
             function (error) {
                 common.usSpinnerService.stop('spnDefectType');
                 //common.aaNotify.error(error);
             });
    };

    $scope.Edit = function (defectType) {
        $scope.ShowPopup();
        $scope.DefectType = defectType;
    };

    $scope.Delete = function (defectTypeId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            DefectTypeSvc.Delete(defectTypeId).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.GetDefectTypeList();
               }
           },
           function (error) {
               //common.aaNotify.error(error);
           });
        }
    };

    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.GetDefectTypeList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.GetDefectTypeList();
    };

    $scope.ShowPopup = function (defectTypeId) {
        $scope.DefectType = {};
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/Setup/DefectType/AddDefectTypePopup.html?v=' + Version,
            controller: AddDefectTypeCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            resolve: {
                DefectTypeId: function () {
                    return defectTypeId;
                }
            }
        });
        modalInstance.result.then(function (defectTypeId) {

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
        $scope.TableName = 'DefectType';
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

var AddDefectTypeCtrl = function ($scope, common, $location, $modalInstance, DefectTypeId, DefectTypeSvc) {

    $scope.InitDefectTypeCtrl = function () {
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
    }

    $scope.SaveDefectType = function () {
        common.usSpinnerService.spin('spnDefectType');
        DefectTypeSvc.Save($scope.DefectType).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   if ($scope.DefectType.Id != undefined && $scope.DefectType.Id > 0) {
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   else {
                       $scope.Paging.PageNo = 1;
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   common.usSpinnerService.stop('spnDefectType');
                   //$scope.Id = response.data.Result // Id of latest created record
                   $scope.ClosePopup();
               }
               else {
                   common.usSpinnerService.stop('spnDefectType');
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnDefectType');
               console.log(error);
           });
    }
    $scope.InitDefectTypeCtrl();
};

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
}