app.controller('RFQSourceController', ['$rootScope', '$scope', 'common', 'RFQSourceSvc', '$modal', '$filter', function ($rootScope, $scope, common, RFQSourceSvc, $modal, $filter) {
    $scope.RFQSource = {};
    $scope.RFQSourceList = {};
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;
    $rootScope.PageHeader = ($filter('translate')('_RFQSourceList_'));
    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        getRFQSourceList();
    }

    function getRFQSourceList() {
        common.usSpinnerService.spin('spnRFQSource');
        RFQSourceSvc.GetRFQSourceList($scope.Paging).then(function (response) {
            common.usSpinnerService.stop('spnRFQSource');
            if (response.data.StatusCode == 200) {
                $scope.RFQSourceList = response.data.Result;
                if ($scope.RFQSourceList.length > 0)
                    advanceSearch.close();
                $scope.Paging = response.data.PageInfo;
                $scope.RequestState = ($scope.RFQSourceList.length > 0) ? 1 : 0;

            } else {
                console.log(response.data.ErrorText);
            }
        },
        function (error) {
            common.usSpinnerService.stop('spnRFQSource');
            //common.aaNotify.error(error);
        });
    }

    $scope.Edit = function (rfqSource) {
        $scope.ShowPopup();
        $scope.RFQSource = rfqSource;
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
        getRFQSourceList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        getRFQSourceList();
    };
    $scope.Delete = function (rfqSourceId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            RFQSourceSvc.Delete(rfqSourceId).then(
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

    $scope.ShowPopup = function (rfqSourceId) {
        $scope.RFQSource = {};
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/Setup/RFQSource/AddRFQSourcePopup.html?v=' + Version,
            controller: AddRFQSourceCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            resolve: {
                RFQSourceId: function () {
                    return rfqSourceId;
                }
            }
        });
        modalInstance.result.then(function (rfqSourceId) {

        }, function () {
        });
    };

    //popup for show audit log 
    $scope.ShowChangeLogPopup = function (transactionId) {
        $scope.TransactionId = transactionId;
        $scope.TableName = 'RFQSource';
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



var AddRFQSourceCtrl = function ($scope, common, $location, $modalInstance, RFQSourceId, RFQSourceSvc) {
    $scope.InitRFQSourceCtrl = function () {

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

    $scope.SaveRFQSource = function () {
        common.usSpinnerService.spin('spnRFQSource');

        RFQSourceSvc.Save($scope.RFQSource).then(
        function (response) {
            if (ShowMessage(common, response.data)) {
                if ($scope.RFQSource.Id != undefined && $scope.RFQSource.Id > 0) {
                    $scope.pageChanged($scope.Paging.PageNo);
                }
                else {
                    $scope.Paging.PageNo = 1;
                    $scope.pageChanged($scope.Paging.PageNo);
                }
                common.usSpinnerService.stop('spnRFQSource');
                //$scope.Id = response.data.Result // Id of latest created record
                $scope.ClosePopup();
            }
            else {
                common.usSpinnerService.stop('spnRFQSource');
            }
        },
       function (error) {
           common.usSpinnerService.stop('spnRFQSource');
       });
    }
    $scope.InitRFQSourceCtrl();
};

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
};