app.controller('AddEditAPQPCtrl', ['$rootScope', '$scope', 'common', 'APQPSvc', 'LookupSvc', 'ContactsSvc', '$filter', '$routeParams', '$timeout', '$modal', '$window',
    function ($rootScope, $scope, common, APQPSvc, LookupSvc, ContactsSvc, $filter, $routeParams, $timeout, $modal, $window) {
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
                $scope.CheckIsReadOnlySaveButton();
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
        $scope.CheckIsReadOnlySaveButton = function () {
            $scope.IsReadOnlySaveButton = false;
            if ($scope.IsReadOnlyKickOff && $scope.IsReadOnlyToolingLaunch && $scope.IsReadOnlyProjectTracking && $scope.IsReadOnlyPPAPSubmission)
                $scope.IsReadOnlySaveButton = true;
        };
        //End implement security role wise
        $scope.setRoleWisePrivilege();

        $('body').removeClass('paginationFixedToBottom haveAdvanceSearch');
        $rootScope.PageHeader = ($filter('translate')('_PageHeading_'));
        $scope.APQP = {};
        $scope.allowExportToSAP = false;
        $scope.allowCheckNPIFStatus = false;
        $scope.hasPricingFieldsAccess = false;
        $scope.Init = function () {
            $scope.SearchCriteria = {};
            $scope.Paging = GetDefaultPageObject();
            $scope.Paging.Criteria = $scope.SearchCriteria;
            $scope.objNPIFApprovalStart = false;
            if (localStorage.getItem("APQPListPaging") && localStorage.getItem("APQPListPageSearchCriteria")) {
                // $scope.$apply(function () {
                $scope.Paging = JSON.parse(localStorage.getItem("APQPListPaging"));
                $scope.SearchCriteria = JSON.parse(localStorage.getItem("APQPListPageSearchCriteria"));
                $scope.Paging.PageSize = 1;
                if (localStorage.getItem("PageNumber")) {
                    $scope.Paging.PageNo = parseInt(localStorage.getItem("PageNumber"), 10);
                }
                $scope.IsNextLast = parseInt($scope.Paging.PageNo) > 1 ? false : true;
                $scope.IsPreviousLast = parseInt($scope.Paging.TotalRecords) > parseInt($scope.Paging.PageNo) ? false : true;
                //});
            }
            if (!IsUndefinedNullOrEmpty($routeParams.Id) && $routeParams.Id != '0') {
                $scope.TransactionMode = 'Edit';
                $scope.Id = $routeParams.Id;
                $scope.APQP.Id = $routeParams.Id;
                $scope.getData($routeParams.Id);
            }
            else {
                $scope.TransactionMode = 'Create';
                common.usSpinnerService.stop('spnAPQP');
                RedirectToAccessDenied();
            }
            /////temp
            //$scope.getData(893);
        };

        $scope.getData = function (apqpId) {
            common.usSpinnerService.spin('spnAPQP');
            APQPSvc.getData(apqpId).then(
               function (response) {
                   if (ShowMessage(common, response.data)) {
                       $scope.APQP = response.data.Result;
                       try {
                           $scope.allowExportToSAP = $scope.APQP.AllowExportToSAP;
                           $scope.allowCheckNPIFStatus = $scope.APQP.AllowCheckNPIFStatus;
                           $scope.hasPricingFieldsAccess = $scope.APQP.HasPricingFieldsAccess;

                           //textbox values dates
                           $scope.APQP.objKickOff.ProjectKickoffDate = convertUTCDateToLocalDate($scope.APQP.objKickOff.ProjectKickoffDate);
                           $scope.APQP.objKickOff.CustomerToolingPOAuthRcvdDate = convertUTCDateToLocalDate($scope.APQP.objKickOff.CustomerToolingPOAuthRcvdDate);
                           //convertLocalDateToUTCDate
                           //tzutil /s "Pacific Standard Time"
                           //tzutil /s "India Standard Time"
                           $scope.APQP.objToolingLaunch.RevisionDate = convertLocalDateToUTCDate($scope.APQP.objToolingLaunch.RevisionDate);
                           $scope.APQP.objToolingLaunch.ToolingKickoffDate = convertLocalDateToUTCDate($scope.APQP.objToolingLaunch.ToolingKickoffDate);
                           $scope.APQP.objProjectTracking.CurrentEstimatedToolingCompletionDate = convertLocalDateToUTCDate($scope.APQP.objProjectTracking.CurrentEstimatedToolingCompletionDate);
                           $scope.APQP.objProjectTracking.ActualToolingCompletionDate = convertLocalDateToUTCDate($scope.APQP.objProjectTracking.ActualToolingCompletionDate);
                           $scope.APQP.objProjectTracking.EstimatedSampleShipmentDate = convertLocalDateToUTCDate($scope.APQP.objProjectTracking.EstimatedSampleShipmentDate);
                           $scope.APQP.objProjectTracking.ActualSampleShipmentDate = convertLocalDateToUTCDate($scope.APQP.objProjectTracking.ActualSampleShipmentDate);
                           $scope.APQP.objPPAPSubmission.ActualPSWDate = convertLocalDateToUTCDate($scope.APQP.objPPAPSubmission.ActualPSWDate);
                           $scope.APQP.objPPAPSubmission.PPAPPartsApprovedDate = convertLocalDateToUTCDate($scope.APQP.objPPAPSubmission.PPAPPartsApprovedDate);

                           //labels values dates
                           $scope.APQP.objKickOff.RevisionDate = convertLocalDateToUTCDate($scope.APQP.objKickOff.RevisionDate);
                           $scope.APQP.objToolingLaunch.PlanToolingCompletionDate = convertLocalDateToUTCDate($scope.APQP.objToolingLaunch.PlanToolingCompletionDate);
                           $scope.APQP.objProjectTracking.ToolingKickoffDate = convertLocalDateToUTCDate($scope.APQP.objProjectTracking.ToolingKickoffDate);
                           $scope.APQP.objProjectTracking.PlanToolingCompletionDate = convertLocalDateToUTCDate($scope.APQP.objProjectTracking.PlanToolingCompletionDate);
                           $scope.APQP.objPPAPSubmission.ActualSampleShipmentDate = convertLocalDateToUTCDate($scope.APQP.objPPAPSubmission.ActualSampleShipmentDate);
                           $scope.APQP.objPPAPSubmission.PSWDate = convertLocalDateToUTCDate($scope.APQP.objPPAPSubmission.PSWDate);

                           //set PPAPSubmissionLevel value
                           $scope.APQP.objKickOff.PPAPSubmissionLevel = Number($scope.APQP.objKickOff.PPAPSubmissionLevel);
                           if (IsUndefinedNullOrEmpty($scope.APQP.objPPAPSubmission.PSWFilePath))
                               $scope.APQP.objPPAPSubmission.PSWFilePath = '';

                           //set decimal formated values
                           if (!isNaN(parseFloat($scope.APQP.objKickOff.PartWeight)))
                               $scope.APQP.objKickOff.PartWeight = $filter("setDecimal")($scope.APQP.objKickOff.PartWeight, 3);
                           if (!isNaN(parseFloat($scope.APQP.objKickOff.PurchasePieceCost)))
                               $scope.APQP.objKickOff.PurchasePieceCost = $filter("setDecimal")($scope.APQP.objKickOff.PurchasePieceCost, 3);
                           if (!isNaN(parseFloat($scope.APQP.objKickOff.PurchaseToolingCost)))
                               $scope.APQP.objKickOff.PurchaseToolingCost = $filter("setDecimal")($scope.APQP.objKickOff.PurchaseToolingCost, 3);
                           if (!isNaN(parseFloat($scope.APQP.objKickOff.SellingPiecePrice)))
                               $scope.APQP.objKickOff.SellingPiecePrice = $filter("setDecimal")($scope.APQP.objKickOff.SellingPiecePrice, 3);
                           if (!isNaN(parseFloat($scope.APQP.objKickOff.SellingToolingPrice)))
                               $scope.APQP.objKickOff.SellingToolingPrice = $filter("setDecimal")($scope.APQP.objKickOff.SellingToolingPrice, 3);
                           if (!isNaN(parseFloat($scope.APQP.objKickOff.SellingPiecePrice)))
                               $scope.APQP.objKickOff.SellingPiecePrice = $filter("setDecimal")($scope.APQP.objKickOff.SellingPiecePrice, 3);

                       } catch (e) {
                           common.usSpinnerService.stop('spnAPQP');
                       }
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
                   console.log(error);
               });
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
                "Name": "CustomerContacts", "Parameters": { "customerId": (!IsUndefinedNullOrEmpty($scope.APQP.objKickOff.CustomerId) ? parseInt($scope.APQP.objKickOff.CustomerId) : 0) }
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
            }
            ];
            $scope.getLookupData();
        };
        $scope.getLookupData = function () {
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                angular.forEach(data.data, function (o) {
                    if (o.Name === "SAM") {
                        $scope.SAMList = o.Data;
                    }
                    else if (o.Name === "APQPStatus") {
                        $scope.APQPStatusList = o.Data;
                    }
                    else if (o.Name === "CustomerContacts") {
                        $scope.CustomerPurchasingContactList = o.Data;
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
                });
                $scope.setLookupProjectStages();
            });
        };
        $scope.setLookupProjectStages = function () {
            $scope.LookUps = [
            {
                "Name": "ProjectStages", "Parameters": { "ProjectCategoryId": $scope.APQP.objProjectTracking.APQPProjectCategoryId }
            }
            ];
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                angular.forEach(data.data, function (o) {
                    if (o.Name === "ProjectStages") {
                        $scope.PTStageList = o.Data;
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
            if (!Isundefinedornull($scope.CustomerPurchasingContactList) && !IsUndefinedNullOrEmpty(objAPQP.objKickOff.CustomerProjectLead)) {
                var objCustomerPurchasingContact = $filter('filter')($scope.CustomerPurchasingContactList, function (rw) {
                    return rw.Name.toLowerCase() == objAPQP.objKickOff.CustomerProjectLead.toLowerCase()
                });
                if (objCustomerPurchasingContact.length <= 0) {
                    objAPQP.objKickOff.CustomerProjectLeadId = '';
                }
            }
            else
                objAPQP.objKickOff.CustomerProjectLeadId = '';
        };
        $scope.ShowPopupGeneratePSW = function () {
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
                $scope.APQP.objPPAPSubmission.PSWFilePath = filePath;
            }, function () {
                $scope.APQP.objPPAPSubmission.PSWFilePath = '';
                console.log('error no file');
            });
        };
        $scope.SaveAPQP = function () {
            common.usSpinnerService.spin('spnAPQP');
            //convert UTC date values into local dates
            try {
                $scope.APQP.objKickOff.ProjectKickoffDate = convertUTCDateToLocalDate($scope.APQP.objKickOff.ProjectKickoffDate);
                $scope.APQP.objKickOff.CustomerToolingPOAuthRcvdDate = convertUTCDateToLocalDate($scope.APQP.objKickOff.CustomerToolingPOAuthRcvdDate);
                $scope.APQP.objToolingLaunch.RevisionDate = convertUTCDateToLocalDate($scope.APQP.objToolingLaunch.RevisionDate);
                $scope.APQP.objToolingLaunch.ToolingKickoffDate = convertUTCDateToLocalDate($scope.APQP.objToolingLaunch.ToolingKickoffDate);
                $scope.APQP.objProjectTracking.CurrentEstimatedToolingCompletionDate = convertUTCDateToLocalDate($scope.APQP.objProjectTracking.CurrentEstimatedToolingCompletionDate);
                $scope.APQP.objProjectTracking.ActualToolingCompletionDate = convertUTCDateToLocalDate($scope.APQP.objProjectTracking.ActualToolingCompletionDate);
                $scope.APQP.objProjectTracking.EstimatedSampleShipmentDate = convertUTCDateToLocalDate($scope.APQP.objProjectTracking.EstimatedSampleShipmentDate);
                $scope.APQP.objProjectTracking.ActualSampleShipmentDate = convertUTCDateToLocalDate($scope.APQP.objProjectTracking.ActualSampleShipmentDate);
                $scope.APQP.objPPAPSubmission.ActualPSWDate = convertUTCDateToLocalDate($scope.APQP.objPPAPSubmission.ActualPSWDate);
                $scope.APQP.objPPAPSubmission.PPAPPartsApprovedDate = convertUTCDateToLocalDate($scope.APQP.objPPAPSubmission.PPAPPartsApprovedDate);
                console.log('in try');
            } catch (e) {
                console.log('in catch');
                common.usSpinnerService.stop('spnAPQP');
            }
            APQPSvc.Save($scope.APQP).then(
               function (response) {
                   if (ShowMessage(common, response.data)) {
                       common.usSpinnerService.stop('spnAPQP');
                       $scope.APQP.Id = response.data.Result;
                       //Start set redirect here for Change Request Form
                       if ($scope.isCallFromChangeRequestForm) {
                           if (!Isundefinedornull($scope.Id)) {
                               $window.location.href = "/APQP/ChangeRequest#/AddEdit/0/" + $scope.Id;
                           }
                       }
                       $scope.isCallFromChangeRequestForm = false;
                       //End set redirect here for Change Request Form

                       //Start GenerateNPIFForm
                       if ($scope.objAllowGenerateNPIFForm) {
                           $scope.GenerateAndDownloadNPIF();
                       }
                       $scope.objAllowGenerateNPIFForm = false;
                       //End GenerateNPIFForm

                       //Start startNPIFApproval
                       if ($scope.objNPIFApprovalStart == true)
                           $scope.startNPIFApproval();//End startNPIFApproval                               
                       else
                           $scope.ResetForm();
                       $scope.objNPIFApprovalStart = false;
                   }
                   else {
                       common.usSpinnerService.stop('spnAPQP');
                       $scope.objAllowGenerateNPIFForm = false;
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnAPQP');
                   console.log(error);
                   $scope.objAllowGenerateNPIFForm = false;
               });
        };

        /*UPDATE COMMON FIELDS ON EDIT FOR CHANGE LOG MNGT*/
        $scope.UpdateRevLevelFields = function (fieldName, updateFromSource, val) {
            $scope.IndividualFields = { FieldName: fieldName, UpdatedFromSource: updateFromSource, RevLevel: val, APQPItemId: $scope.Id };
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
        $scope.UpdateDrawingNumberFields = function (fieldName, updateFromSource, val) {
            $scope.IndividualFields = { FieldName: fieldName, UpdatedFromSource: updateFromSource, DrawingNumber: val, APQPItemId: $scope.Id };
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
        $scope.UpdateAPQPStatusFields = function (fieldName, updateFromSource, val) {
            $scope.IndividualFields = { FieldName: fieldName, UpdatedFromSource: updateFromSource, StatusId: val, APQPItemId: $scope.Id };
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
        $scope.getAPQPItemName = function (apqpStatusId) {
            if (!IsUndefinedNullOrEmpty(apqpStatusId) && $scope.APQPStatusList.length > 0) {
                $scope.APQP.objKickOff.Status = $filter('filter')($scope.APQPStatusList, function (rw) { return rw.Id == parseInt(apqpStatusId, 10) })[0].Name;
            }
            else
                $scope.APQP.objKickOff.Status = "";
        };
        $scope.getManufacturerDetails = function (manufacturerId) {
            if (!IsUndefinedNullOrEmpty(manufacturerId) && manufacturerId > 0) {
                common.usSpinnerService.spin('spnAPQP');
                APQPSvc.getManufacturerDetails(manufacturerId).then(
              function (response) {
                  common.usSpinnerService.stop('spnAPQP');
                  if (response.data.StatusCode == 200) {
                      $scope.CompanyNameWithCode = (!IsUndefinedNullOrEmpty(response.data.Result.SupplierCode) ? response.data.Result.CompanyName + " - " + response.data.Result.SupplierCode : response.data.Result.CompanyName);
                      $scope.APQP.objKickOff.ManufacturerCode = response.data.Result.SupplierCode;
                      $scope.APQP.objKickOff.ManufacturerName = $scope.CompanyNameWithCode; //response.data.Result.CompanyName;
                      $scope.APQP.objKickOff.ManufacturerAddress1 = response.data.Result.Address1;
                      $scope.APQP.objKickOff.ManufacturerAddress2 = response.data.Result.Address2;
                      $scope.APQP.objKickOff.ManufacturerCity = response.data.Result.City;
                      $scope.APQP.objKickOff.ManufacturerState = response.data.Result.State;
                      $scope.APQP.objKickOff.ManufacturerCountry = response.data.Result.Country;
                      $scope.APQP.objKickOff.ManufacturerZip = response.data.Result.Zip;

                      $scope.APQP.objToolingLaunch.ManufacturerName = $scope.CompanyNameWithCode; //response.data.Result.CompanyName;
                      $scope.APQP.objProjectTracking.ManufacturerName = $scope.CompanyNameWithCode; //response.data.Result.CompanyName;
                  }
              },
              function (error) {
                  common.usSpinnerService.stop('spnAPQP');
              });
            }
            else {
                $scope.CompanyNameWithCode = "";
                $scope.APQP.objKickOff.ManufacturerCode = '';
                $scope.APQP.objKickOff.ManufacturerName = '';
                $scope.APQP.objKickOff.ManufacturerAddress1 = '';
                $scope.APQP.objKickOff.ManufacturerAddress2 = '';
                $scope.APQP.objKickOff.ManufacturerCity = '';
                $scope.APQP.objKickOff.ManufacturerState = '';
                $scope.APQP.objKickOff.ManufacturerCountry = '';
                $scope.APQP.objKickOff.ManufacturerZip = '';
                $scope.APQP.objToolingLaunch.ManufacturerName = '';
                $scope.APQP.objProjectTracking.ManufacturerName = '';
            }
        };
        /*UPDATE COMMON FIELDS ON EDIT FOR CHANGE LOG MNGT*/

        $scope.setFormName = function (formName) {
            $scope.formName = formName;
        }

        $scope.PPAPSubmissionSAPDataExport = function () {
            common.usSpinnerService.spin('spnAPQP');
            APQPSvc.PPAPSubmissionSAPDataExport($scope.Id).then(
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
        $scope.GenerateNPIF = function () {
            $scope.objAllowGenerateNPIFForm = true;
            if (confirm($filter("translate")("_TabChangeConfirmSaveMessage_"))) {
                if ($scope.formName.$valid) {
                    $scope.SaveAPQP();
                }
                else
                    common.aaNotify.error($filter("translate")("_InvalidForm_"));
            }
            else
                $scope.GenerateAndDownloadNPIF();
        };
        $scope.GenerateAndDownloadNPIF = function () {
            $scope.objAllowGenerateNPIFForm = false;
            common.usSpinnerService.spin('spnAPQP');
            APQPSvc.GenerateNPIF($scope.Id).then(
               function (response) {
                   common.usSpinnerService.stop('spnAPQP');
                   if (response.data.StatusCode == 200) {
                       //alert(response.data.SuccessMessage);
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

        $scope.ShowPopupDocuments = function (callFrom, isReadOnlyDocs) {
            $scope.IsReadOnlyDocs = isReadOnlyDocs;
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
                console.log(hasDocs);
            }, function () {
                console.log('error no docs');
            });
        };
        $scope.ShowSetDocumentsForItem = function () {
            common.usSpinnerService.spin('spnAPQP');
            var modalInstance = $modal.open({
                templateUrl: '/App_Client/views/APQP/APQP/APQPSetDocumentsForItemPopup.html?v=' + Version,
                controller: SetDocumentsForItemCtrl,
                keyboard: false,
                backdrop: false,
                scope: $scope,
                sizeclass: 'modal-lg',
                resolve: {
                    apqpItemIds: function () {
                        return $scope.Id;
                    }
                }
            });
            modalInstance.result.then(function () {
            }, function () {
            });
        };

        $scope.OpenPSW = function (pswFilePath) {
            window.open(pswFilePath, '_blank');
        };
        $scope.getTemplateAndUserIdsData = function (sectionName) {
            $scope.EmailData = {};
            common.usSpinnerService.spin('spnAPQP');
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

        $scope.saveAndstartNPIFApproval = function () {
            $scope.objNPIFApprovalStart = true;
            if ($scope.formName.$valid) {
                $scope.SaveAPQP();
            }
            else
                common.aaNotify.error($filter("translate")("_InvalidForm_"));
        }

        $scope.startNPIFApproval = function () {

            var modalInstance = $modal.open({
                templateUrl: '/App_Client/views/APQP/APQP/APQPNPIFStartApprovalPopup.html?v=' + Version,
                controller: startNPIFApprovalCtrl,
                keyboard: false,
                backdrop: false,
                scope: $scope,
                sizeclass: 'modal-md',
                resolve: {
                    APQPItemIds: function () {
                        return $scope.Id;
                    },
                    PartNumber: function () {
                        return $scope.APQP.objKickOff.PartNumber;
                    },
                }
            });
            modalInstance.result.then(function (val) {
                if (val == 'false')
                    $scope.APQP.objKickOff.showDocuSignApprovalBtn = false;
                else
                    $scope.APQP.objKickOff.showDocuSignApprovalBtn = true;
            }, function () {
            });

        }

        $scope.checkNPIFDocuSignStatus = function () {
            common.usSpinnerService.spin('spnAPQP');
            APQPSvc.checkNPIFDocuSignStatus().then(
             function (response) {
                 if (ShowMessage(common, response.data)) {

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

        //Start calculations
        $scope.setPlanToolingCompletionDate = function (toolingLeadtimeDays, toolingKickoffDate) {
            if (!Isundefinedornull(toolingLeadtimeDays) && toolingLeadtimeDays > 0 && !IsUndefinedNullOrEmpty(toolingKickoffDate)) {
                var calDate1 = new Date(toolingKickoffDate);
                $scope.APQP.objToolingLaunch.PlanToolingCompletionDate = calDate1.addDays(parseInt(toolingLeadtimeDays));
                $scope.APQP.objProjectTracking.PlanToolingCompletionDate = calDate1.addDays(parseInt(toolingLeadtimeDays));
                $scope.APQP.objProjectTracking.EstimatedSampleShipmentDate = calDate1.addDays(parseInt(toolingLeadtimeDays));
            }
        };
        $scope.setEstimatedSampleShipmentDate = function (pPAPSubmissonPreparationDays, planToolingCompletionDate) {
            if (!Isundefinedornull(pPAPSubmissonPreparationDays) && pPAPSubmissonPreparationDays > 0 && !IsUndefinedNullOrEmpty(planToolingCompletionDate)) {
                var calDate2 = new Date(planToolingCompletionDate);
                $scope.APQP.objProjectTracking.EstimatedSampleShipmentDate = calDate2.addDays(parseInt(pPAPSubmissonPreparationDays));
                $scope.APQP.objPPAPSubmission.PSWDate = calDate2.addDays(parseInt(pPAPSubmissonPreparationDays));
            }
        };
        //End calculations

        //Start Next previous record code
        $scope.PreviousRecord = function () {
            if (parseInt($scope.Paging.TotalRecords) > parseInt($scope.Paging.PageNo)) {
                $scope.Paging.PageNo = $scope.Paging.PageNo + 1;
                localStorage.setItem("PageNumber", $scope.Paging.PageNo);
                $scope.getNextOrPreviousAPQPItemId();
            }
        };
        $scope.NextRecord = function () {
            if (parseInt($scope.Paging.PageNo) > 1) {
                $scope.Paging.PageNo = $scope.Paging.PageNo - 1;
                localStorage.setItem("PageNumber", $scope.Paging.PageNo);
                $scope.getNextOrPreviousAPQPItemId();
            }
        };
        $scope.getNextOrPreviousAPQPItemId = function () {
            common.usSpinnerService.spin('spnAPQP');
            APQPSvc.getNextOrPreviousAPQPItemId($scope.Paging).then(
               function (response) {
                   if (ShowMessage(common, response.data)) {
                       common.usSpinnerService.stop('spnAPQP');
                       $scope.nextorPreviousAPQPItemId = response.data.Result;
                       common.$location.path("AddEdit/" + $scope.nextorPreviousAPQPItemId + "/" + $routeParams.APQPStep);
                   }
                   else {
                       common.usSpinnerService.stop('spnAPQP');
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnAPQP');
               });
        };
        //End Next previous record code

        //popup for show change log
        $scope.ShowChangeLogPopup = function () {
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

        $scope.GoToChangeRequestForm = function () {
            $scope.SaveChangeRequestForm();
        };
        $scope.SaveChangeRequestForm = function () {
            common.usSpinnerService.spin('spnAPQP');
            $scope.isCallFromChangeRequestForm = true;
            $scope.SaveAPQP();
        };
        $scope.RedirectToList = function () {
            if (IsUndefinedNullOrEmpty($routeParams.APQPStep)) {
                //localStorage.clear();
                if (localStorage.getItem("APQPStep"))
                    localStorage.removeItem("APQPStep");
            }
            else {
                localStorage.setItem("APQPStep", $routeParams.APQPStep);
                localStorage.setItem("APQPItemId", $routeParams.Id);
            }
            common.$location.path("/APQPItemList/FormView");
        };
        $scope.ResetForm = function () {
            common.$route.reload();
        };
        $scope.Init();
    }]);

var GeneratePSWCtrl = function ($scope, common, $location, $modalInstance, $timeout, $filter, APQPSvc) {
    $scope.GeneratePSWInit = function () {
        $timeout(function () {
            common.usSpinnerService.stop('spnAPQP');
        }, 0);

        $scope.objSelectAll = { SelectAll: false };
        $scope.GeneratePSWData = {};
        $scope.PSWDocumentsList = {};
        $scope.getGeneratePSWData($scope.Id);
    };

    $scope.getGeneratePSWData = function (apqpId) {
        common.usSpinnerService.spin('spnAPQP');
        APQPSvc.getGeneratePSWData(apqpId).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.GeneratePSWData = response.data.Result;
                   $scope.GeneratePSWData.ToolingKickoffDate = convertUTCDateToLocalDate($scope.GeneratePSWData.ToolingKickoffDate);
                   $scope.GeneratePSWData.PartWeight = $filter("setDecimal")($scope.GeneratePSWData.PartWeight, 3);
                   //** this section is used for the Generate PSW button in PPAP submission tab.
                   $scope.GeneratePSWData.AdditionalEngineeringChanges = "N/A";
                   //    The Supplier Address and Information should default to the MES Ohio Office details:
                   //              "MES, Inc"
                   //              "625 Bear Run Lane"
                   //              "Lewis Center, OH 43214"
                   //              "USA"
                   //Should not be our supplier information.
                   $scope.GeneratePSWData.SupplierName = "MES, Inc";
                   $scope.GeneratePSWData.SupplierAddress1 = "625 Bear Run Lane";
                   $scope.GeneratePSWData.SupplierCity = "Lewis Center";
                   $scope.GeneratePSWData.SupplierState = "OH";
                   $scope.GeneratePSWData.SupplierZip = "43214";

                   $timeout(function () {
                       $scope.getAPQPDocumentData(apqpId);
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
    $scope.ExportGeneratePSW = function () {
        common.usSpinnerService.spin('spnAPQP');
        var DocumentIds = [], strDocumentIds = "";
        angular.forEach($scope.PSWDocumentsList, function (item, index) {
            if (item.chkSelect)
                DocumentIds.push(item.Id);
        });
        if (DocumentIds.length > 0)
            strDocumentIds = DocumentIds.join(",");

        $scope.GeneratePSWData.DocumentIds = strDocumentIds;
        APQPSvc.ExportGeneratePSW($scope.GeneratePSWData).then(
           function (response) {
               common.usSpinnerService.stop('spnAPQP');
               if (response.data.StatusCode == 200) {
                   common.aaNotify.success("PSW generated successfully.");
                   if (response.data.SuccessMessage) {
                       $scope.Cancel(response.data.SuccessMessage);
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
    $scope.getAPQPDocumentData = function (apqpId) {

        APQPSvc.getDocumentsData(apqpId, '0').then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.SearchDocumentData = response.data.Result;
                   $scope.PSWDocumentsList = $scope.SearchDocumentData.lstDocument;
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnAPQP');
           });
    };
    //Start logic for checkbox select deselect here
    $scope.SelectDeselectAll = function () {
        angular.forEach($scope.PSWDocumentsList, function (item) {
            item.chkSelect = $scope.objSelectAll.SelectAll;
        });
    };
    $scope.select = function () {
        $scope.objSelectAll.SelectAll = true;
        angular.forEach($scope.PSWDocumentsList, function (item) {
            if (!item.chkSelect)
                $scope.objSelectAll.SelectAll = false;
        });
    };
    //end logic for checkbox select deselect here

    $scope.Cancel = function (filePath) {
        $modalInstance.close(filePath);
        //$modalInstance.dismiss('cancel');
    };
    $scope.GeneratePSWInit();
};

var DocumentsCtrl = function ($scope, common, $location, $modalInstance, $timeout, $filter, $confirm, CallFrom, APQPSvc, LookupSvc) {
    $scope.DocumentsInit = function () {
        $scope.CallFrom = CallFrom;
        $scope.FolderName = GetAPQPStepName($scope.CallFrom);
        $scope.IsEditMode = "No";
        $scope.DocumentTypeId = 0;
        $scope.AddEditDocument = false;
        $scope.APQPDocument = {};
        $scope.APQPDocumentOriginal = {};
        $scope.APQPDocumentHeader = {};
        $scope.DocumentsList = {};
        $scope.DocumentsListWithGroup = {};
        $scope.SearchDocumentData = {};
        $scope.getAPQPDocumentData($scope.Id);
    };
    $scope.getAPQPDocumentData = function (apqpId) {
        common.usSpinnerService.spin('spnAPQP');
        APQPSvc.getDocumentsData(apqpId, $scope.CallFrom).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.SearchDocumentData = response.data.Result;
                   $scope.APQPDocumentHeader = $scope.SearchDocumentData.objPSWItem;
                   $scope.APQPDocumentHeader.ToolingKickoffDate = convertUTCDateToLocalDate($scope.APQPDocumentHeader.ToolingKickoffDate);
                   $scope.DocumentsList = $scope.SearchDocumentData.lstDocument;
                   angular.forEach($scope.DocumentsList, function (obj, index) {
                       obj.ReceivedDate = convertUTCDateToLocalDate(obj.ReceivedDate);
                   });
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
            "Name": "APQPDocumentType", "Parameters": { "AssociatedToId": $scope.CallFrom, "APQPItemId": $scope.Id, "DocumentTypeId": $scope.DocumentTypeId, "IsEditMode": $scope.IsEditMode }
        }
        ];
        $scope.getDocumentLookupData();
    };
    $scope.getDocumentLookupData = function () {
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            angular.forEach(data.data, function (o) {
                if (o.Name === "APQPDocumentType") {
                    $scope.DocumentLookupList = o.Data;
                }
            });
            common.usSpinnerService.stop('spnAPQP');
        }, function (error) {
            common.usSpinnerService.stop('spnAPQP');
        });
    };

    $scope.AddNewDocument = function () {
        $scope.ResetDocument();
        $scope.AddEditDocument = true;
        $scope.SetDocumentLooksupData();
    };
    $scope.EditDocument = function (item, index, documentId) {
        $scope.APQPDocument = {};
        $scope.APQPDocument = angular.copy(item);
        $scope.APQPDocumentOriginal = angular.copy(item);
        $scope.DocumentTypeId = $scope.APQPDocument.DocumentTypeId;
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
            $scope.APQPDocument.Id = 0;
            $scope.APQPDocumentOriginal = {};
        }
        //else {
        //    $scope.DocumentsList.splice(index, 1);
        //}
        $scope.AddEditDocument = true;
        $scope.SetDocumentLooksupData();
    };
    $scope.DeleteDocument = function (documentId, index) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            common.usSpinnerService.spin('spnAPQP');
            APQPSvc.DeleteDocument(documentId).then(
              function (response) {
                  if (ShowMessage(common, response.data)) {
                      $scope.ResetDocument();
                      $scope.getAPQPDocumentData($scope.Id);
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
        if ($scope.DocumentValidation($scope.APQPDocument)) {
            $scope.APQPDocument.SectionName = $scope.CallFrom;
            $scope.APQPDocument.APQPItemId = $scope.Id;
            $scope.APQPDocument.ReceivedDate = convertUTCDateToLocalDate($scope.APQPDocument.ReceivedDate);
            common.usSpinnerService.spin('spnAPQP');
            APQPSvc.SaveDocument($scope.APQPDocument).then(
              function (response) {
                  if (ShowMessage(common, response.data)) {
                      common.usSpinnerService.stop('spnAPQP');
                      $scope.ResetDocument();
                      $scope.getAPQPDocumentData($scope.Id);
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
    $scope.ShareDocumentFiles = function (lstAPQPSupplierDetails, documentId) {
        common.usSpinnerService.spin('spnAPQP');
        var getAPQPItemsIds = [], strGetAPQPItemsIds = '';
        angular.forEach(lstAPQPSupplierDetails, function (obj, index) {
            if (obj.chkSelect)
                getAPQPItemsIds.push(obj.APQPItemId);
        });
        if (getAPQPItemsIds.length > 0)
            strGetAPQPItemsIds = getAPQPItemsIds.join(",");
        else {
            common.usSpinnerService.stop('spnAPQP');
            common.aaNotify.error('Please select at least one supplier');
            return false;
        }
        APQPSvc.SaveShareDocumentFiles(strGetAPQPItemsIds, documentId).then(
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
          });
    };
    $scope.CancelDocument = function () {
        //if (!Isundefinedornull($scope.APQPDocumentOriginal) && !IsUndefinedNullOrEmpty($scope.APQPDocumentOriginal.DocumentTypeId)) {
        //    $scope.DocumentsList.push($scope.APQPDocumentOriginal);
        //}
        $scope.ResetDocument();
    };
    $scope.ResetDocument = function () {
        $scope.IsEditMode = "No";
        $scope.APQPDocumentOriginal = {};
        $scope.APQPDocument = {}
        $scope.AddEditDocument = false;
        $scope.DocumentTypeId = 0;
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
        else if (Isundefinedornull(objDocument.ReceivedDate)) {
            common.aaNotify.error(($filter('translate')('_ReceivedDate_')) + ' is required.');
            return false;
        }
        return true;
    };
    $scope.SetObjectvalues = function (UploadedFilePath, UploadedFileName) {
        $scope.APQPDocument.FilePath = UploadedFilePath;
        $scope.APQPDocument.FileTitle = UploadedFileName;
    };
    $scope.deleteFile = function (UploadedFilePath) {
        $scope.APQPDocument.FilePath = '';
        $scope.APQPDocument.FileTitle = '';
    };
    $scope.Cancel = function () {
        common.usSpinnerService.spin('spnAPQP');
        APQPSvc.GetDocumentsAvailabilityByAPQPItemId($scope.Id).then(
           function (response) {
               if (response.data.StatusCode == 200) {
                   common.usSpinnerService.stop('spnAPQP');
                   $modalInstance.close(response.data.Result);
               }
               else {
                   common.usSpinnerService.stop('spnAPQP');
                   $modalInstance.dismiss('cancel');
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnAPQP');
               $modalInstance.dismiss('cancel');
           });

    };
    $scope.DocumentsInit();
};

var SetDocumentsForItemCtrl = function ($scope, common, $location, $modalInstance, $timeout, apqpItemIds, APQPSvc) {
    $scope.DocumentsForItemInit = function () {
        $scope.objSelectAll = { SelectAll: false };
        $scope.APQPSetDocumentsForItems = {};
        $scope.getAPQPSetDocumentsForItemData(apqpItemIds);
    };

    $scope.getAPQPSetDocumentsForItemData = function (apqpItemIds) {
        common.usSpinnerService.spin('spnAPQP');
        APQPSvc.getAPQPPredefinedDocumentTypes(apqpItemIds).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.APQPSetDocumentsForItems = response.data.Result;
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
    $scope.SaveSetDocumentForItem = function () {
        common.usSpinnerService.spin('spnAPQP');
        var DocumentTypeIds = [], strDocumentTypeIds = "";
        angular.forEach($scope.APQPSetDocumentsForItems, function (item, index) {
            if (item.chkSelect)
                DocumentTypeIds.push(item.Id);
        });
        if (DocumentTypeIds.length > 0)
            strDocumentTypeIds = DocumentTypeIds.join(",");
        else {
            common.usSpinnerService.stop('spnAPQP');
            common.aaNotify.error("Please select at least one document type.");
            return false;
        }
        APQPSvc.SavePredefinedDocumentType(strDocumentTypeIds, apqpItemIds).then(
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
               console.log(error);
           });
    };

    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    //Start logic for checkbox select deselect here
    $scope.SelectDeselectAll = function () {
        angular.forEach($scope.APQPSetDocumentsForItems, function (item) {
            item.chkSelect = $scope.objSelectAll.SelectAll;
        });
    };
    $scope.select = function () {
        $scope.objSelectAll.SelectAll = true;
        angular.forEach($scope.APQPSetDocumentsForItems, function (item) {
            if (!item.chkSelect)
                $scope.objSelectAll.SelectAll = false;
        });
    };
    //end logic for checkbox select deselect here

    $scope.DocumentsForItemInit();
};

var NPIFDocuSignHistoryCtrl = function ($scope, $modalInstance, $filter, $timeout, common, PartNumber, APQPSvc) {
    $scope.Init = function () {
        if (!IsUndefinedNullOrEmpty($scope.Id) && parseInt($scope.Id) > 0) {
            $scope.lstnpifDocuSign = {};
            $scope.getNPIFDocuSignList($scope.Id);
        }
    };

    $scope.getNPIFDocuSignList = function (apqpItemId) {
        common.usSpinnerService.spin('spnAPQP');
        APQPSvc.getNPIFDocuSignList(apqpItemId).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.lstnpifDocuSign = response.data.Result;
                   $scope.lstnpifDocuSign.PartNumber = PartNumber;
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

    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.Init();
};

var startNPIFApprovalCtrl = function ($scope, $modalInstance, $filter, $routeParams, $timeout, common, APQPItemIds, PartNumber, APQPSvc, LookupSvc) {

    $scope.NPIFApprovalInit = function () {
        $scope.apqpNPIFDocuSign = {
            lstNPIFDocuSignApprovers: []
        };
        $scope.objNPIFRecipients = {};
        $scope.objNPIFRecipients.RoutingOrder = 1;
        $scope.lstRoutingOrder = [{ 'name': '1', 'value': 1 }
                                , { 'name': '2', 'value': 2 }
                                , { 'name': '3', 'value': 3 }
                                , { 'name': '4', 'value': 4 }
                                , { 'name': '5', 'value': 5 }
                                , { 'name': '6', 'value': 6 }];

        $scope.GetPredefinedNPIFRecipients();

        $timeout(function () {
            common.usSpinnerService.stop('spnAPQP');
        }, 0);
    }

    $scope.GetPredefinedNPIFRecipients = function () {
        common.usSpinnerService.spin('spnAPQP');
        APQPSvc.getPredefinedNPIFRecipients(APQPItemIds).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.apqpNPIFDocuSign = response.data.Result;
                   $scope.apqpNPIFDocuSign.PartNumber = PartNumber;
                   $scope.SetLookupData();
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

    $scope.SetLookupData = function () {
        $scope.LookUps = [
          {
              "Name": "NPIFDesignations", "Parameters": {}
          }
        ];
        $scope.getLookupData();
    };

    $scope.getLookupData = function () {
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            angular.forEach(data.data, function (o) {
                if (o.Name === "NPIFDesignations") {
                    $scope.lstDesignation = o.Data;
                    $scope.lstFilteredDesignation = [];
                }
            });
            angular.forEach($scope.apqpNPIFDocuSign.lstNPIFDocuSignApprovers, function (o) {
                $scope.getUsers(o.DesignationId, o);
            });
        });
    };

    $scope.getUsers = function (id, obj) {
        common.usSpinnerService.spin('spnAPQP');
        setTimeout(function () {
            $scope.LookUps = [
                {
                    "Name": "UserByDesignation", "Parameters": { "DesignationId": id }
                }
            ];
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                angular.forEach(data.data, function (o) {
                    if (o.Name === "UserByDesignation") {
                        obj.UserList = o.Data;
                        obj.Email = '';
                        if (!Isundefinedornull(obj.UserList) && obj.UserList.length > 0) {

                            if (obj.DesignationId == 1 && !IsUndefinedNullOrEmpty($scope.APQP.objKickOff.APQPEngineerId)) {
                                obj.UserId = $scope.APQP.objKickOff.APQPEngineerId;
                            }
                            else if (obj.DesignationId == 5 && !IsUndefinedNullOrEmpty($scope.APQP.objKickOff.SAMUserId)) {
                                obj.UserId = $scope.APQP.objKickOff.SAMUserId;
                            }
                            else
                                obj.UserId = obj.UserList[0].Id;

                            $scope.getUserEmail(obj.UserId, obj);
                        }
                    }
                });
            });

        }, 600);

        common.usSpinnerService.stop('spnAPQP');
    };

    $scope.getUserEmail = function (userId, obj) {
        $scope.LookUps = [
               {
                   "Name": "EmailByUser", "Parameters": { "userId": '' + userId + '' }
               }
        ];
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            angular.forEach(data.data, function (o) {
                if (o.Name === "EmailByUser") {
                    if (o.Data.length > 0)
                        obj.Email = o.Data[0].Name;
                }
            });
        });

    };

    $scope.AddNPIFRecipients = function () {
        $scope.NPIFRecipientsValidation($scope.objNPIFRecipients);
        if ($scope.isInvalid)
            return false;
        angular.forEach($scope.apqpNPIFDocuSign.lstNPIFDocuSignApprovers, function (obj) {
            if (!$scope.isInvalid && obj.UserId != '' && obj.UserId == $scope.objNPIFRecipients.UserId) {
                common.aaNotify.error(($filter('translate')('_UserAlreadyExists_')));
                $scope.isInvalid = true;
                return false;
            }
        });
        $scope.apqpNPIFDocuSign.lstNPIFDocuSignApprovers.push($scope.objNPIFRecipients);

        angular.forEach($scope.apqpNPIFDocuSign.lstNPIFDocuSignApprovers, function (obj) {
            obj.APQPItemId = APQPItemIds;
            $scope.lstFilteredDesignation = $filter('filter')($scope.lstFilteredDesignation, function (row) { return row.Id != obj.DesignationId });

        });
        $scope.ResetNPIFRecipients();
        common.aaNotify.success(($filter('translate')('_NPIFRecipientAdded_')));
    };

    $scope.NPIFRecipientsValidation = function (item) {
        $scope.isInvalid = false;
        if (Isundefinedornull(item) || IsUndefinedNullOrEmpty(item.DesignationId)) {
            common.aaNotify.error(($filter('translate')('_DesignationIsReq_')));
            $scope.isInvalid = true;
            return false;
        }
        if (Isundefinedornull(item) || IsUndefinedNullOrEmpty(item.UserId)) {
            common.aaNotify.error(($filter('translate')('_UserIsReq_')));
            $scope.isInvalid = true;
            return false;
        }

        if (!$scope.isInvalid)
            return true;
    };
    $scope.ResetNPIFRecipients = function () {
        $scope.objNPIFRecipients = {};
        $scope.objNPIFRecipients.RoutingOrder = 1;
        $scope.objNPIFRecipients.DesignationId = $scope.lstFilteredDesignation[0].Id;
        $scope.getUsers($scope.objNPIFRecipients.DesignationId, $scope.objNPIFRecipients);
    };
    $scope.DeleteNPIFRecipients = function (index) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            angular.forEach($scope.apqpNPIFDocuSign.lstNPIFDocuSignApprovers, function (obj, i) {
                if (index == i) {
                    $scope.lstFilteredDesignation.push($filter('filter')($scope.lstDesignation, function (row) { return row.Id == obj.DesignationId })[0]);
                }
            });

            $scope.apqpNPIFDocuSign.lstNPIFDocuSignApprovers.splice(index, 1);
            $scope.ResetNPIFRecipients();
            common.aaNotify.success(($filter('translate')('_RecipientDeleteSuccessMessage_')));
        }
    };
    $scope.SaveApprovers = function () {
        common.usSpinnerService.spin('spnAPQP');
        var doExit = false;
        if ($scope.apqpNPIFDocuSign.lstNPIFDocuSignApprovers.length < 1) {
            common.aaNotify.error(($filter('translate')('_AtleastOneApproverReq_')));
            common.usSpinnerService.stop('spnAPQP');
            return false;
        }

        angular.forEach($scope.apqpNPIFDocuSign.lstNPIFDocuSignApprovers, function (obj) {
            if (!doExit) {
                $scope.NPIFRecipientsValidation(obj);
                if ($scope.isInvalid) {
                    doExit = true;
                }
            }
        });
        if ($scope.isInvalid) {
            common.usSpinnerService.stop('spnAPQP');
            return false;

        }

        APQPSvc.SendNPIF($scope.apqpNPIFDocuSign).then(
          function (response) {
              if (response.data.StatusCode == 200) {
                  $scope.ClosePopup(response.data.SuccessMessage);
                  common.usSpinnerService.stop('spnAPQP');
                  common.aaNotify.success(($filter('translate')('_RecipientSentSuccessMessage_')));

              }
              else {
                  common.aaNotify.error(response.data.ErrorText);
                  common.usSpinnerService.stop('spnAPQP');
              }
          },
function (error) {
    common.usSpinnerService.stop('spnAPQP');
    console.log(error);
});
    };
    $scope.ClosePopup = function (val) {
        $modalInstance.close(val);
        $scope.destroyScope();
    };
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
        $scope.destroyScope();
    };
    $scope.destroyScope = function () {
        var lclScope = $scope;
        lclScope.$destroy();
    };
    $scope.NPIFApprovalInit();
}

var APQPSendEmailCtrl = function ($scope, $modalInstance, $timeout, common, SectionName, APQPItemIds, APQPSvc) {
    $scope.APQPSendEmailInit = function () {
        $scope.EmailData.AttachDocument = true;
        $timeout(function () {
            common.usSpinnerService.stop('spnAPQP');
        }, 0);
        $scope.SectionName = SectionName;
    };
    $scope.SendEmail = function () {
        common.usSpinnerService.spin('spnAPQP');
        $scope.EmailData.APQPItemIds = APQPItemIds;
        var EmailIdsList = [];
        angular.forEach($scope.EmailData.SendToIds, function (item, index) {
            if (!IsUndefinedNullOrEmpty(item.Id))
                EmailIdsList.push(item.Id);
        });
        if (EmailIdsList.length <= 0) {
            common.usSpinnerService.stop('spnAPQP');
            common.aaNotify.error("Please select at least one send to user.");
            return false;
        }
        $scope.EmailData.EmailIdsList = EmailIdsList;

        APQPSvc.APQPSendEmail($scope.EmailData).then(
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
               ////common.aaNotify.error(error);
               console.log(error);
           });
    };
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    $scope.SetUploadEmailFile = function (UploadedFilePath, UploadedFileName) {
        $scope.EmailData.EmailAttachment = UploadedFilePath;
        $scope.EmailData.EmailFileName = UploadedFileName;
    };
    $scope.deleteEmailFile = function (UploadedFilePath) {
        $scope.EmailData.EmailAttachment = '';
        $scope.EmailData.EmailFileName = '';
    };
    $scope.APQPSendEmailInit();
}

var ViewChangeLogPageInstance = function ($scope, $modalInstance, $filter, common, AuditLogsSvc, APQPSvc, ChangeRequestSvc) {
    $scope.InitializeAuditLogObjects = function () {
        if (!IsUndefinedNullOrEmpty($scope.Id) && parseInt($scope.Id) > 0) {
            $scope.APQPChangeLogPaging = GetDefaultPageObject();
            $scope.APQPChangeLogCriteria = { ItemId: $scope.Id };
            $scope.APQPHeader = {};
            $scope.APQPChangeLogPaging.Criteria = $scope.APQPChangeLogCriteria;
            $scope.APQPChangeLogList = {};
            $scope.ChangeRequestLogList = {};
            $scope.getHeaderData($scope.Id);
            $scope.GetAPQPChangeLogs(true);
        }
        else
            $scope.ChangeLogCancel();
    }
    $scope.GetAPQPChangeLogs = function (loadAll) {
        common.usSpinnerService.spin('spnAPQP');
        AuditLogsSvc.GetAPQPChangeLogs($scope.APQPChangeLogPaging).then(
             function (response) {
                 common.usSpinnerService.stop('spnAPQP');
                 if (response.data.StatusCode == 200) {
                     $scope.APQPChangeLogList = response.data.Result;
                     $scope.APQPChangeLogPaging = response.data.PageInfo;
                     $scope.APQPChangeLogList = _.groupBy($scope.APQPChangeLogList, 'UpdatedOn');
                     $scope.IsAccordionObjectEmpty = IsObjectEmpty($scope.APQPChangeLogList);
                     if (loadAll)
                         $scope.GetChangeRequestLogs();
                 }
                 else {
                     $scope.IsAccordionObjectEmpty = true;
                 }
             },
             function (error) {
                 $scope.IsAccordionObjectEmpty = true;
                 common.usSpinnerService.stop('spnAPQP');
             });
    };
    $scope.GetChangeRequestLogs = function () {
        common.usSpinnerService.spin('spnAPQP');
        AuditLogsSvc.GetChangeRequestLogs($scope.APQPChangeLogPaging).then(
             function (response) {
                 common.usSpinnerService.stop('spnAPQP');
                 if (response.data.StatusCode == 200) {
                     $scope.ChangeRequestLogList = response.data.Result;
                     $scope.ChangeRequestLogList = _.groupBy($scope.ChangeRequestLogList, 'TimeStamp');
                     $scope.IsAccordionObjectEmptyCR = IsObjectEmpty($scope.ChangeRequestLogList);
                 }
                 else {
                     $scope.IsAccordionObjectEmptyCR = true;
                 }
             },
             function (error) {
                 $scope.IsAccordionObjectEmpty = true;
                 common.usSpinnerService.stop('spnAPQP');
             });
    };
    $scope.getHeaderData = function (apqpId) {
        common.usSpinnerService.spin('spnAPQP');
        APQPSvc.getGeneratePSWData(apqpId).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   $scope.APQPHeader = response.data.Result;
                   $scope.APQPHeader.ToolingKickoffDate = convertUTCDateToLocalDate($scope.APQPHeader.ToolingKickoffDate);
               }
               else {
                   common.usSpinnerService.stop('spnAPQP');
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnAPQP');
           });
    };
    //defect tracking
    $scope.APQPChangeLogPageSizeChanged = function (PageSize) {
        $scope.APQPChangeLogPaging.PageSize = PageSize;
        $scope.GetAPQPChangeLogs(false);
    };
    $scope.APQPChangeLogPageChanged = function (PageNo) {
        $scope.APQPChangeLogPaging.PageNo = PageNo;
        $scope.GetAPQPChangeLogs(false);
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

    $scope.DeleteDocumentFromHistory = function (documentId) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            common.usSpinnerService.spin('spnAPQP');
            ChangeRequestSvc.DeleteDocument(documentId).then(
              function (response) {
                  if (ShowMessage(common, response.data)) {
                      $scope.GetChangeRequestLogs();
                  }
                  else
                      common.usSpinnerService.stop('spnAPQP');
              },
              function (error) {
                  common.usSpinnerService.stop('spnAPQP');
              });
        }
    };

    $scope.InitializeAuditLogObjects();
};