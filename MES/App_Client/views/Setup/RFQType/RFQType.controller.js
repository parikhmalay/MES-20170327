app.controller('RFQTypeController', ['$rootScope', '$scope', 'common', 'RFQTypeSvc', '$modal', '$filter', function ($rootScope, $scope, common, RFQTypeSvc, $modal, $filter) {
    $scope.RFQType = {};
    $scope.RFQTypeList = {};
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;
    $rootScope.PageHeader = ($filter('translate')('_RFQTypes_'));
    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        $scope.getRFQTypeList();
    }

    $scope.getRFQTypeList = function () {
        common.usSpinnerService.spin('spnRFQType');
        RFQTypeSvc.GetRFQTypeList($scope.Paging).then(function (response) {
            common.usSpinnerService.stop('spnRFQType');
            if (response.data.StatusCode == 200) {
                $scope.RFQTypeList = response.data.Result;
                if ($scope.RFQTypeList.length > 0)
                    advanceSearch.close();
                $scope.Paging = response.data.PageInfo;
                $scope.RequestState = ($scope.RFQTypeList.length > 0) ? 1 : 0;

            } else {
                console.log(response.data.ErrorText);
            }
        },
        function (error) {
            common.usSpinnerService.stop('spnRFQType');
            //common.aaNotify.error(error);
        });
    }

    $scope.Edit = function (rfqType) {
        $scope.ShowPopup();
        $scope.RFQType = rfqType;
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
        $scope.getRFQTypeList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.getRFQTypeList();
    };
    $scope.Delete = function (rfqTypeId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            RFQTypeSvc.Delete(rfqTypeId).then(
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

    $scope.ShowPopup = function (rfqTypeId) {
        $scope.RFQType = {};
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/Setup/RFQType/AddRFQTypePopup.html?v=' + Version,
            controller: AddRFQTypeCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            resolve: {
                RFQTypeId: function () {
                    return rfqTypeId;
                }
            }
        });
        modalInstance.result.then(function (rfqTypeId) {

        }, function () {
        });
    };

    //popup for show audit log 
    $scope.ShowChangeLogPopup = function (transactionId) {
        $scope.TransactionId = transactionId;
        $scope.TableName = 'RFQType';
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



var AddRFQTypeCtrl = function ($scope, common, $location, $modalInstance, RFQTypeId, RFQTypeSvc) {
    $scope.InitRFQTypeCtrl = function () {

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

    $scope.SaveRFQType = function () {
        common.usSpinnerService.spin('spnRfqType');

        RFQTypeSvc.Save($scope.RFQType).then(
        function (response) {
            if (ShowMessage(common, response.data)) {
                if ($scope.RFQType.Id != undefined && $scope.RFQType.Id > 0) {
                    $scope.pageChanged($scope.Paging.PageNo);
                }
                else {
                    $scope.Paging.PageNo = 1;
                    $scope.pageChanged($scope.Paging.PageNo);
                }
                common.usSpinnerService.stop('spnRfqType');
                //$scope.Id = response.data.Result // Id of latest created record
                $scope.ClosePopup();
            }
            else {
                common.usSpinnerService.stop('spnRfqType');
            }
        },
       function (error) {
           common.usSpinnerService.stop('spnRfqType');
       });
    }
    $scope.InitRFQTypeCtrl();
};

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
};