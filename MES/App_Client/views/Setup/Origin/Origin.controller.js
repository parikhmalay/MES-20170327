app.controller('OriginController', ['$rootScope', '$scope', 'common', 'OriginSvc', '$modal', '$filter', function ($rootScope, $scope, common, OriginSvc, $modal, $filter) {
    $scope.Origin = {};
    $scope.OriginList = [];
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;
    $rootScope.PageHeader = ($filter('translate')('_Origins_'));
    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        getOriginList();
    }

    function getOriginList() {
        common.usSpinnerService.spin('spnOrigin');
        OriginSvc.GetOriginList($scope.Paging).then(function (response) {
            common.usSpinnerService.stop('spnOrigin');
            if (response.data.StatusCode == 200) {
                $scope.OriginList = response.data.Result;
                if ($scope.OriginList.length > 0)
                    advanceSearch.close();
                $scope.Paging = response.data.PageInfo;

                $scope.RequestState = ($scope.OriginList.length > 0) ? 1 : 0;

            } else {
                console.log(response.data.ErrorText);
            }
        },
        function (error) {
            common.usSpinnerService.stop('spnOrigin');
            //common.aaNotify.error(error);
        });
    }

    $scope.Edit = function (origin) {
        $scope.ShowPopup();
        $scope.Origin = origin;
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
        getOriginList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        getOriginList();
    };
    $scope.Delete = function (originId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            OriginSvc.Delete(originId).then(
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

    $scope.ShowPopup = function (originId) {
        $scope.Origin = {};
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/Setup/Origin/AddOriginPopup.html?v=' + Version,
            controller: AddOriginCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            resolve: {
                OriginId: function () {
                    return originId;
                }
            }
        });
        modalInstance.result.then(function (originId) {

        }, function () {
        });
    };

    //popup for show audit log 
    $scope.ShowChangeLogPopup = function (transactionId) {
        $scope.TransactionId = transactionId;
        $scope.TableName = 'Origin';
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



var AddOriginCtrl = function ($scope, common, $location, $modalInstance, OriginId, OriginSvc) {
    $scope.InitOriginCtrl = function () {

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
    $scope.SaveOrigin = function () {
        common.usSpinnerService.spin('spnOrigin');

        OriginSvc.Save($scope.Origin).then(
        function (response) {
            if (ShowMessage(common, response.data)) {
                if ($scope.Origin.Id != undefined && $scope.Origin.Id > 0) {
                    $scope.pageChanged($scope.Paging.PageNo);
                }
                else {
                    $scope.Paging.PageNo = 1;
                    $scope.pageChanged($scope.Paging.PageNo);
                }
                common.usSpinnerService.stop('spnOrigin');
                //$scope.Id = response.data.Result // Id of latest created record
                $scope.ClosePopup();
            }
            else {
                common.usSpinnerService.stop('spnOrigin');
            }
        },
       function (error) {
           common.usSpinnerService.stop('spnOrigin');
       });
    }
    $scope.InitOriginCtrl();
};

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
};