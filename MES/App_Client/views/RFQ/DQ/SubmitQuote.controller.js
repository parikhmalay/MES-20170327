app.controller('RFQDetailSupplierPartQuoteCtrl', ['$rootScope', '$scope', 'common', 'RFQDetailSupplierPartQuoteSvc', '$routeParams', '$timeout', '$filter', 'LookupSvc',
    function ($rootScope, $scope, common, RFQDetailSupplierPartQuoteSvc, $routeParams, $timeout, $filter, LookupSvc) {
        $rootScope.PageHeader = ($filter('translate')('_PageHeading_'));

        if (!IsUndefinedNullOrEmpty($routeParams.UniqueUrl)) {
            $scope.UniqueUrl = $routeParams.UniqueUrl;
        }

        $scope.RFQSupplierPartQuoteList = {};

        $scope.Init = function () {
            $scope.CurrentTab = 'PartDetail';
            $scope.EnterQuote = true;
            $scope.NoQuote = false;
            $scope.IsForm = true;
            $scope.IsThanks = false;
            $scope.ThanksMsg = '';
            $scope.IsWarning = false;
            $scope.WarningMsg = '';
            $scope.Paging = {};
            $scope.SearchCriteria = { RFQId: '', SupplierId: 0, UniqueUrl: $scope.UniqueUrl };
            $scope.Paging.Criteria = $scope.SearchCriteria;
            //$scope.GetRFQSupplierPartQuoteList();
            $scope.SetLooksupData();
            setTimeout(function () {
                $scope.GetRFQSupplierPartQuoteList();
            }, 1000);
        };
        $scope.GetRFQSupplierPartQuoteList = function () {
            common.usSpinnerService.spin('spnRFQSupplierPartQuote');
            RFQDetailSupplierPartQuoteSvc.GetDQRFQSupplierPartQuoteList($scope.Paging).then(
                 function (response) {
                     common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                     if (response.data.StatusCode == 200) {                  //ShowMessage(common, response.data)
                         $scope.RFQSupplierPartQuoteList = response.data.Result;
                         angular.forEach($scope.RFQSupplierPartQuoteList, function (item, index) {
                             //Part details
                             item.MinOrderQty = '';
                             item.ToolingCost = '0.000';
                             item.NoOfCavities = '0';
                             item.ExchangeRate = '0.000';
                             item.UnitPrice = '0.000';

                             //Raw material
                             item.rFQdqRawMaterial.RawMatInputInKg = '0.000';
                             item.rFQdqRawMaterial.RawMatCostPerKg = '0.000';
                             item.rFQdqRawMaterial.MaterialLoss = '0.000';
                             item.rFQdqRawMaterial.RawMatTotal = '0.000';

                             //PrimaryProcess/Conversion
                             item.rFQdqPrimaryProcessConversion.MachineSize = '0';
                             item.rFQdqPrimaryProcessConversion.CycleTime = '0';
                             item.rFQdqPrimaryProcessConversion.ManPlusMachineRatePerHour = '0.000';
                             item.rFQdqPrimaryProcessConversion.ProcessConversionCostPerPart = '0.000';

                             //Machining
                             item.rFQdqMachining.CycleTime = '0';
                             item.rFQdqMachining.ManPlusMachineRatePerHour = '0.000';
                             item.rFQdqMachining.MachiningCostPerPart = '0.000';

                             //Machining / SecondaryOpr
                             item.rFQdqMachiningSecondaryOperation.CycleTime = '0';
                             item.rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour = '0.000';
                             item.rFQdqMachiningSecondaryOperation.SecondaryCostPerPart = '0.000';

                             //Machining/OtherOpr
                             item.rFQdqMachiningOtherOperation.CycleTime = '0';
                             item.rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour = '0.000';
                             item.rFQdqMachiningOtherOperation.SecondaryCostPerPart = '0.000';

                             //SurfaceTreatment
                             item.rFQdqSurfaceTreatment.PartsPerHour = '0';
                             item.rFQdqSurfaceTreatment.ManPlusMachineRatePerHour = '0.000';
                             item.rFQdqSurfaceTreatment.CoatingCostPerHour = '0.000';

                             //Overhead
                             item.rFQdqOverhead.InventoryCarryingCost = '0.000';
                             item.rFQdqOverhead.PackagingMaterial = '0.000';
                             item.rFQdqOverhead.Packing = '0.000';
                             item.rFQdqOverhead.LocalFreightToPort = '0.000';
                             item.rFQdqOverhead.ProfitAndSGA = '0.000';
                             item.rFQdqOverhead.OverheadPercentPiecePrice = '0.00';

                         });
                         $scope.$broadcast("loadScrollerPanel");
                     }
                     else {
                         $scope.IsForm = false;
                         $scope.IsThanks = false;
                         $scope.IsWarning = true;
                         $scope.WarningMsg = response.data.ErrorText;
                         console.log(response.data.ErrorText);
                     }
                 },
                 function (error) {
                     common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                     //common.aaNotify.error(error);
                 });
        };

        $scope.SetLooksupData = function () {
            $scope.LookUps = [
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
            LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
                angular.forEach(data.data, function (o) {
                    if (o.Name === "MachineDesc") {
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
            });
        };

        $scope.isActive = function (route) {
            return ($scope.CurrentTab == route);
            //if (route == "PartDetail") {
            //    return true;
            //}
        };

        $scope.SaveRFQSupplierPartQuote = function () {
            common.usSpinnerService.spin('spnRFQSupplierPartQuote');
            $scope.isValid = true;
            angular.forEach($scope.RFQSupplierPartQuoteList, function (item, index) {
                if ($scope.isValid) {
                    if ((!isNaN(item.MinOrderQty) && parseFloat(item.MinOrderQty) > 0)
                        || (!isNaN(item.ToolingCost) && parseFloat(item.ToolingCost) > 0)
                        || (!isNaN(item.NoOfCavities) && parseFloat(item.NoOfCavities) > 0)
                        || (!isNaN(item.rFQdqRawMaterial.RawMatInputInKg) && parseFloat(item.rFQdqRawMaterial.RawMatInputInKg) > 0)
                        || (!isNaN(item.rFQdqRawMaterial.RawMatCostPerKg) && parseFloat(item.rFQdqRawMaterial.RawMatCostPerKg) > 0)
                        || (!isNaN(item.rFQdqPrimaryProcessConversion.MachineDescId) && parseFloat(item.rFQdqPrimaryProcessConversion.MachineDescId) > 0)
                        || (!isNaN(item.rFQdqPrimaryProcessConversion.MachineSize) && parseFloat(item.rFQdqPrimaryProcessConversion.MachineSize) > 0)
                        || (!isNaN(item.rFQdqPrimaryProcessConversion.CycleTime) && parseFloat(item.rFQdqPrimaryProcessConversion.CycleTime) > 0)
                        || (!isNaN(item.rFQdqPrimaryProcessConversion.ManPlusMachineRatePerHour) && parseFloat(item.rFQdqPrimaryProcessConversion.ManPlusMachineRatePerHour) > 0)
                        || (!isNaN(item.rFQdqMachining.MachiningDescId) && parseFloat(item.rFQdqMachining.MachiningDescId) > 0)
                        || (!isNaN(item.rFQdqMachining.CycleTime) && parseFloat(item.rFQdqMachining.CycleTime) > 0)
                        || (!isNaN(item.rFQdqMachining.ManPlusMachineRatePerHour) && parseFloat(item.rFQdqMachining.ManPlusMachineRatePerHour) > 0)
                        || (!isNaN(item.rFQdqMachiningSecondaryOperation.SecondaryOperationDescId) && parseFloat(item.rFQdqMachiningSecondaryOperation.SecondaryOperationDescId) > 0)
                        || (!isNaN(item.rFQdqMachiningSecondaryOperation.CycleTime) && parseFloat(item.rFQdqMachiningSecondaryOperation.CycleTime) > 0)
                        || (!isNaN(item.rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour) && parseFloat(item.rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour) > 0)
                        || (!isNaN(item.rFQdqMachiningOtherOperation.SecondaryOperationDescId) && parseFloat(item.rFQdqMachiningOtherOperation.SecondaryOperationDescId) > 0)
                        || (!isNaN(item.rFQdqMachiningOtherOperation.CycleTime) && parseFloat(item.rFQdqMachiningOtherOperation.CycleTime) > 0)
                        || (!isNaN(item.rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour) && parseFloat(item.rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour) > 0)
                        || (!isNaN(item.rFQdqSurfaceTreatment.CoatingTypeId) && parseFloat(item.rFQdqSurfaceTreatment.CoatingTypeId) > 0)
                        || (!isNaN(item.rFQdqSurfaceTreatment.PartsPerHour) && parseFloat(item.rFQdqSurfaceTreatment.PartsPerHour) > 0)
                        || (!isNaN(item.rFQdqSurfaceTreatment.ManPlusMachineRatePerHour) && parseFloat(item.rFQdqSurfaceTreatment.ManPlusMachineRatePerHour) > 0)
                        || (!isNaN(item.rFQdqOverhead.InventoryCarryingCost) && parseFloat(item.rFQdqOverhead.InventoryCarryingCost) > 0)
                        || (!isNaN(item.rFQdqOverhead.PackagingMaterial) && parseFloat(item.rFQdqOverhead.PackagingMaterial) > 0)
                        || (!isNaN(item.rFQdqOverhead.Packing) && parseFloat(item.rFQdqOverhead.Packing) > 0)
                        || (!isNaN(item.rFQdqOverhead.LocalFreightToPort) && parseFloat(item.rFQdqOverhead.LocalFreightToPort) > 0)
                        || (!isNaN(item.rFQdqOverhead.ProfitAndSGA) && parseFloat(item.rFQdqOverhead.ProfitAndSGA) > 0)
                        ) {

                        if (isNaN(parseFloat(item.MinOrderQty))
                         || isNaN(item.NoOfCavities) || parseFloat(item.NoOfCavities) <= 0
                         || isNaN(item.rFQdqRawMaterial.RawMatInputInKg) || parseFloat(item.rFQdqRawMaterial.RawMatInputInKg) <= 0
                         || isNaN(item.rFQdqRawMaterial.RawMatCostPerKg) || parseFloat(item.rFQdqRawMaterial.RawMatCostPerKg) <= 0
                         || isNaN(item.rFQdqPrimaryProcessConversion.MachineDescId) || parseFloat(item.rFQdqPrimaryProcessConversion.MachineDescId) <= 0
                         || isNaN(item.rFQdqPrimaryProcessConversion.CycleTime) || parseFloat(item.rFQdqPrimaryProcessConversion.CycleTime) <= 0
                         || isNaN(item.rFQdqPrimaryProcessConversion.ManPlusMachineRatePerHour) || parseFloat(item.rFQdqPrimaryProcessConversion.ManPlusMachineRatePerHour) <= 0
                         ) {
                            common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                            common.aaNotify.error($filter('translate')('_IsRequiredErrMsg_'));//("Please enter valid value for the required field(s).");
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

            setTimeout(function () {
                if (!$scope.isValid)
                    return false;

                if (!IsObjectEmpty($scope.RFQSupplierPartQuoteList)) {
                    RFQDetailSupplierPartQuoteSvc.SaveSubmitQuote($scope.RFQSupplierPartQuoteList).then(
                   function (response) {
                       common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                       if (response.data.StatusCode == 200) {
                           //need to redirect here to thnaks page.
                           $scope.IsThanks = true;
                           $scope.IsForm = false;
                           $scope.ThanksMsg = response.data.SuccessMessage;
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
            }, 1000);
        };

        $scope.SubmitNoQuote = function () {
            common.usSpinnerService.spin('spnRFQSupplierPartQuote');
            RFQDetailSupplierPartQuoteSvc.SaveSubmitNoQuote($scope.Paging).then(
                function (response) {
                    common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                    if (response.data.StatusCode == 200) {
                        //need to redirect here to thnaks page.
                        $scope.IsThanks = true;
                        $scope.IsForm = false;
                        $scope.ThanksMsg = response.data.SuccessMessage;
                    }
                },
              function (error) {
                  common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                  //common.aaNotify.error(error);
              });
        };

        //Start Calculations and validations
        $scope.CalculateUnitPrice = function (item, key) {
            angular.forEach($scope.RFQSupplierPartQuoteList, function (obj, index) {
                if (index == key) {
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
            });
        };

        $scope.setToolingCost = function (item) {
            item.ToolingCost = $filter('number')(item.ToolingCost, 3);
        };

        $scope.setRawMaterialPriceAssumed = function (item) {
            //set   Raw Material Price Assumed value as it is as item.rFQdqRawMaterial.RawMatTotal(before for SQ it was item.MaterialCost but now same for SQ & DQ) if there is only one part in the list.
            if ($scope.RFQSupplierPartQuoteList.length == 1)
                $scope.RFQSupplierPartQuoteList[0].RawMaterialPriceAssumed = item.rFQdqRawMaterial.RawMatTotal;
        };
        //End Calculations and validations

        $scope.DownloadRSPQFile = function () {
            common.usSpinnerService.spin('spnRFQSupplierPartQuote');
            if (!IsUndefinedNullOrEmpty($routeParams.UniqueUrl)) {
                $scope.SearchCriteria.RFQId = $scope.RFQSupplierPartQuoteList.RfqId;
                $scope.SearchCriteria.UniqueUrl = $scope.UniqueUrl;
                $scope.SearchCriteria.UploadQuoteFilePath = $scope.RFQSupplierPartQuoteList[0].UploadQuoteFilePath;
                $scope.Paging.Criteria = $scope.SearchCriteria;

                RFQDetailSupplierPartQuoteSvc.downloadRfqSupplierPartQuote($scope.Paging).then(
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

        $scope.UploadRSPQFile = function () {
            common.usSpinnerService.spin('spnRFQSupplierPartQuote');
            if (IsUndefinedNullOrEmpty($scope.RFQSupplierPartQuoteList[0].UploadQuoteFilePath) || ($scope.RFQSupplierPartQuoteList[0].UploadQuoteFilePath == '')) {
                common.aaNotify.error($filter('translate')('_SelectFile_'));
                common.usSpinnerService.stop('spnRFQSupplierPartQuote');
            }
            else {
                $scope.SearchCriteria.RFQId = $scope.RFQSupplierPartQuoteList.RfqId;
                $scope.SearchCriteria.UniqueUrl = $scope.UniqueUrl;
                $scope.SearchCriteria.UploadQuoteFilePath = $scope.RFQSupplierPartQuoteList[0].UploadQuoteFilePath;
                $scope.Paging.Criteria = $scope.SearchCriteria;

                RFQDetailSupplierPartQuoteSvc.uploadRfqSupplierPartQuote($scope.Paging).then(
                     function (response) {
                         common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                         if (response.data.StatusCode == 200) {
                             $scope.RFQSupplierPartQuoteList = response.data.Result; 

                             angular.forEach($scope.RFQSupplierPartQuoteList, function (item) {
                                 item.UnitPrice = $filter('number')(item.UnitPrice, 3);
                                 item.rFQdqRawMaterial.MaterialLoss = $filter('number')(IsUndefinedNullOrEmpty(item.rFQdqRawMaterial.MaterialLoss) ? 0 : item.rFQdqRawMaterial.MaterialLoss, 3);
                                 item.rFQdqRawMaterial.RawMatTotal = $filter('number')(IsUndefinedNullOrEmpty(item.rFQdqRawMaterial.RawMatTotal) ? 0 : item.rFQdqRawMaterial.RawMatTotal, 3);
                              
                                 item.rFQdqPrimaryProcessConversion.ProcessConversionCostPerPart = $filter('number')(IsUndefinedNullOrEmpty(item.rFQdqPrimaryProcessConversion.ProcessConversionCostPerPart) ? 0 : item.rFQdqPrimaryProcessConversion.ProcessConversionCostPerPart, 3);
                                 item.rFQdqMachining.MachiningCostPerPart = $filter('number')(IsUndefinedNullOrEmpty(item.rFQdqMachining.MachiningCostPerPart) ? 0 : item.rFQdqMachining.MachiningCostPerPart, 3);
                                 item.rFQdqMachiningSecondaryOperation.SecondaryCostPerPart = $filter('number')(IsUndefinedNullOrEmpty(item.rFQdqMachiningSecondaryOperation.SecondaryCostPerPart) ? 0 : item.rFQdqMachiningSecondaryOperation.SecondaryCostPerPart, 3);
                                 item.rFQdqMachiningOtherOperation.SecondaryCostPerPart = $filter('number')(IsUndefinedNullOrEmpty(item.rFQdqMachiningOtherOperation.SecondaryCostPerPart) ? 0 : item.rFQdqMachiningOtherOperation.SecondaryCostPerPart, 3);
                                 item.rFQdqSurfaceTreatment.CoatingCostPerHour = $filter('number')(IsUndefinedNullOrEmpty(item.rFQdqSurfaceTreatment.CoatingCostPerHour) ? 0 : item.rFQdqSurfaceTreatment.CoatingCostPerHour, 3);

                             });


                             $scope.SearchCriteria.UploadPartFilePath = '';
                             $timeout(function () {
                                 $scope.EnterQuote = true;
                             }, 0);
                         }
                         else {
                             common.aaNotify.error(response.data.ErrorText);
                         }
                     },
             function (error) {
                 common.usSpinnerService.stop('spnRFQSupplierPartQuote');
             });
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

        $scope.RedirectToList = function () {
            common.$location.path("/" + $scope.UniqueUrl);
        }
        $scope.ResetForm = function () {
            common.$route.reload();
        }
        $scope.Init();

    }]);
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