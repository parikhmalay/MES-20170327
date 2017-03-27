app.controller('PreferencesPopupCtrl', ['$scope', '$modal', 'common', function ($scope, $modal, common) {
    $scope.openPreferencePopup = function () {
        common.usSpinnerService.spin('spnUserPreferences');
        var modalPreferencePopup = $modal.open({
            templateUrl: '/App_Client/views/UserManagement/Preferences/Preferences.html?v=' + Version,
            controller: 'PreferencesCtrl',
            keyboard: false,
            backdrop: false,
            scope: $scope,
            resolve: {
            }
            //,sizeclass: 'modal-extra-full modal-fitToScreen'
        });
        modalPreferencePopup.result.then(function () {
        }, function () {
        });
    };
}]);
app.controller('PreferencesCtrl', ['$scope', 'common', '$filter', '$timeout', 'PreferencesSvc', 'LookupSvc', '$modalInstance', '$confirm',
    function ($scope, common, $filter, $timeout, PreferencesSvc, LookupSvc, $modalInstance, $confirm) {
        $scope.Init = function () {
            $scope.PagesizeList = {};
            $scope.DefaultLandingPageList = {};
            $scope.AssignmentUsersList = {};
            $scope.UserPreference = {};
            $scope.UserId = '';
            $scope.SetLooksupData();
        }
        $scope.SetLooksupData = function () {
            $scope.LookUps = [
                {
                    "Name": "Pagesizes", "Parameters": {}
                },
                        {
                            "Name": "DefaultLandingPages", "Parameters": {}
                        },
                        {
                            "Name": "Users", "Parameters": {}
                        },
                        {
                            "Name": "CurrentUser", "Parameters": {}
                        }
            ];
            $scope.getLookupData();
        };
        $scope.getLookupData = function () {

            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                angular.forEach(data.data, function (o) {
                    if (o.Name === "Pagesizes") {
                        $scope.PagesizeList = o.Data;
                    }
                    else if (o.Name === "DefaultLandingPages") {
                        $scope.DefaultLandingPageList = o.Data;
                    }
                    else if (o.Name === "Users") {
                        $scope.AssignmentUsersList = o.Data;
                    }
                    else if (o.Name === "CurrentUser") {
                        if (o.Data.length > 0)
                            $scope.UserId = o.Data[0].Name;
                    }
                });
                $timeout(function () { $scope.getData($scope.UserId); }, 100);
            }, function (error) {
                common.usSpinnerService.stop('spnUserPreferences');
            });
        };
        $scope.SaveUserPreferences = function () {

            $confirm({ text1: ($filter('translate')('_PreferenceConfirmText_')), ok: ($filter('translate')('_PreferenceYes_')), cancel: ($filter('translate')('_PreferenceNo_')) })
                .then(function (ret) {
                    if (ret == 1) {
                        $scope.Cancel();
                        return;
                    }
                    else {
                        $scope.save(0);
                    }
                },
                 function () {
                     $scope.save(1);
                 });
        }
        $scope.save = function (val) {
            common.usSpinnerService.spin('spnUserPreferences');
            $scope.UserPreference.UserId = $scope.UserId;
            PreferencesSvc.Save($scope.UserPreference).then(
               function (response) {
                   common.usSpinnerService.stop('spnUserPreferences');
                   $scope.Cancel();
                   // common.$route.reload();
                   if (val == 0)
                       location.reload(true);
               },
               function (error) {
                   common.usSpinnerService.stop('spnUserPreferences');
                   console.log(error);
               });
        };
        $scope.getData = function (userPreferencesId) {
            common.usSpinnerService.spin('spnUserPreferences');
            PreferencesSvc.getData(userPreferencesId).then(
               function (response) {
                   if (ShowMessage(common, response.data)) {
                       common.usSpinnerService.stop('spnUserPreferences');
                       $scope.UserPreference = response.data.Result;
                       if (IsUndefinedNullOrEmpty(response.data.Result.DefaultLandingPageId)) {
                           $scope.UserPreference.DefaultLandingPageId = 2;
                       }
                   }
                   else {
                       common.usSpinnerService.stop('spnUserPreferences');
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnUserPreferences');
                   console.log(error);
               });
        };
        $scope.Delete = function (userPreferencesId) {
            if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                PreferencesSvc.Delete(userPreferencesId).then(
               function (response) {
                   if (ShowMessage(common, response.data)) {
                       //$scope.getData($scope.UserId);
                   }
               },
               function (error) {
               });
            }
        };
        $scope.Cancel = function () {
            $modalInstance.dismiss('cancel');
        }
        $scope.ClosePopup = function () {
            $modalInstance.close();
        }
        $scope.Init();
    }]);