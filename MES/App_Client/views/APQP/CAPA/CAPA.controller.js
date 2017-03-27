app.controller('CAPACtrl', ['$rootScope', '$scope', 'common', 'CAPASvc', '$filter', '$modal', 'LookupSvc', '$timeout', function ($rootScope, $scope, common, CAPASvc, $filter, $modal, LookupSvc, $timeout) {

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
                    case 146:
                        switch (obj.PrivilegeId) {
                            case 1:                           //none
                                RedirectToAccessDenied();
                                break;
                        }
                        break;
                    case 147:
                        switch (obj.PrivilegeId) {
                            case 1:                           //none
                                RedirectToAccessDenied();
                                break;
                            case 2:                          //read only
                                $scope.IsReadOnlyAddButton = true;
                                $scope.IsReadOnlyDeleteButton = true;
                                break;
                            case 3:                         //write
                                break;
                        }
                        break;
                    case 148:
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

    $rootScope.PageHeader = ($filter('translate')('_CAPAList_'));
    $scope.CAPAList = {};
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;

    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;

        ///Start set search criteria when return from add edit page
        if (localStorage.getItem("CAPAListPaging") && localStorage.getItem("CAPAListPageSearchCriteria")) {
            $scope.Paging = JSON.parse(localStorage.getItem("CAPAListPaging"));
            localStorage.removeItem("CAPAListPaging");
            $scope.SearchCriteria = JSON.parse(localStorage.getItem("CAPAListPageSearchCriteria"));
            localStorage.removeItem("CAPAListPageSearchCriteria");
        }
        ///End set search criteria when return from add edit page

        $scope.GetCAPAList();
        $scope.SetLooksupData();
    };
    $scope.GetCAPAList = function () {
        common.usSpinnerService.spin('spnAPQP');
        CAPASvc.GetCAPAList($scope.Paging).then(
             function (response) {
                 if (response.data.StatusCode == 200) {
                     $scope.CAPAList = response.data.Result;

                     $scope.Paging = response.data.PageInfo;
                     if ($scope.CAPAList.length > 0) {
                         angular.forEach($scope.CAPAList, function (item) {
                             item.CorrectiveActionInitiatedDate = convertUTCDateToLocalDate(item.CorrectiveActionInitiatedDate);
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
                 "Name": "RMAInitiatedBy", "Parameters": {}       // CAPA initiated by
             },
             {
                 "Name": "DefectType", "Parameters": {}
             }
        ];
        $scope.getLookupData();
    };
    $scope.getLookupData = function () {
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            angular.forEach(data.data, function (o) {
                if (o.Name === "RMAInitiatedBy") {
                    $scope.CAPAInitiatedByList = o.Data;
                }
                else if (o.Name === "DefectType") {
                    $scope.DefectTypeList = o.Data;
                }
            });
        });
    }

    $scope.OnPartNoSelect = function ($item, obj) {
        if (!IsNotNullorEmpty($item) || !IsNotNullorEmpty($item.Key)) {
            $scope.SearchCriteria.APQPItemId = $item.Name;
        }
    };

    $scope.Edit = function (id) {
        localStorage.setItem("CAPAListPaging", JSON.stringify($scope.Paging));
        localStorage.setItem("CAPAListPageSearchCriteria", JSON.stringify($scope.SearchCriteria));
        common.$location.path("/AddEdit/" + id);
    };
    $scope.Delete = function (capaId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            CAPASvc.Delete(capaId).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.GetCAPAList();
               }
           },
           function (error) {
               ////common.aaNotify.error(error);
           });
        }
    };
    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.GetCAPAList();
    };
    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.GetCAPAList();
    };
    $scope.Search = function () {
        $scope.SearchCriteria.OpenDateFrom = convertUTCDateToLocalDate($scope.SearchCriteria.OpenDateFrom);
        $scope.SearchCriteria.OpenDateTo = convertUTCDateToLocalDate($scope.SearchCriteria.OpenDateTo);
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

app.controller('AddEditCAPACtrl', ['$rootScope', '$scope', 'common', 'CAPASvc', 'LookupSvc', '$filter', '$routeParams', '$timeout', '$modal', '$window',
    function ($rootScope, $scope, common, CAPASvc, LookupSvc, $filter, $routeParams, $timeout, $modal, $window) {

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
                        case 146:
                            switch (obj.PrivilegeId) {
                                case 1:                           //none
                                    RedirectToAccessDenied();
                                    break;
                            }
                            break;
                            //case 147:
                            //    $scope.PagePrivilegeCase(obj);
                            //    break;
                        case 148:
                            $scope.PagePrivilegeCase(obj);
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

        $scope.SelectAllObj = { SelectAll: false };
        $('body').removeClass('paginationFixedToBottom haveAdvanceSearch');
        $rootScope.PageHeader = ($filter('translate')('_AddEditCAPATitle_'));
        $scope.Init = function () {
            $scope.CAPA = {
                IncludeInPPM: 'Yes',
                CorrectiveActionType: 'External',
                CorrectiveActionInitiatedDate: convertUTCDateToLocalDate(new Date()),
                lstcapaPartAffectedDetails: [],
                lstcapaProblemDescriptions: [],
                lstcapaTempCountermeasures: [],
                lstcapaVerifications: [],
                lstcapaRootCauseWhyMade: [],
                lstcapaRootCauseWhyShipped: [],
                lstcapaPermanentCountermeasures: [],
                lstcapaFeedForwards: [],
                lstcapaApprovals: []
            };
            $scope.Document = {};
            $scope.Header = 'CAPA';
            $scope.objProblemDescription = {};
            $scope.objTempCountermeasures = {};
            $scope.objCAPAVerifications = {};
            $scope.objRootCauseWhyMade = { QueryId: 5 };
            $scope.objRootCauseWhyShipped = { QueryId: 5 };
            $scope.objPermanentCountermeasures = {};
            $scope.objFeedForwards = {};

            if (!IsUndefinedNullOrEmpty($routeParams.Id) && $routeParams.Id != '0') {
                $scope.TransactionMode = 'Edit';
                $scope.CAPA.Id = $routeParams.Id;
                $scope.SetLooksupData();
                $scope.getData($routeParams.Id);
            }
            else {
                $scope.TransactionMode = 'Create';
                $scope.SetLooksupData();
                $timeout(function () {
                    common.usSpinnerService.stop('spnAPQP');
                }, 0);
            }
            $scope.setRoleWisePrivilege();
        }

        //Start fill dropdowns
        $scope.SetLooksupData = function () {
            $scope.LookUps = [
                {
                    "Name": "RMAInitiatedBy", "Parameters": {}       // CAPA initiated by
                },
                {
                    "Name": "SAPCustomers", "Parameters": {}
                },
                {
                    "Name": "CurrentUser", "Parameters": {}
                },
                {
                    "Name": "DefectType", "Parameters": {}
                },
                {
                    "Name": "CAPAQuery", "Parameters": {}
                },
                {
                    "Name": "CAPAApproverTitle", "Parameters": {}
                }
            ];
            $scope.getLookupData();
        };
        $scope.getLookupData = function () {
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                angular.forEach(data.data, function (o) {
                    if (o.Name === "RMAInitiatedBy") {
                        $scope.CAPAInitiatedByList = o.Data;
                    }
                    else if (o.Name === "SAPCustomers") {
                        $scope.SAPCustomersList = o.Data;
                    }
                    else if (o.Name === "CurrentUser") {
                        if (o.Data.length > 0 && $scope.TransactionMode == "Create" && !Isundefinedornull($scope.CAPAInitiatedByList)) {
                            $scope.CAPA.CorrectiveActionInitiatedBy = o.Data[0].Name;
                            var objCAPAInitiatedBy = angular.copy($filter('filter')($scope.CAPAInitiatedByList, function (rw) { return rw.Id == $scope.CAPA.CorrectiveActionInitiatedBy }));
                            if (objCAPAInitiatedBy.length <= 0) {
                                $scope.CAPA.CorrectiveActionInitiatedBy = null;
                            }
                        }
                    }
                    else if (o.Name === "DefectType") {
                        $scope.DefectTypeList = o.Data;
                    }
                    else if (o.Name === "CAPAQuery") {
                        $scope.QueryList = o.Data;
                        $scope.FilteredQueryList = o.Data;
                        $scope.RootCauseWhyShippedQueryList = angular.copy($filter('filter')($scope.QueryList, function (rw) { return rw.Id == 5 }));
                        $scope.RootCauseWhyMadeQueryList = angular.copy($filter('filter')($scope.QueryList, function (rw) { return rw.Id == 5 }));
                    }
                    else if (o.Name === "CAPAApproverTitle") {
                        $scope.CAPAApproverTitlesList = o.Data;
                        if ($scope.TransactionMode == 'Create') {
                            $scope.CAPA.lstcapaApprovals = [];
                            angular.forEach($scope.CAPAApproverTitlesList, function (item) {
                                $scope.temObjApproval = { Title: item.Name, TitleId: item.Id, Name: '', ApprovalDate: null };
                                $scope.CAPA.lstcapaApprovals.push($scope.temObjApproval);
                            });
                        }
                    }
                });
            });
        };

        $scope.SetLooksupForPartOnCustomerChange = function () {
            common.usSpinnerService.spin('spnAPQP');
            if (!IsUndefinedNullOrEmpty($scope.CAPA.CustomerCode)) {
                $scope.CAPA.CustomerName = ($filter('filter')($scope.SAPCustomersList, function (rw) { return rw.Id == $scope.CAPA.CustomerCode })[0].ParentId);
                $scope.LookUps = [
                      {
                          "Name": "SAPItemByCustomer", "Parameters": { "CustomerCode": $scope.CAPA.CustomerCode }    //part numbers
                      }
                ];
                LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                    angular.forEach(data.data, function (o) {
                        if (o.Name === "SAPItemByCustomer") {
                            $scope.SAPPartsList = o.Data;
                            $scope.CAPA.PartNumberItems = [];
                        }
                        //common.usSpinnerService.stop('spnAPQP');
                    });
                    common.usSpinnerService.stop('spnAPQP');
                });
            }
            else {
                $scope.PartsList = [];
                $scope.SAPSuppliersList = [];
                common.usSpinnerService.stop('spnAPQP');
            }
        };
        $scope.SetLooksupForSupplierContact = function (supplierCode, mode) {

            if (!IsUndefinedNullOrEmpty(supplierCode)) {
                $scope.LookUps = [
                    {
                        "Name": "SupplierContacts", "Parameters": { "SupplierCode": supplierCode }
                    }
                    ,
                     {
                         "Name": "SAPSuppliersByCustomer", "Parameters": { "CustomerNameWithCode": ($scope.CAPA.CustomerCode + ' - ' + $scope.CAPA.CustomerName) }
                     }
                ];
                LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                    angular.forEach(data.data, function (o) {
                        if (o.Name === "SupplierContacts") {
                            $scope.SupplierContactList = o.Data;
                        }
                        else if (o.Name === "SAPSuppliersByCustomer" && $scope.TransactionMode == "Edit") {
                            $scope.SAPSuppliersList = o.Data;
                            $scope.getSAPPartsList(mode);
                        }
                    });
                });
            }
            else
                $scope.SupplierContactList = [];
        };
        $scope.setSupplierName = function (code) {
            if (!IsUndefinedNullOrEmpty(code)) {
                $scope.CAPA.SupplierName = $filter('filter')($scope.SAPSuppliersList, function (rw) { return rw.Id == code })[0].ParentId;
            }
            else
                $scope.CAPA.SupplierName = "";
        };
        $scope.setCustomerName = function (code) {
            if (!IsUndefinedNullOrEmpty(code)) {
                $scope.CAPA.CustomerName = $filter('filter')($scope.SAPCustomersList, function (rw) { return rw.Id == code })[0].ParentId;
            }
            else
                $scope.CAPA.CustomerName = "";
        };

        $scope.OnSupplierContactSelect = function ($item, objCAPA) {
            if (!IsNotNullorEmpty($item) || !IsNotNullorEmpty($item.Key)) {
                //objCAPA.SupplierContactName = $item.Id;
                objCAPA.SupplierContactName = $item.Name;
            }
        };

        $scope.setSupplierContact = function (objCAPA) {

        };

        $scope.GetSAPSupplier = function () {

            var PartNumberIds = [];
            var PartCodes = [];
            angular.forEach($scope.CAPA.PartNumberItems, function (item, index) {
                if (!IsUndefinedNullOrEmpty(item.Id) && parseInt(item.Id) > 0) {
                    PartNumberIds.push(item.Id);
                    PartCodes.push(item.Code);
                }
            });
            if (PartNumberIds.length > 0) {
                $scope.CAPA.PartNumber = PartNumberIds.join(",");
                $scope.CAPA.PartCodes = PartCodes.join(",");
            }

            if ($scope.TransactionMode == "Create") {            

                if (PartNumberIds.length > 0) {
                  
                    $scope.LookUps = [
                          {
                              "Name": "SAPSuppliersByPartCode", "Parameters": { "PartCode": $scope.CAPA.PartCodes }    //supplierlist
                          }
                    ];
                    LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                        angular.forEach(data.data, function (o) {
                            if (o.Name === "SAPSuppliersByPartCode") {
                                $scope.SAPSuppliersList = o.Data;
                            }
                        });
                        common.usSpinnerService.stop('spnAPQP');
                    });
                }
                else {
                    common.usSpinnerService.stop('spnAPQP');
                    common.aaNotify.error("Please select at least one part.");
                    return false;
                }
                console.log($scope.CAPA.PartNumber);
            }
        };
        //End fill dropdowns

        //Start Section - 1 - CAPA - Part Name(s) and Part#(s) Affected:
        $scope.DeleteCAPAPartAffectedDetail = function (item) {
            if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                item.IsDeletedFromObject = true;
                common.aaNotify.success(($filter('translate')('_CAPADeleteSuccessMessage_')));

                //common.usSpinnerService.spin('spnAPQP');
                //CAPASvc.DeleteCAPAPartAffectedDetail(CAPAPartAffectedDetailId).then(
                //  function (response) {
                //      if (ShowMessage(common, response.data)) {
                //          $scope.Init();
                //      }
                //      else
                //          common.usSpinnerService.stop('spnAPQP');
                //  },
                //  function (error) {
                //      common.usSpinnerService.stop('spnAPQP');
                //  });
            }
        };
        $scope.ShowPopupDocuments = function (Id) {
            common.usSpinnerService.spin('spnAPQP');
            var modalInstance = $modal.open({
                templateUrl: '/App_Client/views/APQP/CAPA/DocumentPopup.html?v=' + Version,
                controller: ModalDocumentCtrl,
                keyboard: false,
                backdrop: false,
                scope: $scope,
                sizeclass: 'modal-fitToScreen',
                resolve: {
                    CAPAPartAffectedDetailId: function () {
                        return Id;
                    }
                }
            });
            modalInstance.result.then(function () {
            }, function () {
                console.log('error no docs');
            });
        };
        //End Section - 1 - CAPA - Part Name(s) and Part#(s) Affected:

        //Start Section - 2 - CAPA -  Detailed description of problem (date shipped, part numbers)
        $scope.AddProblemDescription = function () {
            $scope.ProblemDescriptionValidation($scope.objProblemDescription);
            if ($scope.isInvalid)
                return false;
            $scope.CAPA.lstcapaProblemDescriptions.push($scope.objProblemDescription);
            $scope.ResetProblemDescription();
            angular.forEach($scope.CAPA.lstcapaProblemDescriptions, function (obj, i) {
                $scope.FilteredQueryList = $filter('filter')($scope.FilteredQueryList, function (row) { return row.Id != obj.QueryId });
            });
            //common.aaNotify.success(($filter('translate')('_Query_')) + ' is added successfully.');
        };
        $scope.ProblemDescriptionValidation = function (item) {
            $scope.isInvalid = false;
            if (Isundefinedornull(item) || IsUndefinedNullOrEmpty(item.QueryId)) {
                common.aaNotify.error(($filter('translate')('_Query_')) + ' is required.');
                $scope.isInvalid = true;
                return false;
            }
            angular.forEach($scope.CAPA.lstcapaProblemDescriptions, function (obj) {
                if (!$scope.isInvalid && obj.QueryId == item.QueryId) {
                    common.aaNotify.error(($filter('translate')('_Query_')) + ' already exists.');
                    $scope.isInvalid = true;
                    return false;
                }
            });
            if (!$scope.isInvalid)
                return true;
        };
        $scope.DeleteProblemDescription = function (index) {
            if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                angular.forEach($scope.CAPA.lstcapaProblemDescriptions, function (obj, i) {
                    if (index == i) {
                        $scope.FilteredQueryList.push($filter('filter')($scope.QueryList, function (row) { return row.Id == obj.QueryId })[0]);
                    }
                });
                $scope.CAPA.lstcapaProblemDescriptions.splice(index, 1);
                $scope.ResetProblemDescription();
                common.aaNotify.success(($filter('translate')('_CAPADeleteSuccessMessage_')));
            }
        };
        $scope.ResetProblemDescription = function () {
            $scope.objProblemDescription = {};
        };
        //End Section - 2 - CAPA -  Detailed description of problem (date shipped, part numbers)

        //Start Section - 3 - CAPA - Temporary Countermeasure
        $scope.AddTempCountermeasures = function () {
            $scope.TempCountermeasuresValidation($scope.objTempCountermeasures);
            if ($scope.isInvalid)
                return false;
            $scope.CAPA.lstcapaTempCountermeasures.push($scope.objTempCountermeasures);
            $scope.ResetTempCountermeasures();
            //common.aaNotify.success(($filter('translate')('_TemporaryCountermeasure_')) + ' is added successfully.');
        };
        $scope.TempCountermeasuresValidation = function (item) {
            $scope.isInvalid = false;
            if (!$scope.isInvalid)
                return true;
        };
        $scope.DeleteTempCountermeasures = function (index) {
            if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                $scope.CAPA.lstcapaTempCountermeasures.splice(index, 1);
                $scope.ResetTempCountermeasures();
                common.aaNotify.success(($filter('translate')('_CAPADeleteSuccessMessage_')));
            }
        };
        $scope.ResetTempCountermeasures = function () {
            $scope.objTempCountermeasures = {};
        };
        //End Section - 3 - CAPA - Temporary Countermeasure

        //Start Section - 4 - CAPA - Verification
        $scope.AddCAPAVerifications = function () {
            $scope.CAPAVerificationsValidation($scope.objCAPAVerifications);
            if ($scope.isInvalid)
                return false;
            $scope.CAPA.lstcapaVerifications.push($scope.objCAPAVerifications);
            $scope.ResetCAPAVerifications();
            //common.aaNotify.success(($filter('translate')('_Verification_')) + ' is added successfully.');
        };
        $scope.CAPAVerificationsValidation = function (item) {
            $scope.isInvalid = false;
            if (!$scope.isInvalid)
                return true;
        };
        $scope.DeleteCAPAVerifications = function (index) {
            if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                $scope.CAPA.lstcapaVerifications.splice(index, 1);
                $scope.ResetCAPAVerifications();
                common.aaNotify.success(($filter('translate')('_CAPADeleteSuccessMessage_')));
            }
        };
        $scope.ResetCAPAVerifications = function () {
            $scope.objCAPAVerifications = {};
        };
        //End Section - 4 - CAPA - Verification

        //Start Section - 5 - CAPA - Root Cause- Why Made?
        $scope.AddRootCauseWhyMade = function () {
            $scope.RootCauseWhyMadeValidation($scope.objRootCauseWhyMade);
            if ($scope.isInvalid)
                return false;
            $scope.CAPA.lstcapaRootCauseWhyMade.push($scope.objRootCauseWhyMade);
            $scope.ResetRootCauseWhyMade();
            //common.aaNotify.success(($filter('translate')('_Query_')) + ' is added successfully.');
        };
        $scope.RootCauseWhyMadeValidation = function (item) {
            $scope.isInvalid = false;
            if (!$scope.isInvalid)
                return true;
        };
        $scope.DeleteRootCauseWhyMade = function (index) {
            if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                $scope.CAPA.lstcapaRootCauseWhyMade.splice(index, 1);
                $scope.ResetRootCauseWhyMade();
                common.aaNotify.success(($filter('translate')('_CAPADeleteSuccessMessage_')));
            }
        };
        $scope.ResetRootCauseWhyMade = function () {
            $scope.objRootCauseWhyMade = { QueryId: 5 };
        };
        //End Section - 5 - CAPA - Root Cause- Why Made?

        //Start Section - 6 - CAPA -  Root Cause- Why shipped?
        $scope.AddRootCauseWhyShipped = function () {
            $scope.RootCauseWhyShippedValidation($scope.objRootCauseWhyShipped);
            if ($scope.isInvalid)
                return false;
            $scope.CAPA.lstcapaRootCauseWhyShipped.push($scope.objRootCauseWhyShipped);
            $scope.ResetRootCauseWhyShipped();
            //common.aaNotify.success(($filter('translate')('_Query_')) + ' is added successfully.');
        };
        $scope.RootCauseWhyShippedValidation = function (item) {
            $scope.isInvalid = false;
            if (!$scope.isInvalid)
                return true;
        };
        $scope.DeleteRootCauseWhyShipped = function (index) {
            if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                $scope.CAPA.lstcapaRootCauseWhyShipped.splice(index, 1);
                $scope.ResetRootCauseWhyShipped();
                common.aaNotify.success(($filter('translate')('_CAPADeleteSuccessMessage_')));
            }
        };
        $scope.ResetRootCauseWhyShipped = function () {
            $scope.objRootCauseWhyShipped = { QueryId: 5 };
        };
        //End Section - 6 - CAPA -  Root Cause- Why shipped?

        //Start -Section - 7 - CAPA -  Permanent Countereasure
        $scope.AddPermanentCountermeasures = function () {
            $scope.PermanentCountermeasuresValidation($scope.objPermanentCountermeasures);
            if ($scope.isInvalid)
                return false;
            $scope.CAPA.lstcapaPermanentCountermeasures.push($scope.objPermanentCountermeasures);
            $scope.ResetPermanentCountermeasures();
            //common.aaNotify.success(($filter('translate')('_PermanentCountermeasure_')) + ' is added successfully.');
        };
        $scope.PermanentCountermeasuresValidation = function (item) {
            $scope.isInvalid = false;
            if (!$scope.isInvalid)
                return true;
        };
        $scope.DeletePermanentCountermeasures = function (index) {
            if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                $scope.CAPA.lstcapaPermanentCountermeasures.splice(index, 1);
                $scope.ResetPermanentCountermeasures();
                common.aaNotify.success(($filter('translate')('_CAPADeleteSuccessMessage_')));
            }
        };
        $scope.ResetPermanentCountermeasures = function () {
            $scope.objPermanentCountermeasures = {};
        };
        //End -Section - 7 - CAPA -  Permanent Countereasure

        //Start Section - 8 - CAPA -  Feed Forward 
        $scope.AddFeedForwards = function () {
            $scope.FeedForwardsValidation($scope.objFeedForwards);
            if ($scope.isInvalid)
                return false;
            $scope.CAPA.lstcapaFeedForwards.push($scope.objFeedForwards);
            $scope.ResetFeedForwards();
            //common.aaNotify.success(($filter('translate')('_Feedforward_')) + ' is added successfully.');
        };
        $scope.FeedForwardsValidation = function (item) {
            $scope.isInvalid = false;
            if (!$scope.isInvalid)
                return true;
        };
        $scope.DeleteFeedForwards = function (index) {
            if (confirm($filter('translate')('_DeleteConfirmText_'))) {
                $scope.CAPA.lstcapaFeedForwards.splice(index, 1);
                $scope.ResetFeedForwards();
                common.aaNotify.success(($filter('translate')('_CAPADeleteSuccessMessage_')));
            }
        };
        $scope.ResetFeedForwards = function () {
            $scope.objFeedForwards = {};
        };
        //End Section - 8 - CAPA -  Feed Forward 

        //popup for show audit log 
        $scope.ShowChangeLogPopup = function () {
            var modalTemplatePreviewOptions = $modal.open({
                templateUrl: '/App_Client/views/APQP/CAPA/AuditLogsPopup.html?v=' + Version,
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

        //Start CRUD
        $scope.getSAPPartsList = function (mode) {
            if (!IsUndefinedNullOrEmpty($scope.CAPA.SupplierCode) && !IsUndefinedNullOrEmpty($scope.CAPA.CustomerCode)) {
                if (mode == 'Add')
                    common.usSpinnerService.spin('spnAPQP');
                var supplierName = $filter('filter')($scope.SAPSuppliersList, function (rw) { return rw.Id == $scope.CAPA.SupplierCode })[0].Name;
                var customerName = $filter('filter')($scope.SAPCustomersList, function (rw) { return rw.Id == $scope.CAPA.CustomerCode })[0].Name;
                CAPASvc.getSAPPartsList(supplierName, customerName).then(
                   function (response) {
                       if (ShowMessage(common, response.data)) {
                           $scope.SAPPartsList = response.data.Result;
                           $timeout(function () {
                               if (mode == 'Add')
                                   common.usSpinnerService.stop('spnAPQP');
                           }, 0);
                       }
                       else {
                           if (mode == 'Add')
                               common.usSpinnerService.stop('spnAPQP');
                       }
                       if (mode == 'Add')
                           $scope.CAPA.lstcapaPartAffectedDetails = [];
                   },
                function (error) {
                    common.usSpinnerService.stop('spnAPQP');
                });
            }
            else {
                $scope.SAPPartsList = [];
                $scope.CAPA.lstcapaPartAffectedDetails = [];
            }
        };
        $scope.GenerateCAPAForm = function () {

            CAPASvc.GenerateCAPAForm($scope.CAPA).then(
               function (response) {
                   //common.usSpinnerService.stop('spnAPQP');
                   if (response.data.StatusCode == 200) {
                       window.open(response.data.SuccessMessage, '_blank');
                   }
                   else {
                       common.aaNotify.error(response.data.ErrorText);
                   }
               },
               function (error) {
                   //common.usSpinnerService.stop('spnAPQP');
               });
        };
        $scope.getData = function (capaId) {
            common.usSpinnerService.spin('spnAPQP');
            CAPASvc.getData(capaId).then(
               function (response) {
                   if (ShowMessage(common, response.data)) {
                       $scope.CAPA = response.data.Result;
                       try {
                           $scope.CAPA.CorrectiveActionInitiatedDate = convertLocalDateToUTCDate($scope.CAPA.CorrectiveActionInitiatedDate);
                           $scope.SetLooksupForSupplierContact($scope.CAPA.SupplierCode, 'Edit');
                           $scope.Header = 'CAPA # [' + $scope.CAPA.Id + ']';

                           angular.forEach($scope.CAPA.lstcapaTempCountermeasures, function (item) {
                               item.EffectiveDate = convertLocalDateToUTCDate(item.EffectiveDate);
                           });
                           angular.forEach($scope.CAPA.lstcapaVerifications, function (item) {
                               item.VerificationDate = convertLocalDateToUTCDate(item.VerificationDate);
                           });
                           angular.forEach($scope.CAPA.lstcapaPermanentCountermeasures, function (item) {
                               item.EffectiveDate = convertLocalDateToUTCDate(item.EffectiveDate);
                           });
                           angular.forEach($scope.CAPA.lstcapaApprovals, function (item) {
                               item.ApprovalDate = convertLocalDateToUTCDate(item.ApprovalDate);
                           });
                       } catch (e) {
                           console.log(e);
                       }
                       $timeout(function () {
                           angular.forEach($scope.CAPA.lstcapaProblemDescriptions, function (obj, i) {
                               $scope.FilteredQueryList = $filter('filter')($scope.FilteredQueryList, function (row) { return row.Id != obj.QueryId });
                           });
                           if (Isundefinedornull($scope.CAPA.lstcapaApprovals) || $scope.CAPA.lstcapaApprovals.length <= 0) {
                               $scope.CAPA.lstcapaApprovals = [];
                               angular.forEach($scope.CAPAApproverTitlesList, function (item) {
                                   $scope.temObjApproval = { Title: item.Name, TitleId: item.Id, Name: '', ApprovalDate: null };
                                   $scope.CAPA.lstcapaApprovals.push($scope.temObjApproval);
                               });
                           }
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
        $scope.SaveCAPA = function (Mode, closeForm, AddToDT, fromGenCAPA) {
            common.usSpinnerService.spin('spnAPQP');
            if (IsUndefinedNullOrEmpty($scope.CAPA.SupplierContactName) || $scope.CAPA.SupplierContactName == "#") {
                common.usSpinnerService.stop('spnAPQP');
                common.aaNotify.error("Supplier contact is required.");
                return false;
            }
            $scope.Mode = Mode;
            $scope.CAPA.PartNumber = "";
            if (($scope.TransactionMode == "Create") || ($scope.TransactionMode == "Edit" && $scope.Mode == "DropdownButton")) {
                var PartNumberIds = [];
                var PartCodes = [];
                angular.forEach($scope.CAPA.PartNumberItems, function (item, index) {
                    if (!IsUndefinedNullOrEmpty(item.Id) && parseInt(item.Id) > 0) {
                        PartNumberIds.push(item.Id);
                        PartCodes.push(item.Code);
                    }
                });

                if (PartNumberIds.length > 0) {
                    $scope.CAPA.PartNumber = PartNumberIds.join(",");
                    $scope.CAPA.PartCodes = PartCodes.join(",");
                } else {
                    common.usSpinnerService.stop('spnAPQP');
                    common.aaNotify.error("Please select at least one part.");
                    return false;
                }
            }
            else if ($scope.getCountForAffectedPartDetail() <= 0) {
                common.usSpinnerService.stop('spnAPQP');
                common.aaNotify.error("Please add at least one part.");
                return false;
            }
            else {
                // Add those rows also that are not added in list but has data.
                if (!IsUndefinedNullOrEmpty($scope.objProblemDescription.QueryId))
                    $scope.CAPA.lstcapaProblemDescriptions.push($scope.objProblemDescription);
                if (!IsUndefinedNullOrEmpty($scope.objTempCountermeasures.Description) || !IsUndefinedNullOrEmpty($scope.objTempCountermeasures.EffectiveDate))
                    $scope.CAPA.lstcapaTempCountermeasures.push($scope.objTempCountermeasures);
                if (!IsUndefinedNullOrEmpty($scope.objCAPAVerifications.Description) || !IsUndefinedNullOrEmpty($scope.objCAPAVerifications.VerificationDate))
                    $scope.CAPA.lstcapaVerifications.push($scope.objCAPAVerifications);
                if (!IsUndefinedNullOrEmpty($scope.objRootCauseWhyMade.Reason))
                    $scope.CAPA.lstcapaRootCauseWhyMade.push($scope.objRootCauseWhyMade);
                if (!IsUndefinedNullOrEmpty($scope.objRootCauseWhyShipped.Reason))
                    $scope.CAPA.lstcapaRootCauseWhyShipped.push($scope.objRootCauseWhyShipped);
                if (!IsUndefinedNullOrEmpty($scope.objPermanentCountermeasures.Description) || !IsUndefinedNullOrEmpty($scope.objPermanentCountermeasures.EffectiveDate))
                    $scope.CAPA.lstcapaPermanentCountermeasures.push($scope.objPermanentCountermeasures);
                if (!IsUndefinedNullOrEmpty($scope.objFeedForwards.Description))
                    $scope.CAPA.lstcapaFeedForwards.push($scope.objFeedForwards);
            }

            try {
                $scope.CAPA.Mode = $scope.Mode;
                $scope.CAPA.AddToDT = AddToDT;
                var objTempDate = convertUTCDateToLocalDate($scope.CAPA.CorrectiveActionInitiatedDate);
                $scope.CAPA.CorrectiveActionInitiatedDate = !Isundefinedornull(objTempDate) ? new Date(objTempDate.setHours(0, 0, 0, 0)) : objTempDate;
                angular.forEach($scope.CAPA.lstcapaTempCountermeasures, function (item) {
                    item.EffectiveDate = convertUTCDateToLocalDate(item.EffectiveDate);
                });
                angular.forEach($scope.CAPA.lstcapaVerifications, function (item) {
                    item.VerificationDate = convertUTCDateToLocalDate(item.VerificationDate);
                });
                angular.forEach($scope.CAPA.lstcapaPermanentCountermeasures, function (item) {
                    item.EffectiveDate = convertUTCDateToLocalDate(item.EffectiveDate);
                });
                angular.forEach($scope.CAPA.lstcapaApprovals, function (item) {
                    item.ApprovalDate = convertUTCDateToLocalDate(item.ApprovalDate);
                });
            } catch (e) {
                common.usSpinnerService.stop('spnAPQP');
                common.aaNotify.error(e);
                return false;
            }

            var SupplierCodeWithName = $filter('filter')($scope.SAPSuppliersList, function (rw) { return rw.Id == $scope.CAPA.SupplierCode })[0].Name;
            var CustomerCodeWithName = $filter('filter')($scope.SAPCustomersList, function (rw) { return rw.Id == $scope.CAPA.CustomerCode })[0].Name;
            $scope.CAPA.SupplierCodeWithName = SupplierCodeWithName;
            $scope.CAPA.CustomerCodeWithName = CustomerCodeWithName;

            CAPASvc.CheckPartAssociationWithSupplier($scope.CAPA).then(
               function (response) {
                   if (response.data.StatusCode == 200) {

                       if (response.data.Result == '') {
                           CAPASvc.Save($scope.CAPA).then(
                              function (response) {
                                  common.usSpinnerService.stop('spnAPQP');

                                  if (ShowMessage(common, response.data)) {
                                      $scope.Id = response.data.Result; // Id of latest created record                       

                                      if (AddToDT == 0) {
                                          $scope.CAPA.Id = response.data.Result;
                                          if (closeForm) {
                                              $scope.RedirectToList();
                                          }
                                          else {
                                              if ($scope.TransactionMode == 'Create')
                                                  $scope.reloadWithId();
                                              else {
                                                  if (fromGenCAPA == 1) {
                                                      common.usSpinnerService.spin('spnAPQP');
                                                      CAPASvc.GenerateCAPAForm($scope.CAPA).then(
                                                       function (response) {
                                                           common.aaNotify.success('Generating CAPA Form ...');

                                                           if (response.data.StatusCode == 200) {
                                                               //window.open(response.data.SuccessMessage, '_blank');
                                                               var hiddenElement = document.createElement('a');
                                                               hiddenElement.target = '_blank';
                                                               hiddenElement.href = response.data.SuccessMessage;
                                                               document.body.appendChild(hiddenElement);
                                                               hiddenElement.click();
                                                               common.usSpinnerService.stop('spnAPQP');

                                                           }
                                                           else {
                                                               common.aaNotify.error(response.data.ErrorText);
                                                           }

                                                       },
                                                        function (error) {
                                                            common.usSpinnerService.stop('spnAPQP');
                                                        });
                                                  }
                                                  $scope.ResetForm();
                                              }
                                          }
                                      }
                                      else
                                          $window.location.href = "/APQP/DefectTracking#/AddEdit/" + $scope.Id;
                                  }

                              },
                              function (error) {
                                  common.usSpinnerService.stop('spnAPQP');
                                  console.log(error);
                              });
                           common.usSpinnerService.stop('spnAPQP');
                       }
                       else {
                           if (!IsUndefinedNullOrEmpty(response.data.Result))
                               confirm(response.data.Result);
                           //common.aaNotify.error(response.data.Result);

                           common.usSpinnerService.stop('spnAPQP');
                       }
                   }
                   else {
                       common.aaNotify.error(response.data.ErrorText);
                   }

               },
               function (error) {
                   common.usSpinnerService.stop('spnAPQP');
               });

        };
        $scope.getCountForAffectedPartDetail = function () {
            var objPartCount = $filter('filter')($scope.CAPA.lstcapaPartAffectedDetails, function (rw) { return rw.IsDeletedFromObject == false });
            return objPartCount.length;
        };
        $scope.reloadWithId = function () {
            common.$location.path('/AddEdit/' + $scope.CAPA.Id);
        };
        $scope.RedirectToList = function () {
            common.$location.path("/");
        };
        $scope.ResetForm = function () {
            common.$route.reload();
        };
        $scope.AddToDefectTracking = function () {
            $scope.SaveCAPA('FinalSaveButton', 0, 1, 0);
        };
        //End CRUD

        //popup for send email       
        $scope.ShowSendEmailPopup = function () {
            var modalTemplatePreviewOptions = $modal.open({
                templateUrl: '/App_Client/views/APQP/CAPA/SendEmail.html?v=' + Version,
                controller: ModalSendEmailCtrl,
                keyboard: true,
                backdrop: true,
                scope: $scope,
                sizeclass: 'modal-md'
            });
            modalTemplatePreviewOptions.result.then(function () {
            }, function () {
            });
        };
        // send email end here
        $scope.Init();

    }]);

var ModalDocumentCtrl = function ($scope, $modalInstance, common, $timeout, $filter, $confirm, LookupSvc, CAPASvc, CAPAPartAffectedDetailId) {
    $scope.DocumentsInit = function () {
        $scope.CallFrom = "CAPA";
        $scope.FolderName = "DTCAPADocuments";
        $scope.IsEditMode = "No";
        $scope.DocumentTypeId = 0;
        $scope.AssociatedToId = 0;
        $scope.AddEditDocument = false;
        $scope.Document = {};
        $scope.DocumentOriginal = {};
        $scope.DocumentsList = {};
        $scope.DocumentsListWithGroup = {};
        $scope.GetPartDocumentList(CAPAPartAffectedDetailId);
    };
    $scope.GetPartDocumentList = function (id) {
        common.usSpinnerService.spin('spnAPQP');
        CAPASvc.GetPartDocumentList(id, $scope.CallFrom).then(
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
            "Name": "CAPADocumentType", "Parameters": { "APQPItemId": CAPAPartAffectedDetailId, "DocumentTypeId": $scope.DocumentTypeId, "IsEditMode": $scope.IsEditMode, "AssociatedToId": $scope.AssociatedToId }
        }
        ];
        $scope.getDocumentLookupData();
    };
    $scope.getDocumentLookupData = function () {
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            angular.forEach(data.data, function (o) {
                if (o.Name === "CAPADocumentType") {
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
            CAPASvc.DeleteDocument(documentId).then(
              function (response) {
                  if (ShowMessage(common, response.data)) {
                      $scope.ResetDocument();
                      $scope.GetPartDocumentList(CAPAPartAffectedDetailId);
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
            $scope.Document.PartAffectedDetailsId = CAPAPartAffectedDetailId;
            common.usSpinnerService.spin('spnAPQP');
            CAPASvc.SaveDocument($scope.Document).then(
              function (response) {
                  if (ShowMessage(common, response.data)) {
                      common.usSpinnerService.stop('spnAPQP');
                      $scope.ResetDocument();
                      $scope.GetPartDocumentList(CAPAPartAffectedDetailId);
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
        if (!IsUndefinedNullOrEmpty($scope.CAPA.Id) && parseInt($scope.CAPA.Id) > 0) {
            //capa
            $scope.CAPAPaging = GetDefaultPageObject();
            $scope.CAPACriteria = { ItemId: $scope.CAPA.Id };
            $scope.CAPAPaging.Criteria = $scope.CAPACriteria;
            $scope.CAPALogsList = {};

            //capa details
            $scope.CAPADetailPaging = GetDefaultPageObject();
            $scope.CAPADetailCriteria = { ItemId: $scope.CAPA.Id };
            $scope.CAPADetailPaging.Criteria = $scope.CAPADetailCriteria;
            $scope.CAPADetailLogsList = {};

            $scope.ViewAuditLogs(true);
        }
        else
            $scope.ChangeLogCancel();
    }
    $scope.ViewAuditLogs = function (loadAll) {
        common.usSpinnerService.spin('spnAPQP');
        AuditLogsSvc.GetAuditLogCAPA($scope.CAPAPaging).then(
             function (response) {
                 common.usSpinnerService.stop('spnAPQP');
                 if (response.data.StatusCode == 200) {
                     $scope.CAPALogsList = response.data.Result;
                     $scope.CAPAPaging = response.data.PageInfo;
                     $scope.CAPALogsList = _.groupBy($scope.CAPALogsList, 'UpdatedOn');
                     $scope.IsAccordionObjectEmptyDT = IsObjectEmpty($scope.CAPALogsList);
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
        AuditLogsSvc.GetAuditLogCAPAAffectedPartDetails($scope.CAPADetailPaging).then(
             function (response) {
                 common.usSpinnerService.stop('spnAPQP');
                 if (response.data.StatusCode == 200) {
                     $scope.CAPADetailLogsList = response.data.Result;
                     $scope.CAPADetailPaging = response.data.PageInfo;
                     $scope.CAPADetailLogsList = _.groupBy($scope.CAPADetailLogsList, 'UpdatedOn');
                     $scope.IsAccordionObjectEmptyDTD = IsObjectEmpty($scope.CAPADetailLogsList);
                     angular.forEach($scope.CAPADetailLogsList, function (item, index) {
                         $scope.CAPADetailLogsList[index] = _.groupBy(item, 'PartNumber');
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
        $scope.CAPAPaging.PageSize = PageSize;
        $scope.ViewAuditLogs(false);
    };
    $scope.DTPageChanged = function (PageNo) {
        $scope.CAPAPaging.PageNo = PageNo;
        $scope.ViewAuditLogs(false);
    };

    //defect tracking details
    $scope.DTDPageSizeChanged = function (PageSize) {
        $scope.CAPADetailPaging.PageSize = PageSize;
        $scope.ViewAuditLogsDTD();
    };
    $scope.DTDPageChanged = function (PageNo) {
        $scope.CAPADetailPaging.PageNo = PageNo;
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

var ModalSendEmailCtrl = function ($scope, $modalInstance, common, CAPASvc) {
    $scope.SendEmailInit = function () {
        $scope.EmailData = { ToEmailIds: '', CCEmailIds: '', EmailSubject: '', EmailBody: '', AttachDocument: false, objCAPA: angular.copy($scope.CAPA), lstEmailDocumentAttachment: [] };
    };

    $scope.SendEmail = function () {
        common.usSpinnerService.spin('spnAPQP');
        CAPASvc.SendEmail($scope.EmailData).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   common.usSpinnerService.stop('spnAPQP');
                   $scope.Cancel();
               }
               else {
                   common.usSpinnerService.stop('spnAPQP');
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnAPQP');
               //common.aaNotify.error(error);
           });
    };
    $scope.SetUploadforSendEmail = function (file, name) {
        $scope.TempFile = { 'FilePath': file, 'FileName': name };
        $scope.EmailData.lstEmailDocumentAttachment.push($scope.TempFile);
    }
    $scope.deleteFileforSendEmail = function (filePath, index) {
        $scope.EmailData.lstEmailDocumentAttachment.splice(index, 1);
    }
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    $scope.SendEmailInit();
}