app.controller('CommodityTypeCtrl', ['$rootScope', '$scope', 'common', 'CommodityTypeSvc', '$modal', 'LookupSvc', '$filter', function ($rootScope, $scope, common, CommodityTypeSvc, $modal, LookupSvc, $filter) {
    $scope.CommodityType = {};
    $scope.CommodityTypeList = {};
    $scope.SupplierList = {};
    $scope.CustomerList = {};
    $scope.SearchCriteria = {};
    $scope.LookUps = [{ "Name": "Suppliers", "Parameters": {} }, { "Name": "Customers", "Parameters": {} }];
    $scope.sortReverse = false;
    $rootScope.PageHeader = ($filter('translate')('_CommodityTypes_'));

    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        $scope.GetCommodityTypesList();
    };

    $scope.GetCommodityTypesList = function () {
        common.usSpinnerService.spin('spnCommodityType');
        CommodityTypeSvc.GetCommodityTypesList($scope.Paging).then(
             function (response) {
                 common.usSpinnerService.stop('spnCommodityType');
                 if (response.data.StatusCode == 200) {
                     $scope.CommodityTypeList = response.data.Result;
                     if ($scope.CommodityTypeList.length > 0)
                         advanceSearch.close();

                     $scope.Paging = response.data.PageInfo;
                     $scope.getLookupData();
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
             function (error) {
                 common.usSpinnerService.stop('spnCommodityType');
                 //common.aaNotify.error(error);
             });
    };

    $scope.Edit = function (commodityType) {
        $scope.ShowPopup();
        $scope.CommodityType = commodityType;
    };

    $scope.Delete = function (commodityTypeId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            CommodityTypeSvc.Delete(commodityTypeId).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.GetCommodityTypesList();
               }
           },
           function (error) {
               //common.aaNotify.error(error);
           });
        }
    };

    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.GetCommodityTypesList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.GetCommodityTypesList();
    };

    $scope.ShowPopup = function (commodityTypeId) {
        $scope.CommodityType = {};
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/Setup/CommodityType/AddCommodityTypePopup.html?v=' + Version,
            controller: AddCommodityTypeCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            resolve: {
                CommodityTypeId: function () {
                    return commodityTypeId;
                }
            }
        });
        modalInstance.result.then(function (commodityTypeId) {

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
                if (o.Name === "Suppliers") {
                    $scope.SupplierList = o.Data;
                }
                if (o.Name === "Customers") {
                    $scope.CustomerList = o.Data;
                }
            });
        });
    }

    //popup for show audit log 
    $scope.ShowChangeLogPopup = function (transactionId) {
        $scope.TransactionId = transactionId;
        $scope.TableName = 'CommodityType';
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

var AddCommodityTypeCtrl = function ($scope, common, $location, $modalInstance, CommodityTypeId, CommodityTypeSvc) {

    $scope.InitCommodityTypeCtrl = function () {
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
    }

    $scope.SaveCommodityType = function () {
        common.usSpinnerService.spin('spnCommodityType');
        CommodityTypeSvc.Save($scope.CommodityType).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   if ($scope.CommodityType.Id != undefined && $scope.CommodityType.Id > 0) {
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   else {
                       $scope.Paging.PageNo = 1;
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   common.usSpinnerService.stop('spnCommodityType');
                   //$scope.Id = response.data.Result // Id of latest created record
                   $scope.ClosePopup();
               }
               else {
                   common.usSpinnerService.stop('spnCommodityType');
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnCommodityType');
               console.log(error);
           });
    }
    $scope.InitCommodityTypeCtrl();
};

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
};