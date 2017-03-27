app.controller('EmailTemplateCtrl', ['$rootScope', '$scope', 'common', 'EmailTemplateSvc', '$modal', '$filter', function ($rootScope, $scope, common, EmailTemplateSvc, $modal, $filter) {
    $scope.EmailTemplate = {};
    $scope.EmailTemplateList = {};
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;
    $rootScope.PageHeader = ($filter('translate')('_EmailTemplates_'));

    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        $scope.GetEmailTemplateList();
    };
    $scope.GetEmailTemplateList = function () {
        common.usSpinnerService.spin('spnEmailTemplate');
        EmailTemplateSvc.GetEmailTemplateList($scope.Paging).then(
             function (response) {
                 common.usSpinnerService.stop('spnEmailTemplate');
                 if (response.data.StatusCode == 200) {
                     $scope.EmailTemplateList = response.data.Result;
                     if ($scope.EmailTemplateList.length > 0)
                         advanceSearch.close();
                     $scope.Paging = response.data.PageInfo;
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
             function (error) {
                 common.usSpinnerService.stop('spnEmailTemplate');
                 //common.aaNotify.error(error);
             });
    };

    $scope.Edit = function (emailTemplate) {
        $scope.ShowPopup();
        $scope.EmailTemplate = emailTemplate;
    };

    $scope.Delete = function (emailTemplateId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            EmailTemplateSvc.Delete(emailTemplateId).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.GetEmailTemplateList();
               }
           },
           function (error) {
               //common.aaNotify.error(error);
           });
        }
    };

    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.GetEmailTemplateList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.GetEmailTemplateList();
    };

    $scope.ShowPopup = function (emailTemplateId) {
        $scope.EmailTemplate = {};
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/Setup/EmailTemplate/AddEmailTemplatePopup.html?v=' + Version,
            controller: AddEmailTemplateCtrl,
            keyboard: false,
            backdrop: false,
            sizeclass: 'modal-lg',
            scope: $scope,
            resolve: {
                EmailTemplateId: function () {
                    return emailTemplateId;
                }
            }
        });
        modalInstance.result.then(function (emailTemplateId) {

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
        $scope.TableName = 'EmailTemplate';
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

var AddEmailTemplateCtrl = function ($scope, common, $location, $modalInstance, EmailTemplateId, EmailTemplateSvc) {

    $scope.InitEmailTemplateCtrl = function () {
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
    $scope.SaveEmailTemplate = function () {
        common.usSpinnerService.spin('spnEmailTemplate');
        EmailTemplateSvc.Save($scope.EmailTemplate).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   if ($scope.EmailTemplate.Id != undefined && $scope.EmailTemplate.Id > 0) {
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   else {
                       $scope.Paging.PageNo = 1;
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   common.usSpinnerService.stop('spnEmailTemplate');
                   $scope.ClosePopup();
               }
               else {
                   common.usSpinnerService.stop('spnEmailTemplate');
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnEmailTemplate');
               console.log(error);
           });
    }

    $scope.SendTestEmail = function () {
        if ($scope.checkValidation()) {
            common.usSpinnerService.spin('spnEmailTemplate');
            EmailTemplateSvc.SendEmail($scope.EmailTemplate).then(
               function (response) {
                   if (ShowMessage(common, response.data)) {
                       common.usSpinnerService.stop('spnEmailTemplate');
                   }
                   else {
                       common.usSpinnerService.stop('spnEmailTemplate');
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnEmailTemplate');
                   //common.aaNotify.error(error);
                   console.log(error);
               });
        }
    }

    $scope.checkValidation = function () {
        if ($scope.EmailTemplate.TestEmailAddress == undefined || $scope.EmailTemplate.TestEmailAddress == null || $scope.EmailTemplate.TestEmailAddress == '') {
            common.aaNotify.error('Please enter valid email address.');
            return false;
        }
            //else if ($scope.EmailTemplate.Title == undefined || $scope.EmailTemplate.Title == null || $scope.EmailTemplate.Title == '') {
            //    common.aaNotify.error('Please enter title.');
            //    return false;
            //}
        else if ($scope.EmailTemplate.EmailSubject == undefined || $scope.EmailTemplate.EmailSubject == null || $scope.EmailTemplate.EmailSubject == '') {
            common.aaNotify.error('Please enter email subject.');
            return false;
        }
        else if ($scope.EmailTemplate.EmailBody == undefined || $scope.EmailTemplate.EmailBody == null || $scope.EmailTemplate.EmailBody == '') {
            common.aaNotify.error('Please enter email content.');
            return false;
        }
        return true;
    }

    $scope.InitEmailTemplateCtrl();
};

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
};