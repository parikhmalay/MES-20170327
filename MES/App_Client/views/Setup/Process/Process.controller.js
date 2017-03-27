app.controller('ProcessController', ['$rootScope', '$scope', 'common', 'ProcessSvc', '$modal', '$filter', function ($rootScope, $scope, common, ProcessSvc, $modal, $filter) {
    $scope.Process = {};
    $scope.ProcessList = [];
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;
    $rootScope.PageHeader = ($filter('translate')('_ProcessList_'));

    $scope.init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        getProcessList();
    }

    function getProcessList() {
        common.usSpinnerService.spin('spnProcess');
        ProcessSvc.GetProcessList($scope.Paging).then(function (response) {
            common.usSpinnerService.stop('spnProcess');
            if (response.data.StatusCode == 200) {
                $scope.ProcessList = response.data.Result;
                if ($scope.ProcessList.length > 0)
                    advanceSearch.close();
                $scope.Paging = response.data.PageInfo;

                $scope.RequestState = ($scope.ProcessList.length > 0) ? 1 : 0;

            } else {
                console.log(response.data.ErrorText);
            }
        },
        function (error) {
            common.usSpinnerService.stop('spnProcess');
            //common.aaNotify.error(error);
        });
    }

    $scope.Edit = function (process) {
        $scope.ShowPopup();
        $scope.Process = process;
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
        getProcessList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        getProcessList();
    };
    $scope.Delete = function (processId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            ProcessSvc.Delete(processId).then(
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

    $scope.ShowPopup = function (processId) {
        $scope.Process = {};
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/Setup/Process/AddProcessPopup.html?v=' + Version,
            controller: AddProcessCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            resolve: {
                ProcessId: function () {
                    return processId;
                }
            }
        });
        modalInstance.result.then(function (processId) {

        }, function () {
        });
    };

    //popup for show audit log 
    $scope.ShowChangeLogPopup = function (transactionId) {
        $scope.TransactionId = transactionId;
        $scope.TableName = 'Process';
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

    $scope.init();

}]);



var AddProcessCtrl = function ($scope, common, $location, $modalInstance, ProcessId, ProcessSvc) {
    $scope.InitProcessCtrl = function () {

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
    $scope.SaveProcess = function () {
        common.usSpinnerService.spin('spnProcess');

        ProcessSvc.Save($scope.Process).then(
        function (response) {
            if (ShowMessage(common, response.data)) {
                if ($scope.Process.Id != undefined && $scope.Process.Id > 0) {
                    $scope.pageChanged($scope.Paging.PageNo);
                }
                else {
                    $scope.Paging.PageNo = 1;
                    $scope.pageChanged($scope.Paging.PageNo);
                }
                common.usSpinnerService.stop('spnProcess');
                //$scope.Id = response.data.Result // Id of latest created record
                $scope.ClosePopup();
            }
            else {
                common.usSpinnerService.stop('spnProcess');
            }
        },
       function (error) {
           common.usSpinnerService.stop('spnProcess');
       });
    }
    $scope.InitProcessCtrl();
};

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
};