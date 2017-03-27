app.controller('ChangeRequestCtrl', ['$rootScope', '$scope', 'common', 'ChangeRequestSvc', '$filter', '$modal', 'LookupSvc', 'IdentitySvc', '$routeParams', '$timeout', function ($rootScope, $scope, common, ChangeRequestSvc, $filter, $modal, LookupSvc, IdentitySvc, $routeParams, $timeout) {
    //Start implement security role wise
    $scope.setRoleWisePrivilege = function () {
        $scope.currentSecurityObject = currentSecurityObject;
        if (!Isundefinedornull($scope.currentSecurityObject) && $scope.currentSecurityObject.length > 0) {
            angular.forEach($scope.currentSecurityObject, function (obj, index) {
                switch (obj.ObjectId) {
                    case 92:
                        $scope.SecurityRoleCase(obj);
                        break;
                    case 106:
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
    $rootScope.PageHeader = ($filter('translate')('_ChangeManagementDashboard_'));
    $scope.SearchCriteria = {};

    $scope.dSearchCriteria = {};
    $scope.SearchCriteria.isFirstTimeLoad = true;
    $scope.Init = function () {
        $scope.SearchCriteria.SearchHeading = ($filter('translate')('_SearchFromCurrentAPQP_'));
        $scope.IsShowFromAPQPRecords = true;
        $scope.IsShowFromSAPRecords = false;
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        $scope.SearchOptionTabs = 'SearchFromCurrentAPQP';
        $scope.SetLooksupData();
        if ($scope.SearchCriteria.isFirstTimeLoad == true) {
            $scope.GetDefaultSearchCriteria();
        }
    };

    $scope.SetLooksupData = function () {
        $scope.LookUps = [
            {
                "Name": "Parts", "Parameters": {
                }
            },
            {
                "Name": "APQPStatus", "Parameters": {
                    "source": "CR"
                }
            },
            {
                "Name": "AssignmentUsers", "Parameters": {}
            }
        ];
        $scope.getLookupData();
    };
    $scope.getLookupData = function () {
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            angular.forEach(data.data, function (o) {
                if (o.Name === "Parts") {
                    $scope.DashPartsList = o.Data;
                }
                else if (o.Name === "APQPStatus") {
                    $scope.DashStatusList = o.Data;
                    //if ($scope.DashStatusList.length > 0) {
                    //    $scope.SearchCriteria.StatusItems = $filter('filter')($scope.DashStatusList, function (rw) {
                    //        return rw.Id == 14
                    //    });
                    //}
                }
                else if (o.Name === "AssignmentUsers") {
                    $scope.DashAssignToList = o.Data;
                }
            });
        });
    };

    $scope.GetDefaultSearchCriteria = function () {
        common.usSpinnerService.spin('spnAPQP');
        IdentitySvc.DefaultSearchCriteria().then(
             function (response) {
                 common.usSpinnerService.stop('spnAPQP');
                 if (response.data.StatusCode == 200) {
                     $scope.dSearchCriteria = response.data.Result;
                     $scope.SearchCriteria.AssignedToItems = $scope.dSearchCriteria.AssignedToItems;
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
    //Start CR dashboard functions
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
            ChangeRequestSvc.GetFromSAPAndInsertInLocalSAPTable().then(
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
                   console.log(error);
                   $scope.SAPItemLoadSpinnerText = "";
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
            $scope.SearchCRItemList();
    };
    $scope.SearchFromSAPRecords = function () {
        $scope.SAPRecordsList = {};
        common.usSpinnerService.spin('spnAPQP');
        ChangeRequestSvc.SearchFromSAPRecords($scope.Paging).then(
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
            templateUrl: '/App_Client/views/APQP/ChangeRequest/CRSAPRecordsPopup.html?v=' + Version,
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
    //End CR dashboard functions

    //Start CR item list page functions
    $scope.SearchCRItemList = function () {
        try {
            common.usSpinnerService.spin('spnAPQP');
            var APQPStatusIds = [], AssignedToIds = [];
            angular.forEach($scope.SearchCriteria.StatusItems, function (item, index) {
                if (!IsUndefinedNullOrEmpty(item.Id) && parseInt(item.Id) > 0)
                    APQPStatusIds.push(item.Id);
            });
            $scope.SearchCriteria.StatusIds = APQPStatusIds.join(",");
           
            angular.forEach($scope.SearchCriteria.AssignedToItems, function (item, index) {
                if (!IsUndefinedNullOrEmpty(item.Id))
                    AssignedToIds.push(item.Id);
            });
            $scope.SearchCriteria.AssignedToIds = AssignedToIds.join(",");

            $scope.Paging.Criteria = $scope.SearchCriteria;
            $scope.GoToListPage("ChangeRequestDashboard");
        } catch (e) {
            common.usSpinnerService.stop('spnAPQP');
            common.aaNotify.error(e);
        }
    };
    $scope.GoToListPage = function (callFrom) {
        if (!IsUndefinedNullOrEmpty(callFrom)) {
            common.usSpinnerService.spin('spnAPQP');
            localStorage.setItem("CRListPageSearchCriteria", JSON.stringify($scope.SearchCriteria));
            common.$location.path("/ChangeRequestList/" + callFrom);
        }
    };
    $scope.setPartNumber = function (partNumberId) {
        if (!IsUndefinedNullOrEmpty(partNumberId) && $scope.DashPartsList.length > 0) {
            $scope.SearchCriteria.PartNumber = $filter('filter')($scope.DashPartsList, function (rw) { return rw.Id == parseInt(partNumberId, 10) })[0].Name;
        }
        else
            $scope.SearchCriteria.PartNumber = "";
    };
    $scope.RedirectToAddEditPage = function () {
        common.$location.path("/AddEdit/");
    }
    //End CR item list page functions

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
        //common.$route.reload();
    }
    $scope.Init();
}]);

var AddSAPRecordsCtrl = function ($scope, common, $location, $modalInstance, $timeout, ChangeRequestSvc) {
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
        if (SAPItemIds.length < 1) {
            common.usSpinnerService.stop('spnAPQP');
            common.aaNotify.error("Please select at least one part.");
            return false;
        }
        else if (SAPItemIds.length > 1) {
            common.usSpinnerService.stop('spnAPQP');
            common.aaNotify.error("Please select only one part.");
            return false;
        }
        else if (SAPItemIds.length > 0)
            strSAPItemIds = SAPItemIds.join(",");

        ChangeRequestSvc.InsertFromSAPRecords(strSAPItemIds).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   common.usSpinnerService.stop('spnAPQP');
                   var ChangeRequestId = response.data.Result;
                   if (!isNaN(parseInt(ChangeRequestId)) && parseInt(ChangeRequestId) > 0) {
                       $scope.Cancel();
                       common.$location.path("/AddEdit/" + ChangeRequestId);
                   }
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