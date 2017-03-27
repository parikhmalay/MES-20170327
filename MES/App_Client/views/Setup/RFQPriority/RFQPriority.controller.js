app.controller('RFQPriorityController', ['$rootScope', '$scope', 'common', 'RFQPrioritySvc', '$modal', '$filter', function ($rootScope, $scope, common, RFQPrioritySvc, $modal, $filter) {
    $scope.RFQPriority = {};
    $scope.RFQPriorityList = [];
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;
    $rootScope.PageHeader = ($filter('translate')('_RFQPrioritys_'));
    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        getRFQPriorityList();
    }

    function getRFQPriorityList() {
        common.usSpinnerService.spin('spnRFQPriority');
        RFQPrioritySvc.GetRFQPriorityList($scope.Paging).then(function (response) {
            common.usSpinnerService.stop('spnRFQPriority');
            if (response.data.StatusCode == 200) {
                $scope.RFQPriorityList = response.data.Result;
                if ($scope.RFQPriorityList.length > 0)
                    advanceSearch.close();
                $scope.Paging = response.data.PageInfo;

                $scope.RequestState = ($scope.RFQPriorityList.length > 0) ? 1 : 0;

            } else {
                console.log(response.data.ErrorText);
            }
        },
        function (error) {
            common.usSpinnerService.stop('spnRFQPriority');
            //common.aaNotify.error(error);
        });
    }

    $scope.Edit = function (rfqPriority) {
        $scope.ShowPopup();
        $scope.RFQPriority = rfqPriority;
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
        getRFQPriorityList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        getRFQPriorityList();
    };
    $scope.Delete = function (rfqPriorityId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            RFQPrioritySvc.Delete(rfqPriorityId).then(
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

    $scope.ShowPopup = function (rfqPriorityId) {
        $scope.RFQPriority = {};
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/Setup/RFQPriority/AddRFQPriorityPopup.html?v=' + Version,
            controller: AddRFQPriorityCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            resolve: {
                RFQPriorityId: function () {
                    return rfqPriorityId;
                }
            }
        });
        modalInstance.result.then(function (rfqPriorityId) {

        }, function () {
        });
    };

    //popup for show audit log 
    $scope.ShowChangeLogPopup = function (transactionId) {
        $scope.TransactionId = transactionId;
        $scope.TableName = 'RFQPriority';
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



var AddRFQPriorityCtrl = function ($scope, common, $location, $modalInstance, RFQPriorityId, RFQPrioritySvc) {
    $scope.InitRFQPriorityCtrl = function () {

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
    $scope.SaveRFQPriority = function () {
        common.usSpinnerService.spin('spnRFQPriority');

        RFQPrioritySvc.Save($scope.RFQPriority).then(
        function (response) {
            if (ShowMessage(common, response.data)) {
                if ($scope.RFQPriority.Id != undefined && $scope.RFQPriority.Id > 0) {
                    $scope.pageChanged($scope.Paging.PageNo);
                }
                else {
                    $scope.Paging.PageNo = 1;
                    $scope.pageChanged($scope.Paging.PageNo);
                }
                common.usSpinnerService.stop('spnRFQPriority');
                //$scope.Id = response.data.Result // Id of latest created record
                $scope.ClosePopup();
            }
            else {
                common.usSpinnerService.stop('spnRFQPriority');
            }
        },
       function (error) {
           common.usSpinnerService.stop('spnRFQPriority');
       });
    }
    $scope.InitRFQPriorityCtrl();
};

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
};