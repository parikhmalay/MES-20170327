app.controller('ForwarderCtrl', ['$rootScope', '$scope', 'common', 'ForwarderSvc', '$modal', '$filter', function ($rootScope, $scope, common, ForwarderSvc, $modal, $filter) {
    $scope.Forwarder = {};
    $scope.ForwarderList = [];
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;
    $rootScope.PageHeader = ($filter('translate')('_Forwarders_'));

    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        $scope.GetForwarderList();
    };
    $scope.GetForwarderList = function () {
        common.usSpinnerService.spin('spnForwarder');
        ForwarderSvc.GetForwarderList($scope.Paging).then(
             function (response) {
                 common.usSpinnerService.stop('spnForwarder');
                 if (response.data.StatusCode == 200) {
                     $scope.ForwarderList = response.data.Result;
                     if ($scope.ForwarderList.length > 0)
                         advanceSearch.close();
                     $scope.Paging = response.data.PageInfo;
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
             function (error) {
                 common.usSpinnerService.stop('spnForwarder');
                 //common.aaNotify.error(error);
             });
    };

    $scope.Edit = function (forwarder) {
        $scope.ShowPopup();
        $scope.Forwarder = forwarder;
    };

    $scope.Delete = function (forwarderId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            ForwarderSvc.Delete(forwarderId).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.GetForwarderList();
               }
           },
           function (error) {
               //common.aaNotify.error(error);
           });
        }
    };

    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.GetForwarderList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.GetForwarderList();
    };

    $scope.ShowPopup = function (forwarderId) {
        $scope.Forwarder = {};
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/Setup/Forwarder/AddForwarderPopup.html?v=' + Version,
            controller: AddForwarderCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            resolve: {
                ForwarderId: function () {
                    return forwarderId;
                }
            }
        });
        modalInstance.result.then(function (forwarderId) {

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
        $scope.TableName = 'Forwarder';
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

var AddForwarderCtrl = function ($scope, common, $location, $modalInstance, ForwarderId, ForwarderSvc) {
    $scope.InitForwarderCtrl = function () {
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
    $scope.SaveForwarder = function () {
        common.usSpinnerService.spin('spnForwarder');
        ForwarderSvc.Save($scope.Forwarder).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   if ($scope.Forwarder.Id != undefined && $scope.Forwarder.Id > 0) {
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   else {
                       $scope.Paging.PageNo = 1;
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   common.usSpinnerService.stop('spnForwarder');
                   //$scope.Id = response.data.Result // Id of latest created record
                   $scope.ClosePopup();
               }
               else {
                   common.usSpinnerService.stop('spnForwarder');
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnForwarder');
               console.log(error);
           });
    }
    $scope.InitForwarderCtrl();
};

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
};