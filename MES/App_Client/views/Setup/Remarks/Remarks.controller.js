app.controller('RemarksController', ['$rootScope', '$scope', 'common', 'RemarksSvc', 'LookupSvc', '$modal', '$filter', function ($rootScope, $scope, common, RemarksSvc, LookupSvc, $modal, $filter) {
    $scope.Remarks = {};
    $scope.RemarksList = {};
    $scope.AssociatedToList = {};
    $scope.SearchCriteria = {};
    $scope.LookUps = [{ "Name": "AssociatedToItems", "Parameters": { "source": "RM" } }];
    $scope.sortReverse = false;
    $rootScope.PageHeader = ($filter('translate')('_RemarkList_'));
    $scope.init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        getRemarksList();
    }

    function getRemarksList() {
        common.usSpinnerService.spin('spnRemarks');
        RemarksSvc.GetRemarksList($scope.Paging).then(function (response) {
            common.usSpinnerService.stop('spnRemarks');
            if (response.data.StatusCode == 200) {
                $scope.RemarksList = response.data.Result;
                if ($scope.RemarksList.length > 0)
                    advanceSearch.close();
                $scope.Paging = response.data.PageInfo;
                getLookupData();
                $scope.RequestState = ($scope.RemarksList.length > 0) ? 1 : 0;

            } else {
                console.log(response.data.ErrorText);
            }
        },
        function (error) {
            common.usSpinnerService.stop('spnRemarks');
            //common.aaNotify.error(error);
        });
    }

    $scope.Edit = function (remarks) {
        $scope.ShowPopup();
        $scope.Remarks = remarks;
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
        getRemarksList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        getRemarksList();
    };
    $scope.Delete = function (remarksId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            RemarksSvc.Delete(remarksId).then(
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

    $scope.ShowPopup = function (remarksId) {
        $scope.Remarks = {};
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/Setup/Remarks/AddRemarksPopup.html?v=' + Version,
            controller: AddRemarksCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            resolve: {
                RemarksId: function () {
                    return remarksId;
                }
            }
        });
        modalInstance.result.then(function (remarksId) {

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
        $scope.TableName = 'Remarks';
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

    $scope.init();

}]);



var AddRemarksCtrl = function ($scope, common, $location, $modalInstance, RemarksId, RemarksSvc) {
    $scope.InitRemarksCtrl = function () {

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
    $scope.SaveRemarks = function () {
        common.usSpinnerService.spin('spnRemarks');

        RemarksSvc.Save($scope.Remarks).then(
        function (response) {
            if (ShowMessage(common, response.data)) {
                if ($scope.Remarks.Id != undefined && $scope.Remarks.Id > 0) {
                    $scope.pageChanged($scope.Paging.PageNo);
                }
                else {
                    $scope.Paging.PageNo = 1;
                    $scope.pageChanged($scope.Paging.PageNo);
                }
                common.usSpinnerService.stop('spnRemarks');
                //$scope.Id = response.data.Result // Id of latest created record
                $scope.ClosePopup();
            }
            else {
                common.usSpinnerService.stop('spnRemarks');
            }
        },
       function (error) {
           common.usSpinnerService.stop('spnRemarks');
       });
    }
    $scope.InitRemarksCtrl();
};

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
};