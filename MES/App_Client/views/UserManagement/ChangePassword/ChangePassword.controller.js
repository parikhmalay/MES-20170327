app.controller('changepasswordPopupCtrl', ['$scope', '$modal', function ($scope, $modal) {
    $scope.openPopup = function () {
        var modalPreferencePopup = $modal.open({
            templateUrl: '/App_Client/views/UserManagement/ChangePassword/ChangePassword.html?v=' + Version,
            controller: 'changepasswordCtrl',
            keyboard: false,
            backdrop: false,
            scope: $scope,
            resolve: {
            }
        });
        modalPreferencePopup.result.then(function () {
        }, function () {
        });
    };
}]);

app.controller('changepasswordCtrl', ['$scope', 'common', 'IdentitySvc', '$modalInstance', '$filter', function ($scope, common, IdentitySvc, $modalInstance, $filter) {
    $scope.ChangePassword = { Password: '', ConfirmPassword: '' };
    $scope.Init = function () {
    }
    $scope.SetChangePassword = function () {
        if ($scope.ChangePassword != null && !angular.isUndefined($scope.ChangePassword) && $scope.ChangePassword.Password != '') {
            common.usSpinnerService.spin('spnChangePassword');
            IdentitySvc.ChangePassword($scope.ChangePassword).then(
                function (response) {
                    common.usSpinnerService.stop('spnChangePassword');
                    if (ShowMessage(common, response.data)) {
                        //$scope.ChangePassword = response.data.Result;
                    }
                },
                function (error) {
                    common.usSpinnerService.stop('spnChangePassword');
                    //common.aaNotify.error(error);
                });
        };
    }


    $scope.ClearData = function () {
        $scope.ChangePassword.Password = $scope.ChangePassword.ConfirmPassword = "";
    }

    $scope.CheckConfirmPassword = function (isSave) {
        if ($scope.ChangePassword != null && !angular.isUndefined($scope.ChangePassword)) {
            if (!angular.isUndefined($scope.ChangePassword.Password) && !angular.isUndefined($scope.ChangePassword.ConfirmPassword) && $scope.ChangePassword.Password != "" && $scope.ChangePassword.ConfirmPassword != "" && $scope.ChangePassword.Password != $scope.ChangePassword.ConfirmPassword)
                //    common.aaNotify.error('New Password and Confirm New Password do not match.');//
                common.aaNotify.error($filter('translate')('_ConfirmpasswordNotMatch_'));
            else {
                if (isSave == true) {
                    $scope.SetChangePassword();
                }
            }
        }
    }
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
    $scope.ClosePopup = function () {
        $modalInstance.close();
    }
    $scope.Init();
}]);
