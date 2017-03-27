app.controller('ShipmentCtrl', ['$rootScope', '$scope', 'common', 'ShipmentSvc', '$modal', '$filter', 'LookupSvc', function ($rootScope, $scope, common, ShipmentSvc, $modal, $filter, LookupSvc) {
    //Start implement security role wise
    $scope.setRoleWisePrivilege = function () {
        $scope.currentSecurityObject = currentSecurityObject;
        if (!Isundefinedornull($scope.currentSecurityObject) && $scope.currentSecurityObject.length > 0) {
            angular.forEach($scope.currentSecurityObject, function (obj, index) {
                switch (obj.ObjectId) {
                    case 78:
                        switch (obj.PrivilegeId) {
                            case 1:                           //none
                                RedirectToAccessDenied();
                                break;
                            case 2:                          //read only
                                $scope.IsReadOnlyDeleteButton = true;
                                $scope.IsReadOnlyAddButton = true;
                                break;
                            case 3:                         //write
                                break;
                        }
                        break;
                    case 81:
                        switch (obj.PrivilegeId) {
                            case 1:                           //none
                                RedirectToAccessDenied();
                                break;
                            case 2:                          //read only
                                $scope.IsReadOnlyDeleteButton = true;
                                $scope.IsReadOnlyAddButton = true;
                                break;
                            case 3:                         //write
                                break;
                        }
                        break;
                    case 79:
                        switch (obj.PrivilegeId) {
                            case 1:                           //none
                                $scope.IsReadOnlyAddButton = true;
                                break;
                            case 2:                          //read only
                                $scope.IsReadOnlyAddButton = true;
                                break;
                            case 3:                         //write
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
    //End implement security role wise
    $scope.setRoleWisePrivilege();

    $scope.Shipment = [];
    $scope.ShipmentList = [];
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;
    $rootScope.PageHeader = ($filter('translate')('_ShipmentList_'));
    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        $scope.SetLookupData();
        $scope.ShipmentModeList =
                     [{ 'name': 'Air', 'value': '1' }
                    , { 'name': 'Land', 'value': '2' }
                    , { 'name': 'Sea - Container', 'value': '3' }
                    , { 'name': 'Sea - LCL', 'value': '4' }
                    , { 'name': 'Sea - LCL - Express', 'value': '5' }];

        $scope.QualityReviewStatusList =
                 [{ 'name': 'Not Reviewed', 'value': '1' }
                , { 'name': 'Approved', 'value': '2' }
                , { 'name': 'Conditionally Approved', 'value': '3' }
                , { 'name': 'Rejected', 'value': '4' }];

        $scope.StatusList =
                [{ 'name': 'Open', 'value': true }
                , { 'name': 'Complete', 'value': false }];

        $scope.GetShipmentList();
    };

    $scope.SetLookupData = function () {
        $scope.LookUps = [
          {
              "Name": "Destinations", "Parameters": {}
          },
          {
              "Name": "Origins", "Parameters": {}
          },
          {
              "Name": "Forwarders", "Parameters": {}
          },
           {
               "Name": "Suppliers", "Parameters": {}
           },
        ];
        $scope.getLookupData();
    };

    $scope.getLookupData = function () {
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            angular.forEach(data.data, function (o) {
                if (o.Name === "Suppliers") {
                    $scope.SupplierList = o.Data;
                }
                else if (o.Name === "Destinations") {
                    $scope.DestinationList = o.Data;
                }
                else if (o.Name === "Origins") {
                    $scope.OriginList = o.Data;
                }
                else if (o.Name === "Forwarders") {
                    $scope.ForwarderList = o.Data;
                }
            });
        });
    }

    $scope.GetShipmentList = function () {
        common.usSpinnerService.spin('spnShipment');

        ShipmentSvc.GetShipmentList($scope.Paging).then(
             function (response) {
                 common.usSpinnerService.stop('spnShipment');
                 if (response.data.StatusCode == 200) {
                     $scope.ShipmentList = response.data.Result;
                     if ($scope.ShipmentList.length > 0)
                         advanceSearch.close();
                     angular.forEach($scope.ShipmentList, function (o) {

                         o.EstShpmntDate = convertUTCDateToLocalDate(o.EstShpmntDate);
                         o.ETAAtWarehouseAtDest = convertUTCDateToLocalDate(o.ETAAtWarehouseAtDest);

                     });

                     $scope.Paging = response.data.PageInfo;
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
             function (error) {
                 common.usSpinnerService.stop('spnShipment');
                 //common.aaNotify.error(error);
             });
    };

    $scope.Edit = function (id) {
        common.$location.path("/AddEdit/" + id);
    };

    $scope.Delete = function (shipmentId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            ShipmentSvc.Delete(shipmentId).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.GetShipmentList();
               }
           },
           function (error) {
               //common.aaNotify.error(error);
           });
        }
    };

    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.GetShipmentList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.GetShipmentList();
    };
    $scope.Search = function () {

        $scope.Init();
    };
    $scope.ResetSearch = function () {
        $scope.SearchCriteria = {};
        $scope.Init();
    }

    //popup for show audit log 
    $scope.ShowChangeLogPopup = function (transactionId) {
        $scope.TransactionId = transactionId;
        $scope.TableName = 'Shipment';
        $scope.SchemaName = 'Shipments';
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

    $scope.RedirectToAddEditPage = function () {
        common.$location.path("/AddEdit/");
    }

    $scope.Init();
}]);

var ViewChangeLogPageInstance = function ($scope, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
}

app.controller('AddEditShipmentCtrl', ['$rootScope', '$scope', 'common', 'ShipmentSvc', '$modal', 'LookupSvc', '$filter', '$routeParams', '$timeout',
    function ($rootScope, $scope, common, ShipmentSvc, $modal, LookupSvc, $filter, $routeParams, $timeout) {
        //Start implement security role wise
        $scope.setRoleWisePrivilege = function () {
            $scope.currentSecurityObject = currentSecurityObject;
            if (!Isundefinedornull($scope.currentSecurityObject) && $scope.currentSecurityObject.length > 0) {
                angular.forEach($scope.currentSecurityObject, function (obj, index) {
                    switch (obj.ObjectId) {
                        case 78:
                            $scope.PagePrivilegeCase(obj);
                            break;
                        case 81:
                            $scope.PagePrivilegeCase(obj);
                            break;
                        case 79:
                            if ($scope.TransactionMode == "Create") {
                                $scope.PagePrivilegeCase(obj);
                            }
                            break;
                        case 80:
                            if ($scope.TransactionMode == "Edit") {
                                $scope.PagePrivilegeCase(obj);
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
        $scope.PagePrivilegeCase = function (obj) {
            switch (obj.PrivilegeId) {
                case 1:                           //none
                    RedirectToAccessDenied();
                    break;
                case 2:                          //read only
                    $scope.IsReadOnlyPage = true;
                    break;
                case 3:                         //write                    
                    break;
            }
        }
        //End implement security role wise


        $rootScope.PageHeader = ($filter('translate')('_PageHeading_'));
        $scope.SelectAllObj = { SelectAll: false };
        $('body').removeClass('paginationFixedToBottom haveAdvanceSearch');
        $scope.Init = function () {
            $scope.Shipment = {
                lstDocument: [],
                lstPOPart: []
            };

            $scope.Shipment.Status = true;
            $scope.Shipment.ShipmentMode = null;
            $scope.Shipment.QualityReviewStatus = null;

            $scope.SetLookupData();
            $scope.Document = {};
            $scope.Document.DocumentFilePath = '';
            $scope.Document.DocumentFileName = '';
            $scope.POPart = {};
            $scope.ShipmentModeList =
                        [{ 'name': 'Air', 'value': 1 }
                       , { 'name': 'Land', 'value': 2 }
                       , { 'name': 'Sea - Container', 'value': 3 }
                       , { 'name': 'Sea - LCL', 'value': 4 }
                       , { 'name': 'Sea - LCL - Express', 'value': 5 }];
            $scope.QualityReviewStatusList =
                 [{ 'name': 'Not Reviewed', 'value': 1 }
                , { 'name': 'Approved', 'value': 2 }
                , { 'name': 'Conditionally Approved', 'value': 3 }
                , { 'name': 'Rejected', 'value': 4 }]

            $scope.StatusList =
                    [{ 'name': 'Open', 'value': true }
                    , { 'name': 'Complete', 'value': false }];


            $timeout(function () {
                if (!IsUndefinedNullOrEmpty($routeParams.Id) && $routeParams.Id != '0') {
                    $scope.TransactionMode = 'Edit';
                    $scope.Shipment.ShipmentId = $routeParams.Id;
                    $scope.getData($routeParams.Id);
                }
                else {
                    $scope.TransactionMode = 'Create';
                }
                $scope.setRoleWisePrivilege();
            }, 1000);
        }

        $scope.SetLookupData = function () {
            $scope.LookUps = [
              {
                  "Name": "Destinations", "Parameters": {}
              },
              {
                  "Name": "Origins", "Parameters": {}
              },
              {
                  "Name": "Forwarders", "Parameters": {}
              },
               {
                   "Name": "Suppliers", "Parameters": {}
               },
               {
                   "Name": "DocumentTypes", "Parameters": { "associatedTo_id": "SH" }
               }
            ];
            $scope.getLookupData();
        };

        $scope.getLookupData = function () {
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                angular.forEach(data.data, function (o) {
                    if (o.Name === "Suppliers") {
                        $scope.SupplierList = o.Data;
                    }
                    else if (o.Name === "Destinations") {
                        $scope.DestinationList = o.Data;
                    }
                    else if (o.Name === "Origins") {
                        $scope.OriginList = o.Data;
                    }
                    else if (o.Name === "Forwarders") {
                        $scope.ForwarderList = o.Data;
                    }
                    else if (o.Name === "DocumentTypes") {
                        $scope.DocumentList = o.Data;
                    }
                });
            });
        }

        $scope.getData = function (Id) {
            common.usSpinnerService.spin('AddEditShipmentCtrl');
            ShipmentSvc.getData(Id).then(function (response) {
                if (ShowMessage(common, response.data)) {
                    common.usSpinnerService.stop('AddEditShipmentCtrl');
                    $scope.Shipment = response.data.Result;

                    $scope.Shipment.EstShpmntDate = convertUTCDateToLocalDate($scope.Shipment.EstShpmntDate);
                    $scope.Shipment.ActShpmntDate = convertUTCDateToLocalDate($scope.Shipment.ActShpmntDate);
                    $scope.Shipment.ETAAtWarehouseAtDest = convertUTCDateToLocalDate($scope.Shipment.ETAAtWarehouseAtDest);
                    $scope.Shipment.ActArrDateAtWarehouseAtDest = convertUTCDateToLocalDate($scope.Shipment.ActArrDateAtWarehouseAtDest);
                    $scope.Shipment.EstForwarderPickupDate = convertUTCDateToLocalDate($scope.Shipment.EstForwarderPickupDate);
                    $scope.Shipment.ActForwarderPickupDate = convertUTCDateToLocalDate($scope.Shipment.InspectionDate);

                    $scope.Shipment.InspectionDate = convertUTCDateToLocalDate($scope.Shipment.InspectionDate);
                }
                else {
                    common.usSpinnerService.stop('AddEditShipmentCtrl');
                }
            },
               function (error) {
                   common.usSpinnerService.stop('AddEditShipmentCtrl');
                   console.log(error);
               });
        };

        $scope.SetObjectvalues = function (file, name) {
            $scope.Shipment.InspectionReportFilePath = file;
            $scope.Shipment.InspectionReportFileName = name;
        }

        $scope.deleteFile = function (filePath) {
            $scope.Shipment.InspectionReportFilePath = '';
            $scope.Shipment.InspectionReportFileName = '';
        }

        ////***Start code for document****/////
        $scope.EditDocument = function (document, index) {
            $scope.AddNewDocument();
            $scope.Document = document;
            $scope.DocumentIndex = index;
        };

        $scope.DeleteDocument = function (index) {
            if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                $scope.Shipment.lstDocument.splice(index, 1);
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
            $scope.AddEditDocument = true;
            $scope.ShowDocumentPopup();
        };

        $scope.SetObjectvaluesDocument = function (file, name) {
            $scope.Document.DocumentFilePath = file;
            $scope.Document.DocumentFileName = name;
        }

        $scope.deleteFileDocument = function (filePath) {
            $scope.Document.DocumentFilePath = '';
            $scope.Document.DocumentFileName = '';
        }

        $scope.DocumentValidation = function (objDocument) {

            if (Isundefinedornull(objDocument.DocumentTypeItem) || Isundefinedornull(objDocument.DocumentTypeItem.Id)) {
                common.aaNotify.error(($filter('translate')('_DocumentType_')) + ' is required.');
                return false;
            }
            return true;
        };
        ////***End code for document****/////

        //popup for show document form
        $scope.ShowDocumentPopup = function () {
            var modalTemplatePreviewOptions = $modal.open({
                templateUrl: '/App_Client/views/ShipmentTracking/Shipment/ShipmentDocument.html?v=' + Version,
                controller: ModalDocumentCtrl,
                keyboard: true,
                backdrop: true,
                scope: $scope,
                sizeclass: 'modal-md'
            });
            modalTemplatePreviewOptions.result.then(function () {
            }, function () {
            });
        };
        // document popup end here

        ////***Start code for POPart****/////
        $scope.EditPOPart = function (POPart, index) {
            $scope.AddNewPOPart();
            $scope.POPart = POPart;
            $scope.POPartIndex = index;
        };

        $scope.DeletePOPart = function (index) {
            if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                $scope.Shipment.lstPOPart.splice(index, 1);
                $scope.ResetPOParts();
                common.aaNotify.success(($filter('translate')('_POPart_')) + ' is deleted successfully.');
            }
        };

        $scope.CancelPOPart = function () {
            $scope.ResetPOParts();
        };

        $scope.ResetPOParts = function () {
            $scope.POPartIndex = null;
            $scope.AddEditPOPart = false;
        };
        $scope.setFormName = function (formName) {
            $scope.FormName = formName;
        };
        $scope.AddNewPOPart = function () {
            $scope.FormName.$aaFormExtensions.$clearErrors();
            $scope.POPart = {};
            $scope.AddEditPOPart = true;
            if (!IsUndefinedNullOrEmpty($scope.Shipment.lstPOPart) && $scope.Shipment.lstPOPart.length > 0) {
                $scope.POPart.PONumber = $scope.Shipment.lstPOPart[$scope.Shipment.lstPOPart.length - 1].PONumber;
            }
            $scope.ShowPOPartPopup();
        };

        ////***End code for POPart****/////

        //popup for show POPart form
        $scope.ShowPOPartPopup = function () {
            var modalTemplatePreviewOptions = $modal.open({
                templateUrl: '/App_Client/views/ShipmentTracking/Shipment/ShipmentPOPart.html?v=' + Version,
                controller: ModalPOPartCtrl,
                keyboard: true,
                backdrop: true,
                scope: $scope,
                sizeclass: 'modal-md'
            });
            modalTemplatePreviewOptions.result.then(function () {
            }, function () {
            });
        };
        // POPart popup end here

        $scope.SaveShipment = function (closeForm) {

            common.usSpinnerService.spin('AddEditShipmentCtrl');
            ShipmentSvc.Save($scope.Shipment).then(
               function (response) {
                   if (ShowMessage(common, response.data)) {
                       common.usSpinnerService.stop('AddEditShipmentCtrl');
                       $scope.Id = response.data.Result; // Id of latest created record
                       $scope.Shipment.Id = response.data.Result;
                       //$scope.RedirectToList();
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
                       common.usSpinnerService.stop('AddEditShipmentCtrl');
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('AddEditShipmentCtrl');
                   console.log(error);
               });

        }

        $scope.reloadWithId = function () {
            common.$location.path('/AddEdit/' + $scope.Shipment.Id);
        };

        $scope.RedirectToList = function () {
            common.$location.path("/");
        }

        $scope.ResetForm = function () {
            common.$route.reload();
        }

        $scope.Init();
    }]);

var ModalDocumentCtrl = function ($scope, $modalInstance, common, $filter) {
    $scope.AddDocument = function () {

        if (!$scope.DocumentValidation($scope.Document))
            return false;
        if (!Isundefinedornull($scope.DocumentIndex)) {
            $scope.Shipment.lstDocument.splice($scope.DocumentIndex, 1);
        }
        $scope.Document.DocumentTypeId = $scope.Document.DocumentTypeItem.Id;
        $scope.Shipment.lstDocument.push($scope.Document);
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
}


var ModalPOPartCtrl = function ($scope, $modalInstance, common, $filter) {

    $scope.AddPOPart = function () {
        if (!Isundefinedornull($scope.POPartIndex)) {
            $scope.Shipment.lstPOPart.splice($scope.POPartIndex, 1);
        }
        if (Isundefinedornull($scope.Shipment.lstPOPart))
            $scope.Shipment.lstPOPart = [];

        $scope.Shipment.lstPOPart.push($scope.POPart);
        $scope.Cancel();
        common.aaNotify.success(($filter('translate')('_POPart_')) + ' is added successfully.');
    };
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
        $scope.ResetPOParts();
        $scope.destroyScope();
    };
    $scope.destroyScope = function () {
        var lclScope = $scope;
        lclScope.$destroy();
    };
}