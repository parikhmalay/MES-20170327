app.controller('StatusController', ['$rootScope', '$scope', 'common', 'StatusSvc', 'LookupSvc', '$modal', '$filter', function ($rootScope, $scope, common, StatusSvc, LookupSvc, $modal, $filter) {
    $scope.Status = {};
    $scope.StatusList = {};
    $scope.AssociatedToList = {};
    $scope.SearchCriteria = {};
    $scope.LookUps = [{ "Name": "AssociatedToItems", "Parameters": { "source": "ST" } }];
    $scope.sortReverse = false;
    $rootScope.PageHeader = ($filter('translate')('_StatusList_'));
    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        getStatusList();
    }

    function getStatusList() {
        common.usSpinnerService.spin('spnStatus');
        StatusSvc.GetStatusList($scope.Paging).then(function (response) {
            common.usSpinnerService.stop('spnStatus');
            if (response.data.StatusCode == 200) {
                $scope.StatusList = response.data.Result;
                if ($scope.StatusList.length > 0)
                    advanceSearch.close();
                $scope.Paging = response.data.PageInfo;
                getLookupData();
                $scope.RequestState = ($scope.StatusList.length > 0) ? 1 : 0;

            } else {
                console.log(response.data.ErrorText);
            }
        },
        function (error) {
            common.usSpinnerService.stop('spnStatus');
            //common.aaNotify.error(error);
        });
    }

    $scope.Edit = function (status) {
        $scope.ShowPopup();
        $scope.Status = status;
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
        getStatusList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        getStatusList();
    };
    $scope.Delete = function (statusId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            StatusSvc.Delete(statusId).then(
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

    $scope.ShowPopup = function (statusId) {
        $scope.Status = {};
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/Setup/Status/AddStatusPopup.html?v=' + Version,
            controller: AddStatusCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            resolve: {
                StatusId: function () {
                    return statusId;
                }
            }
        });
        modalInstance.result.then(function (statusId) {

        }, function () {
        });
    };

    function getLookupData() {
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            angular.forEach(data.data, function (o) {
                if (o.Name === "AssociatedToItems") {
                    $scope.AssociatedToList = o.Data;
                }
            });
        });
    }
    //popup for show audit log 
    $scope.ShowChangeLogPopup = function (transactionId) {
        $scope.TransactionId = transactionId;
        $scope.TableName = 'Status';
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


var AddStatusCtrl = function ($scope, common, $location, $modalInstance, StatusId, StatusSvc) {
    $scope.InitStatusCtrl = function () {

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

    $scope.SaveStatus = function () {
        common.usSpinnerService.spin('spnStatus');

        StatusSvc.Save($scope.Status).then(
        function (response) {
            if (ShowMessage(common, response.data)) {
                if ($scope.Status.Id != undefined && $scope.Status.Id > 0) {
                    $scope.pageChanged($scope.Paging.PageNo);
                }
                else {
                    $scope.Paging.PageNo = 1;
                    $scope.pageChanged($scope.Paging.PageNo);
                }
                common.usSpinnerService.stop('spnStatus');
                //$scope.Id = response.data.Result // Id of latest created record
                $scope.ClosePopup();
            }
            else {
                common.usSpinnerService.stop('spnStatus');
            }
        },
       function (error) {
           common.usSpinnerService.stop('spnStatus');
       });
    }
    $scope.InitStatusCtrl();
};

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
};