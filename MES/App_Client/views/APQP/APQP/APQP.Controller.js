app.controller('APQPCtrl', ['$rootScope', '$scope', 'common', 'APQPSvc', '$filter', '$modal', 'LookupSvc', 'IdentitySvc', '$routeParams', '$timeout', function ($rootScope, $scope, common, APQPSvc, $filter, $modal, LookupSvc, IdentitySvc, $routeParams, $timeout) {
    //Start implement security role wise
    $scope.setRoleWisePrivilege = function () {
        $scope.currentSecurityObject = currentSecurityObject;
        if (!Isundefinedornull($scope.currentSecurityObject) && $scope.currentSecurityObject.length > 0) {
            angular.forEach($scope.currentSecurityObject, function (obj, index) {
                switch (obj.ObjectId) {
                    case 92:
                        $scope.SecurityRoleCase(obj);
                        break;
                    case 105:
                        $scope.SecurityRoleCase(obj);
                        break;
                    default:
                        break;
                }
            });
        }
        else {
            RedirectToAccessDenied();
        }
    }
    $scope.SecurityRoleCase = function (obj) {
        switch (obj.PrivilegeId) {
            case 1:                           //none
                RedirectToAccessDenied();
                break;
            case 2:                          //read only
                $scope.IsReadOnlyAddNewButton = true;
                break;
            case 3:                         //write
                break;
        }
    };
    //End implement security role wise
    $scope.setRoleWisePrivilege();

    $('body').removeClass('paginationFixedToBottom haveAdvanceSearch');
    $rootScope.PageHeader = ($filter('translate')('_APQP_'));
    $scope.SearchCriteria = {};
    //$scope.dSearchCriteria = {};
    //$scope.SearchCriteria.isFirstTimeLoad = true;
    $scope.Init = function () {

        $scope.SearchCriteria.SearchHeading = ($filter('translate')('_SearchFromCurrentAPQP_'));
        $scope.IsShowFromAPQPRecords = true;
        $scope.IsShowFromSAPRecords = false;
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        $scope.SearchOptionTabs = 'SearchFromCurrentAPQP';
        $scope.SetLooksupData();
        $scope.ClearLocalStorageVariable();
        //if ($scope.SearchCriteria.isFirstTimeLoad == true) {
        //    $scope.GetDefaultSearchCriteria();
        //}
    };

    $scope.SetLooksupData = function () {
        common.usSpinnerService.spin('spnAPQP');
        $scope.LookUps = [
            {
                "Name": "SAM", "Parameters": {
                    "source": "SAM"
                }
            },
            {
                "Name": "APQPStatus", "Parameters": {
                    "source": "APQP"
                }
            },
            {
                "Name": "APQPEngineers", "Parameters": {}
            },
            {
                "Name": "SCUsers", "Parameters": {}
            }
        ];
        $scope.getLookupData();
    };
    $scope.getLookupData = function () {
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            angular.forEach(data.data, function (o) {
                if (o.Name === "SAM") {
                    $scope.DashSAMList = o.Data;
                }
                else if (o.Name === "APQPEngineers") {
                    $scope.DashAPQPQualityEngineerList = o.Data;
                }
                else if (o.Name === "SCUsers") {
                    $scope.DashSupplyChainCoordinatorList = o.Data;
                }
                else if (o.Name === "APQPStatus") {
                    $scope.DashAPQPStatusList = o.Data;
                    //if ($scope.DashAPQPStatusList.length > 0) {
                    //    $scope.SearchCriteria.APQPStatusItems = $filter('filter')($scope.DashAPQPStatusList, function (rw) {
                    //        return rw.Id != 8 && rw.Id != 15
                    //    });
                    //}
                }
            });
            $timeout(function () {
                common.usSpinnerService.stop('spnAPQP');
            }, 0);
        }, function (error) {
            common.usSpinnerService.stop('spnAPQP');
        });
    };

    $scope.GetDefaultSearchCriteria = function () {
        common.usSpinnerService.spin('spnAPQP');
        IdentitySvc.DefaultSearchCriteria().then(
             function (response) {
                 common.usSpinnerService.stop('spnAPQP');
                 if (response.data.StatusCode == 200) {
                     $scope.dSearchCriteria = response.data.Result;
                     $scope.SearchCriteria.SAMUserId = $scope.dSearchCriteria.SAMUserId;
                     $scope.SearchCriteria.APQPQualityEngineerId = $scope.dSearchCriteria.APQPQualityEngineerId;
                     $scope.SearchCriteria.SupplyChainCoordinatorId = $scope.dSearchCriteria.SupplyChainCoordinatorId;

                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
         function (error) {
             common.usSpinnerService.stop('spnAPQP');
             //common.aaNotify.error(error);
         });
    };
    //Start apqp dashboard functions
    $scope.ShowFromAPQPRecords = function () {
        $scope.IsShowFromAPQPRecords = true;
        $scope.IsShowFromSAPRecords = false;
        $scope.SearchCriteria.SearchHeading = ($filter('translate')('_SearchFromCurrentAPQP_'));
    };
    $scope.ShowFromSAPRecords = function () {
        $scope.IsShowFromAPQPRecords = false;
        $scope.IsShowFromSAPRecords = true;
        $scope.SearchCriteria.SearchHeading = ($filter('translate')('_AddNewFromSAPRecords_'));
        try {
            $scope.SAPItemLoadSpinnerText = ($filter('translate')('_SAPItemLoadSpinnerText_'));
            common.usSpinnerService.spin('spnAPQP');
            APQPSvc.GetFromSAPAndInsertInLocalSAPTable().then(
               function (response) {
                   if (ShowMessage(common, response.data)) {
                       common.usSpinnerService.stop('spnAPQP');
                       $scope.SAPItemLoadSpinnerText = "";
                       var result = response.data.Result;
                       if (result == 0)
                           common.aaNotify.error("Error while connecting to the API.");
                   }
                   else {
                       common.usSpinnerService.stop('spnAPQP');
                       $scope.SAPItemLoadSpinnerText = "";
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnAPQP');
                   $scope.SAPItemLoadSpinnerText = "";
                   console.log(error);
               });
        }
        catch (e) {
            common.usSpinnerService.stop('spnAPQP');
            common.aaNotify.error(e);
            $scope.SAPItemLoadSpinnerText = "";
        }
    };
    $scope.Search = function () {
        if ($scope.IsShowFromSAPRecords)
            $scope.SearchFromSAPRecords();
        else if ($scope.IsShowFromAPQPRecords)
            $scope.SearchAPQPItemList();
    };
    $scope.SearchFromSAPRecords = function () {
        $scope.SAPRecordsList = {};
        common.usSpinnerService.spin('spnAPQP');
        APQPSvc.SearchFromSAPRecords($scope.Paging).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   common.usSpinnerService.stop('spnAPQP');
                   $scope.SAPRecordsList = response.data.Result;
                   $scope.ShowPopupSAPRecords();
               }
               else {
                   common.usSpinnerService.stop('spnAPQP');
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnAPQP');
               console.log(error);
           });
    };
    $scope.ShowPopupSAPRecords = function () {
        common.usSpinnerService.spin('spnAPQP');
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/APQP/APQP/APQPSAPRecordsPopup.html?v=' + Version,
            controller: AddSAPRecordsCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            sizeclass: 'modal-fitScreen',
            resolve: {
            }
        });
        modalInstance.result.then(function () {

        }, function () {
        });
    };
    //End apqp dashboard functions

    //Start apqp item list page functions
    $scope.SearchAPQPItemList = function () {
        $scope.APQPList = {};
        var APQPStatusIds = [];
        angular.forEach($scope.SearchCriteria.APQPStatusItems, function (item, index) {
            if (!IsUndefinedNullOrEmpty(item.Id))
                APQPStatusIds.push(item.Id);
        });
        $scope.SearchCriteria.APQPStatusIds = APQPStatusIds.join(",");
        common.usSpinnerService.spin('spnAPQP');
        $scope.Paging.Criteria = $scope.SearchCriteria;
        APQPSvc.GetAPQPList($scope.Paging).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.APQPList = response.data.Result;
                   common.usSpinnerService.stop('spnAPQP');
                   if ($scope.APQPList.length > 0) {
                       $scope.GoToListPage("APQPDashboard");
                   }
                   else {
                       common.aaNotify.error("There are no APQP Item(s) found.");
                   }
               }
               else {
                   common.usSpinnerService.stop('spnAPQP');
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnAPQP');
           });
    };
    $scope.GoToListPage = function (callFrom) {
        if (!IsUndefinedNullOrEmpty(callFrom)) {
            common.usSpinnerService.spin('spnAPQP');
            localStorage.setItem("APQPListPageSearchCriteria", JSON.stringify($scope.SearchCriteria));
            common.$location.path("/APQPItemList/" + callFrom);
        }
    };
    //End apqp item list page functions

    //Start logic for checkbox select deselect here
    $scope.SelectDeselectAll = function () {
        angular.forEach($scope.SAPRecordsList, function (item) {
            item.chkSelect = $scope.SelectAll;
        });
    };
    $scope.select = function () {
        $scope.SelectAll = true;
        angular.forEach($scope.SAPRecordsList, function (item) {
            if (!item.chkSelect)
                $scope.SelectAll = false;
        });
    };
    //end logic for checkbox select deselect here

    $scope.isActiveSearchOptionTabs = function (route) {
        return ($scope.SearchOptionTabs == route);
    };
    $scope.ResetSearch = function () {
        $scope.SearchCriteria = {};
        $scope.Init();
    }
    $scope.ClearLocalStorageVariable = function () {
        if (localStorage.getItem("PageNumber"))
            localStorage.removeItem("PageNumber");
        if (localStorage.getItem("APQPListPaging"))
            localStorage.removeItem("APQPListPaging");
        if (localStorage.getItem("APQPListPageSearchCriteria"))
            localStorage.removeItem("APQPListPageSearchCriteria");
        if (localStorage.getItem("APQPItemId"))
            localStorage.removeItem("APQPItemId");
        if (localStorage.getItem("APQPStep"))
            localStorage.removeItem("APQPStep");
    };
    $scope.Init();
}]);

var AddSAPRecordsCtrl = function ($scope, common, $location, $modalInstance, $timeout, APQPSvc) {
    $scope.SAPRecordsInit = function () {
        $timeout(function () {
            common.usSpinnerService.stop('spnAPQP');
        }, 0);
    };
    $scope.InsertFromSAPRecords = function () {
        common.usSpinnerService.spin('spnAPQP');
        var SAPItemIds = [], strSAPItemIds = "";
        angular.forEach($scope.SAPRecordsList, function (item, index) {
            if (item.chkSelect)
                SAPItemIds.push(item.APQPSAPItemId);
        });
        if (SAPItemIds.length > 0)
            strSAPItemIds = SAPItemIds.join(",");
        else {
            common.usSpinnerService.stop('spnAPQP');
            common.aaNotify.error("Please select at least one part.");
            return false;
        }

        APQPSvc.InsertFromSAPRecords(strSAPItemIds).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   common.usSpinnerService.stop('spnAPQP');
                   $scope.Cancel();
                   $scope.SearchAPQPItemList();
               }
               else {
                   common.usSpinnerService.stop('spnAPQP');
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnAPQP');
               console.log(error);
           });
    };

    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    $scope.SAPRecordsInit();
};