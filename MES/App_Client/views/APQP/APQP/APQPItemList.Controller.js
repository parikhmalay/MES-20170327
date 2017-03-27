app.controller('APQPItemListCtrl', ['$rootScope', '$scope', 'common', 'APQPSvc', '$filter', '$modal', 'LookupSvc', 'ContactsSvc', '$routeParams', '$timeout', '$window', '$element',
function ($rootScope, $scope, common, APQPSvc, $filter, $modal, LookupSvc, ContactsSvc, $routeParams, $timeout, $window, $element) {
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
                    case 105:
                        switch (obj.PrivilegeId) {
                            case 1:                           //none
                                RedirectToAccessDenied();
                                break;
                        }
                        break;
                    case 107:
                        $scope.SecurityRoleCaseKickOff(obj);
                        break;
                    case 108:
                        $scope.SecurityRoleCaseToolingLaunch(obj);
                        break;
                    case 110:
                        $scope.SecurityRoleCaseProjectTracking(obj);
                        break;
                    case 111:
                        $scope.SecurityRoleCasePPAPSubmission(obj);
                        break;
                    default:
                        break;
                }
            });
            $scope.checkAllTabsNoneCase();
        }
        else {
            RedirectToAccessDenied();
        }
    }
    $scope.SecurityRoleCaseKickOff = function (obj) {
        $scope.IsHideKickOff = false;
        switch (obj.PrivilegeId) {
            case 1:                           //none
                $scope.IsHideKickOff = true;
                break;
            case 2:                          //read only
                $scope.IsReadOnlyKickOff = true;
                break;
        }
    };
    $scope.SecurityRoleCaseToolingLaunch = function (obj) {
        $scope.IsHideToolingLaunch = false;
        switch (obj.PrivilegeId) {
            case 1:                           //none
                $scope.IsHideToolingLaunch = true;
                break;
            case 2:                          //read only
                $scope.IsReadOnlyToolingLaunch = true;
                break;
        }
    };
    $scope.SecurityRoleCaseProjectTracking = function (obj) {
        $scope.IsHideProjectTracking = false;
        switch (obj.PrivilegeId) {
            case 1:                           //none
                $scope.IsHideProjectTracking = true;
                break;
            case 2:                          //read only
                $scope.IsReadOnlyProjectTracking = true;
                break;
        }
    };
    $scope.SecurityRoleCasePPAPSubmission = function (obj) {
        $scope.IsHidePPAPSubmission = false;
        switch (obj.PrivilegeId) {
            case 1:                           //none
                $scope.IsHidePPAPSubmission = true;
                break;
            case 2:                          //read only
                $scope.IsReadOnlyPPAPSubmission = true;
                break;
        }
    };
    $scope.checkAllTabsNoneCase = function () {
        if ($scope.IsHideKickOff && $scope.IsHideToolingLaunch && $scope.IsHideProjectTracking && $scope.IsHidePPAPSubmission) {
            RedirectToAccessDenied();
        }
    };
    //End implement security role wise
    $scope.setRoleWisePrivilege();

    $rootScope.PageHeader = ($filter('translate')('_APQPItemList_'));
    $scope.allowDeleteRecord = false;
    $scope.allowExportToSAP = false;
    $scope.allowSendDataToSAP = false;
    $scope.hasPricingFieldsAccess = false;
    $scope.Init = function () {
        $scope.SearchCriteria = {};
        $scope.Paging = GetDefaultPageObject();
        //$scope.Paging.PageSize = 10;    //overriding page size preference because browser got hanged while processing more than 10 records so setting default page size 10.
        $scope.Paging.Criteria = $scope.SearchCriteria;
        $scope.TabObject = {};
        $scope.TabObject.CurrentTab = 'KickOff';  //default tab
        //set current tab(first step) according to roles
        $scope.SetFirstStep();
        $scope.SetLastStep();
        $scope.CheckIsReadOnlySaveButton($scope.TabObject.CurrentTab);
        $scope.BackOrNext = '';
        $scope.APQPList = {};
        $scope.selectedClassId = 0;
        $scope.SetLooksupData();
        $scope.SetLooksupDataList();
        //call direct search list items 
        $timeout(function () {
            if (IsUndefinedNullOrEmpty($routeParams.CallFrom)) {
                $scope.SearchAPQPItemList();
            }
            else if ($routeParams.CallFrom == "CallFromQuote" || $routeParams.CallFrom == "CallFromDashboard") {
                $scope.SearchAPQPItemList();
            }
            else if ($routeParams.CallFrom == "APQPDashboard") {
                if (localStorage.getItem("APQPListPageSearchCriteria")) {
                    $scope.$apply(function () {
                        $scope.SearchCriteria = {};
                        //$scope.SearchCriteria.APQPStatusItems = {};
                        $scope.SearchCriteria = JSON.parse(localStorage.getItem("APQPListPageSearchCriteria"));
                    });
                }
                $scope.SearchAPQPItemList();
            }
            else if ($routeParams.CallFrom == "FormView") {
                if (localStorage.getItem("APQPStep")) {
                    $scope.TabObject.CurrentTab = GetAPQPStepName(localStorage.getItem("APQPStep"));
                    $scope.checkAccessIfReturnsFromFormView($scope.TabObject.CurrentTab);
                    $scope.CheckIsReadOnlySaveButton($scope.TabObject.CurrentTab);
                }
                if (localStorage.getItem("APQPItemId")) {
                    $scope.selectedClassId = parseInt(localStorage.getItem("APQPItemId"), 10);
                }
                if (localStorage.getItem("APQPListPaging") && localStorage.getItem("APQPListPageSearchCriteria")) {
                    $scope.$apply(function () {
                        $scope.Paging = {};
                        $scope.SearchCriteria = {};
                        $scope.Paging = JSON.parse(localStorage.getItem("APQPListPaging"));
                        $scope.SearchCriteria = JSON.parse(localStorage.getItem("APQPListPageSearchCriteria"));
                    });
                }
                $scope.SearchAPQPItemList();
            }
        }, 1000);
    };
    $scope.SetFirstStep = function () {
        $scope.FirstStep = "KickOff";
        if (!$scope.IsHideKickOff) {
            $scope.TabObject.CurrentTab = 'KickOff';
            $scope.FirstStep = "KickOff";
        }
        else if (!$scope.IsHideToolingLaunch) {
            $scope.TabObject.CurrentTab = 'ToolingLaunch';
            $scope.FirstStep = "ToolingLaunch";
        }
        else if (!$scope.IsHideProjectTracking) {
            $scope.TabObject.CurrentTab = 'ProjectTracking';
            $scope.FirstStep = "ProjectTracking";
        }
        else if (!$scope.IsHidePPAPSubmission) {
            $scope.TabObject.CurrentTab = 'PpapSubmission';
            $scope.FirstStep = "PpapSubmission";
        }
    };
    $scope.SetLastStep = function () {
        $scope.LastStep = "KickOff";
        if (!$scope.IsHideKickOff) {
            $scope.LastStep = "KickOff";
        }
        if (!$scope.IsHideToolingLaunch) {
            $scope.LastStep = "ToolingLaunch";
        }
        if (!$scope.IsHideProjectTracking) {
            $scope.LastStep = "ProjectTracking";
        }
        if (!$scope.IsHidePPAPSubmission) {
            $scope.LastStep = "PpapSubmission";
        }
    };

    $scope.CopyListObject = function () {
        $scope.CopyAPQPList = angular.copy($scope.APQPList);
    };
    $scope.SetOriginalListObject = function () {
        common.usSpinnerService.spin('spnAPQP');
        $timeout(function () {
            $scope.$apply(function () {
                $scope.APQPList = angular.copy($scope.CopyAPQPList);
                common.usSpinnerService.stop('spnAPQP');
            });
        }, 500);
    };
    $scope.CheckTabClickSave = function (tab) {
        if (IsUndefinedNullOrEmpty($scope.LastClickedTab))
            $scope.LastClickedTab = $scope.TabObject.CurrentTab;
        if ($scope.LastClickedTab != $scope.TabObject.CurrentTab) {
            if (!angular.equals($scope.CopyAPQPList, $scope.APQPList)) {
                if (confirm($filter('translate')('_TabChangeConfirmSaveMessage_'))) {
                    $scope.IsSaveOnTabClick = true;
                    $scope.SaveAPQPListALL();
                }
                else {
                    $scope.SetOriginalListObject();
                    $scope.CheckIsReadOnlySaveButton($scope.TabObject.CurrentTab);
                }
            }
            else
                $scope.CheckIsReadOnlySaveButton($scope.TabObject.CurrentTab);

        }
        else
            $scope.CheckIsReadOnlySaveButton($scope.TabObject.CurrentTab);
    };
    $scope.AssignOldTabOnError = function () {
        $scope.TabObject.CurrentTab = $scope.LastClickedTab;
        $scope.IsSaveOnTabClick = false;
    };
    $scope.CheckIsReadOnlySaveButton = function (tab) {
        $scope.LastClickedTab = $scope.TabObject.CurrentTab; //set default value for tab clicked 
        $scope.IsReadOnlySaveButton = false;
        if (tab == 'KickOff' && $scope.IsReadOnlyKickOff)
            $scope.IsReadOnlySaveButton = true;
        if (tab == 'ToolingLaunch' && $scope.IsReadOnlyToolingLaunch)
            $scope.IsReadOnlySaveButton = true;
        if (tab == 'ProjectTracking' && $scope.IsReadOnlyProjectTracking)
            $scope.IsReadOnlySaveButton = true;
        if (tab == 'PpapSubmission' && $scope.IsReadOnlyPPAPSubmission)
            $scope.IsReadOnlySaveButton = true;
    };
    $scope.checkAccessIfReturnsFromFormView = function (APQPTab) {
        if (APQPTab == "KickOff" && $scope.IsHideKickOff)
            RedirectToAccessDenied();
        else if (APQPTab == "ToolingLaunch" && $scope.IsHideToolingLaunch)
            RedirectToAccessDenied();
        else if (APQPTab == "ProjectTracking" && $scope.IsHideProjectTracking)
            RedirectToAccessDenied();
        else if (APQPTab == "PpapSubmission" && $scope.IsHidePPAPSubmission)
            RedirectToAccessDenied();
    };
    $scope.SetLooksupData = function () {
        $scope.LookUps = [
            {
                "Name": "SAM", "Parameters": { "source": "SAM" }
            },
            {
                "Name": "APQPStatus", "Parameters": { "source": "APQP" }
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
                    //    $scope.SearchCriteria.APQPStatusItems = $filter('filter')($scope.DashAPQPStatusList, function (rw) { return rw.Id != 8 && rw.Id != 15 });
                    //}
                }
            });
        });
    };

    $scope.SetLooksupDataList = function () {
        $scope.LookUps = [
        {
            "Name": "SAM", "Parameters": { "source": "SAM" }
        },
        {
            "Name": "APQPStatus", "Parameters": { "source": "APQP" }
        },
        {
            "Name": "PPAPSubmissionLevel", "Parameters": { "source": "PPAP" }
        },
        {
            "Name": "SupplierWithCode", "Parameters": {}
        },
            //// manufacturer = supplier
        //{
        //    "Name": "Suppliers", "Parameters": {}
        //},
            ///MES Inc. Project Lead / APQP Engineer
        {
            "Name": "APQPEngineers", "Parameters": {}
        },
        {
            "Name": "SCUsers", "Parameters": {}
        },
        {
            "Name": "MESWarehouses", "Parameters": {}
        },
        {
            "Name": "ProjectCategories", "Parameters": {}
        },
        {
            "Name": "ProjectStagesWithoutCategoryId", "Parameters": {}
        },
        {
            "Name": "CustomerContacts", "Parameters": {}
        }
        ];
        $scope.getLookupDataList();
    };
    $scope.getLookupDataList = function () {
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            angular.forEach(data.data, function (o) {
                if (o.Name === "SAM") {
                    $scope.SAMList = o.Data;
                }
                else if (o.Name === "APQPStatus") {
                    $scope.APQPStatusList = o.Data;
                }
                else if (o.Name === "PPAPSubmissionLevel") {
                    $scope.PPAPSubmissionList = o.Data;
                }
                else if (o.Name === "SupplierWithCode") {
                    $scope.SupplierList = o.Data;
                    $scope.ManufacturerList = o.Data;
                }
                    //else if (o.Name === "Suppliers") {
                    //    $scope.ManufacturerList = o.Data;
                    //}
                else if (o.Name === "APQPEngineers") {
                    $scope.MESProjectLeadList = o.Data;
                }
                else if (o.Name === "SCUsers") {
                    $scope.SupplyChainCoordinatorList = o.Data;
                }
                else if (o.Name === "MESWarehouses") {
                    $scope.MESWarehouseList = o.Data;
                }
                else if (o.Name === "ProjectCategories") {
                    $scope.PTCategoryList = o.Data;
                }
                else if (o.Name === "ProjectStagesWithoutCategoryId") {
                    $scope.ProjectStageList = o.Data;
                }
                else if (o.Name === "CustomerContacts") {
                    $scope.CustomerPurchasingContactList = o.Data;
                }
            });
        });
    };
    $scope.setLookupProjectStages = function (aPQPProjectCategoryId, objAPQP) {
        $scope.LookUps = [
        {
            "Name": "ProjectStages", "Parameters": { "ProjectCategoryId": aPQPProjectCategoryId }
        }
        ];
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            angular.forEach(data.data, function (o) {
                if (o.Name === "ProjectStages") {
                    objAPQP.PTStageList = o.Data;
                }
            });
        });
    };
    $scope.setLookupCustomerContacts = function (customerId, objAPQP) {
        $scope.LookUps = [
        {
            "Name": "CustomerContacts", "Parameters": { "customerId": customerId }
        }
        ];
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            angular.forEach(data.data, function (o) {
                if (o.Name === "CustomerContacts") {
                    objAPQP.CustomerPurchasingContactList = o.Data;
                }
            });
        });
    };
    $scope.OnCustomerPurchasingContactSelect = function ($item, objAPQP) {
        if (!IsNotNullorEmpty($item) || !IsNotNullorEmpty($item.Key)) {
            objAPQP.objKickOff.CustomerProjectLeadId = $item.Id;
            objAPQP.objKickOff.CustomerProjectLead = $item.Name;
            if (!IsUndefinedNullOrEmpty(objAPQP.objKickOff.CustomerProjectLeadId) && (objAPQP.objKickOff.CustomerProjectLeadId) > 0) {
                common.usSpinnerService.spin('spnAPQP');
                ContactsSvc.getData(objAPQP.objKickOff.CustomerProjectLeadId).then(
              function (response) {
                  common.usSpinnerService.stop('spnAPQP');
                  if (response.data.StatusCode == 200) {
                      objAPQP.objKickOff.CustomerProjectLeadEmail = response.data.Result.Email;
                      objAPQP.objKickOff.CustomerProjectLeadPhone = response.data.Result.DirectPhone;
                  }
              },
              function (error) {
                  common.usSpinnerService.stop('spnAPQP');
              });
            }
        }
    };
    $scope.setCustomerPurchasingContact = function (objAPQP) {
        if (!Isundefinedornull(objAPQP.CustomerPurchasingContactList) && !IsUndefinedNullOrEmpty(objAPQP.objKickOff.CustomerProjectLead)) {
            var objCustomerPurchasingContact = $filter('filter')(objAPQP.CustomerPurchasingContactList, function (rw) {
                return rw.Name.toLowerCase() == objAPQP.objKickOff.CustomerProjectLead.toLowerCase()
            });
            if (objCustomerPurchasingContact.length <= 0) {
                objAPQP.objKickOff.CustomerProjectLeadId = '';
            }
        }
        else
            objAPQP.objKickOff.CustomerProjectLeadId = '';
    };
    //Start apqp item list page functions
    $scope.SearchAPQPItemList = function () {
        $scope.APQPList = {};
        var APQPStatusIds = [];
        angular.forEach($scope.SearchCriteria.APQPStatusItems, function (item, index) {
            if (!IsUndefinedNullOrEmpty(item.Id))
                APQPStatusIds.push(item.Id);
        });
        $scope.SearchCriteria.APQPStatusIds = APQPStatusIds.join(",");
        common.usSpinnerService.spin('spnAPQP');
        $scope.Paging.Criteria = $scope.SearchCriteria;
        APQPSvc.GetAPQPList($scope.Paging).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.APQPList = response.data.Result;
                   $scope.Paging = response.data.PageInfo;
                   $scope.Paging.Criteria = $scope.SearchCriteria;
                   if ($scope.APQPList.length > 0) {
                       $scope.allowDeleteRecord = $scope.APQPList[0].AllowDeleteRecord;
                       $scope.allowExportToSAP = $scope.APQPList[0].AllowExportToSAP;
                       $scope.allowSendDataToSAP = $scope.APQPList[0].AllowSendDataToSAP;
                       $scope.hasPricingFieldsAccess = $scope.APQPList[0].HasPricingFieldsAccess;
                       $timeout(function () {
                           $scope.$broadcast("loadScrollerPanel");
                       }, 0);
                   }
                   else {
                       common.aaNotify.error("There are no APQP Item(s) found.");
                   }

                   $timeout(function () {
                       if ($scope.APQPList.length > 0) {
                           try {
                               angular.forEach($scope.APQPList, function (APQP, index) {
                                   //textbox values dates
                                   APQP.objKickOff.ProjectKickoffDate = convertLocalDateToUTCDate(APQP.objKickOff.ProjectKickoffDate);
                                   APQP.objKickOff.CustomerToolingPOAuthRcvdDate = convertLocalDateToUTCDate(APQP.objKickOff.CustomerToolingPOAuthRcvdDate);
                                   APQP.objToolingLaunch.RevisionDate = convertLocalDateToUTCDate(APQP.objToolingLaunch.RevisionDate);
                                   APQP.objToolingLaunch.ToolingKickoffDate = convertLocalDateToUTCDate(APQP.objToolingLaunch.ToolingKickoffDate);
                                   APQP.objProjectTracking.CurrentEstimatedToolingCompletionDate = convertLocalDateToUTCDate(APQP.objProjectTracking.CurrentEstimatedToolingCompletionDate);
                                   APQP.objProjectTracking.ActualToolingCompletionDate = convertLocalDateToUTCDate(APQP.objProjectTracking.ActualToolingCompletionDate);
                                   APQP.objProjectTracking.EstimatedSampleShipmentDate = convertLocalDateToUTCDate(APQP.objProjectTracking.EstimatedSampleShipmentDate);
                                   APQP.objProjectTracking.ActualSampleShipmentDate = convertLocalDateToUTCDate(APQP.objProjectTracking.ActualSampleShipmentDate);
                                   APQP.objPPAPSubmission.ActualPSWDate = convertLocalDateToUTCDate(APQP.objPPAPSubmission.ActualPSWDate);
                                   APQP.objPPAPSubmission.PPAPPartsApprovedDate = convertLocalDateToUTCDate(APQP.objPPAPSubmission.PPAPPartsApprovedDate);
                                   //labels values dates
                                   APQP.objKickOff.RevisionDate = convertLocalDateToUTCDate(APQP.objKickOff.RevisionDate);
                                   APQP.objToolingLaunch.PlanToolingCompletionDate = convertLocalDateToUTCDate(APQP.objToolingLaunch.PlanToolingCompletionDate);
                                   APQP.objProjectTracking.ToolingKickoffDate = convertLocalDateToUTCDate(APQP.objProjectTracking.ToolingKickoffDate);
                                   APQP.objProjectTracking.PlanToolingCompletionDate = convertLocalDateToUTCDate(APQP.objProjectTracking.PlanToolingCompletionDate);
                                   APQP.objPPAPSubmission.ActualSampleShipmentDate = convertLocalDateToUTCDate(APQP.objPPAPSubmission.ActualSampleShipmentDate);
                                   APQP.objPPAPSubmission.PSWDate = convertLocalDateToUTCDate(APQP.objPPAPSubmission.PSWDate);
                                   //set PPAPSubmissionLevel value
                                   APQP.objKickOff.PPAPSubmissionLevel = Number(APQP.objKickOff.PPAPSubmissionLevel);
                                   if (IsUndefinedNullOrEmpty(APQP.objPPAPSubmission.PSWFilePath))
                                       APQP.objPPAPSubmission.PSWFilePath = '';
                                   //set selected item bg color
                                   if (parseInt($scope.selectedClassId) == parseInt(APQP.Id)) {
                                       APQP.chkSelect = true;
                                       $scope.SetSelectedClass(APQP);
                                   }

                                   //set decimal formated values
                                   if (!isNaN(parseFloat(APQP.objKickOff.PartWeight)))
                                       APQP.objKickOff.PartWeight = $filter("setDecimal")(APQP.objKickOff.PartWeight, 3);
                                   if (!isNaN(parseFloat(APQP.objKickOff.PurchasePieceCost)))
                                       APQP.objKickOff.PurchasePieceCost = $filter("setDecimal")(APQP.objKickOff.PurchasePieceCost, 3);
                                   if (!isNaN(parseFloat(APQP.objKickOff.PurchaseToolingCost)))
                                       APQP.objKickOff.PurchaseToolingCost = $filter("setDecimal")(APQP.objKickOff.PurchaseToolingCost, 3);
                                   if (!isNaN(parseFloat(APQP.objKickOff.SellingPiecePrice)))
                                       APQP.objKickOff.SellingPiecePrice = $filter("setDecimal")(APQP.objKickOff.SellingPiecePrice, 3);
                                   if (!isNaN(parseFloat(APQP.objKickOff.SellingToolingPrice)))
                                       APQP.objKickOff.SellingToolingPrice = $filter("setDecimal")(APQP.objKickOff.SellingToolingPrice, 3);
                                   if (!isNaN(parseFloat(APQP.objKickOff.SellingPiecePrice)))
                                       APQP.objKickOff.SellingPiecePrice = $filter("setDecimal")(APQP.objKickOff.SellingPiecePrice, 3);

                                   //fill Project stage on the basis categories
                                   if (!IsUndefinedNullOrEmpty(APQP.objProjectTracking.APQPProjectCategoryId) && parseInt(APQP.objProjectTracking.APQPProjectCategoryId) > 0) {
                                       //$scope.setLookupProjectStages(APQP.objProjectTracking.APQPProjectCategoryId, APQP);
                                       APQP.PTStageList = $filter('filter')($scope.ProjectStageList, function (rw) {
                                           return rw.ParentId == parseInt(APQP.objProjectTracking.APQPProjectCategoryId)
                                       });
                                   }
                                   //fill CustomerContacts on the basis customerId
                                   if (!IsUndefinedNullOrEmpty(APQP.objKickOff.CustomerId) && parseInt(APQP.objKickOff.CustomerId) > 0) {
                                       //$scope.setLookupCustomerContacts(APQP.objKickOff.CustomerId, APQP);
                                       APQP.CustomerPurchasingContactList = $filter('filter')($scope.CustomerPurchasingContactList, function (rw) {
                                           return rw.ParentId == parseInt(APQP.objKickOff.CustomerId)
                                       });
                                   }
                               });
                           }
                           catch (e) {
                               common.usSpinnerService.stop('spnAPQP');
                           }
                           advanceSearch.close();
                       }
                       common.usSpinnerService.stop('spnAPQP');
                       $scope.CopyListObject();
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
    $scope.GoToFormView = function (apqpItemId, callFrom, PageNumber) {
        if (!Isundefinedornull(apqpItemId)) {
            common.usSpinnerService.spin('spnAPQP');
            localStorage.setItem("PageNumber", PageNumber);
            localStorage.setItem("APQPListPaging", JSON.stringify($scope.Paging));
            localStorage.setItem("APQPListPageSearchCriteria", JSON.stringify($scope.SearchCriteria));
            common.$location.path("/AddEdit/" + apqpItemId + "/" + callFrom);
        }
    };
    $scope.GoToChangeRequestForm = function (apqpItem) {
        $scope.SaveChangeRequestForm(apqpItem);
    };

    $scope.SetSelectedClass = function (objAPQP) {
        if (objAPQP.chkSelect)
            objAPQP.selectedClassId = objAPQP.Id;
        else
            objAPQP.selectedClassId = 0;
    };
    $scope.SaveAPQPListWithCheckedItem = function () {
        common.usSpinnerService.spin('spnAPQP');
        if ($scope.APQPList.length <= 0) {
            common.usSpinnerService.stop('spnAPQP');
            return false;
        }

        $scope.objToSaveAPQPList = {};
        $scope.objToSaveAPQPList = angular.copy($filter('filter')($scope.APQPList, function (rw) {
            return rw.chkSelect == true
        }));
        if ($scope.objToSaveAPQPList.length > 0) {
            if ($scope.ValidateFields($scope.objToSaveAPQPList, false)) {
                $scope.SaveAPQPItemList($scope.objToSaveAPQPList);
            }
        }
        else {
            common.usSpinnerService.stop('spnAPQP');
            common.aaNotify.error("Please select at least one part.");
            return false;
        }
    };
    $scope.SaveAPQPListALL = function () {
        common.usSpinnerService.spin('spnAPQP');
        if ($scope.APQPList.length <= 0) {
            common.usSpinnerService.stop('spnAPQP');
            return false;
        }
        $scope.objToSaveAPQPList = angular.copy($scope.APQPList);
        if ($scope.ValidateFields($scope.objToSaveAPQPList, true)) {
            $scope.SaveAPQPItemList($scope.objToSaveAPQPList);
        }
    };
    $scope.SaveChangeRequestForm = function (APQPItem) {
        common.usSpinnerService.spin('spnAPQP');
        $scope.isCallFromChangeRequestForm = true;
        $scope.objToSaveAPQPList = {};
        APQPItem.chkSelect = true;
        $scope.objToSaveAPQPList = angular.copy($filter('filter')($scope.APQPList, function (rw) {
            return rw.Id == APQPItem.Id
        }));

        if ($scope.ValidateFields($scope.objToSaveAPQPList, false)) {
            $scope.SaveAPQPItemList($scope.objToSaveAPQPList);
        }
        else
            $scope.isCallFromChangeRequestForm = false;
    };
    $scope.SaveAPQPItemList = function (objAPQPList) {
        //convert UTC date values into local dates
        try {
            angular.forEach(objAPQPList, function (APQP, index) {
                //textbox values dates
                APQP.objKickOff.ProjectKickoffDate = convertUTCDateToLocalDate(APQP.objKickOff.ProjectKickoffDate);
                APQP.objKickOff.CustomerToolingPOAuthRcvdDate = convertUTCDateToLocalDate(APQP.objKickOff.CustomerToolingPOAuthRcvdDate);
                APQP.objToolingLaunch.RevisionDate = convertUTCDateToLocalDate(APQP.objToolingLaunch.RevisionDate);
                APQP.objToolingLaunch.ToolingKickoffDate = convertUTCDateToLocalDate(APQP.objToolingLaunch.ToolingKickoffDate);
                APQP.objProjectTracking.CurrentEstimatedToolingCompletionDate = convertUTCDateToLocalDate(APQP.objProjectTracking.CurrentEstimatedToolingCompletionDate);
                APQP.objProjectTracking.ActualToolingCompletionDate = convertUTCDateToLocalDate(APQP.objProjectTracking.ActualToolingCompletionDate);
                APQP.objProjectTracking.EstimatedSampleShipmentDate = convertUTCDateToLocalDate(APQP.objProjectTracking.EstimatedSampleShipmentDate);
                APQP.objProjectTracking.ActualSampleShipmentDate = convertUTCDateToLocalDate(APQP.objProjectTracking.ActualSampleShipmentDate);
                APQP.objPPAPSubmission.ActualPSWDate = convertUTCDateToLocalDate(APQP.objPPAPSubmission.ActualPSWDate);
                APQP.objPPAPSubmission.PPAPPartsApprovedDate = convertUTCDateToLocalDate(APQP.objPPAPSubmission.PPAPPartsApprovedDate);
            });
        } catch (e) {
            common.usSpinnerService.stop('spnAPQP');
            common.aaNotify.error(e);
            $scope.objAllowGenerateNPIFForm = false;
            if ($scope.IsSaveOnTabClick)
                $scope.AssignOldTabOnError();
            return;
        }
        APQPSvc.SaveAPQPItemList(objAPQPList).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   common.usSpinnerService.stop('spnAPQP');

                   //Start set redirect here for Change Request Form
                   if ($scope.isCallFromChangeRequestForm) {
                       if (!Isundefinedornull(objAPQPList[0].Id)) {
                           $window.location.href = "/APQP/ChangeRequest#/AddEdit/0/" + objAPQPList[0].Id;
                       }
                   }
                   $scope.isCallFromChangeRequestForm = false;
                   //End set redirect here for Change Request Form

                   //Start GenerateNPIFForm
                   if ($scope.objAllowGenerateNPIFForm) {
                       $scope.GenerateAndDownloadNPIF(objAPQPList[0].Id);
                   }
                   $scope.objAllowGenerateNPIFForm = false;
                   //End GenerateNPIFForm

                   if ($scope.BackOrNext == 'Back')
                       $scope.MoveBack();
                   else if ($scope.BackOrNext == 'Next')
                       $scope.MoveNext();
                   $scope.BackOrNext = '';
                   //$scope.ResetForm();                   
                   if ($scope.IsSaveOnTabClick) {
                       $scope.CheckIsReadOnlySaveButton($scope.TabObject.CurrentTab);
                       $scope.IsSaveOnTabClick = false;
                   }
                   $scope.CopyListObject();
               }
               else {
                   common.usSpinnerService.stop('spnAPQP');
                   $scope.objAllowGenerateNPIFForm = false;
                   if ($scope.IsSaveOnTabClick)
                       $scope.AssignOldTabOnError();
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnAPQP');
               console.log(error);
               $scope.objAllowGenerateNPIFForm = false;
               if ($scope.IsSaveOnTabClick)
                   $scope.AssignOldTabOnError();
           });
    };
    $scope.BackSave = function () {
        $scope.BackOrNext = 'Back';
        $scope.SaveAPQPListALL();
    };
    $scope.MoveBack = function () {
        switch ($scope.TabObject.CurrentTab) {
            case 'PpapSubmission':
                if (!$scope.IsHideProjectTracking)
                    $scope.TabObject.CurrentTab = 'ProjectTracking';
                else if (!$scope.IsHideToolingLaunch)
                    $scope.TabObject.CurrentTab = 'ToolingLaunch';
                else if (!$scope.IsHideKickOff)
                    $scope.TabObject.CurrentTab = 'KickOff';
                break;
            case 'ProjectTracking':
                if (!$scope.IsHideToolingLaunch)
                    $scope.TabObject.CurrentTab = 'ToolingLaunch';
                else if (!$scope.IsHideKickOff)
                    $scope.TabObject.CurrentTab = 'KickOff';
                break;
            case 'ToolingLaunch':
                if (!$scope.IsHideKickOff)
                    $scope.TabObject.CurrentTab = 'KickOff';
                break;
            default:
                break;
        }
        $scope.CheckIsReadOnlySaveButton($scope.TabObject.CurrentTab);
    };
    $scope.NextSave = function () {
        $scope.BackOrNext = 'Next';
        $scope.SaveAPQPListALL();
    };
    $scope.MoveNext = function () {
        switch ($scope.TabObject.CurrentTab) {
            case 'KickOff':
                if (!$scope.IsHideToolingLaunch)
                    $scope.TabObject.CurrentTab = 'ToolingLaunch';
                else if (!$scope.IsHideProjectTracking)
                    $scope.TabObject.CurrentTab = 'ProjectTracking';
                else if (!$scope.IsHidePPAPSubmission)
                    $scope.TabObject.CurrentTab = 'PpapSubmission';
                break;
            case 'ToolingLaunch':
                if (!$scope.IsHideProjectTracking)
                    $scope.TabObject.CurrentTab = 'ProjectTracking';
                else if (!$scope.IsHidePPAPSubmission)
                    $scope.TabObject.CurrentTab = 'PpapSubmission';
                break;
            case 'ProjectTracking':
                if (!$scope.IsHidePPAPSubmission)
                    $scope.TabObject.CurrentTab = 'PpapSubmission';
                break;
            default:
                break;
        }
        $scope.CheckIsReadOnlySaveButton($scope.TabObject.CurrentTab);
    };
    $scope.ValidateFields = function (objAPQPList, isSaveAll) {
        $scope.IsError = false;
        $scope.formElements = [];
        angular.forEach($scope.APQPList, function (item, index) {
            if (isSaveAll || item.chkSelect) {
                if (($scope.LastClickedTab == 'KickOff') && (IsUndefinedNullOrEmpty(item.objKickOff.PartNumber) || IsUndefinedNullOrEmpty(item.objKickOff.SAMUserId)
                                                                   || isNaN(parseInt(item.objKickOff.SupplierId)) || parseInt(item.objKickOff.SupplierId) <= 0
                               || IsUndefinedNullOrEmpty(item.objKickOff.MaterialType)
                               || isNaN(parseInt(item.objKickOff.ToolingLeadtimeDays)) || parseInt(item.objKickOff.ToolingLeadtimeDays) <= 0)) {
                    if (IsUndefinedNullOrEmpty(item.objKickOff.PartNumber)) {
                        if (!$('#PartNumber_' + index).hasClass('invalid-class'))
                            $('#PartNumber_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#PartNumber_' + index).removeClass('invalid-class');
                    }
                    if (IsUndefinedNullOrEmpty(item.objKickOff.SAMUserId)) {
                        if (!$('#SAMUserId_' + index).hasClass('invalid-class'))
                            $('#SAMUserId_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#SAMUserId_' + index).removeClass('invalid-class');
                    }
                    if (isNaN(parseInt(item.objKickOff.SupplierId)) || parseInt(item.objKickOff.SupplierId) <= 0) {
                        if (!$('#SupplierId_' + index).hasClass('invalid-class'))
                            $('#SupplierId_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#SupplierId_' + index).removeClass('invalid-class');
                    }
                    if (IsUndefinedNullOrEmpty(item.objKickOff.MaterialType)) {
                        if (!$('#MaterialType_' + index).hasClass('invalid-class'))
                            $('#MaterialType_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#MaterialType_' + index).removeClass('invalid-class');
                    }
                    if (isNaN(parseInt(item.objKickOff.ToolingLeadtimeDays)) || parseInt(item.objKickOff.ToolingLeadtimeDays) <= 0) {
                        if (!$('#ToolingLeadtimeDays_' + index).hasClass('invalid-class'))
                            $('#ToolingLeadtimeDays_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#ToolingLeadtimeDays_' + index).removeClass('invalid-class');
                    }
                }
                else {
                    $('#PartNumber_' + index).removeClass('invalid-class');
                    $('#SAMUserId_' + index).removeClass('invalid-class');
                    $('#SupplierId_' + index).removeClass('invalid-class');
                    $('#MaterialType_' + index).removeClass('invalid-class');
                    $('#ToolingLeadtimeDays_' + index).removeClass('invalid-class');
                }
            }
        });
        angular.forEach(objAPQPList, function (APQP, index) {
            if (!$scope.IsError) {
                if (($scope.LastClickedTab == 'KickOff') && (IsUndefinedNullOrEmpty(APQP.objKickOff.PartNumber) || IsUndefinedNullOrEmpty(APQP.objKickOff.SAMUserId)
                            || isNaN(parseFloat(APQP.objKickOff.SupplierId)) || parseFloat(APQP.objKickOff.SupplierId) <= 0
                            || IsUndefinedNullOrEmpty(APQP.objKickOff.MaterialType)
                            || isNaN(parseFloat(APQP.objKickOff.ToolingLeadtimeDays)) || parseFloat(APQP.objKickOff.ToolingLeadtimeDays) <= 0)) {
                    common.usSpinnerService.stop('spnAPQP');
                    $scope.formElements = $element.find('.invalid-class');
                    if ($scope.formElements.length > 0) {
                        if (isSaveAll) {
                            $scope.formElements[0].focus();
                        }
                        else {
                            $scope.IsFocusedSet = false;
                            angular.forEach($scope.APQPList, function (xItem, xIndex) {
                                if ((xItem.Id == APQP.Id) && !$scope.IsFocusedSet) {
                                    if ($('#PartNumber_' + xIndex).hasClass('invalid-class') && !$scope.IsFocusedSet) {
                                        $('#PartNumber_' + xIndex).focus();
                                        $scope.IsFocusedSet = true;
                                    }
                                    if ($('#SAMUserId_' + xIndex).hasClass('invalid-class') && !$scope.IsFocusedSet) {
                                        $('#SAMUserId_' + xIndex).focus();
                                        $scope.IsFocusedSet = true;
                                    }
                                    if ($('#SupplierId_' + xIndex).hasClass('invalid-class') && !$scope.IsFocusedSet) {
                                        $('#SupplierId_' + xIndex).focus();
                                        $scope.IsFocusedSet = true;
                                    }
                                    if ($('#MaterialType_' + xIndex).hasClass('invalid-class') && !$scope.IsFocusedSet) {
                                        $('#MaterialType_' + xIndex).focus();
                                        $scope.IsFocusedSet = true;
                                    }
                                    if ($('#ToolingLeadtimeDays_' + xIndex).hasClass('invalid-class') && !$scope.IsFocusedSet) {
                                        $('#ToolingLeadtimeDays_' + xIndex).focus();
                                        $scope.IsFocusedSet = true;
                                    }
                                }
                            });
                        }
                    }
                    $scope.IsError = true;
                    $scope.BackOrNext = '';
                    $scope.objAllowGenerateNPIFForm = false;
                    if ($scope.IsSaveOnTabClick)
                        $scope.AssignOldTabOnError();
                    common.aaNotify.error($filter('translate')('_IsRequiredErrMsg_'));
                    return false;
                }
            }
        });
        if (!$scope.IsError) {
            return true;
        }
    };
    $scope.DeleteAPQPItem = function (apqpItemId) {
        common.usSpinnerService.spin('spnAPQP');
        APQPSvc.DeleteAPQPItem(apqpItemId).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   common.usSpinnerService.stop('spnAPQP');
                   $scope.SearchAPQPItemList();
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
    //End apqp item list page functions

    $scope.isActive = function (route) {
        return ($scope.TabObject.CurrentTab == route);
    };
    $scope.ResetSearch = function () {
        if (localStorage.getItem("PageNumber"))
            localStorage.removeItem("PageNumber");
        if (localStorage.getItem("APQPListPaging"))
            localStorage.removeItem("APQPListPaging");
        if (localStorage.getItem("APQPListPageSearchCriteria"))
            localStorage.removeItem("APQPListPageSearchCriteria");
        if (localStorage.getItem("APQPItemId"))
            localStorage.removeItem("APQPItemId");
        if (localStorage.getItem("APQPStep"))
            localStorage.removeItem("APQPStep");

        $scope.Init();
    }
    $scope.BackToDashboard = function () {
        common.$location.path("/APQPDashboard/");
    };
    $scope.ResetForm = function () {
        common.$route.reload();
    };

    $scope.PPAPSubmissionSAPDataExport = function () {
        common.usSpinnerService.spin('spnAPQP');
        var apqpItemIds = [], strapqpItemIds = "";
        angular.forEach($scope.APQPList, function (item, index) {
            if (item.chkSelect)
                apqpItemIds.push(item.Id);
        });
        if (apqpItemIds.length > 0)
            strapqpItemIds = apqpItemIds.join(",");
        else {
            common.usSpinnerService.stop('spnAPQP');
            common.aaNotify.error("Please select at least one part.");
            return false;
        }
        APQPSvc.PPAPSubmissionSAPDataExport(strapqpItemIds).then(
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

    $scope.SendDataToSAP = function () {
        common.usSpinnerService.spin('spnAPQP');
        var apqpItemIds = [], strapqpItemIds = "";
        angular.forEach($scope.APQPList, function (item, index) {
            if (item.chkSelect)
                apqpItemIds.push(item.Id);
        });
        if (apqpItemIds.length > 0) {
            strapqpItemIds = apqpItemIds.join(",");
            APQPSvc.SendDataToSAP(strapqpItemIds).then(
               function (response) {
                   console.log(response);
                   common.usSpinnerService.stop('spnAPQP');
                   if (response.data.StatusCode == 200) {
                       var options = { allowHtml: true, message: response.data.SuccessMessage };
                       common.aaNotify.success(response.data.SuccessMessage, options);
                   }
                   else {
                       common.aaNotify.error(response.data.ErrorText);
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnAPQP');
                   console.log(response.data.ErrorText);
               });
        }
        else {
            common.usSpinnerService.stop('spnAPQP');
            common.aaNotify.error("Please select at least one part.");
            return false;
        }
    };

    $scope.APQPTabWiseDataExport = function () {
        common.usSpinnerService.spin('spnAPQP');
        $scope.SearchCriteria.SectionName = GetAPQPStepNameByTabName($scope.TabObject.CurrentTab);
        APQPSvc.APQPTabWiseDataExport($scope.Paging).then(
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
    $scope.UpdateRevLevelFields = function (fieldName, updateFromSource, val, apqpItemId) {
        $scope.IndividualFields = { FieldName: fieldName, UpdatedFromSource: updateFromSource, RevLevel: val, APQPItemId: apqpItemId };
        common.usSpinnerService.spin('spnAPQP');
        APQPSvc.UpdateIndividualFields($scope.IndividualFields).then(
            function (response) {
                if (ShowMessage(common, response.data)) {
                    common.usSpinnerService.stop('spnAPQP');
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
    $scope.UpdateDrawingNumberFields = function (fieldName, updateFromSource, val, apqpItemId) {
        $scope.IndividualFields = { FieldName: fieldName, UpdatedFromSource: updateFromSource, DrawingNumber: val, APQPItemId: apqpItemId };
        common.usSpinnerService.spin('spnAPQP');
        APQPSvc.UpdateIndividualFields($scope.IndividualFields).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   common.usSpinnerService.stop('spnAPQP');
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
    $scope.UpdateAPQPStatusFields = function (fieldName, updateFromSource, val, apqpItemId) {
        $scope.IndividualFields = { FieldName: fieldName, UpdatedFromSource: updateFromSource, StatusId: val, APQPItemId: apqpItemId };
        common.usSpinnerService.spin('spnAPQP');
        APQPSvc.UpdateIndividualFields($scope.IndividualFields).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   common.usSpinnerService.stop('spnAPQP');
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
    $scope.getAPQPItemName = function (apqpStatusId, objAPQP) {
        if (!IsUndefinedNullOrEmpty(apqpStatusId) && $scope.APQPStatusList.length > 0) {
            objAPQP.objKickOff.Status = $filter('filter')($scope.APQPStatusList, function (rw) { return rw.Id == parseInt(apqpStatusId, 10) })[0].Name;
        }
        else
            objAPQP.objKickOff.Status = "";
    };
    $scope.getManufacturerDetails = function (manufacturerId, objAPQP) {
        if (!IsUndefinedNullOrEmpty(manufacturerId) && manufacturerId > 0) {
            common.usSpinnerService.spin('spnAPQP');
            APQPSvc.getManufacturerDetails(manufacturerId).then(
          function (response) {
              common.usSpinnerService.stop('spnAPQP');
              if (response.data.StatusCode == 200) {
                  $scope.CompanyNameWithCode = (!IsUndefinedNullOrEmpty(response.data.Result.SupplierCode) ? response.data.Result.CompanyName + " - " + response.data.Result.SupplierCode : response.data.Result.CompanyName);
                  objAPQP.objKickOff.ManufacturerCode = response.data.Result.SupplierCode;
                  objAPQP.objKickOff.ManufacturerName = $scope.CompanyNameWithCode; //response.data.Result.CompanyName;
                  objAPQP.objKickOff.ManufacturerAddress1 = response.data.Result.Address1;
                  objAPQP.objKickOff.ManufacturerAddress2 = response.data.Result.Address2;
                  objAPQP.objKickOff.ManufacturerCity = response.data.Result.City;
                  objAPQP.objKickOff.ManufacturerState = response.data.Result.State;
                  objAPQP.objKickOff.ManufacturerCountry = response.data.Result.Country;
                  objAPQP.objKickOff.ManufacturerZip = response.data.Result.Zip;

                  objAPQP.objToolingLaunch.ManufacturerName = $scope.CompanyNameWithCode; //response.data.Result.CompanyName;
                  objAPQP.objProjectTracking.ManufacturerName = $scope.CompanyNameWithCode; //response.data.Result.CompanyName;
              }
          },
          function (error) {
              common.usSpinnerService.stop('spnAPQP');
          });
        }
        else {
            $scope.CompanyNameWithCode = "";
            objAPQP.objKickOff.ManufacturerCode = '';
            objAPQP.objKickOff.ManufacturerName = '';
            objAPQP.objKickOff.ManufacturerAddress1 = '';
            objAPQP.objKickOff.ManufacturerAddress2 = '';
            objAPQP.objKickOff.ManufacturerCity = '';
            objAPQP.objKickOff.ManufacturerState = '';
            objAPQP.objKickOff.ManufacturerCountry = '';
            objAPQP.objKickOff.ManufacturerZip = '';
            objAPQP.objToolingLaunch.ManufacturerName = '';
            objAPQP.objProjectTracking.ManufacturerName = '';
        }
    };
    /*APQP Checked Items*/
    $scope.SelectDeselectAllAPQPItems = function () {
        angular.forEach($scope.APQPList, function (item) {
            item.chkSelect = $scope.SelectAll;
        });
    };
    $scope.selectAPQPItems = function () {
        $scope.SelectAll = true;
        angular.forEach($scope.APQPList, function (item) {
            if (!item.chkSelect)
                $scope.SelectAll = false;
        });
    };
    /*APQP Checked Items*/
    /*PSW*/
    $scope.ShowPopupGeneratePSW = function (Id, objAPQP) {
        $scope.Id = Id;
        common.usSpinnerService.spin('spnAPQP');
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/APQP/APQP/APQPGeneratePSWPopup.html?v=' + Version,
            controller: GeneratePSWCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            sizeclass: 'modal-fitScreen',
            resolve: {

            }
        });
        modalInstance.result.then(function (filePath) {
            objAPQP.objPPAPSubmission.PSWFilePath = filePath;
        }, function () {
            objAPQP.objPPAPSubmission.PSWFilePath = '';
            console.log('error no file');
        });
    };
    $scope.OpenPSW = function (pswFilePath) {
        window.open(pswFilePath, '_blank');
    };
    /*PSW*/
    /*NPIF*/
    $scope.GenerateNPIF = function (apqpItemId, objAPQP) {
        $scope.objAllowGenerateNPIFForm = true;
        if (confirm($filter("translate")("_TabChangeConfirmSaveMessage_"))) {
            common.usSpinnerService.spin('spnAPQP');
            $scope.AlreadySelectedItem = true;
            if (!objAPQP.chkSelect) {
                objAPQP.chkSelect = true;
                $scope.AlreadySelectedItem = false;
            }
            $scope.objToSaveAPQPList = {};
            $scope.objToSaveAPQPList = angular.copy($filter('filter')($scope.APQPList, function (rw) {
                return rw.Id == apqpItemId
            }));
            if ($scope.objToSaveAPQPList.length > 0) {
                if ($scope.ValidateFields($scope.objToSaveAPQPList, false)) {
                    $scope.SaveAPQPItemList($scope.objToSaveAPQPList);
                }
                if (!$scope.AlreadySelectedItem) {
                    objAPQP.chkSelect = false;
                }
            }
            else
                common.usSpinnerService.stop('spnAPQP');
        }
        else
            $scope.GenerateAndDownloadNPIF(apqpItemId);
    };
    $scope.GenerateAndDownloadNPIF = function (apqpItemId) {
        $scope.objAllowGenerateNPIFForm = false;
        common.usSpinnerService.spin('spnAPQP');
        APQPSvc.GenerateNPIF(apqpItemId).then(
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
    /*NPIF*/
    /*Documents*/
    $scope.ShowPopupDocuments = function (callFrom, Id, objAPQP) {
        $scope.Id = Id;
        $scope.IsReadOnlyDocs = $scope.IsReadOnlySaveButton;
        common.usSpinnerService.spin('spnAPQP');
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/APQP/APQP/APQPDocumentsPopup.html?v=' + Version,
            controller: DocumentsCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            sizeclass: 'modal-fitToScreen',
            resolve: {
                CallFrom: function () {
                    return callFrom;
                }
            }
        });
        modalInstance.result.then(function (hasDocs) {
            objAPQP.objKickOff.IsDocument = hasDocs ? 1 : 0;
            objAPQP.objToolingLaunch.IsDocument = hasDocs ? 1 : 0;
            objAPQP.objProjectTracking.IsDocument = hasDocs ? 1 : 0;
            objAPQP.objPPAPSubmission.IsDocument = hasDocs ? 1 : 0;
        }, function () {
            console.log('error no docs');
        });
    };
    /*Documents*/
    /*Predefined Documents*/
    $scope.ShowSetDocumentsForItem = function () {
        common.usSpinnerService.spin('spnAPQP');
        var apqpItemIds = [], strapqpItemIds = "";
        angular.forEach($scope.APQPList, function (item, index) {
            if (item.chkSelect)
                apqpItemIds.push(item.Id);
        });
        if (apqpItemIds.length > 0)
            strapqpItemIds = apqpItemIds.join(",");
        else {
            common.usSpinnerService.stop('spnAPQP');
            common.aaNotify.error("Please select at least one part.");
            return false;
        }
        //$scope.Id = strapqpItemIds;

        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/APQP/APQP/APQPSetDocumentsForItemPopup.html?v=' + Version,
            controller: SetDocumentsForItemCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            sizeclass: 'modal-lg',
            resolve: {
                apqpItemIds: function () {
                    return strapqpItemIds;
                }
            }
        });
        modalInstance.result.then(function () {
        }, function () {
        });
    };
    /*Predefined Documents*/
    /*Send Email*/
    $scope.getTemplateAndUserIdsData = function (sectionName) {
        common.usSpinnerService.spin('spnAPQP');
        var apqpItemIds = [], strapqpItemIds = "";
        angular.forEach($scope.APQPList, function (item, index) {
            if (item.chkSelect)
                apqpItemIds.push(item.Id);
        });
        if (apqpItemIds.length > 0)
            strapqpItemIds = apqpItemIds.join(",");
        else {
            common.usSpinnerService.stop('spnAPQP');
            common.aaNotify.error("Please select at least one part.");
            return false;
        }

        $scope.Id = strapqpItemIds;
        $scope.EmailData = {};
        //set section name here 
        sectionName = GetAPQPStepNameByTabName($scope.TabObject.CurrentTab);

        APQPSvc.getTemplateAndUserIdsData($scope.Id, sectionName).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.EmailData = response.data.Result;
                   $scope.EmailData.EmailAttachment = '';
                   $scope.setLookupSendTo();
                   $timeout(function () {
                       common.usSpinnerService.stop('spnAPQP');
                       $scope.ShowPopupSendEmail(sectionName);
                   }, 0);
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
    //Start send email
    $scope.ShowPopupSendEmail = function (sectionName) {
        common.usSpinnerService.spin('spnAPQP');
        var modalInstance = $modal.open({
            templateUrl: '/App_Client/views/APQP/APQP/APQPSendEmailPopup.html?v=' + Version,
            controller: APQPSendEmailCtrl,
            keyboard: false,
            backdrop: false,
            scope: $scope,
            sizeclass: 'modal-md',
            resolve: {
                SectionName: function () {
                    return sectionName;
                },
                APQPItemIds: function () {
                    return $scope.Id;
                }
            }
        });
        modalInstance.result.then(function () {

        }, function () {
        });
    };
    $scope.setLookupSendTo = function () {
        $scope.LookUps = [
        {
            "Name": "Users", "Parameters": {}
        }
        ];
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            angular.forEach(data.data, function (o) {
                if (o.Name === "Users") {
                    $scope.APQPSendToUserList = o.Data;
                    if ($scope.APQPSendToUserList.length > 0) {
                        var UserIdsArray = $scope.EmailData.UserIds.split(',');
                        var objDefaultSelect = [];
                        angular.forEach(UserIdsArray, function (itemId) {
                            angular.forEach($scope.APQPSendToUserList, function (item) {
                                if (itemId == item.Id)
                                    objDefaultSelect.push(item);
                            });
                        });
                        $scope.EmailData.SendToIds = objDefaultSelect;
                    }
                }
            });
        });
    };
    //End send email
    /*Send Email*/
    //Start calculations
    $scope.setPlanToolingCompletionDate = function (toolingLeadtimeDays, toolingKickoffDate, objAPQP) {
        if (!Isundefinedornull(toolingLeadtimeDays) && toolingLeadtimeDays > 0 && !IsUndefinedNullOrEmpty(toolingKickoffDate)) {
            var calDate1 = new Date(toolingKickoffDate);
            objAPQP.objToolingLaunch.PlanToolingCompletionDate = calDate1.addDays(parseInt(toolingLeadtimeDays));
            objAPQP.objProjectTracking.PlanToolingCompletionDate = calDate1.addDays(parseInt(toolingLeadtimeDays));
            objAPQP.objProjectTracking.EstimatedSampleShipmentDate = calDate1.addDays(parseInt(toolingLeadtimeDays));
        }
    };
    $scope.setEstimatedSampleShipmentDate = function (pPAPSubmissonPreparationDays, planToolingCompletionDate, objAPQP) {
        if (!Isundefinedornull(pPAPSubmissonPreparationDays) && pPAPSubmissonPreparationDays > 0 && !IsUndefinedNullOrEmpty(planToolingCompletionDate)) {
            var calDate2 = new Date(planToolingCompletionDate);
            objAPQP.objProjectTracking.EstimatedSampleShipmentDate = calDate2.addDays(parseInt(pPAPSubmissonPreparationDays));
            objAPQP.objPPAPSubmission.PSWDate = calDate2.addDays(parseInt(pPAPSubmissonPreparationDays));
        }
    };
    //End calculations

    //popup for show change log
    $scope.ShowChangeLogPopup = function (apqpTtemId) {
        $scope.Id = apqpTtemId;
        var modalTemplatePreviewOptions = $modal.open({
            templateUrl: '/App_Client/views/APQP/APQP/AuditLogsPopup.html?v=' + Version,
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
    // change log popup end here
    //popup for NPIF DocuSign History
    $scope.ShowNPIFHistory = function (apqpItemId, PartNumber) {
        $scope.Id = apqpItemId;
        var modalTemplatePreviewOptions = $modal.open({
            templateUrl: '/App_Client/views/APQP/APQP/APQPNPIFDocuSignHistory.html?v=' + Version,
            controller: NPIFDocuSignHistoryCtrl,
            keyboard: true,
            backdrop: true,
            scope: $scope,
            sizeclass: 'modal-md',
            resolve: {                
                PartNumber: function () {
                    return PartNumber;
                },
            }
        });
        modalTemplatePreviewOptions.result.then(function () {
        }, function () {
        });
    };
    // popup for NPIF DocuSign History end here
    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.SearchAPQPItemList();
    };
    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.SearchAPQPItemList();
    };

    $scope.Init();
}]);

app.directive("changeBgcApqpList", function () {
    return {
        replace: false,
        restrict: 'A',
        link: function (scope, $element, $attrs) {
            var e = $element;
            var index = parseInt($attrs.changeBgcApqpList, 10);
            e.on('blur', function () {
                scope.setInvalidClass(scope.APQP, index);
            });
            scope.setInvalidClass = function (item, index) {
                if (IsUndefinedNullOrEmpty(item.objKickOff.PartNumber) || IsUndefinedNullOrEmpty(item.objKickOff.SAMUserId)
                                                        || isNaN(parseInt(item.objKickOff.SupplierId)) || parseInt(item.objKickOff.SupplierId) <= 0
                    || IsUndefinedNullOrEmpty(item.objKickOff.MaterialType)
                    || isNaN(parseInt(item.objKickOff.ToolingLeadtimeDays)) || parseInt(item.objKickOff.ToolingLeadtimeDays) <= 0) {
                    if (IsUndefinedNullOrEmpty(item.objKickOff.PartNumber)) {
                        if (!$('#PartNumber_' + index).hasClass('invalid-class'))
                            $('#PartNumber_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#PartNumber_' + index).removeClass('invalid-class');
                    }
                    if (IsUndefinedNullOrEmpty(item.objKickOff.SAMUserId)) {
                        if (!$('#SAMUserId_' + index).hasClass('invalid-class'))
                            $('#SAMUserId_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#SAMUserId_' + index).removeClass('invalid-class');
                    }
                    if (isNaN(parseInt(item.objKickOff.SupplierId)) || parseInt(item.objKickOff.SupplierId) <= 0) {
                        if (!$('#SupplierId_' + index).hasClass('invalid-class'))
                            $('#SupplierId_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#SupplierId_' + index).removeClass('invalid-class');
                    }
                    if (IsUndefinedNullOrEmpty(item.objKickOff.MaterialType)) {
                        if (!$('#MaterialType_' + index).hasClass('invalid-class'))
                            $('#MaterialType_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#MaterialType_' + index).removeClass('invalid-class');
                    }
                    if (isNaN(parseInt(item.objKickOff.ToolingLeadtimeDays)) || parseInt(item.objKickOff.ToolingLeadtimeDays) <= 0) {
                        if (!$('#ToolingLeadtimeDays_' + index).hasClass('invalid-class'))
                            $('#ToolingLeadtimeDays_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#ToolingLeadtimeDays_' + index).removeClass('invalid-class');
                    }
                }
                else {
                    $('#PartNumber_' + index).removeClass('invalid-class');
                    $('#SAMUserId_' + index).removeClass('invalid-class');
                    $('#SupplierId_' + index).removeClass('invalid-class');
                    $('#MaterialType_' + index).removeClass('invalid-class');
                    $('#ToolingLeadtimeDays_' + index).removeClass('invalid-class');
                }
            }
        }
    }
});