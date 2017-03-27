app.controller('ProjectStageCtrl', ['$rootScope', '$scope', 'common', 'ProjectStageSvc', '$modal', 'LookupSvc', '$filter', function ($rootScope, $scope, common, ProjectStageSvc, $modal, LookupSvc, $filter) {
    $scope.ProjectStage = {};
    $scope.ProjectStageList = [];
    $scope.ProjectCategoryList = {};
    $scope.SearchCriteria = {};
    $scope.LookUps = [{ "Name": "ProjectCategories", "Parameters": {} }];
    $scope.sortReverse = false;
    $rootScope.PageHeader = ($filter('translate')('_ProjectStages_'));
    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        $scope.GetProjectStagesList();
    };

    $scope.GetProjectStagesList = function () {
        common.usSpinnerService.spin('spnProjectStage');
        ProjectStageSvc.GetProjectStagesList($scope.Paging).then(
             function (response) {
                 common.usSpinnerService.stop('spnProjectStage');
                 if (response.data.StatusCode == 200) {
                     $scope.ProjectStageList = response.data.Result;
                     if ($scope.ProjectStageList.length > 0)
                         advanceSearch.close();
                     $scope.Paging = response.data.PageInfo;
                     $scope.getLookupData();
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
             function (error) {
                 common.usSpinnerService.stop('spnProjectStage');
                 //common.aaNotify.error(error);
             });
    };

    $scope.Edit = function (projectStage) {
        $scope.ShowPopup();
        $scope.ProjectStage = projectStage;
    };

    $scope.Delete = function (projectStageId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            ProjectStageSvc.Delete(projectStageId).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.GetProjectStagesList();
               }
           },
           function (error) {
               //common.aaNotify.error(error);
           });
        }
    };

    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.GetProjectStagesList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.GetProjectStagesList();
    };

    $scope.ShowPopup = function (projectStageId) {
        $scope.ProjectStage = {};
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/Setup/ProjectStage/AddProjectStagePopup.html?v=' + Version,
            controller: AddProjectStageCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            resolve: {
                ProjectStageId: function () {
                    return projectStageId;
                }
            }
        });
        modalInstance.result.then(function (projectStageId) {

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
                if (o.Name === "ProjectCategories") {
                    $scope.ProjectCategoryList = o.Data;
                }
            });
        });
    }

    //popup for show audit log 
    $scope.ShowChangeLogPopup = function (transactionId) {
        $scope.TransactionId = transactionId;
        $scope.TableName = 'ProjectStage';
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

var AddProjectStageCtrl = function ($scope, common, $location, $modalInstance, ProjectStageId, ProjectStageSvc) {
    $scope.InitProjectStageCtrl = function () {
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

    $scope.SaveProjectStage = function () {
        common.usSpinnerService.spin('spnProjectStage');
        ProjectStageSvc.Save($scope.ProjectStage).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   if ($scope.ProjectStage.Id != undefined && $scope.ProjectStage.Id > 0) {
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   else {
                       $scope.Paging.PageNo = 1;
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   common.usSpinnerService.stop('spnProjectStage');
                   //$scope.Id = response.data.Result // Id of latest created record
                   $scope.ClosePopup();
               }
               else {
                   common.usSpinnerService.stop('spnProjectStage');
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnProjectStage');
               console.log(error);
           });
    }
    $scope.InitProjectStageCtrl();
};

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
};