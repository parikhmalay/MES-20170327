app.controller('IndustryTypeController', ['$rootScope', '$scope', 'common', 'IndustryTypeSvc', '$modal', '$filter', function ($rootScope, $scope, common, IndustryTypeSvc, $modal, $filter) {
    $scope.IndustryType = {};
    $scope.IndustryTypeList = [];
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;
    $rootScope.PageHeader = ($filter('translate')('_IndustryTypes_'));
    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        getIndustryTypeList();
    }

    function getIndustryTypeList() {
        common.usSpinnerService.spin('spnIndustryType');
        IndustryTypeSvc.GetIndustryTypeList($scope.Paging).then(function (response) {
            common.usSpinnerService.stop('spnIndustryType');
            if (response.data.StatusCode == 200) {
                $scope.IndustryTypeList = response.data.Result;
                if ($scope.IndustryTypeList.length > 0)
                    advanceSearch.close();
                $scope.Paging = response.data.PageInfo;
                debugger;
                $scope.RequestState = ($scope.IndustryTypeList.length > 0) ? 1 : 0;

            } else {
                console.log(response.data.ErrorText);
            }
        },
        function (error) {
            common.usSpinnerService.stop('spnIndustryType');
            //common.aaNotify.error(error);
        });
    }

    $scope.Edit = function (industryType) {
        $scope.ShowPopup();
        $scope.IndustryType = industryType;
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
        getIndustryTypeList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        getIndustryTypeList();
    };
    $scope.Delete = function (industryTypeId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            IndustryTypeSvc.Delete(industryTypeId).then(
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

    $scope.ShowPopup = function (industryTypeId) {
        $scope.IndustryType = {};
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/Setup/IndustryType/AddIndustryTypePopup.html?v=' + Version,
            controller: AddIndustryTypeCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            resolve: {
                IndustryTypeId: function () {
                    return industryTypeId;
                }
            }
        });
        modalInstance.result.then(function (industryTypeId) {

        }, function () {
        });
    };

    //popup for show audit log 
    $scope.ShowChangeLogPopup = function (transactionId) {
        $scope.TransactionId = transactionId;
        $scope.TableName = 'IndustryType';
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



var AddIndustryTypeCtrl = function ($scope, common, $location, $modalInstance, IndustryTypeId, IndustryTypeSvc) {
    $scope.InitIndustryTypeCtrl = function () {

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
    $scope.SaveIndustryType = function () {
        common.usSpinnerService.spin('spnIndustryType');

        IndustryTypeSvc.Save($scope.IndustryType).then(
        function (response) {
            if (ShowMessage(common, response.data)) {
                if ($scope.IndustryType.Id != undefined && $scope.IndustryType.Id > 0) {
                    $scope.pageChanged($scope.Paging.PageNo);
                }
                else {
                    $scope.Paging.PageNo = 1;
                    $scope.pageChanged($scope.Paging.PageNo);
                }
                common.usSpinnerService.stop('spnIndustryType');
                //$scope.Id = response.data.Result // Id of latest created record
                $scope.ClosePopup();
            }
            else {
                common.usSpinnerService.stop('spnIndustryType');
            }
        },
       function (error) {
           common.usSpinnerService.stop('spnIndustryType');
       });
    }
    $scope.InitIndustryTypeCtrl();
};

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
};