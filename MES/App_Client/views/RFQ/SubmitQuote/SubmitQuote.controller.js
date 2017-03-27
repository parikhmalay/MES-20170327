app.controller('RFQSupplierPartQuoteCtrl', ['$rootScope', '$scope', 'common', 'RFQSupplierPartQuoteSvc', '$routeParams', '$timeout', '$filter',
    function ($rootScope, $scope, common, RFQSupplierPartQuoteSvc, $routeParams, $timeout, $filter) {
        if (!IsUndefinedNullOrEmpty($routeParams.UniqueUrl)) {
            $scope.UniqueUrl = $routeParams.UniqueUrl;
        }
        $scope.RFQSupplierPartQuoteList = {};
        $rootScope.PageHeader = ($filter('translate')('_PageHeading_'));
        $scope.Init = function () {
            $scope.EnterQuote = true;
            $scope.NoQuote = false;
            $scope.IsForm = true;
            $scope.IsThanks = false;
            $scope.ThanksMsg = '';
            $scope.IsWarning = false;
            $scope.WarningMsg = '';
            $scope.Paging = {};
            //$scope.UniqueUrl = '0439846d-6b62-4950-9a6d-df77d572bf7d'; // temorary for testing
            $scope.SearchCriteria = { RFQId: '', SupplierId: 0, UniqueUrl: $scope.UniqueUrl };
            $scope.Paging.Criteria = $scope.SearchCriteria;
            $scope.GetRFQSupplierPartQuoteList();
        };

        $scope.GetRFQSupplierPartQuoteList = function () {
            common.usSpinnerService.spin('spnRFQSupplierPartQuote');
            RFQSupplierPartQuoteSvc.GetRFQSupplierPartQuoteList($scope.Paging).then(
                 function (response) {
                     common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                     if (response.data.StatusCode == 200) {                  //ShowMessage(common, response.data)
                         $scope.RFQSupplierPartQuoteList = response.data.Result;
                         angular.forEach($scope.RFQSupplierPartQuoteList, function (item, index) {
                             item.MinOrderQty = '';
                             item.ToolingCost = '0.000';
                             item.MaterialCost = '0.000';
                             item.ProcessCost = '0.000';
                             item.MachiningCost = '0.000';
                             item.OtherProcessCost = '0.000';
                             item.NoOfCavities = '0';
                             item.ExchangeRate = '0.000';
                             item.UnitPrice = '0.000';
                             item.rFQdqRawMaterial.MaterialLoss = '0.000';
                             item.rFQdqRawMaterial.RawMatTotal = '0.000';
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

        //Start Calculations and validations
        $scope.CalculateUnitPrice = function (item, key) {
            angular.forEach($scope.RFQSupplierPartQuoteList, function (obj, index) {
                if (index == key) {
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

                    obj.UnitPrice = $filter('number')(prpUnitPrice + ttlRawMatTotal, 3);
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


        $scope.SaveRFQSupplierPartQuote = function () {
            common.usSpinnerService.spin('spnRFQSupplierPartQuote');
            $scope.isValid = true;
            angular.forEach($scope.RFQSupplierPartQuoteList, function (item, index) {
                if ($scope.isValid) {
                    if ((!isNaN(item.MinOrderQty) && parseFloat(item.MinOrderQty) > 0)
                        || (!isNaN(item.ToolingCost) && parseFloat(item.ToolingCost) > 0)
                        || (!isNaN(item.rFQdqRawMaterial.RawMatInputInKg) && parseFloat(item.rFQdqRawMaterial.RawMatInputInKg) > 0)
                        || (!isNaN(item.rFQdqRawMaterial.RawMatCostPerKg) && parseFloat(item.rFQdqRawMaterial.RawMatCostPerKg) > 0)
                        || (!isNaN(item.ProcessCost) && parseFloat(item.ProcessCost) > 0)
                        || (!isNaN(item.MachiningCost) && parseFloat(item.MachiningCost) > 0)
                        || (!isNaN(item.OtherProcessCost) && parseFloat(item.OtherProcessCost) > 0)
                        || (!isNaN(item.NoOfCavities) && parseFloat(item.NoOfCavities) > 0)) {
                        if (isNaN(parseFloat(item.MinOrderQty))
                             || isNaN(item.rFQdqRawMaterial.RawMatInputInKg) || parseFloat(item.rFQdqRawMaterial.RawMatInputInKg) <= 0
                             || isNaN(item.rFQdqRawMaterial.RawMatCostPerKg) || parseFloat(item.rFQdqRawMaterial.RawMatCostPerKg) <= 0
                             || isNaN(item.ProcessCost) || parseFloat(item.ProcessCost) <= 0
                             || isNaN(item.NoOfCavities) || parseFloat(item.NoOfCavities) <= 0) {
                            common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                            common.aaNotify.error($filter('translate')('_IsRequiredErrMsg_'));//("Please enter valid value for the required field(s).");
                            $scope.isValid = false;
                            return false;
                        }
                    }
                }
            });

            setTimeout(function () {
                if (!$scope.isValid)
                    return false;

                if (!IsObjectEmpty($scope.RFQSupplierPartQuoteList)) {
                    RFQSupplierPartQuoteSvc.SaveSubmitQuote($scope.RFQSupplierPartQuoteList).then(
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
            RFQSupplierPartQuoteSvc.SaveSubmitNoQuote($scope.Paging).then(
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
              });
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

        $scope.DownloadRSPQFile = function () {
            common.usSpinnerService.spin('spnRFQSupplierPartQuote');
            if (!IsUndefinedNullOrEmpty($routeParams.UniqueUrl)) {
                $scope.SearchCriteria.RFQId = $scope.RFQSupplierPartQuoteList[0].RfqId;
                $scope.SearchCriteria.UniqueUrl = $scope.UniqueUrl;
                $scope.SearchCriteria.UploadQuoteFilePath = $scope.RFQSupplierPartQuoteList[0].UploadQuoteFilePath;
                $scope.Paging.Criteria = $scope.SearchCriteria;

                RFQSupplierPartQuoteSvc.downloadRfqSupplierPartQuote($scope.Paging).then(
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
                common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                common.aaNotify.error($filter('translate')('_SelectFile_'));
            }
            else {
                $scope.SearchCriteria.RFQId = $scope.RFQSupplierPartQuoteList.RfqId;
                $scope.SearchCriteria.UniqueUrl = $scope.UniqueUrl;
                $scope.SearchCriteria.UploadQuoteFilePath = $scope.RFQSupplierPartQuoteList[0].UploadQuoteFilePath;
                $scope.Paging.Criteria = $scope.SearchCriteria;

                RFQSupplierPartQuoteSvc.uploadRfqSupplierPartQuote($scope.Paging).then(
                     function (response) {
                         common.usSpinnerService.stop('spnRFQSupplierPartQuote');
                         if (response.data.StatusCode == 200) {
                             $scope.RFQSupplierPartQuoteList = response.data.Result;
                             angular.forEach($scope.RFQSupplierPartQuoteList, function (item) {
                                 item.UnitPrice = $filter('number')(item.UnitPrice, 3);
                                 item.rFQdqRawMaterial.MaterialLoss = $filter('number')(IsUndefinedNullOrEmpty(item.rFQdqRawMaterial.MaterialLoss) ? 0 : item.rFQdqRawMaterial.MaterialLoss, 3);
                                 item.rFQdqRawMaterial.RawMatTotal = $filter('number')(IsUndefinedNullOrEmpty(item.rFQdqRawMaterial.RawMatTotal) ? 0 : item.rFQdqRawMaterial.RawMatTotal, 3);
                               
                             });

                             $scope.SearchCriteria.UploadQuoteFilePath = '';
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

        $scope.RedirectToList = function () {
            common.$location.path("/" + $scope.UniqueUrl);
        }
        $scope.ResetForm = function () {
            common.$route.reload();
        }
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
