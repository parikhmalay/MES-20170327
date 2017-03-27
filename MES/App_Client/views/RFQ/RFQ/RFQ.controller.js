app.controller('RFQCtrl', ['$rootScope', '$scope', 'common', 'RFQSvc', 'IdentitySvc', '$modal', '$filter', 'LookupSvc', '$sce', '$confirm', '$window', '$timeout', function ($rootScope, $scope, common, RFQSvc, IdentitySvc, $modal, $filter, LookupSvc, $sce, $confirm, $window, $timeout) {
    //Start implement security role wise
    $scope.setRoleWisePrivilege = function () {
        $scope.currentSecurityObject = currentSecurityObject;
        if (!Isundefinedornull($scope.currentSecurityObject) && $scope.currentSecurityObject.length > 0) {
            angular.forEach($scope.currentSecurityObject, function (obj, index) {
                switch (obj.ObjectId) {
                    case 9:
                        switch (obj.PrivilegeId) {
                            case 1:                           //none
                                RedirectToAccessDenied();
                                break;
                            case 2:                          //read only
                                $scope.IsDisableDeleteListButton = true;
                                break;
                            case 3:                         //write
                                // $scope.IsDisableDeleteListButton = false;
                                break;
                        }
                        break;
                    case 10:
                        $scope.AddModeSecurityRole(obj);
                        break;
                    case 11:
                        $scope.EditModeSecurityRole(obj);
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
    $scope.AddModeSecurityRole = function (obj) {
        switch (obj.PrivilegeId) {
            case 1:                           //none
                $scope.IsDisableAddNewButton = true;
                break;
            case 2:                          //read only
                $scope.IsDisableAddNewButton = true;
                break;
            case 3:                         //write
                //$scope.IsDisableAddNewButton = false;
                break;
        }
    };
    $scope.EditModeSecurityRole = function (obj) {
        switch (obj.PrivilegeId) {
            case 1:                           //none
                $scope.IsDisableEditButton = true;
                break;
            case 2:                          //read only
                $scope.IsDisableEditButton = false;
                break;
            case 3:                         //write
                // $scope.IsDisableEditButton = false;
                break;
        }
    };

    //End implement security role wise
    $scope.setRoleWisePrivilege();
    $scope.SearchCriteria = {};
    $scope.sortReverse = false;
    $rootScope.PageHeader = ($filter('translate')('_RFQList_'));

    //$scope.dSearchCriteria = {};
    //$scope.SearchCriteria.isFirstTimeLoad = true;

    $scope.EditRFQSupplier = function (rfqId, supplierId) {
        $window.location.href = "/RFQ/SQ/SupplierQuote#/" + rfqId + "/" + supplierId;
    };

    $scope.Init = function () {
        $scope.Paging = GetDefaultPageObject();
        $scope.Paging.Criteria = $scope.SearchCriteria;
        $scope.SetLookupData();

        //if ($scope.SearchCriteria.isFirstTimeLoad == true) {
        //    $scope.GetDefaultSearchCriteria();
        //}

        $timeout(function () {
            ///Start set search criteria when return from add edit page
            if (localStorage.getItem("RFQListPaging") && localStorage.getItem("RFQListPageSearchCriteria")) {
                $scope.Paging = JSON.parse(localStorage.getItem("RFQListPaging"));
                localStorage.removeItem("RFQListPaging");
                $scope.SearchCriteria = JSON.parse(localStorage.getItem("RFQListPageSearchCriteria"));
                $scope.SearchCriteria.RfqDateFrom = convertUTCDateToLocalDate($scope.SearchCriteria.RfqDateFrom);
                $scope.SearchCriteria.RfqDateTo = convertUTCDateToLocalDate($scope.SearchCriteria.RfqDateTo);
                localStorage.removeItem("RFQListPageSearchCriteria");
            }
            ///End set search criteria when return from add edit page

            //Start set dates when called from dashboard
            if (localStorage.getItem("DateFrom")) {
                $scope.SearchCriteria.RfqDateFrom = convertUTCDateToLocalDate(localStorage.getItem("DateFrom"));
                localStorage.removeItem("DateFrom");
            }
            if (localStorage.getItem("DateTo")) {
                $scope.SearchCriteria.RfqDateTo = convertUTCDateToLocalDate(localStorage.getItem("DateTo"));
                localStorage.removeItem("DateTo");
            }
            //End set dates when called from dashboard    

            //if ($scope.SearchCriteria.isFirstTimeLoad == false)

            $scope.GetRFQList();
        }, 1000);
    };

    $scope.GetDefaultSearchCriteria = function () {
        common.usSpinnerService.spin('spnRFQList');
        IdentitySvc.DefaultSearchCriteria().then(
             function (response) {
                 common.usSpinnerService.stop('spnRFQList');
                 if (response.data.StatusCode == 200) {
                     $scope.dSearchCriteria = response.data.Result;
                     $scope.SearchCriteria.SAMItems = $scope.dSearchCriteria.SAMItems;
                     $scope.SearchCriteria.rfqCoordinator = $scope.dSearchCriteria.rfqCoordinator;
                     //$scope.SearchCriteria.isFirstTimeLoad = $scope.dSearchCriteria.isFirstTimeLoad;
                     //Start set dates when called from dashboard
                     if (localStorage.getItem("DateFrom")) {
                         $scope.SearchCriteria.RfqDateFrom = convertUTCDateToLocalDate(localStorage.getItem("DateFrom"));
                         localStorage.removeItem("DateFrom");
                     }
                     if (localStorage.getItem("DateTo")) {
                         $scope.SearchCriteria.RfqDateTo = convertUTCDateToLocalDate(localStorage.getItem("DateTo"));
                         localStorage.removeItem("DateTo");
                     }
                     //End set dates when called from dashboard    
                     $scope.GetRFQList();
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
         function (error) {
             common.usSpinnerService.stop('spnRFQList');
             //common.aaNotify.error(error);
         });
    };
    $scope.SetLookupData = function () {
        $scope.LookUps = [
          {
              "Name": "RFQCoordinators", "Parameters": {}
          },
           {
               "Name": "RFQSources", "Parameters": {}
           },
           {
               "Name": "Commodity", "Parameters": {}
           },
           {
               "Name": "Process", "Parameters": {}
           },
            {
                "Name": "SAM", "Parameters": {}
            },
            {
                "Name": "RFQPriority", "Parameters": {}
            }
        ];
        $scope.getLookupData();
    };

    $scope.getLookupData = function () {
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            angular.forEach(data.data, function (o) {
                if (o.Name === "RFQCoordinators") {
                    $scope.RFQCoordinatorList = o.Data;
                }
                else if (o.Name === "RFQSources") {
                    $scope.RFQSourceList = o.Data;
                }
                else if (o.Name === "Commodity") {
                    $scope.CommodityList = o.Data;
                }
                else if (o.Name === "Process") {
                    $scope.ProcessList = o.Data;
                }
                else if (o.Name === "SAM") {
                    $scope.SAMList = o.Data;
                }
                else if (o.Name === "RFQPriority") {
                    $scope.RFQPriorityList = o.Data;
                }
            });
        });
    }

    $scope.GetRFQList = function () {
        common.usSpinnerService.spin('spnRFQList');
        $scope.Paging.Criteria = $scope.SearchCriteria;
        RFQSvc.GetRFQList($scope.Paging).then(
             function (response) {
                 if (response.data.StatusCode == 200) {
                     $scope.RFQList = response.data.Result;

                     if ($scope.RFQList.length > 0)
                         advanceSearch.close();

                     var legendHtml;
                     angular.forEach($scope.RFQList, function (o) {
                         o.Date = convertLocalDateToUTCDate(o.Date);
                         o.QuoteDueDate = convertLocalDateToUTCDate(o.QuoteDueDate);

                         switch (o.StatusLegend) {

                             case "V":
                                 o.StatusLegend = '<span class="legend-lp void" title="VOID"><i class="fa fa-ban"></i></span>';
                                 break;
                             case "C":
                                 o.StatusLegend = '<span class="legend-lp completed" title="COMPLETED"><i class="fa fa-check"></i></span>';
                                 break;
                             case "H":
                                 o.StatusLegend = '<span class="legend-lp on-hold" title="ON HOLD"><i class="fa fa-lock"></i></span>';
                                 break;

                             case "IC":
                                 o.StatusLegend = '<span class="legend-lp pending-incomplete" title="PENDING/INCOMPLETE"><i class="fa fa-star-half-empty"></i></span>';
                                 break;
                             case "QD":
                                 o.StatusLegend = '<span class="legend-lp quote-due" title="QUOTE DUE"><i class="fa fa-star-half-empty"></i></span>';
                                 break;
                             case "NR":
                                 o.StatusLegend = '<span class="legend-lp need-revision" title="NEED REVISION"><i class="fa fa-exclamation-triangle"></i></span>';
                                 break;
                             case "R":
                             default:
                                 o.StatusLegend = '<span class="legend-lp release" title="RELEASE"><i class="fa fa-unlock"></i></span>';
                                 break;
                         }

                         /* if (o.lstQuotedSuppliers.length > 0) {
                              angular.forEach(o.lstQuotedSuppliers, function (io) {
                                  io.QuoteDate = convertUTCDateToLocalDate(io.QuoteDate);
                              });
                          }
 
                          if (o.lstQuoteToCustomer.length > 0) {
                              angular.forEach(o.lstQuoteToCustomer, function (io) {
                                  io.CreatedDate = convertUTCDateToLocalDate(io.CreatedDate);
                              });
                          }*/

                     });
                     $scope.Paging = response.data.PageInfo;
                     $scope.Paging.Criteria = $scope.SearchCriteria;
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
                 $timeout(function () {
                     common.usSpinnerService.stop('spnRFQList');
                 }, 0);
             },
             function (error) {
                 common.usSpinnerService.stop('spnRFQList');
                 //common.aaNotify.error(error);
             });
    };

    $scope.to_trusted = function (html_code) {
        return $sce.trustAsHtml(html_code);
    }

    $rootScope.$on('updateQuoteToCustomerList', function (e, args) {
        $scope.getQuoteToCustomer(args.Id);
    });
    $scope.ShowQuotesPanel = function (rfqId, $index) {
        $scope.GetQuotedSuppliers(rfqId, $index);
        $scope.getQuoteToCustomer(rfqId, $index);
    };
    $scope.GetQuotedSuppliers = function (rfqId, $index) {

        RFQSvc.GetQuotedSuppliers(rfqId).then(function (response) {
            common.usSpinnerService.spin('spnQuotesPanel_' + $index);
            if (response.data.StatusCode == 200) {
                var breakLoop = 0;

                angular.forEach($scope.RFQList, function (o) {
                    if (o.Id == rfqId && breakLoop == 0) {
                        o.lstQuotedSuppliers = response.data.Result;
                        if (o.lstQuotedSuppliers.length > 0) {
                            angular.forEach(o.lstQuotedSuppliers, function (io) {
                                io.QuoteDate = convertUTCDateToLocalDate(io.QuoteDate);
                            });
                        }
                        breakLoop == 1;
                    }
                });
                common.usSpinnerService.stop('spnQuotesPanel_' + $index);
            }
            else {
                console.log(response.data.ErrorText);
            }
        },
        function (error) {
            common.usSpinnerService.stop('spnQuotesPanel_' + $index);
            //common.aaNotify.error(error);
        });
    };
    $scope.getQuoteToCustomer = function (rfqId, $index) {
        common.usSpinnerService.spin('spnQTCPanel_' + $index);
        RFQSvc.getQuoteToCustomerList(rfqId).then(function (response) {
            if (response.data.StatusCode == 200) {
                var breakLoop = 0;

                angular.forEach($scope.RFQList, function (o) {
                    if (o.Id == rfqId && breakLoop == 0) {
                        o.lstQuoteToCustomer = response.data.Result;
                        if (o.lstQuoteToCustomer.length > 0) {
                            angular.forEach(o.lstQuoteToCustomer, function (io) {
                                io.CreatedDate = convertUTCDateToLocalDate(io.CreatedDate);
                            });
                        }
                        breakLoop == 1;
                    }
                });

                common.usSpinnerService.stop('spnQTCPanel_' + $index);
            }
            else {
                console.log(response.data.ErrorText);
            }
        },
        function (error) {
            common.usSpinnerService.stop('spnQTCPanel_' + $index);
            //common.aaNotify.error(error);
        });
    };

    $scope.Edit = function (id, customerName) {
        localStorage.setItem("RFQListPaging", JSON.stringify($scope.Paging));
        localStorage.setItem("RFQListPageSearchCriteria", JSON.stringify($scope.SearchCriteria));
        $confirm({ title: ($filter('translate')('_ConfirmText_')), ok: ($filter('translate')('_Yes_')), cancel: ($filter('translate')('_No_')), close: 'C', text1: 'RFQ#: ' + id, text2: 'Customer Name: ' + customerName })
        .then(function (ret) {
            if (ret == 1)
                return;
            else
                common.$location.path("/AddEdit/" + id + "/1");
        }
        ,
        function () {
            common.$location.path("/AddEdit/" + id + "/0");
        }
        );
    };
    $scope.Delete = function (rfqId) {
        common.usSpinnerService.spin('spnRFQList');
        if (confirm($filter('translate')('_DeleteRfqConfirmText_'))) {
            RFQSvc.Delete(rfqId).then(
           function (response) {
               common.usSpinnerService.stop('spnRFQList');
               if (ShowMessage(common, response.data)) {
                   $scope.GetRFQList();
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnRFQList');
               //common.aaNotify.error(error);
           });
        }
        common.usSpinnerService.stop('spnRFQList');
    };  

    $scope.pageSizeChanged = function (PageSize) {
        $scope.Paging.PageSize = PageSize;
        $scope.GetRFQList();
    };

    $scope.pageChanged = function (PageNo) {
        $scope.Paging.PageNo = PageNo;
        $scope.GetRFQList();
    };
    $scope.Search = function () {
        if (!IsObjectEmpty($scope.SearchCriteria.CommodityItems)) {
            $scope.Commodity = [];
            angular.forEach($scope.SearchCriteria.CommodityItems, function (item) {
                if (!Isundefinedornull(item.Id) && item.Id > 0)
                    $scope.Commodity.push(item.Id);
            });
            $scope.SearchCriteria.Commodity = $scope.Commodity.join(",");
        }
        if (!IsObjectEmpty($scope.SearchCriteria.ProcessItems)) {
            $scope.Process = [];
            angular.forEach($scope.SearchCriteria.ProcessItems, function (item) {
                if (!Isundefinedornull(item.Id) && item.Id > 0)
                    $scope.Process.push(item.Id);
            });
            $scope.SearchCriteria.Process = $scope.Process.join(",");
        }
        if (!IsObjectEmpty($scope.SearchCriteria.SAMItems)) {
            $scope.SAM = [];
            angular.forEach($scope.SearchCriteria.SAMItems, function (item) {
                if (!Isundefinedornull(item.Id))
                    $scope.SAM.push(item.Id);
            });
            $scope.SearchCriteria.SAM = $scope.SAM.join(",");
        }
        if (!IsObjectEmpty($scope.SearchCriteria.RFQPriorityItems)) {
            $scope.RFQPriority = [];
            angular.forEach($scope.SearchCriteria.RFQPriorityItems, function (item) {
                if (!Isundefinedornull(item.Id) && item.Id > 0)
                    $scope.RFQPriority.push(item.Id);
            });
            $scope.SearchCriteria.rfqPriority = $scope.RFQPriority.join(",");
        }
        //$scope.SearchCriteria.isFirstTimeLoad = false;
        $scope.Init();
    };
    $scope.ResetSearch = function () {
        $scope.SearchCriteria = { CommodityItems: {}, ProcessItems: {}, SAMItems: [], RFQPriorityItems: {} };
        //$scope.SearchCriteria.isFirstTimeLoad = false;
        $scope.Init();
    }

    //popup for show audit log 
    $scope.ShowChangeLogPopup = function (transactionId) {
        $scope.TransactionId = transactionId;
        $scope.TableName = 'RFQ';
        $scope.SchemaName = 'RFQ';
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

    //popup for supplier quote
    $scope.SupplierQuotePopup = function (rfqId, supplierId, RFQData) {
        var modalSupplierQuote = $modal.open({
            templateUrl: '/App_Client/views/RFQ/SupplierQuote/RFQSupplierQuotePopup.html?v=' + Version,
            controller: 'RFQSupplierQuotePopupCtrl',
            keyboard: true,
            backdrop: false,
            scope: $scope,
            resolve: {
                RFQId: function () {
                    return rfqId;
                },
                SupplierId: function () {
                    return supplierId;
                },
                RFQData: function () {
                    return RFQData;
                }
            },
            sizeclass: 'modal-extra-full modal-fitToScreen'
        });
        modalSupplierQuote.result.then(function () {
        }, function () {
        });
    };
    // supplier quote popup end here

    //popup for quote to customer
    $scope.EditQuoteToCustomer = function (qItem, RFQData) {

        $confirm({ title: ($filter('translate')('_RevisionConfirmTextForQuote_')), ok: ($filter('translate')('_Yes_')), cancel: ($filter('translate')('_No_')), close: 'C', text1: 'Quote #: ' + qItem.QuoteNumber, text2: 'Customer Name: ' + RFQData.CustomerName })
        .then(function (ret) {
            if (ret == 1)
                return;
            else
                $scope.QuoteToCustomerPopup(qItem.Id, RFQData, 1);
        }
        ,
        function () {
            $scope.QuoteToCustomerPopup(qItem.Id, RFQData, 0);
        }

        );
    };
    $scope.QuoteToCustomerPopup = function (Id, RFQData, isR) {
        var modalQuoteToCustomer = $modal.open({
            templateUrl: '/App_Client/views/RFQ/Quote/QuoteToCustomerPopup.html?v=' + Version,
            controller: 'QuoteToCustomerPopupCtrl',
            keyboard: true,
            backdrop: false,
            scope: $scope,
            resolve: {
                QuoteId: function () {
                    return Id;
                },
                RFQData: function () {
                    return RFQData;
                },
                QuoteData: function () {
                    return [];
                },
                IsRevision: function () {
                    return isR;
                },
            },
            sizeclass: 'modal-extra-full modal-fitToScreen'
        });
        modalQuoteToCustomer.result.then(function () {
        }, function () {
        });
    };
    //quote to customer end here

    // quote to customer section
    $scope.DownloadQuotePDF = function (path) {
        common.usSpinnerService.spin('spnRFQList');
        if (!IsUndefinedNullOrEmpty(path)) {
            window.open(path, '_blank');
        }
        common.usSpinnerService.stop('spnRFQList');
    };

    $scope.DownloadExtPDF = function (path) {
        common.usSpinnerService.spin('spnRFQList');
        if (!IsUndefinedNullOrEmpty(path)) {
            window.open(path, '_blank');
        }
        common.usSpinnerService.stop('spnRFQList');
    };
    // quote to customer section

    //popup for rfq part cost comparison
    $scope.RFQPartCostComparisonPopup = function (RFQData) {
        var modalRFQPartCostComparison = $modal.open({
            templateUrl: '/App_Client/views/RFQ/SupplierQuote/RFQPartCostComparisonPopup.html?v=' + Version,
            controller: 'RFQPartCostComparisonPopupCtrl',
            keyboard: true,
            backdrop: false,
            scope: $scope,
            resolve: {
                RFQData: function () {
                    return RFQData;
                }
            },
            sizeclass: 'modal-extra-full modal-fitToScreen'
        });
        modalRFQPartCostComparison.result.then(function () {
        }, function () {
        });
    };
    //rfq part cost comparison end here

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

app.controller('AddEditRFQCtrl', ['$rootScope', '$scope', 'common', 'RFQSvc', '$modal', 'LookupSvc', 'EmailTemplateSvc', '$sce', '$filter', '$routeParams', '$timeout', '$window',
function ($rootScope, $scope, common, RFQSvc, $modal, LookupSvc, EmailTemplateSvc, $sce, $filter, $routeParams, $timeout, $window) {
    //Start implement security role wise
    $scope.setRoleWisePrivilege = function () {
        $scope.IsReadOnlyFinalSaveButton = true;
        $scope.currentSecurityObject = currentSecurityObject;
        if (!Isundefinedornull($scope.currentSecurityObject) && $scope.currentSecurityObject.length > 0) {
            angular.forEach($scope.currentSecurityObject, function (obj, index) {
                switch (obj.ObjectId) {
                    case 9:
                        switch (obj.PrivilegeId) {
                            case 1:                           //none
                                RedirectToAccessDenied();
                                break;
                        }
                        break;
                    case 10:
                        if ($scope.TransactionMode == "Create") {
                            $scope.AddEditSecurityRolePage(obj);
                        }
                        break;
                    case 11:
                        if ($scope.TransactionMode == "Edit") {
                            $scope.AddEditSecurityRolePage(obj);
                        }
                        break;
                    case 68:
                        $scope.AddEditSecurityRoleRFQParts(obj);
                        break;
                    case 69:
                        switch (obj.PrivilegeId) {
                            case 1:
                                $scope.IsHideRFQSuppliers = true;
                                break;
                            case 2:
                                $scope.IsReadOnlyRFQSuppliers = true;
                                break;
                            case 3:
                                $scope.IsReadOnlyRFQSuppliers = false;
                                break;
                        }
                        break;
                    case 90:
                        switch (obj.PrivilegeId) {
                            case 1:
                                $scope.IsHideRFQCloseoutEmail = true;
                                break;
                            case 2:
                                $scope.IsReadOnlyRFQCloseoutEmail = true;
                                break;
                            case 3:
                                $scope.IsReadOnlyRFQCloseoutEmail = false;
                                break;
                        }
                        break;
                    default:
                        break;
                }
            });
            if ($scope.IsReadOnlyAddEdit) {

            }
        }
        else {
            RedirectToAccessDenied();
        }
    }
    $scope.AddEditSecurityRolePage = function (obj) {
        switch (obj.PrivilegeId) {
            case 1:                           //none
                RedirectToAccessDenied();
                break;
            case 2:                          //read only
                $scope.IsReadOnlyAddEdit = true;
                break;
            case 3:                         //write
                $scope.IsReadOnlyAddEdit = false;
                $scope.IsReadOnlyFinalSaveButton = false;
                break;
        }
    };
    $scope.AddEditSecurityRoleRFQParts = function (obj) {
        switch (obj.PrivilegeId) {
            case 1:
                $scope.IsHideRFQParts = true;
                break;
            case 2:
                $scope.IsReadOnlyRFQParts = true;
                if (!Isundefinedornull(obj.childObject) && obj.childObject.length > 0) {
                    var EditPartObject = $filter('filter')(obj.childObject, function (rw) { return rw.ObjectId == 23 });
                    if (EditPartObject.length > 0) {
                        if (EditPartObject[0].PrivilegeId == 3) {
                            $scope.IsReadOnlyRFQParts = false;
                            $scope.IsReadOnlyFinalSaveButton = false;
                            $scope.IsReadOnlyAddNewPartButton = true;
                        }
                    }
                    var AddPartObject = $filter('filter')(obj.childObject, function (rw) { return rw.ObjectId == 22 });
                    if (AddPartObject.length > 0) {
                        if (AddPartObject[0].PrivilegeId == 3) {
                            $scope.IsReadOnlyRFQParts = false;
                            $scope.IsReadOnlyFinalSaveButton = false;
                            $scope.IsReadOnlyAddNewPartButton = false;
                        }
                    }
                }
                break;
            case 3:
                $scope.IsReadOnlyRFQParts = false;
                $scope.IsReadOnlyFinalSaveButton = false;
                if (!Isundefinedornull(obj.childObject) && obj.childObject.length > 0) {
                    var EditPartObject = $filter('filter')(obj.childObject, function (rw) { return rw.ObjectId == 23 });
                    if (EditPartObject.length > 0) {
                        if (EditPartObject[0].PrivilegeId == 1 || EditPartObject[0].PrivilegeId == 2) {
                            $scope.IsReadOnlyRFQParts = true;
                            //$scope.IsReadOnlyFinalSaveButton = true;
                        }
                        else if (EditPartObject[0].PrivilegeId == 3) {
                            $scope.IsReadOnlyRFQParts = false;
                            $scope.IsReadOnlyFinalSaveButton = false;
                            $scope.IsReadOnlyAddNewPartButton = true;
                        }
                    }
                    var AddPartObject = $filter('filter')(obj.childObject, function (rw) { return rw.ObjectId == 22 });
                    if (AddPartObject.length > 0) {
                        if (AddPartObject[0].PrivilegeId == 1 || AddPartObject[0].PrivilegeId == 2) {
                            $scope.IsReadOnlyAddNewPartButton = true;
                        }
                        else if (AddPartObject[0].PrivilegeId == 3) {
                            $scope.IsReadOnlyRFQParts = false;
                            $scope.IsReadOnlyFinalSaveButton = false;
                            $scope.IsReadOnlyAddNewPartButton = false;
                        }
                    }
                }
                break;
        }
    };
    //End implement security role wise

    $('body').removeClass('paginationFixedToBottom haveAdvanceSearch');
    $scope.todayDate = new Date();
    $scope.AvailableSuppliersList = [];
    $scope.RFQSuppliersList = [];
    $scope.SearchCriteria = {};
    $scope.EmailData = {
        Ids: [],
        EmailIdsList: [],
        BCCEmailIds: '',
        EmailSubject: '',
        EmailBody: '',
        EmailBodyRFQCloseout: '',
        EmailAttachment: '',
        EmailTypeId: null,
        AttachRFQPDF: null,
        RFQId: '',
        lstEmailDocumentAttachment: []
    };
    $rootScope.PageHeader = ($filter('translate')('_PageHeading_'));
    $scope.sortReverse = false;
    $scope.SelectAllObj = { SelectAllAvailSup: false, SelectAllRFQSup: false };

    $scope.Init = function () {
        $scope.selectedFiles = [];
        $scope.RFQ = {
            lstRFQPart: []
        };
        $scope.RFQ.Date = $scope.todayDate;
        $scope.RFQ.Status = 'H';
        $scope.RFQ.isRevision = false;
        $scope.RFQPart = { RfqPartAttachmentList: [] };
        /* Value    Status         Code       
         * 1        Hold            H
         * 2        Release         R
         * 3        Completed       C 
         * 4        Void            V*/
        $scope.StatusList =
                    [{ 'name': 'Hold', 'value': 'H' }
                   , { 'name': 'Release', 'value': 'R' }
                   , { 'name': 'Completed', 'value': 'C' }
                   , { 'name': 'Void', 'value': 'V' }];

        /* value  
         * TS
         * ISO
         * None */
        $scope.SupplierRequirementList =
                  [{ 'name': 'TS', 'value': 'TS' }
                 , { 'name': 'ISO', 'value': 'ISO' }
                 , { 'name': 'None', 'value': 'None' }];

        $scope.Paging = GetDefaultPageObject();
        $scope.SetLookupData();
        $timeout(function () {
            if (!IsUndefinedNullOrEmpty($routeParams.Id) && $routeParams.Id != '' && !IsUndefinedNullOrEmpty($routeParams.IsR)) {
                $scope.TransactionMode = 'Edit';
                $scope.RFQ.Id = $routeParams.Id;
                $scope.getData($routeParams.Id, $routeParams.IsR);
                $scope.SearchCriteria.RFQId = $routeParams.Id;
                $scope.Paging.Criteria = $scope.SearchCriteria;
                $scope.getAvailableSuppliers();
                $scope.getRFQSuppliers();
                $scope.StatusList.push({ 'name': 'Need Revision', 'value': 'NR' });
            }
            else {
                $scope.TransactionMode = 'Create';
            }
            $scope.setRoleWisePrivilege();
        }, 1000);
    };

    $scope.SetLookupData = function () {
        $scope.LookUps = [
           {
               "Name": "Customers", "Parameters": {}
           },
           {
               "Name": "SAM", "Parameters": {}
           },
           {
               "Name": "Commodity", "Parameters": {}
           },
           {
               "Name": "Process", "Parameters": {}
           },
           {
               "Name": "RFQTypes", "Parameters": {}
           },
           {
               "Name": "RFQPriority", "Parameters": {}
           },
           {
               "Name": "RFQCoordinators", "Parameters": {}
           },
           {
               "Name": "RFQSources", "Parameters": {}
           },
           {
               "Name": "NonAwardReasons", "Parameters": {}
           },
           {
               "Name": "CountryItems", "Parameters": { "SupplierIds": '', "SQIds": '' }
           },
           {
               "Name": "EmailTemplates", "Parameters": {}
           },
           {
               "Name": "RFQSuppliers", "Parameters": { "rfqId": '' + $scope.RFQ.Id + '' }
           },
           {
               "Name": "IndustryTypes", "Parameters": {}
           }
        ];
        $scope.getLookupData();
    };

    $scope.getLookupData = function () {
        LookupSvc.GetLookupByQuery($scope.LookUps).then(function (data) {
            angular.forEach(data.data, function (o) {
                if (o.Name === "Customers") {
                    $scope.CustomerList = o.Data;
                }
                else if (o.Name === "SAM") {
                    $scope.SAMList = o.Data;
                }
                else if (o.Name === "Commodity") {
                    $scope.CommodityList = o.Data;
                }
                else if (o.Name === "Process") {
                    $scope.ProcessList = o.Data;
                }
                else if (o.Name === "RFQTypes") {
                    $scope.RFQTypeList = o.Data;
                }
                else if (o.Name === "RFQCoordinators") {
                    $scope.RFQCoordinatorList = o.Data;
                }
                else if (o.Name === "RFQSources") {
                    $scope.RFQSourceList = o.Data;
                }
                else if (o.Name === "NonAwardReasons") {
                    $scope.NonAwardReasonList = o.Data;
                }
                else if (o.Name === "CountryItems") {
                    $scope.CountryList = o.Data;
                }
                else if (o.Name === "EmailTemplates") {
                    $scope.EmailTemplateList = o.Data;
                }
                else if (o.Name === "RFQSuppliers") {
                    $scope.RFQSupplierList = o.Data;
                }
                else if (o.Name === "RFQPriority") {
                    $scope.RFQPriorityList = o.Data;
                }
                else if (o.Name === "IndustryTypes") {
                    $scope.IndustryTypeList = o.Data;
                }
            });
        });
    }

    $scope.showMsgForParts = function () {
        if (IsUndefinedNullOrEmpty($scope.RFQ.Id))
            common.aaNotify.error(($filter('translate')('_NoRFQAlert_')));
    };

    $scope.showMsg = function () {
        if ($scope.RFQ.lstRFQPart.length == 0)
            common.aaNotify.error(($filter('translate')('_NoPartAlert_')));
        else
            common.aaNotify.error(($filter('translate')('_HoldStatusAlert_')));
    };
    $scope.getCustomerContactLookupData = function () {
        if (!IsUndefinedNullOrEmpty($scope.RFQ.CustomerId) && $scope.RFQ.CustomerId != '0') {
            $scope.CustomerLookUp = [
              {
                  "Name": "CustomerContacts", "Parameters": { "customerId": $scope.RFQ.CustomerId }
              }];

            LookupSvc.GetLookupByQuery($scope.CustomerLookUp).then(function (data) {
                angular.forEach(data.data, function (o) {

                    if (o.Name === "CustomerContacts") {
                        $scope.CustomerContactList = o.Data;
                    }
                });
            });
        }
        else { $scope.CustomerContactList = null; }
    };

    $scope.getData = function (Id, isR) {

        common.usSpinnerService.spin('spnAddEditRFQ');
        RFQSvc.getData(Id, isR).then(function (response) {
            if (ShowMessage(common, response.data)) {
                common.usSpinnerService.stop('spnAddEditRFQ');
                $scope.RFQ = response.data.Result;

                var SAMIdExists = $scope.SAMList.filter(function (l) {
                    if (l.Id == $scope.RFQ.SAMId) {
                        return true;
                    }
                }).length;

                if (SAMIdExists == 0)
                    $scope.RFQ.SAMId = '';

                $scope.getCustomerContactLookupData();
                $scope.RFQ.Date = convertLocalDateToUTCDate($scope.RFQ.Date);
                $scope.RFQ.QuoteDueDate = convertLocalDateToUTCDate($scope.RFQ.QuoteDueDate);
                if (!IsUndefinedNullOrEmpty($scope.RFQ.RfqFilePath))
                    $scope.SetObjectvalues($scope.RFQ.RfqFilePath, $scope.RFQ.RfqFileName, 'R');
                if (IsUndefinedNullOrEmpty($scope.RFQ.UploadPartFilePath))
                    $scope.RFQ.UploadPartFilePath = '';
                if (isR == '1') {
                    $scope.reloadWithId();
                }
            }
            else {
                common.usSpinnerService.stop('spnAddEditRFQ');
            }
        },
           function (error) {
               common.usSpinnerService.stop('spnAddEditRFQ');
               console.log(error);
           });
    };

    $scope.SetObjectvalues = function (file, name, type) {
        if (type == 'R') {
            $scope.RFQ.RfqFilePath = file;
            $scope.RFQ.RfqFileName = name;
        }
        else if (type == 'P') {
            $scope.RFQ.UploadPartFilePath = file;
            $scope.RFQ.RfqPartFileName = name;
        }
    }

    $scope.deleteFile = function (filePath, index, type) {
        if (type == 'R') {
            $scope.RFQ.RfqFilePath = '';
            $scope.RFQ.RfqFileName = '';
        }
        else if (type == 'P')
            $scope.RFQ.UploadPartFilePath = '';
        //FileUploaderSvc.Deletedocument(filePath).then(
        //       function (response) {
        //       },
        //       function (error) {
        //       });
    }

    ////***STEP 2: Start code for RFQPart****/////
    $scope.EditRFQPart = function (RFQPart, index) {

        $scope.RFQPart = RFQPart;
        $scope.RFQPartIndex = index;

        $scope.ShowRFQPartPopup();
    };

    $scope.DeletePart = function (rfqPartId, index) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            if (IsUndefinedNullOrEmpty(rfqPartId)) {
                $scope.RFQ.lstRFQPart.splice(index, 1);
                common.aaNotify.success($filter('translate')('_RFQPartDeleted_'));
            }
            else {
                RFQSvc.DeleteRfqPart(rfqPartId).then(
                    function (response) {
                        if (ShowMessage(common, response.data)) {
                            $scope.RFQ.lstRFQPart.splice(index, 1);
                            $scope.ResetRFQParts();
                        }
                    },
                    function (error) {
                        //common.aaNotify.error(error);
                    });
            }
        };
    };

    $scope.CancelRFQPart = function () {
        $scope.ResetRFQParts();
    };

    $scope.ResetRFQParts = function () {
        $scope.RFQPart = { RfqPartAttachmentList: [] };
        $scope.RFQPartIndex = null;
        $scope.AddRFQPart = false;
    };

    $scope.AddNewRFQPart = function () {
        $scope.selectedFilesforParts = [];
        $scope.AddRFQPart = true;
        if (!IsUndefinedNullOrEmpty($scope.RFQ.lstRFQPart) && $scope.RFQ.lstRFQPart.length > 0) {
            $scope.RFQPart.CustomerPartNo = $scope.RFQ.lstRFQPart[$scope.RFQ.lstRFQPart.length - 1].CustomerPartNo;
            $scope.RFQPart.RevLevel = $scope.RFQ.lstRFQPart[$scope.RFQ.lstRFQPart.length - 1].RevLevel;
            $scope.RFQPart.PartDescription = $scope.RFQ.lstRFQPart[$scope.RFQ.lstRFQPart.length - 1].PartDescription;
            $scope.RFQPart.AdditionalPartDesc = $scope.RFQ.lstRFQPart[$scope.RFQ.lstRFQPart.length - 1].AdditionalPartDesc;
            $scope.RFQPart.MaterialType = $scope.RFQ.lstRFQPart[$scope.RFQ.lstRFQPart.length - 1].MaterialType;
            $scope.RFQPart.EstimatedQty = $scope.RFQ.lstRFQPart[$scope.RFQ.lstRFQPart.length - 1].EstimatedQty;
            $scope.RFQPart.PartWeightKG = $scope.RFQ.lstRFQPart[$scope.RFQ.lstRFQPart.length - 1].PartWeightKG;

            $scope.RFQPartIndex = null;
        }
        $scope.ShowRFQPartPopup();
    };

    $scope.SetUploadforParts = function (file, name) {
        $scope.TempFile = { 'AttachmentPathOnServer': file, 'AttachmentName': name };
        $scope.RFQPart.RfqPartAttachmentList.push($scope.TempFile);
    }
    $scope.deleteFileforParts = function (filePath, index) {
        $scope.RFQPart.RfqPartAttachmentList.splice(index, 1);
    }

    $scope.UploadRFQPartFile = function () {
        common.usSpinnerService.spin('spnAddEditRFQ');
        if (IsUndefinedNullOrEmpty($scope.RFQ.UploadPartFilePath) || ($scope.RFQ.UploadPartFilePath == '')) {
            common.aaNotify.error($filter('translate')('_SelectFile_'));
            common.usSpinnerService.stop('spnAddEditRFQ');
        }
        else {
            RFQSvc.UploadRFQParts($scope.RFQ).then(
                 function (response) {
                     common.usSpinnerService.stop('spnAddEditRFQ');
                     if (response.data.StatusCode == 200) {
                         $scope.RFQ.lstRFQPart = response.data.Result;
                         $scope.RFQ.UploadPartFilePath = '';
                         common.aaNotify.success(response.data.SuccessMessage);
                     }
                     else {
                         common.aaNotify.error(response.data.ErrorText);
                     }
                 },
         function (error) {
             common.usSpinnerService.stop('spnAddEditRFQ');
             //common.aaNotify.error(error);

         });
        }
    };
    //popup for show RFQParts form
    $scope.ShowRFQPartPopup = function () {
        var modalTemplatePreviewOptions = $modal.open({
            templateUrl: '/App_Client/views/RFQ/RFQ/RFQPart.html?v=' + Version,
            controller: ModalRFQPartCtrl,
            keyboard: true,
            backdrop: true,
            scope: $scope,
            sizeclass: 'modal-md'
        });
        modalTemplatePreviewOptions.result.then(function () {
        }, function () {
        });
    };
    // RFQPart popup end here
    ////***End code for RFQPart****/////

    ////***STEP - 3: AVAILABLE SUPPLIERS LIST***////
    $scope.getAvailableSuppliers = function () {
        common.usSpinnerService.spin('spnAddEditRFQ');
        $scope.Paging.Criteria = $scope.SearchCriteria;
        RFQSvc.GetAvailableSuppliersList($scope.Paging).then(
             function (response) {
                 common.usSpinnerService.stop('spnAddEditRFQ');
                 if (response.data.StatusCode == 200) {
                     $scope.AvailableSuppliersList = response.data.Result;
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
     function (error) {
         common.usSpinnerService.stop('spnAddEditRFQ');
         //common.aaNotify.error(error);
     });
    };

    var searchTimeoutDelay;
    $scope.searchGetAvailableSuppliers = function () {
        if (searchTimeoutDelay) {
            $timeout.cancel(searchTimeoutDelay);
        }
        searchTimeoutDelay = $timeout(function () {
            $scope.getAvailableSuppliers();
        }, 1000);
    };

    ////***STEP - 3: RFQ SUPPLIERS LIST***////
    $scope.getRFQSuppliers = function () {
        common.usSpinnerService.spin('spnAddEditRFQ');

        RFQSvc.GetRFQSuppliers($scope.Paging).then(
             function (response) {
                 common.usSpinnerService.stop('spnAddEditRFQ');
                 if (response.data.StatusCode == 200) {
                     $scope.RFQSuppliersList = response.data.Result;

                     angular.forEach($scope.RFQSuppliersList, function (o) {
                         if (o.IsQuoteTypeDQ == true) {
                             o.QuoteTypeDQ = '<span class="legend-lp completed"><i class="fa fa-check" title="DQ"></i></span>';
                         }
                         else
                             o.QuoteTypeDQ = '';

                     });
                 }
                 else {
                     console.log(response.data.ErrorText);
                 }
             },
     function (error) {
         common.usSpinnerService.stop('spnAddEditRFQ');
         //common.aaNotify.error(error);
     });
    };

    $scope.to_trusted = function (html_code) {
        return $sce.trustAsHtml(html_code);
    }

    ////***STEP - 3: SEND RFQ TO AVAILABLE SUPPLIERS (SIMPLIFIED QUOTE)***////
    $scope.SendRFQFunc = function (type) {
        common.usSpinnerService.spin('spnAddEditRFQ');

        $scope.supplierIds = [];

        angular.forEach($scope.AvailableSuppliersList, function (item) {
            if (item.chkSelectAvailSup && !IsUndefinedNullOrEmpty(item.SupplierId))
                $scope.supplierIds.push(item.SupplierId);
        });


        if ($scope.supplierIds.length > 0)
            $scope.SendRFQ(type);
        else {
            common.usSpinnerService.stop('spnAddEditRFQ');
            common.aaNotify.error(($filter('translate')('_AtLeastOneSupplier_')));
        }
    };

    $scope.SendRFQ = function (type) {
        var IsQuoteTypeDQ;
        common.usSpinnerService.spin('spnAddEditRFQ');
        if (type == 'SQ')
            IsQuoteTypeDQ = false;
        else
            IsQuoteTypeDQ = true;
        $scope.supplierData = { "SupplierIdList": [], "IsQuoteTypeDQ": false, "RFQId": '' };
        $scope.supplierData = { "SupplierIdList": $scope.supplierIds, "IsQuoteTypeDQ": IsQuoteTypeDQ, "RFQId": '' + $routeParams.Id + '' };

        RFQSvc.SendRFQToSuppliers($scope.supplierData).then(
             function (response) {
                 if (ShowMessage(common, response.data)) {
                     $scope.ResetSuppliersSection();
                     common.usSpinnerService.stop('spnAddEditRFQ');
                 }
                 else {
                     common.usSpinnerService.stop('spnAddEditRFQ');
                 }
             },
        function (error) {
            common.usSpinnerService.stop('spnAddEditRFQ');
            //common.aaNotify.error(error);
        });
    };

    //popup for send email To RFQ Supplier
    $scope.SendEmail = function () {

        $scope.EmailData = { Ids: [], EmailIdsList: [], EmailSubject: '', EmailBody: '', EmailAttachment: '', RFQId: '' + $routeParams.Id + '', lstEmailDocumentAttachment: [] };
        angular.forEach($scope.RFQSuppliersList, function (item) {

            if (item.chkSelectRFQSup) {
                if (!IsUndefinedNullOrEmpty(item.SupplierId))
                    $scope.EmailData.Ids.push(item.SupplierId);

                if (!IsUndefinedNullOrEmpty(item.Email))
                    $scope.EmailData.EmailIdsList.push(item.Email);
            }
        });
        if ($scope.EmailData.Ids.length > 0)
            $scope.ShowSendEmailPopup();
        else
            common.aaNotify.error($filter('translate')('_AtLeastOneSupplier_'));
    };

    $scope.ShowSendEmailPopup = function () {
        var modalTemplatePreviewOptions = $modal.open({
            templateUrl: '/App_Client/views/RFQ/RFQ/RFQSupplierSendEmail.html?v=' + Version,
            controller: ModalSendEmailCtrl,
            keyboard: true,
            backdrop: true,
            scope: $scope,
            resolve: {
                RFQData: function () {
                    return $scope.RFQ;
                }
            },
            sizeclass: 'modal-md'
        });
        modalTemplatePreviewOptions.result.then(function () {
        }, function () {
        });
    };
    // send email end here
    $scope.ResendQuote = function () {
        $scope.rfqSupplierIds = [];
        $scope.supplierIds = [];
        angular.forEach($scope.RFQSuppliersList, function (item) {
            if (item.chkSelectRFQSup) {
                if (!IsUndefinedNullOrEmpty(item.Id))
                    $scope.rfqSupplierIds.push(item.Id);
                if (!IsUndefinedNullOrEmpty(item.SupplierId))
                    $scope.supplierIds.push(item.SupplierId);
            }
        });
        if ($scope.rfqSupplierIds.length > 0)
            $scope.ResendRFQ();
        else
            common.aaNotify.error(($filter('translate')('_AtLeastOneSupplier_')));
    };
    $scope.ResendRFQ = function () {
        common.usSpinnerService.spin('spnAddEditRFQ');
        $scope.rfqSupplierData = { "RFQSupplierIdList": $scope.rfqSupplierIds, "SupplierIdList": $scope.supplierIds, "RFQId": '' + $routeParams.Id + '' };

        RFQSvc.ResendRFQToSuppliers($scope.rfqSupplierData).then(
             function (response) {
                 if (ShowMessage(common, response.data)) {
                     $scope.ResetSuppliersSection();
                     common.usSpinnerService.stop('spnAddEditRFQ');
                 }
                 else {
                     common.usSpinnerService.stop('spnAddEditRFQ');
                 }
             },
        function (error) {
            common.usSpinnerService.stop('spnAddEditRFQ');
            //common.aaNotify.error(error);
        });
    };

    $scope.EditRFQSupplier = function (rfqId, supplierId) {
        $window.location.href = "/RFQ/SQ/SupplierQuote#/" + rfqId + "/" + supplierId;
    };
    $scope.ResendSingleQuote = function (Id, index) {

        common.usSpinnerService.spin('spnAddEditRFQ');
        $scope.rfqSupplierData = { "Id": Id, "RFQId": '' + $routeParams.Id + '' };

        RFQSvc.ResendRFQToSuppliers($scope.rfqSupplierData).then(
             function (response) {
                 if (ShowMessage(common, response.data)) {
                     $scope.ResetSuppliersSection();
                     common.usSpinnerService.stop('spnAddEditRFQ');
                 }
                 else {
                     common.usSpinnerService.stop('spnAddEditRFQ');
                 }
             },
        function (error) {
            common.usSpinnerService.stop('spnAddEditRFQ');
            //common.aaNotify.error(error);
        });
    };
    $scope.DeleteRFQSupplier = function (Id, index) {
        if (confirm($filter('translate')('_DeleteConfirmText_'))) {
            RFQSvc.DeleteRFQSuppliers(Id).then(
              function (response) {
                  if (ShowMessage(common, response.data)) {
                      $scope.ResetSuppliersSection();
                      common.aaNotify.success(($filter('translate')('_RFQSupplierDeleted_')));
                  }
              },
              function (error) {
                  //common.aaNotify.error(error);
              });
        }
    };

    $scope.ResetSuppliersSection = function () {
        $scope.getAvailableSuppliers();
        $scope.getRFQSuppliers();
    };

    $scope.SetUploadforSendToSupplier = function (file, name) {
        $scope.TempFile = { 'FilePath': file, 'FileName': name };
        $scope.EmailData.lstEmailDocumentAttachment.push($scope.TempFile);
    }
    $scope.deleteFileforSendToSupplier = function (filePath, index) {
        $scope.EmailData.lstEmailDocumentAttachment.splice(index, 1);
    }
    ////***End HERE STEP 3***////

    ////***STEP - 4: RFQ CLOSEOUT - SEND EMAIL  ***////
    //Get email template on change of Email Type ddl
    $scope.GetEmailTemplate = function () {
        if (!IsUndefinedNullOrEmpty($scope.EmailData.EmailTypeId) && $scope.EmailData.EmailTypeId != '0') {
            $scope.getEmailTemplateData($scope.EmailData.EmailTypeId);
        }
        else
            $scope.EmailData.EmailBodyRFQCloseout = null;
    };

    $scope.getEmailTemplateData = function (Id) {

        common.usSpinnerService.spin('spnAddEditRFQ');
        EmailTemplateSvc.getData(Id).then(function (response) {
            if (ShowMessage(common, response.data)) {
                common.usSpinnerService.stop('spnAddEditRFQ');
                $scope.EmailData.EmailBodyRFQCloseout = response.data.Result.EmailBody;
            }
            else {
                common.usSpinnerService.stop('spnAddEditRFQ');
            }
        },
           function (error) {
               common.usSpinnerService.stop('spnAddEditRFQ');
               console.log(error);
           });
    };

    $scope.SendRFQCloseOutEmail = function () {

        if (IsUndefinedNullOrEmpty($scope.EmailData.SendTo)) {
            common.aaNotify.error(($filter('translate')('_SendTo_')) + ' is required.');
            return false;
        }
        else if (IsUndefinedNullOrEmpty($scope.EmailData.EmailSubject)) {
            common.aaNotify.error(($filter('translate')('_EmailSubject_')) + ' is required.');
            return false;
        }

        $scope.EmailData.EmailIdsList.push($scope.EmailData.SendTo);

        common.usSpinnerService.spin('spnAddEditRFQ');
        if ($scope.EmailData.EmailIdsList.length > 0) {
            RFQSvc.SendRFQCloseOut($scope.EmailData).then(
             function (response) {
                 if (ShowMessage(common, response.data)) {
                     common.usSpinnerService.stop('spnAddEditRFQ');
                     $scope.EmailData.lstEmailDocumentAttachment = [];
                 }
                 else {
                     common.usSpinnerService.stop('spnAddEditRFQ');
                 }
             },
             function (error) {
                 common.usSpinnerService.stop('spnAddEditRFQ');
                 //common.aaNotify.error(error);
             });
        }
        else {
            common.usSpinnerService.stop('spnAddEditRFQ');
            common.aaNotify.error($filter('translate')('_AtLeastOneSupplier_'));
        }

    };

    $scope.SetUploadforCloseOut = function (file, name) {
        $scope.TempFile = { 'FilePath': file, 'FileName': name };
        $scope.EmailData.lstEmailDocumentAttachment.push($scope.TempFile);
    }
    $scope.deleteFileforCloseOut = function (filePath, index) {
        $scope.EmailData.lstEmailDocumentAttachment.splice(index, 1);
    }
    ////***END HERE - STEP - 4: RFQ CLOSEOUT - SEND EMAIL  ***////

    //Start logic for checkbox select deselect here
    $scope.SelectDeselectAllAvailSup = function () {
        angular.forEach($scope.AvailableSuppliersList, function (item) {
            if (!item.IsDefault)
                item.chkSelectAvailSup = $scope.SelectAllObj.SelectAllAvailSup;
        });
    };
    $scope.selectAvailSup = function () {
        $scope.SelectAllObj.SelectAllAvailSup = true;
        angular.forEach($scope.AvailableSuppliersList, function (item) {
            if (!item.chkSelectAvailSup)
                $scope.SelectAllObj.SelectAllAvailSup = false;
        });
    };
    $scope.SelectDeselectAllRFQSup = function () {
        angular.forEach($scope.RFQSuppliersList, function (item) {
            item.chkSelectRFQSup = $scope.SelectAllObj.SelectAllRFQSup;
        });
    };
    $scope.selectRFQSup = function () {
        $scope.SelectAllObj.SelectAllRFQSup = true;
        angular.forEach($scope.RFQSuppliersList, function (item) {
            if (!item.chkSelectRFQSup)
                $scope.SelectAllObj.SelectAllRFQSup = false;
        });
    };
    //end logic for checkbox select deselect here

    $scope.SaveRFQ = function (closeForm) {
        common.usSpinnerService.spin('spnAddEditRFQ');
        RFQSvc.Save($scope.RFQ).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   common.usSpinnerService.stop('spnAddEditRFQ');
                   $scope.Id = response.data.Result; // Id of latest created record
                   $scope.RFQ.Id = response.data.Result;
                   $scope.RFQ.isRevision = false;

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
                   common.usSpinnerService.stop('spnAddEditRFQ');
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnAddEditRFQ');
               console.log(error);
           });
    }

    $scope.reloadWithId = function () {
        common.$location.path('/AddEdit/' + $scope.RFQ.Id + "/0");
    };
    $scope.RedirectToList = function () {
        common.$location.path("/");
    }

    $scope.ResetForm = function () {
        common.$route.reload();
    }
    $scope.DownloadPDF = function (blobURL) {
        common.usSpinnerService.spin('spnRFQList');
        console.log(blobURL);
        // for non-IE    
        debugger;
        var reader = new FileReader();
        reader.readAsDataURL(blobURL);
        reader.onload = function (event) {
            var save = document.createElement('a');
            save.href = event.target.result;
            save.target = '_blank';
            save.download = 'unknown file';

            var event = document.createEvent('Event');
            event.initEvent('click', true, true);
            save.dispatchEvent(event);
            (window.URL || window.webkitURL).revokeObjectURL(save.href);
            
        };
      

        common.usSpinnerService.stop('spnRFQList');
    };

 
    $scope.fnCallBack = function () {
        $scope.getAvailableSuppliers();
    };

    $scope.Init();
}]);

////***STEP - 2: RFQ PARTS***////
var ModalRFQPartCtrl = function ($scope, $modalInstance, common, $filter, RFQSvc) {
    $scope.Init = function () {
        if (!Isundefinedornull($scope.RFQ.lstRFQPart)) {
            $scope.lstRFQPartOriginal = angular.copy($scope.RFQ.lstRFQPart);
        }
        else {
            $scope.lstRFQPartOriginal = [];
        }
    };

    $scope.AddRFQPart = function () {
        var alreadyExists = false;
        if (!Isundefinedornull($scope.RFQPartIndex)) {
            $scope.lstRFQPartOriginal.splice($scope.RFQPartIndex, 1);
        }

        if (Isundefinedornull($scope.RFQ.lstRFQPart))
            $scope.RFQ.lstRFQPart = [];

        if (!IsUndefinedNullOrEmpty($scope.RFQ.lstRFQPart) && $scope.RFQ.lstRFQPart.length > 0) {
            angular.forEach($scope.lstRFQPartOriginal, function (o) {

                if (o.CustomerPartNo == $scope.RFQPart.CustomerPartNo) {
                    alreadyExists = true;
                }
            });
        }

        if (!alreadyExists) {

            $scope.RFQ.lstRFQPart.push($scope.RFQPart);

            $scope.RFQPart.RFQId = $scope.RFQ.Id;

            if (!IsUndefinedNullOrEmpty($scope.RFQ.Id))
                $scope.SavePart($scope.RFQPart);

            $modalInstance.dismiss('cancel');
            $scope.ResetRFQParts();

            $scope.destroyScope();
        }
        else {
            common.aaNotify.error(($filter('translate')('_PartNoExists_')));
        }


    };
    $scope.SavePart = function () {

        RFQSvc.SaveRfqPart($scope.RFQPart).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   common.usSpinnerService.stop('spnAddEditRFQ');

               }
               else {
                   common.usSpinnerService.stop('spnAddEditRFQ');
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnAddEditRFQ');
               console.log(error);
           });
    }
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
        $scope.RFQ.lstRFQPart = angular.copy($scope.lstRFQPartOriginal);
        $scope.ResetRFQParts();
        $scope.destroyScope();
    }
    $scope.destroyScope = function () {
        var lclScope = $scope;
        lclScope.$destroy();
    };
    $scope.Init();
}

////***STEP - 3: SENT TO RFQ SUPPLIERS GRID - SEND EMAIL  ***////
var ModalSendEmailCtrl = function ($scope, $modalInstance, common, RFQData, RFQSvc) {
    $scope.Init = function () {
        var projectname = '';
        if (!IsUndefinedNullOrEmpty(RFQData.ProjectName))
            projectname = " - " + RFQData.ProjectName;

        $scope.EmailData.EmailSubject = RFQData.Id + " - " + RFQData.CustomerName + projectname;
    }

    $scope.SendEmail = function () {
        common.usSpinnerService.spin('spnAddEditRFQ');
        RFQSvc.SendEmail($scope.EmailData).then(
           function (response) {
               if (ShowMessage(common, response.data)) {
                   common.usSpinnerService.stop('spnAddEditRFQ');
                   $scope.Cancel();
               }
               else {
                   common.usSpinnerService.stop('spnAddEditRFQ');
               }
           },
           function (error) {
               common.usSpinnerService.stop('spnAddEditRFQ');
               //common.aaNotify.error(error);
           });
    };

    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    }
    $scope.Init();
}
