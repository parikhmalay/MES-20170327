app.controller('ChangeRequestListCtrl', ['$rootScope', '$scope', 'common', '$routeParams', 'ChangeRequestSvc', '$filter', '$modal', 'LookupSvc', '$timeout', function ($rootScope, $scope, common, $routeParams, ChangeRequestSvc, $filter, $modal, LookupSvc, $timeout) {
    //Start implement security role wise
    $scope.setRoleWisePrivilege = function () {
        $scope.currentSecurityObject = currentSecurityObject;
        if (!Isundefinedornull($scope.currentSecurityObject) && $scope.currentSecurityObject.length > 0) {
            angular.forEach($scope.currentSecurityObject, function (obj, index) {
                switch (obj.ObjectId) {
                    case 92:
                        switch (obj.PrivilegeId) {
                            case 1:                           //none
                                RedirectToAccessDenied();
                                break;
                        }
                        break;
                    case 106:
                        switch (obj.PrivilegeId) {
                            case 1:                           //none
                                RedirectToAccessDenied();
                                break;
                        }
                        break;
                    case 100:
                        switch (obj.PrivilegeId) {
                            case 1:                           //none
                                RedirectToAccessDenied();
                                break;
                            case 2:
                                $scope.IsReadOnlyDeleteButton = true;
                                $scope.IsReadOnlyAddButton = true;
                                break;
                            case 3:
                                break;
                        }
                        break;
                    case 101:
                        switch (obj.PrivilegeId) {
                            case 1:                           //none
                                $scope.IsReadOnlyAddButton = true;
                                break;
                            case 2:
                                $scope.IsReadOnlyAddButton = true;
                                break;
                            case 3:
                                break;
                        }
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
    //End implement security role wise
    $scope.setRoleWisePrivilege();

    $rootScope.PageHeader = ($filter('translate')('_ChangeRequestList_'));
    $scope.sortReverse = false;
    $scope.allowDeleteRecord = false;
    $scope.Init = function () {
        $scope.SearchCriteria = {};
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        $scope.ChangeRequestList = {};
        $scope.SetLooksupData();
        $timeout(function () {
            if (IsUndefinedNullOrEmpty($routeParams.CallFrom)) {
                $scope.SearchChangeRequestList();
            }
            else if ($routeParams.CallFrom == "ChangeRequestDashboard") {
                if (localStorage.getItem("CRListPageSearchCriteria")) {
                    $scope.$apply(function () {
                        $scope.SearchCriteria = {};
                        $scope.SearchCriteria = JSON.parse(localStorage.getItem("CRListPageSearchCriteria"));
                        console.log($scope.SearchCriteria);
                    });
                }
                $scope.SearchChangeRequestList();
            }
        }, 300);
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
                else if (o.Name === "AssignmentUsers") {
                    $scope.DashAssignToList = o.Data;
                }
                else if (o.Name === "APQPStatus") {
                    $scope.DashStatusList = o.Data;
                    //if ($scope.DashStatusList.length > 0) {
                    //    $scope.SearchCriteria.StatusItems = $filter('filter')($scope.DashStatusList, function (rw) {
                    //        return rw.Id == 14
                    //    });
                    //}
                }
            });
        });
    };
    $scope.Search = function () {
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
            $scope.SearchChangeRequestList();



        } catch (e) {
            common.usSpinnerService.stop('spnAPQP');
            common.aaNotify.error(e);
        }
    };
    $scope.setPartNumber = function (partNumberId) {
        if (!IsUndefinedNullOrEmpty(partNumberId) && $scope.DashPartsList.length > 0) {
            $scope.SearchCriteria.PartNumber = $filter('filter')($scope.DashPartsList, function (rw) { return rw.Id == parseInt(partNumberId, 10) })[0].Name;
        }
        else
            $scope.SearchCriteria.PartNumber = "";
    };

    //Start apqp item list page functions
    $scope.SearchChangeRequestList = function () {
        $scope.ChangeRequestList = {};
        common.usSpinnerService.spin('spnAPQP');
        $scope.Paging.Criteria = $scope.SearchCriteria;
        ChangeRequestSvc.GetChangeRequestList($scope.Paging).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   common.usSpinnerService.stop('spnAPQP');
                   $scope.ChangeRequestList = response.data.Result;
                   angular.forEach($scope.ChangeRequestList, function (o) {
                       o.UpdatedDate = convertUTCDateToLocalDate(o.UpdatedDate);
                   });
                   $scope.Paging = response.data.PageInfo;
                   if ($scope.ChangeRequestList.length > 0) {
                       $scope.allowDeleteRecord = $scope.ChangeRequestList[0].AllowDeleteRecord;
                       advanceSearch.close();
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
    //End apqp item list page functions
    $scope.Edit = function (id) {
        common.$location.path("/AddEdit/" + id + "/0");
    };
    $scope.RedirectToAddEditPage = function () {
        common.$location.path("/AddEdit/");
    }
    $scope.Delete = function (Id) {
        common.usSpinnerService.spin('spnAPQP');
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            ChangeRequestSvc.Delete(Id).then(
           function (response) {
               common.usSpinnerService.stop('spnAPQP');
               if (ShowMessage(common, response.data)) {
                   $scope.SearchChangeRequestList();
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnAPQP');
               //common.aaNotify.error(error);
           });
        }
        common.usSpinnerService.stop('spnAPQP');
    };
    $scope.ResetSearch = function () {
        if (localStorage.getItem("CRListPageSearchCriteria"))
            localStorage.removeItem("CRListPageSearchCriteria");
        $scope.SearchCriteria = {};
        $scope.Init();
    };

    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.Search();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.Search();
    };
    $scope.Init();
}]);