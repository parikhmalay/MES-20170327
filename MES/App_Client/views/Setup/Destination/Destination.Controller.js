app.controller('DestinationCtrl', ['$rootScope', '$scope', 'common', 'DestinationSvc', '$modal', '$filter', function ($rootScope, $scope, common, DestinationSvc, $modal, $filter) {
    $scope.Destination = {};
    $scope.DestinationList = {};
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;
    $rootScope.PageHeader = ($filter('translate')('_Destinations_'));

    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        $scope.GetDestinationsList();
    };
    $scope.GetDestinationsList = function () {
        common.usSpinnerService.spin('spnDestination');
        DestinationSvc.GetDestinationsList($scope.Paging).then(
             function (response) {
                 common.usSpinnerService.stop('spnDestination');
                 if (response.data.StatusCode == 200) {
                     $scope.DestinationList = response.data.Result;
                     if ($scope.DestinationList.length > 0)
                         advanceSearch.close();

                     $scope.Paging = response.data.PageInfo;
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
             function (error) {
                 common.usSpinnerService.stop('spnDestination');
                 //common.aaNotify.error(error);
             });
    };

    $scope.Edit = function (destination) {
        $scope.ShowPopup();
        $scope.Destination = destination;
    };

    $scope.Delete = function (destinationId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            DestinationSvc.Delete(destinationId).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.GetDestinationsList();
               }
           },
           function (error) {
               //common.aaNotify.error(error);
           });
        }
    };

    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.GetDestinationsList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.GetDestinationsList();
    };

    $scope.ShowPopup = function (destinationId) {
        $scope.Destination = {};
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/Setup/Destination/AddDestinationPopup.html?v=' + Version,
            controller: AddDestinationCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            resolve: {
                DestinationId: function () {
                    return destinationId;
                }
            }
        });
        modalInstance.result.then(function (destinationId) {

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
        $scope.TableName = 'Destination';
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

var AddDestinationCtrl = function ($scope, common, $location, $modalInstance, DestinationId, DestinationSvc) {

    $scope.InitDestinationCtrl = function () {

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
        //$scope.frmAddEdit.$aaFormExtensions.$clearErrors();
        //$scope.$('frmDestination').$setPristine();
        //$scope.form.$setValidity();
        $scope.Destination = {};
    }

    $scope.SaveDestination = function () {
        common.usSpinnerService.spin('spnDestination');
        DestinationSvc.Save($scope.Destination).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   if ($scope.Destination.Id != undefined && $scope.Destination.Id > 0) {
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   else {
                       $scope.Paging.PageNo = 1;
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   common.usSpinnerService.stop('spnDestination');
                   //$scope.Id = response.data.Result // Id of latest created record
                   $scope.ClosePopup();
               }
               else {
                   common.usSpinnerService.stop('spnDestination');
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnDestination');
               console.log(error);
           });
    }
    $scope.InitDestinationCtrl();
};

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
};

//AddDestinationCtrl.$inject = ['$scope', 'common', '$location', '$modalInstance'];
//common.$location.path("/Destination/" + id);