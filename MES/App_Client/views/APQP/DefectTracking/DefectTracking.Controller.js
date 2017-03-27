app.controller('DefectTrackingCtrl', ['$rootScope', '$scope', 'common', 'DefectTrackingSvc', '$filter', '$modal', 'LookupSvc', '$timeout', function ($rootScope, $scope, common, DefectTrackingSvc, $filter, $modal, LookupSvc, $timeout) {
    //Start implement security role wise
    $scope.setRoleWisePrivilege = function () {
        $scope.currentSecurityObject = currentSecurityObject;
        if (!Isundefinedornull($scope.currentSecurityObject) && $scope.currentSecurityObject.length > 0) {
            angular.forEach($scope.currentSecurityObject, function (obj, index) {
                switch (obj.ObjectId) {
                    case 92:
                        switch (obj.PrivilegeId) {
                            case 1:                           //none
                                RedirectToAccessDenied();
                                break;
                        }
                        break;
                    case 112:
                        switch (obj.PrivilegeId) {
                            case 1:                           //none
                                RedirectToAccessDenied();
                                break;
                        }
                        break;
                    case 103:
                        switch (obj.PrivilegeId) {
                            case 1:                           //none
                                RedirectToAccessDenied();
                                break;
                            case 2:
                                $scope.IsReadOnlyDeleteButton = true;
                                $scope.IsReadOnlyAddButton = true;
                                break;
                            case 3:
                                break;
                        }
                        break;
                    case 104:
                        switch (obj.PrivilegeId) {
                            case 1:                           //none
                                $scope.IsReadOnlyAddButton = true;
                                break;
                            case 2:
                                $scope.IsReadOnlyAddButton = true;
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
    //End implement security role wise
    $scope.setRoleWisePrivilege();

    $rootScope.PageHeader = ($filter('translate')('_DefectTrackingList_'));
    $scope.DefectTrackingList = {};
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;
    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;

        ///Start set search criteria when return from add edit page
        if (localStorage.getItem("DefectTrackingListPaging") && localStorage.getItem("DefectTrackingListPageSearchCriteria")) {
            $scope.Paging = JSON.parse(localStorage.getItem("DefectTrackingListPaging"));
            localStorage.removeItem("DefectTrackingListPaging");
            $scope.SearchCriteria = JSON.parse(localStorage.getItem("DefectTrackingListPageSearchCriteria"));
            localStorage.removeItem("DefectTrackingListPageSearchCriteria");
        }
        ///End set search criteria when return from add edit page

        $scope.GetDefectTrackingList();
        $scope.SetLooksupData();
    };
    $scope.OnPartNoSelect = function ($item, obj) {
        if (!IsNotNullorEmpty($item) || !IsNotNullorEmpty($item.Key)) {
            $scope.SearchCriteria.PartNumberId = $item.Id;
            $scope.SearchCriteria.PartNumber = $item.Name;
        }
        console.log($scope.SearchCriteria.PartNumberId)
    };
    $scope.setPartNoSelect = function (obj) {
        if (!Isundefinedornull($scope.PartsList) && !IsUndefinedNullOrEmpty($scope.SearchCriteria.PartNumber)) {
            var objPartNumber = $filter('filter')($scope.PartsList, function (rw) {
                return rw.Name.toLowerCase() == $scope.SearchCriteria.PartNumber.toLowerCase()
            });
            if (objPartNumber.length <= 0) {
                $scope.SearchCriteria.PartNumberId = '-1';
            }
        }
        else
            $scope.SearchCriteria.PartNumberId = '';
    };
    $scope.GetDefectTrackingList = function () {
        common.usSpinnerService.spin('spnAPQP');
        DefectTrackingSvc.GetDefectTrackingList($scope.Paging).then(
             function (response) {
                 if (response.data.StatusCode == 200) {
                     $scope.DefectTrackingList = response.data.Result;

                     $scope.Paging = response.data.PageInfo;
                     if ($scope.DefectTrackingList.length > 0) {
                         angular.forEach($scope.DefectTrackingList, function (item, index) {
                             item.RMADate = convertLocalDateToUTCDate(item.RMADate);
                             item.PartSupplierQuantityArray = [];
                             if (!IsUndefinedNullOrEmpty(item.PartNumber)) {
                                 item.PartNumber = item.PartNumber.split('@');
                                 angular.forEach(item.PartNumber, function (data, num) {
                                     var PartSupplierQuantityObject = { PartNumber: data, Supplier: '', CustomerRejectedPartQty: '' };
                                     item.PartSupplierQuantityArray.push(PartSupplierQuantityObject);
                                 });
                             }
                             if (!IsUndefinedNullOrEmpty(item.Supplier)) {
                                 item.Supplier = item.Supplier.split('@');
                                 angular.forEach(item.Supplier, function (data, num) {
                                     item.PartSupplierQuantityArray[num].Supplier = data;
                                 });
                             }
                             if (!IsUndefinedNullOrEmpty(item.CustomerRejectedPartQty)) {
                                 item.CustomerRejectedPartQty = item.CustomerRejectedPartQty.split('@');
                                 angular.forEach(item.CustomerRejectedPartQty, function (data, num) {
                                     item.PartSupplierQuantityArray[num].CustomerRejectedPartQty = data;
                                 });
                             }
                         });
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
                 ////common.aaNotify.error(error);
             });
    };
    $scope.SetLooksupData = function () {
        $scope.LookUps = [
            {
                "Name": "SAPItemByCustomer", "Parameters": { "CustomerCode": "" }    //part numbers
            },
            {
                "Name": "Users", "Parameters": {}       // RMA initiated by
            },
          {
              "Name": "MESWarehouses", "Parameters": {}
          }
        ];
        $scope.getLookupData();
    };
    $scope.getLookupData = function () {
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            angular.forEach(data.data, function (o) {
                if (o.Name === "SAPItemByCustomer") {
                    $scope.PartsList = o.Data;
                }
                else if (o.Name === "Users") {
                    $scope.RMAInitiatedByList = o.Data;
                }
                else if (o.Name === "MESWarehouses") {
                    $scope.MESWareHouseList = o.Data;
                }
            });
        });
    }
    $scope.Edit = function (id) {
        localStorage.setItem("DefectTrackingListPaging", JSON.stringify($scope.Paging));
        localStorage.setItem("DefectTrackingListPageSearchCriteria", JSON.stringify($scope.SearchCriteria));
        common.$location.path("/AddEdit/" + id);
    };
    $scope.Delete = function (defectTrackingId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            DefectTrackingSvc.Delete(defectTrackingId).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.GetDefectTrackingList();
               }
           },
           function (error) {
               ////common.aaNotify.error(error);
           });
        }
    };

    $scope.DownloadPDF = function (path) {
        common.usSpinnerService.spin('spnAPQP');
        window.open(path, '_blank');
        common.usSpinnerService.stop('spnAPQP');
    };
    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.GetDefectTrackingList();
    };
    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.GetDefectTrackingList();
    };
    $scope.Search = function () {
        $scope.Init();
    };
    $scope.ResetSearch = function () {
        $scope.SearchCriteria = {};
        $scope.Init();
    }
    $scope.RedirectToAddEditPage = function () {
        common.$location.path("/AddEdit/");
    }
    $scope.Init();
}]);

