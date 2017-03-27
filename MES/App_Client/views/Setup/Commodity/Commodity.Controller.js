app.controller('CommodityCtrl', ['$rootScope', '$scope', 'common', 'CommoditySvc', '$modal', 'LookupSvc', '$filter', function ($rootScope, $scope, common, CommoditySvc, $modal, LookupSvc, $filter) {
    $scope.Commodity = {};
    $scope.CommodityList = {};
    $scope.CategoryList = {};
    $scope.SearchCriteria = {};
    $scope.LookUps = [{ "Name": "Categories", "Parameters": {} }];
    $scope.sortReverse = false;
    $rootScope.PageHeader = ($filter('translate')('_Commodities_'));

    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        $scope.GetCommodityList();
    };

    $scope.GetCommodityList = function () {
        common.usSpinnerService.spin('spnCommodity');
        CommoditySvc.GetCommodityList($scope.Paging).then(
             function (response) {
                 common.usSpinnerService.stop('spnCommodity');
                 if (response.data.StatusCode == 200) {
                     $scope.CommodityList = response.data.Result;
                     if ($scope.CommodityList.length > 0)
                         advanceSearch.close();

                     $scope.Paging = response.data.PageInfo;
                     $scope.getLookupData();
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
             function (error) {
                 common.usSpinnerService.stop('spnCommodity');
                 //common.aaNotify.error(error);
             });
    };

    $scope.Edit = function (commodity) {
        $scope.ShowPopup();
        $scope.Commodity = commodity;
    };

    $scope.Delete = function (commodityId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            CommoditySvc.Delete(commodityId).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.GetCommodityList();
               }
           },
           function (error) {
               //common.aaNotify.error(error);
           });
        }
    };

    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.GetCommodityList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.GetCommodityList();
    };

    $scope.ShowPopup = function (commodityId) {
        $scope.Commodity = {};
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/Setup/Commodity/AddCommodityPopup.html?v=' + Version,
            controller: AddCommodityCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            resolve: {
                CommodityId: function () {
                    return commodityId;
                }
            }
        });
        modalInstance.result.then(function (commodityId) {

        }, function () {
        });
    };

    $scope.ResetSearch = function () {
        $scope.SearchCriteria = {};
        $scope.Init();
    }

    $scope.getLookupData = function () {
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            angular.forEach(data.data, function (o) {
                if (o.Name === "Categories") {
                    $scope.CategoryList = o.Data;
                }
            });
        });
    }

    //popup for show audit log 
    $scope.ShowChangeLogPopup = function (transactionId) {
        $scope.TransactionId = transactionId;
        $scope.TableName = 'Commodity';
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

var AddCommodityCtrl = function ($scope, common, $location, $modalInstance, CommodityId, CommoditySvc) {

    $scope.InitCommodityCtrl = function () {
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
    }
    $scope.setValues = function () {
    }

    $scope.SaveCommodity = function () {
        common.usSpinnerService.spin('spnCommodity');
        CommoditySvc.Save($scope.Commodity).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   if ($scope.Commodity.Id != undefined && $scope.Commodity.Id > 0) {
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   else {
                       $scope.Paging.PageNo = 1;
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   common.usSpinnerService.stop('spnCommodity');
                   $scope.ClosePopup();
               }
               else {
                   common.usSpinnerService.stop('spnCommodity');
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnCommodity');
               console.log(error);
           });
    }
    $scope.InitCommodityCtrl();
};

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
};
