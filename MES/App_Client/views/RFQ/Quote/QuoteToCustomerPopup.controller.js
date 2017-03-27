app.controller('QuoteToCustomerPopupCtrl', ['$rootScope', '$scope', 'common', 'QuoteSvc', '$modal', 'LookupSvc', '$filter', '$routeParams', '$timeout', '$window', '$confirm', '$modalInstance', 'QuoteId', 'RFQData', 'QuoteData', 'IsRevision',
function ($rootScope, $scope, common, QuoteSvc, $modal, LookupSvc, $filter, $routeParams, $timeout, $window, $confirm, $modalInstance, QuoteId, RFQData, QuoteData, IsRevision) {
    //Start implement security role wise
    $scope.setRoleWisePrivilege = function () {
        $scope.currentSecurityObject = currentSecurityObject;
        if (!Isundefinedornull($scope.currentSecurityObject) && $scope.currentSecurityObject.length > 0) {
            angular.forEach($scope.currentSecurityObject, function (obj, index) {
                switch (obj.ObjectId) {
                    case 72:
                        switch (obj.PrivilegeId) {
                            case 1:                           //none
                                RedirectToAccessDenied();
                                break;
                        }
                        break;
                    case 73:
                        if ($scope.IsCreate) {
                            $scope.PagePrivilegeCase(obj);
                        }
                        break;
                    case 74:
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
                $scope.IsReadOnlyQuoteToCustomerPage = true;
                break;
            case 3:                         //write
                $scope.IsReadOnlyQuoteToCustomerPage = false;
                break;
        }
    }
    //End implement security role wise

    $scope.NoQuotes = false;
    $scope.IsAccordionObjectEmpty = true;
    $scope.Init = function () {
        $scope.RFQData = RFQData;
        $scope.RFQData.Date = convertUTCDateToLocalDate($scope.RFQData.Date);
        $scope.RFQData.QuoteDueDate = convertUTCDateToLocalDate($scope.RFQData.QuoteDueDate);

        $scope.Quote = {
            lstQuoteDetails: []
        };
        $scope.QuoteCalculationHistoryList = {};
        $scope.objQuoteCalculationHistoryList = [];
        $scope.objQuoteCalculationHistory = {};
        $scope.Quote.StatusId = 1;
        $scope.lstQuoteDetails = {};
        $scope.Quote.LeadTimeInput = '';
        $scope.Quote.ShippingAssumptionInput = '';
        $scope.SearchCriteria = { CustomerId: 0, RfqId: '' };
        $scope.SelectAllObj = { SelectAll: false };

        $timeout(function () {
            if (!IsUndefinedNullOrEmpty(QuoteId) && QuoteId != '') {
                $scope.Quote.Id = (QuoteId);
                if (!IsUndefinedNullOrEmpty(IsRevision)) { $scope.getData(IsRevision); }
                else { $scope.getData(0); }

                $scope.TransactionMode = 'Edit';
                $scope.IsCreate = false;
                if (!IsUndefinedNullOrEmpty(IsRevision) && parseInt(IsRevision) > 0)
                    $scope.TransactionMode = '';
            }
            else {
                if (!Isundefinedornull($scope.RFQData) || !Isundefinedornull(QuoteData)) {
                    $scope.IsCreate = true;
                    $scope.SearchCriteria.RFQId = $scope.RFQData.Id;
                    $scope.SearchCriteria.CustomerId = $scope.RFQData.CustomerId;
                    $scope.Quote = QuoteData;
                    $scope.GetQuotePartsDetail();
                    $scope.SetSupplierLooksupData();

                }
            }
            $scope.setRoleWisePrivilege();
        }, 1000);

        $scope.SetLooksupData();
    };

    //fill default lookups here
    $scope.SetLooksupData = function () {
        $scope.LookUps = [
          {
              "Name": "CustomerForSupplierQuote", "Parameters": {}
          },
          {
              "Name": "RFQForSupplierQuote", "Parameters": { "CustomerId": $scope.SearchCriteria.CustomerId }
          },
          {
              "Name": "SAM", "Parameters": {}
          },
          {
              "Name": "QuoteAssumptions", "Parameters": { "associatedToId": 14 }
          },
          {
              "Name": "QuoteMESComments", "Parameters": { "associatedToId": 15 }
          },
          {
              "Name": "QuoteStatus", "Parameters": {}
          },
          {
               "Name": "MESWarehouses", "Parameters": {}
          }
        ];
        $scope.getLookupData();
    };
    $scope.SetSupplierLooksupData = function () {
        $scope.LookUps = [
          {
              "Name": "SupplierForSupplierQuote", "Parameters": { "RFQId": $scope.SearchCriteria.RFQId }
          },
        ];
        $scope.getLookupData();
    };
    $scope.getLookupData = function () {
        common.usSpinnerService.spin('spnAddEditQuote');
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            common.usSpinnerService.stop('spnAddEditQuote');
            angular.forEach(data.data, function (o) {
                if (o.Name === "CustomerForSupplierQuote") {
                    $scope.CustomerForSupplierQuoteList = o.Data;
                }
                else if (o.Name === "RFQForSupplierQuote") {
                    $scope.RFQForSupplierQuoteList = o.Data;
                }
                else if (o.Name === "SupplierForSupplierQuote") {
                    $scope.SupplierForSupplierQuoteList = o.Data;
                }
                else if (o.Name === "SAM") {
                    $scope.SAMList = o.Data;
                }
                else if (o.Name === "QuoteAssumptions") {
                    $scope.QuoteAssumptionList = o.Data;
                }
                else if (o.Name === "QuoteMESComments") {
                    $scope.QuoteMESCommentList = o.Data;
                }
                else if (o.Name === "QuoteStatus") {
                    $scope.QuoteStatusList = o.Data;
                }
                else if (o.Name === "MESWarehouses") {
                    $scope.MESWareHouseList = o.Data;
                }
            });
        });
    }
    $scope.getData = function (isR) {
        if (!IsUndefinedNullOrEmpty($scope.Quote.Id)) {
            $scope.GetQuoteDetailList(isR);
        }
    };
    $scope.GetQuoteDetailList = function (isR) {
        common.usSpinnerService.spin('spnAddEditQuote');
        if (isR == '1')
            isR = true;
        else
            isR = false;

        QuoteSvc.GetQuoteDetails($scope.Quote.Id, isR).then(
             function (response) {
                 common.usSpinnerService.stop('spnAddEditQuote');
                 if (response.data.StatusCode == 200) {
                     $scope.Quote = response.data.Result;
                     $scope.SearchCriteria.RFQId = $scope.Quote.RfqId;
                     $scope.SearchCriteria.CustomerId = $scope.Quote.CustomerId;
                     $scope.SetSupplierLooksupData();

                     if (isR == '1') {
                         $scope.Quote.isRevision = true;
                         $scope.Quote.Id = '';
                     }
                     else
                         $scope.Quote.isRevision = false;
                     $scope.QuoteCalculationHistoryList = response.data.Result.lstQuoteCalculationHistory;
                     angular.forEach($scope.QuoteCalculationHistoryList, function (items) {
                         items.UpdatedDate = convertUTCDateToLocalDate(items.UpdatedDate);
                     });
                     if ($scope.QuoteCalculationHistoryList.length > 0) {
                         $scope.IsAccordionObjectEmpty = false;
                         $scope.QuoteCalculationHistoryList = _.groupBy($scope.QuoteCalculationHistoryList, 'UpdatedOn');
                     }
                     else
                         $scope.IsAccordionObjectEmpty = true;
                     //$scope.IsAccordionObjectEmpty = IsObjectEmpty($scope.QuoteCalculationHistoryList);
                     $scope.$broadcast("loadScrollerPanel");
                 }
                 else {

                     console.log(response.data.ErrorText);
                 }
             },
             function (error) {
                 common.usSpinnerService.stop('spnAddEditQuote');
                 //common.aaNotify.error(error);
             });
    };
    //Start Calculations and validations
    $scope.CalculateUnitPrice = function (item, key) {
        var prpUnitPrice = 0, EstimatedQty = 0, PartWeightKG = 0, SupplierPriceUsed = 0;
        $scope.TotalAmountWon = 0;

        angular.forEach($scope.Quote.lstQuoteDetails, function (obj, index) {
            var prpUnitPrice = 0, EstimatedQty = 0, PartWeightKG = 0, SupplierPriceUsed = 0;

            if (!IsUndefinedNullOrEmpty(obj.SupplierPriceUsed) && !isNaN(obj.SupplierPriceUsed)) {
                prpUnitPrice = prpUnitPrice + parseFloat(obj.SupplierPriceUsed);
                SupplierPriceUsed = SupplierPriceUsed + parseFloat(obj.SupplierPriceUsed);
            }

            if (!IsUndefinedNullOrEmpty(obj.CustomDuties) && !isNaN(obj.CustomDuties))
                prpUnitPrice = prpUnitPrice + parseFloat(obj.CustomDuties);
            if (!IsUndefinedNullOrEmpty(obj.ShippingCost) && !isNaN(obj.ShippingCost))
                prpUnitPrice = prpUnitPrice + parseFloat(obj.ShippingCost);
            if (!IsUndefinedNullOrEmpty(obj.Warehousing) && !isNaN(obj.Warehousing))
                prpUnitPrice = prpUnitPrice + parseFloat(obj.Warehousing);

            if (!IsUndefinedNullOrEmpty(obj.SGAProfit) && !isNaN(obj.SGAProfit))
                prpUnitPrice = prpUnitPrice + parseFloat(obj.SGAProfit);
            if (!IsUndefinedNullOrEmpty(obj.SalesCommission) && !isNaN(obj.SalesCommission))
                prpUnitPrice = prpUnitPrice + parseFloat(obj.SalesCommission);
            //FinalMESPrice
            obj.FinalMESPrice = $filter('setDecimal')(prpUnitPrice, 3);
            //TotalAnnualCost
            if (!IsUndefinedNullOrEmpty(obj.EstimatedQty) && !isNaN(obj.EstimatedQty))
                EstimatedQty = EstimatedQty + parseFloat(obj.EstimatedQty);
            obj.TotalAnnualCost = $filter('setDecimal')(prpUnitPrice * EstimatedQty, 3);
            $scope.TotalAmountWon = $scope.TotalAmountWon + parseFloat(obj.TotalAnnualCost);
            //FinalMESPerKg
            if (!IsUndefinedNullOrEmpty(obj.PartWeightKG) && !isNaN(obj.PartWeightKG))
                PartWeightKG = parseFloat(obj.PartWeightKG);
            if (PartWeightKG > 0) {
                obj.FinalMESPerKg = $filter('setDecimal')(prpUnitPrice / PartWeightKG, 3);
                obj.SupplierCostPerKg = $filter('setDecimal')(SupplierPriceUsed / PartWeightKG, 3);
            }
        });

        if (!IsUndefinedNullOrEmpty($scope.Quote.StatusId) && !isNaN($scope.Quote.StatusId) && parseFloat($scope.Quote.StatusId) == 2)
            $scope.Quote.AmountWon = $filter('setDecimal')($scope.TotalAmountWon, 3);
        else
            $scope.Quote.AmountWon = $filter('setDecimal')($scope.Quote.AmountWon, 3);
    };

    $scope.CalculateBasedOnPercent = function () {
        common.usSpinnerService.spin('spnAddEditQuote');
        $scope.IsAnySelected = false;
        $scope.IsPartWeightZero = false;
        var prpUnitPrice = 0, EstimatedQty = 0, PartWeightKG = 0;
        var SupplierPriceUsed = 0, CustomDutiesPercent = 0, ShippingCostPercent = 0;
        var WarehousingPercent = 0, SGAProfitPercent = 0, SalesCommissionPercent = 0;
        var ToolingCostPercent = 0, TotalAmountWon = 0, finalMESPrice = 0;

        angular.forEach($scope.Quote.lstQuoteDetails, function (item, index) {
            if (item.chkSelect) {
                $scope.IsAnySelected = true;

                prpUnitPrice = EstimatedQty = PartWeightKG =
                                SupplierPriceUsed = CustomDutiesPercent =
                                ShippingCostPercent = WarehousingPercent =
                                SGAProfitPercent = SalesCommissionPercent =
                                ToolingCostPercent = finalMESPrice = 0;

                if (IsUndefinedNullOrEmpty(item.PartWeightKG))
                    $scope.IsPartWeightZero = true;


                //Calculate CustomDuties
                if (!IsUndefinedNullOrEmpty($scope.Quote.CustomDutiesPercent) && !isNaN($scope.Quote.CustomDutiesPercent)) {
                    CustomDutiesPercent = CustomDutiesPercent + parseFloat($scope.Quote.CustomDutiesPercent);
                }
                if (!IsUndefinedNullOrEmpty(item.SupplierPriceUsed) && !isNaN(item.SupplierPriceUsed)) {
                    prpUnitPrice = prpUnitPrice + parseFloat(item.SupplierPriceUsed);
                    SupplierPriceUsed = SupplierPriceUsed + parseFloat(item.SupplierPriceUsed);
                    item.CustomDuties = $filter('setDecimal')((SupplierPriceUsed * CustomDutiesPercent) / 100, 3);
                }

                //Calculate ShippingCost
                if (!IsUndefinedNullOrEmpty($scope.Quote.ShippingCostPercent) && !isNaN($scope.Quote.ShippingCostPercent)) {
                    ShippingCostPercent = ShippingCostPercent + parseFloat($scope.Quote.ShippingCostPercent);
                }
                if (ShippingCostPercent > 0) {
                    if ($scope.Quote.ShippingCostCalMethod == "%" && SupplierPriceUsed > 0)
                        item.ShippingCost = $filter('setDecimal')((SupplierPriceUsed * ShippingCostPercent) / 100, 3);
                    else {
                        if (!IsUndefinedNullOrEmpty(item.PartWeightKG))
                            item.ShippingCost = $filter('setDecimal')((parseFloat(item.PartWeightKG) * ShippingCostPercent), 3);
                    }
                }

                //Calculate Warehousing
                if (!IsUndefinedNullOrEmpty($scope.Quote.WarehousingPercent) && !isNaN($scope.Quote.WarehousingPercent)) {
                    WarehousingPercent = WarehousingPercent + parseFloat($scope.Quote.WarehousingPercent);
                }
                if (WarehousingPercent > 0) {
                    item.Warehousing = $filter('setDecimal')(((SupplierPriceUsed + parseFloat(item.CustomDuties) + parseFloat(item.ShippingCost)) * WarehousingPercent) / 100, 3);
                }

                //Calculate SGAProfit
                if (!IsUndefinedNullOrEmpty($scope.Quote.SGAProfitPercent) && !isNaN($scope.Quote.SGAProfitPercent)) {
                    SGAProfitPercent = SGAProfitPercent + parseFloat($scope.Quote.SGAProfitPercent);
                }
                if (SGAProfitPercent > 0) {
                    item.SGAProfit = $filter('setDecimal')(((SupplierPriceUsed + parseFloat(item.CustomDuties) + parseFloat(item.ShippingCost) + parseFloat(item.Warehousing)) * SGAProfitPercent) / 100, 3);
                }

                //Calculate SalesCommission
                if (!IsUndefinedNullOrEmpty($scope.Quote.SalesCommissionPercent) && !isNaN($scope.Quote.SalesCommissionPercent)) {
                    SalesCommissionPercent = SalesCommissionPercent + parseFloat($scope.Quote.SalesCommissionPercent);
                }
                if (SalesCommissionPercent > 0) {
                    item.SalesCommission = $filter('setDecimal')(((SupplierPriceUsed + parseFloat(item.CustomDuties) + parseFloat(item.ShippingCost) + parseFloat(item.Warehousing) + parseFloat(item.SGAProfit)) * SalesCommissionPercent) / 100, 3);
                }

                //Calculate ToolingCost
                if (!IsUndefinedNullOrEmpty($scope.Quote.ToolingCostPercent) && !isNaN($scope.Quote.ToolingCostPercent)) {
                    ToolingCostPercent = ToolingCostPercent + parseFloat($scope.Quote.ToolingCostPercent);
                }
                if (ToolingCostPercent > 0) {
                    item.ToolingCost = $filter('setDecimal')((parseFloat(item.SupplierToolingCost) * ToolingCostPercent) / 100, 3);
                    $scope.setToolingCost(item);
                }

                finalMESPrice = $filter('setDecimal')((finalMESPrice + SupplierPriceUsed + parseFloat(item.CustomDuties) + parseFloat(item.Warehousing)
                                             + parseFloat(item.ShippingCost) + parseFloat(item.SGAProfit) + parseFloat(item.SalesCommission)), 3);

                if (!IsUndefinedNullOrEmpty(item.EstimatedQty) && !isNaN(item.EstimatedQty)) {
                    TotalAmountWon = TotalAmountWon + finalMESPrice * parseFloat(item.EstimatedQty);
                }
                //***Start save % calculation data locally
                if ($scope.objQuoteCalculationHistoryList.length > 0) {
                    $scope.objQuoteCalculationHistoryList = $filter('filter')($scope.objQuoteCalculationHistoryList, function (rw) { return rw.PartId != item.PartId });
                }
                $scope.objQuoteCalculationHistory = {
                    QuoteId: '',
                    PartId: item.PartId,
                    CustomDutiesPercent: CustomDutiesPercent,
                    ShippingCostPercent: ShippingCostPercent,
                    ShippingCostCalMethod: $scope.Quote.ShippingCostCalMethod,
                    WarehousingPercent: WarehousingPercent,
                    SGAProfitPercent: SGAProfitPercent,
                    SalesCommissionPercent: SalesCommissionPercent,
                    ToolingCostPercent: ToolingCostPercent,
                    CustomDuties: item.CustomDuties,
                    ShippingCost: item.ShippingCost,
                    Warehousing: item.Warehousing,
                    SGAProfit: item.SGAProfit,
                    SalesCommission: item.SalesCommission,
                    ToolingCost: item.ToolingCost
                };
                $scope.objQuoteCalculationHistoryList.unshift($scope.objQuoteCalculationHistory);
                //*** END save % calculation data locally
            }
        });

        if (!$scope.IsAnySelected) {
            common.aaNotify.error(($filter('translate')('_SelectAtleastOneToCalcMsg_')));
            common.usSpinnerService.stop('spnAddEditQuote');
            return;
        }
        else {
            if ($scope.IsPartWeightZero)
                common.aaNotify.error(($filter('translate')('_ShippingCostValidationMsg_')));

            $scope.CalculateUnitPrice(0, 0);
            common.usSpinnerService.stop('spnAddEditQuote');
        }
    };

    $scope.setAmountWin = function (id) {

        if (IsUndefinedNullOrEmpty($scope.TotalAmountWon)) {
            $scope.TotalAmountWon = 0;

            angular.forEach($scope.Quote.lstQuoteDetails, function (obj, index) {
                var prpUnitPrice = 0, EstimatedQty = 0;

                if (!IsUndefinedNullOrEmpty(obj.SupplierPriceUsed) && !isNaN(obj.SupplierPriceUsed)) {
                    prpUnitPrice = prpUnitPrice + parseFloat(obj.SupplierPriceUsed);
                }

                if (!IsUndefinedNullOrEmpty(obj.CustomDuties) && !isNaN(obj.CustomDuties))
                    prpUnitPrice = prpUnitPrice + parseFloat(obj.CustomDuties);
                if (!IsUndefinedNullOrEmpty(obj.ShippingCost) && !isNaN(obj.ShippingCost))
                    prpUnitPrice = prpUnitPrice + parseFloat(obj.ShippingCost);
                if (!IsUndefinedNullOrEmpty(obj.Warehousing) && !isNaN(obj.Warehousing))
                    prpUnitPrice = prpUnitPrice + parseFloat(obj.Warehousing);

                if (!IsUndefinedNullOrEmpty(obj.SGAProfit) && !isNaN(obj.SGAProfit))
                    prpUnitPrice = prpUnitPrice + parseFloat(obj.SGAProfit);
                if (!IsUndefinedNullOrEmpty(obj.SalesCommission) && !isNaN(obj.SalesCommission))
                    prpUnitPrice = prpUnitPrice + parseFloat(obj.SalesCommission);
                //TotalAnnualCost
                if (!IsUndefinedNullOrEmpty(obj.EstimatedQty) && !isNaN(obj.EstimatedQty))
                    EstimatedQty = EstimatedQty + parseFloat(obj.EstimatedQty);
                obj.TotalAnnualCost = $filter('setDecimal')(prpUnitPrice * EstimatedQty, 3);
                $scope.TotalAmountWon = $scope.TotalAmountWon + parseFloat(obj.TotalAnnualCost);

            });
        }

        if (!IsUndefinedNullOrEmpty(id) && !isNaN(id) && parseInt(id, 10) == 2)
            $scope.Quote.AmountWon = $filter('setDecimal')($scope.TotalAmountWon, 3);
        else
            $scope.Quote.AmountWon = $filter('setDecimal')($scope.Quote.AmountWon, 3);


    };

    $scope.SaveQuote = function (closeForm) {
        common.usSpinnerService.spin('spnAddEditQuote');
        $scope.isValid = true;
        $scope.IsAnySelected = false;
        angular.forEach($scope.Quote.lstQuoteDetails, function (item, index) {
            if (item.chkSelect) {
                $scope.IsAnySelected = true;
            }
        });
        if (!$scope.IsAnySelected) {
            common.aaNotify.error(($filter('translate')('_SelectAtleastOneToSaveMsg_')));
            common.usSpinnerService.stop('spnAddEditQuote');
            return;
        }

        $scope.ValidationSuccess();
        setTimeout(function () {
            if (!$scope.isValid)
                return false;

            if (!IsObjectEmpty($scope.Quote)) {
                $scope.Quote.lstQuoteCalculationHistory = angular.copy($scope.objQuoteCalculationHistoryList);
                QuoteSvc.SaveQuote($scope.Quote).then(
              function (response) {
                  if (ShowMessage(common, response.data)) {
                      common.usSpinnerService.stop('spnAddEditQuote');
                      $scope.Id = response.data.Result; // Id of latest created record
                      common.aaNotify.success('Quote is saved successfully.');
                      QuoteId = response.data.Result;

                      $rootScope.$broadcast("updateQuoteToCustomerList", { Id: $scope.Quote.RfqId });
                      $scope.Quote.isRevision = false;
                      IsRevision = 0;
                      $scope.Init();
                      if (closeForm) {
                          $scope.RedirectToList();
                      }
                  }
                  else {
                      common.usSpinnerService.stop('spnAddEditQuote');
                  }
              },
              function (error) {
                  common.usSpinnerService.stop('spnAddEditRFQ');
                  console.log(error);
              });
            }
            else {
                common.usSpinnerService.stop('spnAddEditQuote');
            }
        }, 1500);
    };

    $scope.GetPartsList = function () {
        common.usSpinnerService.spin('spnAddEditQuote');
        QuoteSvc.GetPartsToQuote($scope.SearchCriteria.RFQId).then(
             function (response) {
                 common.usSpinnerService.stop('spnAddEditQuote');
                 if (response.data.StatusCode == 200) {
                     $scope.Quote = response.data.Result;

                     $scope.Quote.isRevision = false;

                     if ($scope.Quote.StatusId == null)
                         $scope.Quote.StatusId = 1;

                     if ($scope.TransactionMode == 'Create') {
                         $scope.Quote.CustomDutiesPercent = '0.00';
                         $scope.Quote.ShippingCostPercent = '0.00';
                         $scope.Quote.ShippingCostCalMethod = 'KG';
                         $scope.Quote.WarehousingPercent = '0.00';
                         $scope.Quote.SGAProfitPercent = '0.00';
                         $scope.Quote.SalesCommissionPercent = '0.00';
                         $scope.Quote.ToolingCostPercent = '0.00';
                     };

                     $scope.$broadcast("loadScrollerPanel");
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
             function (error) {
                 common.usSpinnerService.stop('spnAddEditQuote');
                 //common.aaNotify.error(error);
             });
    };

    /*fill Quote List - from - Compare Quote Data - START HERE*/
    $scope.GetQuotePartsDetail = function () {
        common.usSpinnerService.spin('spnAddEditQuote');
        QuoteSvc.GetQuotePartsDetail($scope.Quote).then(
             function (response) {
                 common.usSpinnerService.stop('spnAddEditQuote');
                 if (response.data.StatusCode == 200) {
                     $scope.Quote = response.data.Result;

                     $scope.Quote.isRevision = false;

                     if ($scope.Quote.StatusId == null)
                         $scope.Quote.StatusId = 1;

                     if ($scope.TransactionMode == 'Create') {
                         $scope.Quote.CustomDutiesPercent = '0.00';
                         $scope.Quote.ShippingCostPercent = '0.00';
                         $scope.Quote.ShippingCostCalMethod = 'KG';
                         $scope.Quote.WarehousingPercent = '0.00';
                         $scope.Quote.SGAProfitPercent = '0.00';
                         $scope.Quote.SalesCommissionPercent = '0.00';
                         $scope.Quote.ToolingCostPercent = '0.00';
                     };
                     $scope.CalculateUnitPrice(0, 0);
                     $scope.$broadcast("loadScrollerPanel");
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
             function (error) {
                 common.usSpinnerService.stop('spnAddEditQuote');
                 //common.aaNotify.error(error);
             });
    };
    /*fill Quote List - from - Compare Quote Data - END HERE*/

    $scope.setLeadtimeComments = function () {
        angular.forEach($scope.Quote.lstQuoteDetails, function (obj) {
            obj.Leadtime = $scope.Quote.LeadTimeInput;
        });
    };

    $scope.setShippingAssumptions = function () {
        angular.forEach($scope.Quote.lstQuoteDetails, function (obj) {            
            obj.ShippingAssumption = $scope.Quote.ShippingAssumptionInput;
        });
    };

    $scope.exportToExcel = function () {
        common.usSpinnerService.spin('spnAddEditQuote');
        $scope.isValid = true;
        $scope.IsAnySelected = false;
        angular.forEach($scope.Quote.lstQuoteDetails, function (item, index) {
            if (item.chkSelect) {
                $scope.IsAnySelected = true;
            }
        });
        if (!$scope.IsAnySelected) {
            common.aaNotify.error(($filter('translate')('_SelectAtleastOneToSaveMsg_')));
            common.usSpinnerService.stop('spnAddEditQuote');
            return;
        }

        $scope.ValidationSuccess();
        setTimeout(function () {
            if (!$scope.isValid)
                return false;

            if (!IsObjectEmpty($scope.Quote)) {
                $confirm({ title: ($filter('translate')('_ExportFor_')), ok: ($filter('translate')('_IntQuote_')), cancel: ($filter('translate')('_ExtQuote_')), text1: 'Quote#: ' + $scope.Quote.QuoteNumber })
                  .then(function () {
                      $scope.Quote.IsExcelTypeExt = false;

                      exportFunc();
                  }
                  ,
                  function () {
                      $scope.Quote.IsExcelTypeExt = true;

                      exportFunc();
                  }
                );
            }
            else {
                common.usSpinnerService.stop('spnAddEditQuote');
            }
        });
    };
    function exportFunc() {
        QuoteSvc.exportToExcelQuote($scope.Quote).then(
            function (response) {
                common.usSpinnerService.stop('spnAddEditQuote');
                if (response.data.StatusCode == 200) {
                    window.open(response.data.SuccessMessage, '_blank');
                }
                else {
                    common.aaNotify.error(response.data.ErrorText);
                }
            },
           function (error) {
               common.usSpinnerService.stop('spnAddEditQuote');
               //common.aaNotify.error(error);
           });
    };

    $scope.downloadPDF = function () {
        common.usSpinnerService.spin('spnAddEditQuote');

        $scope.ValidationSuccess();
        setTimeout(function () {


            if (!IsObjectEmpty($scope.Quote)) {
                $confirm({ title: ($filter('translate')('_ExportFor_')), ok: ($filter('translate')('_IntQuote_')), cancel: ($filter('translate')('_ExtQuote_')) })
                  .then(function () {
                      $scope.DownloadPDF();
                  }
                  ,
                  function () {
                      $scope.DownloadExtPDF();
                  }
                );
            }
            else {
                common.usSpinnerService.stop('spnAddEditQuote');
            }
        });
    };
    $scope.DownloadPDF = function () {
        alert($scope.Quote.QuoteFilePath);
        common.usSpinnerService.spin('spnAddEditQuote');
        if (!IsObjectEmpty($scope.Quote) && !IsUndefinedNullOrEmpty($scope.Quote.QuoteFilePath)) {
            window.open(path, '_blank');
        }
        common.usSpinnerService.stop('spnAddEditQuote');
    };

    $scope.DownloadExtPDF = function () {
        common.usSpinnerService.spin('spnAddEditQuote');
        if (!IsObjectEmpty($scope.Quote) && !IsUndefinedNullOrEmpty($scope.Quote.ExtQuoteFilePath)) {
            window.open(path, '_blank');
        }
        common.usSpinnerService.stop('spnAddEditQuote');
    };

    $scope.GetQuotedDetails = function (item, key) {

        common.usSpinnerService.spin('spnAddEditQuote');
        item.RfqId = $scope.SearchCriteria.RFQId;
        QuoteSvc.GetQuotedDetails(item).then(
             function (response) {
                 common.usSpinnerService.stop('spnAddEditQuote');
                 if (response.data.StatusCode == 200) {
                     $scope.Quote.lstQuoteDetails[key] = response.data.Result;
                     $scope.setInvalidClass($scope.Quote.lstQuoteDetails[key], key);
                     $scope.CalculateUnitPrice(0, 0);
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
             function (error) {
                 common.usSpinnerService.stop('spnAddEditQuote');
                 //common.aaNotify.error(error);
             });
    };
    //Start yellow field validations on supplier change
    $scope.fnRemoveClass = function (index) {
        $('#SupplierId_' + index).removeClass('invalid-class');
        $('#SupplierPriceUsed_' + index).removeClass('invalid-class');
        $('#CustomDuties_' + index).removeClass('invalid-class');
        $('#ShippingCost_' + index).removeClass('invalid-class');
        $('#Warehousing_' + index).removeClass('invalid-class');
        $('#SGAProfit_' + index).removeClass('invalid-class');
    };
    $scope.setInvalidClass = function (item, index) {
        $timeout(function () {
            if (item.chkSelect) {
                if ((!isNaN(parseFloat(item.SupplierId)) && parseFloat(item.SupplierId) > 0)
                        || (!isNaN(parseFloat(item.SupplierPriceUsed)) && parseFloat(item.SupplierPriceUsed) > 0)
                        || (!isNaN(parseFloat(item.CustomDuties)) && parseFloat(item.CustomDuties) > 0)
                        || (!isNaN(parseFloat(item.ShippingCost)) && parseFloat(item.ShippingCost) > 0)
                        || (!isNaN(parseFloat(item.Warehousing)) && parseFloat(item.Warehousing) > 0)
                        || (!isNaN(parseFloat(item.SGAProfit)) && parseFloat(item.SGAProfit) > 0)
                        || (!isNaN(parseFloat(item.SalesCommission)) && parseFloat(item.SalesCommission) > 0)
                        || (!isNaN(parseFloat(item.ToolingCost)) && parseFloat(item.ToolingCost) > 0)) {
                    if (isNaN(parseFloat(item.SupplierId)) || parseFloat(item.SupplierId) <= 0) {
                        if (!$('#SupplierId_' + index).hasClass('invalid-class'))
                            $('#SupplierId_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#SupplierId_' + index).removeClass('invalid-class');
                    }
                    if (isNaN(parseFloat(item.SupplierPriceUsed)) || parseFloat(item.SupplierPriceUsed) <= 0) {
                        if (!$('#SupplierPriceUsed_' + index).hasClass('invalid-class'))
                            $('#SupplierPriceUsed_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#SupplierPriceUsed_' + index).removeClass('invalid-class');
                    }
                    if (isNaN(parseFloat(item.CustomDuties)) || parseFloat(item.CustomDuties) <= 0) {
                        if (!$('#CustomDuties_' + index).hasClass('invalid-class'))
                            $('#CustomDuties_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#CustomDuties_' + index).removeClass('invalid-class');
                    }
                    if (isNaN(parseFloat(item.ShippingCost)) || parseFloat(item.ShippingCost) <= 0) {
                        if (!$('#ShippingCost_' + index).hasClass('invalid-class'))
                            $('#ShippingCost_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#ShippingCost_' + index).removeClass('invalid-class');
                    }
                    if (isNaN(parseFloat(item.Warehousing)) || parseFloat(item.Warehousing) <= 0) {
                        if (!$('#Warehousing_' + index).hasClass('invalid-class'))
                            $('#Warehousing_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#Warehousing_' + index).removeClass('invalid-class');
                    }
                    if (isNaN(parseFloat(item.SGAProfit)) || parseFloat(item.SGAProfit) <= 0) {
                        if (!$('#SGAProfit_' + index).hasClass('invalid-class'))
                            $('#SGAProfit_' + index).addClass('invalid-class');
                    }
                    else {
                        $('#SGAProfit_' + index).removeClass('invalid-class');
                    }
                }
                else {
                    $scope.fnRemoveClass(index);
                }
            }
            else {
                $scope.fnRemoveClass(index);
            }
        }, 0);
    }
    //End yellow field validations on supplier change

    $scope.setToolingCost = function (item) {     
        if (item.ToolingCost != 0) {
            var x = parseFloat(item.ToolingCost);
            var y = parseFloat(x / 10);
            var z = 0;

            if (parseFloat(y) < 1)
                z = 10;
            else
                z = Math.ceil(y) * 10;
            item.ToolingCost = z;
        }
    };

    ///Start set search parameters according to selection
    $scope.OnCustomerChange = function () {
        $scope.SearchCriteria.RFQId = '';
        $scope.SearchCriteria.SupplierId = 0;
        $scope.LookUps = [
          {
              "Name": "RFQForSupplierQuote", "Parameters": { "CustomerId": IsUndefinedNullOrEmpty($scope.SearchCriteria.CustomerId) ? 0 : $scope.SearchCriteria.CustomerId }
          },
          {
              "Name": "SupplierForSupplierQuote", "Parameters": { "RFQId": $scope.SearchCriteria.RFQId }
          }
        ];
        $scope.getLookupData();
    };
    $scope.OnRFQChange = function () {
        var rowObject = $filter('filter')($scope.Quote.lstQuoteDetails, function (rw) { return rw.Id == $scope.SearchCriteria.RFQId })[0];

        $scope.LookUps = [
            {
                "Name": "RFQForSupplierQuote", "Parameters": { "CustomerId": $scope.SearchCriteria.CustomerId }
            },
            {
                "Name": "SupplierForSupplierQuote", "Parameters": { "RFQId": $scope.SearchCriteria.RFQId }
            }
        ];
        $scope.getLookupData();
        if (!IsUndefinedNullOrEmpty($scope.SearchCriteria.RFQId)) {
            $scope.getData(0);
        }
    };

    $scope.ConcateAssumptions = function () {

        angular.forEach($scope.Quote.QuoteAssumptionList, function (item) {
            if ($scope.Quote.GeneralAssumption == '' || $scope.Quote.GeneralAssumption == null) {
                $scope.Quote.GeneralAssumption = item.Name;
            }
            else {
                if ($scope.Quote.GeneralAssumption.indexOf(item.Name) > -1) { }
                else if ($scope.Quote.GeneralAssumption.indexOf('\n' + item.Name) > -1) { }
                else if ($scope.Quote.GeneralAssumption.indexOf(item.Name + '\n') > -1) { }
                else
                    $scope.Quote.GeneralAssumption += '\n' + item.Name;
            }
        });
    };

    $scope.ConcateComments = function () {

        angular.forEach($scope.Quote.MESCommentsList, function (item) {
            if ($scope.Quote.MESComments == '' || $scope.Quote.MESComments == null) {
                $scope.Quote.MESComments = item.Name;
            }
            else {
                if ($scope.Quote.MESComments.indexOf(item.Name) > -1) { }
                else if ($scope.Quote.MESComments.indexOf('\n' + item.Name) > -1) { }
                else if ($scope.Quote.MESComments.indexOf(item.Name + '\n') > -1) { }
                else
                    $scope.Quote.MESComments += '\n' + item.Name;
            }
        });
    };
    //Start logic for checkbox select deselect here 
    $scope.SelectDeselectAll = function () {
        angular.forEach($scope.Quote.lstQuoteDetails, function (item, index) {
            item.chkSelect = $scope.SelectAllObj.SelectAll;
            $scope.setInvalidClass(item, index);
        });
    };
    $scope.select = function () {
        $scope.SelectAllObj.SelectAll = true;
        angular.forEach($scope.Quote.lstQuoteDetails, function (item) {
            if (!item.chkSelect)
                $scope.SelectAllObj.SelectAll = false;
        });
    };
    //end logic for checkbox select deselect here 

    $scope.ValidationSuccess = function () {
        angular.forEach($scope.Quote.lstQuoteDetails, function (item, index) {
            if (item.chkSelect) {
                if ($scope.isValid) {
                    if ((!isNaN(parseFloat(item.SupplierId)) && parseFloat(item.SupplierId) > 0)
                         || (!isNaN(parseFloat(item.SupplierPriceUsed)) && parseFloat(item.SupplierPriceUsed) > 0)
                         || (!isNaN(parseFloat(item.CustomDuties)) && parseFloat(item.CustomDuties) > 0)
                         || (!isNaN(parseFloat(item.ShippingCost)) && parseFloat(item.ShippingCost) > 0)
                         || (!isNaN(parseFloat(item.Warehousing)) && parseFloat(item.Warehousing) > 0)
                         || (!isNaN(parseFloat(item.SGAProfit)) && parseFloat(item.SGAProfit) > 0)
                         || (!isNaN(parseFloat(item.SalesCommission)) && parseFloat(item.SalesCommission) > 0)
                         || (!isNaN(parseFloat(item.ToolingCost)) && parseFloat(item.ToolingCost) > 0)) {
                        if ((isNaN(parseFloat(item.SupplierId)) || parseFloat(item.SupplierId) <= 0)
                                 || (isNaN(parseFloat(item.SupplierPriceUsed)) || parseFloat(item.SupplierPriceUsed) <= 0)
                                 || (isNaN(parseFloat(item.CustomDuties)) || parseFloat(item.CustomDuties) <= 0)
                                 || (isNaN(parseFloat(item.ShippingCost)) || parseFloat(item.ShippingCost) <= 0)
                                 || (isNaN(parseFloat(item.Warehousing)) || parseFloat(item.Warehousing) <= 0)
                                 || (isNaN(parseFloat(item.SGAProfit)) || parseFloat(item.SGAProfit) <= 0)) {
                            common.usSpinnerService.stop('spnAddEditQuote');
                            common.aaNotify.error(($filter('translate')('_RequiredValidator_')));
                            $scope.isValid = false;
                            return false;
                        }
                    }
                }
            }
        });
    };
    $scope.RedirectToList = function () {
        $modalInstance.dismiss('cancel');
    }

    $scope.GenerateRFQPartQuoteReport = function (rfqId) {
        if (!IsUndefinedNullOrEmpty(rfqId)) {
            $window.open('/RFQ/PartQuoteReportFromQuote/PartQuoteReportFromQuote#/' + rfqId, 'Parts Quote By Supplier', 'resizable=1,width=1000,height=600,top=32,left=32');
        }
    };

    $scope.Init();
}]);
app.directive("changeBackgroundColorQuote", ['$timeout', function ($timeout) {
    return {
        replace: false,
        restrict: 'A',
        link: function (scope, $element, $attrs) {
            var e = $element;
            var index = parseInt($attrs.changeBackgroundColorQuote, 10);
            e.on('blur change', function () {
                scope.setInvalidClass(scope.item, index);
            });
        }
    }
}]);