app.controller('RolesCtrl', ['$rootScope', '$scope', 'common', 'RolesSvc', 'LookupSvc', '$modal', '$filter',
    function ($rootScope, $scope, common, RolesSvc, LookupSvc, $modal, $filter) {
        //Start implement security role wise
        $scope.setRoleWisePrivilege = function () {
            $scope.currentSecurityObject = currentSecurityObject;
            if (!Isundefinedornull($scope.currentSecurityObject) && $scope.currentSecurityObject.length > 0) {
                angular.forEach($scope.currentSecurityObject, function (obj, index) {
                    switch (obj.ObjectId) {
                        case 6:
                            switch (obj.PrivilegeId) {
                                case 1:                           //none
                                    RedirectToAccessDenied();
                                    break;
                                case 2:                          //read only
                                    $scope.IsReadOnlyDeleteButton = true;
                                    $scope.IsReadOnlyAddButton = true;
                                    break;
                                case 3:                         //write
                                    break;
                            }
                            break;
                        case 7:
                            switch (obj.PrivilegeId) {
                                case 1:                           //none
                                    $scope.IsReadOnlyAddButton = true;
                                    break;
                                case 2:                          //read only
                                    $scope.IsReadOnlyAddButton = true;
                                    break;
                                case 3:                         //write
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
        $scope.RoleList = [];
        $scope.SearchCriteria = {};
        $scope.sortReverse = false;
        $scope.SelectAll = false;

        $rootScope.PageHeader = ($filter('translate')('_RoleList_'));

        $scope.Init = function () {
            $scope.Paging = GetDefaultPageObject();
            $scope.Paging.Criteria = $scope.SearchCriteria;
            $scope.SetLooksupData();
            $scope.StatusList = [{ 'name': 'Both Status', 'value': null }
                            , { 'name': 'Active', 'value': true }
                            , { 'name': 'Inactive', 'value': false }];

            $scope.GetRoleList();

        }
        $scope.GetRoleList = function () {
            common.usSpinnerService.spin('spnRole');
            RolesSvc.GetRoleList($scope.Paging).then(
                 function (response) {
                     common.usSpinnerService.stop('spnRole');

                     if (response.data.StatusCode == 200) {
                         $scope.RoleList = response.data.Result;

                         if ($scope.RoleList.length > 0)
                             advanceSearch.close();

                         $scope.Paging = response.data.PageInfo;
                     }
                     else {
                         console.log(response.data.ErrorText);
                     }
                 },
                 function (error) {
                     common.usSpinnerService.stop('spnRole');
                     //common.aaNotify.error(error);
                 });
        };

        $scope.Edit = function (Id) {

            common.$location.path("/AddEdit/" + Id);
        };

        $scope.Delete = function (Id) {
            if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                RolesSvc.Delete(Id).then(
               function (response) {
                   if (ShowMessage(common, response.data)) {
                       $scope.GetRoleList();
                   }
               },
               function (error) {
                   //common.aaNotify.error(error);
               });
            }
        };
        $scope.SetLooksupData = function () {
            $scope.LookUps = [
                {
                    "Name": "Roles", "Parameters": {}
                }
            ];
            $scope.getLookupData();
        };
        $scope.getLookupData = function () {
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                angular.forEach(data.data, function (o) {
                    if (o.Name === "Roles") {
                        $scope.RoleNameList = o.Data;
                    }
                });
            });
        };
        $scope.pageSizeChanged = function (PageSize) {
            $scope.Paging.PageSize = PageSize;
            $scope.GetRoleList();
        };

        $scope.pageChanged = function (PageNo) {
            $scope.Paging.PageNo = PageNo;
            $scope.GetRoleList();
        };

        $scope.ResetSearch = function () {
            $scope.SearchCriteria = {};
            $scope.Init();
        }

        $scope.RedirectToAddEditPage = function () {
            common.$location.path("/AddEdit/");
        }
        $scope.Init();

    }]);


app.controller('AddEditRolesCtrl', ['$rootScope', '$scope', 'common', 'RolesSvc', '$modal', 'LookupSvc', '$filter', '$routeParams', '$timeout',
    function ($rootScope, $scope, common, RolesSvc, $modal, LookupSvc, $filter, $routeParams, $timeout) {
        //Start implement security role wise
        $scope.setRoleWisePrivilege = function () {
            $scope.currentSecurityObject = currentSecurityObject;
            if (!Isundefinedornull($scope.currentSecurityObject) && $scope.currentSecurityObject.length > 0) {
                angular.forEach($scope.currentSecurityObject, function (obj, index) {
                    switch (obj.ObjectId) {
                        case 6:
                            switch (obj.PrivilegeId) {
                                case 1:                           //none
                                    RedirectToAccessDenied();
                                    break;
                            }
                            break;
                        case 7:
                            if ($scope.TransactionMode == "Create") {
                                $scope.PagePrivilegeCase(obj);
                            }
                            break;
                        case 8:
                            if ($scope.TransactionMode == "Edit") {
                                $scope.PagePrivilegeCase(obj);
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
        $scope.PagePrivilegeCase = function (obj) {
            switch (obj.PrivilegeId) {
                case 1:                           //none
                    RedirectToAccessDenied();
                    break;
                case 2:                          //read only
                    $scope.IsReadOnlyPage = true;
                    break;
                case 3:                         //write
                    $scope.IsReadOnlyPage = false;
                    break;
            }
        }
        //End implement security role wise

        $rootScope.PageHeader = ($filter('translate')('_PageHeading_'));
        $scope.SecurityObjectInfo = {};
        $scope.DefaultLandingPageList = {};
        $scope.Role = {};
        $scope.StatusList = [{ 'name': 'Active', 'value': true }
                        , { 'name': 'Inactive', 'value': false }];

        $scope.Init = function () {

            $scope.SetLooksupData();

            $timeout(function () {
                if (!IsUndefinedNullOrEmpty($routeParams.Id) && $routeParams.Id != '0') {
                    $scope.TransactionMode = 'Edit';
                    $scope.Role.Id = $routeParams.Id;
                    $scope.GetRoleData($scope.Role.Id);
                }
                else {
                    $scope.TransactionMode = 'Create';

                    $scope.GetRoleData(0);

                }
                $scope.setRoleWisePrivilege();
            });
        };

        $scope.GetRoleData = function (Id) {
            common.usSpinnerService.spin('spnAddEditRole');
            RolesSvc.getData(Id).then(
                 function (response) {
                     common.usSpinnerService.stop('spnAddEditRole');
                     if (response.data.StatusCode == 200) {
                         $scope.Role = response.data.Result;
                         if ($scope.TransactionMode == 'Create') {

                         }
                     }
                     else {
                         console.log(response.data.ErrorText);
                     }
                 },
                 function (error) {
                     common.usSpinnerService.stop('spnAddEditRole');
                     //common.aaNotify.error(error);
                 });
        };
        $scope.SaveRole = function (closeForm) {
            common.usSpinnerService.spin('spnAddEditRole');
            RolesSvc.Save($scope.Role).then(
               function (response) {
                   if (ShowMessage(common, response.data)) {
                       common.usSpinnerService.stop('spnAddEditRole');
                       $scope.Role.Id = response.data.Result; // Id of latest created record    

                       if (closeForm) { $scope.RedirectToList(); }
                       else {
                           if ($scope.TransactionMode == 'Create')
                               $scope.reloadWithId();
                           else
                               $scope.ResetForm();
                       }
                   }
                   else {
                       common.usSpinnerService.stop('spnAddEditRole');
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnAddEditRole');
                   console.log(error);
               });
        };
        $scope.SetLooksupData = function () {
            $scope.LookUps = [
                  {
                      "Name": "DefaultLandingPages", "Parameters": {}
                  },
                  {
                      "Name": "Users", "Parameters": {}
                  }
            ];
            $scope.getLookupData();
        };

        $scope.getLookupData = function () {
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                angular.forEach(data.data, function (o) {
                    if (o.Name === "DefaultLandingPages") {
                        $scope.DefaultLandingPageList = o.Data;
                    }
                    else if (o.Name === "Users") {
                        $scope.UserList = o.Data;
                    }
                });
            });
        };

        $scope.ValidateRoleName = function () {
            if ($scope.Role != null && !angular.isUndefined($scope.Role) && $scope.Role.RoleName != '' && !angular.isUndefined($scope.Role.RoleName)) {
                if ($scope.TransactionMode == 'Create') {
                    RolesSvc.RoleNameExists($scope.Role.RoleName).then(function (data) {
                        if (data.data.StatusCode == 200) {
                            $scope.IsValidRoleName = !data.data.Result;//If data.data.Result==true then rolename exist otherwise notexist.
                            if ($scope.IsValidRoleName == false)
                                common.aaNotify.danger($filter('translate')('_RolenameAlreadyExist_'));
                        }
                        else if (data.data.StatusCode == 1265) {
                            $scope.IsValidRoleName = true;
                        }
                    }, function (data) {
                        $scope.IsValidRoleName = false;
                    });
                }
                else { $scope.IsValidRoleName = true; }
            }

        };
        $scope.reloadWithId = function () {
            common.$location.path('/AddEdit/' + $scope.Role.Id + "/0");
        };
        $scope.RedirectToList = function () {
            common.$location.path("/");
        };

        $scope.ResetForm = function () {
            common.$route.reload();
        };
        $scope.Init();
    }]);