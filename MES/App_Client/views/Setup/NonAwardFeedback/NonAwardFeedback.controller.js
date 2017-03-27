app.controller('NonAwardFeedbackController', ['$rootScope', '$scope', 'common', 'NonAwardFeedbackSvc', '$modal', '$filter', function ($rootScope, $scope, common, NonAwardFeedbackSvc, $modal, $filter) {
    $scope.NonAwardFeedback = {};
    $scope.NonAwardFeedbackList = {};
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;
    $rootScope.PageHeader = ($filter('translate')('_NonAwardFeedbacks_'));
    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        getNonAwardFeedbackList();
    }

    function getNonAwardFeedbackList() {
        common.usSpinnerService.spin('spnNonAwardFeedback');
        NonAwardFeedbackSvc.GetNonAwardFeedbackList($scope.Paging).then(function (response) {
            common.usSpinnerService.stop('spnNonAwardFeedback');
            if (response.data.StatusCode == 200) {
                $scope.NonAwardFeedbackList = response.data.Result;
                if ($scope.NonAwardFeedbackList.length > 0)
                    advanceSearch.close();
                $scope.Paging = response.data.PageInfo;

                $scope.RequestState = ($scope.NonAwardFeedbackList.length > 0) ? 1 : 0;

            } else {
                console.log(response.data.ErrorText);
            }
        },
        function (error) {
            common.usSpinnerService.stop('spnNonAwardFeedback');
            //common.aaNotify.error(error);
        });
    }

    $scope.Edit = function (nonAwardFeedback) {
        $scope.ShowPopup();
        $scope.NonAwardFeedback = nonAwardFeedback;
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
        getNonAwardFeedbackList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        getNonAwardFeedbackList();
    };
    $scope.Delete = function (nonAwardFeedbackId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            NonAwardFeedbackSvc.Delete(nonAwardFeedbackId).then(
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

    $scope.ShowPopup = function (nonAwardFeedbackId) {
        $scope.NonAwardFeedback = {};
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/Setup/NonAwardFeedback/AddNonAwardFeedbackPopup.html?v=' + Version,
            controller: AddNonAwardFeedbackCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            resolve: {
                NonAwardFeedbackId: function () {
                    return nonAwardFeedbackId;
                }
            }
        });
        modalInstance.result.then(function (nonAwardFeedbackId) {

        }, function () {
        });
    };

    //popup for show audit log 
    $scope.ShowChangeLogPopup = function (transactionId) {
        $scope.TransactionId = transactionId;
        $scope.TableName = 'NonAwardFeedback';
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

var AddNonAwardFeedbackCtrl = function ($scope, common, $location, $modalInstance, NonAwardFeedbackId, NonAwardFeedbackSvc) {
    $scope.InitNonAwardFeedbackCtrl = function () {

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
    $scope.SaveNonAwardFeedback = function () {
        common.usSpinnerService.spin('spnNonAwardFeedback');

        NonAwardFeedbackSvc.Save($scope.NonAwardFeedback).then(
        function (response) {
            if (ShowMessage(common, response.data)) {
                if ($scope.NonAwardFeedback.Id != undefined && $scope.NonAwardFeedback.Id > 0) {
                    $scope.pageChanged($scope.Paging.PageNo);
                }
                else {
                    $scope.Paging.PageNo = 1;
                    $scope.pageChanged($scope.Paging.PageNo);
                }
                common.usSpinnerService.stop('spnNonAwardFeedback');
                //$scope.Id = response.data.Result // Id of latest created record
                $scope.ClosePopup();
            }
            else {
                common.usSpinnerService.stop('spnNonAwardFeedback');
            }
        },
       function (error) {
           common.usSpinnerService.stop('spnNonAwardFeedback');
       });
    }
    $scope.InitNonAwardFeedbackCtrl();
};

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
};