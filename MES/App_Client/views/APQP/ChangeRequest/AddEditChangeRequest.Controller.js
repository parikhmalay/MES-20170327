app.controller('AddEditChangeRequestCtrl', ['$rootScope', '$scope', 'common', 'ChangeRequestSvc', 'LookupSvc', 'FileUploaderSvc', '$filter', '$routeParams', '$window', '$timeout', '$modal',
    function ($rootScope, $scope, common, ChangeRequestSvc, LookupSvc, FileUploaderSvc, $filter, $routeParams, $window, $timeout, $modal) {
        //Start implement security role wise
        $scope.setRoleWisePrivilege = function () {
            $scope.currentSecurityObject = currentSecurityObject;
            if (!Isundefinedornull($scope.currentSecurityObject) && $scope.currentSecurityObject.length > 0) {
                angular.forEach($scope.currentSecurityObject, function (obj, index) {
                    switch (obj.ObjectId) {
                        case 92:
                            $scope.setSecurityRoleCase(obj);
                            break;
                        case 106:
                            $scope.setSecurityRoleCase(obj);
                            break;
                        case 100:
                            $scope.setSecurityRoleCase(obj);
                            break;
                        case 101:
                            switch (obj.PrivilegeId) {
                                case 1:                           //none
                                    RedirectToAccessDenied();
                                    break;
                                case 2:
                                    $scope.IsReadOnlyPage = true;
                                    break;
                                case 3:
                                    break;
                            }
                            break;
                        case 105:
                            switch (obj.PrivilegeId) {
                                case 1:                           //none
                                    $scope.IsReadOnlyAPQPButton = true;
                                    break;
                                case 2:
                                    $scope.IsReadOnlyAPQPButton = true;
                                    break;
                                case 3:
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
        $scope.setSecurityRoleCase = function (obj) {
            switch (obj.PrivilegeId) {
                case 1:                           //none
                    RedirectToAccessDenied();
                    break;
            }
        };
        //End implement security role wise
        $scope.setRoleWisePrivilege();

        $('body').removeClass('paginationFixedToBottom haveAdvanceSearch');
        $rootScope.PageHeader = ($filter('translate')('_ChangeRequestForm_'));
        $scope.donotShow = false;
        $scope.hasPricingFieldsAccess = false;
        $scope.Init = function () {
            $scope.ChangeRequest = {
                lstDocument: []
            };

            $scope.Document = {};
            $scope.SourceOfChangeList = [{ 'Name': 'Customer', 'value': 1 }
                       , { 'Name': 'MES', 'value': 2 }
                       , { 'Name': 'Change Request', 'value': 3 }];
            $scope.SetLooksupData();
            $scope.ChangeRequest.crHeader = 'Change Request';
            $timeout(function () {

                if (!IsUndefinedNullOrEmpty($routeParams.APQPItemId) && parseInt($routeParams.APQPItemId) > 0) {
                    // $scope.TransactionMode = 'Edit';
                    $scope.AddToCR($routeParams.APQPItemId);
                }
                else if (!IsUndefinedNullOrEmpty($routeParams.CRId) && parseInt($routeParams.CRId) > 0) {
                    $scope.ChangeRequest.Id = $routeParams.CRId;
                    $scope.TransactionMode = 'Edit';
                    $scope.getData($routeParams.CRId);
                    $scope.getChangeLog($routeParams.CRId);
                }
                else {
                    $scope.TransactionMode = 'Create';
                }
                $scope.SetInitialValues();

            }, 1000);

        };

        $scope.SetInitialValues = function () {
            $scope.ChangeRequest.apqpPurchasePieceCost = '0.000';
            $scope.ChangeRequest.apqpSellingPiecePrice = '0.000';
            $scope.ChangeRequest.apqpPurchaseToolingCost = '0.000';
            $scope.ChangeRequest.apqpSellingToolingPrice = '0.000';

            $scope.ChangeRequest.PurchasePieceCost = '0.000';
            $scope.ChangeRequest.SellingPiecePrice = '0.000';
            $scope.ChangeRequest.PurchaseToolingCost = '0.000';
            $scope.ChangeRequest.SellingToolingPrice = '0.000';

            $scope.ChangeRequest.TotalPurchasePieceCost = '0.000';
            $scope.ChangeRequest.TotalSellingPiecePrice = '0.000';
            $scope.ChangeRequest.TotalPurchaseToolingCost = '0.000';
            $scope.ChangeRequest.TotalSellingToolingPrice = '0.000';

        };

        $scope.SetLooksupData = function () {
            $scope.LookUps = [

              {
                  "Name": "Users", "Parameters": {}
              },
              {
                  "Name": "Parts", "Parameters": {}
              },
              {
                  "Name": "AssignmentUsers", "Parameters": {}
              },
              {
                  "Name": "APQPStatus", "Parameters": { "source": "CR" }
              },
              {
                  "Name": "crDocumentTypes", "Parameters": { "source": "CR" }
              }

            ];
            $scope.getLookupData();
        };
        $scope.getLookupData = function () {
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                angular.forEach(data.data, function (o) {
                    if (o.Name === "Users") {
                        $scope.WatchersList = o.Data;
                    }
                    else if (o.Name === "Parts") {
                        $scope.PartsList = o.Data;
                    }
                    else if (o.Name === "AssignmentUsers") {
                        $scope.AssignToList = o.Data;
                    }
                    else if (o.Name === "APQPStatus") {
                        $scope.StatusList = o.Data;
                    }
                    else if (o.Name === "crDocumentTypes") {
                        $scope.DocumentTypeList = o.Data;
                    }
                });
            });
        };
        $scope.OnSourceOfChangeSelect = function ($item, objChangeRequest) {
            if (!IsNotNullorEmpty($item) || !IsNotNullorEmpty($item.Key)) {
                objChangeRequest.SourceOfChange = $item.Name;
            }
        };
        $scope.AddToCR = function (apqpItemId) {
            common.usSpinnerService.spin('spnAPQP');
            ChangeRequestSvc.addToCRFromAPQP(apqpItemId).then(
                function (response) {
                    if (ShowMessage(common, response.data)) {
                        common.usSpinnerService.stop('spnAPQP');
                        $scope.SetInitialValues();
                        $scope.ChangeRequest = response.data.Result;
                        $scope.ChangeRequest.lstDocument = [];

                        $scope.reloadWithId();
                    }
                    else {
                        common.usSpinnerService.stop('spnAPQP');
                    }
                },
               function (error) {
                   common.usSpinnerService.stop('spnAPQP');
                   console.log(error);
               });
        };

        $scope.onChangeOfPartNumber = function (Id) {
            common.usSpinnerService.spin('spnAPQP');
            ChangeRequestSvc.getOnChangeOfPartNumber(Id).then(
                function (response) {
                    if (ShowMessage(common, response.data)) {
                        common.usSpinnerService.stop('spnAPQP');
                        $scope.SetInitialValues();
                        $scope.ChangeRequest = response.data.Result;
                        $scope.ChangeRequest.lstDocument = [];
                        $scope.Calculate();
                        $scope.ChangeRequest.crHeader = 'Change Request # [' + $scope.ChangeRequest.Id + '] - ' + $scope.ChangeRequest.Subject;
                    }
                    else {
                        common.usSpinnerService.stop('spnAPQP');
                    }
                },
               function (error) {
                   common.usSpinnerService.stop('spnAPQP');
                   console.log(error);
               });
        };
        $scope.AddToAPQP = function () {
            common.usSpinnerService.spin('spnAPQP'); try {
                $scope.ChangeRequest.DrawingRevDate = convertUTCDateToLocalDate($scope.ChangeRequest.DrawingRevDate);
                $scope.ChangeRequest.MfgStartDateForNewRev = convertUTCDateToLocalDate($scope.ChangeRequest.MfgStartDateForNewRev);
            } catch (e) {
                console.log('in catch');
                common.usSpinnerService.stop('spnAPQP');
            }

            ChangeRequestSvc.AddToAPQP($scope.ChangeRequest).then(
               function (response) {
                   if (ShowMessage(common, response.data)) {
                       common.usSpinnerService.stop('spnAPQP');

                       $scope.ChangeRequest.Id = response.data.Result;
                       if ($scope.TransactionMode == 'Create')
                           $scope.reloadWithId();
                       else
                           $scope.ResetForm();
                   }
                   else {
                       common.usSpinnerService.stop('spnAPQP');
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnAPQP');
                   console.log(error);
               });
        };
        $scope.GoToAPQPForm = function (apqpItemId) {
            if (!Isundefinedornull(apqpItemId)) {
                $window.location.href = "/APQP/APQP#/AddEdit/" + apqpItemId + "/APQPSTEP1";
            }
        };
        $scope.getData = function (Id) {
            common.usSpinnerService.spin('spnAPQP');
            ChangeRequestSvc.getData(Id).then(
               function (response) {
                   if (ShowMessage(common, response.data)) {
                       common.usSpinnerService.stop('spnAPQP');
                       $scope.ChangeRequest = response.data.Result;
                       $scope.hasPricingFieldsAccess = $scope.ChangeRequest.HasPricingFieldsAccess;
                       $scope.ChangeRequest.lstDocument = [];
                       $scope.ChangeRequest.crHeader = 'Change Request # [' + $scope.ChangeRequest.Id + '] - ' + $scope.ChangeRequest.Subject;
                       $scope.ChangeRequest.DrawingRevDate = convertLocalDateToUTCDate($scope.ChangeRequest.DrawingRevDate);
                       $scope.ChangeRequest.MfgStartDateForNewRev = convertLocalDateToUTCDate($scope.ChangeRequest.MfgStartDateForNewRev);

                       $scope.Calculate();
                       if ($scope.ChangeRequest.StatusId == 13)
                           $scope.donotShow = true;
                   }
                   else {
                       common.usSpinnerService.stop('spnAPQP');
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnAPQP');
                   console.log(error);
               });
        };
        $scope.getChangeLog = function (Id) {
            common.usSpinnerService.spin('spnAPQP');
            $scope.ChangeRequestLog = {};
            ChangeRequestSvc.GetChangeLog(Id).then(
               function (response) {
                   if (ShowMessage(common, response.data)) {
                       common.usSpinnerService.stop('spnAPQP');
                       $scope.ChangeRequestLog = response.data.Result;
                       if ($scope.ChangeRequestLog.length > 0) {
                           $scope.ChangeRequestLog = _.groupBy($scope.ChangeRequestLog, "TimeStamp");
                       }
                   }
                   else {
                       common.usSpinnerService.stop('spnAPQP');
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnAPQP');
                   console.log(error);
               });
        };

        $scope.DeleteDocumentFromHistory = function (documentId) {
            if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                common.usSpinnerService.spin('spnAPQP');
                ChangeRequestSvc.DeleteDocument(documentId).then(
                  function (response) {
                      if (ShowMessage(common, response.data)) {
                          //$scope.ResetDocument();
                          $scope.getChangeLog($scope.ChangeRequest.Id);
                      }
                      else
                          common.usSpinnerService.stop('spnAPQP');
                  },
                  function (error) {
                      common.usSpinnerService.stop('spnAPQP');
                  });
            }
        };
        $scope.SaveCR = function (closeForm) {
            common.usSpinnerService.spin('spnAPQP');
            try {
                $scope.ChangeRequest.DrawingRevDate = convertUTCDateToLocalDate($scope.ChangeRequest.DrawingRevDate);
                $scope.ChangeRequest.MfgStartDateForNewRev = convertUTCDateToLocalDate($scope.ChangeRequest.MfgStartDateForNewRev);
            } catch (e) {
                console.log('in catch');
                common.usSpinnerService.stop('spnAPQP');
            }

            ChangeRequestSvc.Save($scope.ChangeRequest).then(
               function (response) {
                   if (ShowMessage(common, response.data)) {
                       common.usSpinnerService.stop('spnAPQP');
                       $scope.ChangeRequest.Id = response.data.Result;
                       if (closeForm) {
                           $scope.RedirectToList();
                       }
                       else {
                           if ($scope.TransactionMode == 'Create')
                               $scope.reloadWithId();
                           else
                               $scope.ResetForm();
                       }
                   }
                   else {
                       common.usSpinnerService.stop('spnAPQP');
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnAPQP');
                   console.log(error);
               });
        };
        $scope.Calculate = function () {
            if ($scope.ChangeRequest.PurchasePieceCost == '')
                $scope.ChangeRequest.PurchasePieceCost = '0.000';
            if ($scope.ChangeRequest.SellingPiecePrice == '')
                $scope.ChangeRequest.SellingPiecePrice = '0.000';
            if ($scope.ChangeRequest.PurchaseToolingCost == '')
                $scope.ChangeRequest.PurchaseToolingCost = '0.000';
            if ($scope.ChangeRequest.SellingToolingPrice == '')
                $scope.ChangeRequest.SellingToolingPrice = '0.000';

            if ($scope.ChangeRequest.apqpPurchasePieceCost == '')
                $scope.ChangeRequest.apqpPurchasePieceCost = '0.000';
            if ($scope.ChangeRequest.apqpSellingPiecePrice == '')
                $scope.ChangeRequest.apqpSellingPiecePrice = '0.000';
            if ($scope.ChangeRequest.apqpPurchaseToolingCost == '')
                $scope.ChangeRequest.apqpPurchaseToolingCost = '0.000';
            if ($scope.ChangeRequest.apqpSellingToolingPrice == '')
                $scope.ChangeRequest.apqpSellingToolingPrice = '0.000';

            //set decimal formated values  
            if (!isNaN(parseFloat($scope.ChangeRequest.apqpPurchasePieceCost)))
                $scope.ChangeRequest.apqpPurchasePieceCost = $filter("setDecimal")($scope.ChangeRequest.apqpPurchasePieceCost, 3);
            if (!isNaN(parseFloat($scope.ChangeRequest.apqpSellingPiecePrice)))
                $scope.ChangeRequest.apqpSellingPiecePrice = $filter("setDecimal")($scope.ChangeRequest.apqpSellingPiecePrice, 3);
            if (!isNaN(parseFloat($scope.ChangeRequest.apqpPurchasePieceCost)))
                $scope.ChangeRequest.apqpPurchaseToolingCost = $filter("setDecimal")($scope.ChangeRequest.apqpPurchaseToolingCost, 3);
            if (!isNaN(parseFloat($scope.ChangeRequest.apqpSellingToolingPrice)))
                $scope.ChangeRequest.apqpSellingToolingPrice = $filter("setDecimal")($scope.ChangeRequest.apqpSellingToolingPrice, 3);
            if (!isNaN(parseFloat($scope.ChangeRequest.PurchasePieceCost)))
                $scope.ChangeRequest.PurchasePieceCost = $filter("setDecimal")($scope.ChangeRequest.PurchasePieceCost, 3);
            if (!isNaN(parseFloat($scope.ChangeRequest.SellingPiecePrice)))
                $scope.ChangeRequest.SellingPiecePrice = $filter("setDecimal")($scope.ChangeRequest.SellingPiecePrice, 3);
            if (!isNaN(parseFloat($scope.ChangeRequest.PurchasePieceCost)))
                $scope.ChangeRequest.PurchaseToolingCost = $filter("setDecimal")($scope.ChangeRequest.PurchaseToolingCost, 3);
            if (!isNaN(parseFloat($scope.ChangeRequest.SellingToolingPrice)))
                $scope.ChangeRequest.SellingToolingPrice = $filter("setDecimal")($scope.ChangeRequest.SellingToolingPrice, 3);
            $scope.ChangeRequest.TotalPurchasePieceCost = $filter("setDecimal")(parseFloat($scope.ChangeRequest.apqpPurchasePieceCost) + parseFloat($scope.ChangeRequest.PurchasePieceCost), 3);
            $scope.ChangeRequest.TotalSellingPiecePrice = $filter("setDecimal")(parseFloat($scope.ChangeRequest.apqpSellingPiecePrice) + parseFloat($scope.ChangeRequest.SellingPiecePrice), 3);
            $scope.ChangeRequest.TotalPurchaseToolingCost = $filter("setDecimal")(parseFloat($scope.ChangeRequest.apqpPurchaseToolingCost) + parseFloat($scope.ChangeRequest.PurchaseToolingCost), 3);
            $scope.ChangeRequest.TotalSellingToolingPrice = $filter("setDecimal")(parseFloat($scope.ChangeRequest.apqpSellingToolingPrice) + parseFloat($scope.ChangeRequest.SellingToolingPrice), 3);
        };
        $scope.reloadWithId = function () {
            common.$location.path('/AddEdit/' + $scope.ChangeRequest.Id + "/0");
        };
        $scope.RedirectToList = function () {
            common.$location.path("/ChangeRequestList/");
        };
        $scope.setSubject = function (item, id) {
            common.usSpinnerService.spin('spnAPQP');
            var partname = '';
            var exitLoop = false;
            angular.forEach(item, function (o) {
                if (o.Id === id && !exitLoop) {
                    partname = o.ParentId;
                    exitLoop = true;
                }
            });
            $scope.ChangeRequest.Subject = "Change Request for Part # - " + partname;
            common.usSpinnerService.stop('spnAPQP');
        };
        $scope.ResetForm = function () {
            common.$route.reload();
        };
        ////***Start code for document****/////
        $scope.EditDocument = function (document, index) {
            $scope.AddNewDocument();
            $scope.Document = document;
            $scope.DocumentIndex = index;
        };
        $scope.DeleteDocument = function (index) {
            if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                $scope.ChangeRequest.lstDocument.splice(index, 1);
                $scope.ResetDocuments();
                common.aaNotify.success(($filter('translate')('_DocumentType_')) + ' is deleted successfully.');
            }
        };

        $scope.CancelDocument = function () {
            $scope.ResetDocuments();
        };

        $scope.ResetDocuments = function () {
            $scope.Document = {};
            $scope.DocumentIndex = null;
            $scope.AddEditDocument = false;
        };
        $scope.AddNewDocument = function () {
            $scope.selectedFiles = [];
            $scope.AddEditDocument = true;
            $scope.Document = {};
            $scope.ShowDocumentPopup();
        };
        $scope.SetObjectvalues = function (file, name) {
            $scope.Document.FilePath = file;
            $scope.Document.FileTitle = name;
        };
        $scope.deleteFile = function (filePath, index) {
            $scope.Document.FilePath = '';
            $scope.Document.FileTitle = '';
        };
        ////***End code for document****/////
        //popup for show document form
        $scope.ShowDocumentPopup = function () {
            var modalTemplatePreviewOptions = $modal.open({
                templateUrl: '/App_Client/views/APQP/ChangeRequest/ChangeRequestDocument.html?v=' + Version,
                controller: ModalDocumentCtrl,
                keyboard: false,
                backdrop: false,
                scope: $scope,
                sizeclass: 'modal-md'
            });
            modalTemplatePreviewOptions.result.then(function () {
            }, function () {
            });
        };
        // document popup end here

        $scope.DocumentValidation = function (objDocument) {
            if (Isundefinedornull(objDocument.DocumentTypeItem) || Isundefinedornull(objDocument.DocumentTypeItem.Id)) {
                common.aaNotify.error(($filter('translate')('_DocumentType_')) + ' is required.');
                return false;
            }
                //if (IsUndefinedNullOrEmpty(objDocument.DocumentTypeId)) {
                //    common.aaNotify.error(($filter('translate')('_DocumentType_')) + ' is required.');
                //    return false;
                //}
            else if (IsUndefinedNullOrEmpty(objDocument.FilePath)) {
                common.aaNotify.error(($filter('translate')('_Document_')) + ' is required.');
                return false;
            }
            return true;
        };

        $scope.Init();
    }]);


var ModalDocumentCtrl = function ($scope, $modalInstance, common, $filter, FileUploaderSvc) {
    $scope.AddDocument = function () {
        if (Isundefinedornull($scope.ChangeRequest.lstDocument))
            $scope.ChangeRequest.lstDocument = [];

        if (!$scope.DocumentValidation($scope.Document))
            return false;
        if (!Isundefinedornull($scope.DocumentIndex)) {
            $scope.ChangeRequest.lstDocument.splice($scope.DocumentIndex, 1);
        }
        $scope.Document.DocumentTypeId = $scope.Document.DocumentTypeItem.Id;
        $scope.ChangeRequest.lstDocument.push($scope.Document);
        $scope.Cancel();
        common.aaNotify.success(($filter('translate')('_DocumentType_')) + ' is added successfully.');
    };
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
        $scope.ResetDocuments();
        $scope.destroyScope();
    };
    $scope.destroyScope = function () {
        var lclScope = $scope;
        lclScope.$destroy();
    };

    ////this is for single file upload

}