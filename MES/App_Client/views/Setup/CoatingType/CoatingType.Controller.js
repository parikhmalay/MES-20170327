app.controller('CoatingTypeCtrl', ['$rootScope', '$scope', 'common', 'CoatingTypeSvc', '$modal', '$filter', function ($rootScope, $scope, common, CoatingTypeSvc, $modal, $filter) {
    $scope.CoatingType = {};
    $scope.CoatingTypeList = {};
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;
    $rootScope.PageHeader = ($filter('translate')('_CoatingTypes_'));
    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        $scope.GetCoatingTypeList();
    };
    $scope.GetCoatingTypeList = function () {
        common.usSpinnerService.spin('spnCoatingType');
        CoatingTypeSvc.GetCoatingTypeList($scope.Paging).then(
             function (response) {
                 common.usSpinnerService.stop('spnCoatingType');
                 if (response.data.StatusCode == 200) {
                     $scope.CoatingTypeList = response.data.Result;
                     if ($scope.CoatingTypeList.length > 0)
                         advanceSearch.close();

                     $scope.Paging = response.data.PageInfo;
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
             function (error) {
                 common.usSpinnerService.stop('spnCoatingType');
                 //common.aaNotify.error(error);
             });
    };

    $scope.Edit = function (coatingType) {
        $scope.ShowPopup();
        $scope.CoatingType = coatingType;
    };

    $scope.Delete = function (coatingTypeId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            CoatingTypeSvc.Delete(coatingTypeId).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.GetCoatingTypeList();
               }
           },
           function (error) {
               //common.aaNotify.error(error);
           });
        }
    };

    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.GetCoatingTypeList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.GetCoatingTypeList();
    };

    $scope.ShowPopup = function (coatingTypeId) {
        $scope.CoatingType = {};
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/Setup/CoatingType/AddCoatingTypePopup.html?v=' + Version,
            controller: AddCoatingTypeCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            resolve: {
                CoatingTypeId: function () {
                    return coatingTypeId;
                }
            }
        });
        modalInstance.result.then(function (coatingTypeId) {

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
        $scope.TableName = 'CoatingType';
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

var AddCoatingTypeCtrl = function ($scope, common, $location, $modalInstance, CoatingTypeId, CoatingTypeSvc) {

    $scope.InitCoatingTypeCtrl = function () {
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

    $scope.SaveCoatingType = function () {
        common.usSpinnerService.spin('spnCoatingType');
        CoatingTypeSvc.Save($scope.CoatingType).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   if ($scope.CoatingType.Id != undefined && $scope.CoatingType.Id > 0) {
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   else {
                       $scope.Paging.PageNo = 1;
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   common.usSpinnerService.stop('spnCoatingType');
                   //$scope.Id = response.data.Result // Id of latest created record
                   $scope.ClosePopup();
               }
               else {
                   common.usSpinnerService.stop('spnCoatingType');
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnCoatingType');
               console.log(error);
           });
    }
    $scope.InitCoatingTypeCtrl();
};

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
}