app.controller('RFQPartCostComparisonPopupCtrl', ['$rootScope', '$scope', 'common', 'SupplierQuoteSvc', '$modal', '$timeout', '$filter', '$window', '$routeParams', '$modalInstance', 'RFQData',
function ($rootScope, $scope, common, SupplierQuoteSvc, $modal, $timeout, $filter, $window, $routeParams, $modalInstance, RFQData) {
    $scope.PartQuoteList = {};
    $scope.RFQ = {

    };
    $scope.Quote = {
        lstQuoteDetails: []
    };

    $scope.SelectAll = false;

    $scope.Init = function () {
        $scope.RFQData = RFQData;
        $scope.RFQData.Date = convertUTCDateToLocalDate($scope.RFQData.Date);
        $scope.RFQData.QuoteDueDate = convertUTCDateToLocalDate($scope.RFQData.QuoteDueDate);

        $timeout(function () {
            if (!IsUndefinedNullOrEmpty($scope.RFQData.Id) && $scope.RFQData.Id != '') {
                $scope.GetList($scope.RFQData.Id);
            }
        }, 1000);
    };

    $scope.onChangeParts = function (supplierId, partId) {  
        angular.forEach($scope.RFQ.lstRFQPart, function (o) {
            if (o.Id == partId) {
                angular.forEach(o.lstRFQPartCostComparison, function (item) {
                    if (item.rfqPartId == partId && item.SupplierId == supplierId)
                    { item.rdoSelect = true; }
                    else
                    { item.rdoSelect = false; }
                });
            }
        });      
    };

    //Start logic for radio select deselect here

    $scope.SelectedParts = function () {
        var exitLoop = 0;
        $scope.PartIds = [];
        angular.forEach($scope.RFQ.lstRFQPart, function (o) {
            exitLoop = 0;
            angular.forEach(o.lstRFQPartCostComparison, function (item) {
                if (exitLoop == 0) {
                    if (item.rdoSelect) {
                        $scope.Quote.lstQuoteDetails.push({ PartId: item.rfqPartId, SupplierId: item.SupplierId });
                        exitLoop = 1;
                    }
                }
            });
        });
    };
    //end logic for radio select deselect here   
    $scope.GetList = function (rfqId) {
        SupplierQuoteSvc.getRFQPartCostComparisonList(rfqId).then(
              function (response) {
                  if (response.data.StatusCode == 200) {
                      $scope.RFQ = response.data.Result;
                      angular.forEach($scope.RFQ.lstRFQPart, function (o) {
                          angular.forEach(o.lstRFQPartCostComparison, function (p) {
                              p.UpdatedDate = convertUTCDateToLocalDate(p.UpdatedDate);
                          });
                      });
                      $scope.$broadcast("loadScrollerPanel");
                  }
              },
             function (error) {
                 common.usSpinnerService.stop('spnRFQPartQuote');
                 //common.aaNotify.error(error);
             });
    };
    $scope.CreateQuote = function (quote) {
 
        common.usSpinnerService.spin('spnCreateQuote');
        $scope.Quote.lstQuoteDetails = [];
        $scope.Quote.RfqId = $scope.RFQData.Id;
        $scope.SelectedParts();
        if (!IsObjectEmpty($scope.Quote)) {
            $scope.RedirectToList();
            $scope.QuoteToCustomerPopup();
        }
        else {
            common.usSpinnerService.stop('spnCreateQuote');
        }
    };
    $scope.BackToRFQ = function () {
        //$window.location.href = "/RFQ/RFQ/RFQ#/AddEdit/" + RFQId + "/0";
        $modalInstance.dismiss('cancel');
    };
    $scope.RedirectToList = function () {
        $modalInstance.dismiss('cancel');
    };
    $scope.Init();

    //popup for quote to customer
    $scope.QuoteToCustomerPopup = function (quote) {
        var modalQuoteToCustomer = $modal.open({
            templateUrl: '/App_Client/views/RFQ/Quote/QuoteToCustomerPopup.html?v=' + Version,
            controller: 'QuoteToCustomerPopupCtrl',
            keyboard: true,
            backdrop: false,
            scope: $scope,
            resolve: {
                QuoteId: function () {
                    return '';
                },
                QuoteData: function () {
                    return $scope.Quote;
                },
                RFQData: function () {
                    return RFQData;
                },
                IsRevision: function () {
                    return false;
                },
            },
            sizeclass: 'modal-extra-full modal-fitToScreen'
        });
        modalQuoteToCustomer.result.then(function () {
        }, function () {
        });
    };
    //quote to customer end here

}]);