app.controller('AddEditDTCtrl', ['$rootScope', '$scope', 'common', 'DefectTrackingSvc', 'LookupSvc', '$filter', '$routeParams', '$timeout', '$modal', '$window',
    function ($rootScope, $scope, common, DefectTrackingSvc, LookupSvc, $filter, $routeParams, $timeout, $modal, $window) {
        //Start implement security role wise
        $scope.setRoleWisePrivilege = function () {
            $scope.currentSecurityObject = currentSecurityObject;
            if (!Isundefinedornull($scope.currentSecurityObject) && $scope.currentSecurityObject.length > 0) {
                angular.forEach($scope.currentSecurityObject, function (obj, index) {
                    switch (obj.ObjectId) {
                        case 92:
                            $scope.setSecurityRoleCase(obj);
                            break;
                        case 112:
                            $scope.setSecurityRoleCase(obj);
                            break;
                        case 103:
                            $scope.setSecurityRoleCase(obj);
                            break;
                        case 104:
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

        $scope.SelectAllObj = { SelectAll: false };
        $('body').removeClass('paginationFixedToBottom haveAdvanceSearch');
        $rootScope.PageHeader = ($filter('translate')('_AddEditDTTitle_'));

        $scope.Init = function () {
            $scope.DefectTracking = {
                IncludeInPPM: 'Yes',
                Finding: 'External',
                QualityOrDeliveryIssue: 'Quality',
                lstDefectTrackingDetail: []
            };
            $scope.Document = {};
            $scope.TabObject = {};
            $scope.TabObject.CurrentTab = 'CustomerSortReworkData';

            if (!IsUndefinedNullOrEmpty($routeParams.Id) && $routeParams.Id != '0') {
                $scope.TransactionMode = 'Edit';
                $scope.DefectTracking.Id = $routeParams.Id;
                $scope.getData($routeParams.Id);
            }
            else {
                $scope.TransactionMode = 'Create';
                $scope.GetNewRMANumber();
            }
        }

        //Start fill dropdowns
        $scope.SetLooksupData = function () {
            $scope.LookUps = [
                 {
                     "Name": "RMAInitiatedBy", "Parameters": {}       // RMA initiated by
                 },
                {
                    "Name": "SAPCustomers", "Parameters": {}
                },
                 {
                     "Name": "CurrentUser", "Parameters": {}
                 },
                  {
                      "Name": "MESWarehouses", "Parameters": {}
                  }
            ];
            $scope.getLookupData();
        };
        $scope.getLookupData = function () {
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                angular.forEach(data.data, function (o) {
                    if (o.Name === "RMAInitiatedBy") {
                        $scope.RMAInitiatedByList = o.Data;
                    }
                    else if (o.Name === "SAPCustomers") {
                        $scope.SAPCustomersList = o.Data;
                    }
                    else if (o.Name === "CurrentUser") {
                        if (o.Data.length > 0 && $scope.TransactionMode == "Create")
                            $scope.DefectTracking.RMAInitiatedBy = o.Data[0].Name;
                    }
                    else if (o.Name === "MESWarehouses") {
                        $scope.MESWareHouseList = o.Data;
                    }
                });
            });
        };
        $scope.SetLooksupForPartOnCustomerChange = function () {
            common.usSpinnerService.spin('spnAPQP');
            if (!IsUndefinedNullOrEmpty($scope.DefectTracking.CustomerCode)) {
                $scope.DefectTracking.CustomerName = ($filter('filter')($scope.SAPCustomersList, function (rw) { return rw.Id == $scope.DefectTracking.CustomerCode })[0].ParentId);
                $scope.LookUps = [
                      {
                          "Name": "SAPItemByCustomer", "Parameters": { "CustomerCode": $scope.DefectTracking.CustomerCode }    //part numbers
                      },

                ];
                LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                    angular.forEach(data.data, function (o) {
                        if (o.Name === "SAPItemByCustomer") {
                            $scope.PartsList = o.Data;
                            $scope.DefectTracking.PartNumberItems = [];
                        }
                        //common.usSpinnerService.stop('spnAPQP');
                    });
                    common.usSpinnerService.stop('spnAPQP');
                });
            }
            else {
                $scope.PartsList = [];

                common.usSpinnerService.stop('spnAPQP');
            }
        };
        $scope.SetLooksupForSupplier = function (partnumber, objDefectTrackingDetail) {
            if (!IsUndefinedNullOrEmpty(partnumber)) {
                $scope.LookUps = [
                    {
                        "Name": "SAPSuppliersByPartCode", "Parameters": { "PartCode": partnumber }
                    }
                ];
                LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                    angular.forEach(data.data, function (o) {
                        if (o.Name === "SAPSuppliersByPartCode") {
                            objDefectTrackingDetail.SAPSuppliersList = o.Data;
                        }
                    });
                });
            }
            else
                objDefectTrackingDetail.SAPSuppliersList = [];
        };

        $scope.SetLooksupForSupplierContact = function (supplierCode, objDefectTrackingDetail) {
            if (!IsUndefinedNullOrEmpty(supplierCode)) {
                $scope.LookUps = [
                    {
                        "Name": "SupplierContacts", "Parameters": { "SupplierCode": supplierCode }
                    }
                ];
                LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                    angular.forEach(data.data, function (o) {
                        if (o.Name === "SupplierContacts") {
                            objDefectTrackingDetail.SupplierContactList = o.Data;

                            objDefectTrackingDetail.SupplierContactName = objDefectTrackingDetail.SupplierContactList[0].Name;
                        }
                    });
                });
            }
            else
                objDefectTrackingDetail.SupplierContactList = [];
        };
        $scope.setSupplierName = function (code, item) {
            if (!IsUndefinedNullOrEmpty(code)) {
                item.SupplierName = $filter('filter')(item.SAPSuppliersList, function (rw) { return rw.Id == code })[0].ParentId;
            }
            else
                item.SupplierName = "";
        };
        //End fill dropdowns

        $scope.getData = function (defectTrackingId) {
            common.usSpinnerService.spin('spnAPQP');
            $scope.SetLooksupData();
            DefectTrackingSvc.getData(defectTrackingId).then(
               function (response) {
                   if (ShowMessage(common, response.data)) {
                       $scope.DefectTracking = response.data.Result;

                       $timeout(function () {
                           try {
                               $scope.DefectTracking.RMADate = convertLocalDateToUTCDate($scope.DefectTracking.RMADate);
                               $scope.SetLooksupForPartOnCustomerChange();
                               if (!Isundefinedornull($scope.DefectTracking.lstDefectTrackingDetail)) {
                                   angular.forEach($scope.DefectTracking.lstDefectTrackingDetail, function (item, index) {
                                       item.DateRejected = convertLocalDateToUTCDate(item.DateRejected);
                                       item.SortedStartDate = convertLocalDateToUTCDate(item.SortedStartDate);
                                       item.CorrectiveActionInitiatedDate = convertLocalDateToUTCDate(item.CorrectiveActionInitiatedDate);
                                       item.CorrectiveActionDueDate = convertLocalDateToUTCDate(item.CorrectiveActionDueDate);
                                       item.ActualCompletedDate = convertLocalDateToUTCDate(item.ActualCompletedDate);
                                       item.SortedEndDate = convertLocalDateToUTCDate(item.SortedEndDate);
                                       item.CustomerIssuedCreditDate = convertLocalDateToUTCDate(item.CustomerIssuedCreditDate);
                                       item.SupplierIssuedDebitDate = convertLocalDateToUTCDate(item.SupplierIssuedDebitDate);
                                       if (IsUndefinedNullOrEmpty(item.DispositionOfParts))
                                           item.DispositionOfParts = "Scrap";
                                       if (IsUndefinedNullOrEmpty(item.Region))
                                           item.Region = "USA";

                                       $scope.SetLooksupForSupplier(item.PartNumber, item);
                                       $scope.SetLooksupForSupplierContact(item.SupplierCode, item);
                                   });
                               }

                           } catch (e) {
                           }

                           common.usSpinnerService.stop('spnAPQP');
                       }, 1500);
                   }
                   else {
                       common.usSpinnerService.stop('spnAPQP');
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnAPQP');
               });
        };
        $scope.GetNewRMANumber = function () {
            common.usSpinnerService.spin('spnAPQP');
            DefectTrackingSvc.GetNewRMANumber().then(
               function (response) {
                   if (ShowMessage(common, response.data)) {
                       $scope.DefectTracking.RMANumber = response.data.Result;
                       $scope.DefectTracking.RMADate = convertUTCDateToLocalDate(new Date());
                       $scope.SetLooksupData();
                       $timeout(function () {
                           common.usSpinnerService.stop('spnAPQP');
                       }, 0);
                   }
                   else {
                       common.usSpinnerService.stop('spnAPQP');
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnAPQP');
               });
        };
        $scope.SaveDefectTracking = function (Mode, closeForm, AddToCAPA) {
            common.usSpinnerService.spin('spnAPQP');
            $scope.Mode = Mode;
            if (($scope.TransactionMode == "Create") || ($scope.TransactionMode == "Edit" && $scope.Mode == "DropdownButton")) {
                var PartNumberIds = [];
                angular.forEach($scope.DefectTracking.PartNumberItems, function (item, index) {
                    if (!IsUndefinedNullOrEmpty(item.Id) && parseInt(item.Id) > 0)
                        PartNumberIds.push(item.Id);
                });
                if (PartNumberIds.length > 0)
                    $scope.DefectTracking.PartNumber = PartNumberIds.join(",");
                else {
                    common.usSpinnerService.stop('spnAPQP');
                    common.aaNotify.error("Please select at least one part.");
                    return false;
                }
            }

            if ($scope.ValidateFields($scope.DefectTracking.lstDefectTrackingDetail)) {
                try {
                    $scope.DefectTracking.RMADate = convertUTCDateToLocalDate($scope.DefectTracking.RMADate);
                    $scope.DefectTracking.Mode = $scope.Mode;
                    $scope.DefectTracking.AddToCAPA = AddToCAPA;
                    if (!Isundefinedornull($scope.DefectTracking.lstDefectTrackingDetail)) {
                        angular.forEach($scope.DefectTracking.lstDefectTrackingDetail, function (item, index) {
                            item.DateRejected = convertUTCDateToLocalDate(item.DateRejected);
                            item.SortedStartDate = convertUTCDateToLocalDate(item.SortedStartDate);
                            item.CorrectiveActionInitiatedDate = convertUTCDateToLocalDate(item.CorrectiveActionInitiatedDate);
                            item.CorrectiveActionDueDate = convertUTCDateToLocalDate(item.CorrectiveActionDueDate);
                            item.ActualCompletedDate = convertUTCDateToLocalDate(item.ActualCompletedDate);
                            item.SortedEndDate = convertUTCDateToLocalDate(item.SortedEndDate);
                            item.CustomerIssuedCreditDate = convertUTCDateToLocalDate(item.CustomerIssuedCreditDate);
                            item.SupplierIssuedDebitDate = convertUTCDateToLocalDate(item.SupplierIssuedDebitDate);
                            $scope.SetLooksupForSupplier(item.PartNumber, item);
                            $scope.SetLooksupForSupplierContact(item.SupplierCode, item);
                        });
                    }
                } catch (e) {
                    common.usSpinnerService.stop('spnAPQP');
                    common.aaNotify.error(e);
                    return false;
                }

                DefectTrackingSvc.Save($scope.DefectTracking).then(
                   function (response) {
                       if (ShowMessage(common, response.data)) {
                           common.usSpinnerService.stop('spnAPQP');
                           $scope.Id = response.data.Result // Id of latest created record                           
                           if (AddToCAPA == 0) {
                               $scope.DefectTracking.Id = response.data.Result;
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
                               $window.location.href = "/APQP/CAPA#/AddEdit/" + $scope.Id;
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
            }
        };
        $scope.reloadWithId = function () {
            common.$location.path('/AddEdit/' + $scope.DefectTracking.Id);
        };
        $scope.ValidateFields = function (objList) {
            if ($scope.TransactionMode == "Create" || $scope.Mode == "DropdownButton")
                return true;

            $scope.IsError = false;
            angular.forEach(objList, function (item, index) {
                if (IsUndefinedNullOrEmpty(item.PartName) || IsUndefinedNullOrEmpty(item.DefectDescription)
                                                    || isNaN(parseInt(item.CustomerInitialRejectQty)) || parseInt(item.CustomerInitialRejectQty) <= 0) {
                    if (IsUndefinedNullOrEmpty(item.PartName)) {
                        if (!$('#PartName_' + index).hasClass('invalid-class'))
                            $('#PartName_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#PartName_' + index).removeClass('invalid-class');
                    }
                    if (IsUndefinedNullOrEmpty(item.DefectDescription)) {
                        if (!$('#DefectDescription_' + index).hasClass('invalid-class'))
                            $('#DefectDescription_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#DefectDescription_' + index).removeClass('invalid-class');
                    }
                    if (isNaN(parseInt(item.CustomerInitialRejectQty)) || parseInt(item.CustomerInitialRejectQty) <= 0) {
                        if (!$('#CustomerInitialRejectQty_' + index).hasClass('invalid-class'))
                            $('#CustomerInitialRejectQty_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#CustomerInitialRejectQty_' + index).removeClass('invalid-class');
                    }
                }
                else {
                    $('#PartName_' + index).removeClass('invalid-class');
                    $('#DefectDescription_' + index).removeClass('invalid-class');
                    $('#CustomerInitialRejectQty_' + index).removeClass('invalid-class');
                }
            });
            angular.forEach(objList, function (item, index) {
                if (!$scope.IsError) {
                    if (IsUndefinedNullOrEmpty(item.PartName) || IsUndefinedNullOrEmpty(item.DefectDescription)
                        || isNaN(parseInt(item.CustomerInitialRejectQty)) || parseInt(item.CustomerInitialRejectQty) <= 0) {
                        common.usSpinnerService.stop('spnAPQP');
                        $scope.IsError = true;
                        common.aaNotify.error($filter('translate')('_IsRequiredErrMsg_'));
                        $scope.TabObject.CurrentTab = 'CustomerSortReworkData';
                        return false;
                    }
                    if (!isNaN(parseInt(item.CustomerRejectedPartQty)) && parseInt(item.CustomerRejectedPartQty) < 0) {
                        common.usSpinnerService.stop('spnAPQP');
                        $scope.IsError = true;
                        common.aaNotify.error("Total Qty Reworked /Saved @ Customer Value Greater than Total Qty.");
                        return false;
                    }
                }
            });
            if (!$scope.IsError) {
                return true;
            }
        };
        $scope.RowwiseCalculations = function (item, index) {
            var varCustomerInitialRejectQty = 0, varCustomerAdditionalRejectQty = 0, varCustomerTotalReworkedQty = 0, varFinalQtyScrapped = 0;
            if (!IsUndefinedNullOrEmpty(item.CustomerInitialRejectQty) && !isNaN(parseInt(item.CustomerInitialRejectQty)))
                varCustomerInitialRejectQty = parseInt(item.CustomerInitialRejectQty);
            if (!IsUndefinedNullOrEmpty(item.CustomerAdditionalRejectQty) && !isNaN(parseInt(item.CustomerAdditionalRejectQty)))
                varCustomerAdditionalRejectQty = parseInt(item.CustomerAdditionalRejectQty);
            if (!IsUndefinedNullOrEmpty(item.CustomerTotalReworkedQty) && !isNaN(parseInt(item.CustomerTotalReworkedQty)))
                varCustomerTotalReworkedQty = parseInt(item.CustomerTotalReworkedQty);
            item.CustomerRejectedPartQty = varCustomerInitialRejectQty + varCustomerAdditionalRejectQty - varCustomerTotalReworkedQty;

            if (!IsUndefinedNullOrEmpty(item.FinalQtyScrapped) && !isNaN(parseInt(item.FinalQtyScrapped)))
                varFinalQtyScrapped = parseInt(item.FinalQtyScrapped);
            item.TotalNumberOfPartsRejected = parseInt(item.CustomerRejectedPartQty) + varFinalQtyScrapped;
        };
        $scope.GenerateCAPAForm = function () {
            common.usSpinnerService.spin('spnAPQP');
            var defectTrackingDetailIds = [], defectTrackingSupplierCodes = [], strdefectTrackingDetailIds = '';
            angular.forEach($scope.DefectTracking.lstDefectTrackingDetail, function (item, index) {
                if (item.chkSelect) {
                    defectTrackingDetailIds.push(item.Id);
                    defectTrackingSupplierCodes.push(item.SupplierCode);
                }
            });
            if (defectTrackingDetailIds.length > 0)
                strdefectTrackingDetailIds = defectTrackingDetailIds.join(",");
            else {
                common.usSpinnerService.stop('spnAPQP');
                common.aaNotify.error("Please select at least one part.");
                return false;
            }

            //Start check if any supplier selected or not and all supplier should be same.
            var obj = _.uniq(defectTrackingSupplierCodes);
            if (obj.length > 1) {
                common.usSpinnerService.stop('spnAPQP');
                alert('A CAPA form can be associated with only one supplier.Please select the same supplier for all parts.')
                return false;
            }
            else if (obj.length == 1 && obj[0] == null) {
                common.usSpinnerService.stop('spnAPQP');
                common.aaNotify.error("Please select supplier.");
                return false;
            }
            else if (obj.length == 0) {
                common.usSpinnerService.stop('spnAPQP');
                common.aaNotify.error("Please select supplier.");
                return false;
            }
            //End check if any supplier selected or not and all supplier should be same.

            DefectTrackingSvc.GenerateCAPAForm($scope.DefectTracking).then(
               function (response) {
                   common.usSpinnerService.stop('spnAPQP');
                   if (response.data.StatusCode == 200) {
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
        $scope.DeleteDefectTrackingDetail = function (defectTrackingDetailId) {
            if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                common.usSpinnerService.spin('spnAPQP');
                DefectTrackingSvc.DeleteDefectTrackingDetail(defectTrackingDetailId).then(
                  function (response) {
                      if (ShowMessage(common, response.data)) {
                          $scope.Init();
                      }
                      else
                          common.usSpinnerService.stop('spnAPQP');
                  },
                  function (error) {
                      common.usSpinnerService.stop('spnAPQP');
                  });
            }
        };
        $scope.AddToCAPAFromDT = function () {
            common.usSpinnerService.spin('spnAPQP');
            var defectTrackingDetailIds = [], defectTrackingSupplierCodes = [], defectTrackingSupplierContactNames = [], strdefectTrackingDetailIds = '';
            angular.forEach($scope.DefectTracking.lstDefectTrackingDetail, function (item, index) {
                if (item.chkSelect) {
                    defectTrackingDetailIds.push(item.Id);
                    defectTrackingSupplierCodes.push(item.SupplierCode);
                    defectTrackingSupplierContactNames.push(item.SupplierContactName);
                }
            });
            if (defectTrackingDetailIds.length > 0)
                strdefectTrackingDetailIds = defectTrackingDetailIds.join(",");
            else {
                common.usSpinnerService.stop('spnAPQP');
                common.aaNotify.error("Please select at least one part.");
                return false;
            }
            //Start check if any supplier(and contact) selected or not and all supplier(and contact) should be same.
            var obj = _.uniq(defectTrackingSupplierCodes);
            var objContact = _.uniq(defectTrackingSupplierContactNames);
            if (obj.length > 1) {
                common.usSpinnerService.stop('spnAPQP');
                alert('A CAPA form can be associated with only one supplier. Please select the same supplier for all parts.');
                return false;
            }
            else if (obj.length == 1 && obj[0] == null) {
                common.usSpinnerService.stop('spnAPQP');
                common.aaNotify.error("Please select supplier.");
                return false;
            }
            else if (obj.length == 0) {
                common.usSpinnerService.stop('spnAPQP');
                common.aaNotify.error("Please select supplier.");
                return false;
            }
            else if (objContact.length > 1) {
                common.usSpinnerService.stop('spnAPQP');
                alert("A CAPA form can be associated with only one supplier contact. Please select the same supplier contact for all parts.");
                //common.aaNotify.error("A CAPA form can be associated with only one supplier contact. Please select the same supplier contact for all parts.");
                return false;
            }
            //End check if any supplier selected or not and all supplier should be same.            
            $scope.SaveDefectTracking('FinalSaveButton', 0, 1);
        };
        //Start logic for checkbox select deselect here
        $scope.SelectDeselectAll = function () {
            angular.forEach($scope.DefectTracking.lstDefectTrackingDetail, function (item) {
                item.chkSelect = $scope.SelectAllObj.SelectAll;
            });
        };
        $scope.select = function () {
            $scope.SelectAllObj.SelectAll = true;
            angular.forEach($scope.DefectTracking.lstDefectTrackingDetail, function (item) {
                if (!item.chkSelect)
                    $scope.SelectAllObj.SelectAll = false;
            });
        };
        //end logic for checkbox select deselect here

        $scope.ShowPopupDocuments = function (callFrom, defectTrackingDetailId) {
            var modalTemplatePreviewOptions = $modal.open({
                templateUrl: '/App_Client/views/APQP/DefectTracking/DocumentPopup.html?v=' + Version,
                controller: ModalDocumentCtrl,
                keyboard: false,
                backdrop: false,
                scope: $scope,
                sizeclass: 'modal-fitToScreen',
                resolve: {
                    CallFrom: function () {
                        return callFrom;
                    },
                    DefectTrackingDetailId: function () {
                        return defectTrackingDetailId;
                    }
                }
            });
            modalTemplatePreviewOptions.result.then(function () {
            }, function () {
            });
        };
        $scope.isActive = function (route) {
            return ($scope.TabObject.CurrentTab == route);
        };
        $scope.RedirectToList = function () {
            common.$location.path("/");
        };
        $scope.ResetForm = function () {
            common.$route.reload();
        };

        //popup for show audit log 
        $scope.ShowChangeLogPopup = function () {
            var modalTemplatePreviewOptions = $modal.open({
                templateUrl: '/App_Client/views/APQP/DefectTracking/AuditLogsPopup.html?v=' + Version,
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
var ModalDocumentCtrl = function ($scope, $modalInstance, common, $timeout, $filter, $confirm, LookupSvc, DefectTrackingSvc, CallFrom, DefectTrackingDetailId) {
    $scope.DocumentsInit = function () {
        $scope.CallFrom = CallFrom;
        $scope.FolderName = "DTCAPADocuments"; //GetDTStepName($scope.CallFrom);
        $scope.IsEditMode = "No";
        $scope.DocumentTypeId = 0;
        $scope.AssociatedToId = 0;
        $scope.AddEditDocument = false;
        $scope.Document = {};
        $scope.DocumentOriginal = {};
        $scope.DocumentsList = {};
        $scope.DocumentsListWithGroup = {};
        $scope.GetPartDocumentList(DefectTrackingDetailId);
    };
    $scope.GetPartDocumentList = function (id) {
        common.usSpinnerService.spin('spnAPQP');
        DefectTrackingSvc.GetPartDocumentList(id, $scope.CallFrom).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.DocumentsList = response.data.Result;
                   $scope.DocumentsListWithGroup = _.groupBy($scope.DocumentsList, 'DocumentType');
                   $scope.IsAccordionObjectEmpty = IsObjectEmpty($scope.DocumentsListWithGroup);
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
    $scope.SetDocumentLooksupData = function () {
        common.usSpinnerService.spin('spnAPQP');
        $scope.LookUps = [
        {
            "Name": "DTDocumentType", "Parameters": { "APQPItemId": DefectTrackingDetailId, "DocumentTypeId": $scope.DocumentTypeId, "IsEditMode": $scope.IsEditMode, "AssociatedToId": $scope.AssociatedToId }
        }
        ];
        $scope.getDocumentLookupData();
    };
    $scope.getDocumentLookupData = function () {
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            angular.forEach(data.data, function (o) {
                if (o.Name === "DTDocumentType") {
                    $scope.DocumentLookupList = o.Data;
                    if ($scope.IsEditMode == "Yes") {
                        $scope.Document.DocumentTypeItem = ($filter('filter')($scope.DocumentLookupList, function (rw) { return rw.Id == $scope.DocumentTypeId && rw.ParentId == $scope.AssociatedToId })[0]);
                    }
                }
            });
            common.usSpinnerService.stop('spnAPQP');
        }, function (error) {
            common.usSpinnerService.stop('spnAPQP');
        });
    };
    $scope.setAssociatedTo = function (item) {
        if (!Isundefinedornull(item)) {
            $scope.Document.AssociatedToId = item.ParentId;
            $scope.Document.DocumentTypeId = item.Id;
        }
        else {
            $scope.Document.AssociatedToId = "";
            $scope.Document.DocumentTypeId = 0;
        }
    };
    $scope.AddNewDocument = function () {
        $scope.ResetDocument();
        $scope.AddEditDocument = true;
        $scope.SetDocumentLooksupData();
    };
    $scope.EditDocument = function (item, index, documentId) {
        $scope.Document = {};
        $scope.Document = angular.copy(item);
        $scope.DocumentOriginal = angular.copy(item);
        $scope.DocumentTypeId = $scope.Document.DocumentTypeId;
        $scope.AssociatedToId = $scope.Document.AssociatedToId;
        $scope.IsEditMode = "Yes";

        if (!IsUndefinedNullOrEmpty(item.FilePath)) {
            $confirm({ text1: ($filter('translate')('_DocumentConfirmText_')), ok: ($filter('translate')('_Yes_')), cancel: ($filter('translate')('_No_')) })
             .then(function (ret) {
                 if (ret == 1) {
                     $scope.EditORRevisionDocument(false, index, documentId);
                 }
                 else {
                     $scope.EditORRevisionDocument(true, index, documentId);
                 }
             },
              function () {
                  $scope.EditORRevisionDocument(false, index, documentId);
              });
        }
        else
            $scope.EditORRevisionDocument(false, index, documentId);
    };
    $scope.EditORRevisionDocument = function (isRevision, index, documentId) {
        if (isRevision) {
            $scope.Document.Id = 0;
            $scope.DocumentOriginal = {};
        }
        $scope.AddEditDocument = true;
        $scope.SetDocumentLooksupData();
    };
    $scope.DeleteDocument = function (documentId, index) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            common.usSpinnerService.spin('spnAPQP');
            DefectTrackingSvc.DeleteDocument(documentId).then(
              function (response) {
                  if (ShowMessage(common, response.data)) {
                      $scope.ResetDocument();
                      $scope.GetPartDocumentList(DefectTrackingDetailId);
                  }
                  else
                      common.usSpinnerService.stop('spnAPQP');
              },
              function (error) {
                  common.usSpinnerService.stop('spnAPQP');
              });
        }
    };
    $scope.SaveDocument = function () {
        if ($scope.DocumentValidation($scope.Document)) {
            $scope.Document.SectionName = $scope.CallFrom;
            $scope.Document.DefectTrackingDetailId = DefectTrackingDetailId;
            common.usSpinnerService.spin('spnAPQP');
            DefectTrackingSvc.SaveDocument($scope.Document).then(
              function (response) {
                  if (ShowMessage(common, response.data)) {
                      common.usSpinnerService.stop('spnAPQP');
                      $scope.ResetDocument();
                      $scope.GetPartDocumentList(DefectTrackingDetailId);
                  }
                  else {
                      common.usSpinnerService.stop('spnAPQP');
                  }
              },
              function (error) {
                  common.usSpinnerService.stop('spnAPQP');
              });
        }
    };
    $scope.CancelDocument = function () {
        //if (!Isundefinedornull($scope.APQPDocumentOriginal) && !IsUndefinedNullOrEmpty($scope.APQPDocumentOriginal.DocumentTypeId)) {
        //    $scope.DocumentsList.push($scope.APQPDocumentOriginal);
        //}
        $scope.ResetDocument();
    };
    $scope.ResetDocument = function () {
        $scope.IsEditMode = "No";
        $scope.DocumentOriginal = {};
        $scope.Document = {}
        $scope.AddEditDocument = false;
        $scope.DocumentTypeId = 0;
        $scope.AssociatedToId = 0;
    };
    $scope.DocumentValidation = function (objDocument) {
        if (IsUndefinedNullOrEmpty(objDocument.DocumentTypeId)) {
            common.aaNotify.error(($filter('translate')('_DocumentType_')) + ' is required.');
            return false;
        }
        else if (IsUndefinedNullOrEmpty(objDocument.FilePath)) {
            common.aaNotify.error(($filter('translate')('_FileTitle_')) + ' is required.');
            return false;
        }
        return true;
    };
    $scope.SetObjectvalues = function (UploadedFilePath, UploadedFileName) {
        $scope.Document.FilePath = UploadedFilePath;
        $scope.Document.FileTitle = UploadedFileName;
    };
    $scope.deleteFile = function (UploadedFilePath) {
        $scope.Document.FilePath = '';
        $scope.Document.FileTitle = '';
    };
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
        //common.$route.reload();
    };
    $scope.DocumentsInit();
};

var ViewChangeLogPageInstance = function ($scope, $modalInstance, common, AuditLogsSvc) {
    $scope.InitializeAuditLogObjects = function () {
        if (!IsUndefinedNullOrEmpty($scope.DefectTracking.Id) && parseInt($scope.DefectTracking.Id) > 0) {
            //defect tracking
            $scope.DefectTrackingPaging = GetDefaultPageObject();
            $scope.DefectTrackingCriteria = { ItemId: $scope.DefectTracking.Id };
            $scope.DefectTrackingPaging.Criteria = $scope.DefectTrackingCriteria;
            $scope.DefectTrackingLogsList = {};

            //defect tracking details
            $scope.DefectTrackingDetailPaging = GetDefaultPageObject();
            $scope.DefectTrackingDetailCriteria = { ItemId: $scope.DefectTracking.Id };
            $scope.DefectTrackingDetailPaging.Criteria = $scope.DefectTrackingDetailCriteria;
            $scope.DefectTrackingDetailLogsList = {};

            $scope.ViewAuditLogs(true);
        }
        else
            $scope.ChangeLogCancel();
    }
    $scope.ViewAuditLogs = function (loadAll) {
        common.usSpinnerService.spin('spnAPQP');
        AuditLogsSvc.GetAuditLogDefectTracking($scope.DefectTrackingPaging).then(
             function (response) {
                 common.usSpinnerService.stop('spnAPQP');
                 if (response.data.StatusCode == 200) {
                     $scope.DefectTrackingLogsList = response.data.Result;
                     $scope.DefectTrackingPaging = response.data.PageInfo;
                     $scope.DefectTrackingLogsList = _.groupBy($scope.DefectTrackingLogsList, 'UpdatedOn');
                     $scope.IsAccordionObjectEmptyDT = IsObjectEmpty($scope.DefectTrackingLogsList);
                     if (loadAll)
                         $scope.ViewAuditLogsDTD();
                 }
                 else {
                     $scope.IsAccordionObjectEmptyDT = true;
                 }
             },
             function (error) {
                 $scope.IsAccordionObjectEmptyDT = true;
                 common.usSpinnerService.stop('spnAPQP');
             });
    };
    $scope.ViewAuditLogsDTD = function () {
        common.usSpinnerService.spin('spnAPQP');
        AuditLogsSvc.GetAuditLogDefectTrackingDetails($scope.DefectTrackingDetailPaging).then(
             function (response) {
                 common.usSpinnerService.stop('spnAPQP');
                 if (response.data.StatusCode == 200) {
                     $scope.DefectTrackingDetailLogsList = response.data.Result;
                     $scope.DefectTrackingDetailPaging = response.data.PageInfo;
                     $scope.DefectTrackingDetailLogsList = _.groupBy($scope.DefectTrackingDetailLogsList, 'UpdatedOn');
                     $scope.IsAccordionObjectEmptyDTD = IsObjectEmpty($scope.DefectTrackingDetailLogsList);
                     angular.forEach($scope.DefectTrackingDetailLogsList, function (item, index) {
                         $scope.DefectTrackingDetailLogsList[index] = _.groupBy(item, 'PartNumber');
                     });
                 }
                 else {
                     $scope.IsAccordionObjectEmptyDTD = true;
                 }
             },
             function (error) {
                 $scope.IsAccordionObjectEmptyDTD = true;
                 common.usSpinnerService.stop('spnAPQP');
             });
    };

    //defect tracking
    $scope.DTPageSizeChanged = function (PageSize) {
        $scope.DefectTrackingPaging.PageSize = PageSize;
        $scope.ViewAuditLogs(false);
    };
    $scope.DTPageChanged = function (PageNo) {
        $scope.DefectTrackingPaging.PageNo = PageNo;
        $scope.ViewAuditLogs(false);
    };

    //defect tracking details
    $scope.DTDPageSizeChanged = function (PageSize) {
        $scope.DefectTrackingDetailPaging.PageSize = PageSize;
        $scope.ViewAuditLogsDTD();
    };
    $scope.DTDPageChanged = function (PageNo) {
        $scope.DefectTrackingDetailPaging.PageNo = PageNo;
        $scope.ViewAuditLogsDTD();
    };
    $scope.setUpdatedDate = function (item) {
        var strDate = '';
        angular.forEach(item, function (obj, index) {
            if (strDate == '')
                strDate = obj[0].UpdatedOnString;
        });
        return strDate;
    };
    $scope.ChangeLogCancel = function () {
        $modalInstance.dismiss('cancel');
    }
    $scope.InitializeAuditLogObjects();
}

app.directive("changeBgcDefectTracking", function () {
    return {
        replace: false,
        restrict: 'A',
        link: function (scope, $element, $attrs) {
            var e = $element;
            var index = parseInt($attrs.changeBgcDefectTracking, 10);
            e.on('blur', function () {
                scope.setInvalidClass(scope.DefectTrackingDetail, index);
            });
            scope.setInvalidClass = function (item, index) {
                if (IsUndefinedNullOrEmpty(item.PartName) || IsUndefinedNullOrEmpty(item.DefectDescription)
                                                        || isNaN(parseInt(item.CustomerInitialRejectQty)) || parseInt(item.CustomerInitialRejectQty) <= 0) {
                    if (IsUndefinedNullOrEmpty(item.PartName)) {
                        if (!$('#PartName_' + index).hasClass('invalid-class'))
                            $('#PartName_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#PartName_' + index).removeClass('invalid-class');
                    }
                    if (IsUndefinedNullOrEmpty(item.DefectDescription)) {
                        if (!$('#DefectDescription_' + index).hasClass('invalid-class'))
                            $('#DefectDescription_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#DefectDescription_' + index).removeClass('invalid-class');
                    }
                    if (isNaN(parseInt(item.CustomerInitialRejectQty)) || parseInt(item.CustomerInitialRejectQty) <= 0) {
                        if (!$('#CustomerInitialRejectQty_' + index).hasClass('invalid-class'))
                            $('#CustomerInitialRejectQty_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#CustomerInitialRejectQty_' + index).removeClass('invalid-class');
                    }
                }
                else {
                    $('#PartName_' + index).removeClass('invalid-class');
                    $('#DefectDescription_' + index).removeClass('invalid-class');
                    $('#CustomerInitialRejectQty_' + index).removeClass('invalid-class');
                }

            }
        }
    }
});