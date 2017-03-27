app.controller('UsersCtrl', ['$rootScope', '$scope', 'common', 'IdentitySvc', '$modal', 'LookupSvc', '$filter', function ($rootScope, $scope, common, IdentitySvc, $modal, LookupSvc, $filter) {
    //Start implement security role wise
    $scope.setRoleWisePrivilege = function () {
        $scope.currentSecurityObject = currentSecurityObject;
        if (!Isundefinedornull($scope.currentSecurityObject) && $scope.currentSecurityObject.length > 0) {
            angular.forEach($scope.currentSecurityObject, function (obj, index) {
                switch (obj.ObjectId) {
                    case 3:
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
                    case 4:
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
    $scope.User = [];
    $scope.UserList = [];
    $scope.CurrentUserId = '';
    $scope.User.RoleId = 1;
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;
    $scope.SelectAll = false;
    $scope.SearchCriteria.Active = null;
    $rootScope.PageHeader = ($filter('translate')('_UserList_'));
    $scope.Init = function () {
        //$scope.GetCurrentUserInfo();
        $scope.StatusList = [{ 'name': '--Select--', 'value': null }
                        , { 'name': 'Active', 'value': true }
                        , { 'name': 'Inactive', 'value': false }];
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        $scope.GetUserList();
        $scope.GetCurrentUserId();
    };

    $scope.GetCurrentUserId = function () {
        $scope.LookUps = [

          {
              "Name": "CurrentUser", "Parameters": {},
            
          },
          {
              "Name": "Roles", "Parameters": {}
          }
        ];
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            angular.forEach(data.data, function (o) {
                if (o.Name === "CurrentUser") {
                    if (o.Data.length > 0)
                        $scope.CurrentUserId = o.Data[0].Name;
                }
                else if (o.Name === "Roles") {
                    $scope.RoleNameList = o.Data;
                }
            });
        });
    };

    $scope.GetUserList = function () {
        common.usSpinnerService.spin('spnUsers');
        IdentitySvc.GetUserList($scope.Paging).then(
             function (response) {
                 common.usSpinnerService.stop('spnUsers');
                 if (response.data.StatusCode == 200) {
                     $scope.UserList = response.data.Result;
                     $scope.Paging = response.data.PageInfo;
                     if ($scope.UserList.length > 0)
                         advanceSearch.close();
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
             function (error) {
                 common.usSpinnerService.stop('spnUsers');
                 //common.aaNotify.error(error);
             });
    };

    $scope.Edit = function (Id) {
        common.$location.path("/AddEdit/" + Id);
    };

    $scope.Delete = function (userId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            IdentitySvc.Delete(userId).then(
            function (response) {
                if (ShowMessage(common, response.data)) {
                    $scope.GetUserList();
                }
            },
            function (error) {
                //common.aaNotify.error(error);
            });
        }
    };

    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.GetUserList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.GetUserList();
    };

    $scope.ResetSearch = function () {
        $scope.SearchCriteria = {};
        $scope.Init();
    }

    //Start logic for checkbox select deselect here
    $scope.SelectDeselectAll = function () {
        angular.forEach($scope.UserList, function (item) {
            item.chkSelect = $scope.SelectAll;
        });
    };
    $scope.select = function () {
        $scope.SelectAll = true;
        angular.forEach($scope.UserList, function (item) {
            if (!item.chkSelect)
                $scope.SelectAll = false;
        });
    };
    //end logic for checkbox select deselect here
    //popup for show audit log 
    $scope.ShowChangeLogPopup = function (transactionId) {
        $scope.TransactionId = transactionId;
        $scope.TableName = 'aspnetUsers';
        $scope.SchemaName = 'dbo';
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
    $scope.RedirectToAddEditPage = function () {
        common.$location.path("/AddEdit/");
    }
    $scope.Init();

}]);

app.controller('AddEditUserCtrl', ['$rootScope', '$scope', 'common', 'IdentitySvc', '$modal', 'LookupSvc', '$filter', '$routeParams', '$timeout',
    function ($rootScope, $scope, common, IdentitySvc, $modal, LookupSvc, $filter, $routeParams, $timeout) {
        //Start implement security role wise
        $scope.setRoleWisePrivilege = function () {
            $scope.currentSecurityObject = currentSecurityObject;
            if (!Isundefinedornull($scope.currentSecurityObject) && $scope.currentSecurityObject.length > 0) {
                angular.forEach($scope.currentSecurityObject, function (obj, index) {
                    switch (obj.ObjectId) {
                        case 3:
                            switch (obj.PrivilegeId) {
                                case 1:                           //none
                                    RedirectToAccessDenied();
                                    break;
                            }
                            break;
                        case 4:
                            if ($scope.TransactionMode == "Create") {
                                $scope.PagePrivilegeCase(obj);
                            }
                            break;
                        case 5:
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

        $scope.Init = function () {
            $scope.IsValidUserName = true;
            $scope.User = {};
            $scope.User.Active = true;
            $scope.User.IsRFQCoordinator = false;
            $scope.SetLookupData();
            $scope.User.PrefixId = 0;

            $scope.StatusList = [{ 'name': 'Active', 'value': true }
                            , { 'name': 'Inactive', 'value': false }];

            $timeout(function () {

                if (!IsUndefinedNullOrEmpty($routeParams.Id) && $routeParams.Id != '0') {
                    $scope.TransactionMode = 'Edit';
                    $scope.User.UserId = $routeParams.Id;
                    $scope.getData($routeParams.Id);
                }
                else {
                    $scope.TransactionMode = 'Create';
                }
                $scope.setRoleWisePrivilege();
            }, 1000);
        };

        $scope.CreateUser = function () {
            if ($scope.IsValidUserName) {
                common.usSpinnerService.spin('spnAddEditUser');
                IdentitySvc.Save($scope.User).then(
                   function (response) {
                       if (ShowMessage(common, response.data)) {
                           common.usSpinnerService.stop('spnAddEditUser');
                           $scope.Id = response.data.Result; // Id of latest created record
                           $scope.RedirectToList();
                       }
                       else {
                           common.usSpinnerService.stop('spnAddEditUser');
                       }
                   },
                   function (error) {
                       common.usSpinnerService.stop('spnAddEditUser');
                       console.log(error);
                   });
            }
        };

        $scope.ValidateUserName = function () {
            if ($scope.User != null && !angular.isUndefined($scope.User) && !angular.isUndefined($scope.User.UserName)) {
                if ($scope.TransactionMode == 'Create') {
                    IdentitySvc.UserNameExists($scope.User.UserName).then(function (data) {
                        if (data.data.StatusCode == 200) {
                            $scope.IsValidUserName = !data.data.Result;//If data.data.Result==true then username exist otherwise notexist.
                            if ($scope.IsValidUserName == false)
                                common.aaNotify.danger($filter('translate')('_UsernameAlreadyExist_'));
                        }
                        else if (data.data.StatusCode == 1265) {
                            $scope.IsValidUserName = true;
                        }
                    }, function (data) {
                        $scope.IsValidUserName = false;
                    });
                }
                else { $scope.IsValidUserName = true; }
            }
        };

        $scope.SetLookupData = function () {
            $scope.LookUps = [
              {
                  "Name": "Countries", "Parameters": {}
              },
              {
                  "Name": "Suppliers", "Parameters": {}
              },
              {
                  "Name": "Prefixes", "Parameters": {}
              },
              {
                  "Name": "Genders", "Parameters": {}
              },
              {
                  "Name": "Designations", "Parameters": {}
              },
              {
                  "Name": "CurrentUser", "Parameters": {}
              },
              {
                  "Name": "Roles", "Parameters": {}
              }
            ];
            $scope.getLookupData();
        };

        $scope.getLookupData = function () {
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                angular.forEach(data.data, function (o) {
                    if (o.Name === "Countries") {
                        $scope.CountryList = o.Data;
                    }
                    else if (o.Name === "Suppliers") {
                        $scope.SupplierList = o.Data;
                    }
                    else if (o.Name === "Prefixes") {
                        $scope.PrefixList = o.Data;
                    }
                    else if (o.Name === "Genders") {
                        $scope.GenderList = o.Data;
                    }
                    else if (o.Name === "Designations") {
                        $scope.DesignationList = o.Data;
                    }
                    else if (o.Name === "CurrentUser") {
                        if (o.Data.length > 0)
                            $scope.UserId = o.Data[0].Name;
                    }
                    else if (o.Name === "Roles") {
                        $scope.RoleList = o.Data;
                    }
                });
            });
        };

        $scope.getData = function (Id) {
            common.usSpinnerService.spin('spnAddEditUser');
            IdentitySvc.getData(Id).then(
               function (response) {
                   if (ShowMessage(common, response.data)) {
                       common.usSpinnerService.stop('spnAddEditUser');
                       $scope.User = response.data.Result;
                   }
                   else {
                       common.usSpinnerService.stop('spnAddEditUser');
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnAddEditUser');
                   console.log(error);
               });
        };

        $scope.RedirectToList = function () {
            common.$location.path("/");
        };

        $scope.ResetForm = function () {
            common.$route.reload();
        };

        $scope.Init();
    }]);