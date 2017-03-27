app.controller('DocumentManagementCtrl', ['$rootScope', '$scope', 'common', 'DocumentManagementSvc', '$filter', '$modal', 'LookupSvc', '$timeout', function ($rootScope, $scope, common, DocumentManagementSvc, $filter, $modal, LookupSvc, $timeout) {
    //Start implement security role wise
    $scope.setRoleWisePrivilege = function () {
        $scope.currentSecurityObject = currentSecurityObject;
        if (!Isundefinedornull($scope.currentSecurityObject) && $scope.currentSecurityObject.length > 0) {
            angular.forEach($scope.currentSecurityObject, function (obj, index) {
                switch (obj.ObjectId) {
                    case 92:
                        $scope.setSecurityRoleCase(obj);
                        break;
                    case 115:
                        $scope.setSecurityRoleCase(obj);
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
    $scope.setSecurityRoleCase = function (obj) {
        switch (obj.PrivilegeId) {
            case 1:                           //none
                RedirectToAccessDenied();
                break;
        }
    };
    //End implement security role wise
    $scope.setRoleWisePrivilege();

    $rootScope.PageHeader = ($filter('translate')('_DocMgmtTitle_'));
    $scope.APQPDocumentList = {};
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;
    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        $scope.GetDocumentManagementList();
        $scope.SetLooksupData();
    };
    $scope.GetDocumentManagementList = function () {
        common.usSpinnerService.spin('spnAPQP');
        var APQPStatusIds = [];
        angular.forEach($scope.SearchCriteria.APQPStatusItems, function (item, index) {
            if (!IsUndefinedNullOrEmpty(item.Id))
                APQPStatusIds.push(item.Id);
        });
        $scope.SearchCriteria.APQPStatusIds = APQPStatusIds.join(",");
        DocumentManagementSvc.GetDocumentManagementList($scope.Paging).then(
             function (response) {
                 if (response.data.StatusCode == 200) {
                     $scope.APQPDocumentList = response.data.Result;
                     $scope.Paging = response.data.PageInfo;
                     if ($scope.APQPDocumentList.length > 0) {
                         advanceSearch.close();
                     }
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
                 $timeout(function () {
                     common.usSpinnerService.stop('spnAPQP');
                 }, 0);
             },
             function (error) {
                 common.usSpinnerService.stop('spnAPQP');
             });
    };

    $scope.SetLooksupData = function () {
        $scope.LookUps = [
            {
                "Name": "SAM", "Parameters": {
                    "source": "SAM"
                }
            },
            {
                "Name": "APQPStatus", "Parameters": {
                    "source": "APQP"
                }
            },
            {
                "Name": "APQPEngineers", "Parameters": {}
            },
            {
                "Name": "SCUsers", "Parameters": {}
            }
        ];
        $scope.getLookupData();
    };
    $scope.getLookupData = function () {
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            angular.forEach(data.data, function (o) {
                if (o.Name === "SAM") {
                    $scope.DashSAMList = o.Data;
                }
                else if (o.Name === "APQPEngineers") {
                    $scope.DashAPQPQualityEngineerList = o.Data;
                }
                else if (o.Name === "SCUsers") {
                    $scope.DashSupplyChainCoordinatorList = o.Data;
                }
                else if (o.Name === "APQPStatus") {
                    $scope.DashAPQPStatusList = o.Data;
                    //if ($scope.DashAPQPStatusList.length > 0) {
                    //    $scope.SearchCriteria.APQPStatusItems = $filter('filter')($scope.DashAPQPStatusList, function (rw) {
                    //        return rw.Id != 8 && rw.Id != 15
                    //    });
                    //}
                }
            });
        });
    };

    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.GetDocumentManagementList();
    };
    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.GetDocumentManagementList();
    };
    $scope.Search = function () {
        $scope.Init();
    };
    $scope.ResetSearch = function () {
        $scope.SearchCriteria = {};
        $scope.Init();
    }
    //document list 
    $scope.ShowPopupDocuments = function (documentManagementId, isDocument) {
        common.usSpinnerService.spin('spnAPQP');
        if (!isDocument) {
            common.aaNotify.error($filter('translate')('_NoDocumentsMessage_'));
            common.usSpinnerService.stop('spnAPQP');
            return false;
        }
        var modalTemplatePreviewOptions = $modal.open({
            templateUrl: '/App_Client/views/APQP/DocumentManagement/APQPDocumentsPopup.html?v=' + Version,
            controller: ModalDocumentCtrl,
            keyboard: true,
            backdrop: true,
            scope: $scope,
            sizeclass: 'modal-fitToScreen',
            resolve: {
                DocumentManagementId: function () {
                    return documentManagementId;
                }
            }
        });
        modalTemplatePreviewOptions.result.then(function () {
        }, function () {
        });
    };
    $scope.Init();
}]);
var ModalDocumentCtrl = function ($scope, $modalInstance, common, $filter, $timeout, DocumentManagementSvc, DocumentManagementId) {
    $scope.DocumentsInit = function () {
        $scope.DocumentsList = {};
        $scope.DocumentsListWithGroup = {};
        $scope.GetDocumentList(DocumentManagementId);
    };

    $scope.GetDocumentList = function (id) {
        common.usSpinnerService.spin('spnAPQP');
        DocumentManagementSvc.GetDocumentList(id).then(
           function (response) {
               if (response.data.StatusCode == 200) {
                   $scope.DocumentsList = response.data.Result;
                   angular.forEach($scope.DocumentsList, function (item) {
                       item.ReceivedDate = convertLocalDateToUTCDate(item.ReceivedDate);
                   });
                   $scope.DocumentsListWithGroup = _.groupBy($scope.DocumentsList, 'DocumentType');
                   $scope.IsAccordionObjectEmpty = IsObjectEmpty($scope.DocumentsListWithGroup);
                   if ($scope.DocumentsList.length <= 0) {
                       common.aaNotify.error($filter('translate')('_NoDocumentsMessage_'));
                       common.usSpinnerService.stop('spnAPQP');
                       $scope.Cancel();
                   }

                   $timeout(function () {
                       common.usSpinnerService.stop('spnAPQP');
                   }, 0);
               }
               else {
                   $scope.IsAccordionObjectEmpty = true;
                   common.usSpinnerService.stop('spnAPQP');
               }
           },
           function (error) {
               $scope.IsAccordionObjectEmpty = true;
               common.usSpinnerService.stop('spnAPQP');
           });
    };

    $scope.DownloadDocuments = function () {
        common.usSpinnerService.spin('spnAPQP');
        var documentIds = [], strdocumentIds = '', documentFilePathList = [];
        angular.forEach($scope.DocumentsList, function (item, index) {
            if (item.chkSelect) {
                documentIds.push(item.Id);
                if (!IsUndefinedNullOrEmpty(item.FilePath))
                    documentFilePathList.push(item.FilePath);
            }
        });
        if (documentIds.length > 0)
            strdocumentIds = documentIds.join(",");
        else {
            common.usSpinnerService.stop('spnAPQP');
            common.aaNotify.error("Please select at least one document.");
            return false;
        }
        if (documentFilePathList.length <= 0) {
            common.usSpinnerService.stop('spnAPQP');
            common.aaNotify.error($filter('translate')('_NoDocumentsMessage_'));
            return false;
        }
        console.log(documentFilePathList);
        DocumentManagementSvc.DownloadDocuments(documentFilePathList).then(
          function (response) {
              common.usSpinnerService.stop('spnAPQP');
              if (response.data.StatusCode == 200) {
                  if (!IsUndefinedNullOrEmpty(response.data.SuccessMessage))
                      window.open(response.data.SuccessMessage, '_blank');
              }
              else {
                  common.aaNotify.error(response.data.ErrorText);
              }
          },
          function (error) {
              common.usSpinnerService.stop('spnAPQP');
          });
    };

    $scope.ShowHideHistory = function (documentItems, documentTypeId, showHistoryButton) {
        console.log(showHistoryButton);
        if (showHistoryButton)
            documentItems.varDocumentTypeId = documentTypeId;
        else
            documentItems.varDocumentTypeId = 0;
        documentItems.showHistoryButton = showHistoryButton;
    };

    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.DocumentsInit();
};
