app.controller('RFQSupplierQuotePopupCtrl', ['$rootScope', '$scope', 'common', 'SupplierQuoteSvc', '$timeout', '$filter', 'LookupSvc', '$window', '$routeParams', '$modalInstance', 'RFQId', 'SupplierId', 'RFQData',
    function ($rootScope, $scope, common, SupplierQuoteSvc, $timeout, $filter, LookupSvc, $window, $routeParams, $modalInstance, RFQId, SupplierId, RFQData) {
        //Start implement security role wise
        $scope.setRoleWisePrivilege = function () {
            $scope.currentSecurityObject = currentSecurityObject;
            if (!Isundefinedornull($scope.currentSecurityObject) && $scope.currentSecurityObject.length > 0) {
                angular.forEach($scope.currentSecurityObject, function (obj, index) {
                    switch (obj.ObjectId) {
                        case 24:
                            switch (obj.PrivilegeId) {
                                case 1:                           //none
                                    RedirectToAccessDenied();
                                    break;
                                case 2:                          //read only
                                    $scope.IsReadOnlySupplierQuotePage = true;
                                    break;
                                case 3:                         //write
                                    $scope.IsReadOnlySupplierQuotePage = false;
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

        $scope.IsCalledFromRFQ = false;
        if (!IsUndefinedNullOrEmpty(RFQId) && !IsUndefinedNullOrEmpty(SupplierId) && SupplierId != 'null' && parseInt(SupplierId) != 0) {
            $scope.IsCalledFromRFQ = true;
        }

        $scope.Manufacturer = [];
        $scope.RFQSupplierPartQuoteList = {};
        //initialize variables
        $scope.Init = function () {
            $scope.RFQData = RFQData;
            $scope.RFQData.Date = convertUTCDateToLocalDate($scope.RFQData.Date);
            $scope.RFQData.QuoteDueDate = convertUTCDateToLocalDate($scope.RFQData.QuoteDueDate);
            $scope.obj = { EnterQuote: true };
            $scope.NoQuote = false;
            $scope.IsForm = false;
            $scope.Paging = {};

            if ($scope.IsCalledFromRFQ) {
                $scope.SearchCriteria = { CustomerId: 0, RFQId: RFQId, SupplierId: parseInt(SupplierId), IsQuoteTypeDQ: false };
                $scope.Paging.Criteria = $scope.SearchCriteria;
            }
            else
                return false;

            $scope.SetLooksupData();
            setTimeout(function () {
                $scope.OnSupplierChange();
            }, 500);
        };
        $scope.setParameters = function () {
            $scope.obj.EnterQuote = true;
        };
        $scope.setOnChangeSupplierValues = function (supplierId) {
            SupplierId = supplierId;
            $scope.SearchCriteria = { CustomerId: 0, RFQId: RFQId, SupplierId: parseInt(SupplierId), IsQuoteTypeDQ: false };
            $scope.Paging.Criteria = $scope.SearchCriteria;
            $scope.OnSupplierChange();
        };

        $scope.OnManufacturerSelect = function ($item, item) {
            if (!IsNotNullorEmpty($item) || !IsNotNullorEmpty($item.Key)) {
                item.ManufacturerId = $item.Id;
                item.Manufacturer = $item.Name;
            }
        }

        //supplier parts list
        $scope.GetRFQSupplierPartQuoteList = function () {
            common.usSpinnerService.spin('spnRFQSupplierPartQuote');
            SupplierQuoteSvc.GetSupplierQuoteList($scope.Paging).then(
                 function (response) {
                     if (response.data.StatusCode == 200) {                  //ShowMessage(common, response.data)
                         $scope.RFQSupplierPartQuoteList = response.data.Result;
                         if ($scope.RFQSupplierPartQuoteList.length > 0) {
                             $scope.NoQuote = $scope.RFQSupplierPartQuoteList[0].NoQuote;
                             $scope.IsForm = true;
                         }
                         else {
                             $scope.IsForm = false;
                         }

                         $scope.$broadcast("loadScrollerPanel");
                     }
                     else {
                         $scope.IsForm = false;
                         console.log(response.data.ErrorText);
                     }
                     angular.forEach($scope.RFQSupplierPartQuoteList, function (item) {
                         item.UnitPrice = $filter('number')(item.UnitPrice, 3);
                         item.rFQdqRawMaterial.MaterialLoss = $filter('number')(IsUndefinedNullOrEmpty(item.rFQdqRawMaterial.MaterialLoss) ? 0 : item.rFQdqRawMaterial.MaterialLoss, 3);
                         item.rFQdqRawMaterial.RawMatTotal = $filter('number')(IsUndefinedNullOrEmpty(item.rFQdqRawMaterial.RawMatTotal) ? 0 : item.rFQdqRawMaterial.RawMatTotal, 3);
                         //$scope.RFQSupplierPartQuoteList[0].ExchangeRate = parseFloat(IsUndefinedNullOrEmpty($scope.RFQSupplierPartQuoteList[0].ExchangeRate) ? 0.000 : $scope.RFQSupplierPartQuoteList[0].ExchangeRate, 3);
                         if (item.IsQuoteTypeDQ) {
                             item.rFQdqPrimaryProcessConversion.ProcessConversionCostPerPart = $filter('number')(IsUndefinedNullOrEmpty(item.rFQdqPrimaryProcessConversion.ProcessConversionCostPerPart) ? 0 : item.rFQdqPrimaryProcessConversion.ProcessConversionCostPerPart, 3);
                             item.rFQdqMachining.MachiningCostPerPart = $filter('number')(IsUndefinedNullOrEmpty(item.rFQdqMachining.MachiningCostPerPart) ? 0 : item.rFQdqMachining.MachiningCostPerPart, 3);
                             item.rFQdqMachiningSecondaryOperation.SecondaryCostPerPart = $filter('number')(IsUndefinedNullOrEmpty(item.rFQdqMachiningSecondaryOperation.SecondaryCostPerPart) ? 0 : item.rFQdqMachiningSecondaryOperation.SecondaryCostPerPart, 3);
                             item.rFQdqMachiningOtherOperation.SecondaryCostPerPart = $filter('number')(IsUndefinedNullOrEmpty(item.rFQdqMachiningOtherOperation.SecondaryCostPerPart) ? 0 : item.rFQdqMachiningOtherOperation.SecondaryCostPerPart, 3);
                             item.rFQdqSurfaceTreatment.CoatingCostPerHour = $filter('number')(IsUndefinedNullOrEmpty(item.rFQdqSurfaceTreatment.CoatingCostPerHour) ? 0 : item.rFQdqSurfaceTreatment.CoatingCostPerHour, 3);
                         }
                     });
                     common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                     //$scope.$evalAsync(function () {  });
                     //$timeout(common.usSpinnerService.stop('spnRFQSupplierPartQuote'), parseInt(parseInt($scope.RFQSupplierPartQuoteList.length / 3) + "000"));
                 },
                 function (error) {
                     $scope.IsForm = false;
                     common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                 });
        };

        //fill default lookups here
        $scope.SetLooksupData = function () {
            $scope.LookUps = [
              {
                  "Name": "SupplierForSupplierQuote", "Parameters": { "RFQId": $scope.SearchCriteria.RFQId }
              },
               {
                   "Name": "MachineDesc", "Parameters": {}
               },
              {
                  "Name": "MachiningDesc", "Parameters": {}
              },
              {
                  "Name": "MachiningSecOperation", "Parameters": {}
              },
              {
                  "Name": "CoatingTypes", "Parameters": {}
              }
            ];
            $scope.getLookupData();
        };
        $scope.getLookupData = function () {
            common.usSpinnerService.spin('spnRFQSupplierPartQuote');
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                //common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                angular.forEach(data.data, function (o) {
                    if (o.Name === "SupplierForSupplierQuote") {
                        $scope.SupplierForSupplierQuoteList = o.Data;
                    }
                    else if (o.Name === "MachineDesc") {
                        $scope.MachineDescList = o.Data;
                    }
                    else if (o.Name === "MachiningDesc") {
                        $scope.MachiningDescList = o.Data;
                    }
                    else if (o.Name === "MachiningSecOperation") {
                        $scope.MachiningSecOperationList = o.Data;
                    }
                    else if (o.Name === "CoatingTypes") {
                        $scope.CoatingTypesList = o.Data;
                    }
                });
            }, function (error) {
                common.usSpinnerService.stop('spnRFQSupplierPartQuote');
            });
        };

        ///Start set search parameters according to selection
        $scope.OnSupplierChange = function () {
            $scope.Paging.Criteria = $scope.SearchCriteria;
            $scope.setParameters();
            $scope.GetRFQSupplierPartQuoteList();
        }
        ////END set search parameters according to selection

        //Start Calculations and validations
        $scope.CalculateUnitPrice = function (item, key) {
            angular.forEach($scope.RFQSupplierPartQuoteList, function (obj, index) {
                if (index == key) {
                    if (!item.IsQuoteTypeDQ) {
                        var prpUnitPrice = 0;
                        var RawMatInputInKg = 0, RawMatCostPerKg = 0, MaterialLoss = 0, ttlRawMatTotal = 0;

                        //calculate RawMaterial values
                        if (!IsUndefinedNullOrEmpty(item.rFQdqRawMaterial.RawMatInputInKg)
                        && !isNaN(parseFloat(item.rFQdqRawMaterial.RawMatInputInKg)) && parseFloat(item.rFQdqRawMaterial.RawMatInputInKg) > 0) {
                            if (!isNaN(parseFloat(item.PartWeightKG)) && parseFloat(item.PartWeightKG) > 0) {
                                MaterialLoss = (parseFloat(item.rFQdqRawMaterial.RawMatInputInKg) - parseFloat(item.PartWeightKG)) / parseFloat(item.rFQdqRawMaterial.RawMatInputInKg);
                            }
                            else {
                                MaterialLoss = parseFloat(item.rFQdqRawMaterial.RawMatInputInKg) / parseFloat(item.rFQdqRawMaterial.RawMatInputInKg);
                            }
                            RawMatInputInKg = parseFloat(item.rFQdqRawMaterial.RawMatInputInKg);
                        }
                        item.rFQdqRawMaterial.MaterialLoss = $filter('number')(MaterialLoss, 3);

                        if (!isNaN(parseFloat(item.rFQdqRawMaterial.RawMatCostPerKg))) {
                            RawMatCostPerKg = parseFloat(item.rFQdqRawMaterial.RawMatCostPerKg);
                        }
                        ttlRawMatTotal = RawMatInputInKg * RawMatCostPerKg;
                        item.rFQdqRawMaterial.RawMatTotal = $filter('number')(ttlRawMatTotal, 3);


                        if (!IsUndefinedNullOrEmpty(item.ProcessCost) && !isNaN(item.ProcessCost))
                            prpUnitPrice = prpUnitPrice + parseFloat(item.ProcessCost);
                        if (!IsUndefinedNullOrEmpty(item.MachiningCost) && !isNaN(item.MachiningCost))
                            prpUnitPrice = prpUnitPrice + parseFloat(item.MachiningCost);
                        if (!IsUndefinedNullOrEmpty(item.OtherProcessCost) && !isNaN(item.OtherProcessCost))
                            prpUnitPrice = prpUnitPrice + parseFloat(item.OtherProcessCost);

                        obj.UnitPrice = $filter('number')(prpUnitPrice + ttlRawMatTotal, 3); //(item.MaterialCost) + (item.ProcessCost) + (item.MachiningCost) + (item.OtherProcessCost);
                        //obj.UnitPrice = $filter('setDecimal')(obj.UnitPrice, 3)
                        return;
                    }
                    else {
                        var prpUnitPrice = 0;
                        var RawMatInputInKg = 0, RawMatCostPerKg = 0, MaterialLoss = 0, ttlRawMatTotal = 0, ttlPrimaryProcess_Conversion = 0, ttlMachining = 0, ttlMachining_SecondaryOpr = 0, ttlMachining_OtherOpr = 0, ttlSurfaceTreatment = 0, ttlOverhead = 0;
                        //calculate RawMaterial values
                        if (!IsUndefinedNullOrEmpty(item.rFQdqRawMaterial.RawMatInputInKg)
                        && !isNaN(parseFloat(item.rFQdqRawMaterial.RawMatInputInKg)) && parseFloat(item.rFQdqRawMaterial.RawMatInputInKg) > 0) {
                            if (!isNaN(parseFloat(item.PartWeightKG)) && parseFloat(item.PartWeightKG) > 0) {
                                MaterialLoss = (parseFloat(item.rFQdqRawMaterial.RawMatInputInKg) - parseFloat(item.PartWeightKG)) / parseFloat(item.rFQdqRawMaterial.RawMatInputInKg);
                            }
                            else {
                                MaterialLoss = parseFloat(item.rFQdqRawMaterial.RawMatInputInKg) / parseFloat(item.rFQdqRawMaterial.RawMatInputInKg);
                            }
                            RawMatInputInKg = parseFloat(item.rFQdqRawMaterial.RawMatInputInKg);
                        }
                        item.rFQdqRawMaterial.MaterialLoss = $filter('number')(MaterialLoss, 3);

                        if (!isNaN(parseFloat(item.rFQdqRawMaterial.RawMatCostPerKg))) {
                            RawMatCostPerKg = parseFloat(item.rFQdqRawMaterial.RawMatCostPerKg);
                        }
                        ttlRawMatTotal = RawMatInputInKg * RawMatCostPerKg;
                        item.rFQdqRawMaterial.RawMatTotal = $filter('number')(ttlRawMatTotal, 3);

                        //calculate PrimaryProcess/Conversion values
                        if (!IsUndefinedNullOrEmpty(item.rFQdqPrimaryProcessConversion.CycleTime)
                            && !isNaN(item.rFQdqPrimaryProcessConversion.CycleTime)
                            && !IsUndefinedNullOrEmpty(item.rFQdqPrimaryProcessConversion.ManPlusMachineRatePerHour)
                            && !isNaN(item.rFQdqPrimaryProcessConversion.ManPlusMachineRatePerHour)) {
                            ttlPrimaryProcess_Conversion = (parseFloat(item.rFQdqPrimaryProcessConversion.CycleTime) * parseFloat(item.rFQdqPrimaryProcessConversion.ManPlusMachineRatePerHour)) / 3600;
                        }
                        item.rFQdqPrimaryProcessConversion.ProcessConversionCostPerPart = $filter('number')(ttlPrimaryProcess_Conversion, 3);

                        //calculate Machining values
                        if (!IsUndefinedNullOrEmpty(item.rFQdqMachining.CycleTime)
                        && !isNaN(item.rFQdqMachining.CycleTime)
                        && !IsUndefinedNullOrEmpty(item.rFQdqMachining.ManPlusMachineRatePerHour)
                        && !isNaN(item.rFQdqMachining.ManPlusMachineRatePerHour)) {
                            ttlMachining = (parseFloat(item.rFQdqMachining.CycleTime) * parseFloat(item.rFQdqMachining.ManPlusMachineRatePerHour)) / 3600;
                        }
                        item.rFQdqMachining.MachiningCostPerPart = $filter('number')(ttlMachining, 3);

                        //calculate Machining/SecondaryOpr values
                        if (!IsUndefinedNullOrEmpty(item.rFQdqMachiningSecondaryOperation.CycleTime)
                            && !isNaN(item.rFQdqMachiningSecondaryOperation.CycleTime)
                            && !IsUndefinedNullOrEmpty(item.rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour)
                            && !isNaN(item.rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour)) {
                            ttlMachining_SecondaryOpr = (parseFloat(item.rFQdqMachiningSecondaryOperation.CycleTime) * parseFloat(item.rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour)) / 3600;
                        }
                        item.rFQdqMachiningSecondaryOperation.SecondaryCostPerPart = $filter('number')(ttlMachining_SecondaryOpr, 3);

                        //calculate Machining/OtherOpr values
                        if (!IsUndefinedNullOrEmpty(item.rFQdqMachiningOtherOperation.CycleTime)
                            && !isNaN(item.rFQdqMachiningOtherOperation.CycleTime)
                            && !IsUndefinedNullOrEmpty(item.rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour)
                            && !isNaN(item.rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour)) {
                            ttlMachining_OtherOpr = (parseFloat(item.rFQdqMachiningOtherOperation.CycleTime) * parseFloat(item.rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour)) / 3600;
                        }
                        item.rFQdqMachiningOtherOperation.SecondaryCostPerPart = $filter('number')(ttlMachining_OtherOpr, 3);

                        //calculate SurfaceTreatment values
                        if (!IsUndefinedNullOrEmpty(item.rFQdqSurfaceTreatment.PartsPerHour)
                            && !isNaN(item.rFQdqSurfaceTreatment.PartsPerHour)
                            && !IsUndefinedNullOrEmpty(item.rFQdqSurfaceTreatment.ManPlusMachineRatePerHour)
                            && !isNaN(item.rFQdqSurfaceTreatment.ManPlusMachineRatePerHour) && parseFloat(item.rFQdqSurfaceTreatment.PartsPerHour) > 0) {
                            ttlSurfaceTreatment = parseFloat(item.rFQdqSurfaceTreatment.ManPlusMachineRatePerHour) / parseFloat(item.rFQdqSurfaceTreatment.PartsPerHour);
                        }
                        item.rFQdqSurfaceTreatment.CoatingCostPerHour = $filter('number')(ttlSurfaceTreatment, 3);

                        //calculate Overhead values
                        if (!IsUndefinedNullOrEmpty(item.rFQdqOverhead.InventoryCarryingCost)
                            && !isNaN(item.rFQdqOverhead.InventoryCarryingCost)) {
                            ttlOverhead = ttlOverhead + parseFloat(item.rFQdqOverhead.InventoryCarryingCost);
                        }
                        if (!IsUndefinedNullOrEmpty(item.rFQdqOverhead.PackagingMaterial)
                            && !isNaN(item.rFQdqOverhead.PackagingMaterial)) {
                            ttlOverhead = ttlOverhead + parseFloat(item.rFQdqOverhead.PackagingMaterial);
                        }
                        if (!IsUndefinedNullOrEmpty(item.rFQdqOverhead.Packing)
                            && !isNaN(item.rFQdqOverhead.Packing)) {
                            ttlOverhead = ttlOverhead + parseFloat(item.rFQdqOverhead.Packing);
                        }
                        if (!IsUndefinedNullOrEmpty(item.rFQdqOverhead.LocalFreightToPort)
                            && !isNaN(item.rFQdqOverhead.LocalFreightToPort)) {
                            ttlOverhead = ttlOverhead + parseFloat(item.rFQdqOverhead.LocalFreightToPort);
                        }
                        if (!IsUndefinedNullOrEmpty(item.rFQdqOverhead.ProfitAndSGA)
                            && !isNaN(item.rFQdqOverhead.ProfitAndSGA)) {
                            ttlOverhead = ttlOverhead + parseFloat(item.rFQdqOverhead.ProfitAndSGA);
                        }

                        //Calculate UnitPrice
                        prpUnitPrice = ttlRawMatTotal + ttlPrimaryProcess_Conversion
                                + ttlMachining + ttlMachining_SecondaryOpr
                                + ttlMachining_OtherOpr + ttlSurfaceTreatment + ttlOverhead;
                        item.UnitPrice = $filter('number')(prpUnitPrice, 3);

                        //Calculate OverheadPercentPiecePrice  on the unit price
                        item.rFQdqOverhead.OverheadPercentPiecePrice = $filter('number')(((ttlOverhead * 100) / prpUnitPrice), 3);

                        return;
                    }
                }
            });
        };
        $scope.setToolingCost = function (item) {
            item.ToolingCost = $filter('number')(item.ToolingCost, 3);
        };
        $scope.setRawMaterialPriceAssumed = function (item) {
            //set   Raw Material Price Assumed value as it is as item.rFQdqRawMaterial.RawMatTotal(before for SQ it was item.MaterialCost but now same for SQ & DQ) if there is only one part in the list.
            //if ($scope.RFQSupplierPartQuoteList.length == 1 && !item.IsQuoteTypeDQ)
            //    $scope.RFQSupplierPartQuoteList[0].RawMaterialPriceAssumed = item.MaterialCost;
            if ($scope.RFQSupplierPartQuoteList.length == 1)
                $scope.RFQSupplierPartQuoteList[0].RawMaterialPriceAssumed = item.rFQdqRawMaterial.RawMatTotal;
        };
        //End Calculations and validations
        //$scope.selectionChanged = function (val) {
        //    console.log(val);
        //};
        $scope.SaveRFQSupplierPartQuote = function (closeForm) {
            common.usSpinnerService.spin('spnRFQSupplierPartQuote');
            $scope.isValid = true;
            $scope.ValidationSuccess();
            setTimeout(function () {
                if (!$scope.isValid)
                    return false;
                if (!IsObjectEmpty($scope.RFQSupplierPartQuoteList)) {
                    SupplierQuoteSvc.SaveSupplierQuoteList($scope.RFQSupplierPartQuoteList).then(
                   function (response) {
                       common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                       if (ShowMessage(common, response.data)) {
                           $scope.IsForm = true;
                           if (closeForm)
                               $scope.BackToRFQ();
                       }
                   },
                   function (error) {
                       common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                       //common.aaNotify.error(error);
                   });
                }
                else {
                    common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                }
            }, 1500);
        };
        $scope.ValidationSuccess = function () {
            angular.forEach($scope.RFQSupplierPartQuoteList, function (item, index) {
                ////check simplified quote validation
                if (!item.IsQuoteTypeDQ && $scope.isValid) {
                    if ((!isNaN(parseFloat(item.MinOrderQty)) && parseFloat(item.MinOrderQty) > 0)
                        || (!isNaN(parseFloat(item.ToolingCost)) && parseFloat(item.ToolingCost) > 0)
                        || (!isNaN(parseFloat(item.rFQdqRawMaterial.RawMatInputInKg)) && parseFloat(item.rFQdqRawMaterial.RawMatInputInKg) > 0)
                        || (!isNaN(parseFloat(item.rFQdqRawMaterial.RawMatCostPerKg)) && parseFloat(item.rFQdqRawMaterial.RawMatCostPerKg) > 0)
                        || (!isNaN(parseFloat(item.ProcessCost)) && parseFloat(item.ProcessCost) > 0)
                        || (!isNaN(parseFloat(item.MachiningCost)) && parseFloat(item.MachiningCost) > 0)
                        || (!isNaN(parseFloat(item.OtherProcessCost)) && parseFloat(item.OtherProcessCost) > 0)
                        || (!isNaN(parseFloat(item.NoOfCavities)) && parseFloat(item.NoOfCavities) > 0)) {

                        if (isNaN(parseFloat(item.MinOrderQty))
                             || isNaN(parseFloat(item.rFQdqRawMaterial.RawMatInputInKg)) || parseFloat(item.rFQdqRawMaterial.RawMatInputInKg) <= 0
                             || isNaN(parseFloat(item.rFQdqRawMaterial.RawMatCostPerKg)) || parseFloat(item.rFQdqRawMaterial.RawMatCostPerKg) <= 0
                             || isNaN(parseFloat(item.ProcessCost)) || parseFloat(item.ProcessCost) <= 0
                             || isNaN(parseFloat(item.NoOfCavities)) || parseFloat(item.NoOfCavities) <= 0) {
                            common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                            common.aaNotify.error($filter('translate')('_IsRequiredErrMsg_'));
                            $scope.isValid = false;
                            return false;
                        }
                    }
                }
                ////check Detail quote validation
                if (item.IsQuoteTypeDQ && $scope.isValid) {
                    if ((!isNaN(parseFloat(item.MinOrderQty)) && parseFloat(item.MinOrderQty) > 0)
                        || (!isNaN(parseFloat(item.ToolingCost)) && parseFloat(item.ToolingCost) > 0)
                        || (!isNaN(parseFloat(item.NoOfCavities)) && parseFloat(item.NoOfCavities) > 0)
                        || (!isNaN(parseFloat(item.rFQdqRawMaterial.RawMatInputInKg)) && parseFloat(item.rFQdqRawMaterial.RawMatInputInKg) > 0)
                        || (!isNaN(parseFloat(item.rFQdqRawMaterial.RawMatCostPerKg)) && parseFloat(item.rFQdqRawMaterial.RawMatCostPerKg) > 0)
                        || (!isNaN(parseFloat(item.rFQdqPrimaryProcessConversion.MachineDescId)) && parseFloat(item.rFQdqPrimaryProcessConversion.MachineDescId) > 0)
                        || (!isNaN(parseFloat(item.rFQdqPrimaryProcessConversion.MachineSize)) && parseFloat(item.rFQdqPrimaryProcessConversion.MachineSize) > 0)
                        || (!isNaN(parseFloat(item.rFQdqPrimaryProcessConversion.CycleTime)) && parseFloat(item.rFQdqPrimaryProcessConversion.CycleTime) > 0)
                        || (!isNaN(parseFloat(item.rFQdqPrimaryProcessConversion.ManPlusMachineRatePerHour)) && parseFloat(item.rFQdqPrimaryProcessConversion.ManPlusMachineRatePerHour) > 0)
                        || (!isNaN(parseFloat(item.rFQdqMachining.MachiningDescId)) && parseFloat(item.rFQdqMachining.MachiningDescId) > 0)
                        || (!isNaN(parseFloat(item.rFQdqMachining.CycleTime)) && parseFloat(item.rFQdqMachining.CycleTime) > 0)
                        || (!isNaN(parseFloat(item.rFQdqMachining.ManPlusMachineRatePerHour)) && parseFloat(item.rFQdqMachining.ManPlusMachineRatePerHour) > 0)
                        || (!isNaN(parseFloat(item.rFQdqMachiningSecondaryOperation.SecondaryOperationDescId)) && parseFloat(item.rFQdqMachiningSecondaryOperation.SecondaryOperationDescId) > 0)
                        || (!isNaN(parseFloat(item.rFQdqMachiningSecondaryOperation.CycleTime)) && parseFloat(item.rFQdqMachiningSecondaryOperation.CycleTime) > 0)
                        || (!isNaN(parseFloat(item.rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour)) && parseFloat(item.rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour) > 0)
                        || (!isNaN(parseFloat(item.rFQdqMachiningOtherOperation.SecondaryOperationDescId)) && parseFloat(item.rFQdqMachiningOtherOperation.SecondaryOperationDescId) > 0)
                        || (!isNaN(parseFloat(item.rFQdqMachiningOtherOperation.CycleTime)) && parseFloat(item.rFQdqMachiningOtherOperation.CycleTime) > 0)
                        || (!isNaN(parseFloat(item.rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour)) && parseFloat(item.rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour) > 0)
                        || (!isNaN(parseFloat(item.rFQdqSurfaceTreatment.CoatingTypeId)) && parseFloat(item.rFQdqSurfaceTreatment.CoatingTypeId) > 0)
                        || (!isNaN(parseFloat(item.rFQdqSurfaceTreatment.PartsPerHour)) && parseFloat(item.rFQdqSurfaceTreatment.PartsPerHour) > 0)
                        || (!isNaN(parseFloat(item.rFQdqSurfaceTreatment.ManPlusMachineRatePerHour)) && parseFloat(item.rFQdqSurfaceTreatment.ManPlusMachineRatePerHour) > 0)
                        || (!isNaN(parseFloat(item.rFQdqOverhead.InventoryCarryingCost)) && parseFloat(item.rFQdqOverhead.InventoryCarryingCost) > 0)
                        || (!isNaN(parseFloat(item.rFQdqOverhead.PackagingMaterial)) && parseFloat(item.rFQdqOverhead.PackagingMaterial) > 0)
                        || (!isNaN(parseFloat(item.rFQdqOverhead.Packing)) && parseFloat(item.rFQdqOverhead.Packing) > 0)
                        || (!isNaN(parseFloat(item.rFQdqOverhead.LocalFreightToPort)) && parseFloat(item.rFQdqOverhead.LocalFreightToPort) > 0)
                        || (!isNaN(parseFloat(item.rFQdqOverhead.ProfitAndSGA)) && parseFloat(item.rFQdqOverhead.ProfitAndSGA) > 0)
                        ) {

                        if (isNaN(parseFloat(item.MinOrderQty))
                             || isNaN(parseFloat(item.NoOfCavities)) || parseFloat(item.NoOfCavities) <= 0
                             || isNaN(parseFloat(item.rFQdqRawMaterial.RawMatInputInKg)) || parseFloat(item.rFQdqRawMaterial.RawMatInputInKg) <= 0
                             || isNaN(parseFloat(item.rFQdqRawMaterial.RawMatCostPerKg)) || parseFloat(item.rFQdqRawMaterial.RawMatCostPerKg) <= 0
                             || isNaN(parseFloat(item.rFQdqPrimaryProcessConversion.MachineDescId)) || parseFloat(item.rFQdqPrimaryProcessConversion.MachineDescId) <= 0
                             || isNaN(parseFloat(item.rFQdqPrimaryProcessConversion.CycleTime)) || parseFloat(item.rFQdqPrimaryProcessConversion.CycleTime) <= 0
                             || isNaN(parseFloat(item.rFQdqPrimaryProcessConversion.ManPlusMachineRatePerHour)) || parseFloat(item.rFQdqPrimaryProcessConversion.ManPlusMachineRatePerHour) <= 0
                            ) {
                            common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                            common.aaNotify.error($filter('translate')('_IsRequiredErrMsg_')); //common.aaNotify.error("Please enter valid value for the required field(s).");
                            $scope.isValid = false;
                            return false;
                        }

                        //Machining
                        if ((!isNaN(parseFloat(item.rFQdqMachining.CycleTime)) && parseFloat(item.rFQdqMachining.CycleTime) > 0)) {
                            if ((isNaN(parseFloat(item.rFQdqMachining.ManPlusMachineRatePerHour)) || parseFloat(item.rFQdqMachining.ManPlusMachineRatePerHour) <= 0)) {
                                common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                                common.aaNotify.error($filter('translate')('_IsRequiredErrMsg_'));
                                $scope.isValid = false;
                                return false;
                            }
                        }
                        else if ((!isNaN(parseFloat(item.rFQdqMachining.ManPlusMachineRatePerHour)) && parseFloat(item.rFQdqMachining.ManPlusMachineRatePerHour) > 0)) {
                            if ((isNaN(parseFloat(item.rFQdqMachining.CycleTime)) || parseFloat(item.rFQdqMachining.CycleTime) <= 0)) {
                                common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                                common.aaNotify.error($filter('translate')('_IsRequiredErrMsg_'));
                                $scope.isValid = false;
                                return false;
                            }
                        }

                        //Machining 2/Secondary Opr.
                        if ((!isNaN(parseFloat(item.rFQdqMachiningSecondaryOperation.CycleTime)) && parseFloat(item.rFQdqMachiningSecondaryOperation.CycleTime) > 0)) {
                            if ((isNaN(parseFloat(item.rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour)) || parseFloat(item.rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour) <= 0)) {
                                common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                                common.aaNotify.error($filter('translate')('_IsRequiredErrMsg_'));
                                $scope.isValid = false;
                                return false;
                            }
                        }
                        else if ((!isNaN(parseFloat(item.rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour)) && parseFloat(item.rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour) > 0)) {
                            if ((isNaN(parseFloat(item.rFQdqMachiningSecondaryOperation.CycleTime)) || parseFloat(item.rFQdqMachiningSecondaryOperation.CycleTime) <= 0)) {
                                common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                                common.aaNotify.error($filter('translate')('_IsRequiredErrMsg_'));
                                $scope.isValid = false;
                                return false;
                            }
                        }

                        //Machining 3/Other Opr.
                        if ((!isNaN(parseFloat(item.rFQdqMachiningOtherOperation.CycleTime)) && parseFloat(item.rFQdqMachiningOtherOperation.CycleTime) > 0)) {
                            if ((isNaN(parseFloat(item.rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour)) || parseFloat(item.rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour) <= 0)) {
                                common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                                common.aaNotify.error($filter('translate')('_IsRequiredErrMsg_'));
                                $scope.isValid = false;
                                return false;
                            }
                        }
                        else if ((!isNaN(parseFloat(item.rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour)) && parseFloat(item.rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour) > 0)) {
                            if ((isNaN(parseFloat(item.rFQdqMachiningOtherOperation.CycleTime)) || parseFloat(item.rFQdqMachiningOtherOperation.CycleTime) <= 0)) {
                                common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                                common.aaNotify.error($filter('translate')('_IsRequiredErrMsg_'));
                                $scope.isValid = false;
                                return false;
                            }
                        }

                        //Surface Treatment
                        if ((!isNaN(parseFloat(item.rFQdqSurfaceTreatment.PartsPerHour)) && parseFloat(item.rFQdqSurfaceTreatment.PartsPerHour) > 0)) {
                            if ((isNaN(parseFloat(item.rFQdqSurfaceTreatment.ManPlusMachineRatePerHour)) || parseFloat(item.rFQdqSurfaceTreatment.ManPlusMachineRatePerHour) <= 0)) {
                                common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                                common.aaNotify.error($filter('translate')('_IsRequiredErrMsg_'));
                                $scope.isValid = false;
                                return false;
                            }
                        }
                        else if ((!isNaN(parseFloat(item.rFQdqSurfaceTreatment.ManPlusMachineRatePerHour)) && parseFloat(item.rFQdqSurfaceTreatment.ManPlusMachineRatePerHour) > 0)) {
                            if ((isNaN(parseFloat(item.rFQdqSurfaceTreatment.PartsPerHour)) || parseFloat(item.rFQdqSurfaceTreatment.PartsPerHour) <= 0)) {
                                common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                                common.aaNotify.error($filter('translate')('_IsRequiredErrMsg_'));
                                $scope.isValid = false;
                                return false;
                            }
                        }
                    }
                }
            });
        };
        $scope.exportToExcel = function () {
            common.usSpinnerService.spin('spnRFQSupplierPartQuote');
            if (!IsObjectEmpty($scope.RFQSupplierPartQuoteList)) {
                $scope.SearchCriteria.IsQuoteTypeDQ = $scope.RFQSupplierPartQuoteList[0].IsQuoteTypeDQ;
                $scope.Paging.Criteria = $scope.SearchCriteria;

                SupplierQuoteSvc.exportToExcelSupplierQuote($scope.Paging).then(
               function (response) {
                   common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                   if (response.data.StatusCode == 200) {
                       window.open(response.data.SuccessMessage, '_blank');
                   }
                   else {
                       common.aaNotify.error(response.data.ErrorText);
                   }
               },
               function (error) {
                   common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                   //common.aaNotify.error(error);
               });
            }
            else {
                common.usSpinnerService.stop('spnRFQSupplierPartQuote');
            }
        };

        $scope.SetObjectvalues = function (file, name, type) {
            if (type == 'F')
                $scope.RFQSupplierPartQuoteList[0].QuoteAttachmentFilePath = file;
            else if (type == 'Q')
                $scope.RFQSupplierPartQuoteList[0].UploadQuoteFilePath = file;
        }
        $scope.deleteFile = function (filePath) {
            $scope.RFQSupplierPartQuoteList[0].QuoteAttachmentFilePath = '';
        }

        $scope.DownloadQuoteFile = function () {
            common.usSpinnerService.spin('spnRFQSupplierPartQuote');
            if (!IsUndefinedNullOrEmpty($scope.SearchCriteria.SupplierId)) {
                SupplierQuoteSvc.downloadRfqSupplierPartQuote($scope.Paging).then(
                function (response) {
                    common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                    if (response.data.StatusCode == 200) {
                        window.open(response.data.SuccessMessage, '_blank');
                    }
                    else {
                        common.aaNotify.error(response.data.ErrorText);
                    }
                },
                function (error) {
                    common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                });
            }
            else {
                common.usSpinnerService.stop('spnRFQSupplierPartQuote');
            }
        };

        $scope.UploadQuoteFile = function () {
            common.usSpinnerService.spin('spnRFQSupplierPartQuote');
            if (IsUndefinedNullOrEmpty($scope.RFQSupplierPartQuoteList[0].UploadQuoteFilePath) || ($scope.RFQSupplierPartQuoteList[0].UploadQuoteFilePath == '')) {
                common.aaNotify.error($filter('translate')('_SelectFile_'));
                common.usSpinnerService.stop('spnRFQSupplierPartQuote');
            }
            else {
                $scope.SearchCriteria.UploadQuoteFilePath = $scope.RFQSupplierPartQuoteList[0].UploadQuoteFilePath;
                $scope.Paging.Criteria = $scope.SearchCriteria;
                SupplierQuoteSvc.uploadRfqSupplierPartQuote($scope.Paging).then(
                     function (response) {
                         common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                         if (response.data.StatusCode == 200) {
                             $scope.RFQSupplierPartQuoteList = response.data.Result;
                             angular.forEach($scope.RFQSupplierPartQuoteList, function (item) {
                                 item.UnitPrice = $filter('number')(item.UnitPrice, 3);
                                 item.rFQdqRawMaterial.MaterialLoss = $filter('number')(IsUndefinedNullOrEmpty(item.rFQdqRawMaterial.MaterialLoss) ? 0 : item.rFQdqRawMaterial.MaterialLoss, 3);
                                 item.rFQdqRawMaterial.RawMatTotal = $filter('number')(IsUndefinedNullOrEmpty(item.rFQdqRawMaterial.RawMatTotal) ? 0 : item.rFQdqRawMaterial.RawMatTotal, 3);
                                 //$scope.RFQSupplierPartQuoteList[0].ExchangeRate = parseFloat(IsUndefinedNullOrEmpty($scope.RFQSupplierPartQuoteList[0].ExchangeRate) ? 0 : $scope.RFQSupplierPartQuoteList[0].ExchangeRate, 3);
                                 if (item.IsQuoteTypeDQ) {
                                     item.rFQdqPrimaryProcessConversion.ProcessConversionCostPerPart = $filter('number')(IsUndefinedNullOrEmpty(item.rFQdqPrimaryProcessConversion.ProcessConversionCostPerPart) ? 0 : item.rFQdqPrimaryProcessConversion.ProcessConversionCostPerPart, 3);
                                     item.rFQdqMachining.MachiningCostPerPart = $filter('number')(IsUndefinedNullOrEmpty(item.rFQdqMachining.MachiningCostPerPart) ? 0 : item.rFQdqMachining.MachiningCostPerPart, 3);
                                     item.rFQdqMachiningSecondaryOperation.SecondaryCostPerPart = $filter('number')(IsUndefinedNullOrEmpty(item.rFQdqMachiningSecondaryOperation.SecondaryCostPerPart) ? 0 : item.rFQdqMachiningSecondaryOperation.SecondaryCostPerPart, 3);
                                     item.rFQdqMachiningOtherOperation.SecondaryCostPerPart = $filter('number')(IsUndefinedNullOrEmpty(item.rFQdqMachiningOtherOperation.SecondaryCostPerPart) ? 0 : item.rFQdqMachiningOtherOperation.SecondaryCostPerPart, 3);
                                     item.rFQdqSurfaceTreatment.CoatingCostPerHour = $filter('number')(IsUndefinedNullOrEmpty(item.rFQdqSurfaceTreatment.CoatingCostPerHour) ? 0 : item.rFQdqSurfaceTreatment.CoatingCostPerHour, 3);
                                 }
                             });
                             $scope.SearchCriteria.UploadPartFilePath = '';
                             $timeout(function () {
                                 $scope.obj.EnterQuote = true;
                             }, 0);
                         }
                         else {
                             console.log(response.data.ErrorText);
                         }
                     },
             function (error) {
                 common.usSpinnerService.stop('spnRFQSupplierPartQuote');
             });
            }
        };

        $scope.ResetForm = function () {
            common.$route.reload();
        }

        $scope.BackToRFQ = function () {
            $modalInstance.dismiss('cancel');
        };

        $scope.showHideSidePanel = function () {
            $(".left-pop-up-panel").toggleClass('widthZero');
            $(".right-pop-up-panel").toggleClass('widthFull');
        };

        $scope.Init();

    }]);

app.directive("changeBackgroundColorSimple", function () {
    return {
        replace: false,
        restrict: 'A',
        link: function (scope, $element, $attrs) {
            var e = $element;
            var index = parseInt($attrs.changeBackgroundColorSimple, 10);
            e.on('blur', function () {
                scope.setInvalidClass(scope.item, index);
            });
            scope.setInvalidClass = function (item, index) {
                if ((!isNaN(parseFloat(item.MinOrderQty)) && parseFloat(item.MinOrderQty) > 0)
                            || (!isNaN(parseFloat(item.ToolingCost)) && parseFloat(item.ToolingCost) > 0)
                            || (!isNaN(parseFloat(item.rFQdqRawMaterial.RawMatInputInKg)) && parseFloat(item.rFQdqRawMaterial.RawMatInputInKg) > 0)
                            || (!isNaN(parseFloat(item.rFQdqRawMaterial.RawMatCostPerKg)) && parseFloat(item.rFQdqRawMaterial.RawMatCostPerKg) > 0)
                            || (!isNaN(parseFloat(item.ProcessCost)) && parseFloat(item.ProcessCost) > 0)
                            || (!isNaN(parseFloat(item.MachiningCost)) && parseFloat(item.MachiningCost) > 0)
                            || (!isNaN(parseFloat(item.OtherProcessCost)) && parseFloat(item.OtherProcessCost) > 0)
                            || (!isNaN(parseFloat(item.NoOfCavities)) && parseFloat(item.NoOfCavities) > 0)) {
                    if (isNaN(parseFloat(item.MinOrderQty))) {
                        if (!$('#MinOrderQty_' + index).hasClass('invalid-class'))
                            $('#MinOrderQty_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#MinOrderQty_' + index).removeClass('invalid-class');
                    }
                    if (isNaN(parseFloat(item.rFQdqRawMaterial.RawMatInputInKg)) || parseFloat(item.rFQdqRawMaterial.RawMatInputInKg) <= 0) {
                        if (!$('#RawMatInputInKg_' + index).hasClass('invalid-class'))
                            $('#RawMatInputInKg_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#RawMatInputInKg_' + index).removeClass('invalid-class');
                    }
                    if (isNaN(parseFloat(item.rFQdqRawMaterial.RawMatCostPerKg)) || parseFloat(item.rFQdqRawMaterial.RawMatCostPerKg) <= 0) {
                        if (!$('#RawMatCostPerKg_' + index).hasClass('invalid-class'))
                            $('#RawMatCostPerKg_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#RawMatCostPerKg_' + index).removeClass('invalid-class');
                    }
                    if (isNaN(parseFloat(item.ProcessCost)) || parseFloat(item.ProcessCost) <= 0) {
                        if (!$('#ProcessCost_' + index).hasClass('invalid-class'))
                            $('#ProcessCost_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#ProcessCost_' + index).removeClass('invalid-class');
                    }
                    if (isNaN(parseFloat(item.NoOfCavities)) || parseFloat(item.NoOfCavities) <= 0) {
                        if (!$('#NoOfCavities_' + index).hasClass('invalid-class'))
                            $('#NoOfCavities_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#NoOfCavities_' + index).removeClass('invalid-class');
                    }
                }
                else {
                    $('#MinOrderQty_' + index).removeClass('invalid-class');
                    $('#RawMatInputInKg_' + index).removeClass('invalid-class');
                    $('#RawMatCostPerKg_' + index).removeClass('invalid-class');
                    $('#ProcessCost_' + index).removeClass('invalid-class');
                    $('#NoOfCavities_' + index).removeClass('invalid-class');
                }
            }

        }
    }
});

app.directive("changeBackgroundColorDetail", function () {
    return {
        replace: false,
        restrict: 'A',
        link: function (scope, $element, $attrs) {
            var e = $element;
            var index = parseInt($attrs.changeBackgroundColorDetail, 10);
            e.on('blur', function () {
                scope.setInvalidClass(scope.item, index);
            });
            scope.setInvalidClass = function (item, index) {
                if ((!isNaN(parseFloat(item.MinOrderQty)) && parseFloat(item.MinOrderQty) > 0)
                        || (!isNaN(parseFloat(item.ToolingCost)) && parseFloat(item.ToolingCost) > 0)
                        || (!isNaN(parseFloat(item.NoOfCavities)) && parseFloat(item.NoOfCavities) > 0)
                        || (!isNaN(parseFloat(item.rFQdqRawMaterial.RawMatInputInKg)) && parseFloat(item.rFQdqRawMaterial.RawMatInputInKg) > 0)
                        || (!isNaN(parseFloat(item.rFQdqRawMaterial.RawMatCostPerKg)) && parseFloat(item.rFQdqRawMaterial.RawMatCostPerKg) > 0)
                        || (!isNaN(parseFloat(item.rFQdqPrimaryProcessConversion.MachineDescId)) && parseFloat(item.rFQdqPrimaryProcessConversion.MachineDescId) > 0)
                        || (!isNaN(parseFloat(item.rFQdqPrimaryProcessConversion.MachineSize)) && parseFloat(item.rFQdqPrimaryProcessConversion.MachineSize) > 0)
                        || (!isNaN(parseFloat(item.rFQdqPrimaryProcessConversion.CycleTime)) && parseFloat(item.rFQdqPrimaryProcessConversion.CycleTime) > 0)
                        || (!isNaN(parseFloat(item.rFQdqPrimaryProcessConversion.ManPlusMachineRatePerHour)) && parseFloat(item.rFQdqPrimaryProcessConversion.ManPlusMachineRatePerHour) > 0)
                        || (!isNaN(parseFloat(item.rFQdqMachining.MachiningDescId)) && parseFloat(item.rFQdqMachining.MachiningDescId) > 0)
                        || (!isNaN(parseFloat(item.rFQdqMachining.CycleTime)) && parseFloat(item.rFQdqMachining.CycleTime) > 0)
                        || (!isNaN(parseFloat(item.rFQdqMachining.ManPlusMachineRatePerHour)) && parseFloat(item.rFQdqMachining.ManPlusMachineRatePerHour) > 0)
                        || (!isNaN(parseFloat(item.rFQdqMachiningSecondaryOperation.SecondaryOperationDescId)) && parseFloat(item.rFQdqMachiningSecondaryOperation.SecondaryOperationDescId) > 0)
                        || (!isNaN(parseFloat(item.rFQdqMachiningSecondaryOperation.CycleTime)) && parseFloat(item.rFQdqMachiningSecondaryOperation.CycleTime) > 0)
                        || (!isNaN(parseFloat(item.rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour)) && parseFloat(item.rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour) > 0)
                        || (!isNaN(parseFloat(item.rFQdqMachiningOtherOperation.SecondaryOperationDescId)) && parseFloat(item.rFQdqMachiningOtherOperation.SecondaryOperationDescId) > 0)
                        || (!isNaN(parseFloat(item.rFQdqMachiningOtherOperation.CycleTime)) && parseFloat(item.rFQdqMachiningOtherOperation.CycleTime) > 0)
                        || (!isNaN(parseFloat(item.rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour)) && parseFloat(item.rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour) > 0)
                        || (!isNaN(parseFloat(item.rFQdqSurfaceTreatment.CoatingTypeId)) && parseFloat(item.rFQdqSurfaceTreatment.CoatingTypeId) > 0)
                        || (!isNaN(parseFloat(item.rFQdqSurfaceTreatment.PartsPerHour)) && parseFloat(item.rFQdqSurfaceTreatment.PartsPerHour) > 0)
                        || (!isNaN(parseFloat(item.rFQdqSurfaceTreatment.ManPlusMachineRatePerHour)) && parseFloat(item.rFQdqSurfaceTreatment.ManPlusMachineRatePerHour) > 0)
                        || (!isNaN(parseFloat(item.rFQdqOverhead.InventoryCarryingCost)) && parseFloat(item.rFQdqOverhead.InventoryCarryingCost) > 0)
                        || (!isNaN(parseFloat(item.rFQdqOverhead.PackagingMaterial)) && parseFloat(item.rFQdqOverhead.PackagingMaterial) > 0)
                        || (!isNaN(parseFloat(item.rFQdqOverhead.Packing)) && parseFloat(item.rFQdqOverhead.Packing) > 0)
                        || (!isNaN(parseFloat(item.rFQdqOverhead.LocalFreightToPort)) && parseFloat(item.rFQdqOverhead.LocalFreightToPort) > 0)
                        || (!isNaN(parseFloat(item.rFQdqOverhead.ProfitAndSGA)) && parseFloat(item.rFQdqOverhead.ProfitAndSGA) > 0)
                        ) {
                    if (isNaN(parseFloat(item.MinOrderQty))) {
                        if (!$('#MinOrderQtyDQ_' + index).hasClass('invalid-class'))
                            $('#MinOrderQtyDQ_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#MinOrderQtyDQ_' + index).removeClass('invalid-class');
                    }
                    if (isNaN(parseFloat(item.rFQdqRawMaterial.RawMatInputInKg)) || parseFloat(item.rFQdqRawMaterial.RawMatInputInKg) <= 0) {
                        if (!$('#RawMatInputInKgDQ_' + index).hasClass('invalid-class'))
                            $('#RawMatInputInKgDQ_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#RawMatInputInKgDQ_' + index).removeClass('invalid-class');
                    }
                    if (isNaN(parseFloat(item.rFQdqRawMaterial.RawMatCostPerKg)) || parseFloat(item.rFQdqRawMaterial.RawMatCostPerKg) <= 0) {
                        if (!$('#RawMatCostPerKgDQ_' + index).hasClass('invalid-class'))
                            $('#RawMatCostPerKgDQ_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#RawMatCostPerKgDQ_' + index).removeClass('invalid-class');
                    }
                    if (isNaN(parseFloat(item.rFQdqPrimaryProcessConversion.MachineDescId)) || parseFloat(item.rFQdqPrimaryProcessConversion.MachineDescId) <= 0) {
                        if (!$('#MachineDescId_' + index).hasClass('invalid-class'))
                            $('#MachineDescId_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#MachineDescId_' + index).removeClass('invalid-class');
                    }
                    if (isNaN(parseFloat(item.rFQdqPrimaryProcessConversion.CycleTime)) || parseFloat(item.rFQdqPrimaryProcessConversion.CycleTime) <= 0) {
                        if (!$('#MachineCycleTime_' + index).hasClass('invalid-class'))
                            $('#MachineCycleTime_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#MachineCycleTime_' + index).removeClass('invalid-class');
                    }
                    if (isNaN(parseFloat(item.rFQdqPrimaryProcessConversion.ManPlusMachineRatePerHour)) || parseFloat(item.rFQdqPrimaryProcessConversion.ManPlusMachineRatePerHour) <= 0) {
                        if (!$('#ManMachineRatePerHour_' + index).hasClass('invalid-class'))
                            $('#ManMachineRatePerHour_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#ManMachineRatePerHour_' + index).removeClass('invalid-class');
                    }
                    if (isNaN(parseFloat(item.NoOfCavities)) || parseFloat(item.NoOfCavities) <= 0) {
                        if (!$('#NoOfCavitiesDQ_' + index).hasClass('invalid-class'))
                            $('#NoOfCavitiesDQ_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#NoOfCavitiesDQ_' + index).removeClass('invalid-class');
                    }

                    //*** Start these validations will be fired section wise if in a section, there is an value then the other values of that section will have to enter. ***//
                    //*** Start Machining **//
                    if ((isNaN(parseFloat(item.rFQdqMachining.ManPlusMachineRatePerHour)) || parseFloat(item.rFQdqMachining.ManPlusMachineRatePerHour) <= 0)
                        && (isNaN(parseFloat(item.rFQdqMachining.CycleTime)) || parseFloat(item.rFQdqMachining.CycleTime) <= 0)) {
                        $('#MManPlusMachineRatePerHour_' + index).removeClass('invalid-class');
                        $('#MCycleTime_' + index).removeClass('invalid-class');
                    }
                    else {
                        if (!isNaN(parseFloat(item.rFQdqMachining.CycleTime)) && parseFloat(item.rFQdqMachining.CycleTime) > 0) {
                            if (isNaN(parseFloat(item.rFQdqMachining.ManPlusMachineRatePerHour)) || parseFloat(item.rFQdqMachining.ManPlusMachineRatePerHour) <= 0) {
                                if (!$('#MManPlusMachineRatePerHour_' + index).hasClass('invalid-class'))
                                    $('#MManPlusMachineRatePerHour_' + index).addClass('invalid-class');
                            }
                            else {
                                $('#MManPlusMachineRatePerHour_' + index).removeClass('invalid-class');
                            }
                            $('#MCycleTime_' + index).removeClass('invalid-class');
                        }
                        if (!isNaN(parseFloat(item.rFQdqMachining.ManPlusMachineRatePerHour)) && parseFloat(item.rFQdqMachining.ManPlusMachineRatePerHour) > 0) {
                            if (isNaN(parseFloat(item.rFQdqMachining.CycleTime)) || parseFloat(item.rFQdqMachining.CycleTime) <= 0) {
                                if (!$('#MCycleTime_' + index).hasClass('invalid-class'))
                                    $('#MCycleTime_' + index).addClass('invalid-class');
                            }
                            else {
                                $('#MCycleTime_' + index).removeClass('invalid-class');
                            }
                            $('#MManPlusMachineRatePerHour_' + index).removeClass('invalid-class');
                        }
                    }
                    //*** End Machining **//
                    //*** Start Machining 2/Secondary Opr. **//
                    if ((isNaN(parseFloat(item.rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour)) || parseFloat(item.rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour) <= 0)
                        && (isNaN(parseFloat(item.rFQdqMachiningSecondaryOperation.CycleTime)) || parseFloat(item.rFQdqMachiningSecondaryOperation.CycleTime) <= 0)) {
                        $('#MSOManPlusMachineRatePerHour_' + index).removeClass('invalid-class');
                        $('#MSOCycleTime_' + index).removeClass('invalid-class');
                    }
                    else {
                        if (!isNaN(parseFloat(item.rFQdqMachiningSecondaryOperation.CycleTime)) && parseFloat(item.rFQdqMachiningSecondaryOperation.CycleTime) > 0) {
                            if (isNaN(parseFloat(item.rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour)) || parseFloat(item.rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour) <= 0) {
                                if (!$('#MSOManPlusMachineRatePerHour_' + index).hasClass('invalid-class'))
                                    $('#MSOManPlusMachineRatePerHour_' + index).addClass('invalid-class');
                            }
                            else {
                                $('#MSOManPlusMachineRatePerHour_' + index).removeClass('invalid-class');
                            }
                            $('#MSOCycleTime_' + index).removeClass('invalid-class');
                        }
                        if (!isNaN(parseFloat(item.rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour)) && parseFloat(item.rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour) > 0) {
                            if (isNaN(parseFloat(item.rFQdqMachiningSecondaryOperation.CycleTime)) || parseFloat(item.rFQdqMachiningSecondaryOperation.CycleTime) <= 0) {
                                if (!$('#MSOCycleTime_' + index).hasClass('invalid-class'))
                                    $('#MSOCycleTime_' + index).addClass('invalid-class');
                            }
                            else {
                                $('#MSOCycleTime_' + index).removeClass('invalid-class');
                            }
                            $('#MSOManPlusMachineRatePerHour_' + index).removeClass('invalid-class');
                        }
                    }
                    //*** End Machining 2/Secondary Opr. **//
                    //*** Start Machining 3/Other Opr. **//
                    if ((isNaN(parseFloat(item.rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour)) || parseFloat(item.rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour) <= 0)
                        && (isNaN(parseFloat(item.rFQdqMachiningOtherOperation.CycleTime)) || parseFloat(item.rFQdqMachiningOtherOperation.CycleTime) <= 0)) {
                        $('#MOOManMachineRatePerHour_' + index).removeClass('invalid-class');
                        $('#MOOCycleTime_' + index).removeClass('invalid-class');
                    }
                    else {
                        if (!isNaN(parseFloat(item.rFQdqMachiningOtherOperation.CycleTime)) && parseFloat(item.rFQdqMachiningOtherOperation.CycleTime) > 0) {
                            if (isNaN(parseFloat(item.rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour)) || parseFloat(item.rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour) <= 0) {
                                if (!$('#MOOManMachineRatePerHour_' + index).hasClass('invalid-class'))
                                    $('#MOOManMachineRatePerHour_' + index).addClass('invalid-class');
                            }
                            else {
                                $('#MOOManMachineRatePerHour_' + index).removeClass('invalid-class');
                            }
                            $('#MOOCycleTime_' + index).removeClass('invalid-class');
                        }
                        if (!isNaN(parseFloat(item.rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour)) && parseFloat(item.rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour) > 0) {
                            if (isNaN(parseFloat(item.rFQdqMachiningOtherOperation.CycleTime)) || parseFloat(item.rFQdqMachiningOtherOperation.CycleTime) <= 0) {
                                if (!$('#MOOCycleTime_' + index).hasClass('invalid-class'))
                                    $('#MOOCycleTime_' + index).addClass('invalid-class');
                            }
                            else {
                                $('#MOOCycleTime_' + index).removeClass('invalid-class');
                            }
                            $('#MOOManMachineRatePerHour_' + index).removeClass('invalid-class');
                        }
                    }
                    //*** End Machining 3/Other Opr. **//
                    //*** Start Surface Treatment **//
                    if ((isNaN(parseFloat(item.rFQdqSurfaceTreatment.ManPlusMachineRatePerHour)) || parseFloat(item.rFQdqSurfaceTreatment.ManPlusMachineRatePerHour) <= 0)
                        && (isNaN(parseFloat(item.rFQdqSurfaceTreatment.PartsPerHour)) || parseFloat(item.rFQdqSurfaceTreatment.PartsPerHour) <= 0)) {
                        $('#STManPlusMachineRatePerHour_' + index).removeClass('invalid-class');
                        $('#STPartsPerHour_' + index).removeClass('invalid-class');
                    }
                    else {
                        if (!isNaN(parseFloat(item.rFQdqSurfaceTreatment.PartsPerHour)) && parseFloat(item.rFQdqSurfaceTreatment.PartsPerHour) > 0) {
                            if (isNaN(parseFloat(item.rFQdqSurfaceTreatment.ManPlusMachineRatePerHour)) || parseFloat(item.rFQdqSurfaceTreatment.ManPlusMachineRatePerHour) <= 0) {
                                if (!$('#STManPlusMachineRatePerHour_' + index).hasClass('invalid-class'))
                                    $('#STManPlusMachineRatePerHour_' + index).addClass('invalid-class');
                            }
                            else {
                                $('#STManPlusMachineRatePerHour_' + index).removeClass('invalid-class');
                            }
                            $('#STPartsPerHour_' + index).removeClass('invalid-class');
                        }
                        if (!isNaN(parseFloat(item.rFQdqSurfaceTreatment.ManPlusMachineRatePerHour)) && parseFloat(item.rFQdqSurfaceTreatment.ManPlusMachineRatePerHour) > 0) {
                            if (isNaN(parseFloat(item.rFQdqSurfaceTreatment.PartsPerHour)) || parseFloat(item.rFQdqSurfaceTreatment.PartsPerHour) <= 0) {
                                if (!$('#STPartsPerHour_' + index).hasClass('invalid-class'))
                                    $('#STPartsPerHour_' + index).addClass('invalid-class');
                            }
                            else {
                                $('#STPartsPerHour_' + index).removeClass('invalid-class');
                            }
                            $('#STManPlusMachineRatePerHour_' + index).removeClass('invalid-class');
                        }
                    }
                    //*** End Surface Treatment **//
                    //*** End these validations will be fired section wise if in a section, there is an value then the other values of that section will have to enter. ***//

                }
                else {
                    $('#MinOrderQtyDQ_' + index).removeClass('invalid-class');
                    $('#RawMatInputInKgDQ_' + index).removeClass('invalid-class');
                    $('#RawMatCostPerKgDQ_' + index).removeClass('invalid-class');
                    $('#MachineDescId_' + index).removeClass('invalid-class');
                    $('#MachineCycleTime_' + index).removeClass('invalid-class');
                    $('#ManMachineRatePerHour_' + index).removeClass('invalid-class');
                    $('#NoOfCavitiesDQ_' + index).removeClass('invalid-class');

                    $('#MManPlusMachineRatePerHour_' + index).removeClass('invalid-class');
                    $('#MCycleTime_' + index).removeClass('invalid-class');

                    $('#MSOManPlusMachineRatePerHour_' + index).removeClass('invalid-class');
                    $('#MSOCycleTime_' + index).removeClass('invalid-class');

                    $('#MOOManMachineRatePerHour_' + index).removeClass('invalid-class');
                    $('#MOOCycleTime_' + index).removeClass('invalid-class');

                    $('#STManPlusMachineRatePerHour_' + index).removeClass('invalid-class');
                    $('#STPartsPerHour_' + index).removeClass('invalid-class');
                }
            }
        }
    }
});