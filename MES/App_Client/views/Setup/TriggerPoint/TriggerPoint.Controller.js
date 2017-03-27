app.controller('TriggerPointCtrl', ['$rootScope', '$scope', 'common', 'TriggerPointSvc', '$modal', 'LookupSvc', '$filter', function ($rootScope, $scope, common, TriggerPointSvc, $modal, LookupSvc, $filter) {
    $scope.TriggerPoint = {};
    $scope.TriggerPointList = {};
    $scope.LookUps = [{"Name": "UserWithDesignation", "Parameters": {}}];
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;
    $scope.UserList = {};
    $rootScope.PageHeader = ($filter('translate')('_TriggerPoints_'));
    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        $scope.GetTriggerPointsList();

    };

    $scope.GetTriggerPointsList = function () {
        common.usSpinnerService.spin('spnTriggerPoint');
        TriggerPointSvc.GetTriggerPointsList($scope.Paging).then(
             function (response) {
                 common.usSpinnerService.stop('spnTriggerPoint');
                 if (response.data.StatusCode == 200) {
                     $scope.TriggerPointList = response.data.Result;
                     if ($scope.TriggerPointList.length > 0)
                         advanceSearch.close();
                     $scope.Paging = response.data.PageInfo;
                     $scope.getLookupData();
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
             function (error) {
                 common.usSpinnerService.stop('spnTriggerPoint');
                 //common.aaNotify.error(error);
             });
    };

    $scope.Edit = function (triggerPoint) {
        $scope.ShowPopup();
        $scope.TriggerPoint = triggerPoint;
    };

    $scope.Delete = function (triggerPointId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            TriggerPointSvc.Delete(triggerPointId).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.GetTriggerPointsList();
               }
           },
           function (error) {
               //common.aaNotify.error(error);
           });
        }
    };

    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.GetTriggerPointsList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.GetTriggerPointsList();
    };

    $scope.ShowPopup = function (triggerPointId) {
        $scope.TriggerPoint = {};
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/Setup/TriggerPoint/AddTriggerPointPopup.html?v=' + Version,
            controller: AddTriggerPointCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            resolve: {
                TriggerPointId: function () {
                    return triggerPointId;
                }
            }
        });
        modalInstance.result.then(function (triggerPointId) {

        }, function () {
        });
    };

    $scope.ResetSearch = function () {
        $scope.SearchCriteria = {};
        $scope.Init();
    }

    $scope.getLookupData = function () {
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            angular.forEach(data.data, function (o) {
                if (o.Name === "UserWithDesignation") {
                    $scope.UserList = o.Data;
                }
            });
        });
    }
    //popup for show audit log 
    $scope.ShowChangeLogPopup = function (transactionId) {
        $scope.TransactionId = transactionId;
        $scope.TableName = 'TriggerPoint';
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

var AddTriggerPointCtrl = function ($scope, common, $location, $modalInstance, TriggerPointId, TriggerPointSvc) {
   
    $scope.InitTriggerPointCtrl = function () {

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
    $scope.setValues = function () {
    }

    $scope.SaveTriggerPoint = function () {
        common.usSpinnerService.spin('spnTriggerPoint');

        TriggerPointSvc.Save($scope.TriggerPoint).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   if ($scope.TriggerPoint.Id != undefined && $scope.TriggerPoint.Id > 0) {
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   else {
                       $scope.Paging.PageNo = 1;
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   common.usSpinnerService.stop('spnTriggerPoint');
                   $scope.ClosePopup();
               }
               else {
                   common.usSpinnerService.stop('spnTriggerPoint');
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnTriggerPoint');
               console.log(error);
           });
    }
    $scope.InitTriggerPointCtrl();
};

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
};