app.controller('DocumentTypeCtrl', ['$rootScope', '$scope', 'common', 'DocumentTypeSvc', '$modal', 'LookupSvc', '$filter', 'AuditLogsSvc',
    function ($rootScope, $scope, common, DocumentTypeSvc, $modal, LookupSvc, $filter, AuditLogsSvc) {
        $scope.DocumentType = {};
        $scope.DocumentTypeList = {};
        $scope.AssociatedToList = {};
        $scope.SearchCriteria = {};
        $scope.LookUps = [{ "Name": "AssociatedToItems", "Parameters": { "source": "DT" } }];
        $scope.sortReverse = false;
        $rootScope.PageHeader = ($filter('translate')('_DocumentTypes_'));

        $scope.Init = function () {
            $scope.Paging = GetDefaultPageObject();
            $scope.Paging.Criteria = $scope.SearchCriteria;
            $scope.GetDocumentTypesList();
        };

        $scope.GetDocumentTypesList = function () {
            common.usSpinnerService.spin('spnDocumentType');
            DocumentTypeSvc.GetDocumentTypesList($scope.Paging).then(
                 function (response) {
                     common.usSpinnerService.stop('spnDocumentType');
                     if (response.data.StatusCode == 200) {
                         $scope.DocumentTypeList = response.data.Result;
                         if ($scope.DocumentTypeList.length > 0)
                             advanceSearch.close();
                         $scope.Paging = response.data.PageInfo;
                         $scope.getLookupData();
                     }
                     else {
                         console.log(response.data.ErrorText);
                     }
                 },
                 function (error) {
                     common.usSpinnerService.stop('spnDocumentType');
                     //common.aaNotify.error(error);
                 });
        };

        $scope.Edit = function (documentType) {
            $scope.ShowPopup();
            $scope.DocumentType = documentType;
        };

        $scope.Delete = function (documentTypeId) {
            if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                DocumentTypeSvc.Delete(documentTypeId).then(
               function (response) {
                   if (ShowMessage(common, response.data)) {
                       $scope.GetDocumentTypesList();
                   }
               },
               function (error) {
                   //common.aaNotify.error(error);
               });
            }
        };

        $scope.pageSizeChanged = function (PageSize) {
            $scope.Paging.PageSize = PageSize;
            $scope.GetDocumentTypesList();
        };

        $scope.pageChanged = function (PageNo) {
            $scope.Paging.PageNo = PageNo;
            $scope.GetDocumentTypesList();
        };

        $scope.ShowPopup = function (documentTypeId) {
            $scope.DocumentType = {};
            var modalInstance = $modal.open({
                templateUrl: '/App_Client/views/Setup/DocumentType/AddDocumentTypePopup.html?v=' + Version,
                controller: AddDocumentTypeCtrl,
                keyboard: false,
                backdrop: false,
                scope: $scope,
                resolve: {
                    DocumentTypeId: function () {
                        return documentTypeId;
                    }
                }
            });
            modalInstance.result.then(function (documentTypeId) {

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
                    if (o.Name === "AssociatedToItems") {
                        $scope.AssociatedToList = o.Data;
                    }
                });
            });
        }

        //popup for show audit log 
        $scope.ShowChangeLogPopup = function (transactionId) {
            $scope.TransactionId = transactionId;
            $scope.TableName = 'DocumentType';
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

var AddDocumentTypeCtrl = function ($scope, common, $location, $modalInstance, DocumentTypeId, DocumentTypeSvc) {

    $scope.InitDocumentTypeCtrl = function () {
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

    $scope.SaveDocumentType = function () {
        common.usSpinnerService.spin('spnDocumentType');
        DocumentTypeSvc.Save($scope.DocumentType).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   if ($scope.DocumentType.Id != undefined && $scope.DocumentType.Id > 0) {
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   else {
                       $scope.Paging.PageNo = 1;
                       $scope.pageChanged($scope.Paging.PageNo);
                   }
                   common.usSpinnerService.stop('spnDocumentType');
                   //$scope.Id = response.data.Result // Id of latest created record
                   $scope.ClosePopup();
               }
               else {
                   common.usSpinnerService.stop('spnDocumentType');
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnDocumentType');
               console.log(error);
           });
    }
    $scope.InitDocumentTypeCtrl();
};

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
}