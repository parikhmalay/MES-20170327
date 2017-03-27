app.controller('ProjectCategoryCtrl', ['$rootScope', '$scope', 'common', 'ProjectCategorySvc', '$modal', '$filter', function ($rootScope, $scope, common, ProjectCategorySvc, $modal, $filter) {
    $scope.ProjectCategory = {};
    $scope.ProjectCategoryList = [];
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;
    $rootScope.PageHeader = ($filter('translate')('_ProjectCategories_'));

    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        $scope.GetProjectCategoryList();
    };
    $scope.GetProjectCategoryList = function () {
        common.usSpinnerService.spin('spnProjectCategory');
        ProjectCategorySvc.GetProjectCategoryList($scope.Paging).then(
             function (response) {
                 common.usSpinnerService.stop('spnProjectCategory');
                 if (response.data.StatusCode == 200) {
                     $scope.ProjectCategoryList = response.data.Result;
                     if ($scope.ProjectCategoryList.length > 0)
                         advanceSearch.close();
                     $scope.Paging = response.data.PageInfo;
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
             function (error) {
                 common.usSpinnerService.stop('spnProjectCategory');
                 //common.aaNotify.error(error);
             });
    };

    $scope.Edit = function (projectCategory) {
        $scope.ShowPopup();
        $scope.ProjectCategory = projectCategory;
    };

    $scope.Delete = function (projectCategoryId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            ProjectCategorySvc.Delete(projectCategoryId).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.GetProjectCategoryList();
               }
           },
           function (error) {
               //common.aaNotify.error(error);
           });
        }
    };

    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.GetProjectCategoryList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.GetProjectCategoryList();
    };

    $scope.ShowPopup = function (projectCategoryId) {
        $scope.ProjectCategory = {};
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/Setup/ProjectCategory/AddProjectCategoryPopup.html?v=' + Version,
            controller: AddProjectCategoryCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            resolve: {
                ProjectCategoryId: function () {
                    return projectCategoryId;
                }
            }
        });
        modalInstance.result.then(function (projectCategoryId) {

        }, function () {
        });
    };

    $scope.ResetSearch = function () {
        $scope.SearchCriteria = {};
        $scope.Init();
    }

    //popup for show audit log 
    $scope.ShowChangeLogPopup = function (transactionId) {
        $scope.TransactionId = transactionId;
        $scope.TableName = 'ProjectCategory';
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

var AddProjectCategoryCtrl = function ($scope, common, $location, $modalInstance, ProjectCategoryId, ProjectCategorySvc) {
    $scope.InitProjectCategoryCtrl = function () {
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
    $scope.SaveProjectCategory = function () {
        common.usSpinnerService.spin('spnProjectCategory');
        ProjectCategorySvc.Save($scope.ProjectCategory).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   if ($scope.ProjectCategory.Id != undefined && $scope.ProjectCategory.Id > 0) {
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   else {
                       $scope.Paging.PageNo = 1;
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   common.usSpinnerService.stop('spnProjectCategory');
                   //$scope.Id = response.data.Result // Id of latest created record
                   $scope.ClosePopup();
               }
               else {
                   common.usSpinnerService.stop('spnProjectCategory');
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnProjectCategory');
               console.log(error);
           });
    }
    $scope.InitProjectCategoryCtrl();
};

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
};
