app.controller('SecondaryOperationDescController', ['$rootScope', '$scope', 'common', 'SecondaryOperationDescSvc', '$modal', '$filter', function ($rootScope, $scope, common, SecondaryOperationDescSvc, $modal, $filter) {
    $scope.SecondaryOperationDesc = {};
    $scope.SecondaryOperationDescList = {};
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;
    $rootScope.PageHeader = ($filter('translate')('_SecondaryOperationDescs_'));
    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        getSecondaryOperationDescList();
    }

    function getSecondaryOperationDescList() {
        common.usSpinnerService.spin('spnSecondaryOperationDesc');
        SecondaryOperationDescSvc.GetSecondaryOperationDescList($scope.Paging).then(function (response) {
            common.usSpinnerService.stop('spnSecondaryOperationDesc');
            if (response.data.StatusCode == 200) {
                $scope.SecondaryOperationDescList = response.data.Result;
                if ($scope.SecondaryOperationDescList.length > 0)
                    advanceSearch.close();
                $scope.Paging = response.data.PageInfo;
                $scope.RequestState = ($scope.SecondaryOperationDescList.length > 0) ? 1 : 0;

            } else {
                console.log(response.data.ErrorText);
            }
        },
        function (error) {
            common.usSpinnerService.stop('spnSecondaryOperationDesc');
            //common.aaNotify.error(error);
        });
    }

    $scope.Edit = function (secondaryOperationDesc) {
        $scope.ShowPopup();
        $scope.SecondaryOperationDesc = secondaryOperationDesc;
    };

    $scope.ResetSearch = function () {
        $scope.SearchCriteria = {};
        $scope.Init();
    }

    $scope.ShowMessage = function (Message) {
        if (Message != null) {
            if (Message.Type == 'Error' || Message.Type == '1') {
                common.aaNotify.error(Message.Description);
                return false;
            }
            else {
                common.aaNotify.success(Message.Description);
                return true;
            }
        }
        else {
            return true;
        }
    };
    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        getSecondaryOperationDescList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        getSecondaryOperationDescList();
    };
    $scope.Delete = function (secondaryOperationDescId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            SecondaryOperationDescSvc.Delete(secondaryOperationDescId).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   common.$location.path("/#");
               }
           },
           function (error) {
               //common.aaNotify.error(error);
           });
        }
    };

    $scope.ShowPopup = function (secondaryOperationDescId) {
        $scope.SecondaryOperationDesc = {};
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/Setup/SecondaryOperationDesc/AddSecondaryOperationDescPopup.html?v=' + Version,
            controller: AddSecondaryOperationDescCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            resolve: {
                SecondaryOperationDescId: function () {
                    return secondaryOperationDescId;
                }
            }
        });
        modalInstance.result.then(function (secondaryOperationDescId) {

        }, function () {
        });
    };

    //popup for show audit log 
    $scope.ShowChangeLogPopup = function (transactionId) {
        $scope.TransactionId = transactionId;
        $scope.TableName = 'SecondaryOperationDesc';
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



var AddSecondaryOperationDescCtrl = function ($scope, common, $location, $modalInstance, SecondaryOperationDescId, SecondaryOperationDescSvc) {
    $scope.InitSecondaryOperationDescCtrl = function () {

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

    $scope.SaveSecondaryOperationDesc = function () {
        common.usSpinnerService.spin('spnSecondaryOperationDesc');

        SecondaryOperationDescSvc.Save($scope.SecondaryOperationDesc).then(
        function (response) {
            if (ShowMessage(common, response.data)) {
                if ($scope.SecondaryOperationDesc.Id != undefined && $scope.SecondaryOperationDesc.Id > 0) {
                    $scope.pageChanged($scope.Paging.PageNo);
                }
                else {
                    $scope.Paging.PageNo = 1;
                    $scope.pageChanged($scope.Paging.PageNo);
                }
                common.usSpinnerService.stop('spnSecondaryOperationDesc');
                //$scope.Id = response.data.Result // Id of latest created record
                $scope.ClosePopup();
            }
            else {
                common.usSpinnerService.stop('spnSecondaryOperationDesc');
            }
        },
       function (error) {
           common.usSpinnerService.stop('spnSecondaryOperationDesc');
       });
    }
    $scope.InitSecondaryOperationDescCtrl();
};

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
};